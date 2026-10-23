using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Assembly = System.Reflection.Assembly;

#nullable enable

public class TypeManager {
	public BindingTouch BindingTouch;
	BGenFrameworks Frameworks { get; }
	AttributeManager AttributeManager { get { return BindingTouch.AttributeManager; } }
	NamespaceCache NamespaceCache { get { return BindingTouch.NamespaceCache; } }
	TypeCache TypeCache { get { return BindingTouch.TypeCache; } }

	Dictionary<Type, string>? nsnumberReturnMap;
	Dictionary<Type, string>? nsnumberToValueMap;
	HashSet<string> typesThatMustAlwaysBeGloballyNamed = new ();

	public void SetTypesThatMustAlwaysBeGloballyNamed (Type [] types)
	{
		foreach (var t in types) {
			// The generator will create special *Appearance types (these are
			// nested classes). If we've bound a type with the same
			// *Appearance name, we can end up in a situation where the csc
			// compiler uses the the type we don't want due to C#'s resolution
			// rules - this happens if the bound *Appearance type is
			// referenced from the containing type of the special *Appearance
			// type. So always reference the bound *Appearance types using
			// global:: syntax.
			if (t.Name.EndsWith ("Appearance", StringComparison.Ordinal))
				typesThatMustAlwaysBeGloballyNamed.Add (t.Name);
		}
	}
	public Dictionary<Type, string> NSNumberReturnMap {
		get {
			if (nsnumberReturnMap is not null)
				return nsnumberReturnMap;
			Tuple<Type?, string> [] typeMap = {
				new (TypeCache.System_Boolean, ".BoolValue"),
				new (TypeCache.System_Byte, ".ByteValue"),
				new (TypeCache.System_Double, ".DoubleValue"),
				new (TypeCache.System_Float, ".FloatValue"),
				new (TypeCache.System_Int16, ".Int16Value"),
				new (TypeCache.System_Int32, ".Int32Value"),
				new (TypeCache.System_Int64, ".Int64Value"),
				new (TypeCache.System_SByte, ".SByteValue"),
				new (TypeCache.System_UInt16, ".UInt16Value"),
				new (TypeCache.System_UInt32, ".UInt32Value"),
				new (TypeCache.System_UInt64, ".UInt64Value"),
				new (TypeCache.System_nfloat, ".NFloatValue"),
				new (TypeCache.System_nint, ".NIntValue"),
				new (TypeCache.System_nuint, ".NUIntValue"),
			};
			nsnumberReturnMap = new ();
			foreach (var tuple in typeMap) {
				if (tuple.Item1 is not null)
					nsnumberReturnMap [tuple.Item1] = tuple.Item2;
			}
			return nsnumberReturnMap;
		}
	}

	/// <summary>
	/// Maps the NSNumber types to the corresponding ToValue function. For example
	/// int => "ToInt32"
	/// </summary>
	public Dictionary<Type, string> NSNumberToValueMap {
		get {
			if (nsnumberToValueMap is not null)
				return nsnumberToValueMap;
			Tuple<Type?, string> [] typeMap = {
				new (TypeCache.System_Boolean, "ToBool"),
				new (TypeCache.System_Byte, "ToByte"),
				new (TypeCache.System_Double, "ToDouble"),
				new (TypeCache.System_Float, "ToFloat"),
				new (TypeCache.System_Int16, "ToInt16"),
				new (TypeCache.System_Int32, "ToInt32"),
				new (TypeCache.System_Int64, "ToInt64"),
				new (TypeCache.System_SByte, "ToSByte"),
				new (TypeCache.System_UInt16, "ToUInt16"),
				new (TypeCache.System_UInt32, "ToUInt32"),
				new (TypeCache.System_UInt64, "ToUInt64"),
				new (TypeCache.System_nfloat, "ToNFloat"),
				new (TypeCache.System_nint, "ToNInt"),
				new (TypeCache.System_nuint, "ToNUInt"),
			};
			nsnumberToValueMap = new ();
			foreach (var tuple in typeMap) {
				if (tuple.Item1 is not null)
					nsnumberToValueMap [tuple.Item1] = tuple.Item2;
			}
			return nsnumberToValueMap;
		}
	}

