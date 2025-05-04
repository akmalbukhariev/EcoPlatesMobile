namespace EcoPlatesMobile.Views.User.Pages;

using System.ComponentModel;
using EcoPlatesMobile.ViewModels.User;
using Microsoft.Maui.Controls; 

public partial class DetailProductPage : ContentPage
{
    private DetailProductPageViewModel viewModel;
    public DetailProductPage()
	{
		InitializeComponent();
        viewModel = new DetailProductPageViewModel();
        BindingContext = viewModel;

        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);

        viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await viewModel.LoadDataAsync();
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleTo(1.3, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }

        await Shell.Current.GoToAsync("..");
    }

    private void Share_Tapped(object sender, TappedEventArgs e)
    {
         
    }

    private async void Like_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleTo(1.3, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }
        await viewModel.ProductLiked();
    }

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShowLikedView) && viewModel.ShowLikedView)
        {
            await likeView.DisplayAsAnimation();
            viewModel.ShowLikedView = false;
        }
    }

    private async void Company_Tapped(object sender, TappedEventArgs e)
    {
        await gridCompany.ScaleTo(0.95, 100, Easing.CubicOut);
        await gridCompany.ScaleTo(1.0, 100, Easing.CubicIn);

        await Shell.Current.GoToAsync($"{nameof(UserCompanyPage)}?CompanyId={viewModel.CompanyId}");
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}