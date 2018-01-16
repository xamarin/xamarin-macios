using System;
using System.Collections.Generic;
using Mono.Cecil;

using Mono.Tuner;

using Xamarin.Tuner;

namespace MonoTouch.Tuner {

	public static class Extensions {
		
		public static bool? GetIsDirectBindingConstant (this TypeDefinition type, DerivedLinkContext link_context)
		{
			if (link_context?.IsDirectBindingValue == null)
				return null;
			
			bool? value;
			if (link_context.IsDirectBindingValue.TryGetValue (type, out value))
				return value;

			return null;
		}

		public static bool IsPlatformType (this TypeReference type, string @namespace, string name)
		{
#if MONOMAC
			if (Xamarin.Bundler.Driver.IsUnified) {
				return type.Is (@namespace, name);
			} else {
				return type.Is ("MonoMac." + @namespace, name);
			}
#else
			return type.Is (@namespace, name);
#endif
		}
	}
}
