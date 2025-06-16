using System.Threading.Tasks;
using EcoPlatesMobile.Models.Responses;
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

    public LocationSettingPage()
    {
        InitializeComponent();

        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await InitCircle();
    }

    private async Task InitCircle()
    {
        loading.ShowLoading = true;

        var location = await AppService.Get<LocationService>().GetCurrentLocationAsync();
        if (location == null) return;

        currentCenter = new Location(location.Latitude, location.Longitude);
        map.MoveToRegion(MapSpan.FromCenterAndRadius(currentCenter, Distance.FromKilometers(2)));

        distanceSlider.Value = AppService.Get<AppControl>().UserInfo.radius_km;
        
        distanceCircle = new Circle
        {
            Center = currentCenter,
            Radius = new Distance(distanceSlider.Value * 1000), // km → meters
            //StrokeColor = Color.FromArgb("#4481c2ff"), // semi-transparent stroke
            //FillColor = Color.FromArgb("#221E90FF"),   // semi-transparent fill
            StrokeColor = Color.FromArgb("#99000000"),
            FillColor = Color.FromArgb("#55000000"),
            StrokeWidth = 1
        };

        map.MapElements.Add(distanceCircle);

        loading.ShowLoading = false;
    }
     
    private async void Close_Tapped(object sender, TappedEventArgs e)
    {
        await AppNavigatorService.NavigateTo("..");
    }

    private async void UseCurrentLocation_Clicked(object sender, EventArgs e)
    {
        var location = await AppService.Get<LocationService>().GetCurrentLocationAsync();
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
        double selectedDistance = distanceSlider.Value;
        
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
                { "user_id", AppService.Get<AppControl>().UserInfo.user_id.ToString() },
                { "location_latitude", center.Latitude.ToString("F6") },
                { "location_longitude", center.Longitude.ToString("F6") },
                { "radius_km", ((int)selectedDistance).ToString() }
            };

            loading.ShowLoading = true;
            var apiService = AppService.Get<UserApiService>();
            Response response = await apiService.UpdateUserProfileInfo(null, additionalData);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                AppService.Get<AppControl>().UserInfo.location_latitude = center.Latitude;
                AppService.Get<AppControl>().UserInfo.location_longitude = center.Longitude;
                AppService.Get<AppControl>().UserInfo.radius_km = (int)selectedDistance;

                await AlertService.ShowAlertAsync("Update location", "Success.");
                await Shell.Current.GoToAsync("..", true);
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
}