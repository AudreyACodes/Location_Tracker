using Microsoft.Maui.ApplicationModel;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Handles checking and requesting the location permission required
    /// by the Location Tracker application.
    /// </summary>
    public static class LocationPermissionService
    {
        public static async Task<PermissionStatus> CheckAndRequestLocationPermissionAsync()
        {
            // Check first so the app does not repeatedly prompt a user
            // who has already granted location access.
            PermissionStatus status =
                await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
            {
                return status;
            }

            // Request foreground location access when permission
            // has not already been granted.
            status =
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status;
        }
    }
}