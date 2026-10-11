using Microsoft.Maui.Devices.Sensors;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Retrieves the device's current geographic location using
    /// the .NET MAUI geolocation API.
    /// </summary>
    public class GeolocationService
    {
        public async Task<Location?> GetCurrentLocationAsync()
        {
            GeolocationRequest request = new(
                GeolocationAccuracy.High,
                TimeSpan.FromSeconds(10));

            return await Geolocation.Default.GetLocationAsync(request);
        }
    }
}