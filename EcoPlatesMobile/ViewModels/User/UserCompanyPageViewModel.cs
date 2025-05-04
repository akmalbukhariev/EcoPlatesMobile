using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

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

        [ObservableProperty] bool isLoading;
        [ObservableProperty] bool isRefreshing;
        [ObservableProperty] bool showLikedView;
        [ObservableProperty] bool isLikedViewLiked;

        public UserCompanyPageViewModel()
        {
            Products = new ObservableRangeCollection<ProductModel>();

            CompanyImage = "no_image.png";
            CompanyName = "Maker name";
            PhoneNumber = "1234567890";
            WorkingTime = "00 ~ 00";
        }

        public async Task LoadDataAsync()
        {
            Products.Clear();

            try
            {
                IsLoading = true;

                var apiService = AppService.Get<UserApiService>();
                CompanyWithPosterListResponse response = await apiService.getCompanyWithPosters(CompanyId);

                if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
                {
                    CompanyImage = response.resultData.logo_url;
                    LikeImage = response.resultData.liked ? "liked.png" : "like.png";
                    CompanyName = response.resultData.company_name;
                    PhoneNumber = response.resultData.phone_number;
                    WorkingTime = response.resultData.working_hours;
                    CompanyType = response.resultData.business_type;
                     
                    var items = response.resultData;

                    if (items == null || items.posterList.Count == 0)
                    {
                        return;
                    }

                    var productModels = items.posterList.Select(item => new ProductModel
                    {
                        PromotionId = item.poster_id ?? 0,
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        Count = "2 qoldi",
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

        public IRelayCommand RefreshProductCommand => new RelayCommand(async () =>
        {
            await LoadDataAsync();
        });
    }
}
