using System;
using System.IO;
using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.iOS.Tasks
{
	// https://github.com/xamarin/xamarin-macios/issues/4110
	// [TestFixture ("iPhone")]
	// [TestFixture ("iPhoneSimulator")]
	public class ProjectReferenceTests : ProjectTest
	{

		public ProjectReferenceTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			// We set MSBuildExtensionsPath when building with xbuild to redirect to our locally build XI/XM, but that confuses MSBuild, which uses MSBuildExtensionsPathFallbackPathsOverride instead.
			// So if MSBuildExtensionsPath is set, move the value temporarily to MSBuildExtensionsPathFallbackPathsOverride instead, since we're using MSBuild here.
			// This will become unnecessary when PR #4111 is merged.
			var msbuildExtensions = Environment.GetEnvironmentVariable ("MSBuildExtensionsPath");
			if (!string.IsNullOrEmpty (msbuildExtensions)) {
				Environment.SetEnvironmentVariable ("MSBuildExtensionsPath", null);
				Environment.SetEnvironmentVariable ("MSBuildExtensionsPathFallbackPathsOverride", msbuildExtensions);
			}
			try {
				NugetRestore ("../MyAppWithPackageReference/MyAppWithPackageReference.csproj");
				NugetRestore ("../MyExtensionWithPackageReference/MyExtensionWithPackageReference.csproj");

				// Can't use the in-process MSBuild engine, because it complains that the project file is invalid (the attribute 'Version' in the element '<PackageReference>' is unrecognized)
				var rv = ExecutionHelper.Execute ("msbuild", $"../MyAppWithPackageReference/MyAppWithPackageReference.csproj /p:Platform={Platform} /p:Configuration=Debug", out var output);
				if (rv != 0) {
					Console.WriteLine ("Build failed:");
					Console.WriteLine (output);
					Assert.Fail ("Build failed");
				}
			} finally {
				if (!string.IsNullOrEmpty (msbuildExtensions)) {
					Environment.SetEnvironmentVariable ("MSBuildExtensionsPath", msbuildExtensions);
					Environment.SetEnvironmentVariable ("MSBuildExtensionsPathFallbackPathsOverride", null);
				}
			}
		}
	}
}

