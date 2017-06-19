using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace Xamarin.Linker {

	// this can be used (directly) with Xamarin.Mac and as the base of Xamarin.iOS step
	public class CoreRemoveAttributes : MobileRemoveAttributes {
		
		protected override bool IsRemovedAttribute (CustomAttribute attribute)
		{
			// note: this also avoid calling FullName (which allocates a string)
			var attr_type = attribute.Constructor.DeclaringType;
			switch (attr_type.Name) {
			case "AdviceAttribute":
			case "FieldAttribute":
			case "PreserveAttribute":	// the ApplyPreserveAttribute substep is executed before this
			case "LinkerSafeAttribute":
				return attr_type.Namespace == Namespaces.Foundation;
			// used for documentation, not at runtime
			case "AvailabilityAttribute":
			case "AvailabilityBaseAttribute":
			case "DeprecatedAttribute":
			case "IntroducedAttribute":
			case "iOSAttribute":
			case "MacAttribute":
			case "LionAttribute":
			case "MountainLionAttribute":
			case "MavericksAttribute":
			case "NotImplementedAttribute":
			case "ObsoletedAttribute":
			case "SinceAttribute":
			case "ThreadSafeAttribute":
			case "UnavailableAttribute":
			case "LinkWithAttribute":
			case "DesignatedInitializerAttribute":
				return attr_type.Namespace == Namespaces.ObjCRuntime;
			default:
				return base.IsRemovedAttribute (attribute);
			}
		}

		protected override void WillRemoveAttribute (ICustomAttributeProvider provider, CustomAttribute attribute)
		{
			var attr_type = attribute.Constructor.DeclaringType;
			if (attr_type.Namespace == Namespaces.ObjCRuntime) {
				switch (attr_type.Name) {
				case "AvailabilityAttribute":
				case "AvailabilityBaseAttribute":
				case "DeprecatedAttribute":
				case "IntroducedAttribute":
					var dict = context.Annotations.GetCustomAnnotations ("Availability");
					List<CustomAttribute> attribs;
					object attribObjects;
					if (!dict.TryGetValue (provider, out attribObjects)) {
						attribs = new List<CustomAttribute> ();
						dict [provider] = attribs;
					} else {
						attribs = (List<CustomAttribute>) attribObjects;
					}
					// Make sure the attribute is resolved, since after removing the attribute
					// it won't be able to do it. The 'CustomAttribute.Resolve' method is private, but fetching
					// any property will cause it to be called.
					var dummy = attribute.HasConstructorArguments;
					attribs.Add (attribute);
					break;
				}
			}

			base.WillRemoveAttribute (provider, attribute);
		}
	}
}
