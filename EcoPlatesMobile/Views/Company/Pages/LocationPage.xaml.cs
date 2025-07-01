using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class LocationPage : BasePage
{
	public LocationPage()
	{
		InitializeComponent();
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
				await AlertService.ShowAlertAsync(AppResource.MessageUpdateLocation, AppResource.Success);
				await Shell.Current.GoToAsync("..", true);
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
		await AnimateElementScaleDown(sender as Image);
        await AppNavigatorService.NavigateTo("..");
    }
}