// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Formatters;

static class SmartEnumFormatter {
	/// <summary>
	/// Return the declaration of the extension class for a given smart enum using the provided class name.
	/// </summary>
	/// <param name="smartEnumChange">The smart enum code change.</param>
	/// <param name="extensionClassName">The name to use for the extension class.</param>
	/// <returns>The class declaration for the extension class for a smart enum change.</returns>
	public static CompilationUnitSyntax ToSmartEnumExtensionDeclaration (this in CodeChanges smartEnumChange,
		string extensionClassName)
	{
		// add to a set to make sure we do no duplicate them and make sure static and partial are present 
		var modifiers = new HashSet<SyntaxToken> (smartEnumChange.Modifiers) {
			Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword),
		};

		var compilationUnit = CompilationUnit ().WithMembers (
				SingletonList<MemberDeclarationSyntax> (
					ClassDeclaration (extensionClassName)
						.WithModifiers (TokenList (modifiers))
						.WithOpenBraceToken (MissingToken (SyntaxKind.OpenBraceToken)) // do not add open/close brace
						.WithCloseBraceToken (MissingToken (SyntaxKind.CloseBraceToken))))
			.NormalizeWhitespace (); // no diff between dotnet and mono
		return compilationUnit;
	}

	/// <summary>
	/// Return the declaration of the extension class for a given smart enum using the provided class name.
	/// </summary>
	/// <param name="smartEnumChange">The smart enum code change.</param>
	/// <param name="extensionClassName">The name to use for the extension class.</param>
	/// <returns>The class declaration for the extension class for a smart enum change.</returns>
	public static CompilationUnitSyntax? ToSmartEnumExtensionDeclaration (this in CodeChanges? smartEnumChange,
		string extensionClassName)
		=> smartEnumChange is null ? null : ToSmartEnumExtensionDeclaration (smartEnumChange.Value, extensionClassName);
}
