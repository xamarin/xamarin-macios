using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;

namespace Microsoft.Macios.Generator.Emitters;

class EnumEmitter (ISymbolBindingContext<EnumDeclarationSyntax> context, TabbedStringBuilder builder)
	: ICodeEmitter {

	public string SymbolName => $"{context.SymbolName}Extensions";

	public bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		return true;
	}
}
