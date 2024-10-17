using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Context;

/// <summary>
/// Interface that represents a symbol binding context. We use an interface to allow the usage of a coveriance type,
/// because it removes the need to a cast.
/// </summary>
/// <typeparam name="T">The base type declaration whose context we have</typeparam>
interface ISymbolBindingContext<out T> where T : BaseTypeDeclarationSyntax {
	BaseTypeDeclarationSyntax DeclarationSyntax { get; }
	string Namespace { get; }
	string SymbolName { get; }
	RootBindingContext RootBindingContext { get; }
	SemanticModel SemanticModel { get; }
	INamedTypeSymbol Symbol { get; }
	bool IsStatic { get; }
}
