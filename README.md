# Location Tracker

Location Tracker is a .NET MAUI application that records device location data, stores location history in SQLite, displays saved movement history, and calculates total traveled distance.

The application supports Android as the primary platform and Windows as the additional .NET MAUI target.

## Project Purpose

The purpose of this project is to demonstrate multiplatform software development using .NET MAUI while applying software engineering concepts including separation of responsibilities, dependency inversion, persistent data storage, secure handling of sensitive location data, location permission handling, automated testing, and maintainable service design.

## Main Features

1. Current location display using .NET MAUI geolocation.

2. Start Tracking and Stop Tracking controls.

3. Continuous foreground location tracking.

4. Persistent location history stored in SQLite.

5. Saved location history that remains available after the application is closed and reopened.

6. Android map and heat map visualization based on saved location readings.

7. Windows saved location history displayed as a scrolling list.

8. Total distance statistics displayed in miles.

9. Graceful location permission handling.

10. Unit testing of the ViewModel using a fake location provider.

11. Android support as the primary application target.

12. Windows support as an additional .NET MAUI target.

## Application Architecture

The application separates major responsibilities into dedicated components.

MainPage  
↓  
LocationPermissionService  

MainPage  
↓  
LocationTrackingService  
↓  
ILocationProvider  
↓  
MauiLocationProvider  

LocationTrackingService  
↓  
LocationDatabaseService  
↓  
SQLite Database  

Saved database records are used by:

HeatMapService  
DistanceStatisticsService  

The application also contains a testable ViewModel architecture:

LocationTrackerViewModel  
↓  
ILocationProvider  
↓  
MauiLocationProvider during normal application use  

For automated tests:

LocationTrackerViewModel  
↓  
ILocationProvider  
↓  
FakeLocationProvider  

This architecture allows the application to use real device location services during normal operation while allowing simulated location behavior during automated testing.

## Key Design Decisions

The application uses an interface named `ILocationProvider` to separate location behavior from the .NET MAUI geolocation implementation.

This dependency inversion allows `FakeLocationProvider` to be supplied to `LocationTrackerViewModel` during unit testing without requiring real device movement.

SQLite is used for persistent storage so location history remains available after the application is closed and reopened.

Location tracking begins only when the user selects Start Tracking. The user can stop foreground tracking at any time by selecting Stop Tracking.

Android uses the .NET MAUI map to display saved location history and the heat map.

Windows displays saved location history as a list. This satisfies the project requirement that persisted history be visualized as a map or list while allowing the Windows version to operate without an external Azure Maps dependency.

Distance statistics are calculated from consecutive saved location readings. The user interface displays the total distance in miles.

## Location Permissions

Location Tracker requires location permission because the application must access device position information to display the current position, record movement, save location history, and calculate traveled distance.

Before requesting location permission, the application explains why the permission is needed.

If location permission is denied, the application remains open and informs the user that location tracking cannot begin until permission is granted.

If permission has already been granted, the application opens normally without requesting permission again.

## SQLite Storage

Location readings are converted into `LocationRecord` objects and stored in the application's SQLite database.

Each record contains:

1. Latitude

2. Longitude

3. Timestamp

The database is stored inside the application's private local application data directory using `FileSystem.AppDataDirectory`.

Saved records remain available after the application is closed and reopened.

## Android Visualization

The Android version displays saved location history on a map.

Saved location readings are passed to `HeatMapService`, which creates a visual representation of areas where location readings are concentrated.

This allows previously recorded movement to remain visible when the application is reopened.

## Windows Visualization

The Windows version displays saved location history in a scrolling list.

Each saved record displays:

1. Timestamp

2. Latitude

3. Longitude

The list is populated from the same persistent SQLite database used by the tracking system.

Saved Windows location records remain available after the application is closed and reopened.

## Distance Statistics

`DistanceStatisticsService` calculates the distance between consecutive saved coordinates and combines those values to determine total traveled distance.

The application displays the resulting total distance in miles.

Distance statistics work on both Android and Windows.

## Unit Testing

The solution contains a separate test project named:

`Location_Tracker.Tests`

The tests use `FakeLocationProvider` instead of the real device geolocation service.

`LocationTrackerViewModel` receives location functionality through `ILocationProvider`, allowing the fake provider to be injected during testing.

Current tests verify:

1. `LocationTrackerViewModel` starts tracking using `FakeLocationProvider`.

2. `LocationTrackerViewModel` stops tracking using `FakeLocationProvider`.

3. A simulated location update from `FakeLocationProvider` updates the ViewModel latitude and longitude values.

All current automated tests pass successfully.

## Android Testing

The primary Android test environment is the Pixel 7 emulator.

Android Extended Controls can simulate GPS movement.

To test continuous movement:

1. Open Extended Controls.

2. Select Location.

3. Select Routes.

4. Choose a saved route.

5. Start tracking in Location Tracker.

6. Play the emulator route.

7. Allow multiple location readings to be collected.

8. Stop tracking after the route completes.

9. Verify that saved location history, the heat map, and distance statistics have updated.

10. Close and reopen the application to verify that saved history remains available.

## Windows Testing

The Windows version can be launched using Windows Machine in Visual Studio.

Testing verifies:

1. The application builds and launches successfully.

2. Current position is displayed.

3. Start Tracking begins location tracking.

4. Stop Tracking ends location tracking.

5. Location records are stored in SQLite.

6. Saved location history appears in the Windows list.

7. Saved records remain available after the application is closed and reopened.

8. Distance statistics update from saved location records.

## Google Maps API Key

A valid Google Maps API key is required for Android map functionality.

The real API key must not be committed to the public GitHub repository.

The Android manifest uses the placeholder:

`${MAPS_API_KEY}`

The real API key must be configured locally before building and running the Android version.

## Security Considerations

Location data is treated as sensitive application data.

The SQLite database is stored within the application's private local application data directory.

Android application backup is disabled using:

`android:allowBackup="false"`

This reduces the possibility of the SQLite location history being included in normal Android application backups.

Raw latitude and longitude values are not written to diagnostic logs when location records are saved.

API keys and credentials must not be committed to GitHub.

The public repository contains only sanitized configuration values.

## Build and Run

### Android

1. Open the solution in Visual Studio.

2. Restore required NuGet packages.

3. Configure the required Google Maps API key locally.

4. Select the Pixel 7 Android emulator.

5. Build the solution.

6. Run the Location Tracker project.

7. Review the location permission explanation.

8. Grant location permission.

9. Select Start Tracking to begin collecting location readings.

10. Select Stop Tracking when tracking is complete.

### Windows

1. Open the solution in Visual Studio.

2. Select Windows Machine.

3. Build the solution.

4. Run the Location Tracker project.

5. Verify the current position.

6. Select Start Tracking.

7. Allow location readings to be collected.

8. Select Stop Tracking.

9. Verify that saved records appear in Saved Location History.

10. Close and reopen the application to verify persistence.

## Technologies

.NET MAUI

C#

SQLite

Google Maps

.NET MAUI Geolocation

xUnit

Visual Studio

Android Emulator

Windows