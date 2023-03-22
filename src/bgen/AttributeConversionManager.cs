using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjCRuntime;

#nullable enable

public static class AttributeConversionManager {
	public static Attribute ConvertPlatformAttribute (CustomAttributeData attribute, PlatformName platform)
	{
		var constructorArguments = new object [attribute.ConstructorArguments.Count];
		for (int i = 0; i < attribute.ConstructorArguments.Count; ++i)
			constructorArguments [i] = attribute.ConstructorArguments [i].Value!;

		Func<string> createErrorMessage = () => {
			var b = new System.Text.StringBuilder (" Types { ");
			for (int i = 0; i < constructorArguments.Length; ++i)
				b.Append (constructorArguments [i].GetType ().ToString () + " ");
			b.Append ("}");
			return b.ToString ();
		};

		Func<string> unknownFormatError = () => $"Unknown format for old style availability attribute {attribute.GetAttributeType ().FullName} {attribute.ConstructorArguments.Count} {createErrorMessage ()}";

		if (AttributeFactory.ConstructorArguments.TryGetCtorArguments (
				constructorArguments, platform, out var ctorValues, out var ctorTypes)) {
			return AttributeFactory.CreateNewAttribute<IntroducedAttribute> (ctorTypes!, ctorValues!);
		}

		throw new NotImplementedException (unknownFormatError ());
	}

	struct ParsedAvailabilityInfo {
		public readonly PlatformName Platform;
		public readonly int Major;
		public readonly int Minor;

		public ParsedAvailabilityInfo (PlatformName platform, int major, int minor)
		{
			Platform = platform;
			Major = major;
			Minor = minor;
		}

		public ParsedAvailabilityInfo (PlatformName platform)
		{
			Platform = platform;
			Major = -1;
			Minor = -1;
		}
	}

	static PlatformName ParsePlatformName (string s)
	{
		switch (s) {
		case "iOS":
			return PlatformName.iOS;
		case "Mac":
			return PlatformName.MacOSX;
		case "Watch":
			return PlatformName.WatchOS;
		case "TV":
			return PlatformName.TvOS;
		default:
			return PlatformName.None;
		}
	}

	static ParsedAvailabilityInfo DetermineOldAvailabilityVersion (CustomAttributeNamedArgument arg)
	{
		var enumName = Enum.GetName (typeof (Platform), (ulong) arg.TypedValue.Value!);
		if (enumName is null)
			throw new NotImplementedException ($"Unknown version format \"{enumName}\" in DetermineOldAvailabilityVersion. Are there two values | together?");

		string [] enumParts = enumName.Split ('_');
		switch (enumParts.Length) {
		case 1:
			if (enumName == "None")
				return new ParsedAvailabilityInfo (PlatformName.None);
			break;
		case 2: {
			if (enumParts [1] != "Version")
				break;

			PlatformName platform = ParsePlatformName (enumParts [0]);
			if (platform == PlatformName.None)
				break;
			return new (platform);
		}
		case 3: {
			PlatformName platform = ParsePlatformName (enumParts [0]);
			if (platform == PlatformName.None)
				break;
			int major = int.Parse (enumParts [1]);
			int minor = int.Parse (enumParts [2]);

			return new (platform, major, minor);
		}
		}
		throw new NotImplementedException ($"Unknown version format \"{enumName}\" in DetermineOldAvailabilityVersion");
	}

	public static IEnumerable<Attribute> ConvertAvailability (CustomAttributeData attribute)
	{
		string? message = null;
		if (attribute.NamedArguments is null)
			yield break;

		if (attribute.NamedArguments.Any (x => x.MemberName == "Message"))
			message = (string) attribute.NamedArguments.First (x => x.MemberName == "Message").TypedValue.Value!;

		foreach (var arg in attribute.NamedArguments) {
			switch (arg.MemberName) {
			case "Introduced": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateNewAttribute<IntroducedAttribute> (availInfo.Platform, availInfo.Major, availInfo.Minor, message: message);
				continue;
			}
			case "Deprecated": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateNewAttribute<DeprecatedAttribute> (availInfo.Platform, availInfo.Major, availInfo.Minor, message: message);
				continue;
			}
			case "Obsoleted": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateNewAttribute<ObsoletedAttribute> (availInfo.Platform, availInfo.Major, availInfo.Minor, message: message);
				continue;
			}
			case "Unavailable": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateNewAttribute<UnavailableAttribute> (availInfo.Platform, message: message);
				continue;
			}
			case "Message":
				continue;
			default:
				throw new NotImplementedException ($"ConvertAvailability found unknown named argument {arg.MemberName}");
			}
		}
	}
}
