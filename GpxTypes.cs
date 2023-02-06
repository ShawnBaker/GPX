using System;
using System.Collections.Generic;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// A GPX file.
	/// </summary>
	public class Gpx
	{
		public string FileName { get; set; } = "";
		public string Version { get; set; } = "";
		public string Creator { get; set; } = "";
		public GpxMetadata Metadata { get; } = new GpxMetadata();
		public GpxRouteList Routes { get; } = new GpxRouteList();
		public GpxPointList Waypoints { get; } = new GpxPointList();
		public GpxTrackList Tracks { get; } = new GpxTrackList();
		public GpxNamespaceList Namespaces { get; } = new GpxNamespaceList();
		public GpxExtensionList Extensions { get; } = new GpxExtensionList();
	}

	/// <summary>
	/// A bounds read from a GPX file.
	/// </summary>
	public class GpxBounds
	{
		public double MinLatitude { get; set; }
		public double MaxLatitude { get; set; }
		public double MinLongitude { get; set; }
		public double MaxLongitude { get; set; }

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
		public string Author { get; set; }
		public string Year { get; set; }
		public Uri Licence { get; set; }

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
		public string Id { get; set; }
		public string Domain { get; set; }

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
		public string Prefix { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
		public GpxNamespace Namespace { get; set; }
		public GpxExtensionList Children { get; } = new GpxExtensionList();

		public bool HasData => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Value) || Children.Count > 0;
	}

	/// <summary>
	/// A list of extensions read from a GPX file.
	/// </summary>
	public class GpxExtensionList : List<GpxExtension> { }

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
		public Uri Href { get; set; }
		public string Text { get; set; }
		public string Type { get; set; }

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
	public class GpxLinkList : List<GpxLink> { }

	/// <summary>
	/// The metatdata read from a GPX file.
	/// </summary>
	public class GpxMetadata
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public GpxPerson Author { get; set; }
		public GpxCopyright Copyright { get; set; }
		public GpxLinkList Links { get; } = new GpxLinkList();
		public DateTime? Time { get; set; }
		public string Keywords { get; set; }
		public GpxBounds Bounds { get; set; }
		public GpxExtensionList Extensions { get; } = new GpxExtensionList();

		public bool HasData => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Description) || Author != null ||
								Copyright != null || Links.Count > 0 || Time != null || !string.IsNullOrEmpty(Keywords) ||
								Bounds != null;
	}

	/// <summary>
	/// A namespace read from a GPX file.
	/// </summary>
	public class GpxNamespace
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}

	/// <summary>
	/// A list of namespaces read from a GPX file.
	/// </summary>
	public class GpxNamespaceList : List<GpxNamespace> { }

	/// <summary>
	/// A person read from a GPX file.
	/// </summary>
	public class GpxPerson
	{
		public string Name { get; set; }
		public GpxEmail Email { get; set; }
		public GpxLink Link { get; set; }

		public bool HasData => Name != null || Email.HasData || Link.HasData;
	}
}
