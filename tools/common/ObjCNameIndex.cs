using System;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace ClassRedirector {
	public class ObjCNameIndex {
		public ObjCNameIndex (string objCName, int mapIndex)
		{
			ObjCName = objCName;
			MapIndex = mapIndex;
		}
		public string ObjCName { get; private set; }
		public int MapIndex { get; private set; }
	}
}

