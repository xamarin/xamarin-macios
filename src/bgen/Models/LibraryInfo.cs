using System.Collections.Generic;
using System.Linq;
using ObjCRuntime;
using Xamarin.Utils;

#nullable enable

public class LibraryInfo {
	private LibraryInfo () { }
	public string AttributeDll { get; set; }
	public TargetFramework TargetFramework { get; set; }
	public string baselibdll { get; set; }
	public bool nostdlib { get; set; }

	public bool IsDotNet {
		get { return TargetFramework.IsDotNet; }
	}

	public static class LibraryInfoBuilder {
		public static LibraryInfo Build (List<string> refs, BindingTouchConfig config)
		{
			LibraryInfo libraryInfo = new();
			SetTargetFramework (config.TargetFramework, libraryInfo);
			libraryInfo.nostdlib =  DetermineOmitStdLibrary (config.OmitStandardLibrary, libraryInfo.TargetFramework.Platform);
			libraryInfo.baselibdll = DetermineBaseLibDll (libraryInfo.TargetFramework, config.Baselibdll);
			libraryInfo.AttributeDll = config.Attributedll;
			AddAndFixReferences (libraryInfo, refs);
			return libraryInfo;
		}

		static void SetTargetFramework (string? fx, LibraryInfo libraryInfo)
		{
			if (fx is null)
				throw ErrorHelper.CreateError (86);
			TargetFramework tf;
			if (!TargetFramework.TryParse (fx, out tf))
				throw ErrorHelper.CreateError (68, fx);

			if (!TargetFramework.IsValidFramework (tf))
				throw ErrorHelper.CreateError (70, tf,
					string.Join (" ", TargetFramework.ValidFrameworks.Select ((v) => v.ToString ()).ToArray ()));

			libraryInfo.TargetFramework = tf;
		}

		static bool DetermineOmitStdLibrary(bool? omitStandardLibary, ApplePlatform currentPlatform)
		{
			if (omitStandardLibary is not null)
				return (bool)omitStandardLibary;

			switch (currentPlatform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				return true;
			default:
				throw ErrorHelper.CreateError (1053, currentPlatform);
			}
		}

		static string? DetermineBaseLibDll (TargetFramework TargetFramework, string? baselibdll)
		{
			if (!string.IsNullOrEmpty (baselibdll))
				return baselibdll;

			PlatformName currentPlatform = LibraryManager.DetermineCurrentPlatform (TargetFramework.Platform);
			switch (currentPlatform) {
			case PlatformName.iOS:
				return currentPlatform.GetPath ("lib/mono/Xamarin.iOS/Xamarin.iOS.dll");
			case PlatformName.TvOS:
				return currentPlatform.GetPath ("lib/mono/Xamarin.TVOS/Xamarin.TVOS.dll");
			case PlatformName.WatchOS:
				return currentPlatform.GetPath ("lib/mono/Xamarin.WatchOS/Xamarin.WatchOS.dll");
			case PlatformName.MacCatalyst:
				return currentPlatform.GetPath ("lib/mono/Xamarin.MacCatalyst/Xamarin.MacCatalyst.dll");
			case PlatformName.MacOSX:
				if (TargetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile)
					return currentPlatform.GetPath ("lib", "reference", "mobile", "Xamarin.Mac.dll");
				if (TargetFramework == TargetFramework.Xamarin_Mac_4_5_Full ||
				    TargetFramework == TargetFramework.Xamarin_Mac_4_5_System)
					return currentPlatform.GetPath ("lib", "reference", "full", "Xamarin.Mac.dll");
				if (TargetFramework == TargetFramework.DotNet_macOS)
					return currentPlatform.GetPath ("lib", "mono", "Xamarin.Mac", "Xamarin.Mac.dll");
				throw ErrorHelper.CreateError (1053, TargetFramework);
			default:
				throw ErrorHelper.CreateError (1053, TargetFramework);
			}
		}

		static void AddAndFixReferences (LibraryInfo libraryInfo, List<string> references)
		{
			PlatformName currentPlatform = LibraryManager.DetermineCurrentPlatform (libraryInfo.TargetFramework.Platform);
			switch (libraryInfo.TargetFramework.Platform) {
			case ApplePlatform.iOS:
				if (!libraryInfo.IsDotNet) {
					references.Add ("Facades/System.Drawing.Common");
					ReferenceFixer.FixSDKReferences (currentPlatform, "lib/mono/Xamarin.iOS", references);
				}
				break;
			case ApplePlatform.TVOS:
				if (!libraryInfo.IsDotNet) {
					references.Add ("Facades/System.Drawing.Common");
					ReferenceFixer.FixSDKReferences (currentPlatform, "lib/mono/Xamarin.TVOS", references);
				}
				break;
			case ApplePlatform.WatchOS:
				if (!libraryInfo.IsDotNet) {
					references.Add ("Facades/System.Drawing.Common");
					ReferenceFixer.FixSDKReferences (currentPlatform, "lib/mono/Xamarin.WatchOS", references);
				}
				break;
			case ApplePlatform.MacCatalyst:
				if (!libraryInfo.IsDotNet) {
					ReferenceFixer.FixSDKReferences (currentPlatform, "lib/mono/Xamarin.MacCatalyst", references);
				}
				break;
			case ApplePlatform.MacOSX:
				if (libraryInfo.TargetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
					references.Add ("Facades/System.Drawing.Common");
					ReferenceFixer.FixSDKReferences (currentPlatform, "lib/mono/Xamarin.Mac", references);
				} else if (libraryInfo.TargetFramework == TargetFramework.Xamarin_Mac_4_5_Full) {
					references.Add ("Facades/System.Drawing.Common");
					ReferenceFixer.FixSDKReferences (currentPlatform, "lib/mono/4.5", references);
				} else if (libraryInfo.TargetFramework == TargetFramework.Xamarin_Mac_4_5_System) {
					ReferenceFixer.FixSDKReferences ("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5",
						references, forceSystemDrawing: true);
				} else if (libraryInfo.TargetFramework == TargetFramework.DotNet_macOS) {
					// Do nothing
				} else {
					throw ErrorHelper.CreateError (1053, libraryInfo.TargetFramework);
				}
				break;
			default:
				throw ErrorHelper.CreateError (1053, libraryInfo.TargetFramework);
			}
		}
	}
}
