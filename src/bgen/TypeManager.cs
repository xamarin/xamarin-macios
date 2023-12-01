using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation;

#nullable enable

public class TypeManager {
	public BindingTouch BindingTouch;
	Frameworks Frameworks { get; }
	AttributeManager AttributeManager { get { return BindingTouch.AttributeManager; } }
	NamespaceCache NamespaceCache { get { return BindingTouch.NamespaceCache; } }
	TypeCache TypeCache { get { return BindingTouch.TypeCache; } }

	Dictionary<Type, string>? nsnumberReturnMap;
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

	public Type? GetUnderlyingNullableType (Type type)
	{
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

	public string FormatType (Type? usedIn, Type type)
	{
		return FormatTypeUsedIn (usedIn?.Namespace, type);
	}

	public string FormatType (Type? usedIn, Type type, bool protocolized)
	{
		return FormatTypeUsedIn (usedIn?.Namespace, type, protocolized);
	}

	public string FormatType (Type? usedIn, string @namespace, string name)
	{
		string tname;
		if ((usedIn is not null && @namespace == usedIn.Namespace) || BindingTouch.NamespaceCache.StandardNamespaces.Contains (@namespace))
			tname = name;
		else
			tname = "global::" + @namespace + "." + name;

		return tname;
	}

	public string FormatTypeUsedIn (string? usedInNamespace, Type? type, bool protocolized = false)
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


		var interfaceTag = protocolized == true ? "I" : "";
		string tname;
		// we are adding the usage of ReflectedType just for those cases in which we have nested enums/classes, this soluction does not
		// work with nested/nested/nested classes. But we are not writing a general solution because:
		// 1. We have only encountered nested classes.
		// 2. We are not going to complicate the code more than needed if we have never ever faced a situation with a super complicated nested hierarchy, 
		//    so we only solve the problem we have, no more.
		var parentClass = (type.ReflectedType is null) ? String.Empty : type.ReflectedType.Name + ".";
		if (typesThatMustAlwaysBeGloballyNamed.Contains (type.Name))
			tname = $"global::{type.Namespace}.{parentClass}{interfaceTag}{type.Name}";
		else if ((usedInNamespace is not null && type.Namespace == usedInNamespace) ||
				 BindingTouch.NamespaceCache.StandardNamespaces.Contains (type.Namespace ?? String.Empty) ||
				 string.IsNullOrEmpty (type.FullName))
			tname = interfaceTag + type.Name;
		else
			tname = $"global::{type.Namespace}.{parentClass}{interfaceTag}{type.Name}";

		var targs = type.GetGenericArguments ();
		if (targs.Length > 0) {
			var isNullable = GetUnderlyingNullableType (type) is not null;
			if (isNullable)
				return FormatTypeUsedIn (usedInNamespace, targs [0]) + "?";

			return tname.RemoveArity () + "<" + string.Join (", ", targs.Select (l => FormatTypeUsedIn (usedInNamespace, l)).ToArray ()) + ">";
		}

		return tname;
	}

	public string? RenderType (Type t, ICustomAttributeProvider? provider = null)
	{
		if (!t.IsEnum) {
			switch (Type.GetTypeCode (t)) {
			case TypeCode.Char:
				return "char";
			case TypeCode.String:
				return "string";
			case TypeCode.Int32:
				return "int";
			case TypeCode.UInt32:
				return "uint";
			case TypeCode.Int64:
				return "long";
			case TypeCode.UInt64:
				return "ulong";
			case TypeCode.Single:
				return "float";
			case TypeCode.Double:
				return "double";
			case TypeCode.Decimal:
				return "decimal";
			case TypeCode.SByte:
				return "sbyte";
			case TypeCode.Byte:
				return "byte";
			case TypeCode.Boolean:
				return "bool";
			}
		}

		if (t == TypeCache.System_Void)
			return "void";

		if (t == TypeCache.System_IntPtr) {
			return AttributeManager.HasNativeAttribute (provider) ? "nint" : "IntPtr";
		} else if (t == TypeCache.System_UIntPtr) {
			return AttributeManager.HasNativeAttribute (provider) ? "nuint" : "UIntPtr";
		}

		if (t.Namespace is not null) {
			string ns = t.Namespace;
			if (NamespaceCache.ImplicitNamespaces.Contains (ns) || t.IsGenericType) {
				var targs = t.GetGenericArguments ();
				if (targs.Length == 0)
					return t.Name;
				return $"global::{t.Namespace}." + t.Name.RemoveArity () + "<" + string.Join (", ", targs.Select (l => FormatTypeUsedIn (null, l)).ToArray ()) + ">";
			}
			if (NamespaceCache.NamespacesThatConflictWithTypes.Contains (ns))
				return "global::" + t.FullName;
			if (t.Name == t.Namespace)
				return "global::" + t.FullName;
			else
				return t.FullName;
		}

		return t.FullName;
	}

	// TODO: If we ever have an API with nested properties of the same name more than
	// 2 deep, we'll need to have this return a list of PropertyInfo and comb through them all.
	public PropertyInfo? GetParentTypeWithSameNamedProperty (BaseTypeAttribute bta, string propertyName)
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

		return new (typesArray, strongDictionaries.ToArray ());
	}
}
