 
using EcoPlatesMobile.Services;
using System.Threading.Tasks;

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
        bool isLoggedIn = appStoreService.Get(AppKeys.IsLoggedIn, false);
        string phoneNumber = appStoreService.Get(AppKeys.PhoneNumber, "");

        loading.ShowLoading = true;
        if (isLoggedIn)
        {
            string strColor = "#8338EC";
            if (role == UserRole.Company)
            {
                userSessionService.SetUser(UserRole.Company);
                await appControl.LoginCompany(phoneNumber);
            }
            else if (role == UserRole.User)
            {
                strColor = "#007100";
                userSessionService.SetUser(UserRole.User);
                await appControl.LoginUser(phoneNumber);
            }

            AppService.Get<IStatusBarService>().SetStatusBarColor(strColor, false);
        }
        loading.ShowLoading = false;
    }
     
    private async void Button_Clicked(object sender, EventArgs e)
    {
        string strColor = "#8338EC";
        if (sender == btnComapny)
        {
            userSessionService.SetUser(UserRole.Company);
        }
        else if (sender == btnUser)
        {
            strColor = "#007100";
            userSessionService.SetUser(UserRole.User);
        }

        AppService.Get<IStatusBarService>().SetStatusBarColor(strColor, false);
        await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
    }
}