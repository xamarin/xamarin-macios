using System;
using System.Collections.Generic;
using System.Linq;
using Clang;
using Clang.Ast;
using Mono.Cecil;

namespace Extrospection {
	public static class AttributeHelpers {
		static bool Skip (ICustomAttributeProvider item)
		{
			// Before accessing CustomAttributes we must query HasCustomAttributes
			// else Cecil will happily allocate an empty collection and impact performance
			return !item.HasCustomAttributes;
		}

		// These both return out Version and bool as you can have an an attribute with no version (null) which is different no matching attribute at all
		public static bool FindDeprecated (ICustomAttributeProvider item, out Version version)
		{
			version = null;

			if (Skip (item))
				return false;

			foreach (var attribute in item.CustomAttributes.Where (x => HasDeprecated (x, Helpers.Platform)))
				return GetPlatformVersion (attribute, out version);

			return false;
		}

		public static bool FindObsolete (ICustomAttributeProvider item, out Version version)
		{
			version = null;

			if (Skip (item))
				return false;

			foreach (var attribute in item.CustomAttributes.Where (x => HasObsoleted (x, Helpers.Platform)))
				return GetPlatformVersion (attribute, out version);

			return false;
		}

		// Confusingly enough, ObsoletedOSPlatformAttribute is a deprecation not obsolete attribute in NET7
		public static bool HasDeprecated (CustomAttribute attribute, Platforms platform) => HasMatchingPlatformAttribute ("DeprecatedAttribute", attribute, platform);
		public static bool HasObsoleted (CustomAttribute attribute, Platforms platform) => HasMatchingPlatformAttribute ("ObsoletedAttribute", attribute, platform);

		static bool HasMatchingPlatformAttribute (string expectedAttributeName, CustomAttribute attribute, Platforms platform)
		{
			if (attribute.Constructor.DeclaringType.Name == expectedAttributeName) {
				byte attrPlatform = (byte) attribute.ConstructorArguments [0].Value;
				if (attrPlatform == Helpers.GetPlatformManagedValue (platform))
					return true;
			}
			return false;
		}

		public static bool HasUnsupportedOSPlatform (CustomAttribute attribute, Platforms platform)
		{
			if (attribute.Constructor.DeclaringType.Name == "UnsupportedOSPlatformAttribute")
				return IsOSPlatformAttribute (attribute, platform);
			return false;
		}

		public static bool HasObsoletedOSPlatform (CustomAttribute attribute, Platforms platform)
		{
			if (attribute.Constructor.DeclaringType.Name == "ObsoletedOSPlatformAttribute")
				return IsOSPlatformAttribute (attribute, platform);
			return false;
		}

		public static bool HasObsolete (CustomAttribute attribute, Platforms platform)
		{
			// This intentionally does not include "ObsoletedOSPlatformAttribute", as that is for deprecation not obsolete
			return attribute.Constructor.DeclaringType.Name == "ObsoleteAttribute";
		}

		static bool GetPlatformVersion (CustomAttribute attribute, out Version version)
		{
			// Three different Attribute flavors
			// (PlatformName platform, PlatformArchitecture architecture = PlatformArchitecture.None, string message = null)
			// (PlatformName platform, int majorVersion, int minorVersion, PlatformArchitecture architecture = PlatformArchitecture.None, string message = null)
			// (PlatformName platform, int majorVersion, int minorVersion, int subminorVersion, PlatformArchitecture architecture = PlatformArchitecture.None, string message = null)
			switch (attribute.ConstructorArguments.Count) {
			case 3:
				version = null;
				return true;
			case 5:
				version = new Version ((int) attribute.ConstructorArguments [1].Value, (int) attribute.ConstructorArguments [2].Value);
				return true;
			case 6:
				version = new Version ((int) attribute.ConstructorArguments [1].Value, (int) attribute.ConstructorArguments [2].Value, (int) attribute.ConstructorArguments [3].Value);
				return true;
			default:
				throw new InvalidOperationException ($"GetPlatformVersion with unexpected number of arguments {attribute.ConstructorArguments.Count} {attribute.Constructor.DeclaringType.Name}");
			}
		}

		public static bool FindObjcDeprecated (IEnumerable<Attr> attrs, out VersionTuple version)
		{
			AvailabilityAttr attr = attrs.OfType<AvailabilityAttr> ().FirstOrDefault (x => !x.Deprecated.IsEmpty && x.Platform.Name == Helpers.ClangPlatformName);
			if (attr is not null) {
				version = attr.Deprecated;
				return true;
			} else {
				version = VersionTuple.Empty;
				return false;
			}
		}

