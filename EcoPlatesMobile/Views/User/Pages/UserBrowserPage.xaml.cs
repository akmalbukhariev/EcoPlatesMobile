using System.ComponentModel;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;
using Microsoft.Maui.Maps;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Helper;
using EcoPlatesMobile.Resources.Languages;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Application = Microsoft.Maui.Controls.Application;
using The49.Maui.BottomSheet;

#if ANDROID
using EcoPlatesMobile.Platforms.Android;
#endif

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserBrowserPage : BasePage
{
    private bool created = false;
    private List<CustomPin> customPins = new();

    private UserBrowserPageViewModel viewModel;
    private UserApiService userApiService;
    private AppControl appControl;
    private LocationService locationService;
    private bool mapIsVisible = false;
    public UserBrowserPage(UserBrowserPageViewModel vm, UserApiService userApiService, AppControl appControl, LocationService locationService)
    {
        InitializeComponent();

        this.viewModel = vm;
        this.userApiService = userApiService;
        this.appControl = appControl;
        this.locationService = locationService;

        tabSwitcher.TabChanged += TabSwitcher_TabChanged;
        viewModel.PropertyChanged += ViewModel_PropertyChanged;

        loading.ChangeColor(Constants.COLOR_USER);
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        WireMapReady();

        Shell.SetTabBarIsVisible(this, true);

        bool isWifiOn = await appControl.CheckWifi();
        if (!isWifiOn) return;

        var location = await locationService.GetCurrentLocationAsync();
        if (location == null) return;

        if (appControl.RefreshBrowserPage)
        {
            /*await Task.WhenAll
            (
                viewModel.LoadInitialAsync(),
                GetAllCompaniesUsingMap()
            );*/

            await viewModel.LoadInitialAsync();
            await GetAllCompaniesUsingMap();

            appControl.RefreshBrowserPage = false;
        }

        lbSelectedDistance.Text = $"{AppResource.SelectedDistanceIs}: {appControl.UserInfo.radius_km} {AppResource.Km}";
    }

    private CancellationTokenSource refreshCts;
    private async Task GetAllCompaniesUsingMap()
    {
        try
        {
            viewModel.IsLoading = true;

            await EnsureMapReadyAsync();
            await MoveMap();

            if (appControl.IsLoggedIn)
            {
                map.IsZoomEnabled = true;
                map.IsScrollEnabled = true;
            }
            else
            {
                map.IsZoomEnabled = false;
                map.IsScrollEnabled = false;
                viewModel.IsLoading = false;
                return;
            }

            refreshCts?.Cancel();
            refreshCts = new CancellationTokenSource();
            var ct = refreshCts.Token;

            var userInfo = appControl.UserInfo;

            CompanyLocationRequest request = new CompanyLocationRequest()
            {
                radius_km = userInfo.radius_km,
                user_lat = userInfo.location_latitude,
                user_lon = userInfo.location_longitude,
                business_type = BusinessType.OTHER.GetValue()
            };

            CompanyListResponse response = await userApiService.GetCompaniesByCurrentLocationWithoutLimit(request).ConfigureAwait(false);

            if (ct.IsCancellationRequested) return;

            if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
            {
                var pins = response.resultData?
                    .Select(item => new CustomPin
                    {
                        CompanyId = (long)item.company_id,
                        Label = item.company_name,
                        Location = new Location(
                            (double)item.location_latitude,
                            (double)item.location_longitude),
                        LogoUrl = item.logo_url
                    })
                    .ToList();

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await _renderLock.WaitAsync(ct);
                    try
                    {
                        await ApplyPinsDiffAsync(pins, ct);
                    }
                    finally
                    {
                        _renderLock.Release();
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            viewModel.IsLoading = false;
        }
    }

    private async Task ApplyPinsDiffAsync(List<CustomPin> newPins, CancellationToken ct)
    {
        // Simple approach: replace only if something actually changed
        // (You can build a hash from CompanyId + Lat/Lon to compare)
        var changed = NeedRefresh(customPins, newPins);
        if (!changed) return;

        customPins.Clear();
        customPins.AddRange(newPins);

        map.Pins.Clear();
        map.MapElements.Clear();

    #if ANDROID
        if (map?.Handler?.PlatformView is Android.Gms.Maps.MapView nativeMapView)
        {
            var renderer = new PinIconRenderer(customPins);
            renderer.EventPinClick += PinCompanyClicked;

            var tcs = new TaskCompletionSource();
            renderer.RenderingFinished.ContinueWith(_ => tcs.TrySetResult(), ct);

            nativeMapView.GetMapAsync(renderer);
            await tcs.Task; // wait render finish w/o blocking UI input forever
        }
    #else
        foreach (var p in _customPins)
        {
            map.Pins.Add(new Pin { Label = p.Label, Location = p.Location, Type = PinType.Place });
        }
    #endif
    }

    private bool NeedRefresh(List<CustomPin> oldPins, List<CustomPin> newPins)
    {
        if (oldPins.Count != newPins.Count) return true;
        // cheap compare by CompanyId + rounded coords
        var a = oldPins.Select(x => (x.CompanyId, lat: Math.Round(x.Location.Latitude, 6), lon: Math.Round(x.Location.Longitude, 6))).OrderBy(t => t.CompanyId);
        var b = newPins.Select(x => (x.CompanyId, lat: Math.Round(x.Location.Latitude, 6), lon: Math.Round(x.Location.Longitude, 6))).OrderBy(t => t.CompanyId);
        return !a.SequenceEqual(b);
    }


    private async Task MoveMap()
    {
        await EnsureMapReadyAsync();

        var position = new Location(appControl.UserInfo.location_latitude, appControl.UserInfo.location_longitude);
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(3000))); // zoom level
        });

        /*
        map.Pins.Add(new Pin
        {
            Label = AppResource.YouAreHere,
            Location = position,
            Type = PinType.Generic
        });
        */
    }

    private async Task RefreshCustomPins()
    {
        map.Pins.Clear();
        map.MapElements.Clear();

#if ANDROID
        if (map?.Handler?.PlatformView is Android.Gms.Maps.MapView nativeMapView)
        {
            PinIconRenderer render = new PinIconRenderer(customPins);
            render.EventPinClick += PinCompanyClicked;
            nativeMapView.GetMapAsync(render);

            await render.RenderingFinished;
        }
#endif
    }

    private void PinCompanyClicked(CustomPin pin)
    {
        Application.Current.Dispatcher.Dispatch(async () =>
        {
            await AppNavigatorService.NavigateTo($"{nameof(UserCompanyPage)}?CompanyId={pin.CompanyId}");
        });
    }

    private async void TabSwitcher_TabChanged(object? sender, string e)
    {
        const int animationDuration = 400;
        double screenWidth = this.Width;

        if (screenWidth <= 0) screenWidth = 400;

        if (e == tabSwitcher.Tab1_Title)
        {
            list.IsVisible = true;
            map.IsVisible = true;
            borderBlock.IsVisible = false;

            await Task.WhenAll(
                list.TranslateTo(0, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(screenWidth, 0, animationDuration, Easing.CubicInOut),
                borderBottom.TranslateTo(0, 100, 250, Easing.CubicIn)
            );

            mapIsVisible = false;
            borderBottom.IsVisible = false;
            borderBackground.IsVisible = false;

            boxTemp.IsVisible = false;
            //Grid.SetColumnSpan(borderSearch, 2);
        }
        else if (e == tabSwitcher.Tab2_Title && !mapIsVisible)
        {
            list.IsVisible = true;
            map.IsVisible = true;

            if (appControl.IsLoggedIn)
            {
                borderBottom.TranslationY = 100;
                borderBottom.IsVisible = true;
            }
            else
            {
                borderBlock.IsVisible = true;
            }

            if (map.TranslationX != screenWidth)
            {
                map.TranslationX = screenWidth;
            }

            await Task.WhenAll(
                list.TranslateTo(-screenWidth, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(0, 0, animationDuration, Easing.CubicInOut)

            );

            if (appControl.IsLoggedIn)
            {
                await borderBottom.TranslateTo(0, 0, 250, Easing.CubicOut);
            }
            else
            {
                borderBackground.IsVisible = true;
            }
            mapIsVisible = true;

            boxTemp.IsVisible = true;
            //Grid.SetColumnSpan(borderSearch, 1);
        }
    }

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShowLikedView) && viewModel.ShowLikedView)
        {
            await likeView.DisplayAsAnimation();
            viewModel.ShowLikedView = false;
        }
    }



    // fields
    private readonly SemaphoreSlim _renderLock = new(1,1);
    private TaskCompletionSource<bool> _mapReadyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    // call this once (e.g., in OnAppearing or after InitializeComponent)
    private void WireMapReady()
    {
    #if ANDROID
        map.HandlerChanged += (_, __) =>
        {
            if (map?.Handler?.PlatformView is Android.Gms.Maps.MapView native)
            {
                native.GetMapAsync(new MapReadyCallback(g =>
                {
                    _mapReadyTcs.TrySetResult(true);
                }));
            }
        };
    #else
        // iOS: map is usually ready when Handler is set; still complete TCS.
        _mapReadyTcs.TrySetResult(true);
    #endif
    }

    private Task EnsureMapReadyAsync() => _mapReadyTcs.Task;




    private async void Bottom_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(borderBottom);

        /*viewModel.IsLoading = true;
        var location = await locationService.GetCurrentLocationAsync();
        viewModel.IsLoading = false;

        if (location == null) return;*/

        await AppNavigatorService.NavigateTo(nameof(LocationSettingPage));
    }

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(borderSearch);

        await AppNavigatorService.NavigateTo(nameof(UserBrowserSearchPage));
    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
    }
}