using System.ComponentModel;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;
using Microsoft.Maui.Controls.Shapes;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserBrowserPage : BasePage
{
    private UserBrowserPageViewModel viewModel;
	public UserBrowserPage()
	{
		InitializeComponent();
		viewModel = new UserBrowserPageViewModel();

		BindingContext = viewModel;

        tabSwitcher.TabChanged += TabSwitcher_TabChanged;
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetTabBarIsVisible(this, true);

        AppService.Get<AppControl>().ShowCompanyMoreInfo = false;
        await viewModel.LoadInitialAsync();
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

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShowLikedView) && viewModel.ShowLikedView)
        {
            await likeView.DisplayAsAnimation();
            viewModel.ShowLikedView = false;
        }
    }
}