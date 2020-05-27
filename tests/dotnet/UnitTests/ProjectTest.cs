using System.IO;

using NUnit.Framework;

namespace Xamarin.Tests {
	[TestFixture]
	public class DotNetProjectTest {
		void Build (string project, string subdir = null)
		{
			var project_dir = Path.Combine (Configuration.SourceRoot, "tests", "dotnet", project);
			if (!string.IsNullOrEmpty (subdir))
				project_dir = Path.Combine (project_dir, subdir);

			var project_path = Path.Combine (project_dir, project + ".csproj");
			if (!File.Exists (project_path))
				project_path = Path.ChangeExtension (project_path, "sln");

			DotNet.AssertBuild (project_path);
		}

		[Test]
		public void BuildMySingleView ()
		{
			Build ("MySingleView");
		}

		[Test]
		public void BuildMyCocoaApp ()
		{
			Build ("MyCocoaApp");
		}

		[Test]
		public void BuildMyTVApp ()
		{
			Build ("MyTVApp");
		}

		[Test]
		public void BuildMyWatchApp ()
		{
			Build ("MyWatchApp");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("watchOS")]
		[TestCase ("macOS")]
		public void BuildMyClassLibrary (string platform)
		{
			Build ("MyClassLibrary", platform);
		}
	}
}
