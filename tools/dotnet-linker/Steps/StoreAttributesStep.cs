using System;

using Mono.Cecil;

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
	// * The registrar needs some of the system attributes that the linker might remove, so
	//   store those elsewhere for the static registrar's use.
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
			case Namespaces.ObjCRuntime:
				switch (attr_type.Name) {
				case "AdoptsAttribute":
					store = remove = LinkContext.App.Optimizations.RegisterProtocols == true;
					break;
				case "NativeAttribute":
					store = remove = LinkContext.App.Optimizations.RemoveDynamicRegistrar == true;
					break;
				case "NativeNameAttribute":
					store = remove = true;
					break;
				}
				break;
			case Namespaces.Foundation:
				switch (attr_type.Name) {
				case "ModelAttribute":
					store = remove = LinkContext.App.Optimizations.RemoveDynamicRegistrar == true;
					break;
				case "ProtocolAttribute":
					// ProtocolAttribute could in theory be removed as well, but it currently
					// makes the linker keep the wrapper type, and we need the wrapper type,
					// so in order to remove ProtocolAttribute we'd need to add logic to keep
					// the wrapper type somehow.
					store = LinkContext.App.Optimizations.RegisterProtocols == true;
					break;
				case "ProtocolMemberAttribute":
					store = remove = LinkContext.App.Optimizations.RegisterProtocols == true;
					break;
				}
				break;
			}

			if (store)
				LinkContext.StoreCustomAttribute (provider, attribute);
		}
	}
}
