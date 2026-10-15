using Microsoft.Maui.Devices.Sensors;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Provides access to the real .NET MAUI foreground
    /// geolocation service used by the application.
    /// </summary>
    public class MauiLocationProvider : ILocationProvider
    {
        private bool _handlersRegistered;

        public bool IsListening =>
            Geolocation.IsListeningForeground;

        public event Action<Location>? LocationChanged;

        public event Action<string>? ListeningFailed;

        public async Task<bool> StartListeningAsync()
        {
            try
            {
                if (Geolocation.IsListeningForeground)
                {
                    return true;
                }

                RegisterHandlers();

                GeolocationListeningRequest request =
                    new(GeolocationAccuracy.High);

                bool started =
                    await Geolocation
                        .StartListeningForegroundAsync(request);

                if (!started)
                {
                    UnregisterHandlers();
                }

                return started;
            }
            catch
            {
                UnregisterHandlers();
                return false;
            }
        }

        public void StopListening()
        {
            try
            {
                if (Geolocation.IsListeningForeground)
                {
                    Geolocation.StopListeningForeground();
                }
            }
            finally
            {
                UnregisterHandlers();
            }
        }

        private void RegisterHandlers()
        {
            if (_handlersRegistered)
            {
                return;
            }

            Geolocation.LocationChanged += OnLocationChanged;
            Geolocation.ListeningFailed += OnListeningFailed;

            _handlersRegistered = true;
        }

        private void UnregisterHandlers()
        {
            if (!_handlersRegistered)
            {
                return;
            }

            Geolocation.LocationChanged -= OnLocationChanged;
            Geolocation.ListeningFailed -= OnListeningFailed;

            _handlersRegistered = false;
        }

        private void OnLocationChanged(
            object? sender,
            GeolocationLocationChangedEventArgs e)
        {
            LocationChanged?.Invoke(e.Location);
        }

        private void OnListeningFailed(
            object? sender,
            GeolocationListeningFailedEventArgs e)
        {
            ListeningFailed?.Invoke(e.Error.ToString());
        }
    }
}