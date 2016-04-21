using System;
using System.Collections.Generic;
using Mono.Cecil;

using Mono.Tuner;

namespace MonoTouch.Tuner {

	public static class Extensions {
		
		internal static HashSet<TypeDefinition> needs_isdirectbinding_check;
		
		public static bool IsDirectBindingCheckRequired (this TypeDefinition type)
		{
			if (needs_isdirectbinding_check == null)
				return true;
			
			return needs_isdirectbinding_check.Contains (type);
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
			if (Xamarin.Bundler.Driver.App.IsUnified) {
				return type.Is (@namespace, name);
			} else {
				return type.Is ("MonoTouch." + @namespace, name);
			}
#endif
		}
	}
}
