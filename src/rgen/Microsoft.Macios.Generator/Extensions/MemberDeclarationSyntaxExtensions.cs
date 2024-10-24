using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Extensions;

static class MemberDeclarationSyntaxExtensions {
	/// <summary>
	/// Get all the attribute changes that were made to the member.
	/// </summary>
	/// <param name="self">The member declaration syntax that triggered the changes.</param>
	/// <param name="semanticModel">The current semantic model.</param>
	/// <returns>All attributes that got changed.</returns>
	public static ImmutableArray<AttributeCodeChange> GetAttributeCodeChanges (this MemberDeclarationSyntax self,
		SemanticModel semanticModel)
	{
		var bucket = ImmutableArray.CreateBuilder<AttributeCodeChange> ();
		foreach (AttributeListSyntax attributeListSyntax in self.AttributeLists) {
			foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes) {
				if (semanticModel.GetSymbolInfo (attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
					continue; // if we can't get the symbol, ignore it
				var name = attributeSymbol.ContainingType.ToDisplayString ();
				var arguments = ImmutableArray.CreateBuilder<string> ();
				var argumentList = attributeSyntax.ArgumentList?.Arguments;
				if (argumentList is not null) {
					foreach (var argSyntax in argumentList) {
						// there are two types of argument nodes, those that are literal and those that
						// are a literal expression
						if (argSyntax.Expression is LiteralExpressionSyntax literalExpressionSyntax) {
							arguments.Add (literalExpressionSyntax.ToFullString ().Trim ()
								.Replace ("\"", string.Empty));
						}
					}
				}

				bucket.Add (new(name, arguments.ToImmutable ()));
			}
		}

		return bucket.ToImmutable ();
	}

	/// <summary>
	/// Return if the member has a specific attribute.
	/// </summary>
	/// <param name="self">The member declaration whose attributes we want to check.</param>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	/// <param name="attribute">The attribute name we want to find.</param>
	/// <returns>True if the attribute is present in the given member declaration.</returns>
	public static bool HasAttribute (this MemberDeclarationSyntax self, SemanticModel semanticModel,
		string attribute)
	{
		foreach (AttributeListSyntax attributeListSyntax in self.AttributeLists)
		foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes) {
			if (semanticModel.GetSymbolInfo (attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
				continue; // if we can't get the symbol, ignore it

			var currentName = attributeSymbol.ContainingType.ToDisplayString ();

			// Check the full name of the [Binding] attribute.
			if (currentName == attribute)
				return true;
		}

		return false;
	}
}
