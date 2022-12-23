using System.Text.Json;

#nullable enable

namespace Xamarin.Tests {

	// Add the XCAssets before the build
	[TestFixture (ApplePlatform.iOS, "iossimulator-x64", "iphonesimulator", true)]
	[TestFixture (ApplePlatform.iOS, "ios-arm64;ios-arm", "iphoneos", true)]
	[TestFixture (ApplePlatform.TVOS, "tvossimulator-x64", "appletvsimulator", true)]
	[TestFixture (ApplePlatform.MacCatalyst, "maccatalyst-x64", "macosx", true)]
	[TestFixture (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", "macosx", true)]
	[TestFixture (ApplePlatform.MacOSX, "osx-x64", "macosx", true)]
	[TestFixture (ApplePlatform.MacOSX, "osx-arm64;osx-x64", "macosx", true)] // https://github.com/xamarin/xamarin-macios/issues/12410
																			  // Build, add the XCAssets, then build again
	[TestFixture (ApplePlatform.iOS, "iossimulator-x64", "iphonesimulator", false)]
	[TestFixture (ApplePlatform.iOS, "ios-arm64;ios-arm", "iphoneos", false)]
	[TestFixture (ApplePlatform.TVOS, "tvossimulator-x64", "appletvsimulator", false)]
	[TestFixture (ApplePlatform.MacCatalyst, "maccatalyst-x64", "macosx", false)]
	[TestFixture (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", "macosx", false)]
	[TestFixture (ApplePlatform.MacOSX, "osx-x64", "macosx", false)]
	[TestFixture (ApplePlatform.MacOSX, "osx-arm64;osx-x64", "macosx", false)] // https://github.com/xamarin/xamarin-macios/issues/12410
	public class AssetsTest : TestBaseClass {

		readonly ApplePlatform platform;
		readonly string runtimeIdentifiers;
		readonly string sdkVersion;
		readonly bool isStartingWithAssets;
		string projectPath = string.Empty;
		string appPath = string.Empty;

		public AssetsTest (ApplePlatform platform, string runtimeIdentifiers, string sdkVersion, bool isStartingWithAssets)
		{
			this.platform = platform;
			this.runtimeIdentifiers = runtimeIdentifiers;
			this.sdkVersion = sdkVersion;
			this.isStartingWithAssets = isStartingWithAssets;
		}

		[SetUp]
		public void Init ()
		{
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			var project = "AppWithXCAssets";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			projectPath = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out appPath);
			ConfigureAssets ();
		}

		[TearDown]
		public void Cleanup ()
		{
			DeleteAssets ();
		}

		[Test]
		public void TestXCAssets ()
		{
			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			var resourcesDirectory = GetResourcesDirectory (platform, appPath);

			var assetsCar = Path.Combine (resourcesDirectory, "Assets.car");
			Assert.That (assetsCar, Does.Exist, "Assets.car");

			var doc = ProcessAssets (assetsCar, GetFullSdkVersion ());
			Assert.IsNotNull (doc, "There was an issue processing the asset binary.");

			var foundAssets = FindAssets (doc);

			// Seems the 2 vectors are not being consumed in MacCatalyst but they still appear in the image Datasets
			var expectedAssets = platform == ApplePlatform.MacCatalyst ? ExpectedAssetsMacCatalyst : ExpectedAssets;
			Assert.AreEqual (expectedAssets, foundAssets, "Incorrect assets");

			var arm64txt = Path.Combine (resourcesDirectory, "arm64.txt");
			var armtxt = Path.Combine (resourcesDirectory, "arm.txt");
			var x64txt = Path.Combine (resourcesDirectory, "x64.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-arm64")), File.Exists (arm64txt), "arm64.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-arm")), File.Exists (armtxt), "arm.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-x64")), File.Exists (x64txt), "x64.txt");
		}

		void ConfigureAssets ()
		{
			Clean (projectPath);

			// We either want the assets added before the build, or we will be adding them after the build
			if (isStartingWithAssets)
				CopyAssets ();

			DotNet.AssertBuild (projectPath, GetDefaultProperties (runtimeIdentifiers));
			if (!isStartingWithAssets) {
				CopyAssets ();
				DotNet.AssertBuild (projectPath, GetDefaultProperties (runtimeIdentifiers));
			}
		}

		void DeleteAssets ()
		{
			var xcassetsDir = Path.Combine (projectPath, "../Assets.xcassets");
			File.Delete (xcassetsDir);
		}

		void CopyAssets ()
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

		string GetFullSdkVersion () => sdkVersion switch {
			"iphonesimulator" => sdkVersion + Configuration.sdk_version,
			"iphoneos" => sdkVersion + Configuration.sdk_version,
			"appletvsimulator" => sdkVersion + Configuration.tvos_sdk_version,
			"macosx" => sdkVersion + Configuration.macos_sdk_version,
			_ => throw new ArgumentOutOfRangeException (nameof (sdkVersion), $"Not expected sdkVersion: {sdkVersion}"),
		};

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

		JsonDocument ProcessAssets (string assetsPath, string sdkVersion)
		{
			var output = new StringBuilder ();
			var stderr = new StringBuilder ();
			var executable = "xcrun";
			var arguments = new string [] { "--sdk", sdkVersion, "assetutil", "--info", assetsPath };
			var rv = Execution.RunWithStringBuildersAsync (executable, arguments, standardOutput: output, standardError: stderr, timeout: TimeSpan.FromSeconds (120)).Result;
			Assert.AreEqual (0, rv.ExitCode, $"Processing Assets Error: {stderr}. Unexpected ExitCode");
			var s = output.ToString ();

			// This Execution call produces an output with an objc warning. We just want the json below it.
			if (s.StartsWith ("objc", StringComparison.Ordinal))
				s = s.Substring (s.IndexOf (Environment.NewLine) + 1);

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

		HashSet<string> FindAssets (JsonDocument doc)
		{
			var jsonArray = doc.RootElement.EnumerateArray ();
			var foundElements = new HashSet<string> ();

			foreach (var item in jsonArray) {
				var result = GetTarget (item);
				if (result is not null)
					foundElements.Add (result);
			}
			return foundElements;
		}

		string? GetTarget (JsonElement item)
		{
			if (item.TryGetProperty ("AssetType", out var assetType)) {
				foreach (var target in XCAssetTargets) {
					var result = GetTarget (item, assetType, target);
					if (result is not null)
						return result;
				}
			}
			return null;
		}

		string? GetTarget (JsonElement item, JsonElement assetType, XCAssetTarget target)
		{
			if (assetType.ToString () == target.AssetType && item.TryGetProperty (target.CategoryName, out var value)) {
				if (target.Values.Contains (value.ToString ()))
					return string.Concat (assetType.ToString (), ".", value.ToString ());
			}
			return null;
		}

		static readonly HashSet<string> ExpectedAssetsMacCatalyst = new HashSet<string> () {
			"Color.ColorTest",
			"Contents.SpritesTest",
			"Data.BmpImageDataTest",
			"Data.DngImageDataTest",
			"Data.EpsImageDataTest",
			"Data.JsonDataTest",
			"Data.TiffImageDataTest",
			"Image.samplejpeg.jpeg",
			"Image.samplejpg.jpg",
			"Image.samplepdf.pdf",
			"Image.samplepng2.png",
			"Image.spritejpeg.jpeg",
			"Image.xamlogo.svg",
			"Texture Rendition.TextureTest",
		};

		static readonly HashSet<string> ExpectedAssets = new HashSet<string> (ExpectedAssetsMacCatalyst) {
			"Vector.samplepdf.pdf",
			"Vector.xamlogo.svg",
		};

		class XCAssetTarget {
			public string AssetType { get; set; }
			public string CategoryName { get; set; }
			public string [] Values { get; set; }
			public XCAssetTarget (string assetType, string categoryName, string [] values)
			{
				AssetType = assetType;
				CategoryName = categoryName;
				Values = values;
			}
		}

		static XCAssetTarget [] XCAssetTargets = {
				new ("Image", "RenditionName", new string [] { "samplejpeg.jpeg", "samplejpg.jpg",
					"samplepdf.pdf", "samplepng2.png", "spritejpeg.jpeg", "xamlogo.svg" }),

				new ("Data", "Name", new string [] { "BmpImageDataTest", "JsonDataTest", "DS_StoreDataTest",
					"DngImageDataTest", "EpsImageDataTest", "TiffImageDataTest" }),

				new ("Color", "Name", new string [] { "ColorTest" }),

				new ("Contents", "Name", new string [] { "SpritesTest" }),

				new ("Texture Rendition", "Name", new string [] { "TextureTest" }),

				new ("Vector", "RenditionName", new string [] { "samplepdf.pdf", "xamlogo.svg" }),
		};
	}
}
