using System;
using System.Collections.Generic;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// A list of points read from a GPX file.
	/// </summary>
	public class GpxPoints : List<GpxPoint>
	{
		/// <summary>
		/// Gets the time from the first point.
		/// </summary>
		/// <returns>The time from the first point.</returns>
		public DateTime StartTime
		{
			get
			{
				if (Count > 0)
				{
					return this[0].NonNullTime;
				}
				return DateTime.MinValue;
			}
		}

		/// <summary>
		/// Gets the time between the first and last points.
		/// </summary>
		/// <returns>The time between the first and last points.</returns>
		public TimeSpan TimeRange
		{
			get
			{
				if (Count > 1)
				{
					return this[Count - 1].NonNullTime - this[0].NonNullTime;
				}
				return TimeSpan.Zero;
			}
		}

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
			if (Count > 0)
			{
				low = double.MaxValue;
				high = double.MinValue;
				foreach (GpxPoint point in this)
				{
					if (point.Elevation.HasValue)
					{
						if (point.Elevation < low)
						{
							low = (double)point.Elevation;
						}
						if (point.Elevation > high)
						{
							high = (double)point.Elevation;
						}
					}
				}
			}
			else
			{
				low = 0;
				high = 0;
			}
			return high - low;
		}

		/// <summary>
		/// Gets the difference between the lowest and highest elevations.
		/// </summary>
		/// <returns>The difference between the lowest and highest elevations.</returns>
		public double ElevationRange => GetElevationRange(out _, out _);

		/// <summary>
		/// Calculates the distance between two GPS corordinates.
		/// </summary>
		/// <param name="point1">First GPS coordinate.</param>
		/// <param name="point2">Second GPS coordinate.</param>
		/// <returns>The distance between the two GPS corordinates.</returns>
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
		/// <returns>List of points.</returns>
		public GpxPoints GetReducedElevationPoints()
		{
			return DouglasPeucker.Reduce(this, 0.5, ElevationDistance);
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
				DateTime offsetTime = StartTime + offset;
				int index = 0;
				while (index < Count && this[index].NonNullTime < offsetTime)
				{
					if (index > 0)
					{
						distance += GpxPoints.DistanceBetweenPoints(this[index], this[index - 1]);
					}
					index++;
				}
				if (index == this.Count)
				{
					point = this[Count - 1].Clone();
				}
				else if (index == 0 || this[index].NonNullTime == offsetTime)
				{
					point = this[index].Clone();
				}
				else
				{
					point = this[index - 1].Clone();
					double portion = (offsetTime - point.NonNullTime).TotalSeconds / (this[index].NonNullTime - point.NonNullTime).TotalSeconds;
					point.Elevation += (this[index].Elevation - point.Elevation) * portion;
					distance += GpxPoints.DistanceBetweenPoints(this[index], this[index - 1]) * portion;
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
			double time1 = (point1.NonNullTime - StartTime).TotalSeconds;
			double time2 = (point2.NonNullTime - StartTime).TotalSeconds;
			double time = (point.NonNullTime - StartTime).TotalSeconds;
			double area = Math.Abs(.5 * (point1.NonNullElevation * time2 + point2.NonNullElevation * time + point.NonNullElevation * time1 -
									point2.NonNullElevation * time1 - point.NonNullElevation * time2 - point1.NonNullElevation * time));
			double bottom = Math.Sqrt(Math.Pow(point1.NonNullElevation - point2.NonNullElevation, 2) + Math.Pow(time1 - time2, 2));
			double height = area / bottom * 2;
			return height;
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
