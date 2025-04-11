using System.ComponentModel;
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

        viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShowLikedView) && viewModel.ShowLikedView)
        {
            await likeView.DisplayAsAnimation();
            viewModel.ShowLikedView = false;
        }
    }

    protected override async void OnAppearing()
	{
		base.OnAppearing();

		await viewModel.LoadPromotionAsync();
    }
}