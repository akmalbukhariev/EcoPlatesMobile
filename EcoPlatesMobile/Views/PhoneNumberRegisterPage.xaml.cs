using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.User.Pages;

namespace EcoPlatesMobile.Views;

public partial class PhoneNumberRegisterPage : BasePage
{
    private UserSessionService userSessionService;
    private CompanyApiService companyApiService;
    private UserApiService userApiService;

    public PhoneNumberRegisterPage(UserSessionService userSessionService, CompanyApiService companyApiService, UserApiService userApiService)
    {
        InitializeComponent();

        if (userSessionService.Role == UserRole.User)
        {
            header.HeaderBackground = btnNext.BackgroundColor = Colors.Green;
        }

        this.userSessionService = userSessionService;
        this.companyApiService = companyApiService;
        this.userApiService = userApiService;
    }

    private void OnOfferTapped(object sender, EventArgs e)
    {
        //string url = "https://your-link.com"; // Replace with your actual link
        //Launcher.OpenAsync(new Uri(url));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    { 
        string rawPhone = entryNumber.GetEntryText();
        if (string.IsNullOrWhiteSpace(rawPhone))
        {
            await AlertService.ShowAlertAsync(AppResource.PhoneNumber, AppResource.MessageEnterPhoneNumber);
            return;
        }

        if (userSessionService.Role == UserRole.User)
        {
            loading.ChangeColor(Colors.Green);
        }
        else
        {
            loading.ChangeColor(Color.FromArgb("#8338EC"));
        }

        string phoneNumber = $"998{rawPhone}";
        loading.ShowLoading = true;

        try
        {
            Response response = null;
            bool isRegistered = false;

            if (userSessionService.Role == UserRole.Company)
            {
                response = await companyApiService.CheckUser(phoneNumber);

                if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
                {
                    userSessionService.IsCompanyRegistrated = true;
                    isRegistered = true;
                }
                else if (response.resultCode == ApiResult.COMPANY_NOT_EXIST.GetCodeToString())
                {
                    userSessionService.IsCompanyRegistrated = false;
                }
            }
            else if (userSessionService.Role == UserRole.User)
            {
                response = await userApiService.CheckUser(phoneNumber);

                if (response.resultCode == ApiResult.USER_EXIST.GetCodeToString())
                {
                    userSessionService.IsUserRegistrated = true;
                    isRegistered = true;
                }
                else if (response.resultCode == ApiResult.USER_NOT_EXIST.GetCodeToString())
                {
                    userSessionService.IsUserRegistrated = false;
                }
            }
            
            if (isRegistered)
            {
                await AppNavigatorService.NavigateTo($"{nameof(AuthorizationPage)}?PhoneNumber={phoneNumber}");
            }
            else if (response != null)
            {
                if (response.resultCode == ApiResult.COMPANY_NOT_EXIST.GetCodeToString() ||
                    response.resultCode == ApiResult.USER_NOT_EXIST.GetCodeToString())
                {
                    await AlertService.ShowAlertAsync(AppResource.PhoneNumberNotRegistered, AppResource.MessageEnterPhoneNumberNotRegistered);
                    await AppNavigatorService.NavigateTo($"{nameof(AuthorizationPage)}?PhoneNumber={phoneNumber}");
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
                }
            }
            else
            {
                await AlertService.ShowAlertAsync(AppResource.Error, AppResource.ErrorUnexpected);
            }
        }
        finally
        {
            loading.ShowLoading = false;
        }
    }
}