using System.Text.RegularExpressions;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views;

public partial class PhoneNumberNewPage : BasePage
{
    private UserSessionService sessionService;
    private UserApiService userApiService;
    private CompanyApiService companyApiService;
    private AppControl appControl;
    public PhoneNumberNewPage(UserSessionService sessionService, UserApiService userApiService, CompanyApiService companyApiService, AppControl appControl)
    {
        InitializeComponent();

        this.sessionService = sessionService;
        this.userApiService = userApiService;
        this.companyApiService = companyApiService;
        this.appControl = appControl;

        this.Loaded += (s, e) =>
        {
            phoneEntry.Focus();
        };

        phoneEntry.TextChanged += (s, e) =>
        {
            Color btnColor = Color.FromArgb("#0088cc");
            if (sessionService.Role == UserRole.User)
            {
                btnColor = Colors.Green;
            }
            else if (sessionService.Role == UserRole.Company)
            {
                btnColor = Color.FromArgb("#8338EC");
            }

            btnContinue.IsEnabled = !string.IsNullOrWhiteSpace(phoneEntry.Text);
            btnContinue.BackgroundColor = btnContinue.IsEnabled ? btnColor : Color.FromArgb("#e0e0e0");
            btnContinue.TextColor = btnContinue.IsEnabled ? Colors.White : Colors.Gray;
        };
    }

    private void PhoneEntry_Completed(object sender, EventArgs e)
    {
        // Logic when user presses "done" or enter
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(imBack);

        await AppNavigatorService.NavigateTo("..");
    }

    private async void Continue_Clicked(object sender, EventArgs e)
    {
        string phoneNumber = phoneEntry.Text.Trim();
        if (!IsValidUzbekistanPhoneNumber(phoneNumber))
        {
            await AlertService.ShowAlertAsync(AppResource.PhoneNumber, AppResource.MessagePhoneNumberIsNotValid);
            return;
        }

        Response response = null;
        if (sessionService.Role == UserRole.User)
        {
            response = await userApiService.CheckUser(phoneNumber);
        }
        else if (sessionService.Role == UserRole.Company)
        {
            response = await companyApiService.CheckUser(phoneNumber);
        }

        if (response?.resultCode == ApiResult.USER_EXIST.GetCodeToString() || response?.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
        {
            await AlertService.ShowAlertAsync(AppResource.PhoneNumber, "The phone numbe is already exist!");
            return;
        }

        if (sessionService.Role == UserRole.User)
        {
            response = await userApiService.UpdateUserPhoneNumber();
            if (response?.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                appControl.LogoutUser();
            }
            else
            {
                await AlertService.ShowAlertAsync(AppResource.Error, response?.resultMsg);
            }
        }
        else if (sessionService.Role == UserRole.Company)
        {
            //response = await companyApiService.CheckUser(phoneNumber);
        }

        
    }
    
    public static bool IsValidUzbekistanPhoneNumber(string phoneNumber)
    { 
        return Regex.IsMatch(phoneNumber, Constants.PHONE_PATTERN);
    }
}