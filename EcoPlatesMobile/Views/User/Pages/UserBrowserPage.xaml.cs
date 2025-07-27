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

        loading.ChangeColor(Constants.COLOR_USER);
        BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        /*
        if (created)
        {
            tabSwitcher.Init();
        }
        
        created = true;
        */

        Shell.SetTabBarIsVisible(this, true);

        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;
        
        bool enable = await locationService.IsLocationEnabledAsync();
        if (!enable) return;

        if (appControl.RefreshBrowserPage)
        {
            await viewModel.LoadInitialAsync();
            await GetAllCompaniesUsingMap();

            appControl.RefreshBrowserPage = false;
        }

        lbSelectedDistance.Text = $"{AppResource.SelectedDistanceIs}: {appControl.UserInfo.radius_km} {AppResource.Km}";
    }

    private async Task GetAllCompaniesUsingMap()
    {
        try
        {
            viewModel.IsLoading = true; 
            //loading.ShowLoading = true;

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

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    _customPins.Clear();
                    _customPins.AddRange(pins);
                    await RefreshCustomPins();
                });
            }

            MoveMap();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            viewModel.IsLoading = false;
            //loading.ShowLoading = false;
        }
    }

    private void MoveMap()
    {
        var position = new Location(appControl.UserInfo.location_latitude, appControl.UserInfo.location_longitude);
        map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(3000))); // zoom level

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
            PinIconRenderer render = new PinIconRenderer(_customPins);
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

            await Task.WhenAll(
                list.TranslateTo(0, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(screenWidth, 0, animationDuration, Easing.CubicInOut),
                borderBottom.TranslateTo(0, 100, 250, Easing.CubicIn)
            );

            mapIsVisible = false;
            borderBottom.IsVisible = false;

            Grid.SetColumnSpan(entrySearch, 2);
        }
        else if (e == tabSwitcher.Tab2_Title && !mapIsVisible)
        {
            list.IsVisible = true;
            map.IsVisible = true;

            borderBottom.TranslationY = 100;
            borderBottom.IsVisible = true;

            if (map.TranslationX != screenWidth)
            {
                map.TranslationX = screenWidth;
            }

            await Task.WhenAll(
                list.TranslateTo(-screenWidth, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(0, 0, animationDuration, Easing.CubicInOut)
                
            );

            await borderBottom.TranslateTo(0, 0, 250, Easing.CubicOut);
            mapIsVisible = true;
            
            Grid.SetColumnSpan(entrySearch, 1);
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

    private async void Bottom_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(borderBottom);

        bool enable = await locationService.IsLocationEnabledAsync();
        if (!enable) return;

        await AppNavigatorService.NavigateTo(nameof(LocationSettingPage));
    }

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await AppNavigatorService.NavigateTo(nameof(UserBrowserSearchPage));
    }
}