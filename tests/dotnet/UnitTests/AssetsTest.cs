using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

#nullable enable

namespace Xamarin.Tests {

	// Add the XCAssets before the build
	[TestFixture]
	public class AssetsTest : TestBaseClass {
		const string project = "AppWithXCAssets";

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", true)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", true)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", true)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64", true)] // https://github.com/xamarin/xamarin-macios/issues/12410
																	 // Build, add the XCAssets, then build again
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", false)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", false)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64", false)] // https://github.com/xamarin/xamarin-macios/issues/12410
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
			Assert.IsNotNull (doc, "There was an issue processing the asset binary.");

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

			CollectionAssert.AreEquivalent (expectedAssets, foundAssets, $"Incorrect assets in {assetsCar}");

			var arm64txt = Path.Combine (resourcesDirectory, "arm64.txt");
			var x64txt = Path.Combine (resourcesDirectory, "x64.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-arm64")), File.Exists (arm64txt), "arm64.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-x64")), File.Exists (x64txt), "x64.txt");
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
			var output = new StringBuilder ();
			var executable = "ln";
			var arguments = new string [] { "-s", sourceDir, destDir };
			var rv = Execution.RunWithStringBuildersAsync (executable, arguments, standardOutput: output, standardError: output, timeout: TimeSpan.FromSeconds (60)).Result;
			Assert.AreEqual (0, rv.ExitCode, $"Creating Symlink Error: {rv.StandardError}. Unexpected ExitCode");
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

		// msbuild will only update the assets if they are newer than the outputs from previous build
		// so we will touch the first (non-DS_Store) file the symlink points to in order to give them newer modified times
		void ProcessUpdateSymlink (string xcassetsDir)
		{
			var output = new StringBuilder ();
			var assets = Directory.EnumerateFiles (xcassetsDir, "*.*", SearchOption.AllDirectories).ToArray ();

			// assets first value is a .DS_Store file that work trigger MSBuild recompile so we want the second value
			Assert.Greater (assets.Length, 1);

			var executable = "touch";
			var arguments = new string [] { assets [1] };
			var rv = Execution.RunWithStringBuildersAsync (executable, arguments, standardOutput: output, standardError: output, timeout: TimeSpan.FromSeconds (120)).Result;
			Assert.AreEqual (0, rv.ExitCode, $"Processing Update Symlink Error: {rv.StandardError}. Unexpected ExitCode");
		}

		public static JsonDocument ProcessAssets (string assetsPath, string sdkVersion)
		{
			var output = new StringBuilder ();
			var stderr = new StringBuilder ();
			var executable = "xcrun";
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var tmpfile = Path.Combine (tmpdir, "Assets.json");
			var arguments = new string [] { "--sdk", sdkVersion, "assetutil", "--info", assetsPath, "-o", tmpfile };
			var rv = Execution.RunWithStringBuildersAsync (executable, arguments, standardOutput: output, standardError: stderr, timeout: TimeSpan.FromSeconds (120)).Result;
			Assert.AreEqual (0, rv.ExitCode, $"Processing Assets Error: {stderr}. Unexpected ExitCode");
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
					Assert.AreEqual ("5", schemaVersion.ToString (), "Verify SchemaVersion");
					break;
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					Assert.AreEqual ("2", schemaVersion.ToString (), "Verify SchemaVersion");
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
			"PackedImage:ZZZZExplicitlyPackedAsset-1.0.0-gamut0",
			"Texture Rendition:TextureTest",
		};

		static HashSet<string> ExpectedAssetsMacCatalyst => new HashSet<string> (ExpectedAssetsAllPlatforms) {
			"Image:Icon16.png",
			"Image:Icon32.png",
		};

		static readonly HashSet<string> ExpectedAssetsiOS = new HashSet<string> (ExpectedAssetsAllPlatforms) {
			"Vector:samplepdf.pdf",
			"Vector:xamlogo.svg",
			"Image:Icon16.png",
			"Image:Icon32.png",
			"Image:Icon64.png",
		};

		static readonly HashSet<string> ExpectedAssetstvOS = new HashSet<string> (ExpectedAssetsAllPlatforms) {
			"Vector:samplepdf.pdf",
			"Vector:xamlogo.svg",
			"Image:Icon16.png",
			"Image:Icon32.png",
		};

		static readonly HashSet<string> ExpectedAssetsmacOS = new HashSet<string> (ExpectedAssetsAllPlatforms) {
			"Vector:samplepdf.pdf",
			"Vector:xamlogo.svg",
			"Image:Icon16.png",
			"Image:Icon32.png",
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
