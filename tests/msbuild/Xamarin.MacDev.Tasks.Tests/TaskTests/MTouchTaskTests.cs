using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks {
	class CustomMTouchTask : MTouch {
		public CustomMTouchTask ()
		{
			Architectures = "Default";
			Debug = false;
			ExtraArgs = null;
			FastDev = false;
			I18n = null;
			LinkMode = "SdkOnly";
			Profiling = false;
			SdkIsSimulator = true;
			UseLlvm = false;
			UseThumb = false;
			AppExtensionReferences = new Microsoft.Build.Framework.ITaskItem [] { };
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
	public class MTouchTaskTests : TestBase {
		CustomMTouchTask Task {
			get; set;
		}

		public override void Setup ()
		{
			base.Setup ();

			var tmpdir = Cache.CreateTemporaryDirectory ();

			Task = CreateTask<CustomMTouchTask> ();
			Task.ToolExe = "/path/to/mtouch";

			Task.AppBundleDir = AppBundlePath;
			Task.MinimumOSVersion = PDictionary.FromFile (Path.Combine (MonoTouchProjectPath, "Info.plist")).GetMinimumOSVersion ();
			Task.CompiledEntitlements = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "Resources", "Entitlements.plist");
			Task.IntermediateOutputPath = Path.Combine ("obj", "mtouch-cache");
			Task.MainAssembly = new TaskItem ("Main.exe");
			Task.References = new [] { new TaskItem ("a.dll"), new TaskItem ("b with spaces.dll"), new TaskItem ("c\"quoted\".dll") };
			Task.ResponseFilePath = Path.Combine (tmpdir, "response-file.rsp");
			Task.SdkRoot = "/path/to/sdkroot";
			Task.SdkVersion = "6.1";
			Task.SymbolsList = Path.Combine (tmpdir, "mtouch-symbol-list");
			Task.TargetFrameworkMoniker = "Xamarin.iOS,v1.0";
		}

		[Test]
		public void StandardCommandline ()
		{
			var args = Task.GenerateCommandLineCommands ();
			// --reference=*/a.dll
			Assert.IsTrue (Task.ResponseFile.Contains ("--reference=" + Path.GetFullPath ("a.dll")), "#1a");
			// "--reference=*/b with spaces.dll"
			Assert.IsTrue (Task.ResponseFile.Contains ("\"--reference=" + Path.GetFullPath ("b with spaces.dll") + "\""), "#1b");
			// "--reference=*/c\"quoted\".dll"
			Assert.IsTrue (Task.ResponseFile.Contains ("\"--reference=" + Path.GetFullPath ("c\\\"quoted\\\".dll") + "\""), "#1c");
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
			Assert.IsTrue (args.Contains ("--customarg"), "#1");
		}

		[Test]
		public void StandardCommandline_MinimumOsVersion ()
		{
			Task.MinimumOSVersion = "10.0";
			Task.GenerateCommandLineCommands ();
			Assert.That (Task.ResponseFile, Does.Contain ("--targetver=10.0"), "#1");
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

			Task.TargetFrameworkMoniker = frameworkIdentifier + ",v1.0";
		}

		[Test]
		public void StandardCommandline_WithBitcodeEnabled_iOS ()
		{
			MTouchEnableBitcode ("Xamarin.iOS");

			var ex = Assert.Throws<InvalidOperationException> (() => Task.GenerateCommandLineCommands (), "Exception");
			Assert.AreEqual ("Bitcode is currently not supported on iOS.", ex.Message, "Message");
		}

		[Test]
		public void StandardCommandline_WithBitcodeEnabled_watchOS ()
		{
			MTouchEnableBitcode ("Xamarin.WatchOS");

			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (Task.ResponseFile.Contains ("--bitcode=full"));
		}

		[Test]
		public void StandardCommandline_WithBitcodeEnabled_tvOS ()
		{
			MTouchEnableBitcode ("Xamarin.TVOS");

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
				Assert.IsFalse (args.Contains ("$"), "#1");
				Assert.IsTrue (args.Contains ("xyz-path/to-xyz"), "#ProjectDir");
				Assert.That (args, Does.Match ("xxx-.*/MySingleView/bin/iPhoneSimulator/Debug/MySingleView.app-xxx"), "#AppBundleDir");
				Assert.IsTrue (args.Contains ("yyy-Main.exe-yyy"), "#TargetPath");
				Assert.IsTrue (args.Contains ("yzy--yzy"), "#TargetDir");
				Assert.IsTrue (args.Contains ("zzz-Main.exe-zzz"), "#TargetName");
				Assert.IsTrue (args.Contains ("zyx-.exe-zyx"), "#TargetExt");
			} finally {
				Task.ExtraArgs = null;
			}
		}

		[Test]
		public void BuildEntitlementFlagsTest ()
		{
			var args = Task.GenerateCommandLineCommands ();
			Assert.That (args, Does.Contain ("\"--gcc_flags=-Xlinker -sectcreate -Xlinker __TEXT -Xlinker __entitlements -Xlinker"), "#1");
			Assert.That (args, Does.Contain ("Entitlements.plist"), "#2");
		}

		[Test]
		public void ReferenceFrameworkFileResolution_WhenReceivedReferencePathExists ()
		{
			using (var sdk = new TempSdk ()) {
				Task.TargetFrameworkMoniker = "MonoTouch,v1.0";

				var expectedPath = Path.Combine (Cache.CreateTemporaryDirectory (), "tmpfile");

				Task.References = new [] { new TaskItem (expectedPath, new Dictionary<string, string> { { "FrameworkFile", "true" } }) };

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

		[TestCase ("Xamarin.iOS,v1.0", "Xamarin.iOS")]
		public void ReferenceFrameworkFileResolution_WhenFacadeFileExists (string targetFrameworkMoniker, string frameworkDir)
		{
			using (var sdk = new TempSdk ()) {
				Task.TargetFrameworkMoniker = targetFrameworkMoniker;
				var expectedPath = Path.Combine (Sdks.XamIOS.LibDir, "mono", frameworkDir, "Facades", "System.Collections.dll");
				Directory.CreateDirectory (Path.GetDirectoryName (expectedPath));
				File.WriteAllText (expectedPath, "");

				Task.References = new [] { new TaskItem ("System.Collections.dll", new Dictionary<string, string> { { "FrameworkFile", "true" } }) };

				var args = Task.GenerateCommandLineCommands ();

				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
					// In Windows, the path slashes are escaped.
					expectedPath = expectedPath.Replace ("\\", "\\\\");

				Assert.IsTrue (Task.ResponseFile.Contains (expectedPath), string.Format (
					@"Failed to resolve facade assembly to the Sdk path.
	Expected path:{0}

	Actual args:{1}", expectedPath, Task.ResponseFile));
			}
		}

		[TestCase ("Xamarin.iOS,v1.0", "Xamarin.iOS")]
		public void ReferenceFrameworkFileResolution_WhenFrameworkFileExists (string targetFrameworkMoniker, string frameworkDir)
		{
			using (var sdk = new TempSdk ()) {
				Task.TargetFrameworkMoniker = targetFrameworkMoniker;
				var expectedPath = Path.Combine (Sdks.XamIOS.LibDir, "mono", frameworkDir, "System.Collections.dll");
				Directory.CreateDirectory (Path.GetDirectoryName (expectedPath));
				File.WriteAllText (expectedPath, "");

				Task.References = new [] { new TaskItem ("System.Collections.dll", new Dictionary<string, string> { { "FrameworkFile", "true" } }) };

				var args = Task.GenerateCommandLineCommands ();

				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
					// In Windows, the path slashes are escaped.
					expectedPath = expectedPath.Replace ("\\", "\\\\");

				Assert.IsTrue (Task.ResponseFile.Contains (expectedPath), string.Format (
					@"Failed to resolve facade assembly to the Sdk path.
	Expected path:{0}

	Actual args:{1}", expectedPath, Task.ResponseFile));
			}
		}

		[TestCase ("Xamarin.iOS,v1.0", "Xamarin.iOS")]
		public void ReferenceFrameworkFileResolution_WhenResolutionFails (string targetFrameworkMoniker, string frameworkDir)
		{
			using (var sdk = new TempSdk ()) {
				Task.TargetFrameworkMoniker = targetFrameworkMoniker;

				Task.References = new [] { new TaskItem ("/usr/foo/System.Collections.dll", new Dictionary<string, string> { { "FrameworkFile", "true" } }) };

				var args = Task.GenerateCommandLineCommands ();

				Assert.IsTrue (Task.ResponseFile.Contains ("/usr/foo/System.Collections.dll"));
			}
		}

		[Test]
		public void NativeReference_None ()
		{
			// non-framework native references do not have to copy anything else (back to Windows)

			Task.NativeReferences = null;
			var items = Task.GetAdditionalItemsToBeCopied ().ToArray ();
			Assert.That (items.Count (), Is.EqualTo (0), "null");

			Task.NativeReferences = new TaskItem [] { };
			items = Task.GetAdditionalItemsToBeCopied ().ToArray ();
			Assert.That (items.Count (), Is.EqualTo (0), "none");

			var temp = Cache.CreateTemporaryDirectory ();
			var native_lib = Path.Combine (temp, "libFoo");
			File.WriteAllText (native_lib, "fake lib");
			Task.NativeReferences = new [] { new TaskItem (native_lib) };
			items = Task.GetAdditionalItemsToBeCopied ().ToArray ();
			Assert.That (items.Count (), Is.EqualTo (0), "non-framework path");
		}

		[Test]
		public void NativeReference_Framework ()
		{
			var temp = Cache.CreateTemporaryDirectory ();
			var fx = Path.Combine (temp, "project", "Universal.xcframework", "macos-arm64_x86_64", "Universal.framework");
			Directory.CreateDirectory (fx);

			var native_lib = Path.Combine (fx, "Universal");
			File.WriteAllText (native_lib, "fake lib");

			// other files
			File.WriteAllText (Path.Combine (fx, "Info.plist"), "fake info.plist");
			var headers = Path.Combine (fx, "Headers");
			Directory.CreateDirectory (headers);
			File.WriteAllText (Path.Combine (headers, "Universal.h"), "fake headers");
			var signature = Path.Combine (fx, "_CodeSignature");
			Directory.CreateDirectory (signature);
			File.WriteAllText (Path.Combine (signature, "CodeResources"), "fake resources");

			// a native reference to a framework needs to bring (all of) the framework files (back to Windows)

			Task.NativeReferences = new [] { new TaskItem (native_lib) };
			var items = Task.GetAdditionalItemsToBeCopied ().ToArray ();
			// 3 additional files (as we do not duplicate the TaskItem for the native library itself)
			Assert.That (items.Count (), Is.EqualTo (3), "framework files");
		}

		class TempSdk : IDisposable {
			MonoTouchSdk sdk;

			public TempSdk ()
			{
				SdkDir = Cache.CreateTemporaryDirectory ();
				Directory.CreateDirectory (Path.Combine (SdkDir, "bin"));
				File.WriteAllText (Path.Combine (SdkDir, "Version"), "1.0.0.0"); // Fake Version file so that MonoTouchSdk detects this as a real Sdk location.
				File.WriteAllText (Path.Combine (SdkDir, "bin", "mtouch"), "echo \"fake mtouch\""); // Fake mtouch binary so that MonoTouchSdk detects this as a real Sdk location.
				Directory.CreateDirectory (Path.Combine (SdkDir, "lib"));
				sdk = Sdks.XamIOS;

				Sdks.XamIOS = new MonoTouchSdk (SdkDir);
			}

			public string SdkDir { get; private set; }

			public void Dispose ()
			{
				Sdks.XamIOS = sdk;
			}
		}
	}
}
