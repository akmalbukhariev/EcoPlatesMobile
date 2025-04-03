using CommunityToolkit.Mvvm.ComponentModel;
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
        public ObservableCollection<ProductModel> products;

        [ObservableProperty]
        public bool isLoading;

        public UserMainPageViewModel()
        {
            products = new ObservableCollection<ProductModel>();
            /*
            products =
            [
                new ProductModel
                {
                    Image = "cake.png",
                    Count = "2 qoldi",
                    Name = "Tort rogalik",
                    ComapnyName = "Safia &amp; Bakery",
                    NewPrice = "15 000 so'm",
                    OldPrice = "25 000 so'm",
                    Stars = "3.1",
                    Distance = "1 km"
                },
                new ProductModel
                {
                    Image = "cake.png",
                    Count = "2 qoldi",
                    Name = "Tort rogalik",
                    ComapnyName = "Safia &amp; Bakery",
                    NewPrice = "15 000 so'm",
                    OldPrice = "25 000 so'm",
                    Stars = "3.1",
                    Distance = "1 km"
                },
                new ProductModel
                {
                    Image = "cake.png",
                    Count = "2 qoldi",
                    Name = "Tort rogalik",
                    ComapnyName = "Safia &amp; Bakery",
                    NewPrice = "15 000 so'm",
                    OldPrice = "25 000 so'm",
                    Stars = "3.1",
                    Distance = "1 km"
                },
                new ProductModel
                {
                    Image = "cake.png",
                    Count = "2 qoldi",
                    Name = "Tort rogalik",
                    ComapnyName = "Safia &amp; Bakery",
                    NewPrice = "15 000 so'm",
                    OldPrice = "25 000 so'm",
                    Stars = "3.1",
                    Distance = "1 km"
                },
                new ProductModel
                {
                    Image = "cake.png",
                    Count = "2 qoldi",
                    Name = "Tort rogalik",
                    ComapnyName = "Safia &amp; Bakery",
                    NewPrice = "15 000 so'm",
                    OldPrice = "25 000 so'm",
                    Stars = "3.1",
                    Distance = "1 km"
                },
                new ProductModel
                {
                    Image = "cake.png",
                    Count = "2 qoldi",
                    Name = "Tort rogalik",
                    ComapnyName = "Safia &amp; Bakery",
                    NewPrice = "15 000 so'm",
                    OldPrice = "25 000 so'm",
                    Stars = "3.1",
                    Distance = "1 km"
                },
                new ProductModel
                {
                    Image = "cake.png",
                    Count = "2 qoldi",
                    Name = "Tort rogalik",
                    ComapnyName = "Safia &amp; Bakery",
                    NewPrice = "15 000 so'm",
                    OldPrice = "25 000 so'm",
                    Stars = "3.1",
                    Distance = "1 km"
                },
                new ProductModel
                {
                    Image = "cake.png",
                    Count = "2 qoldi",
                    Name = "Tort rogalik",
                    ComapnyName = "Safia &amp; Bakery",
                    NewPrice = "15 000 so'm",
                    OldPrice = "25 000 so'm",
                    Stars = "3.1",
                    Distance = "1 km"
                },
            ];
            */
        }
        
        public async Task LoadPromotionAsync()
        {
            if(IsLoading) return;
            
            try
            {
                IsLoading = true;
                var apiService = AppService.Get<UserApiService>(); 
                PosterLocationRequest request = new PosterLocationRequest
                {
                    category = PosterType.CAKE.GetValue(),
                    offset = 0,
                    pageSize = 10,
                    radius_km = 10,
                    user_lat = 37.518313,
                    user_lon = 126.724187
                };
                PosterListResponse response = await apiService.GetPostersByCurrentLocation(request);
                if(response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    Products.Clear();
                    foreach (var item in response.resultData)
                    {
                        Products.Add(new ProductModel
                        {
                            Image = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                            Count = "2 qoldi", // You can customize this if you have stock data
                            Name = item.title,
                            ComapnyName = item.company_name,
                            NewPrice = $"{item.new_price:N0} so'm",
                            OldPrice = $"{item.old_price:N0} so'm",
                            Stars = "3.1", // Replace with actual rating if available
                            Distance = $"{item.distance_km:0.0} km"
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
