using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserFavoritesPage : BasePage
{
	private UserFavoritesViewModel viewModel;
    private AppControl appControl;

    public UserFavoritesPage(UserFavoritesViewModel vm, AppControl appControl)
	{
		InitializeComponent();

        this.viewModel = vm;
        this.appControl = appControl;
  
        tabSwitcher.TabChanged += TabSwitcher_TabChanged;

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(this, true); 

        if (appControl.RefreshFavoriteProduct)
        {
            await viewModel.LoadInitialProductAsync();
            appControl.RefreshFavoriteProduct = false;
        }

        if (appControl.RefreshFavoriteCompany)
        {
            await viewModel.LoadInitialCompanyAsync();
            appControl.RefreshFavoriteCompany = false;
        }
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

    private async void DeleteProduct_Invoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is ProductModel product)
        {
            await viewModel.DeleteProduct(product);
        }
    }

    private async void DeleteCompany_Invoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is CompanyModel company)
        {
            await viewModel.DeleteCompany(company);
        }
    }
}