	Dictionary<Type, string>? nsvalueReturnMap;
	public Dictionary<Type, string> NSValueReturnMap {
		get {
			if (nsvalueReturnMap is not null)
				return nsvalueReturnMap;
			Tuple<Type?, string> [] general = {
				new (TypeCache.CGAffineTransform, ".CGAffineTransformValue" ),
				new (TypeCache.NSRange, ".RangeValue" ),
				new (TypeCache.CGVector, ".CGVectorValue" ),
				new (TypeCache.SCNMatrix4, ".SCNMatrix4Value" ),
				new (TypeCache.CLLocationCoordinate2D, ".CoordinateValue" ),
				new (TypeCache.SCNVector3, ".Vector3Value" ),
				new (TypeCache.SCNVector4, ".Vector4Value" ),
				new (TypeCache.CoreGraphics_CGPoint, ".CGPointValue"),
				new (TypeCache.CoreGraphics_CGRect, ".CGRectValue"),
				new (TypeCache.CoreGraphics_CGSize, ".CGSizeValue"),
				new (TypeCache.MKCoordinateSpan, ".CoordinateSpanValue"),
			};

			Tuple<Type?, string> [] uiKitMap = Array.Empty<Tuple<Type?, string>> ();
			if (Frameworks.HaveUIKit)
				uiKitMap = new Tuple<Type?, string> [] {
					new (TypeCache.UIEdgeInsets, ".UIEdgeInsetsValue"),
					new (TypeCache.UIOffset, ".UIOffsetValue"),
					new (TypeCache.NSDirectionalEdgeInsets, ".DirectionalEdgeInsetsValue"),
				};

			Tuple<Type?, string> [] coreMedia = Array.Empty<Tuple<Type?, string>> ();
			if (Frameworks.HaveCoreMedia)
				coreMedia = new Tuple<Type?, string> [] {
					new (TypeCache.CMTimeRange, ".CMTimeRangeValue"),
					new (TypeCache.CMTime, ".CMTimeValue"),
					new (TypeCache.CMTimeMapping, ".CMTimeMappingValue"),
					new (TypeCache.CMVideoDimensions, ".CMVideoDimensionsValue"),
				};

			Tuple<Type?, string> [] animation = Array.Empty<Tuple<Type?, string>> ();
			if (Frameworks.HaveCoreAnimation)
				animation = new Tuple<Type?, string> [] {
					new (TypeCache.CATransform3D, ".CATransform3DValue"),
				};

			nsvalueReturnMap = new ();
			foreach (var typeMap in new [] { general, uiKitMap, coreMedia, animation }) {
				foreach (var tuple in typeMap) {
					if (tuple.Item1 is not null)
						nsvalueReturnMap [tuple.Item1] = tuple.Item2;
				}
			}
			return nsvalueReturnMap;
		}
	}
	
#pragma warning disable format
	Dictionary<Type, string>? nsvalueBindAsMap;
	public Dictionary<Type, string> NSValueBindAsMap {
		get {
			if (nsvalueBindAsMap is not null)
				return nsvalueBindAsMap;
			Tuple<Type?, string> [] general = {
				new (TypeCache.CGAffineTransform, "ToCGAffineTransform"),
				new (TypeCache.NSRange, "ToNSRange"),
				new (TypeCache.CGVector, "ToCGVector"),
				new (TypeCache.SCNMatrix4, "ToSCNMatrix4"),
				new (TypeCache.CLLocationCoordinate2D, "ToCLLocationCoordinate2D"),
				new (TypeCache.SCNVector3, "ToSCNVector3"),
				new (TypeCache.SCNVector4, "ToSCNVector4"),
				new (TypeCache.CoreGraphics_CGPoint, "ToCGPoint"),
				new (TypeCache.CoreGraphics_CGRect, "ToCGRect"),
				new (TypeCache.CoreGraphics_CGSize, "ToCGSize"),
				new (TypeCache.MKCoordinateSpan, "ToMKCoordinateSpan"),
			};

			Tuple<Type?, string> [] uiKitMap = Array.Empty<Tuple<Type?, string>> ();
			if (Frameworks.HaveUIKit)
				uiKitMap = new Tuple<Type?, string> [] {
					new (TypeCache.UIEdgeInsets, "ToUIEdgeInsets"),
					new (TypeCache.UIOffset, "ToUIOffset"),
					new (TypeCache.NSDirectionalEdgeInsets, "ToNSDirectionalEdgeInsets"),
				};

			Tuple<Type?, string> [] coreMedia = Array.Empty<Tuple<Type?, string>> ();
			if (Frameworks.HaveCoreMedia)
				coreMedia = new Tuple<Type?, string> [] {
					new (TypeCache.CMTimeRange, "ToCMTimeRange"), 
					new (TypeCache.CMTime, "ToCMTime"),
					new (TypeCache.CMTimeMapping, "ToCMTimeMapping"),
					new (TypeCache.CMVideoDimensions, "ToCMVideoDimensions"),
				};

			Tuple<Type?, string> [] animation = Array.Empty<Tuple<Type?, string>> ();
			if (Frameworks.HaveCoreAnimation)
				animation = new Tuple<Type?, string> [] { new(TypeCache.CATransform3D, "ToCATransform3D"), };

			nsvalueBindAsMap = new();
			foreach (var typeMap in new [] { general, uiKitMap, coreMedia, animation }) {
				foreach (var tuple in typeMap) {
					if (tuple.Item1 is not null)
						nsvalueBindAsMap [tuple.Item1] = tuple.Item2;
				}
			}

			return nsvalueBindAsMap;
		}
	}
	
#pragma warning restore format

