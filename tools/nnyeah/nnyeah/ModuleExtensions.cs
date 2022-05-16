using System;
using Mono.Cecil;
using System.Collections.Generic;

namespace Microsoft.MaciOS.Nnyeah {
	public static class ModuleExtensions {
		static Dictionary<string, PlatformName> moduleToPlatform = new Dictionary<string, PlatformName> {
			{ "Xamarin.iOS", PlatformName.iOS },
			{ "Xamarin.Mac", PlatformName.macOS },
			{ "Xamarin.TVOS", PlatformName.tvOS },
			{ "Xamarin.WatchOS", PlatformName.watchOS }
		};

		public static bool DependsOnXamarin (this ModuleDefinition module)
		{
			return module.XamarinPlatformName () != PlatformName.None;
		}

		public static PlatformName XamarinPlatformName (this ModuleDefinition module)
		{
			foreach (var possibleModule in module.AssemblyReferences) {
				if (moduleToPlatform.TryGetValue (possibleModule.Name, out var platformName))
					return platformName;
			}
			return PlatformName.None;
		}
	}
}
