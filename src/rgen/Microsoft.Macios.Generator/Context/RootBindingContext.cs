using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

	public Dictionary<string, string> Libraries { get; } = new();

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

		if (libraryName.IsValidIdentifier () && !Libraries.ContainsKey (libraryName))
			Libraries.Add (libraryName, libraryPath!);

		// return that the library name is NOT null AND that it is a valid C# identifier by using the SyntaxFacts.
		return libraryName.IsValidIdentifier ();
	}

	/// <summary>
	/// Returns if the given variable name is the one that would be used for a given library.
	/// </summary>
	/// <param name="libraryName">The library against which we are comparing.</param>
	/// <param name="variableName">The variable name.</param>
	/// <remarks>The method does the comparison without creating a new string.</remarks>
	/// <returns>True if the variable name matches the pattern {libraryName}Library.</returns>
	static bool IsLibraryName (string libraryName, string variableName) =>
		// the length of the variable names has to be equal to the current lib name legth + 7 which is the length of
		// the word Library
		variableName.Length == libraryName.Length + 7
		// has to start with the name of the lib
		&& variableName.StartsWith (libraryName, StringComparison.Ordinal)
		// has to end with the word library
		&& variableName.EndsWith ("Library", StringComparison.Ordinal);

	public bool IsSystemLibrary (string name)
	{
		// use the semantic model to get the ObjcRuntime.Constants type and see if we do have a value for the library
		var symbol = Compilation.GetTypeByMetadataName ("ObjCRuntime.Constants");
		// this should not happen, we should have the Constants type
		if (symbol is null)
			return false;
		return symbol.GetMembers ().OfType<IFieldSymbol> ()
			.Select (f => f.Name)
			.Any (s => IsLibraryName (s, name));
	}
}
