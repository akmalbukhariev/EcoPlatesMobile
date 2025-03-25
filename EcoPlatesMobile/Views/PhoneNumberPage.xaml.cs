using EcoPlatesMobile.Core;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.User.Pages;

namespace EcoPlatesMobile.Views;

public partial class PhoneNumberPage : BasePage
{
	public PhoneNumberPage()
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
        
        LoginRequest request = new LoginRequest()
        {
            phone_number = entryNumber.GetEntryText()
        };

        if (session.Role == UserRole.Company)
        {
            var apiService = AppService.Get<CompanyApiService>();
            if (apiService != null)
            {
                LoginCompanyResponse response = await apiService.Login(request);
                if (response.resultCode == ApiResult.USER_EXIST.GetCodeToString())
                {
                    await Navigation.PushAsync(new AuthorizationPage());
                }
                else if (response.resultCode == ApiResult.USER_NOT_EXIST.GetCodeToString())
                {
                    await Navigation.PushAsync(new UserRegistrationPage());
                }
            }
        }
        else if (session.Role == UserRole.User)
        {
            var apiService = AppService.Get<UserApiService>();
            if (apiService != null)
            {
                LoginUserResponse response = await apiService.Login(request);
                if (response.resultCode == ApiResult.USER_EXIST.GetCodeToString())
                {
                    await Navigation.PushAsync(new AuthorizationPage());
                }
                else if (response.resultCode == ApiResult.USER_NOT_EXIST.GetCodeToString())
                {
                    await Navigation.PushAsync(new CompanyRegistrationPage());
                }
            }
        }

    }
}