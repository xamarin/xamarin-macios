using System;
using System.Collections.Generic;
using Mono.Cecil;

using Xamarin.Tuner;

namespace Xamarin.Linker {

	// this can be used (directly) with Xamarin.Mac and as the base of Xamarin.iOS step
	public class CoreRemoveAttributes : MobileRemoveAttributes {
		
		protected DerivedLinkContext LinkContext {
			get {
				return (DerivedLinkContext) base.context;
			}
		}

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
			case "RequiresSuperAttribute":
			case "BindingImplAttribute":
				return attr_type.Namespace == Namespaces.ObjCRuntime;
			case "AdoptsAttribute":
				return attr_type.Namespace == Namespaces.ObjCRuntime && LinkContext.App.Optimizations.RegisterProtocols == true;
			case "ProtocolAttribute":
			case "ProtocolMemberAttribute":
				return attr_type.Namespace == Namespaces.Foundation && LinkContext.App.Optimizations.RegisterProtocols == true;
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
					LinkContext.StoreCustomAttribute (provider, attribute, "Availability");
					break;
				case "AdoptsAttribute":
				case "BindingImplAttribute":
					LinkContext.StoreCustomAttribute (provider, attribute);
					break;
				}
			} else if (attr_type.Namespace == Namespaces.Foundation) {
				switch (attr_type.Name) {
				case "ProtocolAttribute":
				case "ProtocolMemberAttribute":
					LinkContext.StoreCustomAttribute (provider, attribute);
					break;
				}
			}

			base.WillRemoveAttribute (provider, attribute);
		}
	}
}
