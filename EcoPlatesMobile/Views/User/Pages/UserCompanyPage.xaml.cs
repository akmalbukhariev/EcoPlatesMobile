using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserCompanyPage : ContentPage
{
	private UserCompanyPageViewModel viewModel;

    public UserCompanyPage()
	{
		InitializeComponent();
		viewModel = new UserCompanyPageViewModel();

		BindingContext = viewModel;
	}

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        //await Navigation.PopAsync();
    }

    private void Share_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void Like_Tapped(object sender, TappedEventArgs e)
    {

    }
}