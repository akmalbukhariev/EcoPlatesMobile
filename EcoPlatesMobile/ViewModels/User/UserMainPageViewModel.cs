using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public partial class UserMainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ProductModel> products;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isRefreshing;

        private int offset = 0;
        private const int PageSize = 6;
        
        public UserMainPageViewModel()
        {
            Products = new ObservableCollection<ProductModel>();
        }

        /// <summary>
        /// Load product posters with pagination and refresh handling.
        /// </summary>
        /// <param name="isRefresh">If true, skip the IsLoading check to allow refresh to proceed.</param>
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
                            ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
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

        /// <summary>
        /// Command to load more items as user scrolls down.
        /// </summary>
        public IRelayCommand LoadMoreCommand => new RelayCommand(async () =>
        {
             

            await LoadPromotionAsync();
        });

        /// <summary>
        /// Command to refresh the list when user pulls from top.
        /// </summary>
        public IRelayCommand RefreshCommand => new RelayCommand(async () =>
        {
            Products.Clear();
            await LoadPromotionAsync(isRefresh: true);
        });
    }
}