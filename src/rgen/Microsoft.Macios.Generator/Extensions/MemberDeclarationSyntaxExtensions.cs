using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Extensions;

static class MemberDeclarationSyntaxExtensions {
	public static bool HasAttribute (this MemberDeclarationSyntax self, GeneratorSyntaxContext context,
		string attribute)
	{
		foreach (AttributeListSyntax attributeListSyntax in self.AttributeLists)
		foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes) {
			if (context.SemanticModel.GetSymbolInfo (attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
				continue; // if we can't get the symbol, ignore it

			var currentName = attributeSymbol.ContainingType.ToDisplayString ();

			// Check the full name of the [Binding] attribute.
			if (currentName == attribute)
				return true;
		}

		return false;
	}
}
