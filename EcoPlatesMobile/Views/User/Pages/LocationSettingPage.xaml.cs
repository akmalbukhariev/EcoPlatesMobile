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
     
    int selectedDistance = 1;
    private MapBottomSheet bottomSheet;

    public LocationSettingPage(LocationService locationService, AppControl appControl, UserApiService userApiService)
    {
        InitializeComponent();

        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);

        bottomSheet = new MapBottomSheet();
        bottomSheet.Dismissed += BottomSheet_Closed;
        bottomSheet.EventValueDistanceChanged += DistanceSliderValueChanged;
        bottomSheet.EventShowResultsClicked += ShowResultsClicked;

        this.locationService = locationService;
        this.appControl = appControl;
        this.userApiService = userApiService;
        map.PropertyChanged += Map_PropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
         
        InitCircle();

        borderBottom.TranslationY = 0;
        borderBottom.IsVisible = true;
    }

    private void InitCircle()
    {
        loading.ShowLoading = true;
         
        currentCenter = new Location(appControl.UserInfo.location_latitude, appControl.UserInfo.location_longitude);
        selectedDistance = appControl.UserInfo.radius_km;

        UpdateSelectedDistanceLabel();
         
        distanceCircle = new Circle
        {
            Center = currentCenter,
            Radius = Distance.FromKilometers(selectedDistance), // km → meters
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

        MoveMap();
        loading.ShowLoading = false;
    }

    private async void BottomSheet_Closed(object? sender, The49.Maui.BottomSheet.DismissOrigin e)
    {
        borderBottom.TranslationY = 100;
        borderBottom.IsVisible = true;
        
        await borderBottom.TranslateTo(0, 0, 250, Easing.CubicOut);
    }
     
    private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(map.VisibleRegion)) return;
         
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

    private void DistanceSliderValueChanged(int km)
    {
        selectedDistance = km;
        UpdateSelectedDistanceLabel();
         
        if (distanceCircle != null)
            distanceCircle.Radius = Distance.FromKilometers(km);

        MoveMap();
    }
     
    private async void ShowResultsClicked()
    {
        /*
        if (selectedDistance == appControl.UserInfo.radius_km)
        {
            await AppNavigatorService.NavigateTo("..");
            return;
        }
        */

        await bottomSheet.DismissAsync();

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

    private void UpdateSelectedDistanceLabel()
    {
        lbSelectedDistance.Text = $"{AppResource.SelectedDistanceIs}: {selectedDistance} {AppResource.Km}";
    }

    private void MoveMap()
    {
        map.MoveToRegion(MapSpan.FromCenterAndRadius(currentCenter, Distance.FromKilometers(selectedDistance)));
    }

    private async void Bottom_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(borderBottom);
        borderBottom.IsVisible = false;

        bottomSheet.ShowAsync();
    }

    private async void Close_Tapped(object sender, TappedEventArgs e)
    {
        await AppNavigatorService.NavigateTo("..");
    }
}