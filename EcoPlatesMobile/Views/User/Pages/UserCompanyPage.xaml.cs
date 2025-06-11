using System.ComponentModel;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserCompanyPage : BasePage
{
	private UserCompanyPageViewModel viewModel;

    public UserCompanyPage()
	{
		InitializeComponent();
		viewModel = new UserCompanyPageViewModel();
          
        BindingContext = viewModel;
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

    private async void Home_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleTo(1.3, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }

        await Shell.Current.GoToAsync($"///{nameof(UserMainPage)}");
    }

    private async void Like_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleTo(1.3, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }

        await viewModel.CompanyLiked();
    }

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShowLikedView) && viewModel.ShowLikedView)
        {
            await likeView.DisplayAsAnimation();
            viewModel.ShowLikedView = false;
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void PhoneNumber_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleTo(1.3, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }

        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(viewModel.PhoneNumber);
    }

    private async void MakerLocation_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleTo(1.3, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }
         
    }

    private async void Message_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleTo(1.3, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }

    }
}