 
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using System.Threading.Tasks;
using Plugin.Firebase.CloudMessaging;

namespace EcoPlatesMobile.Views;

public partial class LoginPage : BasePage
{ 
    private AppStoreService appStoreService;
    private AppControl appControl;
    private UserSessionService userSessionService;
  
    public LoginPage(AppStoreService appStoreService, AppControl appControl, UserSessionService userSessionService)
	{
		InitializeComponent();

        this.appStoreService = appStoreService;
        this.appControl = appControl;
        this.userSessionService = userSessionService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        UserRole role = appStoreService.Get(AppKeys.UserRole, UserRole.None);
        appControl.IsLoggedIn = appStoreService.Get(AppKeys.IsLoggedIn, false);
        string phoneNumber = appStoreService.Get(AppKeys.PhoneNumber, "");

        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;

        loading.ShowLoading = true;
        
        Color color = Colors.White;
        if (appControl.IsLoggedIn)
        { 
            if (role == UserRole.Company)
            {
                color = Constants.COLOR_COMPANY;
                userSessionService.SetUser(UserRole.Company);
                await appControl.LoginCompany(phoneNumber);
            }
            else if (role == UserRole.User)
            {
                color = Constants.COLOR_USER;
                userSessionService.SetUser(UserRole.User);
                await appControl.LoginUser(phoneNumber);
            }
        }

        AppService.Get<IStatusBarService>().SetStatusBarColor(color.ToArgbHex(), false);

        loading.ShowLoading = false;
    }
     
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            Color color = Constants.COLOR_COMPANY;
            UserRole userRole = UserRole.User;
            if (sender == btnComapny)
            {
                userSessionService.SetUser(UserRole.Company);
                userRole = UserRole.Company;
            }
            else if (sender == btnUser)
            {
                color = Constants.COLOR_USER;
                userSessionService.SetUser(UserRole.User);
            }

            AppService.Get<IStatusBarService>().SetStatusBarColor(color.ToArgbHex(), false);

            if (!appControl.IsLoggedIn && userRole == UserRole.User)
            {
                Application.Current.MainPage = new AppUserShell();
            }
            else
            {
                await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
            }
        });
    }
}