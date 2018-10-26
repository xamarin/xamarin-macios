using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public class RuntimeTests
	{
		public string TestProjectRoot => Path.Combine (TI.FindSourceDirectory (), "TestProjects");

		[Test]
		public void AssemblyRegistration ()
		{
			var projectName = "AssemblyRegistration";
			var projectPath = Path.Combine (TestProjectRoot, projectName, $"{projectName}.csproj");

			TI.CleanUnifiedProject (projectPath);
			TI.BuildProject (projectPath, true);
			TI.RunAndAssert (Path.Combine (Path.GetDirectoryName (projectPath), $"bin/Debug/{projectName}.app/Contents/MacOS/{projectName}"), new StringBuilder (), "Run");
		}
	}
}
