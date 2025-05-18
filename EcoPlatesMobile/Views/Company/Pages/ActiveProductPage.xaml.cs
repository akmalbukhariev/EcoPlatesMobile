using EcoPlatesMobile.ViewModels.Company;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class ActiveProductPage : ContentPage
{
	private ActiveProductPageViewModel viewModel;
	public ActiveProductPage()
	{
		InitializeComponent();

		viewModel = new ActiveProductPageViewModel();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
         
        await viewModel.LoadInitialAsync();
    }
}