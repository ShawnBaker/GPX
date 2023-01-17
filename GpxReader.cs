using System;
using System.Xml;
using System.Xml.Linq;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// Reads a Gpx object from a file.
	/// </summary>
	public static class GpxReader
	{
		private static XmlNamespaceManager ns;

		/// <summary>
		/// Loads a GPX file.
		/// </summary>
		/// <param name="fileName">Full path and name of the GPX file.</param>
		/// <returns>A Gpx object, null if the file fails to load.</returns>
		public static Gpx Load(string fileName)
		{
			var gpx = new Gpx();
			try
			{
				// save the file name
				gpx.FileName= fileName;

				// get the namespace
				var gpxElement = XElement.Load(fileName);
				string xmlns = gpxElement.Attribute("xmlns")?.Value;
				if (string.IsNullOrEmpty(xmlns))
				{
					xmlns = "http://www.topografix.com/GPX/1/1";
				}

				// load the document and namespace
				var root = XElement.Load(fileName);
				XmlDocument doc = new XmlDocument();
				doc.Load(fileName);
				ns = new XmlNamespaceManager(doc.NameTable);
				ns.AddNamespace("ns", xmlns);

				// read the top level (gpx) node
				var gpxNode = ReadGpx(doc, gpx);
				if (gpxNode == null)
				{
					return null;
				}

				// read the metadata, routes, waypoints and tracks
				ReadMetadata(gpxNode, gpx.Metadata);
				ReadRoutes(gpxNode, gpx.Routes);
				ReadWaypoints(gpxNode, gpx.Waypoints);
				ReadTracks(gpxNode, gpx.Tracks);
			}
			catch
			{
				return null;
			}
			return gpx;
		}

		private static GpxBounds ReadBounds(XmlNode parentNode, string name)
		{
			var node = parentNode.SelectSingleNode("ns:" + name, ns);
			if (node != null)
			{
				var bounds = new GpxBounds();
				bounds.MinLatitude = ReadDoubleAttr(node, "minlat");
				bounds.MaxLatitude = ReadDoubleAttr(node, "maxlat");
				bounds.MinLongitude = ReadDoubleAttr(node, "minlon");
				bounds.MaxLongitude = ReadDoubleAttr(node, "maxlon");
				return bounds;
			}
			return null;
		}

		private static GpxCopyright ReadCopyright(XmlNode parentNode, string name)
		{
			var node = parentNode.SelectSingleNode("ns:" + name, ns);
			if (node != null)
			{
				var copyright = new GpxCopyright();
				copyright.Author = ReadStringAttr(node, "author");
				copyright.Year = ReadString(node, "year");
				copyright.Licence = ReadUri(node, "license");
				return copyright;
			}
			return null;
		}

		private static DateTime? ReadDateTime(XmlNode parentNode, string name)
		{
			var node = parentNode.SelectSingleNode("ns:" + name, ns);
			if (node != null && DateTime.TryParse(node.InnerText, out DateTime value))
			{
				return value;
			}
			return null;
		}

		private static double? ReadDouble(XmlNode parentNode, string name)
		{
			var node = parentNode.SelectSingleNode("ns:" + name, ns);
			if (node != null && double.TryParse(node.InnerText, out double value))
			{
				return value;
			}
			return null;
		}

		private static double ReadDoubleAttr(XmlNode node, string name)
		{
			var attr = node.Attributes[name];
			if (attr != null && double.TryParse(attr.InnerText, out double value))
			{
				return value;
			}
			return 0;
		}

		private static GpxEmail ReadEmail(XmlNode parentNode, string name)
		{
			var node = parentNode.SelectSingleNode("ns:" + name, ns);
			if (node != null)
			{
				var email = new GpxEmail();
				email.Id = ReadStringAttr(node, "id");
				email.Domain = ReadStringAttr(node, "domain");
				return email;
			}
			return null;
		}

		private static GpxFix? ReadFix(XmlNode parentNode)
		{
			var fixNode = parentNode.SelectSingleNode("ns:fix", ns);
			if (fixNode != null)
			{
				switch (fixNode.InnerText.ToLower())
				{
					case "none":
						return GpxFix.None;
					case "2d":
						return GpxFix.TwoD;
					case "3d":
						return GpxFix.ThreeD;
					case "dgps":
						return GpxFix.DGPS;
					case "pps":
						return GpxFix.PPS;
				}
			}
			return null;
		}

		private static XmlNode ReadGpx(XmlDocument doc, Gpx gpx)
		{
			var gpxNode = doc.SelectSingleNode("//ns:gpx", ns);
			if (gpxNode == null)
			{
				return null;
			}
			gpx.Version = ReadStringAttr(gpxNode, "version");
			gpx.Creator = ReadStringAttr(gpxNode, "creator");
			return gpxNode;
		}

		private static GpxLink ReadLink(XmlNode parentNode)
		{
			var node = parentNode.SelectSingleNode("ns:link", ns);
			if (node != null)
			{
				var link = new GpxLink();
				link.Href = ReadUriAttr(node, "href");
				link.Text = ReadString(node, "text");
				link.Type = ReadString(node, "type");
				return link;
			}
			return null;
		}

		private static void ReadLinks(XmlNode parentNode, GpxLinks links)
		{
			links.Clear();
			var nodes = parentNode.SelectNodes("ns:link", ns);
			foreach (XmlNode node in nodes)
			{
				var link = new GpxLink();
				link.Href = ReadUriAttr(node, "href");
				link.Text = ReadString(node, "text");
				link.Type = ReadString(node, "type");
				links.Add(link);
			}
		}

		private static void ReadMetadata(XmlNode parentNode, GpxMetadata metadata)
		{
			var node = parentNode.SelectSingleNode("ns:metadata", ns);
			if (node != null)
			{
				metadata.Name = ReadString(node, "name");
				metadata.Description = ReadString(node, "desc");
				metadata.Author = ReadPerson(node, "author");
				metadata.Copyright = ReadCopyright(node, "copyright");
				ReadLinks(node, metadata.Links);
				metadata.Time = ReadDateTime(node, "time");
				metadata.Keywords = ReadString(node, "keywords");
				metadata.Bounds = ReadBounds(node, "bounds");
			}
		}
		private static GpxPerson ReadPerson(XmlNode parentNode, string name)
		{
			var node = parentNode.SelectSingleNode("ns:" + name, ns);
			if (node != null)
			{
				var person = new GpxPerson();
				person.Name = ReadString(node, "name");
				person.Email = ReadEmail(node, "email");
				person.Link = ReadLink(node);
				return person;
			}
			return null;
		}

		private static GpxPoint ReadPoint(XmlNode ptNode)
		{
			var point = new GpxPoint();
			point.Latitude = ReadDoubleAttr(ptNode, "lat");
			point.Longitude = ReadDoubleAttr(ptNode, "lon");
			point.Elevation = ReadDouble(ptNode, "ele");
			point.Time = ReadDateTime(ptNode, "time");
			point.MagneticVariation = ReadDouble(ptNode, "magvar");
			point.GeoidHeight = ReadDouble(ptNode, "geoidheight");
			point.Name = ReadString(ptNode, "name");
			point.Comment = ReadString(ptNode, "cmt");
			point.Description = ReadString(ptNode, "desc");
			point.Source = ReadString(ptNode, "src");
			ReadLinks(ptNode, point.Links);
			point.SymbolName = ReadString(ptNode, "sym");
			point.Type = ReadString(ptNode, "type");
			point.Fix = ReadFix(ptNode);
			point.NumSatellites = ReadUInt(ptNode, "sat");
			point.Hdop = ReadDouble(ptNode, "hdop");
			point.Pdop = ReadDouble(ptNode, "pdop");
			point.Vdop = ReadDouble(ptNode, "vdop");
			point.AgeOfDgpsData = ReadDouble(ptNode, "ageofdgpsdata");
			point.DgpsId = ReadUInt(ptNode, "dgpsid");
			return point;
		}
		
		private static GpxRoute ReadRoute(XmlNode rteNode)
		{
			var route = new GpxRoute();
			route.Name= ReadString(rteNode, "name");
			route.Comment = ReadString(rteNode, "cmt");
			route.Description = ReadString(rteNode, "desc");
			route.Source = ReadString(rteNode, "src");
			ReadLinks(rteNode, route.Links);
			route.Number = ReadUInt(rteNode, "number");
			route.Type = ReadString(rteNode, "type");
			var nodes = rteNode.SelectNodes("ns:rtept", ns);
			foreach (XmlNode node in nodes)
			{
				var point = ReadPoint(node);
				route.Points.Add(point);
			}
			return route;
		}

		private static void ReadRoutes(XmlNode gpxNode, GpxRoutes routes)
		{
			routes.Clear();
			var nodes = gpxNode.SelectNodes("ns:rte", ns);
			foreach (XmlNode node in nodes)
			{
				var route = ReadRoute(node);
				routes.Add(route);
			}
		}

		private static string ReadString(XmlNode parentNode, string name)
		{
			var node = parentNode.SelectSingleNode("ns:" + name, ns);
			if (node != null)
			{
				return node.InnerText;
			}
			return null;
		}

		private static string ReadStringAttr(XmlNode node, string name)
		{
			var str = node.Attributes[name];
			if (str != null)
			{
				return str.InnerText;
			}
			return null;
		}

		private static GpxTrack ReadTrack(XmlNode trkNode)
		{
			var track = new GpxTrack();
			track.Name = ReadString(trkNode, "name");
			track.Comment = ReadString(trkNode, "cmt");
			track.Description = ReadString(trkNode, "desc");
			track.Source = ReadString(trkNode, "src");
			ReadLinks(trkNode, track.Links);
			track.Number = ReadUInt(trkNode, "number");
			track.Type = ReadString(trkNode, "type");
			var nodes = trkNode.SelectNodes("ns:trkseg", ns);
			foreach (XmlNode node in nodes)
			{
				var segment = ReadTrackSegment(node);
				segment.Track = track;
				track.Segments.Add(segment);
			}
			return track;
		}

		private static GpxTrackSegment ReadTrackSegment(XmlNode trkNode)
		{
			var segment = new GpxTrackSegment();
			var nodes = trkNode.SelectNodes("ns:trkpt", ns);
			foreach (XmlNode node in nodes)
			{
				var point = ReadPoint(node);
				segment.Points.Add(point);
			}
			return segment;
		}

		private static void ReadTracks(XmlNode gpxNode, GpxTracks tracks)
		{
			tracks.Clear();
			var nodes = gpxNode.SelectNodes("ns:trk", ns);
			foreach (XmlNode node in nodes)
			{
				var track = ReadTrack(node);
				tracks.Add(track);
			}
		}

		private static uint? ReadUInt(XmlNode parentNode, string name)
		{
			var node = parentNode.SelectSingleNode("ns:" + name, ns);
			if (node != null && uint.TryParse(node.InnerText, out uint value))
			{
				return value;
			}
			return null;
		}

		private static Uri ReadUri(XmlNode parentNode, string name)
		{
			var node = parentNode.SelectSingleNode("ns:" + name, ns);
			if (node != null)
			{
				return new Uri(node.InnerText);
			}
			return null;
		}

		private static Uri ReadUriAttr(XmlNode node, string name)
		{
			var uri = node.Attributes[name];
			if (uri != null)
			{
				return new Uri(uri.InnerText);
			}
			return null;
		}

		private static void ReadWaypoints(XmlNode gpxNode, GpxPoints waypoints)
		{
			waypoints.Clear();
			var nodes = gpxNode.SelectNodes("ns:wpt", ns);
			foreach (XmlNode node in nodes)
			{
				var waypoint = ReadPoint(node);
				waypoints.Add(waypoint);
			}
		}
	}
}
