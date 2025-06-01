using CommunityToolkit.Maui.Views;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.Company.Pages;

[QueryProperty(nameof(ProductModel), nameof(ProductModel))]
public partial class CompanyEditProductPage : BasePage
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

    public CompanyEditProductPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        isNewImageSelected = false;

        if (ProductModel != null)
        {
            imSelectedProduct.Source = ProductModel.ProductImage;
            fullImage.Source = ProductModel.ProductImage;
            entryProductName.SetEntryText(ProductModel.ProductName);
            entryNewPrice.Text = ProductModel.NewPriceDigit.ToString();
            entryOldPrice.Text = ProductModel.OldPriceDigit.ToString();
            editorDescription.Text = ProductModel.description;
        }
    }

    private async void ProductImage_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(imSelectedProduct);

        fullImage.TranslationY = -100;
        fullImage.Opacity = 0;
        fullImage.IsVisible = true;
        boxFullImage.IsVisible = true;

        await Task.WhenAll(
            fullImage.TranslateTo(0, 0, 250, Easing.SinIn),
            fullImage.FadeTo(1, 250, Easing.SinIn)
        );
    }

    private async void ChangeImage_Clicked(object sender, EventArgs e)
    {
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
            using (FileStream localFileStream = File.Create(localFilePath))
            {
                await sourceStream.CopyToAsync(localFileStream);
            }

            imSelectedProduct.Source = ImageSource.FromFile(localFilePath);
            imageStream = await result.OpenReadAsync();
            isNewImageSelected = true;
        }
    }

    private async void BtnUpdate_Clicked(object sender, EventArgs e)
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
              
            if (title.Trim().Equals(ProductModel.ProductName?.Trim() ?? string.Empty) &&
                oldPriceText.Trim().Equals(ProductModel.OldPriceDigit?.ToString() ?? string.Empty) &&
                newPriceText.Trim().Equals(ProductModel.NewPriceDigit?.ToString() ?? string.Empty) &&
                editorDescription.Text.Equals(ProductModel.description) &&
                !isNewImageSelected)
            {
                IsLoading.IsVisible = false;
                IsLoading.IsRunning = false;

                await Shell.Current.GoToAsync("..");
                return;
            }

            if (oldPriceText.Trim().Equals(newPriceText.Trim()))
            {
                await DisplayAlert("Error", "Product prices can not be equal.", "OK");
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
                { "delete_image", isNewImageSelected.ToString().ToLower() },
                { "description", editorDescription.Text ?? string.Empty },
            };
            response = await apiService.UpdatePoster(imageStream, additionalData);
              
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

    private async void OnImage_Swiped(object sender, SwipedEventArgs e)
    {
        await Task.WhenAll(
            fullImage.TranslateTo(0, -100, 250, Easing.SinOut),
            fullImage.FadeTo(0, 250, Easing.SinOut)
        );

        boxFullImage.IsVisible = false;
        fullImage.IsVisible = false;
        fullImage.Opacity = 1;
        fullImage.TranslationY = 0;
    }

    private void OnImage_Tapped(object sender, TappedEventArgs e)
    {
        boxFullImage.IsVisible = false;
        fullImage.IsVisible = false;
    }

    private string GetFileNameFromUrl(string url)
    {
        Uri uri = new Uri(url);
        return Path.GetFileName(uri.LocalPath);
    }
  }