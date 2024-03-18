using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;
using Xamarin.MacDev;

using Microsoft.Build.Framework;
using Microsoft.Build.Logging.StructuredLogger;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class DetectSigningIdentityTests {
		[Test]
		public void BundleIdentifierInPartialAppManifest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertDotNetAvailable ();

			// https://github.com/xamarin/xamarin-macios/issues/12051
			var csproj = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>{Configuration.DotNetTfm}-macos</TargetFramework>
		<OutputType>Exe</OutputType>
    </PropertyGroup>

	<ItemGroup>
		<PartialAppManifest Include=""MyPartialManifest.plist"" />
	</ItemGroup>
</Project>";

			var partialPList = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
        <key>CFBundleIdentifier</key>
        <string>com.xamarin.detectsigningidentitytest</string>
</dict>
</plist>";

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var csprojPath = Path.Combine (tmpdir, "BundleIdentifierInPartialAppManifest.csproj");
			File.WriteAllText (csprojPath, csproj);

			// Create an empty main app manifest
			var mainPListPath = Path.Combine (tmpdir, "Info.plist");
			new PDictionary ().Save (mainPListPath);

			// Save our custom partial app manifest
			var partialPListPath = Path.Combine (tmpdir, "MyPartialManifest.plist");
			File.WriteAllText (partialPListPath, partialPList);

			var engine = new BuildEngine ();
			var properties = new Dictionary<string, string> {
				{ "_CanOutputAppBundle", "true" },
			};
			var rv = engine.RunTarget (ApplePlatform.MacOSX, ExecutionMode.DotNet, csprojPath, target: "_DetectSigningIdentity", properties: properties);
			Assert.AreEqual (0, rv.ExitCode, "Exit code");

			// Find the BundleIdentifier parameter to the DetectSigningIdentity task.
			var recordArgs = BinLog.ReadBuildEvents (rv.BinLogPath).ToList ();
			var taskIndex = recordArgs.FindIndex (v => v is TaskStartedEventArgs tsea && tsea.TaskName == "DetectSigningIdentity");
			Assert.That (taskIndex, Is.GreaterThan (0), "Task index");
			var taskParameterIndex = recordArgs.FindIndex (taskIndex + 1, v => v is BuildMessageEventArgs bmea && bmea.Message.StartsWith ("Task Parameter:BundleIdentifier="));
			Assert.That (taskParameterIndex, Is.GreaterThan (0), "Parameter index");
			var taskParameter = (BuildMessageEventArgs) recordArgs [taskParameterIndex];
			var bundleIdentifier = taskParameter.Message.Substring ("Task Parameter:BundleIdentifier=".Length);
			Assert.AreEqual ("com.xamarin.detectsigningidentitytest", bundleIdentifier, "Bundle identifier");
		}
	}
}

