using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using UIKit;
using UserNotifications;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Models.Responses.Notification;
using EcoPlatesMobile.Services;

namespace EcoPlatesMobile
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // 1) Make sure we receive callbacks when user taps notification
            UNUserNotificationCenter.Current.Delegate = this;

            // 2) If app was launched by tapping a notification (cold start),
            //    iOS will put the payload in LaunchOptionsRemoteNotificationKey.
            if (options != null &&
                options.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
            {
                var remoteNotification =
                    options[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;

                if (remoteNotification != null)
                {
                    // appRunning = false (same idea as Android)
                    HandleNotificationTap(remoteNotification, appRunning: false);
                }
            }

            // If you have any Firebase init for iOS, keep it here
            // e.g. Firebase.Core.App.Configure();

            return base.FinishedLaunching(app, options);
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(
            UNUserNotificationCenter center,
            UNNotificationResponse response,
            Action completionHandler)
        {
            var userInfo = response?.Notification?.Request?.Content?.UserInfo;
            if (userInfo != null)
            {
                HandleNotificationTap(userInfo, appRunning: true);
            }

            completionHandler?.Invoke();
        }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(
            UNUserNotificationCenter center,
            UNNotification notification,
            Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Allow notification to show while app is in foreground
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
        }

        // =========  COMMON HANDLING (similar to Android.HandleIntent)  =========

        private void HandleNotificationTap(NSDictionary userInfo, bool appRunning)
        {
            var title = GetFirstValue(userInfo,
                "notification_title",
                Constants.NOTIFICATION_TITLE,
                "title");

            var bodyJson = GetFirstValue(userInfo,
                "notification_body",
                Constants.NOTIFICATION_BODY,
                "payload");

            if (string.IsNullOrEmpty(bodyJson))
                return;

            var notificationData = new NotificationData
            {
                title = string.IsNullOrWhiteSpace(title) ? "SaleTop" : title,
                body = bodyJson
            };

            // mirror Android: only store in AppControl on cold start
            if (!appRunning)
            {
                AppService.Get<AppControl>().NotificationData = notificationData;
            }

            // always notify UI (same as MessagingCenter.Send in MainActivity)
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MessagingCenter.Send<object, NotificationData>(
                    this,
                    Constants.NOTIFICATION_BODY,
                    notificationData);
            });
        }

        /// <summary>
        /// Try to get a value from top-level or nested "data" like Android's GetFirstExtra.
        /// </summary>
        private static string? GetFirstValue(NSDictionary dict, params string[] keys)
        {
            if (dict == null) return null;

            // 1) Top-level keys
            foreach (var k in keys)
            {
                if (dict[k] != null)
                    return dict[k]?.ToString();
            }

            // 2) Nested "data" dictionary (FCM data payload pattern)
            if (dict["data"] is NSDictionary dataDict)
            {
                foreach (var k in keys)
                {
                    if (dataDict[k] != null)
                        return dataDict[k]?.ToString();
                }
            }

            // 3) Fallback: inside "aps" -> "alert" (if backend is sending like APNs)
            if (dict["aps"] is NSDictionary aps)
            {
                if (aps["alert"] is NSDictionary alert)
                {
                    foreach (var k in keys)
                    {
                        if (alert[k] != null)
                            return alert[k]?.ToString();
                    }
                }
                else if (aps["alert"] != null)
                {
                    // simple string "alert"
                    var s = aps["alert"].ToString();
                    if (!string.IsNullOrEmpty(s))
                        return s;
                }
            }

            return null;
        }
    }
}
