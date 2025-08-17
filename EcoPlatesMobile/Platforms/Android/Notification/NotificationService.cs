using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using EcoPlatesMobile.Models.Company;
using EcoPlatesMobile.Models.Responses.Notification;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Platforms.Android.Notification;
using EcoPlatesMobile.Services;
using Newtonsoft.Json.Linq;

[assembly: Dependency(typeof(NotificationService))]
namespace EcoPlatesMobile.Platforms.Android.Notification
{
    public class NotificationService : INotificationService
    {
        private UserSessionService userSessionService;
        private AppControl appControl;
        public NotificationService()
        {
            userSessionService = AppService.Get<UserSessionService>();
            appControl = AppService.Get<AppControl>();
        }

        public void SendNotification(string title, string bodyRaw)
        {
            var context = global::Android.App.Application.Context;

            EnsureChannel(context, MainActivity.Channel_ID, "SaleTop Messages");

            string content = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(bodyRaw))
                {
                    // If it's JSON, parse it; else use as-is
                    var looksJson = bodyRaw.TrimStart().StartsWith("{");
                    if (looksJson)
                    {
                        var o = JObject.Parse(bodyRaw);
                        var type = (o["notificationType"]?.ToString() ?? "").ToUpperInvariant();

                        if (type == "NEW_MESSAGE")
                        {
                            int? senderId = o["sender_id"]?.Type == JTokenType.Integer
                                ? o.Value<int?>("sender_id")
                                : TryParseInt(o["sender_id"]?.ToString());

                            var userInfo = appControl.UserInfo;
                            var companyInfo = appControl.CompanyInfo;
                            if (senderId.HasValue)
                            {
                                if (userInfo != null && userInfo.user_id == senderId.Value) return;
                                if (companyInfo != null && companyInfo.company_id == senderId.Value) return;
                            }

                            content = o["message"]?.ToString();
                        }
                        else if (type == "NEW_POSTER")
                        {
                            if (userSessionService.Role == UserRole.Company) return;
                            content = o["new_poster_name"]?.ToString();
                        }
                    }
                    else
                    {
                        // Plain text (e.g., from notification.body)
                        content = bodyRaw;
                    }
                }
            }
            catch { /* ignore and fall back */ }

            if (string.IsNullOrWhiteSpace(content))
                content = "New message"; // guaranteed non-empty, renders on all skins

            var intent = new Intent(context, typeof(MainActivity))
                .AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            intent.PutExtra(Utilities.Constants.NOTIFICATION_TITLE, string.IsNullOrWhiteSpace(title) ? "SaleTop" : title);
            intent.PutExtra(Utilities.Constants.NOTIFICATION_BODY, bodyRaw ?? "");

            var pendingIntent = PendingIntent.GetActivity(
                context, MainActivity.NotificationID, intent,
                PendingIntentFlags.Immutable | PendingIntentFlags.OneShot);

            var builder = new NotificationCompat.Builder(context, MainActivity.Channel_ID)
                .SetSmallIcon(Resource.Drawable.notification_icon) // white-only drawable
                .SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Mipmap.appicon))
                .SetContentTitle(string.IsNullOrWhiteSpace(title) ? "SaleTop" : title)
                .SetContentText(content)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(content))
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .SetCategory(NotificationCompat.CategoryMessage)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            NotificationManagerCompat.From(context).Notify(MainActivity.NotificationID, builder.Build());
        }
        static void EnsureChannel(Context ctx, string channelId, string channelName)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var nm = (NotificationManager)ctx.GetSystemService(Context.NotificationService);
                if (nm.GetNotificationChannel(channelId) == null)
                {
                    var ch = new NotificationChannel(channelId, channelName, NotificationImportance.High)
                    {
                        LockscreenVisibility = NotificationVisibility.Public
                    };
                    ch.EnableVibration(true);
                    ch.EnableLights(true);
                    nm.CreateNotificationChannel(ch);
                }
            }
        }

        static int? TryParseInt(string s)
            => int.TryParse(s, out var v) ? v : (int?)null;

        /*
        public void SendNotification(string title, string bodyJson)
        {
            var context = global::Android.App.Application.Context;

            var jObject = JObject.Parse(bodyJson);
            var notificationTypeValue = jObject["notificationType"]?.ToString();

            if (!Enum.TryParse(notificationTypeValue, out NotificationType notificationType))
            {
                Console.WriteLine("Unknown notification type.");
                return;
            }

            string strContent = string.Empty;
            switch (notificationType)
            {
                case NotificationType.NEW_POSTER:
                    if (userSessionService.Role == UserRole.Company) return;

                    var posterData = jObject.ToObject<NewPosterPushNotificationResponse>();
                    strContent = posterData.new_poster_name;
                    break;

                case NotificationType.NEW_MESSAGE:
                    var messageData = jObject.ToObject<NewMessagePushNotificationResponse>();
                    strContent = messageData.message;
                    break;

                default:
                    Console.WriteLine("Unhandled notification type.");
                    break;
            }

            var intent = new Intent(context, typeof(MainActivity));
            intent.PutExtra(Utilities.Constants.NOTIFICATION_TITLE, title);
            intent.PutExtra(Utilities.Constants.NOTIFICATION_BODY, bodyJson);
            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop | ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(
                context,
                MainActivity.NotificationID,
                intent,
                PendingIntentFlags.Immutable | PendingIntentFlags.OneShot);

            var builder = new NotificationCompat.Builder(context, MainActivity.Channel_ID)
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetContentTitle(title)
                .SetContentText(strContent)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetPriority((int)NotificationPriority.High);

            NotificationManagerCompat.From(context).Notify(MainActivity.NotificationID, builder.Build());
        }
        */
    }
}