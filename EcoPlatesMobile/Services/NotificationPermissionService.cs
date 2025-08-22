#if ANDROID
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using EcoPlatesMobile.Resources.Languages;
using Microsoft.Maui.ApplicationModel;
#endif

public static class NotificationPermissionHelper
{
    public static async Task<bool> EnsureEnabledAsync(Page hostPage)
    {
#if ANDROID
        var ctx = Platform.AppContext;

        // First: are notifications enabled for this app (covers channels too)
        bool enabled = NotificationManagerCompat.From(ctx).AreNotificationsEnabled();

        // Android 13+ requires runtime permission
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();

            // Re-check after request
            enabled = status == PermissionStatus.Granted &&
                      NotificationManagerCompat.From(ctx).AreNotificationsEnabled();
        }

        if (enabled) return true;

        bool open = await hostPage.DisplayAlert(
            AppResource.NotificationOff,
            AppResource.MessageTurnOnNotification,
             AppResource.OpenSettings,
            AppResource.Cancel);

        if (open) OpenAndroidNotificationSettings();
        return false;

#elif IOS
        var center = UNUserNotificationCenter.Current;
        var settings = await center.GetNotificationSettingsAsync();

        bool isAuthorized = settings.AuthorizationStatus is
            UNAuthorizationStatus.Authorized or
            UNAuthorizationStatus.Provisional or
            UNAuthorizationStatus.Ephemeral;

        if (!isAuthorized && settings.AuthorizationStatus == UNAuthorizationStatus.NotDetermined)
        {
            var _ = await center.RequestAuthorizationAsync(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge);

            settings = await center.GetNotificationSettingsAsync();
            isAuthorized = settings.AuthorizationStatus is
                UNAuthorizationStatus.Authorized or
                UNAuthorizationStatus.Provisional or
                UNAuthorizationStatus.Ephemeral;
        }

        if (isAuthorized) return true;

        bool open = await hostPage.DisplayAlert(
            "Notifications are off",
            "Turn on notifications for this app in iOS Settings.",
            "Open Settings",
            "Cancel");

        if (open) OpeniOSAppSettings();
        return false;
#else
        return true;
#endif
    }

#if ANDROID
    static void OpenAndroidNotificationSettings()
    {
        var ctx = Platform.AppContext;
        Intent intent;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O) // API 26+
        {
            intent = new Intent(Android.Provider.Settings.ActionAppNotificationSettings)
                .PutExtra(Android.Provider.Settings.ExtraAppPackage, ctx.PackageName);
        }
        else
        {
            intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings)
                .SetData(Android.Net.Uri.Parse("package:" + ctx.PackageName));
        }

        intent.AddFlags(ActivityFlags.NewTask);
        ctx.StartActivity(intent);
    }
#endif

#if IOS
    static void OpeniOSAppSettings()
    {
        var url = new NSUrl(UIApplication.OpenSettingsUrlString);
        if (UIApplication.SharedApplication.CanOpenUrl(url))
            UIApplication.SharedApplication.OpenUrl(url);
    }
#endif
}