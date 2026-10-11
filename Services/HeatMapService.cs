using Location_Tracker.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using MauiMap = Microsoft.Maui.Controls.Maps.Map;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Converts saved geographic locations into density-based map circles.
    /// Nearby coordinates are grouped together so frequently visited areas
    /// produce stronger heat-map intensity than less frequently visited areas.
    /// </summary>
    public class HeatMapService
    {
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
             * Rounding coordinates to four decimal places groups readings
             * that occurred within a small geographic area. Each group
             * represents one heat-map density region.
             */
            var locationGroups = savedLocations
                .GroupBy(record => new
                {
                    Latitude = Math.Round(record.Latitude, 4),
                    Longitude = Math.Round(record.Longitude, 4)
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
                 * Density is normalized against the busiest location.
                 * This allows visual intensity to scale with the stored data
                 * instead of displaying every coordinate identically.
                 */
                double densityRatio =
                    (double)group.Density / maximumDensity;

                Color fillColor;
                double radiusMeters;

                if (densityRatio >= 0.67)
                {
                    // High-density area.
                    fillColor = Color.FromArgb("#99FF0000");
                    radiusMeters = 90;
                }
                else if (densityRatio >= 0.34)
                {
                    // Medium-density area.
                    fillColor = Color.FromArgb("#88FFA500");
                    radiusMeters = 65;
                }
                else
                {
                    // Lower-density area.
                    fillColor = Color.FromArgb("#775FFD00");
                    radiusMeters = 45;
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