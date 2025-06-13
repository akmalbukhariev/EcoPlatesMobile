using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Services
{
    public class LocationService
    {
        public async Task<Location?> GetCurrentLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                    return location;

                // Try last known location if current is null
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
