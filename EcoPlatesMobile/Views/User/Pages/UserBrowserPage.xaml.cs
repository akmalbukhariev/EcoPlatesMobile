using EcoPlatesMobile.ViewModels.User;
using Microsoft.Maui.Controls.Shapes;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserBrowserPage : ContentPage
{
	private UserBrowserPageViewModel viewModel;
	public UserBrowserPage()
	{
		InitializeComponent();
		viewModel = new UserBrowserPageViewModel();

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

            list.IsVisible = true;
            map.IsVisible = true; // Ensure Map is visible before animation

            // Move List into View, Move Map Out
            await Task.WhenAll(
                list.TranslateTo(0, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(screenWidth, 0, animationDuration, Easing.CubicInOut) // Move Map out
            );

            Console.WriteLine($"Final Positions: List X={list.TranslationX}, Map X={map.TranslationX}");
        }
        else if (e == tabSwitcher.Tab2_Title) // Show Map
        {
            Console.WriteLine("Animating to Map...");

            list.IsVisible = true;
            map.IsVisible = true; // Make sure map is visible BEFORE animating

            // Reset TranslationX if necessary
            if (map.TranslationX != screenWidth)
            {
                map.TranslationX = screenWidth; // Ensure it's in the right position
            }

            await Task.WhenAll(
                list.TranslateTo(-screenWidth, 0, animationDuration, Easing.CubicInOut), // Move List out
                map.TranslateTo(0, 0, animationDuration, Easing.CubicInOut) // Bring Map in
            );

            Console.WriteLine($"Final Positions: List X={list.TranslationX}, Map X={map.TranslationX}");
        }
    }
}