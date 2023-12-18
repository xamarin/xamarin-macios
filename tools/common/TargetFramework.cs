//
// TargetFramework.cs
//
// Authors:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable // until we get around to fixing this file

namespace Xamarin.Utils {
	public struct TargetFramework : IEquatable<TargetFramework> {
		const string TFMVersion = Xamarin.DotNetVersions.Version;
		public const string DotNet_iOS_String = ".NETCoreApp,Version=" + TFMVersion + ",Profile=ios"; // Short form: netX.Y-ios
		public const string DotNet_tvOS_String = ".NETCoreApp,Version=" + TFMVersion + ",Profile=tvos"; // Short form: netX.Y-tvos
		public const string DotNet_watchOS_String = ".NETCoreApp,Version=" + TFMVersion + ",Profile=watchos"; // Short form: netX.Y-watchos
		public const string DotNet_macOS_String = ".NETCoreApp,Version=" + TFMVersion + ",Profile=macos"; // Short form: netX.Y-macos
		public const string DotNet_MacCatalyst_String = ".NETCoreApp,Version=" + TFMVersion + ",Profile=maccatalyst"; // Short form: netX.Y-maccatalyst

		public static readonly TargetFramework Empty = new TargetFramework ();
		public static readonly TargetFramework Net_2_0 = Parse ("2.0");
		public static readonly TargetFramework Net_3_0 = Parse ("3.0");
		public static readonly TargetFramework Net_3_5 = Parse ("3.5");
		public static readonly TargetFramework Net_4_0 = Parse ("4.0");
		public static readonly TargetFramework Net_4_5 = Parse ("4.5");
		public static readonly TargetFramework Xamarin_Mac_2_0 = Parse ("Xamarin.Mac,v2.0");

		public static readonly TargetFramework Xamarin_iOS_1_0 = Parse ("Xamarin.iOS,v1.0");
		public const string Xamarin_WatchOS_1_0_String = "Xamarin.WatchOS,v1.0";
		public static readonly TargetFramework Xamarin_WatchOS_1_0 = Parse (Xamarin_WatchOS_1_0_String);
		public static readonly TargetFramework Xamarin_TVOS_1_0 = Parse ("Xamarin.TVOS,v1.0");
		public static readonly TargetFramework Xamarin_MacCatalyst_1_0 = Parse ("Xamarin.MacCatalyst,v1.0");

		public static readonly TargetFramework Xamarin_Mac_2_0_Mobile = Parse ("Xamarin.Mac,Version=v2.0,Profile=Mobile");
		public static readonly TargetFramework Xamarin_Mac_4_5_Full = Parse ("Xamarin.Mac,Version=v4.5,Profile=Full");
		public static readonly TargetFramework Xamarin_Mac_4_5_System = Parse ("Xamarin.Mac,Version=v4.5,Profile=System");

		public static readonly TargetFramework DotNet_iOS = Parse (DotNet_iOS_String);
		public static readonly TargetFramework DotNet_tvOS = Parse (DotNet_tvOS_String);
		public static readonly TargetFramework DotNet_watchOS = Parse (DotNet_watchOS_String);
		public static readonly TargetFramework DotNet_macOS = Parse (DotNet_macOS_String);
		public static readonly TargetFramework DotNet_MacCatalyst = Parse (DotNet_MacCatalyst_String);

		public static readonly TargetFramework [] ValidFrameworksMac = new [] {
			Xamarin_Mac_2_0_Mobile, Xamarin_Mac_4_5_Full, Xamarin_Mac_4_5_System,
			DotNet_macOS,
		};

		public static readonly TargetFramework [] ValidFrameworksiOS = new [] {
			Xamarin_iOS_1_0, Xamarin_WatchOS_1_0, Xamarin_TVOS_1_0,
			Xamarin_MacCatalyst_1_0,
			DotNet_iOS, DotNet_tvOS, DotNet_watchOS, DotNet_MacCatalyst,
		};

		public static IEnumerable<TargetFramework> AllValidFrameworks {
			get { return ValidFrameworksMac.Union (ValidFrameworksiOS); }
		}

#if MTOUCH
		public static IEnumerable<TargetFramework> ValidFrameworks { get { return ValidFrameworksiOS; } }
#elif MMP
		public static IEnumerable<TargetFramework> ValidFrameworks { get { return ValidFrameworksMac; } }
#else
		public static IEnumerable<TargetFramework> ValidFrameworks { get { return AllValidFrameworks; } }
#endif

		public static bool IsValidFramework (TargetFramework framework)
		{
			foreach (var tf in ValidFrameworks)
				if (tf == framework)
					return true;
			return false;
		}

		public bool IsDotNet {
			get { return Identifier == ".NETCoreApp" && Version.Major >= 5; }
		}

		public static TargetFramework Parse (string targetFrameworkString)
		{
			TargetFramework targetFramework;
			TryParse (targetFrameworkString, out targetFramework);
			return targetFramework;
		}

