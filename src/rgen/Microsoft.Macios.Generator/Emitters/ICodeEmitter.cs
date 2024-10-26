using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Interface to be implemented by all those classes that know how to emit code for a binding.
/// </summary>
interface ICodeEmitter {
	public string SymbolName { get; }
	bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics);
}
