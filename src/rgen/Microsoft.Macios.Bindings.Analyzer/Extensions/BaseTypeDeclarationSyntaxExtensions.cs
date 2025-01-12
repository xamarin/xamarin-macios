// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Bindings.Analyzer.Extensions;

public static class BaseTypeDeclarationSyntaxExtensions {

	/// <summary>
	/// Returns if the based type declaration was declared as a partial one.
	/// </summary>
	/// <param name="baseTypeDeclarationSyntax">The declaration under test.</param>
	/// <returns>True if the declaration is partial.</returns>
	public static bool IsPartial (this BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
		=> baseTypeDeclarationSyntax.Modifiers.Any (x => x.IsKind (SyntaxKind.PartialKeyword));

	/// <summary>
	/// Returns if the based type declaration was declared as a static one.
	/// </summary>
	/// <param name="baseTypeDeclarationSyntax">The declaration under test.</param>
	/// <returns>True if the declaration is static.</returns>
	public static bool IsStatic (this BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
		=> baseTypeDeclarationSyntax.Modifiers.Any (x => x.IsKind (SyntaxKind.StaticKeyword));

}
