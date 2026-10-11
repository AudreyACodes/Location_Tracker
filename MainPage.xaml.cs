using Location_Tracker.Services;

namespace Location_Tracker
{
    public partial class MainPage : ContentPage
    {
        private readonly LocationTrackingService _locationTrackingService = new();

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
        }

        protected override void OnDisappearing()
        {
            // Stop foreground tracking when the page is no longer visible
            // so the app does not continue unnecessary location work.
            _locationTrackingService.StopTracking();

            base.OnDisappearing();
        }
    }
}