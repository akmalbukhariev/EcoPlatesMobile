using EcoPlatesMobile.Models.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.ViewModels.User
{
    public class UserFavoritesViewModel
    {
        public ObservableCollection<CompanyModel> Companies { get; set; }
        public ObservableCollection<ProductModel> Products { get; set; }

        public UserFavoritesViewModel()
        {
            Companies = new ObservableCollection<CompanyModel>()
            {
                new CompanyModel(),
                new CompanyModel(),
                new CompanyModel(),
                new CompanyModel(),
                new CompanyModel(),
                new CompanyModel(),
                new CompanyModel(),
            };
        }
    }
}
