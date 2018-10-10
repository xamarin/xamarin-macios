using System;
using System.Collections.Generic;
using System.Linq;
using Clang;
using Clang.Ast;
using Mono.Cecil;

namespace Extrospection
{
	public static class AttributeHelpers
	{
		// These both return out Version and bool as you can have an an attribute with no version (null) which is different no matching attribute at all
		public static bool FindDeprecated (IEnumerable<CustomAttribute> attributes, out Version version)
		{
			foreach (var attribute in attributes.Where (x => HasDeprecated (x, Helpers.Platform)))
				return GetPlatformVersion (attribute, out version);

			version = null;
			return false;
		}

		public static bool FindObsolete (IEnumerable<CustomAttribute> attributes, out Version version)
		{
			foreach (var attribute in attributes.Where (x => HasObsoleted (x, Helpers.Platform)))
				return GetPlatformVersion (attribute, out version);

			version = null;
			return false;
		}

		public static bool HasDeprecated (CustomAttribute attribute, Platforms platform) => HasMatchingPlatformAttribute ("DeprecatedAttribute", attribute, platform);
		public static bool HasObsoleted (CustomAttribute attribute, Platforms platform) => HasMatchingPlatformAttribute ("ObsoletedAttribute", attribute, platform);

		static bool HasMatchingPlatformAttribute (string expectedAttributeName, CustomAttribute attribute, Platforms platform)
		{
			if (attribute.Constructor.DeclaringType.Name == expectedAttributeName) {
				byte attrPlatform = (byte)attribute.ConstructorArguments[0].Value;
				if (attrPlatform == Helpers.GetPlatformManagedValue (platform))
					return true;
			}
			return false;
		}

		public static bool HasAdviced (IEnumerable<CustomAttribute> attributes)
		{
			return attributes.Any (x => x.Constructor.DeclaringType.Name == "AdviceAttribute");
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
				version = new Version ((int)attribute.ConstructorArguments[1].Value, (int)attribute.ConstructorArguments[2].Value);
				return true;
			case 6:
				version = new Version ((int)attribute.ConstructorArguments[1].Value, (int)attribute.ConstructorArguments[2].Value, (int)attribute.ConstructorArguments[3].Value);
				return true;
			default:
				throw new InvalidOperationException ("GetPlatformVersion with unexpected number of arguments {attribute.ConstructorArguments.Count}");
			}
		}

		public static bool FindObjcDeprecated (IEnumerable<Attr> attrs, out VersionTuple version)
		{
			AvailabilityAttr attr = attrs.OfType<AvailabilityAttr> ().FirstOrDefault (x => !x.Deprecated.IsEmpty && x.Platform.Name == Helpers.ClangPlatformName);
			if (attr != null) {
				version = attr.Deprecated;
				return true;
			} else {
				version = VersionTuple.Empty;
				return false;
			}
		}
	}
}
