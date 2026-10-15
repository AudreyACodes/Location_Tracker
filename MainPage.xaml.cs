using System.Diagnostics;
using Location_Tracker.Models;
using Location_Tracker.Services;
using Microsoft.Maui.Maps;

namespace Location_Tracker
{
    /// <summary>
    /// Coordinates the Location Tracker user interface while delegating
    /// permission handling, location tracking, persistence, heat map
    /// generation, and distance calculations to their respective services.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private readonly LocationTrackingService _locationTrackingService = new();
        private readonly LocationDatabaseService _locationDatabase = new();
        private readonly HeatMapService _heatMapService = new();
        private readonly DistanceStatisticsService _distanceStatisticsService = new();

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

                List<LocationRecord> savedLocations =
                    await _locationDatabase.GetLocationsAsync();

                Debug.WriteLine(
                    $"Heat map data: {savedLocations.Count} " +
                    "saved location record(s) retrieved.");

                _heatMapService.RenderHeatMap(
                    LocationMap,
                    savedLocations);

                UpdateDistanceDisplay(savedLocations);

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

                StartTrackingButton.IsEnabled = true;
                StopTrackingButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Location Tracker could not load normally: {ex.Message}");
            }
        }

        private async void OnStartTrackingClicked(
            object sender,
            EventArgs e)
        {
            try
            {
                PermissionStatus status =
                    await LocationPermissionService
                        .CheckAndRequestLocationPermissionAsync();

                if (status != PermissionStatus.Granted)
                {
                    Debug.WriteLine(
                        "Location permission was not granted.");

                    return;
                }

                bool trackingStarted =
                    await _locationTrackingService.StartTrackingAsync();

                if (trackingStarted)
                {
                    StartTrackingButton.IsEnabled = false;
                    StopTrackingButton.IsEnabled = true;

                    Debug.WriteLine(
                        "Location tracking started.");
                }
                else
                {
                    Debug.WriteLine(
                        "Foreground location tracking did not start.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Location tracking could not start: {ex.Message}");
            }
        }

        private async void OnStopTrackingClicked(
            object sender,
            EventArgs e)
        {
            _locationTrackingService.StopTracking();

            StartTrackingButton.IsEnabled = true;
            StopTrackingButton.IsEnabled = false;

            Debug.WriteLine(
                "Location tracking stopped.");

            await RefreshDistanceDisplayAsync();
        }

        private void UpdateDistanceDisplay(
            List<LocationRecord> locations)
        {
            double totalDistanceKilometers =
                _distanceStatisticsService
                    .CalculateTotalDistanceKilometers(locations);

            double totalDistanceMiles =
                totalDistanceKilometers * 0.621371;

            DistanceLabel.Text =
                $"Total Distance: {totalDistanceMiles:F2} mi";
        }

        private async Task RefreshDistanceDisplayAsync()
        {
            try
            {
                List<LocationRecord> savedLocations =
                    await _locationDatabase.GetLocationsAsync();

                UpdateDistanceDisplay(savedLocations);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Distance statistics could not be updated: {ex.Message}");
            }
        }

        protected override void OnDisappearing()
        {
            _locationTrackingService.StopTracking();

            StartTrackingButton.IsEnabled = true;           
            StopTrackingButton.IsEnabled = false;

            base.OnDisappearing();
        }
    }
}