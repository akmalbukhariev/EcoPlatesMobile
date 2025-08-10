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
                // 1) Check if device location services (GPS) are ON  → show "A" message if OFF
        #if ANDROID
                var lm = (Android.Locations.LocationManager)Android.App.Application.Context
                    .GetSystemService(Android.Content.Context.LocationService);

                bool isGpsOn = lm.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider)
                            || lm.IsProviderEnabled(Android.Locations.LocationManager.NetworkProvider);

                if (!isGpsOn)
                {
                    // A message (GPS/location services OFF)
                    bool openLocationSettings = await AlertService.ShowConfirmationAsync(
                        "Location Services Off",//AppResource.LocationServicesOffTitle
                        "Turn on Location Services in Settings to use the map.",//AppResource.MessageLocationServicesOff
                        AppResource.OpenSettings,                       
                        AppResource.Cancel);

                    if (openLocationSettings)
                    {
                        // Opens the device's Location settings (works on Huawei too)
                        var intent = new Android.Content.Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                        intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                        Android.App.Application.Context.StartActivity(intent);
                    }
                    return null;
                }
        #endif

                // 2) Check permission  → show "B" message if NOT granted
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Denied)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status != PermissionStatus.Granted)
                {
                    // B message (permission missing)
                    bool openAppSettings = await AlertService.ShowConfirmationAsync(
                        "Location Permission Required",//AppResource.LocationPermissionRequiredTitle
                        AppResource.MessageLocationPermission,         // e.g., "Allow location in App Settings to use map features."
                        AppResource.OpenSettings,
                        AppResource.Cancel);

                    if (openAppSettings)
                    {
                        AppInfo.ShowSettingsUI(); // Opens app details screen
                    }
                    return null;
                }

                // 3) Try to get location
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                return location ?? await Geolocation.GetLastKnownLocationAsync();
            }
            catch (FeatureNotEnabledException)
            {
                // Extra safety: if we reach here, GPS is off — show "A" message
                await AlertService.ShowAlertAsync(
                    //AppResource.LocationServicesOffTitle,
                    //AppResource.MessageLocationServicesOff,
                    AppResource.Ok);
                return null;
            }
            catch (PermissionException)
            {
                // Extra safety: permission issue — show "B" message
                await AlertService.ShowAlertAsync(
                    AppResource.LocationPermissionRequired,
                    AppResource.MessageLocationPermission,
                    AppResource.Ok);
                return null;
            }
            catch (Exception)
            {
                await AlertService.ShowAlertAsync(
                    AppResource.LocationPermissionRequired,
                    AppResource.MessageLocationPermission,
                    AppResource.Ok);
                return null;
            }
        }

        /*
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
        /*
    }
}
