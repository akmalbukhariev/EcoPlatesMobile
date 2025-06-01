using CommunityToolkit.Maui;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Company;
using EcoPlatesMobile.ViewModels.User;
using Microsoft.Extensions.Logging;
using RestSharp;
using SkiaSharp.Views.Maui.Controls.Hosting;

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
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
 
                }) 
                .UseMauiMaps();
 
#if DEBUG
    		builder.Logging.AddDebug();
#endif
            RegisterSingleton(builder);
            RegisterTransient(builder);
            
            var mauiApp = builder.Build();
            AppService.Init(mauiApp.Services);

            return mauiApp;
        }

        private static void RegisterSingleton(MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<AppControl>();
            builder.Services.AddSingleton<UserSessionService>();
            builder.Services.AddSingleton(sp =>
                new RestClient(new RestClientOptions(Constants.BASE_USER_URL)
                {
                    MaxTimeout = -1,
                    //AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                    //ThrowOnAnyError = false,
                    //Timeout = TimeSpan.FromSeconds(30)
                }));
            builder.Services.AddSingleton(sp =>
                new RestClient(new RestClientOptions(Constants.BASE_COMPANY_URL)
                {
                    ThrowOnAnyError = false,
                    Timeout = TimeSpan.FromSeconds(30)
                }));
            builder.Services.AddSingleton<IViewModelFactory, ViewModelFactory>();

            builder.Services.AddSingleton<UserMainPageViewModel>();
            builder.Services.AddSingleton<ActiveProductPageViewModel>();
            builder.Services.AddSingleton<NonActiveProductPageViewModel>();
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
        }
    }
}
