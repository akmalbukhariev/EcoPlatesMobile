using System.Globalization;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class LocationPage : BasePage
{
	private AppControl appControl;
	private CompanyApiService companyApiService;
	private LocationService locationService;

    public LocationPage(AppControl appControl, CompanyApiService companyApiService, LocationService locationService)
	{
		InitializeComponent();

		this.appControl = appControl;
		this.companyApiService = companyApiService;
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

		map.Pins.Clear();
 
		var pin = new Pin
		{
			Label = AppResource.YourWorkplace,
			Location = new Location((double)appControl.CompanyInfo.location_latitude, (double)appControl.CompanyInfo.location_longitude),
			Type = PinType.Place,
		};
  
		map.Pins.Add(pin);
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

            var center = new Location(
				visibleRegion.Center.Latitude,
				visibleRegion.Center.Longitude
			);

			var additionalData = new Dictionary<string, string>
			{
				{ "company_id", appControl.CompanyInfo.company_id.ToString() },
				{ "location_latitude", center.Latitude.ToString("F6", CultureInfo.InvariantCulture) },
				{ "location_longitude", center.Longitude.ToString("F6", CultureInfo.InvariantCulture) }
			};

			loading.ShowLoading = true;
			Response response = await companyApiService.UpdateCompanyProfileInfo(null, additionalData);

			if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
			{
				appControl.CompanyInfo.location_latitude = center.Latitude;
				appControl.CompanyInfo.location_longitude = center.Longitude;
				
				await AlertService.ShowAlertAsync(AppResource.MessageUpdateLocation, AppResource.Success);
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

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
		await AnimateElementScaleDown(imBack);
        await AppNavigatorService.NavigateTo("..");
    }
}