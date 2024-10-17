using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.Emitters;

class EnumEmitter (ISymbolBindingContext<EnumDeclarationSyntax> context, TabbedStringBuilder builder)
	: ICodeEmitter {

	public string SymbolName => $"{context.SymbolName}Extensions";

	void Emit (TabbedStringBuilder classBlock, (IFieldSymbol Symbol, FieldData FieldData) enumField, int index)
	{
		var typeNamespace = enumField.Symbol.ContainingType.ContainingNamespace.Name;
		if (!context.RootBindingContext.TryComputeLibraryName (enumField.FieldData.LibraryName, typeNamespace,
				out string? libraryName, out string? libraryPath)) {
			return;
		}

		classBlock.AppendLine ($"[Field (\"{enumField.FieldData.SymbolName}\", \"{libraryPath ?? libraryName}\")]");

		using (var propertyBlock = classBlock.CreateBlock ($"internal unsafe static IntPtr {enumField.FieldData.SymbolName}", true))
		using (var getterBlock = propertyBlock.CreateBlock ("get", true)) {
			getterBlock.AppendLine ($"fixed (IntPtr *storage = &values [{index}])");
			var lib = (libraryPath is null) ? $"Libraries.{libraryName}.Handle" : $"\"{libraryPath}\"";
			getterBlock.AppendLine (
				$"\treturn Dlfcn.CachePointer ({lib}, \"{enumField.FieldData.SymbolName}\", storage);");
		}
	}

	void Emit (TabbedStringBuilder classBlock, ImmutableArray<(IFieldSymbol Symbol, FieldData FieldData)> fields)
	{
		for (var index = 0; index < fields.Length; index++) {
			var field = fields [index];
			classBlock.AppendLine ();
			Emit (classBlock, field, index);
		}
	}

	void Emit (TabbedStringBuilder classBlock, INamedTypeSymbol enumSymbol,
		ImmutableArray<(IFieldSymbol Symbol, FieldData FieldData)>? members)
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
			foreach ((IFieldSymbol? fieldSymbol, FieldData? fieldData) in members) {
				getValueBlock.AppendLine ($"if (constant.IsEqualTo ({fieldData.SymbolName}))");
				getValueBlock.AppendLine ($"\treturn {enumSymbol.Name}.{fieldSymbol.Name};");
			}

			getValueBlock.AppendLine (
				"throw new NotSupportedException ($\"{constant} has no associated enum value on this platform.\");");
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
			diagnostics = new ImmutableArray<Diagnostic> ();
			return false;
		}
		// in the old generator we had to copy over the enum, in this new approach the only code
		// we need to create is the extension class for the enum that is backed by fields
		builder.AppendLine ();
		builder.AppendLine ($"namespace {context.Namespace};");
		builder.AppendLine ();

		builder.AppendGeneratedCodeAttribute ();
		using (var classBlock = builder.CreateBlock ($"static public partial class {SymbolName}", true)) {
			classBlock.AppendLine ();
			classBlock.AppendLine ($"static IntPtr[] values = new IntPtr [{members.Value.Length}];");
			// foreach member in the enum we need to create a field that holds the value, the property emitter
			// will take care of generating the property. Do not order by name to keep the order of the enum
			Emit (classBlock, members.Value);
			classBlock.AppendLine ();

			// emit the extension methods that will be used to get the values from the enum
			Emit (classBlock, context.Symbol, members);
			classBlock.AppendLine ();
		}

		return true;
	}
}
