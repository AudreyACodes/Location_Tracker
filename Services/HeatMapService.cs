using Location_Tracker.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using MauiMap = Microsoft.Maui.Controls.Maps.Map;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Converts saved geographic locations into density based map regions.
    /// Nearby readings are grouped so frequently visited areas display
    /// with greater heat map intensity than less frequently visited areas.
    /// </summary>
    public class HeatMapService
    {
        private const int CoordinateGroupingDecimalPlaces = 4;

        private const double HighDensityThreshold = 0.67;
        private const double MediumDensityThreshold = 0.34;

        private const double HighDensityRadiusMeters = 90;
        private const double MediumDensityRadiusMeters = 65;
        private const double LowDensityRadiusMeters = 45;

        /// <summary>
        /// Calculates location density from saved records and renders
        /// the resulting heat map regions on the supplied MAUI map.
        /// </summary>
        public void RenderHeatMap(
            MauiMap map,
            List<LocationRecord> savedLocations)
        {
            map.MapElements.Clear();
            map.Pins.Clear();

            if (savedLocations.Count == 0)
            {
                return;
            }

            /*
             * Coordinates are rounded before grouping so readings from
             * approximately the same geographic area contribute to one
             * density region instead of being rendered independently.
             */
            var locationGroups = savedLocations
                .GroupBy(record => new
                {
                    Latitude = Math.Round(
                        record.Latitude,
                        CoordinateGroupingDecimalPlaces),

                    Longitude = Math.Round(
                        record.Longitude,
                        CoordinateGroupingDecimalPlaces)
                })
                .Select(group => new
                {
                    Latitude = group.Average(record => record.Latitude),
                    Longitude = group.Average(record => record.Longitude),
                    Density = group.Count()
                })
                .ToList();

            int maximumDensity =
                locationGroups.Max(group => group.Density);

            foreach (var group in locationGroups)
            {
                /*
                 * Each region is normalized against the highest density
                 * found in the saved data. The resulting ratio controls
                 * both the color intensity and geographic radius.
                 */
                double densityRatio =
                    (double)group.Density / maximumDensity;

                Color fillColor;
                double radiusMeters;

                if (densityRatio >= HighDensityThreshold)
                {
                    fillColor = Color.FromArgb("#99FF0000");
                    radiusMeters = HighDensityRadiusMeters;
                }
                else if (densityRatio >= MediumDensityThreshold)
                {
                    fillColor = Color.FromArgb("#88FFA500");
                    radiusMeters = MediumDensityRadiusMeters;
                }
                else
                {
                    fillColor = Color.FromArgb("#775FFD00");
                    radiusMeters = LowDensityRadiusMeters;
                }

                Circle heatCircle = new()
                {
                    Center = new Microsoft.Maui.Devices.Sensors.Location(
                        group.Latitude,
                        group.Longitude),

                    Radius = new Distance(radiusMeters),
                    FillColor = fillColor,
                    StrokeColor = fillColor,
                    StrokeWidth = 2
                };

                map.MapElements.Add(heatCircle);
            }
        }
    }
}