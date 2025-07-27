using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoPlatesMobile.Resources.Languages;

#if ANDROID
using Android.Content;
using Android.Locations;
#endif

#if IOS
using CoreLocation;
#endif

namespace EcoPlatesMobile.Services
{
    public class LocationService
    {
        public async Task<bool> IsLocationEnabledAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                return false;

            bool enable = IsLocationEnabled();

            if (!enable)
            {
                bool openSettings = await AlertService.ShowConfirmationAsync(
                            AppResource.LocationPermissionRequired,
                            AppResource.MessageLocationPermission,
                            AppResource.OpenSettings,
                            AppResource.Cancel);

                if (openSettings)
                {
                    AppInfo.ShowSettingsUI();
                }
                return false;
            }

            return true;
        }

        private bool IsLocationEnabled()
        {
#if ANDROID
            var locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            return locationManager?.IsProviderEnabled(LocationManager.GpsProvider) == true ||
                   locationManager?.IsProviderEnabled(LocationManager.NetworkProvider) == true;
#elif IOS
            return CLLocationManager.LocationServicesEnabled;
#else
            return true;
#endif
        }

        public async Task<Microsoft.Maui.Devices.Sensors.Location?> GetCurrentLocationAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Denied)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Denied || status == PermissionStatus.Disabled)
                {
                    bool openSettings = await AlertService.ShowConfirmationAsync(
                        AppResource.LocationPermissionRequired,
                        AppResource.MessageLocationPermission,
                        AppResource.OpenSettings,
                        AppResource.Cancel);

                    if (openSettings)
                    {
                        AppInfo.ShowSettingsUI();
                    }

                    return null;
                }

                if (status != PermissionStatus.Granted)
                {
                    return null;
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                    return location;

                return await Geolocation.GetLastKnownLocationAsync();
            }
            catch (Exception ex)
            {
                await AlertService.ShowAlertAsync(AppResource.LocationPermissionRequired, AppResource.MessageLocationPermission, AppResource.Ok);
                return null;
            }
        }
    }
}
