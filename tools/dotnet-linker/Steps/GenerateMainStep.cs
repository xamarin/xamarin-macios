using System.Collections.Generic;
using System.IO;

using Xamarin.Linker;

namespace Xamarin {

	public class GenerateMainStep : ConfigurationAwareStep {
		protected override void EndProcess ()
		{
			base.EndProcess ();

			var items = new List<MSBuildItem> ();

			foreach (var abi in Configuration.Abis) {
				var file = Path.Combine (Configuration.CacheDirectory, $"main.{abi.AsArchString ()}.m");
				var contents = @"
int
main (int argc, char** argv)
{
	return 0;
}
";
				File.WriteAllText (file, contents);

				items.Add (new MSBuildItem {
					Include = file,
					Metadata = {
						{ "Arch", abi.AsArchString () },
					},
				});
			}

			Configuration.WriteOutputForMSBuild ("_MainFile", items);
		}
	}
}
