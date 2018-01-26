// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using IKVM.Reflection;
using Type = IKVM.Reflection.Type;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using System.IO;

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

	void CopyObsolete (ICustomAttributeProvider provider)
	{
		foreach (var oa in AttributeManager.GetCustomAttributes<ObsoleteAttribute> (provider))
			print ("[Obsolete (\"{0}\", {1})]", oa.Message, oa.IsError ? "true" : "false");
	}

	// caller already:
	//	- setup the header and namespace
	//	- call/emit PrintPlatformAttributes on the type
	void GenerateEnum (Type type)
	{
		if (AttributeManager.HasAttribute<FlagsAttribute> (type))
			print ("[Flags]");

		var native = AttributeManager.GetCustomAttribute<NativeAttribute> (type);
		if (native != null) {
			if (String.IsNullOrEmpty (native.NativeName))
				print ("[Native]");
			else
				print ("[Native (\"{0}\")]", native.NativeName);
		}
		CopyObsolete (type);

		var unique_constants = new HashSet<string> ();
		var fields = new Dictionary<FieldInfo, FieldAttribute> ();
		Tuple<FieldInfo, FieldAttribute> null_field = null;
		Tuple<FieldInfo, FieldAttribute> default_symbol = null;
		var underlying_type = GetCSharpTypeName (TypeManager.GetUnderlyingEnumType (type));
		print ("public enum {0} : {1} {{", type.Name, underlying_type);
		indent++;
		foreach (var f in type.GetFields ()) {
			// skip value__ field 
			if (f.IsSpecialName)
				continue;
			PrintPlatformAttributes (f);
			CopyObsolete (f);
			print ("{0} = {1},", f.Name, f.GetRawConstantValue ());
			var fa = AttributeManager.GetCustomAttribute<FieldAttribute> (f);
			if (fa == null)
				continue;
			if (f.IsUnavailable ())
				continue;
			if (fa.SymbolName == null)
				null_field = new Tuple<FieldInfo, FieldAttribute> (f, fa);
			else if (unique_constants.Contains (fa.SymbolName))
				throw new BindingException (1046, true, $"The [Field] constant {fa.SymbolName} cannot only be used once inside enum {type.Name}.");
			else {
				fields.Add (f, fa);
				unique_constants.Add (fa.SymbolName);
			}
			if (AttributeManager.GetCustomAttribute<DefaultEnumValueAttribute> (f) != null) {
				if (default_symbol != null)
					throw new BindingException (1045, true, $"Only a single [DefaultEnumValue] attribute can be used inside enum {type.Name}.");
				default_symbol = new Tuple<FieldInfo, FieldAttribute> (f, fa);
			}
		}
		indent--;
		print ("}");
		unique_constants.Clear ();

		var library_name = type.Namespace;
		var error = AttributeManager.GetCustomAttribute<ErrorDomainAttribute> (type);
		if ((fields.Count > 0) || (error != null)) {
			print ("");
			// the *Extensions has the same version requirement as the enum itself
			PrintPlatformAttributes (type);
			print_generated_code ();
			print ("static public partial class {0}Extensions {{", type.Name);
			indent++;

			var field = fields.FirstOrDefault ();
			var fieldAttr = field.Value;

			ComputeLibraryName (fieldAttr, type, field.Key?.Name, out library_name, out string library_path);
		}

		if (error != null) {
			// this attribute is important for our tests
			print ("[Field (\"{0}\", \"{1}\")]", error.ErrorDomain, library_name);
			print ("static NSString _domain;");
			print ("");
			print ("public static NSString GetDomain (this {0} self)", type.Name);
			print ("{");
			indent++;
			print ("if (_domain == null)");
			indent++;
			print ("_domain = Dlfcn.GetStringConstant (Libraries.{0}.Handle, \"{1}\");", library_name, error.ErrorDomain);
			indent--;
			print ("return _domain;");
			indent--;
			print ("}");
		}

		if (fields.Count > 0) {
			print ("static IntPtr[] values = new IntPtr [{0}];", fields.Count);
			print ("");

			int n = 0;
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
				bool useFieldAttrLibName = libname != null && !string.Equals (libname, library_name, StringComparison.OrdinalIgnoreCase);
				print ("[Field (\"{0}\", \"{1}\")]", fa.SymbolName, useFieldAttrLibName ? libname : libPath ?? library_name);
				print ("internal unsafe static IntPtr {0} {{", fa.SymbolName);
				indent++;
				print ("get {");
				indent++;
				print ("fixed (IntPtr *storage = &values [{0}])", n++);
				indent++;
				print ("return Dlfcn.CachePointer (Libraries.{0}.Handle, \"{1}\", storage);", useFieldAttrLibName ? libname : library_name, fa.SymbolName);
				indent--;
				indent--;
				print ("}");
				indent--;
				print ("}");
				print ("");
			}
			
			print ("public static NSString GetConstant (this {0} self)", type.Name);
			print ("{");
			indent++;
			print ("IntPtr ptr = IntPtr.Zero;");
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
			print ("return (NSString) Runtime.GetNSObject (ptr);");
			indent--;
			print ("}");
			
			print ("");
			
			print ("public static {0} GetValue (NSString constant)", type.Name);
			print ("{");
			indent++;
			print ("if (constant == null)");
			indent++;
			// if we do not have a enum value that maps to a null field then we throw
			if (null_field == null)
				print ("throw new ArgumentNullException (nameof (constant));");
			else
				print ("return {0}.{1};", type.Name, null_field.Item1.Name);
			indent--;
			foreach (var kvp in fields) {
				print ("if (constant.IsEqualTo ({0}))", kvp.Value.SymbolName);
				indent++;
				print ("return {0}.{1};", type.Name, kvp.Key.Name);
				indent--;
			}
			// if there's no default then we throw on unknown constants
			if (default_symbol == null)
				print ("throw new NotSupportedException (constant + \" has no associated enum value in \" + nameof ({0}) + \" on this platform.\");", type.Name);
			else
				print ("return {0}.{1};", type.Name, default_symbol.Item1.Name);
			indent--;
			print ("}");
		}
			
		if ((fields.Count > 0) || (error != null)) {
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