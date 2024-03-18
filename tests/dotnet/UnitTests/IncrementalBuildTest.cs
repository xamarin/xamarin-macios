using Microsoft.Build.Logging.StructuredLogger;

#nullable enable

namespace Xamarin.Tests {
	public class IncrementalBuildTest : TestBaseClass {

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void NativeLink (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GenerateProject (platform, name: nameof (NativeLink), runtimeIdentifiers: runtimeIdentifiers, out var appPath);
			var properties = new Dictionary<string, string> (verbosity);
			SetRuntimeIdentifiers (properties, runtimeIdentifiers);

			var mainContents = @"
class MainClass {
	static int Main ()
	{
		return 123;
	}
}
";
			var mainFile = Path.Combine (Path.GetDirectoryName (project_path)!, "Main.cs");

			File.WriteAllText (mainFile, mainContents);

			// Build the first time
			var rv = DotNet.AssertBuild (project_path, properties);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_AOTCompile", "A");
			AssertTargetExecuted (allTargets, "_CompileNativeExecutable", "A");
			AssertTargetExecuted (allTargets, "_LinkNativeExecutable", "A");

			// Capture the current time
			var timestamp = DateTime.UtcNow;
			File.WriteAllText (mainFile, mainContents);

			// Build again
			rv = DotNet.AssertBuild (project_path, properties);

			// Check that some targets executed
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_AOTCompile", "B");
			AssertTargetNotExecuted (allTargets, "_CompileNativeExecutable", "B");
			AssertTargetExecuted (allTargets, "_LinkNativeExecutable", "B");

			// Verify that the timestamp of the executable has been updated
			var executable = GetNativeExecutable (platform, appPath!);
			Assert.That (File.GetLastWriteTimeUtc (executable), Is.GreaterThan (timestamp), "B: Executable modified");
		}

		void AssertTargetExecuted (IEnumerable<TargetExecutionResult> executedTargets, string targetName, string message)
		{
			var targets = executedTargets.Where (v => v.TargetName == targetName);
			if (!targets.Any ())
				Assert.Fail ($"The target '{targetName}' was not executed: no corresponding targets found in binlog ({message})");
			if (!targets.Any (v => !v.Skipped))
				Assert.Fail ($"The target '{targetName}' was not executed: the target was found {targets.Count ()} time(s) in the binlog, but they were all skipped ({message})");
		}

		void AssertTargetNotExecuted (IEnumerable<TargetExecutionResult> executedTargets, string targetName, string message)
		{
			var targets = executedTargets.Where (v => v.TargetName == targetName);
			if (targets.Any (v => !v.Skipped))
				Assert.Fail ($"The target '{targetName}' was unexpectedly executed ({message})");
		}
	}
}
