using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Emitters;

#pragma warning disable CS9113 // Parameter is unread. This class is work in progress
class ClassEmitter (RootBindingContext context, TabbedStringBuilder builder) : ICodeEmitter {
#pragma warning restore CS9113 // Parameter is unread.
	public string GetSymbolName (in CodeChanges codeChanges) => codeChanges.Name;

	public IEnumerable<string> UsingStatements => [];

	public bool TryEmit (in CodeChanges codeChanges, [NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{

		builder.AppendLine ();
		diagnostics = null;
		// add the namespace and the class declaration
		var modifiers = $"{string.Join (' ', codeChanges.Modifiers)} ";
		using (var namespaceBlock = builder.CreateBlock ($"namespace {codeChanges.Namespace [^1]}", true)) {
			using (var classBlock = namespaceBlock.CreateBlock ($"{(string.IsNullOrWhiteSpace (modifiers) ? string.Empty : modifiers)}class {GetSymbolName (codeChanges)}", true)) {
				classBlock.AppendLine ("// TODO: add binding code here");
			}
		}
		return true;
	}
}
