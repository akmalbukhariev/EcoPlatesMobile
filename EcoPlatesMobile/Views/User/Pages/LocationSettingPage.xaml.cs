using System.Globalization;
using System.Threading.Tasks;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class LocationSettingPage : BasePage
{
    private Circle distanceCircle;
    private Location currentCenter;

    private LocationService locationService;
    private AppControl appControl;
    private UserApiService userApiService;

    public LocationSettingPage(LocationService locationService, AppControl appControl, UserApiService userApiService)
    {
        InitializeComponent();

        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);

        this.locationService = locationService;
        this.appControl = appControl;
        this.userApiService = userApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await InitCircle();
    }

    private async Task InitCircle()
    {
        loading.ShowLoading = true;

        var location = await locationService.GetCurrentLocationAsync();
        if (location == null) return;

        currentCenter = new Location(location.Latitude, location.Longitude);
        map.MoveToRegion(MapSpan.FromCenterAndRadius(currentCenter, Distance.FromKilometers(2)));

        distanceSlider.Value = appControl.UserInfo.radius_km;
        
        distanceCircle = new Circle
        {
            Center = currentCenter,
            Radius = new Distance(distanceSlider.Value * 1000), // km → meters
            StrokeColor = Color.FromArgb("#99000000"),
            FillColor = Color.FromArgb("#55000000"),
            StrokeWidth = 1
        };

        map.MapElements.Add(distanceCircle);

        var currentLocationPin = new Pin
        {
            Label = AppResource.YouAreHere,
            Location = currentCenter,
            Type = PinType.Generic
        };
        map.Pins.Add(currentLocationPin);

        loading.ShowLoading = false;
    }
      
    private async void UseCurrentLocation_Clicked(object sender, EventArgs e)
    {
        var location = await locationService.GetCurrentLocationAsync();
        if (location != null)
        {
            var center = new Location(location.Latitude, location.Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(5)));
        }
    }
    
    private void DistanceSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int km = (int)Math.Round(e.NewValue);
        distanceLabel.Text = $"{km} km";

        if (distanceCircle != null)
        {
            distanceCircle.Radius = new Distance(km * 1000);
        }

        map.MoveToRegion(MapSpan.FromCenterAndRadius(currentCenter, Distance.FromKilometers(km)));
    }

    private async void ShowResults_Clicked(object sender, EventArgs e)
    {
        int selectedDistance = (int)Math.Round(distanceSlider.Value);

        if (selectedDistance == appControl.UserInfo.radius_km)
        {
            await AppNavigatorService.NavigateTo("..", true);
            return;
        }

        try
        {
            var visibleRegion = map.VisibleRegion;
            if (visibleRegion == null)
            {
                return;
            }

            var center = new Location(
                visibleRegion.Center.Latitude,
                visibleRegion.Center.Longitude
            );

            var additionalData = new Dictionary<string, string>
            {
                { "user_id", appControl.UserInfo.user_id.ToString() },
                { "location_latitude", center.Latitude.ToString("F6", CultureInfo.InvariantCulture) },
                { "location_longitude", center.Longitude.ToString("F6", CultureInfo.InvariantCulture) },
                { "radius_km", ((int)selectedDistance).ToString() }
            };

            loading.ShowLoading = true; 
            Response response = await userApiService.UpdateUserProfileInfo(null, additionalData);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            { 
                appControl.UserInfo.location_latitude = center.Latitude;
                appControl.UserInfo.location_longitude = center.Longitude;
                appControl.UserInfo.radius_km = selectedDistance;

                appControl.RefreshMainPage = true;
                appControl.RefreshBrowserPage = true;
                appControl.RefreshFavoriteProduct = true;
                appControl.RefreshFavoriteCompany = true;
 
                await AppNavigatorService.NavigateTo("..", true);
            }
            else
            {
                await AlertService.ShowAlertAsync("Error", response.resultMsg);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            loading.ShowLoading = false;
        }
    }

    private async void Close_Tapped(object sender, TappedEventArgs e)
    {
        await AppNavigatorService.NavigateTo("..");
    }
}