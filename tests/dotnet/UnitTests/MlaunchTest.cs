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
			var rv = DotNet.Execute ("build", project_path, properties, target: "ComputeMlaunchInstallArguments");

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchInstallArguments", out var mlaunchInstallArguments))
				Assert.Fail ("Could not find the property 'MlaunchInstallArguments' in the binlog.");

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchPath", out var mlaunchPath))
				Assert.Fail ("Could not find the property 'MlaunchPath' in the binlog.");
			Assert.That (mlaunchPath, Does.Exist, "mlaunch existence");

			var expectedArguments = new StringBuilder ();
			expectedArguments.Append ("--installdev ");
			expectedArguments.Append (appPath.Substring (Path.GetDirectoryName (project_path)!.Length + 1)).Append ('/');
			expectedArguments.Append ($" --wait-for-exit:false");
			Assert.AreEqual (expectedArguments.ToString (), mlaunchInstallArguments);

			var scriptContents = File.ReadAllText (outputPath).Trim ('\n');
			var expectedScriptContents = mlaunchPath + " " + expectedArguments.ToString ();
			Assert.AreEqual (expectedScriptContents, scriptContents, "Script contents");
		}

		public static object [] GetMlaunchRunArgumentsTestCases ()
		{
			return new object [] {
				new object [] {ApplePlatform.iOS, "iossimulator-x64;iossimulator-arm64", $":v2:runtime=com.apple.CoreSimulator.SimRuntime.iOS-{SdkVersions.iOS.Replace('.', '-')},devicetype=com.apple.CoreSimulator.SimDeviceType.iPhone-15-Pro" },
				new object [] {ApplePlatform.iOS, "ios-arm64", "" },
				new object [] {ApplePlatform.TVOS, "tvossimulator-arm64", $":v2:runtime=com.apple.CoreSimulator.SimRuntime.tvOS-{SdkVersions.TVOS.Replace('.', '-')},devicetype=com.apple.CoreSimulator.SimDeviceType.Apple-TV-4K-3rd-generation-1080p" },
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
			var rv = DotNet.Execute ("build", project_path, properties, target: "ComputeMlaunchRunArguments");

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchRunArguments", out var mlaunchRunArguments))
				Assert.Fail ("Could not find the property 'MlaunchRunArguments' in the binlog.");

			if (!BinLog.TryFindPropertyValue (rv.BinLogPath, "MlaunchPath", out var mlaunchPath))
				Assert.Fail ("Could not find the property 'MlaunchPath' in the binlog.");
			Assert.That (mlaunchPath, Does.Exist, "mlaunch existence");

			var expectedArguments = new StringBuilder ();
			var isSim = runtimeIdentifiers.Contains ("simulator");
			expectedArguments.Append (isSim ? "--launchsim " : "--launchdev ");
			expectedArguments.Append (appPath.Substring (Path.GetDirectoryName (project_path)!.Length + 1)).Append ('/');
			if (isSim) {
				expectedArguments.Append (" --device \"");
				expectedArguments.Append (device);
				expectedArguments.Append ('"');
			}
			expectedArguments.Append ($" --wait-for-exit:true");
			Assert.AreEqual (expectedArguments.ToString (), mlaunchRunArguments);

			var scriptContents = File.ReadAllText (outputPath).Trim ('\n');
			var expectedScriptContents = mlaunchPath + " " + expectedArguments.ToString ();
			Assert.AreEqual (expectedScriptContents, scriptContents, "Script contents");
		}
	}
}
