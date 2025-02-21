using System;

using Mono.Cecil;

#nullable enable

namespace Xamarin.Linker.Steps {
	// The .NET linker comes with a way to remove attributes (by passing '--link-attributes
	// some.xml' as a command-line argument), but this has a few problems:
	//
	// * We'd need to figure out which attributes to remove before running the linker,
	//   but the code to figure out which optimizations have been enabled (and which attributes
	//   should be removed) is in our custom linker code. We'd need to refactor a big chunk
	//   of code to move this logic out of our custom linker code.
	// * We need to keep the removed attributes around, because the static registrar needs
	//   them. Our custom linker logic is not notified for removed attributes, which means
	//   we'd need to store all attributes for the attribute types we're interested in (as
	//   opposed to this solution, where we only store attributes that are actually removed).
	// * The attributes we want removed may contain references to types we don't want
	//   linked away. If we ask the linker to remove those attributes, then the types may
	//   be linked away as well, and there's no good way around this.
	//
	// The end result is that a custom step is the best solution for now.

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
			case Namespaces.Foundation:
				switch (attr_type.Name) {
				case "ProtocolAttribute":
				case "ProtocolMemberAttribute":
					store = LinkContext.App.Optimizations.RegisterProtocols == true;
					remove = store;
					break;
				}
				break;
			case Namespaces.ObjCRuntime:
				switch (attr_type.Name) {
				case "NativeNameAttribute":
					store = true;
					remove = store;
					break;
				case "AdoptsAttribute":
					store = LinkContext.App.Optimizations.RegisterProtocols == true;
					remove = store;
					break;
				}
				break;
			}

			if (store)
				LinkContext.StoreCustomAttribute (provider, attribute);
		}
	}
}
