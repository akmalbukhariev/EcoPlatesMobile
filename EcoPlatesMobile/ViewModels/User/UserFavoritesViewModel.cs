using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EcoPlatesMobile.ViewModels.User
{
    public partial class UserFavoritesViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<ProductModel> products;
        [ObservableProperty] private ObservableRangeCollection<CompanyModel> companies;

        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshingProduct;
        [ObservableProperty] private bool isRefreshingCompany;

        private int offset = 0;
        private const int PageSize = 4;
        private bool hasMoreProductItems = true;

        public UserFavoritesViewModel()
        {
            products = new ObservableRangeCollection<ProductModel>();
            companies = new ObservableRangeCollection<CompanyModel>(); 
        }

        public async Task DeleteProduct(ProductModel product)
        {
            try
            {
                IsLoading = true;

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

        public async Task LoadProductFavoritesAsync(bool isRefresh = false)
        {
            if (IsLoading || (!hasMoreProductItems && !isRefresh))
                return;

            try
            {
                if (isRefresh)
                {
                    IsRefreshingProduct = true;
                    offset = 0;
                    hasMoreProductItems = true;
                    Products.Clear();
                }
                else
                {
                    IsLoading = true;
                }

                var apiService = AppService.Get<UserApiService>();

                PaginationWithLocationRequest request = new PaginationWithLocationRequest()
                {
                     user_lat = 37.518313,
                     user_lon = 126.724187,
                     offset = offset,
                     pageSize = PageSize
                };
                BookmarkPromotionListResponse response = await apiService.GetUserBookmarkPromotion(request);

                if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreProductItems = false;
                        return;
                    }

                    var productModels = items.Select(item => new ProductModel
                    {
                        PromotionId = item.poster_id ?? 0,
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = $"{item.new_price:N0} so'm",
                        OldPrice = $"{item.old_price:N0} so'm",
                        Stars = "3.1",
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    Products.AddRange(productModels);

                    offset += PageSize;
                    if (productModels.Count < PageSize)
                    {
                        hasMoreProductItems = false;
                    }
                }
                else
                {
                    hasMoreProductItems = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadProductFavoritesAsync: {ex.Message}");
            }
            finally
            {
                IsRefreshingProduct = false;
                IsLoading = false;
            }
        }

        public async Task LoadCompanyFavoritesAsync(bool isRefresh = false)
        {
            
        }
         
        public IRelayCommand RefreshProductCommand => new RelayCommand(async () =>
        {
            await LoadProductFavoritesAsync(isRefresh: true);
        });

        public IRelayCommand RefreshCompanyCommand => new RelayCommand(async () =>
        {
            await LoadCompanyFavoritesAsync(isRefresh: true);
        });

        public IRelayCommand LoadProductMoreCommand => new RelayCommand(async () =>
        {
            if (IsLoading || IsRefreshingProduct || !hasMoreProductItems)
                return;

            await LoadProductFavoritesAsync();
        });
    }
}
