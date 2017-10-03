using System;
using System.Collections.Generic;

using IKVM.Reflection;
using Type = IKVM.Reflection.Type;

public static class AttributeManager
{
	// This method gets the System.Type for a IKVM.Reflection.Type to a System.Type.
	// It knows about our mock attribute logic, so it will return the corresponding non-mocked System.Type for a mocked IKVM.Reflection.Type.
	static System.Type ConvertType (Type type, ICustomAttributeProvider provider)
	{
		System.Type rv;
		if (type.Assembly == TypeManager.CorlibAssembly) {
			rv = typeof (int).Assembly.GetType (type.FullName);
		} else if (type.Assembly == TypeManager.SystemAssembly) {
			rv = typeof (System.ComponentModel.EditorBrowsableAttribute).Assembly.GetType (type.FullName);
		} else if (type.Assembly == TypeManager.BindingAssembly) {
			// Types (attributes) in the binding assembly are mocked in the generator itself.
			rv = typeof (TypeManager).Assembly.GetType (type.FullName);
		} else if (type.Assembly == TypeManager.PlatformAssembly) {
			// Types (attributes) in the platform assemblies are mocked in the generator itself.

			switch (type.FullName) {
			case "ObjCRuntime.iOSAttribute":
			case "ObjCRuntime.LionAttribute":
			case "ObjCRuntime.AvailabilityAttribute":
			case "ObjCRuntime.MacAttribute":
			case "ObjCRuntime.SinceAttribute":
			case "ObjCRuntime.MountainLionAttribute":
			case "ObjCRuntime.MavericksAttribute":
				throw ErrorHelper.CreateError (1061, $"The attribute '{type.FullName}' found on '{Generator.FormatProvider (provider)}' is not a valid binding attribute. Please remove this attribute.");
			}

			var prefix = BindingTouch.NamespacePlatformPrefix;
			var n = type.FullName;
			if (!string.IsNullOrEmpty (prefix) && type.Namespace.StartsWith (prefix, System.StringComparison.Ordinal)) {
				n = "XamCore." + n.Substring (prefix.Length + 1);
			} else {
				n = "XamCore." + n;
			}
			rv = typeof (TypeManager).Assembly.GetType (n);
		} else {
			throw ErrorHelper.CreateError (1054, "Internal error: can't convert type '{0}' (unknown assembly). Please file a bug report (https://bugzilla.xamarin.com) with a test case.", type.AssemblyQualifiedName);
		}
		if (rv == null)
			throw ErrorHelper.CreateError (1055, "Internal error: failed to convert type '{0}'. Please file a bug report (https://bugzilla.xamarin.com) with a test case.", type.AssemblyQualifiedName);
		return rv;
	}

	// This method gets the IKVM.Reflection.Type for a System.Type.
	// It knows about our mock attribute logic, so it will return the mocked IKVM.Reflection.Type for a mocked System.Type.
	static Type ConvertType (System.Type type, ICustomAttributeProvider provider)
	{
		Type rv;
		if (type.Assembly == typeof (int).Assembly) {
			rv = TypeManager.CorlibAssembly.GetType (type.FullName);
		} else if (type.Assembly == typeof (System.ComponentModel.EditorBrowsableAttribute).Assembly) {
			rv = TypeManager.SystemAssembly.GetType (type.FullName);
		} else if (type.Assembly == typeof (TypeManager).Assembly) {
			// Types (attributes) in the generator are mocked types from
			// either the binding assembly or the platform assembly.
			rv = TypeManager.BindingAssembly.GetType (type.FullName);
			if (rv == null) {
				string fullname;
				if (type.Namespace?.StartsWith ("XamCore.", System.StringComparison.Ordinal) == true) {
					var prefix = BindingTouch.NamespacePlatformPrefix;
					if (!string.IsNullOrEmpty (prefix)) {
						fullname = prefix + "." + type.FullName.Substring (8);
					} else {
						fullname = type.FullName.Substring (8);
					}
				} else {
					fullname = type.FullName;
				}
				rv = TypeManager.PlatformAssembly.GetType (fullname);
			}
		} else {
			throw ErrorHelper.CreateError (1054, "Internal error: can't convert type '{0}' (unknown assembly). Please file a bug report (https://bugzilla.xamarin.com) with a test case.", type.AssemblyQualifiedName);
		}
		if (rv == null)
			throw ErrorHelper.CreateError (1055, "Internal error: failed to convert type '{0}'. Please file a bug report (https://bugzilla.xamarin.com) with a test case.", type.AssemblyQualifiedName);
		return rv;
	}

