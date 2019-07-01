using System;
using System.IO;
using System.Reflection;

namespace Xamarin.Tests {
	public partial  class Configuration {
		public static string SampleRootDirectory {
			get {
				var rv = Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), "repositories");
				var nuget_conf = Path.Combine (rv, "NuGet.config");
				Directory.CreateDirectory (rv);
				if (!File.Exists (nuget_conf)) {
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
				var global_json = Path.Combine (rv, "global.json");
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
				return rv;
			}
		}
	}
}
