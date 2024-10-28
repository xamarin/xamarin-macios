using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Extensions;

public static class MethodSymbolExtensions {
	public static bool TryGetDeclaration (this IMethodSymbol methodSymbol,
		[NotNullWhen (true)] out string? methodDeclaration)
	{
		methodDeclaration = null;
		if (!methodSymbol.IsPartialDefinition)
			return false;

		var declarationSyntax = methodSymbol
			.DeclaringSyntaxReferences
			.FirstOrDefault ()?
			.GetSyntax () as MethodDeclarationSyntax;
		return declarationSyntax is not null && declarationSyntax.TryGetDeclaration (out methodDeclaration);
	}
}
