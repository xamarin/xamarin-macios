using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

#nullable enable

namespace Xamarin.Tests {
	public class AppIconTest : TestBaseClass {
		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void DefaultValues (ApplePlatform platform, string runtimeIdentifiers)
		{
			var expectedAssets = new HashSet<string> ();
			switch (platform) {
			case ApplePlatform.iOS:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:Icon64.png");
				break;
			case ApplePlatform.TVOS:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			case ApplePlatform.MacOSX:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			case ApplePlatform.MacCatalyst:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}

			TestXCAssetsImpl (platform, runtimeIdentifiers, extraAssets: expectedAssets.ToArray ());
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void IncludeAllIcons (ApplePlatform platform, string runtimeIdentifiers)
		{
			var expectedAssets = new HashSet<string> ();
			switch (platform) {
			case ApplePlatform.iOS:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:Icon64.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				expectedAssets.Add ("MultiSized Image:AppIcons");
				break;
			case ApplePlatform.TVOS:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			case ApplePlatform.MacOSX:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Icon Image:Icon128.png");
				expectedAssets.Add ("Icon Image:Icon16.png");
				expectedAssets.Add ("Icon Image:Icon256.png");
				expectedAssets.Add ("Icon Image:Icon32.png");
				expectedAssets.Add ("Icon Image:Icon512.png");
				expectedAssets.Add ("Icon Image:Icon64.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				expectedAssets.Add ("MultiSized Image:AppIcons");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-1.1.0-gamut0");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-2.1.0-gamut0");
				break;
			case ApplePlatform.MacCatalyst:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}

			TestXCAssetsImpl (
				platform,
				runtimeIdentifiers,
				new Dictionary<string, string> () { { "IncludeAllAppIcons", "true" } },
				extraAssets: expectedAssets.ToArray ());
		}


		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void XSAppIconAssets (ApplePlatform platform, string runtimeIdentifiers)
		{
			var expectedAssets = new HashSet<string> ();
			switch (platform) {
			case ApplePlatform.iOS:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:Icon64.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				break;
			case ApplePlatform.TVOS:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			case ApplePlatform.MacOSX:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Icon Image:Icon128.png");
				expectedAssets.Add ("Icon Image:Icon16.png");
				expectedAssets.Add ("Icon Image:Icon256.png");
				expectedAssets.Add ("Icon Image:Icon32.png");
				expectedAssets.Add ("Icon Image:Icon512.png");
				expectedAssets.Add ("Icon Image:Icon64.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-1.1.0-gamut0");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-2.1.0-gamut0");
				break;
			case ApplePlatform.MacCatalyst:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Icon Image:Icon128.png");
				expectedAssets.Add ("Icon Image:Icon16.png");
				expectedAssets.Add ("Icon Image:Icon256.png");
				expectedAssets.Add ("Icon Image:Icon32.png");
				expectedAssets.Add ("Icon Image:Icon512.png");
				expectedAssets.Add ("Icon Image:Icon64.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-1.1.0-gamut0");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-2.1.0-gamut0");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}

			TestXCAssetsImpl (
				platform,
				runtimeIdentifiers,
				new Dictionary<string, string> () {
					{ "_XSAppIconAssets", "Resources/Images.xcassets/AlternateAppIcons.appiconset" }
				},
				expectedAssets.ToArray ());
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		// launch images don't exist on Mac Catalyst or macOS.
		public void XSLaunchImageAssets (ApplePlatform platform, string runtimeIdentifiers)
		{
			var expectedAssets = new HashSet<string> ();
			switch (platform) {
			case ApplePlatform.iOS:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:Icon64.png");
				break;
			case ApplePlatform.TVOS:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon1920x1080.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:Icon3840x2160.png");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}

			TestXCAssetsImpl (
				platform,
				runtimeIdentifiers,
				new Dictionary<string, string> () {
					{ "_XSLaunchImageAssets", $"Resources/Images.xcassets/{platform.AsString ()}LaunchImage.launchimage" }
				},
				expectedAssets.ToArray ());
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void AlternateAppIcon (ApplePlatform platform, string runtimeIdentifiers)
		{
			var expectedAssets = new HashSet<string> ();
			switch (platform) {
			case ApplePlatform.iOS:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:Icon64.png");
				expectedAssets.Add ("MultiSized Image:AppIcons");
				break;
			case ApplePlatform.TVOS:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			case ApplePlatform.MacOSX:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Icon Image:Icon128.png");
				expectedAssets.Add ("Icon Image:Icon16.png");
				expectedAssets.Add ("Icon Image:Icon256.png");
				expectedAssets.Add ("Icon Image:Icon32.png");
				expectedAssets.Add ("Icon Image:Icon512.png");
				expectedAssets.Add ("Icon Image:Icon64.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("MultiSized Image:AppIcons");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-1.1.0-gamut0");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-2.1.0-gamut0");
				break;
			case ApplePlatform.MacCatalyst:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}

			TestXCAssetsImpl (
				platform,
				runtimeIdentifiers,
				new Dictionary<string, string> () {
					{ "AddTheseAlternateAppIcons", platform == ApplePlatform.TVOS ? "BrandAssets" : "AppIcons" }
				},
				expectedAssets.ToArray ());
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void AlternateAppIcons (ApplePlatform platform, string runtimeIdentifiers)
		{
			var expectedAssets = new HashSet<string> ();
			switch (platform) {
			case ApplePlatform.iOS:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:Icon64.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				expectedAssets.Add ("MultiSized Image:AppIcons");
				break;
			case ApplePlatform.TVOS:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			case ApplePlatform.MacOSX:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Icon Image:Icon128.png");
				expectedAssets.Add ("Icon Image:Icon16.png");
				expectedAssets.Add ("Icon Image:Icon256.png");
				expectedAssets.Add ("Icon Image:Icon32.png");
				expectedAssets.Add ("Icon Image:Icon512.png");
				expectedAssets.Add ("Icon Image:Icon64.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				expectedAssets.Add ("MultiSized Image:AppIcons");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-1.1.0-gamut0");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-2.1.0-gamut0");
				break;
			case ApplePlatform.MacCatalyst:
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}

			TestXCAssetsImpl (
				platform,
				runtimeIdentifiers,
				new Dictionary<string, string> () {
					{ "AddTheseAlternateAppIcons", platform == ApplePlatform.TVOS ? "BrandAssets;AlternateBrandAssets" : "AppIcons;AlternateAppIcons" }
				},
				expectedAssets.ToArray ());
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void AlternateAppIcon_Failure (ApplePlatform platform, string runtimeIdentifiers)
		{
			TestXCAssetsImpl (
				platform,
				runtimeIdentifiers,
				new Dictionary<string, string> () {
					{ "AddTheseAlternateAppIcons", "InexistentAppIcon" }
				},
				expectedErrorMessages: new string [] { "Can't find the AlternateAppIcon 'InexistentAppIcon' among the image resources." });
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void AppIcon_1 (ApplePlatform platform, string runtimeIdentifiers)
		{
			var expectedAssets = new HashSet<string> ();
			switch (platform) {
			case ApplePlatform.iOS:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:Icon64.png");
				expectedAssets.Add ("MultiSized Image:AppIcons");
				break;
			case ApplePlatform.TVOS:
				expectedAssets.Add ("Image:Icon-blue-1280x768.png");
				expectedAssets.Add ("Image:Icon-blue-1920x720.png");
				expectedAssets.Add ("Image:Icon-blue-2320x720.png");
				expectedAssets.Add ("Image:Icon-blue-3840x1440.png");
				expectedAssets.Add ("Image:Icon-blue-400x240.png");
				expectedAssets.Add ("Image:Icon-blue-4640x1440.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:ZZZZFlattenedImage-1.1.0-gamut0");
				expectedAssets.Add ("Image:ZZZZFlattenedImage-2.1.0-gamut0");
				expectedAssets.Add ("Image:ZZZZRadiosityImage-1.0.0");
				expectedAssets.Add ("Image:ZZZZRadiosityImage-2.0.0");
				expectedAssets.Add ("ImageStack:AppIcons");
				break;
			case ApplePlatform.MacOSX:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Icon Image:Icon128.png");
				expectedAssets.Add ("Icon Image:Icon16.png");
				expectedAssets.Add ("Icon Image:Icon256.png");
				expectedAssets.Add ("Icon Image:Icon32.png");
				expectedAssets.Add ("Icon Image:Icon512.png");
				expectedAssets.Add ("Icon Image:Icon64.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("MultiSized Image:AppIcons");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-1.1.0-gamut0");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-2.1.0-gamut0");
				break;
			case ApplePlatform.MacCatalyst:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Icon Image:Icon128.png");
				expectedAssets.Add ("Icon Image:Icon16.png");
				expectedAssets.Add ("Icon Image:Icon256.png");
				expectedAssets.Add ("Icon Image:Icon32.png");
				expectedAssets.Add ("Icon Image:Icon512.png");
				expectedAssets.Add ("Icon Image:Icon64.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("MultiSized Image:AppIcons");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-1.1.0-gamut0");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-2.1.0-gamut0");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
			TestXCAssetsImpl (
				platform,
				runtimeIdentifiers,
				new Dictionary<string, string> () {
					{ "AppIcon", platform == ApplePlatform.TVOS ? "BrandAssets" : "AppIcons" }
				},
				expectedAssets.ToArray ());
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void AppIcon_2 (ApplePlatform platform, string runtimeIdentifiers)
		{
			var expectedAssets = new HashSet<string> ();
			switch (platform) {
			case ApplePlatform.iOS:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:Icon64.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				break;
			case ApplePlatform.TVOS:
				expectedAssets.Add ("Image:Icon-green-1280x768.png");
				expectedAssets.Add ("Image:Icon-green-1920x720.png");
				expectedAssets.Add ("Image:Icon-green-2320x720.png");
				expectedAssets.Add ("Image:Icon-green-3840x1440.png");
				expectedAssets.Add ("Image:Icon-green-400x240.png");
				expectedAssets.Add ("Image:Icon-green-4640x1440.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("Image:ZZZZFlattenedImage-1.1.0-gamut0");
				expectedAssets.Add ("Image:ZZZZFlattenedImage-2.1.0-gamut0");
				expectedAssets.Add ("Image:ZZZZRadiosityImage-1.0.0");
				expectedAssets.Add ("Image:ZZZZRadiosityImage-2.0.0");
				expectedAssets.Add ("ImageStack:AlternateAppIcons");
				break;
			case ApplePlatform.MacOSX:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Icon Image:Icon128.png");
				expectedAssets.Add ("Icon Image:Icon16.png");
				expectedAssets.Add ("Icon Image:Icon256.png");
				expectedAssets.Add ("Icon Image:Icon32.png");
				expectedAssets.Add ("Icon Image:Icon512.png");
				expectedAssets.Add ("Icon Image:Icon64.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-1.1.0-gamut0");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-2.1.0-gamut0");
				break;
			case ApplePlatform.MacCatalyst:
				expectedAssets.Add ("Icon Image:Icon1024.png");
				expectedAssets.Add ("Icon Image:Icon128.png");
				expectedAssets.Add ("Icon Image:Icon16.png");
				expectedAssets.Add ("Icon Image:Icon256.png");
				expectedAssets.Add ("Icon Image:Icon32.png");
				expectedAssets.Add ("Icon Image:Icon512.png");
				expectedAssets.Add ("Icon Image:Icon64.png");
				expectedAssets.Add ("Image:Icon16.png");
				expectedAssets.Add ("Image:Icon32.png");
				expectedAssets.Add ("MultiSized Image:AlternateAppIcons");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-1.1.0-gamut0");
				expectedAssets.Add ("PackedImage:ZZZZPackedAsset-2.1.0-gamut0");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
			TestXCAssetsImpl (
				platform,
				runtimeIdentifiers,
				new Dictionary<string, string> () {
					{ "AppIcon", platform == ApplePlatform.TVOS ? "AlternateBrandAssets" : "AlternateAppIcons" }
				},
				expectedAssets.ToArray ());
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void AppIcon_Failure (ApplePlatform platform, string runtimeIdentifiers)
		{
			TestXCAssetsImpl (
				platform,
				runtimeIdentifiers,
				new Dictionary<string, string> () {
					{ "AppIcon", "InexistentAppIcon" }
				},
				expectedErrorMessages: new string [] { "Can't find the AppIcon 'InexistentAppIcon' among the image resources." });
		}

		void TestXCAssetsImpl (ApplePlatform platform, string runtimeIdentifiers, Dictionary<string, string>? extraProperties = null, IEnumerable<string>? extraAssets = null, string []? expectedErrorMessages = null)
		{
			var projectPath = string.Empty;
			var appPath = string.Empty;

			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			var project = "AppWithXCAssets";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			projectPath = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out appPath);

			Clean (projectPath);

			var properties = GetDefaultProperties (runtimeIdentifiers, extraProperties);
			if (expectedErrorMessages is not null) {
				var rv = DotNet.AssertBuildFailure (projectPath, properties);
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
				AssertErrorMessages (errors, expectedErrorMessages);
				return; // nothing else to test here
			}

			DotNet.AssertBuild (projectPath, properties);

			var resourcesDirectory = GetResourcesDirectory (platform, appPath);
			var assetsCar = Path.Combine (resourcesDirectory, "Assets.car");
			Assert.That (assetsCar, Does.Exist, "Assets.car");

			Console.WriteLine ($"Assets: {assetsCar}");

			var doc = AssetsTest.ProcessAssets (assetsCar, GetFullSdkVersion (platform, runtimeIdentifiers));
			Assert.IsNotNull (doc, "There was an issue processing the asset binary.");

			var foundAssets = AssetsTest.FindAssets (platform, doc);

			var expectedAssets = new HashSet<string> ();
			if (extraAssets is not null) {
				foreach (var asset in extraAssets)
					expectedAssets.Add (asset);
			}

			CollectionAssert.AreEquivalent (expectedAssets, foundAssets, "Incorrect assets");
		}

		public static string GetFullSdkVersion (ApplePlatform platform, string runtimeIdentifiers)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				if (runtimeIdentifiers.Contains ("simulator")) {
					return $"iphonesimulator{Configuration.sdk_version}";
				} else {
					return $"iphoneos{Configuration.sdk_version}";
				}
			case ApplePlatform.TVOS:
				if (runtimeIdentifiers.Contains ("simulator")) {
					return $"appletvsimulator{Configuration.tvos_sdk_version}";
				} else {
					return $"appletvos{Configuration.tvos_sdk_version}";
				}
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return $"macosx{Configuration.macos_sdk_version}";
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}
	}
}
