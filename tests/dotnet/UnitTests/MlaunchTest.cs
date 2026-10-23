using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

using Mono.Cecil;

using Xamarin.Tests;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class MlaunchTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void GetMlaunchInstallArguments (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var outputPath = Path.Combine (Cache.CreateTemporaryDirectory (), "install.sh");
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["EnableCodeSigning"] = "false"; // Skip code signing, since that would require making sure we have code signing configured on bots.

			// Create the app manifest first, since it's required to compute the mlaunch install arguments
			DotNet.Execute ("build", project_path, properties, target: "_DetectSdkLocations;_DetectAppManifest;_CompileAppManifest;_WriteAppManifest");

			properties ["MlaunchInstallScript"] = outputPath;
			var rv = DotNet.Execute ("build", project_path, properties, assert_success: false, target: "ComputeMlaunchInstallArguments");

			if (rv.ExitCode != 0) {
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).Select (v => v.Message).OfType<string> ().ToArray ();
				Assert.That (string.Join ("\n", errors), Does.Contain ("No applicable and available devices found."));
				return;
			}

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchInstallArguments", out var mlaunchInstallArguments))
				Assert.Fail ("Could not find the property 'MlaunchInstallArguments' in the binlog.");

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchPath", out var mlaunchPath))
				Assert.Fail ("Could not find the property 'MlaunchPath' in the binlog.");
			Assert.That (mlaunchPath, Does.Exist, "mlaunch existence");

			var expectedArguments = new StringBuilder ();
			expectedArguments.Append ("--installdev ");
			expectedArguments.Append (appPath.Substring (Path.GetDirectoryName (project_path)!.Length + 1)).Append ('/');
			expectedArguments.Append ($" --wait-for-exit:false");
			Assert.That (mlaunchInstallArguments, Is.EqualTo (expectedArguments.ToString ()));

			var scriptContents = File.ReadAllText (outputPath).Trim ('\n');
			var expectedScriptContents = $"'{mlaunchPath}' " + expectedArguments.ToString ();
			Assert.That (scriptContents, Is.EqualTo (expectedScriptContents), "Script contents");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "--installdev", "", "")]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "--installdev", "explicit-device", "--devname explicit-device")]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64", "--installsim", "", "")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64", "--installdev", "", "")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64", "--installsim", "", "")]
		[NonParallelizable]
		public void DeployToDevice (ApplePlatform platform, string runtimeIdentifiers, string expectedInstallArgument, string device, string expectedDeviceArgument)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["EnableCodeSigning"] = "false"; // Skip code signing, since that would require making sure we have code signing configured on bots.
			properties ["Device"] = device;
			string? mlaunchOutput = null;
			if (!string.IsNullOrEmpty (device) && !OperatingSystem.IsWindows ()) {
				var temporaryDirectory = Cache.CreateTemporaryDirectory ();
				var mlaunchPath = Path.Combine (temporaryDirectory, "mlaunch");
				mlaunchOutput = Path.Combine (temporaryDirectory, "mlaunch-output.txt");
				File.WriteAllText (mlaunchPath, $"#!/bin/sh\nprintf '%s\\n' \"$*\" >> '{mlaunchOutput}'\n");
				File.SetUnixFileMode (mlaunchPath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
				properties ["MlaunchPath"] = mlaunchPath;
			}

			// Build the app first, since 'DeployToDevice' is meant to deploy an already-built app.
			var cleanProperties = new Dictionary<string, string> (properties) {
				["BuildProjectReferences"] = "false",
			};
			DotNet.Execute ("build", project_path, cleanProperties, target: "Clean");
			DotNet.AssertBuild (project_path, properties);

			var rv = DotNet.Execute ("build", project_path, properties, assert_success: false, target: "DeployToDevice");
			var executedTargets = BinLog.GetAllTargets (rv.BinLogPath).Where (v => !v.Skipped).Select (v => v.TargetName);
			Assert.That (executedTargets, Does.Not.Contain ("Build"), "Build target");
			Assert.That (executedTargets, Does.Not.Contain ("Compile"), "Compile target");
			Assert.That (executedTargets, Does.Not.Contain ("CoreCompile"), "CoreCompile target");

			// The 'MlaunchInstallArguments' property is computed (and thus present in the binlog) as soon as a device/simulator
			// was found, regardless of whether the actual install (done by mlaunch, executed via an <Exec/> task) succeeds - and
			// bots may not have any devices/simulators available to actually install to, so only verify the computed arguments,
			// not whether the whole build (which includes actually installing the app) succeeded.
			if (BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchInstallArguments", out var mlaunchInstallArguments) && !string.IsNullOrEmpty (mlaunchInstallArguments)) {
				Assert.That (mlaunchInstallArguments, Does.StartWith (expectedInstallArgument), "install arguments");
				if (!string.IsNullOrEmpty (expectedDeviceArgument))
					Assert.That (mlaunchInstallArguments, Does.Contain (expectedDeviceArgument), "device arguments");
				if (mlaunchOutput is not null) {
					Assert.That (mlaunchOutput, Does.Exist, "mlaunch output");
					Assert.That (File.ReadAllLines (mlaunchOutput), Does.Contain (mlaunchInstallArguments), "mlaunch invocation");
				}
				return;
			}

			Assert.That (rv.ExitCode, Is.Not.EqualTo (0), "should have failed if no arguments were computed");
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).Select (v => v.Message).OfType<string> ().ToArray ();
			Assert.That (string.Join ("\n", errors), Does.Contain ("No applicable and available devices found."));
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void DeployToDevice_AppNotBuilt (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["EnableCodeSigning"] = "false";

			// Make sure the app manifest doesn't exist, regardless of whether this project has been built by another test already.
			var manifestPath = Path.Combine (appPath, "Info.plist");
			if (File.Exists (manifestPath))
				File.Delete (manifestPath);

			var rv = DotNet.Execute ("build", project_path, properties, assert_success: false, target: "DeployToDevice");

			Assert.That (rv.ExitCode, Is.Not.EqualTo (0), "should fail because the app hasn't been built");
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).Select (v => v.Message).OfType<string> ().ToArray ();
			Assert.That (string.Join ("\n", errors), Does.Contain ("The app must be built before the arguments to launch the app using mlaunch can be computed."));
		}

		[Test]
		[NonParallelizable]
		public void RunDeploysToDevice ()
		{
			if (OperatingSystem.IsWindows ()) {
				Assert.Ignore ("Launching an app from the command line is not supported on Windows.");
				return;
			}

			var platform = ApplePlatform.iOS;
			var runtimeIdentifiers = "ios-arm64";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath ("MySimpleApp", runtimeIdentifiers: runtimeIdentifiers, platform: platform, out _);
			var temporaryDirectory = Cache.CreateTemporaryDirectory ();
			var mlaunchPath = Path.Combine (temporaryDirectory, "mlaunch");
			var mlaunchOutput = Path.Combine (temporaryDirectory, "mlaunch-output.txt");
			File.WriteAllText (mlaunchPath, $"#!/bin/sh\nprintf '%s\\n' \"$*\" >> '{mlaunchOutput}'\n");
			File.SetUnixFileMode (mlaunchPath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Device"] = "explicit-device";
			properties ["EnableCodeSigning"] = "false";
			properties ["MlaunchPath"] = mlaunchPath;

			var cleanProperties = new Dictionary<string, string> (properties) {
				["BuildProjectReferences"] = "false",
			};
			DotNet.Execute ("build", project_path, cleanProperties, target: "Clean");
			DotNet.AssertBuild (project_path, properties);
			DotNet.AssertBuild (project_path, properties, target: "Run");

			var invocations = File.ReadAllLines (mlaunchOutput);
			Assert.That (invocations, Has.Length.EqualTo (2), "mlaunch invocation count");
			Assert.That (invocations [0], Does.StartWith ("--installdev "), "install invocation");
			Assert.That (invocations [1], Does.StartWith ("--launchdev "), "run invocation");
		}

		public static object [] GetMlaunchRunArgumentsTestCases ()
		{
			return new object [] {
				new object [] {ApplePlatform.iOS, "iossimulator-x64;iossimulator-arm64", @":v2:udid=[A-F0-9-]+" },
				new object [] {ApplePlatform.iOS, "ios-arm64", "" },
				new object [] {ApplePlatform.TVOS, "tvossimulator-arm64", @":v2:udid=[A-F0-9-]+" },
			};
		}

		[Test]
		[TestCaseSource (nameof (GetMlaunchRunArgumentsTestCases))]
		public void GetMlaunchRunArguments (ApplePlatform platform, string runtimeIdentifiers, string device)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var outputPath = Path.Combine (Cache.CreateTemporaryDirectory (), "launch.sh");
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["EnableCodeSigning"] = "false"; // Skip code signing, since that would require making sure we have code signing configured on bots.

			// Create the app manifest first, since it's required to compute the mlaunch run arguments
			DotNet.Execute ("build", project_path, properties, target: "_DetectSdkLocations;_DetectAppManifest;_CompileAppManifest;_WriteAppManifest");

			properties ["MlaunchRunScript"] = outputPath;
			var rv = DotNet.Execute ("build", project_path, properties, assert_success: false, target: "ComputeMlaunchRunArguments");

			if (rv.ExitCode != 0) {
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).Select (v => v.Message).OfType<string> ().ToArray ();
				Assert.That (string.Join ("\n", errors), Does.Contain ("No applicable and available devices found."));
				return;
			}

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchRunArguments", out var mlaunchRunArguments))
				Assert.Fail ("Could not find the property 'MlaunchRunArguments' in the binlog.");

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchPath", out var mlaunchPath))
				Assert.Fail ("Could not find the property 'MlaunchPath' in the binlog.");
			Assert.That (mlaunchPath, Does.Exist, "mlaunch existence");

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "RunCommand", out var runCommand))
				Assert.Fail ("Could not find the property 'RunCommand' in the binlog.");
			Assert.That (runCommand, Is.EqualTo ($"{mlaunchPath}"), "Run command");

			var expectedArguments = new StringBuilder ();
			var isSim = runtimeIdentifiers.Contains ("simulator");
			expectedArguments.Append (isSim ? "--launchsim " : "--launchdev ");
			expectedArguments.Append (appPath.Substring (Path.GetDirectoryName (project_path)!.Length + 1)).Append ('/');
			if (isSim) {
				expectedArguments.Append (" --device ");
				expectedArguments.Append (device);
			}
			expectedArguments.Append ($" --wait-for-exit:true");
			Assert.That (mlaunchRunArguments, Does.Match (expectedArguments.ToString ()), "arguments");

			var scriptContents = File.ReadAllText (outputPath).Trim ('\n');
			var expectedScriptContents = $"'{mlaunchPath}' " + expectedArguments.ToString ();
			Assert.That (scriptContents, Does.Match (expectedScriptContents), "Script contents");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64;iossimulator-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64")]
		public void GetMlaunchRunArguments_NewProperties (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var outputPath = Path.Combine (tmpdir, "launch.sh");
			var stdoutPath = Path.Combine (tmpdir, "stdout.txt");
			var stderrPath = Path.Combine (tmpdir, "stderr.txt");
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["EnableCodeSigning"] = "false";

			// Create the app manifest first, since it's required to compute the mlaunch run arguments
			DotNet.Execute ("build", project_path, properties, target: "_DetectSdkLocations;_DetectAppManifest;_CompileAppManifest;_WriteAppManifest");

			// Set the new properties (these should take precedence over the _Mlaunch-prefixed ones)
			properties ["MlaunchRunScript"] = outputPath;
			properties ["WaitForExit"] = "false";
			properties ["StandardOutputPath"] = stdoutPath;
			properties ["StandardErrorPath"] = stderrPath;
			var rv = DotNet.Execute ("build", project_path, properties, assert_success: false, target: "ComputeMlaunchRunArguments");

			if (rv.ExitCode != 0) {
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).Select (v => v.Message).OfType<string> ().ToArray ();
				Assert.That (string.Join ("\n", errors), Does.Contain ("No applicable and available devices found."));
				return;
			}

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchRunArguments", out var mlaunchRunArguments))
				Assert.Fail ("Could not find the property 'MlaunchRunArguments' in the binlog.");

			Assert.Multiple (() => {
				Assert.That (mlaunchRunArguments, Does.Contain ("--wait-for-exit:false"), "WaitForExit should be false");
				Assert.That (mlaunchRunArguments, Does.Contain ($"--stdout {stdoutPath}"), "StandardOutputPath");
				Assert.That (mlaunchRunArguments, Does.Contain ($"--stderr {stderrPath}"), "StandardErrorPath");
			});
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64;iossimulator-arm64")]
		public void GetMlaunchRunArguments_NewPropertiesTakePrecedence (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var outputPath = Path.Combine (tmpdir, "launch.sh");
			var stdoutNew = Path.Combine (tmpdir, "stdout-new.txt");
			var stderrNew = Path.Combine (tmpdir, "stderr-new.txt");
			var stdoutOld = Path.Combine (tmpdir, "stdout-old.txt");
			var stderrOld = Path.Combine (tmpdir, "stderr-old.txt");
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["EnableCodeSigning"] = "false";

			// Create the app manifest first, since it's required to compute the mlaunch run arguments
			DotNet.Execute ("build", project_path, properties, target: "_DetectSdkLocations;_DetectAppManifest;_CompileAppManifest;_WriteAppManifest");

			// Set both old and new properties - new ones should take precedence
			properties ["MlaunchRunScript"] = outputPath;
			properties ["WaitForExit"] = "false";
			properties ["_MlaunchWaitForExit"] = "true";
			properties ["StandardOutputPath"] = stdoutNew;
			properties ["_MlaunchStandardOutputPath"] = stdoutOld;
			properties ["StandardErrorPath"] = stderrNew;
			properties ["_MlaunchStandardErrorPath"] = stderrOld;
			var rv = DotNet.Execute ("build", project_path, properties, assert_success: false, target: "ComputeMlaunchRunArguments");

			if (rv.ExitCode != 0) {
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).Select (v => v.Message).OfType<string> ().ToArray ();
				Assert.That (string.Join ("\n", errors), Does.Contain ("No applicable and available devices found."));
				return;
			}

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchRunArguments", out var mlaunchRunArguments))
				Assert.Fail ("Could not find the property 'MlaunchRunArguments' in the binlog.");

			Assert.Multiple (() => {
				Assert.That (mlaunchRunArguments, Does.Contain ("--wait-for-exit:false"), "WaitForExit (new) should take precedence");
				Assert.That (mlaunchRunArguments, Does.Contain ($"--stdout {stdoutNew}"), "StandardOutputPath (new) should take precedence");
				Assert.That (mlaunchRunArguments, Does.Not.Contain (stdoutOld), "Old StandardOutputPath should not be used");
				Assert.That (mlaunchRunArguments, Does.Contain ($"--stderr {stderrNew}"), "StandardErrorPath (new) should take precedence");
				Assert.That (mlaunchRunArguments, Does.Not.Contain (stderrOld), "Old StandardErrorPath should not be used");
			});
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void GetDesktopRunArguments_WaitForExitTakesPrecedence (ApplePlatform platform)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);
			var properties = GetDefaultProperties ();
			properties ["EnableCodeSigning"] = "false";

			// Build first so that _AppBundleName and other properties are available
			DotNet.AssertBuild (project_path, properties);

			// Set WaitForExit=true, which should take precedence over OpenWaitForExit=false
			properties ["WaitForExit"] = "true";
			properties ["OpenWaitForExit"] = "false";
			var rv = DotNet.Execute ("build", project_path, properties, target: "_PrepareRunDesktop");

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "RunArguments", out var runArguments))
				Assert.Fail ("Could not find the property 'RunArguments' in the binlog.");

			Assert.That (runArguments, Does.Contain ("-W"), "WaitForExit=true should result in -W in RunArguments");
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void GetDesktopRunArguments_WaitForExitFalse (ApplePlatform platform)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);
			var properties = GetDefaultProperties ();
			properties ["EnableCodeSigning"] = "false";

			// Build first so that _AppBundleName and other properties are available
			DotNet.AssertBuild (project_path, properties);

			// Set WaitForExit=false, which should take precedence over OpenWaitForExit=true
			properties ["WaitForExit"] = "false";
			properties ["OpenWaitForExit"] = "true";
			var rv = DotNet.Execute ("build", project_path, properties, target: "_PrepareRunDesktop");

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "RunArguments", out var runArguments))
				Assert.Fail ("Could not find the property 'RunArguments' in the binlog.");

			Assert.That (runArguments, Does.Not.Contain ("-W"), "WaitForExit=false should result in no -W in RunArguments");
		}
	}
}
