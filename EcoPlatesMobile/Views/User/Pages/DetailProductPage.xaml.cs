namespace EcoPlatesMobile.Views.User.Pages;

using System.ComponentModel;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.ViewModels.User;
using EcoPlatesMobile.Views.Chat;
using Microsoft.Maui.Controls;

public partial class DetailProductPage : BasePage
{
    private readonly DetailProductPageViewModel viewModel;
    
    public DetailProductPage(DetailProductPageViewModel vm)
    {
        InitializeComponent();

        viewModel = vm;

        viewModel.PropertyChanged += ViewModel_PropertyChanged;
        reviewView.EventReviewClick += ReviewView_EventReviewClick;

        reviewView.EventCloseClick += ReviewView_EventCloseClick;

        if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            imProduct.HeightRequest = 270;
        }

        BindingContext = viewModel;
    }

    private void ReviewView_EventCloseClick()
    {
        blockingOverlay.IsVisible = false;
        reviewView.IsVisible = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool isWifiOn = await appControl.CheckWifiOrNetwork();
        if (!isWifiOn) return;

        await viewModel.LoadDataAsync();

        fullImage.Source = viewModel.ProductImage;
        UpdateStars();
        appControl.NotificationData = null;

        similarProductView.Category = viewModel.ProductModel.Category;
        similarProductView.SetService(viewModel.userApiService, viewModel.appControl);

        await similarProductView.LoadInitialAsync();
    }

    private void UpdateStars()
    {
        starContainer.Children.Clear();

        for (int i = 1; i <= 5; i++)
        {
            var image = new Image
            {
                HeightRequest = 20,
                WidthRequest = 20,
                Margin = new Thickness(1)
            };

            if (i <= Math.Floor(viewModel.AvgRating))
            {
                image.Source = "star1.png";
            }
            else
            {
                image.Source = "star_gray.png";
            }

            starContainer.Children.Add(image);
        }

        var label = new Label
        {
            Text = viewModel.Stars,
            TextColor = Colors.Black,
            FontFamily = "RobotoVar",
            Margin = new Thickness(5, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center
        };
        starContainer.Children.Add(label);
    }

    private async void Home_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
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
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            if (sender is VisualElement element)
            {
                await AnimateElementScaleDown(element);
            }

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            if (!appControl.IsLoggedIn)
            {
                await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
                return;
            }

            await viewModel.ProductLiked();
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

    private async void Company_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await gridCompany.ScaleTo(0.95, 100, Easing.CubicOut);
            await gridCompany.ScaleTo(1.0, 100, Easing.CubicIn);

            await AppNavigatorService.NavigateTo($"{nameof(UserCompanyPage)}?CompanyId={viewModel.CompanyId}");
        });
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
    }

    private async void Star_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            if (!appControl.IsLoggedIn)
            {
                await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
                return;
            }

            blockingOverlay.IsVisible = true;

            reviewView.Scale = 0.5;
            reviewView.Opacity = 0;
            reviewView.IsVisible = true;

            await Task.WhenAll(
                reviewView.FadeTo(1, 200),
                reviewView.ScaleTo(1, 200, Easing.BounceOut)
            );

        });
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

    private async void OnProductImage_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(imProduct);

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
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            boxFullImage.IsVisible = false;
            boxFullImage.Opacity = 0;        // hide overlay
            boxFullImage.InputTransparent = true;  // let taps pass through again
            
            fullImage.IsVisible = false;
        });
    }

    private async void Overlay_Tapped(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            ReviewView_EventCloseClick();
        });
    }

    private async void Message_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(sender as HorizontalStackLayout);

            if (!appControl.IsLoggedIn)
            {
                await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
                return;
            }

            viewModel.IsLoading = true;
            bool isWifiOn = await appControl.CheckWifiOrNetworkFor(true);
            viewModel.IsLoading = false;
            
		    if (!isWifiOn) return;	

            await Shell.Current.GoToAsync(nameof(ChattingPage), new Dictionary<string, object>
            {
                ["ChatPageModel"] = viewModel.GetChatPageModel()
            });
        });
    }

    void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        // Tune this threshold to match your desired behavior:
        // Usually around (image height - header height).
        const double threshold = 120;

        // 0 -> transparent, 1 -> solid
        var t = e.ScrollY / threshold;
        if (t < 0) t = 0;
        if (t > 1) t = 1;

        headerBg.Opacity = t;
    }
    
    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            if (sender is VisualElement element)
            {
                await AnimateElementScaleDown(element);
            }

            await Back();
        });
    }
}