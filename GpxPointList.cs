using System;
using System.Collections.Generic;
using System.Drawing;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// A list of points read from a GPX file.
	/// </summary>
	public class GpxPointList : List<GpxPoint>
	{
		/// <summary>
		/// Gets the time from the first point that has a time.
		/// </summary>
		/// <returns>The time from the first point that has a time.</returns>
		public DateTime StartTime
		{
			get
			{
				foreach (var point in this)
				{
					if (point.Time.HasValue)
					{
						return point.Time.Value;
					}
				}
				return DateTime.MinValue;
			}
		}

		/// <summary>
		/// Gets the time from the last point that has a time.
		/// </summary>
		/// <returns>The time from the last point that has a time.</returns>
		public DateTime EndTime
		{
			get
			{
				for (int i = Count - 1; i >= 0; i--)
				{
					var point = this[i];
					if (point.Time.HasValue)
					{
						return point.Time.Value;
					}
				}
				return DateTime.MinValue;
			}
		}

		/// <summary>
		/// Gets the time between the first and last points.
		/// </summary>
		/// <returns>The time between the first and last points.</returns>
		public TimeSpan Duration => EndTime - StartTime;

		/// <summary>
		/// Gets the total distance between all points.
		/// </summary>
		/// <returns>The total distance between all points.</returns>
		public double Distance
		{
			get
			{
				double distance = 0;
				for (int i = 1; i < Count; i++)
				{
					distance += DistanceBetweenPoints(this[i - 1], this[i]);
				}
				return distance;
			}
		}

		/// <summary>
		/// Gets the total range in elevation and the lowest and highest elevations.
		/// </summary>
		/// <param name="low">The returned lowest elevation.</param>
		/// <param name="high">The returned highest elevation.</param>
		/// <returns>The total range in elevation.</returns>
		public double GetElevationRange(out double low, out double high)
		{
			low = 0;
			high = 0;
			if (Count > 0)
			{
				double? lowest = null;
				double? highest = null;
				foreach (GpxPoint point in this)
				{
					if (point.Elevation.HasValue)
					{
						double elevation = point.Elevation.Value;
						if (!lowest.HasValue || elevation < lowest)
						{
							lowest = elevation;
						}
						if (!highest.HasValue || elevation > highest)
						{
							highest = elevation;
						}
					}
				}
				if (lowest.HasValue)
				{
					low = lowest.Value;
					high = highest.Value;
				}
			}
			return high - low;
		}

		/// <summary>
		/// Gets the difference between the lowest and highest elevations.
		/// </summary>
		/// <returns>The difference between the lowest and highest elevations.</returns>
		public double ElevationRange => GetElevationRange(out _, out _);

		/// <summary>
		/// Calculates the distance between two GPS corordinates in kilometers.
		/// </summary>
		/// <param name="point1">First GPS coordinate.</param>
		/// <param name="point2">Second GPS coordinate.</param>
		/// <returns>The distance between the two GPS corordinates in kilometers.</returns>
		public static double DistanceBetweenPoints(GpxPoint point1, GpxPoint point2)
		{
			var earthRadiusKm = 6371;

			var dLat = ToRadians(point2.Latitude - point1.Latitude);
			var dLon = ToRadians(point2.Longitude - point1.Longitude);

			double lat1 = ToRadians(point1.Latitude);
			double lat2 = ToRadians(point2.Latitude);

			var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
					Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			return earthRadiusKm * c;
		}


		/// <summary>
		/// Gets enough points to draw an elevation graph.
		/// </summary>
		/// <param name="tolerance">Tolerance (between 0 and 1) to use with the Douglas Peucker algorithm.</param>
		/// <returns>List of points.</returns>
		public GpxPointList GetReducedElevationPoints(double tolerance = 0.5)
		{
			return DouglasPeucker.Reduce(this, tolerance, ElevationDistance);
		}

		/// <summary>
		/// Gets enough points to draw a map.
		/// </summary>
		/// <param name="tolerance">Tolerance (between 0 and 1) to use with the Douglas Peucker algorithm.</param>
		/// <returns>List of points.</returns>
		public GpxPointList GetReducedLocationPoints(double tolerance = 0.5)
		{
			return DouglasPeucker.Reduce(this, tolerance, LocationDistance);
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
			point = new GpxPoint();
			distance = 0;
			if (Count > 0)
			{
				if (offset < TimeSpan.Zero) offset = TimeSpan.Zero;
				else if (offset > Duration) offset = Duration;
				DateTime offsetTime = StartTime + offset;
				int index = 0;
				while (index < Count && this[index].TimeValue < offsetTime)
				{
					if (index > 0)
					{
						distance += GpxPointList.DistanceBetweenPoints(this[index], this[index - 1]);
					}
					index++;
				}
				if (index == this.Count)
				{
					point = this[Count - 1].Clone();
				}
				else if (index == 0 || this[index].TimeValue == offsetTime)
				{
					point = this[index].Clone();
				}
				else
				{
					point = this[index - 1].Clone();
					double portion = (offsetTime - point.TimeValue).TotalSeconds / (this[index].TimeValue - point.TimeValue).TotalSeconds;
					point.Elevation += (this[index].Elevation - point.Elevation) * portion;
					distance += GpxPointList.DistanceBetweenPoints(this[index], this[index - 1]) * portion;
				}
				point.Time = StartTime + offset;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Used by the Douglas Peucker algorithm to compare elevation points.
		/// </summary>
		private double ElevationDistance(GpxPoint point1, GpxPoint point2, GpxPoint point)
		{
			if (point1.Time == null || point2.Time == null || point.Time == null ||
				point1.Elevation == null || point2.Elevation == null || point.Elevation == null)
			{
				return 0;
			}
			double time1 = (point1.TimeValue - StartTime).TotalSeconds;
			double time2 = (point2.TimeValue - StartTime).TotalSeconds;
			double time = (point.TimeValue - StartTime).TotalSeconds;
			double area = Math.Abs(.5 * (point1.ElevationValue * time2 + point2.ElevationValue * time + point.ElevationValue * time1 -
									point2.ElevationValue * time1 - point.ElevationValue * time2 - point1.ElevationValue * time));
			double bottom = Math.Sqrt(Math.Pow(point1.ElevationValue - point2.ElevationValue, 2) + Math.Pow(time1 - time2, 2));
			return area / bottom * 2;
		}

		/// <summary>
		/// Used by the Douglas Peucker algorithm to compare elevation points.
		/// </summary>
		private double LocationDistance(GpxPoint point1, GpxPoint point2, GpxPoint point)
		{
			double area = Math.Abs(.5 * (point1.Latitude * point2.Longitude + point2.Latitude * point.Longitude +
										point.Latitude * point1.Longitude - point2.Latitude * point1.Longitude -
										point.Latitude * point2.Longitude - point1.Latitude * point.Longitude));
			double bottom = Math.Sqrt(Math.Pow(point1.Latitude - point2.Latitude, 2) + Math.Pow(point1.Longitude - point2.Longitude, 2));
			return area / bottom * 2;
		}

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <param name="degrees">An angle in degrees.</param>
		/// <returns>The angle in radians.</returns>
		public static double ToRadians(double degrees)
		{
			return (Math.PI / 180) * degrees;
		}
	}
}
