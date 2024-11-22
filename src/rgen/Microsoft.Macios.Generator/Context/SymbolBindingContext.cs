using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.Context;

class SymbolBindingContext {

	public RootBindingContext RootBindingContext { get; }
	public SemanticModel SemanticModel { get; }
	public INamedTypeSymbol Symbol { get; }

	public BaseTypeDeclarationSyntax DeclarationSyntax { get; }
	public string Namespace => Symbol.ContainingNamespace.ToDisplayString ();
	public string SymbolName => Symbol.Name;

	public bool IsStatic => Symbol.IsStatic;
	public SymbolAvailability SymbolAvailability { get; }

	public SymbolBindingContext (RootBindingContext rootBindingContext,
		SemanticModel semanticModel, INamedTypeSymbol symbol, BaseTypeDeclarationSyntax declarationSyntax)
	{
		RootBindingContext = rootBindingContext;
		SemanticModel = semanticModel;
		Symbol = symbol;
		DeclarationSyntax = declarationSyntax;
		SymbolAvailability = symbol.GetSupportedPlatforms ();
	}
}
