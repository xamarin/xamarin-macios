//
// PlatformAvailability2.cs: implements new AvailabilityBaseAttribute
// and its subclasses: Introduced, Deprecated, Obsoleted, and
// Unavailable.
//
// This addresses scalability issues with the AvailabilityAttribute
// introduced originally for XAMCORE_2_0 where the Platform enum
// cannot cleanly scale to other platforms (e.g. WatchOS).
//
// The pmcs preprocessor translates all legacy availability
// attributes into these new ones for all builds.
//
// Used by unit tests to automatically skip selectors not available
// on the host platform and can be used by tooling (IDE) to hide
// APIs not available on the target platform.
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Text;

namespace XamCore.ObjCRuntime
{
	[Flags]
	public enum PlatformArchitecture : byte
	{
		None = 0x00,
		Arch32 = 0x01,
		Arch64 = 0x02,
		All = 0xff
	}

	public enum PlatformName : byte
	{
		None,
		MacOSX,
		iOS,
		WatchOS,
		TvOS
	}

	public enum AvailabilityKind
	{
		Introduced,
		Deprecated,
		Obsoleted,
		Unavailable
	}

	[AttributeUsage (
		AttributeTargets.Class |
		AttributeTargets.Constructor |
		AttributeTargets.Delegate |
		AttributeTargets.Enum |
		AttributeTargets.Event |
		AttributeTargets.Field |
		AttributeTargets.Interface |
		AttributeTargets.Method |
		AttributeTargets.Property |
		AttributeTargets.Struct,
		AllowMultiple = true
	)]
	public abstract class AvailabilityBaseAttribute : Attribute
	{
		public AvailabilityKind AvailabilityKind { get; private set; }
		public PlatformName Platform { get; private set; }
		public Version Version { get; private set; }
		public PlatformArchitecture Architecture { get; private set; }
		public string Message { get; private set; }

		internal AvailabilityBaseAttribute ()
		{
		}

		internal AvailabilityBaseAttribute (
			AvailabilityKind availabilityKind,
			PlatformName platform,
			Version version,
			PlatformArchitecture architecture,
			string message)
		{
			AvailabilityKind = availabilityKind;
			Platform = platform;
			Version = version;
			Architecture = architecture;
			Message = message;
		}

		public override string ToString ()
		{
			var builder = new StringBuilder ();
			builder.AppendFormat ("[{0} ({1}.{2}", AvailabilityKind, nameof (PlatformName), Platform);
			
			if (Version != null) {
				builder.AppendFormat (", {0},{1}", Version.Major, Version.Minor);
				if (Version.Build >= 0)
					builder.AppendFormat (",{0}", Version.Build);
			}

			if (Architecture != PlatformArchitecture.None)
				builder.AppendFormat (", {0}.{1}", nameof (PlatformArchitecture), Architecture);

			if (Message != null)
				builder.AppendFormat (", message: \"{0}\"", Message.Replace ("\"", "\"\""));

			builder.Append (")]");
			return builder.ToString ();
		}
	}

	public class IntroducedAttribute : AvailabilityBaseAttribute
	{
		public IntroducedAttribute (PlatformName platform,
			PlatformArchitecture architecture = PlatformArchitecture.None,
			string message = null)
			: base (AvailabilityKind.Introduced, platform, null, architecture, message)
		{
		}

		public IntroducedAttribute (PlatformName platform, int majorVersion, int minorVersion,
			PlatformArchitecture architecture = PlatformArchitecture.None,
			string message = null)
			: base (AvailabilityKind.Introduced,
				platform, new Version (majorVersion, minorVersion),
				architecture, message)
		{
		}

		public IntroducedAttribute (PlatformName platform, int majorVersion, int minorVersion, int subminorVersion,
			PlatformArchitecture architecture = PlatformArchitecture.None,
			string message = null)
			: base (AvailabilityKind.Introduced,
				platform, new Version (majorVersion, minorVersion, subminorVersion),
				architecture, message)
		{
		}
	}

	public sealed class DeprecatedAttribute : AvailabilityBaseAttribute
	{
		public DeprecatedAttribute (PlatformName platform,
			PlatformArchitecture architecture = PlatformArchitecture.None,
			string message = null)
			: base (AvailabilityKind.Deprecated, platform, null, architecture, message)
		{
		}

