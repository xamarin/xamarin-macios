using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Extensions;

public static class PropertySymbolExtensions {

	public static bool TryGetDeclaration (this IPropertySymbol propertySymbol,
		[NotNullWhen (true)] out string? propertyDeclaration)
	{
		propertyDeclaration = null;
		var propertySyntax = propertySymbol.DeclaringSyntaxReferences
			.FirstOrDefault ()?
			.GetSyntax () as PropertyDeclarationSyntax;
		return propertySyntax is not null && propertySyntax.TryGetDeclaration (out propertyDeclaration);
	}
}
