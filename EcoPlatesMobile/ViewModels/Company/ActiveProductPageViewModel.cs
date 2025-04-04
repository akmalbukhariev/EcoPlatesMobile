using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.ViewModels.Company
{ 
    public partial class ActiveProductPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ProductModel> products;
        
        public ActiveProductPageViewModel()
        {
             
        }
    }
}