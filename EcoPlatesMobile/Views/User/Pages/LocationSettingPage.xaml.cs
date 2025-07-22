using System.ComponentModel;
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

    private const double OffsetRatio = 0.008;    // controls how much the circle moves up visually
    private const double SpanPadding = 1.15;
    private bool _skipCenterSync;

    public LocationSettingPage(LocationService locationService, AppControl appControl, UserApiService userApiService)
    {
        InitializeComponent();

        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);

        this.locationService = locationService;
        this.appControl = appControl;
        this.userApiService = userApiService;
        map.PropertyChanged += Map_PropertyChanged;
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

        distanceSlider.Value = appControl.UserInfo.radius_km;
        double radiusKm = distanceSlider.Value;

        UpdateMapView(radiusKm);
  
        distanceCircle = new Circle
        {
            Center = currentCenter,
            Radius = Distance.FromKilometers(radiusKm), // km → meters
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

    private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(map.VisibleRegion)) return;

        if (_skipCenterSync)
        {
            _skipCenterSync = false;   // reset for next user pan
            return;                    // ⬅️  ignore this programmatic zoom
        }

        // ----- user actually panned the map -----
        var center = map.VisibleRegion?.Center;
        if (center == null) return;

        currentCenter = center;

        if (distanceCircle != null)
            distanceCircle.Center = center;

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
        /*
        int km = (int)Math.Round(e.NewValue);
        distanceLabel.Text = $"{km} km";

        if (distanceCircle != null)
        {
            distanceCircle.Radius = new Distance(km * 1000);
        }

        map.MoveToRegion(MapSpan.FromCenterAndRadius(currentCenter, Distance.FromKilometers(km)));
        */

        int km = (int)Math.Round(e.NewValue);
        distanceLabel.Text = $"{km} km";

        if (distanceCircle != null)
            distanceCircle.Radius = Distance.FromKilometers(km);

        UpdateZoomOnly(km); 
    }

    void UpdateMapView(double km)
    {
        // push the circle a bit up the screen
        double verticalOffset = OffsetRatio * km;
        var adjustedCenter = new Location(
            currentCenter.Latitude - verticalOffset,
            currentCenter.Longitude);

        // use a slightly larger span so the circle never touches the edges
        map.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                adjustedCenter,
                Distance.FromKilometers(km * SpanPadding)));
    }
    
    void UpdateZoomOnly(double km)
    {
        _skipCenterSync = true;

        // 🛠️ Use exact currentCenter without applying offset
        map.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                currentCenter,
                Distance.FromKilometers(km * SpanPadding)));
    }
    
    private async void ShowResults_Clicked(object sender, EventArgs e)
    {
        int selectedDistance = (int)Math.Round(distanceSlider.Value);

        /*
        if (selectedDistance == appControl.UserInfo.radius_km)
        {
            await AppNavigatorService.NavigateTo("..");
            return;
        }
        */

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
        }
    }

    private async void Close_Tapped(object sender, TappedEventArgs e)
    {
        await AppNavigatorService.NavigateTo("..");
    }
}