using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.Emitters;

class ClassEmitter (ClassBindingContext context, TabbedStringBuilder builder) : ICodeEmitter {
	public string SymbolNamespace => context.Namespace;
	public string SymbolName => context.SymbolName;
	public IEnumerable<string> UsingStatements => [];
	readonly ConstructorEmitter constructorEmitter = new(context);
	readonly FieldEmitter fieldEmitter = new(context);
	readonly MethodEmitter methodEmitter = new(context);
	readonly PropertyEmitter propertyEmitter = new(context);

	public bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		var diagnosticsBucket= ImmutableArray.CreateBuilder<Diagnostic> ();
		builder.AppendLine ();
		diagnostics = null;
		builder.AppendLine ($"namespace {context.Namespace};");
		builder.AppendLine ();
		// only register the class if it is not static
		if (!context.IsStatic) {
			builder.AppendLine ($"[Register(\"{context.RegisterName}\", true)]");
		}

		using (var classBlock =
		       builder.CreateBlock (
			       $"public unsafe {(context.IsStatic ? "static " : string.Empty)}partial class {context.SymbolName}",
			       true)) {
			// generate the class handle only if the class is not static
			if (!context.IsStatic) {
				classBlock.AppendGeneratedCodeAttribute (optimizable: true);
				classBlock.AppendLine (
					$"static readonly NativeHandle class_ptr = Class.GetHandle (\"{context.RegisterName}\");");
				classBlock.AppendLine ();
				classBlock.AppendRaw (Documentation.ClassHandle);
				classBlock.AppendLine ("public override NativeHandle ClassHandle { get { return class_ptr; } }");
				classBlock.AppendLine ();
			}

			if (context.Symbol.TryGetExportedMembers (out var exportedMembers)) {
				// relay the emitting of the different members to the correct emitters

			} else {
				// TODO: Diagnostic
			}
		} // class block

		return true;
	}
}
