using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class IBToolTaskTests
	{
		static IBTool CreateIBToolTask (PlatformFramework framework, string projectDir, string intermediateOutputPath)
		{
			var interfaceDefinitions = new List<ITaskItem> ();
			var sdk = IPhoneSdks.GetSdk (framework);
			var version = IPhoneSdkVersion.GetDefault (sdk, false);
			var root = sdk.GetSdkPath (version, false);
			var usr = Path.Combine (sdk.DeveloperRoot, "usr");
			var bin = Path.Combine (usr, "bin");
			string platform;

			switch (framework) {
			case PlatformFramework.WatchOS:
				platform = "WatchOS";
				break;
			case PlatformFramework.TVOS:
				platform = "AppleTVOS";
				break;
			default:
				platform = "iPhoneOS";
				break;
			}

			foreach (var item in Directory.EnumerateFiles (projectDir, "*.storyboard", SearchOption.AllDirectories))
				interfaceDefinitions.Add (new TaskItem (item));

			foreach (var item in Directory.EnumerateFiles (projectDir, "*.xib", SearchOption.AllDirectories))
				interfaceDefinitions.Add (new TaskItem (item));

			return new IBTool {
				AppManifest = new TaskItem (Path.Combine (projectDir, "Info.plist")),
				InterfaceDefinitions = interfaceDefinitions.ToArray (),
				IntermediateOutputPath = intermediateOutputPath,
				BuildEngine = new TestEngine (),
				ResourcePrefix = "Resources",
				ProjectDir = projectDir,
				SdkPlatform = platform,
				SdkVersion = version.ToString (),
				SdkUsrPath = usr,
				SdkBinPath = bin,
				SdkRoot = root,
			};
		}

		[Test]
		public void TestBasicIBToolFunctionality ()
		{
			var tmp = Path.GetTempPath ();

			Directory.CreateDirectory (tmp);

			try {
				var ibtool = CreateIBToolTask (PlatformFramework.iOS, "../MyIBToolLinkTest", tmp);
				var bundleResources = new HashSet<string> ();

				Assert.IsTrue (ibtool.Execute (), "Execution of IBTool task failed.");

				foreach (var bundleResource in ibtool.BundleResources) {
					Assert.IsTrue (File.Exists (bundleResource.ItemSpec), "File does not exist: {0}", bundleResource.ItemSpec);
					Assert.IsNotNullOrEmpty (bundleResource.GetMetadata ("LogicalName"), "The 'LogicalName' metadata must be set.");
					Assert.IsNotNullOrEmpty (bundleResource.GetMetadata ("Optimize"), "The 'Optimize' metadata must be set.");

					bundleResources.Add (bundleResource.GetMetadata ("LogicalName"));
				}

				string[] expected = { "LaunchScreen~ipad.nib/objects-8.0+.nib",
					"LaunchScreen~ipad.nib/runtime.nib",
					"LaunchScreen~iphone.nib/objects-8.0+.nib",
					"LaunchScreen~iphone.nib/runtime.nib",
					"Main.storyboardc/BYZ-38-t0r-view-8bC-Xf-vdC~ipad.nib/objects-8.0+.nib",
					"Main.storyboardc/BYZ-38-t0r-view-8bC-Xf-vdC~ipad.nib/runtime.nib",
					"Main.storyboardc/BYZ-38-t0r-view-8bC-Xf-vdC~iphone.nib/objects-8.0+.nib",
					"Main.storyboardc/BYZ-38-t0r-view-8bC-Xf-vdC~iphone.nib/runtime.nib",
					"Main.storyboardc/UIViewController-BYZ-38-t0r~ipad.nib/objects-8.0+.nib",
					"Main.storyboardc/UIViewController-BYZ-38-t0r~ipad.nib/runtime.nib",
					"Main.storyboardc/UIViewController-BYZ-38-t0r~iphone.nib/objects-8.0+.nib",
					"Main.storyboardc/UIViewController-BYZ-38-t0r~iphone.nib/runtime.nib",
					"Main~ipad.storyboardc/Info-8.0+.plist",
					"Main~ipad.storyboardc/Info.plist",
					"Main~iphone.storyboardc/Info-8.0+.plist",
					"Main~iphone.storyboardc/Info.plist"
				};

				foreach (var bundleResource in expected)
					Assert.IsTrue (bundleResources.Contains (bundleResource), "BundleResources should include '{0}'", bundleResource);

				Assert.AreEqual (expected.Length, bundleResources.Count, "Unexpected number of BundleResources");
			} finally {
				Directory.Delete (tmp, true);
			}
		}
	}
}
