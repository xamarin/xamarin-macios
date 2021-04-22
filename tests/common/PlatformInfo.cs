//
// PlatformInfo.cs: info about the host platform
// and AvailabilityBaseAttribute extensions for tests
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;
#if !(MONOMAC || __MACOS__)
using UIKit;
#endif

namespace Xamarin.Tests 
{
	public sealed class PlatformInfo
	{
		static PlatformInfo GetHostPlatformInfo ()
		{
			string name;
			string version;
#if __MACCATALYST__
			name = "MacCatalyst";
			version = UIDevice.CurrentDevice.SystemVersion;
#elif __TVOS__ || __IOS__
			name = UIDevice.CurrentDevice.SystemName;
			version = UIDevice.CurrentDevice.SystemVersion;
#elif __WATCHOS__
			name = WatchKit.WKInterfaceDevice.CurrentDevice.SystemName;
			version = WatchKit.WKInterfaceDevice.CurrentDevice.SystemVersion;
#elif MONOMAC || __MACOS__
			using (var plist = NSDictionary.FromFile ("/System/Library/CoreServices/SystemVersion.plist")) {
				name = (NSString)plist ["ProductName"];
				version = (NSString)plist ["ProductVersion"];
			}
#else
#error Unknown platform
#endif
			name = name?.Replace (" ", String.Empty)?.ToLowerInvariant ();
			if (name == null)
				throw new FormatException ("Product name is `null`");

			var platformInfo = new PlatformInfo ();

			if (name.StartsWith ("maccatalyst", StringComparison.Ordinal))
				platformInfo.Name = PlatformName.MacCatalyst;
			else if (name.StartsWith ("mac", StringComparison.Ordinal))
				platformInfo.Name = PlatformName.MacOSX;
			else if (name.StartsWith ("ios", StringComparison.Ordinal) || name.StartsWith ("iphoneos", StringComparison.Ordinal))
				platformInfo.Name = PlatformName.iOS;
			else if (name.StartsWith ("tvos", StringComparison.Ordinal))
				platformInfo.Name = PlatformName.TvOS;
			else if (name.StartsWith ("watchos", StringComparison.Ordinal))
				platformInfo.Name = PlatformName.WatchOS;
			else
				throw new FormatException ($"Unknown product name: {name}");

			platformInfo.Version = Version.Parse (version);

			if (IntPtr.Size == 4)
				platformInfo.Architecture = PlatformArchitecture.Arch32;
			else if (IntPtr.Size == 8)
				platformInfo.Architecture = PlatformArchitecture.Arch64;

			return platformInfo;
		}

		public static readonly PlatformInfo Host = GetHostPlatformInfo ();

		public PlatformName Name { get; private set; }
		public PlatformArchitecture Architecture { get; private set; }
		public Version Version { get; private set; }

		public bool IsMac => Name == PlatformName.MacOSX;
		public bool IsIos => Name == PlatformName.iOS;
		public bool IsArch32 => Architecture.HasFlag (PlatformArchitecture.Arch32);
		public bool IsArch64 => Architecture.HasFlag (PlatformArchitecture.Arch64);

		PlatformInfo ()
		{
		}
	}

	public static class AvailabilityExtensions
	{
#if NET
		public static AvailabilityBaseAttribute Convert (this OSPlatformAttribute a)
		{
			if (a == null)
				return null;

			PlatformName p;
			int n = 0;
			switch (a.PlatformName) {
			case string dummy when a.PlatformName.StartsWith ("ios"):
				p = PlatformName.iOS;
				n = "ios".Length;
				break;
			case string dummy when a.PlatformName.StartsWith ("tvos"):
				p = PlatformName.TvOS;
				n = "tvos".Length;
				break;
			case string dummy when a.PlatformName.StartsWith ("watchos"):
				p = PlatformName.WatchOS;
				n = "watchos".Length;
				break;
			case string dummy when a.PlatformName.StartsWith ("macos"):
				p = PlatformName.MacOSX;
				n = "macos".Length;
				break;
			case string dummy when a.PlatformName.StartsWith ("maccatalyst"):
				p = PlatformName.MacCatalyst;
				n = "maccatalyst".Length;
				break;
			default:
				return null;
			}
			bool versioned = Version.TryParse (a.PlatformName [n..], out var v);

			if (a is SupportedOSPlatformAttribute) {
				if (!versioned)
					throw new FormatException (a.PlatformName);
				return new IntroducedAttribute (p, v.Major, v.Minor);
			}
			if (a is UnsupportedOSPlatformAttribute) {
				// if a version is provided then it means it was deprecated
				// if no version is provided then it means it's unavailable
				if (versioned)
					return new DeprecatedAttribute (p, v.Major, v.Minor);
				else
					return new UnavailableAttribute (p);
			}
			return null;
		}
#endif

		public static bool IsAvailableOnHostPlatform (this ICustomAttributeProvider attributeProvider)
		{
			return attributeProvider.IsAvailable (PlatformInfo.Host);
		}

		public static bool IsAvailable (this ICustomAttributeProvider attributeProvider, PlatformInfo targetPlatform)
		{
#if NET
			var list = new List<AvailabilityBaseAttribute> ();
			foreach (var ca in attributeProvider.GetCustomAttributes (true)) {
				if (ca is OSPlatformAttribute aa)
					list.Add (aa.Convert ());
				// FIXME - temporary, while older attributes co-exists (in manual bindings)
				if (ca is AvailabilityBaseAttribute old)
					list.Add (old);
			}
			return list.IsAvailable (targetPlatform);
#else
			return attributeProvider
				.GetCustomAttributes (true)
				.OfType<AvailabilityBaseAttribute> ()
				.IsAvailable (targetPlatform);
#endif
		}

		public static bool IsAvailableOnHostPlatform (this IEnumerable<AvailabilityBaseAttribute> attributes)
		{
			return attributes.IsAvailable (PlatformInfo.Host);
		}

		public static bool IsAvailable (this IEnumerable<AvailabilityBaseAttribute> attributes, PlatformInfo targetPlatform)
		{
			// always "available" from a binding perspective if
			// there are no explicit annotations saying otherwise
			var available = true;

			foreach (var attr in attributes) {
				if (attr.Platform != targetPlatform.Name)
					continue;

				switch (attr.AvailabilityKind) {
				case AvailabilityKind.Introduced:
					if (attr.Version != null)
						available &= targetPlatform.Version >= attr.Version;

					if (attr.Architecture != PlatformArchitecture.None &&
						attr.Architecture != PlatformArchitecture.All)
						available &= attr.Architecture.HasFlag (targetPlatform.Architecture);
					break;
				case AvailabilityKind.Deprecated:
				case AvailabilityKind.Obsoleted:
					if (attr.Version != null)
						available &= targetPlatform.Version < attr.Version;
					// FIXME: handle architecture-level _un_availability?
					// we didn't do this with the old AvailabilityAttribute...
					break;
				case AvailabilityKind.Unavailable:
					available = false;
					break;
				}

				if (!available)
					return false;
			}

			return available;
		}
	}
}
