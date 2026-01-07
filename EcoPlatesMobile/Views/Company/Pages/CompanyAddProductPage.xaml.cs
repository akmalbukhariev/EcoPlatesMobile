using CommunityToolkit.Maui.Views;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using System.Text.RegularExpressions;
using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class CompanyAddProductPage : BasePage
{
    private ProductModel productModel;

    private Stream? imageStream = null;
    private bool isNewImageSelected = false;
 
    private CompanyApiService companyApiService;
    private IKeyboardHelper keyboardHelper; 

    public CompanyAddProductPage(CompanyApiService companyApiService, IKeyboardHelper keyboardHelper)
    {
        InitializeComponent();

        this.companyApiService = companyApiService; 
        this.keyboardHelper = keyboardHelper;

        appControl.RebuildPosterTypeList();
        pickType.ItemsSource = appControl.PosterTypeList.Keys.ToList();

        entryProductName.SetMaxLength(30);
        entryOldPrice.MaxLength = 13;
        entryNewPrice.MaxLength = 13;
        editorDescription.MaxLength = 150;
        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        isNewImageSelected = false;
    }

    private async void SelectImage_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
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
                if (!await appControl.EnsureGalleryPermissionAsync())
                    return;

                result = await appControl.TryPickPhotoAsync();
            }
            else if (action == AppResource.TakePhoto)
            {
                if (!await appControl.EnsureCameraPermissionAsync())
                    return;

                result = await appControl.TryCapturePhotoAsync();
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
        });
    }

    private void PosterType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //string selectedType = pickType.SelectedItem as string;
        //string sss = appControl.PosterTypeList[selectedType];
    }

    private async void Register_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            keyboardHelper.HideKeyboard();

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            try
            {
                var title = entryProductName.GetEntryText()?.Trim();
                string selectedType = pickType.SelectedItem as string;
                var oldPrice = GetRawNumber(entryOldPrice);
                var newPrice = GetRawNumber(entryNewPrice);

                if (string.IsNullOrWhiteSpace(title))
                {
                    await DisplayAlert(AppResource.Error, AppResource.PleaseEnterProductName, AppResource.Ok);
                    return;
                }

                if (string.IsNullOrEmpty(selectedType))
                {
                    await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.MessageSelectPosterType);
                    return;
                }

                if (oldPrice == newPrice)
                {
                    await DisplayAlert(AppResource.Error, AppResource.MessageOldAndNewPrice, AppResource.Ok);
                    return;
                }

                if (oldPrice == null || newPrice == null)
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
                    { "category", appControl.PosterTypeList[selectedType]},
                    { "old_price", oldPrice.ToString() },
                    { "new_price", newPrice.ToString() },
                    { "description", editorDescription.Text ?? string.Empty },
                };

                Response response = await companyApiService.RegisterPoster(imageStream, additionalData);
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
                    return;
                }

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    appControl.RefreshCompanyProfilePage = true;
                    await Toast.Make(AppResource.MessageModeration, ToastDuration.Short).Show();
                    //await AlertService.ShowAlertAsync(AppResource.Info, AppResource.MessageModeration);
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, AppResource.ErrorUnexpected);
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
}