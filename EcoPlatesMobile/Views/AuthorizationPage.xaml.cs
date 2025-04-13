using EcoPlatesMobile.Services.Api;

namespace EcoPlatesMobile.Views;

public partial class AuthorizationPage : BasePage
{
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

    private void Button_Clicked(object sender, EventArgs e)
    {
		var session = AppService.Get<UserSessionService>();
		if(session?.Role == UserRole.Company)
		{
			Application.Current.MainPage = new AppCompanyShell();
		}
		else if(session?.Role == UserRole.User)
		{
			Application.Current.MainPage = new AppUserShell();
		}
    }
}