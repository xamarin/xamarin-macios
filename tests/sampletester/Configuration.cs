using System;
using System.IO;
using System.Reflection;

namespace Xamarin.Tests {
	public partial  class Configuration {
		static object lock_obj = new object ();
		static string sample_root_directory;
		public static string SampleRootDirectory {
			get {
				lock (lock_obj) {
					if (sample_root_directory == null) {
						sample_root_directory = Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), "repositories");
						Directory.CreateDirectory (sample_root_directory);
						CreateNugetConfig (sample_root_directory);
						CreateGlobalConfig (sample_root_directory);
					}
				}
				return sample_root_directory;
			}
		}

		static void CreateNugetConfig (string root)
		{
			var nuget_conf = Path.Combine (root, "NuGet.config");
			// We're cloning into a subdirectory of xamarin-macios, which already has a NuGet.config
			// So create a Nuget.config that clears out any previous configuration, so that none of the
			// sample tests pick up xamarin-macios' NuGet.config.
			File.WriteAllText (nuget_conf,
@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
	<config>
		<clear />
	</config>
</configuration>
");
		}

		static void CreateGlobalConfig (string root)
		{
			var global_json = Path.Combine (root, "global.json");
			// Workaround for https://github.com/NuGet/Home/issues/7956
			// See also:
			// * https://github.com/mono/mono/issues/13537
			// * https://github.com/xamarin/maccore/issues/1811
			// The version number here must match the version in tools/devops/build-samples.csx
			File.WriteAllText (global_json,
				@"{
""sdk"": {
	""version"": ""2.2.204""
}
			}
");
		}

		static string tested_hash;
		public static string TestedHash {
			get {
				lock (lock_obj) {
					if (tested_hash != null)
						return tested_hash;

					tested_hash = GetCurrentHash (Environment.CurrentDirectory);
					return tested_hash;
				}
			}
		}

		public static string GetCurrentHash (string directory)
		{
			using (var p = System.Diagnostics.Process.Start ("git", "log -1 --pretty=%H")) {
				p.StartInfo.WorkingDirectory = directory;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;
				p.Start ();
				var hash = p.StandardOutput.ReadToEnd ().Trim ();
				p.WaitForExit ();
				return hash;
			}
		}

		public static string GetCurrentRemoteUrl (string directory)
		{
			using (var p = System.Diagnostics.Process.Start ("git", "remote get-url origin")) {
				p.StartInfo.WorkingDirectory = directory;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;
				p.Start ();
				var url = p.StandardOutput.ReadToEnd ().Trim ();
				p.WaitForExit ();
				return url;
			}
		}

		static string mono_version;
		public static string MonoVersion {
			get {
				lock (lock_obj) {
					if (mono_version != null)
						return mono_version;

					using (var p = System.Diagnostics.Process.Start ("mono", "--version")) {
						p.StartInfo.RedirectStandardOutput = true;
						p.StartInfo.UseShellExecute = false;
						p.Start ();
						mono_version = p.StandardOutput.ReadLine ().Trim (); // We only care about the first line
						p.StandardOutput.ReadToEnd ();
						p.WaitForExit ();
					}
				}

				return mono_version;
			}
		}

		static string sw_version;
		public static string OSVersion {
			get {
				lock (lock_obj) {
					if (sw_version != null)
						return sw_version;

					using (var p = System.Diagnostics.Process.Start ("sw_vers")) {
						p.StartInfo.RedirectStandardOutput = true;
						p.StartInfo.UseShellExecute = false;
						p.Start ();
						sw_version = p.StandardOutput.ReadToEnd ().Replace ('\n', ';').Replace ((char) 9, ' ');
						p.WaitForExit ();
					}
				}

				return sw_version;
			}
		}
	}
}
