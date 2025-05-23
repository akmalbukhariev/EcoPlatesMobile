using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Microsoft.Maui.Handlers;

namespace EcoPlatesMobile
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        /*static MainActivity()
        {
            EntryHandler.Mapper.AppendToMapping("CustomNumeric", (handler, view) =>
            {
                if (view is Entry)
                {
                    handler.PlatformView.InputType = InputTypes.ClassNumber | InputTypes.NumberVariationNormal;
                }
            });
        }*/

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);

            Instance = this;
        }

        public static Activity Instance { get; private set; }
    }
}
