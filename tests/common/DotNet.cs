using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Utils;

using NUnit.Framework;

namespace Xamarin.Tests {
	public static class DotNet {

		static string default_version = "3.1.101";

		static Dictionary<string, Tuple<string, string>> versions = new Dictionary<string, Tuple<string, string>> {
			{ "3.1.101", new Tuple<string, string> ("https://download.visualstudio.microsoft.com/download/pr/515b77f4-4678-4b6f-a981-c48cf5607c5a/24b33941ba729ec421aa358fa452fd2f/dotnet-sdk-3.1.101-osx-x64.tar.gz", "b20865ce6c53b7f3321b4d18ccab52307183e4f1210215385870baaaa814d0ec57c8d41aca9a64b7edca4da1dec841f006e07f2af78dd68465a55ea9f25ca057") },
		};

		static Dictionary<string, string> downloaded = new Dictionary<string, string> ();

		static bool VerifyChecksum (string file, string checksum)
		{
			using (var provider = new System.Security.Cryptography.SHA512CryptoServiceProvider ()) {
				using (var fs = new FileStream (file, FileMode.Open, FileAccess.Read)) {
					var hashed = provider.ComputeHash (fs);
					var sb = new StringBuilder ();
					foreach (var b in hashed) {
						sb.Append ($"{b.ToString ("x2")}");
					}
					return sb.ToString () == checksum;
				}
			}

		}

		// Downloads the specified dotnet version, extracts it and returns the directory where it was extracted.
		static string DownloadPackage (string version = null)
		{
			return DownloadPackage (version, out var _);
		}

		static string DownloadPackage (string requested_dotnet_version, out string actual_dotnet_version, bool install_sdks = true)
		{
			if (string.IsNullOrEmpty (requested_dotnet_version))
				requested_dotnet_version = default_version;

			if (!versions.TryGetValue (requested_dotnet_version, out var info))
				throw new Exception ($"Don't know the download url for dotnet {requested_dotnet_version}");

			actual_dotnet_version = requested_dotnet_version;

			lock (downloaded) {
				if (downloaded.TryGetValue (requested_dotnet_version, out var local_path))
					return local_path;

				var url = info.Item1;
				var checksum = info.Item2; // shasum -a 512
				var fn = Path.GetFileName (info.Item1);
				var cache_dir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Caches", "xamarin-macios");
				var cached = Path.Combine (cache_dir, fn);
				var download = true;

				if (File.Exists (cached) && VerifyChecksum (cached, checksum))
					download = false;

				if (download) {
					using (var wc = new System.Net.WebClient ()) {
						wc.DownloadFile (url, cached);
						if (!VerifyChecksum (cached, checksum))
							throw new Exception ($"Download of url {url} into {cached} failed sha512 checksum {checksum}");
					}
				}

				local_path = Path.Combine (cache_dir, Path.GetFileNameWithoutExtension (Path.GetFileNameWithoutExtension (cached)));
				if (Directory.Exists (local_path))
					Directory.Delete (local_path, true);
				Directory.CreateDirectory (local_path);

				var rv = ExecutionHelper.Execute ("tar", new string [] { "xvf", cached }, timeout: TimeSpan.FromMinutes (1), workingDirectory: local_path, environmentVariables: null, stdout: null, stderr: null);
				if (rv != 0)
					throw new Exception ($"Extraction of {cached} failed, 'tar xvf {cached}' failed with exit code {rv}.");

				downloaded [requested_dotnet_version] = local_path;

				if (install_sdks)
					InstallSdks (actual_dotnet_version);

				return local_path;
			}
		}

		public static void InstallSdks (string dotnet_version)
		{
			var install_script = Path.Combine (Configuration.RootPath, "msbuild", "dotnet5", "install-into-dotnet.sh");
			var env = new Dictionary<string, string> () {
				{ "DOTNET_PATH", Path.Combine (DownloadPackage (dotnet_version, out var actual_version), "sdk", actual_version, "Sdks") },
				{ "IOS_TARGETDIR", Configuration.IOS_DESTDIR },
				{ "MAC_TARGETDIR", Configuration.MAC_DESTDIR },
			};
			var output = new StringBuilder ();
			var rv = ExecutionHelper.Execute (install_script, new string [] { }, timeout: TimeSpan.FromSeconds (10), environmentVariables: env, stdout: output, stderr: output);
			Console.WriteLine ($"{string.Join (" ", env.Select ((v) => $"{v.Key}={v.Value}"))} {install_script}");
			Console.WriteLine (output);
			if (rv != 0)
				throw new Exception ($"Failed to install into custom dotnet location, exit code: {rv}.");
		}

		public static string Executable {
			get {
				return Path.Combine (DownloadPackage (), "dotnet");
			}
		}

		public static int Execute (string verb, string project, Dictionary<string, string> properties, out StringBuilder output, string verbosity = "diagnostic", bool assert_success = true)
		{
			verb = verb.ToLowerInvariant ();

			switch (verb) {
			case "clean":
			case "build":
				var args = new List<string> ();
				args.Add (verb);
				args.Add (project);
				foreach (var prop in properties) {
					args.Add ($"/p:{prop.Key}={prop.Value}");
				}
				if (!string.IsNullOrEmpty (verbosity))
					args.Add ($"/verbosity:{verbosity}");
				var env = new Dictionary<string, string> ();
				env ["MSBuildSDKsPath"] = null;
				env ["MSBUILD_EXE_PATH"] = null;
				output = new StringBuilder ();
				var rv = ExecutionHelper.Execute (Executable, args, env, output, output, timeout: TimeSpan.FromMinutes (10));
				if (rv != 0) {
					Console.WriteLine ($"'dotnet {StringUtils.FormatArguments (args)}' failed with exit code {rv}.");
					Console.WriteLine (output);
				}
				if (assert_success)
					Assert.AreEqual (0, rv, $"Exit code: dotnet {StringUtils.FormatArguments (args)}");
				return rv;
			default:
				throw new NotImplementedException ($"Unknown dotnet action: '{verb}'");
			}
		}

		public static void CompareApps (string old_app, string new_app)
		{
			Console.WriteLine ($"Comparing: {old_app} {new_app}");

			var old_files = Directory.GetFiles (old_app, "*.*", SearchOption.AllDirectories).Select ((v) => v.Substring (old_app.Length + 1));
			var new_files = Directory.GetFiles (new_app, "*.*", SearchOption.AllDirectories).Select ((v) => v.Substring (new_app.Length + 1));

			var extra_old_files = old_files.Except (new_files);
			var extra_new_files = new_files.Except (old_files);

			Assert.That (extra_new_files, Is.Empty, "Extra dotnet files");
			Assert.That (extra_old_files, Is.Empty, "Missing dotnet files");

			// Print out a size comparison. A size difference does not fail the test, because some size differences are normal.
			var total_diff = 0L;
			foreach (var file in new_files) {
				var new_size = new FileInfo (Path.Combine ((string) new_app, file)).Length;
				var old_size = new FileInfo (Path.Combine ((string) old_app, file)).Length;
				if (new_size == old_size)
					continue;
				var diff = new_size - old_size;
				Console.WriteLine ($"{file}: {old_size} bytes -> {new_size} bytes. Diff: {diff}");
				total_diff += diff;
			}
			Console.WriteLine ($"Size comparison complete, total size diff: {total_diff}");

		}
	}
}
