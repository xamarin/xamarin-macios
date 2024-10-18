using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;

namespace Microsoft.Macios.Generator.Emitters;

class InterfaceEmitter (ISymbolBindingContext<InterfaceDeclarationSyntax> context, TabbedStringBuilder builder) : ICodeEmitter {
	public string SymbolName { get; } = string.Empty;
	public bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		return true;
	}
}