	public Type? GetUnderlyingNullableType (Type? type)
	{
		if (type is null)
			return null;

		if (!type.IsConstructedGenericType)
			return null;

		var gt = type.GetGenericTypeDefinition ();
		if (gt.IsNested)
			return null;

		if (gt.Namespace != "System")
			return null;

		if (gt.Name != "Nullable`1")
			return null;

		return type.GenericTypeArguments [0];
	}

	public TypeManager (BindingTouch bindingTouch)
	{
		if (bindingTouch.Frameworks is null)
			throw ErrorHelper.CreateError (3, bindingTouch.CurrentPlatform);

		BindingTouch = bindingTouch;
		Frameworks = bindingTouch.Frameworks;
	}

	public string PrimitiveType (Type t, bool formatted = false)
	{
		if (t == TypeCache.System_Void)
			return "void";
		if (t == TypeCache.System_Int32)
			return "int";
		if (t == TypeCache.System_Int16)
			return "short";
		if (t == TypeCache.System_Byte)
			return "byte";
		if (t == TypeCache.System_Float)
			return "float";
		if (t == TypeCache.System_Boolean)
			return "bool";
		if (t == TypeCache.System_Char)
			return "char";
		if (t == TypeCache.System_nfloat)
			return "nfloat";

		return formatted ? FormatType (null, t) : t.Name;
	}

	public string FormatType (Type? usedIn, Type? type)
	{
		return FormatTypeUsedIn (usedIn?.Namespace, type);
	}

