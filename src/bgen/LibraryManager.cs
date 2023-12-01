using System.Collections.Generic;
using ObjCRuntime;
using Xamarin.Utils;

#nullable enable

public class LibraryManager {
	public List<string> Libraries = new ();
	public string GetAttributeLibraryPath (LibraryInfo libraryInfo, PlatformName currentPlatform)
	{
		if (!string.IsNullOrEmpty (libraryInfo.AttributeDll))
			return libraryInfo.AttributeDll!;

		if (libraryInfo.IsDotNet)
			return currentPlatform.GetPath ("lib", "Xamarin.Apple.BindingAttributes.dll");

		switch (currentPlatform) {
		case PlatformName.iOS:
			return currentPlatform.GetPath ("lib", "bgen", "Xamarin.iOS.BindingAttributes.dll");
		case PlatformName.WatchOS:
			return currentPlatform.GetPath ("lib", "bgen", "Xamarin.WatchOS.BindingAttributes.dll");
		case PlatformName.TvOS:
			return currentPlatform.GetPath ("lib", "bgen", "Xamarin.TVOS.BindingAttributes.dll");
		case PlatformName.MacCatalyst:
			return currentPlatform.GetPath ("lib", "bgen", "Xamarin.MacCatalyst.BindingAttributes.dll");
		case PlatformName.MacOSX:
			if (libraryInfo.TargetFramework == TargetFramework.Xamarin_Mac_4_5_Full) {
				return currentPlatform.GetPath ("lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (libraryInfo.TargetFramework == TargetFramework.Xamarin_Mac_4_5_System) {
				return currentPlatform.GetPath ("lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (libraryInfo.TargetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
				return currentPlatform.GetPath ("lib", "bgen", "Xamarin.Mac-mobile.BindingAttributes.dll");
			} else {
				throw ErrorHelper.CreateError (1053, libraryInfo.TargetFramework);
			}
		default:
			throw new BindingException (1047, currentPlatform);
		}
	}

	public IEnumerable<string> GetLibraryDirectories (LibraryInfo libraryInfo, PlatformName currentPlatform)
	{
		if (!libraryInfo.IsDotNet) {
			switch (currentPlatform) {
			case PlatformName.iOS:
				yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.iOS");
				break;
			case PlatformName.WatchOS:
				yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.WatchOS");
				break;
			case PlatformName.TvOS:
				yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.TVOS");
				break;
			case PlatformName.MacCatalyst:
				yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.MacCatalyst");
				break;
			case PlatformName.MacOSX:
				if (libraryInfo.TargetFramework == TargetFramework.Xamarin_Mac_4_5_Full) {
					yield return currentPlatform.GetPath ("lib", "reference", "full");
					yield return currentPlatform.GetPath ("lib", "mono", "4.5");
				} else if (libraryInfo.TargetFramework == TargetFramework.Xamarin_Mac_4_5_System) {
					yield return "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5";
					yield return currentPlatform.GetPath ("lib", "mono", "4.5");
				} else if (libraryInfo.TargetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
					yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.Mac");
				} else {
					throw ErrorHelper.CreateError (1053, libraryInfo.TargetFramework);
				}
				break;
			default:
				throw new BindingException (1047, currentPlatform);
			}
		}
		foreach (var lib in Libraries)
			yield return lib;
	}

	public static bool DetermineSkipSystemDrawing (TargetFramework targetFramework) =>
		 targetFramework.Platform is ApplePlatform.MacOSX &&
			   (targetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile ||
				targetFramework == TargetFramework.Xamarin_Mac_4_5_Full);

	public static PlatformName DetermineCurrentPlatform (ApplePlatform applePlatform)
	{
		switch (applePlatform) {
		case ApplePlatform.iOS:
			return PlatformName.iOS;
		case ApplePlatform.TVOS:
			return PlatformName.TvOS;
		case ApplePlatform.WatchOS:
			return PlatformName.WatchOS;
		case ApplePlatform.MacCatalyst:
			return PlatformName.MacCatalyst;
		case ApplePlatform.MacOSX:
			return PlatformName.MacOSX;
		default:
			return PlatformName.None;
		}
	}
}

