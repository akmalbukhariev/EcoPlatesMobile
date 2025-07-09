using System.ComponentModel;
using System.Globalization;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserCompanyPage : BasePage
{
	private UserCompanyPageViewModel viewModel;
    private AppControl appControl;

    public UserCompanyPage(UserCompanyPageViewModel vm, AppControl appControl)
    {
        InitializeComponent();

        this.viewModel = vm;
        this.appControl = appControl;

        viewModel.PropertyChanged += ViewModel_PropertyChanged;

        BindingContext = viewModel;
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

        await appControl.MoveUserHome();
    }

    private async void Like_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await AnimateElementScaleDown(element);
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
            await AnimateElementScaleDown(element);
        }

        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(viewModel.PhoneNumber);
    }

    private async void MakerLocation_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await AnimateElementScaleDown(element);
 
            var userLocation = new Location(appControl.UserInfo.location_latitude, appControl.UserInfo.location_longitude);
            var companyLocation = new Location(viewModel.Latitude, viewModel.Longitude);

            string uri = $"https://www.google.com/maps/dir/?api=1" +
             $"&origin={userLocation.Latitude.ToString(CultureInfo.InvariantCulture)},{userLocation.Longitude.ToString(CultureInfo.InvariantCulture)}" +
             $"&destination={companyLocation.Latitude.ToString(CultureInfo.InvariantCulture)},{companyLocation.Longitude.ToString(CultureInfo.InvariantCulture)}" +
             $"&travelmode=driving";

            await Launcher.Default.OpenAsync(uri);

            //await AppNavigatorService.NavigateTo($"{nameof(CompanyNavigatorPage)}?Latitude={viewModel.Latitude}&Longitude={viewModel.Longitude}");
        }
    }

    private async void Message_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await AnimateElementScaleDown(element);
        }
    }
}