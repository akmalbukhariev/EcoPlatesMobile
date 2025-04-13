
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.ViewModels.User
{
    public partial class UserBrowserPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<CompanyModel> companies;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;
        [ObservableProperty] private bool showLikedView;
        [ObservableProperty] private bool isLikedViewLiked;

        private int offset = 0;
        private const int PageSize = 4;
        private bool hasMoreItems = true;

        public UserBrowserPageViewModel()
        {
            Companies = new ObservableRangeCollection<CompanyModel>();
            LikeCompanyCommand = new Command<CompanyModel>(CompanyLiked);
        }

        private async void CompanyLiked(CompanyModel product)
        {
            product.Liked = !product.Liked;

            /*
            SaveOrUpdateBookmarksPromotionRequest request = new SaveOrUpdateBookmarksPromotionRequest()
            {
                user_id = 16,
                promotion_id = product.PromotionId,
                deleted = product.Liked,
            };

            var apiService = AppService.Get<UserApiService>();
            Response response = await apiService.UpdateUserBookmarkPromotionStatus(request);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                IsLikedViewLiked = product.Liked;
                ShowLikedView = true;
            }
            */
        }

        public async Task LoadCompaniesAsync(bool isRefresh = false)
        {
            if (IsLoading || (!hasMoreItems && !isRefresh))
                return;

            try
            {
                if (isRefresh)
                {
                    IsRefreshing = true;
                    offset = 0;
                    hasMoreItems = true;
                    Companies.Clear();
                }
                else
                {
                    IsLoading = true;
                }
                 
                var apiService = AppService.Get<UserApiService>();

                CompanyLocationRequest request = new CompanyLocationRequest()
                {
                    radius_km = 2,
                    user_lat = 37.518313,
                    user_lon = 126.724187,
                    offset = offset,
                    pageSize = PageSize,
                };

                CompanyListResponse response = await apiService.GetCompaniesByCurrentLocation(request);

                if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreItems = false;
                        return;
                    }

                    var companyModels = items.Select(item => new CompanyModel
                    {
                        CompanyId = item.company_id ?? 0,
                        CompanyImage = string.IsNullOrWhiteSpace(item.logo_url) ? "no_image.png" : item.logo_url,
                        CompanyName = item.company_name,
                        WorkingTime = item.working_hours,
                        Stars = "3.1",
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    Companies.AddRange(companyModels);

                    offset += PageSize;
                    if (companyModels.Count < PageSize)
                    {
                        hasMoreItems = false;
                    }
                }
                else
                {
                    hasMoreItems = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadPromotionAsync: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
                IsLoading = false;
            }
        }

        public async Task DeleteProduct(CompanyModel product)
        {
            try
            {
                IsLoading = true;
                /*
                SaveOrUpdateBookmarksPromotionRequest request = new SaveOrUpdateBookmarksPromotionRequest()
                {
                    user_id = 16,
                    promotion_id = product.PromotionId,
                    deleted = false,
                };

                var apiService = AppService.Get<UserApiService>();
                Response response = await apiService.UpdateUserBookmarkPromotionStatus(request);
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    products.Remove(product);
                }
                */
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] DeleteProduct: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public ICommand LikeCompanyCommand { get; }

        public IRelayCommand LoadMoreCommand => new RelayCommand(async () =>
        {
            if (IsLoading || IsRefreshing || !hasMoreItems)
                return;

            await LoadCompaniesAsync();
        });

        public IRelayCommand RefreshCommand => new RelayCommand(async () =>
        {
            await LoadCompaniesAsync(isRefresh: true);
        });
    }
}