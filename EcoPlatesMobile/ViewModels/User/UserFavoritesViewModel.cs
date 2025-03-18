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
        public ObservableCollection<FavoriteCompany> FavoriteItems { get; set; }

        public UserFavoritesViewModel()
        {
            FavoriteItems = new ObservableCollection<FavoriteCompany>();
        }
    }
}
