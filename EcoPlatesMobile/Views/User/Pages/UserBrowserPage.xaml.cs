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
    private CancellationTokenSource? cts;
    private bool mapIsVisible = false;
    private Task? _mapWarmupTask;
    //private bool _mapBootstrapped;
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

        cts = new CancellationTokenSource();
        var location = await locationService.GetCurrentLocationAsync(cts.Token);
        if (location == null) return;

        if (viewModel.IsRefreshing)
            viewModel.IsRefreshing = false;

        if (appControl.RefreshBrowserPage)
        {
            _mapWarmupTask = Task.Run(async () =>
            {
                try
                {
                    await GetAllCompaniesUsingMap().ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
                finally
                {
                    Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
                        viewModel.IsLoading = false);
                }
            });
              
            await viewModel.LoadInitialAsync();

            appControl.RefreshBrowserPage = false;
        }
        else
        {
            viewModel.IsLoading = false;
        }

        lbSelectedDistance.Text = $"{AppResource.SelectedDistanceIs}: {appControl.UserInfo.radius_km} {AppResource.Km}";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    
        cts?.Cancel();
        cts = null;
    } 

    private CancellationTokenSource refreshCts;
    private async Task GetAllCompaniesUsingMap()
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() => viewModel.IsLoading = true);
            await Task.Yield(); 

            await EnsureMapReadyAsync();
            MoveMap();

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
                        await ApplyPinsDiffAsync(pins ?? new List<CustomPin>(), ct);
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
        
        customPins.Clear();
        map.Pins.Clear();
        map.MapElements?.Clear();

        if (newPins == null || newPins.Count == 0)
        {
        #if ANDROID
            await ClearNativeMapAsync(ct); // native GoogleMap.Clear()
        #endif
            return;
        }

        customPins.AddRange(newPins);

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
#endif
         
    }
     
    private void MoveMap()
    {
        var position = new Location(appControl.UserInfo.location_latitude, appControl.UserInfo.location_longitude);
        MainThread.InvokeOnMainThreadAsync(() =>
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(3000))); // zoom level
        });
    }

#if ANDROID
    private Task ClearNativeMapAsync(CancellationToken ct)
    {
        if (map?.Handler?.PlatformView is Android.Gms.Maps.MapView native)
        {
            var tcs = new TaskCompletionSource();
            native.GetMapAsync(new MapReadyCallback(g =>
            {
                if (ct.IsCancellationRequested) { tcs.TrySetCanceled(ct); return; }
                g.Clear(); // <- clears markers added by PinIconRenderer
                tcs.TrySetResult();
            }));
            return tcs.Task;
        }
        return Task.CompletedTask;
    }
#endif
    
    private void PinCompanyClicked(CustomPin pin)
    {
        Application.Current.Dispatcher.Dispatch(async () =>
        {
            await AppNavigatorService.NavigateTo($"{nameof(UserCompanyPage)}?CompanyId={pin.CompanyId}");
        });
    }

    private async void TabSwitcher_TabChanged(object? sender, string e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
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
        });
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
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(borderBottom);

            /*viewModel.IsLoading = true;
            var location = await locationService.GetCurrentLocationAsync();
            viewModel.IsLoading = false;

            if (location == null) return;*/

            await AppNavigatorService.NavigateTo(nameof(LocationSettingPage));
        });
    }

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(borderSearch);

            await AppNavigatorService.NavigateTo(nameof(UserBrowserSearchPage));
        });
    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
        });
    }
}