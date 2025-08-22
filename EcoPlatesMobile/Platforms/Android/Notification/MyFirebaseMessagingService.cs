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
using Android.Runtime;
using Android.OS;

/*
namespace EcoPlatesMobile.Platforms.Android.Notification
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    internal class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);


            new NotificationService().SendNotification("ZZZ TEST TITLE", "ZZZ TEST BODY");
        }
    }
}
*/
