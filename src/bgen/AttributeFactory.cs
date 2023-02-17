using System;
using System.Collections.Generic;
using System.Linq;
using ObjCRuntime;

#nullable enable

public static class AttributeFactory {
	public static readonly Type PlatformEnum = typeof (PlatformName);
#if !NET
	public static readonly Type PlatformArch = typeof (PlatformArchitecture);
#endif

	public static readonly Type IntroducedAttributeType = typeof (IntroducedAttribute);
	public static readonly Type UnavailableAttributeType = typeof (UnavailableAttribute);
	public static readonly Type ObsoletedAttributeType = typeof (ObsoletedAttribute);
	public static readonly Type DeprecatedAttributeType = typeof (DeprecatedAttribute);

	public static Attribute CreateNewAttribute (Type attribType, Type [] ctorTypes, object? [] ctorValues)
	{
		var ctor = attribType.GetConstructor (ctorTypes);
		if (ctor is null)
			throw ErrorHelper.CreateError (1058, attribType.FullName);

		return (Attribute) ctor.Invoke (ctorValues);
	}

	static Attribute CreateMajorMinorAttribute (Type type, PlatformName platform, int major, int minor, string? message)
	{
#if NET
		var ctorValues = new object? [] { (byte) platform, major, minor, message };
		var ctorTypes = new [] { PlatformEnum, typeof (int), typeof (int), typeof (string) };
#else
		var ctorValues = new object? [] { (byte) platform, major, minor, (byte) 0xff, message };
		var ctorTypes = new [] { PlatformEnum, typeof (int), typeof (int), PlatformArch, typeof (string) };
#endif
		return CreateNewAttribute (type, ctorTypes, ctorValues);
	}

	static Attribute CreateUnspecifiedAttribute (Type type, PlatformName platform, string? message)
	{
#if NET
		var ctorValues = new object? [] { (byte) platform, message };
		var ctorTypes = new [] { PlatformEnum, typeof (string) };
#else
		var ctorValues = new object? [] { (byte) platform, (byte) 0xff, message };
		var ctorTypes = new [] { PlatformEnum, PlatformArch, typeof (string) };
#endif
		return CreateNewAttribute (type, ctorTypes, ctorValues);
	}

	public static Attribute CreateNewIntroducedAttribute (PlatformName platform, int major, int minor, string? message = null)
	{
		return CreateMajorMinorAttribute (IntroducedAttributeType, platform, major, minor, message);
	}

	public static Attribute CreateNewUnspecifiedIntroducedAttribute (PlatformName platform, string? message = null)
	{
		return CreateUnspecifiedAttribute (IntroducedAttributeType, platform, message);
	}

	public static Attribute CreateObsoletedAttribute (PlatformName platform, int major, int minor, string? message = null)
	{
		return CreateMajorMinorAttribute (ObsoletedAttributeType, platform, major, minor, message);
	}

	public static Attribute CreateDeprecatedAttribute (PlatformName platform, int major, int minor, string? message = null)
	{
		return CreateMajorMinorAttribute (DeprecatedAttributeType, platform, major, minor, message);
	}

	public static Attribute CreateUnavailableAttribute (PlatformName platformName, string? message = null)
	{
#if NET
		var ctorValues = new object? [] { (byte) platformName, message };
		var ctorTypes = new [] { PlatformEnum, typeof (string) };
#else
		var ctorValues = new object [] { (byte) platformName, (byte) 0xff, message };
		var ctorTypes = new [] { PlatformEnum, PlatformArch, typeof (string) };
#endif
		return CreateNewAttribute (UnavailableAttributeType, ctorTypes, ctorValues);
	}

	public static AvailabilityBaseAttribute CreateNoVersionSupportedAttribute (PlatformName platform)
	{
		switch (platform) {
		case PlatformName.iOS:
		case PlatformName.TvOS:
		case PlatformName.MacOSX:
		case PlatformName.MacCatalyst:
			return new IntroducedAttribute (platform);
		case PlatformName.WatchOS:
			throw new InvalidOperationException ("CreateNoVersionSupportedAttribute for WatchOS never makes sense");
		default:
			throw new NotImplementedException ();
		}
	}

	public static AvailabilityBaseAttribute CreateUnsupportedAttribute (PlatformName platform)
	{
		switch (platform) {
		case PlatformName.iOS:
		case PlatformName.MacCatalyst:
		case PlatformName.MacOSX:
		case PlatformName.TvOS:
			return new UnavailableAttribute (platform);
		case PlatformName.WatchOS:
			throw new InvalidOperationException ("CreateUnsupportedAttribute for WatchOS never makes sense");
		default:
			throw new NotImplementedException ();
		}
	}

