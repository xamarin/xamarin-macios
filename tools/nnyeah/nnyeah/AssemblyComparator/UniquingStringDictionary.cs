using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.MaciOS.Nnyeah.AssemblyComparator {
	[XmlRoot ("dictionary")]
	public class UniquingStringDictionary : Dictionary<string, string>, IXmlSerializable {
		// if the key and the value are the same, we will serialized out the
		// UnusedStringValue instead.
		const string UnusedStringValue = "^";
		const string kKey = "k";
		const string kValue = "v";
		const string kItem = "i";

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void ReadXml (System.Xml.XmlReader reader)
		{
			XmlSerializer keySerializer = new XmlSerializer (typeof (string));
			XmlSerializer valueSerializer = new XmlSerializer (typeof (string));

			bool wasEmpty = reader.IsEmptyElement;
			reader.Read ();

			if (wasEmpty)
				return;

			while (reader.NodeType != System.Xml.XmlNodeType.EndElement) {
				reader.ReadStartElement (kItem);

				reader.ReadStartElement (kKey);
				var key = (string) keySerializer.Deserialize (reader);
				reader.ReadEndElement ();

				reader.ReadStartElement (kValue);
				var value = (string) valueSerializer.Deserialize (reader);
				value = value != UnusedStringValue ? value : key;
				reader.ReadEndElement ();

				this.Add (key, value);

				reader.ReadEndElement ();
				reader.MoveToContent ();
			}
			reader.ReadEndElement ();
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			XmlSerializer keySerializer = new XmlSerializer (typeof (string));
			XmlSerializer valueSerializer = new XmlSerializer (typeof (string));

			foreach (var kvp in this) {
				writer.WriteStartElement (kItem);

				writer.WriteStartElement (kKey);
				keySerializer.Serialize (writer, kvp.Key);
				writer.WriteEndElement ();

				writer.WriteStartElement (kValue);
				if (kvp.Value == UnusedStringValue) {
					throw new Exception (String.Format(Errors.E0012, kvp.Key, kvp.Value));
				}
				valueSerializer.Serialize (writer, kvp.Key == kvp.Value ? UnusedStringValue : kvp.Value);
				writer.WriteEndElement ();

				writer.WriteEndElement ();
			}
		}
	}
}
