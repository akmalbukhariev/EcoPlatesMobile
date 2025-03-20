using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
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
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(sp =>
                new RestClient(new RestClientOptions(Constants.BASE_USER_URL)
                {
                    ThrowOnAnyError = true,
                    Timeout = TimeSpan.FromSeconds(30)
                }));
            builder.Services.AddSingleton(sp =>
                new RestClient(new RestClientOptions(Constants.BASE_COMPANY_URL)
                {
                    ThrowOnAnyError = true,
                    Timeout = TimeSpan.FromSeconds(30)
                }));

            builder.Services.AddTransient(sp =>
                new UserApiService(
                    sp.GetServices<RestClient>().First(rc => rc.Options.BaseUrl == new Uri(Constants.BASE_USER_URL))
                ));
            builder.Services.AddTransient(sp =>
                new CompanyApiService(
                    sp.GetServices<RestClient>().First(rc => rc.Options.BaseUrl == new Uri(Constants.BASE_COMPANY_URL))
                ));

            return builder.Build();
        }
    }
}
