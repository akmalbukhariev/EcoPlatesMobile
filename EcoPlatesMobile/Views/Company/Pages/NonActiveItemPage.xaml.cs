using EcoPlatesMobile.ViewModels.Company;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class NonActiveItemPage : ContentPage
{
	private NonActiveProductPageViewModel viewModel;
	public NonActiveItemPage()
	{
		InitializeComponent();
		
		viewModel = new NonActiveProductPageViewModel();
		BindingContext = viewModel;
	}
}