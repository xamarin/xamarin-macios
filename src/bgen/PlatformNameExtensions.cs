using System;
using System.IO;
using ObjCRuntime;
using Xamarin.Utils;

public static class PlatformNameExtensions {

	public static string GetApplicationClassName (this PlatformName currentPlatform)
	{
		switch (currentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
		case PlatformName.MacCatalyst:
			return "UIApplication";
		case PlatformName.MacOSX:
			return "NSApplication";
		default:
			throw new BindingException (1047, currentPlatform);
		}
	}

	public static int GetXamcoreVersion (this PlatformName currentPlatform)
	{
#if NET
		return 4;
#else
		switch (currentPlatform) {
		case PlatformName.MacOSX:
		case PlatformName.iOS:
			return 2;
		case PlatformName.TvOS:
		case PlatformName.WatchOS:
			return 3;
		default:
			return 4;
		}
#endif
	}

	public static string GetCoreImageMap (this PlatformName currentPlatform)
	{
		switch (currentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
		case PlatformName.MacCatalyst:
			return "CoreImage";
		case PlatformName.MacOSX:
			return "Quartz";
		default:
			throw new BindingException (1047, currentPlatform);
		}
	}

	public static string GetCoreServicesMap (this PlatformName currentPlatform)
	{
		switch (currentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
		case PlatformName.MacCatalyst:
			return "MobileCoreServices";
		case PlatformName.MacOSX:
			return "CoreServices";
		default:
			throw new BindingException (1047, currentPlatform);
		}
	}

	public static string GetPDFKitMap (this PlatformName currentPlatform)
	{
		switch (currentPlatform) {
		case PlatformName.iOS:
		case PlatformName.MacCatalyst:
			return "PDFKit";
		case PlatformName.MacOSX:
			return "Quartz";
		default:
			throw new BindingException (1047, currentPlatform);
		}
	}

	public static ApplePlatform AsApplePlatform (this PlatformName platform)
	{
		switch (platform) {
		case PlatformName.iOS:
			return ApplePlatform.iOS;
		case PlatformName.TvOS:
			return ApplePlatform.TVOS;
		case PlatformName.MacCatalyst:
			return ApplePlatform.MacCatalyst;
		case PlatformName.MacOSX:
			return ApplePlatform.MacOSX;
		case PlatformName.WatchOS:
			return ApplePlatform.WatchOS;
		case PlatformName.None:
			return ApplePlatform.None;
		default:
			throw new ArgumentOutOfRangeException (nameof (platform), platform, $"Unknown platform: {platform}");
		}
	}

	static string GetSdkRoot (this PlatformName currentPlatform)
	{
		switch (currentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
		case PlatformName.MacCatalyst:
			var sdkRoot = Environment.GetEnvironmentVariable ("MD_MTOUCH_SDK_ROOT");
			if (string.IsNullOrEmpty (sdkRoot))
				sdkRoot = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current";
			return sdkRoot;
		case PlatformName.MacOSX:
			var macSdkRoot = Environment.GetEnvironmentVariable ("XamarinMacFrameworkRoot");
			if (string.IsNullOrEmpty (macSdkRoot))
				macSdkRoot = "/Library/Frameworks/Xamarin.Mac.framework/Versions/Current";
			return macSdkRoot;
		default:
			throw new BindingException (1047, currentPlatform);
		}
	}

	public static string GetPath (this PlatformName currentPlatform, params string [] paths)
	{
		var fullPaths = new string [paths.Length + 1];
		fullPaths [0] = currentPlatform.GetSdkRoot ();
		Array.Copy (paths, 0, fullPaths, 1, paths.Length);
		return Path.Combine (fullPaths);
	}
}
