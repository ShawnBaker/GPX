using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// Reads a Gpx object from a file.
	/// </summary>
	public static class GpxReader
	{
		private static XmlNamespaceManager ns;

        /// <summary>
        /// Asynchronously loads a GPX file from an XML reader.
        /// </summary>
        /// <param name="reader">XML reader containing the GPX file.</param>
        /// <returns>A Gpx object, null if the stream fails to load.</returns>
        public static async Task<Gpx> LoadAsync(XmlReader reader)
		{
            return await Task.Run(() => Load(reader));
		}

        /// <summary>
        /// Asynchronously loads a GPX file.
        /// </summary>
        /// <param name="fileName">Full path and name of the GPX file.</param>
        /// <returns>A Gpx object, null if the file fails to load.</returns>
        public static async Task<Gpx> LoadAsync(string fileName)
        {
            return await Task.Run(() => Load(fileName));
        }

        /// <summary>
        /// Asynchronously loads a GPX file from a text reader.
        /// </summary>
        /// <param name="reader">Text reader containing the GPX file.</param>
        /// <returns>A Gpx object, null if the stream fails to load.</returns>
        public static async Task<Gpx> LoadAsync(TextReader reader)
        {
            return await Task.Run(() => Load(reader));
        }

        /// <summary>
        /// Asynchronously loads a GPX file from a stream.
        /// </summary>
        /// <param name="stream">Stream containing the GPX file.</param>
        /// <returns>A Gpx object, null if the stream fails to load.</returns>
        public static async Task<Gpx> LoadAsync(Stream stream)
        {
            return await Task.Run(() => Load(stream));
        }

        /// <summary>
        /// Loads a GPX file from an XML reader.
        /// </summary>
        /// <param name="reader">XML reader containing the GPX file.</param>
        /// <returns>A Gpx object, null if the stream fails to load.</returns>
        public static Gpx Load(XmlReader reader)
		{
            if (reader == null)
            {
                return null;
            }

            var gpx = new Gpx();
			try
			{
				// load the document
				XmlDocument doc = new XmlDocument();
                doc.Load(reader);
				XmlNode gpxNode = null;
				foreach (XmlNode node in doc.ChildNodes)
				{
					if (node.Name == "gpx")
					{
						gpxNode = node;
						break;
					}
				}
				if (gpxNode == null)
				{
					return null;
				}
				
				// get the namespace
				ns = new XmlNamespaceManager(doc.NameTable);
				string xmlns = gpxNode.Attributes["xmlns"]?.Value;
				if (string.IsNullOrEmpty(xmlns))
				{
					return null;
				}
				ns.AddNamespace("ns", xmlns);

				// add the extension namespaces
				foreach (XmlAttribute attr in gpxNode.Attributes)
				{
					if (attr.Name.StartsWith("xmlns") && attr.Name != "xmlns")
					{
						GpxNamespace newNs = new GpxNamespace();
						string[] parts = attr.Name.Split(new char[] { ':' });
						newNs.Name = parts[1];
						newNs.Value = attr.Value;
						gpx.Namespaces.Add(newNs);
						ns.AddNamespace(newNs.Name, newNs.Value);
					}
				}

                // read the top level (gpx) node
                ReadGpx(gpxNode, gpx);

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

        /// <summary>
        /// Loads a GPX file.
        /// </summary>
        /// <param name="fileName">Full path and name of the GPX file.</param>
        /// <returns>A Gpx object, null if the file fails to load.</returns>
        public static Gpx Load(string fileName)
		{
			Gpx gpx = null;
			XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(fileName);
                gpx = Load(reader);
            }
            finally
            {
                reader?.Close();
            }
			return gpx;
		}

        /// <summary>
        /// Loads a GPX file from a text reader.
        /// </summary>
        /// <param name="reader">Text reader containing the GPX file.</param>
        /// <returns>A Gpx object, null if the stream fails to load.</returns>
        public static Gpx Load(TextReader reader)
        {
            Gpx gpx = null;
            XmlTextReader xmlReader = null;
            try
            {
                xmlReader = new XmlTextReader(reader);
                gpx = Load(xmlReader);
            }
            finally
            {
                xmlReader?.Close();
            }
			return gpx;
        }

        /// <summary>
        /// Loads a GPX file from a stream.
        /// </summary>
        /// <param name="stream">Stream containing the GPX file.</param>
        /// <returns>A Gpx object, null if the stream fails to load.</returns>
        public static Gpx Load(Stream stream)
		{
            Gpx gpx = null;
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(stream);
                gpx = Load(reader);
            }
            finally
            {
                reader?.Close();
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

		private static GpxExtension ReadExtension(XmlNode node)
		{
			// add the extension namespaces
			var extension = new GpxExtension();
			var names = node.Name.Split(':');
			extension.Prefix = names[0];
			extension.Name = names[1];
			extension.Value = node.InnerText;
			foreach (XmlAttribute attr in node.Attributes)
			{
				if (attr.Name.ToLower().StartsWith("xmlns:"))
				{
					extension.Namespace = new GpxNamespace();
					names = attr.Name.Split(':');
					extension.Namespace.Name = names[1];
					extension.Namespace.Value = attr.Value;
				}
			}
			foreach (XmlNode childNode in node.ChildNodes)
			{
				if (!childNode.Name.StartsWith("#"))
				{
					extension.Children.Add(ReadExtension(childNode));
				}
			}
			return extension;
		}

		private static bool ReadExtensions(XmlNode parentNode, GpxExtensionList extensions)
		{
			extensions.Clear();
			var node = parentNode.SelectSingleNode("ns:extensions", ns);
			if (node != null)
			{
				foreach (XmlNode extNode in node.ChildNodes)
				{
					var ext = ReadExtension(extNode);
					extensions.Add(ext);
				}
			}
			return false;
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

		private static void ReadGpx(XmlNode gpxNode, Gpx gpx)
		{
			gpx.Version = ReadStringAttr(gpxNode, "version");
			gpx.Creator = ReadStringAttr(gpxNode, "creator");
			ReadExtensions(gpxNode, gpx.Extensions);
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

		private static void ReadLinks(XmlNode parentNode, GpxLinkList links)
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
				ReadExtensions(node, metadata.Extensions);
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

		private static GpxPoint ReadPoint(XmlNode node)
		{
			var point = new GpxPoint();
			point.Latitude = ReadDoubleAttr(node, "lat");
			point.Longitude = ReadDoubleAttr(node, "lon");
			point.Elevation = ReadDouble(node, "ele");
			point.Time = ReadDateTime(node, "time");
			point.MagneticVariation = ReadDouble(node, "magvar");
			point.GeoidHeight = ReadDouble(node, "geoidheight");
			point.Name = ReadString(node, "name");
			point.Comment = ReadString(node, "cmt");
			point.Description = ReadString(node, "desc");
			point.Source = ReadString(node, "src");
			ReadLinks(node, point.Links);
			point.SymbolName = ReadString(node, "sym");
			point.Type = ReadString(node, "type");
			point.Fix = ReadFix(node);
			point.NumSatellites = ReadUInt(node, "sat");
			point.Hdop = ReadDouble(node, "hdop");
			point.Pdop = ReadDouble(node, "pdop");
			point.Vdop = ReadDouble(node, "vdop");
			point.AgeOfDgpsData = ReadDouble(node, "ageofdgpsdata");
			point.DgpsId = ReadUInt(node, "dgpsid");
			ReadExtensions(node, point.Extensions);
			return point;
		}
		
		private static GpxRoute ReadRoute(XmlNode node)
		{
			var route = new GpxRoute();
			route.Name= ReadString(node, "name");
			route.Comment = ReadString(node, "cmt");
			route.Description = ReadString(node, "desc");
			route.Source = ReadString(node, "src");
			ReadLinks(node, route.Links);
			route.Number = ReadUInt(node, "number");
			route.Type = ReadString(node, "type");
			ReadExtensions(node, route.Extensions);
			var nodes = node.SelectNodes("ns:rtept", ns);
			foreach (XmlNode ptNode in nodes)
			{
				var point = ReadPoint(ptNode);
				route.Points.Add(point);
			}
			return route;
		}

		private static void ReadRoutes(XmlNode gpxNode, GpxRouteList routes)
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

		private static GpxTrack ReadTrack(XmlNode node)
		{
			var track = new GpxTrack();
			track.Name = ReadString(node, "name");
			track.Comment = ReadString(node, "cmt");
			track.Description = ReadString(node, "desc");
			track.Source = ReadString(node, "src");
			ReadLinks(node, track.Links);
			track.Number = ReadUInt(node, "number");
			track.Type = ReadString(node, "type");
			ReadExtensions(node, track.Extensions);
			var nodes = node.SelectNodes("ns:trkseg", ns);
			foreach (XmlNode segNode in nodes)
			{
				var segment = ReadTrackSegment(segNode);
				segment.Track = track;
				track.Segments.Add(segment);
			}
			return track;
		}

		private static GpxTrackSegment ReadTrackSegment(XmlNode node)
		{
			var segment = new GpxTrackSegment();
			ReadExtensions(node, segment.Extensions);
			var nodes = node.SelectNodes("ns:trkpt", ns);
			foreach (XmlNode ptNode in nodes)
			{
				var point = ReadPoint(ptNode);
				segment.Points.Add(point);
			}
			return segment;
		}

		private static void ReadTracks(XmlNode gpxNode, GpxTrackList tracks)
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

		private static void ReadWaypoints(XmlNode gpxNode, GpxPointList waypoints)
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
