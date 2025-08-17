using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using static AndroidX.ConstraintLayout.Widget.ConstraintSet.Constraint;

using Plugin.Firebase.CloudMessaging;
using Android.Content;
using System.Threading.Tasks;
using AndroidX.Annotations;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Models.Responses.Notification;

namespace EcoPlatesMobile
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              LaunchMode = LaunchMode.SingleTop,
              ConfigurationChanges = ConfigChanges.ScreenSize
              | ConfigChanges.Orientation
              | ConfigChanges.UiMode
              | ConfigChanges.ScreenLayout
              | ConfigChanges.SmallestScreenSize
              | ConfigChanges.Density,
              ScreenOrientation = ScreenOrientation.Portrait,
              Exported = true)]
    public class MainActivity : MauiAppCompatActivity
    {
        public const int NotificationID = 1001;
        public const string Channel_ID = "saletop_messages";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HandleIntent(Intent);

            CreateNotificationChannel();
             
            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FFFFFF"));

            Instance = this;

            TryFixTabsWithRetry();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            HandleIntent(intent, true);
        }
         
        private static async Task HandleIntent(Intent intent, bool appRunning = false)
        {
            if (intent == null) return;

            if (intent.HasExtra(Utilities.Constants.NOTIFICATION_TITLE) && intent.HasExtra(Utilities.Constants.NOTIFICATION_BODY))
            {
                var title = intent.GetStringExtra(Utilities.Constants.NOTIFICATION_TITLE);
                var body = intent.GetStringExtra(Utilities.Constants.NOTIFICATION_BODY);

                var notificationData = new NotificationData()
                {
                    title = title,
                    body = body
                };

                if (!appRunning)
                {
                    AppService.Get<AppControl>().NotificationData = notificationData;
                } 

                if (Instance is MainActivity mainActivity)
                {
                    MessagingCenter.Send<MainActivity, NotificationData>(mainActivity, Constants.NOTIFICATION_BODY, notificationData);
                }        
            }

            FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

            var nm = (NotificationManager)GetSystemService(NotificationService);
            var ch = new NotificationChannel(Channel_ID, "SaleTop Messages", NotificationImportance.High);
            ch.LockscreenVisibility = NotificationVisibility.Public;
            ch.EnableVibration(true);
            ch.EnableLights(true);
            nm.CreateNotificationChannel(ch);

            FirebaseCloudMessagingImplementation.ChannelId = Channel_ID;
        }
        
        public static Activity Instance { get; private set; }

        void TryFixTabsWithRetry(int attemptsLeft = 10)
        {
            PostDelayed(() =>
            {
                var rootView = Window.DecorView?.RootView;
                if (rootView == null)
                {
#if DEBUG
                    Android.Util.Log.Warn("TabFix", "❌ RootView is null");
#endif
                    return;
                }

                var bottomNav = FindBottomNavigationView(rootView);
                if (bottomNav == null)
                {
#if DEBUG
                    Android.Util.Log.Warn("TabFix", $"❌ BottomNavigationView not found. Attempts left: {attemptsLeft - 1}");
#endif
                    if (attemptsLeft > 1) TryFixTabsWithRetry(attemptsLeft - 1);
                    return;
                }

                var menuView = bottomNav.GetChildAt(0);
                if (menuView is ViewGroup menuGroup)
                {
                    if (menuGroup.ChildCount == 0)
                    {
#if DEBUG
                        Android.Util.Log.Warn("TabFix", $"❌ MenuView has no children. Retrying... ({attemptsLeft - 1} left)");
#endif
                        if (attemptsLeft > 1) TryFixTabsWithRetry(attemptsLeft - 1);
                        return;
                    }
#if DEBUG
                    Android.Util.Log.Info("TabFix", "✅ MenuView is ready. Applying font fix.");
#endif
                    SetTabTextNotBold(bottomNav);
                }

            }, 600);
        }

        void PostDelayed(Action action, int delayMillis)
        {
            var handler = new Handler(Looper.MainLooper);
            handler.PostDelayed(action, delayMillis);
        }

        BottomNavigationView? FindBottomNavigationView(Android.Views.View view)
        {
            if (view is BottomNavigationView navView)
                return navView;

            if (view is ViewGroup group)
            {
                for (int i = 0; i < group.ChildCount; i++)
                {
                    var result = FindBottomNavigationView(group.GetChildAt(i));
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        void SetTabTextNotBold(BottomNavigationView bottomNav)
        {
            // Make sure tab labels are visible (important!)
            bottomNav.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityLabeled;

            // Recursively walk and apply font
            ApplyFontToTextViews(bottomNav);
        }

        void ApplyFontToTextViews(Android.Views.View view, int depth = 0)
        {
            string indent = new string(' ', depth * 2);
#if DEBUG
            Android.Util.Log.Debug("TabFix", $"{indent}{view.GetType().Name}");
#endif

            if (view is TextView tv)
            {
                var typeface = Typeface.Create("RobotoVar", TypefaceStyle.Normal); // Use registered alias from MAUI
                tv.Typeface = typeface;
#if DEBUG
                Android.Util.Log.Info("TabFix", $"{indent}✅ Font applied to: {tv.Text}");
#endif
            }

            if (view is ViewGroup group)
            {
                for (int i = 0; i < group.ChildCount; i++)
                {
                    ApplyFontToTextViews(group.GetChildAt(i), depth + 1);
                }
            }
        }
    }
}
