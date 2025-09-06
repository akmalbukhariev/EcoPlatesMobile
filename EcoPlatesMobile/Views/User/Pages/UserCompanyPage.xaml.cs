using System.ComponentModel;
using System.Globalization;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserCompanyPage : BasePage
{
    private UserCompanyPageViewModel viewModel;
    private AppControl appControl;
    private LocationService locationService;
    
    public UserCompanyPage(UserCompanyPageViewModel vm, AppControl appControl, LocationService locationService)
    {
        InitializeComponent();

        this.viewModel = vm;
        this.appControl = appControl;
        this.locationService = locationService;

        viewModel.PropertyChanged += ViewModel_PropertyChanged;

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;

        viewModel.IsLoading = true;    
        await viewModel.LoadDataAsync();
        
        fullImage.Source = viewModel.CompanyImage;
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

        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;
        
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

        if (!appControl.IsLoggedIn)
        {
            await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage)); 
            return;
        }

        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(viewModel.PhoneNumber);
    }

    private async void MakerLocation_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await AnimateElementScaleDown(element);

            if (!appControl.IsLoggedIn)
            {
                await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage)); 
                return;
            }

            viewModel.IsLoading = true;
            var userLocation = await locationService.GetCurrentLocationAsync();
            if (userLocation == null)
            {
                viewModel.IsLoading = false;
                return;
            }
            viewModel.IsLoading = false;

            //var userLocation = new Location(appControl.UserInfo.location_latitude, appControl.UserInfo.location_longitude);
            var companyLocation = new Location(viewModel.Latitude, viewModel.Longitude);

            string uri = $"https://www.google.com/maps/dir/?api=1" +
             $"&origin={userLocation.Latitude.ToString(CultureInfo.InvariantCulture)},{userLocation.Longitude.ToString(CultureInfo.InvariantCulture)}" +
             $"&destination={companyLocation.Latitude.ToString(CultureInfo.InvariantCulture)},{companyLocation.Longitude.ToString(CultureInfo.InvariantCulture)}" +
             $"&travelmode=driving";

            await Launcher.Default.OpenAsync(uri);
        }
    }

    private async void Message_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await AnimateElementScaleDown(element);
        }

        if (!appControl.IsLoggedIn)
        {
            await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage)); 
            return;
        }
    }

    private async void OnCompanyImage_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(imCompany);

        fullImage.TranslationY = -100;
        fullImage.Opacity = 0;
        fullImage.IsVisible = true;
        boxFullImage.IsVisible = true;

        await Task.WhenAll(
            fullImage.TranslateTo(0, 0, 250, Easing.SinIn),
            fullImage.FadeTo(1, 250, Easing.SinIn)
        );
    }

    private async void OnImage_Swiped(object sender, SwipedEventArgs e)
    {
        await Task.WhenAll(
            fullImage.TranslateTo(0, -100, 250, Easing.SinOut),
            fullImage.FadeTo(0, 250, Easing.SinOut)
        );

        boxFullImage.IsVisible = false;
        fullImage.IsVisible = false;
        fullImage.Opacity = 1;
        fullImage.TranslationY = 0;
    }

    private void OnImage_Tapped(object sender, TappedEventArgs e)
    {
        boxFullImage.IsVisible = false;
        fullImage.IsVisible = false;
    }
}