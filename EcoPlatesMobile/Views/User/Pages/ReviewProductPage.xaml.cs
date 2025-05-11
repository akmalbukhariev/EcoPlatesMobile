using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.User.Pages;

[QueryProperty(nameof(ProductImage), nameof(ProductImage))]
[QueryProperty(nameof(ProductName), nameof(ProductName))]
[QueryProperty(nameof(PromotionId), nameof(PromotionId))]
public partial class ReviewProductPage : ContentPage
{
    private string _productImage;
    public string ProductImage
    {
        get => _productImage;
        set
        {
            _productImage = value;
            UpdateProductImage();
        }
    }

    private string _productName;
    public string ProductName
    {
        get => _productName;
        set
        {
            _productName = value;
            UpdateProductName();
        }
    }

    private long _promotionId;
    public long PromotionId
    {
        get => _promotionId;
        set
        {
            _promotionId = value;
        }
    }
     
    private int selectedRating = 0;
    private bool showedOption = false;

    public ReviewProductPage()
	{
		InitializeComponent();

        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);
    }
     
    private void UpdateProductName()
    {
        if (!string.IsNullOrEmpty(ProductName))
        {
            lbProductTitle.Text = ProductName;
        }
    }

    private void UpdateProductImage()
    {
        try
        {
            string imageUrl = _productImage.Replace("Uri: ", "").Trim();
            var uri = new Uri(imageUrl);
            imProduct.Source = ImageSource.FromUri(uri);
        }
        catch (Exception ex)
        {
            imProduct.Source = ImageSource.FromFile("no_image.png");
        }
    }

    private async void OnStarTapped(object sender, TappedEventArgs e)
    {
        selectedRating = int.Parse(e.Parameter.ToString());

        var stars = new[] { Star1, Star2, Star3, Star4, Star5 };
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].Source = i < selectedRating ? "star_yellow.png" : "star_gray.png";
        }

        if (!showedOption)
        {
            lbReviewTitle2.IsVisible = true;

            stackOption.IsVisible = true;
            stackOption.IsVisible = true;

            stackOption.Opacity = 0;
            stackOption.TranslationY = 20;

            await Task.WhenAll(
                stackOption.FadeTo(1, 300, Easing.SinIn),
                stackOption.TranslateTo(0, 0, 300, Easing.SinIn)
            );
            showedOption = true;
        }
    }

    private void OnOption1Tapped(object sender, TappedEventArgs e)
    {
        checkBox1.IsChecked = !checkBox1.IsChecked;
    }

    private void OnOption2Tapped(object sender, TappedEventArgs e)
    {
        checkBox2.IsChecked = !checkBox2.IsChecked;
    }

    private void OnOption3Tapped(object sender, TappedEventArgs e)
    {
        checkBox3.IsChecked = !checkBox3.IsChecked;
    }

    private async void Finished_Clicked(object sender, EventArgs e)
    {
        try
        {
            loading.IsRunning = true;
            RegisterPosterFeedbackRequest request = new RegisterPosterFeedbackRequest()
            {
                user_id = 16,
                promotion_id = PromotionId,
                feedback_type1 = checkBox1.IsChecked ? PosterFeedbackType.GREAT_VALUE.GetValue() : PosterFeedbackType.NONE.GetValue(),
                feedback_type2 = checkBox2.IsChecked ? PosterFeedbackType.DELICIOUS_FOOD.GetValue() : PosterFeedbackType.NONE.GetValue(),
                feedback_type3 = checkBox3.IsChecked ? PosterFeedbackType.GREAT_VALUE.GetValue() : PosterFeedbackType.NONE.GetValue(),
                rating = selectedRating,
            };

            var apiService = AppService.Get<UserApiService>();
            Response response = await apiService.RegisterPosterFeedBack(request);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                await AlertService.ShowAlertAsync("Rating", "Thank you.");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Finished_Clicked: {ex.ToString()}");
        }
        finally
        {
            loading.IsRunning = false;
        }
    }
}