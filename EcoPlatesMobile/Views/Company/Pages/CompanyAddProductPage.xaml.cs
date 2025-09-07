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

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class CompanyAddProductPage : BasePage
{
    private ProductModel productModel;

    private Stream? imageStream = null;
    private bool isNewImageSelected = false;

    private AppControl appControl;
    private CompanyApiService companyApiService;
    private IKeyboardHelper keyboardHelper;

    public CompanyAddProductPage(CompanyApiService companyApiService, AppControl appControl, IKeyboardHelper keyboardHelper)
    {
        InitializeComponent();

        this.companyApiService = companyApiService;
        this.appControl = appControl;
        this.keyboardHelper = keyboardHelper;

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
    }

    private async void RegisterOrUpdate_Clicked(object sender, EventArgs e)
    {
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
}