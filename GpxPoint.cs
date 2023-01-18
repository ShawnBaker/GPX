using System;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// A point from a GPX file.
	/// </summary>
	public class GpxPoint
	{
		// public variables
		public double Latitude;
		public double Longitude;
		public double? Elevation;
		public DateTime? Time;
		public double? MagneticVariation;
		public double? GeoidHeight;
		public string Name;
		public string Comment;
		public string Description;
		public string Source;
		public GpxLinks Links = new GpxLinks();
		public string SymbolName;
		public string Type;
		public GpxFix? Fix;
		public uint? NumSatellites;
		public double? Hdop;
		public double? Vdop;
		public double? Pdop;
		public double? AgeOfDgpsData;
		public uint? DgpsId;
		public GpxExtensions Extensions = new GpxExtensions();

		/// <summary>
		/// Creates an empty point.
		/// </summary>
		public GpxPoint() { }

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

		public override string ToString()
		{
			return Latitude.ToString() + ", " + Longitude.ToString();
		}

		internal DateTime NonNullTime => Time ?? DateTime.MinValue;
		internal double NonNullElevation => Elevation ?? 0;
	}
}
