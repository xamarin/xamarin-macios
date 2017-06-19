//
// TargetFramework.cs
//
// Authors:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All Rights Reserved.

using System;

namespace Xamarin.Utils
{
	public enum ApplePlatform {
		None,
		MacOSX,
		iOS,
		WatchOS,
		TVOS,
	}

	public struct TargetFramework : IEquatable<TargetFramework>
	{
		public static readonly TargetFramework Empty = new TargetFramework ();
		public static readonly TargetFramework Default = 
#if MONOMAC
			Parse ("4.0");
#else
			Parse ("MonoTouch,v1.0");	
#endif
		public static readonly TargetFramework Net_2_0 = Parse ("2.0");
		public static readonly TargetFramework Net_3_0 = Parse ("3.0");
		public static readonly TargetFramework Net_3_5 = Parse ("3.5");
		public static readonly TargetFramework Net_4_0 = Parse ("4.0");
		public static readonly TargetFramework Net_4_5 = Parse ("4.5");
		public static readonly TargetFramework Xamarin_Mac_2_0 = Parse ("Xamarin.Mac,v2.0");

		public static readonly TargetFramework MonoTouch_1_0 = Parse ("MonoTouch,v1.0");
		public static readonly TargetFramework Xamarin_iOS_1_0 = Parse ("Xamarin.iOS,v1.0");
		public static readonly TargetFramework Xamarin_WatchOS_1_0 = Parse ("Xamarin.WatchOS,v1.0");
		public static readonly TargetFramework Xamarin_TVOS_1_0 = Parse ("Xamarin.TVOS,v1.0");

		public static readonly TargetFramework XamMac_1_0 = Parse ("XamMac,v1.0");
		public static readonly TargetFramework Xamarin_Mac_2_0_Mobile = Parse ("Xamarin.Mac,Version=v2.0,Profile=Mobile");
		public static readonly TargetFramework Xamarin_Mac_4_5_Full = Parse ("Xamarin.Mac,Version=v4.5,Profile=Full");
		public static readonly TargetFramework Xamarin_Mac_4_5_System = Parse ("Xamarin.Mac,Version=v4.5,Profile=System");
#if MTOUCH
		public static readonly TargetFramework [] ValidFrameworks = new TargetFramework[] { Xamarin_iOS_1_0, Xamarin_WatchOS_1_0, Xamarin_TVOS_1_0 };
#elif BGENERATOR
		public static readonly TargetFramework [] ValidFrameworks = new TargetFramework[]
		{
			MonoTouch_1_0, Xamarin_iOS_1_0, Xamarin_TVOS_1_0, Xamarin_WatchOS_1_0,
			XamMac_1_0, Xamarin_Mac_2_0_Mobile, Xamarin_Mac_4_5_Full, Xamarin_Mac_4_5_System
		};
#endif

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
			if (profile != null) {
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
			this.identifier = identifier != null ? identifier.Trim () : null;
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
			return obj is TargetFramework ? Equals ((TargetFramework)obj) : false;
		}

		public override int GetHashCode ()
		{
			var hash = 0;
			if (Identifier != null)
				hash ^= Identifier.ToLowerInvariant ().GetHashCode ();
			if (Version != null)
				hash ^= Version.GetHashCode ();
			if (Profile != null)
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

			return String.Format ("{0},Version=v{1}{2}", id, Version == null ? "0.0" : Version.ToString (), Profile == null ? string.Empty : (",Profile=" + Profile));
		}

		public string MonoFrameworkDirectory {
			get {
				if (Identifier == "Xamarin.Mac")
					return Identifier;

				if (Version == null)
					return null;

				return Version.ToString ();
			}
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
				default:
					return ApplePlatform.MacOSX;
				}
			}
		}
	}
}
