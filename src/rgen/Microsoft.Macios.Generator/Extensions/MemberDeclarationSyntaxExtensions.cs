using System.Collections.Generic;
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
		SemanticModel semanticModel) => AttributeCodeChange.From (self.AttributeLists, semanticModel);

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

	/// <summary>
	/// Return if the symgbol has at least one of the provided attributes.
	/// </summary>
	/// <param name="self">The member declaration whose attributes we want to check.</param>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	/// <param name="attributeNames">List with the attributes we are testing against.</param>
	/// <returns>True if the member has been tagged with at least one of the given attributes. False otherwise.</returns>
	public static bool HasAtLeastOneAttribute (this MemberDeclarationSyntax self, SemanticModel semanticModel,
		params string [] attributeNames)
	{
		// create a hash set for faster look up
		var attributeList = new HashSet<string> (attributeNames);
		foreach (AttributeListSyntax attributeListSyntax in self.AttributeLists)
			foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes) {
				if (semanticModel.GetSymbolInfo (attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
					continue; // if we can't get the symbol, ignore it

				var currentName = attributeSymbol.ContainingType.ToDisplayString ();

				if (attributeList.Contains (currentName))
					return true;
			}

		return false;
	}
}
