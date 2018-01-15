using System;
using System.Collections.Generic;
using System.IO;

namespace Extrospection {
	class Sanitizer {

		static readonly string [] Platforms = new [] { "iOS", "tvOS", "watchOS", "macOS" };

		static bool IsEntry (string line)
		{
			return line.Length > 0 && line [0] == '!';
		}

		// Entries are correct if each line
		// * is empty; or
		// * starts with a '!' (entries); or 
		// * starts with a "#" (comments) except for
		// 	*  "#!" since it's a commented entry (and should not be committed)
		static void CorrectEntries (List<string> entries, string filename)
		{
			foreach (var entry in entries) {
				if (IsEntry (entry))
					continue;
				if (entry.Length == 0)
					continue;
				if (entry [0] != '#') {
					Log ($"?bad-entry? '{entry}' in '{filename}'");
				} else if (entry [1] == '!') {
					Log ($"?bad-comment? '{entry}' in '{filename}'");
				}
			}
		}

		// Comments can be duplicated but not entries
		static void UniqueEntries (List<string> entries, string filename)
		{
			for (int i = 0; i < entries.Count; i++) {
				var entry = entries [i];
				if (!IsEntry (entry))
					continue;
				for (int j = i + 1; j < entries.Count; j++) {
					if (entry == entries [j])
						Log ($"?dupe-entry? {entry} in '{filename}'");
				}
			}
		}

		// it's either common (for all/most) or platform specific - not both
		static void NoDuplicateInCommonIgnores ()
		{
			foreach (var kvp in commons) {
				var fx = kvp.Key;
				var common = kvp.Value;
				foreach (var platform in Platforms) {
					var p = Path.Combine (directory, $"{platform}-{fx}.ignore");
					if (!File.Exists (p))
						continue;
					foreach (var entry in File.ReadAllLines (p)) {
						if (!IsEntry (entry))
							continue;
						if (!common.Contains (entry))
							continue;
						Log ($"?dupe-common? Entry '{entry}' in both 'common-{fx}.ignore' and '{Path.GetFileName (p)}' files");
					}
				}
			}
		}

		// it's either something in our todo list or something we can ignore - not both
		static void NoIgnoredTodo ()
		{
			foreach (var file in Directory.GetFiles (directory, "*.todo")) {
				var last = file.LastIndexOf ('-');
				var fx = file.Substring (last + 1, file.Length - last - 6);
				// check if it's in common or in the same platform
				if (commons.TryGetValue (fx, out var common)) {
					foreach (var entry in File.ReadAllLines (file)) {
						if (!IsEntry (entry))
							continue;
						if (common.Contains (entry))
							Log ($"?dupe-todo? Entry '{entry}' in both 'common-{fx}.ignore' and '{Path.GetFileName (file)}' files");
					}
				}
				var platform = Path.ChangeExtension (file, ".ignore");
				if (File.Exists (platform)) {
					var specific = new List<string> (File.ReadAllLines (platform));
					foreach (var entry in File.ReadAllLines (file)) {
						if (!IsEntry (entry))
							continue;
						if (specific.Contains (entry))
							Log ($"?dupe-todo? Entry '{entry}' in both '{Path.GetFileName (platform)}' and '{Path.GetFileName (file)}' files");
					}
				}
			}
		}

		static void NoFixedTodo ()
		{
			foreach (var file in Directory.GetFiles (directory, "*.todo")) {
				var last = file.LastIndexOf ('-');
				var fx = file.Substring (last + 1, file.Length - last - 6);
				var raw = Path.ChangeExtension (file, ".raw");
				if (File.Exists (raw)) {
					var specific = new List<string> (File.ReadAllLines (raw));
					foreach (var entry in File.ReadAllLines (file)) {
						if (!IsEntry (entry))
							continue;
						if (!specific.Contains (entry))
							Log ($"?fixed-todo? Entry '{entry}' in '{Path.GetFileName (file)}' is not found in corresponding '{Path.GetFileName (raw)}' file");
					}
				} else {
					// no .raw then everything is fixed
					foreach (var entry in File.ReadAllLines (file)) {
						if (!IsEntry (entry))
							continue;
						Log ($"?fixed-todo? Entry '{entry}' in '{Path.GetFileName (file)}' might be fixed since there's no corresponding '{Path.GetFileName (raw)}' file");
					}
				}
			}
		}

