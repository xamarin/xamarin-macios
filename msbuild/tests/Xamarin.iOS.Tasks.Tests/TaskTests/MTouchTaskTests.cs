using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Utilities;

using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	class CustomMTouchTask : MTouchTaskBase
	{
		public CustomMTouchTask ()
		{
			Architectures = "Default";
			Debug = false;
			EnableGenericValueTypeSharing = true;
			ExtraArgs = null;
			FastDev = false;
			I18n = null;
			LinkMode = "SdkOnly";
			Profiling = false;
			SdkIsSimulator = true;
			UseLlvm = false;
			UseThumb = false;
			AppExtensionReferences = new Microsoft.Build.Framework.ITaskItem[] { };
		}

		public string ResponseFile = "";

		public new string GenerateCommandLineCommands ()
		{
			var cl = base.GenerateCommandLineCommands ();

			try {
				using (StreamReader sr = new StreamReader (ResponseFilePath))
					ResponseFile = sr.ReadToEnd ();
			} catch (Exception e) {
				Assert.Fail ($"The file could not be read: {e.Message}");
			}

			return cl;
		}
	}

	[TestFixture]
	public class MTouchTaskTests : TestBase
	{
		CustomMTouchTask Task {
			get; set;
		}

		public override void Setup ()
		{
			base.Setup ();

			Task = CreateTask<CustomMTouchTask> ();
			Task.ToolExe = "/path/to/mtouch";

			Task.AppBundleDir = AppBundlePath;
			Task.AppManifest = new TaskItem (Path.Combine (MonoTouchProjectPath, "Info.plist"));
			Task.CompiledEntitlements = Path.Combine ("..", "bin", "Resources", "Entitlements.plist");
			Task.IntermediateOutputPath = Path.Combine ("obj", "mtouch-cache");
			Task.MainAssembly = new TaskItem ("Main.exe");
			Task.References = new [] { new TaskItem ("a.dll"), new TaskItem ("b with spaces.dll"), new TaskItem ("c\"quoted\".dll") };
			Task.ResponseFilePath = Path.Combine (Path.GetTempPath (), "response-file.rsp");
			Task.SdkRoot = "/path/to/sdkroot";
			Task.SdkVersion = "6.1";
			Task.SymbolsList = Path.Combine (Path.GetTempPath (), "mtouch-symbol-list");
			Task.TargetFrameworkIdentifier = "Xamarin.iOS";
		}

		[Test]
		public void StandardCommandline ()
		{
			var args = Task.GenerateCommandLineCommands ();
			// -r=*/a.dll
			Assert.IsTrue (Task.ResponseFile.Contains ("-r=" + Path.GetFullPath ("a.dll")), "#1a");
			// "-r=*/b with spaces.dll"
			Assert.IsTrue (Task.ResponseFile.Contains ("\"-r=" + Path.GetFullPath ("b with spaces.dll") + "\""), "#1b");
			// "-r=*/c\"quoted\".dll"
			Assert.IsTrue (Task.ResponseFile.Contains ("\"-r=" + Path.GetFullPath ("c\\\"quoted\\\".dll") + "\""), "#1c");
			Assert.IsTrue (Task.ResponseFile.Contains ("Main.exe"), "#2");

			var expectedSimArg = $"--sim={Path.GetFullPath (AppBundlePath)}";
			Assert.IsTrue (Task.ResponseFile.Contains (expectedSimArg), "#3");
			Assert.IsTrue (Task.ResponseFile.Contains ("--sdk="), "#4");
		}

		[Test]
		public void StandardCommandline_WithExtraArgs ()
		{
			Task.Debug = true;
			Task.GenerateCommandLineCommands ();
			Assert.IsTrue (Task.ResponseFile.Contains ("--debug"), "#1");

			Task.Debug = false;
			Task.GenerateCommandLineCommands ();
			Assert.IsFalse (Task.ResponseFile.Contains ("--debug"), "#2");
		}

		[Test]
		public void StandardCommandline_WithMtouchDebug ()
		{
			Task.ProjectDir = "path/to";
			Task.ExtraArgs = "--customarg";

			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (Task.ResponseFile.Contains ("--customarg"), "#1");
		}

		[Test]
		public void StandardCommandline_NoMinimumOsVersion ()
		{
			var modifiedPListPath = SetPListKey ("MinimumOSVersion", null);
			Task.AppManifest = new TaskItem (modifiedPListPath); 
			var args = Task.GenerateCommandLineCommands ();
			Assert.IsFalse (Task.ResponseFile.Contains ("--targetver"), "#1");
		}

		[Test]
		public void StandardCommandline_WithSdk ()
		{
			Task.SdkVersion = "7.5";

			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (Task.ResponseFile.Contains ("--sdk=7.5"), "#1");
		}

		public void MTouchEnableBitcode (string frameworkIdentifier)
		{
			Task.EnableBitcode = true;

			Task.TargetFrameworkIdentifier = frameworkIdentifier;
		}

		[Test]
		[ExpectedException (typeof(InvalidOperationException), ExpectedMessage = "Bitcode is currently not supported on iOS.")]
		public void StandardCommandline_WithBitcodeEnabled_iOS ()
		{
			MTouchEnableBitcode("Xamarin.iOS");

			Task.GenerateCommandLineCommands ();
		}

		[Test]
		public void StandardCommandline_WithBitcodeEnabled_watchOS ()
		{
			MTouchEnableBitcode("Xamarin.WatchOS");

			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (Task.ResponseFile.Contains ("--bitcode=full"));
		}

		[Test]
		public void StandardCommandline_WithBitcodeEnabled_tvOS ()
		{
			MTouchEnableBitcode("Xamarin.TVOS");

			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (Task.ResponseFile.Contains ("--bitcode=asmonly"));
		}

		[Test]
		public void StandardCommandline_WithFloat32 ()
		{
			Task.UseFloat32 = true;

			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (Task.ResponseFile.Contains ("--aot-options=-O=float32"));
		}

		[Test]
		public void StandardCommandline_WithoutFloat32 ()
		{
			Task.UseFloat32 = false;

			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (Task.ResponseFile.Contains ("--aot-options=-O=-float32"));
		}

		[Test]
		public void ParsedExtraArgs ()
		{
			try {
				Task.ProjectDir = "path/to";
				Task.ExtraArgs = "xyz-${ProjectDir}-xyz xxx-${AppBundleDir}-xxx yyy-${TargetPath}-yyy yzy-${TargetDir}-yzy zzz-${TargetName}-zzz zyx-${TargetExt}-zyx";
				var args = Task.GenerateCommandLineCommands ();
				Assert.IsFalse (Task.ResponseFile.Contains ("$"), "#1");
				Assert.IsTrue (Task.ResponseFile.Contains ("xyz-path/to-xyz"), "#ProjectDir");
				Assert.IsTrue (Task.ResponseFile.Contains ("xxx-../MySingleView/bin/iPhoneSimulator/Debug/MySingleView.app-xxx"), "#AppBundleDir");
				Assert.IsTrue (Task.ResponseFile.Contains ("yyy-Main.exe-yyy"), "#TargetPath");
				Assert.IsTrue (Task.ResponseFile.Contains ("yzy--yzy"), "#TargetDir");
				Assert.IsTrue (Task.ResponseFile.Contains ("zzz-Main.exe-zzz"), "#TargetName");
				Assert.IsTrue (Task.ResponseFile.Contains ("zyx-.exe-zyx"), "#TargetExt");
			} finally {
				Task.ExtraArgs = null;
			}
		}

		[Test]
		public void BuildEntitlementFlagsTest ()
		{
			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (Task.ResponseFile.Contains ("\"--gcc_flags=-Xlinker -sectcreate -Xlinker __TEXT -Xlinker __entitlements -Xlinker"), "#1");
			Assert.IsTrue (Task.ResponseFile.Contains ("Entitlements.plist"), "#2");
		}

		[Test]
		public void ReferenceFrameworkFileResolution_WhenReceivedReferencePathExists()
		{
			using (var sdk = new TempSdk()) {
				Task.TargetFrameworkIdentifier = "MonoTouch";

				var expectedPath = Path.GetTempFileName ();

				Task.References = new[] { new TaskItem (expectedPath, new Dictionary<string, string> { { "FrameworkFile", "true" } }) };

				var args = Task.GenerateCommandLineCommands ();

				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
					// In Windows, the path slashes are escaped.
					expectedPath = expectedPath.Replace ("\\", "\\\\");

				Assert.IsTrue (Task.ResponseFile.Contains (expectedPath));
			}
		}

		[Test]
		public void ResponseFileTest ()
		{
			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (args.Contains ($"@{Task.ResponseFilePath}"), "#@response-file");
		}

		[TestCase("Xamarin.iOS", "Xamarin.iOS")]
		public void ReferenceFrameworkFileResolution_WhenFacadeFileExists(string targetFramework, string frameworkDir)
		{
			using (var sdk = new TempSdk()) {
				Task.TargetFrameworkIdentifier = targetFramework;
				var expectedPath = Path.Combine (IPhoneSdks.MonoTouch.LibDir, "mono", frameworkDir, "Facades", "System.Collections.dll");
				Directory.CreateDirectory (Path.GetDirectoryName (expectedPath));
				File.WriteAllText (expectedPath, "");

				Task.References = new[] { new TaskItem ("System.Collections.dll", new Dictionary<string, string> { { "FrameworkFile", "true" } }) };

				var args = Task.GenerateCommandLineCommands ();

				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
					// In Windows, the path slashes are escaped.
					expectedPath = expectedPath.Replace ("\\", "\\\\");

				Assert.IsTrue (Task.ResponseFile.Contains (expectedPath), string.Format(
					@"Failed to resolve facade assembly to the Sdk path.
	Expected path:{0}

	Actual args:{1}", expectedPath, Task.ResponseFile));
			}
		}

		[TestCase("Xamarin.iOS", "Xamarin.iOS")]
		public void ReferenceFrameworkFileResolution_WhenFrameworkFileExists(string targetFramework, string frameworkDir)
		{
			using (var sdk = new TempSdk()) {
				Task.TargetFrameworkIdentifier = targetFramework;
				var expectedPath = Path.Combine (IPhoneSdks.MonoTouch.LibDir, "mono", frameworkDir, "System.Collections.dll");
				Directory.CreateDirectory (Path.GetDirectoryName (expectedPath));
				File.WriteAllText (expectedPath, "");

				Task.References = new[] { new TaskItem ("System.Collections.dll", new Dictionary<string, string> { { "FrameworkFile", "true" } }) };

				var args = Task.GenerateCommandLineCommands ();

				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
					// In Windows, the path slashes are escaped.
					expectedPath = expectedPath.Replace ("\\", "\\\\");

				Assert.IsTrue (Task.ResponseFile.Contains (expectedPath), string.Format(
					@"Failed to resolve facade assembly to the Sdk path.
	Expected path:{0}

	Actual args:{1}", expectedPath, Task.ResponseFile));
			}
		}

		[TestCase("Xamarin.iOS", "Xamarin.iOS")]
		public void ReferenceFrameworkFileResolution_WhenResolutionFails(string targetFramework, string frameworkDir)
		{
			using (var sdk = new TempSdk()) {
				Task.TargetFrameworkIdentifier = targetFramework;

				Task.References = new[] { new TaskItem ("/usr/foo/System.Collections.dll", new Dictionary<string, string> { { "FrameworkFile", "true" } }) };

				var args = Task.GenerateCommandLineCommands ();

				Assert.IsTrue (Task.ResponseFile.Contains ("/usr/foo/System.Collections.dll"));
			}
		}

		class TempSdk : IDisposable
		{
			MonoTouchSdk sdk;

			public TempSdk ()
			{
				SdkDir = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());
				Directory.CreateDirectory (SdkDir);
				sdk = IPhoneSdks.MonoTouch;

				IPhoneSdks.MonoTouch = new MonoTouchSdk (SdkDir);
			}

			public string SdkDir { get; private set; }

			public void Dispose ()
			{
				IPhoneSdks.MonoTouch = sdk;
				Directory.Delete (SdkDir, true);
			}
		}
	}
}

