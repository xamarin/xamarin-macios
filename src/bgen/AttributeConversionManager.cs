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

		object? [] ctorValues;
		Type [] ctorTypes;

		switch (attribute.ConstructorArguments.Count) {
		case 2:
			if (constructorArguments [0] is byte &&
				constructorArguments [1] is byte) {
#if NET
				ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], null };
				ctorTypes = new [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (string) };
#else
				ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (byte) 0xff, null };
				ctorTypes = new [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
#endif
				break;
			}
			throw new NotImplementedException (unknownFormatError ());
		case 3:
			if (constructorArguments [0] is byte &&
				constructorArguments [1] is byte &&
				constructorArguments [2] is byte) {
#if NET
				ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (int) (byte) constructorArguments [2], null };
				ctorTypes = new [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (int), typeof (string) };
#else
				ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (int) (byte) constructorArguments [2], (byte) 0xff, null };
				ctorTypes = new [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
#endif
				break;
			}
#if !NET
			if (constructorArguments [0] is byte &&
				constructorArguments [1] is byte &&
				constructorArguments [2] is bool) {
				byte arch = (bool) constructorArguments [2] ? (byte) 2 : (byte) 0xff;
				ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], arch, null };
				ctorTypes = new [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
				break;
			}
#endif
			throw new NotImplementedException (unknownFormatError ());
#if !NET
		case 4:
			if (constructorArguments [0] is byte &&
				constructorArguments [1] is byte &&
				constructorArguments [2] is byte &&
				constructorArguments [3] is bool) {
				byte arch = (bool) constructorArguments [3] ? (byte) 2 : (byte) 0xff;
				ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (int) (byte) constructorArguments [2], arch, null };
				ctorTypes = new [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
				break;
			}
			if (constructorArguments [0] is byte &&
				constructorArguments [1] is byte &&
				constructorArguments [2] is byte &&
				constructorArguments [3] is byte /* ObjCRuntime.PlatformArchitecture */) {
				ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (int) (byte) constructorArguments [2], constructorArguments [3], null };
				ctorTypes = new [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
				break;
			}

			throw new NotImplementedException (unknownFormatError ());
#endif
		default:
			throw new NotImplementedException ($"Unknown count {attribute.ConstructorArguments.Count} {createErrorMessage ()}");
		}

		return AttributeFactory.CreateNewAttribute (AttributeFactory.IntroducedAttributeType, ctorTypes, ctorValues);
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
				yield return AttributeFactory.CreateNewIntroducedAttribute (availInfo.Platform, availInfo.Major, availInfo.Minor, message: message);
				continue;
			}
			case "Deprecated": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateDeprecatedAttribute (availInfo.Platform, availInfo.Major, availInfo.Minor, message: message);
				continue;
			}
			case "Obsoleted": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateObsoletedAttribute (availInfo.Platform, availInfo.Major, availInfo.Minor, message: message);
				continue;
			}
			case "Unavailable": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateUnavailableAttribute (availInfo.Platform, message: message);
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
