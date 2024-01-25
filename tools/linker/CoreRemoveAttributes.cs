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

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			if (LinkContext.App.Optimizations.CustomAttributesRemoval != true)
				return false;
			return base.IsActiveFor (assembly);
		}

		protected override bool IsRemovedAttribute (CustomAttribute attribute)
		{
			// note: this also avoid calling FullName (which allocates a string)
			var attr_type = attribute.Constructor.DeclaringType;
			switch (attr_type.Name) {
			case "AdviceAttribute":
			case "FieldAttribute":
			case "PreserveAttribute":   // the ApplyPreserveAttribute substep is executed before this
			case "LinkerSafeAttribute":
				return attr_type.Namespace == Namespaces.Foundation;
			// used for documentation, not at runtime
			case "AvailabilityAttribute":
			case "AvailabilityBaseAttribute":
			case "DeprecatedAttribute":
			case "IntroducedAttribute":
			case "NotImplementedAttribute":
			case "ObsoletedAttribute":
			case "ThreadSafeAttribute":
			case "UnavailableAttribute":
			case "LinkWithAttribute":
			case "DesignatedInitializerAttribute":
			case "RequiresSuperAttribute":
			case "BindingImplAttribute":
			case "NoiOSAttribute":
			case "NoMacAttribute":
			case "NoTVAttribute":
			case "NoWatchAttribute":
			// special subclasses of IntroducedAttribute
			case "TVAttribute":
			case "MacCatalystAttribute":
			case "WatchAttribute":
				return attr_type.Namespace == Namespaces.ObjCRuntime;
			// special subclasses of IntroducedAttribute
			case "iOSAttribute":
			case "MacAttribute":
				return String.IsNullOrEmpty (attr_type.Namespace);
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
				case "AvailabilityAttribute":       // obsolete (could be present in user code)
				case "AvailabilityBaseAttribute":   // base type for IntroducedAttribute and DeprecatedAttribute (could be in user code)
				case "DeprecatedAttribute":
				case "IntroducedAttribute":
				// they are subclasses of ObjCRuntime.IntroducedAttribute
				case "TVAttribute":
				case "MacCatalystAttribute":
				case "WatchAttribute":
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
			} else if (String.IsNullOrEmpty (attr_type.Namespace)) {
				switch (attr_type.Name) {
				// they are subclasses of ObjCRuntime.IntroducedAttribute
				case "iOSAttribute":
				case "MacAttribute":
					LinkContext.StoreCustomAttribute (provider, attribute, "Availability");
					break;
				}
			}

			base.WillRemoveAttribute (provider, attribute);
		}
	}
}
