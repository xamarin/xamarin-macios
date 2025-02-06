// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Formatters;
using Microsoft.Macios.Generator.IO;

namespace Microsoft.Macios.Generator.Emitters;

class EnumEmitter : ICodeEmitter {

	public string GetSymbolName (in Binding binding) => $"{binding.Name}Extensions";
	public IEnumerable<string> UsingStatements => ["Foundation", "ObjCRuntime", "System"];

	void EmitEnumFieldAtIndex (TabbedStringBuilder classBlock, in Binding binding, int index)
	{
		var enumField = binding.EnumMembers [index];
		if (enumField.FieldInfo is null)
			return;

		var (fieldData, libraryName, libraryPath) = enumField.FieldInfo.Value;

		classBlock.AppendMemberAvailability (enumField.SymbolAvailability);
		classBlock.WriteLine ($"[Field (\"{fieldData.SymbolName}\", \"{libraryPath ?? libraryName}\")]");
		using (var propertyBlock = classBlock.CreateBlock ($"internal unsafe static IntPtr {fieldData.SymbolName}", true))
		using (var getterBlock = propertyBlock.CreateBlock ("get", true)) {
			getterBlock.WriteLine ($"fixed (IntPtr *storage = &values [{index}])");
			getterBlock.WriteLine (
				$"\treturn Dlfcn.CachePointer (Libraries.{libraryName}.Handle, \"{fieldData.SymbolName}\", storage);");
		}
	}

	bool TryEmit (TabbedStringBuilder classBlock, in Binding binding)
	{
		// keep track of the field symbols, they have to be unique, if we find a duplicate we return false and
		// abort the code generation
		var backingFields = new HashSet<string> ();
		for (var index = 0; index < binding.EnumMembers.Length; index++) {
			if (binding.EnumMembers [index].FieldInfo is null)
				continue;
			if (!backingFields.Add (binding.EnumMembers [index].FieldInfo!.Value.FieldData.SymbolName)) {
				return false;
			}
			classBlock.WriteLine ();
			EmitEnumFieldAtIndex (classBlock, binding, index);
		}
		return true;
	}

	void EmitExtensionMethods (TabbedStringBuilder classBlock, in Binding binding)
	{
		if (binding.EnumMembers.Length == 0)
			return;

		// smart enum require 4 diff methods to be able to retrieve the values

		// Get constant
		using (var getConstantBlock = classBlock.CreateBlock ($"public static NSString? GetConstant (this {binding.Name} self)", true)) {
			getConstantBlock.WriteLine ("IntPtr ptr = IntPtr.Zero;");
			using (var switchBlock = getConstantBlock.CreateBlock ("switch ((int) self)", true)) {
				for (var index = 0; index < binding.EnumMembers.Length; index++) {
					var enumMember = binding.EnumMembers [index];
					if (enumMember.FieldInfo is null)
						continue;
					var (fieldData, _, _) = enumMember.FieldInfo.Value;
					switchBlock.WriteLine ($"case {index}: // {fieldData.SymbolName}");
					switchBlock.WriteLine ($"\tptr = {fieldData.SymbolName};");
					switchBlock.WriteLine ("\tbreak;");
				}
			}

			getConstantBlock.WriteLine ("return (NSString?) Runtime.GetNSObject (ptr);");
		}

		classBlock.WriteLine ();
		// Get value
		using (var getValueBlock = classBlock.CreateBlock ($"public static {binding.Name} GetValue (NSString constant)", true)) {
			getValueBlock.WriteLine ("if (constant is null)");
			getValueBlock.WriteLine ("\tthrow new ArgumentNullException (nameof (constant));");
			foreach (var enumMember in binding.EnumMembers) {
				if (enumMember.FieldInfo is null)
					continue;
				var (fieldData, _, _) = enumMember.FieldInfo.Value;
				getValueBlock.WriteLine ($"if (constant.IsEqualTo ({fieldData.SymbolName}))");
				getValueBlock.WriteLine ($"\treturn {binding.Name}.{enumMember.Name};");
			}

			getValueBlock.WriteLine (
				"throw new NotSupportedException ($\"The constant {constant} has no associated enum value on this platform.\");");
		}

		classBlock.WriteLine ();
		// To ConstantArray
		classBlock.WriteRaw (
@$"internal static NSString?[]? ToConstantArray (this {binding.Name}[]? values)
{{
	if (values is null)
		return null;
	var rv = new global::System.Collections.Generic.List<NSString?> ();
	for (var i = 0; i < values.Length; i++) {{
		var value = values [i];
		rv.Add (value.GetConstant ());
	}}
	return rv.ToArray ();
}}");
		classBlock.WriteLine ();
		classBlock.WriteLine ();
		// ToEnumArray
		classBlock.WriteRaw (
@$"internal static {binding.Name}[]? ToEnumArray (this NSString[]? values)
{{
	if (values is null)
		return null;
	var rv = new global::System.Collections.Generic.List<{binding.Name}> ();
	for (var i = 0; i < values.Length; i++) {{
		var value = values [i];
		rv.Add (GetValue (value));
	}}
	return rv.ToArray ();
}}");
	}

	public bool TryEmit (in BindingContext bindingContext, [NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		if (bindingContext.Changes.BindingType != BindingType.SmartEnum) {
			diagnostics = [Diagnostic.Create (
					Diagnostics
						.RBI0000, // An unexpected error occurred while processing '{0}'. Please fill a bug report at https://github.com/xamarin/xamarin-macios/issues/new.
					null,
					bindingContext.Changes.FullyQualifiedSymbol)];
			return false;
		}
		// in the old generator we had to copy over the enum, in this new approach, the only code
		// we need to create is the extension class for the enum that is backed by fields
		bindingContext.Builder.WriteLine ();
		bindingContext.Builder.WriteLine ($"namespace {string.Join (".", bindingContext.Changes.Namespace)};");
		bindingContext.Builder.WriteLine ();

		bindingContext.Builder.AppendMemberAvailability (bindingContext.Changes.SymbolAvailability);
		bindingContext.Builder.AppendGeneratedCodeAttribute ();
		var extensionClassDeclaration =
			bindingContext.Changes.ToSmartEnumExtensionDeclaration (GetSymbolName (bindingContext.Changes));
		using (var classBlock = bindingContext.Builder.CreateBlock (extensionClassDeclaration.ToString (), true)) {
			classBlock.WriteLine ();
			classBlock.WriteLine ($"static IntPtr[] values = new IntPtr [{bindingContext.Changes.EnumMembers.Length}];");
			// foreach member in the enum we need to create a field that holds the value, the property emitter
			// will take care of generating the property. Do not order it by name to keep the order of the enum
			if (!TryEmit (classBlock, bindingContext.Changes)) {
				diagnostics = []; // empty diagnostics since it was a user error
				return false;
			}
			classBlock.WriteLine ();

			// emit the extension methods that will be used to get the values from the enum
			EmitExtensionMethods (classBlock, bindingContext.Changes);
			classBlock.WriteLine ();
		}

		return true;
	}
}
