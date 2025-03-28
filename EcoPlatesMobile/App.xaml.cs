using EcoPlatesMobile.Views;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.User.Pages;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;


namespace EcoPlatesMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            RegisterRoutes();
            Setting();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            //return new Window(new NavigationPage(new LoginPage())); 
            return new Window(new CompanyAddItemPage());
            //return new Window(new UserProfilePage());
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(AppRoutes.LoginPage, typeof(LoginPage));
            Routing.RegisterRoute(AppRoutes.PhoneNumberPage, typeof(PhoneNumberPage));
            Routing.RegisterRoute(AppRoutes.UserMainPage, typeof(UserMainPage));
            Routing.RegisterRoute(AppRoutes.CompanyRegistrationPage, typeof(CompanyRegistrationPage));
        }

        private void Setting()
        {
#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(Picker), (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
            });

            Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping(nameof(DatePicker), (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
            });

            Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping(nameof(TimePicker), (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
            });

            //Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping("Force24Hour", (handler, view) =>
            //{ 
                //handler.PlatformView?.SetOnClickListener(new My24HourTimePickerClickListener(handler));
            //});
#endif
        }
    }
}