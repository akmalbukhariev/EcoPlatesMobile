using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using EcoPlatesMobile.Helper;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Resources.Languages;
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

    public CompanyRegistrationPage()
    {
        InitializeComponent();

        CompanyTypeList = new ObservableCollection<CompanyTypeModel>(
            AppService.Get<AppControl>().BusinessTypeList.Select(kvp => new CompanyTypeModel
            {
                Type = kvp.Key,
                Type_value = kvp.Value
            })
        );

        BindingContext = this;

        startTimePicker.Time = new TimeSpan(9, 0, 0);
        endTimePicker.Time = new TimeSpan(6, 0, 0);

        loading.ChangeColor(Color.FromArgb("#8338EC"));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrEmpty(_phoneNumber))
            entryPhone.SetEntryText(_phoneNumber.Replace("998", ""));
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

    private async void CompanyTypeTapped(object sender, EventArgs e)
    {
        var popup = new CompanyTypePickerPopup(CompanyTypeList);
        var result = await this.ShowPopupAsync(popup);

        if (result is CompanyTypeModel selected)
        {
            labelCompanyType.Text = selected.Type;
            selectedCompanyType = selected;
        }
    }

    private async void BtnRegister_Clicked(object sender, EventArgs e)
    {
        try
        {
            string companyName = entryCompanyName.GetEntryText();
            string selectedType = selectedCompanyType == null ? null : selectedCompanyType.Type_value;
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

            Location location = await AppService.Get<LocationService>().GetCurrentLocationAsync();

            var additionalData = new Dictionary<string, string>
            {
                { "company_name", companyName },
                { "business_type", selectedType },
                { "phone_number", phoneNumber},
                { "location_latitude",  location.Latitude.ToString()},
                { "location_longitude", location.Longitude.ToString()},
                { "working_hours", formattedWorkingHours},
            };

            loading.ShowLoading = true;
            var apiService = AppService.Get<CompanyApiService>();

            Response response = await apiService.RegisterCompany(imageStream, additionalData);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                await AlertService.ShowAlertAsync(AppResource.Success, AppResource.MessageRegistrationSuccess);
                await AppService.Get<AppControl>().LoginCompany(_phoneNumber);
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
    }
}