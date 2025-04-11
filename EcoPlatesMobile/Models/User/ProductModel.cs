using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.User
{
    /*public class ProductModel : BaseModel
    {
        public string ProductImage { get { return GetValue<string>(); } set => SetValue(value); }
        public string Count { get { return GetValue<string>(); } set => SetValue(value); }
        public string Name { get { return GetValue<string>(); } set => SetValue(value); }
        public string ComapnyName { get { return GetValue<string>(); } set => SetValue(value); }
        public string NewPrice { get { return GetValue<string>(); } set => SetValue(value); }
        public string OldPrice { get { return GetValue<string>(); } set => SetValue(value); }
        public string Stars { get { return GetValue<string>(); } set => SetValue(value); }
        public string Distance { get { return GetValue<string>(); } set => SetValue(value); }
    }*/

    public partial class ProductModel : ObservableObject
    {
        public long PromotionId;
        [ObservableProperty]
        private string productImage;
        [ObservableProperty]
        private string count;
        [ObservableProperty]
        private string productName;
        [ObservableProperty]
        private string productMakerName;
        [ObservableProperty]
        private string newPrice;
        [ObservableProperty]
        private string oldPrice;
        [ObservableProperty]
        private string stars;
        [ObservableProperty]
        private bool liked;
        public long BookmarkId = 0;
        [ObservableProperty]
        private string distance;
    }
}
