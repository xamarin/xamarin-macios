using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class EmbeddedExtension : ProjectTest {
		public EmbeddedExtension (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var proj = SetupProjectPaths ("NativeExtensionEmbedding/managed/ManagedContainer");
			MonoTouchProject = proj;

			var xcodeProjectFolder = Path.Combine (proj.ProjectPath, "..", "..", "native");
			string [] xcodeBuildArgs = new [] { "-configuration", "Debug", "-target", "NativeTodayExtension", "-sdk", Platform == "iPhoneSimulator" ? "iphonesimulator" : "iphoneos" };
			var env = new System.Collections.Generic.Dictionary<string, string> { { "DEVELOPER_DIR", Configuration.XcodeLocation } };
			Assert.AreEqual (0, ExecutionHelper.Execute ("/usr/bin/xcodebuild", xcodeBuildArgs.Concat (new [] { "clean" }).ToList (), xcodeProjectFolder, Console.WriteLine, Console.Error.WriteLine));

			var buildOutput = new StringBuilder ();
			var buildCode = ExecutionHelper.Execute ("/usr/bin/xcodebuild", xcodeBuildArgs.Concat (new [] { "build" }).ToList (), xcodeProjectFolder, t => buildOutput.Append (t), t => buildOutput.Append (t));
			Assert.AreEqual (0, buildCode, $"Build Failed:{buildOutput}");

			var properties = new Dictionary<string, string> ()
			{
				{ "Platform", Platform },
			};

			RunTarget (proj, "Clean", executionMode: ExecutionMode.MSBuild, properties: properties);
			RunTarget (proj, "Build", executionMode: ExecutionMode.MSBuild, properties: properties);

			var expectedFilepath = Path.Combine (AppBundlePath, "PlugIns", "NativeTodayExtension.appex", "NativeTodayExtension");

			Assert.That (File.Exists (expectedFilepath), $"NativeTodayExtension, file path '{expectedFilepath}' missing.");

			var expectedDirectories = new List<string> ();
			if (Platform == "iPhone") {
				expectedDirectories.Add (Path.Combine (AppBundlePath, "_CodeSignature"));
				expectedDirectories.Add (Path.Combine (AppBundlePath, "PlugIns", "NativeTodayExtension.appex", "_CodeSignature"));
			}

			foreach (var dir in expectedDirectories)
				Assert.That (dir, Does.Exist, "Directory should exist.");
		}
	}
}
