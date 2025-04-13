using EcoPlatesMobile.Models.User;
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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await viewModel.LoadCompaniesAsync();
    }

    private async void TabSwitcher_TabChanged(object? sender, string e)
    {
        const int animationDuration = 400;
        double screenWidth = this.Width;

        if (screenWidth <= 0) screenWidth = 400;

        if (e == tabSwitcher.Tab1_Title)
        {
            list.IsVisible = true;
            map.IsVisible = true;

            await Task.WhenAll(
                list.TranslateTo(0, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(screenWidth, 0, animationDuration, Easing.CubicInOut)
            );
        }
        else if (e == tabSwitcher.Tab2_Title)
        {
            list.IsVisible = true;
            map.IsVisible = true;

            if (map.TranslationX != screenWidth)
            {
                map.TranslationX = screenWidth;
            }

            await Task.WhenAll(
                list.TranslateTo(-screenWidth, 0, animationDuration, Easing.CubicInOut),
                map.TranslateTo(0, 0, animationDuration, Easing.CubicInOut)
            );
        }
    }

    private async void DeleteItem_Invoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is CompanyModel company)
        {
            await viewModel.DeleteProduct(company);
        }
    }
}