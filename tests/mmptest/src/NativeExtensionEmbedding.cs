using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.Tests;

namespace Xamarin.MMP.Tests {
	[TestFixture]
	public class NativeExtensionEmbeddingTests {
		[TestCase (true)]
		[TestCase (false)]
		public void NativeExtensionEmbedding (bool XM45)
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.CopyDirectory (Path.Combine (TI.FindSourceDirectory (), @"NativeExtension"), tmpDir);

				var xcodeProjectFolder = Path.Combine (tmpDir, "NativeExtension");
				string [] xcodeBuildArgs = new [] { "-configuration", "Debug", "-target", "NativeTodayExtension", "-arch", "x86_64" };
				var env = new System.Collections.Generic.Dictionary<string, string> { { "DEVELOPER_DIR", Configuration.XcodeLocation } };
				Assert.AreEqual (0, ExecutionHelper.Execute ("/usr/bin/xcodebuild", xcodeBuildArgs.Concat (new [] { "clean" }).ToArray (), out var _, workingDirectory: xcodeProjectFolder, environment_variables: env, stdout_callback: Console.WriteLine, stderr_callback: Console.Error.WriteLine));
				Assert.AreEqual (0, ExecutionHelper.Execute ("/usr/bin/xcodebuild", xcodeBuildArgs.Concat (new [] { "build" }).ToArray (), out var _, workingDirectory: xcodeProjectFolder, environment_variables: env, stdout_callback: Console.WriteLine, stderr_callback: Console.Error.WriteLine));

				var items = @"
<ItemGroup>
	<AdditionalAppExtensions Include=""$(MSBuildProjectDirectory)/NativeExtension"">
		<Name>NativeTodayExtension</Name>
		<BuildOutput>build/Debug</BuildOutput>
	</AdditionalAppExtensions>
</ItemGroup>";

				var config = new TI.UnifiedTestConfig (tmpDir) { XM45 = XM45, ItemGroup = items };
				string csprojTarget = TI.GenerateUnifiedExecutableProject (config);
				TI.BuildProject (csprojTarget);

				Assert.That (File.Exists (Path.Combine (tmpDir, "bin", "Debug", (XM45 ? "XM45Example.app" : "UnifiedExample.app"), "Contents", "PlugIns", "NativeTodayExtension.appex", "Contents", "MacOS", "NativeTodayExtension")), "NativeTodayExtension");
			});
		}
	}
}
