using EcoPlatesMobile.Models.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using Newtonsoft.Json;
using System.Linq;

namespace EcoPlatesMobile.Views.Company.Pages;
 
public partial class CompanyProfileInfoPage : BasePage
{
    private CompanyInfo _companyInfo;
      
    private Stream? imageStream = null;
    private bool isNewImageSelected = false;

    public CompanyProfileInfoPage()
    {
        InitializeComponent();

        pickType.ItemsSource = AppService.Get<AppControl>().BusinessTypeList.Keys.ToList();
        _companyInfo = AppService.Get<AppControl>().CompanyInfo;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        imCompany.Source = _companyInfo.logo_url;
        fullImage.Source = _companyInfo.logo_url;
        entryCompanyName.Text = _companyInfo.company_name;
        lbPhoneNUmber.Text = _companyInfo.phone_number;

        string[] times = _companyInfo.working_hours.Split(" - ");
        if (times.Length == 2)
        {
            if (DateTime.TryParse(times[0], out DateTime startTime) &&
                DateTime.TryParse(times[1], out DateTime endTime))
            {
                startTimePicker.Time = startTime.TimeOfDay;
                endTimePicker.Time = endTime.TimeOfDay;
            }
            else
            {
                Console.WriteLine("Invalid time format received from server.");
            }
        }
        
        var selectedItem = AppService.Get<AppControl>().BusinessTypeList.FirstOrDefault(kvp => kvp.Value == _companyInfo.business_type).Key;

        if (selectedItem != null)
        {
            pickType.SelectedItem = selectedItem;
        }
    }

    private async void BorderImage_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(borderCompanyImage);

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
                                                 AppResource.Cancel, null, 
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
            /*
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, result.FileName);

            using (Stream sourceStream = await result.OpenReadAsync())
            using (FileStream localFileStream = File.OpenWrite(localFilePath))
            {
                await sourceStream.CopyToAsync(localFileStream);
            }

            imCompany.Source = ImageSource.FromFile(localFilePath);
            fullImage.Source = imCompany.Source;
            imageStream = await result.OpenReadAsync();
            isNewImageSelected = true;
            */

            string localFilePath = Path.Combine(FileSystem.CacheDirectory, result.FileName);

            using (Stream sourceStream = await result.OpenReadAsync())
            using (FileStream localFileStream = File.Create(localFilePath))
            {
                await sourceStream.CopyToAsync(localFileStream);
            }

            imCompany.Source = ImageSource.FromFile(localFilePath);
            fullImage.Source = imCompany.Source;
 
            imageStream = File.OpenRead(localFilePath);

            isNewImageSelected = true;
        }
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", true);
    }

    private async void Done_Clicked(object sender, EventArgs e)
    {
        try
        {
            string enteredName = entryCompanyName.Text?.Trim();
            string selectedType = pickType.SelectedItem as string;
            TimeSpan selectedStartTime = startTimePicker.Time;
            TimeSpan selectedEndTime = endTimePicker.Time;

            TimeSpan? startTimeFromServer = null;
            TimeSpan? endTimeFromServer = null;

            if (!string.IsNullOrEmpty(_companyInfo.working_hours))
            {
                var parts = _companyInfo.working_hours.Split(" - ");
                if (parts.Length == 2 &&
                    DateTime.TryParse(parts[0], out var startTime) &&
                    DateTime.TryParse(parts[1], out var endTime))
                {
                    startTimeFromServer = startTime.TimeOfDay;
                    endTimeFromServer = endTime.TimeOfDay;
                }
            }

            bool isSame = enteredName == _companyInfo.company_name?.Trim() &&
            AppService.Get<AppControl>().BusinessTypeList[selectedType].ToUpper() == _companyInfo.business_type.ToUpper() &&
            startTimeFromServer.HasValue && endTimeFromServer.HasValue &&
            selectedStartTime == startTimeFromServer.Value &&
            selectedEndTime == endTimeFromServer.Value;

            if (!isSame || isNewImageSelected)
            { 
                string formattedWorkingHours = $"{DateTime.Today.Add(startTimePicker.Time):hh:mm tt} - {DateTime.Today.Add(endTimePicker.Time):hh:mm tt}";
                var additionalData = new Dictionary<string, string>
                {
                    { "company_id", _companyInfo.company_id.ToString() },
                    { "company_name", enteredName },
                    { "business_type", AppService.Get<AppControl>().BusinessTypeList[selectedType] },
                    { "working_hours",  formattedWorkingHours},
                };

                if (!isNewImageSelected)
                {
                    imageStream = null;
                }

                loading.ShowLoading = true;
                var apiService = AppService.Get<CompanyApiService>();
                Response response = await apiService.UpdateCompanyProfileInfo(imageStream, additionalData);

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    await AlertService.ShowAlertAsync(AppResource.UpdateProduct, AppResource.Success);
                    await Shell.Current.GoToAsync("..", true);
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
                }
            }
            else
            {
                await Shell.Current.GoToAsync("..", true);
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

    private async void PhoneNumber_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(grdPhoneNumber);
        await AppNavigatorService.NavigateTo(nameof(PhoneNumberChangePage));
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

    private void BusinessType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedDisplay = pickType.SelectedItem as string;
        if (selectedDisplay != null && AppService.Get<AppControl>().BusinessTypeList.TryGetValue(selectedDisplay, out var backendValue))
        {
             
        }
    }

    private async void ButtonLogOut_Clicked(object sender, EventArgs e)
    {
        bool answer = await Application.Current.MainPage.DisplayAlert(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes,AppResource.No
                            );
        if (!answer) return;

        AppService.Get<AppControl>().LogoutCompany();
    }
}