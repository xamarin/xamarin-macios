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
			task.AppBundleName = "AppBundleName";
			task.CompiledAppManifest = new TaskItem (Path.Combine (tmpdir, "TemporaryAppManifest.plist"));
			task.MinSupportedOSPlatformVersion = "10.0";
			task.SupportedOSPlatformVersion = "15.0";
			task.SdkVersion = task.DefaultSdkVersion ?? string.Empty;
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform).ToString ();
			task.DefaultSdkVersion = task.CurrentSdk.GetInstalledSdkVersions (false).First ().ToString ()!;

			return task;
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "26.9", true, false, false)]
		[TestCase (ApplePlatform.iOS, "27.0", true, false, true)]
		[TestCase (ApplePlatform.iOS, "27.1", true, false, true)]
		[TestCase (ApplePlatform.MacCatalyst, "26.9", true, false, false)]
		[TestCase (ApplePlatform.MacCatalyst, "27.0", true, false, true)]
		[TestCase (ApplePlatform.MacCatalyst, "28.0", true, false, true)]
		[TestCase (ApplePlatform.TVOS, "27.0", true, false, false)]
		[TestCase (ApplePlatform.MacOSX, "27.0", true, false, false)]
		[TestCase (ApplePlatform.iOS, "27.0", false, false, false)]
		[TestCase (ApplePlatform.MacCatalyst, "27.0", false, false, false)]
		[TestCase (ApplePlatform.iOS, "27.0", true, true, false)]
		[TestCase (ApplePlatform.MacCatalyst, "27.0", true, true, false)]
		public void SceneLifecycleValidation (ApplePlatform platform, string sdkVersion, bool validateSceneManifest, bool isAppExtension, bool expectError)
		{
			var task = CreateTask<SceneLifecycleValidationTask> ();
			Assert.That (task.ValidateSceneManifest, Is.True, "Default");
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform).ToString ();
			task.ValidateSceneManifest = validateSceneManifest;
			task.IsAppExtension = isAppExtension;
			task.AppManifest = new TaskItem ("Info.plist");

			task.ValidateManifest (new PDictionary (), sdkVersion);

			Assert.That (Engine.Logger.ErrorEvents.Count, Is.EqualTo (expectError ? 1 : 0), "Errors");
			if (expectError) {
				var error = Engine.Logger.ErrorEvents [0];
				Assert.That (error.Code, Is.EqualTo ("MT7199"), "Code");
				Assert.That (error.File, Is.EqualTo ("Info.plist"), "File");
				Assert.That (error.Message, Does.Contain ("https://aka.ms/uiscene"), "Migration instructions");
				Assert.That (error.Message, Does.Contain ("ValidateSceneManifest to false"), "Opt-out");
				Assert.That (error.Message, Does.Contain ("does not disable the operating system's requirement"), "OS requirement");
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.MacCatalyst, false)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		public void SceneManifestDictionary (ApplePlatform platform, bool invalidManifest)
		{
			var task = CreateTask<SceneLifecycleValidationTask> ();
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform).ToString ();
			var plist = new PDictionary ();
			if (invalidManifest)
				plist.Add ("UIApplicationSceneManifest", new PString ("invalid"));
			else
				plist.Add ("UIApplicationSceneManifest", new PDictionary ());

			task.ValidateManifest (plist, "27.0");

			Assert.That (Engine.Logger.ErrorEvents.Count, Is.EqualTo (invalidManifest ? 1 : 0), "Errors");
		}

		public class SceneLifecycleValidationTask : CompileAppManifest {
			public void ValidateManifest (PDictionary plist, string sdkVersion)
			{
				ValidateSceneLifecycle (plist, AppleSdkVersion.Parse (sdkVersion));
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS, true, true)]
		[TestCase (ApplePlatform.iOS, true, false)]
		[TestCase (ApplePlatform.iOS, false, true)]
		[TestCase (ApplePlatform.MacCatalyst, true, true)]
		[TestCase (ApplePlatform.MacCatalyst, true, false)]
		[TestCase (ApplePlatform.MacCatalyst, false, true)]
		public void MissingSceneManifest (ApplePlatform platform, bool validate, bool validateSceneManifest)
		{
			var task = CreateTask (platform: platform);
			task.Validate = validate;
			task.ValidateSceneManifest = validateSceneManifest;
			var expectError = validate && validateSceneManifest && AppleSdkVersion.Parse (task.DefaultSdkVersion) >= new AppleSdkVersion (27, 0);

			ExecuteTask (task, expectedErrorCount: expectError ? 1 : 0);

			if (expectError) {
				Assert.That (Engine.Logger.ErrorEvents [0].Code, Is.EqualTo ("MT7199"), "Code");
				var compiledManifest = task.CompiledAppManifest ?? throw new InvalidOperationException ("Missing compiled app manifest.");
				Assert.That (File.Exists (compiledManifest.ItemSpec), Is.False, "Failed validation must not produce an up-to-date manifest");
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		public void SceneManifestFromPartialManifest (ApplePlatform platform)
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir, platform);
			task.Validate = true;
			var partialPath = Path.Combine (dir, "SceneManifest.plist");
			var partial = new PDictionary {
				{ "UIApplicationSceneManifest", new PDictionary () },
			};
			partial.Save (partialPath);
			task.PartialAppManifests = [new TaskItem (partialPath)];

			ExecuteTask (task);

			var compiledManifest = task.CompiledAppManifest ?? throw new InvalidOperationException ("Missing compiled app manifest.");
			var plist = PDictionary.OpenFile (compiledManifest.ItemSpec);
			Assert.That (plist.Get<PDictionary> ("UIApplicationSceneManifest"), Is.Not.Null, "Merged scene manifest");
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
			task.SupportedOSPlatformVersion = "14.0";

			ExecuteTask (task);

			var plist = PDictionary.OpenFile (task.CompiledAppManifest!.ItemSpec);
			Assert.That (plist.GetMinimumOSVersion (), Is.EqualTo ("14.0"), "MinimumOSVersion");
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
			task.SupportedOSPlatformVersion = "14.0";

			ExecuteTask (task);

			var plist = PDictionary.OpenFile (task.CompiledAppManifest!.ItemSpec);
			Assert.That (plist.GetMinimumOSVersion (), Is.EqualTo ("13.0"), "MinimumOSVersion");
		}

		[Test]
		[TestCase (false, "14.0")]
		[TestCase (true, "13.0")]
		public void MultipleMinimumOSVersions_Overwrite (bool overwrite, string expectedMinimumOSVersion)
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir);

			var mainPath = Path.Combine (dir, "Info.plist");
			var main = new PDictionary ();
			main.SetMinimumOSVersion ("14.0");
			main.Save (mainPath);

			var partialPath = Path.Combine (dir, "PartialAppManifest.plist");
			var partial = new PDictionary ();
			partial.SetMinimumOSVersion ("13.0");
			partial.Save (partialPath);

			task.AppManifest = new TaskItem (mainPath);
			var partialAppManifest = new TaskItem (partialPath);
			partialAppManifest.SetMetadata ("Overwrite", overwrite ? "true" : "false");
			task.PartialAppManifests = new [] { partialAppManifest };
			task.SupportedOSPlatformVersion = "14.0";

			ExecuteTask (task);

			var plist = PDictionary.OpenFile (task.CompiledAppManifest!.ItemSpec);
			Assert.That (plist.GetMinimumOSVersion (), Is.EqualTo (expectedMinimumOSVersion), "MinimumOSVersion");
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
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Is.EqualTo ("The MinimumOSVersion value in the Info.plist (10.0) does not match the SupportedOSPlatformVersion value (11.0) in the project file (if there is no SupportedOSPlatformVersion value in the project file, then a default value has been assumed). Either change the value in the Info.plist to match the SupportedOSPlatformVersion value, or remove the value in the Info.plist (and add a SupportedOSPlatformVersion value to the project file if it doesn't already exist)."));
		}

		[Test]
		public void SupportedOSPlatformVersion ()
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir);

			task.SupportedOSPlatformVersion = "11.0";

			ExecuteTask (task);

			var plist = PDictionary.OpenFile (task.CompiledAppManifest!.ItemSpec);
			Assert.That (plist.GetMinimumOSVersion (), Is.EqualTo ("11.0"), "MinimumOSVersion");
		}

		[Test]
		public void MacCatalystVersionCheck ()
		{
			var task = CreateTask (platform: ApplePlatform.MacCatalyst);
			task.SupportedOSPlatformVersion = "14.2";
			ExecuteTask (task);

			var plist = PDictionary.OpenFile (task.CompiledAppManifest!.ItemSpec);
			Assert.That (plist.GetMinimumSystemVersion (), Is.EqualTo ("11.0"), "MinimumOSVersion");
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
		[TestCase (ApplePlatform.iOS, true, "iphonesimulator")]
		[TestCase (ApplePlatform.iOS, false, "iphoneos")]
		[TestCase (ApplePlatform.MacCatalyst, false, "macosx")]
		[TestCase (ApplePlatform.TVOS, true, "appletvsimulator")]
		[TestCase (ApplePlatform.TVOS, false, "appletvos")]
		[TestCase (ApplePlatform.MacOSX, false, "macosx")]
		public void XcodeVariables (ApplePlatform platform, bool isSimulator, string expectedDTPlatformName)
		{
			var task = CreateTask (platform: platform);
			task.SdkIsSimulator = isSimulator;
			ExecuteTask (task);

			var plist = PDictionary.OpenFile (task.CompiledAppManifest!.ItemSpec);
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
			Assert.That (plist.GetString ("DTPlatformName")?.Value, Is.EqualTo (expectedDTPlatformName), "Expected DTPlatformName");
		}

		[Test]
		[TestCase ("ARM64")]
		[TestCase ("x86_64")]
		[TestCase ("x86_64, ARM64")]
		public void MacCatalystDoesNotInjectRequiredDeviceCapabilities (string targetArchitectures)
		{
			// UIRequiredDeviceCapabilities is neither required nor evaluated for Mac Catalyst (the
			// macOS App Store ignores hardware capability values such as 'arm64'). Injecting an
			// architecture-specific value would also make the Info.plist differ between the x64 and
			// arm64 slices of a universal build, which breaks merging the per-RID app bundles. Verify
			// we never inject it for Mac Catalyst.
			var task = CreateTask (platform: ApplePlatform.MacCatalyst);
			task.TargetArchitectures = targetArchitectures;
			ExecuteTask (task);

			var plist = PDictionary.OpenFile (task.CompiledAppManifest!.ItemSpec);
			Assert.That (plist.ContainsKey (ManifestKeys.UIRequiredDeviceCapabilities), Is.False, "UIRequiredDeviceCapabilities");
		}

		[Test]
		[TestCase ("metal")]
		[TestCase ("arm64")]
		public void MacCatalystPreservesUserRequiredDeviceCapabilities (string capability)
		{
			// A shared iOS/iPad/Mac Catalyst Info.plist may legitimately declare
			// UIRequiredDeviceCapabilities. It's ignored on Mac Catalyst, but we must preserve it
			// as-authored (and identically between the architecture slices of a universal build, so
			// the app bundles can be merged).
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir, ApplePlatform.MacCatalyst);
			task.TargetArchitectures = "ARM64";

			var manifest = new PDictionary ();
			manifest [ManifestKeys.UIRequiredDeviceCapabilities] = new PArray { new PString (capability) };
			var manifestPath = Path.Combine (dir, "Info.plist");
			manifest.Save (manifestPath);
			task.AppManifest = new TaskItem (manifestPath);

			ExecuteTask (task);

			var plist = PDictionary.OpenFile (task.CompiledAppManifest!.ItemSpec);
			var array = plist.Get<PArray> (ManifestKeys.UIRequiredDeviceCapabilities);
			Assert.That (array, Is.Not.Null, "present");
			Assert.That (array!.OfType<PString> ().Select (x => x.Value).ToArray (), Is.EqualTo (new [] { capability }), "preserved");
		}
	}
}
