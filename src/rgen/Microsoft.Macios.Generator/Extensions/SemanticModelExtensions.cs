using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.Extensions;

static class SemanticModelExtensions {

	/// <summary>
	/// Returns the name and namespace of the symbol that has been declared in the passed base type declaration
	/// syntax node.
	/// </summary>
	/// <param name="self">The current semantic model.</param>
	/// <param name="declaration">The named type declaration syntaxt.</param>
	/// <returns>A tuple containing the name and namespace of the type. If they could not be calculated, they will
	/// be set to be string.Empty.</returns>
	public static (string Name, ImmutableArray<string> Namespace, SymbolAvailability SymbolAvailability) GetSymbolData (this SemanticModel self,
		BaseTypeDeclarationSyntax declaration)
	{
		var symbol = self.GetDeclaredSymbol (declaration);
		var name = symbol?.Name ?? string.Empty;
		var bucket = ImmutableArray.CreateBuilder<string> ();
		var ns = symbol?.ContainingNamespace;
		while (ns is not null) {
			if (!string.IsNullOrWhiteSpace (ns.Name))
				// prepend the namespace so that we can read from top to bottom
				bucket.Insert (0, ns.Name);
			ns = ns.ContainingNamespace;
		}
		var availability = symbol?.GetSupportedPlatforms () ?? new SymbolAvailability ();
		return (name, bucket.ToImmutableArray (), availability);
	}
}