		static string directory;
		static Dictionary<string, List<string>> commons = new Dictionary<string, List<string>> ();
		static int count;

		public static void Log (string s)
		{
			Console.WriteLine (s);
			count++;
		}

		public static int Main (string [] args)
		{
			directory = args.Length == 0 ? "." : args [0];

			// cache stuff
			foreach (var file in Directory.GetFiles (directory, "common-*.ignore")) {
				var path = Path.GetFileName (file); 
				var fx = path.Substring (7, path.Length - 14);
				var common = new List<string> (File.ReadAllLines (file));
				commons.Add (fx, common); 
			}

			// *.ignore validations

			// basic sanity for ignores
			foreach (var kvp in commons) {
				var fx = kvp.Key;
				var common = kvp.Value;
				CorrectEntries (common, $"common-{fx}.ignore");
			}
			foreach (var file in Directory.GetFiles (directory, "*.ignore")) {
				var filename = Path.GetFileName (file);
				// already processed from cache - don't reload them
				if (filename.StartsWith ("common-", StringComparison.Ordinal))
					continue;
				var entries = new List<string> (File.ReadAllLines (file));
				CorrectEntries (entries, filename);
			}

			// uniqueness - check that each .ignore file has no duplicate
			foreach (var kvp in commons) {
				var fx = kvp.Key;
				var common = kvp.Value;
				UniqueEntries (common, $"common-{fx}.ignore");
			}
			foreach (var file in Directory.GetFiles (directory, "*.ignore")) {
				var entries = new List<string> (File.ReadAllLines (file));
				UniqueEntries (entries, Path.GetFileName (file));
			}

			// platform specific ignore entries should *not* be duplicated in common-*.ignore
			NoDuplicateInCommonIgnores ();

			// TODO entries present in all platforms .ignore files should be moved to common

			// ignored entries should all exists in the unfiltered .unclassified (raw)
			// * common-{fx}.ignored must be part of _at least_ one *-{fx}.raw file
			foreach (var kvp in commons) {
				var fx = kvp.Key;
				var common = kvp.Value;
				//ExistingCommonEntries (common, $"common-{fx}.ignore");
				List<string> [] raws = new List<string> [Platforms.Length];
				for (int i=0; i < raws.Length; i++) {
					var fname = Path.Combine (directory, $"{Platforms[i]}-{fx}.raw");
					if (File.Exists (fname))
						raws [i] = new List<string> (File.ReadAllLines (fname));
					else
						raws [i] = new List<string> ();
				}
				foreach (var entry in common) {
					if (!entry.StartsWith ("!", StringComparison.Ordinal))
						continue;
					bool found = false;
					foreach (var platform in raws) {
						found = platform.Contains (entry);
						if (found)
							break;
					}
					if (!found)
						Log ($"?unknown-entry? {entry} in 'common-{fx}.ignore'");
				}
			}
			// * a platform ignore must existing in it's corresponding raw file
			foreach (var file in Directory.GetFiles (directory, "*.ignore")) {
				var shortname = Path.GetFileName (file);
				if (shortname.StartsWith ("common-", StringComparison.Ordinal))
					continue;
				// FIXME temporary hack for old data files
				if (!shortname.Contains ("-"))
					continue;
				var rawfile = Path.ChangeExtension (file, ".raw");
				var raws = new List<string> (File.ReadAllLines (rawfile));
				foreach (var entry in File.ReadAllLines (file)) {
					if (!entry.StartsWith ("!", StringComparison.Ordinal))
						continue;
					if (raws.Contains (entry))
						continue;
					Log ($"?unknown-entry? {entry} in '{shortname}'");
				}
			}

			// *.todo validations

			// entries in .todo files should *not* be present in *.ignore files
			NoIgnoredTodo ();

			// entries in .todo should be found in .raw files - else it's likely fixed (and out of date)
			NoFixedTodo ();

			if (count == 0)
				Console.WriteLine ("Sanity check passed");
			else
				Console.WriteLine ($"Sanity check failed ({count})");

			// useful when updating stuff locally - we report but we don't fail
			return Environment.GetEnvironmentVariable ("XTRO_SANITY_SKIP") == "1" ? 0 : count;
		}
	}
}
