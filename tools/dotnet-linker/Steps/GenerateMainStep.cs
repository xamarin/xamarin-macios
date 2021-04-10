using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamarin.Linker;
using Xamarin.Utils;

namespace Xamarin {

	public class GenerateMainStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Generate Main";
		protected override int ErrorCode { get; } = 2320;

		protected override void TryEndProcess ()
		{
			base.TryEndProcess ();

			var registration_methods = new List<string> (Configuration.RegistrationMethods);
			var items = new List<MSBuildItem> ();

			var app = Configuration.Application;

			// We want this called before any other initialization methods.
			registration_methods.Insert (0, "xamarin_initialize_dotnet");

			foreach (var abi in Configuration.Abis) {

				var file = Path.Combine (Configuration.CacheDirectory, $"main.{abi.AsArchString ()}.mm");
				var contents = new StringBuilder ();

				contents.AppendLine ("#include <stdlib.h>");
				contents.AppendLine ("static void xamarin_initialize_dotnet ()");
				contents.AppendLine ("{");
				if (Configuration.InvariantGlobalization) {
					contents.AppendLine ("\tsetenv (\"DOTNET_SYSTEM_GLOBALIZATION_INVARIANT\", \"1\", 1);");
				}
				contents.AppendLine ("}");
				contents.AppendLine ();

				Configuration.Target.GenerateMain (contents, app.Platform, abi, file, registration_methods);

				var item = new MSBuildItem {
					Include = file,
					Metadata = {
						{ "Arch", abi.AsArchString () },
					},
				};
				if (app.EnableDebug)
					item.Metadata.Add ("Arguments", "-DDEBUG");
				items.Add (item);
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
