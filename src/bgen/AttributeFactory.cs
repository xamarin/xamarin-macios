using System;
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
		var ctorValues = new object [] { (byte) platform, major, minor, message };
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
		var ctorValues = new object [] { (byte) platform, message };
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
		var ctorValues = new object [] { (byte) platformName, message };
		var ctorTypes = new [] { PlatformEnum, typeof (string) };
#else
		var ctorValues = new object [] { (byte) platformName, (byte) 0xff, message };
		var ctorTypes = new [] { PlatformEnum, PlatformArch, typeof (string) };
#endif
		return CreateNewAttribute (UnavailableAttributeType, ctorTypes, ctorValues);
	}
}
