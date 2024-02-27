using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;
using Xamarin.MacDev;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class CollectAppManifestsTests {
		[Test]
		public void PartialAppManifest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertDotNetAvailable ();

			var csproj = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>{Configuration.DotNetTfm}-macos</TargetFramework>
		<RuntimeIdentifier>osx-x64</RuntimeIdentifier>
		<OutputType>Exe</OutputType>

		<CollectAppManifestsDependsOn>
			$(CollectAppManifestsDependsOn);
			AddPartialManifests;
		</CollectAppManifestsDependsOn>
    </PropertyGroup>

	<Target Name=""AddPartialManifests"">
		<ItemGroup>
			<PartialAppManifest Include=""MyPartialManifest.plist"" />
		</ItemGroup>
	</Target>
</Project>";

			var partialPList = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
        <key>CFBundleDisplayName</key>
        <string>PartialAppManifestDisplayName</string>
        <key>CFBundleIdentifier</key>
        <string>com.xamarin.partialappmanifest</string>
</dict>
</plist>";

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var csprojPath = Path.Combine (tmpdir, "PartialAppManifest.csproj");
			File.WriteAllText (csprojPath, csproj);

			// Create an empty main app manifest
			var mainPListPath = Path.Combine (tmpdir, "Info.plist");
			new PDictionary ().Save (mainPListPath);

			// Save our custom partial app manifest
			var partialPListPath = Path.Combine (tmpdir, "MyPartialManifest.plist");
			File.WriteAllText (partialPListPath, partialPList);

			var engine = new BuildEngine ();
			var properties = new Dictionary<string, string> {
				{ "_CreateAppManifest", "true" },
			};
			var rv = engine.RunTarget (ApplePlatform.MacOSX, ExecutionMode.DotNet, csprojPath, target: "_WriteAppManifest", properties: properties);
			Assert.AreEqual (0, rv.ExitCode, "Exit code");

			var appManifestPath = Path.Combine (tmpdir, "bin", "Debug", Configuration.DotNetTfm + "-macos", "osx-x64", "PartialAppManifest.app", "Contents", "Info.plist");
			Assert.That (appManifestPath, Does.Exist, "App manifest existence");

			var plist = PDictionary.FromFile (appManifestPath);
			Assert.AreEqual ("PartialAppManifestDisplayName", plist.GetCFBundleDisplayName (), "Bundle display name");
			Assert.AreEqual ("com.xamarin.partialappmanifest", plist.GetCFBundleIdentifier (), "Bundle identifier");
		}
	}
}