	static System.Attribute CreateAttributeInstance (CustomAttributeData attribute, ICustomAttributeProvider provider)
	{
		System.Type attribType = ConvertType (attribute.AttributeType, provider);

		var constructorArguments = new object [attribute.ConstructorArguments.Count];

		for (int i = 0; i < constructorArguments.Length; i++) {
			var value = attribute.ConstructorArguments [i].Value;
			switch (attribute.ConstructorArguments [i].ArgumentType.FullName) {
			case "System.Type":
				if (value != null) {
					if (attribType.Assembly == typeof (TypeManager).Assembly) {
						constructorArguments [i] = value;
					} else {
						constructorArguments [i] = System.Type.GetType (((Type) value).FullName);
					}
					if (constructorArguments [i] == null)
						throw ErrorHelper.CreateError (1056, "Internal error: failed to instantiate mock attribute '{0}' (could not convert type constructor argument #{1}). Please file a bug report (https://bugzilla.xamarin.com) with a test case.", attribType.FullName, i + 1);
				}
				break;
			default:
				constructorArguments [i] = value;
				break;
			}
		}

		var parameters = attribute.Constructor.GetParameters ();
		var ctorTypes = new System.Type [parameters.Length];
		for (int i = 0; i < ctorTypes.Length; i++) {
			var paramType = parameters [i].ParameterType;
			switch (paramType.FullName) {
			case "System.Type":
				if (attribType.Assembly == typeof (TypeManager).Assembly) {
					ctorTypes [i] = typeof (Type);
				} else {
					ctorTypes [i] = typeof (System.Type);
				}
				break;
			default:
				ctorTypes [i] = ConvertType (paramType, provider);
				break;
			}
			if (ctorTypes [i] == null)
				throw ErrorHelper.CreateError (1057, "Internal error: failed to instantiate mock attribute '{0}' (could not convert constructor type #{1} ({2})). Please file a bug report (https://bugzilla.xamarin.com) with a test case.", attribType.FullName, i, paramType.FullName);
		}
		var ctor = attribType.GetConstructor (ctorTypes);
		if (ctor == null)
			throw ErrorHelper.CreateError (1058, "Internal error: could not find a constructor for the mock attribute '{0}'. Please file a bug report (https://bugzilla.xamarin.com) with a test case.", attribType.FullName);
		var instance = ctor.Invoke (constructorArguments);

		for (int i = 0; i < attribute.NamedArguments.Count; i++) {
			var arg = attribute.NamedArguments [i];
			var value = arg.TypedValue.Value;
			if (arg.TypedValue.ArgumentType == TypeManager.System_String_Array) {
				var typed_values = (CustomAttributeTypedArgument []) arg.TypedValue.Value;
				var arr = new string [typed_values.Length];
				for (int a = 0; a < arr.Length; a++)
					arr [a] = (string) typed_values [a].Value;
				value = arr;
			} else if (arg.TypedValue.ArgumentType.FullName == "System.Type[]") {
				var typed_values = (CustomAttributeTypedArgument []) arg.TypedValue.Value;
				var arr = new Type [typed_values.Length];
				for (int a = 0; a < arr.Length; a++)
					arr [a] = (Type) typed_values [a].Value;
				value = arr;
			} else if (arg.TypedValue.ArgumentType.IsArray) {
				throw ErrorHelper.CreateError (1059, "Internal error: failed to instantiate mock attribute '{0}' (unknown type for the named argument #{1} ({2}). Please file a bug report (https://bugzilla.xamarin.com) with a test case.", attribType.FullName, i + 1, arg.MemberName);
			}
			if (arg.IsField) {
				attribType.GetField (arg.MemberName).SetValue (instance, value);
			} else {
				attribType.GetProperty (arg.MemberName).SetValue (instance, value, new object [0]);
			}
		}

		return (System.Attribute) instance;
	}

