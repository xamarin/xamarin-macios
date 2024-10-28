using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.Context;

class ClassBindingContext : SymbolBindingContext {
	public string RegisterName { get; }

	public ClassBindingContext (RootBindingContext context, SemanticModel semanticModel,
		INamedTypeSymbol symbol, ClassDeclarationSyntax declarationSyntax)
		: base (context, semanticModel, symbol, declarationSyntax)
	{
		symbol.TryGetBindingData (out var bindingData);
		// if null, either due to bindingData being null or the property being null, use the symbol name
		RegisterName = bindingData?.Name ?? symbol.Name;
	}
}
