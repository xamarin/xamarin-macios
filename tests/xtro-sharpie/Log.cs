using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Extrospection {

	static class Log {
		static Dictionary<string, List<string>> lists = new Dictionary<string, List<string>> (StringComparer.OrdinalIgnoreCase);

		public static IList<string> On (string fx)
		{
			List<string> list;
			if (!lists.TryGetValue (fx, out list)) {
				list = new List<string> ();
				lists.Add (fx, list);
			}
			return list;
		}

		public static void Save (string outputDirectory)
		{
			if (!string.IsNullOrEmpty (outputDirectory))
				Directory.CreateDirectory (outputDirectory);

			foreach (var kvp in lists) {
				var framework = kvp.Key;
				var list = kvp.Value.Distinct ().ToList ();

				// not generally useful but we want to keep the data sane
				var raw = Path.Combine (outputDirectory, $"{Helpers.Platform}-{framework}.raw");
				File.WriteAllLines (raw, list);

				// load ignore and pending files and remove them
				// 1. common.framework.ignore - long term (shared cross platforms) **preferred**
				Remove (list, Path.Combine (outputDirectory, $"common-{framework}.ignore"));
				// 2. platform.framework.ignore - long term (platform specific) **special cases**
				Remove (list, Path.Combine (outputDirectory, $"{Helpers.Platform}-{framework}.ignore"));
				// 3. platform.framework.pending - short term
				Remove (list, Path.Combine (outputDirectory, $"{Helpers.Platform}-{framework}.todo"));

				var fname = Path.Combine (outputDirectory, $"{Helpers.Platform}-{framework}.unclassified");
				if (list.Count == 0) {
					if (File.Exists (fname))
						File.Delete (fname);
				} else {
					list.Sort ();
					File.WriteAllLines (fname, list);
				}
			}
		}

		static void Remove (IList<string> list, string file)
		{
			if (!File.Exists (file))
				return;
			foreach (var line in File.ReadAllLines (file)) {
				if (line.StartsWith ("!", StringComparison.Ordinal))
					list.Remove (line);
			}
		}
	}
}
