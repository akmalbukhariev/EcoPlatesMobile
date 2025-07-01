using EcoPlatesMobile.Resources.Languages;

namespace EcoPlatesMobile.Views;

[QueryProperty(nameof(IsUser), nameof(IsUser))]
public partial class AboutPage : BasePage
{
    private bool _isUser = false;
    public bool IsUser
    {
        get => _isUser;
        set
        {
            _isUser = value;
        }
    }

    public AboutPage()
    {
        InitializeComponent();

        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        lbTitle.Text = AppResource.AboutApp;

        if (_isUser)
        {
            headerGrid.BackgroundColor = Colors.Green;
            btnClose.BackgroundColor = Colors.Green;

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
            btnClose.BackgroundColor = Color.FromArgb("#8338EC");
            headerGrid.BackgroundColor = Color.FromArgb("#8338EC");

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
        await AppNavigatorService.NavigateTo("..");
    }

    private async void Close_Clicked(object sender, EventArgs e)
    {
        await AppNavigatorService.NavigateTo("..");
    }
}