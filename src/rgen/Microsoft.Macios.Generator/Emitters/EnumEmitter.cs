using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;

namespace Microsoft.Macios.Generator.Emitters;

#pragma warning disable CS9113 // Parameter is unread. This class is work in progress
class EnumEmitter (ISymbolBindingContext<EnumDeclarationSyntax> context, TabbedStringBuilder builder)
#pragma warning restore CS9113 // Parameter is unread.
	: ICodeEmitter {

	public string SymbolName => $"{context.SymbolName}Extensions";

	public IEnumerable<string> UsingStatements => [];
	
	public bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		return true;
	}
}
