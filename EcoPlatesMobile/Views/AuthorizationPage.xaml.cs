using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

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
		Application.Current.MainPage = new ContentPage
		{
			BackgroundColor = Colors.White,
			Content = new ActivityIndicator
			{
				IsRunning = true,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			}
		};

		await Task.Delay(100);

		var session = AppService.Get<UserSessionService>();
		if (session?.Role == UserRole.Company)
		{
			var apiService = AppService.Get<CompanyApiService>();
			LoginRequest request = new LoginRequest()
			{
				phone_number = _phoneNumber
			};

			LoginCompanyResponse response = await apiService.Login(request);
			if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
			{
				AppService.Get<AppControl>().CompanyInfo = response.resultData;
				Application.Current.MainPage = new AppCompanyShell();
			}
		}
		else if (session?.Role == UserRole.User)
		{
			var apiService = AppService.Get<UserApiService>();

			Application.Current.MainPage = new AppUserShell();
		}
	}
}