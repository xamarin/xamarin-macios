// Copyright 2015 Xamarin Inc. All rights reserved.
using System;
using System.Collections.Generic;
using IKVM.Reflection;
using Type = IKVM.Reflection.Type;

using Foundation;

public partial class Generator {

	List<string> filters = new List<string> ();

	string GetVisibility (MethodAttributes attributes)
	{
		if ((attributes & MethodAttributes.FamORAssem) == MethodAttributes.FamORAssem)
			return "protected internal ";
		if ((attributes & MethodAttributes.Public) == MethodAttributes.Public)
			return "public ";
		if ((attributes & MethodAttributes.Family) == MethodAttributes.Family)
			return "protected ";
		return String.Empty;
	}

	public void GenerateFilter (Type type)
	{
		var is_abstract = AttributeManager.HasAttribute<AbstractAttribute> (type);
		var filter = AttributeManager.GetCustomAttribute<CoreImageFilterAttribute> (type);
		var base_type = AttributeManager.GetCustomAttribute<BaseTypeAttribute> (type);
		var type_name = type.Name;
		var native_name = base_type.Name ?? type_name;
		var base_name = base_type.BaseType.Name;

		// internal static CIFilter FromName (string filterName, IntPtr handle)
		filters.Add (type_name);

		// type declaration
		print ("public{0} partial class {1} : {2} {{", 
			is_abstract ? " abstract" : String.Empty,
			type_name, base_name);
		print ("");
		indent++;

		// default constructor - if type is not abstract
		string v;
		if (!is_abstract) {
			v = GetVisibility (filter.DefaultCtorVisibility);
			if (v.Length > 0) {
				print_generated_code ();
				print ("{0}{1} () : base (\"{2}\")", v, type.Name, native_name);
				PrintEmptyBody ();
			}
		}

		// IntPtr constructor - always present
		var intptrctor_visibility = filter.IntPtrCtorVisibility;
		if (intptrctor_visibility == MethodAttributes.PrivateScope) {
			// since it was not generated code we never fixed the .ctor(IntPtr) visibility for unified
			if (Generator.XamcoreVersion >= 3) {
				intptrctor_visibility = MethodAttributes.FamORAssem;
			} else {
				intptrctor_visibility = MethodAttributes.Public;
			}
		}
		print_generated_code ();
		print ("{0}{1} (IntPtr handle) : base (handle)", GetVisibility (intptrctor_visibility), type_name);
		PrintEmptyBody ();

		// NSObjectFlag constructor - always present (needed to implement NSCoder for subclasses)
		print_generated_code ();
		print ("[EditorBrowsable (EditorBrowsableState.Advanced)]");
		print ("protected {0} (NSObjectFlag t) : base (t)", type_name);
		PrintEmptyBody ();

		// NSCoder constructor - all filters conforms to NSCoding
		print_generated_code ();
		print ("[EditorBrowsable (EditorBrowsableState.Advanced)]");
		print ("[Export (\"initWithCoder:\")]");
		print ("public {0} (NSCoder coder) : base (NSObjectFlag.Empty)", type_name);
		print ("{");
		indent++;
		print ("IntPtr h;");
		print ("if (IsDirectBinding) {");
		indent++;
		print ("h = global::{0}.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle (\"initWithCoder:\"), coder.Handle);", ns.CoreObjCRuntime);
		indent--;
		print ("} else {");
		indent++;
		print ("h = global::{0}.Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, Selector.GetHandle (\"initWithCoder:\"), coder.Handle);", ns.CoreObjCRuntime);
		indent--;
		print ("}");
		print ("InitializeHandle (h, \"initWithCoder:\");");
		indent--;
		print ("}");
		print ("");

		// string constructor
		// default is protected (for abstract) but backward compatibility (XAMCORE_2_0) requires some hacks
		v = GetVisibility (filter.StringCtorVisibility);
		if (is_abstract && (v.Length == 0))
			v = "protected ";
		if (v.Length > 0) {
			print_generated_code ();
			print ("{0} {1} (string name) : base (CreateFilter (name))", v, type_name);
			PrintEmptyBody ();
		}

		// properties
		foreach (var p in type.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
			if (p.IsUnavailable ())
				continue;
			
			print ("");
			print_generated_code ();
			var ptype = p.PropertyType.Name;
			// keep C# names as they are reserved keywords (e.g. Boolean also exists in OpenGL for Mac)
			switch (ptype) {
			case "Boolean":
				ptype = "bool";
				break;
			case "Int32":
				ptype = "int";
				break;
			case "Single":
				ptype = "float";
				break;
			case "String":
				ptype = "string";
				break;
			}
			print ("public {0} {1} {{", ptype, p.Name);
			indent++;

			var name = AttributeManager.GetCustomAttribute<CoreImageFilterPropertyAttribute> (p)?.Name;
			if (p.GetGetMethod () != null)
				GenerateFilterGetter (ptype, name);
			if (p.GetSetMethod () != null)
				GenerateFilterSetter (ptype, name);
			
			indent--;
			print ("}");
		}

		indent--;
		print ("}");

		// namespace closing (it's optional to use namespaces even if it's a bad practice, ref #35283)
		if (indent > 0) {
			indent--;
			print ("}");
		}
	}

