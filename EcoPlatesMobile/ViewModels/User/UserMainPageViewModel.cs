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
            if(isLoading) return;

            try
            {
                isLoading = true;
                var apiService = AppService.Get<UserApiService>(); 
                PosterLocationRequest request = new PosterLocationRequest
                {
                    category = PosterType.CAKE,
                    offset = 0,
                    pageSize = 10,
                    radius_km = 10,
                    user_lat = 37.518313,
                    user_lon = 126.724187
                };
                PosterListResponse response = await apiService.GetPostersByCurrentLocation(request);
                if(response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    int g = 0;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
