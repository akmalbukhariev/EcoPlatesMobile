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
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;

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
    private UserSessionService userSessionService;
    private IStatusBarService statusBarService;

    public CompanyEditProductPage(CompanyApiService companyApiService, AppControl appControl, IKeyboardHelper keyboardHelper,
    UserSessionService userSessionService, IStatusBarService statusBarService)
    {
        InitializeComponent();

        this.companyApiService = companyApiService;
        this.appControl = appControl;
        this.keyboardHelper = keyboardHelper;
        this.statusBarService = statusBarService;
        this.userSessionService = userSessionService;

        entryProductName.SetMaxLength(30);
        entryOldPrice.MaxLength = 13;
        entryNewPrice.MaxLength = 13;
        editorDescription.MaxLength = 150;

        appControl.RebuildPosterTypeList();
        pickType.ItemsSource = appControl.PosterTypeList.Keys.ToList();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (userSessionService.Role == UserRole.User)
        {
            statusBarService.SetStatusBarColor(Constants.COLOR_USER.ToArgbHex(), false);
        }
        else
        {
            statusBarService.SetStatusBarColor(Constants.COLOR_COMPANY.ToArgbHex(), false);
        }

        isNewImageSelected = false;

        if (ProductModel != null)
        {
            imSelectedProduct.Source = ProductModel.ProductImage;
            fullImage.Source = ProductModel.ProductImage;

            string displayText = appControl.PosterTypeList.FirstOrDefault(x => x.Value == ProductModel.Category).Key;

            pickType.SelectedItem = displayText;

            entryProductName.SetEntryText(ProductModel.ProductName);
            entryOldPrice.Text = (ProductModel.OldPriceDigit ?? 0m).ToString("#,0", CultureInfo.InvariantCulture);
            entryNewPrice.Text = (ProductModel.NewPriceDigit ?? 0m).ToString("#,0", CultureInfo.InvariantCulture);

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
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(imSelectedProduct);

            fullImage.TranslationY = -100;
            fullImage.Opacity = 0;
            fullImage.IsVisible = true;
            
            boxFullImage.IsVisible = true;
            boxFullImage.Opacity = 0.5;      // show dark overlay
            boxFullImage.InputTransparent = false; // start catching taps

            await Task.WhenAll(
                fullImage.TranslateTo(0, 0, 250, Easing.SinIn),
                fullImage.FadeTo(1, 250, Easing.SinIn)
            );
        });
    }

    private async void ChangeImage_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
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
        });
    }
    
    private void PosterType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //string selectedType = pickType.SelectedItem as string;
        //string sss = appControl.PosterTypeList[selectedType];
    }

    private async void Done_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(sender as Image);

            keyboardHelper.HideKeyboard();

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            try
            {
                var title = entryProductName.GetEntryText()?.Trim();
                string selectedType = pickType.SelectedItem as string;
                var type = appControl.PosterTypeList[selectedType];
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
                    await DisplayAlert(AppResource.Error, AppResource.PleaseEnterBothPrices, AppResource.Ok);
                    return;
                }

                IsLoading.IsVisible = true;
                IsLoading.IsRunning = true;

                Response response;

                long? modelOld = ProductModel?.OldPriceDigit is null ? null
                              : Convert.ToInt64(ProductModel.OldPriceDigit);
                long? modelNew = ProductModel?.NewPriceDigit is null ? null
                                : Convert.ToInt64(ProductModel.NewPriceDigit);

                bool titleSame = string.Equals(title?.Trim(), ProductModel?.ProductName?.Trim(), StringComparison.Ordinal);
                bool typeSame = string.Equals(ProductModel.Category,type);
                bool oldSame = modelOld == oldPrice;
                bool newSame = modelNew == newPrice;
                bool descSame = string.Equals(editorDescription?.Text?.Trim() ?? string.Empty,
                                            ProductModel?.description?.Trim() ?? string.Empty,
                                            StringComparison.Ordinal);
                bool noNewImage = !isNewImageSelected;

                if (titleSame && oldSame && newSame && descSame && noNewImage && typeSame)
                {
                    IsLoading.IsVisible = false;
                    IsLoading.IsRunning = false;
                    await Shell.Current.GoToAsync("..");
                    return;
                }

                if (string.IsNullOrEmpty(selectedType))
                {
                    await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.MessageSelectPosterType);
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
                    { "category", appControl.PosterTypeList[selectedType]},
                    { "old_price", oldPrice.ToString() },
                    { "new_price", newPrice.ToString() },
                    { "image_file_name", oldFileName },
                    { "delete_image", isNewImageSelected.ToString().ToLower() },
                    { "description", editorDescription.Text ?? string.Empty },
                };
                response = await companyApiService.UpdatePoster(imageStream, additionalData);
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
                    return;
                }

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    appControl.RefreshCompanyProfilePage = true;
                    //await AlertService.ShowAlertAsync(AppResource.UpdateProduct, AppResource.Success);
                    await Toast.Make(AppResource.MessageModeration, ToastDuration.Short).Show();
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
        });
    }
   
    bool _isFormattingPrice;
    
    void PriceTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isFormattingPrice) return;

        if (sender is not Entry entry) return;

        var newText = e.NewTextValue ?? string.Empty;

        // digits only
        var digits = Regex.Replace(newText, "[^0-9]", "");

        _isFormattingPrice = true;
        Dispatcher.Dispatch(() =>
        {
            try
            {
                if (digits.Length == 0)
                {
                    entry.Text = string.Empty;
                    entry.CursorPosition = 0;
                    entry.SelectionLength = 0;
                    return;
                }

                // limit if you want (optional): max 13 digits
                // if (digits.Length > 13) digits = digits.Substring(0, 13);

                if (!long.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                {
                    // revert safely
                    entry.Text = e.OldTextValue ?? string.Empty;
                    entry.CursorPosition = entry.Text.Length;
                    entry.SelectionLength = 0;
                    return;
                }

                var formatted = value.ToString("#,0", CultureInfo.InvariantCulture);

                if (entry.Text != formatted)
                    entry.Text = formatted;

                // ✅ iOS-safe: always keep caret at end
                entry.CursorPosition = formatted.Length;
                entry.SelectionLength = 0;
            }
            finally
            {
                _isFormattingPrice = false;
            }
        });
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
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
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
        });
    }

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            bool isWifiOn = await appControl.CheckWifiOrNetwork();
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
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
                    return;
                }

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
        });
    }

    private async void BtnInActive_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            bool isWifiOn = await appControl.CheckWifiOrNetwork();
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
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
                    return;
                }
                
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
        });
    }

    private void ShowLoading(bool show)
    { 
        IsLoading.IsVisible = show;
        IsLoading.IsRunning = show;
    }

    private async void OnImage_Swiped(object sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Down)
        {
            // Move image down and fade it out
            await Task.WhenAll(
                fullImage.TranslateTo(0, 100, 250, Easing.SinOut),
                fullImage.FadeTo(0, 250, Easing.SinOut)
            );

            boxFullImage.IsVisible = false; // Optionally hide the container box
            boxFullImage.Opacity = 0;        // hide overlay
            boxFullImage.InputTransparent = true;  // let taps pass through again

            fullImage.IsVisible = false; // Hide the image after animation
            fullImage.Opacity = 1; // Reset opacity for future animations
            fullImage.TranslationY = 0; // Reset position for future animations
        }
        else if (e.Direction == SwipeDirection.Up)
        {
            // Move image up and fade it out
            await Task.WhenAll(
                fullImage.TranslateTo(0, -100, 250, Easing.SinOut),
                fullImage.FadeTo(0, 250, Easing.SinOut)
            );

            // Reset image visibility and properties after animation
            boxFullImage.IsVisible = false; // Optionally hide the container box
            boxFullImage.Opacity = 0;        // hide overlay
            boxFullImage.InputTransparent = true;  // let taps pass through again

            fullImage.IsVisible = false; // Hide the image after the animation
            fullImage.Opacity = 1; // Reset opacity
            fullImage.TranslationY = 0; // Reset position
        }
    }

    private void OnImage_Tapped(object sender, TappedEventArgs e)
    {
        boxFullImage.IsVisible = false;
        boxFullImage.Opacity = 0;        // hide overlay
        boxFullImage.InputTransparent = true;  // let taps pass through again

        fullImage.IsVisible = false;
    }

    private string GetFileNameFromUrl(string url)
    {
        Uri uri = new Uri(url);
        return Path.GetFileName(uri.LocalPath);
    }
  }