//
// PlatformInfo.cs: info about the host platform
// and AvailabilityBaseAttribute extensions for tests
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;
#if !(MONOMAC || __MACOS__)
using UIKit;
#endif

using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.Tests {
	public sealed class PlatformInfo {
		static PlatformInfo GetHostPlatformInfo ()
		{
			string name;
			string version;
#if __MACCATALYST__
			name = "MacCatalyst";
			version = iOSSupportVersion;
#elif __TVOS__ || __IOS__
			name = UIDevice.CurrentDevice.SystemName;
			version = UIDevice.CurrentDevice.SystemVersion;
#elif MONOMAC || __MACOS__
			using (var plist = NSDictionary.FromFile ("/System/Library/CoreServices/SystemVersion.plist")) {
				name = (NSString) plist ["ProductName"];
				version = (NSString) plist ["ProductVersion"];
			}
#else
#error Unknown platform
#endif
			name = name?.Replace (" ", String.Empty)?.ToLowerInvariant ();
			if (name is null)
				throw new FormatException ("Product name is `null`");

			var platformInfo = new PlatformInfo ();

			if (name.StartsWith ("maccatalyst", StringComparison.Ordinal))
				platformInfo.Name = ApplePlatform.MacCatalyst;
			else if (name.StartsWith ("mac", StringComparison.Ordinal))
				platformInfo.Name = ApplePlatform.MacOSX;
			else if (name.StartsWith ("ios", StringComparison.Ordinal) || name.StartsWith ("iphoneos", StringComparison.Ordinal) || name.StartsWith ("ipados", StringComparison.Ordinal))
				platformInfo.Name = ApplePlatform.iOS;
			else if (name.StartsWith ("tvos", StringComparison.Ordinal))
				platformInfo.Name = ApplePlatform.TVOS;
			else
				throw new FormatException ($"Unknown product name: {name}");

			platformInfo.Version = Version.Parse (version);

			return platformInfo;
		}

#if __MACCATALYST__
		static string _iOSSupportVersion;
		internal static string iOSSupportVersion {
			get {
				if (_iOSSupportVersion is null) {
					// This is how Apple does it: https://github.com/llvm/llvm-project/blob/62ec4ac90738a5f2d209ed28c822223e58aaaeb7/lldb/source/Host/macosx/objcxx/HostInfoMacOSX.mm#L100-L105
					using var dict = NSMutableDictionary.FromFile ("/System/Library/CoreServices/SystemVersion.plist");
					using var str = (NSString) "iOSSupportVersion";
					using var obj = dict.ObjectForKey (str);
					_iOSSupportVersion = obj.ToString ();
				}
				return _iOSSupportVersion;
			}
		}
#endif

		public static readonly PlatformInfo Host = GetHostPlatformInfo ();

		public ApplePlatform Name { get; private set; }
		public Version Version { get; private set; }

		public bool IsMac => Name == ApplePlatform.MacOSX;
		public bool IsIos => Name == ApplePlatform.iOS;

		PlatformInfo ()
		{
		}
	}

	public static class AvailabilityExtensions {
		public static bool IsAvailableOnHostPlatform (this ICustomAttributeProvider attributeProvider)
		{
			return attributeProvider.IsAvailable (PlatformInfo.Host);
		}

		[UnconditionalSuppressMessage ("Trimming", "IL2045", Justification = "Some of the attributes this method uses may have been linked away, so things might not work. It actually works though, so unless something changes, we're going to assume it's trimmer-compatible.")]
		public static bool IsAvailable (this ICustomAttributeProvider attributeProvider, PlatformInfo targetPlatform)
		{
			var customAttributes = attributeProvider.GetCustomAttributes (true);

			customAttributes = customAttributes.ToArray (); // don't iterate twice
			if (customAttributes.Any (v => v is ObsoleteAttribute))
				return false;

			return customAttributes
				.OfType<OSPlatformAttribute> ()
				.IsAvailable (targetPlatform);
		}

		public static bool IsAvailableOnHostPlatform (this IEnumerable<OSPlatformAttribute> attributes)
		{
			return attributes.IsAvailable (PlatformInfo.Host);
		}

		static IEnumerable<(OSPlatformAttribute Attribute, ApplePlatform Platform, Version Version)> ParseAttributes (IEnumerable<OSPlatformAttribute> attributes)
		{
			foreach (var attr in attributes) {
				if (!attr.TryParse (out ApplePlatform? platform, out var version))
					continue;

				yield return new (attr, platform.Value, version);
			}
		}

		public static bool IsAvailable (this IEnumerable<OSPlatformAttribute> attributes, PlatformInfo targetPlatform)
		{
			var parsedAttributes = ParseAttributes (attributes);

			var available = IsAvailable (parsedAttributes, targetPlatform, targetPlatform.Name);
			if (available.HasValue)
				return available.Value;

			if (targetPlatform.Name == ApplePlatform.MacCatalyst) {
				available = IsAvailable (parsedAttributes, targetPlatform, ApplePlatform.iOS);
				if (available.HasValue)
					return available.Value;
			}

			// Our current attribute logic assumes that no attribute means that an API is available on all versions of that platform.
			// This is not correct: the correct logic is that an API is available on a platform if there are no availability attributes
			// for any other platforms. However, enforcing this here would make a few of our tests fail, so we must keep the incorrect
			// logic until we've got all the right attributes implemented.
			return true;

			// Correct logic:
			// Only available if there aren't any attributes for other platforms
			// return attributes.Count () == 0;
		}

		[UnconditionalSuppressMessage ("Trimming", "IL2045", Justification = "Some of the attributes this method uses may have been linked away, so things might not work. It actually works though, so unless something changes, we're going to assume it's trimmer-compatible.")]
		public static bool? IsAvailable (IEnumerable<(OSPlatformAttribute Attribute, ApplePlatform Platform, Version Version)> attributes, PlatformInfo targetPlatform, ApplePlatform attributePlatform)
		{
			// First we check for any unsupported attributes, and only once we know that there aren't any unsupported
			// attributes, we check for supported attributes. Otherwise we might determine that an API is available
			// if we check the supported attribute first for OS=3.0 here:
			//     [SupportedOSPlatform ("1.0")
			//     [UnsupportedOSPlatform ("2.0")
			// if we run into the SupportedOSPlatform attribute first.
			foreach (var (attr, platform, version) in attributes) {
				if (platform != attributePlatform)
					continue;

				// At this point we can't ascertain that the API is available, only that it's unavailable,
				// so only return in that case. We need to check the SupportedOSPlatform attributes
				// to see if the API is available.
				if (attr is UnsupportedOSPlatformAttribute || attr is ObsoletedOSPlatformAttribute) {
					var isUnsupported = version is not null && targetPlatform.Version >= version;
					if (isUnsupported)
						return false;
				}
			}

			foreach (var (attr, platform, version) in attributes) {
				if (platform != attributePlatform)
					continue;

				if (attr is SupportedOSPlatformAttribute)
					return version is null || targetPlatform.Version >= version;
			}

			return null;
		}
	}
}
