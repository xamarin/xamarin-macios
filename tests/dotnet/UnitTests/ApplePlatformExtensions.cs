using Xamarin.Tests;

namespace Xamarin.Utils {
	public static class ApplePlatformExtensionsWithVersions {
		public static string ToFrameworkWithPlatformVersion (this ApplePlatform @this, bool isExecutable /* and not library */)
		{
			var netVersion = Configuration.DotNetTfm;
			var targetPlatformVersion = isExecutable ? GetDefaultTargetPlatformVersionExecutable (@this) : GetDefaultTargetPlatformVersionLibrary (@this);
			switch (@this) {
			case ApplePlatform.iOS:
				return netVersion + "-ios" + targetPlatformVersion;
			case ApplePlatform.MacOSX:
				return netVersion + "-macos" + targetPlatformVersion;
			case ApplePlatform.TVOS:
				return netVersion + "-tvos" + targetPlatformVersion;
			case ApplePlatform.MacCatalyst:
				return netVersion + "-maccatalyst" + targetPlatformVersion;
			default:
				return "Unknown";
			}
		}

		public static string GetDefaultTargetPlatformVersionExecutable (this ApplePlatform @this)
		{
			switch (@this) {
			case ApplePlatform.iOS: return SdkVersions.TargetPlatformVersionExecutableiOS;
			case ApplePlatform.TVOS: return SdkVersions.TargetPlatformVersionExecutabletvOS;
			case ApplePlatform.MacOSX: return SdkVersions.TargetPlatformVersionExecutablemacOS;
			case ApplePlatform.MacCatalyst: return SdkVersions.TargetPlatformVersionExecutableMacCatalyst;
			default:
				return "Unknown";
			}
		}

		public static string GetDefaultTargetPlatformVersionLibrary (this ApplePlatform @this)
		{
			switch (@this) {
			case ApplePlatform.iOS: return SdkVersions.TargetPlatformVersionLibraryiOS;
			case ApplePlatform.TVOS: return SdkVersions.TargetPlatformVersionLibrarytvOS;
			case ApplePlatform.MacOSX: return SdkVersions.TargetPlatformVersionLibrarymacOS;
			case ApplePlatform.MacCatalyst: return SdkVersions.TargetPlatformVersionLibraryMacCatalyst;
			default:
				return "Unknown";
			}
		}
	}

	[TestFixture]
	public class TargetFrameworkTest {
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		public void DefaultLibraryTargetPlatformVersion (ApplePlatform platform)
		{
			// We might have to change the assert if the first minor OS version we release for a given .NET version is >0 (this happened for both .NET 7 and .NET 8).
			Assert.That (platform.GetDefaultTargetPlatformVersionLibrary (), Does.EndWith (".0"), "Default TPV for a library must end with .0");
		}

		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		public void MajorTargetPlatformVersion (ApplePlatform platform)
		{
			var vLibrary = Version.Parse (platform.GetDefaultTargetPlatformVersionLibrary ());
			var vExecutable = Version.Parse (platform.GetDefaultTargetPlatformVersionExecutable ());
			// We might have to change the assert if we release support for a new major OS version within a .NET releases (this happened for .NET 8)
			Assert.AreEqual (vExecutable.Major, vLibrary.Major, "The major version must be the same between the default TPV for library and executable projects.");
		}
	}
}