	public string FormatType (Type? usedIn, Type? type, byte []? nullabilityBytes)
	{
		if (type is null) {
			throw new BindingException (1065, true);
		}
		if (nullabilityBytes is null || nullabilityBytes.Length == 0) {
			return FormatTypeUsedIn (usedIn?.Namespace, type);
		}

		// The byte array encodes nullability for the type tree in depth-first order.
		// Byte 0 is for the outer type itself (which the caller handles separately via
		// [NullAllowed] / IsNullable), so we skip it. The rest are for generic arguments.
		// A single-byte array means uniform nullability for all positions.
		var targs = type.GetGenericArguments ();
		if (targs.Length == 0 && !type.IsArray) {
			return FormatTypeUsedIn (usedIn?.Namespace, type);
		}

		// Start index: for single-byte (uniform), don't skip byte 0 since it applies everywhere.
		// For multi-byte, skip byte 0 (the outer type, handled by the caller via [NullAllowed]).
		int index = nullabilityBytes.Length == 1 ? 0 : 1;

		// For array types, format the element type with nullability. The array's own
		// nullability (byte 0) is handled by the caller via [NullAllowed], so we only
		// need to format the element type starting at index (which is past byte 0).
		if (type.IsArray) {
			var elementFormatted = FormatTypeUsedIn (usedIn?.Namespace, type.GetElementType (), nullabilityBytes, ref index);
			return elementFormatted + "[" + new string (',', type.GetArrayRank () - 1) + "]";
		}
		var formattedArgs = new string [targs.Length];
		for (int i = 0; i < targs.Length; i++) {
			formattedArgs [i] = FormatTypeUsedIn (usedIn?.Namespace, targs [i], nullabilityBytes, ref index);
		}

		// Get the outer type name without generic args
		var usedInNamespace = usedIn?.Namespace;
		string tname;
		var parentClass = (type.ReflectedType is null) ? String.Empty : type.ReflectedType.Name + ".";
		if (typesThatMustAlwaysBeGloballyNamed.Contains (type.Name)) {
			tname = $"global::{type.Namespace}.{parentClass}{type.Name}";
		} else if ((usedInNamespace is not null && type.Namespace == usedInNamespace) ||
				 BindingTouch.NamespaceCache.StandardNamespaces.Contains (type.Namespace ?? String.Empty) ||
				 string.IsNullOrEmpty (type.FullName)) {
			tname = type.Name;
		} else {
			tname = $"global::{type.Namespace}.{parentClass}{type.Name}";
		}

		return tname.RemoveArity () + "<" + string.Join (", ", formattedArgs) + ">";
	}

	public string FormatType (Type? usedIn, string? @namespace, string name)
	{
		string tname;
		if ((usedIn is not null && @namespace == usedIn.Namespace) || (@namespace is not null && BindingTouch.NamespaceCache.StandardNamespaces.Contains (@namespace)))
			tname = name;
		else
			tname = "global::" + @namespace + "." + name;

		return tname;
	}

	public string FormatTypeUsedIn (string? usedInNamespace, Type? type)
	{
		if (type is null)
			throw new BindingException (1065, true);
		if (type == TypeCache.System_Void)
			return "void";
		if (type == TypeCache.System_SByte)
			return "sbyte";
		if (type == TypeCache.System_Int32)
			return "int";
		if (type == TypeCache.System_Int16)
			return "short";
		if (type == TypeCache.System_Int64)
			return "long";
		if (type == TypeCache.System_Byte)
			return "byte";
		if (type == TypeCache.System_UInt16)
			return "ushort";
		if (type == TypeCache.System_UInt32)
			return "uint";
		if (type == TypeCache.System_UInt64)
			return "ulong";
		if (type == TypeCache.System_Byte)
			return "byte";
		if (type == TypeCache.System_Float)
			return "float";
		if (type == TypeCache.System_Double)
			return "double";
		if (type == TypeCache.System_Boolean)
			return "bool";
		if (type == TypeCache.System_String)
			return "string";
		if (type == TypeCache.System_nfloat)
			return "nfloat";
		if (type == TypeCache.System_nint)
			return "nint";
		if (type == TypeCache.System_nuint)
			return "nuint";
		if (type == TypeCache.System_Char)
			return "char";
		if (type == TypeCache.System_nfloat)
			return "nfloat";

		if (type.IsArray) {
			return FormatTypeUsedIn (usedInNamespace, type.GetElementType ()) + "[" + new string (',', type.GetArrayRank () - 1) + "]";
		}


		string tname;
		// we are adding the usage of ReflectedType just for those cases in which we have nested enums/classes, this soluction does not
		// work with nested/nested/nested classes. But we are not writing a general solution because:
		// 1. We have only encountered nested classes.
		// 2. We are not going to complicate the code more than needed if we have never ever faced a situation with a super complicated nested hierarchy, 
		//    so we only solve the problem we have, no more.
		var parentClass = (type.ReflectedType is null) ? String.Empty : type.ReflectedType.Name + ".";
		if (typesThatMustAlwaysBeGloballyNamed.Contains (type.Name))
			tname = $"global::{type.Namespace}.{parentClass}{type.Name}";
		else if ((usedInNamespace is not null && type.Namespace == usedInNamespace) ||
				 BindingTouch.NamespaceCache.StandardNamespaces.Contains (type.Namespace ?? String.Empty) ||
				 string.IsNullOrEmpty (type.FullName))
			tname = type.Name;
		else
			tname = $"global::{type.Namespace}.{parentClass}{type.Name}";

		var targs = type.GetGenericArguments ();
		if (targs.Length > 0) {
			var isNullable = GetUnderlyingNullableType (type) is not null;
			if (isNullable)
				return FormatTypeUsedIn (usedInNamespace, targs [0]) + "?";

			return tname.RemoveArity () + "<" + string.Join (", ", targs.Select (l => FormatTypeUsedIn (usedInNamespace, l)).ToArray ()) + ">";
		}

		return tname;
	}

