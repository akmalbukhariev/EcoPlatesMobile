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

    private async Task MoveToCurrentLocation()
    {
        var location = await locationService.GetCurrentLocationAsync();
        if (location != null)
        {
            var center = new Location(location.Latitude, location.Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(5)));
        }
    }

    private async void Save_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);

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
    }

    /*
    public async Task<ImageSource?> CaptureAndCropCenterOfMap()
    {
        var screenshot = await grdMain.CaptureAsync();
        if (screenshot == null)
            return null;

        using var stream = await screenshot.OpenReadAsync();

        // Load as Rgba32 explicitly
        using Image<Rgba32> image = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(stream);

        int cropHeight = 150;
        int x = 0;
        int y = (image.Height / 2) - (cropHeight / 2);
        int width = image.Width;

        // Safety bounds check
        if (y < 0) y = 0;
        if (y + cropHeight > image.Height) cropHeight = image.Height - y;

        // Perform crop using explicit type in lambda
        Image<Rgba32> cropped = image.Clone((SixLabors.ImageSharp.Processing.IImageProcessingContext ctx) =>
            ctx.Crop(new Rectangle(x, y, width, cropHeight)));

        var outputStream = new MemoryStream();
        await cropped.SaveAsPngAsync(outputStream);
        outputStream.Position = 0;

        return ImageSource.FromStream(() => outputStream);
    }
    */

    /*
    public async Task<ImageSource?> CaptureMapOnly()
    {
        // Capture only the Map-only grid
        var screenshot = await MapOnlyContainer.CaptureAsync();
        if (screenshot == null)
            return null;

        using var stream = await screenshot.OpenReadAsync();
        using Image<Rgba32> image = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(stream);

        int cropHeight = 150;
        int x = 0;
        int y = (image.Height / 2) - (cropHeight / 2);
        int width = image.Width;

        if (y < 0) y = 0;
        if (y + cropHeight > image.Height) cropHeight = image.Height - y;

        Image<Rgba32> cropped = image.Clone(ctx => ctx.Crop(new Rectangle(x, y, width, cropHeight)));

        var outputStream = new MemoryStream();
        await cropped.SaveAsPngAsync(outputStream);
        outputStream.Position = 0;

        var imageBytes = outputStream.ToArray();
        return ImageSource.FromStream(() => new MemoryStream(imageBytes));
    }
    */

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        appControl.LocationForRegister = null;
        await AnimateElementScaleDown(sender as Microsoft.Maui.Controls.Image);
        await AppNavigatorService.NavigateTo("..");
    }
}