using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace u2ignore {
	class MainClass {
		public static int Main (string [] args)
		{
			if (args.Length != 1) {
				Console.WriteLine ("This tool will copy entries for a particular rule from *.unclassified files to the corresponding *.ignore files.");
				Console.WriteLine ("This can be useful when writing a new rule that has a lot of failures.");
				Console.WriteLine ("The one and only argument must be the rule's identifier (!...!)");
				return 1;
			}
			var id = args [0];

			var dir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			while (Path.GetFileName (dir) != "xtro-sharpie")
				dir = Path.GetDirectoryName (dir);

			var ignored_files = Directory.GetFiles (dir, "*.unclassified", SearchOption.TopDirectoryOnly);
			var dict = new Dictionary<string, Dictionary<string, List<string>>> ();
			foreach (var ignored in ignored_files) {
				var name = Path.GetFileNameWithoutExtension (ignored);
				var split = name.Split ('-');
				var platform = split [0];
				var framework = split [1];
				if (!dict.TryGetValue (framework, out var entries))
					dict [framework] = entries = new Dictionary<string, List<string>> ();
				foreach (var line in File.ReadAllLines (ignored)) {
					if (!line.Contains (id))
						continue;
					if (!entries.TryGetValue (line, out var list))
						entries [line] = list = new List<string> ();
					list.Add (platform);
				}
			}

			var header = new string [] { "", $"# Initial result from new rule {id}" };
			foreach (var kvp in dict) {
				var framework = kvp.Key;
				var entries = kvp.Value;
				var written_to = new HashSet<string> ();
				foreach (var kvp2 in entries.OrderBy ((v) => v.Key)) {
					var failure = kvp2.Key;
					var platforms = kvp2.Value;

					string [] files;
					if (platforms.Count == 4) {
						// same failure in all platforms, the result goes into the common file.
						files = new string [] { "common" };
					} else {
						files = platforms.ToArray ();
					}
					foreach (var file in files) {
						var path = Path.Combine (dir, $"{file}-{framework}.ignore");
						if (written_to.Add (path)) {
							if (File.Exists (path) && !File.ReadAllText (path).EndsWith ("\n", StringComparison.Ordinal))
								File.AppendAllLines (path, new string [] { "" });
							File.AppendAllLines (path, header);
						}
						File.AppendAllLines (path, new string [] { failure });
					}
				}
			}


			return 0;
		}
	}
}
