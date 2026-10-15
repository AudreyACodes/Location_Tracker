using Microsoft.Maui.Devices.Sensors;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Defines the location operations used by the application so
    /// real device location services can be replaced with a fake
    /// implementation during unit testing.
    /// </summary>
    public interface ILocationProvider
    {
        bool IsListening { get; }

        event Action<Location>? LocationChanged;

        event Action<string>? ListeningFailed;

        Task<bool> StartListeningAsync();

        void StopListening();
    }
}