using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;

using NUnit.Framework;

using Xamarin.Utils;

namespace Xamarin.Tests {
	[TestFixture]
	public class DotNetProjectTest {
		Dictionary<string, string> verbosity = new Dictionary<string, string> { { "_BundlerVerbosity", "1" } };

		string GetProjectPath (string project, string subdir = null)
		{
			var project_dir = Path.Combine (Configuration.SourceRoot, "tests", "dotnet", project);
			if (!string.IsNullOrEmpty (subdir))
				project_dir = Path.Combine (project_dir, subdir);

			var project_path = Path.Combine (project_dir, project + ".csproj");
			if (!File.Exists (project_path))
				project_path = Path.ChangeExtension (project_path, "sln");

			if (!File.Exists (project_path))
				throw new FileNotFoundException ($"Could not find the project or solution {project}");

			return project_path;
		}

		void Clean (string project_path)
		{
			var dirs = Directory.GetDirectories (Path.GetDirectoryName (project_path), "*", SearchOption.AllDirectories);
			foreach (var dir in dirs) {
				var name = Path.GetFileName (dir);
				if (name != "bin" && name != "obj")
					continue;
				Directory.Delete (dir, true);
			}
		}

		[Test]
		public void BuildMySingleView ()
		{
			var platform = ApplePlatform.iOS;
			var project_path = GetProjectPath ("MySingleView");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			AssertThatLinkerExecuted (result);
			AssertAppContents (platform, Path.Combine (Path.GetDirectoryName (project_path), "bin", "Debug", "net5.0", "ios-x64", "MySingleView.app"));
		}

		[Test]
		public void BuildMyCocoaApp ()
		{
			var platform = ApplePlatform.MacOSX;
			var project_path = GetProjectPath ("MyCocoaApp");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			AssertThatLinkerExecuted (result);
			AssertAppContents (platform, Path.Combine (Path.GetDirectoryName (project_path), "bin", "Debug", "net5.0", "osx-x64", "MyCocoaApp.app"));
		}

		[Test]
		public void BuildMyTVApp ()
		{
			var platform = ApplePlatform.TVOS;
			var project_path = GetProjectPath ("MyTVApp");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			AssertThatLinkerExecuted (result);
			AssertAppContents (platform, Path.Combine (Path.GetDirectoryName (project_path), "bin", "Debug", "net5.0", "tvos-x64", "MyTVApp.app"));
		}

