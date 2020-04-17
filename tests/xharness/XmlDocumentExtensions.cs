using System;
using System.IO;
using System.Xml;

namespace Xharness {
	public static class XmlDocumentExtensions {

		public static void Save (this XmlDocument doc, string path, IHarness harness)
		{
			if (!File.Exists (path)) {
				doc.Save (path);
				harness.Log (1, "Created {0}", path);
			} else {
				var tmpPath = path + ".tmp";
				doc.Save (tmpPath);
				var existing = File.ReadAllText (path);
				var updated = File.ReadAllText (tmpPath);

				if (existing == updated) {
					File.Delete (tmpPath);
					harness.Log (1, "Not saved {0}, no change", path);
				} else {
					File.Delete (path);
					File.Move (tmpPath, path);
					harness.Log (1, "Updated {0}", path);
				}
			}
		}
	}
}
