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
using EcoPlatesMobile.Models.Responses;
using System.Globalization;


#if ANDROID
using EcoPlatesMobile.Platforms.Android;
#endif

#if IOS
using EcoPlatesMobile.Platforms.iOS;
using MapKit;
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
    //private MapBottomSheet bottomSheet;
    private CancellationTokenSource refreshCts;
    private Circle distanceCircle;
    private Location currentCenter;
    private bool mapIsVisible = false;
    private bool isBottomSheetOpened = false;
    int selectedDistance = 1;
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

        //bottomSheet = new MapBottomSheet();
        bottomSheet.Dismissed += BottomSheet_Closed;
        bottomSheet.EventValueDistanceChanged += DistanceSliderValueChanged;
        bottomSheet.EventShowResultsClicked += ShowResultsClicked;
        map.PropertyChanged += Map_PropertyChanged;

        loading.ChangeColor(Constants.COLOR_USER);
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (appControl.IsLoggedIn)
        {
            map.IsShowingUser = true;
        }

        WireMapReady();

        Shell.SetTabBarIsVisible(this, true);

        cts = new CancellationTokenSource();
        var location = await locationService.GetCurrentLocationAsync(cts.Token);
        if (location == null) return;

        if (viewModel.IsRefreshing)
            viewModel.IsRefreshing = false;

        if (appControl.RefreshBrowserPage)
        {
            await RefreshPage();
            appControl.RefreshBrowserPage = false;
        }
        else
        {
            viewModel.IsLoading = false;
        }

        lbSelectedDistance.Text = $"{AppResource.SelectedDistanceIs}: {appControl.UserInfo.radius_km} {AppResource.Km}";

    }

    private async Task RefreshPage()
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
                viewModel.IsLoading = false;
            }
        });

        bool isWifiOn = await appControl.CheckWifiOrNetwork();
        if (!isWifiOn)
        {
            viewModel.IsLoading = false;
            return;
        }

        await viewModel.LoadInitialAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        cts?.Cancel();
        cts = null;
    }

    #region BottomView
    private void InitCircle()
    {
        loading.ShowLoading = true;

        currentCenter = new Location(appControl.UserInfo.location_latitude, appControl.UserInfo.location_longitude);
        selectedDistance = appControl.UserInfo.radius_km;

        UpdateSelectedDistanceLabel();
        bottomSheet.SetValue(selectedDistance);

        int maxVal = appControl.UserInfo.max_radius_km == 0 ? Constants.MaxRadius : appControl.UserInfo.max_radius_km;
        bottomSheet.SetMaxValue(maxVal);

        distanceCircle = new Circle
        {
            Center = currentCenter,
            Radius = Distance.FromKilometers(selectedDistance), // km → meters
            StrokeColor = Color.FromArgb("#99000000"),
            FillColor = Color.FromArgb("#55000000"),
            StrokeWidth = 1
        };

        map.MapElements.Add(distanceCircle);

        loading.ShowLoading = false;
    }

    private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(map.VisibleRegion)) return;

        var center = map.VisibleRegion?.Center;
        if (center == null) return;

        currentCenter = center;

        if (distanceCircle != null)
            distanceCircle.Center = center;

        /*
        if (map.Pins.Count > 0)
        {
            map.Pins.Clear();
            map.Pins.Add(new Pin
            {
                Label = AppResource.YouAreHere,
                Location = currentCenter,
                Type = PinType.Generic
            });
        }
        */
    }

    private void DistanceSliderValueChanged(int km)
    {
        selectedDistance = km;
        UpdateSelectedDistanceLabel();

        if (distanceCircle != null)
            distanceCircle.Radius = Distance.FromKilometers(km);

        map.MoveToRegion(MapSpan.FromCenterAndRadius(currentCenter, Distance.FromKilometers(selectedDistance)));
    }

    private void UpdateSelectedDistanceLabel()
    {
        lbSelectedDistance.Text = $"{AppResource.SelectedDistanceIs}: {selectedDistance} {AppResource.Km}";
    }

    private async void ShowResultsClicked()
    {
        await ShowResultsAsync();
    }

    private async Task ShowResultsAsync()
    {
        /*
        if (selectedDistance == appControl.UserInfo.radius_km)
        {
            await AppNavigatorService.NavigateTo("..");
            return;
        }
        */

        await bottomSheet.DismissAsync();

        bool isWifiOn = await appControl.CheckWifiOrNetwork();
        if (!isWifiOn) return;

        try
        {
            var visibleRegion = map.VisibleRegion;
            if (visibleRegion == null)
            {
                return;
            }

            var additionalData = new Dictionary<string, string>
            {
                { "user_id", appControl.UserInfo.user_id.ToString() },
                { "location_latitude", currentCenter.Latitude.ToString("F6", CultureInfo.InvariantCulture) },
                { "location_longitude", currentCenter.Longitude.ToString("F6", CultureInfo.InvariantCulture) },
                { "radius_km", ((int)selectedDistance).ToString() }
            };

            loading.ShowLoading = true;
            Response response = await userApiService.UpdateUserProfileInfo(null, additionalData);
            bool isOk = await appControl.CheckUserState(response);
            if (!isOk)
            {
                await appControl.LogoutUser();
                return;
            }

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                appControl.UserInfo.location_latitude = currentCenter.Latitude;
                appControl.UserInfo.location_longitude = currentCenter.Longitude;
                appControl.UserInfo.radius_km = selectedDistance;

                appControl.RefreshMainPage = true;
                appControl.RefreshBrowserPage = true;
                appControl.RefreshFavoriteProduct = true;
                appControl.RefreshFavoriteCompany = true;

                await AppNavigatorService.NavigateTo("..", true);
            }
            else
            {
                await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            loading.ShowLoading = false;
            RemoveCircle();
            await RefreshPage();
        }
    }

    private async void Bottom_Tapped(object sender, TappedEventArgs e)
    {
        //await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        //{
        await AnimateElementScaleDown(borderBottom);
        borderBottom.IsVisible = false;
        isBottomSheetOpened = true;

        await bottomSheet.ShowAsync();
        InitCircle();
        //});
    }

    private async void BottomSheet_Closed()
    {
        borderBottom.TranslationY = 100;
        borderBottom.IsVisible = true;
        isBottomSheetOpened = false;

        await borderBottom.TranslateTo(0, 0, 250, Easing.CubicOut);

        RemoveCircle();
    }

    private void RemoveCircle()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (distanceCircle != null && map.MapElements.Contains(distanceCircle))
            {
                map.MapElements.Remove(distanceCircle);
                distanceCircle = null;
            }
        });
    }
    #endregion

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
            bool isOk = await appControl.CheckUserState(response);
            if (!isOk)
            {
                await appControl.LogoutUser();
                return;
            }

            if (ct.IsCancellationRequested) return;

            if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
            {
                var pins = response.resultData?
                    .Where(i => !i.deleted && i.status != UserOrCompanyStatus.BANNED)
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
    
#if IOS
    private PinIconRenderer? _iosRenderer;
#endif

    private async Task ApplyPinsDiffAsync(List<CustomPin> newPins, CancellationToken ct)
    {
        customPins.Clear();
        map.Pins.Clear();
        map.MapElements?.Clear();

        if (newPins == null || newPins.Count == 0)
        {
#if ANDROID || IOS
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
#elif IOS
        if (map?.Handler?.PlatformView is MKMapView nativeMapView)
        {
            // create or reuse renderer (no delegate!)
            _iosRenderer ??= new PinIconRenderer(customPins);

            _iosRenderer.EventPinClick -= PinCompanyClicked;
            _iosRenderer.EventPinClick += PinCompanyClicked;

            var tcs = new TaskCompletionSource();
            _iosRenderer.RenderingFinished.ContinueWith(_ => tcs.TrySetResult(), ct);

            _iosRenderer.Attach(nativeMapView);

            await tcs.Task;
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
#elif IOS
    private Task ClearNativeMapAsync(CancellationToken ct)
    {
        if (map?.Handler?.PlatformView is MapKit.MKMapView native)
        {
            // ensure we touch UIKit on main thread
            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (ct.IsCancellationRequested) return;

                var ann = native.Annotations;
                if (ann != null && ann.Length > 0)
                {
                    native.RemoveAnnotations(ann); // removes all CustomPinAnnotation, etc.
                }
            });
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

                if (isBottomSheetOpened)
                {
                    await bottomSheet.DismissAsync();
                }

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

    private readonly SemaphoreSlim _renderLock = new(1, 1);
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

    private void Map_Tapped(object sender, TappedEventArgs e)
    {

    }
}