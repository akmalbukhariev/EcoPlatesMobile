using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Services.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.ViewModels.Chat
{
    public partial class ChatedUserPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<SenderIdInfo> users;
        private CompanyApiService companyApiService;
        public ChatedUserPageViewModel(CompanyApiService companyApiService)
        {
            this.companyApiService = companyApiService;

            this.users = new ObservableRangeCollection<SenderIdInfo>();
        }

        public async Task LoadData()
        { 
            
        }
    }
}
