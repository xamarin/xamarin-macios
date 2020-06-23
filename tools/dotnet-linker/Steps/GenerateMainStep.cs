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
				var contents = $@"
#include ""xamarin/xamarin.h""

void xamarin_setup_impl ()
{{
	setenv (""DOTNET_SYSTEM_GLOBALIZATION_INVARIANT"", ""1"", 1); // https://github.com/xamarin/xamarin-macios/issues/8906
	xamarin_executable_name = ""{Configuration.AssemblyName}"";
}}

void xamarin_initialize_callbacks () __attribute__ ((constructor));
void xamarin_initialize_callbacks ()
{{
	xamarin_setup = xamarin_setup_impl;
}}

int
main (int argc, char** argv)
{{
	@autoreleasepool {{ return xamarin_main (argc, argv, XamarinLaunchModeApp); }}
}}
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
