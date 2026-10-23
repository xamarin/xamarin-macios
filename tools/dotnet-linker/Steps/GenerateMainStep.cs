using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Xamarin.Bundler;
using Xamarin.Linker;

#nullable enable

namespace Xamarin {

	public class GenerateMainStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Generate Main";
		protected override int ErrorCode { get; } = 2320;

		// These properties are set by the runtime (in xamarin_vm_initialize / xamarin_bridge_vm_initialize)
		// and passed to coreclr_initialize, so they must not also come from the runtimeconfig.json (otherwise
		// we'd pass duplicate properties to coreclr_initialize). Keep in sync with runtime/runtime.m and the
		// _RuntimeConfigReservedProperties item group in Xamarin.Shared.Sdk.targets.
		static readonly string [] reservedRuntimeConfigProperties = new [] {
			"APP_CONTEXT_BASE_DIRECTORY",
			"APP_PATHS",
			"PINVOKE_OVERRIDE",
			"TRUSTED_PLATFORM_ASSEMBLIES",
			"NATIVE_DLL_SEARCH_DIRECTORIES",
			"RUNTIME_IDENTIFIER",
			"SYSTEM_CORELIB_DIRECTORY",
			"STARTUP_HOOKS",
		};

		protected override void TryEndProcess ()
		{
			base.TryEndProcess ();

			var registration_methods = new List<string> (Configuration.RegistrationMethods);
			var items = new List<MSBuildItem> ();

			var app = Configuration.Application;

			// For CoreCLR we bake the runtimeconfig.json 'configProperties' directly into the app (as C arrays
			// in the generated main), instead of shipping the binary runtimeconfig format and decoding it at
			// startup. MonoVM and NativeAOT are unaffected (they don't use xamarin_bridge_compute_properties).
			if (app.XamarinRuntime == XamarinRuntime.CoreCLR)
				app.RuntimeConfigProperties = LoadRuntimeConfigProperties ();

			if (app.GenerateTrustedPlatformAssemblies) {
				app.TrustedPlatformAssemblies.AddRange (app.Assemblies.Select (v => v.FileName));
				app.TrustedPlatformAssemblies.AddRange (Configuration.AddedAssemblies.Select (v => Path.GetFileName (v.Path)));
			}

			// We want this called before any other initialization methods.
			registration_methods.Insert (0, "xamarin_initialize_dotnet");

			var abi = Configuration.Abi;
			var file = Path.Combine (Configuration.CacheDirectory, $"main.{abi.AsArchString ()}.mm");
			var contents = new StringBuilder ();

			contents.AppendLine ("#include <stdlib.h>");
			contents.AppendLine ();
			contents.AppendLine ("static void xamarin_initialize_dotnet ()");
			contents.AppendLine ("{");
			if (Configuration.Application.PackageManagedDebugSymbols && (Configuration.Application.UseInterpreter || Configuration.Application.XamarinRuntime == XamarinRuntime.CoreCLR))
				contents.AppendLine ($"\tsetenv (\"DOTNET_MODIFIABLE_ASSEMBLIES\", \"debug\", 1);");
			contents.AppendLine ("}");
			contents.AppendLine ();

			Configuration.Application.GenerateMain (contents, app.Platform, abi, file, registration_methods);

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
				extensionlib = "libtvextension-dotnet-coreclr.a";
			} else if (app.IsExtension) {
				extensionlib = "libextension-dotnet-coreclr.a";
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

		// Reads the (already merged with the *.runtimeconfig.dev.json) runtimeconfig.json file and extracts
		// the 'runtimeOptions.configProperties' as a string->string dictionary, matching the value conversion
		// done by the shared RuntimeConfigParserTask (from dotnet/runtime) that MonoVM uses.
		Dictionary<string, string>? LoadRuntimeConfigProperties ()
		{
			var path = Configuration.RuntimeConfigurationFilePath;
			if (string.IsNullOrEmpty (path) || !File.Exists (path))
				return null;

			var options = new JsonDocumentOptions {
				AllowTrailingCommas = true,
				CommentHandling = JsonCommentHandling.Skip,
			};

			using var document = JsonDocument.Parse (File.ReadAllText (path), options);
			if (!document.RootElement.TryGetProperty ("runtimeOptions", out var runtimeOptions))
				return null;
			if (!runtimeOptions.TryGetProperty ("configProperties", out var configProperties))
				return null;
			if (configProperties.ValueKind != JsonValueKind.Object)
				throw ErrorHelper.CreateError (2323, $"The 'runtimeOptions.configProperties' value in '{path}' must be a JSON object, but it's a {configProperties.ValueKind}.");

			var result = new Dictionary<string, string> ();
			foreach (var property in configProperties.EnumerateObject ()) {
				result [property.Name] = property.Value.ValueKind switch {
					JsonValueKind.String => property.Value.GetString () ?? "",
					JsonValueKind.True => "true",
					JsonValueKind.False => "false",
					JsonValueKind.Number => property.Value.GetRawText (),
					_ => throw ErrorHelper.CreateError (2321, $"Unsupported value for the runtime configuration property '{property.Name}' in '{path}'."),
				};
			}

			foreach (var reserved in reservedRuntimeConfigProperties) {
				if (result.ContainsKey (reserved))
					throw ErrorHelper.CreateError (2322, $"The runtime configuration property '{reserved}' can't be set by the user, it's reserved for the runtime.");
			}

			return result;
		}
	}
}
