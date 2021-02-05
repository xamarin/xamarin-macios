//
// PlatformAvailability2.cs: implements new AvailabilityBaseAttribute
// and its subclasses: Introduced, Deprecated, Obsoleted, and
// Unavailable.
//
// This addresses scalability issues with the AvailabilityAttribute
// introduced originally for the Unified profile where the Platform enum
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
		TvOS,
		MacCatalyst,
		[Obsolete ("Use 'MacCatalyst' instead.")]
		UIKitForMac = MacCatalyst, // temporary
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
#if NET
			switch (AvailabilityKind) {
			case AvailabilityKind.Introduced:
				builder.Append ("[SupportedOSPlatform (\"");
				break;
			case AvailabilityKind.Obsoleted:
				switch (Platform) {
				case PlatformName.iOS:
					builder.AppendLine ("#if __IOS__");
					break;
				case PlatformName.TvOS:
					builder.AppendLine ("#if __TVOS__");
					break;
				case PlatformName.WatchOS:
					builder.AppendLine ("#if __WATCHOS__");
					break;
				case PlatformName.MacOSX:
					builder.AppendLine ("#if __MACOS__");
					break;
				case PlatformName.MacCatalyst:
					builder.AppendLine ("#if __MACCATALYST__");
					break;
				}
				builder.Append ("[Obsolete (\"Starting with ");
				break;
			case AvailabilityKind.Deprecated:
			case AvailabilityKind.Unavailable:
				builder.Append ("[UnsupportedOSPlatform (\"");
				break;
			}

			switch (Platform) {
			case PlatformName.iOS:
				builder.Append ("ios");
				break;
			case PlatformName.TvOS:
				builder.Append ("tvos");
				break;
			case PlatformName.WatchOS:
				builder.Append ("watchos");
				break;
			case PlatformName.MacOSX:
				builder.Append ("macos"); // no 'x'
				break;
			case PlatformName.MacCatalyst:
				builder.Append ("maccatalyst");
				break;
			}

			if (Version != null) {
				builder.Append (Version.Major).Append ('.').Append (Version.Minor);
				if (Version.Build >= 0)
					builder.Append ('.').Append (Version.Build);
			}

			switch (AvailabilityKind) {
			case AvailabilityKind.Obsoleted:
				if (!String.IsNullOrEmpty (Message))
					builder.Append (' ').Append (Message);
				else
					builder.Append ('.'); // intro check messages to they end with a '.'
				// TODO add a URL (wiki?) and DiagnosticId (one per platform?) for documentation
				builder.AppendLine ("\", DiagnosticId = \"BI1234\", UrlFormat = \"https://github.com/xamarin/xamarin-macios/wiki/Obsolete\")]");
				builder.Append ("#endif");
				break;
			case AvailabilityKind.Introduced:
			case AvailabilityKind.Deprecated:
			case AvailabilityKind.Unavailable:
				builder.Append ("\")]");
				break;
			}
#else
			builder.AppendFormat ("[{0} ({1}.{2}", AvailabilityKind, nameof (PlatformName), Platform);
			
			if (Version != null) {
				builder.AppendFormat (", {0},{1}", Version.Major, Version.Minor);
				if (Version.Build >= 0)
					builder.AppendFormat (",{0}", Version.Build);
			}

			if (Architecture != PlatformArchitecture.None)
				builder.Append (", ObjCRuntime.PlatformArchitecture.").Append (Architecture);

			if (Message != null)
				builder.AppendFormat (", message: \"{0}\"", Message.Replace ("\"", "\"\""));

			builder.Append (")]");
#endif
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

#if !XAMCORE_4_0
		[Obsolete ("Use the overload that takes '(major, minor)', since tvOS is always 64-bit.")]
		public TVAttribute (byte major, byte minor, bool onlyOn64 = false)
			: base (PlatformName.TvOS, (int)major, (int)minor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}
#endif

		public TVAttribute (byte major, byte minor, byte subminor)
			: base (PlatformName.TvOS, (int)major, (int)minor, subminor)
		{
		}

#if !XAMCORE_4_0
		[Obsolete ("Use the overload that takes '(major, minor, subminor)', since tvOS is always 64-bit.")]
		public TVAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
			: base (PlatformName.TvOS, (int)major, (int)minor, (int)subminor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}
#endif
	}
	
	public sealed class WatchAttribute : IntroducedAttribute
	{
		public WatchAttribute (byte major, byte minor)
			: base (PlatformName.WatchOS, (int)major, (int)minor)
		{
		}

#if !XAMCORE_4_0
		[Obsolete ("Use the overload that takes '(major, minor)', since watchOS is never 64-bit.")] // not yet at least
		public WatchAttribute (byte major, byte minor, bool onlyOn64 = false)
			: base (PlatformName.WatchOS, (int)major, (int)minor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}
#endif

		public WatchAttribute (byte major, byte minor, byte subminor)
			: base (PlatformName.WatchOS, (int)major, (int)minor, subminor)
		{
		}

#if !XAMCORE_4_0
		[Obsolete ("Use the overload that takes '(major, minor)', since watchOS is never 64-bit.")] // not yet at least
		public WatchAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
			: base (PlatformName.WatchOS, (int)major, (int)minor, (int)subminor, onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All)
		{
		}
#endif
	}

	public sealed class MacCatalystAttribute : IntroducedAttribute
	{
		public MacCatalystAttribute (byte major, byte minor)
			: base (PlatformName.MacCatalyst, (int) major, (int) minor)
		{
		}

		public MacCatalystAttribute (byte major, byte minor, byte subminor)
			: base (PlatformName.MacCatalyst, (int) major, (int) minor, subminor)
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

	public sealed class NoMacCatalystAttribute : UnavailableAttribute
	{
		public NoMacCatalystAttribute ()
			: base (PlatformName.MacCatalyst)
		{
		}
	}
}

