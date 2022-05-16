using System;
using Mono.Cecil;
using System.Collections.Generic;

namespace Microsoft.MaciOS.Nnyeah {
	public static class ModuleExtensions {
		static Dictionary<string, PlatformName> xamarinModuleToPlatform = new Dictionary<string, PlatformName> {
			{ "Xamarin.iOS", PlatformName.iOS },
			{ "Xamarin.Mac", PlatformName.macOS },
			{ "Xamarin.TVOS", PlatformName.tvOS },
			{ "Xamarin.WatchOS", PlatformName.watchOS }
		};

		static Dictionary<string, PlatformName> microsoftModuleToPlatform = new Dictionary<string, PlatformName> {
			{ "Microsoft.iOS", PlatformName.iOS },
			{ "Microsoft.macOS", PlatformName.macOS },
			{ "Microsoft.tvOS", PlatformName.tvOS },
			{ "Microsoft.watchOS", PlatformName.watchOS }
		};

		public static bool DependsOnXamarin (this ModuleDefinition module)
		{
			return module.XamarinPlatformName () != PlatformName.None;
		}

		public static PlatformName XamarinPlatformName (this ModuleDefinition module)
		{
			return FindPlatformName (module, xamarinModuleToPlatform);
		}

		public static bool DependsOnMicrosoft (this ModuleDefinition module)
		{
			return module.MicrosoftPlatformName () != PlatformName.None;
		}

		public static PlatformName MicrosoftPlatformName (this ModuleDefinition module)
		{
			return FindPlatformName (module, microsoftModuleToPlatform);
		}

		static PlatformName FindPlatformName (ModuleDefinition module, Dictionary<string, PlatformName> knownModules)
		{
			foreach (var possibleModule in module.AssemblyReferences) {
				if (knownModules.TryGetValue (possibleModule.Name, out var platformName)) {
					return platformName;
				}
			}
			return PlatformName.None;
		}
	}
}
