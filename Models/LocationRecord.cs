using SQLite;

namespace Location_Tracker.Models
{
    /// <summary>
    /// Represents one recorded device location that will be stored
    /// in the application's local SQLite database.
    /// </summary>
    public class LocationRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime Timestamp { get; set; }
    }
}