	// Overload that consumes nullability bytes in depth-first order.
	// The byte array from [NullableAttribute] encodes nullability for each type position:
	//   0 = oblivious, 1 = not nullable, 2 = nullable
	// Value types do NOT consume bytes from the array (their nullability is structural).
	// The index tracks our position as we traverse the type tree depth-first.
	string FormatTypeUsedIn (string? usedInNamespace, Type? type, byte [] nullabilityBytes, ref int index)
	{
		if (type is null) {
			throw new BindingException (1065, true);
		}

		// Value types don't have bytes in the NullableAttribute array.
		// Their nullability is determined structurally (by being Nullable<T>).
		if (type.IsValueType) {
			if (type == TypeCache.System_SByte) {
				return "sbyte";
			}
			if (type == TypeCache.System_Int32) {
				return "int";
			}
			if (type == TypeCache.System_Int16) {
				return "short";
			}
			if (type == TypeCache.System_Int64) {
				return "long";
			}
			if (type == TypeCache.System_Byte) {
				return "byte";
			}
			if (type == TypeCache.System_UInt16) {
				return "ushort";
			}
			if (type == TypeCache.System_UInt32) {
				return "uint";
			}
			if (type == TypeCache.System_UInt64) {
				return "ulong";
			}
			if (type == TypeCache.System_Float) {
				return "float";
			}
			if (type == TypeCache.System_Double) {
				return "double";
			}
			if (type == TypeCache.System_Boolean) {
				return "bool";
			}
			if (type == TypeCache.System_nfloat) {
				return "nfloat";
			}
			if (type == TypeCache.System_nint) {
				return "nint";
			}
			if (type == TypeCache.System_nuint) {
				return "nuint";
			}
			if (type == TypeCache.System_Char) {
				return "char";
			}

			// Generic value types (e.g. Nullable<T>, KeyValuePair<K,V>)
			var vtargs = type.GetGenericArguments ();
			if (vtargs.Length > 0) {
				var isNullableValueType = GetUnderlyingNullableType (type) is not null;
				if (isNullableValueType) {
					return FormatTypeUsedIn (usedInNamespace, vtargs [0], nullabilityBytes, ref index) + "?";
				}

				// Non-nullable generic value type: recurse into its args
				var formattedVtArgs = new string [vtargs.Length];
				for (int i = 0; i < vtargs.Length; i++) {
					formattedVtArgs [i] = FormatTypeUsedIn (usedInNamespace, vtargs [i], nullabilityBytes, ref index);
				}

				string vtname;
				var vtParentClass = (type.ReflectedType is null) ? String.Empty : type.ReflectedType.Name + ".";
				if (typesThatMustAlwaysBeGloballyNamed.Contains (type.Name)) {
					vtname = $"global::{type.Namespace}.{vtParentClass}{type.Name}";
				} else if ((usedInNamespace is not null && type.Namespace == usedInNamespace) ||
						 BindingTouch.NamespaceCache.StandardNamespaces.Contains (type.Namespace ?? String.Empty) ||
						 string.IsNullOrEmpty (type.FullName)) {
					vtname = type.Name;
				} else {
					vtname = $"global::{type.Namespace}.{vtParentClass}{type.Name}";
				}
				return vtname.RemoveArity () + "<" + string.Join (", ", formattedVtArgs) + ">";
			}

			// Non-generic value type: use the standard name
			string vname;
			var vParentClass = (type.ReflectedType is null) ? String.Empty : type.ReflectedType.Name + ".";
			if (typesThatMustAlwaysBeGloballyNamed.Contains (type.Name)) {
				vname = $"global::{type.Namespace}.{vParentClass}{type.Name}";
			} else if ((usedInNamespace is not null && type.Namespace == usedInNamespace) ||
					 BindingTouch.NamespaceCache.StandardNamespaces.Contains (type.Namespace ?? String.Empty) ||
					 string.IsNullOrEmpty (type.FullName)) {
				vname = type.Name;
			} else {
				vname = $"global::{type.Namespace}.{vParentClass}{type.Name}";
			}
			return vname;
		}

		// Reference types consume one byte from the array.
		// For single-byte (uniform) arrays, the same byte applies to all positions
		// without incrementing the index.
		byte currentByte;
		if (nullabilityBytes.Length == 1) {
			currentByte = nullabilityBytes [0];
		} else {
			currentByte = index < nullabilityBytes.Length ? nullabilityBytes [index] : (byte) 0;
			index++;
		}

		bool isCurrentNullable = currentByte == 2;

		// Simple reference types
		if (type == TypeCache.System_String) {
			return "string" + (isCurrentNullable ? "?" : "");
		}

		if (type.IsArray) {
			return FormatTypeUsedIn (usedInNamespace, type.GetElementType (), nullabilityBytes, ref index) + "[" + new string (',', type.GetArrayRank () - 1) + "]" + (isCurrentNullable ? "?" : "");
		}

		string tname;
		var parentClass = (type.ReflectedType is null) ? String.Empty : type.ReflectedType.Name + ".";
		if (typesThatMustAlwaysBeGloballyNamed.Contains (type.Name)) {
			tname = $"global::{type.Namespace}.{parentClass}{type.Name}";
		} else if ((usedInNamespace is not null && type.Namespace == usedInNamespace) ||
				 BindingTouch.NamespaceCache.StandardNamespaces.Contains (type.Namespace ?? String.Empty) ||
				 string.IsNullOrEmpty (type.FullName)) {
			tname = type.Name;
		} else {
			tname = $"global::{type.Namespace}.{parentClass}{type.Name}";
		}

		var targs = type.GetGenericArguments ();
		if (targs.Length > 0) {
			var formattedArgs = new string [targs.Length];
			for (int i = 0; i < targs.Length; i++) {
				formattedArgs [i] = FormatTypeUsedIn (usedInNamespace, targs [i], nullabilityBytes, ref index);
			}
			return tname.RemoveArity () + "<" + string.Join (", ", formattedArgs) + ">" + (isCurrentNullable ? "?" : "");
		}

		return tname + (isCurrentNullable ? "?" : "");
	}

