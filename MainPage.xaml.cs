using System.Diagnostics;
using Location_Tracker.Models;
using Location_Tracker.Services;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
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
                    CurrentPositionLabel.Text =
                        "Current Position: Location permission not granted.";

                    return;
                }

                await UpdateCurrentPositionAsync();

                List<LocationRecord> savedLocations =
                    await _locationDatabase.GetLocationsAsync();

                Debug.WriteLine(
                    $"Location history loaded: {savedLocations.Count} saved record(s).");

                UpdateDistanceDisplay(savedLocations);

                if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    // Windows uses a list because the Windows MAUI map
                    // requires an Azure Maps resource that is unavailable
                    // under the current student subscription regions.
                    LocationHistoryList.ItemsSource = savedLocations;
                }
                else
                {
                    // Android uses the map and heat-map visualization.
                    _heatMapService.RenderHeatMap(
                        LocationMap,
                        savedLocations);

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

            await UpdateCurrentPositionAsync();
            await RefreshSavedLocationDisplayAsync();
        }

        private async Task UpdateCurrentPositionAsync()
        {
            try
            {
                GeolocationRequest request =
                    new(
                        GeolocationAccuracy.High,
                        TimeSpan.FromSeconds(10));

                Location? currentLocation =
                    await Geolocation.GetLocationAsync(request);

                if (currentLocation is null)
                {
                    CurrentPositionLabel.Text =
                        "Current Position: Not available";

                    return;
                }

                CurrentPositionLabel.Text =
                    $"Current Position: " +
                    $"{currentLocation.Latitude:F6}, " +
                    $"{currentLocation.Longitude:F6}";
            }
            catch (Exception ex)
            {
                CurrentPositionLabel.Text =
                    "Current Position: Not available";

                Debug.WriteLine(
                    $"Current position could not be retrieved: {ex.Message}");
            }
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

        private async Task RefreshSavedLocationDisplayAsync()
        {
            try
            {
                List<LocationRecord> savedLocations =
                    await _locationDatabase.GetLocationsAsync();

                UpdateDistanceDisplay(savedLocations);

                if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    LocationHistoryList.ItemsSource = null;
                    LocationHistoryList.ItemsSource = savedLocations;
                }
                else
                {
                    _heatMapService.RenderHeatMap(
                        LocationMap,
                        savedLocations);

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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Saved location display could not be refreshed: {ex.Message}");
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