using System;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable enable

namespace ClassRedirector
{
	public class CSToObjCMap : Dictionary<string, ObjCNameIndex> {
		const string objMapName = "CSToObjCMap";
		const string elementName = "Element";
		const string csNameName = "CSName";
		public CSToObjCMap () : base ()
		{
		}

		public static XElement ToXElement (CSToObjCMap map)
		{
			return new XElement (objMapName, Elements (map));
		}

		static IEnumerable <XElement> Elements (CSToObjCMap map)
		{
			return map.Select (kvp => new XElement (elementName, new XAttribute (csNameName, kvp.Key), ObjCNameIndex.ToXElement (kvp.Value)));
		}

		public static CSToObjCMap FromXElement (XElement xmap)
		{
			var map = new CSToObjCMap ();
			var elements = from el in xmap.Descendants (elementName)
						   select new KeyValuePair<string?, ObjCNameIndex?> (el.Attribute (csNameName)?.Value,
						   ObjCNameIndex.FromXElement (el.Element (ObjCNameIndex.ObjNameIndexName)));
			foreach (var elem in elements) {
				if (elem.Key is not null && elem.Value is not null)
					map.Add (elem.Key, elem.Value);
			}
			return map;
		}

		public static CSToObjCMap? FromXDocument (XDocument doc)
		{
			var el = doc.Descendants (objMapName).FirstOrDefault ();
			return el is null ? null : FromXElement (el);
		}

		public static XDocument ToXDocument (CSToObjCMap map)
		{
			return new XDocument (ToXElement (map));
		}
	}
}

