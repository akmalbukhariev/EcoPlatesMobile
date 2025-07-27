using Android.App;
using Android.Runtime;

namespace EcoPlatesMobile
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override async void OnCreate()
        {
            base.OnCreate();

            //FirebaseApp.InitializeApp(this);
            /*
            try
            {
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

                int g = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token error: {ex.Message}");
            }
            */
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
