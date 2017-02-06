using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Tuner;

using Xamarin.Tuner;

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

		public static bool IsGeneratedCode (this MethodDefinition self, DerivedLinkContext link_context)
		{
			if (link_context.GeneratedCode != null)
				return link_context.GeneratedCode.Contains (self);

			// check the property too
			if (self.IsGetter || self.IsSetter) {
				if (HasGeneratedCodeAttribute (GetPropertyByAccessor (self)))
					return true;
			}
			return HasGeneratedCodeAttribute (self);
		}
	}
}