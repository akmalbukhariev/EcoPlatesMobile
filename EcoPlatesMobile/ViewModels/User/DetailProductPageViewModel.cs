using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.ViewModels.User
{
    [QueryProperty(nameof(ProductModel), nameof(ProductModel))]
    public partial class DetailProductPageViewModel : ObservableObject
    {
        [ObservableProperty] ProductModel productModel;
        [ObservableProperty] private ImageSource productImage;
        [ObservableProperty] private ImageSource companyImage;
        [ObservableProperty] private string companyName;
        [ObservableProperty] private string productName;
        [ObservableProperty] private string stars;
        [ObservableProperty] private string rating;
        [ObservableProperty] private string workingTime;
        [ObservableProperty] private string oldPrice;
        [ObservableProperty] private string newPrice;
        [ObservableProperty] private string userNeedToKnow = ".........";
        [ObservableProperty] private string averageRating;
        [ObservableProperty] private string totalRating;
        [ObservableProperty] private string feedbackType;
        [ObservableProperty] private string highlightTitle;
        [ObservableProperty] private int feedbackCount;
        [ObservableProperty] private List<PosterTypeInfo> typeInfoList;
        [ObservableProperty] private ImageSource likeImage;

        bool likedProduct = false;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool showLikedView;
        [ObservableProperty] private bool isLikedViewLiked;
        public int CompanyId { get; set; } = 0;

        private UserApiService userApiService;
        private AppControl appControl;

        public DetailProductPageViewModel(UserApiService userApiService, AppControl appControl)
        {
            this.userApiService = userApiService;
            this.appControl = appControl;

            LikeImage = "like.png";
        }

        public async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                PosterGetFeedbackRequest request = new PosterGetFeedbackRequest()
                {
                    promotion_id = (int)ProductModel.PromotionId,
                    user_id = appControl.UserInfo.user_id
                };

                SpecificPromotionWithCompanyInfoResponse response = await userApiService.GetSpecificPromotionWithCompanyInfo(request);

                if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    PosterRatingCompanyInfo info = response.resultData;
                    CompanyId = (int)info.company_id;
                    ProductImage = info.image_url;
                    CompanyImage = info.logo_url;
                    CompanyName = info.company_name;
                    ProductName = info.title;
                    Rating = info.rating?.ToString();
                    WorkingTime = info.working_hours;
                    likedProduct = response.resultData.liked;
                    LikeImage = likedProduct ? "liked.png" : "like.png";
                    OldPrice = info.old_price.ToString() + " so'm";
                    NewPrice = info.new_price.ToString() + " so'm";
                    UserNeedToKnow = info.description;

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
                        HighlightTitle = $"{AppResource.Top} {TypeInfoList.Count} {AppResource.Highlights}";
                    }
                }
                else //if (response.resultCode == ApiResult.POSTER_NOT_EXIST.GetCodeToString())
                {
                    await AlertService.ShowAlertAsync(AppResource.Info, AppResource.MessageInfo);
                    await AppNavigatorService.NavigateTo("..");
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
            likedProduct = !likedProduct;
            SaveOrUpdateBookmarksPromotionRequest request = new SaveOrUpdateBookmarksPromotionRequest()
            {
                user_id = appControl.UserInfo.user_id,
                promotion_id = ProductModel.PromotionId,
                deleted = likedProduct ? false : true,
            };
            
            Response response = await userApiService.UpdateUserBookmarkPromotionStatus(request);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                IsLikedViewLiked = likedProduct;
                ShowLikedView = true;
                LikeImage = likedProduct ? "liked.png" : "like.png";

                appControl.RefreshAllPages();
            }
        }
    }
}
