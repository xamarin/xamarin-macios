using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Emitters;

class ClassEmitter : ICodeEmitter {
	public string GetSymbolName (in CodeChanges codeChanges) => codeChanges.Name;

	public IEnumerable<string> UsingStatements => [
		"System", 
		"System.Drawing", 
		"System.Diagnostics",
		"System.ComponentModel",
		"System.Threading.Tasks",
		"System.Runtime.Versioning",
		"System.Runtime.InteropServices",
		"System.Diagnostics.CodeAnalysis",
		"ObjCRuntime",
	];
	
		
	void EmitDefaultConstructors (in BindingContext bindingContext, TabbedStringBuilder classBlock, bool disableDefaultCtor)
	{

		if (!disableDefaultCtor) {
			classBlock.AppendGeneratedCodeAttribute ();
			classBlock.AppendDesignatedInitializer ();
			classBlock.AppendRaw (
$@"[Export (""init"")]
public {bindingContext.Changes.Name} () : base (NSObjectFlag.Empty)
{{
	if (IsDirectBinding)
		InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, global::ObjCRuntime.Selector.GetHandle (""init"")), ""init"");
	else
		InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, global::ObjCRuntime.Selector.GetHandle (""init"")), ""init"");
}}
");
		classBlock.AppendLine ();
		}

		classBlock.AppendGeneratedCodeAttribute ();
		classBlock.AppendEditorBrowsableAttribute (EditorBrowsableState.Advanced);
		classBlock.AppendLine ($"protected {bindingContext.Changes.Name} (NSObjectFlag t) : base (t) {{}}");
		
		classBlock.AppendLine ();
		classBlock.AppendGeneratedCodeAttribute ();
		classBlock.AppendEditorBrowsableAttribute (EditorBrowsableState.Advanced);
		classBlock.AppendLine ($"protected internal {bindingContext.Changes.Name} (NativeHandle handle) : base (handle) {{}}");
	}

	public bool TryEmit (in BindingContext bindingContext, [NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		if (bindingContext.Changes.BindingType != BindingType.Class) {
			diagnostics = [Diagnostic.Create (
					Diagnostics
						.RBI0000, // An unexpected error occurred while processing '{0}'. Please fill a bug report at https://github.com/xamarin/xamarin-macios/issues/new.
					null,
					bindingContext.Changes.FullyQualifiedSymbol)];
			return false;
		}
		
		// namespace declaration
		bindingContext.Builder.AppendLine ();
		bindingContext.Builder.AppendLine ($"namespace {string.Join (".", bindingContext.Changes.Namespace)};");
		bindingContext.Builder.AppendLine ();
		
		// register the class only if we are not dealing with a static class
		var bindingData = (BindingTypeData<Class>) bindingContext.Changes.BindingData;
		// registration depends on the class name. If the binding data contains a name, we use that one, else
		// we use the name of the class
		var registrationName = bindingData.Name ?? bindingContext.Changes.Name;
		
		if (!bindingContext.Changes.IsStatic) {
			bindingContext.Builder.AppendLine ($"[Register (\"{registrationName}\", true)]");
		}
		var modifiers = $"{string.Join (' ', bindingContext.Changes.Modifiers)} ";
		using (var classBlock = bindingContext.Builder.CreateBlock ($"{(string.IsNullOrWhiteSpace (modifiers) ? string.Empty : modifiers)}class {bindingContext.Changes.Name}", true)) {
			if (!bindingContext.Changes.IsStatic) {
				classBlock.AppendGeneratedCodeAttribute (optimizable: true);
				classBlock.AppendLine ($"static readonly NativeHandle class_ptr = Class.GetHandle (\"{registrationName}\");");
				classBlock.AppendLine ();
				classBlock.AppendLine ("public override NativeHandle ClassHandle => class_ptr;");
				classBlock.AppendLine ();

				EmitDefaultConstructors (bindingContext: bindingContext, 
					classBlock: classBlock, 
					disableDefaultCtor: bindingData.Flags.HasFlag (Class.DisableDefaultCtor));
			}
			
			classBlock.AppendLine ("// TODO: add binding code here");
		}
		return true;
	}
}
