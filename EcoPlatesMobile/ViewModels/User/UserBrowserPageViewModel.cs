
using System.Collections.ObjectModel;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.ViewModels.User
{
    public class UserBrowserPageViewModel
    {
        public ObservableCollection<CompanyModel> Companies {  get; set; }

        public UserBrowserPageViewModel()
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