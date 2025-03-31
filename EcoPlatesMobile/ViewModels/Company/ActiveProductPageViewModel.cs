using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.ViewModels.Company
{ 
    public partial class ActiveProductPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<ProductModel> products;
        
        public ActiveProductPageViewModel()
        {
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
        }
    }
}