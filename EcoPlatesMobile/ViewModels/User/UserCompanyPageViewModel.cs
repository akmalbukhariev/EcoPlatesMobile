using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.ViewModels.User
{
    public partial class UserCompanyPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<ProductModel> products;

        [ObservableProperty] private bool isRefreshingProduct;

        public UserCompanyPageViewModel()
        {
            products = new ObservableRangeCollection<ProductModel>();
            products.Add(new ProductModel());
            products.Add(new ProductModel());
            products.Add(new ProductModel());
            products.Add(new ProductModel());
            products.Add(new ProductModel());
            products.Add(new ProductModel());
            products.Add(new ProductModel());
            products.Add(new ProductModel());
            products.Add(new ProductModel());
        }

        public IRelayCommand RefreshProductCommand => new RelayCommand(async () =>
        {
            //await LoadProductFavoritesAsync(isRefresh: true);
        });

        public IRelayCommand LoadProductMoreCommand => new RelayCommand(async () =>
        {
            /*
            if (IsLoading || IsRefreshingProduct || !hasMoreProductItems)
                return;

            await LoadProductFavoritesAsync();
            */
        });
    }
}
