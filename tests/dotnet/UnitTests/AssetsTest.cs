using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

#nullable enable

namespace Xamarin.Tests {

	// Add the XCAssets before the build
	[TestFixture]
	public class AssetsTest : TestBaseClass {
		const string project = "AppWithXCAssets";

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64", true)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64", true)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", true)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64", true)] // https://github.com/dotnet/macios/issues/12410
																	 // Build, add the XCAssets, then build again
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64", false)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", false)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64", false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64", false)] // https://github.com/dotnet/macios/issues/12410
		public void TestXCAssets (ApplePlatform platform, string runtimeIdentifiers, bool isStartingWithAssets)
		{
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var config = "Debug";
			var projectPath = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: config);

			try {
				ConfigureAssets (projectPath, runtimeIdentifiers, config, isStartingWithAssets);

				TestXCAssetsImpl (platform, runtimeIdentifiers, isStartingWithAssets, projectPath, appPath);
			} finally {
				DeleteAssets (projectPath);
			}
		}

		void TestXCAssetsImpl (ApplePlatform platform, string runtimeIdentifiers, bool isStartingWithAssets, string projectPath, string appPath)
		{

			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			var resourcesDirectory = GetResourcesDirectory (platform, appPath);

			var assetsCar = Path.Combine (resourcesDirectory, "Assets.car");
			Assert.That (assetsCar, Does.Exist, "Assets.car");

			var doc = ProcessAssets (assetsCar, GetFullSdkVersion (platform, runtimeIdentifiers));
			Assert.That (doc, Is.Not.Null, "There was an issue processing the asset binary.");

			var foundAssets = FindAssets (platform, doc);

			// Seems the 2 vectors are not being consumed in MacCatalyst but they still appear in the image Datasets
			HashSet<string> expectedAssets;
			switch (platform) {
			case ApplePlatform.iOS:
				expectedAssets = ExpectedAssetsiOS;
				break;
			case ApplePlatform.TVOS:
				expectedAssets = ExpectedAssetstvOS;
				break;
			case ApplePlatform.MacOSX:
				expectedAssets = ExpectedAssetsmacOS;
				break;
			case ApplePlatform.MacCatalyst:
				expectedAssets = ExpectedAssetsMacCatalyst;
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}

			Assert.That (foundAssets, Is.EquivalentTo (expectedAssets), $"Incorrect assets in {assetsCar}");

			var arm64txt = Path.Combine (resourcesDirectory, "arm64.txt");
			var x64txt = Path.Combine (resourcesDirectory, "x64.txt");
			Assert.That (File.Exists (arm64txt), Is.EqualTo (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-arm64"))), "arm64.txt");
			Assert.That (File.Exists (x64txt), Is.EqualTo (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-x64"))), "x64.txt");
		}

		// Verify that image assets coming from a referenced library aren't lost when actool is re-run
		// on an incremental build (https://github.com/dotnet/macios/issues/5755). The app has its own
		// image asset ('AppImage'), and it references a library with another image asset ('Image').
		// Touching the app's own asset forces actool to regenerate Assets.car, and the library's asset
		// must still be present afterwards.
		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		public void LibraryImageAssetsSurviveIncrementalBuild (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			Configuration.IgnoreIfIgnoredPlatform (platform);

			const string project = "AppWithLibraryWithResourcesReference";
			var config = "Debug";
			var projectPath = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: config);
			Clean (projectPath);
			Clean (GetProjectPath ("LibraryWithResources", platform: platform));

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = config;

			// Clean build: both the app's own asset and the library's asset must be present.
			DotNet.AssertBuild (projectPath, properties);

			var sdkVersion = GetFullSdkVersion (platform, runtimeIdentifiers);
			var resourcesDirectory = GetResourcesDirectory (platform, appPath);
			var assetsCar = Path.Combine (resourcesDirectory, "Assets.car");
			Assert.That (assetsCar, Does.Exist, "Assets.car after clean build");

			var assetsAfterCleanBuild = FindImageAssetNames (assetsCar, sdkVersion);
			Assert.That (assetsAfterCleanBuild, Does.Contain ("AppImage"), "App image asset after clean build");
			Assert.That (assetsAfterCleanBuild, Does.Contain ("Image"), "Library image asset after clean build");

			// Touch the app's own asset so that actool re-runs and regenerates Assets.car.
			var appAsset = Path.Combine (Path.GetDirectoryName (projectPath)!, "..", "AppImages.xcassets", "AppImage.imageset", "Contents.json");
			Assert.That (appAsset, Does.Exist, "App asset to touch");
			Configuration.Touch (appAsset);

			// Incremental build: the library's asset must still be present.
			DotNet.AssertBuild (projectPath, properties);

			var assetsAfterIncrementalBuild = FindImageAssetNames (assetsCar, sdkVersion);
			Assert.That (assetsAfterIncrementalBuild, Does.Contain ("AppImage"), "App image asset after incremental build");
			Assert.That (assetsAfterIncrementalBuild, Does.Contain ("Image"), "Library image asset after incremental build (issue #5755)");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		public void ACToolExtraArgsInvalidateIncrementalBuild (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var projectPath = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			Clean (projectPath);

			var result = DotNet.AssertBuild (projectPath, properties);
			var allTargets = BinLog.GetAllTargets (result.BinLogPath);
			AssertTargetExecuted (allTargets, "_CoreCompileImageAssets", "Initial build");

			var assetsCar = Path.Combine (GetResourcesDirectory (platform, appPath), "Assets.car");
			Assert.That (assetsCar, Does.Exist, "Assets.car after initial build");
			var initialTimestamp = File.GetLastWriteTimeUtc (assetsCar);

			result = DotNet.AssertBuild (projectPath, properties);
			allTargets = BinLog.GetAllTargets (result.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_CoreCompileImageAssets", "Unchanged build");
			Assert.That (File.GetLastWriteTimeUtc (assetsCar), Is.EqualTo (initialTimestamp), "Assets.car after unchanged build");

			properties ["ACToolExtraArgs"] = "--compress-pngs";
			result = DotNet.AssertBuild (projectPath, properties);
			allTargets = BinLog.GetAllTargets (result.BinLogPath);
			AssertTargetExecuted (allTargets, "_CoreCompileImageAssets", "Changed ACToolExtraArgs");
			Assert.That (File.GetLastWriteTimeUtc (assetsCar), Is.GreaterThan (initialTimestamp), "Assets.car after changing ACToolExtraArgs");
		}

		// Returns the set of image asset (imageset) names in the given compiled Assets.car.
		static HashSet<string> FindImageAssetNames (string assetsCar, string sdkVersion)
		{
			var doc = ProcessAssets (assetsCar, sdkVersion);
			Assert.That (doc, Is.Not.Null, "There was an issue processing the asset binary.");

			var names = new HashSet<string> ();
			foreach (var item in doc.RootElement.EnumerateArray ()) {
				if (item.TryGetProperty ("AssetType", out var assetType) && assetType.ToString () == "Image" && item.TryGetProperty ("Name", out var name))
					names.Add (name.ToString ());
			}
			return names;
		}

		void ConfigureAssets (string projectPath, string runtimeIdentifiers, string config, bool isStartingWithAssets)
		{
			Clean (projectPath);

			// We either want the assets added before the build, or we will be adding them after the build
			if (isStartingWithAssets)
				CopyAssets (projectPath);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = config;

			DotNet.AssertBuild (projectPath, properties);
			if (!isStartingWithAssets) {
				CopyAssets (projectPath);
				DotNet.AssertBuild (projectPath, properties);
			}
		}

		void DeleteAssets (string projectPath)
		{
			var xcassetsDir = Path.Combine (projectPath, "../Assets.xcassets");
			File.Delete (xcassetsDir);
		}

		void CopyAssets (string projectPath)
		{
			var testingAssetsDir = new DirectoryInfo (Path.Combine (projectPath, "../../TestingAssets"));
			var xcassetsDir = new DirectoryInfo (Path.Combine (projectPath, "../Assets.xcassets"));

			Assert.That (testingAssetsDir, Does.Exist, $"Could not find testingAssetsDir: {testingAssetsDir}");
			MakeSymlinks (testingAssetsDir.FullName, xcassetsDir.FullName);
			Assert.That (xcassetsDir, Does.Exist, $"Could not find xcassetsDir: {xcassetsDir}");

			// update timestamps on all symlink files so msbuild spots them as new additions
			ProcessUpdateSymlink (xcassetsDir.FullName);
		}

		void MakeSymlinks (string sourceDir, string destDir)
		{
			var executable = "ln";
			var arguments = new string [] { "-s", sourceDir, destDir };
			var rv = Execution.RunAsync (executable, arguments, timeout: TimeSpan.FromSeconds (60)).Result;
			Assert.That (rv.ExitCode, Is.EqualTo (0), $"Creating Symlink Error: {rv.Output.MergedOutput}. Unexpected ExitCode");
		}

		public static string GetFullSdkVersion (ApplePlatform platform, string runtimeIdentifiers)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				if (runtimeIdentifiers.Contains ("simulator")) {
					return $"iphonesimulator{Configuration.ios_sdk_version}";
				} else {
					return $"iphoneos{Configuration.ios_sdk_version}";
				}
			case ApplePlatform.TVOS:
				if (runtimeIdentifiers.Contains ("simulator")) {
					return $"appletvsimulator{Configuration.tvos_sdk_version}";
				} else {
					return $"appletvos{Configuration.tvos_sdk_version}";
				}
			case ApplePlatform.MacOSX:
				return $"macosx{Configuration.macos_sdk_version}";
			case ApplePlatform.MacCatalyst:
				return $"macosx{Configuration.maccatalyst_sdk_version}";
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		// msbuild will only update the assets if they are newer than the outputs from previous build
		// so we will touch the first (non-DS_Store) file the symlink points to in order to give them newer modified times
		void ProcessUpdateSymlink (string xcassetsDir)
		{
			var assets = Directory.EnumerateFiles (xcassetsDir, "*.*", SearchOption.AllDirectories).ToArray ();

			// assets first value is a .DS_Store file that work trigger MSBuild recompile so we want the second value
			Assert.That (assets.Length, Is.GreaterThan (1));

			var executable = "touch";
			var arguments = new string [] { assets [1] };
			var rv = Execution.RunAsync (executable, arguments, timeout: TimeSpan.FromSeconds (120)).Result;
			Assert.That (rv.ExitCode, Is.EqualTo (0), $"Processing Update Symlink Error: {rv.Output.MergedOutput}. Unexpected ExitCode");
		}

		public static JsonDocument ProcessAssets (string assetsPath, string sdkVersion)
		{
			var executable = "xcrun";
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var tmpfile = Path.Combine (tmpdir, "Assets.json");
			var arguments = new string [] { "--sdk", sdkVersion, "assetutil", "--info", assetsPath, "-o", tmpfile };
			var rv = Execution.RunAsync (executable, arguments, timeout: TimeSpan.FromSeconds (120)).Result;
			Assert.That (rv.ExitCode, Is.EqualTo (0), $"Processing Assets Error: {rv.Output.StandardError}. Unexpected ExitCode");
			var s = File.ReadAllText (tmpfile);

			try {
				return JsonDocument.Parse (s);
			} catch (Exception e) {
				Console.WriteLine ($"Failure to parse json:");
				Console.WriteLine (e);
				Console.WriteLine ("Json document:");
				Console.WriteLine (s);
				Assert.Fail ($"Failure to parse json: {e.Message}\nJson document:\n{s}");
				throw;
			}
		}

		public static HashSet<string> FindAssets (ApplePlatform platform, JsonDocument doc)
		{
			var jsonArray = doc.RootElement.EnumerateArray ();
			var foundElements = new HashSet<string> ();

			foreach (var item in jsonArray) {
				var result = GetTarget (platform, item);
				if (result is not null)
					foundElements.Add (result);
			}
			return foundElements;
		}

		static string? GetTarget (ApplePlatform platform, JsonElement item)
		{
			if (item.TryGetProperty ("SchemaVersion", out var schemaVersion)) {
				switch (platform) {
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					Assert.That (schemaVersion.ToString (), Is.EqualTo ("2"), "Verify SchemaVersion");
					break;
				default:
					throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
				}
			} else if (item.TryGetProperty ("AssetType", out var assetType)) {
				foreach (var target in XCAssetTargets) {
					if (TryGetTarget (item, assetType, target, out var result))
						return result;
				}
				Assert.Fail ($"Unable to match asset type '{assetType}' for {item}'");
			} else {
				Assert.Fail ($"Unable to get property 'AssetType' for {item}");
			}
			return null;
		}

		static bool TryGetTarget (JsonElement item, JsonElement assetType, XCAssetTarget target, [NotNullWhen (true)] out string? result)
		{
			result = null;
			if (assetType.ToString () == target.AssetType && item.TryGetProperty (target.CategoryName, out var value)) {
				result = string.Concat (assetType.ToString (), ":", value.ToString ());
				return true;
			}
			return false;
		}

		static readonly HashSet<string> ExpectedAssetsAllPlatforms = new HashSet<string> () {
			"Color:ColorTest",
			"Contents:SpritesTest",
			"Data:BmpImageDataTest",
			"Data:DngImageDataTest",
			"Data:EpsImageDataTest",
			"Data:JsonDataTest",
			"Data:TiffImageDataTest",
			"Image:samplejpeg.jpeg",
			"Image:samplejpg.jpg",
			"Image:samplepdf.pdf",
			"Image:samplepng.png",
			"Image:samplepng2.png",
			"Image:spritejpeg.jpeg",
			"Image:xamlogo.svg",
			"Image:Icon16.png",
			"Image:Icon32.png",
			"PackedImage:ZZZZExplicitlyPackedAsset-1.0.0-gamut0",
			"Texture Rendition:TextureTest",
		};

		static HashSet<string> ExpectedAssetsMacCatalyst => new HashSet<string> (ExpectedAssetsAllPlatforms) {
			"Icon Image:Icon1024.png",
			"Icon Image:Icon128.png",
			"Icon Image:Icon16.png",
			"Icon Image:Icon256.png",
			"Icon Image:Icon32.png",
			"Icon Image:Icon512.png",
			"Icon Image:Icon64.png",
			"MultiSized Image:AlternateAppIcons",
			"MultiSized Image:AppIcons",
			"PackedImage:ZZZZPackedAsset-1.1.0-gamut0",
			"PackedImage:ZZZZPackedAsset-2.1.0-gamut0",
		};

		static readonly HashSet<string> ExpectedAssetsiOS = new HashSet<string> (ExpectedAssetsAllPlatforms) {
			"Icon Image:Icon1024.png",
			"Image:Icon64.png",
			"MultiSized Image:AlternateAppIcons",
			"MultiSized Image:AppIcons",
			"Vector:samplepdf.pdf",
			"Vector:xamlogo.svg",
		};

		static readonly HashSet<string> ExpectedAssetstvOS = new HashSet<string> (ExpectedAssetsAllPlatforms) {
			"Image:Icon-blue-1280x768.png",
			"Image:Icon-blue-1920x720.png",
			"Image:Icon-blue-2320x720.png",
			"Image:Icon-blue-3840x1440.png",
			"Image:Icon-blue-400x240.png",
			"Image:Icon-blue-4640x1440.png",
			"Image:Icon-green-400x240.png",
			"Image:ZZZZFlattenedImage-1.1.0-gamut0",
			"Image:ZZZZFlattenedImage-2.1.0-gamut0",
			"Image:ZZZZRadiosityImage-1.0.0",
			"Image:ZZZZRadiosityImage-2.0.0",
			"ImageStack:AlternateAppIcons",
			"ImageStack:AppIcons",
			"Vector:samplepdf.pdf",
			"Vector:xamlogo.svg",
		};

		static readonly HashSet<string> ExpectedAssetsmacOS = new HashSet<string> (ExpectedAssetsAllPlatforms) {
			"Icon Image:Icon1024.png",
			"Icon Image:Icon128.png",
			"Icon Image:Icon16.png",
			"Icon Image:Icon256.png",
			"Icon Image:Icon32.png",
			"Icon Image:Icon512.png",
			"Icon Image:Icon64.png",
			"MultiSized Image:AlternateAppIcons",
			"MultiSized Image:AppIcons",
			"PackedImage:ZZZZPackedAsset-1.1.0-gamut0",
			"PackedImage:ZZZZPackedAsset-2.1.0-gamut0",
			"Vector:samplepdf.pdf",
			"Vector:xamlogo.svg",
		};

		class XCAssetTarget {
			public string AssetType { get; set; }
			public string CategoryName { get; set; }
			public XCAssetTarget (string assetType, string categoryName)
			{
				AssetType = assetType;
				CategoryName = categoryName;
			}
		}

		static XCAssetTarget [] XCAssetTargets = {
				new ("Color", "Name"),
				new ("Contents", "Name"),
				new ("Data", "Name"),
				new ("Icon Image", "RenditionName"),
				new ("Image", "RenditionName"),
				new ("ImageStack", "Name"),
				new ("MultiSized Image", "Name"),
				new ("PackedImage", "RenditionName"),
				new ("Texture Rendition", "Name"),
				new ("Vector", "RenditionName"),
		};
	}
}
