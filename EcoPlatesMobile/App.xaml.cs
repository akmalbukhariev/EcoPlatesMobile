using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.User.Pages;

using Microsoft.Maui.Platform;

namespace EcoPlatesMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry),(handler, view) =>
            {
            handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(Picker), (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
            });
#endif
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new CompanyRegistrationPage());
        }
    }
}