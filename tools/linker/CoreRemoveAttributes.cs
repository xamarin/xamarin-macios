using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Collections.Generic;
using Xamarin.Tuner;

namespace Xamarin.Linker
{

	// this can be used (directly) with Xamarin.Mac and as the base of Xamarin.iOS step
	public class CoreRemoveAttributes : MobileRemoveAttributes
	{
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
			case "AdoptsAttribute":
			case "BlockProxyAttribute":
			case "LinkerOptimizeAttribute":
			case "NativeAttribute":
			case "ReleaseAttribute":
			case "UserDelegateTypeAttribute":
				return attr_type.Namespace == Namespaces.ObjCRuntime && !LinkContext.DynamicRegistrationSupported;
			//case "ExportAttribute":
			//case "ModelAttribute":
			//case "RegisterAttribute":
			//case "ProtocolAttribute":
			case "ProtocolMemberAttribute":
				return attr_type.Namespace == Namespaces.Foundation && !LinkContext.DynamicRegistrationSupported;
			case "AdviceAttribute":
			case "FieldAttribute":
			case "PreserveAttribute":       // the ApplyPreserveAttribute substep is executed before this
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
					StoreAttributeAsAnnotation ("Availability", provider, attribute);
					break;
				case "AdoptsAttribute":
				case "BlockProxyAttribute":
				case "NativeAttribute":
				case "ReleaseAttribute":
				case "UserDelegateTypeAttribute":
					StoreAttributeAsAnnotation (attr_type.Name, provider, attribute);
					break;
				}
			} else if (attr_type.Namespace == Namespaces.Foundation) {
				switch (attr_type.Name) {
				//case "ExportAttribute":
				//case "ModelAttribute":
				//case "RegisterAttribute":
				case "ProtocolAttribute":
				case "ProtocolMemberAttribute":
					StoreAttributeAsAnnotation (attr_type.Name, provider, attribute);
					break;
				}
			} else if (attr_type.Namespace == "System.Runtime.CompilerServices") {
				switch (attr_type.Name) {
				case "CompilerGeneratedAttribute":
					StoreAttributeAsAnnotation (attr_type.Name, provider, attribute);
					break;
				}
			}

			base.WillRemoveAttribute (provider, attribute);
		}

		void StoreAttributeAsAnnotation (string name, ICustomAttributeProvider provider, CustomAttribute attribute)
		{
			var dict = context.Annotations.GetCustomAnnotations (name);
			List<ICustomAttribute> attribs;
			object attribObjects;
			if (!dict.TryGetValue (provider, out attribObjects)) {
				attribs = new List<ICustomAttribute> ();
				dict [provider] = attribs;
			} else {
				attribs = (List<ICustomAttribute>) attribObjects;
			}
			// Make sure the attribute is resolved, since after removing the attribute
			// it won't be able to do it. The 'CustomAttribute.Resolve' method is private, but fetching
			// any property will cause it to be called.
			// We also need to store the constructor's DeclaringType separately, because it may
			// be nulled out from the constructor by the linker if the attribute type itself is linked away.
			var dummy = attribute.HasConstructorArguments;
			attribs.Add (new AttributeStorage { Attribute = attribute, AttributeType = attribute.Constructor.DeclaringType } );
		}

		class AttributeStorage : ICustomAttribute
		{
			public CustomAttribute Attribute;
			public TypeReference AttributeType { get; set; }

			public bool HasFields => Attribute.HasFields;
			public bool HasProperties => Attribute.HasProperties;
			public bool HasConstructorArguments => Attribute.HasConstructorArguments;

			public Collection<CustomAttributeNamedArgument> Fields => Attribute.Fields;
			public Collection<CustomAttributeNamedArgument> Properties => Attribute.Properties;
			public Collection<CustomAttributeArgument> ConstructorArguments => Attribute.ConstructorArguments;
		}
	}
}
