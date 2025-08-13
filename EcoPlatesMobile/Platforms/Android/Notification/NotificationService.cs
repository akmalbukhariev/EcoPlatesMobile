using Android.App;
using Android.Content;
using Android.Graphics;
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

        public void SendNotification(string title, string bodyJson)
        {
            var context = global::Android.App.Application.Context;

            string content = null;
            try
            {
                UserInfo userInfo = appControl.UserInfo;
                CompanyInfo companyInfo = appControl.CompanyInfo;

                if (!string.IsNullOrEmpty(bodyJson))
                {
                    var jObject = JObject.Parse(bodyJson);
                    var type = jObject["notificationType"]?.ToString();

                    if (type == nameof(NotificationType.NEW_MESSAGE))
                    {
                        var messageData = jObject.ToObject<NewMessagePushNotificationResponse>();

                        if (userInfo != null && userInfo.user_id == messageData.sender_id) return;
                        if (companyInfo != null && companyInfo.company_id == messageData.sender_id) return;

                        content = jObject["message"]?.ToString();
                    }
                    else if (type == nameof(NotificationType.NEW_POSTER))
                    {
                        if (userSessionService.Role == UserRole.Company) return;

                        content = jObject["new_poster_name"]?.ToString();
                    }
                }
            }
            catch { /* keep content null; we'll show a generic text */ }

            if (string.IsNullOrEmpty(content)) content = "You have a new notification";

            var intent = new Intent(context, typeof(MainActivity))
                .AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            intent.PutExtra(Utilities.Constants.NOTIFICATION_TITLE, title);
            intent.PutExtra(Utilities.Constants.NOTIFICATION_BODY, bodyJson ?? "");

            var pendingIntent = PendingIntent.GetActivity(
                context, MainActivity.NotificationID, intent,
                PendingIntentFlags.Immutable | PendingIntentFlags.OneShot);

            var builder = new NotificationCompat.Builder(context, MainActivity.Channel_ID)
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Mipmap.appicon))
                .SetContentTitle(title)
                .SetContentText(content)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(content))
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetPriority((int)NotificationPriority.High);

            NotificationManagerCompat.From(context).Notify(MainActivity.NotificationID, builder.Build());
        }

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