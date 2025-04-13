
using CommunityToolkit.Mvvm.ComponentModel;

namespace EcoPlatesMobile.Models.User
{
    public partial class CompanyModel : ObservableObject
    {
        public long CompanyId;
        public long BookmarkId = 0;
        [ObservableProperty] private string companyImage;
        [ObservableProperty] private string companyName;
        [ObservableProperty] private string workingTime;
        [ObservableProperty] private string stars;
        [ObservableProperty] private string distance;
        [ObservableProperty] private bool liked;
    }
}
