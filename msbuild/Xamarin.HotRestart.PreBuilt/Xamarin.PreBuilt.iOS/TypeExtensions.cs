using System;
using System.Linq;

namespace Xamarin.iOS.HotRestart {
	public static class TypeExtensions {
		public static bool HasBaseType (this Type type, string fullName)
		{
			if (type?.BaseType == null) {
				return false;
			}

			if (type.BaseType.FullName == fullName) {
				return true;
			} else {
				return HasBaseType (type.BaseType, fullName);
			}
		}

		public static bool ContainStaticFields (this Type type)
		{
			var baseTypeContainsStaticFields = false;

			if (type.BaseType != null) {
				baseTypeContainsStaticFields = ContainStaticFields (type.BaseType);
			}

			return type.GetFields ().Any (f => f.IsStatic) || baseTypeContainsStaticFields;
		}
	}
}