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


#if ANDROID
using EcoPlatesMobile.Platforms.Android;
#endif

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserBrowserPage : BasePage
{
    private bool created = false;
    private List<CustomPin> _customPins = new();

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
        btnRadiusLocation.EventClick += Click_RadiusLocation;
        btnMyLocation.EventClick += Click_MyLocation;

        BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (created)
        {
            tabSwitcher.Init();
        }

        created = true;

        Shell.SetTabBarIsVisible(this, true);

        if (appControl.RefreshBrowserPage)
        {
            await viewModel.LoadInitialAsync();
            await CenterMapToCurrentLocation();
            await GetAllCompaniesUsingMap();

            appControl.RefreshBrowserPage = false;
        }
    }

    private async Task GetAllCompaniesUsingMap()
    {
        try
        {
            viewModel.IsLoading = true; 

            var userInfo = appControl.UserInfo;
            CompanyLocationRequest request = new CompanyLocationRequest()
            {
                radius_km = userInfo.radius_km,
                user_lat = userInfo.location_latitude,
                user_lon = userInfo.location_longitude,
                business_type = BusinessType.OTHER.GetValue()
            };

            CompanyListResponse response = await userApiService.GetCompaniesByCurrentLocationWithoutLimit(request);

            if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
            {
                var pins = response.resultData.Select(item => new CustomPin
                {
                    CompanyId = (long)item.company_id,
                    Label = item.company_name,
                    Location = new Location(
                    (double)item.location_latitude,
                    (double)item.location_longitude),
                    LogoUrl = item.logo_url
                }).ToList();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _customPins.Clear();
                    _customPins.AddRange(pins);
                    RefreshCustomPins();
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

    private void RefreshCustomPins()
    {
        map.Pins.Clear();
        map.MapElements.Clear();

#if ANDROID
        if (map?.Handler?.PlatformView is Android.Gms.Maps.MapView nativeMapView)
        {
            PinIconRenderer render = new PinIconRenderer(_customPins);
            render.EventPinClick += PinCompanyClicked;
            nativeMapView.GetMapAsync(render);
        }
#endif
    }

    private async Task CenterMapToCurrentLocation()
    {
        try
        {
            var location = await locationService.GetCurrentLocationAsync();

            if (location != null)
            {
                var position = new Location(location.Latitude, location.Longitude);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(300))); // zoom level
            }
            else
            {
                await DisplayAlert($"{AppResource.Location} {AppResource.Error}", AppResource.CouldNotGetCurrentLocation, AppResource.Ok);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(AppResource.Error, $"{AppResource.FailedGetLocation} {ex.Message}", AppResource.Ok);
        }
    }

    private void PinCompanyClicked(CustomPin pin)
    {
        Application.Current.Dispatcher.Dispatch(async () =>
        {
            await AppNavigatorService.NavigateTo($"{nameof(UserCompanyPage)}?CompanyId={pin.CompanyId}");
        });
    }

    private async void Click_RadiusLocation()
    {
        //await CenterMapToCurrentLocation();
        await AppNavigatorService.NavigateTo(nameof(LocationSettingPage));
    }

    private async void Click_MyLocation()
    {
        try
        {
            loading.ShowLoading = true;

            var location = await locationService.GetCurrentLocationAsync();

            if (location != null)
            {
                var center = new Location(location.Latitude, location.Longitude);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(5)));
 
                map.Pins.Clear();
 
                var pin = new Pin
                {
                    Label = AppResource.YouAreHere,
                    Location = center,
                    Type = PinType.Place,
                };

                map.Pins.Add(pin);
            }
            else
            {
                await DisplayAlert(AppResource.Error, AppResource.MessageLocationPermission, AppResource.Ok);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to retrieve location: {ex.Message}");
        }
        finally
        {
            loading.ShowLoading = false;
        }
    }
  
    private async void TabSwitcher_TabChanged(object? sender, string e)
    {
        const int animationDuration = 400;
        double screenWidth = this.Width;

        if (screenWidth <= 0) screenWidth = 400;

        if (e == tabSwitcher.Tab1_Title)
        {
            btnMyLocation.IsVisible = false;

            list.IsVisible = true;
            map.IsVisible = true;

            await Task.WhenAll(
                list.TranslateTo(0, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(screenWidth, 0, animationDuration, Easing.CubicInOut)
            );

            mapIsVisible = false;
        }
        else if (e == tabSwitcher.Tab2_Title && !mapIsVisible)
        {
            btnMyLocation.IsVisible = true;

            list.IsVisible = true;
            map.IsVisible = true;

            if (map.TranslationX != screenWidth)
            {
                map.TranslationX = screenWidth;
            }

            await Task.WhenAll(
                list.TranslateTo(-screenWidth, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(0, 0, animationDuration, Easing.CubicInOut)
            );

            mapIsVisible = true;
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

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await AppNavigatorService.NavigateTo(nameof(UserBrowserSearchPage));
    }
}