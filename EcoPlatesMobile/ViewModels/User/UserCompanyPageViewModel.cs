using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.User.Pages;

namespace EcoPlatesMobile.ViewModels.User
{
    [QueryProperty(nameof(CompanyId), nameof(CompanyId))]
    public partial class UserCompanyPageViewModel : ObservableObject
    {
        [ObservableProperty] int companyId;
        [ObservableProperty] ObservableRangeCollection<ProductModel> products;

        [ObservableProperty] ImageSource companyImage;
        [ObservableProperty] string companyName;
        [ObservableProperty] string phoneNumber;
        [ObservableProperty] string workingTime;
        [ObservableProperty] string companyType;
        [ObservableProperty] ImageSource likeImage;

        bool likedCompany = false;
        [ObservableProperty] bool isLoading;
        [ObservableProperty] bool isRefreshing;
        [ObservableProperty] bool showLikedView;
        [ObservableProperty] bool isLikedViewLiked;
        public double Latitude;
        public double Longitude;

        private UserApiService userApiService;
        private AppControl appControl;

        public UserCompanyPageViewModel(UserApiService userApiService, AppControl appControl)
        {
            this.userApiService = userApiService;
            this.appControl = appControl;

            Products = new ObservableRangeCollection<ProductModel>();

            CompanyImage = "no_image.png";
            CompanyName = "Maker name";
            PhoneNumber = "1234567890";
            WorkingTime = "00 ~ 00";
            appControl.RebuildBusinessTypeList();
            ClickProductCommand = new Command<ProductModel>(ProductClicked);
        }

        private async void ProductClicked(ProductModel product)
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            await Shell.Current.GoToAsync(nameof(DetailProductPage), new Dictionary<string, object>
            {
                ["ProductModel"] = product
            });
        }

        public async Task CompanyLiked()
        { 
            likedCompany = !likedCompany;

            SaveOrUpdateBookmarksCompanyRequest request = new SaveOrUpdateBookmarksCompanyRequest()
            {
                user_id = appControl.UserInfo.user_id,
                company_id = CompanyId,
                deleted = likedCompany ? false : true,
            };
            
            Response response = await userApiService.UpdateUserBookmarkCompanyStatus(request);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                IsLikedViewLiked = likedCompany;
                ShowLikedView = true;
                LikeImage = likedCompany ? "liked.png" : "like.png";

                appControl.RefreshAllPages();
            }
        }

        public async Task LoadDataAsync()
        {
            Products.Clear();

            try
            {
                IsLoading = true;
 
                CompanyWithPosterListResponse response = await userApiService.GetCompanyWithPosters(CompanyId);

                if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
                {
                    CompanyImage = response.resultData.logo_url;
                    Latitude = response.resultData.location_latitude;
                    Longitude = response.resultData.location_longitude;
                    likedCompany = response.resultData.liked;
                    LikeImage = likedCompany ? "liked.png" : "like.png";
                    CompanyName = response.resultData.company_name;
                    PhoneNumber = response.resultData.phone_number;
                    WorkingTime = appControl.FormatWorkingHours(response.resultData.working_hours);
                    CompanyType = appControl.BusinessTypeList.FirstOrDefault(item => item.Value == response.resultData.business_type).Key;
                     
                    var items = response.resultData;

                    if (items == null || items.posterList.Count == 0)
                    {
                        return;
                    }

                    var productModels = items.posterList.Select(item => new ProductModel
                    {
                        PromotionId = item.poster_id ?? 0,
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        //Count = "2 qoldi",
                        ProductName = item.title,
                        NewPrice = $"{item.new_price:N0} so'm",
                        OldPrice = $"{item.old_price:N0} so'm",
                        Stars = $"{item.avg_rating}({item.total_reviews})",
                    }).ToList();

                    Products.AddRange(productModels);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadDataAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        public ICommand ClickProductCommand { get; }

        public IRelayCommand RefreshProductCommand => new RelayCommand(async () =>
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            await LoadDataAsync();
        });
    }
}