		public static bool TryParse (string targetFrameworkString,
			out TargetFramework targetFramework)
		{
			targetFramework = Empty;

			var s = targetFrameworkString;

			if (String.IsNullOrEmpty (s))
				return false;

			s = s.Trim ();

			string identifier = null;
			string version = null;
			string profile = null;

			var fields = targetFrameworkString.Split (new char [] { ',' });
			switch (fields.Length) {
			case 1:
				// This is just a version number, in which case default identifier to .NETFramework.
				identifier = ".NETFramework";
				version = fields [0];
				break;
			case 2:
				identifier = fields [0];
				version = fields [1];
				break;
			case 3:
				identifier = fields [0];
				version = fields [1];
				profile = fields [2];
				break;
			default:
				throw new Exception ();
			}

			identifier = identifier.Trim ();
			version = version.Trim ();
			profile = profile?.Trim ();

			// Parse version.
			// It can optionally start with 'Version=' or 'v' (or 'Version=v')
			if (version.StartsWith ("Version=", StringComparison.Ordinal))
				version = version.Substring ("Version=".Length);
			if (version.StartsWith ("v", StringComparison.OrdinalIgnoreCase))
				version = version.Substring (1);
			Version parsed_version;
			if (!Version.TryParse (version, out parsed_version))
				return false;

			// If we got a profile, then the 'Profile=' part is mandatory.
			if (profile is not null) {
				if (!profile.StartsWith ("Profile=", StringComparison.Ordinal))
					return false;

				profile = profile.Substring ("Profile=".Length);
			}

			targetFramework = new TargetFramework (identifier, parsed_version, profile);
			return true;
		}

		readonly string identifier;
		public string Identifier {
			get { return identifier; }
		}

		readonly Version version;
		public Version Version {
			get { return version; }
		}

		readonly string profile;
		public string Profile {
			get { return profile; }
		}

		public TargetFramework (string identifier, Version version, string profile = null)
		{
			this.identifier = identifier is not null ? identifier.Trim () : null;
			this.version = version;
			this.profile = profile;
		}

		public static bool operator == (TargetFramework a, TargetFramework b)
		{
			return a.Equals (b);
		}

		public static bool operator != (TargetFramework a, TargetFramework b)
		{
			return !a.Equals (b);
		}

		public bool Equals (TargetFramework other)
		{
			return String.Equals (other.Identifier, Identifier, StringComparison.OrdinalIgnoreCase)
				&& other.Version == Version
				&& other.Profile == Profile;
		}

		public override bool Equals (object obj)
		{
			return obj is TargetFramework ? Equals ((TargetFramework) obj) : false;
		}

		public override int GetHashCode ()
		{
			var hash = 0;
			if (Identifier is not null)
				hash ^= Identifier.ToLowerInvariant ().GetHashCode ();
			if (Version is not null)
				hash ^= Version.GetHashCode ();
			if (Profile is not null)
				hash ^= Profile.GetHashCode ();
			return hash;
		}

		public override string ToString ()
		{
			var id = Identifier;
			if (String.Equals (id, ".NETFramework", StringComparison.OrdinalIgnoreCase))
				id = ".NETFramework";
			else if (String.Equals (id, "Xamarin.Mac", StringComparison.OrdinalIgnoreCase))
				id = "Xamarin.Mac";

			return String.Format ("{0},Version=v{1}{2}", id, Version is null ? "0.0" : Version.ToString (), Profile is null ? string.Empty : (",Profile=" + Profile));
		}

		public ApplePlatform Platform {
			get {
				switch (Identifier) {
				case "MonoTouch":
				case "Xamarin.iOS":
					return ApplePlatform.iOS;
				case "Xamarin.WatchOS":
					return ApplePlatform.WatchOS;
				case "Xamarin.TVOS":
					return ApplePlatform.TVOS;
				case "Xamarin.MacCatalyst":
					return ApplePlatform.MacCatalyst;
				case ".NETCoreApp":
					switch (Profile) {
					case "ios":
						return ApplePlatform.iOS;
					case "tvos":
						return ApplePlatform.TVOS;
					case "watchos":
						return ApplePlatform.WatchOS;
					case "macos":
						return ApplePlatform.MacOSX;
					case "maccatalyst":
						return ApplePlatform.MacCatalyst;
					default:
						throw new InvalidOperationException (string.Format ("Invalid .NETCoreApp Profile for Apple platforms: {0}", Profile));
					}
				default:
					return ApplePlatform.MacOSX;
				}
			}
		}

		public static TargetFramework GetTargetFramework (ApplePlatform platform, bool isDotNet)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				return isDotNet ? DotNet_iOS : Xamarin_iOS_1_0;
			case ApplePlatform.TVOS:
				return isDotNet ? DotNet_tvOS : Xamarin_TVOS_1_0;
			case ApplePlatform.MacCatalyst:
				return DotNet_MacCatalyst;
			case ApplePlatform.WatchOS:
				return Xamarin_WatchOS_1_0;
			case ApplePlatform.MacOSX:
				return isDotNet ? DotNet_macOS : Xamarin_Mac_2_0;
			default:
				throw new ArgumentOutOfRangeException (nameof (platform), string.Format ("Unknown platform: {0}", platform.ToString ()));
			}
		}
	}
}
