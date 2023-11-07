//
// PlatformAvailability.cs: implements the AvailabilityAttribute
// that mirrors the native Clang one so all platform availability
// metadata can be preserved on the managed side.
//
// Used by unit tests to automatically skip selectors not available
// on the host platform and can be used by tooling (IDE) to hide
// APIs not available on the target platform.
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc.

#if COREBUILD || (!XAMCORE_3_0 && !NET)

using System;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if !COREBUILD
#if MONOMAC
using Foundation;
#else
using UIKit;
#endif
#endif

namespace ObjCRuntime {
	// iOS versions are stored in the lower 4 bytes an Mac versions in
	// the higher 4 bytes. Each 4 byte version has the format AAJJNNSS
	// where AA is the supported architecture flags, JJ is the maJor
	// version, NN is the miNor version, and SS is the Subminor version.
	//
	// Only iOS and Mac versions and architectures can be ORed together.
	//
	[Flags]
#if !COREBUILD
	[Obsolete ("Use [Introduced|Deprecated|Obsoleted|Unavailable] attributes with PlatformName.")]
#endif
	public enum Platform : ulong {
		None = 0,

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

		// NOTE: Update PlatformHelper.IsValid when adding a version

		Mac_10_0 = 0x000a000000000000,
		Mac_10_1 = 0x000a010000000000,
		Mac_10_2 = 0x000a020000000000,
		Mac_10_3 = 0x000a030000000000,
		Mac_10_4 = 0x000a040000000000,
		Mac_10_5 = 0x000a050000000000,
		Mac_10_6 = 0x000a060000000000,
		Mac_10_7 = 0x000a070000000000,
		Mac_10_8 = 0x000a080000000000,
		Mac_10_9 = 0x000a090000000000,
		Mac_10_10 = 0x000a0a0000000000,
		Mac_10_10_3 = 0x000a0a0300000000,
		Mac_10_11 = 0x000a0b0000000000,
		Mac_10_11_3 = 0x000a0b0300000000,
		Mac_10_12 = 0x000a0c0000000000,

		// NOTE: Update PlatformHelper.IsValid when adding a version

		iOS_Version = 0x0000000000ffffff,
		Mac_Version = 0x00ffffff00000000,

		Mac_Arch32 = 0x0100000000000000,
		Mac_Arch64 = 0x0200000000000000,
		Mac_Arch = 0xff00000000000000,

		iOS_Arch32 = 0x0000000001000000,
		iOS_Arch64 = 0x0000000002000000,
		iOS_Arch = 0x00000000ff000000
	}

	[Obsolete ("Use [Introduced|Deprecated|Obsoleted|Unavailable] attributes with PlatformName.")]
	public static class PlatformHelper {
		public static bool IsValid (this Platform platform)
		{
#pragma warning disable 0618
			switch (ToMacVersion (platform)) {
			case Platform.None:
			case Platform.Mac_Version:
			case Platform.Mac_10_0:
			case Platform.Mac_10_1:
			case Platform.Mac_10_2:
			case Platform.Mac_10_3:
			case Platform.Mac_10_4:
			case Platform.Mac_10_5:
			case Platform.Mac_10_6:
			case Platform.Mac_10_7:
			case Platform.Mac_10_8:
			case Platform.Mac_10_9:
			case Platform.Mac_10_10:
			case Platform.Mac_10_10_3:
			case Platform.Mac_10_11:
			case Platform.Mac_10_12:
				break;
			default:
				return false;
			}

			switch (ToIosVersion (platform)) {
			case Platform.None:
			case Platform.iOS_Version:
			case Platform.iOS_2_0:
			case Platform.iOS_2_2:
			case Platform.iOS_3_0:
			case Platform.iOS_3_1:
			case Platform.iOS_3_2:
			case Platform.iOS_4_0:
			case Platform.iOS_4_1:
			case Platform.iOS_4_2:
			case Platform.iOS_4_3:
			case Platform.iOS_5_0:
			case Platform.iOS_5_1:
			case Platform.iOS_6_0:
			case Platform.iOS_6_1:
			case Platform.iOS_7_0:
			case Platform.iOS_7_1:
			case Platform.iOS_8_0:
			case Platform.iOS_8_1:
			case Platform.iOS_8_2:
			case Platform.iOS_8_3:
			case Platform.iOS_8_4:
			case Platform.iOS_9_0:
			case Platform.iOS_9_1:
			case Platform.iOS_9_2:
				break;
			default:
				return false;
			}

			return true;
#pragma warning restore 0618
		}

