using System;
using System.Collections.Generic;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// A track segment read from a GPX file.
	/// </summary>
	public class GpxTrackSegment
	{
		public GpxTrack Track { get; set; } = null;
		public GpxPointList Points { get; } = new GpxPointList();
		public GpxExtensionList Extensions { get; } = new GpxExtensionList();

		public override string ToString()
		{
			return string.Format("Segment {0} - {1} Points", Track.Segments.IndexOf(this) + 1, Points.Count.ToString());
		}
	}

	/// <summary>
	/// A list of track segments read from a GPX file.
	/// </summary>
	public class GpxTrackSegmentList : List<GpxTrackSegment>
	{
		/// <summary>
		/// True if any of the segments contain points.
		/// </summary>
		public bool HasPoints
		{
			get
			{
				foreach (var segment in this)
				{
					if (segment.Points.Count > 0) return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets the first time associated with a point.
		/// </summary>
		public DateTime StartTime
		{
			get
			{
				foreach (var segment in this)
				{
					DateTime t = segment.Points.StartTime;
					if (t != DateTime.MinValue)
					{
						return t;
					}
				}
				return DateTime.MinValue;
			}
		}

		/// <summary>
		/// Gets the last time associated with a point.
		/// </summary>
		public DateTime EndTime
		{
			get
			{
				for (int i = Count - 1; i >= 0; i--)
				{
					DateTime t = this[i].Points.EndTime;
					if (t != DateTime.MinValue)
					{
						return t;
					}
				}
				return DateTime.MinValue;
			}
		}

		/// <summary>
		/// Gets the duration of the track.
		/// </summary>
		public TimeSpan Duration => EndTime - StartTime;

		/// <summary>
		/// Gets the distance of the track.
		/// </summary>
		public double Distance
		{
			get
			{
				double distance = 0;
				foreach (var segment in this)
				{
					distance += segment.Points.Distance;
				}
				return distance;
			}
		}

		/// <summary>
		/// Gets the elevation range of the track.
		/// </summary>
		public double ElevationRange
		{
			get
			{
				double range = 0;
				foreach (var segment in this)
				{
					range += segment.Points.ElevationRange;
				}
				return range;
			}
		}

		/// <summary>
		/// Gets the location and distance at a specific time.
		/// </summary>
		/// <param name="offset">Offset into the timeline.</param>
		/// <param name="point">Location at the offset time.</param>
		/// <param name="distance">Distance at the offset time.</param>
		/// <returns>True if there are points to do the calculation with, false if not.</returns>
		public bool LocationAtOffset(TimeSpan offset, out GpxPoint point, out double distance)
		{
			if (offset < TimeSpan.Zero) offset = TimeSpan.Zero;
			else if (offset > Duration) offset = Duration;
			point = new GpxPoint();
			distance = 0;
			DateTime offsetTime = StartTime + offset;
			foreach (var segment in this)
			{
				if (offsetTime <= segment.Points.EndTime)
				{
					return segment.Points.LocationAtOffset(offsetTime - segment.Points.StartTime, out point, out distance);
				}
			}
			return false;
		}
	}
}
