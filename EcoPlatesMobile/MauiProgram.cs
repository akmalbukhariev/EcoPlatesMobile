using CommunityToolkit.Maui;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Company;
using EcoPlatesMobile.ViewModels.User;
using Microsoft.Extensions.Logging;
using RestSharp;
using Microsoft.Maui.Maps.Handlers;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.User.Pages;
using EcoPlatesMobile.Views;
using The49.Maui.BottomSheet;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.CloudMessaging;
using EcoPlatesMobile.Platforms.Android.Notification;


//using Plugin.LocalNotification;
#if ANDROID
using EcoPlatesMobile.Platforms.Android;
using Plugin.Firebase.Core.Platforms.Android;
#elif IOS
using Plugin.Firebase.Core.Platforms.iOS;
#endif

namespace EcoPlatesMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .RegisterFirebaseServices()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Roboto-Variable.ttf", "RobotoVar");
                    fonts.AddFont("Roboto-Italic-Variable.ttf", "RobotoVarItalic");

                })
                .UseMauiMaps()
                .UseBottomSheet();   
#if DEBUG
            builder.Logging.AddDebug();
#endif
            RegisterSingleton(builder);
            RegisterTransient(builder);

            var mauiApp = builder.Build();
            AppService.Init(mauiApp.Services);

            return mauiApp;
        }
 
        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
        #if IOS
                events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) => {
                    CrossFirebase.Initialize();
                    FirebaseCloudMessagingImplementation.Initialize();
                    return false;
                }));
        #elif ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                CrossFirebase.Initialize(activity)));
        #endif
            });

            return builder;
        }

        private static void RegisterSingleton(MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IUpdateService, UpdateService>();
            builder.Services.AddSingleton<LanguageService>();
            builder.Services.AddSingleton<LocationService>();
            builder.Services.AddSingleton<AppControl>();
            builder.Services.AddSingleton<AppStoreService>();
            builder.Services.AddSingleton<UserSessionService>();
            builder.Services.AddSingleton<LanguageService>();
            builder.Services.AddSingleton<ChatWebSocketService>();
            builder.Services.AddSingleton(sp =>
                new RestClient(new RestClientOptions(Constants.BASE_USER_URL)
                {
                    ThrowOnAnyError = false,
                    Timeout = TimeSpan.FromSeconds(30)
                }));
            builder.Services.AddSingleton(sp =>
                new RestClient(new RestClientOptions(Constants.BASE_COMPANY_URL)
                {
                    ThrowOnAnyError = false,
                    Timeout = TimeSpan.FromSeconds(30)
                }));
            builder.Services.AddSingleton(sp =>
                new RestClient(new RestClientOptions(Constants.BASE_CHAT_URL)
                {
                    ThrowOnAnyError = false,
                    Timeout = TimeSpan.FromSeconds(30)
                }));

#if ANDROID
            builder.Services.AddSingleton<IStatusBarService, StatusBarService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddSingleton<IKeyboardHelper, KeyboardHelper>();
#endif
        }

        private static void RegisterTransient(MauiAppBuilder builder)
        {
            builder.Services.AddTransient(sp =>
                new UserApiService(
                    sp.GetServices<RestClient>().First(rc => rc.Options.BaseUrl == new Uri(Constants.BASE_USER_URL))
                ));
            builder.Services.AddTransient(sp =>
                new CompanyApiService(
                    sp.GetServices<RestClient>().First(rc => rc.Options.BaseUrl == new Uri(Constants.BASE_COMPANY_URL))
                ));
            builder.Services.AddTransient(sp =>
                new ChatApiService(
                    sp.GetServices<RestClient>().First(rc => rc.Options.BaseUrl == new Uri(Constants.BASE_CHAT_URL))
                ));

            #region Company
            builder.Services.AddTransient<ActiveProductPage>();
            builder.Services.AddTransient<InActiveProductPage>();
            builder.Services.AddTransient<CompanyAddProductPage>();
            builder.Services.AddTransient<CompanyEditProductPage>();
            builder.Services.AddTransient<CompanyProfilePage>();
            builder.Services.AddTransient<CompanyProfileInfoPage>();
            builder.Services.AddTransient<CompanyRegistrationPage>();
            builder.Services.AddTransient<LocationPage>();
            builder.Services.AddTransient<LocationRegistrationPage>();

            builder.Services.AddTransient<ActiveProductPageViewModel>();
            builder.Services.AddTransient<InActiveProductPageViewModel>();
            #endregion

            #region User
            builder.Services.AddTransient<UserMainPage>();
            builder.Services.AddTransient<UserMainSearchPage>();
            builder.Services.AddTransient<UserBrowserPage>();
            builder.Services.AddTransient<UserBrowserSearchPage>();
            builder.Services.AddTransient<UserCompanyPage>();
            builder.Services.AddTransient<UserFavoritesPage>();
            builder.Services.AddTransient<DetailProductPage>();

            builder.Services.AddTransient<LocationSettingPage>();
            builder.Services.AddTransient<ReviewProductPage>();
            builder.Services.AddTransient<UserProfileInfoPage>();
            builder.Services.AddTransient<UserProfilePage>();
            builder.Services.AddTransient<UserRegistrationPage>();

            builder.Services.AddTransient<UserMainPageViewModel>();
            builder.Services.AddTransient<UserMainSearchPageViewModel>();
            builder.Services.AddTransient<UserBrowserPageViewModel>();
            builder.Services.AddTransient<UserBrowserSearchPageViewModel>();
            builder.Services.AddTransient<UserCompanyPageViewModel>();
            builder.Services.AddTransient<UserFavoritesViewModel>();
            builder.Services.AddTransient<DetailProductPageViewModel>();
            #endregion

            #region Both
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<AuthorizationPage>();
            builder.Services.AddTransient<LanguagePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<PhoneNumberChangePage>();
            builder.Services.AddTransient<PhoneNumberNewPage>();
            builder.Services.AddTransient<PhoneNumberRegisterPage>();
            builder.Services.AddTransient<SuggestionsPage>();
            builder.Services.AddTransient<PhoneNumberNewPage>();
            builder.Services.AddTransient<Views.Chat.ChattingPage>();
            builder.Services.AddTransient<Views.Chat.ChatedUserPage>();

            builder.Services.AddTransient<ViewModels.Chat.ChattingPageViewModel>();
            builder.Services.AddTransient<ViewModels.Chat.ChatedUserPageViewModel>();
            #endregion
        }
    }
}
