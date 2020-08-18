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
			AssertAppContents (platform, Path.Combine (Path.GetDirectoryName (project_path), "bin", "Debug", "net5.0-ios", "ios-x64", "MySingleView.app"));
		}

		[Test]
		public void BuildMyCocoaApp ()
		{
			var platform = ApplePlatform.MacOSX;
			var project_path = GetProjectPath ("MyCocoaApp");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			AssertThatLinkerExecuted (result);
			AssertAppContents (platform, Path.Combine (Path.GetDirectoryName (project_path), "bin", "Debug", "net5.0-macos", "osx-x64", "MyCocoaApp.app"));
		}

		[Test]
		public void BuildMyTVApp ()
		{
			var platform = ApplePlatform.TVOS;
			var project_path = GetProjectPath ("MyTVApp");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			AssertThatLinkerExecuted (result);
			AssertAppContents (platform, Path.Combine (Path.GetDirectoryName (project_path), "bin", "Debug", "net5.0-tvos", "tvos-x64", "MyTVApp.app"));
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
			var assemblies = FilterToAssembly (lines, assemblyName);
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			assemblies = assemblies.Distinct ();
			Assert.That (assemblies.Count (), Is.EqualTo (1), $"Unique assemblies\n\t{string.Join ("\n\t", assemblies)}");
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
			var assemblies = FilterToAssembly (lines, assemblyName);
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
		[TestCase ("tvOS")]
		[TestCase ("watchOS")]
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
			var assemblies = FilterToAssembly (lines, assemblyName);
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

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("watchOS")]
		[TestCase ("macOS")]
		public void BuildBindingsTest2 (string platform)
		{
			var assemblyName = "bindings-test2";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");

			Clean (project_path);
			CopyDotNetSupportingFiles (dotnet_bindings_dir);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = result.StandardOutput.ToString ().Split ('\n');
			// Find the resulting binding assembly from the build log
			var assemblies = FilterToAssembly (lines, assemblyName);
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Distinct ().Count (), Is.EqualTo (1), "Unique assemblies");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");

			// Verify that there's one resource in the binding assembly, and its name
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			Assert.That (ad.MainModule.Resources.Count, Is.EqualTo (1), "1 resource");
			Assert.That (ad.MainModule.Resources [0].Name, Is.EqualTo ("libtest2.a"), "libtest2.a");
		}

		[TestCase ("iOS", "monotouch")]
		[TestCase ("tvOS", "monotouch")]
		[TestCase ("watchOS", "monotouch")]
		[TestCase ("macOS", "xammac")]
		public void BuildBundledResources (string platform, string prefix)
		{
			var assemblyName = "BundledResources";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");

			Clean (project_path);
			CopyDotNetSupportingFiles (dotnet_bindings_dir);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = result.StandardOutput.ToString ().Split ('\n');
			// Find the resulting binding assembly from the build log
			var assemblies = FilterToAssembly (lines, assemblyName);
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Distinct ().Count (), Is.EqualTo (1), "Unique assemblies");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");

			// Verify that there's one resource in the binding assembly, and its name
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			Assert.That (ad.MainModule.Resources.Count, Is.EqualTo (2), "2 resources");
			// Sort the resources before we assert, since we don't care about the order, and sorted order makes the asserts simpler.
			var resources = ad.MainModule.Resources.OrderBy (v => v.Name).ToArray ();
			Assert.That (resources [0].Name, Is.EqualTo ($"__{prefix}_content_basn3p08.png"), $"__{prefix}_content_basn3p08.png");
			Assert.That (resources [1].Name, Is.EqualTo ($"__{prefix}_content_xamvideotest.mp4"), $"__{prefix}_content_xamvideotest.mp4");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		// [TestCase ("watchOS")] // No watchOS Touch.Client project for .NET yet
		// [TestCase ("macOS")] // No macOS Touch.Client project for .NET yet
		public void BuildInterdependentBindingProjects (string platform)
		{
			var assemblyName = "interdependent-binding-projects";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");

			Clean (project_path);
			CopyDotNetSupportingFiles (dotnet_bindings_dir);
			CopyDotNetSupportingFiles (dotnet_bindings_dir.Replace (assemblyName, "bindings-test"));
			CopyDotNetSupportingFiles (dotnet_bindings_dir.Replace (assemblyName, "bindings-test2"));
			var cleanupSupportFiles = CopyDotNetSupportingFiles (Path.Combine (Configuration.SourceRoot, "external", "Touch.Unit", "Touch.Client/dotnet"));
			try {
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
						if (!v.Contains ($"{assemblyName}.app", StringComparison.Ordinal))
							return false;
						return true;
					});
				Assert.That (assemblies, Is.Not.Empty, "Assemblies");
				// Make sure there's no other assembly confusing our logic
				assemblies = assemblies.Distinct ();
				Assert.That (assemblies.Count (), Is.EqualTo (1), $"Unique assemblies: {string.Join (", ", assemblies)}");
				var asm = assemblies.First ();
				Assert.That (asm, Does.Exist, "Assembly existence");

				// Verify that the resources
				var asmDir = Path.GetDirectoryName (asm);
				var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
				Assert.That (ad.MainModule.Resources.Count, Is.EqualTo (0), "0 resources for interdependent-binding-projects.dll");

				var ad1 = AssemblyDefinition.ReadAssembly (Path.Combine (asmDir, "bindings-test.dll"), new ReaderParameters { ReadingMode = ReadingMode.Deferred });
				Assert.That (ad1.MainModule.Resources.Count, Is.EqualTo (1), "1 resource for bindings-test.dll");
				Assert.That (ad1.MainModule.Resources [0].Name, Is.EqualTo ("libtest.a"), "libtest.a - bindings-test.dll");

				var ad2 = AssemblyDefinition.ReadAssembly (Path.Combine (asmDir, "bindings-test2.dll"), new ReaderParameters { ReadingMode = ReadingMode.Deferred });
				Assert.That (ad2.MainModule.Resources.Count, Is.EqualTo (1), "1 resource for bindings-test2.dll");
				Assert.That (ad2.MainModule.Resources [0].Name, Is.EqualTo ("libtest2.a"), "libtest2.a - bindings-test2.dll");
			} finally {
				foreach (var file in cleanupSupportFiles)
					File.Delete (file);
			}
		}

		string[] CopyDotNetSupportingFiles (string targetDirectory)
		{
			var srcDirectory = Path.Combine (Configuration.SourceRoot, "tests", "dotnet");
			var files = new string [] { "global.json", "NuGet.config" };
			var targets = new string [files.Length];
			for (var i = 0; i < files.Length; i++) {
				var fn = files [i];
				targets [i] = Path.Combine (targetDirectory, fn);
				File.Copy (Path.Combine (srcDirectory, fn), targets [i], true);
			}
			return targets;
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

		IEnumerable<string> FilterToAssembly (IEnumerable<string> lines, string assemblyName)
		{
			return lines.
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
					if (v.Contains ("/ref/", StringComparison.Ordinal))
						return false; // Skip reference assemblies
					return true;
				});
		}
	}
}
