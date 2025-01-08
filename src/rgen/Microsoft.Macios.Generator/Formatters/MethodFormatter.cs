using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Formatters;

static class MethodFormatter {

	public static CompilationUnitSyntax? ToDeclaration (this in Method? method)
	{
		if (method is null)
			return null;
		var compilationUnit = CompilationUnit ()
			.WithMembers (
				SingletonList<MemberDeclarationSyntax> (
					MethodDeclaration (
							returnType: method.Value.ReturnType.GetIdentifierSyntax (),
							identifier: Identifier (method.Value.Name)
								.WithLeadingTrivia (Space)
								.WithTrailingTrivia (Space)) // adding the spaces manually to follow the mono style
						.WithModifiers (TokenList (method.Value.Modifiers))
						.WithParameterList (method.Value.Parameters.GetParameterList ())));
		return compilationUnit;
	}
}
