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

		void RestoreRoslynNuget (string projectPath)
		{
			TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Versions/Current/Commands/msbuild", $"/t:restore {projectPath}", "Restore Nuget");
		}

		[Test]
		public void XMModernRosylnProjet_ShouldBuildAndRunWithMSBuild ()
		{
			string projectPath = Path.Combine (RoslynTestProjectRoot, "Modern/RoslynTestApp.sln");

			TI.CleanUnifiedProject (projectPath);
			RestoreRoslynNuget (projectPath);
			TI.BuildProject (projectPath, true);
			TI.RunAndAssert (Path.Combine (RoslynTestProjectRoot, "Modern/bin/Debug/RoslynTestApp.app/Contents/MacOS/RoslynTestApp"), new StringBuilder (), "Run");
		}

		[Test]
		public void XMFullRosylnProjet_ShouldBuildAndRunWithMSBuild ()
		{
			string projectPath = Path.Combine (RoslynTestProjectRoot, "Full/RoslynTestApp.sln");

			TI.CleanUnifiedProject (projectPath);
			RestoreRoslynNuget (projectPath);
			TI.BuildProject (projectPath, true);
			TI.RunAndAssert (Path.Combine (RoslynTestProjectRoot, "Full/bin/Debug/RoslynTestApp.app/Contents/MacOS/RoslynTestApp"), new StringBuilder (), "Run");
		}
	}
}
