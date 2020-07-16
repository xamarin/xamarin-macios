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
			var mtouchPaths = SetupProjectPaths ("ManagedContainer", "../NativeExtensionEmbedding/managed", true, "iPhoneSimulator");

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);

			var xcodeProjectFolder = Path.Combine (mtouchPaths.ProjectPath , "..", "..", "native");
			string [] xcodeBuildArgs = new [] { "-configuration", "Debug", "-target", "NativeTodayExtension", "-sdk", "iphonesimulator" };
			var env = new System.Collections.Generic.Dictionary<string, string> { { "DEVELOPER_DIR", Configuration.XcodeLocation } };
			Assert.AreEqual (0, ExecutionHelper.Execute ("/usr/bin/xcodebuild", xcodeBuildArgs.Concat (new [] { "clean" }).ToArray (), out var _, workingDirectory: xcodeProjectFolder, environment_variables: env, stdout_callback: Console.WriteLine, stderr_callback: Console.Error.WriteLine));

			var buildOutput = new StringBuilder ();
			var buildCode = ExecutionHelper.Execute ("/usr/bin/xcodebuild", xcodeBuildArgs.Concat (new [] { "build" }).ToArray (), out var _, workingDirectory: xcodeProjectFolder, environment_variables: env, stdout_callback: t => buildOutput.Append (t), stderr_callback: t => buildOutput.Append (t));
			Assert.AreEqual (0, buildCode, $"Build Failed:{buildOutput}");

			AppBundlePath = mtouchPaths.AppBundlePath;

			var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);

			RunTarget (proj, "Clean", 0);
			RunTarget (proj, "Build", 0);

			Assert.That (File.Exists (Path.Combine (AppBundlePath, "PlugIns", "NativeTodayExtension.appex", "NativeTodayExtension")), "NativeTodayExtension");
		}
	}
}

