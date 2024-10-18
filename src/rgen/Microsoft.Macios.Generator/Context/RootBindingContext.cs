using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Context;

/// <summary>
/// Shared context through the entire code generation. This context allows to collect data that will be
/// later use to generate the Trampoline.g.cs file. once all classed are processed.
///
/// The class also provides a number or properties that will allow to determine the platform we are binding and access
/// to the current compilation.
/// </summary>
class RootBindingContext {
	readonly Dictionary<string, string> _libraries = new ();

	public PlatformName CurrentPlatform { get; set; }
	public Compilation Compilation { get; set; }
	public bool BindThirdPartyLibrary { get; set; }

	public RootBindingContext (Compilation compilation)
	{
		Compilation = compilation;
		CurrentPlatform = PlatformName.None;
		// use the reference assembly to determine what platform we are binding
		foreach (var referencedAssemblyName in compilation.ReferencedAssemblyNames) {
			switch (referencedAssemblyName.Name) {
			case "Microsoft.iOS":
				CurrentPlatform = PlatformName.iOS;
				break;
			case "Microsoft.MacCatalyst":
				CurrentPlatform = PlatformName.MacCatalyst;
				break;
			case "Microsoft.macOS":
				CurrentPlatform = PlatformName.MacOSX;
				break;
			case "Microsoft.tvOS":
				CurrentPlatform = PlatformName.TvOS;
				break;
			default:
				CurrentPlatform = PlatformName.None;
				break;
			}
		}
	}
}
