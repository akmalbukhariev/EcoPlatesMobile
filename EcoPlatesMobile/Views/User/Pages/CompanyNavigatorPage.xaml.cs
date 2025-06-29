
using EcoPlatesMobile.Services;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace EcoPlatesMobile.Views.User.Pages;

[QueryProperty(nameof(Latitude), nameof(Latitude))]
[QueryProperty(nameof(Longitude), nameof(Longitude))]
public partial class CompanyNavigatorPage : BasePage
{
    private double latitude;
    public double Latitude
    {
        get => latitude;
        set
        {
            latitude = value;
        }
    }

    private double longitude;
    public double Longitude
    {
        get => longitude;
        set
        {
            longitude = value;
        }
    }

    public CompanyNavigatorPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        AppControl control = AppService.Get<AppControl>();
        double user_latitude = control.UserInfo.location_latitude;
        double user_longitude = control.UserInfo.location_longitude;
        
        /*
        // Create user pin
        var userPosition = new Location(user_latitude, user_longitude);
        var userPin = new Microsoft.Maui.Controls.Maps.Pin
        {
            Label = "You",
            Location = userPosition,
            Type = PinType.SavedPin
        };

        // Create company pin
        var companyPosition = new Location(Latitude, Longitude);
        var companyPin = new Microsoft.Maui.Controls.Maps.Pin
        {
            Label = "Company",
            Location = companyPosition,
            Type = PinType.Place
        };

        // Add pins to map
        map.Pins.Clear();
        map.Pins.Add(userPin);
        map.Pins.Add(companyPin);

        // Move map to show both points
        var distance = Location.CalculateDistance(userPosition, companyPosition, DistanceUnits.Kilometers);
        var midLatitude = (user_latitude + Latitude) / 2;
        var midLongitude = (user_longitude + Longitude) / 2;

        map.MoveToRegion(MapSpan.FromCenterAndRadius(
            new Location(midLatitude, midLongitude),
            Distance.FromKilometers(distance + 0.5) // Add margin
        ));

        DrawPath(userPosition, companyPosition);
        */
    }

    private void DrawPath(Location from, Location to)
    {
    #if ANDROID || IOS
        var polyline = new Polyline
        {
            StrokeColor = Colors.Blue,
            StrokeWidth = 5
        };

        polyline.Geopath.Add(from);
        polyline.Geopath.Add(to);

        map.MapElements.Add(polyline);
    #endif
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);
        await AppNavigatorService.NavigateTo("..");
    }
}