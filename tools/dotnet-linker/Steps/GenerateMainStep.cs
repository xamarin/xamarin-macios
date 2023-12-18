using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamarin.Linker;

#nullable enable

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
				contents.AppendLine ();
				contents.AppendLine ("static void xamarin_initialize_dotnet ()");
				contents.AppendLine ("{");
				if (Configuration.Application.PackageManagedDebugSymbols && Configuration.Application.UseInterpreter)
					contents.AppendLine ($"\tsetenv (\"DOTNET_MODIFIABLE_ASSEMBLIES\", \"debug\", 1);");
				contents.AppendLine ("}");
				contents.AppendLine ();

				Configuration.Target.GenerateMain (contents, app.Platform, abi, file, registration_methods);

				var item = new MSBuildItem (
					file,
					new Dictionary<string, string> {
						{ "Arch", abi.AsArchString () },
					}
				);
				if (app.EnableDebug)
					item.Metadata.Add ("Arguments", "-DDEBUG");
				items.Add (item);

				if (app.RequiresPInvokeWrappers) {
					var state = Configuration.PInvokeWrapperGenerationState!;
					item = new MSBuildItem (
						state.SourcePath,
						new Dictionary<string, string> {
							{ "Arch", abi.AsArchString () },
						}
					);
					if (app.EnableDebug)
						item.Metadata.Add ("Arguments", "-DDEBUG");
					items.Add (item);
				}
			}

			Configuration.WriteOutputForMSBuild ("_MainFile", items);

			var linkWith = new List<MSBuildItem> ();
			if (Configuration.CompilerFlags.LinkWithLibraries is not null) {
				foreach (var lib in Configuration.CompilerFlags.LinkWithLibraries) {
					linkWith.Add (new MSBuildItem (lib));
				}
			}
			if (Configuration.CompilerFlags.ForceLoadLibraries is not null) {
				foreach (var lib in Configuration.CompilerFlags.ForceLoadLibraries) {
					linkWith.Add (new MSBuildItem (
						lib,
						new Dictionary<string, string> {
							{ "ForceLoad", "true" },
						}
					));
				}
			}

			string? extensionlib = null;
			if (app.IsTVExtension) {
				extensionlib = "libtvextension-dotnet.a";
			} else if (app.IsExtension) {
				if (app.XamarinRuntime == Bundler.XamarinRuntime.CoreCLR) {
					extensionlib = "libextension-dotnet-coreclr.a";
				} else {
					extensionlib = "libextension-dotnet.a";
				}
			}
			if (!string.IsNullOrEmpty (extensionlib)) {
				linkWith.Add (new MSBuildItem (
					Path.Combine (Configuration.XamarinNativeLibraryDirectory, extensionlib),
					new Dictionary<string, string> {
						{ "ForceLoad", "true" },
					}
				));
			}

			Configuration.WriteOutputForMSBuild ("_MainLinkWith", linkWith);
		}
	}
}
