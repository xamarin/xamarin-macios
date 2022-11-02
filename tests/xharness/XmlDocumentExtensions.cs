using System;
using System.IO;
using System.Xml;

namespace Xharness {
	public static class XmlDocumentExtensions {

		public static void Save (this XmlDocument doc, string path, IHarness harness) =>
			doc.Save (path, (level, message) => harness.Log (level, message));

		public static void Save (this XmlDocument doc, string path, Action<int, string> log)
		{
			if (!File.Exists (path)) {
				Directory.CreateDirectory (Path.GetDirectoryName (path));
				doc.Save (path);
				log?.Invoke (1, $"Created {path}");
			} else {
				var tmpPath = path + ".tmp";
				doc.Save (tmpPath);
				var existing = File.ReadAllText (path);
				var updated = File.ReadAllText (tmpPath);

				if (existing == updated) {
					File.Delete (tmpPath);
					log?.Invoke (1, $"Not saved {path}, no change");
				} else {
					File.Delete (path);
					File.Move (tmpPath, path);
					log?.Invoke (1, $"Updated {path}");
				}
			}
		}
	}
}
