using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Context;

class SymbolBindingContext {

	public RootBindingContext RootBindingContext { get; }
	public SemanticModel SemanticModel { get; }
	public INamedTypeSymbol Symbol { get; }

	public BaseTypeDeclarationSyntax DeclarationSyntax { get; }
	public string Namespace => Symbol.ContainingNamespace.ToDisplayString ();
	public string SymbolName => Symbol.Name;

	public bool IsStatic => Symbol.IsStatic;

	// keep track of the fields we have added to the symbol
	Dictionary<string, int> fields = new ();

	public SymbolBindingContext (RootBindingContext rootBindingContext,
		SemanticModel semanticModel, INamedTypeSymbol symbol, BaseTypeDeclarationSyntax declarationSyntax)
	{
		RootBindingContext = rootBindingContext;
		SemanticModel = semanticModel;
		Symbol = symbol;
		DeclarationSyntax = declarationSyntax;
	}

	public string GetUniqueVariableName (string hintName)
	{
		if (fields.TryGetValue (hintName, out var count)) {

		} else {
			fields.Add (hintName, 0);
			count = 0;
		}
		return $"{hintName}_{count}";
	}
}
