using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public UserMainPageViewModel()
        {
            products = new ObservableCollection<ProductModel>();
            
            products.Add(new ProductModel
            {
                Image = "cake.png",
                Count = "2 qoldi",
                Name = "Tort rogalik",
                ComapnyName = "Safia &amp; Bakery",
                NewPrice = "15 000 so'm",
                OldPrice = "25 000 so'm",
                Stars = "3.1",
                Distance = "1 km"
            });
            products.Add(new ProductModel
            {
                Image = "cake.png",
                Count = "2 qoldi",
                Name = "Tort rogalik",
                ComapnyName = "Safia &amp; Bakery",
                NewPrice = "15 000 so'm",
                OldPrice = "25 000 so'm",
                Stars = "3.1",
                Distance = "1 km"
            });
            products.Add(new ProductModel
            {
                Image = "cake.png",
                Count = "2 qoldi",
                Name = "Tort rogalik",
                ComapnyName = "Safia &amp; Bakery",
                NewPrice = "15 000 so'm",
                OldPrice = "25 000 so'm",
                Stars = "3.1",
                Distance = "1 km"
            });
            products.Add(new ProductModel
            {
                Image = "cake.png",
                Count = "2 qoldi",
                Name = "Tort rogalik",
                ComapnyName = "Safia &amp; Bakery",
                NewPrice = "15 000 so'm",
                OldPrice = "25 000 so'm",
                Stars = "3.1",
                Distance = "1 km"
            });
            products.Add(new ProductModel
            {
                Image = "cake.png",
                Count = "2 qoldi",
                Name = "Tort rogalik",
                ComapnyName = "Safia &amp; Bakery",
                NewPrice = "15 000 so'm",
                OldPrice = "25 000 so'm",
                Stars = "3.1",
                Distance = "1 km"
            });
            products.Add(new ProductModel
            {
                Image = "cake.png",
                Count = "2 qoldi",
                Name = "Tort rogalik",
                ComapnyName = "Safia &amp; Bakery",
                NewPrice = "15 000 so'm",
                OldPrice = "25 000 so'm",
                Stars = "3.1",
                Distance = "1 km"
            });
            products.Add(new ProductModel
            {
                Image = "cake.png",
                Count = "2 qoldi",
                Name = "Tort rogalik",
                ComapnyName = "Safia &amp; Bakery",
                NewPrice = "15 000 so'm",
                OldPrice = "25 000 so'm",
                Stars = "3.1",
                Distance = "1 km"
            });
            products.Add(new ProductModel
            {
                Image = "cake.png",
                Count = "2 qoldi",
                Name = "Tort rogalik",
                ComapnyName = "Safia &amp; Bakery",
                NewPrice = "15 000 so'm",
                OldPrice = "25 000 so'm",
                Stars = "3.1",
                Distance = "1 km"
            });
        }
    }
}
