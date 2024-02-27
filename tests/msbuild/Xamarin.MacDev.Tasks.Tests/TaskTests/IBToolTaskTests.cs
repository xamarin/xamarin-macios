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
	public class IBToolTaskTests : TestBase {
		IBTool CreateIBToolTask (ApplePlatform framework, string projectDir, string intermediateOutputPath)
		{
			var interfaceDefinitions = new List<ITaskItem> ();
			var sdk = Sdks.GetSdk (framework);
			var version = AppleSdkVersion.GetDefault (sdk, false);
			var root = sdk.GetSdkPath (version, false);
			var usr = Path.Combine (sdk.DeveloperRoot, "usr");
			var bin = Path.Combine (usr, "bin");
			string platform;

			switch (framework) {
			case ApplePlatform.WatchOS:
				platform = "WatchOS";
				break;
			case ApplePlatform.TVOS:
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

			var task = CreateTask<IBTool> ();
			task.InterfaceDefinitions = interfaceDefinitions.ToArray ();
			task.IntermediateOutputPath = intermediateOutputPath;
			task.MinimumOSVersion = PDictionary.FromFile (Path.Combine (projectDir, "Info.plist")).GetMinimumOSVersion ();
			task.ResourcePrefix = "Resources";
			task.ProjectDir = projectDir;
			task.SdkDevPath = Configuration.xcode_root;
			task.SdkPlatform = platform;
			task.SdkVersion = version.ToString ();
			task.SdkUsrPath = usr;
			task.SdkBinPath = bin;
			task.SdkRoot = root;
			task.TargetFrameworkMoniker = "Xamarin.iOS,v1.0";
			return task;
		}

		[Test]
		public void TestBasicIBToolFunctionality ()
		{
			var tmp = Cache.CreateTemporaryDirectory ("basic-ibtool");

			var srcdir = Path.Combine (Configuration.TestProjectsDirectory, "MyIBToolLinkTest");
			var ibtool = CreateIBToolTask (ApplePlatform.iOS, srcdir, tmp);
			var bundleResources = new HashSet<string> ();

			Assert.IsTrue (ibtool.Execute (), "Execution of IBTool task failed.");

			foreach (var bundleResource in ibtool.BundleResources) {
				Assert.IsTrue (File.Exists (bundleResource.ItemSpec), "File does not exist: {0}", bundleResource.ItemSpec);
				Assert.That (bundleResource.GetMetadata ("LogicalName"), Is.Not.Null.Or.Empty, "The 'LogicalName' metadata must be set.");
				Assert.That (bundleResource.GetMetadata ("Optimize"), Is.Not.Null.Or.Empty, "The 'Optimize' metadata must be set.");

				bundleResources.Add (bundleResource.GetMetadata ("LogicalName"));
			}

			string [] expected = {
				"LaunchScreen.nib",
				"Main.storyboardc/UIViewController-BYZ-38-t0r.nib",
				"Main.storyboardc/BYZ-38-t0r-view-8bC-Xf-vdC.nib",
				"Main.storyboardc/Info.plist",
			};

			var inexistentResource = bundleResources.Except (expected).ToArray ();
			var unexpectedResource = expected.Except (bundleResources).ToArray ();

			Assert.That (inexistentResource, Is.Empty, "No missing resources");
			Assert.That (unexpectedResource, Is.Empty, "No extra resources");
		}

		[Test]
		public void TestAdvancedIBToolFunctionality ()
		{
			var tmp = Cache.CreateTemporaryDirectory ("advanced-ibtool");
			IBTool ibtool;

			var srcdir = Path.Combine (Configuration.TestProjectsDirectory, "IBToolTaskTests", "LinkedAndTranslated");
			ibtool = CreateIBToolTask (ApplePlatform.iOS, srcdir, tmp);
			var bundleResources = new HashSet<string> ();

			// Add some ResourceTags...
			foreach (var storyboard in ibtool.InterfaceDefinitions) {
				var tag = Path.GetFileNameWithoutExtension (storyboard.ItemSpec);
				storyboard.SetMetadata ("ResourceTags", tag);
			}

			ibtool.EnableOnDemandResources = true;

			Assert.IsTrue (ibtool.Execute (), "Execution of IBTool task failed.");

			foreach (var bundleResource in ibtool.BundleResources) {
				var bundleName = bundleResource.GetMetadata ("LogicalName");
				var tag = bundleResource.GetMetadata ("ResourceTags");

				Assert.IsTrue (File.Exists (bundleResource.ItemSpec), "File does not exist: {0}", bundleResource.ItemSpec);
				Assert.That (bundleResource.GetMetadata ("LogicalName"), Is.Not.Null.Or.Empty, "The 'LogicalName' metadata must be set.");
				Assert.That (bundleResource.GetMetadata ("Optimize"), Is.Not.Null.Or.Empty, "The 'Optimize' metadata must be set.");

				Assert.That (tag, Is.Not.Null.Or.Empty, "The 'ResourceTags' metadata should be set.");
				Assert.IsTrue (bundleName.Contains (".lproj/" + tag + ".storyboardc/"), "BundleResource does not have the proper ResourceTags set: {0}", bundleName);

				bundleResources.Add (bundleName);
			}

			ibtool.EnableOnDemandResources = true;

			string [] expected = {
					"en.lproj/Main.storyboardc/UIViewController-BYZ-38-t0r.nib",
					"en.lproj/Main.storyboardc/BYZ-38-t0r-view-8bC-Xf-vdC.nib",
					"en.lproj/Main.storyboardc/Info.plist",
					"en.lproj/Main.storyboardc/MyLinkedViewController.nib",
					"en.lproj/Linked.storyboardc/5xv-Yx-H4r-view-gMo-tm-chA.nib",
					"en.lproj/Linked.storyboardc/Info.plist",
					"en.lproj/Linked.storyboardc/MyLinkedViewController.nib",
					"Base.lproj/Main.storyboardc/UIViewController-BYZ-38-t0r.nib",
					"Base.lproj/Main.storyboardc/BYZ-38-t0r-view-8bC-Xf-vdC.nib",
					"Base.lproj/Main.storyboardc/Info.plist",
					"Base.lproj/Main.storyboardc/MyLinkedViewController.nib",
					"Base.lproj/Linked.storyboardc/5xv-Yx-H4r-view-gMo-tm-chA.nib",
					"Base.lproj/Linked.storyboardc/Info.plist",
					"Base.lproj/Linked.storyboardc/MyLinkedViewController.nib",
					"Base.lproj/LaunchScreen.storyboardc/01J-lp-oVM-view-Ze5-6b-2t3.nib",
					"Base.lproj/LaunchScreen.storyboardc/UIViewController-01J-lp-oVM.nib",
					"Base.lproj/LaunchScreen.storyboardc/Info.plist",
				};

			var inexistentResource = bundleResources.Except (expected).ToArray ();
			var unexpectedResource = expected.Except (bundleResources).ToArray ();

			Assert.That (inexistentResource, Is.Empty, "No missing resources");
			Assert.That (unexpectedResource, Is.Empty, "No extra resources");
		}

		IBTool CreateIBToolTask (ApplePlatform framework, string projectDir, string intermediateOutputPath, params string [] fileNames)
		{
			var ibtool = CreateIBToolTask (framework, projectDir, intermediateOutputPath);
			var interfaceDefinitions = new List<ITaskItem> ();

			foreach (var name in fileNames)
				interfaceDefinitions.Add (new TaskItem (Path.Combine (projectDir, name)));

			ibtool.InterfaceDefinitions = interfaceDefinitions.ToArray ();

			return ibtool;
		}

		void TestGenericAndDeviceSpecificXibsGeneric (params string [] fileNames)
		{
			var tmp = Cache.CreateTemporaryDirectory ("advanced-ibtool");
			IBTool ibtool;

			var srcdir = Path.Combine (Configuration.TestProjectsDirectory, "IBToolTaskTests", "GenericAndDeviceSpecific");
			ibtool = CreateIBToolTask (ApplePlatform.iOS, srcdir, tmp, fileNames);
			var bundleResources = new HashSet<string> ();

			// Add some ResourceTags...
			foreach (var storyboard in ibtool.InterfaceDefinitions) {
				var tag = Path.GetFileNameWithoutExtension (storyboard.ItemSpec);
				storyboard.SetMetadata ("ResourceTags", tag);
			}

			ibtool.EnableOnDemandResources = true;

			Assert.IsTrue (ibtool.Execute (), "Execution of IBTool task failed.");

			foreach (var bundleResource in ibtool.BundleResources) {
				var bundleName = bundleResource.GetMetadata ("LogicalName");
				var tag = bundleResource.GetMetadata ("ResourceTags");

				Assert.IsTrue (File.Exists (bundleResource.ItemSpec), "File does not exist: {0}", bundleResource.ItemSpec);
				Assert.That (bundleResource.GetMetadata ("LogicalName"), Is.Not.Null.Or.Empty, "The 'LogicalName' metadata must be set.");
				Assert.That (bundleResource.GetMetadata ("Optimize"), Is.Not.Null.Or.Empty, "The 'Optimize' metadata must be set.");

				Assert.That (tag, Is.Not.Null.Or.Empty, "The 'ResourceTags' metadata should be set.");
				Assert.AreEqual (Path.Combine (tmp, "ibtool", tag + ".nib"), bundleResource.ItemSpec, $"BundleResource {bundleName} is not at the expected location.");

				bundleResources.Add (bundleName);
			}

			string [] expected = {
					"View.nib",
					"View~ipad.nib",
				};

			var inexistentResource = bundleResources.Except (expected).ToArray ();
			var unexpectedResource = expected.Except (bundleResources).ToArray ();

			Assert.That (inexistentResource, Is.Empty, "No missing resources");
			Assert.That (unexpectedResource, Is.Empty, "No extra resources");
		}

		[Test]
		public void TestGenericAndDeviceSpecificXibsGenericFirst ()
		{
			TestGenericAndDeviceSpecificXibsGeneric ("View.xib", "View~ipad.xib");
		}

		[Test]
		public void TestGenericAndDeviceSpecificXibsGenericLast ()
		{
			TestGenericAndDeviceSpecificXibsGeneric ("View~ipad.xib", "View.xib");
		}
	}
}
