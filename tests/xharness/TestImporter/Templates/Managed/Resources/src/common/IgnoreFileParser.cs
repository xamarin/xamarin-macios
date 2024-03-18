using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System;

namespace BCLTests {

	/// <summary>
	/// Class that can parser a file/stream with the ignored tests and will
	/// return a list of the ignored tests.
	/// </summary>
	public static class IgnoreFileParser {

		static string ParseLine (string line)
		{
			// we have to make sure of several things, first, lets
			// remove any char after the first # which would mean
			// we have comments:
			var pos = line.IndexOf ('#');
			if (pos > -1) {
				line = line.Remove (pos);
			}
			line = line.Trim ();
			return line;
		}
		public static async Task<IEnumerable<string>> ParseStreamAsync (TextReader textReader)
		{
			var ignoredMethods = new List<string> ();
			string line;
			while ((line = await textReader.ReadLineAsync ()) is not null) {
				// we have to make sure of several things, first, lets
				// remove any char after the first # which would mean
				// we have comments:
				line = ParseLine (line);
				if (string.IsNullOrEmpty (line))
					continue;
				ignoredMethods.Add (line);
			}
			return ignoredMethods;
		}

		public static async Task<IEnumerable<string>> ParseAssemblyResourcesAsync (Assembly asm)
		{
			var ignoredTests = new List<string> ();
			// the project generator added the required resources,
			// we extract them, parse them and add the result
			foreach (var resourceName in asm.GetManifestResourceNames ()) {
				if (resourceName.EndsWith (".ignore", StringComparison.Ordinal)) {
					using (var stream = asm.GetManifestResourceStream (resourceName))
					using (var reader = new StreamReader (stream)) {
						var ignored = await ParseStreamAsync (reader);
						// we could have more than one file, lets add them
						ignoredTests.AddRange (ignored);
					}
				}
			}
			return ignoredTests;
		}

		public static async Task<IEnumerable<string>> ParseContentFilesAsync (string contentDir)
		{
			var ignoredTests = new List<string> ();
			foreach (var f in Directory.GetFiles (contentDir, "*.ignore")) {
				using (var reader = new StreamReader (f)) {
					var ignored = await ParseStreamAsync (reader);
					ignoredTests.AddRange (ignored);
				}
			}
			return ignoredTests;
		}

		public static async Task<IEnumerable<string>> ParseTraitsContentFileAsync (string contentDir, bool isXUnit)
		{
			var ignoredTraits = new List<string> ();
			var ignoreFile = Path.Combine (contentDir, isXUnit ? "xunit-excludes.txt" : "nunit-excludes.txt");
			using (var reader = new StreamReader (ignoreFile)) {
				string line;
				while ((line = await reader.ReadLineAsync ()) is not null) {
					if (string.IsNullOrEmpty (line))
						continue;
					ignoredTraits.Add (line);
				}
			}
			return ignoredTraits;
		}

		public static IEnumerable<string> ParseTraitsContentFile (string contentDir, bool isXUnit)
		{
			var ignoredTraits = new List<string> ();
			var ignoreFile = Path.Combine (contentDir, isXUnit ? "xunit-excludes.txt" : "nunit-excludes.txt");
			using (var reader = new StreamReader (ignoreFile)) {
				string line;
				while ((line = reader.ReadLine ()) is not null) {
					if (string.IsNullOrEmpty (line))
						continue;
					ignoredTraits.Add (line);
				}
			}
			return ignoredTraits;
		}

		public static IEnumerable<string> ParseContentFiles (string contentDir)
		{
			var ignoredTests = new List<string> ();
			foreach (var f in Directory.GetFiles (contentDir, "*.ignore")) {
				using (var reader = new StreamReader (f)) {
					string line;
					while ((line = reader.ReadLine ()) is not null) {

						line = ParseLine (line);
						if (string.IsNullOrEmpty (line))
							continue;
						ignoredTests.Add (line);
					}
				}
			}
			return ignoredTests;
		}
	}
}
