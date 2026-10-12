using System.Diagnostics;
using Location_Tracker.Models;
using Location_Tracker.Services;
using Microsoft.Maui.Maps;

namespace Location_Tracker
{
    /// <summary>
    /// Coordinates the Location Tracker user interface while delegating
    /// permission handling, location tracking, persistence, and heat map
    /// generation to their respective services.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private readonly LocationTrackingService _locationTrackingService = new();
        private readonly LocationDatabaseService _locationDatabase = new();
        private readonly HeatMapService _heatMapService = new();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                PermissionStatus status =
                    await LocationPermissionService
                        .CheckAndRequestLocationPermissionAsync();

                if (status != PermissionStatus.Granted)
                {
                    return;
                }

                bool trackingStarted =
                    await _locationTrackingService.StartTrackingAsync();

                if (!trackingStarted)
                {
                    Debug.WriteLine(
                        "Foreground location tracking did not start.");
                }

                List<LocationRecord> savedLocations =
                    await _locationDatabase.GetLocationsAsync();

                Debug.WriteLine(
                    $"Heat map data: {savedLocations.Count} " +
                    "saved location record(s) retrieved.");

                _heatMapService.RenderHeatMap(
                    LocationMap,
                    savedLocations);

                /*
                 * Center the map on the latest persisted reading so the
                 * user sees the most recently recorded area immediately.
                 */
                if (savedLocations.Count > 0)
                {
                    LocationRecord latestLocation =
                        savedLocations[^1];

                    Location center = new(
                        latestLocation.Latitude,
                        latestLocation.Longitude);

                    LocationMap.MoveToRegion(
                        MapSpan.FromCenterAndRadius(
                            center,
                            Distance.FromKilometers(1)));
                }
            }
            catch (Exception ex)
            {
                // Preserve the application session while recording
                // unexpected failures for debugging and diagnosis.
                Debug.WriteLine(
                    $"Location Tracker could not load normally: {ex.Message}");
            }
        }

        protected override void OnDisappearing()
        {
            _locationTrackingService.StopTracking();

            base.OnDisappearing();
        }
    }
}