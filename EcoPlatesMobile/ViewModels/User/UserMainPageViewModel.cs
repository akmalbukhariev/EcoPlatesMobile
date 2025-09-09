using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Utilities;
using System.Diagnostics;
using System.Windows.Input;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Views.User.Pages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Views.User.Components;
using EcoPlatesMobile.Views;

namespace EcoPlatesMobile.ViewModels.User
{
    //https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm-community-toolkit-features
    //https://github.com/dotnet-architecture/eshop-mobile-client/blob/main/eShopOnContainers/Services/Navigation/MauiNavigationService.cs
    //https://github.com/dotnet/maui

    public partial class UserMainPageViewModel : ObservableObject
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
       
        private UserApiService userApiService;
        private AppControl appControl;

        public UserMainPageViewModel(UserApiService userApiService, AppControl appControl)
        {
            this.userApiService = userApiService;
            this.appControl = appControl;

            Products = new ObservableRangeCollection<ProductModel>();

            LikeProductCommand = new Command<ProductModel>(ProductLiked);
            ClickProductCommand = new Command<ProductModel>(ProductClicked);

            LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        } 
        
        private async void ProductLiked(ProductModel product)
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            if (!appControl.IsLoggedIn)
            {
                await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage)); 
                return;
            }

            var original = product.Liked;
            product.Liked = !original;
            SaveOrUpdateBookmarksPromotionRequest request = new SaveOrUpdateBookmarksPromotionRequest()
            {
                user_id = appControl.UserInfo.user_id,
                promotion_id = product.PromotionId,
                deleted = product.Liked ? false : true,
            };

            Response response = await userApiService.UpdateUserBookmarkPromotionStatus(request);
            
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                IsLikedViewLiked = product.Liked;
                ShowLikedView = true;

                appControl.RefreshAllPages();
            }
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

        public async Task LoadInitialAsync()
        {
            offset = 0;
            hasMoreItems = true;
            Products.Clear();
            
            try
            {
                IsLoading = true;
                
                PosterLocationRequest request = new PosterLocationRequest
                {
                    business_type = BusinessType == BusinessType.OTHER ? null : BusinessType.GetValue(),
                    offset = offset,
                    pageSize = PageSize,
                    radius_km = appControl.IsLoggedIn ? appControl.UserInfo.radius_km : Constants.MaxRadius,
                    user_lat = appControl.UserInfo.location_latitude,
                    user_lon = appControl.UserInfo.location_longitude
                };

                PosterListResponse response = appControl.IsLoggedIn ? await userApiService.GetPostersByCurrentLocation(request) :
                                                                       await userApiService.GetPostersByCurrentLocationWithoutLogin(request);

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
                        NewPrice = appControl.GetUzbCurrency(item.new_price),
                        OldPrice = appControl.GetUzbCurrency(item.old_price),
                        Stars = item.avg_rating.ToString(),
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    ProductView.BeginNewAnimationCycle();
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
 
                PosterLocationRequest request = new PosterLocationRequest
                {
                    business_type = BusinessType == BusinessType.OTHER ? null : BusinessType.GetValue(),
                    offset = offset,
                    pageSize = PageSize,
                    radius_km = appControl.IsLoggedIn ? appControl.UserInfo.radius_km : Constants.MaxRadius,
                    user_lat = appControl.UserInfo.location_latitude,
                    user_lon = appControl.UserInfo.location_longitude
                };

                PosterListResponse response = appControl.IsLoggedIn ? await userApiService.GetPostersByCurrentLocation(request) :
                                                                       await userApiService.GetPostersByCurrentLocationWithoutLogin(request);

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
                        NewPrice = appControl.GetUzbCurrency(item.new_price),
                        OldPrice = appControl.GetUzbCurrency(item.old_price),
                        Stars = item.avg_rating.ToString(),
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    ProductView.BeginNewAnimationCycle();
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

        public IAsyncRelayCommand LoadMoreCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }

        private async Task LoadMoreAsync()
        {
            if (IsLoading || IsRefreshing || !hasMoreItems) return;
            bool online = await appControl.CheckWifi(); if (!online) return;
            await LoadPromotionAsync();
        }

        private async Task RefreshAsync()
        {
            bool online = await appControl.CheckWifi(); if (!online) return;
            await LoadPromotionAsync(isRefresh: true);
        }

        /*
        public IRelayCommand LoadMoreCommand => new RelayCommand( async () =>
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            if (IsLoading || IsRefreshing || !hasMoreItems)
                return;

            await LoadPromotionAsync();
        });

        public IRelayCommand RefreshCommand => new RelayCommand( async () =>
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            await LoadPromotionAsync(isRefresh: true);
        });
        */
    } 
}