		[Test]
		public void BuildMyWatchApp ()
		{
			var project_path = GetProjectPath ("MyWatchApp");
			Clean (project_path);
			var result = DotNet.AssertBuildFailure (project_path, verbosity);
			Assert.That (result.StandardOutput.ToString (), Does.Contain ("The specified RuntimeIdentifier 'watchos-x86' is not recognized."), "Missing runtime pack for watchOS");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("watchOS")]
		[TestCase ("macOS")]
		public void BuildMyClassLibrary (string platform)
		{
			var project_path = GetProjectPath ("MyClassLibrary", platform);
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			Assert.That (result.StandardOutput.ToString (), Does.Not.Contain ("Task \"ILLink\""), "Linker executed unexpectedly.");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("watchOS")]
		[TestCase ("macOS")]
		public void BuildEmbeddedResourcesTest (string platform)
		{
			var assemblyName = "EmbeddedResources";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");
			Clean (project_path);
			CopyDotNetSupportingFiles (dotnet_bindings_dir);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = result.StandardOutput.ToString ().Split ('\n');
			// Find the resulting binding assembly from the build log
			var assemblies = lines.
				Select (v => v.Trim ()).
				Where (v => {
					if (v.Length < 10)
						return false;
					if (v [0] != '/')
						return false;
					if (!v.EndsWith ($"{assemblyName}.dll", StringComparison.Ordinal))
						return false;
					if (!v.Contains ("/bin/", StringComparison.Ordinal))
						return false;
					return true;
				});
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Distinct ().Count (), Is.EqualTo (1), "Unique assemblies");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");
			// Verify that there's one resource in the assembly, and its name
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			Assert.That (ad.MainModule.Resources.Count, Is.EqualTo (1), "1 resource");
			Assert.That (ad.MainModule.Resources [0].Name, Is.EqualTo ("EmbeddedResources.Welcome.resources"), "libtest.a");
			var asm_dir = Path.GetDirectoryName (asm);
			Assert.That (Path.Combine (asm_dir, "en-AU", "EmbeddedResources.resources.dll"), Does.Exist, "en-AU");
			Assert.That (Path.Combine (asm_dir, "de", "EmbeddedResources.resources.dll"), Does.Exist, "de");
			Assert.That (Path.Combine (asm_dir, "es", "EmbeddedResources.resources.dll"), Does.Exist, "es");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("watchOS")]
		[TestCase ("macOS")]
		public void BuildFSharpLibraryTest (string platform)
		{
			var assemblyName = "fsharplibrary";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.fsproj");
			Clean (project_path);
			CopyDotNetSupportingFiles (dotnet_bindings_dir);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = result.StandardOutput.ToString ().Split ('\n');
			// Find the resulting binding assembly from the build log
			var assemblies = lines.
				Select (v => v.Trim ()).
				Where (v => {
					if (v.Length < 10)
						return false;
					if (v [0] != '/')
						return false;
					if (!v.EndsWith ($"{assemblyName}.dll", StringComparison.Ordinal))
						return false;
					if (!v.Contains ("/bin/", StringComparison.Ordinal))
						return false;
					return true;
				});
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Distinct ().Count (), Is.EqualTo (1), "Unique assemblies");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");
			// Verify that there's no resources in the assembly
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			Assert.That (ad.MainModule.Resources.Count (), Is.EqualTo (2), "2 resources"); // There are 2 embedded resources by default by the F# compiler.
		}

		[TestCase ("iOS")]
		[TestCase ("macOS")]
		public void BuildBindingsTest (string platform)
		{
			var assemblyName = "bindings-test";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");

			Clean (project_path);
			CopyDotNetSupportingFiles (dotnet_bindings_dir);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = result.StandardOutput.ToString ().Split ('\n');
			// Find the resulting binding assembly from the build log
			var assemblies = lines.
				Select (v => v.Trim ()).
				Where (v => {
					if (v.Length < 10)
						return false;
					if (v [0] != '/')
						return false;
					if (!v.EndsWith ($"{assemblyName}.dll", StringComparison.Ordinal))
						return false;
					if (!v.Contains ("/bin/", StringComparison.Ordinal))
						return false;
					return true;
				});
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Distinct ().Count (), Is.EqualTo (1), "Unique assemblies");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");

			// Verify that there's one resource in the binding assembly, and its name
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			Assert.That (ad.MainModule.Resources.Count, Is.EqualTo (1), "1 resource");
			Assert.That (ad.MainModule.Resources [0].Name, Is.EqualTo ("libtest.a"), "libtest.a");
		}

		void CopyDotNetSupportingFiles (string targetDirectory)
		{
			var srcDirectory = Path.Combine (Configuration.SourceRoot, "tests", "dotnet");
			foreach (var fn in new string [] { "global.json", "NuGet.config" })
				File.Copy (Path.Combine (srcDirectory, fn), Path.Combine (targetDirectory, fn), true);
		}

		void AssertThatLinkerExecuted (ExecutionResult result)
		{
			var output = result.StandardOutput.ToString ();
			Assert.That (output, Does.Contain ("Building target \"_RunILLink\" completely."), "Linker did not executed as expected.");
			Assert.That (output, Does.Contain ("Pipeline Steps:"), "Custom steps did not run as expected.");
		}

		void AssertAppContents (ApplePlatform platform, string app_directory)
		{
			string info_plist_path;
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				info_plist_path = Path.Combine (app_directory, "Info.plist");
				break;
			case ApplePlatform.MacOSX:
				info_plist_path = Path.Combine (app_directory, "Contents", "Info.plist");
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
			Assert.That (info_plist_path, Does.Exist, "Info.plist");
		}
	}
}