		public DeprecatedAttribute (PlatformName platform, int majorVersion, int minorVersion,
			PlatformArchitecture architecture = PlatformArchitecture.None,
			string message = null)
			: base (AvailabilityKind.Deprecated,
				platform, new Version (majorVersion, minorVersion),
				architecture, message)
		{
		}

		public DeprecatedAttribute (PlatformName platform, int majorVersion, int minorVersion, int subminorVersion,
			PlatformArchitecture architecture = PlatformArchitecture.None,
			string message = null)
			: base (AvailabilityKind.Deprecated,
				platform, new Version (majorVersion, minorVersion, subminorVersion),
				architecture, message)
		{
		}
	}

	public sealed class ObsoletedAttribute : AvailabilityBaseAttribute
	{
		public ObsoletedAttribute (PlatformName platform,
			PlatformArchitecture architecture = PlatformArchitecture.None,
			string message = null)
			: base (AvailabilityKind.Obsoleted, platform, null, architecture, message)
		{
		}

		public ObsoletedAttribute (PlatformName platform, int majorVersion, int minorVersion,
			PlatformArchitecture architecture = PlatformArchitecture.None,
			string message = null)
			: base (AvailabilityKind.Obsoleted,
				platform, new Version (majorVersion, minorVersion),
				architecture, message)
		{
		}

		public ObsoletedAttribute (PlatformName platform, int majorVersion, int minorVersion, int subminorVersion,
			PlatformArchitecture architecture = PlatformArchitecture.None,
			string message = null)
			: base (AvailabilityKind.Obsoleted,
				platform, new Version (majorVersion, minorVersion, subminorVersion),
				architecture, message)
		{
		}
	}

	public class UnavailableAttribute : AvailabilityBaseAttribute
	{
		public UnavailableAttribute (PlatformName platform,
			PlatformArchitecture architecture = PlatformArchitecture.All,
			string message = null)
			: base (AvailabilityKind.Unavailable,
				platform, null, architecture, message)
		{
		}
	}

	public sealed class TVAttribute : IntroducedAttribute
	{
		public TVAttribute (byte major, byte minor)
			: base (PlatformName.TvOS, (int)major, (int)minor)
		{
		}

		public TVAttribute (byte major, byte minor, bool onlyOn64 = false)
			: base (PlatformName.TvOS, (int)major, (int)minor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}

		public TVAttribute (byte major, byte minor, byte subminor)
			: base (PlatformName.TvOS, (int)major, (int)minor, subminor)
		{
		}

		public TVAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
			: base (PlatformName.TvOS, (int)major, (int)minor, (int)subminor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}
	}
	
	public sealed class WatchAttribute : IntroducedAttribute
	{
		public WatchAttribute (byte major, byte minor)
			: base (PlatformName.WatchOS, (int)major, (int)minor)
		{
		}

		public WatchAttribute (byte major, byte minor, bool onlyOn64 = false)
			: base (PlatformName.WatchOS, (int)major, (int)minor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}

		public WatchAttribute (byte major, byte minor, byte subminor)
			: base (PlatformName.WatchOS, (int)major, (int)minor, subminor)
		{
		}

		public WatchAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
			: base (PlatformName.WatchOS, (int)major, (int)minor, (int)subminor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}
	}

	public sealed class NoMacAttribute : UnavailableAttribute
	{
		public NoMacAttribute ()
			: base (PlatformName.MacOSX)
		{
		}
	}

	public sealed class NoiOSAttribute : UnavailableAttribute
	{
		public NoiOSAttribute ()
			: base (PlatformName.iOS)
		{
		}
	}

	public sealed class NoWatchAttribute : UnavailableAttribute
	{
		public NoWatchAttribute ()
			: base (PlatformName.WatchOS)
		{
		}
	}

	public sealed class NoTVAttribute : UnavailableAttribute
	{
		public NoTVAttribute ()
			: base (PlatformName.TvOS)
		{
		}
	}
}

namespace XamCore.ObjCRuntime.Extensions
{
	public sealed class MacAttribute : IntroducedAttribute
	{
		public MacAttribute (byte major, byte minor)
			: base (PlatformName.MacOSX, (int)major, (int)minor)
		{
		}

		public MacAttribute (byte major, byte minor, bool onlyOn64 = false)
			: base (PlatformName.MacOSX, (int)major, (int)minor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}

		public MacAttribute (byte major, byte minor, byte subminor)
			: base (PlatformName.MacOSX, (int)major, (int)minor, subminor)
		{
		}

