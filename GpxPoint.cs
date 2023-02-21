using System;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// A point from a GPX file.
	/// </summary>
	public class GpxPoint
	{
		// public properties
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public double? Elevation { get; set; }
		public DateTime? Time { get; set; }
		public double? MagneticVariation { get; set; }
		public double? GeoidHeight { get; set; }
		public string Name { get; set; }
		public string Comment { get; set; }
		public string Description { get; set; }
		public string Source { get; set; }
		public GpxLinkList Links { get; } = new GpxLinkList();
		public string SymbolName { get; set; }
		public string Type { get; set; }
		public GpxFix? Fix { get; set; }
		public uint? NumSatellites { get; set; }
		public double? Hdop { get; set; }
		public double? Vdop { get; set; }
		public double? Pdop { get; set; }
		public double? AgeOfDgpsData { get; set; }
		public uint? DgpsId { get; set; }
		public GpxExtensionList Extensions { get; } = new GpxExtensionList();

		/// <summary>
		/// Creates an empty point.
		/// </summary>
		public GpxPoint() { }

		/// <summary>
		/// Creates a point from values.
		/// </summary>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public GpxPoint(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		/// <summary>
		/// Creates a point from values.
		/// </summary>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		/// <param name="elevation">Elevation.</param>
		/// <param name="time">Time.</param>
		/// <param name="fix">Fix.</param>
		/// <param name="hdop">Horizontal dilution of precision.</param>
		public GpxPoint(double latitude, double longitude, double? elevation, DateTime? time, GpxFix? fix, double? hdop)
		{
			Latitude = latitude;
			Longitude = longitude;
			Elevation = elevation;
			Time = time;
			Fix = fix;
			Hdop = hdop;
		}

		/// <summary>
		/// Creates a clone of this point.
		/// </summary>
		/// <returns>A clone of this point</returns>
		public GpxPoint Clone()
		{
			GpxPoint point = new GpxPoint(Latitude, Longitude, Elevation, Time, Fix, Hdop);
			point.MagneticVariation = MagneticVariation;
			point.GeoidHeight = GeoidHeight;
			point.Name = Name;
			point.Comment = Comment;
			point.Description = Description;
			point.Source = Source;
			point.Links.AddRange(Links);
			point.SymbolName = SymbolName;
			point.Type = Type;
			point.NumSatellites = NumSatellites;
			point.Vdop = Vdop;
			point.Pdop = Pdop;
			point.AgeOfDgpsData = AgeOfDgpsData;
			point.DgpsId = DgpsId;
			return point;
		}

		/// <summary>
		/// Gets a string representing the point.
		/// </summary>
		/// <returns>A string representing the point.</returns>
		public override string ToString()
		{
			return Latitude.ToString() + ", " + Longitude.ToString();
		}

		/// <summary>
		/// Gets the time or DateTime.MinValue if it doesn't exist.
		/// </summary>
		public DateTime TimeValue => Time ?? DateTime.MinValue;

		/// <summary>
		/// Gets the elevation or 0 if it doesn't exist.
		/// </summary>
		public double ElevationValue => Elevation ?? 0;
	}
}
