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
				return rv;
			}
		}
	}
}
