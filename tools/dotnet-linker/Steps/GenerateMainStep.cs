using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamarin.Linker;

namespace Xamarin {

	public class GenerateMainStep : ConfigurationAwareStep {
		protected override void EndProcess ()
		{
			base.EndProcess ();

			var items = new List<MSBuildItem> ();

			foreach (var abi in Configuration.Abis) {

				var file = Path.Combine (Configuration.CacheDirectory, $"main.{abi.AsArchString ()}.m");
				var contents = new StringWriter ();

				contents.WriteLine ("#include \"xamarin/xamarin.h\"");
				contents.WriteLine ();
				contents.WriteLine ("void xamarin_setup_impl ()");
				contents.WriteLine ("{");
				contents.WriteLine ("\tsetenv (\"DOTNET_SYSTEM_GLOBALIZATION_INVARIANT\", \"1\", 1); // https://github.com/xamarin/xamarin-macios/issues/8906");
				contents.WriteLine ("\txamarin_executable_name = \"{0}\";", Configuration.AssemblyName);
				contents.WriteLine ("}");
				contents.WriteLine ();
				contents.WriteLine ("void xamarin_initialize_callbacks () __attribute__ ((constructor));");
				contents.WriteLine ("void xamarin_initialize_callbacks ()");
				contents.WriteLine ("{");
				contents.WriteLine ("\txamarin_setup = xamarin_setup_impl;");
				contents.WriteLine ("}");
				contents.WriteLine ();
				contents.WriteLine ("int");
				contents.WriteLine ("main (int argc, char** argv)");
				contents.WriteLine ("{");
				contents.WriteLine ("\t@autoreleasepool { return xamarin_main (argc, argv, XamarinLaunchModeApp); }");
				contents.WriteLine ("}");

				File.WriteAllText (file, contents.ToString ());

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
