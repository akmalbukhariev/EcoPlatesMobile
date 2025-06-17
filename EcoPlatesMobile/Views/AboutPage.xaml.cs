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

        if (_isUser)
        {
            headerGrid.BackgroundColor = Colors.Green;
            btnClose.BackgroundColor = Colors.Green;

            roleIcon.Source = "about_user.png";
            roleTitle.Text = "For Clients";
            roleSubtitle.Text = "Discover discounted food items nearby";

            point1.Text = "• Browse a variety of discounted food offers";
            point2.Text = "• View available items on a map";
            point3.Text = "• Adjust the radius to find offers within your location";
            point4.Text = "• Receive notifications about new deals near you";

            finalMessage.Text = "Save money, reduce waste, and support your local community.";
        }
        else
        {
            btnClose.BackgroundColor = Color.FromArgb("#8338EC");
            headerGrid.BackgroundColor = Color.FromArgb("#8338EC");
            
            roleIcon.Source = "about_company.png";
            roleTitle.Text = "For Sellers";
            roleSubtitle.Text = "Turn near-expiry stock into sales instead of waste";

            point1.Text = "• Upload edible products before their expiration date.";
            point2.Text = "• Offer discounts to attract nearby customers.";
            point3.Text = "• Get notified when your product is about to expire";
            point4.Text = "• Reach new buyers who are actively looking for affordable items.";

            finalMessage.Text = "Sell smarter, reduce waste, and promote a sustainable business image.";
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