		public MacAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
			: base (PlatformName.MacOSX, (int)major, (int)minor, (int)subminor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}

	}
	
	public sealed class iOSAttribute : IntroducedAttribute
	{
		public iOSAttribute (byte major, byte minor)
			: base (PlatformName.iOS, (int)major, (int)minor)
		{
		}

		public iOSAttribute (byte major, byte minor, bool onlyOn64 = false)
			: base (PlatformName.iOS, (int)major, (int)minor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}

		public iOSAttribute (byte major, byte minor, byte subminor)
			: base (PlatformName.iOS, (int)major, (int)minor, subminor)
		{
		}

		public iOSAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
			: base (PlatformName.iOS, (int)major, (int)minor, (int)subminor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}
	}
	
	public sealed class MavericksAttribute : IntroducedAttribute
	{
		public MavericksAttribute ()
			: base (PlatformName.MacOSX, 10, 9)
		{
		}
	}

	public sealed class MountainLionAttribute : IntroducedAttribute
	{
		public MountainLionAttribute ()
			: base (PlatformName.MacOSX, 10, 8)
		{
		}
	}

	public sealed class LionAttribute : IntroducedAttribute
	{
		public LionAttribute ()
			: base (PlatformName.MacOSX, 10, 7)
		{
		}
	}

	public enum Platform : ulong
	{
		None =  0x0,
		// Processed in generator-attribute-manager.cs
		//            0xT000000000MMmmss
		iOS_Version = 0x0000000000ffffff,
		iOS_2_0 =     0x0000000000020000,
		iOS_2_2 =     0x0000000000020200,
		iOS_3_0 =     0x0000000000030000,
		iOS_3_1 =     0x0000000000030100,
		iOS_3_2 =     0x0000000000030200,
		iOS_4_0 =     0x0000000000040000,
		iOS_4_1 =     0x0000000000040100,
		iOS_4_2 =     0x0000000000040200,
		iOS_4_3 =     0x0000000000040300,
		iOS_5_0 =     0x0000000000050000,
		iOS_5_1 =     0x0000000000050100,
		iOS_6_0 =     0x0000000000060000,
		iOS_6_1 =     0x0000000000060100,
		iOS_7_0 =     0x0000000000070000,
		iOS_7_1 =     0x0000000000070100,
		iOS_8_0 =     0x0000000000080000,
		iOS_8_1 =     0x0000000000080100,
		iOS_8_2 =     0x0000000000080200,
		iOS_8_3 =     0x0000000000080300,
		iOS_8_4 =     0x0000000000080400,
		iOS_9_0 =     0x0000000000090000,
		iOS_9_1 =     0x0000000000090100,
		iOS_9_2 =     0x0000000000090200,
		iOS_9_3 =     0x0000000000090300,
		iOS_10_0 =    0x00000000000a0000,
		iOS_11_0 =    0x00000000000b0000,

		//            0xT000000000MMmmss
		Mac_Version = 0x1000000000ffffff,
		Mac_10_0  =   0x1000000000000000,
		Mac_10_1  =   0x1000000000010000,
		Mac_10_2  =   0x1000000000020000,
		Mac_10_3  =   0x1000000000030000,
		Mac_10_4  =   0x1000000000040000,
		Mac_10_5  =   0x1000000000050000,
		Mac_10_6  =   0x1000000000060000,
		Mac_10_7  =   0x1000000000070000,
		Mac_10_8  =   0x1000000000080000,
		Mac_10_9  =   0x1000000000090000,
		Mac_10_10 =   0x10000000000a0000,
		Mac_10_10_3 = 0x10000000000a0300,
		Mac_10_11   = 0x10000000000b0000,
		Mac_10_11_3 = 0x10000000000b0300,
		Mac_10_12   = 0x10000000000c0000,
		Mac_10_13   = 0x10000000000d0000,

		//              0xT000000000MMmmss
		Watch_Version = 0x2000000000ffffff,
		Watch_1_0 =     0x2000000000010000,
		Watch_2_0 =     0x2000000000020000,
		Watch_3_0 =     0x2000000000030000,
		Watch_4_0 =     0x2000000000040000,

		//             0xT000000000MMmmss
		TV_Version =   0x3000000000ffffff,
		TV_9_0 =       0x3000000000090000,
		TV_10_0 =      0x30000000000a0000,
		TV_11_0 =      0x30000000000b0000,
	}
	
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

}
