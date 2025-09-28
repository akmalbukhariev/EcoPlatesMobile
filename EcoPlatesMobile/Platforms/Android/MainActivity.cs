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
using AndroidX.Core.App;

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
        private static readonly Android.Graphics.Color AccentGreen = Android.Graphics.Color.ParseColor("#007100");

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

        private static string GetFirstExtra(Intent intent, params string[] keys)
        {
            if (intent == null) return null;

            // 1) Top-level
            foreach (var k in keys)
            {
                var v = intent.GetStringExtra(k) ?? intent.Extras?.GetString(k);
                if (!string.IsNullOrEmpty(v)) return v;
            }

            // 2) intent_key_fcm_notification
            var notif = intent.Extras?.Get("intent_key_fcm_notification") as Bundle;
            if (notif != null)
            {
                // 2a) Direct inside notif
                foreach (var k in keys)
                {
                    var v = notif.GetString(k);
                    if (!string.IsNullOrEmpty(v)) return v;
                }

                // 2b) Nested "data" bundle inside notif
                var dataBundle = notif.Get("data") as Bundle;
                if (dataBundle != null)
                {
                    foreach (var k in keys)
                    {
                        var v = dataBundle.GetString(k);
                        if (!string.IsNullOrEmpty(v)) return v;
                    }
                }
            }

            return null;
        }

        private static async Task HandleIntent(Intent intent, bool appRunning = false)
        {  
            var title = GetFirstExtra(intent,
                "notification_title",
                Utilities.Constants.NOTIFICATION_TITLE,
                "title");
 
            var bodyJson = GetFirstExtra(intent,
                "notification_body",
                Utilities.Constants.NOTIFICATION_BODY,
                "payload");

            //ShowToast($"Title: {title ?? "(null)"}");
            //ShowToast($"Body len: {(bodyJson?.Length ?? 0)}");

            if (!string.IsNullOrEmpty(bodyJson))
            {
                var notificationData = new NotificationData { title = title ?? "SaleTop", body = bodyJson };

                if (!appRunning)
                    AppService.Get<AppControl>().NotificationData = notificationData;

                if (Instance is MainActivity mainActivity)
                    MessagingCenter.Send(mainActivity, Constants.NOTIFICATION_BODY, notificationData);
            }

            FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        }
        
        /*
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
        */

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var nm = (NotificationManager)GetSystemService(NotificationService);
                var ch = new NotificationChannel(Channel_ID, "SaleTop Messages", NotificationImportance.High)
                {
                    Description = "General notifications",
                    LightColor = AccentGreen,
                    LockscreenVisibility = NotificationVisibility.Public
                };
                ch.EnableVibration(true);
                ch.EnableLights(true);
                nm.CreateNotificationChannel(ch);
            }

            FirebaseCloudMessagingImplementation.ChannelId = Channel_ID;
            FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.notification_icon;
             
            FirebaseCloudMessagingImplementation.NotificationBuilderProvider = fcm =>
            {
                var ctx = Android.App.Application.Context;
                var title = string.IsNullOrWhiteSpace(fcm.Title) ? "SaleTop" : fcm.Title;
                var body = fcm.Body ?? string.Empty;
                
                var useChannel = string.IsNullOrWhiteSpace(FirebaseCloudMessagingImplementation.ChannelId)
                    ? Channel_ID
                    : FirebaseCloudMessagingImplementation.ChannelId;

                return new AndroidX.Core.App.NotificationCompat.Builder(ctx, useChannel)
                    .SetSmallIcon(Resource.Drawable.notification_icon)
                    .SetContentTitle(title)
                    .SetContentText(body)
                    .SetStyle(new AndroidX.Core.App.NotificationCompat.BigTextStyle().BigText(body))
                    .SetPriority(AndroidX.Core.App.NotificationCompat.PriorityHigh)
                    .SetAutoCancel(true)
                    .SetColor(AccentGreen)
                    .SetColorized(true);
            };
        }

        /*
        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            var ch = new NotificationChannel(Channel_ID, "SaleTop Messages", NotificationImportance.High);
            ch.LockscreenVisibility = NotificationVisibility.Public;
            ch.EnableVibration(true);
            ch.EnableLights(true);
            notificationManager.CreateNotificationChannel(ch);

            FirebaseCloudMessagingImplementation.ChannelId = Channel_ID;
            FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.notification_icon;
        }
        */

        public static Activity Instance { get; private set; }

        void TryFixTabsWithRetry(int attemptsLeft = 10)
        {
            PostDelayed(() =>
            {
                var rootView = Window.DecorView?.RootView;
                if (rootView == null)
                { 
                    return;
                }

                var bottomNav = FindBottomNavigationView(rootView);
                if (bottomNav == null)
                { 
                    if (attemptsLeft > 1) TryFixTabsWithRetry(attemptsLeft - 1);
                    return;
                }

                var menuView = bottomNav.GetChildAt(0);
                if (menuView is ViewGroup menuGroup)
                {
                    if (menuGroup.ChildCount == 0)
                    { 
                        if (attemptsLeft > 1) TryFixTabsWithRetry(attemptsLeft - 1);
                        return;
                    } 
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
            bottomNav.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityLabeled;

            ApplyFontToTextViews(bottomNav);
        }

        void ApplyFontToTextViews(Android.Views.View view, int depth = 0)
        {
            string indent = new string(' ', depth * 2);

            if (view is TextView tv)
            {
                var typeface = Typeface.Create("RobotoVar", TypefaceStyle.Normal);
                tv.Typeface = typeface; 
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
