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
              ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : MauiAppCompatActivity
    {
        public const int NotificationID = 1001;
        public const string Channel_ID = "Plugin.LocalNotification.GENERAL";

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
            HandleIntent(intent);
        }

        private static void HandleIntent(Intent intent)
        {
            FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        }

        private void CreateNotificationChannel()
        {
            var channelId = $"{PackageName}.general";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);
            FirebaseCloudMessagingImplementation.ChannelId = channelId;
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
                var typeface = Typeface.Create("Roboto", TypefaceStyle.Normal); // Use registered alias from MAUI
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
