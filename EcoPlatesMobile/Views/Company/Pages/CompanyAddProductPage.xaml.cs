using CommunityToolkit.Maui.Views;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class CompanyAddProductPage : BasePage
{
    private Stream? imageStream = null;
    public CompanyAddProductPage()
	{
		InitializeComponent();
	}

    private async void SelectImage_Tapped(object sender, TappedEventArgs e)
    {
        if (borderProductIcon.IsVisible)
        {
            await AnimateElementScaleDown(borderProductIcon);
        }
        else if (borderSelectedProduct.IsVisible)
        {
            await AnimateElementScaleDown(borderSelectedProduct);
        }

        string action = await DisplayActionSheet("Choose an option", "Cancel", null, "Select from Gallery", "Take a Photo");

        FileResult result = null;

        if (action == "Select from Gallery")
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                result = await MediaPicker.PickPhotoAsync();
            }
        }
        else if (action == "Take a Photo")
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                result = await MediaPicker.CapturePhotoAsync();
            }
        }

        if (result != null)
        {
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, result.FileName);

            using (Stream sourceStream = await result.OpenReadAsync())
            using (FileStream localFileStream = File.OpenWrite(localFilePath))
            {
                await sourceStream.CopyToAsync(localFileStream);
            }
 
            imSelectedProduct.Source = ImageSource.FromFile(localFilePath);

            imageStream = await result.OpenReadAsync();

            borderProductIcon.IsVisible = false;
            borderSelectedProduct.IsVisible = true;
        }
    }

    private async void Register_Clicked(object sender, EventArgs e)
    {
        try
        {
            var apiService = AppService.Get<CompanyApiService>();
 
            var additionalData = new Dictionary<string, string>
            {
                { "company_id", "11" },
                { "title", entryProductName.GetEntryText() ?? string.Empty },
                { "old_price", entryOldPrice.Text ?? "0" },
                { "new_price", entryNewPrice.Text ?? "0" },
            };

            IsLoading.IsVisible = true;
            IsLoading.IsRunning = true;

            Response response = await apiService.RegisterPoster(imageStream, additionalData);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                await AlertService.ShowAlertAsync("Register poster", "Success.");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await AlertService.ShowAlertAsync("Error", response.resultMsg);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsLoading.IsVisible = false;
            IsLoading.IsRunning = false;
        }
    }
}