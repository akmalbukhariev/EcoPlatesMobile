using EcoPlatesMobile.Models.Company;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views;

namespace EcoPlatesMobile.Services
{
    public class AppControl
    {
        public bool ShowCompanyMoreInfo { get; set; } = true;
        public CompanyInfo CompanyInfo { get; set; }
        public Dictionary<string, string> BusinessTypeList = new Dictionary<string, string>
        {
            { "Restaurant", "RESTAURANT" },
            { "Bakery", "BAKERY" },
            { "Fast Food", "FAST_FOOD" },
            { "Cafe", "CAFE" },
            { "Supermarket", "SUPERMARKET" },
            //{ "Other", "OTHER" }
        };

        public async Task LoginCompany(string phoneNumber)
        {
            var apiService = AppService.Get<CompanyApiService>();
            LoginRequest request = new LoginRequest()
            {
                phone_number = phoneNumber
            };

            LoginCompanyResponse response = await apiService.Login(request);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                AppService.Get<AppControl>().CompanyInfo = response.resultData;

                var store = AppService.Get<AppStoreService>();
                store.Set(AppKeys.UserRole, UserRole.Company);
                store.Set(AppKeys.IsLoggedIn, true);
                store.Set(AppKeys.PhoneNumber, phoneNumber);

                Application.Current.MainPage = new ContentPage
                {
                    BackgroundColor = Colors.White,
                    Content = new ActivityIndicator
                    {
                        IsRunning = true,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    }
                };

                await Task.Delay(100);

                Application.Current.MainPage = new AppCompanyShell();
            }
        }

        public void LogoutCompany()
        { 
            var store = AppService.Get<AppStoreService>();
            store.Remove(AppKeys.UserRole);
            store.Remove(AppKeys.IsLoggedIn);
            store.Remove(AppKeys.PhoneNumber); 

            Application.Current.MainPage = new AppEntryShell();
        }
    }
}
