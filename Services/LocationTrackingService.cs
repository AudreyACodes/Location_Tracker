using System.Diagnostics;
using Location_Tracker.Models;
using Microsoft.Maui.Devices.Sensors;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Listens for foreground location changes and saves each
    /// reported position to the local SQLite database.
    /// </summary>
    public class LocationTrackingService
    {
        private readonly LocationDatabaseService _locationDatabase = new();
        private bool _handlersRegistered;

        public async Task<bool> StartTrackingAsync()
        {
            try
            {
                if (Geolocation.IsListeningForeground)
                {
                    return true;
                }

                // Register the event handlers once so each location update
                // is processed only once.
                if (!_handlersRegistered)
                {
                    Geolocation.LocationChanged += OnLocationChanged;
                    Geolocation.ListeningFailed += OnListeningFailed;
                    _handlersRegistered = true;
                }

                // Foreground listening provides multiple location readings
                // as the device position changes.
                GeolocationListeningRequest request =
                    new(GeolocationAccuracy.High);

                bool started =
                    await Geolocation.StartListeningForegroundAsync(request);

                if (!started)
                {
                    UnregisterHandlers();
                }

                return started;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Location tracking could not start: {ex.Message}");

                UnregisterHandlers();
                return false;
            }
        }

        public void StopTracking()
        {
            try
            {
                if (Geolocation.IsListeningForeground)
                {
                    Geolocation.StopListeningForeground();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Location tracking could not stop normally: {ex.Message}");
            }
            finally
            {
                UnregisterHandlers();
            }
        }

        private async void OnLocationChanged(
            object? sender,
            GeolocationLocationChangedEventArgs e)
        {
            try
            {
                // Convert the location reported by .NET MAUI into the
                // application's SQLite location-record model.
                LocationRecord locationRecord = new()
                {
                    Latitude = e.Location.Latitude,
                    Longitude = e.Location.Longitude,
                    Timestamp = e.Location.Timestamp.UtcDateTime
                };

                await _locationDatabase.SaveLocationAsync(locationRecord);

                Debug.WriteLine(
                    $"Location saved: {locationRecord.Latitude}, " +
                    $"{locationRecord.Longitude}");
            }
            catch (Exception ex)
            {
                // A failed database write must not crash location tracking.
                Debug.WriteLine(
                    $"Location could not be saved: {ex.Message}");
            }
        }

        private void OnListeningFailed(
            object? sender,
            GeolocationListeningFailedEventArgs e)
        {
            // Record a normal geolocation failure without terminating the app.
            Debug.WriteLine(
                $"Location listening failed: {e.Error}");
        }

        private void UnregisterHandlers()
        {
            if (!_handlersRegistered)
            {
                return;
            }

            Geolocation.LocationChanged -= OnLocationChanged;
            Geolocation.ListeningFailed -= OnListeningFailed;
            _handlersRegistered = false;
        }
    }
}
