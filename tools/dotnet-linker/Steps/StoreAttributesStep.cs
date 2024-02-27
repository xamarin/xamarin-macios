using System;

using Mono.Cecil;

#nullable enable

namespace Xamarin.Linker.Steps {
	// The registrar needs some of the system attributes that the linker might remove, so store those elsewhere for the static registrar's use.
	public class StoreAttributesStep : AttributeIteratorBaseStep {
		protected override void ProcessAttribute (ICustomAttributeProvider provider, CustomAttribute attribute, out bool remove)
		{
			// don't remove any of these
			remove = false;

			// this avoids calling FullName (which allocates a string)
			var attr_type = attribute.Constructor.DeclaringType;
			var store = false;
			switch (attr_type.Namespace) {
			case "System.Runtime.CompilerServices":
				switch (attr_type.Name) {
				case "ExtensionAttribute":
					store = true;
					break;
				}
				break;
			case "System.Runtime.Versioning":
				switch (attr_type.Name) {
				case "SupportedOSPlatformAttribute":
				case "UnsupportedOSPlatformAttribute":
					LinkContext.StoreCustomAttribute (provider, attribute, "Availability");
					break;
				}
				break;
			case "Foundation":
				switch (attr_type.Name) {
				case "ProtocolAttribute":
					store = LinkContext.App.Optimizations.RegisterProtocols == true;
					break;
				}
				break;
			}

			if (store)
				LinkContext.StoreCustomAttribute (provider, attribute);
		}
	}
}
