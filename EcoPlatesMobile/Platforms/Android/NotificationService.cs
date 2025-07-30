using Android.App;
using Android.Content;
using AndroidX.Core.App;

using EcoPlatesMobile.Services;

[assembly: Dependency(typeof(EcoPlatesMobile.Platforms.Android.NotificationService))]
namespace EcoPlatesMobile.Platforms.Android
{
    public class NotificationService : INotificationService
    {
        public void SendNotification(string title, string body)
        {   
            var context = global::Android.App.Application.Context;

            var intent = new Intent(context, Java.Lang.Class.FromType(typeof(MainActivity)));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

            string encodedBody = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(body));

            intent.PutExtra("notification_tapped", true);
            intent.PutExtra("title", title);
            intent.PutExtra("body", encodedBody);

            var pendingIntent = PendingIntent.GetActivity(
                context,
                MainActivity.NotificationID,
                intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var builder = new NotificationCompat.Builder(context, MainActivity.Channel_ID)
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetPriority((int)NotificationPriority.High);

            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager.Notify(MainActivity.NotificationID, builder.Build());
        }
    }
}