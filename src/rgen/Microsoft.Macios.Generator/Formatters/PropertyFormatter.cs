using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Formatters;

static class PropertyFormatter {
	/// <summary>
	/// Return the declaration represented by the given property.
	/// </summary>
	/// <param name="property">The property whose declaration we want to generate.</param>
	/// <returns>A compilation unit syntax node with the declaration of the property.</returns>
	public static CompilationUnitSyntax? ToDeclaration (this in Property? property)
	{
		if (property is null)
			return null;
		
		var compilationUnit = CompilationUnit ().WithMembers ( 
			SingletonList<MemberDeclarationSyntax> (
				PropertyDeclaration (
						type: IdentifierName (property.Value.Type), 
						identifier: Identifier (property.Value.Name))
					.WithModifiers (TokenList (property.Value.Modifiers)))).NormalizeWhitespace ();
		return compilationUnit;
	}
}
