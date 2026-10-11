using Location_Tracker.Services;

namespace Location_Tracker
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Request location access after the page appears so Android
            // can display its runtime permission prompt to the user.
            PermissionStatus status =
                await LocationPermissionService.CheckAndRequestLocationPermissionAsync();

            // Stop location-dependent processing safely when permission
            // has not been granted.
            if (status != PermissionStatus.Granted)
            {
                return;
            }
        }
    }
}