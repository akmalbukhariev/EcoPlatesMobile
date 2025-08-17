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
using EcoPlatesMobile.Services;


namespace EcoPlatesMobile.Platforms.Android.Notification
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    internal class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
 
            if (!AppService.Get<AppStoreService>().Get(AppKeys.IsLoggedIn, false)) return;

            var notif = message.GetNotification();

            var title = message.Data.TryGetValue("title_text", out var t) ? t
                     : message.Data.TryGetValue("title", out var t2) ? t2
                     : "SaleTop";

            var bodyRaw = message.Data.TryGetValue("payload", out var p) ? p
                   :  message.Data.TryGetValue("body", out var b) ? b
                   :  notif?.Body; 
             
            new NotificationService().SendNotification(title, bodyRaw);
        }
    }
}
