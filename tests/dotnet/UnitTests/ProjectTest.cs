using System.IO;

using NUnit.Framework;

namespace Xamarin.Tests {
	[TestFixture]
	public class DotNetProjectTest {
		string GetProjectPath (string project, string subdir = null)
		{
			var project_dir = Path.Combine (Configuration.SourceRoot, "tests", "dotnet", project);
			if (!string.IsNullOrEmpty (subdir))
				project_dir = Path.Combine (project_dir, subdir);

			var project_path = Path.Combine (project_dir, project + ".csproj");
			if (!File.Exists (project_path))
				project_path = Path.ChangeExtension (project_path, "sln");

			if (!File.Exists (project_path))
				throw new FileNotFoundException ($"Could not find the project or solution {project}");

			return project_path;
		}

		void Clean (string project_path)
		{
			var dirs = Directory.GetDirectories (Path.GetDirectoryName (project_path), "*", SearchOption.AllDirectories);
			foreach (var dir in dirs) {
				var name = Path.GetFileName (dir);
				if (name != "bin" && name != "obj")
					continue;
				Directory.Delete (dir, true);
			}
		}

		[Test]
		public void BuildMySingleView ()
		{
			var project_path = GetProjectPath ("MySingleView");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path);
			AssertThatLinkerExecuted (result);
		}

		[Test]
		public void BuildMyCocoaApp ()
		{
			var project_path = GetProjectPath ("MyCocoaApp");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path);
			AssertThatLinkerExecuted (result);
		}

		[Test]
		public void BuildMyTVApp ()
		{
			var project_path = GetProjectPath ("MyTVApp");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path);
			AssertThatLinkerExecuted (result);
		}

		[Test]
		public void BuildMyWatchApp ()
		{
			var project_path = GetProjectPath ("MyWatchApp");
			Clean (project_path);
			var result = DotNet.AssertBuildFailure (project_path);
			Assert.That (result.StandardOutput.ToString (), Does.Contain ("The specified RuntimeIdentifier 'watchos-x86' is not recognized."), "Missing runtime pack for watchOS");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("watchOS")]
		[TestCase ("macOS")]
		public void BuildMyClassLibrary (string platform)
		{
			var project_path = GetProjectPath ("MyClassLibrary", platform);
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path);
			Assert.That (result.StandardOutput.ToString (), Does.Not.Contain ("Task \"ILLink\""), "Linker executed unexpectedly.");
		}

		void AssertThatLinkerExecuted (ExecutionResult result)
		{
			var output = result.StandardOutput.ToString ();
			Assert.That (output, Does.Contain ("Building target \"_RunILLink\" completely."), "Linker did not executed as expected.");
			Assert.That (output, Does.Contain ("Hello SetupStep"), "Custom steps did not run as expected.");
		}
	}
}
