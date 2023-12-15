using System;

#nullable enable

public static class TypeExtensions {

	public static bool IsSubclassOfNotNullable (this Type? type, Type? parent)
	{
		if (type is null || parent is null)
			return false;

		return type.IsSubclassOf (parent);
	}
}
