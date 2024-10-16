using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Context;

/// <summary>
/// Interface that represents a symbol binding context. We use an interface to allow the usage of a coveriance type,
/// because it removes the need to a cast.
/// </summary>
/// <typeparam name="T">The base type declaration whose context we have</typeparam>
interface ISymbolBindingContext<out T> where T : BaseTypeDeclarationSyntax {
	T DeclarationSyntax { get; }
	string Namespace { get; }
	string SymbolName { get; }
	RootBindingContext RootBindingContext { get; init; }
	SemanticModel SemanticModel { get; init; }
	INamedTypeSymbol Symbol { get; init; }
	bool IsStatic { get; }
}
