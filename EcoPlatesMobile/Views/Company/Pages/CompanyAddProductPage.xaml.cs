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
    private bool isNewImageSelected = false;

    public CompanyAddProductPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        isNewImageSelected = false;

        if (ProductModel != null)
        {
            btnRegisterOrUpdate.Text = "Update";
            borderProductIcon.IsVisible = false;
            borderSelectedProduct.IsVisible = true;

            imSelectedProduct.Source = ProductModel.ProductImage;
            entryProductName.SetEntryText(ProductModel.ProductName);
            entryNewPrice.Text = ProductModel.NewPriceDigit.ToString();
            entryOldPrice.Text = ProductModel.OldPriceDigit.ToString();
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
            isNewImageSelected = true;

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
  
            IsLoading.IsVisible = true;
            IsLoading.IsRunning = true;

            Response response;
            if (ProductModel == null)
            {
                if (imageStream == null)
                {
                    await DisplayAlert("Error", "Please select an image before registering.", "OK");
                    return;
                }

                var additionalData = new Dictionary<string, string>
                {
                    { "company_id", ProductModel.CompanyId.ToString() },
                    { "title", title },
                    { "old_price", oldPrice.ToString() },
                    { "new_price", newPrice.ToString() },
                };

                response = await apiService.RegisterPoster(imageStream, additionalData);
            }
            else
            {
                if (title.Trim().Equals(ProductModel.ProductName.Trim()) &&
                    oldPriceText.Trim().Equals(newPriceText.Trim()) &&
                    !isNewImageSelected)
                {
                    IsLoading.IsVisible = false;
                    IsLoading.IsRunning = false;

                    await Shell.Current.GoToAsync("..");
                    return;
                }

                string oldFileName = GetFileNameFromUrl(ProductModel.ProductImage);
                                
                if (!isNewImageSelected)
                {
                    imageStream = null;
                }

                var additionalData = new Dictionary<string, string>
                {
                    { "company_id", ProductModel.CompanyId.ToString() },
                    { "poster_id", ProductModel.PromotionId.ToString() },
                    { "title", title },
                    { "old_price", oldPrice.ToString() },
                    { "new_price", newPrice.ToString() },
                    { "image_file_name", oldFileName },
                    { "delete_image", isNewImageSelected.ToString().ToLower() }
                };
                response = await apiService.UpdatePoster(imageStream, additionalData);
            }

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

    private string GetFileNameFromUrl(string url)
    {
        Uri uri = new Uri(url);
        return Path.GetFileName(uri.LocalPath);
    }

    private async Task<byte[]> GetImageBytesFromUrl(string url)
    {
        using var httpClient = new HttpClient();
        return await httpClient.GetByteArrayAsync(url);
    }
}