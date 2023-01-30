using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Xml;

namespace FrozenNorth.Gpx
{
	/// <summary>
	/// Writes a Gpx object to a file.
	/// </summary>
	public static class GpxWriter
	{
		/// <summary>
		/// Format string for writing the time.
		/// </summary>
		public const string TimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

		// local variables
		private static XmlWriter xmlWriter;
		private static Gpx gpx;

        /// <summary>
        /// Asynchronously saves a GPX file.
        /// </summary>
        /// <param name="gpx">Gpx object containing the data to be saved.</param>
        /// <param name="fileName">Full path and name of the GPX file.</param>
        /// <returns>True if the file was successfully saved, false if not.</returns>
        public static async Task<bool> SaveAsync(Gpx gpx, string fileName)
        {
            return await Task.Run(() => Save(gpx, fileName));
        }

        /// <summary>
        /// Asynchronously saves a GPX file to a stream.
        /// </summary>
        /// <param name="gpx">Gpx object containing the data to be saved.</param>
        /// <param name="stream">Stream to write the XML to.</param>
        /// <returns>True if the file was successfully saved, false if not.</returns>
        public static async Task<bool> SaveAsync(Gpx gpx, Stream stream)
        {
            return await Task.Run(() => Save(gpx, stream));
        }

        /// <summary>
        /// Asynchronously saves a GPX file to a TextWriter.
        /// </summary>
        /// <param name="gpx">Gpx object containing the data to be saved.</param>
        /// <param name="writer">TextWriter to write the XML to.</param>
        /// <returns>True if the file was successfully saved, false if not.</returns>
        public static async Task<bool> SaveAsync(Gpx gpx, TextWriter writer)
        {
            return await Task.Run(() => Save(gpx, writer));
        }

        /// <summary>
        /// Asynchronously saves a GPX file to a XmlWriter.
        /// </summary>
        /// <param name="gpx">Gpx object containing the data to be saved.</param>
        /// <param name="writer">XmlWriter to write the XML to.</param>
        /// <returns>True if the file was successfully saved, false if not.</returns>
        public static async Task<bool> SaveAsync(Gpx gpx, XmlWriter writer)
		{
            return await Task.Run(() => Save(gpx, writer));
        }

        /// <summary>
        /// Saves a GPX file.
        /// </summary>
        /// <param name="gpx">Gpx object containing the data to be saved.</param>
        /// <param name="fileName">Full path and name of the GPX file.</param>
        /// <returns>True if the file was successfully saved, false if not.</returns>
        public static bool Save(Gpx gpx, string fileName)
        {
            return SaveInternal(gpx, XmlWriter.Create(fileName, GetSettings()));
        }

        /// <summary>
        /// Saves a GPX file to a stream.
        /// </summary>
        /// <param name="gpx">Gpx object containing the data to be saved.</param>
        /// <param name="writer">Stream to write the XML to.</param>
        /// <returns>True if the file was successfully saved, false if not.</returns>
        public static bool Save(Gpx gpx, Stream stream)
        {
            return SaveInternal(gpx, XmlWriter.Create(stream, GetSettings()));
        }

        /// <summary>
        /// Saves a GPX file to a TextWriter.
        /// </summary>
        /// <param name="gpx">Gpx object containing the data to be saved.</param>
        /// <param name="writer">TextWriter to write the XML to.</param>
        /// <returns>True if the file was successfully saved, false if not.</returns>
        public static bool Save(Gpx gpx, TextWriter writer)
        {
            return SaveInternal(gpx, XmlWriter.Create(writer, GetSettings()));
        }

		/// <summary>
		/// Saves a GPX file to a XmlWriter.
		/// </summary>
		/// <param name="gpx">Gpx object containing the data to be saved.</param>
		/// <param name="writer">XmlWriter to write the XML to.</param>
		/// <returns>True if the file was successfully saved, false if not.</returns>
		public static bool Save(Gpx gpx, XmlWriter writer)
		{
            return SaveInternal(gpx, XmlWriter.Create(writer, GetSettings()));
        }

