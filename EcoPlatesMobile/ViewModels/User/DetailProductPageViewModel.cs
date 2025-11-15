using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models.Chat;
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
        private string CompanyPhone = "";
        private string CompanyImageUrl = "";
        private string token_frb = "";

        bool likedProduct = false;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool showLikedView;
        [ObservableProperty] private bool isLikedViewLiked;
        public int CompanyId { get; set; } = 0;
        public double AvgRating = 0;

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

                SpecificPromotionWithCompanyInfoResponse response = appControl.IsLoggedIn ? await userApiService.GetSpecificPromotionWithCompanyInfo(request) :
                                                                                            await userApiService.GetSpecificPromotionWithCompanyInfoWithoutLogin(request);
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutUser();
                    return;
                }

                if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    PosterRatingCompanyInfo info = response.resultData;
                    CompanyId = (int)info.company_id;
                    ProductImage = info.image_url;
                    CompanyImage = info.logo_url;
                    CompanyImageUrl = info.logo_url;
                    CompanyName = info.company_name;
                    CompanyPhone = info.phone_number;
                    ProductName = info.title;
                    Rating = info.rating?.ToString();
                    WorkingTime = appControl.FormatWorkingHours(info.working_hours);
                    likedProduct = response.resultData.liked;
                    LikeImage = likedProduct ? "liked.png" : "like.png";
                    OldPrice = appControl.GetUzbCurrency(info.old_price);
                    NewPrice = appControl.GetUzbCurrency(info.new_price);
                    UserNeedToKnow = info.description;
                    token_frb = info.token_frb;
                    
                    if (info.ratingInfo != null)
                    {
                        Stars = $"{info.ratingInfo.avg_rating} ({info.ratingInfo.total_reviews})";
                        Rating = $"{info.ratingInfo.avg_rating} / 5.0";
                        AvgRating = info.ratingInfo.avg_rating;
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
            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            likedProduct = !likedProduct;
            SaveOrUpdateBookmarksPromotionRequest request = new SaveOrUpdateBookmarksPromotionRequest()
            {
                user_id = appControl.UserInfo.user_id,
                promotion_id = ProductModel.PromotionId,
                deleted = likedProduct ? false : true,
            };

            Response response = await userApiService.UpdateUserBookmarkPromotionStatus(request);
            bool isOk = await appControl.CheckUserState(response);
            if (!isOk)
            {
                await appControl.LogoutUser();
                return;
            }
            
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                IsLikedViewLiked = likedProduct;
                ShowLikedView = true;
                LikeImage = likedProduct ? "liked.png" : "like.png";

                appControl.RefreshAllPages();
            }
        }

        public ChatPageModel GetChatPageModel()
        {
            return new ChatPageModel()
            {
                ReceiverName = CompanyName,
                ReceiverPhone = CompanyPhone,
                ReceiverImage = CompanyImageUrl,
                ReceiverFrbToken = token_frb,
                SenderId = appControl.UserInfo.user_id,
                SenderName = appControl.UserInfo.first_name,
                SenderImage = appControl.UserInfo.profile_picture_url,
                SenderPhone = appControl.UserInfo.phone_number,
                SenderType = UserRole.User.ToString().ToUpper(),
                ReceiverId = CompanyId,
                ReceiverType = UserRole.Company.ToString().ToUpper(),
                PosterId = (int)ProductModel.PromotionId,
            };
        }
    }
}
