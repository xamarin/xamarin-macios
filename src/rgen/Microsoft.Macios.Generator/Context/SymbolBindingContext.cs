using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Context;

class SymbolBindingContext {

	public RootBindingContext RootBindingContext { get; init; }
	public SemanticModel SemanticModel { get; init; }
	public INamedTypeSymbol Symbol { get; init; }

	public bool IsStatic => Symbol.IsStatic;

	public SymbolBindingContext (RootBindingContext rootBindingContext,
		SemanticModel semanticModel, INamedTypeSymbol symbol)
	{
		RootBindingContext = rootBindingContext;
		SemanticModel = semanticModel;
		Symbol = symbol;
	}

}

class SymbolBindingContext<T> : SymbolBindingContext, ISymbolBindingContext<T> where T : BaseTypeDeclarationSyntax {

	public T DeclarationSyntax { get; }
	public string Namespace => Symbol.ContainingNamespace.ToDisplayString ();
	public string SymbolName => Symbol.Name;

	public SymbolBindingContext (RootBindingContext rootBindingContext,
		SemanticModel semanticModel, INamedTypeSymbol symbol, T declarationSyntax)
		: base (rootBindingContext, semanticModel, symbol)
	{
		DeclarationSyntax = declarationSyntax;
	}
}
