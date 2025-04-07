using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using EcoPlatesMobile.Core;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.ViewModels.User
{
    //https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm-community-toolkit-features
    //https://github.com/dotnet-architecture/eshop-mobile-client/blob/main/eShopOnContainers/Services/Navigation/MauiNavigationService.cs
    //https://github.com/dotnet/maui

    /*public partial class UserMainPageViewModel : ObservableObject
    {
        [ObservableProperty]private ObservableCollection<ProductModel> products;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;
        [ObservableProperty] private bool hasMoreItems = true;

        private int offset = 0;
        private const int PageSize = 15;
        
        public UserMainPageViewModel()
        {
            Products = new ObservableCollection<ProductModel>();
        }
 
        public async Task LoadPromotionAsync(bool isRefresh = false)
        {
            try
            {
                if (isRefresh)
                {
                    IsRefreshing = true;
                    IsLoading = false;

                    offset = 0;
                    Products.Clear();
                }
                else
                {
                    IsRefreshing = false;
                    IsLoading = true;
                }

                var apiService = AppService.Get<UserApiService>();

                PosterLocationRequest request = new PosterLocationRequest
                {
                    category = PosterType.CAKE.GetValue(),
                    offset = offset,
                    pageSize = PageSize,
                    radius_km = 10,
                    user_lat = 37.518313,
                    user_lon = 126.724187
                };

                PosterListResponse response = await apiService.GetPostersByCurrentLocation(request);

                if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    foreach (var item in items)
                    {
                        Products.Add(new ProductModel
                        {
                            ProductImage = "no_image.png",//string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                            Count = "2 qoldi",
                            ProductName = item.title,
                            ProductMakerName = item.company_name,
                            NewPrice = $"{item.new_price:N0} so'm",
                            OldPrice = $"{item.old_price:N0} so'm",
                            Stars = "3.1",
                            Distance = $"{item.distance_km:0.0} km"
                        });
                    }

                    offset += PageSize;
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
 
        public IRelayCommand LoadMoreCommand => new RelayCommand( async() =>
        {
            await LoadPromotionAsync();
        });
 
        public IRelayCommand RefreshCommand => new RelayCommand(async () =>
        {
            Products.Clear();
            await LoadPromotionAsync(isRefresh: true);
        });
    } */

    public partial class UserMainPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<ProductModel> products;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;

        private int offset = 0;
        private const int PageSize = 4;
        private bool hasMoreItems = true;

        public UserMainPageViewModel()
        {
            Products = new ObservableRangeCollection<ProductModel>();
            Products.Add(
                new ProductModel
                    {
                        ProductImage = "no_image.png",
                        Count = "2 qoldi",
                        ProductName = "test",
                        ProductMakerName = "test maker",
                        NewPrice = "1200 so'm",
                        OldPrice = "500 so'm",
                        Stars = "3.1",
                        Distance = "1 km"
                    }
            );
        }
        
        public async Task LoadPromotionAsync(bool isRefresh = false)
        {
            return;
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

                PosterLocationRequest request = new PosterLocationRequest
                {
                    category = PosterType.CAKE.GetValue(),
                    offset = offset,
                    pageSize = PageSize,
                    radius_km = 10,
                    user_lat = 37.518313,
                    user_lon = 126.724187
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
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = $"{item.new_price:N0} so'm",
                        OldPrice = $"{item.old_price:N0} so'm",
                        Stars = "3.1",
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

        public IRelayCommand LoadMoreCommand => new RelayCommand( async () =>
        {
            return;
            if (IsLoading || IsRefreshing || !hasMoreItems)
                return;

            await LoadPromotionAsync();
        });

        public IRelayCommand RefreshCommand => new RelayCommand( async () =>
        {
            return;
            await LoadPromotionAsync(isRefresh: true);
        });
    } 

}