using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Formatters;

static class MethodFormatter {

	static TypeSyntax GetIdentifierSyntax (this in MethodReturnType returnType)
	{
		if (returnType.IsArray) {
			// could be a params array or simply an array
			var arrayType = ArrayType (IdentifierName (returnType.Type))
				.WithRankSpecifiers (SingletonList (
					ArrayRankSpecifier (
						SingletonSeparatedList<ExpressionSyntax> (OmittedArraySizeExpression ()))));
			return returnType.IsNullable
				? NullableType (arrayType)
				: arrayType;
		}

		// dealing with a non-array type
		return returnType.IsNullable
			? NullableType (IdentifierName (returnType.Type))
			: IdentifierName (returnType.Type);
	}
	
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
