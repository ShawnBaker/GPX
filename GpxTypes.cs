using System;
using System.Collections.Generic;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// A GPX file.
	/// </summary>
	public class Gpx
	{
		public string FileName = "";
		public string Version = "";
		public string Creator = "";
		public GpxMetadata Metadata = new GpxMetadata();
		public GpxRoutes Routes = new GpxRoutes();
		public GpxPoints Waypoints = new GpxPoints();
		public GpxTracks Tracks = new GpxTracks();
		public GpxNamespaces Namespaces = new GpxNamespaces();
		public GpxExtensions Extensions = new GpxExtensions();
	}

	/// <summary>
	/// A bounds read from a GPX file.
	/// </summary>
	public class GpxBounds
	{
		public double MinLatitude;
		public double MaxLatitude;
		public double MinLongitude;
		public double MaxLongitude;

		public override string ToString()
		{
			return string.Format("{0},{1} - {2},{3}", MinLatitude, MinLongitude, MaxLatitude, MaxLongitude);
		}
	}

	/// <summary>
	/// A copyright read from a GPX file.
	/// </summary>
	public class GpxCopyright
	{
		public string Author;
		public string Year;
		public Uri Licence;

		public bool HasData => !string.IsNullOrEmpty(Author) || !string.IsNullOrEmpty(Year) || Licence != null;

		public override string ToString()
		{
			string str = Year ?? "";
			if (!string.IsNullOrEmpty(Author))
			{
				if (!string.IsNullOrEmpty(str)) str += ",";
				str += Author;
			}
			if (Licence != null)
			{
				if (!string.IsNullOrEmpty(str)) str += ",";
				str += Licence.ToString();
			}
			return str;
		}
	}

	/// <summary>
	/// An email read from a GPX file.
	/// </summary>
	public class GpxEmail
	{
		public string Id;
		public string Domain;

		public bool HasData => !string.IsNullOrEmpty(Id) || !string.IsNullOrEmpty(Domain);

		public override string ToString()
		{
			string str = Id ?? "";
			if (!string.IsNullOrEmpty(Domain))
			{
				if (!string.IsNullOrEmpty(str)) str += "@";
				str += Domain;
			}
			return str;
		}
	}

	/// <summary>
	/// An extension read from a GPX file.
	/// </summary>
	public class GpxExtension
	{
		public string Prefix;
		public string Name;
		public string Value;
		public GpxNamespace Namespace;
		public GpxExtensions Children = new GpxExtensions();

		public bool HasData => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Value) || Children.Count > 0;
	}

	/// <summary>
	/// A list of extensions read from a GPX file.
	/// </summary>
	public class GpxExtensions : List<GpxExtension> { }

	/// <summary>
	/// A fix read from a GPX file.
	/// </summary>
	public enum GpxFix
	{
		None,
		TwoD,
		ThreeD,
		DGPS,
		PPS
	}

	/// <summary>
	/// A link read from a GPX file.
	/// </summary>
	public class GpxLink
	{
		public Uri Href;
		public string Text;
		public string Type;

		public bool HasData => Href != null || !string.IsNullOrEmpty(Text) || !string.IsNullOrEmpty(Type);

		public override string ToString()
		{
			string str = (Href != null) ? Href.ToString() : "";
			if (!string.IsNullOrEmpty(Text))
			{
				if (!string.IsNullOrEmpty(str)) str += ",";
				str += Text;
			}
			if (!string.IsNullOrEmpty(Type))
			{
				if (!string.IsNullOrEmpty(str)) str += ",";
				str += Type;
			}
			return str;
		}
	}

	/// <summary>
	/// A list of links read from a GPX file.
	/// </summary>
	public class GpxLinks : List<GpxLink> { }

	/// <summary>
	/// The metatdata read from a GPX file.
	/// </summary>
	public class GpxMetadata
	{
		public string Name;
		public string Description;
		public GpxPerson Author;
		public GpxCopyright Copyright;
		public GpxLinks Links = new GpxLinks();
		public DateTime? Time;
		public string Keywords;
		public GpxBounds Bounds;
		public GpxExtensions Extensions = new GpxExtensions();

		public bool HasData => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Description) || Author != null ||
								Copyright != null || Links.Count > 0 || Time != null || !string.IsNullOrEmpty(Keywords) ||
								Bounds != null;
	}

	/// <summary>
	/// A namespace read from a GPX file.
	/// </summary>
	public class GpxNamespace
	{
		public string Name;
		public string Value;
	}

	/// <summary>
	/// A list of namespaces read from a GPX file.
	/// </summary>
	public class GpxNamespaces : List<GpxNamespace> { }

	/// <summary>
	/// A person read from a GPX file.
	/// </summary>
	public class GpxPerson
	{
		public string Name;
		public GpxEmail Email;
		public GpxLink Link;

		public bool HasData => Name != null || Email.HasData || Link.HasData;
	}

	/// <summary>
	/// A route read from a GPX file.
	/// </summary>
	public class GpxRoute
	{
		public string Name = null;
		public string Comment = null;
		public string Description = null;
		public string Source = null;
		public GpxLinks Links = new GpxLinks();
		public uint? Number;
		public string Type = null;
		public GpxPoints Points = new GpxPoints();
		public GpxExtensions Extensions = new GpxExtensions();

		public bool HasData => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Comment) || !string.IsNullOrEmpty(Description) ||
								!string.IsNullOrEmpty(Source) || Links.Count > 0 || Number != null || !string.IsNullOrEmpty(Type) ||
								Points.Count > 0;

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name)) return Name;
			if (!string.IsNullOrEmpty(Description)) return Description;
			if (!string.IsNullOrEmpty(Comment)) return Comment;
			if (Points.Count > 0)
			{
				var point = Points[0];
				if (!string.IsNullOrEmpty(point.Name)) return point.Name;
				if (!string.IsNullOrEmpty(point.Description)) return point.Description;
				if (!string.IsNullOrEmpty(point.Comment)) return point.Comment;
				return string.Format("{0},{1}", point.Latitude, point.Longitude);
			}
			return "Route";
		}
	}

	/// <summary>
	/// A list of routes read from a GPX file.
	/// </summary>
	public class GpxRoutes : List<GpxRoute> { }

	/// <summary>
	/// A track read from a GPX file.
	/// </summary>
	public class GpxTrack
	{
		// public variables
		public string Name;
		public string Comment;
		public string Description;
		public string Source;
		public GpxLinks Links = new GpxLinks();
		public uint? Number;
		public string Type;
		public GpxTrackSegments Segments = new GpxTrackSegments();
		public GpxExtensions Extensions = new GpxExtensions();

		public bool HasData => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Comment) || !string.IsNullOrEmpty(Description) ||
								!string.IsNullOrEmpty(Source) || Links.Count > 0 || Number != null || !string.IsNullOrEmpty(Type) ||
								Segments.Count > 0;

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name)) return Name;
			if (!string.IsNullOrEmpty(Description)) return Description;
			if (!string.IsNullOrEmpty(Comment)) return Comment;
			if (Segments.Count > 0 && Segments[0].Points.Count > 0)
			{
				var point = Segments[0].Points[0];
				if (!string.IsNullOrEmpty(point.Name)) return point.Name;
				if (!string.IsNullOrEmpty(point.Description)) return point.Description;
				if (!string.IsNullOrEmpty(point.Comment)) return point.Comment;
				return string.Format("{0},{1}", point.Latitude, point.Longitude);
			}
			return "Track";
		}
	}

	/// <summary>
	/// A list of tracks read from a GPX file.
	/// </summary>
	public class GpxTracks : List<GpxTrack> { }

	/// <summary>
	/// A track segment read from a GPX file.
	/// </summary>
	public class GpxTrackSegment
	{
		public GpxTrack Track = null;
		public GpxPoints Points = new GpxPoints();
		public GpxExtensions Extensions = new GpxExtensions();

		public override string ToString()
		{
			return string.Format("Segment {0} - {1} Points", Track.Segments.IndexOf(this) + 1, Points.Count.ToString());
		}
	}

	/// <summary>
	/// A list of track segments read from a GPX file.
	/// </summary>
	public class GpxTrackSegments : List<GpxTrackSegment> { }
}
