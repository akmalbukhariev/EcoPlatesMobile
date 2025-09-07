using CommunityToolkit.Maui.Views;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using System.Globalization;
using System.Text.RegularExpressions;

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
     
    private CompanyApiService companyApiService;
    private AppControl appControl;
    private IKeyboardHelper keyboardHelper;

    public CompanyEditProductPage(CompanyApiService companyApiService, AppControl appControl, IKeyboardHelper keyboardHelper)
    {
        InitializeComponent();

        this.companyApiService = companyApiService;
        this.appControl = appControl;
        this.keyboardHelper = keyboardHelper;

        entryProductName.SetMaxLength(30);
        entryOldPrice.MaxLength = 13;
        entryNewPrice.MaxLength = 13;
        editorDescription.MaxLength = 150;
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
            entryNewPrice.Text = (ProductModel.NewPriceDigit ?? 0m).ToString("0.######", CultureInfo.InvariantCulture).Replace(".", "");
            entryOldPrice.Text = (ProductModel.OldPriceDigit ?? 0m).ToString("0.######", CultureInfo.InvariantCulture).Replace(".", "");
            editorDescription.Text = ProductModel.description;

            if (ProductModel.IsThisActivePage)
            {
                grdBottom.IsVisible = false;
                btnInactive.IsVisible = true;
            }
            else
            {
                grdBottom.IsVisible = true;
                btnInactive.IsVisible = false;
            }
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
        string action = await DisplayActionSheet(AppResource.ChooseOption,
                                                AppResource.Cancel, 
                                                null, AppResource.SelectGallery,
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
        }
    }

    private async void Done_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);

        keyboardHelper.HideKeyboard();
        
        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;

        try
        {
            var title = entryProductName.GetEntryText()?.Trim();
            //var oldPriceText = entryOldPrice.Text?.Trim();
            //var newPriceText = entryNewPrice.Text?.Trim();

            var oldPrice = GetRawNumber(entryOldPrice);
            var newPrice = GetRawNumber(entryNewPrice);

            if (string.IsNullOrWhiteSpace(title))
            {
                await DisplayAlert(AppResource.Error, AppResource.PleaseEnterProductName, AppResource.Ok);
                return;
            }

             
            if (oldPrice == newPrice)
            {
                await DisplayAlert(AppResource.Error, AppResource.MessageOldAndNewPrice, AppResource.Ok);
                return;
            }
            
            if (oldPrice is null || newPrice is null)
            {
                //await DisplayAlert(AppResource.Error, AppResource.PleaseEnterBothPrices, AppResource.Ok);
                await DisplayAlert(AppResource.Error, "Please Enter Both Prices", AppResource.Ok);
                return;
            }

            IsLoading.IsVisible = true;
            IsLoading.IsRunning = true;

            Response response;

            /*
            var culture = CultureInfo.CurrentCulture;

            if (string.Equals(title?.Trim(), ProductModel.ProductName?.Trim(), StringComparison.Ordinal)
                && EqualMoney(ProductModel.OldPriceDigit, oldPriceText, culture)
                && EqualMoney(ProductModel.NewPriceDigit, newPriceText, culture)
                && string.Equals(editorDescription?.Text?.Trim() ?? "", ProductModel.description?.Trim() ?? "", StringComparison.Ordinal)
                && !isNewImageSelected)
            {
                IsLoading.IsVisible = false;
                IsLoading.IsRunning = false;

                await Shell.Current.GoToAsync("..");
                return;
            }*/

            long? modelOld = ProductModel?.OldPriceDigit is null ? null
                          : Convert.ToInt64(ProductModel.OldPriceDigit);
            long? modelNew = ProductModel?.NewPriceDigit is null ? null
                            : Convert.ToInt64(ProductModel.NewPriceDigit);

            bool titleSame = string.Equals(title?.Trim(), ProductModel?.ProductName?.Trim(), StringComparison.Ordinal);
            bool oldSame   = modelOld == oldPrice;
            bool newSame   = modelNew == newPrice;
            bool descSame  = string.Equals(editorDescription?.Text?.Trim() ?? string.Empty,
                                        ProductModel?.description?.Trim() ?? string.Empty,
                                        StringComparison.Ordinal);
            bool noNewImage = !isNewImageSelected;

            if (titleSame && oldSame && newSame && descSame && noNewImage)
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
                { "delete_image", isNewImageSelected.ToString().ToLower() },
                { "description", editorDescription.Text ?? string.Empty },
            };
            response = await companyApiService.UpdatePoster(imageStream, additionalData);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                await AlertService.ShowAlertAsync(AppResource.UpdateProduct, AppResource.Success);
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

    readonly HashSet<Entry> formatting = new();

    void PriceTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;

        // prevent re-entrancy per Entry
        if (formatting.Contains(entry)) return;

        var oldText = e.OldTextValue ?? "";
        var newText = e.NewTextValue ?? "";

        // keep only digits that the user typed (keyboard can be Numeric)
        var digits = Regex.Replace(newText, "[^0-9]", "");
        if (digits.Length == 0)
        {
            formatting.Add(entry);
            entry.Text = "";
            entry.CursorPosition = 0;
            formatting.Remove(entry);
            return;
        }

        // guard against values too large for long
        if (!long.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
        {
            formatting.Add(entry);
            entry.Text = oldText; // revert
            formatting.Remove(entry);
            return;
        }

        // remember caret distance from the end
        var oldLen = oldText.Length;
        var oldCursor = entry.CursorPosition >= 0 ? entry.CursorPosition : oldLen;
        var cursorFromEnd = oldLen - oldCursor;

        // format with commas always
        var formatted = value.ToString("#,0", CultureInfo.InvariantCulture);

        formatting.Add(entry);
        entry.Text = formatted;

        // restore caret relative to the end
        var newLen = formatted.Length;
        var newPos = Math.Max(0, newLen - Math.Max(0, cursorFromEnd));
        entry.CursorPosition = Math.Min(newPos, newLen);

        formatting.Remove(entry);
    }

    long? GetRawNumber(Entry e)
    {
        var digits = Regex.Replace(e.Text ?? "", "[^0-9]", "");
        if (string.IsNullOrEmpty(digits)) return null;
        return long.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var v) ? v : null;
    }

    bool EqualMoney(decimal? modelValue, string text, CultureInfo culture, int scale = 2)
    {
        if (modelValue is null)
            return string.IsNullOrWhiteSpace(text);

        if (!decimal.TryParse(text, NumberStyles.Number, culture, out var parsed))
            return false;
        
        return decimal.Round(parsed, scale) == decimal.Round(modelValue.Value, scale);
    }

    private async void BtnActive_Clicked(object sender, EventArgs e)
    { 
        bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes, AppResource.No);

        if (!answer) return;

        try
        {
            ShowLoading(true);

            ChangePosterDeletionRequest request = new ChangePosterDeletionRequest()
            {
                poster_id = ProductModel.PromotionId,
                deleted = false
            };

            Response response = await companyApiService.ChangePosterDeletionStatus(request);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                appControl.RefreshCompanyProfilePage = true;
                await Shell.Current.GoToAsync("..");
                return;
            }
            else
            {
                await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
            }
        }
        catch (Exception ex)
        {
            await AlertService.ShowAlertAsync(AppResource.Error, ex.Message);
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        bool isWifiOn = await appControl.CheckWifi();
        if (!isWifiOn) return;

        bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes, AppResource.No);

        if (!answer) return;
        
        ProductModel.CompanyId = (long)appControl.CompanyInfo.company_id;
        try
        {
            ShowLoading(true);

            Response response = await companyApiService.DeletePoster(ProductModel.PromotionId);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                appControl.RefreshCompanyProfilePage = true;
                await Shell.Current.GoToAsync("..");
                return;
            }
            else
            {
                await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
            }
        }
        catch (Exception ex)
        {
            await AlertService.ShowAlertAsync(AppResource.Error, ex.Message);
        }
        finally
        {
            ShowLoading(true);
        }
    }

    private async void BtnInActive_Clicked(object sender, EventArgs e)
    {
        bool isWifiOn = await appControl.CheckWifi();
        if (!isWifiOn) return;

        bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes, AppResource.No);

        if (!answer) return;

        try
        {
            ShowLoading(true);

            ChangePosterDeletionRequest request = new ChangePosterDeletionRequest()
            {
                poster_id = ProductModel.PromotionId,
                deleted = true
            };

            Response response = await companyApiService.ChangePosterDeletionStatus(request);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                appControl.RefreshCompanyProfilePage = true;
                await Shell.Current.GoToAsync("..");
                return;
            }
            else
            {
                await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
            }
        }
        catch (Exception ex)
        {
            await AlertService.ShowAlertAsync(AppResource.Error, ex.Message);
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private void ShowLoading(bool show)
    { 
        IsLoading.IsVisible = show;
        IsLoading.IsRunning = show;
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