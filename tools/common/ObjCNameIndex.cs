using System;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace ClassRedirector
{
	public class ObjCNameIndex {
		public const string ObjNameIndexName = "ObjNameIndex";
		const string nameName = "Name";
		const string indexName = "Index";
		public ObjCNameIndex (string objCName, int mapIndex)
		{
			ObjCName = objCName;
			MapIndex = mapIndex;
		}
		public string ObjCName { get; private set; }
		public int MapIndex { get; private set; }

		public static XElement ToXElement (ObjCNameIndex nameIndex)
		{
			return new XElement (ObjNameIndexName,
				new XElement (nameName, nameIndex.ObjCName),
				new XElement (indexName, nameIndex.MapIndex));
		}

		public static ObjCNameIndex? FromXElement (XElement? objNameIndex)
		{
			if (objNameIndex is null)
				return null;
			var name = (string?) objNameIndex.Element (nameName);
			var index = (int?) objNameIndex.Element (indexName);
			if (name is null || index is null)
				return null;
			return new ObjCNameIndex (name, index.Value);
		}
	}
}

