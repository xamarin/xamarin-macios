#if !NET
using System;
using PlatformArchitecture = ObjCRuntime.PlatformArchitecture;
using PlatformName = ObjCRuntime.PlatformName;

// These _must_ be in a less nested namespace than the copies they are shadowing in PlatformAvailability.cs
// Since those are in ObjcRuntime these must be global
#if COREBUILD
public
#endif
sealed class MacAttribute : ObjCRuntime.IntroducedAttribute {
	public MacAttribute (byte major, byte minor)
		: base (PlatformName.MacOSX, (int) major, (int) minor)
	{
	}

	[Obsolete ("Use the overload that takes '(major, minor)', since macOS is always 64-bit.")]
	public MacAttribute (byte major, byte minor, bool onlyOn64 = false)
		: base (PlatformName.MacOSX, (int) major, (int) minor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
	{
	}

	/* This variant can _not_ exist as the AttributeConversionManager.ConvertPlatformAttribute sees PlatformArchitecture as a byte
	   and byte,byte,byte already exists below
	public MacAttribute (byte major, byte minor, PlatformArchitecture arch)
		: base (PlatformName.MacOSX, (int)major, (int)minor, arch)
	{
	}
	*/

	public MacAttribute (byte major, byte minor, byte subminor)
		: base (PlatformName.MacOSX, (int) major, (int) minor, subminor)
	{
	}

	[Obsolete ("Use the overload that takes '(major, minor, subminor)', since macOS is always 64-bit.")]
	public MacAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
		: base (PlatformName.MacOSX, (int) major, (int) minor, (int) subminor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
	{
	}

	public MacAttribute (byte major, byte minor, byte subminor, PlatformArchitecture arch)
		: base (PlatformName.MacOSX, (int) major, (int) minor, (int) subminor, arch)
	{
	}
}
#if COREBUILD
public
#endif
sealed class iOSAttribute : ObjCRuntime.IntroducedAttribute {
	public iOSAttribute (byte major, byte minor)
		: base (PlatformName.iOS, (int) major, (int) minor)
	{
	}

	[Obsolete ("Use the overload that takes '(major, minor)', since iOS is always 64-bit.")]
	public iOSAttribute (byte major, byte minor, bool onlyOn64 = false)
		: base (PlatformName.iOS, (int) major, (int) minor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
	{
	}

	public iOSAttribute (byte major, byte minor, byte subminor)
		: base (PlatformName.iOS, (int) major, (int) minor, subminor)
	{
	}

	[Obsolete ("Use the overload that takes '(major, minor, subminor)', since iOS is always 64-bit.")]
	public iOSAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
		: base (PlatformName.iOS, (int) major, (int) minor, (int) subminor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
	{
	}
}

#if COREBUILD
public
#endif
enum Platform : ulong {
	None = 0x0,
	// Processed in generator-attribute-manager.cs
	//            0xT000000000MMmmss
	iOS_Version = 0x0000000000ffffff,
	iOS_2_0 = 0x0000000000020000,
	iOS_2_2 = 0x0000000000020200,
	iOS_3_0 = 0x0000000000030000,
	iOS_3_1 = 0x0000000000030100,
	iOS_3_2 = 0x0000000000030200,
	iOS_4_0 = 0x0000000000040000,
	iOS_4_1 = 0x0000000000040100,
	iOS_4_2 = 0x0000000000040200,
	iOS_4_3 = 0x0000000000040300,
	iOS_5_0 = 0x0000000000050000,
	iOS_5_1 = 0x0000000000050100,
	iOS_6_0 = 0x0000000000060000,
	iOS_6_1 = 0x0000000000060100,
	iOS_7_0 = 0x0000000000070000,
	iOS_7_1 = 0x0000000000070100,
	iOS_8_0 = 0x0000000000080000,
	iOS_8_1 = 0x0000000000080100,
	iOS_8_2 = 0x0000000000080200,
	iOS_8_3 = 0x0000000000080300,
	iOS_8_4 = 0x0000000000080400,
	iOS_9_0 = 0x0000000000090000,
	iOS_9_1 = 0x0000000000090100,
	iOS_9_2 = 0x0000000000090200,
	iOS_9_3 = 0x0000000000090300,
	iOS_10_0 = 0x00000000000a0000,
	iOS_11_0 = 0x00000000000b0000,

	//            0xT000000000MMmmss
	Mac_Version = 0x1000000000ffffff,
	Mac_10_0 = 0x1000000000000000,
	Mac_10_1 = 0x1000000000010000,
	Mac_10_2 = 0x1000000000020000,
	Mac_10_3 = 0x1000000000030000,
	Mac_10_4 = 0x1000000000040000,
	Mac_10_5 = 0x1000000000050000,
	Mac_10_6 = 0x1000000000060000,
	Mac_10_7 = 0x1000000000070000,
	Mac_10_8 = 0x1000000000080000,
	Mac_10_9 = 0x1000000000090000,
	Mac_10_10 = 0x10000000000a0000,
	Mac_10_10_3 = 0x10000000000a0300,
	Mac_10_11 = 0x10000000000b0000,
	Mac_10_11_3 = 0x10000000000b0300,
	Mac_10_12 = 0x10000000000c0000,
	Mac_10_13 = 0x10000000000d0000,
	Mac_10_14 = 0x10000000000e0000,

	//              0xT000000000MMmmss
	Watch_Version = 0x2000000000ffffff,
	Watch_1_0 = 0x2000000000010000,
	Watch_2_0 = 0x2000000000020000,
	Watch_3_0 = 0x2000000000030000,
	Watch_4_0 = 0x2000000000040000,

	//             0xT000000000MMmmss
	TV_Version = 0x3000000000ffffff,
	TV_9_0 = 0x3000000000090000,
	TV_10_0 = 0x30000000000a0000,
	TV_11_0 = 0x30000000000b0000,
}

#if COREBUILD // As this does not derive from IntroducedAttribute or friends it can not be used in non-generated code
[AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
public class AvailabilityAttribute : Attribute
{
	public AvailabilityAttribute () { }

	public Platform Introduced;
	public Platform Deprecated;
	public Platform Obsoleted;
	public Platform Unavailable;
	public string Message;

	public AvailabilityAttribute (
		Platform introduced,
		Platform deprecated = Platform.None,
		Platform obsoleted = Platform.None,
		Platform unavailable = Platform.None)
	{
		Introduced = introduced;
		Deprecated = deprecated;
		Obsoleted = obsoleted;
		Unavailable = unavailable;
	}
}
#endif
#endif // !NET
