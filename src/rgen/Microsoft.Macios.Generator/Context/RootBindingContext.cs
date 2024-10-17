using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
	public PlatformName CurrentPlatform { get; set; }
	public Compilation Compilation { get; set; }

	public Dictionary<string, string> Libraries { get; } = new ();

	public RootBindingContext (Compilation compilation)
	{
		Compilation = compilation;
		CurrentPlatform = compilation.GetCurrentPlatform ();
	}

	// TODO: clean code coming from the old generator
	public bool TryComputeLibraryName (string? attributeLibraryName, string typeNamespace,
		[NotNullWhen (true)] out string? libraryName,
		out string? libraryPath)
	{
		libraryPath = null;

		if (!string.IsNullOrEmpty (attributeLibraryName)) {
			// Remapped
			libraryName = attributeLibraryName;
			if (libraryName [0] == '+') {
				switch (libraryName) {
				case "+CoreImage":
					CurrentPlatform.TryGetCoreImageMap (out libraryName);
					break;
				case "+CoreServices":
					CurrentPlatform.TryGetCoreServicesMap (out libraryName);
					break;
				case "+PDFKit":
					libraryName = "PdfKit";
					CurrentPlatform.TryGetPDFKitMap (out libraryPath);
					break;
				}
			} else {
				// we get something in LibraryName from FieldAttribute so we assume
				// it is a path to a library, so we save the path and change library name
				// to a valid identifier if needed
				libraryPath = libraryName;
				// without extension makes more sense, but we can't change it since it breaks compat
				libraryName = Path.GetFileNameWithoutExtension (libraryName);

				if (libraryName.Contains ('.'))
					libraryName = libraryName.Replace (".", string.Empty);
			}
		} else {
			libraryName = typeNamespace;
		}

		if (libraryName is not null && !Libraries.ContainsKey (libraryName))
			Libraries.Add (libraryName, libraryPath!);

		return libraryName is not null;
	}

	public bool IsSystemLibrary (string name)
	{
		// use the semantic model to get the ObjcRuntime.Constants type and see if we do have a value for the library
		var symbol = Compilation.GetTypeByMetadataName ("ObjCRuntime.Constants");
		// this should not happen, we should have the Constants type
		if (symbol is null)
			return false;
		return symbol.GetMembers ().OfType<IFieldSymbol> ()
			.Select (f => f.Name)
			.Any (s => s.Equals ($"{name}Library", StringComparison.Ordinal));
	}
}
