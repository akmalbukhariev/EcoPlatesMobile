using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserMainPage : ContentPage
{
	private UserMainPageViewModel viewModel;

    public UserMainPage()
	{
		InitializeComponent();

		viewModel =  new UserMainPageViewModel();
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await viewModel.LoadPromotionAsync();
	}
}