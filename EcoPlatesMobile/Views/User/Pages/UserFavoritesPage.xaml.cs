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

    private async void TabSwitcher_TabChanged(object? sender, string e)
    {
        const int animationDuration = 400;
        double screenWidth = this.Width;

        if (screenWidth <= 0) screenWidth = 400;

        if (e == tabSwitcher.Tab1_Title)
        {
            listCompany.IsVisible = true;
            listProduct.IsVisible = true;

            await Task.WhenAll(
                listCompany.TranslateTo(0, 0, animationDuration, Easing.CubicInOut),
                listProduct.TranslateTo(screenWidth, 0, animationDuration, Easing.CubicInOut)
            );
        }
        else if (e == tabSwitcher.Tab2_Title)
        {
            listCompany.IsVisible = true;
            listProduct.IsVisible = true;

            if (listProduct.TranslationX != screenWidth)
            {
                listProduct.TranslationX = screenWidth;
            }

            await Task.WhenAll(
                listCompany.TranslateTo(-screenWidth, 0, animationDuration, Easing.CubicInOut),
                listProduct.TranslateTo(0, 0, animationDuration, Easing.CubicInOut)
            );
        }
    }
}