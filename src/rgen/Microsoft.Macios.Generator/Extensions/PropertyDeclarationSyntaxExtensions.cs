using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Extensions;

public static class PropertyDeclarationSyntaxExtensions {

	public static bool TryGetDeclaration (this PropertyDeclarationSyntax self,
		[NotNullWhen (true)] out string? propertyDeclaration)
	{
		propertyDeclaration = null;
		if (!self.Modifiers.Any(SyntaxKind.PartialKeyword))
			return false;
		// we do not care about the accessors list
		var newSyntax = self.WithAccessorList (null);
		// remove attr list
		foreach (var attrList in newSyntax.AttributeLists) {
			newSyntax = newSyntax?.RemoveNode (attrList, SyntaxRemoveOptions.KeepNoTrivia);
			if (newSyntax is null)
				return false;
		}
		// care about the block but not about the getter/setter
		propertyDeclaration = newSyntax.ToFullString ().Trim();
		return true;
	}
}
