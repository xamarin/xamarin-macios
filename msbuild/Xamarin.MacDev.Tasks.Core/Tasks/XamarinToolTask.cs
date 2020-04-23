using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	// This is a clone of XamarinTask, except that it subclasses ToolTask instead of Task
	public abstract class XamarinToolTask : ToolTask {

		public string SessionId { get; set; }

		[Required]
		public string TargetFrameworkMoniker { get; set; }

		public string Product {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return "Xamarin.iOS";
				case ApplePlatform.MacOSX:
					return "Xamarin.Mac";
				default:
					// FIXME: better error
					throw new NotImplementedException ($"Invalid platform: {Platform}");
				}
			}
		}

		ApplePlatform? platform;
		public ApplePlatform Platform {
			get {
				if (!platform.HasValue)
					platform = PlatformFrameworkHelper.GetFramework (TargetFrameworkMoniker);
				return platform.Value;
			}
		}

		TargetFramework? target_framework;
		public TargetFramework TargetFramework {
			get {
				if (!target_framework.HasValue)
					target_framework = TargetFramework.Parse (TargetFrameworkMoniker);
				return target_framework.Value;
			}
		}

		public string PlatformName {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return "iOS";
				case ApplePlatform.TVOS:
					return "tvOS";
				case ApplePlatform.WatchOS:
					return "watchOS";
				case ApplePlatform.MacOSX:
					return "macOS";
				default:
					// FIXME: better error
					throw new NotImplementedException ($"Invalid platform: {Platform}");
				}
			}
		}

		public string GetAppManifest (string appBundlePath)
		{
			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.WatchOS:
			case ApplePlatform.TVOS:
				return Path.Combine (appBundlePath, "Info.plist");
			case ApplePlatform.MacOSX:
				return Path.Combine (appBundlePath, "Contents", "Info.plist");
			default:
				throw new NotImplementedException ($"Unknown platform: {Platform}"); // FIXME: better error

			}
		}

		public string GetDeploymentTarget (string appBundlePath)
		{
			var manifest = GetAppManifest (appBundlePath);
			var plist = PDictionary.FromFile (manifest);
			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.WatchOS:
			case ApplePlatform.TVOS:
				return plist.GetMinimumOSVersion ();
			case ApplePlatform.MacOSX:
				return plist.GetMinimumSystemVersion ();
			default:
				throw new NotImplementedException ($"Unknown platform: {Platform}"); // FIXME: better error

			}
		}
	}
}
