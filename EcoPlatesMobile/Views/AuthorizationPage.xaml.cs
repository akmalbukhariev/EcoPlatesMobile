using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.User.Pages;

namespace EcoPlatesMobile.Views;

[QueryProperty(nameof(PhoneNumber), nameof(PhoneNumber))]
public partial class AuthorizationPage : BasePage
{
	private string _phoneNumber;
	public string PhoneNumber
	{
		set
		{
			_phoneNumber = value;
		}
	}

	private UserSessionService userSessionService;
	private AppControl appControl;

	public AuthorizationPage(UserSessionService userSessionService, AppControl appControl)
	{
		InitializeComponent();

		this.userSessionService = userSessionService;
		this.appControl = appControl;

        #region 
        // number1.NextEntry = number2;
        // number2.PreviousEntry = number1;
        // number2.NextEntry = number3;
        // number3.PreviousEntry = number2;
        // number3.NextEntry = number4;
        // number4.PreviousEntry = number3;
        #endregion

        if (userSessionService.Role == UserRole.User)
        {
            header.HeaderBackground = btnNext.BackgroundColor = Colors.Green;
        }
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();
		lbTitle.Text = $"{AppResource.Please}, {_phoneNumber} {AppResource.ConfirmTitle}";

		/*
		 * Use phone verification API
		 */
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		if (userSessionService.Role == UserRole.Company)
		{
			if (userSessionService.IsCompanyRegistrated)
			{
				await appControl.LoginCompany(_phoneNumber);
			}
			else
			{
				await AppNavigatorService.NavigateTo($"{nameof(CompanyRegistrationPage)}?PhoneNumber={_phoneNumber}");
			}
		}
		else if (userSessionService.Role == UserRole.User)
		{
            if (userSessionService.IsUserRegistrated)
            {
                await appControl.LoginUser(_phoneNumber);
            }
            else
            {
                await AppNavigatorService.NavigateTo($"{nameof(UserRegistrationPage)}?PhoneNumber={_phoneNumber}");
            }
        }
	}
}