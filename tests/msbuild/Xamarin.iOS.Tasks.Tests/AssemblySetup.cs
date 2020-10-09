using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Xamarin.Tests;

[SetUpFixture]
public class AssemblySetup {
	[OneTimeSetUp]
	public void AssemblyInitialization ()
	{
		// Seems like the ServiceHubUniqueLogDir environment variable is set when executing unit tests from within VS
		var is_in_vsmac = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("ServiceHubUniqueLogDir"));
		const string msbuild_exe_path = "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/msbuild/15.0/bin/MSBuild.dll";
		if (is_in_vsmac) {
			var env = new Dictionary<string, string> {
				{ "MD_APPLE_SDK_ROOT", Path.GetDirectoryName (Path.GetDirectoryName (Configuration.xcode_root)) },
				{ "TargetFrameworkFallbackSearchPaths", Path.Combine (Configuration.TargetDirectoryXI, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks") },
				{ "MSBuildExtensionsPathFallbackPathsOverride", Path.Combine (Configuration.TargetDirectoryXI, "Library", "Frameworks", "Mono.framework", "External", "xbuild") },
				{ "MD_MTOUCH_SDK_ROOT", Path.Combine (Configuration.TargetDirectoryXI, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current") },
				{ "MSBUILD_EXE_PATH", msbuild_exe_path },
			};

			foreach (var kvp in env)
				Environment.SetEnvironmentVariable (kvp.Key, kvp.Value);

			Console.WriteLine ($"Detected that we're running inside Visual Studio, and the environment has been configured.");
		} else {
			Console.WriteLine ("Detected that we're not running inside Visual Studio, and thus will not set MSBUILD_EXE_PATH.");
		}
	}
}
