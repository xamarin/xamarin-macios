using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {
	class NUnitTestTasksEnumerable : IEnumerable<NUnitExecuteTask> {

		readonly Jenkins jenkins;
		readonly IProcessManager processManager;
		
		public NUnitTestTasksEnumerable (Jenkins jenkins, IProcessManager processManager)
		{
			this.jenkins = jenkins ?? throw new ArgumentNullException (nameof (jenkins));
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public IEnumerator<NUnitExecuteTask> GetEnumerator ()
		{
			var netstandard2Project = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "..", "msbuild", "tests", "Xamarin.iOS.Tasks.Tests", "Xamarin.iOS.Tasks.Tests.csproj")));
			var buildiOSMSBuild = new MSBuildTask (jenkins: jenkins, testProject: netstandard2Project, processManager: processManager) {
				SpecifyPlatform = false,
				SpecifyConfiguration = true,
				ProjectConfiguration = "Debug",
				Platform = TestPlatform.iOS,
				SolutionPath = Path.GetFullPath (Path.Combine (Harness.RootDirectory, "..", "msbuild", "Xamarin.MacDev.Tasks.sln")),
				SupportsParallelExecution = false,
			};
			var nunitExecutioniOSMSBuild = new NUnitExecuteTask (jenkins, buildiOSMSBuild, processManager) {
				TestLibrary = Path.Combine (Harness.RootDirectory, "..", "msbuild", "tests", "Xamarin.iOS.Tasks.Tests", "bin", "Debug", "net461", "Xamarin.iOS.Tasks.Tests.dll"),
				TestProject = netstandard2Project,
				ProjectConfiguration = "Debug",
				Platform = TestPlatform.iOS,
				TestName = "MSBuild tests",
				Mode = "iOS",
				Timeout = TimeSpan.FromMinutes (60),
				Ignored = !jenkins.IncludeiOSMSBuild,
				SupportsParallelExecution = false,
			};
			yield return nunitExecutioniOSMSBuild;

			var installSourcesProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "..", "tools", "install-source", "InstallSourcesTests", "InstallSourcesTests.csproj")));
			var buildInstallSources = new MSBuildTask (jenkins: jenkins, testProject: installSourcesProject, processManager: processManager) {
				SpecifyPlatform = false,
				SpecifyConfiguration = false,
				Platform = TestPlatform.iOS,
			};
			buildInstallSources.SolutionPath = Path.GetFullPath (Path.Combine (Harness.RootDirectory, "..", "tools", "install-source", "install-source.sln")); // this is required for nuget restore to be executed
			var nunitExecutionInstallSource = new NUnitExecuteTask (jenkins, buildInstallSources, processManager) {
				TestLibrary = Path.Combine (Harness.RootDirectory, "..", "tools", "install-source", "InstallSourcesTests", "bin", "Release", "InstallSourcesTests.dll"),
				TestProject = installSourcesProject,
				Platform = TestPlatform.iOS,
				TestName = "Install Sources tests",
				Mode = "iOS",
				Timeout = TimeSpan.FromMinutes (60),
				Ignored = !jenkins.IncludeMac && !jenkins.IncludeSimulator,
			};
			yield return nunitExecutionInstallSource;

			var buildMTouch = new MakeTask (jenkins: jenkins, processManager: processManager) {
				TestProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "mtouch", "mtouch.sln"))),
				SpecifyPlatform = false,
				SpecifyConfiguration = false,
				Platform = TestPlatform.iOS,
				Target = "dependencies",
				WorkingDirectory = Path.GetFullPath (Path.Combine (Harness.RootDirectory, "mtouch")),
			};
			var nunitExecutionMTouch = new NUnitExecuteTask (jenkins, buildMTouch, processManager) {
				TestLibrary = Path.Combine (Harness.RootDirectory, "mtouch", "bin", "Debug", "mtouch.dll"),
				TestProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "mtouch", "mtouch.csproj"))),
				Platform = TestPlatform.iOS,
				TestName = "MTouch tests",
				Timeout = TimeSpan.FromMinutes (180),
				Ignored = !jenkins.IncludeMtouch,
				InProcess = true,
			};
			yield return nunitExecutionMTouch;

			var buildGenerator = new MakeTask (jenkins: jenkins, processManager: processManager) {
				TestProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "..", "src", "generator.sln"))),
				SpecifyPlatform = false,
				SpecifyConfiguration = false,
				Platform = TestPlatform.iOS,
				Target = "build-unit-tests",
				WorkingDirectory = Path.GetFullPath (Path.Combine (Harness.RootDirectory, "generator")),
			};
			var runGenerator = new NUnitExecuteTask (jenkins, buildGenerator, processManager) {
				TestLibrary = Path.Combine (Harness.RootDirectory, "generator", "bin", "Debug", "generator-tests.dll"),
				TestProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "generator", "generator-tests.csproj"))),
				Platform = TestPlatform.iOS,
				TestName = "Generator tests",
				Mode = "NUnit",
				Timeout = TimeSpan.FromMinutes (10),
				Ignored = !jenkins.IncludeBtouch,
			};
			yield return runGenerator;

			var buildCecilTests = new MakeTask (jenkins: jenkins, processManager: processManager) {
				Platform = TestPlatform.All,
				TestName = "Cecil",
				Target = "build",
				WorkingDirectory = Path.Combine (Harness.RootDirectory, "cecil-tests"),
				Ignored = !jenkins.IncludeCecil,
				Timeout = TimeSpan.FromMinutes (5),
			};
			var runCecilTests = new NUnitExecuteTask (jenkins, buildCecilTests, processManager) {
				TestLibrary = Path.Combine (buildCecilTests.WorkingDirectory, "bin", "Debug", "cecil-tests.dll"),
				TestProject = new TestProject (Path.Combine (buildCecilTests.WorkingDirectory, "cecil-tests.csproj")),
				Platform = TestPlatform.iOS,
				TestName = "Cecil-based tests",
				Timeout = TimeSpan.FromMinutes (5),
				Ignored = !jenkins.IncludeCecil,
				InProcess = true,
			};
			yield return runCecilTests;

			var buildSampleTestsProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "sampletester", "sampletester.sln")));
			var buildSampleTests = new MSBuildTask (jenkins: jenkins, testProject: buildSampleTestsProject, processManager: processManager) {
				SpecifyPlatform = false,
				Platform = TestPlatform.All,
				ProjectConfiguration = "Debug",
			};
			var runSampleTests = new NUnitExecuteTask (jenkins, buildSampleTests, processManager) {
				TestLibrary = Path.Combine (Harness.RootDirectory, "sampletester", "bin", "Debug", "sampletester.dll"),
				TestProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "sampletester", "sampletester.csproj"))),
				Platform = TestPlatform.All,
				TestName = "Sample tests",
				Timeout = TimeSpan.FromDays (1), // These can take quite a while to execute.
				InProcess = true,
				Ignored = true, // Ignored by default, can be run manually. On CI will execute if the label 'run-sample-tests' is present on a PR (but in Azure Devops on a different bot).
			};
			yield return runSampleTests;
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}
}
