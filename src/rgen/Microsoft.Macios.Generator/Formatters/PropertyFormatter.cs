// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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
	public static CompilationUnitSyntax ToDeclaration (this in Property property)
	{
		var compilationUnit = CompilationUnit ().WithMembers (
			SingletonList<MemberDeclarationSyntax> (
				PropertyDeclaration (
						type: property.ReturnType.GetIdentifierSyntax (),
						identifier: Identifier (property.Name))
					.WithModifiers (TokenList (property.Modifiers)))).NormalizeWhitespace ();
		return compilationUnit;
	}
	
	/// <summary>
	/// Return the declaration represented by the given property.
	/// </summary>
	/// <param name="property">The property whose declaration we want to generate.</param>
	/// <returns>A compilation unit syntax node with the declaration of the property.</returns>
	public static CompilationUnitSyntax? ToDeclaration (this in Property? property)
		=> property?.ToDeclaration ();
}
