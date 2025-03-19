using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserBrowserPage : ContentPage
{
	private UserBrowserPageViewModel viewModel;
	public UserBrowserPage()
	{
		InitializeComponent();
		viewModel = new UserBrowserPageViewModel();

		BindingContext = viewModel;
	}
}