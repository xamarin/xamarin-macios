#nullable enable
using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using NUnit.Framework;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Utils;

namespace Xamarin.iOS.Tasks {
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
			task.SdkPlatform = PlatformFrameworkHelper.GetSdkPlatform (platform, false);
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

			task.AppManifest = mainPath;

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

			task.AppManifest = mainPath;
			task.PartialAppManifests = new [] { new TaskItem (partialPath) };

			ExecuteTask (task);

			var plist = PDictionary.FromFile (task.CompiledAppManifest.ItemSpec);
			Assert.AreEqual ("13.0", plist.GetMinimumOSVersion (), "MinimumOSVersion");
		}
	}
}
