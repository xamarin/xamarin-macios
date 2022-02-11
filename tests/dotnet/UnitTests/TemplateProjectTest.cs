using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using NUnit.Framework;

using Xamarin.Utils;

using Microsoft.Build.Framework;
using Microsoft.Build.Logging.StructuredLogger;

namespace Xamarin.Tests {
	public class TemplateProjectTest : TestBaseClass {
		const string EmptyAppManifest =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
<key>CFBundleIdentifier</key>
<string>ID</string>
</dict>
</plist>";

		const string EmptyMainFile =
@"using System;
using Foundation;

class MainClass {
	static void Main ()
	{
		Console.WriteLine (typeof (NSObject));
	}
}
";

		[TestCase (ApplePlatform.MacOSX)]
		public void CreateAppBundleDependsOnTest (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var magic = Guid.NewGuid ().ToString ();
			var csproj = $@"<Project Sdk=""Microsoft.NET.Sdk"">
<PropertyGroup>
	<TargetFramework>{platform.ToFramework ()}</TargetFramework>
	<OutputType>Exe</OutputType>
	<CreateAppBundleDependsOn>FailTheBuild;$(CreateAppBundleDependsOn)</CreateAppBundleDependsOn>
	</PropertyGroup>
	<Target Name=""FailTheBuild"">
		<Error Text=""{magic}"" />
	</Target>
</Project>";

			var tmpdir = Cache.CreateTemporaryDirectory ();
			Configuration.CopyDotNetSupportingFiles (tmpdir);
			var project_path = Path.Combine (tmpdir, "TestProject.csproj");
			File.WriteAllText (project_path, csproj);
			File.WriteAllText (Path.Combine (tmpdir, "Info.plist"), EmptyAppManifest);
			File.WriteAllText (Path.Combine (tmpdir, "Main.cs"), EmptyMainFile);

			var properties = GetDefaultProperties ();
			var result = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (result.BinLogPath);
			Assert.That (errors, Has.Some.Matches<BuildLogEvent> (v => v.Message.Contains (magic)), "Expected error");
		}

		// https://github.com/xamarin/xamarin-macios/issues/13503
		[TestCase (ApplePlatform.MacOSX, null)]
		public void NativeReferenceStaticLibraryForceLoad (ApplePlatform platform, bool? simulator)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var magic = Guid.NewGuid ().ToString ();
			var pathToNativeLibrary = Path.Combine (Configuration.GetTestLibraryDirectory (platform, simulator), "libtest.a");
			var relativePathToNativeLibrary = Path.GetRelativePath (tmpdir, pathToNativeLibrary);
			var csproj = $@"<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>{platform.ToFramework ()}</TargetFramework>
		<OutputType>Exe</OutputType>
	</PropertyGroup>
	<ItemGroup>
		<NativeReference Include=""{relativePathToNativeLibrary}"">
			<Kind>Static</Kind>
			<ForceLoad>True</ForceLoad>
		</NativeReference>
	</ItemGroup>
</Project>";

			Configuration.CopyDotNetSupportingFiles (tmpdir);
			var project_path = Path.Combine (tmpdir, "TestProject.csproj");
			File.WriteAllText (project_path, csproj);
			File.WriteAllText (Path.Combine (tmpdir, "Info.plist"), EmptyAppManifest);
			File.WriteAllText (Path.Combine (tmpdir, "Main.cs"), EmptyMainFile);

			DotNet.AssertBuild (project_path, GetDefaultProperties ());
		}
	}
}
