using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace governance {
	struct ModuleInfo {
		public string Name;
		public string Hash;
		public string Url;

		public ModuleInfo (string name, string hash, string url)
		{
			Name = name;
			Hash = hash;
			Url = url;
		}
	}

	public static class StringExtensions {
		public static IEnumerable<string> SplitLines (this string item)
		{
			return item.Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static IEnumerable<string> Split (this string item, char character)
		{
			return item.Split (new char [] { character }, 1, StringSplitOptions.None);
		}
	}

	class EntryPoint {
		static IEnumerable<ModuleInfo> GetModules ()
		{
			string currentDirectory = Environment.CurrentDirectory;

			Xamarin.Bundler.Driver.RunCommand ("/usr/bin/git", new string [] { "submodule", "status" }, new string [] { }, out StringBuilder output, true, 0);
			foreach (var line in output.ToString ().SplitLines ()) {
				var bits = line.ToString ().Split (' ');
				if (bits.Length >= 3) {
					var hash = bits [1];
					var path = bits [2];
					var name = path.Split ('/').Last ();

					Directory.SetCurrentDirectory (path);
					Xamarin.Bundler.Driver.RunCommand ("/usr/bin/git", new string [] { "remote", "-v" }, new string [] { }, out StringBuilder remoteOutput, true, 0);
					var urlBits = remoteOutput.ToString ().SplitLines ().First ().Split (' ');
					// Yes, git uses spaces and tabs in this line
					var url = urlBits [0].Split ('\t') [1];
					Directory.SetCurrentDirectory (currentDirectory);

					yield return new ModuleInfo (name, hash, url);
				}
			}

			Xamarin.Bundler.Driver.RunCommand ("/usr/bin/git", new string [] { "show", "HEAD:mk/mono.mk" }, new string [] { }, out StringBuilder monoOutput, true, 0);
			var lines = monoOutput.ToString ().SplitLines ().ToArray ();
			var version = lines [0].Split (' ').Last ();
			var monoUrl = lines [3].Split (' ').Last ();
			yield return new ModuleInfo ("mono", version, monoUrl);
		}

		static HashSet<string> KnownDevDependencies = new HashSet<string> () {
			"Touch.Unit",
			"guiunit",
		};

		static bool IsDevDependency (string name) => KnownDevDependencies.Contains (name);

		static void Main (string [] args)
		{
			// Yes, I am hand outputing json here. Any questions?
			using (var file = new StreamWriter ("cgmanifest.json")) {
				file.WriteLine ("{");
				file.WriteLine ("   \"Registrations\": [");
				var modules = GetModules ().ToList ();
				foreach (var module in modules) {
					file.WriteLine ("       {");
					file.WriteLine ("           \"Component\": {");
					file.WriteLine ("               \"Type\": \"git\",");
					file.WriteLine ("               \"Git\": {");
					file.WriteLine ("                   \"RepositoryUrl\": \"" + module.Url + "\",");
					file.WriteLine ("                   \"CommitHash\": \"" + module.Hash + "\"");
					file.WriteLine ("               }");
					file.WriteLine ("           },");
					file.WriteLine ("           \"DevelopmentDependency\": " + (IsDevDependency (module.Name) ? "true" : "false"));
					file.WriteLine ("       }" + (modules.IndexOf (module) == modules.Count - 1 ? "" : ","));
				}
				file.WriteLine ("   ]");
				file.WriteLine ("}");
			}
		}
	}
}
