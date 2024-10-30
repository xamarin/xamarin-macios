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
			var appIconsManifestPath = actool.PartialAppManifest.ItemSpec!;
			var appIconsManifest = PDictionary.FromFile (appIconsManifestPath)!;
			Assert.AreEqual (0, appIconsManifest.Count, $"Partial plist contents: {actool.PartialAppManifest.ItemSpec}");
			var expectedXml =
				"""
				<?xml version="1.0" encoding="UTF-8"?>
				<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
				<plist version="1.0">
				<dict>
				</dict>
				</plist>
				""";
			PListAsserts.AreStringsEqual (expectedXml, File.ReadAllText (appIconsManifestPath), "Partial plist contents");
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

			var appIconsManifestPath = actool.PartialAppManifest.ItemSpec!;
			string expectedXml;
			if (platform == ApplePlatform.TVOS) {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIcons</key>
						<dict>
							<key>CFBundleAlternateIcons</key>
							<dict>
								<key>AlternateAppIcons</key>
								<dict>
									<key>CFBundleIconName</key>
									<string>AlternateAppIcons</string>
								</dict>
							</dict>
						</dict>
					</dict>
					</plist>
					""";
			} else {
				expectedXml = "";
			}
			PListAsserts.AreStringsEqual (expectedXml, File.ReadAllText (appIconsManifestPath), "Partial plist contents");
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

			var appIconsManifestPath = actool.PartialAppManifest?.ItemSpec!;
			string expectedXml;
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst) {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIconFile</key>
						<string>AlternateAppIcons</string>
						<key>CFBundleIconName</key>
						<string>AlternateAppIcons</string>
					</dict>
					</plist>
					""";
			} else if (platform == ApplePlatform.TVOS) {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIcons</key>
						<dict>
							<key>CFBundleAlternateIcons</key>
							<dict>
								<key>AlternateAppIcons</key>
								<dict>
									<key>CFBundleIconName</key>
									<string>AlternateAppIcons</string>
								</dict>
							</dict>
							<key>CFBundlePrimaryIcon</key>
							<string>AppIcon</string>
						</dict>
						<key>TVTopShelfImage</key>
						<dict>
							<key>TVTopShelfPrimaryImage</key>
							<string>TopShelfImage</string>
							<key>TVTopShelfPrimaryImageWide</key>
							<string>TopShelfImageWide</string>
						</dict>
					</dict>
					</plist>
					""";
			} else {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIcons</key>
						<dict>
							<key>CFBundleAlternateIcons</key>
							<dict>
								<key>AppIcons</key>
								<dict>
									<key>CFBundleIconFiles</key>
									<array>
										<string>AppIcons60x60</string>
										<string>AppIcons76x76</string>
									</array>
									<key>CFBundleIconName</key>
									<string>AppIcons</string>
								</dict>
							</dict>
							<key>CFBundlePrimaryIcon</key>
							<dict>
								<key>CFBundleIconFiles</key>
								<array>
									<string>AlternateAppIcons60x60</string>
								</array>
								<key>CFBundleIconName</key>
								<string>AlternateAppIcons</string>
							</dict>
						</dict>
						<key>CFBundleIcons~ipad</key>
						<dict>
							<key>CFBundleAlternateIcons</key>
							<dict>
								<key>AppIcons</key>
								<dict>
									<key>CFBundleIconFiles</key>
									<array>
										<string>AppIcons60x60</string>
										<string>AppIcons76x76</string>
									</array>
									<key>CFBundleIconName</key>
									<string>AppIcons</string>
								</dict>
							</dict>
							<key>CFBundlePrimaryIcon</key>
							<dict>
								<key>CFBundleIconFiles</key>
								<array>
									<string>AlternateAppIcons60x60</string>
									<string>AlternateAppIcons76x76</string>
								</array>
								<key>CFBundleIconName</key>
								<string>AlternateAppIcons</string>
							</dict>
						</dict>
					</dict>
					</plist>
					""";
			}
			PListAsserts.AreStringsEqual (expectedXml, File.ReadAllText (appIconsManifestPath), "Partial plist contents");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void AppIcon (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			actool.AppIcon = "AppIcons";

			ExecuteTask (actool);

			Assert.IsNotNull (actool.PartialAppManifest, "PartialAppManifest");

			var appIconsManifestPath = actool.PartialAppManifest.ItemSpec!;
			string expectedXml;
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst) {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIconFile</key>
						<string>AppIcons</string>
						<key>CFBundleIconName</key>
						<string>AppIcons</string>
					</dict>
					</plist>
					""";
			} else if (platform == ApplePlatform.TVOS) {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIcons</key>
						<dict>
							<key>CFBundlePrimaryIcon</key>
							<string>AppIcons</string>
						</dict>
						<key>TVTopShelfImage</key>
						<dict>
							<key>TVTopShelfPrimaryImage</key>
							<string>TopShelfImage</string>
							<key>TVTopShelfPrimaryImageWide</key>
							<string>TopShelfImageWide</string>
						</dict>
					</dict>
					</plist>
					""";
			} else {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIcons</key>
						<dict>
							<key>CFBundlePrimaryIcon</key>
							<dict>
								<key>CFBundleIconFiles</key>
								<array>
									<string>AppIcons60x60</string>
								</array>
								<key>CFBundleIconName</key>
								<string>AppIcons</string>
							</dict>
						</dict>
						<key>CFBundleIcons~ipad</key>
						<dict>
							<key>CFBundlePrimaryIcon</key>
							<dict>
								<key>CFBundleIconFiles</key>
								<array>
									<string>AppIcons60x60</string>
									<string>AppIcons76x76</string>
								</array>
								<key>CFBundleIconName</key>
								<string>AppIcons</string>
							</dict>
						</dict>
					</dict>
					</plist>
					""";
			}
			PListAsserts.AreStringsEqual (expectedXml, File.ReadAllText (appIconsManifestPath), "Partial plist contents");
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
				actool.AppIcon = "AppIcons";
				actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("AlternateAppIcons") };
			} else {
				actool.AppIcon = "AppIcons";
				actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("AlternateAppIcons") };
			}

			ExecuteTask (actool);

			Assert.IsNotNull (actool.PartialAppManifest, "PartialAppManifest");

			var appIconsManifestPath = actool.PartialAppManifest.ItemSpec!;
			string expectedXml;
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst) {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIconFile</key>
						<string>AppIcons</string>
						<key>CFBundleIconName</key>
						<string>AppIcons</string>
					</dict>
					</plist>
					""";
			} else if (platform == ApplePlatform.TVOS) {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIcons</key>
						<dict>
							<key>CFBundleAlternateIcons</key>
							<dict>
								<key>AlternateAppIcons</key>
								<dict>
									<key>CFBundleIconName</key>
									<string>AlternateAppIcons</string>
								</dict>
							</dict>
							<key>CFBundlePrimaryIcon</key>
							<string>AppIcons</string>
						</dict>
						<key>TVTopShelfImage</key>
						<dict>
							<key>TVTopShelfPrimaryImage</key>
							<string>TopShelfImage</string>
							<key>TVTopShelfPrimaryImageWide</key>
							<string>TopShelfImageWide</string>
						</dict>
					</dict>
					</plist>
					""";
			} else {
				expectedXml =
					"""
					<?xml version="1.0" encoding="UTF-8"?>
					<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
					<plist version="1.0">
					<dict>
						<key>CFBundleIcons</key>
						<dict>
							<key>CFBundleAlternateIcons</key>
							<dict>
								<key>AlternateAppIcons</key>
								<dict>
									<key>CFBundleIconFiles</key>
									<array>
										<string>AlternateAppIcons60x60</string>
										<string>AlternateAppIcons76x76</string>
									</array>
									<key>CFBundleIconName</key>
									<string>AlternateAppIcons</string>
								</dict>
							</dict>
							<key>CFBundlePrimaryIcon</key>
							<dict>
								<key>CFBundleIconFiles</key>
								<array>
									<string>AppIcons60x60</string>
								</array>
								<key>CFBundleIconName</key>
								<string>AppIcons</string>
							</dict>
						</dict>
						<key>CFBundleIcons~ipad</key>
						<dict>
							<key>CFBundleAlternateIcons</key>
							<dict>
								<key>AlternateAppIcons</key>
								<dict>
									<key>CFBundleIconFiles</key>
									<array>
										<string>AlternateAppIcons60x60</string>
										<string>AlternateAppIcons76x76</string>
									</array>
									<key>CFBundleIconName</key>
									<string>AlternateAppIcons</string>
								</dict>
							</dict>
							<key>CFBundlePrimaryIcon</key>
							<dict>
								<key>CFBundleIconFiles</key>
								<array>
									<string>AppIcons60x60</string>
									<string>AppIcons76x76</string>
								</array>
								<key>CFBundleIconName</key>
								<string>AppIcons</string>
							</dict>
						</dict>
					</dict>
					</plist>
					""";
			}
			PListAsserts.AreStringsEqual (expectedXml, File.ReadAllText (appIconsManifestPath), "Partial plist contents");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void AlternateIcons (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("AlternateAppIcons") };

			ExecuteTask (actool);

			string expectedXml;
			switch (platform) {
			case ApplePlatform.TVOS:
				expectedXml = """
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>CFBundleIcons</key>
	<dict>
		<key>CFBundleAlternateIcons</key>
		<dict>
			<key>AlternateAppIcons</key>
			<dict>
				<key>CFBundleIconName</key>
				<string>AlternateAppIcons</string>
			</dict>
		</dict>
	</dict>
</dict>
</plist>
""";
				break;
			case ApplePlatform.iOS:
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				expectedXml = "";
				break;
			default:
				throw new NotImplementedException (platform.ToString ());
			}

			var appIconsManifestPath = actool.PartialAppManifest.ItemSpec!;
			PListAsserts.AreStringsEqual (expectedXml, File.ReadAllText (appIconsManifestPath), "Partial plist contents");
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
			string expectedErrorMessage;
			switch (platform) {
			case ApplePlatform.TVOS:
				expectedErrorMessage = "Can't find the AppIcon 'InexistentAppIcons' among the image resources. There are 2 app icons in the image resources: AlternateBrandAssets, AppIcons.";
				break;
			case ApplePlatform.iOS:
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				expectedErrorMessage = "Can't find the AppIcon 'InexistentAppIcons' among the image resources. There are 2 app icons in the image resources: AlternateAppIcons, AppIcons.";
				break;
			default:
				throw new NotImplementedException (platform.ToString ());
			}
			Assert.AreEqual (expectedErrorMessage, Engine.Logger.ErrorEvents [0].Message, "Error message");
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
			string expectedErrorMessage;
			switch (platform) {
			case ApplePlatform.TVOS:
				expectedErrorMessage = "Can't find the AlternateAppIcon 'InexistentAlternateAppIcons' among the image resources. There are 5 app icons in the image resources: AlternateAppIcons, AppIcon, AppIcon-AppStore, AppIcons, AppIcons-AppStore.";
				break;
			case ApplePlatform.iOS:
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				expectedErrorMessage = "Can't find the AlternateAppIcon 'InexistentAlternateAppIcons' among the image resources. There are 2 app icons in the image resources: AlternateAppIcons, AppIcons.";
				break;
			default:
				throw new NotImplementedException (platform.ToString ());
			}
			Assert.AreEqual (expectedErrorMessage, Engine.Logger.ErrorEvents [0].Message, "Error message");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void BothAlternateAndMainIcon (ApplePlatform platform)
		{
			var actool = CreateACToolTaskWithResources (platform);
			actool.AlternateAppIcons = new ITaskItem [] { new TaskItem ("AppIcons") };
			actool.AppIcon = "AppIcons";

			ExecuteTask (actool, 1);
			Assert.AreEqual ($"The image resource '{actool.AppIcon}' is specified as both 'AppIcon' and 'AlternateAppIcon'.", Engine.Logger.ErrorEvents [0].Message, "Error message");
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