	static AvailabilityBaseAttribute CloneFromOtherPlatform (AvailabilityBaseAttribute attr, PlatformName platform)
	{
		if (attr.Version is null) {
			switch (attr.AvailabilityKind) {
			case AvailabilityKind.Introduced:
				return new IntroducedAttribute (platform, message: attr.Message);
			case AvailabilityKind.Deprecated:
				return new DeprecatedAttribute (platform, message: attr.Message);
			case AvailabilityKind.Obsoleted:
				return new ObsoletedAttribute (platform, message: attr.Message);
			case AvailabilityKind.Unavailable:
				return new UnavailableAttribute (platform, message: attr.Message);
			default:
				throw new NotImplementedException ();
			}
		}

		// Due to the absurd API of Version, you can not pass a -1 to the build constructor
		// nor can you coerse to 0, as that will fail with "16.0.0 <= 16.0" => false in the registrar
		// So determine if the build is -1, and use the 2 or 3 param ctor...
		var version = attr.Version;
		var minimum = Xamarin.SdkVersions.GetMinVersion (platform.AsApplePlatform ());
		if (version < minimum)
			version = minimum;
		if (version.Build == -1) {
			switch (attr.AvailabilityKind) {
			case AvailabilityKind.Introduced:
				return new IntroducedAttribute (platform, version.Major, version.Minor, message: attr.Message);
			case AvailabilityKind.Deprecated:
				return new DeprecatedAttribute (platform, version.Major, version.Minor, message: attr.Message);
			case AvailabilityKind.Obsoleted:
				return new ObsoletedAttribute (platform, version.Major, version.Minor, message: attr.Message);
			case AvailabilityKind.Unavailable:
				return new UnavailableAttribute (platform, message: attr.Message);
			default:
				throw new NotImplementedException ();
			}
		}

		switch (attr.AvailabilityKind) {
		case AvailabilityKind.Introduced:
			return new IntroducedAttribute (platform, version.Major, version.Minor, version.Build, message: attr.Message);
		case AvailabilityKind.Deprecated:
			return new DeprecatedAttribute (platform, version.Major, version.Minor, version.Build, message: attr.Message);
		case AvailabilityKind.Obsoleted:
			return new ObsoletedAttribute (platform, version.Major, version.Minor, version.Build, message: attr.Message);
		case AvailabilityKind.Unavailable:
			return new UnavailableAttribute (platform, message: attr.Message);
		default:
			throw new NotImplementedException ();
		}
	}

	// Find the introduced attribute with the highest version between the target list and the additions.
	// If the destination list has an introduced attribute, replace it if it's not the one with the highest version
	// If the destination list does not have an introduced attribute, then add one if there's one in the additions and there's not already an unavailable attribute.
	public static void FindHighestIntroducedAttributes (List<AvailabilityBaseAttribute> dest, IEnumerable<AvailabilityBaseAttribute> additions)
	{
		if (!additions.Any ())
			return;

		foreach (var platform in BindingTouch.AllPlatformNames) {
			// find the availability attribute with the highest version we're trying to add
			var latestAddition = additions
				.Where (v => v.AvailabilityKind == AvailabilityKind.Introduced && v.Platform == platform)
				.OrderBy (v => v.Version)
				.LastOrDefault ();
			if (latestAddition is null)
				continue;

			var added = CloneFromOtherPlatform (latestAddition, latestAddition.Platform);
			var idx = dest.FindIndex (v => v.Platform == platform && v.AvailabilityKind == AvailabilityKind.Introduced);
			if (idx == -1) {
				// no existing introduced attribute: add it unless there's already an unavailable attribute
				if (!dest.Any (v => v.Platform == platform && v.AvailabilityKind == AvailabilityKind.Unavailable))
					dest.Add (added);
			} else if (added.Version > dest [idx].Version) {
				// replace any existing introduced attribute if the existing version is lower than the added one
				dest [idx] = added;
			}
		}
	}

	static bool IsValidToCopyTo (List<AvailabilityBaseAttribute> dest, AvailabilityBaseAttribute addition, bool allowIntroducedOnUnavailable = false)
	{
		// If we are duplicating an existing attribute
		if (dest.Any (d => d.Platform == addition.Platform && d.AvailabilityKind == addition.AvailabilityKind))
			return false;
		// If we are introduced and there is already an Unavailable 
		return allowIntroducedOnUnavailable
			   || (addition is not IntroducedAttribute
				   || !dest.Any (d => d.Platform == addition.Platform && d.AvailabilityKind == AvailabilityKind.Unavailable));
	}

	public static void CopyValidAttributes (List<AvailabilityBaseAttribute> dest, IEnumerable<AvailabilityBaseAttribute> additions)
	{
		foreach (var addition in additions.Where (a => IsValidToCopyTo (dest, a))) {
			dest.Add (CloneFromOtherPlatform (addition, addition.Platform));
		}
	}
}
