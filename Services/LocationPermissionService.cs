using Microsoft.Maui.ApplicationModel;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Manages the foreground location permission required by the
    /// Location Tracker application.
    /// </summary>
    public static class LocationPermissionService
    {
        /// <summary>
        /// Checks the current foreground location permission and requests
        /// access only when permission has not already been granted.
        /// </summary>
        /// <returns>
        /// The resulting location permission status.
        /// </returns>
        public static async Task<PermissionStatus> CheckAndRequestLocationPermissionAsync()
        {
            PermissionStatus status =
                await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
            {
                return status;
            }

            status =
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status;
        }
    }
}