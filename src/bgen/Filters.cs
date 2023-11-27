// Copyright 2015 Xamarin Inc. All rights reserved.
// Copyright Microsoft Corp.
using System;
using System.Collections.Generic;
using System.Reflection;
using Foundation;
using ObjCRuntime;

#nullable enable

public partial class Generator {
	readonly List<string> filters = new ();

	static string GetVisibility (MethodAttributes attributes)
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

		// filters are now exposed as protocols so we need to conform to them
		var interfaces = String.Empty;
		foreach (var i in type.GetInterfaces ()) {
			interfaces += $", I{i.Name}";
		}

		// type declaration
		print ("public{0} partial class {1} : {2}{3} {{",
			is_abstract ? " abstract" : String.Empty,
			type_name, base_name, interfaces);
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
			if (BindingTouch.IsDotNet) {
				intptrctor_visibility = MethodAttributes.FamORAssem;
			} else if (CurrentPlatform.GetXamcoreVersion () >= 3) {
				intptrctor_visibility = MethodAttributes.FamORAssem;
			} else {
				intptrctor_visibility = MethodAttributes.Public;
			}
		}
		print_generated_code ();
		print ("{0}{1} ({2} handle) : base (handle)", GetVisibility (intptrctor_visibility), type_name, NativeHandleType);
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
		print ("if (coder is null)");
		indent++;
		print ("throw new ArgumentNullException (nameof (coder));");
		indent--;
		print ("{0} h;", NativeHandleType);
		print ("if (IsDirectBinding) {");
		indent++;
		print ("h = global::ObjCRuntime.Messaging.{0}_objc_msgSend_{0} (this.Handle, Selector.GetHandle (\"initWithCoder:\"), coder.Handle);", NativeHandleType);
		indent--;
		print ("} else {");
		indent++;
		print ("h = global::ObjCRuntime.Messaging.{0}_objc_msgSendSuper_{0} (this.SuperHandle, Selector.GetHandle (\"initWithCoder:\"), coder.Handle);", NativeHandleType);
		indent--;
		print ("}");
		print ("InitializeHandle (h, \"initWithCoder:\");");
		indent--;
		print ("}");
		print ("");

		// string constructor
		// default is protected (for abstract)
		v = GetVisibility (filter.StringCtorVisibility);
		if (is_abstract && (v.Length == 0))
			v = "protected ";
		if (v.Length > 0) {
			print_generated_code ();
			print ("{0} {1} (string name) : base (CreateFilter (name))", v, type_name);
			PrintEmptyBody ();
		}

		// properties
		GenerateProperties (type, type);

		// protocols
		GenerateProtocolProperties (type, type, new HashSet<string> ());

		indent--;
		print ("}");

		// namespace closing (it's optional to use namespaces even if it's a bad practice, ref #35283)
		if (indent > 0) {
			indent--;
			print ("}");
		}
	}

	void GenerateProtocolProperties (Type type, Type originalType, HashSet<string> processed)
	{
		foreach (var i in type.GetInterfaces ()) {
			if (!IsProtocolInterface (i, false, out var protocol))
				continue;

			// the same protocol can be included more than once (interfaces) - but we must generate only once
			var pname = i.Name;
			if (processed.Contains (pname))
				continue;
			processed.Add (pname);

			print ("");
			print ($"// {pname} protocol members ");
			GenerateProperties (i, originalType, fromProtocol: true);

			// also include base interfaces/protocols
			GenerateProtocolProperties (i, originalType, processed);
		}
	}

	void GenerateProperties (Type type, Type? originalType = null, bool fromProtocol = false)
	{
		foreach (var p in type.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
			if (p.IsUnavailable (this))
				continue;
			if (AttributeManager.HasAttribute<StaticAttribute> (p))
				continue;

			print ("");

			// an export will be present (only) if it's defined in a protocol
			var export = AttributeManager.GetCustomAttribute<ExportAttribute> (p);

			// this is a bit special since CoreImage filter protocols are much newer than the our generated, key-based bindings
			// so we do not want to advertise the protocol versions since most properties would be incorrectly advertised
			PrintPropertyAttributes (p, originalType, skipTypeInjection: export is not null);
			print_generated_code ();

			var ptype = p.PropertyType.Name;
			var nullable = false;
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
			// adding `using ImageIO;` would lead to `error CS0104: 'CGImageProperties' is an ambiguous reference between 'CoreGraphics.CGImageProperties' and 'ImageIO.CGImageProperties'`
			case "CGImageMetadata":
				ptype = "ImageIO.CGImageMetadata";
				break;
			case "CIVector":
			case "CIColor":
			case "CIImage":
				// protocol-based bindings have annotations - but the older, key-based, versions did not
				if (!fromProtocol)
					nullable = true;
				break;
			}
			if (AttributeManager.HasAttribute<NullAllowedAttribute> (p))
				nullable = true;
			print ("public {0}{1} {2} {{", ptype, nullable ? "?" : "", p.Name);
			indent++;

			var name = AttributeManager.GetCustomAttribute<CoreImageFilterPropertyAttribute> (p)?.Name;
			// we can skip the name when it's identical to a protocol selector
			if (name is null) {
				if (export is null)
					throw new BindingException (1074, true, type.Name, p.Name);

				if (export.Selector is null)
					throw new BindingException (1082, true, type.Name, p.Name);

				var sel = export.Selector!;
				if (sel.StartsWith ("input", StringComparison.Ordinal))
					name = sel;
				else
					name = "input" + sel.Capitalize ();
			}

			if (p.GetGetMethod () is not null) {
				PrintFilterExport (p, export, setter: false);
				GenerateFilterGetter (ptype, name);
			}
			if (p.GetSetMethod () is not null) {
				PrintFilterExport (p, export, setter: true);
				GenerateFilterSetter (ptype, name);
			}

			indent--;
			print ("}");
		}
	}

	void PrintFilterExport (PropertyInfo p, ExportAttribute? export, bool setter)
	{
		if (export is null)
			return;

		var selector = export.Selector!;
		if (setter)
			selector = "set" + selector!.Capitalize () + ":";

		if (export.ArgumentSemantic != ArgumentSemantic.None && !p.PropertyType.IsPrimitive)
			print ($"[Export (\"{selector}\", ArgumentSemantic.{export.ArgumentSemantic})]");
		else
			print ($"[Export (\"{selector}\")]");
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
			print ("if (nsv is not null)");
			indent++;
			print ("return nsv.CGAffineTransformValue;");
			indent--;
			print ("return CGAffineTransform.MakeIdentity ();");
			break;
		// NSObject should not be added
		// NSNumber should not be added - it should be bound as a float (common), int32 or bool
		case "CGColorSpace":
		case "CGImage":
		case "ImageIO.CGImageMetadata":
		case "CIBarcodeDescriptor":
			print ("return Runtime.GetINativeObject <{0}> (GetHandle (\"{1}\"), false)!;", propertyType, propertyName);
			break;
		case "AVCameraCalibrationData":
		case "MLModel":
		case "NSAttributedString":
		case "NSData":
			print ("return Runtime.GetNSObject <{0}> (GetHandle (\"{1}\"), false)!;", propertyType, propertyName);
			break;
		case "CIColor":
		case "CIImage":
		case "CIVector":
			// `!` -> assume the method/property signature is correct (since it's based on headers)
			print ($"return (ValueForKey (\"{propertyName}\") as {propertyType})!;");
			break;
		case "CGPoint":
			print ("return GetPoint (\"{0}\");", propertyName);
			break;
		case "CGRect":
			print ("return GetRect (\"{0}\");", propertyName);
			break;
		case "float":
			print ("return GetFloat (\"{0}\");", propertyName);
			break;
		case "int":
			print ("return GetInt (\"{0}\");", propertyName);
			break;
#if NET
		case "IntPtr":
#else
		case "nint":
#endif
			print ("return GetNInt (\"{0}\");", propertyName);
			break;
#if NET
		case "UIntPtr":
#else
		case "nuint":
#endif
			print ("return GetNUInt (\"{0}\");", propertyName);
			break;
		case "string":
			// NSString should not be added - it should be bound as a string
			print ($"var handle = GetHandle (\"{propertyName}\");");
			print ("return CFString.FromHandle (handle)!;");
			break;
		case "CIVector[]":
			print ($"var handle = GetHandle (\"{propertyName}\");");
			print ("return CFArray.ArrayFromHandle<CIVector> (handle)!;");
			break;
		default:
			throw new BindingException (1075, true, propertyType);
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
#if NET
		case "IntPtr":
#else
		case "nint":
#endif
			print ("SetNInt (\"{0}\", value);", propertyName);
			break;
#if NET
		case "UIntPtr":
#else
		case "nuint":
#endif
			print ("SetNUInt (\"{0}\", value);", propertyName);
			break;
		// NSObject should not be added
		case "AVCameraCalibrationData":
		case "CGColorSpace":
		case "CIBarcodeDescriptor":
		case "CGImage":
		case "ImageIO.CGImageMetadata":
			print ($"SetHandle (\"{propertyName}\", value.GetHandle ());");
			break;
		case "CGPoint":
		case "CGRect":
		case "CIColor":
		case "CIImage":
		case "CIVector":
		case "MLModel":
		case "NSAttributedString":
		case "NSData":
			// NSNumber should not be added - it should be bound as a int or a float
			print ("SetValue (\"{0}\", value);", propertyName);
			break;
		case "string":
			// NSString should not be added - it should be bound as a string
			print ($"SetString (\"{propertyName}\", value);");
			break;
		case "CIVector[]":
			print ("if (value is null) {");
			indent++;
			print ($"SetHandle (\"{propertyName}\", IntPtr.Zero);");
			indent--;
			print ("} else {");
			indent++;
			print ("var ptr = CFArray.Create (value);");
			print ($"SetHandle (\"{propertyName}\", ptr);");
			print ($"CFObject.CFRelease (ptr);");
			indent--;
			print ("}");
			break;
		default:
			throw new BindingException (1075, true, propertyType);
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
