# Location Tracker

Location Tracker is a .NET MAUI application that records device location data, stores location history in SQLite, displays saved movement on a map, generates a heat map from saved location density, and calculates total traveled distance.

## Project Purpose

The purpose of this project is to demonstrate multiplatform software development using .NET MAUI while applying software engineering concepts such as separation of responsibilities, persistent data storage, location permission handling, testing, and maintainable service design.

## Main Features

1. Current location display using .NET MAUI geolocation.

2. Start Tracking and Stop Tracking controls.

3. Continuous foreground location tracking.

4. Persistent location history stored in SQLite.

5. Saved location history that remains available after the application is closed and reopened.

6. Heat map visualization based on saved location readings.

7. Total distance statistics displayed in miles.

8. Unit testing using a fake location provider.

9. Android support as the primary application target.

10. Windows support as an additional .NET MAUI target.

## Application Architecture

The application separates major responsibilities into dedicated services.

MainPage  
↓  
LocationPermissionService  
↓  
LocationTrackingService  
↓  
ILocationProvider  
↓  
MauiLocationProvider  

LocationTrackingService also sends location records to:

LocationDatabaseService  
↓  
SQLite Database

Saved database records are used by:

HeatMapService  
DistanceStatisticsService

The test project provides:

FakeLocationProvider  
↓  
LocationTrackingService Tests

This structure allows the application to use real device location services during normal operation while allowing simulated location behavior during automated testing.

## Key Design Decisions

The application uses an interface named `ILocationProvider` to separate location tracking logic from the .NET MAUI geolocation implementation. This allows a fake location provider to be supplied during unit testing.

SQLite is used for persistent storage so location history remains available after the application is closed and reopened.

Location tracking only begins when the user selects Start Tracking. The user can stop foreground tracking at any time by selecting Stop Tracking.

The heat map is generated from persisted location records rather than only the current device position. This allows previously recorded movement to remain visible when the application is reopened.

Distance statistics are calculated from consecutive saved location readings. The user interface displays the total distance in miles.

## Location Permissions

Location Tracker requires location permission because the application must access device position information in order to record movement and display location history.

If location permission is not granted, tracking cannot begin.

## SQLite Storage

Location readings are converted into `LocationRecord` objects and stored in the application's SQLite database.

Each record contains:

1. Latitude

2. Longitude

3. Timestamp

The database is stored within the application's local application data area.

## Heat Map

The application retrieves saved location records from SQLite and passes them to `HeatMapService`.

The heat map represents areas where saved location readings are concentrated. This provides a visual representation of location history instead of displaying only the current position.

## Distance Statistics

`DistanceStatisticsService` calculates the distance between consecutive saved coordinates and combines those values to determine total traveled distance.

The application displays the resulting distance in miles.

## Unit Testing

The solution contains a separate test project named:

`Location_Tracker.Tests`

The test project uses `FakeLocationProvider` instead of the real device geolocation service.

The fake provider allows tests to verify location behavior without requiring physical device movement or Android emulator GPS input.

Current tests verify:

1. The fake location provider can start listening.

2. The fake location provider can stop listening.

3. The fake provider can send simulated location coordinates.

## Android Testing

The primary test environment is the Pixel 7 Android emulator.

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

9. Verify that location history and distance statistics have updated.

## Google Maps API Key

A valid Google Maps API key is required for Android map functionality.

The real API key must not be committed to the public GitHub repository.

The repository should contain only the sanitized Android configuration. Configure the real API key locally before building and running the Android version.

## Security Considerations

Location data is treated as sensitive application data.

The application stores location history locally in the application data directory.

Raw latitude and longitude values should not be unnecessarily exposed in production diagnostic output.

API keys and credentials must not be committed to GitHub.

## Build and Run

1. Open the solution in Visual Studio.

2. Restore required NuGet packages.

3. Configure the required Google Maps API key locally.

4. Select the Pixel 7 Android emulator.

5. Build the solution.

6. Run the Location Tracker project.

7. Grant location permission when requested.

8. Select Start Tracking to begin collecting location readings.

9. Select Stop Tracking when tracking is complete.

## Technologies

.NET MAUI

C Sharp

SQLite

Google Maps

.NET MAUI Geolocation

xUnit

Visual Studio

Android Emulator