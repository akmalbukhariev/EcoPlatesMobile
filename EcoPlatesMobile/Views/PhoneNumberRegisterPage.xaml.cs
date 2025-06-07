using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.User.Pages;

namespace EcoPlatesMobile.Views;

public partial class PhoneNumberRegisterPage : BasePage
{
    public PhoneNumberRegisterPage()
    {
        InitializeComponent();
    }

    private void OnOfferTapped(object sender, EventArgs e)
    {
        //string url = "https://your-link.com"; // Replace with your actual link
        //Launcher.OpenAsync(new Uri(url));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var session = AppService.Get<UserSessionService>();
        if (session == null) return;

        string phoneNumber = entryNumber.GetEntryText();

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            await AlertService.ShowAlertAsync("Phone Number", "Please enter the phone number!");
            return;
        }

        Response response = null;

        loadingView.ShowLoading = true;
        try
        {
            phoneNumber = $"998{phoneNumber}";
            if (session.Role == UserRole.Company)
            {
                var apiService = AppService.Get<CompanyApiService>();
                response = await apiService.CheckUser(phoneNumber);

                if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
                {
                    session.IsCompanyRegistrated = true;
                    await AppNavigatorService.NavigateTo($"{nameof(AuthorizationPage)}?PhoneNumber={phoneNumber}");
                }
                else if (response.resultCode == ApiResult.COMPANY_NOT_EXIST.GetCodeToString())
                {
                    session.IsCompanyRegistrated = false;
                    await AlertService.ShowAlertAsync("Phone Number Not Registered", "This phone number is not registered in our system. You will be redirected to the registration page.");
                    await AppNavigatorService.NavigateTo($"{nameof(AuthorizationPage)}?PhoneNumber={phoneNumber}");
                }
                else
                {
                    await AlertService.ShowAlertAsync("Error", response.resultMsg);
                }
            }
            else if (session.Role == UserRole.User)
            {
                var apiService = AppService.Get<UserApiService>();
                response = await apiService.CheckUser(phoneNumber);

                if (response.resultCode == ApiResult.USER_EXIST.GetCodeToString())
                {
                    await Navigation.PushAsync(new AuthorizationPage());
                }
                else if (response.resultCode == ApiResult.USER_NOT_EXIST.GetCodeToString())
                {
                    await Navigation.PushAsync(new UserRegistrationPage());
                }
                else
                {
                    await AlertService.ShowAlertAsync("Error", response.resultMsg);
                }
            }
        }
        finally
        {
            loadingView.ShowLoading = false;
        }
    }
}