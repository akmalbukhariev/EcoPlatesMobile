using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.ViewModels.User
{
    [QueryProperty(nameof(ProductModel), nameof(ProductModel))]
    public partial class DetailProductPageViewModel : ObservableObject
    {
        [ObservableProperty] ProductModel productModel;
        [ObservableProperty] private ImageSource prodyctImage;
        [ObservableProperty] private ImageSource companyImage;
        [ObservableProperty] private string companyName;
        [ObservableProperty] private string productName;
        [ObservableProperty] private string stars;
        [ObservableProperty] private string rating;
        [ObservableProperty] private string workingTime;
        [ObservableProperty] private string oldPrice;
        [ObservableProperty] private string newPrice;
        [ObservableProperty] private string userNeedToKnow = ". The store will provide packaging for your food, but we encourage you to bring your own bag to carry it home in.";
        [ObservableProperty] private string averageRating;
        [ObservableProperty] private string totalRating;
        [ObservableProperty] private string feedbackType;
        [ObservableProperty] private int feedbackCount;
        [ObservableProperty] private List<PosterTypeInfo> typeInfoList;
        [ObservableProperty] private ImageSource likeImage;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool showLikedView;
        [ObservableProperty] private bool isLikedViewLiked;
        public int CompanyId { get; set; } = 0;

        public DetailProductPageViewModel()
        {
            LikeImage = "like.png";
        }

        public async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                var apiService = AppService.Get<UserApiService>();

                PosterGetFeedbackRequest request = new PosterGetFeedbackRequest()
                {
                    promotion_id = (int)ProductModel.PromotionId,
                    user_id = 16
                };

                SpecificPromotionWithCompanyInfoResponse response = await apiService.GetSpecificPromotionWithCompanyInfo(request);

                if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    PosterRatingCompanyInfo info = response.resultData;
                    CompanyId = (int)info.company_id;
                    ProdyctImage = info.image_url;
                    CompanyImage = info.logo_url;
                    CompanyName = info.company_name;
                    ProductName = info.title;
                    Rating = info.rating?.ToString();
                    WorkingTime = info.working_hours;
                    LikeImage = info.liked ? "liked.png" : "like.png";
                    OldPrice = info.old_price.ToString() + " so'm";
                    NewPrice = info.new_price.ToString() + " so'm";
                    UserNeedToKnow = info.user_need_to_know;

                    if (info.ratingInfo != null)
                    {
                        Stars = $"{info.ratingInfo.avg_rating} ({info.ratingInfo.total_reviews})";
                        Rating = $"{info.ratingInfo.avg_rating} / 5.0";
                    }
                    else
                    {
                        Stars = "0.0";
                        Rating = "0.0 / 5.0";
                    }

                    if (info.typeInfo != null)
                    {
                        TypeInfoList = info.typeInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadDataAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task ProductLiked()
        {
            ProductModel.Liked = !ProductModel.Liked;
            SaveOrUpdateBookmarksPromotionRequest request = new SaveOrUpdateBookmarksPromotionRequest()
            {
                user_id = 16,
                promotion_id = ProductModel.PromotionId,
                deleted = ProductModel.Liked ? false : true,
            };

            var apiService = AppService.Get<UserApiService>();
            Response response = await apiService.UpdateUserBookmarkPromotionStatus(request);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                IsLikedViewLiked = ProductModel.Liked;
                ShowLikedView = true;
                LikeImage = ProductModel.Liked ? "liked.png" : "like.png";
            }
        }
    }
}