		public static Platform ToVersion (this Platform platform)
		{
			return platform & ~(Platform.Mac_Arch | Platform.iOS_Arch);
		}

		public static Platform ToMacVersion (this Platform platform)
		{
			return platform & Platform.Mac_Version;
		}

		public static Platform ToIosVersion (this Platform platform)
		{
			return platform & Platform.iOS_Version;
		}

		public static Platform ToArch (this Platform platform)
		{
			return platform & (Platform.Mac_Arch | Platform.iOS_Arch);
		}

		public static Platform ToMacArch (this Platform platform)
		{
			return platform & Platform.Mac_Arch;
		}

		public static Platform ToIosArch (this Platform platform)
		{
			return platform & Platform.iOS_Arch;
		}

		public static int CompareMacVersion (this Platform a, Platform b)
		{
			return ((ulong) ToMacVersion (a)).CompareTo ((ulong) ToMacVersion (b));
		}

		public static int CompareIosVersion (this Platform a, Platform b)
		{
			return ((uint) ToIosVersion (a)).CompareTo ((uint) ToIosVersion (b));
		}

		public static bool IsMac (this Platform platform)
		{
			return ToMacVersion (platform) != Platform.None;
		}

		public static bool IsIos (this Platform platform)
		{
			return ToIosVersion (platform) != Platform.None;
		}

		public static bool Is64BitOnlyOnCurrentPlatform (this Platform platform)
		{
#if MONOMAC
			return platform.ToMacArch () == Platform.Mac_Arch64;
#else
			return platform.ToIosArch () == Platform.iOS_Arch64;
#endif
		}

#if !PLATFORM_AVAILIBILITY_TEST && !COREBUILD
		static Platform? hostApiPlatform;

		public static Platform GetHostApiPlatform ()
		{
			if (hostApiPlatform is not null && hostApiPlatform.HasValue)
				return hostApiPlatform.Value;

#if MONOMAC
			using (var plist = NSDictionary.FromFile ("/System/Library/CoreServices/SystemVersion.plist"))
				return (hostApiPlatform = ParseApiPlatform (
					(NSString)plist ["ProductName"],
					(NSString)plist ["ProductVersion"])
				).Value;
#elif WATCH
			hostApiPlatform = null;
			Console.WriteLine ("PlatformHelper.GetHostApiPlatform () not implemented for WatchOS.");
			throw new NotImplementedException ();
#else
			return (hostApiPlatform = ParseApiPlatform (
			 	UIDevice.CurrentDevice.SystemName,
				UIDevice.CurrentDevice.SystemVersion)
			).Value;
#endif
		}
#endif

		public static Platform ParseApiPlatform (string productName, string productVersion)
		{
			if (productName is null)
				throw new ArgumentNullException ("productName");
			if (productVersion is null)
				throw new ArgumentNullException ("productVersion");

			var product = productName.Replace (" ", String.Empty).ToLowerInvariant ();

			var version = productVersion.Split ('.');
			var ci = CultureInfo.InvariantCulture;
			byte major, minor;
			if (!Byte.TryParse (version [0], NumberStyles.Integer, ci, out major) || !Byte.TryParse (version [1], NumberStyles.Integer, ci, out minor))
				throw new FormatException ("Bad version format: " + productVersion);

			Platform platform;
			if (product.StartsWith ("mac", StringComparison.Ordinal))
				platform = (Platform) ((ulong) major << 48 | (ulong) minor << 40);
			else if (product.StartsWith ("iphone", StringComparison.Ordinal) || product.StartsWith ("ios", StringComparison.Ordinal))
				platform = (Platform) ((ulong) major << 16 | (ulong) minor << 8);
			else
				throw new FormatException ("Unknown product name: " + productName);

			// remove version check or we create non-future proof binaries, ref bug #25211
			// if (!IsValid (platform))
			// 	throw new FormatException ("Unknown product version: " + productVersion);

			return platform;
		}

#if !COREBUILD && !WATCH && !NET
#if MONOMAC
		const int sys1 = 1937339185;
		const int sys2 = 1937339186;

