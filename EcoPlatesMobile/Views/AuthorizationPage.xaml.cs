using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Company.Pages;

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

	public AuthorizationPage()
	{
		InitializeComponent();

		// number1.NextEntry = number2;
		// number2.PreviousEntry = number1;
		// number2.NextEntry = number3;
		// number3.PreviousEntry = number2;
		// number3.NextEntry = number4;
		// number4.PreviousEntry = number3;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		lbTitle.Text = $"Iltimos, {_phoneNumber} telefon raqamiga SMS orqali yuborilgan tasdiqlash kodini kiriting";
		//Use phone verification API
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		var session = AppService.Get<UserSessionService>();
		if (session?.Role == UserRole.Company)
		{
			if (session.IsCompanyRegistrated)
			{
				await AppService.Get<AppControl>().LoginCompany(_phoneNumber);
			}
			else
			{
				await AppNavigatorService.NavigateTo($"{nameof(CompanyRegistrationPage)}?PhoneNumber={_phoneNumber}");
			}
		}
		else if (session?.Role == UserRole.User)
		{
			var apiService = AppService.Get<UserApiService>();

			Application.Current.MainPage = new AppUserShell();
		}
	}
}