using CommunityToolkit.Maui.Views;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.Company.Pages;
 
public partial class CompanyAddProductPage : BasePage
{
    private ProductModel productModel;
     
    private Stream? imageStream = null;
    private bool isNewImageSelected = false;

    private AppControl appControl;
    private CompanyApiService companyApiService;

    public CompanyAddProductPage(CompanyApiService companyApiService, AppControl appControl)
	{
		InitializeComponent();

        this.companyApiService = companyApiService;
        this.appControl = appControl;

        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        isNewImageSelected = false;
    }

    private async void SelectImage_Tapped(object sender, TappedEventArgs e)
    {
        if (borderProductIcon.IsVisible)
        {
            await AnimateElementScaleDown(borderProductIcon);
        }
        else
        {
            await AnimateElementScaleDown(imSelectedProduct);
        }
        
        string action = await DisplayActionSheet(AppResource.ChooseOption, 
                                                 AppResource.Cancel, 
                                                 null, 
                                                 AppResource.SelectGallery, 
                                                 AppResource.TakePhoto);

        FileResult result = null;

        if (action == AppResource.SelectGallery)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                result = await MediaPicker.PickPhotoAsync();
            }
        }
        else if (action == AppResource.TakePhoto)
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

            borderProductIcon.IsVisible = false;
            imSelectedProduct.IsVisible = true;
        }
    }

    private async void RegisterOrUpdate_Clicked(object sender, EventArgs e)
    {
        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;
        
        try
        {
            var title = entryProductName.GetEntryText()?.Trim();
            var oldPriceText = entryOldPrice.Text?.Trim();
            var newPriceText = entryNewPrice.Text?.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                await DisplayAlert(AppResource.Error, AppResource.PleaseEnterProductName, AppResource.Ok);
                return;
            }

            if (decimal.TryParse(oldPriceText, out decimal oldPrice) &&
                decimal.TryParse(newPriceText, out decimal newPrice))
            {
                if (oldPrice == newPrice)
                {
                    await DisplayAlert(AppResource.Error, AppResource.MessageOldAndNewPrice, AppResource.Ok);
                    return;
                }
            }
            else
            {
                await DisplayAlert(AppResource.Error, AppResource.MessageValidPrice, AppResource.Ok);
                return;
            }

            IsLoading.IsVisible = true;
            IsLoading.IsRunning = true;

            if (imageStream == null)
            {
                await DisplayAlert(AppResource.Error, AppResource.MessageSelectImage, AppResource.Ok);
                return;
            }

            var additionalData = new Dictionary<string, string>
            {
                { "company_id", appControl.CompanyInfo.company_id.ToString() },
                { "title", title },
                { "old_price", oldPrice.ToString() },
                { "new_price", newPrice.ToString() },
                { "description", editorDescription.Text ?? string.Empty },
            };

            Response response = await companyApiService.RegisterPoster(imageStream, additionalData);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                await AlertService.ShowAlertAsync(AppResource.RegisterProduct, AppResource.Success);
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(AppResource.Error, ex.Message, AppResource.Ok);
        }
        finally
        {
            IsLoading.IsVisible = false;
            IsLoading.IsRunning = false;
        }
    }
}