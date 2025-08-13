using EcoPlatesMobile.Models.Company;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.Responses.Notification;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using Plugin.Firebase.CloudMessaging;
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
        public bool IsPhoneNumberRegisterPage { get; set; } = true;
        public bool IsNotificationHandled { get; set; } = false;
        public NotificationData NotificationData { get; set; }
        public CompanyInfo CompanyInfo { get; set; }
        public UserInfo UserInfo { get; set; }

        public Location LocationForRegister { get; set; } = null;
        public object NotificationSubscriber { get; set; }
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

                #region Check the Firebase token and save it to the server
                string frbToken = await GetFirebaseToken();
                if (frbToken != response.resultData.token_frb)
                {
                    var additionalData = new Dictionary<string, string>
                    {
                        { "company_id", response.resultData.company_id.ToString() },
                        { "notification_enabled", response.resultData.notification_enabled.ToString() },
                        { "token_frb", frbToken },
                    };

                    await apiService.UpdateCompanyProfileInfo(null, additionalData);
                }
                #endregion

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

                #region Check the Firebase token and save it to the server
                string frbToken = await GetFirebaseToken();
                if (frbToken != response.resultData.token_frb)
                {
                    var additionalData = new Dictionary<string, string>
                    {
                        { "user_id", response.resultData.user_id.ToString() },
                        { "notification_enabled", response.resultData.notification_enabled.ToString() },
                        { "token_frb", frbToken },
                    };

                    await apiService.UpdateUserProfileInfo(null, additionalData);
                }
                #endregion

                Application.Current.MainPage = new ContentPage
                {
                    BackgroundColor = Colors.White,
                    Content = new ActivityIndicator
                    {
                        IsRunning = true,
                        Color = Constants.COLOR_USER,
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
            CompanyInfo = null;
            
            if (NotificationSubscriber != null)
            {
                MessagingCenter.Unsubscribe<MainActivity, NotificationData>(
                    NotificationSubscriber,
                    Constants.NOTIFICATION_BODY);
                NotificationSubscriber = null;
            }

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
            UserInfo = null;

            if (NotificationSubscriber != null)
            {
                MessagingCenter.Unsubscribe<MainActivity, NotificationData>(
                    NotificationSubscriber,
                    Constants.NOTIFICATION_BODY);
                NotificationSubscriber = null;
            }

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
                    Color = Constants.COLOR_USER,
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

        public async Task<bool> CheckWifi()
        {
            if (!IsConnectedToWifi())
            {
                await AlertService.ShowAlertAsync(AppResource.Wifi, AppResource.MessageWifi, AppResource.Ok);
                return false;
            }

            return true;
        }

        public bool IsConnectedToWifi()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
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

        public async Task<string> GetFirebaseToken()
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

            return token;
        }

        public string FormatWorkingHours(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;

            // Split by space, dash, or tilde to isolate time-like parts
            var tokens = raw.Split(new[] { ' ', '-', '~' }, StringSplitOptions.RemoveEmptyEntries);

            // Try to find exactly two times in the tokens
            var timeParts = tokens
                .Select(t => t.Trim())
                .Where(t => DateTime.TryParse(t, out _))
                .ToList();

            if (timeParts.Count == 2 &&
                DateTime.TryParse(timeParts[0], out var start) &&
                DateTime.TryParse(timeParts[1], out var end))
            {
                // Format in 24-hour style with tilde
                return $"{start:HH\\:mm} ~ {end:HH\\:mm}";
            }

            // If parsing fails, just return original
            return raw;
        }

#region Check url image
        private readonly string[] AllowedBases =
        {
            "http://95.182.117.246:8080/uploads-user/profile-pictures/",
            "http://95.182.117.246:8080/uploads-company/profile-pictures/",
            "http://95.182.117.246:8080/uploads-company/poster-pictures/" // trailing slash OK/handled
        };

        public string GetImageUrlOrFallback(string? imageUrl, string fallback = "no_image.png")
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return fallback;

            // Must be a valid absolute http/https URL
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return fallback;
            }

            // Normalize for comparison (ignore trailing '/')
            string normUrl = TrimEndSlash(uri.ToString());
            var normBases = AllowedBases.Select(TrimEndSlash).ToArray();

            // Case 1: exactly the base path (no filename) -> fallback
            if (normBases.Any(b => string.Equals(normUrl, b, StringComparison.OrdinalIgnoreCase)))
                return fallback;

            // Case 2: must start with one of the bases AND have something after it (a file part)
            bool hasAllowedPrefixWithFile = normBases.Any(b =>
                normUrl.StartsWith(b, StringComparison.OrdinalIgnoreCase) &&
                normUrl.Length > b.Length); // ensures there's more than the base

            if (!hasAllowedPrefixWithFile)
                return fallback;

            return imageUrl;
        }

        private string TrimEndSlash(string s) => s.TrimEnd('/');
#endregion

#region Photo
        public async Task<FileResult?> TryPickPhotoAsync()
        {
            try
            {
                return await MediaPicker.PickPhotoAsync();
            }
            catch (OperationCanceledException) { return null; }
            catch (PermissionException) { return null; }
        }

        public async Task<FileResult?> TryCapturePhotoAsync()
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                    return null;

                return await MediaPicker.CapturePhotoAsync();
            }
            catch (OperationCanceledException) { return null; }
            catch (PermissionException) { return null; }
        }

        public async Task<bool> EnsureGalleryPermissionAsync()
        {
#if ANDROID
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            return status == PermissionStatus.Granted;
#elif IOS
            var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Photos>();
            return status == PermissionStatus.Granted;
#else
            return true;
#endif
        }

        public async Task<bool> EnsureCameraPermissionAsync()
        {
        #if ANDROID || IOS
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Camera>();
            return status == PermissionStatus.Granted;
        #else
            return true;
        #endif
        }
#endregion
    }
}
