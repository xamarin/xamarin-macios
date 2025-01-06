using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Formatters;

static class ConstructorFormatter {

	public static CompilationUnitSyntax? ToDeclaration (this in Constructor? constructor)
	{
		if (constructor is null)
			return null;

		var compilationUnit = CompilationUnit ().WithMembers (
			SingletonList<MemberDeclarationSyntax> (
				ConstructorDeclaration (Identifier (constructor.Value.Type)
						.WithTrailingTrivia (Space) // add spaces manually to use the mono style
					)
					.WithModifiers (TokenList (constructor.Value.Modifiers))
					.WithParameterList (constructor.Value.Parameters.GetParameterList ())
			));
		return compilationUnit;
	}
}
