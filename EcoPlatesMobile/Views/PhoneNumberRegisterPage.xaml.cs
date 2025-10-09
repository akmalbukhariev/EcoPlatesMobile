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
using System.Text.RegularExpressions;

namespace EcoPlatesMobile.Views;

public partial class PhoneNumberRegisterPage : BasePage
{
    private UserSessionService userSessionService;
    private CompanyApiService companyApiService;
    private UserApiService userApiService;
    private AppControl appControl;
    private IKeyboardHelper keyboardHelper;

    public PhoneNumberRegisterPage(UserSessionService userSessionService,
                                    CompanyApiService companyApiService,
                                    UserApiService userApiService,
                                    AppControl appControl,
                                    IKeyboardHelper keyboardHelper)
    {
        InitializeComponent();

        this.userSessionService = userSessionService;
        this.companyApiService = companyApiService;
        this.userApiService = userApiService;
        this.appControl = appControl;
        this.keyboardHelper = keyboardHelper;

        if (userSessionService.Role == UserRole.User)
        {
            header.HeaderBackground = btnNext.BackgroundColor = Constants.COLOR_USER;
            loading.ChangeColor(Constants.COLOR_USER);
        }
        else
        {
            loading.ChangeColor(Constants.COLOR_COMPANY);
        }
        
        this.Loaded += (s, e) =>
        {
            entryNumber.Focus();
        };
    }
    
    private void OnOfferTapped(object sender, EventArgs e)
    {
        //string url = "https://your-link.com"; // Replace with your actual link
        //Launcher.OpenAsync(new Uri(url));
    }

    private async void ButtonNext_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            //await entryNumber.UnFocus();
            keyboardHelper.HideKeyboard();

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            string rawPhone = entryNumber.GetEntryText();
            if (string.IsNullOrWhiteSpace(rawPhone))
            {
                await AlertService.ShowAlertAsync(AppResource.PhoneNumber, AppResource.MessageEnterPhoneNumber);
                return;
            }

            string phoneNumber = $"998{rawPhone}";

            if (!appControl.IsValidUzbekistanPhoneNumber(phoneNumber))
            {
                await AlertService.ShowAlertAsync(AppResource.PhoneNumber, AppResource.MessagePhoneNumberIsNotValid);
                return;
            }

            loading.ShowLoading = true;

            try
            {
                Response response = null;
                bool isRegistered = false;

                #region Check phone number
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
                    else if (response.resultCode == ApiResult.BLOCK_USER.GetCodeToString())
                    {
                        appControl.StrBlockUntill = response.resultMsg;
                        appControl.ResultCode = ApiResult.BLOCK_USER;
                        await AlertService.ShowAlertAsync(AppResource.Info, AppResource.MessageBlocked);
                        await AppNavigatorService.NavigateTo(nameof(BlockedPage));
                        return;
                    }
                    else if (response.resultCode == ApiResult.DELETE_USER.GetCodeToString())
                    {
                        appControl.StrBlockUntill = response.resultMsg;
                        appControl.ResultCode = ApiResult.DELETE_USER;
                        await AlertService.ShowAlertAsync(AppResource.Info, AppResource.MessageSoftDelete);
                        await AppNavigatorService.NavigateTo(nameof(BlockedPage));
                        return;
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
                    else if (response.resultCode == ApiResult.BLOCK_USER.GetCodeToString())
                    {
                        appControl.StrBlockUntill = response.resultMsg;
                        appControl.ResultCode = ApiResult.BLOCK_USER;
                        await AlertService.ShowAlertAsync(AppResource.Info, AppResource.MessageBlocked);
                        await AppNavigatorService.NavigateTo(nameof(BlockedPage));
                        return;
                    }
                    else if (response.resultCode == ApiResult.DELETE_USER.GetCodeToString())
                    {
                        appControl.StrBlockUntill = response.resultMsg;
                        appControl.ResultCode = ApiResult.DELETE_USER;
                        await AlertService.ShowAlertAsync(AppResource.Info, AppResource.MessageSoftDelete);
                        await AppNavigatorService.NavigateTo(nameof(BlockedPage));
                        return;
                    }
                }
                #endregion

                if (isRegistered)
                {
                    appControl.IsPhoneNumberRegisterPage = true;
                    await AppNavigatorService.NavigateTo($"{nameof(AuthorizationPage)}?PhoneNumber={phoneNumber}");
                }
                else if (response != null)
                {
                    if (response.resultCode == ApiResult.COMPANY_NOT_EXIST.GetCodeToString() ||
                        response.resultCode == ApiResult.USER_NOT_EXIST.GetCodeToString())
                    {
                        //await AlertService.ShowAlertAsync(AppResource.PhoneNumberNotRegistered, AppResource.MessageEnterPhoneNumberNotRegistered);

                        bool answer = await AlertService.ShowConfirmationAsync(
                                    AppResource.Confirm,
                                    AppResource.MessageEnterPhoneNumberNotRegistered,
                                    AppResource.Yes, AppResource.No);

                        if (!answer) return;

                        appControl.IsPhoneNumberRegisterPage = true;
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
        });
    }
}