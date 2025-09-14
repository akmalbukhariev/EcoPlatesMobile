using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views;
 
public partial class AboutPage : BasePage
{
    private UserSessionService userSessionService;

    public AboutPage(UserSessionService userSessionService)
    {
        InitializeComponent();
        this.userSessionService = userSessionService;

        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);
        btnClose.Text = AppResource.Close;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        lbTitle.Text = AppResource.AboutApp;

        if (userSessionService.Role == UserRole.User)
        {
            headerGrid.BackgroundColor = Constants.COLOR_USER;
            btnClose.BackgroundColor = Constants.COLOR_USER;

            roleIcon.Source = "about_user.png";
            roleTitle.Text = AppResource.ForClient;
            roleSubtitle.Text = AppResource.RoleSubtitleClient;

            point1.Text = $"• {AppResource.AboutAppPoint1Client}";
            point2.Text = $"• {AppResource.AboutAppPoint2Client}";
            point3.Text = $"• {AppResource.AboutAppPoint3Client}";
            point4.Text = $"• {AppResource.AboutAppPoint4Client}";

            finalMessage.Text = AppResource.FinalMessageClient;
        }
        else
        {
            btnClose.BackgroundColor = Constants.COLOR_COMPANY;
            headerGrid.BackgroundColor = Constants.COLOR_COMPANY;

            roleIcon.Source = "about_company.png";
            roleTitle.Text = AppResource.ForSeller;
            roleSubtitle.Text = AppResource.RoleSubtitleSeller;

            point1.Text = $"• {AppResource.AboutAppPoint1Seller}";
            point2.Text = $"• {AppResource.AboutAppPoint2Seller}";
            point3.Text = $"• {AppResource.AboutAppPoint3Seller}";
            point4.Text = $"• {AppResource.AboutAppPoint4Seller}";

            finalMessage.Text = AppResource.FinalMessageSeller;
        }
    }

    private async void Close_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AppNavigatorService.NavigateTo("..");
        });
    }

    private async void Close_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AppNavigatorService.NavigateTo("..");
        });
    }
}