#nullable enable
using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using NUnit.Framework;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class CompileAppManifestTaskTests : TestBase {
		CompileAppManifest CreateTask (string? tmpdir = null, ApplePlatform platform = ApplePlatform.iOS)
		{
			if (string.IsNullOrEmpty (tmpdir))
				tmpdir = Cache.CreateTemporaryDirectory ();

			var task = CreateTask<CompileAppManifest> ();
			task.AssemblyName = "AssemblyName";
			task.AppBundleName = "AppBundleName";
			task.CompiledAppManifest = new TaskItem (Path.Combine (tmpdir, "TemporaryAppManifest.plist"));
			task.DefaultSdkVersion = Sdks.GetAppleSdk (platform).GetInstalledSdkVersions (false).First ().ToString ();
			task.SdkVersion = task.DefaultSdkVersion;
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform, true).ToString ();

			return task;
		}

		[Test]
		public void DefaultMinimumOSVersion ()
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir);

			ExecuteTask (task);

			var plist = PDictionary.FromFile (task.CompiledAppManifest.ItemSpec);
			Assert.AreEqual (task.SdkVersion, plist.GetMinimumOSVersion (), "MinimumOSVersion");
		}

		[Test]
		public void MainMinimumOSVersions ()
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir);

			var mainPath = Path.Combine (dir, "Info.plist");
			var main = new PDictionary ();
			main.SetMinimumOSVersion ("14.0");
			main.Save (mainPath);

			task.AppManifest = new TaskItem (mainPath);

			ExecuteTask (task);

			var plist = PDictionary.FromFile (task.CompiledAppManifest.ItemSpec);
			Assert.AreEqual ("14.0", plist.GetMinimumOSVersion (), "MinimumOSVersion");
		}

		[Test]
		public void MultipleMinimumOSVersions ()
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir);

			var mainPath = Path.Combine (dir, "Info.plist");
			var main = new PDictionary ();
			main.SetMinimumOSVersion ("14.0");
			main.Save (mainPath);

			// The version in the partial app manifest takes precedence.
			var partialPath = Path.Combine (dir, "PartialAppManifest.plist");
			var partial = new PDictionary ();
			partial.SetMinimumOSVersion ("13.0");
			partial.Save (partialPath);

			task.AppManifest = new TaskItem (mainPath);
			task.PartialAppManifests = new [] { new TaskItem (partialPath) };

			ExecuteTask (task);

			var plist = PDictionary.FromFile (task.CompiledAppManifest.ItemSpec);
			Assert.AreEqual ("13.0", plist.GetMinimumOSVersion (), "MinimumOSVersion");
		}

		[Test]
		public void ErrorWithMismatchedInfoPlistMinimumOSVersion ()
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir);

			var plist = new PDictionary ();
			plist.SetMinimumOSVersion ("10.0");
			var manifest = Path.Combine (dir, "Info.plist");
			plist.Save (manifest);
			task.AppManifest = new TaskItem (manifest);
			task.SupportedOSPlatformVersion = "11.0";

			ExecuteTask (task, expectedErrorCount: 1);
			Assert.AreEqual ("The MinimumOSVersion value in the Info.plist (10.0) does not match the SupportedOSPlatformVersion value (11.0) in the project file (if there is no SupportedOSPlatformVersion value in the project file, then a default value has been assumed). Either change the value in the Info.plist to match the SupportedOSPlatformVersion value, or remove the value in the Info.plist (and add a SupportedOSPlatformVersion value to the project file if it doesn't already exist).", Engine.Logger.ErrorEvents [0].Message);
		}

		[Test]
		public void SupportedOSPlatformVersion ()
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir);

			task.SupportedOSPlatformVersion = "11.0";

			ExecuteTask (task);

			var plist = PDictionary.FromFile (task.CompiledAppManifest.ItemSpec);
			Assert.AreEqual ("11.0", plist.GetMinimumOSVersion (), "MinimumOSVersion");
		}

		[Test]
		public void MacCatalystVersionCheck ()
		{
			var task = CreateTask (platform: ApplePlatform.MacCatalyst);
			task.SupportedOSPlatformVersion = "14.2";
			ExecuteTask (task);

			var plist = PDictionary.FromFile (task.CompiledAppManifest.ItemSpec);
			Assert.AreEqual ("11.0", plist.GetMinimumSystemVersion (), "MinimumOSVersion");
		}

		[Test]
		public void MacCatalystVersionCheckUnmappedError ()
		{
			var task = CreateTask (platform: ApplePlatform.MacCatalyst);
			task.SupportedOSPlatformVersion = "10.0";

			ExecuteTask (task, expectedErrorCount: 1);
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.StartWith ("Could not map the Mac Catalyst version 10.0 to a corresponding macOS version. Valid Mac Catalyst versions are:"));
		}

		[Test]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.MacCatalyst, false)]
		[TestCase (ApplePlatform.TVOS, true)]
		[TestCase (ApplePlatform.TVOS, false)]
		[TestCase (ApplePlatform.MacOSX, false)]
		public void XcodeVariables (ApplePlatform platform, bool isSimulator)
		{
			var task = CreateTask (platform: platform);
			task.SdkIsSimulator = isSimulator;
			ExecuteTask (task);

			var plist = PDictionary.FromFile (task.CompiledAppManifest.ItemSpec);
			var variables = new string [] {
				"DTCompiler",
				"DTPlatformBuild",
				"DTPlatformName",
				"DTPlatformVersion",
				"DTSDKBuild",
				"DTSDKName",
				"DTXcode",
				"DTXcodeBuild",
			};
			foreach (var variable in variables) {
				var value = plist.GetString (variable)?.Value;
				Assert.That (value, Is.Not.Null.And.Not.Empty, variable);
			}
		}
	}
}
