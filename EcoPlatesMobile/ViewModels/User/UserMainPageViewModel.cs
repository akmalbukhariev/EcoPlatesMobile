using EcoPlatesMobile.Models.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.ViewModels.User
{
    public class UserMainPageViewModel
    {
        public ObservableCollection<ProductModel> Products {  get; set; }

        public UserMainPageViewModel()
        {
            Products = new ObservableCollection<ProductModel>
            {
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
            };
        }
    }
}
