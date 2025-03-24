namespace EcoPlatesMobile.Views;

public partial class PhoneNumberPage : BasePage
{
	public PhoneNumberPage()
	{
		InitializeComponent(); 
    }

    private void OnOfferTapped(object sender, EventArgs e)
    {
        //string url = "https://your-link.com"; // Replace with your actual link
        //Launcher.OpenAsync(new Uri(url));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AuthorizationPage());
    }
}