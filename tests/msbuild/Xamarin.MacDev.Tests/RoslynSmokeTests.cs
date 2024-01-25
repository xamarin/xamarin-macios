using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MMP.Tests {
	[TestFixture]
	public partial class MMPTests {
		public string RoslynTestProjectRoot => Path.Combine (Configuration.TestProjectsDirectory, "RoslynTestApp");

		[Test]
		public void XMModernRoslynProject_ShouldBuildAndRunWithMSBuild ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			string projectPath = Path.Combine (RoslynTestProjectRoot, "Modern/RoslynTestApp.sln");

			TI.CleanUnifiedProject (projectPath);
			TI.BuildProject (projectPath);
			TI.RunAndAssert (Path.Combine (RoslynTestProjectRoot, "Modern/bin/Debug/RoslynTestApp.app/Contents/MacOS/RoslynTestApp"), Array.Empty<string> (), "Run");
		}

		[Test]
		public void XMFullRoslynProject_ShouldBuildAndRunWithMSBuild ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			string projectPath = Path.Combine (RoslynTestProjectRoot, "Full/RoslynTestApp.sln");

			TI.CleanUnifiedProject (projectPath);
			TI.BuildProject (projectPath);
			TI.RunAndAssert (Path.Combine (RoslynTestProjectRoot, "Full/bin/Debug/RoslynTestApp.app/Contents/MacOS/RoslynTestApp"), Array.Empty<string> (), "Run");
		}
	}
}
