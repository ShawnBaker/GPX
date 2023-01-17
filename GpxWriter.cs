using System;
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

		private static XmlWriter textWriter;

		/// <summary>
		/// Saves a GPX file.
		/// </summary>
		/// <param name="gpx">Gpx object containing the data to be saved.</param>
		/// <param name="fileName">Full path and name of the GPX file.</param>
		/// <returns>True if the file was successfully saved, false if not.</returns>
		public static bool Save(Gpx gpx, string fileName)
		{
			XmlWriterSettings settings = new XmlWriterSettings()
			{
				Indent = true,
				IndentChars = "\t"
			};
			textWriter = XmlTextWriter.Create(fileName, settings);
			textWriter.WriteStartDocument();
			textWriter.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
			gpx.Version = "1.1";
			WriteStringAttr("version", gpx.Version);
			WriteStringAttr("creator", gpx.Creator);
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
			textWriter.WriteEndElement();
			textWriter.WriteEndDocument();
			textWriter.Close();
			return true;
		}

		private static void WriteBounds(string name, GpxBounds bounds)
		{
			if (bounds != null)
			{
				textWriter.WriteStartElement(name);
				WriteDoubleAttr("minlat", bounds.MinLatitude);
				WriteDoubleAttr("minlon", bounds.MinLongitude);
				WriteDoubleAttr("maxlat", bounds.MaxLatitude);
				WriteDoubleAttr("maxlon", bounds.MaxLongitude);
				textWriter.WriteEndElement();
			}
		}

		private static void WriteCopyright(string name, GpxCopyright copyright)
		{
			if (copyright != null && copyright.HasData)
			{
				textWriter.WriteStartElement(name);
				WriteStringAttr("author", copyright.Author);
				WriteString("year", copyright.Year);
				WriteUri("license", copyright.Licence);
				textWriter.WriteEndElement();
			}
		}

		private static void WriteDateTime(string name, DateTime? value)
		{
			if (value != null)
			{
				string dt = value.Value.ToUniversalTime().ToString(TimeFormat);
				textWriter.WriteElementString(name, dt);
			}
		}

		private static void WriteDouble(string name, double? value)
		{
			if (value != null)
			{
				textWriter.WriteElementString(name, value.ToString());
			}
		}

		private static void WriteDoubleAttr(string name, double? value)
		{
			if (value != null)
			{
				textWriter.WriteAttributeString(name, value.ToString());
			}
		}

		private static void WriteEmail(string name, GpxEmail email)
		{
			if (email != null && email.HasData)
			{
				textWriter.WriteStartElement(name);
				WriteStringAttr("id", email.Id);
				WriteStringAttr("domain", email.Domain);
				textWriter.WriteEndElement();
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
				textWriter.WriteElementString(name, str);
			}
		}

		private static void WriteLink(string name, GpxLink link)
		{
			if (link != null && link.HasData)
			{
				textWriter.WriteStartElement(name);
				WriteUriAttr("href", link.Href);
				WriteString("text", link.Text);
				WriteString("type", link.Type);
				textWriter.WriteEndElement();
			}
		}

		private static void WriteLinks(string name, GpxLinks links)
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
				textWriter.WriteStartElement("metadata");
				WriteString("name", metadata.Name);
				WriteString("desc", metadata.Description);
				WritePerson("author", metadata.Author);
				WriteCopyright("copyright", metadata.Copyright);
				WriteLinks("link", metadata.Links);
				WriteDateTime("time", metadata.Time);
				WriteString("keywords", metadata.Keywords);
				WriteBounds("bounds", metadata.Bounds);
				textWriter.WriteEndElement();
			}
		}

		private static void WritePerson(string name, GpxPerson person)
		{
			if (person != null && person.HasData)
			{
				textWriter.WriteStartElement(name);
				WriteString("name", person.Name);
				WriteEmail("email", person.Email);
				WriteLink("link", person.Link);
				textWriter.WriteEndElement();
			}
		}

		private static void WritePoint(string name, GpxPoint point)
		{
			textWriter.WriteStartElement(name);
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
			textWriter.WriteEndElement();
		}

		private static void WriteRoute(GpxRoute route)
		{
			if (route != null && route.HasData)
			{
				textWriter.WriteStartElement("rte");
				WriteString("name", route.Name);
				WriteString("cmt", route.Comment);
				WriteString("desc", route.Description);
				WriteString("src", route.Source);
				WriteLinks("link", route.Links);
				WriteUInt("number", route.Number);
				WriteString("type", route.Type);
				foreach (GpxPoint point in route.Points)
				{
					WritePoint("rtept", point);
				}
				textWriter.WriteEndElement();
			}
		}

		private static void WriteString(string name, string str)
		{
			if (str != null)
			{
				textWriter.WriteElementString(name, str);
			}
		}

		private static void WriteStringAttr(string name, string str)
		{
			if (str != null)
			{
				textWriter.WriteAttributeString(name, str);
			}
		}

		private static void WriteTrack(GpxTrack track)
		{
			if (track != null && track.HasData)
			{
				textWriter.WriteStartElement("trk");
				WriteString("name", track.Name);
				WriteString("cmt", track.Comment);
				WriteString("desc", track.Description);
				WriteString("src", track.Source);
				WriteLinks("link", track.Links);
				WriteUInt("number", track.Number);
				WriteString("type", track.Type);
				foreach (GpxTrackSegment segment in track.Segments)
				{
					if (segment.Points.Count > 0)
					{
						textWriter.WriteStartElement("trkseg");
						foreach (GpxPoint point in segment.Points)
						{
							WritePoint("trkpt", point);
						}
						textWriter.WriteEndElement();
					}
				}
				textWriter.WriteEndElement();
			}
		}

		private static void WriteUInt(string name, uint? value)
		{
			if (value != null)
			{
				textWriter.WriteElementString(name, value.ToString());
			}
		}

		private static void WriteUri(string name, Uri uri)
		{
			if (uri != null)
			{
				textWriter.WriteElementString(name, uri.ToString());
			}
		}

		private static void WriteUriAttr(string name, Uri uri)
		{
			if (uri != null)
			{
				textWriter.WriteAttributeString(name, uri.ToString());
			}
		}
	}
}
