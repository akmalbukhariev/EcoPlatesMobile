using System.Globalization;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using Microsoft.Maui.Maps;
 

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class LocationRegistrationPage : BasePage
{
    private AppControl appControl;
    private LocationService locationService;
     
    public LocationRegistrationPage(AppControl appControl, LocationService locationService)
    {
        InitializeComponent();

        this.appControl = appControl;
        this.locationService = locationService;

        loading.ChangeColor(Constants.COLOR_COMPANY);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await MoveToCurrentLocation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    
        cts?.Cancel();
        cts = null;
    } 	
    
    private async Task MoveToCurrentLocation()
    {
        cts = new CancellationTokenSource();
        var location = await locationService.GetCurrentLocationAsync(cts.Token);
        if (location != null)
        {
            var center = new Location(location.Latitude, location.Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(5)));
        }
    }

    private async void Save_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(sender as Image);

            bool isWifiOn = await appControl.CheckWifi();
            if (!isWifiOn) return;

            try
            {
                var visibleRegion = map.VisibleRegion;
                if (visibleRegion == null)
                {
                    return;
                }

                bool yes = await AlertService.ShowConfirmationAsync(AppResource.Confirm, AppResource.ConfirmCompanyLocation, AppResource.Yes, AppResource.No);
                if (!yes) return;

                loading.ShowLoading = true;
                var center = new Location(
                    visibleRegion.Center.Latitude,
                    visibleRegion.Center.Longitude
                );

                await Task.Delay(1000);
                //appControl.ClippedMap = await CaptureAndCropCenterOfMap();
                appControl.LocationForRegister = center;
                loading.ShowLoading = false;

                await AppNavigatorService.NavigateTo("..");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                loading.ShowLoading = false;
            }
        });
    }   
    
    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            appControl.LocationForRegister = null;
            await AnimateElementScaleDown(imBack);
            await AppNavigatorService.NavigateTo("..");
        });
    }
}