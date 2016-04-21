using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Tuner;

namespace Xamarin.Linker {

	public static class MobileExtensions {

		public static bool HasCustomAttribute (this ICustomAttributeProvider provider, string @namespace, string name)
		{
			if (provider == null || !provider.HasCustomAttributes)
				return false;

			foreach (CustomAttribute attribute in provider.CustomAttributes) {
				TypeReference tr = attribute.Constructor.DeclaringType;
				if (tr.Is (@namespace, name))
					return true;
			}
			return false;
		}

		static bool HasGeneratedCodeAttribute (ICustomAttributeProvider provider)
		{
			return provider.HasCustomAttribute ("System.Runtime.CompilerServices", "CompilerGeneratedAttribute");
		}

		static PropertyDefinition GetPropertyByAccessor (MethodDefinition method)
		{
			foreach (PropertyDefinition property in method.DeclaringType.Properties) {
				if (property.GetMethod == method || property.SetMethod == method)
					return property;
			}
			return null;
		}

		internal static HashSet<MethodDefinition> generated_code;

		public static bool IsGeneratedCode (this MethodDefinition self)
		{
			if (generated_code != null)
				return generated_code.Contains (self);

			// check the property too
			if (self.IsGetter || self.IsSetter) {
				if (HasGeneratedCodeAttribute (GetPropertyByAccessor (self)))
					return true;
			}
			return HasGeneratedCodeAttribute (self);
		}
	}
}