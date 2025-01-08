using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Emitters;

class TrampolineEmitter : ICodeEmitter {
	public string SymbolNamespace => string.Empty;
	public string GetSymbolName (in CodeChanges codeChanges) => string.Empty;
	public IEnumerable<string> UsingStatements { get; } = [];
	public bool TryEmit (in BindingContext bindingContext, [NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		return true;
	}

}
