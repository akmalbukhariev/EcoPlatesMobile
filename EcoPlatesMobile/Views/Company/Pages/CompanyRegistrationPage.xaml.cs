using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Views;
using EcoPlatesMobile.Helper;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.Company.Pages;

[QueryProperty(nameof(PhoneNumber), nameof(PhoneNumber))]
public partial class CompanyRegistrationPage : BasePage
{
    private string _phoneNumber;
    public string PhoneNumber
    {
        set
        {
            _phoneNumber = value;
        }
    }

    public ObservableCollection<CompanyTypeModel> CompanyTypeList { get; set; }

    private CompanyTypeModel selectedCompanyType = null;
    private Stream? imageStream = null;
    private bool isNewImageSelected = false;

    private CompanyApiService companyApiService;
    private AppControl appControl;
    private LocationService locationService;
    private IKeyboardHelper keyboardHelper;
    
    public CompanyRegistrationPage(CompanyApiService companyApiService, AppControl appControl, LocationService locationService, IKeyboardHelper keyboardHelper)
    {
        InitializeComponent();
 
        this.appControl = appControl;
        this.companyApiService = companyApiService;
        this.locationService = locationService;
        this.keyboardHelper = keyboardHelper;

        appControl.RebuildBusinessTypeList();
        CompanyTypeList = new ObservableCollection<CompanyTypeModel>(
            appControl.BusinessTypeList.Select(kvp => new CompanyTypeModel
            {
                Type = kvp.Key,
                Type_value = kvp.Value
            })
        );

        pickType.ItemsSource = appControl.BusinessTypeList.Keys.ToList();
 
        startTimePicker.Time = new TimeSpan(9, 0, 0);
        endTimePicker.Time = new TimeSpan(18, 0, 0);

        loading.ChangeColor(Constants.COLOR_COMPANY);

        entryCompanyName.SetMaxLength(20);
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrEmpty(_phoneNumber))
            entryPhone.SetEntryText(_phoneNumber.Replace("998", ""));

        loading.ShowLoading = true;
        CancelAndDisposeCts();
        cts = new CancellationTokenSource();
        appControl.LocationForRegister = await locationService.GetCurrentLocationAsync(cts.Token);
        loading.ShowLoading = false;

        if (appControl.LocationForRegister != null)
        {
            string lat = appControl.LocationForRegister.Latitude.ToString("F6", CultureInfo.InvariantCulture);
            string lon = appControl.LocationForRegister.Longitude.ToString("F6", CultureInfo.InvariantCulture);

            lbLocation.Text = $"{AppResource.Location} {lat}, {lon}";
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        CancelAndDisposeCts();
    }    

    private async void SelectImage_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
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
        });
    }

    private async void CompanyTypeTapped(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            var popup = new CompanyTypePickerPopup(CompanyTypeList);
            var result = await this.ShowPopupAsync(popup);

            if (result is CompanyTypeModel selected)
            {
                labelCompanyType.Text = selected.Type;
                selectedCompanyType = selected;
            }
        });
    }

    private async void BtnRegister_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            keyboardHelper.HideKeyboard();

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            try
            {
                string companyName = entryCompanyName.GetEntryText();
                string selectedType = pickType.SelectedItem as string;
                string phoneNumber = _phoneNumber;
                string formattedWorkingHours = $"{DateTime.Today.Add(startTimePicker.Time):hh:mm tt} - {DateTime.Today.Add(endTimePicker.Time):hh:mm tt}";

                if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(phoneNumber))
                {
                    await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.MessageFieldCannotBeEmty);
                    return;
                }

                if (string.IsNullOrEmpty(selectedType))
                {
                    await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.MessageSelectCompanyType);
                    return;
                }

                if (imageStream == null)
                {
                    await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.MessageSelectCompanyLogo);
                    return;
                }
                 
                if (appControl.LocationForRegister == null)
                {
                    await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.MessageLocationEmpty);
                    return;
                }

                bool yes = await AlertService.ShowConfirmationAsync(AppResource.Confirm, AppResource.ConfirmCompanyLocationAdd, AppResource.Yes, AppResource.No);
                if (!yes) return;

                var additionalData = new Dictionary<string, string>
                {
                    { "company_name", companyName },
                    { "business_type", appControl.BusinessTypeList[selectedType] },
                    { "phone_number", phoneNumber},
                    { "location_latitude", appControl.LocationForRegister.Latitude.ToString("F6", CultureInfo.InvariantCulture) },
                    { "location_longitude", appControl.LocationForRegister.Longitude.ToString("F6", CultureInfo.InvariantCulture) },
                    { "working_hours", formattedWorkingHours},
                };

                loading.ShowLoading = true;

                Response response = await companyApiService.RegisterCompany(imageStream, additionalData);
                  
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    await AlertService.ShowAlertAsync(AppResource.Success, AppResource.MessageRegistrationSuccess);
                    await appControl.LoginCompany(_phoneNumber);
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
        });
    }

    private async void BusinessType_SelectedIndexChanged(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            string selectedDisplay = pickType.SelectedItem as string;
            if (selectedDisplay != null && appControl.BusinessTypeList.TryGetValue(selectedDisplay, out var backendValue))
            {

            }
        });
    }

    private async void Location_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(sender as Border);

            CancelAndDisposeCts();
            cts = new CancellationTokenSource();
            loading.ShowLoading = true;
            var locationService = new LocationService();
            var location = await locationService.GetCurrentLocationAsync(cts.Token);

            loading.ShowLoading = false;

            if (location == null)
            {
                return;
            }

            await AppNavigatorService.NavigateTo(nameof(LocationRegistrationPage));
        });
    }
}