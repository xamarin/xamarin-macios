using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Context;

static class ContextFactory {
	public static bool TryCreate<T> (RootBindingContext context, SemanticModel semanticModel,
		INamedTypeSymbol symbol, T declarationSyntax, [NotNullWhen (true)] out ISymbolBindingContext<T>? bindingContext) where T : BaseTypeDeclarationSyntax
	{
		bindingContext = declarationSyntax switch {
			ClassDeclarationSyntax c => Unsafe.As<ISymbolBindingContext<T>> (
				new ClassBindingContext (context, semanticModel, symbol, c)),
			EnumDeclarationSyntax => new SymbolBindingContext<T> (context, semanticModel, symbol, declarationSyntax),
			_ => null
		};
		return bindingContext is not null;
	}
}
