using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public class RuntimeTests
	{
		[Test]
		public void AssemblyRegistration ()
		{
			var projectName = "AssemblyRegistration";
			var projectPath = Path.Combine (Configuration.TestProjectsDirectory, projectName, $"{projectName}.csproj");

			TI.CleanUnifiedProject (projectPath);
			TI.BuildProject (projectPath);
			TI.RunAndAssert (Path.Combine (Path.GetDirectoryName (projectPath), $"bin/Debug/{projectName}.app/Contents/MacOS/{projectName}"), Array.Empty<string> (), "Run");
		}
	}
}