	public string RenderType (Type t, ICustomAttributeProvider? provider = null)
	{
		var nullable = string.Empty;
		if (provider is not null && !t.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (provider))
			nullable = "?";

		if (!t.IsEnum) {
			switch (Type.GetTypeCode (t)) {
			case TypeCode.Char:
				return "char" + nullable;
			case TypeCode.String:
				return "string" + nullable;
			case TypeCode.Int32:
				return "int" + nullable;
			case TypeCode.UInt32:
				return "uint" + nullable;
			case TypeCode.Int64:
				return "long" + nullable;
			case TypeCode.UInt64:
				return "ulong" + nullable;
			case TypeCode.Single:
				return "float" + nullable;
			case TypeCode.Double:
				return "double" + nullable;
			case TypeCode.Decimal:
				return "decimal" + nullable;
			case TypeCode.SByte:
				return "sbyte" + nullable;
			case TypeCode.Byte:
				return "byte" + nullable;
			case TypeCode.Boolean:
				return "bool" + nullable;
			}
		}

		if (t == TypeCache.System_Void)
			return "void";

		if (t == TypeCache.System_IntPtr) {
			return (AttributeManager.HasNativeAttribute (provider) ? "nint" : "IntPtr") + nullable;
		} else if (t == TypeCache.System_UIntPtr) {
			return (AttributeManager.HasNativeAttribute (provider) ? "nuint" : "UIntPtr") + nullable;
		}

		if (t.Namespace is not null) {
			string ns = t.Namespace;
			var isImplicitNamespace = NamespaceCache.ImplicitNamespaces.Contains (ns);
			var isInMultipleNamespaces = IsInMultipleNamespaces (t);
			var nonGlobalCandidate = isImplicitNamespace && !isInMultipleNamespaces;
			if (nonGlobalCandidate || t.IsGenericType) {
				var targs = t.GetGenericArguments ();
				if (targs.Length == 0)
					return t.Name + nullable;
				return $"global::{t.Namespace}." + t.Name.RemoveArity () + "<" + string.Join (", ", targs.Select (l => FormatTypeUsedIn (null, l)).ToArray ()) + ">" + nullable;
			}
			if (isInMultipleNamespaces)
				return "global::" + t.FullName + nullable;
			if (NamespaceCache.NamespacesThatConflictWithTypes.Contains (ns))
				return "global::" + t.FullName + nullable;
			if (t.Name == t.Namespace)
				return "global::" + t.FullName + nullable;
			else
				return t.FullName + nullable;
		}

		return t.FullName + nullable;
	}

