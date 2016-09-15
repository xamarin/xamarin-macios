// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

// If the enum is used to represent error code then this attribute can be used to
// generate an extension type that will return the associated error domain based
// on the field name (given as a parameter)
[AttributeUsage (AttributeTargets.Enum)]
public class ErrorDomainAttribute : Attribute {

	public ErrorDomainAttribute (string domain)
	{
		ErrorDomain = domain;
	}

	public string ErrorDomain { get; set; }
}

[AttributeUsage (AttributeTargets.Field)]
public class DefaultEnumValueAttribute : Attribute {

	public DefaultEnumValueAttribute ()
	{
	}
}

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

	// caller already:
	//	- setup the header and namespace
	//	- call/emit PrintPlatformAttributes on the type
	void GenerateEnum (Type type)
	{
		if (HasAttribute (type, typeof (FlagsAttribute)))
			print ("[Flags]");

		var native = GetAttribute<NativeAttribute> (type);
		if (native != null) {
			if (String.IsNullOrEmpty (native.NativeName))
				print ("[Native]");
			else
				print ("[Native (\"{0}\")]", native.NativeName);
		}

		var fields = new Dictionary<FieldInfo, FieldAttribute> ();
		Tuple<FieldInfo, FieldAttribute> null_field = null;
		Tuple<FieldInfo, FieldAttribute> default_symbol = null;
		print ("public enum {0} : {1} {{", type.Name, GetCSharpTypeName (Enum.GetUnderlyingType (type)));
		indent++;
		foreach (var f in type.GetFields ()) {
			// skip value__ field 
			if (f.IsSpecialName)
				continue;
			PrintPlatformAttributes (f);
			print ("{0} = {1},", f.Name, f.GetRawConstantValue ());
			var fa = GetAttribute<FieldAttribute> (f);
			if (fa == null)
				continue;
			if (f.IsUnavailable ())
				continue;
			if (fa.SymbolName == null)
				null_field = new Tuple<FieldInfo, FieldAttribute> (f, fa);
			else
				fields.Add (f, fa);
			if (GetAttribute<DefaultEnumValueAttribute> (f) != null) {
				if (default_symbol != null)
					throw new BindingException (1045, true, $"Only a single [DefaultEnumValue] attribute can be used inside enum {type.Name}.");
				default_symbol = new Tuple<FieldInfo, FieldAttribute> (f, fa);
			}
		}
		indent--;
		print ("}");

		var library_name = type.Namespace;
		var error = GetAttribute<ErrorDomainAttribute> (type);
		if ((fields.Count > 0) || (error != null)) {
			print ("");
			// the *Extensions has the same version requirement as the enum itself
			PrintPlatformAttributes (type);
			print ("[CompilerGenerated]");
			print ("static public partial class {0}Extensions {{", type.Name);
			indent++;

			// note: not every binding namespace will start with ns.Prefix (e.g. MonoTouch.)
			if (!String.IsNullOrEmpty (ns.Prefix) && library_name.StartsWith (ns.Prefix))
				library_name = library_name.Substring (ns.Prefix.Length + 1);

			// there might not be any other fields in the framework
			if (!libraries.ContainsKey (library_name))
				libraries.Add (library_name, null);
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
			foreach (var kvp in fields) {
				var f = kvp.Key;
				var fa = kvp.Value;
				print ("[Field (\"{0}\", \"{1}\")]", fa.SymbolName, library_name);
				print ("static NSString _{0};", fa.SymbolName);
				print ("");
				// the attributes (availability and field) are important for our tests
				PrintPlatformAttributes (f);
				print ("internal static NSString {0} {{", fa.SymbolName);
				indent++;
				print ("get {");
				indent++;
				print ("if (_{0} == null)", fa.SymbolName);
				indent++;
				print ("_{0} = Dlfcn.GetStringConstant (Libraries.{1}.Handle, \"{0}\");", fa.SymbolName, library_name);
				indent--;
				print ("return _{0};", fa.SymbolName);
				indent--;
				print ("}");
				indent--;
				print ("}");
				print ("");
			}
			
			print ("public static NSString GetConstant (this {0} self)", type.Name);
			print ("{");
			indent++;
			print ("switch (self) {");
			var default_symbol_name = default_symbol?.Item2.SymbolName;
			foreach (var kvp in fields) {
				print ("case {0}.{1}:", type.Name, kvp.Key.Name);
				var sn = kvp.Value.SymbolName;
				if (sn == default_symbol_name)
					print ("default:");
				indent++;
				print ("return {0};", sn);
				indent--;
			}
			print ("}");
			if (default_symbol_name == null) {
				// note: a `[Field (null)]` does not need extra code
				print ("return null;");
			}
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
				print ("else if (constant == {0})", kvp.Value.SymbolName);
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