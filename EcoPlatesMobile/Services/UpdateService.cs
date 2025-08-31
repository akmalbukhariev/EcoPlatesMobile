using System.Text.Json;
using System.Text.RegularExpressions;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

public class UpdateService : IUpdateService
{
    private readonly string jsonUrl;
    private readonly AppStoreService store;
    private AppControl appControl;

    private const string LastCheckKey = "lastUpdateCheckUtc";

    public UpdateService(AppStoreService storeService, AppControl appControl)
    {
        jsonUrl = $"http://{Constants.SERVER_DOMAIN}/latest.json";
        store = storeService;
        this.appControl = appControl;
    }

    public async Task CheckAndPromptAsync()
    {
        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
            var raw = await http.GetStringAsync(jsonUrl);
            using var doc = JsonDocument.Parse(raw);

            var platformKey = DeviceInfo.Platform == DevicePlatform.iOS ? "ios" : "android";
            if (!doc.RootElement.TryGetProperty(platformKey, out var cfg)) return;

            var latestStr = cfg.GetProperty("latest").GetString();
            var force = cfg.TryGetProperty("force", out var f) && f.GetBoolean();
            var storeUrl = cfg.GetProperty("storeUrl").GetString();

            if (string.IsNullOrWhiteSpace(latestStr) || string.IsNullOrWhiteSpace(storeUrl))
                return;

            var current = ParseVersionSafe(AppInfo.Current.VersionString);
            var latest = ParseVersionSafe(latestStr);

            bool hasUpdate = current < latest;
            if (!hasUpdate) return;

            if (force)
            {
                appControl.UpdatePending = true;

                await AlertService.ShowAlertAsync("Update Required",
                    "To continue using the app, please install the latest update.",
                    "Update");

                await Launcher.OpenAsync(new Uri(storeUrl));
            }
            else
            {
                appControl.UpdatePending = false;

                var go = await AlertService.ShowConfirmationAsync("Update Available",
                    "A new update is available. Would you like to update the app now?",
                    "Update", "Later");

                if (go)
                    await Launcher.OpenAsync(new Uri(storeUrl));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /* once per day
    public async Task CheckAndPromptAsync()
    {
        var lastCheck = store.Get<DateTime>(LastCheckKey, DateTime.MinValue);
        if (lastCheck > DateTime.UtcNow.AddDays(-1))
            return;

        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
            var raw = await http.GetStringAsync(jsonUrl);
            using var doc = JsonDocument.Parse(raw);

            var platformKey = DeviceInfo.Platform == DevicePlatform.iOS ? "ios" : "android";
            if (!doc.RootElement.TryGetProperty(platformKey, out var cfg)) return;

            var latestStr = cfg.GetProperty("latest").GetString();
            var force = cfg.TryGetProperty("force", out var f) && f.GetBoolean();
            var storeUrl = cfg.GetProperty("storeUrl").GetString();

            if (string.IsNullOrWhiteSpace(latestStr) || string.IsNullOrWhiteSpace(storeUrl))
                return;

            var current = ParseVersionSafe(AppInfo.Current.VersionString);
            var latest = ParseVersionSafe(latestStr);

            bool hasUpdate = current < latest;

            if (!hasUpdate)
            {
                store.Set(LastCheckKey, DateTime.UtcNow);
                return;
            }

            string title;
            string msg;

            if (force)
            {
                title = "Update Required";
                msg = "To continue using the app, please install the latest update.";

                await AlertService.ShowAlertAsync(title, msg, "Update");
                await Launcher.OpenAsync(new Uri(storeUrl));
            }
            else
            {
                title = "Update Available";
                msg = "A new update is available. Would you like to update the app now?";

                bool go = await AlertService.ShowConfirmationAsync(title, msg, "Update", "Later");
                if (go)
                    await Launcher.OpenAsync(new Uri(storeUrl));
            }

            store.Set(LastCheckKey, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    */

    private static Version ParseVersionSafe(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return new Version(0, 0, 0);
        var m = Regex.Match(v, @"\d+(\.\d+){0,3}");
        if (m.Success)
        {
            var core = m.Value;
            var parts = core.Split('.');
            if (parts.Length == 1) core += ".0";
            return Version.Parse(core);
        }
        return new Version(0, 0, 0);
    }
}