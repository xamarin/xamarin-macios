//
// PlatformAvailability2.cs: implements new AvailabilityBaseAttribute
// and its subclasses: Introduced, Deprecated, Obsoleted, and
// Unavailable.
//
// This addresses scalability issues with the AvailabilityAttribute
// introduced originally for XAMCORE_2_0 where the Platform enum
// cannot cleanly scale to other platforms (e.g. WatchOS).
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

namespace ObjCRuntime
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

