using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;

namespace EcoPlatesMobile.Views;

public partial class PhoneNumberChangePage : BasePage
{
    private AppControl appControl;
    UserSessionService userSessionService;

    public PhoneNumberChangePage(UserSessionService userSessionService, AppControl appControl)
	{
		InitializeComponent();

        this.userSessionService = userSessionService;
        this.appControl = appControl;
         
        if (userSessionService.Role == UserRole.User)
        {
            lbPhone.Text = appControl.UserInfo.phone_number;
        }
        else if (userSessionService.Role == UserRole.Company)
        {
            lbPhone.Text = appControl.CompanyInfo.phone_number;
        }
    }

    private async void Next_Clicked(object sender, EventArgs e)
    {
        await AppNavigatorService.NavigateTo(nameof(PhoneNumberNewPage));
    }
}