        /// <summary>
        /// Saves a GPX file to a XmlWriter.
        /// </summary>
        /// <param name="gpx">Gpx object containing the data to be saved.</param>
        /// <param name="writer">XmlWriter to write the XML to.</param>
        /// <returns>True if the file was successfully saved, false if not.</returns>
        public static bool SaveInternal(Gpx gpx, XmlWriter writer)
        {
            if (gpx == null || writer == null)
            {
                return false;
            }
            try
            {
                GpxWriter.gpx = gpx;
				xmlWriter = writer;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
                gpx.Version = "1.1";
                WriteStringAttr("version", gpx.Version);
                WriteStringAttr("creator", gpx.Creator);
                foreach (var ns in gpx.Namespaces)
                {
                    xmlWriter.WriteAttributeString(ns.Name, "http://www.w3.org/2000/xmlns/", ns.Value);
                }
                WriteMetadata(gpx.Metadata);
                foreach (GpxRoute route in gpx.Routes)
                {
                    WriteRoute(route);
                }
                foreach (GpxPoint point in gpx.Waypoints)
                {
                    WritePoint("wpt", point);
                }
                foreach (GpxTrack track in gpx.Tracks)
                {
                    WriteTrack(track);
                }
                WriteExtensions(gpx.Extensions);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

		private static XmlWriterSettings GetSettings()
		{
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t"
            };
			return settings;
        }

        private static void WriteBounds(string name, GpxBounds bounds)
		{
			if (bounds != null)
			{
				xmlWriter.WriteStartElement(name);
				WriteDoubleAttr("minlat", bounds.MinLatitude);
				WriteDoubleAttr("minlon", bounds.MinLongitude);
				WriteDoubleAttr("maxlat", bounds.MaxLatitude);
				WriteDoubleAttr("maxlon", bounds.MaxLongitude);
				xmlWriter.WriteEndElement();
			}
		}

		private static void WriteCopyright(string name, GpxCopyright copyright)
		{
			if (copyright != null && copyright.HasData)
			{
				xmlWriter.WriteStartElement(name);
				WriteStringAttr("author", copyright.Author);
				WriteString("year", copyright.Year);
				WriteUri("license", copyright.Licence);
				xmlWriter.WriteEndElement();
			}
		}

		private static void WriteDateTime(string name, DateTime? value)
		{
			if (value != null)
			{
				string dt = value.Value.ToUniversalTime().ToString(TimeFormat);
				xmlWriter.WriteElementString(name, dt);
			}
		}

		private static void WriteDouble(string name, double? value)
		{
			if (value != null)
			{
				xmlWriter.WriteElementString(name, value.ToString());
			}
		}

		private static void WriteDoubleAttr(string name, double? value)
		{
			if (value != null)
			{
				xmlWriter.WriteAttributeString(name, value.ToString());
			}
		}

		private static void WriteEmail(string name, GpxEmail email)
		{
			if (email != null && email.HasData)
			{
				xmlWriter.WriteStartElement(name);
				WriteStringAttr("id", email.Id);
				WriteStringAttr("domain", email.Domain);
				xmlWriter.WriteEndElement();
			}
		}

		private static void WriteExtension(GpxExtension extension, GpxNamespace ns)
		{
			if (extension != null)
			{
				if (extension.Children.Count == 0)
				{
					var names = extension.Name.Split(':');
					if (extension.Namespace != null)
					{
						xmlWriter.WriteElementString(extension.Prefix, extension.Name, extension.Namespace.Value, extension.Value);
					}
					else if (extension.Prefix == ns.Name)
					{
						xmlWriter.WriteElementString(extension.Prefix, extension.Name, ns.Value, extension.Value);
					}
					else
					{
						GpxNamespace gpxNs = gpx.Namespaces.Find(n => n.Name == extension.Prefix);
						if (gpxNs != null)
						{
							xmlWriter.WriteElementString(extension.Prefix, extension.Name, gpxNs.Value, extension.Value);
						}
					}
				}
				else
				{
					var names = extension.Name.Split(':');
					if (extension.Namespace != null)
					{
						xmlWriter.WriteStartElement(extension.Prefix, extension.Name, extension.Namespace.Value);
						ns = extension.Namespace;
					}
					else if (extension.Prefix == ns.Name)
					{
						xmlWriter.WriteStartElement(extension.Prefix, extension.Name, ns.Value);
					}
					else
					{
						GpxNamespace gpxNs = gpx.Namespaces.Find(n => n.Name == extension.Prefix);
						if (gpxNs != null)
						{
							xmlWriter.WriteStartElement(extension.Prefix, extension.Name, gpxNs.Value);
						}
					}
					foreach (GpxExtension ext in extension.Children)
					{
						WriteExtension(ext, ns);
					}
					xmlWriter.WriteEndElement();
				}
			}
		}

		private static void WriteExtensions(GpxExtensionList extensions)
		{
			if (extensions != null && extensions.Count > 0)
			{
				xmlWriter.WriteStartElement("extensions");
				foreach (GpxExtension extension in extensions)
				{
					WriteExtension(extension, null);
				}
				xmlWriter.WriteEndElement();
			}
		}

		private static void WriteFix(string name, GpxFix? fix)
		{
			if (fix != null)
			{
				string str = "";
				switch (fix)
				{
					case GpxFix.None:
						str = "none";
						break;
					case GpxFix.TwoD:
						str = "2d";
						break;
					case GpxFix.ThreeD:
						str = "3d";
						break;
					case GpxFix.DGPS:
						str = "dgps";
						break;
					case GpxFix.PPS:
						str = "pps";
						break;
				}
				xmlWriter.WriteElementString(name, str);
			}
		}

		private static void WriteLink(string name, GpxLink link)
		{
			if (link != null && link.HasData)
			{
				xmlWriter.WriteStartElement(name);
				WriteUriAttr("href", link.Href);
				WriteString("text", link.Text);
				WriteString("type", link.Type);
				xmlWriter.WriteEndElement();
			}
		}

		private static void WriteLinks(string name, GpxLinkList links)
		{
			foreach (GpxLink link in links)
			{
				WriteLink(name, link);
			}
		}

		private static void WriteMetadata(GpxMetadata metadata)
		{
			if (metadata.HasData)
			{
				xmlWriter.WriteStartElement("metadata");
				WriteString("name", metadata.Name);
				WriteString("desc", metadata.Description);
				WritePerson("author", metadata.Author);
				WriteCopyright("copyright", metadata.Copyright);
				WriteLinks("link", metadata.Links);
				WriteDateTime("time", metadata.Time);
				WriteString("keywords", metadata.Keywords);
				WriteBounds("bounds", metadata.Bounds);
				WriteExtensions(metadata.Extensions);
				xmlWriter.WriteEndElement();
			}
		}

		private static void WritePerson(string name, GpxPerson person)
		{
			if (person != null && person.HasData)
			{
				xmlWriter.WriteStartElement(name);
				WriteString("name", person.Name);
				WriteEmail("email", person.Email);
				WriteLink("link", person.Link);
				xmlWriter.WriteEndElement();
			}
		}

		private static void WritePoint(string name, GpxPoint point)
		{
			xmlWriter.WriteStartElement(name);
			WriteDoubleAttr("lat", point.Latitude);
			WriteDoubleAttr("lon", point.Longitude);
			WriteDouble("ele", point.Elevation);
			WriteDateTime("time", point.Time);
			WriteDouble("magvar", point.MagneticVariation);
			WriteDouble("geoidheight", point.GeoidHeight);
			WriteString("name", point.Name);
			WriteString("cmt", point.Comment);
			WriteString("desc", point.Description);
			WriteString("src", point.Source);
			WriteLinks("link", point.Links);
			WriteString("sym", point.SymbolName);
			WriteString("type", point.Type);
			WriteFix("fix", point.Fix);
			WriteUInt("sat", point.NumSatellites);
			WriteDouble("hdop", point.Hdop);
			WriteDouble("pdop", point.Pdop);
			WriteDouble("vdop", point.Vdop);
			WriteDouble("ageofdgpsdata", point.AgeOfDgpsData);
			WriteUInt("dgpsid", point.DgpsId);
			WriteExtensions(point.Extensions);
			xmlWriter.WriteEndElement();
		}

		private static void WriteRoute(GpxRoute route)
		{
			if (route != null && route.HasData)
			{
				xmlWriter.WriteStartElement("rte");
				WriteString("name", route.Name);
				WriteString("cmt", route.Comment);
				WriteString("desc", route.Description);
				WriteString("src", route.Source);
				WriteLinks("link", route.Links);
				WriteUInt("number", route.Number);
				WriteString("type", route.Type);
				WriteExtensions(route.Extensions);
				foreach (GpxPoint point in route.Points)
				{
					WritePoint("rtept", point);
				}
				xmlWriter.WriteEndElement();
			}
		}

		private static void WriteString(string name, string str)
		{
			if (str != null)
			{
				xmlWriter.WriteElementString(name, str);
			}
		}

		private static void WriteStringAttr(string name, string str)
		{
			if (str != null)
			{
				xmlWriter.WriteAttributeString(name, str);
			}
		}

		private static void WriteTrack(GpxTrack track)
		{
			if (track != null && track.HasData)
			{
				xmlWriter.WriteStartElement("trk");
				WriteString("name", track.Name);
				WriteString("cmt", track.Comment);
				WriteString("desc", track.Description);
				WriteString("src", track.Source);
				WriteLinks("link", track.Links);
				WriteUInt("number", track.Number);
				WriteString("type", track.Type);
				WriteExtensions(track.Extensions);
				foreach (GpxTrackSegment segment in track.Segments)
				{
					WriteExtensions(segment.Extensions);
					if (segment.Points.Count > 0)
					{
						xmlWriter.WriteStartElement("trkseg");
						foreach (GpxPoint point in segment.Points)
						{
							WritePoint("trkpt", point);
						}
						xmlWriter.WriteEndElement();
					}
				}
				xmlWriter.WriteEndElement();
			}
		}

		private static void WriteUInt(string name, uint? value)
		{
			if (value != null)
			{
				xmlWriter.WriteElementString(name, value.ToString());
			}
		}

		private static void WriteUri(string name, Uri uri)
		{
			if (uri != null)
			{
				xmlWriter.WriteElementString(name, uri.ToString());
			}
		}

		private static void WriteUriAttr(string name, Uri uri)
		{
			if (uri != null)
			{
				xmlWriter.WriteAttributeString(name, uri.ToString());
			}
		}
	}
}
