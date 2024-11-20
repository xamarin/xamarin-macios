using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Emitters;

public class TrampolineEmitter : ICodeEmitter {
	public string SymbolNamespace => string.Empty;
	public string SymbolName { get; } = string.Empty;
	public IEnumerable<string> UsingStatements { get; } = [];
	public bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		return true;
	}

}
