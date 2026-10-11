using System.Diagnostics;
using Location_Tracker.Models;
using Location_Tracker.Services;
using Microsoft.Maui.Maps;

namespace Location_Tracker
{
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

            // Location tracking can begin only after the user grants
            // foreground location permission.
            PermissionStatus status =
                await LocationPermissionService.CheckAndRequestLocationPermissionAsync();

            if (status != PermissionStatus.Granted)
            {
                return;
            }

            // Start collecting foreground location updates.
            await _locationTrackingService.StartTrackingAsync();

            // Retrieve all saved readings so density can be calculated
            // from the user's accumulated location history.
            List<LocationRecord> savedLocations =
                await _locationDatabase.GetLocationsAsync();

            Debug.WriteLine(
                $"Heat map data: {savedLocations.Count} saved location record(s) retrieved.");

            // Convert stored coordinates into density-based map regions.
            _heatMapService.RenderHeatMap(
                LocationMap,
                savedLocations);

            /*
             * Center the map on the most recent stored location after
             * the heat-map regions have been rendered.
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

        protected override void OnDisappearing()
        {
            // Stop foreground tracking when this page is no longer visible.
            _locationTrackingService.StopTracking();

            base.OnDisappearing();
        }
    }
}