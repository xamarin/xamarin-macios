using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Emitters;

class ClassEmitter : ICodeEmitter {
	public string GetSymbolName (in CodeChanges codeChanges) => codeChanges.Name;

	public IEnumerable<string> UsingStatements => [];

	public bool TryEmit (in BindingContext bindingContext, [NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{

		bindingContext.Builder.AppendLine ();
		diagnostics = null;
		// add the namespace and the class declaration
		var modifiers = $"{string.Join (' ', bindingContext.Changes.Modifiers)} ";
		using (var namespaceBlock = bindingContext.Builder.CreateBlock ($"namespace {bindingContext.Changes.Namespace [^1]}", true)) {
			using (var classBlock = namespaceBlock.CreateBlock ($"{(string.IsNullOrWhiteSpace (modifiers) ? string.Empty : modifiers)}class {GetSymbolName (bindingContext.Changes)}", true)) {
				classBlock.AppendLine ("// TODO: add binding code here");
			}
		}
		return true;
	}
}