	bool IsInMultipleNamespaces (Type? type)
	{
		if (type is null)
			return false;

		if (NamespaceCache.TypesInMultipleNamespaces.Contains (type.Name))
			return true;

		return IsInMultipleNamespaces (type.GetElementType ());
	}

	// TODO: If we ever have an API with nested properties of the same name more than
	// 2 deep, we'll need to have this return a list of PropertyInfo and comb through them all.
	public PropertyInfo? GetParentTypeWithSameNamedProperty (BaseTypeAttribute? bta, string propertyName)
	{
		if (bta is null)
			return null;

		Type? currentType = bta.BaseType;
		while (currentType is not null && currentType != TypeCache.NSObject) {
			PropertyInfo? prop = currentType.GetProperty (propertyName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			if (prop is not null)
				return prop;
			currentType = currentType.BaseType;
		}
		return null;
	}

	public bool IsNativeType (Type pt)
	{
		return (pt == TypeCache.System_Int32 || pt == TypeCache.System_Int64 || pt == TypeCache.System_Byte || pt == TypeCache.System_Int16);
	}

	// Is this a wrapped type of NSObject from the MonoTouch/MonoMac binding world?
	public bool IsWrappedType (Type? t)
	{
		if (t is null)
			return false;
		if (t.IsInterface)
			return true;
		if (TypeCache.NSObject is not null)
			return t.IsSubclassOf (TypeCache.NSObject) || t == TypeCache.NSObject;
		return false;
	}

	public bool IsArrayOfWrappedType (Type t)
	{
		return t.IsArray && IsWrappedType (t.GetElementType ());
	}

	// Is this type something that derives from DictionaryContainerType (or an interface marked up with StrongDictionary)
	public bool IsDictionaryContainerType (Type t)
	{
		return t.IsSubclassOf (TypeCache.DictionaryContainerType) || (t.IsInterface && AttributeManager.HasAttribute<StrongDictionaryAttribute> (t));
	}

	public Api ParseApi (Assembly api, bool processEnums)
	{
		var types = new List<Type> ();
		var strongDictionaries = new List<Type> ();

		foreach (var t in api.GetTypes ()) {
			if ((processEnums && t.IsEnum) ||
				AttributeManager.HasAttribute<BaseTypeAttribute> (t) ||
				AttributeManager.HasAttribute<ProtocolAttribute> (t) ||
				AttributeManager.HasAttribute<StaticAttribute> (t) ||
				AttributeManager.HasAttribute<PartialAttribute> (t))
				// skip those types that are not available
				types.Add (t);
			if (AttributeManager.HasAttribute<StrongDictionaryAttribute> (t))
				strongDictionaries.Add (t);
		}

		// we should sort the types based on the full name
		var typesArray = types.ToArray ();
		Array.Sort (typesArray, (a, b) => string.CompareOrdinal (a.FullName, b.FullName));

		return new (typesArray, strongDictionaries.ToArray (), api);
	}
}
