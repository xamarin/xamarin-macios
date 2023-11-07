using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using Xamarin.Utils;

namespace Extrospection {
	class Sanitizer {

		static List<string> Platforms;
		static List<string> AllPlatforms;
		static bool Autosanitize = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("AUTO_SANITIZE"));

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
				} else if (entry.Length > 1 && entry [1] == '!') {
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
				if (!IsIncluded (file))
					continue;
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

		static bool IsIncluded (string file)
		{
			var name = Path.GetFileName (file);
			foreach (var p in Platforms) {
				if (name.StartsWith (p, StringComparison.Ordinal))
					return true;
			}
			return false;
		}

		static void NoFixedTodo ()
		{
			foreach (var file in Directory.GetFiles (directory, "*.todo")) {
				if (!IsIncluded (file))
					continue;
				var last = file.LastIndexOf ('-');
				var fx = file.Substring (last + 1, file.Length - last - 6);
				var raw = Path.ChangeExtension (file, ".raw");
				var failures = new List<string> ();
				var entries = File.ReadAllLines (file);
				if (File.Exists (raw)) {
					var specific = new List<string> (File.ReadAllLines (raw));
					foreach (var entry in entries) {
						if (!IsEntry (entry))
							continue;
						if (!specific.Contains (entry)) {
							Log ($"?fixed-todo? Entry '{entry}' in '{Path.GetFileName (file)}' is not found in corresponding '{Path.GetFileName (raw)}' file");
							failures.Add (entry);
						}
					}
				} else {
					// no .raw then everything is fixed
					foreach (var entry in entries) {
						if (!IsEntry (entry))
							continue;
						Log ($"?fixed-todo? Entry '{entry}' in '{Path.GetFileName (file)}' might be fixed since there's no corresponding '{Path.GetFileName (raw)}' file");
						failures.Add (entry);
					}
				}
				if (failures.Count > 0 && Autosanitize) {
					var sanitized = new List<string> (entries);
					foreach (var failure in failures)
						sanitized.Remove (failure);
					File.WriteAllLines (file, sanitized);
					// since we are in AUTO_SANITIZE, if the file is empty, remove it.
					if (sanitized.Count == 0) {
						File.Delete (file);
					}
				}
			}
		}

		static void NoEmptyTodo ()
		{
			foreach (var file in Directory.GetFiles (directory, "*.todo")) {
				if (!IsIncluded (file))
					continue;
				if (!(File.ReadLines (file).Count () > 0)) {
					Log ($"?empty-todo? File '{Path.GetFileName (file)}' is empty. Empty todo files should be removed.");
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
			AllPlatforms = args.Skip (1).First ().Split (' ').ToList ();
			Platforms = args.Skip (2).ToList ();

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
				if (!IsIncluded (file))
					continue;
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
			// * common-{fx}.ignored must be part of _all_ one *-{fx}.raw file
			foreach (var kvp in commons) {
				var fx = kvp.Key;
				var common = kvp.Value;
				//ExistingCommonEntries (common, $"common-{fx}.ignore");

				if (!IsFrameworkIncludedInAnySelectedPlatform (fx))
					continue;

				var raws = new RawInfo [Platforms.Count];
				for (int i = 0; i < raws.Length; i++) {
					var fname = Path.Combine (directory, $"{Platforms [i]}-{fx}.raw");
					raws [i] = new RawInfo () {
						Platform = Platforms [i],
					};
					if (File.Exists (fname))
						raws [i].Entries = new HashSet<string> (File.ReadAllLines (fname));
				}
				var unknownFailures = new List<string> ();
				foreach (var entry in common) {
					if (!entry.StartsWith ("!", StringComparison.Ordinal))
						continue;
					var foundRaws = raws.Where (v => v.Entries.Contains (entry));
					var rawCount = foundRaws.Count ();
					if (rawCount == 0) {
						Log ($"?unknown-entry? {entry} in '{Path.Combine (directory, $"common-{fx}.ignore")}'");
						unknownFailures.Add (entry);
					} else if (rawCount < GetPlatformCount (fx)) {
						var notFound = raws.Where (v => !v.Entries.Contains (entry));
						Log ($"?not-common? {entry} in '{Path.Combine (directory, $"common-{fx}.ignore")}': not in {string.Join (", ", notFound.Select (v => v.Platform))}");
						unknownFailures.Add (entry);
						if (Autosanitize) {
							foreach (var nf in foundRaws) {
								var ignore = Path.Combine (directory, $"{nf.Platform}-{fx}.ignore");
								var sanitized = new List<string> ();
								if (File.Exists (ignore))
									sanitized.AddRange (File.ReadAllLines (ignore));
								sanitized.Add (entry);
								File.WriteAllLines (ignore, sanitized);
							}
						}
					}
				}
				if (unknownFailures.Count > 0 && Autosanitize) {
					var sanitized = new List<string> (common);
					foreach (var failure in unknownFailures)
						sanitized.Remove (failure);
					File.WriteAllLines (Path.Combine (directory, $"common-{fx}.ignore"), sanitized);
				}
			}
			// * a platform ignore must existing in it's corresponding raw file
			foreach (var file in Directory.GetFiles (directory, "*.ignore")) {
				if (!IsIncluded (file))
					continue;
				var shortname = Path.GetFileName (file);
				if (shortname.StartsWith ("common-", StringComparison.Ordinal))
					continue;
				// FIXME temporary hack for old data files
				if (!shortname.Contains ("-"))
					continue;
				var rawfile = Path.ChangeExtension (file, ".raw");
				var raws = new List<string> (File.Exists (rawfile) ? File.ReadAllLines (rawfile) : Array.Empty<string> ());
				var failures = new List<string> ();
				var lines = File.ReadAllLines (file);
				foreach (var entry in lines) {
					if (!entry.StartsWith ("!", StringComparison.Ordinal))
						continue;
					if (raws.Contains (entry))
						continue;
					Log ($"?unknown-entry? {entry} in '{shortname}'");
					failures.Add (entry);
				}
				if (failures.Count > 0 && Autosanitize) {
					var sanitized = new List<string> (lines);
					foreach (var failure in failures)
						sanitized.Remove (failure);
					if (sanitized.Count > 0) {
						File.WriteAllLines (file, sanitized);
					} else {
						File.Delete (file);
					}
				}
			}

			// *.todo validations

			// entries in .todo files should *not* be present in *.ignore files
			NoIgnoredTodo ();

			// entries in .todo should be found in .raw files - else it's likely fixed (and out of date)
			NoFixedTodo ();

			// empty files should be removed
			NoEmptyTodo ();

			if (count == 0)
				Console.WriteLine ("Sanity check passed");
			else
				Console.WriteLine ($"Sanity check failed ({count})");

			// useful when updating stuff locally - we report but we don't fail
			var sanitizedOrSkippedSanity =
				!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XTRO_SANITY_SKIP"))
				|| Autosanitize;
			return sanitizedOrSkippedSanity ? 0 : count;
		}

		static List<Frameworks> frameworks; // the frameworks for the selected platforms
		static List<Frameworks> all_frameworks; // the frameworks for all platforms

		// If the given framework is skipped in all the currently building platforms.
		static bool IsFrameworkIncludedInAnySelectedPlatform (string framework)
		{
			return Platforms.Any (v => IsFrameworkIncludedInPlatform (v, framework));
		}

		// If the given framework is skipped in the specified platform.
		static bool IsFrameworkIncludedInPlatform (string platform, string framework)
		{
			// If the framework isn't in any of the registered frameworks for any platform, then treat it as included.
			if (!GetAllFrameworks ().Any (v => v.Any (x => string.Equals (x.Key, framework, StringComparison.OrdinalIgnoreCase))))
				return true;

			// If the framework is registered for the current platform, then it's included.
			var frameworks = Frameworks.GetFrameworks (ApplePlatformExtensions.Parse (platform), false);
			if (frameworks.Any (x => string.Equals (x.Key, framework, StringComparison.OrdinalIgnoreCase)))
				return true;

			// found only for platforms that aren't included in the current build
			return false;
		}

		static List<Frameworks> GetAllFrameworks ()
		{
			if (all_frameworks is null)
				all_frameworks = GetFrameworks (AllPlatforms);
			return all_frameworks;
		}

		static Frameworks GetFrameworks (string platform)
		{
			return Frameworks.GetFrameworks (ApplePlatformExtensions.Parse (platform), false);
		}

		static List<Frameworks> GetFrameworks (IEnumerable<string> platforms)
		{
			var rv = new List<Frameworks> ();
			foreach (var platform in platforms)
				rv.Add (GetFrameworks (platform));
			return rv;
		}

		static int GetPlatformCount (string framework)
		{
			if (frameworks is null)
				frameworks = GetFrameworks (Platforms);
			var rv = frameworks.Count (v => v.Any (fw => string.Equals (fw.Key, framework, StringComparison.OrdinalIgnoreCase)));
			if (rv == 0) // If we could find the framework in our list of framework, assume it's available in all platforms we're building for.
				return Platforms.Count;
			return rv;
		}
	}

	class RawInfo {
		public string Platform;
		public HashSet<string> Entries = new HashSet<string> ();
	}
}
