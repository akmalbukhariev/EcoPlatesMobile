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
        
        if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            imCompany.HeightRequest = 270;
        }

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool isWifiOn = await appControl.CheckWifiOrNetwork();
        if (!isWifiOn) return;

        viewModel.IsLoading = true;
        await viewModel.LoadDataAsync();

        fullImage.Source = viewModel.CompanyImage;

        if (!appControl.IsLoggedIn)
        {
            viewModel.PhoneNumber = "************";
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    
        cts?.Cancel();
        cts = null;
    } 	
    
    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            if (sender is VisualElement element)
            {
                await AnimateElementScaleDown(element);
            }

            await Shell.Current.GoToAsync("..");
        });
    }

    private async void Home_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            if (sender is VisualElement element)
            {
                await AnimateElementScaleDown(element);
            }

            await appControl.MoveUserHome();
        });
    }

    private async void Like_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            if (sender is VisualElement element)
            {
                await AnimateElementScaleDown(element);
            }

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            await viewModel.CompanyLiked();
        });
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
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
    }

    private async void PhoneNumber_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
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
        });
    }

    private async void MakerLocation_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            if (sender is VisualElement element)
            {
                await AnimateElementScaleDown(element);

                if (!appControl.IsLoggedIn)
                {
                    await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
                    return;
                }

                cts = new CancellationTokenSource();
                viewModel.IsLoading = true;
                var userLocation = await locationService.GetCurrentLocationAsync(cts.Token);
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
        });
    }

    private async void Message_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
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
        });
    }

    private async void OnCompanyImage_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(imCompany);

            fullImage.TranslationY = -100;
            fullImage.Opacity = 0;
            fullImage.IsVisible = true;
            
            boxFullImage.IsVisible = true;
            boxFullImage.Opacity = 0.5;      // show dark overlay
            boxFullImage.InputTransparent = false; // start catching taps

            await Task.WhenAll(
                fullImage.TranslateTo(0, 0, 250, Easing.SinIn),
                fullImage.FadeTo(1, 250, Easing.SinIn)
            );
        });
    }

    private async void OnImage_Swiped(object sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Down)
        {
            // Move image down and fade it out
            await Task.WhenAll(
                fullImage.TranslateTo(0, 100, 250, Easing.SinOut),
                fullImage.FadeTo(0, 250, Easing.SinOut)
            );

            boxFullImage.IsVisible = false; // Optionally hide the container box
            boxFullImage.Opacity = 0;        // hide overlay
            boxFullImage.InputTransparent = true;  // let taps pass through again

            fullImage.IsVisible = false; // Hide the image after animation
            fullImage.Opacity = 1; // Reset opacity for future animations
            fullImage.TranslationY = 0; // Reset position for future animations
        }
        else if (e.Direction == SwipeDirection.Up)
        {
            // Move image up and fade it out
            await Task.WhenAll(
                fullImage.TranslateTo(0, -100, 250, Easing.SinOut),
                fullImage.FadeTo(0, 250, Easing.SinOut)
            );

            // Reset image visibility and properties after animation
            boxFullImage.IsVisible = false; // Optionally hide the container box
            boxFullImage.Opacity = 0;        // hide overlay
            boxFullImage.InputTransparent = true;  // let taps pass through again

            fullImage.IsVisible = false; // Hide the image after the animation
            fullImage.Opacity = 1; // Reset opacity
            fullImage.TranslationY = 0; // Reset position
        }
    }

    private async void OnImage_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            boxFullImage.IsVisible = false;
            boxFullImage.Opacity = 0;        // hide overlay
            boxFullImage.InputTransparent = true;  // let taps pass through again

            fullImage.IsVisible = false;
        });
    }
}