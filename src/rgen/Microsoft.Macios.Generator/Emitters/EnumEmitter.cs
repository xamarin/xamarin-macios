using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Emitters;

class EnumEmitter (RootBindingContext context, TabbedStringBuilder builder) : ICodeEmitter {

	public string GetSymbolName (in CodeChanges codeChanges) => $"{codeChanges.Name}Extensions";
	public IEnumerable<string> UsingStatements => ["Foundation", "ObjCRuntime", "System"];

	void EmitEnumFieldAtIndex (TabbedStringBuilder classBlock, in CodeChanges codeChanges, int index)
	{
		var enumField = codeChanges.EnumMembers [index];
		if (enumField.FieldData is null)
			return;
		if (!context.TryComputeLibraryName (enumField.FieldData.Value.LibraryName, codeChanges.Namespace[^1],
				out string? libraryName, out string? libraryPath)) {
			return;
		}

		classBlock.AppendMemberAvailability (enumField.SymbolAvailability);
		classBlock.AppendLine ($"[Field (\"{enumField.FieldData.Value.SymbolName}\", \"{libraryPath ?? libraryName}\")]");
		using (var propertyBlock = classBlock.CreateBlock ($"internal unsafe static IntPtr {enumField.FieldData.Value.SymbolName}", true))
		using (var getterBlock = propertyBlock.CreateBlock ("get", true)) {
			getterBlock.AppendLine ($"fixed (IntPtr *storage = &values [{index}])");
			getterBlock.AppendLine (
				$"\treturn Dlfcn.CachePointer (Libraries.{libraryName}.Handle, \"{enumField.FieldData.Value.SymbolName}\", storage);");
		}
	}

	bool TryEmit (TabbedStringBuilder classBlock, in CodeChanges codeChanges)
	{
		// keep track of the field symbols, they have to be unique, if we find a duplicate we return false and
		// abort the code generation
		var backingFields = new HashSet<string> ();
		for (var index = 0; index < codeChanges.EnumMembers.Length; index++) {
			if (codeChanges.EnumMembers[index].FieldData is null)
				continue;
			if (!backingFields.Add (codeChanges.EnumMembers[index].FieldData!.Value.SymbolName)) {
				return false;
			}
			classBlock.AppendLine ();
			EmitEnumFieldAtIndex (classBlock, codeChanges, index);
		}
		return true;
	}

	void EmitExtensionMethods (TabbedStringBuilder classBlock, in CodeChanges codeChanges)
	{
		if (codeChanges.EnumMembers.Length == 0)
			return;

		// smart enum require 4 diff methods to be able to retrieve the values

		// Get constant
		using (var getConstantBlock = classBlock.CreateBlock ($"public static NSString? GetConstant (this {codeChanges.Name} self)", true)) {
			getConstantBlock.AppendLine ("IntPtr ptr = IntPtr.Zero;");
			using (var switchBlock = getConstantBlock.CreateBlock ("switch ((int) self)", true)) {
				for (var index = 0; index < codeChanges.EnumMembers.Length; index++) {
					var enumMember = codeChanges.EnumMembers [index];
					if (enumMember.FieldData is null)
						continue;
					switchBlock.AppendLine ($"case {index}: // {enumMember.FieldData.Value.SymbolName}");
					switchBlock.AppendLine ($"\tptr = {enumMember.FieldData.Value.SymbolName};");
					switchBlock.AppendLine ("\tbreak;");
				}
			}

			getConstantBlock.AppendLine ("return (NSString?) Runtime.GetNSObject (ptr);");
		}

		classBlock.AppendLine ();
		// Get value
		using (var getValueBlock = classBlock.CreateBlock ($"public static {codeChanges.Name} GetValue (NSString constant)", true)) {
			getValueBlock.AppendLine ("if (constant is null)");
			getValueBlock.AppendLine ("\tthrow new ArgumentNullException (nameof (constant));");
			foreach (var enumMember in codeChanges.EnumMembers) {
				if (enumMember.FieldData is null)
					continue;
				getValueBlock.AppendLine ($"if (constant.IsEqualTo ({enumMember.FieldData.Value.SymbolName}))");
				getValueBlock.AppendLine ($"\treturn {codeChanges.Name}.{enumMember.Name};");
			}

			getValueBlock.AppendLine (
				"throw new NotSupportedException ($\"The constant {constant} has no associated enum value on this platform.\");");
		}

		classBlock.AppendLine ();
		// To ConstantArray
		classBlock.AppendRaw (
@$"internal static NSString?[]? ToConstantArray (this {codeChanges.Name}[]? values)
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
		classBlock.AppendLine ();
		classBlock.AppendLine ();
		// ToEnumArray
		classBlock.AppendRaw (
@$"internal static {codeChanges.Name}[]? ToEnumArray (this NSString[]? values)
{{
	if (values is null)
		return null;
	var rv = new global::System.Collections.Generic.List<{codeChanges.Name}> ();
	for (var i = 0; i < values.Length; i++) {{
		var value = values [i];
		rv.Add (GetValue (value));
	}}
	return rv.ToArray ();
}}");
	}

	public bool TryEmit (in CodeChanges codeChanges, [NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		if (codeChanges.BindingType != BindingType.SmartEnum) {
			diagnostics = [Diagnostic.Create (
					Diagnostics
						.RBI0000, // An unexpected error ocurred while processing '{0}'. Please fill a bug report at https://github.com/xamarin/xamarin-macios/issues/new.
					null,
					codeChanges.FullyQualifiedSymbol)];
			return false;
		}
		// in the old generator we had to copy over the enum, in this new approach, the only code
		// we need to create is the extension class for the enum that is backed by fields
		builder.AppendLine ();
		builder.AppendLine ($"namespace {string.Join(".", codeChanges.Namespace)};");
		builder.AppendLine ();

		builder.AppendMemberAvailability (codeChanges.SymbolAvailability);
		builder.AppendGeneratedCodeAttribute ();
		using (var classBlock = builder.CreateBlock ($"static public partial class {GetSymbolName (codeChanges)}", true)) {
			classBlock.AppendLine ();
			classBlock.AppendLine ($"static IntPtr[] values = new IntPtr [{codeChanges.EnumMembers.Length}];");
			// foreach member in the enum we need to create a field that holds the value, the property emitter
			// will take care of generating the property. Do not order it by name to keep the order of the enum
			if (!TryEmit (classBlock, codeChanges)) {
				diagnostics = []; // empty diagnostics since it was a user error
				return false;
			}
			classBlock.AppendLine ();

			// emit the extension methods that will be used to get the values from the enum
			EmitExtensionMethods (classBlock, codeChanges);
			classBlock.AppendLine ();
		}

		return true;
	}
}
