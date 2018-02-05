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

#if XAMCORE_2_0 || __UNIFIED__
using ObjCRuntime;
using Foundation;
#if !(MONOMAC || __MACOS__)
using UIKit;
#endif
#elif MONOMAC || __MACOS__
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace Xamarin.Tests 
{
	public sealed class PlatformInfo
	{
		static PlatformInfo GetHostPlatformInfo ()
		{
			string name;
			string version;
#if __TVOS__ || __IOS__
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

			var platformInfo = new PlatformInfo ();

			if (name != null && name.StartsWith ("mac", StringComparison.Ordinal))
				platformInfo.Name = PlatformName.MacOSX;
			else if (name != null && (name.StartsWith ("ios", StringComparison.Ordinal) || name.StartsWith ("iphoneos", StringComparison.Ordinal)))
				platformInfo.Name = PlatformName.iOS;
			else if (name != null && name.StartsWith ("tvos", StringComparison.Ordinal))
				platformInfo.Name = PlatformName.TvOS;
			else if (name != null && name.StartsWith ("watchos", StringComparison.Ordinal))
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
		public static bool IsAvailableOnHostPlatform (this ICustomAttributeProvider attributeProvider)
		{
			return attributeProvider.IsAvailable (PlatformInfo.Host);
		}

		public static bool IsAvailable (this ICustomAttributeProvider attributeProvider, PlatformInfo targetPlatform)
		{
			return attributeProvider
				.GetCustomAttributes (true)
				.OfType<AvailabilityBaseAttribute> ()
				.IsAvailable (targetPlatform);
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
