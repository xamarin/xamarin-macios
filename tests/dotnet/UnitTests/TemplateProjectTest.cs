using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Logging.StructuredLogger;

#nullable enable

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
			var project_path = Path.Combine (tmpdir, "TestProject.csproj");
			File.WriteAllText (project_path, csproj);
			File.WriteAllText (Path.Combine (tmpdir, "Info.plist"), EmptyAppManifest);
			File.WriteAllText (Path.Combine (tmpdir, "Main.cs"), EmptyMainFile);

			var properties = GetDefaultProperties ();
			var result = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (result.BinLogPath);
			Assert.That (errors, Has.Some.Matches<BuildLogEvent> (v => v?.Message?.Contains (magic) == true), "Expected error");
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

			var project_path = Path.Combine (tmpdir, "TestProject.csproj");
			File.WriteAllText (project_path, csproj);
			File.WriteAllText (Path.Combine (tmpdir, "Info.plist"), EmptyAppManifest);
			File.WriteAllText (Path.Combine (tmpdir, "Main.cs"), EmptyMainFile);

			DotNet.AssertBuild (project_path, GetDefaultProperties ());
		}

		[Test]
		[TestCase (ApplePlatform.MacOSX)]
		[TestCase (ApplePlatform.MacCatalyst)]
		public void VeryManyRegisteredAssemblies (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var assemblies = 200;

			var xm = Configuration.XamarinMacMobileDll;
			var csc = Configuration.DotNetCscCommand;

			var references = new List<string> ();

			for (var i = 0; i < assemblies; i++) {
				var refprojdir = Path.Combine (tmpdir, $"Project{i}");
				Directory.CreateDirectory (refprojdir);
				var refcs = $"public class C{i} : Foundation.NSObject {{}}";
				var reffn = Path.Combine (refprojdir, $"C{i}.cs");
				File.WriteAllText (reffn, refcs);
				var refcsproj =
@$"<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>{platform.ToFramework ()}</TargetFramework>
	</PropertyGroup>
</Project>";
				var refcsprojfn = Path.Combine (refprojdir, Path.GetFileName (refprojdir) + ".csproj");
				File.WriteAllText (refcsprojfn, refcsproj);

				references.Add (refcsprojfn);
			}

			var projdir = Path.Combine (tmpdir, $"Main");
			Directory.CreateDirectory (projdir);

			var cs = new StringBuilder ();
			cs.AppendLine ("using System;");
			cs.AppendLine ("public class MainClass {");
			cs.AppendLine ("\tstatic void Main () {");
			for (var i = 0; i < assemblies; i++)
				cs.AppendLine ($"\t\tConsole.WriteLine (new C{i} ().Handle);");
			cs.AppendLine ($"\t\tConsole.WriteLine (Environment.GetEnvironmentVariable (\"MAGIC_WORD\"));");
			cs.AppendLine ("\t}");
			cs.AppendLine ("}");
			var fn = Path.Combine (projdir, $"Main.cs");
			File.WriteAllText (fn, cs.ToString ());

			var csproj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>{platform.ToFramework ()}</TargetFramework>
		<OutputType>Exe</OutputType>
		<ApplicationId>com.microsoft.VeryManyRegisteredAssemblies</ApplicationId>
		<MonoBundlingExtraArgs>--registrar:static --optimize:remove-dynamic-registrar</MonoBundlingExtraArgs>
		<MtouchExtraArgs>$(MonoBundlingExtraArgs)</MtouchExtraArgs>
		<LinkMode>Full</LinkMode>
		<MtouchLink>Full</MtouchLink>
	</PropertyGroup>
	<ItemGroup>
		{string.Join ("\n\t\t", references.Select (v => $"<ProjectReference Include=\"{v}\" />"))}
	</ItemGroup>
</Project>";
			var csprojfn = Path.Combine (projdir, Path.GetFileName (projdir) + ".csproj");
			File.WriteAllText (csprojfn, csproj);

			DotNet.AssertBuild (csprojfn);
			ExecuteProjectWithMagicWordAndAssert (csprojfn, platform);
		}

		[TestCase (ApplePlatform.MacOSX)]
		public void NoBundleIdentifierError (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var csproj = $@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>{platform.ToFramework ()}</TargetFramework>
		<OutputType>Exe</OutputType>
	</PropertyGroup>
</Project>";

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var project_path = Path.Combine (tmpdir, "TestProject.csproj");
			File.WriteAllText (project_path, csproj);
			File.WriteAllText (Path.Combine (tmpdir, "Main.cs"), EmptyMainFile);

			var properties = GetDefaultProperties ();
			var result = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (result.BinLogPath).ToArray ();
			Assert.AreEqual (1, errors.Length, "Errors");
			Assert.AreEqual ("A bundle identifier is required. Either add an 'ApplicationId' property in the project file, or add a 'CFBundleIdentifier' entry in the project's Info.plist file.", errors [0].Message);
		}
	}
}
