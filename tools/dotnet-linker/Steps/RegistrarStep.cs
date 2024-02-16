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

			switch (app.Registrar) {
			case RegistrarMode.Dynamic:
				// Nothing to do here
				break;
			case RegistrarMode.PartialStatic:
				// The method name is created in StaticRegistrar.Specialize.
				var method = Configuration.Target.StaticRegistrar.GetInitializationMethodName (Configuration.PlatformAssembly);
				Configuration.RegistrationMethods.Add (method);
				Configuration.CompilerFlags.AddLinkWith (Configuration.PartialStaticRegistrarLibrary);
				break;
			case RegistrarMode.Static:
				Configuration.Target.StaticRegistrar.Register (Configuration.GetNonDeletedAssemblies (this));
				goto case RegistrarMode.ManagedStatic;
			case RegistrarMode.ManagedStatic:
				var dir = Configuration.CacheDirectory;
				var header = Path.Combine (dir, "registrar.h");
				var code = Path.Combine (dir, "registrar.mm");
				if (app.Registrar == RegistrarMode.ManagedStatic) {
					// Every api has been registered if we're using the managed registrar
					// (since we registered types before the trimmer did anything),
					// so we need to remove those that were later trimmed away by the trimmer.
					Configuration.Target.StaticRegistrar.FilterTrimmedApi (Annotations);
				}
				Configuration.Target.StaticRegistrar.Generate (header, code, out var initialization_method);

				var items = new List<MSBuildItem> ();
				foreach (var abi in Configuration.Abis) {
					items.Add (new MSBuildItem (
						code,
						new Dictionary<string, string> {
							{ "Arch", abi.AsArchString () },
							{ "Arguments", "-std=c++14" },
						}
					));
				}

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
