using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views;

public partial class PhoneNumberNewPage : BasePage
{
    private UserSessionService userSessionService;
    private UserApiService userApiService;
    private CompanyApiService companyApiService;
    private AppControl appControl;

    private const int MaxPhoneLength = 9;
    public PhoneNumberNewPage(UserSessionService userSessionService, UserApiService userApiService, CompanyApiService companyApiService, AppControl appControl)
    {
        InitializeComponent();

        this.userSessionService = userSessionService;
        this.userApiService = userApiService;
        this.companyApiService = companyApiService;
        this.appControl = appControl;

        if (userSessionService.Role == UserRole.User)
        {
            loading.ChangeColor(Colors.Green);
        }
        else
        {
            loading.ChangeColor(Color.FromArgb("#8338EC"));
        }

        this.Loaded += (s, e) =>
        {
            phoneEntry.Focus();
        };
    }

    private void PhoneEntry_TextChanged(object sender, TextChangedEventArgs e)
    { 
        var color = GetRoleColor(userSessionService.Role);

        bool isFilled = !string.IsNullOrWhiteSpace(phoneEntry.Text);
        btnContinue.IsEnabled = isFilled;
        btnContinue.BackgroundColor = isFilled ? color : Color.FromArgb("#e0e0e0");
        btnContinue.TextColor = isFilled ? Colors.White : Colors.Gray;

        string newText = e.NewTextValue ?? "";
        string digitsOnly = new string(newText.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length > MaxPhoneLength)
            digitsOnly = digitsOnly.Substring(0, MaxPhoneLength);
 
        if (phoneEntry.Text != digitsOnly)
        {
            phoneEntry.TextChanged -= PhoneEntry_TextChanged;
            phoneEntry.Text = digitsOnly;
            phoneEntry.CursorPosition = digitsOnly.Length;
            phoneEntry.TextChanged += PhoneEntry_TextChanged;
        }
    }

    private static Color GetRoleColor(UserRole role) => role switch
    {
        UserRole.User => Colors.Green,
        UserRole.Company => Color.FromArgb("#8338EC"),
        _ => Color.FromArgb("#0088cc")
    };

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(imBack);
        await AppNavigatorService.NavigateTo("..");
    }

    private async void Continue_Clicked(object sender, EventArgs e)
    {
        var rawPhone = phoneEntry.Text?.Trim();
        if (!appControl.IsValidUzbekistanPhoneNumber(rawPhone))
        {
            await AlertService.ShowAlertAsync(AppResource.PhoneNumber, AppResource.MessagePhoneNumberIsNotValid);
            return;
        }

        string phoneNumber = $"998{rawPhone}";

        Response response = null;

        loading.ShowLoading = true;

        try
        {
            #region Check phone number
            if (userSessionService.Role == UserRole.User)
                response = await userApiService.CheckUser(phoneNumber);
            else if (userSessionService.Role == UserRole.Company)
                response = await companyApiService.CheckUser(phoneNumber);

            if (response != null &&
                (response.resultCode == ApiResult.USER_EXIST.GetCodeToString() ||
                 response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString()))
            {
                loading.ShowLoading = false;
                await AlertService.ShowAlertAsync(AppResource.PhoneNumber, AppResource.MessagePhoneExist);
                return;
            }
            #endregion

            appControl.IsPhoneNumberRegisterPage = false;
            await AppNavigatorService.NavigateTo($"{nameof(AuthorizationPage)}?PhoneNumber={phoneNumber}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            loading.ShowLoading = false;
        }
    }
}