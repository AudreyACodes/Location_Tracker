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

        /// <summary>
        /// Starts foreground geolocation monitoring and registers the
        /// event handlers used to process location updates.
        /// </summary>
        /// <returns>
        /// True when foreground location monitoring starts successfully;
        /// otherwise, false.
        /// </returns>
        public async Task<bool> StartTrackingAsync()
        {
            try
            {
                if (Geolocation.IsListeningForeground)
                {
                    return true;
                }

                // Register handlers once so a location update is not
                // processed multiple times by duplicate subscriptions.
                if (!_handlersRegistered)
                {
                    Geolocation.LocationChanged += OnLocationChanged;
                    Geolocation.ListeningFailed += OnListeningFailed;
                    _handlersRegistered = true;
                }

                // High-accuracy foreground monitoring supplies the sequence
                // of readings used to build the stored location history.
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

        /// <summary>
        /// Stops foreground location monitoring and removes registered
        /// geolocation event handlers.
        /// </summary>
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
                // Convert the MAUI location reading into the application's
                // persistent SQLite model before saving it.
                LocationRecord locationRecord = new()
                {
                    Latitude = e.Location.Latitude,
                    Longitude = e.Location.Longitude,
                    Timestamp = e.Location.Timestamp.UtcDateTime
                };

                await _locationDatabase.SaveLocationAsync(locationRecord);

                // Do not write sensitive latitude or longitude values
                // to diagnostic logs.
                Debug.WriteLine(
                    "Location record saved successfully.");
            }
            catch (Exception ex)
            {
                // A failed database write should not terminate
                // foreground location monitoring.
                Debug.WriteLine(
                    $"Location could not be saved: {ex.Message}");
            }
        }

        private void OnListeningFailed(
            object? sender,
            GeolocationListeningFailedEventArgs e)
        {
            // Preserve application execution while recording the
            // geolocation failure for diagnostic purposes.
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