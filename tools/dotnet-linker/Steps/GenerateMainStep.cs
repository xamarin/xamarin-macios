using System.Collections.Generic;
using System.IO;

using Xamarin.Linker;

namespace Xamarin {

	public class GenerateMainStep : ConfigurationAwareStep {
		protected override void EndProcess ()
		{
			base.EndProcess ();

			var registration_methods = Configuration.RegistrationMethods;
			var items = new List<MSBuildItem> ();

			var app = Configuration.Application;
			foreach (var abi in Configuration.Abis) {

				var file = Path.Combine (Configuration.CacheDirectory, $"main.{abi.AsArchString ()}.mm");
				var contents = new StringWriter ();

				contents.WriteLine ("#include \"xamarin/xamarin.h\"");
				contents.WriteLine ();
				if (registration_methods != null) {
					foreach (var method in registration_methods) {
						contents.Write ("extern \"C\" void ");
						contents.Write (method);
						contents.WriteLine (" ();");
					}
				}
				contents.WriteLine ("void xamarin_setup_impl ()");
				contents.WriteLine ("{");
				contents.WriteLine ("\tsetenv (\"DOTNET_SYSTEM_GLOBALIZATION_INVARIANT\", \"1\", 1); // https://github.com/xamarin/xamarin-macios/issues/8906");
				contents.WriteLine ("\txamarin_executable_name = \"{0}\";", Configuration.AssemblyName);
				if (registration_methods != null) {
					for (int i = 0; i < registration_methods.Count; i++) {
						contents.Write ("\t");
						contents.Write (registration_methods [i]);
						contents.WriteLine ("();");
					}
				}
				if (!app.IsDefaultMarshalManagedExceptionMode)
					contents.WriteLine ("\txamarin_marshal_managed_exception_mode = MarshalManagedExceptionMode{0};", app.MarshalManagedExceptions);
				contents.WriteLine ("\txamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionMode{0};", app.MarshalObjectiveCExceptions);
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

			var linkWith = new List<MSBuildItem> ();
			if (Configuration.CompilerFlags.LinkWithLibraries != null) {
				foreach (var lib in Configuration.CompilerFlags.LinkWithLibraries) {
					linkWith.Add (new MSBuildItem {
						Include = lib,
					});
				}
			}
			if (Configuration.CompilerFlags.ForceLoadLibraries != null) {
				foreach (var lib in Configuration.CompilerFlags.ForceLoadLibraries) {
					linkWith.Add (new MSBuildItem {
						Include = lib,
						Metadata = {
							{ "ForceLoad", "true" },
						},
					});
				}
			}

			Configuration.WriteOutputForMSBuild ("_MainLinkWith", linkWith);
		}
	}
}
