using System.IO;

using NUnit.Framework;

namespace Xamarin.Tests {
	[TestFixture]
	public class DotNetProjectTest {
		void Build (string project)
		{
			var project_path = Path.Combine (Configuration.SourceRoot, "tests", "dotnet", project, project + ".csproj");
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
	}
}
