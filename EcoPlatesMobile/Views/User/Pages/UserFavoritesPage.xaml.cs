using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserFavoritesPage : ContentPage
{
	private UserFavoritesViewModel viewModel;

    public UserFavoritesPage()
	{
		InitializeComponent();

		viewModel = new UserFavoritesViewModel();
		BindingContext = viewModel;

        tabSwitcher.TabChanged += TabSwitcher_TabChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await viewModel.LoadProductFavoritesAsync();
    }

    private async void TabSwitcher_TabChanged(object? sender, string e)
    {
        const int animationDuration = 400;
        double screenWidth = this.Width;

        if (screenWidth <= 0) screenWidth = 400;

        if (e == tabSwitcher.Tab1_Title)
        {
            listProduct.IsVisible = true;
            listCompany.IsVisible = true;
             
            await Task.WhenAll(
                listProduct.TranslateTo(0, 0, animationDuration, Easing.CubicInOut),
                listCompany.TranslateTo(screenWidth, 0, animationDuration, Easing.CubicInOut)
            );
        }
        else if (e == tabSwitcher.Tab2_Title)
        {
            listProduct.IsVisible = true;
            listCompany.IsVisible = true;
             
            if (listCompany.TranslationX != screenWidth)
            {
                listCompany.TranslationX = screenWidth;
            }

            await Task.WhenAll(
                listProduct.TranslateTo(-screenWidth, 0, animationDuration, Easing.CubicInOut),
                listCompany.TranslateTo(0, 0, animationDuration, Easing.CubicInOut)
            );
        }
    }

    private async void DeleteItem_Invoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is ProductModel product)
        {
            await viewModel.DeleteProduct(product);
        }
    }
}