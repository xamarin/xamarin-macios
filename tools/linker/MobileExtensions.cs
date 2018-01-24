using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Tuner;

using XamCore.ObjCRuntime;
using Xamarin.Bundler;
using Xamarin.Tuner;

namespace Xamarin.Linker {

	public static class MobileExtensions {

		// Returns a string representation of the specified provider that is suitable for user-visible error/warning messages.
		public static string AsString (this ICustomAttributeProvider provider)
		{
			if (provider is MemberReference member)
				return member.DeclaringType.FullName + "." + member.Name;
			if (provider is MethodReturnType returnType)
				return AsString ((ICustomAttributeProvider) returnType.Method);
			return provider.ToString ();
		}

		// This method will look in any stored attributes in the link context as well as the provider itself.
		public static bool HasCustomAttribute (this ICustomAttributeProvider provider, DerivedLinkContext context, string @namespace, string name)
		{
			if (provider?.HasCustomAttribute (@namespace, name) == true)
				return true;
			
			return context?.GetCustomAttributes (provider, @namespace, name)?.Count > 0;
		}

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

		static bool HasGeneratedCodeAttribute (ICustomAttributeProvider provider, DerivedLinkContext context)
		{
			return provider.HasCustomAttribute (context, "System.Runtime.CompilerServices", "CompilerGeneratedAttribute");
		}

		// The 'provider' parameter is only used in error messages to explain where the broken attribute comes from
		// (in particular it's not used to get the custom attributes themselves, since those may not come from this provider instance)
		static BindingImplOptions? GetBindingImplAttribute (ICustomAttributeProvider provider, IEnumerable<ICustomAttribute> attributes)
		{
			if (attributes == null)
				return null;
			
			foreach (var ca in attributes) {
				TypeReference tr = ca.AttributeType;
				if (!tr.Is (Namespaces.ObjCRuntime, "BindingImplAttribute"))
					continue;

				if (ca.HasFields)
					throw ErrorHelper.CreateError (2105, "The [BindingImpl] attribute on the member '{0}' is invalid: did not expect fields.", provider.AsString ());
				if (ca.HasProperties)
					throw ErrorHelper.CreateError (2105, "The [BindingImpl] attribute on the member '{0}' is invalid: did not expect properties.", provider.AsString ());

				switch (ca.ConstructorArguments.Count) {
				case 1:
					var arg = ca.ConstructorArguments [0];
					if (!arg.Type.Is (Namespaces.ObjCRuntime, "BindingImplOptions"))
						throw ErrorHelper.CreateError (2105, "The [BindingImpl] attribute on the member '{0}' is invalid: did not expect a constructor with a '{1}' parameter type (expected 'ObjCRuntime.BindingImplOptions).", provider.AsString (), arg.Type.FullName);
					return (BindingImplOptions) (int) arg.Value;
				default:
					throw ErrorHelper.CreateError (2105, "The [BindingImpl] attribute on the member '{0}' is invalid: did not expect a constructor with a {1} parameters (expected 1 parameters).", provider.AsString (), ca.ConstructorArguments.Count);
				}
			}

			return null;
		}

		static BindingImplOptions? GetBindingImplAttribute (ICustomAttributeProvider provider, DerivedLinkContext context)
		{
			if (provider != null && provider.HasCustomAttributes) {
				var rv = GetBindingImplAttribute (provider, provider.CustomAttributes);
				if (rv != null)
					return rv;
			}

			return GetBindingImplAttribute (provider, context?.GetCustomAttributes (provider, Namespaces.ObjCRuntime, "BindingImplOptions"));
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

		public static bool IsBindingImplOptimizableCode (this MethodDefinition self, DerivedLinkContext link_context)
		{
			var attrib = GetBindingImplAttribute (self, link_context);
			if ((attrib & BindingImplOptions.Optimizable) == BindingImplOptions.Optimizable)
				return true;

			// Check the property too
			if (self.IsGetter || self.IsSetter) {
				attrib = GetBindingImplAttribute (GetPropertyByAccessor (self), link_context);
				if ((attrib & BindingImplOptions.Optimizable) == BindingImplOptions.Optimizable)
					return true;
			}

			return false;
		}

		public static bool IsOptimizableCode (this MethodDefinition self, DerivedLinkContext link_context)
		{
			return IsGeneratedCode (self, link_context) || IsBindingImplOptimizableCode (self, link_context);
		}
	}
}