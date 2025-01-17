// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Emitters;

static partial class BindingSyntaxFactory {
	/// <summary>
	/// Returns an argument syntax for the provided kind and literal expression.
	/// </summary>
	/// <param name="kind">The kind of the literal value argument.</param>
	/// <param name="literal">The value of the argument.</param>
	/// <returns>A literal argument with the provided value.</returns>
	static ArgumentSyntax GetLiteralExpressionArgument (SyntaxKind kind, string literal)
	{
		return Argument (LiteralExpression (kind, Literal (literal)));
	}

	static CompilationUnitSyntax StaticInvocationExpression (string staticClassName, string methodName,
		SyntaxNodeOrToken [] argumentList)
	{
		var invocation = InvocationExpression (
			MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName (staticClassName),
				IdentifierName (methodName).WithTrailingTrivia (Space)
			)
		).WithArgumentList (ArgumentList (SeparatedList<ArgumentSyntax> (argumentList)).NormalizeWhitespace ());

		var compilationUnit = CompilationUnit ().WithMembers (
			SingletonList<MemberDeclarationSyntax> (
				GlobalStatement (
					ExpressionStatement (invocation))));
		return compilationUnit;
	}


	static CompilationUnitSyntax StaticInvocationGenericExpression (string staticClassName, string methodName,
		string genericName,
		ArgumentListSyntax argumentList, bool suppressNullableWarning = false)
	{
		var invocation = InvocationExpression (
			MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName (staticClassName),
				GenericName (
						Identifier (methodName))
					.WithTypeArgumentList (TypeArgumentList (
						SingletonSeparatedList<TypeSyntax> (IdentifierName (genericName))))
					.WithTrailingTrivia (Space)
			)
		).WithArgumentList (argumentList);

		var compilationUnit = CompilationUnit ().WithMembers (
			SingletonList<MemberDeclarationSyntax> (
				GlobalStatement (
					ExpressionStatement (
						suppressNullableWarning
							? PostfixUnaryExpression (SyntaxKind.SuppressNullableWarningExpression, invocation)
							: invocation
					))));
		return compilationUnit;
	}
}
