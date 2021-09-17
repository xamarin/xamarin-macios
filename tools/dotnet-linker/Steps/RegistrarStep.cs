using System.Collections.Generic;
using System.IO;

using Xamarin.Bundler;
using Xamarin.Utils;

using Mono.Cecil;

namespace Xamarin.Linker {
	public class RegistrarStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Registrar";
		protected override int ErrorCode { get; } = 2360;

		protected override void TryEndProcess ()
		{
			var app = Configuration.Application;

			app.SelectRegistrar ();

			switch (app.Registrar) {
			case RegistrarMode.Dynamic:
				// Nothing to do here
				break;
			case RegistrarMode.PartialStatic:
				string method = null;
				switch (app.Platform) {
				case ApplePlatform.iOS:
					method = "xamarin_create_classes_Xamarin_iOS";
					break;
				case ApplePlatform.WatchOS:
					method = "xamarin_create_classes_Xamarin_WatchOS";
					break;
				case ApplePlatform.TVOS:
					method = "xamarin_create_classes_Xamarin_TVOS";
					break;
				case ApplePlatform.MacOSX:
					method = "xamarin_create_classes_Xamarin_Mac";
					break;
				case ApplePlatform.MacCatalyst:
					method = "xamarin_create_classes_Xamarin_MacCatalyst";
					break;
				default:
					Report (ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName));
					break;
				}
				Configuration.RegistrationMethods.Add (method);
				Configuration.CompilerFlags.AddLinkWith (Configuration.PartialStaticRegistrarLibrary);
				break;
			case RegistrarMode.Static:
				var dir = Configuration.CacheDirectory;
				var header = Path.Combine (dir, "registrar.h");
				var code = Path.Combine (dir, "registrar.mm");
				var bundled_assemblies = new List<AssemblyDefinition> ();
				foreach (var assembly in Configuration.Assemblies) {
					if (Annotations.GetAction (assembly) != Mono.Linker.AssemblyAction.Delete)
						bundled_assemblies.Add (assembly);
				}
				Configuration.Target.StaticRegistrar.Generate (bundled_assemblies, header, code);

				var items = new List<MSBuildItem> ();
				foreach (var abi in Configuration.Abis) {
					items.Add (new MSBuildItem {
						Include = code,
						Metadata = {
							{ "Arch", abi.AsArchString () },
							{ "Arguments", "-std=c++14" },
						},
					});
				}

				Configuration.WriteOutputForMSBuild ("_RegistrarFile", items);
				Configuration.RegistrationMethods.Add ("xamarin_create_classes");
				break;
			case RegistrarMode.Default: // We should have resolved 'Default' to an actual mode by now.
			default:
				Report (ErrorHelper.CreateError (99, Errors.MX0099, $"Invalid registrar mode: {app.Registrar}"));
				break;
			}
		}
	}
}