		public static bool HasAnyDeprecationForCurrentPlatform (ICustomAttributeProvider item)
		{
			if (Skip (item))
				return false;

			// Properties are a special case  as it is generated on the property itself and not the individual get_ \ set_ methods
			// Cecil does not have a link between the MethodDefinition we have and the hosting PropertyDefinition, so we have to dig to find the match
			if (item is MethodDefinition method) {
				PropertyDefinition property = method.DeclaringType.Properties.FirstOrDefault (p => p.GetMethod == method || p.SetMethod == method);
				if (property is not null && HasAnyDeprecationForCurrentPlatform (property)) {
					return true;
				}
			}

			// This allows us to accept [Deprecated (iOS)] for watch and tv, which many of our bindings currently have
			// If we want to force separate tv\watch attributes remove GetRelatedPlatforms and just check Helpers.Platform
			if (Helpers.IsDotNet) {
				foreach (var attribute in item.CustomAttributes) {
					if (AttributeHelpers.HasObsolete (attribute, Helpers.Platform))
						return true;

					// Consider 'HasObsoletedOSPlatform' (Deprecated) and 'UnsupportedOSPlatform' (Obsoleted/Unavailable)
					if (AttributeHelpers.HasObsoletedOSPlatform (attribute, Helpers.Platform) ||
						AttributeHelpers.HasUnsupportedOSPlatform (attribute, Helpers.Platform))
						return true;

					// The only related platforms for .NET is iOS for Mac Catalyst
					if (Helpers.Platform == Platforms.MacCatalyst &&
							(AttributeHelpers.HasObsoletedOSPlatform (attribute, Platforms.iOS) ||
							 AttributeHelpers.HasUnsupportedOSPlatform (attribute, Platforms.iOS)))
						return true;
				}
			} else {
				Platforms [] platforms = GetRelatedPlatforms ();
				foreach (var attribute in item.CustomAttributes) {
					if (platforms.Any (x => AttributeHelpers.HasDeprecated (attribute, x)) || platforms.Any (x => AttributeHelpers.HasObsoleted (attribute, x)))
						return true;
				}
			}
			return false;
		}

		public static bool HasAnyObsoleted (ICustomAttributeProvider item)
		{
			if (Skip (item))
				return false;

			foreach (var attribute in item.CustomAttributes) {
				if (AttributeHelpers.HasObsoleted (attribute, Helpers.Platform))
					return true;
			}

			return false;
		}

		static Platforms [] GetRelatedPlatforms ()
		{
			// TV and Watch also implictly accept iOS
			switch (Helpers.Platform) {
			case Platforms.macOS:
				return new Platforms [] { Platforms.macOS };
			case Platforms.iOS:
				return new Platforms [] { Platforms.iOS };
			case Platforms.tvOS:
				return new Platforms [] { Platforms.iOS, Platforms.tvOS };
			case Platforms.watchOS:
				return new Platforms [] { Platforms.iOS, Platforms.watchOS };
			case Platforms.MacCatalyst:
				return new Platforms [] { Platforms.iOS, Platforms.MacCatalyst };
			default:
				throw new InvalidOperationException ($"Unknown {Helpers.Platform} in GetPlatforms");
			}
		}

		public static bool HasAnyAdvice (ICustomAttributeProvider item)
		{
			if (Skip (item))
				return false;

			if (HasAdviced (item.CustomAttributes))
				return true;

			// Properties are a special case for [Advice], as it is generated on the property itself and not the individual get_ \ set_ methods
			// Cecil does not have a link between the MethodDefinition we have and the hosting PropertyDefinition, so we have to dig to find the match
			if (item is MethodDefinition method) {
				PropertyDefinition property = method.DeclaringType.Properties.FirstOrDefault (p => p.GetMethod == method || p.SetMethod == method);
				if (property is not null && HasAdviced (property.CustomAttributes))
					return true;
			}
			return false;
		}

		static bool HasAdviced (IEnumerable<CustomAttribute> attributes)
		{
			return attributes.Any (x => x.Constructor.DeclaringType.Name == "AdviceAttribute");
		}

		public static bool IsOSPlatformAttribute (CustomAttribute attrib, Platforms platform)
		{
			switch (attrib.AttributeType.Name) {
			case "SupportedOSPlatformAttribute":
			case "UnsupportedOSPlatformAttribute":
			case "ObsoletedOSPlatformAttribute":
				var platformName = (string) attrib.ConstructorArguments [0].Value;
				return platformName.StartsWith (platform.AsPlatformAttributeString (), StringComparison.OrdinalIgnoreCase);
			default:
				return false;
			}
		}
	}
}
