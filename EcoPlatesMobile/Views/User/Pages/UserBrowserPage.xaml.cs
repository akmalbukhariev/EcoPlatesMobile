using System.ComponentModel;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;
using Microsoft.Maui.Maps;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Helper;

#if ANDROID
using EcoPlatesMobile.Platforms.Android;
#endif

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserBrowserPage : BasePage
{
    private bool created = false;
    private bool openedCompanyPage = false;
    private UserBrowserPageViewModel viewModel;
    private List<CustomPin> _customPins = new();

    public UserBrowserPage()
    {
        InitializeComponent();
        viewModel = new UserBrowserPageViewModel();

        BindingContext = viewModel;

        tabSwitcher.TabChanged += TabSwitcher_TabChanged;
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
        btnLocation.EventClick += Click_Location;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!openedCompanyPage)
        {
            if (created)
            {
                tabSwitcher.Init();
            }

            created = true;

            Shell.SetTabBarIsVisible(this, true);

            AppService.Get<AppControl>().ShowCompanyMoreInfo = false;

            await viewModel.LoadInitialAsync();
            await CenterMapToCurrentLocation();
            await GetAllCompaniesUsingMap();
        }

        openedCompanyPage = false;
    }

    private async Task GetAllCompaniesUsingMap()
    {
        try
        {
            viewModel.IsLoading = true;
            var apiService = AppService.Get<UserApiService>();

            var userInfo = AppService.Get<AppControl>().UserInfo;
            CompanyLocationRequest request = new CompanyLocationRequest()
            {
                radius_km = 2,
                user_lat = userInfo.location_latitude,
                user_lon = userInfo.location_longitude,
                business_type = BusinessType.OTHER.GetValue()
            };

            CompanyListResponse response = await apiService.GetCompaniesByCurrentLocationWithoutLimit(request);

            if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
            {
                foreach (var item in response.resultData)
                {
                    _customPins.Add(new CustomPin
                    {
                        CompanyId = (long)item.company_id,
                        Label = item.company_name,
                        Location = new Location(
                            (double)item.location_latitude,
                            (double)item.location_longitude),
                        LogoUrl = item.logo_url
                    });
                }

                RefreshCustomPins();
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
            var location = await AppService.Get<LocationService>().GetCurrentLocationAsync();

            if (location != null)
            {
                var position = new Location(location.Latitude, location.Longitude);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(300))); // zoom level
            }
            else
            {
                await DisplayAlert("Location Error", "Could not get current location.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to get location: {ex.Message}", "OK");
        }
    }

    private void PinCompanyClicked(CustomPin pin)
    {
        Application.Current.Dispatcher.Dispatch(async () =>
        {
            openedCompanyPage = true;
            await AppNavigatorService.NavigateTo($"{nameof(UserCompanyPage)}?CompanyId={pin.CompanyId}");
        });
    }

    private async void Click_Location()
    {
        //await CenterMapToCurrentLocation();
        await AppNavigatorService.NavigateTo(nameof(LocationSettingPage));
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

            await Task.WhenAll(
                list.TranslateTo(0, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(screenWidth, 0, animationDuration, Easing.CubicInOut)
            );
        }
        else if (e == tabSwitcher.Tab2_Title)
        {
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
}