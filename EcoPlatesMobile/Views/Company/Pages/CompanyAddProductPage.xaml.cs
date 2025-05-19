using CommunityToolkit.Maui.Views;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.Company.Pages;

[QueryProperty(nameof(ProductModel), nameof(ProductModel))]
public partial class CompanyAddProductPage : BasePage
{
    private ProductModel productModel;
    public ProductModel ProductModel
    {
        get => productModel;
        set
        {
            productModel = value;
        }
    }
    private Stream? imageStream = null;
    public CompanyAddProductPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(ProductModel != null)
        {
            btnRegisterOrUpdate.Text = "Update";
            borderProductIcon.IsVisible = false;
            borderSelectedProduct.IsVisible = true;

            imSelectedProduct.Source = ProductModel.ProductImage;
            entryProductName.SetEntryText(ProductModel.ProductName);
            entryNewPrice.Text = ProductModel.NewPrice;
            entryOldPrice.Text = ProductModel.OldPrice;
        }
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

    private async void RegisterOrUpdate_Clicked(object sender, EventArgs e)
    {
        try
        {
            var title = entryProductName.GetEntryText()?.Trim();
            var oldPriceText = entryOldPrice.Text?.Trim();
            var newPriceText = entryNewPrice.Text?.Trim();
 
            if (imageStream == null)
            {
                await DisplayAlert("Error", "Please select an image before registering.", "OK");
                return;
            }
 
            if (string.IsNullOrWhiteSpace(title))
            {
                await DisplayAlert("Error", "Please enter a product name.", "OK");
                return;
            }
 
            if (decimal.TryParse(oldPriceText, out decimal oldPrice) && 
                decimal.TryParse(newPriceText, out decimal newPrice))
            {
                if (oldPrice == newPrice)
                {
                    await DisplayAlert("Error", "Old price and new price cannot be the same.", "OK");
                    return;
                }
            }
            else
            {
                await DisplayAlert("Error", "Please enter valid prices.", "OK");
                return;
            }


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