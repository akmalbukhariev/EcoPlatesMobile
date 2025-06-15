using System.Threading.Tasks;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Services;
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
        var location = await Geolocation.GetLocationAsync();
        if (location == null) return;

        currentCenter = new Location(location.Latitude, location.Longitude);
        map.MoveToRegion(MapSpan.FromCenterAndRadius(currentCenter, Distance.FromKilometers(2)));

        distanceCircle = new Circle
        {
            Center = currentCenter,
            Radius = new Distance(distanceSlider.Value * 1000), // km → meters
            StrokeColor = Color.FromArgb("#4481c2ff"), // semi-transparent stroke
            FillColor = Color.FromArgb("#221E90FF"),   // semi-transparent fill
            StrokeWidth = 2
        };

        map.MapElements.Add(distanceCircle);
    }
    
    private async void Save_Clicked(object sender, EventArgs e)
    {
        /*
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
                { "company_id", AppService.Get<AppControl>().CompanyInfo.company_id.ToString() },
                { "location_latitude", center.Latitude.ToString("F6") },
                { "location_longitude", center.Longitude.ToString("F6") }
            };

            loading.ShowLoading = true;
            var apiService = AppService.Get<CompanyApiService>();
            Response response = await apiService.UpdateCompanyProfileInfo(null, additionalData);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
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
        */
    }

    private void Close_Tapped(object sender, TappedEventArgs e)
    {
    }

    private async void UseCurrentLocation_Clicked(object sender, EventArgs e)
    {
        var location = await Geolocation.GetLocationAsync();
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

    private void ShowResults_Clicked(object sender, EventArgs e)
    {
        double selectedDistance = distanceSlider.Value;
        // TODO: Use `selectedDistance` and current map location to filter data
    }
}