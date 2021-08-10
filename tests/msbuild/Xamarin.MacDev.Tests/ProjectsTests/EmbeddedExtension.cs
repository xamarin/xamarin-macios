using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.iOS.Tasks {
	public class EmbeddedExtension : ProjectTest {
		// Simulator only as extensions require signing otherwise, which is hard on bots
		public EmbeddedExtension () : base ("iPhoneSimulator")
		{
		}

		[Test]
		public void BasicTest ()
		{
			var proj = SetupProjectPaths ("NativeExtensionEmbedding/managed/ManagedContainer");
			MonoTouchProject = proj;

			var xcodeProjectFolder = Path.Combine (proj.ProjectPath , "..", "..", "native");
			string [] xcodeBuildArgs = new [] { "-configuration", "Debug", "-target", "NativeTodayExtension", "-sdk", "iphonesimulator" };
			var env = new System.Collections.Generic.Dictionary<string, string> { { "DEVELOPER_DIR", Configuration.XcodeLocation } };
			Assert.AreEqual (0, ExecutionHelper.Execute ("/usr/bin/xcodebuild", xcodeBuildArgs.Concat (new [] { "clean" }).ToList (), xcodeProjectFolder, Console.WriteLine, Console.Error.WriteLine));

			var buildOutput = new StringBuilder ();
			var buildCode = ExecutionHelper.Execute ("/usr/bin/xcodebuild", xcodeBuildArgs.Concat (new [] { "build" }).ToList (), xcodeProjectFolder, t => buildOutput.Append (t), t => buildOutput.Append (t));
			Assert.AreEqual (0, buildCode, $"Build Failed:{buildOutput}");

			RunTarget (proj, "Clean", 0);
			RunTarget (proj, "Build", 0);

			var expectedFilepath = Path.Combine (AppBundlePath, "PlugIns", "NativeTodayExtension.appex", "NativeTodayExtension");

			Assert.That (File.Exists (expectedFilepath), $"NativeTodayExtension, file path '{expectedFilepath}' missing.");
		}
	}
}
