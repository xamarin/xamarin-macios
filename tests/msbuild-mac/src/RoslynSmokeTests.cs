using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public partial class MMPTests
	{
		public string RoslynTestProjectRoot => Path.Combine (TI.FindSourceDirectory (), "TestProjects/RoslynTestApp/");

		[Test]
		public void XMModernRosylnProjet_ShouldBuildAndRunWithMSBuild ()
		{
			string projectPath = Path.Combine (RoslynTestProjectRoot, "Modern/RoslynTestApp.sln");

			TI.CleanUnifiedProject (projectPath);
			TI.BuildProject (projectPath);
			TI.RunAndAssert (Path.Combine (RoslynTestProjectRoot, "Modern/bin/Debug/RoslynTestApp.app/Contents/MacOS/RoslynTestApp"), Array.Empty<string> (), "Run");
		}

		[Test]
		public void XMFullRosylnProjet_ShouldBuildAndRunWithMSBuild ()
		{
			string projectPath = Path.Combine (RoslynTestProjectRoot, "Full/RoslynTestApp.sln");

			TI.CleanUnifiedProject (projectPath);
			TI.BuildProject (projectPath);
			TI.RunAndAssert (Path.Combine (RoslynTestProjectRoot, "Full/bin/Debug/RoslynTestApp.app/Contents/MacOS/RoslynTestApp"), Array.Empty<string> (), "Run");
		}
	}
}
