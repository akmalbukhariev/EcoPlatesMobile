using EcoPlatesMobile.Models.Company;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using System.Text.RegularExpressions;

namespace EcoPlatesMobile.Services
{
    public class AppControl
    {
        public bool RefreshFavoriteProduct { get; set; } = true;
        public bool RefreshFavoriteCompany { get; set; } = true;
        public bool RefreshMainPage { get; set; } = true;
        public bool RefreshBrowserPage { get; set; } = true;
        public bool RefreshUserProfilePage { get; set; } = true;
        public bool RefreshCompanyProfilePage { get; set; } = true;
        public bool IsPhoneNumberRegisterPage {  get; set; } = true;
        public CompanyInfo CompanyInfo { get; set; }
        public UserInfo UserInfo { get; set; }
        public Location LocationForRegister { get; set; } = null;

        public Dictionary<string, string> BusinessTypeList = new Dictionary<string, string>
        {
            { AppResource.Restaurant, "RESTAURANT" },
            { AppResource.Bakery, "BAKERY" },
            { AppResource.FastFood, "FAST_FOOD" },
            { AppResource.Cafe, "CAFE" },
            { AppResource.Supermarket, "SUPERMARKET" },
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

                RefreshCompanyProfilePage = true;
                Application.Current.MainPage = new AppCompanyShell();
            }
        }

        public async Task LoginUser(string phoneNumber)
        {
            var apiService = AppService.Get<UserApiService>();
            LoginRequest request = new LoginRequest()
            {
                phone_number = phoneNumber
            };

            LoginUserResponse response = await apiService.Login(request);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                AppService.Get<AppControl>().UserInfo = response.resultData;

                var store = AppService.Get<AppStoreService>();
                store.Set(AppKeys.UserRole, UserRole.User);
                store.Set(AppKeys.IsLoggedIn, true);
                store.Set(AppKeys.PhoneNumber, phoneNumber);

                Application.Current.MainPage = new ContentPage
                {
                    BackgroundColor = Colors.White,
                    Content = new ActivityIndicator
                    {
                        IsRunning = true,
                        Color = Colors.Green,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    }
                };

                await Task.Delay(100);

                RefreshMainPage = true;
                RefreshBrowserPage = true;
                RefreshFavoriteCompany = true;
                RefreshFavoriteProduct = true;
                RefreshUserProfilePage = true;
                Application.Current.MainPage = new AppUserShell();
            }
        }

        public async Task LogoutCompany()
        {
            CompanyApiService companyApi = AppService.Get<CompanyApiService>();
            Response response = await companyApi.LogOut();
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                //do something
            }

            var store = AppService.Get<AppStoreService>();
            store.Remove(AppKeys.UserRole);
            store.Remove(AppKeys.IsLoggedIn);
            store.Remove(AppKeys.PhoneNumber);

            await companyApi.ClearTokenAsync();

            Application.Current.MainPage = new AppEntryShell();
        }

        public async Task LogoutUser()
        {
            UserApiService userApi = AppService.Get<UserApiService>();
            Response response = await userApi.LogOut();
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                //do something
            }

            var store = AppService.Get<AppStoreService>();
            store.Remove(AppKeys.UserRole);
            store.Remove(AppKeys.IsLoggedIn);
            store.Remove(AppKeys.PhoneNumber);

            await userApi.ClearTokenAsync();

            Application.Current.MainPage = new AppEntryShell();
        }

        public async Task MoveUserHome()
        {
            Application.Current.MainPage = new ContentPage
            {
                BackgroundColor = Colors.White,
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    Color = Colors.Green,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            };
            await Task.Delay(100);

            RefreshMainPage = true;
            RefreshBrowserPage = true;
            RefreshFavoriteCompany = true;
            RefreshFavoriteProduct = true;
            RefreshUserProfilePage = true;
            Application.Current.MainPage = new AppUserShell();
        }

        public void RefreshAllPages()
        {
            RefreshMainPage = true;
            RefreshBrowserPage = true;
            RefreshFavoriteProduct = true;
            RefreshFavoriteCompany = true;
            RefreshUserProfilePage = true;
        }

        public bool IsValidUzbekistanPhoneNumber(string phoneNumber)
        {
            string PHONE_PATTERN = @"^(\+998|998)?(90|91|93|94|95|97|98|99|33|88|20)\d{7}$";
            return Regex.IsMatch(phoneNumber, PHONE_PATTERN);
        }
    }
}
