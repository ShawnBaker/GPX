# FrozenNorth.Gpx

Reads and writes GPX files.

# API

```
FrozenNorth.Gpx.GpxReader

public static Gpx Load(string fileName)
public static Gpx Load(Stream stream)
public static Gpx Load(TextReader reader)
public static Gpx Load(XmlReader reader)
public static async Task<Gpx> LoadAsync(string fileName)
public static async Task<Gpx> LoadAsync(Stream stream)
public static async Task<Gpx> LoadAsync(TextReader reader)
public static async Task<Gpx> LoadAsync(XmlReader reader)
```

```
FrozenNorth.Gpx.GpxWriter

public static bool Save(Gpx gpx, string fileName)
public static bool Save(Gpx gpx, Stream stream)
public static bool Save(Gpx gpx, TextWriter writer)
public static bool Save(Gpx gpx, XmlWriter writer)
public static async Task<bool> SaveAsync(Gpx gpx, string fileName)
public static async Task<bool> SaveAsync(Gpx gpx, Stream stream)
public static async Task<bool> SaveAsync(Gpx gpx, TextWriter writer)
public static async Task<bool> SaveAsync(Gpx gpx, XmlWriter writer)
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
