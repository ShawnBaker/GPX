﻿using System.Collections.Generic;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// A track read from a GPX file.
	/// </summary>
	public class GpxTrack
	{
		// public properties
		public string Name { get; set; }
		public string Comment { get; set; }
		public string Description { get; set; }
		public string Source { get; set; }
		public GpxLinkList Links { get; } = new GpxLinkList();
		public uint? Number { get; set; }
		public string Type { get; set; }
		public GpxTrackSegmentList Segments { get; } = new GpxTrackSegmentList();
		public GpxExtensionList Extensions { get; } = new GpxExtensionList();

		/// <summary>
		/// True if any of the properties contain data.
		/// </summary>
		public bool HasData => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Comment) || !string.IsNullOrEmpty(Description) ||
								!string.IsNullOrEmpty(Source) || Links.Count > 0 || Number != null || !string.IsNullOrEmpty(Type) ||
								Segments.HasPoints;

		/// <summary>
		/// A displayable name for the track.
		/// </summary>
		public string DisplayName
		{
			get
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
		/// Gets a displayable name for the track.
		/// </summary>
		/// <returns>A displayable name for the track.</returns>
		public override string ToString()
        {
            return DisplayName;
        }
	}

	/// <summary>
	/// A list of tracks read from a GPX file.
	/// </summary>
	public class GpxTrackList : List<GpxTrack> { }
}
