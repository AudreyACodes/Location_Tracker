using Location_Tracker.Models;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Calculates distance statistics from saved location history.
    /// </summary>
    public class DistanceStatisticsService
    {
        /// <summary>
        /// Calculates the total traveled distance in kilometers
        /// using consecutive saved location readings.
        /// </summary>
        public double CalculateTotalDistanceKilometers(
            List<LocationRecord> locations)
        {
            if (locations == null || locations.Count < 2)
            {
                return 0;
            }

            double totalDistance = 0;

            for (int i = 1; i < locations.Count; i++)
            {
                LocationRecord previous = locations[i - 1];
                LocationRecord current = locations[i];

                totalDistance += CalculateDistanceKilometers(
                    previous.Latitude,
                    previous.Longitude,
                    current.Latitude,
                    current.Longitude);
            }

            return totalDistance;
        }

        private static double CalculateDistanceKilometers(
            double latitude1,
            double longitude1,
            double latitude2,
            double longitude2)
        {
            const double EarthRadiusKilometers = 6371.0;

            double latitudeDifference =
                DegreesToRadians(latitude2 - latitude1);

            double longitudeDifference =
                DegreesToRadians(longitude2 - longitude1);

            double firstLatitude =
                DegreesToRadians(latitude1);

            double secondLatitude =
                DegreesToRadians(latitude2);

            double a =
                Math.Sin(latitudeDifference / 2) *
                Math.Sin(latitudeDifference / 2) +
                Math.Cos(firstLatitude) *
                Math.Cos(secondLatitude) *
                Math.Sin(longitudeDifference / 2) *
                Math.Sin(longitudeDifference / 2);

            double c =
                2 * Math.Atan2(
                    Math.Sqrt(a),
                    Math.Sqrt(1 - a));

            return EarthRadiusKilometers * c;
        }

        private static double DegreesToRadians(
            double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}