namespace EcoPlatesMobile.Views.User.Pages;

using System.ComponentModel;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.ViewModels.User;
using Microsoft.Maui.Controls; 

public partial class DetailProductPage : BasePage
{
    private DetailProductPageViewModel viewModel;
    public DetailProductPage()
	{
		InitializeComponent();
        viewModel = new DetailProductPageViewModel();
        BindingContext = viewModel;

        viewModel.PropertyChanged += ViewModel_PropertyChanged;
        reviewView.EventReviewClick += ReviewView_EventReviewClick;

        reviewView.EventCloseClick += ReviewView_EventCloseClick;
    }

    private void ReviewView_EventCloseClick()
    {
        blockingOverlay.IsVisible = false;
        reviewView.IsVisible = false;
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
            await AnimateElementScaleDown(element);
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void Home_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        { 
            await AnimateElementScaleDown(element);
        }

        Application.Current.MainPage = new AppUserShell();
    }

    private async void Like_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await AnimateElementScaleDown(element);
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

    private async void Star_Tapped(object sender, TappedEventArgs e)
    {
        blockingOverlay.IsVisible = true;

        reviewView.Scale = 0.5;
        reviewView.Opacity = 0;
        reviewView.IsVisible = true;

        await Task.WhenAll(
            reviewView.FadeTo(1, 200),
            reviewView.ScaleTo(1, 200, Easing.BounceOut)
        );
    }

    private async void ReviewView_EventReviewClick()
    {
        blockingOverlay.IsVisible = false;
        reviewView.IsVisible = false;
        await Shell.Current.GoToAsync(nameof(ReviewProductPage), true, new Dictionary<string, object>
        {
            ["ProductImage"] = viewModel.ProductImage.ToString().Trim(),
            ["ProductName"] = viewModel.ProductName,
            ["PromotionId"] = viewModel.ProductModel.PromotionId
        });
    }

    private void Overlay_Tapped(object sender, EventArgs e)
    {
        ReviewView_EventCloseClick();
    }
}