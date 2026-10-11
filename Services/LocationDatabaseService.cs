using Location_Tracker.Models;
using SQLite;

namespace Location_Tracker.Services
{
    /// <summary>
    /// Manages local SQLite storage for recorded device locations.
    /// Database responsibilities stay separate from the user interface
    /// and geolocation logic.
    /// </summary>
    public class LocationDatabaseService
    {
        private SQLiteAsyncConnection? _database;

        /// <summary>
        /// Creates the database connection and location table the first
        /// time the database is needed.
        /// </summary>
        private async Task InitializeAsync()
        {
            if (_database is not null)
            {
                return;
            }

            string databasePath = Path.Combine(
                FileSystem.AppDataDirectory,
                "locations.db3");

            _database = new SQLiteAsyncConnection(databasePath);

            // Creates the table only if it does not already exist,
            // preserving previously stored location records.
            await _database.CreateTableAsync<LocationRecord>();
        }

        /// <summary>
        /// Saves one location record to the SQLite database.
        /// </summary>
        public async Task<int> SaveLocationAsync(LocationRecord location)
        {
            await InitializeAsync();

            return await _database!.InsertAsync(location);
        }

        /// <summary>
        /// Retrieves all saved locations in chronological order.
        /// </summary>
        public async Task<List<LocationRecord>> GetLocationsAsync()
        {
            await InitializeAsync();

            return await _database!
                .Table<LocationRecord>()
                .OrderBy(location => location.Timestamp)
                .ToListAsync();
        }
    }
}
