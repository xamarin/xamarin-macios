using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public partial class MMPTests
	{
		const string nugetPath = "/Library/Frameworks/Mono.framework/Versions/Current/Commands/nuget";

		public string RoslynTestProjectRoot => Path.Combine (TI.FindSourceDirectory (), "TestProjects/RoslynTestApp/");

		void RestoreRoslynNuget (string projectType)
		{
			StringBuilder nugetArgs = new StringBuilder ("restore " + Path.Combine (RoslynTestProjectRoot + projectType + "/") + "packages.config");

			TI.RunAndAssert (nugetPath, nugetArgs, "Restore Nuget");
		}

		[Test]
		public void XMModernRosylnProjet_ShouldBuildAndRunWithMSBuild ()
		{
			string projectPath = Path.Combine (RoslynTestProjectRoot, "Modern/RoslynTestApp.sln");

			TI.CleanUnifiedProject (projectPath);
			RestoreRoslynNuget ("Modern");
			TI.BuildProject (projectPath, true);
			TI.RunAndAssert (Path.Combine (RoslynTestProjectRoot, "Modern/bin/Debug/RoslynTestApp.app/Contents/MacOS/RoslynTestApp"), new StringBuilder (), "Run");
		}

		[Test]
		public void XMFullRosylnProjet_ShouldBuildAndRunWithMSBuild ()
		{
			string projectPath = Path.Combine (RoslynTestProjectRoot, "Full/RoslynTestApp.sln");

			TI.CleanUnifiedProject (projectPath);
			RestoreRoslynNuget ("Full");
			TI.BuildProject (projectPath, true);
			TI.RunAndAssert (Path.Combine (RoslynTestProjectRoot, "Full/bin/Debug/RoslynTestApp.app/Contents/MacOS/RoslynTestApp"), new StringBuilder (), "Run");
		}
	}
}
