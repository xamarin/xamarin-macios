using System;
using System.IO;
using System.Reflection;

namespace Xamarin.Tests {
	public partial class Configuration {
		static object lock_obj = new object ();
		static string sample_root_directory;
		public static string SampleRootDirectory {
			get {
				lock (lock_obj) {
					if (sample_root_directory is null) {
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
			// So create a Nuget.config that clears out any previous configuration and adds nuget.org as
			// single nuget source so that none of the sample tests pick up xamarin-macios' NuGet.config.
			File.WriteAllText (nuget_conf,
@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
	<config>
		<clear />
	</config>
	<packageSources>
		<clear />
		<add key=""nuget.org"" value=""https://api.nuget.org/v3/index.json"" />
	</packageSources>
	<disabledPackageSources>
		<clear />
	</disabledPackageSources>
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
					if (tested_hash is not null)
						return tested_hash;

					tested_hash = GetCurrentHash (Environment.CurrentDirectory);
					return tested_hash;
				}
			}
		}

		public static string GetCurrentHash (string directory)
		{
			return ProcessHelper.RunProcess ("git", "log -1 --pretty=%H", directory).Trim ();
		}

		public static string GetCurrentRemoteUrl (string directory)
		{
			return ProcessHelper.RunProcess ("git", "remote get-url origin", directory).Trim ();
		}

		static string mono_version;
		public static string MonoVersion {
			get {
				lock (lock_obj) {
					if (mono_version is not null)
						return mono_version;

					// We only care about the first line
					mono_version = ProcessHelper.RunProcess ("mono", "--version").Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries) [0].Trim ();
				}

				return mono_version;
			}
		}

		static string sw_version;
		public static string OSVersion {
			get {
				lock (lock_obj) {
					if (sw_version is not null)
						return sw_version;

					sw_version = ProcessHelper.RunProcess ("sw_vers").Replace ('\n', ';').Replace ((char) 9, ' ');
				}

				return sw_version;
			}
		}
	}
}
