using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Firebase.Messaging;
using Android.Util;
using AndroidX.Core.App;
using Android.App;
using Android.Content;
using EcoPlatesMobile.Models.Responses.User;


namespace EcoPlatesMobile.Platforms.Android.Notification
{
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    internal class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            var title = message.Data["title"];
            var bodyJson = message.Data["body"];

            // parse JSON
            var parsed = Newtonsoft.Json.JsonConvert.DeserializeObject<NewPosterPushNotificationResponse>(bodyJson);
            var newPosterName = parsed.new_poster_name;

            new NotificationService().SendNotification(title, newPosterName);
        }

        
    }
}
