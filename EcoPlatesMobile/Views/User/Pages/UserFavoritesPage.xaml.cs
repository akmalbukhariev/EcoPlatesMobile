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

        if (screenWidth <= 0) screenWidth = 400; // Fallback if width is 0

        Console.WriteLine($"Switching tab: {e}, Screen Width: {screenWidth}");

        if (e == tabSwitcher.Tab1_Title) // Show List
        {
            Console.WriteLine("Animating to List...");

            listCompany.IsVisible = true;
            listProduct.IsVisible = true; // Ensure Map is visible before animation

            // Move List into View, Move Map Out
            await Task.WhenAll(
                listCompany.TranslateTo(0, 0, animationDuration, Easing.CubicInOut),
                listProduct.TranslateTo(screenWidth, 0, animationDuration, Easing.CubicInOut) // Move Map out
            );

            Console.WriteLine($"Final Positions: List X={listCompany.TranslationX}, Map X={listProduct.TranslationX}");
        }
        else if (e == tabSwitcher.Tab2_Title) // Show Map
        {
            Console.WriteLine("Animating to Map...");

            listCompany.IsVisible = true;
            listProduct.IsVisible = true; // Make sure map is visible BEFORE animating

            // Reset TranslationX if necessary
            if (listProduct.TranslationX != screenWidth)
            {
                listProduct.TranslationX = screenWidth; // Ensure it's in the right position
            }

            await Task.WhenAll(
                listCompany.TranslateTo(-screenWidth, 0, animationDuration, Easing.CubicInOut), // Move List out
                listProduct.TranslateTo(0, 0, animationDuration, Easing.CubicInOut) // Bring Map in
            );

            Console.WriteLine($"Final Positions: List X={listCompany.TranslationX}, Map X={listProduct.TranslationX}");
        }
    }
}