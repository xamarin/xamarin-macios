using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxToken = Microsoft.CodeAnalysis.SyntaxToken;

namespace Microsoft.Macios.Generator.Extensions;

public static class MethodDeclarationSyntaxExtensions {

	static readonly SyntaxToken EmptyToken = SyntaxFactory.Token (SyntaxKind.None);

	/// <summary>
	/// Returns the signature of a partial method without the attrlist and the ending semicolon.
	/// </summary>
	/// <param name="self">The martial method whose signature we want to retrieve</param>
	/// <param name="methodDeclaration">The declaration of the method.</param>
	/// <returns>The original signature of the method.</returns>
	public static bool TryGetDeclaration (this MethodDeclarationSyntax self,
		[NotNullWhen (true)] out string? methodDeclaration)
	{
		// create a new syntax (trees are red only) by remove the semicolon token from the partial
		// method declaration. Then, if we have any attribute list, remove them all. Return the full string
		// which is the signature of the method.
		methodDeclaration = null;
		if (!self.Modifiers.Any(SyntaxKind.PartialKeyword))
			return false;
		var newSyntax = self.ReplaceToken (self.SemicolonToken, EmptyToken);
		foreach (var attrList in newSyntax.AttributeLists) {
			newSyntax = newSyntax.RemoveNode (attrList, SyntaxRemoveOptions.KeepNoTrivia);
			if (newSyntax is null)
				return false;
		}
		methodDeclaration = newSyntax.ToFullString ().Trim();
		return true;
	}
}
