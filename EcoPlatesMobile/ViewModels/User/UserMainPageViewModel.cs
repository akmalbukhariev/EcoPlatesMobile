using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Views.User.Pages;
using EcoPlatesMobile.Services;

namespace EcoPlatesMobile.ViewModels.User
{
    //https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm-community-toolkit-features
    //https://github.com/dotnet-architecture/eshop-mobile-client/blob/main/eShopOnContainers/Services/Navigation/MauiNavigationService.cs
    //https://github.com/dotnet/maui

    public partial class UserMainPageViewModel : ObservableObject, IViewModel
    {
        [ObservableProperty] private ObservableRangeCollection<ProductModel> products;
        [ObservableProperty] private ProductModel selectedProduct;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;
        [ObservableProperty] private bool showLikedView;
        [ObservableProperty] private bool isLikedViewLiked;

        public BusinessType BusinessType { get; set; }
        private int offset = 0;
        private const int PageSize = 4;
        private bool hasMoreItems = true;
       
        public UserMainPageViewModel()
        {
            Products = new ObservableRangeCollection<ProductModel>();

            LikeProductCommand = new Command<ProductModel>(ProductLiked);
            ClickProductCommand = new Command<ProductModel>(ProductClicked);
        } 
        
        private async void ProductLiked(ProductModel product)
        {
            product.Liked = !product.Liked;
            SaveOrUpdateBookmarksPromotionRequest request = new SaveOrUpdateBookmarksPromotionRequest()
            {
                user_id = AppService.Get<AppControl>().UserInfo.user_id,
                promotion_id = product.PromotionId,
                deleted = product.Liked ? false : true,
            };

            var apiService = AppService.Get<UserApiService>();
            Response response = await apiService.UpdateUserBookmarkPromotionStatus(request);
            
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                IsLikedViewLiked = product.Liked;
                ShowLikedView = true;
            }
        }

        private async void ProductClicked(ProductModel product)
        {
            await Shell.Current.GoToAsync(nameof(DetailProductPage), new Dictionary<string, object>
            {
                ["ProductModel"] = product
            });
        }

        public async Task LoadInitialAsync()
        {
            offset = 0;
            hasMoreItems = true;
            Products.Clear();

            try
            {
                IsLoading = true;

                var apiService = AppService.Get<UserApiService>();

                var userInfo = AppService.Get<AppControl>().UserInfo;

                PosterLocationRequest request = new PosterLocationRequest
                {
                    business_type = BusinessType.GetValue(),
                    offset = offset,
                    pageSize = PageSize,
                    radius_km = 10,
                    user_lat = userInfo.location_latitude,//37.518313,
                    user_lon = userInfo.location_longitude//126.724187
                };

                PosterListResponse response = await apiService.GetPostersByCurrentLocation(request);

                if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreItems = false;
                        return;
                    }

                    var productModels = items.Select(item => new ProductModel
                    {
                        PromotionId = item.poster_id ?? 0,
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        //Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = $"{item.new_price:N0} so'm",
                        OldPrice = $"{item.old_price:N0} so'm",
                        Stars = item.avg_rating.ToString(),
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();
                    Products.AddRange(productModels);

                    offset += PageSize;
                    if (productModels.Count < PageSize)
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
                Debug.WriteLine($"[ERROR] LoadInitialAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadPromotionAsync(bool isRefresh = false)
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
                    Products.Clear();
                }
                else
                {
                    IsLoading = true;
                }

                var apiService = AppService.Get<UserApiService>();

                var userInfo = AppService.Get<AppControl>().UserInfo;

                PosterLocationRequest request = new PosterLocationRequest
                {
                    business_type = BusinessType.GetValue(),
                    offset = offset,
                    pageSize = PageSize,
                    radius_km = 10,
                    user_lat = userInfo.location_latitude,//37.518313,
                    user_lon = userInfo.location_longitude//126.724187
                };

                PosterListResponse response = await apiService.GetPostersByCurrentLocation(request);

                if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreItems = false;
                        return;
                    }

                    var productModels = items.Select(item => new ProductModel
                    { 
                        PromotionId = item.poster_id ?? 0,
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        //Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = $"{item.new_price:N0} so'm",
                        OldPrice = $"{item.old_price:N0} so'm",
                        Stars = item.avg_rating.ToString(),
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    Products.AddRange(productModels);

                    offset += PageSize;
                    if (productModels.Count < PageSize)
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
         
        public ICommand LikeProductCommand { get; }
        public ICommand ClickProductCommand { get; }
        
        public IRelayCommand LoadMoreCommand => new RelayCommand( async () =>
        {
            if (IsLoading || IsRefreshing || !hasMoreItems)
                return;

            await LoadPromotionAsync();
        });

        public IRelayCommand RefreshCommand => new RelayCommand( async () =>
        {
            await LoadPromotionAsync(isRefresh: true);
        });
    } 
}