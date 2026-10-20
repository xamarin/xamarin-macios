using System.Collections.Generic;
using System.IO;

using Xamarin.Bundler;
using Xamarin.Utils;

using Mono.Cecil;

#nullable enable

namespace Xamarin.Linker {
	public class RegistrarStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Registrar";
		protected override int ErrorCode { get; } = 2360;

		protected override void TryEndProcess ()
		{
			var app = Configuration.Application;

			// Always write this item file so that changing registrar modes doesn't leave stale generated registrar sources behind.
			Configuration.WriteOutputForMSBuild ("_RegistrarFile", new List<MSBuildItem> ());

			switch (app.Registrar) {
			case RegistrarMode.Dynamic:
				// Nothing to do here
				break;
			case RegistrarMode.PartialStatic:
				// The method name is created in StaticRegistrar.Specialize.
				var method = Configuration.Application.StaticRegistrar.GetInitializationMethodName (Configuration.PlatformAssembly);
				Configuration.RegistrationMethods.Add (method);
				Configuration.CompilerFlags.AddLinkWith (Configuration.PartialStaticRegistrarLibrary);
				break;
			case RegistrarMode.Static:
				Configuration.Application.StaticRegistrar.Register (Configuration.GetNonDeletedAssemblies (this));
				goto case RegistrarMode.ManagedStatic;
			case RegistrarMode.TrimmableStatic:
			case RegistrarMode.ManagedStatic:
				var dir = Configuration.CacheDirectory;
				var header = Path.Combine (dir, "registrar.h");
				var code = Path.Combine (dir, "registrar.mm");
				var separateAssemblies = app.Registrar == RegistrarMode.TrimmableStatic && app.XamarinRuntime == XamarinRuntime.CoreCLR;
				var architecture = Configuration.Abi & Abi.ArchMask;
				var assemblyObject = separateAssemblies && (architecture == Abi.ARM64 || architecture == Abi.x86_64)
					? Path.Combine (dir, "registrar-assemblies.o")
					: null;
				var assemblySource = separateAssemblies && assemblyObject is null
					? Path.Combine (dir, "registrar-assemblies.mm")
					: null;
#if !ASSEMBLY_PREPARER
				if (app.Registrar == RegistrarMode.ManagedStatic || app.Registrar == RegistrarMode.TrimmableStatic) {
					// Every api has been registered if we're using the managed registrar
					// (since we registered types before the trimmer did anything),
					// so we need to remove those that were later trimmed away by the trimmer.
					Configuration.Application.StaticRegistrar.FilterTrimmedApi (Annotations);
				}
#endif
				Configuration.Application.StaticRegistrar.Generate (header, code, out var initialization_method, assemblySource, assemblyObject);

				var items = new List<MSBuildItem> ();
				var abi = Configuration.Abi;
				items.Add (new MSBuildItem (
					code,
					new Dictionary<string, string> {
						{ "Arch", abi.AsArchString () },
						{ "Arguments", "-std=c++14" },
						// The generated code #includes the generated header, so the header
						// has to be next to the code when it's compiled. Declare it here, so
						// that it's copied to the Mac when building remotely from Windows.
						{ "AdditionalDependencies", header },
					}
				));
				if (assemblySource is not null) {
					items.Add (new MSBuildItem (
						assemblySource,
						new Dictionary<string, string> {
							{ "Arch", abi.AsArchString () },
							{ "Arguments", "-std=c++14" },
						}
					));
				}
				if (assemblyObject is not null)
					items.Add (new MSBuildItem (assemblyObject));

				Configuration.WriteOutputForMSBuild ("_RegistrarFile", items);
				Configuration.RegistrationMethods.Add (initialization_method);
				break;
			case RegistrarMode.Default: // We should have resolved 'Default' to an actual mode by now.
			default:
				Report (ErrorHelper.CreateError (99, Errors.MX0099, $"Invalid registrar mode: {app.Registrar}"));
				break;
			}
		}
	}
}
