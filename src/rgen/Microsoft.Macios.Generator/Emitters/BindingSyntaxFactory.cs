// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
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
		SyntaxNodeOrToken [] argumentList, bool suppressNullableWarning = false)
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
					ExpressionStatement (
						suppressNullableWarning
							? PostfixUnaryExpression (SyntaxKind.SuppressNullableWarningExpression, invocation)
							: invocation))));
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

	static CompilationUnitSyntax ThrowException (string type, string message)
	{
		var argumentList = ArgumentList (SingletonSeparatedList (
				GetLiteralExpressionArgument (SyntaxKind.StringLiteralExpression, message)
			)).WithLeadingTrivia (Space);

		var throwExpression = ThrowStatement (
			ObjectCreationExpression (IdentifierName (type))
				.WithArgumentList (argumentList)).NormalizeWhitespace ();

		return CompilationUnit ().WithMembers (
			SingletonList<MemberDeclarationSyntax> (
				GlobalStatement (throwExpression)));
	}

	static CompilationUnitSyntax ThrowNotSupportedException (string message)
		=> ThrowException (type: "NotSupportedException", message: message);

	/// <summary>
	/// Generates the syntax to declare the variable used by a field property.  
	/// </summary>
	/// <param name="property">The field property whose backing variable we want to generate.</param>
	/// <returns>The variable declaration syntax.</returns>
	public static CompilationUnitSyntax FieldPropertyBackingVariable (in Property property)
	{
		var variableType = property.ReturnType.Name;
		if (property.ReturnType.SpecialType is SpecialType.System_IntPtr or SpecialType.System_UIntPtr
			&& property.ReturnType.MetadataName is not null) {
			variableType = property.ReturnType.MetadataName;
		}
		var compilationUnit = CompilationUnit ().WithMembers (
			SingletonList<MemberDeclarationSyntax> (
				FieldDeclaration (
						VariableDeclaration (
								property.IsReferenceType  // nullable only for reference types
									? NullableType (IdentifierName (variableType))
									: IdentifierName (variableType)
							)
							.WithVariables (
								SingletonSeparatedList (
									VariableDeclarator (
										Identifier (property.BackingField)))))
					.WithModifiers (TokenList (Token (SyntaxKind.StaticKeyword))))) // fields are static variables
			.NormalizeWhitespace ();
		return compilationUnit;
	}
}
