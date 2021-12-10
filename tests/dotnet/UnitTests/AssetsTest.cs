using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Text.Json;

using Mono.Cecil;
using NUnit.Framework;
using Xamarin.Utils;
using Xamarin.Tests;
using Xamarin.MacDev;

#nullable enable

namespace Xamarin.Tests {

	[TestFixture (ApplePlatform.iOS, "iossimulator-x64", "iphonesimulator")]
	[TestFixture (ApplePlatform.iOS, "ios-arm64;ios-arm", "iphoneos")]
	[TestFixture (ApplePlatform.TVOS, "tvossimulator-x64", "appletvsimulator")]
	[TestFixture (ApplePlatform.MacCatalyst, "maccatalyst-x64", "macosx")]
	[TestFixture (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", "macosx")]
	[TestFixture (ApplePlatform.MacOSX, "osx-x64", "macosx")]
	[TestFixture (ApplePlatform.MacOSX, "osx-arm64;osx-x64", "macosx")] // https://github.com/xamarin/xamarin-macios/issues/12410
	public class AssetsTest : TestBaseClass {

		readonly ApplePlatform platform;
		readonly string runtimeIdentifiers;
		readonly string sdkVersion;
		string project_path = string.Empty;
		string appPath = string.Empty;

		public AssetsTest (ApplePlatform platform, string runtimeIdentifiers, string sdkVersion)
		{
			this.platform = platform;
			this.runtimeIdentifiers = runtimeIdentifiers;
			this.sdkVersion = sdkVersion;
		}

		[SetUp]
		public void Init ()
		{
			var project = "AppWithXCAssets";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out appPath);
			DeleteAssets (project_path);
		}

		[TestCase (true)] // Add the XCAssets before the build
		[TestCase (false)] // Build, add the XCAssets, then build again
		public void TestXCAssets (bool startWithAssets)
		{
			var project = "AppWithXCAssets";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);

			ConfigureAssets (project_path, runtimeIdentifiers, startWithAssets);

			var appExecutable = GetNativeExecutable (platform, appPath);
			DotNetProjectTest.ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			var resourcesDirectory = GetResourcesDirectory (platform, appPath);

			var assetsCar = Path.Combine (resourcesDirectory, "Assets.car");
			Assert.That (assetsCar, Does.Exist, "Assets.car");

			var doc = ProcessAssets (assetsCar, GetFullSdkVersion (sdkVersion));
			Assert.IsNotNull (doc, "There was an issue processing the asset binary.");

			var foundAssets = FindAssets (doc);

			// Seems the 2 vectors are not being consumed in MacCatalyst but they still appear in the image Datasets
			var TotalUniqueAssets = platform == ApplePlatform.MacCatalyst ? 14 : 16;

			Assert.AreEqual (TotalUniqueAssets, foundAssets.Count, "Wrong number of assets found");
			Assert.IsFalse (foundAssets.Contains ("Data.DS_StoreDataTest"), "DS_Store files should not be included.");

			var arm64txt = Path.Combine (resourcesDirectory, "arm64.txt");
			var armtxt = Path.Combine (resourcesDirectory, "arm.txt");
			var x64txt = Path.Combine (resourcesDirectory, "x64.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-arm64")), File.Exists (arm64txt), "arm64.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-arm")), File.Exists (armtxt), "arm.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-x64")), File.Exists (x64txt), "x64.txt");
		}

		void ConfigureAssets (string project_path, string runtimeIdentifiers, bool startWithAssets)
		{
			// We either want the assets added before the build, or we will be adding them after the build
			if (startWithAssets)
				CopyAssets (project_path);

			Clean (project_path);

			DotNet.AssertBuild (project_path, GetDefaultProperties (runtimeIdentifiers));
			if (!startWithAssets) {
				CopyAssets (project_path);
				// Building the project twice without cleaning in between fails: https://github.com/xamarin/maccore/issues/2530
				Clean (project_path);
				DotNet.AssertBuild (project_path, GetDefaultProperties (runtimeIdentifiers));
			}
		}

		void DeleteAssets (string project_path)
		{
			var xcassetsDir = Path.Combine (project_path, "../Assets.xcassets");
			File.Delete (xcassetsDir);
		}

		void CopyAssets (string project_path)
		{
			var testingAssetsDir = new DirectoryInfo (Path.Combine (project_path, "../../TestingAssets"));
			var xcassetsDir = new DirectoryInfo (Path.Combine (project_path, "../Assets.xcassets"));

			Assert.That (testingAssetsDir, Does.Exist, $"Could not find testingAssetsDir: {testingAssetsDir}");
			MakeSymlinks (testingAssetsDir.FullName, xcassetsDir.FullName);
			Assert.That (xcassetsDir, Does.Exist, $"Could not find xcassetsDir: {xcassetsDir}");
		}

		void MakeSymlinks (string sourceDir, string destDir)
		{
			var output = new StringBuilder ();
			var executable = "ln";
			var arguments = new string [] { "-s", sourceDir, destDir };
			var rv = Execution.RunWithStringBuildersAsync (executable, arguments, standardOutput: output, standardError: output, timeout: TimeSpan.FromSeconds (60)).Result;
			Assert.AreEqual (0, rv.ExitCode, $"Creating Symlink Error: {rv.StandardError}. Unexpected ExitCode");
			return;
		}

		string GetFullSdkVersion (string sdkVersion) => sdkVersion switch {
			"iphonesimulator" => sdkVersion + Configuration.sdk_version,
			"iphoneos" => sdkVersion + Configuration.sdk_version,
			"appletvsimulator" => sdkVersion + Configuration.tvos_sdk_version,
			"macosx" => sdkVersion + Configuration.macos_sdk_version,
			_ => throw new ArgumentOutOfRangeException (nameof (sdkVersion), $"Not expected sdkVersion: {sdkVersion}"),
		};

		JsonDocument ProcessAssets (string assetsPath, string sdkVersion)
		{
			var output = new StringBuilder ();
			var executable = "xcrun";
			var arguments = new string [] { "--sdk", sdkVersion, "assetutil", "--info", assetsPath };
			var rv = Execution.RunWithStringBuildersAsync (executable, arguments, standardOutput: output, standardError: output, timeout: TimeSpan.FromSeconds (120)).Result;
			Assert.AreEqual (0, rv.ExitCode, $"Processing Assets Error: {rv.StandardError}. Unexpected ExitCode");
			var s = output.ToString ();

			// This Execution call produces an output with an objc warning. We just want the json below it.
			if (s.StartsWith ("objc"))
				output.Remove (0, s.IndexOf (Environment.NewLine) + 1);

			return JsonDocument.Parse (output.ToString ());
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

