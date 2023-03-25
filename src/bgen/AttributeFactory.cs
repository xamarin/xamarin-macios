using System;
using System.Collections.Generic;
using System.Linq;
using ObjCRuntime;

#nullable enable

public static partial class AttributeFactory {

	public static T CreateNewAttribute<T> (Type [] ctorTypes, object? [] ctorValues)
		where T : Attribute
	{
		var attribType = typeof (T);
		var ctor = attribType.GetConstructor (ctorTypes);
		if (ctor is null)
			throw ErrorHelper.CreateError (1058, attribType.FullName);

		return (T) ctor.Invoke (ctorValues);
	}

	public static T CreateNewAttribute<T> (PlatformName platform, int major, int minor, string? message = null)
		where T : Attribute
	{
		var args = new ConstructorArguments (platform, major, minor, message);
		return CreateNewAttribute<T> (args.GetCtorTypes (), args.GetCtorValues ());
	}

	public static T CreateNewAttribute<T> (PlatformName platform, int major, int minor, int build, string? message = null)
		where T : Attribute
	{
		var args = new ConstructorArguments (platform, major, minor, build, message);
		return CreateNewAttribute<T> (args.GetCtorTypes (), args.GetCtorValues ());
	}

	public static T CreateNewAttribute<T> (PlatformName platform, string? message = null) where T : Attribute
	{
		var args = new ConstructorArguments (platform, message);
		return CreateNewAttribute<T> (args.GetCtorTypes (), args.GetCtorValues ());
	}

	public static IntroducedAttribute CreateNoVersionSupportedAttribute (PlatformName platform)
	{
		switch (platform) {
		case PlatformName.iOS:
		case PlatformName.TvOS:
		case PlatformName.MacOSX:
		case PlatformName.MacCatalyst:
			return new (platform);
		case PlatformName.WatchOS:
			throw new InvalidOperationException ("CreateNoVersionSupportedAttribute for WatchOS never makes sense");
		default:
			throw new NotImplementedException ();
		}
	}

	public static UnavailableAttribute CreateUnsupportedAttribute (PlatformName platform)
	{
		switch (platform) {
		case PlatformName.iOS:
		case PlatformName.MacCatalyst:
		case PlatformName.MacOSX:
		case PlatformName.TvOS:
			return new (platform);
		case PlatformName.WatchOS:
			throw new InvalidOperationException ("CreateUnsupportedAttribute for WatchOS never makes sense");
		default:
			throw new NotImplementedException ();
		}
	}

	public static AvailabilityBaseAttribute CloneFromOtherPlatform (AvailabilityBaseAttribute attr, PlatformName platform)
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