	static T [] FilterAttributes<T> (IList<CustomAttributeData> attributes, ICustomAttributeProvider provider) where T : System.Attribute
	{
		if (attributes == null || attributes.Count == 0)
			return Array.Empty<T> ();

		var type = ConvertType (typeof (T), provider);
		List<T> list = null;
		for (int i = 0; i < attributes.Count; i++) {
			var attrib = attributes [i];
			if (attrib.AttributeType != type && !IsSubclassOf (type, attrib.AttributeType))
				continue;

			if (list == null)
				list = new List<T> ();
			list.Add ((T) CreateAttributeInstance (attributes [i], provider));
		}

		if (list != null)
			return list.ToArray ();

		return Array.Empty<T> ();
	}

	public static T [] GetCustomAttributes<T> (ICustomAttributeProvider provider) where T : System.Attribute
	{
		return FilterAttributes<T> (GetIKVMAttributes (provider), provider);
	}

	static IList<CustomAttributeData> GetIKVMAttributes (ICustomAttributeProvider provider)
	{
		var member = provider as MemberInfo;
		if (member != null)
			return CustomAttributeData.GetCustomAttributes (member);
		var assembly = provider as Assembly;
		if (assembly != null)
			return CustomAttributeData.GetCustomAttributes (assembly);
		var pinfo = provider as ParameterInfo;
		if (pinfo != null)
			return CustomAttributeData.GetCustomAttributes (pinfo);
		var module = provider as Module;
		if (module != null)
			return CustomAttributeData.GetCustomAttributes (module);
		throw new BindingException (1051, true, "Internal error: Don't know how to get attributes for {0}. Please file a bug report (https://bugzilla.xamarin.com) with a test case.", provider.GetType ().FullName);
	}

	public static bool HasAttribute (ICustomAttributeProvider provider, string type_name)
	{
		var attribs = GetIKVMAttributes (provider);
		for (int i = 0; i < attribs.Count; i++)
			if (attribs [i].AttributeType.Name == type_name)
				return true;
		return false;
	}

	public static bool HasAttribute<T> (ICustomAttributeProvider provider) where T : Attribute
	{
		var attribute_type = ConvertType (typeof (T), provider);
		var attribs = GetIKVMAttributes (provider);
		if (attribs == null || attribs.Count == 0)
			return false;

		for (int i = 0; i < attribs.Count; i++) {
			var attrib = attribs [i];
			if (attrib.AttributeType == attribute_type)
				return true;
			if (IsSubclassOf (attribute_type, attrib.AttributeType))
				return true;
		}

		return false;
	}

	public static T GetCustomAttribute<T> (ICustomAttributeProvider provider) where T : System.Attribute
	{
		var rv = GetCustomAttributes<T> (provider);
		if (rv == null || rv.Length == 0)
			return null;

		if (rv.Length == 1)
			return rv [0];

		string name = (provider as MemberInfo)?.Name;
		if (provider is ParameterInfo) {
			var pi = (ParameterInfo) provider;
			name = $"the method {pi.Member.DeclaringType.FullName}.{pi.Member.Name}'s parameter #{pi.Position} ({pi.Name})";
		} else if (provider is MemberInfo) {
			var mi = (MemberInfo) provider;
			name = $"the member {mi.DeclaringType.FullName}.{mi.Name}";
		} else if (provider is Assembly) {
			name = $"the assembly {((Assembly) provider).FullName}";
		} else if (provider is Module) {
			name = $"the module {((Module) provider).FullyQualifiedName}";
		} else {
			name = $"the member {provider.ToString ()}";
		}
		throw ErrorHelper.CreateError (1059, "Found {0} {1} attributes on {1}. At most one was expected.", rv.Length, typeof (T).FullName, name);
	}

	public static ICustomAttributeProvider GetReturnTypeCustomAttributes (MethodInfo method)
	{
		return method.ReturnParameter;
	}

	static bool IsSubclassOf (Type base_class, Type derived_class)
	{
		return derived_class.IsSubclassOf (base_class);
	}
}
