using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Emitters;

#pragma warning disable CS9113 // Parameter is unread. This class is work in progress
class EnumEmitter (SymbolBindingContext context, TabbedStringBuilder builder)
#pragma warning restore CS9113 // Parameter is unread.
	: ICodeEmitter {

	public string SymbolNamespace => context.Namespace;
	public string SymbolName => $"{context.SymbolName}Extensions";
	public IEnumerable<string> UsingStatements => ["Foundation", "ObjCRuntime", "System"];

	void Emit (TabbedStringBuilder classBlock, (IFieldSymbol Symbol, FieldData<EnumValue> FieldData) enumField, int index)
	{
		var typeNamespace = enumField.Symbol.ContainingType.ContainingNamespace.Name;
		if (!context.RootBindingContext.TryComputeLibraryName (enumField.FieldData.LibraryName, typeNamespace,
				out string? libraryName, out string? libraryPath)) {
			return;
		}

		// availability is the merge of the container plus whatever we added to the current members
		var availability = enumField.Symbol.GetSupportedPlatforms ();

		classBlock.AppendMemberAvailability (availability);
		classBlock.AppendLine ($"[Field (\"{enumField.FieldData.SymbolName}\", \"{libraryPath ?? libraryName}\")]");
		using (var propertyBlock = classBlock.CreateBlock ($"internal unsafe static IntPtr {enumField.FieldData.SymbolName}", true))
		using (var getterBlock = propertyBlock.CreateBlock ("get", true)) {
			getterBlock.AppendLine ($"fixed (IntPtr *storage = &values [{index}])");
			getterBlock.AppendLine (
				$"\treturn Dlfcn.CachePointer (Libraries.{libraryName}.Handle, \"{enumField.FieldData.SymbolName}\", storage);");
		}
	}

	bool TryEmit (TabbedStringBuilder classBlock, ImmutableArray<(IFieldSymbol Symbol, FieldData<EnumValue> FieldData)> fields)
	{
		// keep track of the field symbols, they have to be unique, if we find a duplicate we return false and
		// abort the code generation
		var backingFields = new HashSet<string> ();
		for (var index = 0; index < fields.Length; index++) {
			if (!backingFields.Add (fields [index].FieldData.SymbolName)) {
				return false;
			}
			var field = fields [index];
			classBlock.AppendLine ();
			Emit (classBlock, field, index);
		}
		return true;
	}

	void Emit (TabbedStringBuilder classBlock, INamedTypeSymbol enumSymbol,
		ImmutableArray<(IFieldSymbol Symbol, FieldData<EnumValue> FieldData)>? members)
	{
		if (members is null)
			return;

		// smart enum require 4 diff methods to be able to retrieve the values

		// Get constant
		using (var getConstantBlock = classBlock.CreateBlock ($"public static NSString? GetConstant (this {enumSymbol.Name} self)", true)) {
			getConstantBlock.AppendLine ("IntPtr ptr = IntPtr.Zero;");
			using (var switchBlock = getConstantBlock.CreateBlock ("switch ((int) self)", true)) {
				for (var index = 0; index < members.Value.Length; index++) {
					var (_, fieldData) = members.Value [index];
					switchBlock.AppendLine ($"case {index}: // {fieldData.SymbolName}");
					switchBlock.AppendLine ($"\tptr = {fieldData.SymbolName};");
					switchBlock.AppendLine ("\tbreak;");
				}
			}

			getConstantBlock.AppendLine ("return (NSString?) Runtime.GetNSObject (ptr);");
		}

		classBlock.AppendLine ();
		// Get value
		using (var getValueBlock = classBlock.CreateBlock ($"public static {enumSymbol.Name} GetValue (NSString constant)", true)) {
			getValueBlock.AppendLine ("if (constant is null)");
			getValueBlock.AppendLine ("\tthrow new ArgumentNullException (nameof (constant));");
			foreach ((IFieldSymbol fieldSymbol, FieldData<EnumValue> fieldData) in members) {
				getValueBlock.AppendLine ($"if (constant.IsEqualTo ({fieldData.SymbolName}))");
				getValueBlock.AppendLine ($"\treturn {enumSymbol.Name}.{fieldSymbol.Name};");
			}

			getValueBlock.AppendLine (
				"throw new NotSupportedException ($\"The constant {constant} has no associated enum value on this platform.\");");
		}

		classBlock.AppendLine ();
		// To ConstantArray
		classBlock.AppendRaw (
@$"internal static NSString?[]? ToConstantArray (this {enumSymbol.Name}[]? values)
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
@$"internal static {enumSymbol.Name}[]? ToEnumArray (this NSString[]? values)
{{
	if (values is null)
		return null;
	var rv = new global::System.Collections.Generic.List<{enumSymbol.Name}> ();
	for (var i = 0; i < values.Length; i++) {{
		var value = values [i];
		rv.Add (GetValue (value));
	}}
	return rv.ToArray ();
}}");
	}

	public bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		if (!context.Symbol.TryGetEnumFields (out var members,
				out diagnostics) || members.Value.Length == 0) {
			// return true to indicate that we did generate code, even if it's empty, the analyzer will take care
			// of the rest
			return true;
		}
		// in the old generator we had to copy over the enum, in this new approach the only code
		// we need to create is the extension class for the enum that is backed by fields
		builder.AppendLine ();
		builder.AppendLine ($"namespace {context.Namespace};");
		builder.AppendLine ();

		builder.AppendMemberAvailability (context.SymbolAvailability);
		builder.AppendGeneratedCodeAttribute ();
		using (var classBlock = builder.CreateBlock ($"static public partial class {SymbolName}", true)) {
			classBlock.AppendLine ();
			classBlock.AppendLine ($"static IntPtr[] values = new IntPtr [{members.Value.Length}];");
			// foreach member in the enum we need to create a field that holds the value, the property emitter
			// will take care of generating the property. Do not order by name to keep the order of the enum
			if (!TryEmit (classBlock, members.Value)) {
				diagnostics = []; // empty diagnostics since it was a user error
				return false;
			}
			classBlock.AppendLine ();

			// emit the extension methods that will be used to get the values from the enum
			Emit (classBlock, context.Symbol, members);
			classBlock.AppendLine ();
		}

		return true;
	}
}
