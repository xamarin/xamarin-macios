using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;

namespace Microsoft.Macios.Generator.Emitters;

#pragma warning disable CS9113 // Parameter is unread, this class is work in progress
class InterfaceEmitter (SymbolBindingContext context, TabbedStringBuilder builder) : ICodeEmitter {
#pragma warning restore CS9113 // Parameter is unread.
	public string SymbolNamespace => context.Namespace;
	public string SymbolName { get; } = string.Empty;
	public IEnumerable<string> UsingStatements => [];
	public bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		return true;
	}
}
