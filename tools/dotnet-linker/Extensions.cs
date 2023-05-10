using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
using System;

#nullable enable

namespace Xamarin.Linker {
	public static class Extensions {
		public static bool Inherits (this TypeReference self, string @namespace, string name, IMetadataResolver resolver)
		{
			if (@namespace is null)
				throw new ArgumentNullException ("namespace");
			if (name is null)
				throw new ArgumentNullException ("name");
			if (self is null)
				return false;

			TypeReference current = resolver.Resolve (self);
			while (current is not null) {
				if (current.Is (@namespace, name))
					return true;
				if (current.Is ("System", "Object"))
					return false;

				TypeDefinition td = resolver.Resolve (current);
				if (td is null)
					return false; // could not resolve type
				current = td.BaseType;
			}
			return false;
		}
	}
}
