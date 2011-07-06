using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace VotGESOrders.Web.Models
{
	public static class XMLStringSerializer
	{
		public static string Serialize<T>(T data) {
			using (var memoryStream = new MemoryStream()) {
				var serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(memoryStream, data);

				memoryStream.Seek(0, SeekOrigin.Begin);

				var reader = new StreamReader(memoryStream);
				string content = reader.ReadToEnd();
				content = content.Replace("encoding=\"utf-8\"", "");
				content = Uri.EscapeUriString(content);
				return content;
			}
		}

		public static T Deserialize<T>(string xml) {
			xml = Uri.UnescapeDataString(xml);
			using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(xml))) {
				var serializer = new XmlSerializer(typeof(T));
				T theObject = (T)serializer.Deserialize(stream);
				return theObject;
			}
		}
		
	}

}