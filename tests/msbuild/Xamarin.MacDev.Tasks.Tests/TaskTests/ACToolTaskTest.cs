using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class ACToolTaskTests : TestBase {
		ACTool CreateACToolTask (ApplePlatform platform, string projectDir, out string intermediateOutputPath, params string [] imageAssets)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			intermediateOutputPath = Cache.CreateTemporaryDirectory ();

			var sdk = Sdks.GetAppleSdk (platform);
			var version = AppleSdkVersion.UseDefault.ToString ();
			var root = sdk.GetSdkPath (version, false);
			var usr = Path.Combine (sdk.DeveloperRoot, "usr");
			var bin = Path.Combine (usr, "bin");
			string sdkPlatform;
			var uiDeviceFamily = "";

			switch (platform) {
			case ApplePlatform.TVOS:
				sdkPlatform = "AppleTVOS";
				uiDeviceFamily = "TV";
				break;
			case ApplePlatform.iOS:
				sdkPlatform = "iPhoneOS";
				uiDeviceFamily = "IPhone, IPad";
				break;
			case ApplePlatform.MacOSX:
				sdkPlatform = "MacOSX";
				break;
			case ApplePlatform.MacCatalyst:
				sdkPlatform = "MacCatalyst";
				break;
			default:
				throw new NotImplementedException (platform.ToString ());
			}

			var task = CreateTask<ACTool> ();
			task.ImageAssets = imageAssets
				.Select (v => {
					var spl = v.Split ('|');
					var rv = new TaskItem (spl [0]);
					rv.SetMetadata ("Link", spl [1]);
					return rv;
				})
				.Cast<ITaskItem> ()
				.ToArray ();
			task.IntermediateOutputPath = intermediateOutputPath;
			task.MinimumOSVersion = Xamarin.SdkVersions.GetMinVersion (platform).ToString ();
			task.OutputPath = Path.Combine (intermediateOutputPath, "OutputPath");
			task.ProjectDir = projectDir;
			task.SdkDevPath = Configuration.xcode_root;
			task.SdkPlatform = sdkPlatform;
			task.SdkVersion = version.ToString ();
			task.SdkUsrPath = usr;
			task.SdkBinPath = bin;
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform, true).ToString ();
			task.UIDeviceFamily = uiDeviceFamily;
			return task;
		}

		ACTool CreateACToolTaskWithResources (ApplePlatform platform)
		{
			var projectDir = Path.Combine (Configuration.SourceRoot, "tests", "dotnet", "AppWithXCAssets", platform.AsString ());
			var files = Directory.GetFiles (Path.Combine (projectDir, "Resources", "Images.xcassets"), "*", SearchOption.AllDirectories);
			var imageAssets = files.Select (v => v + "|" + v.Substring (projectDir.Length + 1)).ToArray ();
			return CreateACToolTask (
				platform,
				projectDir,
				out var _,
				imageAssets
			);
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void DefaultAppIcons (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			ExecuteTask (actool);

			Assert.IsNotNull (actool.PartialAppManifest, "PartialAppManifest");
			var appIconsManifest = PDictionary.FromFile (actool.PartialAppManifest.ItemSpec!)!;
			Assert.AreEqual (0, appIconsManifest.Count, $"Partial plist contents: {actool.PartialAppManifest.ItemSpec}");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void AllAppIcons (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			actool.IncludeAllAppIcons = true;

			ExecuteTask (actool);

			Assert.IsNotNull (actool.PartialAppManifest, "PartialAppManifest");

			var appIconsManifest = PDictionary.FromFile (actool.PartialAppManifest.ItemSpec!)!;
			Assert.AreEqual (0, appIconsManifest.Count, $"Partial plist contents: {actool.PartialAppManifest.ItemSpec}");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void AllAppIconsWithAppIcon (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			actool.IncludeAllAppIcons = true;
			if (platform == ApplePlatform.TVOS) {
				actool.AppIcon = "AlternateBrandAssets";
			} else {
				actool.AppIcon = "AlternateAppIcons";
			}

			ExecuteTask (actool);

			Assert.IsNotNull (actool.PartialAppManifest, "PartialAppManifest");

			var appIconsManifest = PDictionary.FromFile (actool.PartialAppManifest?.ItemSpec)!;
			Assert.AreEqual (2, appIconsManifest.Count, $"Partial plist contents: {actool.PartialAppManifest.ItemSpec}");
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst) {
				Assert.AreEqual ("AlternateAppIcons", appIconsManifest.Get<PString> ("CFBundleIconFile")?.Value, "CFBundleIconFile");
				Assert.AreEqual ("AlternateAppIcons", appIconsManifest.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");
			} else if (platform == ApplePlatform.TVOS) {
				var cfBundleIcons = appIconsManifest.Get<PDictionary> ("CFBundleIcons");
				Assert.AreEqual (1, cfBundleIcons.Count, "CFBundleIcons.Count");
				Assert.AreEqual ("AlternateAppIcons", cfBundleIcons.Get<PString> ("CFBundlePrimaryIcon")?.Value, "CFBundlePrimaryIcon");

				var tvTopShelfImage = appIconsManifest.Get<PDictionary> ("TVTopShelfImage");
				Assert.AreEqual (2, tvTopShelfImage.Count, "TVTopShelfImage.Count");
				Assert.AreEqual ("TopShelfImage", tvTopShelfImage.Get<PString> ("TVTopShelfPrimaryImage")?.Value, "TVTopShelfPrimaryImage");
				Assert.AreEqual ("TopShelfImageWide", tvTopShelfImage.Get<PString> ("TVTopShelfPrimaryImageWide")?.Value, "TVTopShelfPrimaryImageWide");
			} else {
				{
					// iPhone
					var cfBundleIcons = appIconsManifest.Get<PDictionary> ("CFBundleIcons");
					Assert.AreEqual (2, cfBundleIcons.Count, "CFBundleIcons.Count");

					var cfBundlePrimaryIcon = cfBundleIcons.Get<PDictionary> ("CFBundlePrimaryIcon");
					Assert.AreEqual (2, cfBundlePrimaryIcon.Count, "CFBundlePrimaryIcon.Length");

					var cfBundleIconFiles = cfBundlePrimaryIcon.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (1, cfBundleIconFiles.Count, "CFBundleIconFiles.Length");
					Assert.AreEqual ("AlternateAppIcons60x60", ((PString) cfBundleIconFiles [0]).Value, "CFBundleIconFiles[0].Value");
					Assert.AreEqual ("AlternateAppIcons", cfBundlePrimaryIcon.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");

					var cfBundleAlternateIcons = cfBundleIcons.Get<PDictionary> ("CFBundleAlternateIcons");
					Assert.AreEqual (1, cfBundleAlternateIcons.Count, "CFBundleAlternateIcons.Count");

					var alternateAppIcons = cfBundleAlternateIcons.Get<PDictionary> ("AppIcons");
					Assert.AreEqual (2, alternateAppIcons.Count, "AppIcons.Count");
					Assert.AreEqual ("AppIcons", alternateAppIcons.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");

					var appIcons_cfBundleIconFiles = alternateAppIcons.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (2, appIcons_cfBundleIconFiles.Count, "AppIcons.CFBundleIconFiles.Length");
					Assert.AreEqual ("AppIcons60x60", ((PString) appIcons_cfBundleIconFiles [0]).Value, "AppIcons.CFBundleIconFiles[0].Value");
					Assert.AreEqual ("AppIcons76x76", ((PString) appIcons_cfBundleIconFiles [1]).Value, "AppIcons.CFBundleIconFiles[1].Value");
				}
				{
					// iPad
					var cfBundleIcons = appIconsManifest.Get<PDictionary> ("CFBundleIcons~ipad");
					Assert.AreEqual (2, cfBundleIcons.Count, "CFBundleIcons.Count");

					var cfBundlePrimaryIcon = cfBundleIcons.Get<PDictionary> ("CFBundlePrimaryIcon");
					Assert.AreEqual (2, cfBundlePrimaryIcon.Count, "CFBundlePrimaryIcon.Length");

					var cfBundleIconFiles = cfBundlePrimaryIcon.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (2, cfBundleIconFiles.Count, "CFBundleIconFiles.Length");
					Assert.AreEqual ("AlternateAppIcons60x60", ((PString) cfBundleIconFiles [0]).Value, "CFBundleIconFiles[0].Value");
					Assert.AreEqual ("AlternateAppIcons76x76", ((PString) cfBundleIconFiles [1]).Value, "CFBundleIconFiles[1].Value");
					Assert.AreEqual ("AlternateAppIcons", cfBundlePrimaryIcon.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");

					var cfBundleAlternateIcons = cfBundleIcons.Get<PDictionary> ("CFBundleAlternateIcons");
					Assert.AreEqual (1, cfBundleAlternateIcons.Count, "CFBundleAlternateIcons.Count");

					var appIcons = cfBundleAlternateIcons.Get<PDictionary> ("AppIcons");
					Assert.AreEqual (2, appIcons.Count, "AppIcons.Count");
					Assert.AreEqual ("AppIcons", appIcons.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");

					var appIcons_cfBundleIconFiles = appIcons.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (2, appIcons_cfBundleIconFiles.Count, "AppIcons.CFBundleIconFiles.Length");
					Assert.AreEqual ("AppIcons60x60", ((PString) appIcons_cfBundleIconFiles [0]).Value, "AppIcons.CFBundleIconFiles[0].Value");
					Assert.AreEqual ("AppIcons76x76", ((PString) appIcons_cfBundleIconFiles [1]).Value, "AppIcons.CFBundleIconFiles[1].Value");
				}
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void AppIcon (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			if (platform == ApplePlatform.TVOS) {
				actool.AppIcon = "BrandAssets";
			} else {
				actool.AppIcon = "AppIcons";
			}

			ExecuteTask (actool);

			Assert.IsNotNull (actool.PartialAppManifest, "PartialAppManifest");

			var appIconsManifest = PDictionary.FromFile (actool.PartialAppManifest.ItemSpec!)!;
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst) {
				Assert.AreEqual (2, appIconsManifest.Count, $"Partial plist contents: {actool.PartialAppManifest.ItemSpec}");
				Assert.AreEqual ("AppIcons", appIconsManifest.Get<PString> ("CFBundleIconFile")?.Value, "CFBundleIconFile");
				Assert.AreEqual ("AppIcons", appIconsManifest.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");
			} else if (platform == ApplePlatform.TVOS) {
				Assert.AreEqual (2, appIconsManifest.Count, $"Partial plist contents: {actool.PartialAppManifest.ItemSpec}");

				var cfBundleIcons = appIconsManifest.Get<PDictionary> ("CFBundleIcons");
				Assert.AreEqual (1, cfBundleIcons.Count, "CFBundleIcons.Count");
				Assert.AreEqual ("AppIcons", cfBundleIcons.Get<PString> ("CFBundlePrimaryIcon")?.Value, "CFBundlePrimaryIcon");

				var tvTopShelfImage = appIconsManifest.Get<PDictionary> ("TVTopShelfImage");
				Assert.AreEqual (2, tvTopShelfImage.Count, "TVTopShelfImage.Count");
				Assert.AreEqual ("TopShelfImage", tvTopShelfImage.Get<PString> ("TVTopShelfPrimaryImage")?.Value, "TVTopShelfPrimaryImage");
				Assert.AreEqual ("TopShelfImageWide", tvTopShelfImage.Get<PString> ("TVTopShelfPrimaryImageWide")?.Value, "TVTopShelfPrimaryImageWide");
			} else {
				{
					// iPhone
					var cfBundleIcons = appIconsManifest.Get<PDictionary> ("CFBundleIcons");
					Assert.AreEqual (1, cfBundleIcons.Count, "CFBundleIcons.Count");

					var cfBundlePrimaryIcon = cfBundleIcons.Get<PDictionary> ("CFBundlePrimaryIcon");
					Assert.AreEqual (2, cfBundlePrimaryIcon.Count, "CFBundlePrimaryIcon.Length");

					var cfBundleIconFiles = cfBundlePrimaryIcon.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (1, cfBundleIconFiles.Count, "CFBundleIconFiles.Length");
					Assert.AreEqual ("AppIcons60x60", ((PString) cfBundleIconFiles [0]).Value, "CFBundleIconFiles[0].Value");
					Assert.AreEqual ("AppIcons", cfBundlePrimaryIcon.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");
				}
				{
					// iPad
					var cfBundleIcons = appIconsManifest.Get<PDictionary> ("CFBundleIcons~ipad");
					Assert.AreEqual (1, cfBundleIcons.Count, "CFBundleIcons.Count");

					var cfBundlePrimaryIcon = cfBundleIcons.Get<PDictionary> ("CFBundlePrimaryIcon");
					Assert.AreEqual (2, cfBundlePrimaryIcon.Count, "CFBundlePrimaryIcon.Length");

					var cfBundleIconFiles = cfBundlePrimaryIcon.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (2, cfBundleIconFiles.Count, "CFBundleIconFiles.Length");
					Assert.AreEqual ("AppIcons60x60", ((PString) cfBundleIconFiles [0]).Value, "CFBundleIconFiles[0].Value");
					Assert.AreEqual ("AppIcons76x76", ((PString) cfBundleIconFiles [1]).Value, "CFBundleIconFiles[1].Value");
					Assert.AreEqual ("AppIcons", cfBundlePrimaryIcon.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");
				}
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void AppIconAndAlternateIcons (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			if (platform == ApplePlatform.TVOS) {
				actool.AppIcon = "BrandAssets";
				actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("AlternateBrandAssets") };
			} else {
				actool.AppIcon = "AppIcons";
				actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("AlternateAppIcons") };
			}

			ExecuteTask (actool);

			Assert.IsNotNull (actool.PartialAppManifest, "PartialAppManifest");

			var appIconsManifest = PDictionary.FromFile (actool.PartialAppManifest.ItemSpec!)!;
			Assert.AreEqual (2, appIconsManifest.Count, $"Partial plist contents: {actool.PartialAppManifest.ItemSpec}");
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst) {
				Assert.AreEqual ("AppIcons", appIconsManifest.Get<PString> ("CFBundleIconFile")?.Value, "CFBundleIconFile");
				Assert.AreEqual ("AppIcons", appIconsManifest.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");
			} else if (platform == ApplePlatform.TVOS) {
				var cfBundleIcons = appIconsManifest.Get<PDictionary> ("CFBundleIcons");
				Assert.AreEqual (1, cfBundleIcons.Count, "CFBundleIcons.Count");
				Assert.AreEqual ("AppIcons", cfBundleIcons.Get<PString> ("CFBundlePrimaryIcon")?.Value, "CFBundlePrimaryIcon");

				var tvTopShelfImage = appIconsManifest.Get<PDictionary> ("TVTopShelfImage");
				Assert.AreEqual (2, tvTopShelfImage.Count, "TVTopShelfImage.Count");
				Assert.AreEqual ("TopShelfImage", tvTopShelfImage.Get<PString> ("TVTopShelfPrimaryImage")?.Value, "TVTopShelfPrimaryImage");
				Assert.AreEqual ("TopShelfImageWide", tvTopShelfImage.Get<PString> ("TVTopShelfPrimaryImageWide")?.Value, "TVTopShelfPrimaryImageWide");
			} else {
				{
					// iPhone
					var cfBundleIcons = appIconsManifest.Get<PDictionary> ("CFBundleIcons");
					Assert.AreEqual (2, cfBundleIcons.Count, "CFBundleIcons.Count");

					var cfBundlePrimaryIcon = cfBundleIcons.Get<PDictionary> ("CFBundlePrimaryIcon");
					Assert.AreEqual (2, cfBundlePrimaryIcon.Count, "CFBundlePrimaryIcon.Length");
					Assert.AreEqual ("AppIcons", cfBundlePrimaryIcon.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");

					var cfBundleIconFiles = cfBundlePrimaryIcon.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (1, cfBundleIconFiles.Count, "CFBundleIconFiles.Length");
					Assert.AreEqual ("AppIcons60x60", ((PString) cfBundleIconFiles [0]).Value, "CFBundleIconFiles[0].Value");

					var cfBundleAlternateIcons = cfBundleIcons.Get<PDictionary> ("CFBundleAlternateIcons");
					Assert.AreEqual (1, cfBundleAlternateIcons.Count, "CFBundleAlternateIcons.Count");

					var alternateAppIcons = cfBundleAlternateIcons.Get<PDictionary> ("AlternateAppIcons");
					Assert.AreEqual (2, alternateAppIcons.Count, "CFBundleAlternateIcons.Count");
					Assert.AreEqual ("AlternateAppIcons", alternateAppIcons.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");

					var alternateAppIcons_CFBundleIconFiles = alternateAppIcons.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (2, alternateAppIcons_CFBundleIconFiles.Count, "AlternateAppIcons.CFBundleIconFiles.Count");
					Assert.AreEqual ("AlternateAppIcons60x60", ((PString) alternateAppIcons_CFBundleIconFiles [0]).Value, "AlternateAppIcons.CFBundleIconFiles[0]");
					Assert.AreEqual ("AlternateAppIcons76x76", ((PString) alternateAppIcons_CFBundleIconFiles [1]).Value, "AlternateAppIcons.CFBundleIconFiles[1]");
				}
				{
					// iPad
					var cfBundleIcons = appIconsManifest.Get<PDictionary> ("CFBundleIcons~ipad");
					Assert.AreEqual (2, cfBundleIcons.Count, "CFBundleIcons.Count");

					var cfBundlePrimaryIcon = cfBundleIcons.Get<PDictionary> ("CFBundlePrimaryIcon");
					Assert.AreEqual (2, cfBundlePrimaryIcon.Count, "CFBundlePrimaryIcon.Length");
					Assert.AreEqual ("AppIcons", cfBundlePrimaryIcon.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");

					var cfBundleIconFiles = cfBundlePrimaryIcon.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (2, cfBundleIconFiles.Count, "CFBundleIconFiles.Length");
					Assert.AreEqual ("AppIcons60x60", ((PString) cfBundleIconFiles [0]).Value, "CFBundleIconFiles[0].Value");
					Assert.AreEqual ("AppIcons76x76", ((PString) cfBundleIconFiles [1]).Value, "CFBundleIconFiles[1].Value");

					var cfBundleAlternateIcons = cfBundleIcons.Get<PDictionary> ("CFBundleAlternateIcons");
					Assert.AreEqual (1, cfBundleAlternateIcons.Count, "CFBundleAlternateIcons.Count");

					var alternateAppIcons = cfBundleAlternateIcons.Get<PDictionary> ("AlternateAppIcons");
					Assert.AreEqual (2, alternateAppIcons.Count, "CFBundleAlternateIcons.Count");
					Assert.AreEqual ("AlternateAppIcons", alternateAppIcons.Get<PString> ("CFBundleIconName")?.Value, "CFBundleIconName");

					var alternateAppIcons_CFBundleIconFiles = alternateAppIcons.Get<PArray> ("CFBundleIconFiles");
					Assert.AreEqual (2, alternateAppIcons_CFBundleIconFiles.Count, "AlternateAppIcons.CFBundleIconFiles.Count");
					Assert.AreEqual ("AlternateAppIcons60x60", ((PString) alternateAppIcons_CFBundleIconFiles [0]).Value, "AlternateAppIcons.CFBundleIconFiles[0]");
					Assert.AreEqual ("AlternateAppIcons76x76", ((PString) alternateAppIcons_CFBundleIconFiles [1]).Value, "AlternateAppIcons.CFBundleIconFiles[1]");
				}
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void AlternateIcons (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			if (platform == ApplePlatform.TVOS) {
				actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("AlternateBrandAssets") };
			} else {
				actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("AlternateAppIcons") };
			}

			ExecuteTask (actool);

			var appIconsManifest = PDictionary.FromFile (actool.PartialAppManifest.ItemSpec!)!;
			Assert.AreEqual (0, appIconsManifest.Count, $"Partial plist contents: {actool.PartialAppManifest.ItemSpec}");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void InexistentAppIcon (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			actool.AppIcon = "InexistentAppIcons";

			ExecuteTask (actool, 1);
			Assert.AreEqual ("Can't find the AppIcon 'InexistentAppIcons' among the image resources.", Engine.Logger.ErrorEvents [0].Message, "Error message");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void InexistentAlternateIcons (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("InexistentAlternateAppIcons") };

			ExecuteTask (actool, 1);
			Assert.AreEqual ("Can't find the AlternateAppIcon 'InexistentAlternateAppIcons' among the image resources.", Engine.Logger.ErrorEvents [0].Message, "Error message");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void BothAlternateAndMainIcon (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			if (platform == ApplePlatform.TVOS) {
				actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("BrandAssets") };
				actool.AppIcon = "BrandAssets";
			} else {
				actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("AppIcons") };
				actool.AppIcon = "AppIcons";
			}

			ExecuteTask (actool, 1);
			Assert.AreEqual ($"The image resource '{actool.AppIcon}' is specified as both 'AppIcon' and 'AlternateAppIcon'", Engine.Logger.ErrorEvents [0].Message, "Error message");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void XSAppIconAssetsAndAppIcon (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			actool.AppIcon = "AppIcons";
			actool.XSAppIconAssets = "Resources/Images.xcassets/AppIcons.appiconset";

			ExecuteTask (actool, 1);
			Assert.AreEqual ("Can't specify both 'XSAppIconAssets' in the Info.plist and 'AppIcon' in the project file. Please select one or the other.", Engine.Logger.ErrorEvents [0].Message, "Error message");
		}
	}
}
