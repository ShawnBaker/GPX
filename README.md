# FrozenNorth.Gpx

Reads and writes GPX files.

# API

```
FrozenNorth.Gpx.GpxReader

/// <summary>
/// Loads a GPX file.
/// </summary>
/// <param name="fileName">Full path and name of the GPX file.</param>
/// <returns>A Gpx object, null if the file fails to load.</returns>
public static Gpx Load(string fileName)
```

```
FrozenNorth.Gpx.GpxWriter

/// <summary>
/// Saves a GPX file.
/// </summary>
/// <param name="gpx">Gpx object containing the data to be saved.</param>
/// <param name="fileName">Full path and name of the GPX file.</param>
/// <returns>True if the file was successfully saved, false if not.</returns>
public static bool Save(Gpx gpx, string fileName)
```

# Classes

```
FrozenNorth.Gpx

Gpx
GpxBounds
GpxCopyright
GpxEmail
GpxFix
GpxLink
GpxLinks
GpxMetadata
GpxPerson
GpxPoint
GpxPoints
GpxReader
GpxRoute
GpxRoutes
GpxTrack
GpxTracks
GpxTrackSegment
GpxTrackSegments
GpxWriter
```

# Usage

```
Gpx gpx = GpxReader.Load(fileName);
...
GpxWriter.Save(gpx, fileName);

```

# Attributions

The icon is from the [IconMarketPK](https://www.flaticon.com/authors/iconmarketpk) package on FlatIcon.

# License

MIT License

Copyright © 2023 Shawn Baker
