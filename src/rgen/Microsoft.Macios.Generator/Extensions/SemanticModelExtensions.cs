// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.Extensions;

static partial class SemanticModelExtensions {

	/// <summary>
	/// Return an array with the namespaces in for the symbol.
	/// </summary>
	/// <param name="symbol">The symbol whose namespaces we want to retrieve.</param>
	/// <returns>An array with the namespaces that contain the symbol from bigger to smaller one.</returns>
	public static ImmutableArray<string> GetNamespaceArray (this ISymbol? symbol)
	{
		var bucket = ImmutableArray.CreateBuilder<string> ();
		var ns = symbol?.ContainingNamespace;
		while (ns is not null) {
			if (!string.IsNullOrWhiteSpace (ns.Name))
				// prepend the namespace so that we can read from top to bottom
				bucket.Insert (0, ns.Name);
			ns = ns.ContainingNamespace;
		}

		return bucket.ToImmutable ();
	}

	/// <summary>
	/// Retrieves all the data from a symbol needed for a binding/transformation.
	/// </summary>
	/// <param name="symbol">The symbol being queried.</param>
	/// <param name="name">Symbol name.</param>
	/// <param name="baseClass">Symbol base class.</param>
	/// <param name="interfaces">List of the interfaces implemented by the symbol.</param>
	/// <param name="namespaces">Collection with the namespaces of the symbol.</param>
	/// <param name="symbolAvailability">The symbols availability.</param>
	public static void GetSymbolData (ISymbol? symbol,
		out string name,
		out string? baseClass,
		out ImmutableArray<string> interfaces,
		out ImmutableArray<string> namespaces,
		out SymbolAvailability symbolAvailability)
	{
		name = symbol?.Name ?? string.Empty;
		baseClass = null;
		var interfacesBucket = ImmutableArray.CreateBuilder<string> ();
		if (symbol is INamedTypeSymbol namedTypeSymbol) {
			baseClass = namedTypeSymbol.BaseType?.ToDisplayString ().Trim ();
			foreach (var symbolInterface in namedTypeSymbol.Interfaces) {
				interfacesBucket.Add (symbolInterface.ToDisplayString ().Trim ());
			}
		}
		symbolAvailability = symbol?.GetSupportedPlatforms () ?? new SymbolAvailability ();
		interfaces = interfacesBucket.ToImmutable ();
		namespaces = symbol.GetNamespaceArray ();
	}

}
