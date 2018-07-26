using System;
using System.IO;
using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class ProjectReferenceTests : ProjectTest
	{

		public ProjectReferenceTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			NugetRestore ("../MyAppWithPackageReference/MyAppWithPackageReference.csproj");
			NugetRestore ("../MyExtensionWithPackageReference/MyExtensionWithPackageReference.csproj");

			// Can't use the in-process MSBuild engine, because it complains that the project file is invalid (the attribute 'Version' in the element '<PackageReference>' is unrecognized)
			var rv = ExecutionHelper.Execute ("msbuild", $"../MyAppWithPackageReference/MyAppWithPackageReference.csproj /p:Platform={Platform} /p:Configuration=Debug", out var output);
			if (rv != 0) {
				Console.WriteLine ("Build failed:");
				Console.WriteLine (output);
				Assert.Fail ("Build failed");
			}
		}
	}
}

