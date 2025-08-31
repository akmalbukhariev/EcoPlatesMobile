using EcoPlatesMobile.Models.Responses.Notification;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.User.Pages;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Newtonsoft.Json;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.EventArgs;
using System.Text;

namespace EcoPlatesMobile
{
    public partial class App : Application
    {
        private readonly INotificationService notificationService;
        private readonly UserSessionService userSessionService;
        private IUpdateService updateService;
        private AppControl appControl;
        public App(INotificationService notificationService, UserSessionService userSessionService, IUpdateService updateService, AppControl appControl)
        {
            InitializeComponent();
            this.notificationService = notificationService;
            this.userSessionService = userSessionService;
            this.updateService = updateService;
            this.appControl = appControl;

            RegisterRoutes();
            Setting();
        }
  
        protected override Window CreateWindow(IActivationState? activationState)
        {
            AppService.Get<LanguageService>().Init();
            
            return new Window(new AppEntryShell());
        }
        
        protected override async void OnStart()
        {
            base.OnStart();

            await updateService.CheckAndPromptAsync();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (appControl.UpdatePending)
            {
                updateService.CheckAndPromptAsync();
            }
        }

        private void RegisterRoutes()
        {
            #region Entry pages
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(PhoneNumberRegisterPage), typeof(PhoneNumberRegisterPage));
            Routing.RegisterRoute(nameof(AuthorizationPage), typeof(AuthorizationPage));
            #endregion

            #region User pages
            Routing.RegisterRoute(nameof(ReviewProductPage), typeof(ReviewProductPage));
            Routing.RegisterRoute(nameof(DetailProductPage), typeof(DetailProductPage));
            Routing.RegisterRoute(nameof(UserCompanyPage), typeof(UserCompanyPage));
            Routing.RegisterRoute(nameof(UserMainPage), typeof(UserMainPage));
            Routing.RegisterRoute(nameof(UserRegistrationPage), typeof(UserRegistrationPage));
            Routing.RegisterRoute(nameof(UserProfileInfoPage), typeof(UserProfileInfoPage));
            Routing.RegisterRoute(nameof(LocationSettingPage), typeof(LocationSettingPage));
            Routing.RegisterRoute(nameof(UserMainSearchPage), typeof(UserMainSearchPage));//
            Routing.RegisterRoute(nameof(CompanyNavigatorPage), typeof(CompanyNavigatorPage));
            #endregion

            #region Company pages
            Routing.RegisterRoute(nameof(ActiveProductPage), typeof(ActiveProductPage));
            Routing.RegisterRoute(nameof(InActiveProductPage), typeof(InActiveProductPage));
            Routing.RegisterRoute(nameof(CompanyProfileInfoPage), typeof(CompanyProfileInfoPage));
            Routing.RegisterRoute(nameof(CompanyProfilePage), typeof(CompanyProfilePage));
            Routing.RegisterRoute(nameof(CompanyAddProductPage), typeof(CompanyAddProductPage));
            Routing.RegisterRoute(nameof(CompanyEditProductPage), typeof(CompanyEditProductPage));
            Routing.RegisterRoute(nameof(PhoneNumberChangePage), typeof(PhoneNumberChangePage));
            Routing.RegisterRoute(nameof(PhoneNumberNewPage), typeof(PhoneNumberNewPage));
            Routing.RegisterRoute(nameof(CompanyRegistrationPage), typeof(CompanyRegistrationPage));
            Routing.RegisterRoute(nameof(LocationPage), typeof(LocationPage));
            Routing.RegisterRoute(nameof(UserBrowserSearchPage), typeof(UserBrowserSearchPage));
            Routing.RegisterRoute(nameof(LocationRegistrationPage), typeof(LocationRegistrationPage));
            #endregion

            #region Both
            Routing.RegisterRoute(nameof(Views.Chat.ChattingPage), typeof(Views.Chat.ChattingPage));
            Routing.RegisterRoute(nameof(Views.Chat.ChatedUserPage), typeof(Views.Chat.ChatedUserPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(SuggestionsPage), typeof(SuggestionsPage));
            #endregion
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

            EditorHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
                if (handler.PlatformView is Android.Widget.EditText editText)
                {
                    editText.Background = null; // Removes underline
                }
            });

            //Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping("Force24Hour", (handler, view) =>
            //{ 
            //handler.PlatformView?.SetOnClickListener(new My24HourTimePickerClickListener(handler));
            //});
#endif
        }

        public void ReloadAppShell()
        {
            MainPage = new AppEntryShell();
        }
    }
}