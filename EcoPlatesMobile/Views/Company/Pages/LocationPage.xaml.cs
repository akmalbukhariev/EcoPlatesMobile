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

	protected override async void OnDisappearing()
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
		await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
		{
			await AnimateElementScaleDown(imSave);

			bool isWifiOn = await appControl.CheckWifiOrNetwork();
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
				bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
                    return;
                }
				
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
		});
	}

	#region For iOS
	private void OnZoomInClicked(object sender, EventArgs e)
	{
		var region = map.VisibleRegion;
		if (region == null)
			return;

		// Zoom in: halve the radius, but don't go too small
		var newRadiusKm = Math.Max(region.Radius.Kilometers / 2, 0.1);

		var newSpan = MapSpan.FromCenterAndRadius(
			region.Center,
			Distance.FromKilometers(newRadiusKm));

		map.MoveToRegion(newSpan);
	}

	private void OnZoomOutClicked(object sender, EventArgs e)
	{
		var region = map.VisibleRegion;
		if (region == null)
			return;

		// Zoom out: double the radius, but clamp it to a sane max
		var newRadiusKm = Math.Min(region.Radius.Kilometers * 2, 100);

		var newSpan = MapSpan.FromCenterAndRadius(
			region.Center,
			Distance.FromKilometers(newRadiusKm));

		map.MoveToRegion(newSpan);
	}

	private async void OnMyLocationClicked(object sender, EventArgs e)
	{
		// Reuse your existing logic to move to current location
		await MoveToCurrentLocation();
	}
	#endregion


	private async void Back_Tapped(object sender, TappedEventArgs e)
	{
		await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
		{
			await AnimateElementScaleDown(imBack);
			await AppNavigatorService.NavigateTo("..");
		});
	}
}