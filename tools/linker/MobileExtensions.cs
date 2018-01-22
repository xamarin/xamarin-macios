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

		public static bool HasCustomAttribute (this ICustomAttributeProvider provider, DerivedLinkContext context, string @namespace, string name)
		{
			if (provider?.HasCustomAttributes == true) {
				foreach (CustomAttribute attribute in provider.CustomAttributes) {
					TypeReference tr = attribute.Constructor.DeclaringType;
					if (tr.Is (@namespace, name))
						return true;
				}
			}

			return context?.GetCustomAttributes (provider, @namespace, name)?.Count > 0;
		}

		static bool HasGeneratedCodeAttribute (ICustomAttributeProvider provider, DerivedLinkContext context)
		{
			return provider.HasCustomAttribute (context, "System.Runtime.CompilerServices", "CompilerGeneratedAttribute");
		}

		static bool HasOptimizableAttribute (ICustomAttributeProvider provider, DerivedLinkContext context)
		{
			return provider.HasCustomAttribute (context, Namespaces.ObjCRuntime, "LinkerOptimizeAttribute");
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
			// check the property too
			if (self.IsGetter || self.IsSetter) {
				if (HasGeneratedCodeAttribute (GetPropertyByAccessor (self), link_context))
					return true;
			}
			return HasGeneratedCodeAttribute (self, link_context);
		}

		public static bool IsOptimizableCode (this MethodDefinition self, DerivedLinkContext link_context)
		{
			if (self.IsGetter || self.IsSetter) {
				var accessor = GetPropertyByAccessor (self);
				if (HasOptimizableAttribute (accessor, link_context))
					return true;
			}
			return HasOptimizableAttribute (self, link_context);
		}
	}
}