		// Deprecated in OSX 10.8 - but no good alternative is (yet) available
#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.8")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 8)]
#endif
		[DllImport ("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		static extern int Gestalt (int selector, out int result);

		static int osx_major, osx_minor;

		public static bool CheckSystemVersion (int major, int minor)
		{
			if (osx_major == 0) {
				Gestalt (sys1, out osx_major);
				Gestalt (sys2, out osx_minor);
			}
			return osx_major > major || (osx_major == major && osx_minor >= minor);
		}
#else
		public static bool CheckSystemVersion (int major, int minor)
		{
			return UIDevice.CurrentDevice.CheckSystemVersion (major, minor);
		}
#endif
#endif
	}

	[AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
#if !COREBUILD
	[Obsolete ("Use [Introduced|Deprecated|Obsoleted|Unavailable] attributes with PlatformName.")]
#endif
	public class AvailabilityAttribute : Attribute {
		public static AvailabilityAttribute Merge (IEnumerable<object> attrs)
		{
			AvailabilityAttribute merged = null;

			foreach (var attr in attrs) {
				var aattr = attr as AvailabilityAttribute;
				if (aattr is null)
					continue;

				if (merged is null)
					merged = new AvailabilityAttribute ();

				merged.Introduced |= aattr.Introduced;
				merged.Obsoleted |= aattr.Obsoleted;
				merged.Deprecated |= aattr.Deprecated;
				merged.Unavailable |= aattr.Unavailable;
				if (!String.IsNullOrEmpty (aattr.Message)) {
					if (!String.IsNullOrEmpty (merged.Message))
						merged.Message += "; ";
					merged.Message += aattr.Message;
				}
			}

			return merged;
		}

		public static AvailabilityAttribute Get (MemberInfo member)
		{
			return Merge (member.GetCustomAttributes (typeof (AvailabilityAttribute), true));
		}

		static void Check (string property, Platform existing, Platform updated)
		{
			if (!PlatformHelper.IsValid (updated)) {
				throw new Exception (String.Format ("Platform setting determined invalid, cannot set '{0}' to '{1}' " +
					"as it is already set for the same platform to '{2}'",
					property, updated, existing));
			}
		}

		Platform introduced;
		Platform deprecated;
		Platform obsoleted;
		Platform unavailable;

		/// <summary>
		/// The first version in which this declaration was introduced.
		/// </summary>
		public Platform Introduced {
			get { return introduced; }
			set {
				Check ("Introduced", introduced, value);
				introduced = value;
			}
		}

		/// <summary>
		/// The first version in which this declaration was deprecated,
		/// meaning that users should migrate away from this API.
		/// </summary>
		public Platform Deprecated {
			get { return deprecated; }
			set {
				Check ("Deprecated", deprecated, value);
				deprecated = value;
			}
		}

		/// <summary>
		/// The first version in which this declaration was obsoleted,
		/// meaning that it was removed completely and can no longer be used.
		/// </summary>
		public Platform Obsoleted {
			get { return obsoleted; }
			set {
				Check ("Obsoleted", obsoleted, value);
				obsoleted = value;
			}
		}

		/// <summary>
		/// This declaration is never available on this platform.
		/// </summary>
		public Platform Unavailable {
			get { return unavailable; }
			set {
				switch (value) {
				case Platform.None:
				case Platform.Mac_Version:
				case Platform.iOS_Version:
				case Platform.iOS_Version | Platform.Mac_Version:
					unavailable = value;
					break;
				default:
					throw new Exception ("Unavailable applies to all platform versions; " +
						"it should be one of Platform.None, Platform.Mac_Version, or Platform.iOS_Version");
				}
			}
		}

		public Platform IntroducedVersion {
			get { return Introduced.ToVersion (); }
		}

		public Platform IntroducedArchitecture {
			get { return Introduced.ToArch (); }
		}

		public Platform DeprecatedVersion {
			get { return Deprecated.ToVersion (); }
		}

		public Platform DeprecatedArchitecture {
			get { return Deprecated.ToArch (); }
		}

		public Platform ObsoletedVersion {
			get { return Obsoleted.ToVersion (); }
		}

		public Platform ObsoletedArchitecture {
			get { return Obsoleted.ToArch (); }
		}

		/// <summary>
		/// An optional message to inform the user about availability
		/// </summary>
		public string Message { get; set; }

		public bool AlwaysAvailable {
			get {
				return
					introduced == Platform.None &&
					deprecated == Platform.None &&
					obsoleted == Platform.None &&
					unavailable == Platform.None;
			}
		}

		public AvailabilityAttribute ()
		{
		}

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

		public override string ToString ()
		{
			if (AlwaysAvailable)
				return "[Availability]";

			var builder = new StringBuilder ();
			var haveVersion = false;

			builder.Append ("[Availability (");

			Action<string, Platform> append = (name, platform) => {
				if (platform != Platform.None) {
					if (haveVersion)
						builder.Append (", ");
					builder.AppendFormat ("{0} = Platform.{1}",
						name, platform.ToString ().Replace (", ", " | Platform."));
					haveVersion = true;
				}
			};

			append ("Introduced", introduced);
			append ("Deprecated", deprecated);
			append ("Obsoleted", obsoleted);
			append ("Unavailable", unavailable);

			if (Message is not null)
				builder.AppendFormat (", Message = \"{0}\"",
					Message.Replace ("\"", "\\\""));

			builder.Append (")]");

			return builder.ToString ();
		}
	}

#if !COREBUILD
	[Obsolete ("Use [Introduced|Deprecated|Obsoleted|Unavailable] attributes with PlatformName.")]
#endif
	public sealed class iOSAttribute : AvailabilityAttribute {
		public iOSAttribute (byte major, byte minor)
			: this (major, minor, 0)
		{
		}

		public iOSAttribute (byte major, byte minor, byte subminor)
			: base ((Platform) ((ulong) major << 16 | (ulong) minor << 8 | (ulong) subminor))
		{
		}

#if !NET
		[Obsolete ("Use the overload that takes '(major, minor)', since iOS is always 64-bit.")]
		public iOSAttribute (byte major, byte minor, bool onlyOn64 = false)
			: this (major, minor, 0, onlyOn64)
		{
		}

		[Obsolete ("Use the overload that takes '(major, minor, subminor)', since iOS is always 64-bit.")]
		public iOSAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
			: base ((Platform) ((ulong) major << 48 | (ulong) minor << 40 | (ulong) subminor << 32) | (onlyOn64 ? Platform.iOS_Arch64 : Platform.None))
		{
		}
#endif

	}

#if !COREBUILD
	[Obsolete ("Use [Introduced|Deprecated|Obsoleted|Unavailable] attributes with PlatformName.")]
#endif
	public sealed class MacAttribute : AvailabilityAttribute {
		public MacAttribute (byte major, byte minor)
#if NET
			: this (major, minor, 0)
#else
			: this (major, minor, 0, false)
#endif
		{
		}

#if !NET
		[Obsolete ("Use the overload that takes '(major, minor, subminor)', since macOS is always 64-bit.")]
		public MacAttribute (byte major, byte minor, bool onlyOn64 = false)
			: this (major, minor, 0, onlyOn64)
		{
		}

		[Obsolete ("Use the overload that takes '(major, minor, subminor)', since macOS is always 64-bit.")]
		public MacAttribute (byte major, byte minor, PlatformArchitecture arch)
			: this (major, minor, 0, arch)
		{
		}
#endif

		public MacAttribute (byte major, byte minor, byte subminor)
#if NET
			: base ((Platform)((ulong)major << 48 | (ulong)minor << 40 | (ulong)subminor << 32))
#else
			: this (major, minor, subminor, false)
#endif
		{
		}

#if !NET
		[Obsolete ("Use the overload that takes '(major, minor, subminor)', since macOS is always 64-bit.")]
		public MacAttribute (byte major, byte minor, byte subminor, bool onlyOn64)
			: base ((Platform) ((ulong) major << 48 | (ulong) minor << 40 | (ulong) subminor << 32) | (onlyOn64 ? Platform.Mac_Arch64 : Platform.None))
		{
		}
#endif

#if !NET
		[Obsolete ("Use the overload that takes '(major, minor, subminor)', since macOS is always 64-bit.")]
		public MacAttribute (byte major, byte minor, byte subminor, PlatformArchitecture arch)
			: base ((Platform) ((ulong) major << 48 | (ulong) minor << 40 | (ulong) subminor << 32) | (arch == PlatformArchitecture.Arch64 ? Platform.Mac_Arch64 : Platform.None))
		{
		}
#endif

	}
}

#endif
