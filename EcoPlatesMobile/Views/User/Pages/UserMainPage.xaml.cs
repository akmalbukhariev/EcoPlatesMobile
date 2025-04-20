using System.ComponentModel;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserMainPage : ContentPage
{
	private UserMainPageViewModel viewModel;

    public UserMainPage()
	{
		InitializeComponent();

		viewModel =  new UserMainPageViewModel(Navigation);
		BindingContext = viewModel;

        viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override async void OnAppearing()
	{
		base.OnAppearing();

        await viewModel.LoadInitialAsync();
    }

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShowLikedView) && viewModel.ShowLikedView)
        {
            await likeView.DisplayAsAnimation();
            viewModel.ShowLikedView = false;
        }
    }
}