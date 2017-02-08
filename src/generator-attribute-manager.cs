using System;
using System.Reflection;

public static class AttributeManager
{
	public static System.Attribute GetCustomAttribute (ICustomAttributeProvider provider, Type type, bool inherit = false)
	{
		if (provider == null)
			return null;

		var pi = provider as ParameterInfo;
		if (pi != null)
			return Attribute.GetCustomAttribute (pi, type, inherit);
		var mi = provider as MemberInfo;
		if (mi != null)
			return Attribute.GetCustomAttribute (mi, type, inherit);
		var asm = provider as Assembly;
		if (asm != null)
			return Attribute.GetCustomAttribute (asm, type, inherit);
		throw new BindingException (1051, true, "Internal error: Don't know how to get attributes for {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", provider.GetType ().FullName);
	}

	public static T GetCustomAttribute <T> (ICustomAttributeProvider provider, bool inherit = false) where T: System.Attribute
	{
		return (T) GetCustomAttribute (provider, typeof (T), inherit);
	}

	public static T [] GetCustomAttributes<T> (ICustomAttributeProvider provider, bool inherits) where T : System.Attribute
	{
		return (T []) provider.GetCustomAttributes (typeof (T), inherits);
	}

	public static object [] GetCustomAttributes (ICustomAttributeProvider provider, Type type, bool inherits)
	{
		if (type == null)
			throw new System.ArgumentNullException (nameof (type));

		return (System.Attribute []) provider.GetCustomAttributes (type, inherits);
	}

	public static object [] GetCustomAttributes (ICustomAttributeProvider provider, bool inherits)
	{
		return provider.GetCustomAttributes (inherits);
	}

	public static bool HasAttribute<T> (ICustomAttributeProvider provider, bool inherit = false) where T : Attribute
	{
		return HasAttribute (provider, typeof (T), inherit);
	}

	public static bool HasAttribute (ICustomAttributeProvider provider, string type_name, bool inherit = false)
	{
		foreach (var attr in AttributeManager.GetCustomAttributes (provider, inherit)) {
			if (attr.GetType ().Name == type_name)
				return true;
		}

		return false;
	}

	public static bool HasAttribute (ICustomAttributeProvider provider, Type attribute_type, bool inherits = false)
	{
		var attribs = GetCustomAttributes (provider, attribute_type, inherits);
		if (attribs == null || attribs.Length == 0)
			return false;

		return true;
	}

	public static ICustomAttributeProvider GetReturnTypeCustomAttributes (MethodInfo method)
	{
		return method.ReturnTypeCustomAttributes;
	}
}
