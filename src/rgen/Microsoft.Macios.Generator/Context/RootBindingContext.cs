using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;

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
		CurrentPlatform = compilation.GetCurrentPlatform ();
	}
}
