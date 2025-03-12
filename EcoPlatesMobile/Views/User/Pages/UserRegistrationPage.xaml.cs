namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserRegistrationPage : ContentPage
{
	public UserRegistrationPage()
	{
		InitializeComponent();
	}

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}