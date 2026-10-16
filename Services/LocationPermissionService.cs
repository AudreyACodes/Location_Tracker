using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Manages the foreground location permission required by the
    /// Location Tracker application.
    /// </summary>
    public static class LocationPermissionService
    {
        /// <summary>
        /// Checks the current foreground location permission, explains
        /// why the permission is needed, requests access when necessary,
        /// and handles a denied request gracefully.
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

            Page? currentPage =
                Application.Current?
                    .Windows
                    .FirstOrDefault()?
                    .Page;

            if (currentPage is not null)
            {
                await currentPage.DisplayAlertAsync(
                    "Location Permission",
                    "Location Tracker uses your location to display your current position, save your location history, and calculate the distance you travel.",
                    "Continue");
            }

            status =
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted &&
                currentPage is not null)
            {
                await currentPage.DisplayAlertAsync(
                    "Location Permission Not Granted",
                    "Location access is required for tracking. The app will remain open, but location tracking will not start until permission is granted.",
                    "OK");
            }

            return status;
        }
    }
}