	void GenerateFilterGetter (string propertyType, string propertyName)
	{
		print ("get {");
		indent++;
		switch (propertyType) {
		case "bool":
			print ("return GetBool (\"{0}\");", propertyName);
			break;
		// NSValue should not be added - the only case that is used (right now) if for CGAffineTransform
		case "CGAffineTransform":
			print ("var val = ValueForKey (\"{0}\");", propertyName);
			print ("var nsv = (val as NSValue);");
			print ("if (nsv != null)");
			indent++;
			print ("return nsv.CGAffineTransformValue;");
			indent--;
			print ("return new CGAffineTransform (1, 0, 0, 1, 0, 0);");
			break;
		// NSObject should not be added
		case "AVCameraCalibrationData":
		case "CGColorSpace":
		case "CIBarcodeDescriptor":
			print ("return Runtime.GetINativeObject <{0}> (GetHandle (\"{1}\"), false);", propertyType, propertyName);
			break;
		case "CIColor":
			print ("return GetColor (\"{0}\");", propertyName);
			break;
		case "CIImage":
			print ("return GetImage (\"{0}\");", propertyName);
			break;
		case "CIVector":
			print ("return GetVector (\"{0}\");", propertyName);
			break;
		case "float":
			print ("return GetFloat (\"{0}\");", propertyName);
			break;
		case "int":
			print ("return GetInt (\"{0}\");", propertyName);
			break;
		case "NSAttributedString":
		case "NSData":
			// NSNumber should not be added - it should be bound as a float (common), int32 or bool
			print ("return ValueForKey (\"{0}\") as {1};", propertyName, propertyType);
			break;
		case "string":
			// NSString should not be added - it should be bound as a string
			print ("return (string) (ValueForKey (\"{0}\") as NSString);", propertyName);
			break;
		default:
			throw new BindingException (1018, true, "Unimplemented CoreImage property type {0}", propertyType);
		}
		indent--;
		print ("}");
	}

	void GenerateFilterSetter (string propertyType, string propertyName)
	{
		print ("set {");
		indent++;
		switch (propertyType) {
		case "bool":
			print ("SetBool (\"{0}\", value);", propertyName);
			break;
		// NSValue should not be added - the only case that is used (right now) if for CGAffineTransform
		case "CGAffineTransform":
			print ("SetValue (\"{0}\", NSValue.FromCGAffineTransform (value));", propertyName);
			break;
		// NSNumber should not be added - it should be bound as a int or a float
		case "float":
			print ("SetFloat (\"{0}\", value);", propertyName);
			break;
		case "int":
			print ("SetInt (\"{0}\", value);", propertyName);
			break;
		// NSObject should not be added
		case "AVCameraCalibrationData":
		case "CGColorSpace":
		case "CIBarcodeDescriptor":
			print ("SetHandle (\"{0}\", value == null ? IntPtr.Zero : value.Handle);", propertyName);
			break;
		case "CIColor":
		case "CIImage":
		case "CIVector":
		case "NSAttributedString":
		case "NSData":
		// NSNumber should not be added - it should be bound as a int or a float
			print ("SetValue (\"{0}\", value);", propertyName);
			break;
		case "string":
			// NSString should not be added - it should be bound as a string
			print ("using (var ns = new NSString (value))");
			indent++;
			print ("SetValue (\"{0}\", ns);", propertyName);
			indent--;
			break;
		default:
			throw new BindingException (1018, true, "Unimplemented CoreImage property type {0}", propertyType);
		}
		indent--;
		print ("}");
	}

	void PrintEmptyBody ()
	{
		print ("{");
		print ("}");
		print ("");
	}
}
