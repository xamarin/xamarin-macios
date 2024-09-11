// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Foundation;
using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

public partial class Generator {

	static string GetCSharpTypeName (Type type)
	{
		switch (type.Name) {
		case "Byte":
			return "byte";
		case "SByte":
			return "sbyte";
		case "Int16":
			return "short";
		case "UInt16":
			return "ushort";
		case "Int32":
			return "int";
		case "UInt32":
			return "uint";
		case "Int64":
			return "long";
		case "UInt64":
			return "ulong";
		default:
			throw new NotSupportedException ("Enum based on " + type.Name);
		}
	}

	void CopyNativeName (ICustomAttributeProvider provider)
	{
		foreach (var oa in AttributeManager.GetCustomAttributes<NativeNameAttribute> (provider))
			print ("[NativeName (\"{0}\")]", oa.NativeName);
	}

	// caller already:
	//	- setup the header and namespace
	//	- call/emit PrintPlatformAttributes on the type
	void GenerateEnum (Type type)
	{
		var isFlagsEnum = AttributeManager.HasAttribute<FlagsAttribute> (type);
		if (isFlagsEnum)
			print ("[Flags]");

		var native = AttributeManager.GetCustomAttribute<NativeAttribute> (type);
		if (native is not null) {
			var sb = new StringBuilder ();
			sb.Append ("[Native");
			var hasNativeName = !string.IsNullOrEmpty (native.NativeName);
			var hasConvertToManaged = !string.IsNullOrEmpty (native.ConvertToManaged);
			var hasConvertToNative = !string.IsNullOrEmpty (native.ConvertToNative);
			if (hasNativeName || hasConvertToManaged || hasConvertToNative) {
				sb.Append (" (");
				if (hasNativeName)
					sb.Append ('"').Append (native.NativeName).Append ('"');
				if (hasConvertToManaged) {
					if (hasNativeName)
						sb.Append (", ");
					sb.Append ("ConvertToManaged = \"");
					sb.Append (native.ConvertToManaged);
					sb.Append ('"');
				}
				if (hasConvertToNative) {
					if (hasNativeName || hasConvertToManaged)
						sb.Append (", ");
					sb.Append ("ConvertToNative = \"");
					sb.Append (native.ConvertToNative);
					sb.Append ('"');
				}
				sb.Append (")");
			}
			sb.Append ("]");
			print (sb.ToString ());
		}
		CopyNativeName (type);

		var unique_constants = new HashSet<string> ();
		var fields = new Dictionary<FieldInfo, FieldAttribute> ();
		Tuple<FieldInfo, FieldAttribute> null_field = null;
		Tuple<FieldInfo, FieldAttribute> default_symbol = null;
		var underlying_type = GetCSharpTypeName (type.GetEnumUnderlyingType ());
		var is_internal = AttributeManager.HasAttribute<InternalAttribute> (type);
		var backingFieldType = AttributeManager.GetCustomAttribute<BackingFieldTypeAttribute> (type)?.BackingFieldType ?? TypeCache.NSString;
		var isBackingFieldValueType = backingFieldType.IsValueType;
		var visibility = is_internal ? "internal" : "public";

		if (backingFieldType != TypeCache.System_nint &&
			backingFieldType != TypeCache.System_nuint &&
			backingFieldType != TypeCache.System_Int32 &&
			backingFieldType != TypeCache.System_Int64 &&
			backingFieldType != TypeCache.System_UInt32 &&
			backingFieldType != TypeCache.System_UInt64 &&
			backingFieldType != TypeCache.NSString &&
			backingFieldType != TypeCache.NSNumber) {
			exceptions.Add (ErrorHelper.CreateError (1088 /* The backing field type '{0}' is invalid. Valid backing field types are: "NSString", "NSNumber", "nint" and "nuint". */, backingFieldType.FullName));
			backingFieldType = TypeCache.NSString;
		}

		print ("{0} enum {1} : {2} {{", visibility, type.Name, underlying_type);
		indent++;
		foreach (var f in type.GetFields ()) {
			// skip value__ field 
			if (f.IsSpecialName)
				continue;
			WriteDocumentation (f);
			PrintPlatformAttributes (f);
			PrintObsoleteAttributes (f);
			PrintExperimentalAttribute (f);
			print ("{0} = {1},", f.Name, f.GetRawConstantValue ());
			var fa = AttributeManager.GetCustomAttribute<FieldAttribute> (f);
			if (fa is null)
				continue;
			if (f.IsUnavailable (this))
				continue;
			if (fa.SymbolName is null)
				null_field = new Tuple<FieldInfo, FieldAttribute> (f, fa);
			else if (unique_constants.Contains (fa.SymbolName))
				throw new BindingException (1046, true, fa.SymbolName, type.Name);
			else {
				fields.Add (f, fa);
				unique_constants.Add (fa.SymbolName);
			}
			if (AttributeManager.GetCustomAttribute<DefaultEnumValueAttribute> (f) is not null) {
				if (default_symbol is not null)
					throw new BindingException (1045, true, type.Name);
				default_symbol = new Tuple<FieldInfo, FieldAttribute> (f, fa);
			}
		}
		indent--;
		print ("}");
		unique_constants.Clear ();

		var library_name = type.Namespace;
		var error = AttributeManager.GetCustomAttribute<ErrorDomainAttribute> (type);
		if ((fields.Count > 0) || (error is not null) || (null_field is not null)) {
			print ("");
			if (BindingTouch.SupportsXmlDocumentation) {
				print ($"/// <summary>Extension methods for the <see cref=\"global::{type.FullName}\" /> enumeration.</summary>");
				if (error is not null) {
					print ($"/// <remarks>");
					print ($"///   <para>The extension method for the <see cref=\"global::{type.FullName}\" /> enumeration can be used to fetch the error domain associated with these error codes.</para>");
					print ($"/// </remarks>");
				}
			}
			// the *Extensions has the same version requirement as the enum itself
			PrintPlatformAttributes (type);
			PrintExperimentalAttribute (type);
			print_generated_code ();
			print ("static {1} partial class {0}Extensions {{", type.Name, visibility);
			indent++;

			if (error is not null) {
				if (!TryComputeLibraryName (error.LibraryName, type, out library_name, out var _))
					throw ErrorHelper.CreateError (1087, /* Missing value for the 'LibraryName' property for the '[ErrorDomain]' attribute for {0} (e.g. '[ErrorDomain ("MyDomain", LibraryName = "__Internal")]') */ type.FullName);
			} else {
				var field = fields.FirstOrDefault ();
				var fieldAttr = field.Value;

				if (!TryComputeLibraryName (fieldAttr?.LibraryName, type, out library_name, out var _))
					throw ErrorHelper.CreateError (1042, /* Missing '[Field (LibraryName=value)]' for {0} (e.g."__Internal") */ type.FullName + "." + field.Key?.Name);
			}
		}

		if (error is not null) {
			// this attribute is important for our tests
			print ("[Field (\"{0}\", \"{1}\")]", error.ErrorDomain, library_name);
			print ("static NSString? _domain;");
			print ("");
			if (BindingTouch.SupportsXmlDocumentation) {
				print ($"/// <summary>Returns the error domain associated with the {type.FullName} value</summary>");
				print ($"/// <param name=\"self\">The enumeration value</param>");
				print ($"/// <remarks>");
				print ($"///   <para>See the <see cref=\"global::Foundation.NSError\" /> for information on how to use the error domains when reporting errors.</para>");
				print ($"/// </remarks>");
			}
			print ("public static NSString? GetDomain (this {0} self)", type.Name);
			print ("{");
			indent++;
			print ("if (_domain is null)");
			indent++;
			print ("_domain = Dlfcn.GetStringConstant (Libraries.{0}.Handle, \"{1}\");", library_name, error.ErrorDomain);
			indent--;
			print ("return _domain;");
			indent--;
			print ("}");
		}

		if ((fields.Count > 0) || (null_field is not null)) {
			print ("static IntPtr[] values = new IntPtr [{0}];", fields.Count);
			print ("");

			int n = 0;
			var backingFieldTypeName = TypeManager.FormatType (type, backingFieldType);
			foreach (var kvp in fields) {
				var f = kvp.Key;
				var fa = kvp.Value;
				// the attributes (availability and field) are important for our tests
				PrintPlatformAttributes (f);
				libraries.TryGetValue (library_name, out var libPath);
				var libname = fa.LibraryName?.Replace ("+", string.Empty);
				// We need to check for special cases inside FieldAttributes
				// fa.LibraryName could contain a different framework i.e UITrackingRunLoopMode (UIKit) inside NSRunLoopMode enum (Foundation).
				// libPath could have a custom path i.e. CoreImage which in macOS is inside Quartz
				// library_name contains the Framework constant name the Field is inside of, used as fallback.
				bool useFieldAttrLibName = libname is not null && !string.Equals (libname, library_name, StringComparison.OrdinalIgnoreCase);
				print ("[Field (\"{0}\", \"{1}\")]", fa.SymbolName, useFieldAttrLibName ? libname : libPath ?? library_name);
				print ("internal unsafe static {1} {0} {{", fa.SymbolName, isBackingFieldValueType ? backingFieldTypeName + "*" : "IntPtr");
				indent++;
				print ("get {");
				indent++;
				print ("fixed (IntPtr *storage = &values [{0}])", n++);
				indent++;
				var cast = isBackingFieldValueType ? $"({backingFieldTypeName} *) " : string.Empty;
				print ("return {2}Dlfcn.CachePointer (Libraries.{0}.Handle, \"{1}\", storage);", useFieldAttrLibName ? libname : library_name, fa.SymbolName, cast);
				indent--;
				indent--;
				print ("}");
				indent--;
				print ("}");
				print ("");
			}

			if (BindingTouch.SupportsXmlDocumentation) {
				print ($"/// <summary>Retrieves the <see cref=\"global::{backingFieldType.FullName}\" /> constant that describes <paramref name=\"self\" />.</summary>");
				print ($"/// <param name=\"self\">The instance on which this method operates.</param>");
			}
			print ("public static {2}{1}? GetConstant (this {0} self)", type.Name, backingFieldTypeName, isBackingFieldValueType ? "unsafe " : string.Empty);
			print ("{");
			indent++;
			if (isBackingFieldValueType) {
				print ($"{backingFieldTypeName}* ptr = null;");
			} else {
				print ("IntPtr ptr = IntPtr.Zero;");
			}
			// can be empty - and the C# compiler emit `warning CS1522: Empty switch block`
			if (fields.Count > 0) {
				print ("switch (({0}) self) {{", underlying_type);
				var default_symbol_name = default_symbol?.Item2.SymbolName;
				// more than one enum member can share the same numeric value - ref: #46285
				foreach (var kvp in fields) {
					print ("case {0}: // {1}.{2}", Convert.ToInt64 (kvp.Key.GetRawConstantValue ()), type.Name, kvp.Key.Name);
					var sn = kvp.Value.SymbolName;
					if (sn == default_symbol_name)
						print ("default:");
					indent++;
					print ("ptr = {0};", sn);
					print ("break;");
					indent--;
				}
				print ("}");
			}
			if (isBackingFieldValueType) {
				print ("if (ptr is null)");
				print ("\t return null;");
				print ("return *ptr;");
			} else {
				print ("return ({0}?) Runtime.GetNSObject (ptr);", backingFieldTypeName);
			}
			indent--;
			print ("}");

			print ("");

			var nullable = null_field is not null;
			if (BindingTouch.SupportsXmlDocumentation) {
				print ($"/// <summary>Retrieves the <see cref=\"global::{type.FullName}\" /> value named by <paramref name=\"constant\" />.</summary>");
				print ($"/// <param name=\"constant\">The name of the constant to retrieve.</param>");
			}
			print ("public static {3}{0} GetValue ({2}{1} constant)", type.Name, nullable ? "?" : "", backingFieldTypeName, isBackingFieldValueType ? "unsafe " : string.Empty);
			print ("{");
			indent++;
			if (!(isBackingFieldValueType && !nullable)) {
				print ("if (constant is null)");
				indent++;
				// if we do not have a enum value that maps to a null field then we throw
				if (!nullable)
					print ("throw new ArgumentNullException (nameof (constant));");
				else
					print ("return {0}.{1};", type.Name, null_field.Item1.Name);
				indent--;
			}
			foreach (var kvp in fields) {
				if (isBackingFieldValueType) {
					print ($"if (constant == *{kvp.Value.SymbolName})");
				} else {
					print ("if (constant.IsEqualTo ({0}))", kvp.Value.SymbolName);
				}
				indent++;
				print ("return {0}.{1};", type.Name, kvp.Key.Name);
				indent--;
			}
			// if there's no default then we throw on unknown constants
			if (default_symbol is null)
				print ("throw new NotSupportedException ($\"{constant} has no associated enum value on this platform.\");");
			else
				print ("return {0}.{1};", type.Name, default_symbol.Item1.Name);
			indent--;
			print ("}");

			if (BindingTouch.SupportsXmlDocumentation) {
				print ($"/// <summary>Converts an array of <see cref=\"global::{type.FullName}\" /> enum values into an array of their corresponding constants.</summary>");
				print ($"/// <param name=\"values\">The array of enum values to convert.</param>");
			}
			print ($"internal static {backingFieldTypeName}?[]? ToConstantArray (this {type.Name}[]? values)");
			print ("{");
			indent++;
			print ("if (values is null)");
			print ("\treturn null;");
			print ($"var rv = new global::System.Collections.Generic.List<{backingFieldTypeName}?> ();");
			print ("for (var i = 0; i < values.Length; i++) {");
			indent++;
			print ("var value = values [i];");
			print ("rv.Add (value.GetConstant ());");
			indent--;
			print ("}");
			print ("return rv.ToArray ();");
			indent--;
			print ("}");
			print ("");
			if (BindingTouch.SupportsXmlDocumentation) {
				print ($"/// <summary>Converts an array of <see cref=\"{backingFieldTypeName}\" /> values into an array of their corresponding enum values.</summary>");
				print ($"/// <param name=\"values\">The array if <see cref=\"{backingFieldTypeName}\" /> values to convert.</param>");
			}
			print ($"internal static {type.Name}[]? ToEnumArray (this {backingFieldTypeName}[]? values)");
			print ("{");
			indent++;
			print ("if (values is null)");
			print ("\treturn null;");
			print ($"var rv = new global::System.Collections.Generic.List<{type.Name}> ();");
			print ("for (var i = 0; i < values.Length; i++) {");
			indent++;
			print ("var value = values [i];");
			print ("rv.Add (GetValue (value));");
			indent--;
			print ("}");
			print ("return rv.ToArray ();");
			indent--;
			print ("}");
			print ("");

			if (isFlagsEnum) {
				if (BindingTouch.SupportsXmlDocumentation) {
					print ($"/// <summary>Retrieves all the <see cref=\"global::{type.FullName}\" /> constants named by the flags <paramref name=\"value\" />.</summary>");
					print ($"/// <param name=\"value\">The flags to retrieve</param>");
					print ($"/// <remarks>Any flags that are not recognized will be ignored.</remarks>");
				}
				print ($"public static {backingFieldTypeName}[] ToArray (this {type.Name} value)");
				print ("{");
				indent++;
				print ($"var rv = new global::System.Collections.Generic.List<{backingFieldTypeName}> ();");
				foreach (var kvp in fields) {
					print ($"if (value.HasFlag ({type.Name}.{kvp.Key.Name}) && {kvp.Value.SymbolName} != IntPtr.Zero)");
					indent++;
					print ($"rv.Add (({backingFieldTypeName}) Runtime.GetNSObject ({kvp.Value.SymbolName})!);");
					indent--;
				}
				print ($"// In order to be forward-compatible, any unknown values are ignored.");
				print ("return rv.ToArray ();");
				indent--;
				print ("}");
				print ("");

				print ($"public static {type.Name} ToFlags (global::System.Collections.Generic.IEnumerable<{backingFieldTypeName}{(nullable ? "?" : "")}>{(nullable ? "?" : "")} constants)");
				print ("{");
				indent++;
				print ($"var rv = default ({type.Name});");
				print ($"if (constants is null)");
				indent++;
				print ("return rv;");
				indent--;
				print ($"foreach (var constant in constants) {{");
				indent++;
				var first = true;
				if (nullable) {
					print ("if (constant is null)");
					indent++;
					print ("continue;");
					indent--;
					first = false;
				}
				foreach (var kvp in fields) {
					print ($"{(first ? "if" : "else if")} (constant.IsEqualTo ({kvp.Value.SymbolName}))");
					first = false;
					indent++;
					print ($"rv |= {type.Name}.{kvp.Key.Name};");
					indent--;
				}
				print ($"// In order to be forward-compatible, any unknown values are ignored.");
				indent--;
				print ("}");
				print ("return rv;");
				indent--;
				print ("}");
				print ("");
			}
		}

		if ((fields.Count > 0) || (error is not null) || (null_field is not null)) {
			indent--;
			print ("}");
		}

		// namespace closing (it's optional to use namespaces even if it's a bad practice, ref #35283)
		if (indent > 0) {
			indent--;
			print ("}");
		}
	}
}
