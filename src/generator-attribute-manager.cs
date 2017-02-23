using System;
#if IKVM
using IKVM.Reflection;
using Type = IKVM.Reflection.Type;
#else
using System.Reflection;
#endif

public static class AttributeManager
{
	public static T GetCustomAttribute<T> (ICustomAttributeProvider provider) where T: System.Attribute
	{
		if (provider == null)
			return null;

		var type = typeof (T);
		var pi = provider as ParameterInfo;
		if (pi != null)
			return (T) Attribute.GetCustomAttribute (pi, type);
		var mi = provider as MemberInfo;
		if (mi != null)
			return (T) Attribute.GetCustomAttribute (mi, type);
		var asm = provider as Assembly;
		if (asm != null)
			return (T) Attribute.GetCustomAttribute (asm, type);
		throw new BindingException (1051, true, "Internal error: Don't know how to get attributes for {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", provider.GetType ().FullName);
	}

	public static T [] GetCustomAttributes<T> (ICustomAttributeProvider provider) where T : System.Attribute
	{
		return (T []) provider.GetCustomAttributes (typeof (T), false);
	}

	public static bool HasAttribute<T> (ICustomAttributeProvider provider) where T : Attribute
	{
		var attribs = provider.GetCustomAttributes (typeof (T), false);
		if (attribs == null || attribs.Length == 0)
			return false;

		return true;
	}

	public static bool HasAttribute (ICustomAttributeProvider provider, string type_name)
	{
		foreach (var attr in provider.GetCustomAttributes (false)) {
			if (attr.GetType ().Name == type_name)
				return true;
		}

		return false;
	}

	public static ICustomAttributeProvider GetReturnTypeCustomAttributes (MethodInfo method)
	{
		return method.ReturnTypeCustomAttributes;
	}
}
