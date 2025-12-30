using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.User
{
    public partial class ProductModel : ObservableObject
    {
        public long CompanyId;
        public long PromotionId;
        public long BookmarkId = 0;
        public decimal? NewPriceDigit;
        public decimal? OldPriceDigit;
        public string description;
        public bool IsThisActivePage;
        [ObservableProperty] private string productImage;
        [ObservableProperty] private string count;
        [ObservableProperty] private string productName;
        [ObservableProperty] private string productMakerName;
        [ObservableProperty] private string newPrice;
        [ObservableProperty] private string oldPrice;
        [ObservableProperty] private string stars;
        [ObservableProperty] private bool liked;
        [ObservableProperty] private string distance;
        [ObservableProperty] private bool showCheckProduct;
        [ObservableProperty] private bool isNonActiveProduct;
        [ObservableProperty] private bool isCheckedProduct;
        public string Category = PosterType.FOOD_GENERAL.GetValue();
    }
}
