using System.Diagnostics;
using Location_Tracker.Models;
using Location_Tracker.Services;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Location_Tracker
{
    public partial class MainPage : ContentPage
    {
        private readonly LocationTrackingService _locationTrackingService = new();
        private readonly LocationDatabaseService _locationDatabase = new();

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

            // Start listening for multiple foreground location updates.
            await _locationTrackingService.StartTrackingAsync();

            // Retrieve previously saved locations from SQLite.
            List<LocationRecord> savedLocations =
                await _locationDatabase.GetLocationsAsync();

            Debug.WriteLine(
                $"Map data: {savedLocations.Count} saved location record(s) retrieved.");

            DisplaySavedLocations(savedLocations);
        }

        private void DisplaySavedLocations(List<LocationRecord> savedLocations)
        {
            LocationMap.Pins.Clear();

            foreach (LocationRecord record in savedLocations)
            {
                // Convert the stored latitude and longitude into the
                // Location object required by the .NET MAUI Map control.
                Microsoft.Maui.Devices.Sensors.Location mapLocation =
                    new(record.Latitude, record.Longitude);

                Pin pin = new()
                {
                    Label = "Saved Location",
                    Address = record.Timestamp.ToLocalTime().ToString("g"),
                    Type = PinType.Place,
                    Location = mapLocation
                };

                LocationMap.Pins.Add(pin);
            }

            // Center the map on the most recently recorded location
            // instead of leaving the map at its default Maui, Hawaii region.
            if (savedLocations.Count > 0)
            {
                LocationRecord latestLocation = savedLocations[^1];

                Microsoft.Maui.Devices.Sensors.Location center =
                    new(latestLocation.Latitude, latestLocation.Longitude);

                LocationMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        center,
                        Distance.FromKilometers(1)));
            }
        }

        protected override void OnDisappearing()
        {
            _locationTrackingService.StopTracking();

            base.OnDisappearing();
        }
    }
}