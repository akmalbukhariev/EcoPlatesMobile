using Android.App;
using Android.Content;
using AndroidX.Core.App;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Platforms.Android.Notification;
using EcoPlatesMobile.Services;

[assembly: Dependency(typeof(NotificationService))]
namespace EcoPlatesMobile.Platforms.Android.Notification
{
    public class NotificationService : INotificationService
    {
        public void SendNotification(string title, string bodyJson)
        {
            var context = global::Android.App.Application.Context;

            var parsedObj = Newtonsoft.Json.JsonConvert.DeserializeObject<NewPosterPushNotificationResponse>(bodyJson); 

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
                .SetContentText(parsedObj.new_poster_name)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetPriority((int)NotificationPriority.High);

            NotificationManagerCompat.From(context).Notify(MainActivity.NotificationID, builder.Build());
        }
    }
}