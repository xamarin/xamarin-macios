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

		public static readonly TargetFramework Xamarin_iOS_1_0 = Parse ("Xamarin.iOS,v1.0");
		public static readonly TargetFramework Xamarin_WatchOS_1_0 = Parse ("Xamarin.WatchOS,v1.0");
		public static readonly TargetFramework Xamarin_TVOS_1_0 = Parse ("Xamarin.TVOS,v1.0");

#if MTOUCH
		public static readonly TargetFramework [] ValidFrameworks = new TargetFramework[] { Xamarin_iOS_1_0, Xamarin_WatchOS_1_0, Xamarin_TVOS_1_0 };
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
			Version version = null;

			var comma = s.IndexOf (',');
			if (comma >= 0) {
				identifier = s.Substring (0, comma).Trim ();
				s = s.Substring (comma + 1);
			}

			s = s.Trim ();
			if (s.Length >= 1 && Char.ToLowerInvariant (s [0]) == 'v')
				s = s.Substring (1);

			if (!Version.TryParse (s.Trim (), out version))
				return false;

			targetFramework = new TargetFramework (identifier, version);
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

		public TargetFramework (string identifier, Version version)
		{
			this.identifier = identifier != null ? identifier.Trim () : null;
			this.version = version;
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
			return String.Equals (other.Identifier, Identifier,
				StringComparison.OrdinalIgnoreCase) && other.Version == Version;
		}

		public override bool Equals (object obj)
		{
			return obj is TargetFramework ? Equals ((TargetFramework)obj) : false;
		}

		public override int GetHashCode ()
		{
			var hash = 0;
			if (Identifier != null)
				hash ^= Identifier.GetHashCode ();
			if (Version != null)
				hash ^= Version.GetHashCode ();
			return hash;
		}

		public override string ToString ()
		{
			var id = Identifier;
			if (String.IsNullOrEmpty (id) ||
				String.Equals (id, ".NETFramework", StringComparison.OrdinalIgnoreCase))
				id = ".NETFramework";
			else if (String.Equals (id, "Xamarin.Mac", StringComparison.OrdinalIgnoreCase))
				id = "Xamarin.Mac";

			return String.Format ("{0},v{1}", id, Version == null ? "0.0" : Version.ToString ());
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
