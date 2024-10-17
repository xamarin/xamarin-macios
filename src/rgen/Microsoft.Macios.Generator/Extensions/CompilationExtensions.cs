using System;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

public static class CompilationExtensions {

	public static PlatformName GetCurrentPlatform (this Compilation self)
	{
		// use the reference assembly to determine what platform we are binding
		foreach (var referenceAssembly in self.ReferencedAssemblyNames) {
			switch (referenceAssembly.Name) {
			case "Microsoft.iOS":
				return PlatformName.iOS;
			case "Microsoft.MacCatalyst":
				return PlatformName.MacCatalyst;
			case "Microsoft.macOS":
				return PlatformName.MacOSX;
			case "Microsoft.tvOS":
				return PlatformName.TvOS;
			}
		}

		return PlatformName.None;
	}
}
