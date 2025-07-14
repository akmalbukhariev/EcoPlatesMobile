using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoPlatesMobile.Resources.Languages;

namespace EcoPlatesMobile.Services
{
    public class LocationService
    {
        public async Task<Location?> GetCurrentLocationAsync()
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
                Console.WriteLine($"Location error: {ex.Message}");
                return null;
            }
        }
    }
}
