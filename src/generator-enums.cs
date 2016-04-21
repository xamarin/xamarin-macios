// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
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
			fields.Add (f, fa);
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
			print ("static public class {0}Extensions {{", type.Name);
			indent++;

			// note: not every binding namespace will start with ns.Prefix (e.g. MonoTouch.)
			if (!String.IsNullOrEmpty (ns.Prefix) && library_name.StartsWith (ns.Prefix))
				library_name = library_name.Substring (ns.Prefix.Length + 1);

			// there might not be any other fields in the framework
			if (!libraries.Contains (library_name))
				libraries.Add (library_name);
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
				// the attributes (availability and field) are important for our tests
				PrintPlatformAttributes (f);
				print ("[Field (\"{0}\", \"{1}\")]", fa.SymbolName, library_name);
				print ("static NSString _{0};", fa.SymbolName);
				print ("");
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
			foreach (var kvp in fields) {
				print ("if (self == {0}.{1})", type.Name, kvp.Key.Name);
				indent++;
				print ("return {0};", kvp.Value.SymbolName);
				indent--;
			}
			print ("return null;");
			indent--;
			print ("}");
			
			print ("");
			
			print ("public static {0} GetValue (NSString constant)", type.Name);
			print ("{");
			indent++;
			print ("if (constant == null)");
			indent++;
			print ("throw new ArgumentNullException (nameof (constant));");
			indent--;
			foreach (var kvp in fields) {
				print ("else if (constant == {0})", kvp.Value.SymbolName);
				indent++;
				print ("return {0}.{1};", type.Name, kvp.Key.Name);
				indent--;
			}
			print ("throw new NotSupportedException (constant + \" has no associated enum value in \" + nameof ({0}) + \" on this platform.\");", type.Name);
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