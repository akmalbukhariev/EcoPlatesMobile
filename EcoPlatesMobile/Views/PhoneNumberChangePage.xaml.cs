using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

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
            header.HeaderBackground = Constants.COLOR_USER;
            lbPhone.Text = appControl.UserInfo.phone_number;
        }
        else if (userSessionService.Role == UserRole.Company)
        {
            header.HeaderBackground = Constants.COLOR_COMPANY;
            lbPhone.Text = appControl.CompanyInfo.phone_number;
        }
    }

    private async void Next_Clicked(object sender, EventArgs e)
    {
        await AppNavigatorService.NavigateTo(nameof(PhoneNumberNewPage));
    }
}