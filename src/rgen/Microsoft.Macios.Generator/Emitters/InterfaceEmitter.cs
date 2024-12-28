using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Emitters;

#pragma warning disable CS9113 // Parameter is unread, this class is work in progress
class InterfaceEmitter (RootBindingContext context, TabbedStringBuilder builder) : ICodeEmitter {
#pragma warning restore CS9113 // Parameter is unread.
	public string GetSymbolName (in CodeChanges codeChanges) => string.Empty;
	public IEnumerable<string> UsingStatements => [];
	public bool TryEmit (in CodeChanges codeChanges, [NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		return true;
	}
}
