using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public class Issue3199
	{
		public string TestProjectRoot => Path.Combine (TI.FindSourceDirectory (), "TestProjects");

		[Test]
		public void BuildTest ()
		{
			var projectName = "Issue3199";
			var projectPath = Path.Combine (TestProjectRoot, projectName, $"{projectName}.csproj");

			TI.CleanUnifiedProject (projectPath);
			TI.BuildProject (projectPath, true);
		}
	}
}
