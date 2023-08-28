using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Xml;

using Mono.Cecil;

using Xamarin.Tests;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class DotNetProjectTest : TestBaseClass {
		[Test]
		[TestCase (null)]
		[TestCase ("iossimulator-x86")]
		[TestCase ("iossimulator-x64")]
		[TestCase ("iossimulator-arm64")]
		[TestCase ("ios-arm64")]
		[TestCase ("ios-arm")]
		public void BuildMySingleView (string runtimeIdentifier)
		{
			var platform = ApplePlatform.iOS;
			var project_path = GetProjectPath ("MySingleView", runtimeIdentifiers: runtimeIdentifier, platform: platform, out var appPath);
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			var result = DotNet.AssertBuild (project_path, properties);
			AssertThatLinkerExecuted (result);
			AssertAppContents (platform, appPath);
			var infoPlistPath = Path.Combine (appPath, "Info.plist");
			var infoPlist = PDictionary.FromFile (infoPlistPath)!;
			Assert.AreEqual ("com.xamarin.mysingletitle", infoPlist.GetString ("CFBundleIdentifier").Value, "CFBundleIdentifier");
			Assert.AreEqual ("MySingleTitle", infoPlist.GetString ("CFBundleDisplayName").Value, "CFBundleDisplayName");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleVersion").Value, "CFBundleVersion");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleShortVersionString").Value, "CFBundleShortVersionString");
		}

		[Test]
		[TestCase (null)]
		[TestCase ("osx-x64")]
		[TestCase ("osx-arm64")]
		public void BuildMyCocoaApp (string runtimeIdentifier)
		{
			var platform = ApplePlatform.MacOSX;
			var project_path = GetProjectPath ("MyCocoaApp", runtimeIdentifiers: runtimeIdentifier, platform: platform, out var appPath);
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			var result = DotNet.AssertBuild (project_path, properties);
			AssertThatLinkerExecuted (result);
			AssertAppContents (platform, appPath);
		}

		[Test]
		[TestCase (null)]
		[TestCase ("tvossimulator-x64")]
		[TestCase ("tvossimulator-arm64")]
		[TestCase ("tvos-arm64")]
		public void BuildMyTVApp (string runtimeIdentifier)
		{
			var platform = ApplePlatform.TVOS;
			var project_path = GetProjectPath ("MyTVApp", runtimeIdentifiers: runtimeIdentifier, platform: platform, out var appPath);
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			var result = DotNet.AssertBuild (project_path, properties);
			AssertThatLinkerExecuted (result);
			AssertAppContents (platform, appPath);
		}

		[Test]
		[TestCase (null)]
		[TestCase ("maccatalyst-x64")]
		[TestCase ("maccatalyst-arm64")]
		public void BuildMyCatalystApp (string runtimeIdentifier)
		{
			var platform = ApplePlatform.MacCatalyst;
			var project_path = GetProjectPath ("MyCatalystApp", runtimeIdentifiers: runtimeIdentifier, platform: platform, out var appPath);
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			var result = DotNet.AssertBuild (project_path, properties);
			AssertThatLinkerExecuted (result);
			AssertAppContents (platform, appPath);
			var infoPlistPath = Path.Combine (appPath, "Contents", "Info.plist");
			var infoPlist = PDictionary.FromFile (infoPlistPath)!;
			Assert.AreEqual ("com.xamarin.mycatalystapp", infoPlist.GetString ("CFBundleIdentifier").Value, "CFBundleIdentifier");
			Assert.AreEqual ("MyCatalystApp", infoPlist.GetString ("CFBundleDisplayName").Value, "CFBundleDisplayName");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleVersion").Value, "CFBundleVersion");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleShortVersionString").Value, "CFBundleShortVersionString");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("macOS")]
		[TestCase ("MacCatalyst")]
		[Category ("AllPlatforms")]
		public void BuildMyClassLibrary (string platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var project_path = GetProjectPath ("MyClassLibrary", platform);
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			Assert.That (result.StandardOutput.ToString (), Does.Not.Contain ("Task \"ILLink\""), "Linker executed unexpectedly.");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("macOS")]
		[TestCase ("MacCatalyst")]
		[Category ("AllPlatforms")]
		public void BuildEmbeddedResourcesTest (string platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var assemblyName = "EmbeddedResources";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = BinLog.PrintToLines (result.BinLogPath);
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
			var asm_dir = Path.GetDirectoryName (asm)!;
			Assert.That (Path.Combine (asm_dir, "en-AU", "EmbeddedResources.resources.dll"), Does.Exist, "en-AU");
			Assert.That (Path.Combine (asm_dir, "de", "EmbeddedResources.resources.dll"), Does.Exist, "de");
			Assert.That (Path.Combine (asm_dir, "es", "EmbeddedResources.resources.dll"), Does.Exist, "es");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("macOS")]
		[TestCase ("MacCatalyst")]
		[Category ("AllPlatforms")]
		public void BuildFSharpLibraryTest (string platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var assemblyName = "fsharplibrary";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.fsproj");
			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = BinLog.PrintToLines (result.BinLogPath);
			// Find the resulting binding assembly from the build log
			var assemblies = FilterToAssembly (lines, assemblyName).Distinct ();
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Count (), Is.EqualTo (1), $"Unique assemblies:\n\t{string.Join ("\n\t", assemblies)}");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");
			// Verify that there's no resources in the assembly
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			Assert.That (ad.MainModule.Resources.Count (), Is.EqualTo (2), "2 resources"); // There are 2 embedded resources by default by the F# compiler.
		}

		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[Category ("AllPlatforms")]
		public void BuildBindingsTest (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var assemblyName = "bindings-test";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform.AsString ());
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");

			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = BinLog.PrintToLines (result.BinLogPath).ToList ();
			Console.WriteLine (string.Join ("\n", lines));
			// Find the resulting binding assembly from the build log
			var assemblies = FilterToAssembly (lines, assemblyName);
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Distinct ().Count (), Is.EqualTo (1), "Unique assemblies");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");

			// Verify that there's one resource in the binding assembly, and its name
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			Assert.That (ad.MainModule.Resources.Count, Is.EqualTo (0), "no embedded resources");
			var resourceBundle = Path.Combine (project_dir, "bin", "Debug", platform.ToFramework (), assemblyName + ".resources");
			Assert.That (resourceBundle, Does.Exist, "Bundle existence");
		}

		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[Category ("AllPlatforms")]
		public void BuildBindingsTest2 (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var assemblyName = "bindings-test2";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform.AsString ());
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");

			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = BinLog.PrintToLines (result.BinLogPath);
			// Find the resulting binding assembly from the build log
			var assemblies = FilterToAssembly (lines, assemblyName);
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Distinct ().Count (), Is.EqualTo (1), "Unique assemblies");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");

			// Verify that there's one resource in the binding assembly, and its name
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			Assert.That (ad.MainModule.Resources.Count, Is.EqualTo (0), "no embedded resources");
			var resourceBundle = Path.Combine (project_dir, "bin", "Debug", platform.ToFramework (), assemblyName + ".resources");
			Assert.That (resourceBundle, Does.Exist, "Bundle existence");
		}

		[TestCase ("iOS", "monotouch")]
		[TestCase ("tvOS", "monotouch")]
		[TestCase ("macOS", "xammac")]
		[TestCase ("MacCatalyst", "monotouch")]
		[Category ("AllPlatforms")]
		public void BuildBundledResources (string platform, string prefix)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var assemblyName = "BundledResources";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");

			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = BinLog.PrintToLines (result.BinLogPath);
			// Find the resulting binding assembly from the build log
			var assemblies = FilterToAssembly (lines, assemblyName);
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Distinct ().Count (), Is.EqualTo (1), "Unique assemblies");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");

			// Verify that there's one resource in the binding assembly, and its name
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			// Sort the resources before we assert, since we don't care about the order, and sorted order makes the asserts simpler.
			var resources = ad.MainModule.Resources.OrderBy (v => v.Name).Select (v => v.Name).ToArray ();
			var expectedResources = new string [] {
				$"__{prefix}_content_basn3p08__with__loc.png",
				$"__{prefix}_content_basn3p08.png",
				$"__{prefix}_content_xamvideotest.mp4",
			};
			Assert.That (resources, Is.EqualTo (expectedResources), "Resources");
		}

		[TestCase ("iOS")]
		[TestCase ("tvOS")]
		[TestCase ("macOS")]
		[TestCase ("MacCatalyst")]
		[Category ("AllPlatforms")]
		public void BuildInterdependentBindingProjects (string platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var assemblyName = "interdependent-binding-projects";
			var dotnet_bindings_dir = Path.Combine (Configuration.SourceRoot, "tests", assemblyName, "dotnet");
			var project_dir = Path.Combine (dotnet_bindings_dir, platform);
			var project_path = Path.Combine (project_dir, $"{assemblyName}.csproj");

			Clean (project_path);
			var result = DotNet.AssertBuild (project_path, verbosity);
			var lines = BinLog.PrintToLines (result.BinLogPath);
			// Find the resulting binding assembly from the build log
			var assemblies = lines.
				Select (v => v.Trim ()).
				Where (v => {
					if (v.Length < 10)
						return false;
					if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
						if (v [1] != ':') {
							return false;
						}
					} else {
						if (v [0] != '/') {
							return false;
						}
					}
					if (!v.EndsWith ($"{assemblyName}.dll", StringComparison.Ordinal))
						return false;
					if (!v.Contains (Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
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

			// Verify that the resources have been linked away
			var asmDir = Path.GetDirectoryName (asm)!;
			var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			Assert.That (ad.MainModule.Resources.Count, Is.EqualTo (0), "0 resources for interdependent-binding-projects.dll");

			var ad1 = AssemblyDefinition.ReadAssembly (Path.Combine (asmDir, "bindings-test.dll"), new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			// The native library is removed from the resources by the linker
			Assert.That (ad1.MainModule.Resources.Count, Is.EqualTo (0), "0 resources for bindings-test.dll");

			var ad2 = AssemblyDefinition.ReadAssembly (Path.Combine (asmDir, "bindings-test2.dll"), new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			// The native library is removed from the resources by the linker
			Assert.That (ad2.MainModule.Resources.Count, Is.EqualTo (0), "0 resources for bindings-test2.dll");

		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x86;iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "iossimulator-x86;iossimulator-x64;iossimulator-arm64")]
		[TestCase (ApplePlatform.iOS, "ios-arm;ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64;tvossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		public void BuildFatApp (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			var result = DotNet.AssertBuild (project_path, properties);
			AssertThatLinkerExecuted (result);
			var infoPlistPath = GetInfoPListPath (platform, appPath);
			Assert.That (infoPlistPath, Does.Exist, "Info.plist");
			var infoPlist = PDictionary.FromFile (infoPlistPath)!;
			Assert.AreEqual ("com.xamarin.mysimpleapp", infoPlist.GetString ("CFBundleIdentifier").Value, "CFBundleIdentifier");
			Assert.AreEqual ("MySimpleApp", infoPlist.GetString ("CFBundleDisplayName").Value, "CFBundleDisplayName");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleVersion").Value, "CFBundleVersion");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleShortVersionString").Value, "CFBundleShortVersionString");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x86;iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm;ios-arm64", "MtouchLink=SdkOnly")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		public void BuildFatMonoTouchTest (ApplePlatform platform, string runtimeIdentifiers, params string [] additionalProperties)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = Path.Combine (Configuration.SourceRoot, "tests", "monotouch-test", "dotnet", platform.AsString (), "monotouch-test.csproj");
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			if (additionalProperties is not null) {
				foreach (var prop in additionalProperties) {
					var eq = prop.IndexOf ('=');
					var name = prop.Substring (0, eq);
					var value = prop.Substring (eq + 1);
					properties [name] = value;
				}
			}
			var result = DotNet.AssertBuild (project_path, properties);
			var appPath = Path.Combine (Path.GetDirectoryName (project_path)!, "bin", "Debug", platform.ToFramework (), "monotouchtest.app");
			var infoPlistPath = GetInfoPListPath (platform, appPath);
			Assert.That (infoPlistPath, Does.Exist, "Info.plist");
			var infoPlist = PDictionary.FromFile (infoPlistPath)!;
			Assert.AreEqual ("com.xamarin.monotouch-test", infoPlist.GetString ("CFBundleIdentifier").Value, "CFBundleIdentifier");
			Assert.AreEqual ("MonoTouchTest", infoPlist.GetString ("CFBundleDisplayName").Value, "CFBundleDisplayName");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64;iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64;iossimulator-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64;tvossimulator-x64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64;tvossimulator-arm64")]
		public void InvalidRuntimeIdentifiers (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			var rv = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			Assert.AreEqual (1, errors.Length, "Error count");
			Assert.AreEqual ($"Building for all the runtime identifiers '{runtimeIdentifiers}' at the same time isn't possible, because they represent different platform variations.", errors [0].Message, "Error message");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", false)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true, null, "Release")]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true, "PublishTrimmed=true;UseInterpreter=true")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", false)]
		[Category ("AllPlatforms")]
		public void IsNotMacBuild (ApplePlatform platform, string runtimeIdentifiers, bool isDeviceBuild, string? extraProperties = null, string configuration = "Debug")
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			if (isDeviceBuild)
				Configuration.AssertDeviceAvailable ();

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["IsMacEnabled"] = "false";
			if (!string.IsNullOrEmpty (configuration))
				properties ["Configuration"] = configuration;
			if (extraProperties is not null) {
				foreach (var assignment in extraProperties.Split (';')) {
					var split = assignment.Split ('=');
					properties [split [0]] = split [1];
				}
			}
			var result = DotNet.AssertBuild (project_path, properties);
			AssertThatLinkerDidNotExecute (result);
			switch (platform) {
			case ApplePlatform.iOS:
				var appExecutable = Path.Combine (appPath, Path.GetFileName (project_path));
				Assert.That (appPath, Does.Not.Exist, "There is an .app");
				Assert.That (appExecutable, Does.Not.Empty, "There is no executable");
				Assert.That (Path.Combine (appPath, Configuration.GetBaseLibraryName (platform, true)), Does.Not.Exist, "Platform assembly is in the bundle");
				break;
			case ApplePlatform.MacCatalyst:
				break;
			}
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		public void IsOverrideRuntimeIdentifier (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			// Be specific that we want "RuntimeIdentifier" specified on the command line, and "RuntimeIdentifiers" in a file
			properties ["file:RuntimeIdentifiers"] = properties ["RuntimeIdentifiers"];
			properties.Remove ("RuntimeIdentifiers");
			properties ["cmdline:RuntimeIdentifier"] = "maccatalyst-x64";
			var rv = DotNet.AssertBuild (project_path, properties);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			Assert.AreEqual (1, warnings.Length, "Warning Count");
			Assert.AreEqual ("RuntimeIdentifier was set on the command line, and will override the value for RuntimeIdentifiers set in the project file.", warnings [0].Message, "Warning message");
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		public void IsNotOverrideRuntimeIdentifier (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			var projectPath = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (projectPath);
			var props = GetDefaultProperties ();
			props ["RuntimeIdentifier"] = "maccatalyst-x64";
			props ["RuntimeIdentifiers"] = "maccatalyst-arm64";
			var rv = DotNet.AssertBuildFailure (projectPath, props);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			Assert.AreEqual ("Both RuntimeIdentifier and RuntimeIdentifiers were passed on the command line, but only one of them can be set at a time.", errors [0].Message);
			Assert.AreEqual (1, errors.Length, "Error count");
		}

		[Test]
		[TestCase ("NativeDynamicLibraryReferencesApp", ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase ("NativeDynamicLibraryReferencesApp", ApplePlatform.MacOSX, "osx-x64")]
		[TestCase ("NativeFileReferencesApp", ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase ("NativeFileReferencesApp", ApplePlatform.MacOSX, "osx-x64")]
		[TestCase ("NativeFrameworkReferencesApp", ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase ("NativeFrameworkReferencesApp", ApplePlatform.MacOSX, "osx-x64")]
		[TestCase ("NativeXCFrameworkReferencesApp", ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase ("NativeXCFrameworkReferencesApp", ApplePlatform.MacOSX, "osx-x64")]
		public void BuildAndExecuteNativeReferencesTestApp (string project, ApplePlatform platform, string runtimeIdentifier)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifier, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			DotNet.AssertBuild (project_path, properties);

			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst) {
				var appExecutable = Path.Combine (appPath, "Contents", "MacOS", Path.GetFileNameWithoutExtension (project_path));
				Assert.That (appExecutable, Does.Exist, "There is an executable");
				ExecuteWithMagicWordAndAssert (appExecutable);
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-x64", false)] // valid RID in a previous preview (and common mistake)
		[TestCase (ApplePlatform.iOS, "iossimulator-x84", true)] // it's x86, not x84
		[TestCase (ApplePlatform.iOS, "iossimulator-arm", true)] // we don't support this
		[TestCase (ApplePlatform.iOS, "helloworld", true)] // random text
		[TestCase (ApplePlatform.iOS, "osx-x64", false)] // valid RID for another platform
		[TestCase (ApplePlatform.TVOS, "tvos-x64", false)] // valid RID in a previous preview (and common mistake)
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x46", true)] // it's x64, not x46
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm", true)] // we don't support this
		[TestCase (ApplePlatform.TVOS, "helloworld", true)] // random text
		[TestCase (ApplePlatform.TVOS, "osx-x64", false)] // valid RID for another platform
		[TestCase (ApplePlatform.MacOSX, "osx-x46", true)] // it's x64, not x46
		[TestCase (ApplePlatform.MacOSX, "macos-arm64", true)] // it's osx, not macos
		[TestCase (ApplePlatform.MacOSX, "helloworld", true)] // random text
		[TestCase (ApplePlatform.MacOSX, "ios-arm64", false)] // valid RID for another platform
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x46", true)] // it's x64, not x46
		[TestCase (ApplePlatform.MacCatalyst, "helloworld", true)] // random text
		[TestCase (ApplePlatform.MacCatalyst, "osx-x64", false)] // valid RID for another platform
		public void InvalidRuntimeIdentifier (ApplePlatform platform, string runtimeIdentifier, bool notRecognized)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			var rv = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			var uniqueErrors = errors.Select (v => v.Message).Distinct ().ToArray ();
			Assert.AreEqual (1, uniqueErrors.Length, "Error count");
			string expectedError;
			if (notRecognized) {
				expectedError = $"The specified RuntimeIdentifier '{runtimeIdentifier}' is not recognized. See https://aka.ms/netsdk1083 for more information.";
			} else {
				expectedError = $"The RuntimeIdentifier '{runtimeIdentifier}' is invalid.";
			}
			Assert.AreEqual (expectedError, uniqueErrors [0], "Error message");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "win10-x86", "The specified RuntimeIdentifier 'win10-x86' is not recognized.")]
		[TestCase (ApplePlatform.TVOS, "win10-x64", "The specified RuntimeIdentifier 'win10-x64' is not recognized.")]
		[TestCase (ApplePlatform.MacOSX, "win10-arm64", "The specified RuntimeIdentifier 'win10-arm64' is not recognized.")]
		[TestCase (ApplePlatform.MacCatalyst, "win10-arm64", "The specified RuntimeIdentifier 'win10-arm64' is not recognized.")]
		public void InvalidRuntimeIdentifier_Restore (ApplePlatform platform, string runtimeIdentifier, string? failureMessagePattern)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			if (string.IsNullOrEmpty (failureMessagePattern)) {
				DotNet.AssertRestore (project_path, properties);
			} else {
				var rv = DotNet.Restore (project_path, properties);
				Assert.AreNotEqual (0, rv.ExitCode, "Expected failure");
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
				Assert.That (errors.Length, Is.GreaterThan (0), "Error count");
				Assert.That (errors [0].Message, Does.Match (failureMessagePattern), "Message failure");
			}
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void FilesInAppBundle (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);

			// Build
			DotNet.AssertBuild (project_path, properties);

			// Simulate a crash dump
			File.WriteAllText (Path.Combine (appPath, "mono_crash.mem.123456.something.blob"), "A crash dump");
			File.WriteAllText (Path.Combine (appPath, "mono_crash.123456.somethingelse.blob"), "A crash dump");

			// Build again
			DotNet.AssertBuild (project_path, properties);

			// Create a file that isn't a crash report.
			File.WriteAllText (Path.Combine (appPath, "otherfile.txt"), "A file");
			var otherFileInDir = Path.Combine (appPath, "otherdir", "otherfile.log");
			Directory.CreateDirectory (Path.GetDirectoryName (otherFileInDir)!);
			File.WriteAllText (otherFileInDir, "A log");

			// Build again - this time it'll fail
			var rv = DotNet.Build (project_path, properties);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			Assert.AreNotEqual (0, rv.ExitCode, "Unexpected success");
			Assert.AreEqual (1, warnings.Length, "Warning Count");
			Assert.AreEqual ($"Found files in the root directory of the app bundle. This will likely cause codesign to fail. Files:\nbin/Debug/{Configuration.DotNetTfm}-maccatalyst/maccatalyst-x64/MySimpleApp.app/otherfile.txt\nbin/Debug/{Configuration.DotNetTfm}-maccatalyst/maccatalyst-x64/MySimpleApp.app/otherdir\nbin/Debug/{Configuration.DotNetTfm}-maccatalyst/maccatalyst-x64/MySimpleApp.app/otherdir/otherfile.log", warnings [0].Message, "Warning");

			// Build again, asking for automatic removal of the extraneous files.
			var enableAutomaticCleanupProperties = new Dictionary<string, string> (properties);
			enableAutomaticCleanupProperties ["EnableAutomaticAppBundleRootDirectoryCleanup"] = "true";
			rv = DotNet.AssertBuild (project_path, enableAutomaticCleanupProperties);
			warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			Assert.AreEqual (0, warnings.Length, "Warning Count");

			// Verify that the files were in fact removed.
			Assert.That (Path.Combine (appPath, "otherfile.txt"), Does.Not.Exist, "otherfile");
			Assert.That (Path.GetDirectoryName (otherFileInDir), Does.Not.Exist, "otherdir");
		}

		[Test]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void BuildCoreCLR (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["UseMonoRuntime"] = "false";
			var rv = DotNet.AssertBuild (project_path, properties);

			AssertThatLinkerExecuted (rv);
			var infoPlistPath = GetInfoPListPath (platform, appPath);
			Assert.That (infoPlistPath, Does.Exist, "Info.plist");
			var infoPlist = PDictionary.FromFile (infoPlistPath)!;
			Assert.AreEqual ("com.xamarin.mysimpleapp", infoPlist.GetString ("CFBundleIdentifier").Value, "CFBundleIdentifier");
			Assert.AreEqual ("MySimpleApp", infoPlist.GetString ("CFBundleDisplayName").Value, "CFBundleDisplayName");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleVersion").Value, "CFBundleVersion");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleShortVersionString").Value, "CFBundleShortVersionString");

			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			var createdump = Path.Combine (appPath, "Contents", "MonoBundle", "createdump");
			Assert.That (createdump, Does.Not.Exist, "createdump existence");
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void AbsoluteOutputPath (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var outputPath = Cache.CreateTemporaryDirectory ();
			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["OutputPath"] = outputPath + "/";
			var rv = DotNet.AssertBuild (project_path, properties);

			AssertThatLinkerExecuted (rv);

			var appPath = Path.Combine (outputPath, project + ".app");
			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		public void SimpleAppWithOldReferences (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "SimpleAppWithOldReferences";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			DotNet.AssertBuild (project_path, GetDefaultProperties (runtimeIdentifiers));

			var appExecutable = GetNativeExecutable (platform, appPath);
			Assert.That (appExecutable, Does.Exist, "There is an executable");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void BindingWithDefaultCompileInclude (ApplePlatform platform)
		{
			var project = "BindingWithDefaultCompileInclude";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);

			var rv = DotNet.AssertBuild (project_path, GetDefaultProperties ());

			var dllPath = Path.Combine (Path.GetDirectoryName (project_path)!, "bin", "Debug", platform.ToFramework (), $"{project}.dll");
			Assert.That (dllPath, Does.Exist, "Binding assembly");

			// Verify that the MyNativeClass class exists in the assembly, and that it's actually a class.
			var ad = AssemblyDefinition.ReadAssembly (dllPath, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			var myNativeClass = ad.MainModule.Types.FirstOrDefault (v => v.FullName == "MyApiDefinition.MyNativeClass");
			Assert.IsFalse (myNativeClass!.IsInterface, "IsInterface");
			var myStruct = ad.MainModule.Types.FirstOrDefault (v => v.FullName == "MyClassLibrary.MyStruct");
			Assert.IsTrue (myStruct!.IsValueType, "MyStruct");

			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")] // https://github.com/xamarin/xamarin-macios/issues/12410
		public void AppWithResources (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "AppWithResources";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			DotNet.AssertBuild (project_path, GetDefaultProperties (runtimeIdentifiers));

			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			var resourcesDirectory = GetResourcesDirectory (platform, appPath);

			var fontDirectory = resourcesDirectory;
			var fontAFile = Path.Combine (fontDirectory, "A.ttc");
			var fontBFile = Path.Combine (fontDirectory, "B.otf");
			var fontCFile = Path.Combine (fontDirectory, "C.ttf");

			Assert.That (fontAFile, Does.Exist, "A.ttc existence");
			Assert.That (fontBFile, Does.Exist, "B.otf existence");
			Assert.That (fontCFile, Does.Exist, "C.ttf existence");

			var plist = PDictionary.FromFile (GetInfoPListPath (platform, appPath))!;
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.MacCatalyst:
				var uiAppFonts = plist.GetArray ("UIAppFonts");
				Assert.IsNotNull (uiAppFonts, "UIAppFonts");
				Assert.AreEqual (1, uiAppFonts.Count, "UIAppFonts.Count");
				Assert.AreEqual ("B.otf", ((PString) uiAppFonts [0]).Value, "UIAppFonts [0]");
				break;
			case ApplePlatform.MacOSX:
				var applicationFontsPath = plist.GetString ("ATSApplicationFontsPath")?.Value;
				Assert.AreEqual (".", applicationFontsPath, "ATSApplicationFontsPath");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}

			var assetsCar = Path.Combine (resourcesDirectory, "Assets.car");
			Assert.That (assetsCar, Does.Exist, "Assets.car");

			var mainStoryboard = Path.Combine (resourcesDirectory, "Main.storyboardc");
			Assert.That (mainStoryboard, Does.Exist, "Main.storyboardc");

			var scnAssetsDir = Path.Combine (resourcesDirectory, "art.scnassets");
			Assert.That (Path.Combine (scnAssetsDir, "scene.scn"), Does.Exist, "scene.scn");
			Assert.That (Path.Combine (scnAssetsDir, "texture.png"), Does.Exist, "texture.png");

			var colladaScene = Path.Combine (resourcesDirectory, "scene.dae");
			Assert.That (colladaScene, Does.Exist, "Collada - scene.dae");

			var atlasTexture = Path.Combine (resourcesDirectory, "Archer_Attack.atlasc", "Archer_Attack.plist");
			Assert.That (atlasTexture, Does.Exist, "AtlasTexture - Archer_Attack");

			var mlModel = Path.Combine (resourcesDirectory, "SqueezeNet.mlmodelc");
			Assert.That (mlModel, Does.Exist, "CoreMLModel");

			var arm64txt = Path.Combine (resourcesDirectory, "arm64.txt");
			var armtxt = Path.Combine (resourcesDirectory, "arm.txt");
			var x64txt = Path.Combine (resourcesDirectory, "x64.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-arm64")), File.Exists (arm64txt), "arm64.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-arm")), File.Exists (armtxt), "arm.txt");
			Assert.AreEqual (runtimeIdentifiers.Split (';').Any (v => v.EndsWith ("-x64")), File.Exists (x64txt), "x64.txt");
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")] // https://github.com/xamarin/xamarin-macios/issues/12410
		public void DoubleBuild (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "AppWithResources";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var projectPath = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out _);
			Clean (projectPath);

			DotNet.AssertBuild (projectPath, GetDefaultProperties (runtimeIdentifiers));
			DotNet.AssertBuild (projectPath, GetDefaultProperties (runtimeIdentifiers));
		}

		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		[Category ("AllPlatforms")]
		public void LibraryReferencingBindingLibrary (ApplePlatform platform)
		{
			var project = "LibraryReferencingBindingLibrary";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var projectPath = GetProjectPath (project, runtimeIdentifiers: string.Empty, platform: platform, out _);
			Clean (projectPath);

			DotNet.AssertBuild (projectPath, GetDefaultProperties ());

			var bindir = GetBinDir (projectPath, platform, string.Empty);
			var bindingResourcePackages = new List<string> () {
				Path.Combine ("BindingWithUncompressedResourceBundle.resources", "libtest.a"),
				Path.Combine ("BindingWithUncompressedResourceBundle.resources", "manifest"),
			};

			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				bindingResourcePackages.Add (Path.Combine ("bindings-framework-test.resources", "XStaticArTest.framework", "XStaticArTest"));
				bindingResourcePackages.Add (Path.Combine ("bindings-framework-test.resources", "XStaticObjectTest.framework", "XStaticObjectTest"));
				bindingResourcePackages.Add (Path.Combine ("bindings-framework-test.resources", "XTest.framework", "Info.plist"));
				bindingResourcePackages.Add (Path.Combine ("bindings-framework-test.resources", "XTest.framework", "XTest"));
				bindingResourcePackages.Add (Path.Combine ("bindings-framework-test.resources", "manifest"));
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				bindingResourcePackages.Add ("bindings-framework-test.resources.zip");
				break;
			}

			DumpFiles (bindir);
			foreach (var brp in bindingResourcePackages) {
				var file = Path.Combine (bindir, brp);
				Assert.That (file, Does.Exist, "Existence");
			}

			// Whether the binding project produces a compressed binding package or not depends on whether there are
			// symlinks in the resources, which, for xcframeworks, depends not only on the current platform we're testing,
			// but which platforms are included in the build: if the current build doesn't support neither macOS nor Mac Catalyst,
			// then we won't create an xcframework with symlinks, which means that building the binding project for iOS and tvOS
			// will produce a non-compressed binding package. Thus we assert that we either have a non-compressed or a compressed
			// package here.
			var hasCompressedResources = File.Exists (Path.Combine (bindir, "BindingWithDefaultCompileInclude.resources.zip"));
			var hasDirectoryResources = Directory.Exists (Path.Combine (bindir, "BindingWithDefaultCompileInclude.resources"));
			if (!hasDirectoryResources && !hasCompressedResources)
				Assert.Fail ($"Could not find either BindingWithDefaultCompileInclude.resources.zip or BindingWithDefaultCompileInclude.resources in {bindir}");
		}

		void DumpFiles (string dir)
		{
			var files = Directory.GetFileSystemEntries (dir, "*", SearchOption.AllDirectories).ToArray ();
			Console.WriteLine ($"DumpFiles ({dir}): {files.Length} files:");
			foreach (var file in files)
				Console.WriteLine ($"    {file}");
		}

		void AssertAppContents (ApplePlatform platform, string app_directory)
		{
			var info_plist_path = GetInfoPListPath (platform, app_directory);
			Assert.That (info_plist_path, Does.Exist, "Info.plist");

			var assets_path = string.Empty;
			switch (platform) {
			case ApplePlatform.iOS:
				break; // sample project doesn't have assets
			case ApplePlatform.TVOS:
				assets_path = Path.Combine (app_directory, "Assets.car");
				break;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				assets_path = Path.Combine (app_directory, "Contents", "Resources", "Assets.car");
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
			if (!string.IsNullOrEmpty (assets_path))
				Assert.That (assets_path, Does.Exist, "Assets.car");

			var libxamarin = Directory.GetFileSystemEntries (app_directory, "libxamarin*dylib", SearchOption.AllDirectories);
			Assert.That (libxamarin, Has.Length.LessThanOrEqualTo (1), $"No more than one libxamarin should be present, but found {libxamarin.Length}:\n\t{string.Join ("\n\t", libxamarin)}");
		}

		IEnumerable<string> FilterToAssembly (IEnumerable<string> lines, string assemblyName)
		{
			var rv = FilterToAssembly2 (lines, assemblyName, false);
			if (!rv.Any ()) {
				Console.WriteLine ($"Could not find any matching lines of {lines.Count ()} lines matching {assemblyName}");
				rv = FilterToAssembly2 (lines, assemblyName, true);
			}
			return rv;
		}

		IEnumerable<string> FilterToAssembly2 (IEnumerable<string> lines, string assemblyName, bool log)
		{
			return lines.
				Select (v => v.Trim ()).
				Where (v => {
					if (v.Length < 10) {
						if (log)
							Console.WriteLine ($"    1: {v}");
						return false;
					}
					if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
						if (v [1] != ':') {
							if (log)
								Console.WriteLine ($"    2a: {v}");
							return false;
						}
					} else {
						if (v [0] != '/') {
							if (log)
								Console.WriteLine ($"    2b: {v}");
							return false;
						}
					}
					if (!v.EndsWith ($"{assemblyName}.dll", StringComparison.Ordinal)) {
						if (log)
							Console.WriteLine ($"    3: {v}");
						return false;
					}
					if (!v.Contains (Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal)) {
						if (log)
							Console.WriteLine ($"    4: {v}");
						return false;
					}
					if (v.Contains (Path.DirectorySeparatorChar + "ref" + Path.DirectorySeparatorChar, StringComparison.Ordinal)) {
						if (log)
							Console.WriteLine ($"    5: {v}");
						return false; // Skip reference assemblies
					}
					if (!File.Exists (v)) {
						if (log)
							Console.WriteLine ($"    6: {v}");
						return false;
					}

					Console.WriteLine ($"    YAY 7: {v}");
					return true;
				});
		}

		// This is copied from the KillEverything method in xharness/Microsoft.DotNet.XHarness.iOS.Shared/Hardware/SimulatorDevice.cs and modified to work here.
		[OneTimeSetUp]
		public void KillEverything ()
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				Console.WriteLine ($"Skipped killing everything, because there's nothing to kill on Windows.");
				return;
			}

			ExecutionHelper.Execute ("launchctl", new [] { "remove", "com.apple.CoreSimulator.CoreSimulatorService" }, timeout: TimeSpan.FromSeconds (10));

			var to_kill = new string [] { "iPhone Simulator", "iOS Simulator", "Simulator", "Simulator (Watch)", "com.apple.CoreSimulator.CoreSimulatorService", "ibtoold" };

			var args = new List<string> ();
			args.Add ("-9");
			args.AddRange (to_kill);
			ExecutionHelper.Execute ("killall", args, timeout: TimeSpan.FromSeconds (10));

			var dirsToBeDeleted = new [] {
				Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Saved Application State", "com.apple.watchsimulator.savedState"),
				Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Saved Application State", "com.apple.iphonesimulator.savedState"),
			};

			foreach (var dir in dirsToBeDeleted) {
				try {
					if (Directory.Exists (dir))
						Directory.Delete (dir, true);
				} catch (Exception e) {
					Console.WriteLine ("Could not delete the directory '{0}': {1}", dir, e.Message);
				}
			}

			// https://github.com/xamarin/xamarin-macios/issues/10012
			ExecutionHelper.Execute ("xcrun", new [] { "simctl", "list" });
		}


		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		// [TestCase ("MacCatalyst", "")] - No extension support yet
		public void BuildProjectsWithExtensions (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var consumingProjectDir = GetProjectPath ("ExtensionConsumer", platform: platform);
			var extensionProjectDir = GetProjectPath ("ExtensionProject", platform: platform);

			Clean (extensionProjectDir);
			Clean (consumingProjectDir);

			DotNet.AssertBuild (consumingProjectDir, verbosity);

			var extensionPath = Path.Combine (Path.GetDirectoryName (consumingProjectDir)!, "bin", "Debug", platform.ToFramework (), GetDefaultRuntimeIdentifier (platform), "MySimpleApp.app", GetPlugInsRelativePath (platform), "ExtensionProject.appex");
			Assert.That (Directory.Exists (extensionPath), $"App extension directory does not exist: {extensionPath}");

			var pathToSearch = Path.Combine (Path.GetDirectoryName (consumingProjectDir)!, "bin", "Debug");
			string [] configFiles = Directory.GetFiles (pathToSearch, "*.runtimeconfig.*", SearchOption.AllDirectories);
			Assert.AreNotEqual (0, configFiles.Length, "runtimeconfig.json file does not exist");
		}

		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		// [TestCase ("MacCatalyst", "")] - No extension support yet
		public void BuildProjectsWithExtensionsAndFrameworks (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);
			var consumingProjectDir = GetProjectPath ("ExtensionConsumerWithFrameworks", runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			var extensionProjectDir = GetProjectPath ("ExtensionProjectWithFrameworks", platform: platform);

			Clean (extensionProjectDir);
			Clean (consumingProjectDir);

			DotNet.AssertBuild (consumingProjectDir, verbosity);

			var extensionPath = Path.Combine (Path.GetDirectoryName (consumingProjectDir)!, "bin", "Debug", platform.ToFramework (), GetDefaultRuntimeIdentifier (platform), "ExtensionConsumerWithFrameworks.app", GetPlugInsRelativePath (platform), "ExtensionProjectWithFrameworks.appex");
			Assert.That (Directory.Exists (extensionPath), $"App extension directory does not exist: {extensionPath}");
			var extensionFrameworksPath = Path.Combine (extensionPath, GetFrameworksRelativePath (platform));
			Assert.IsFalse (Directory.Exists (extensionFrameworksPath), $"App extension framework directory exists when it shouldn't: {extensionFrameworksPath}");

			var pathToSearch = Path.Combine (Path.GetDirectoryName (consumingProjectDir)!, "bin", "Debug");
			var configFiles = Directory.GetFiles (pathToSearch, "*.runtimeconfig.*", SearchOption.AllDirectories);
			Assert.AreNotEqual (0, configFiles.Length, "runtimeconfig.json file does not exist");

			var appFrameworksPath = Path.Combine (appPath, GetFrameworksRelativePath (platform));
			Assert.That (Directory.Exists (appFrameworksPath), $"App Frameworks directory does not exist: {appFrameworksPath}");

			Assert.That (File.Exists (Path.Combine (appFrameworksPath, "SomewhatUnknownD.framework", "SomewhatUnknownD")), "SomewhatUnknownD");
			Assert.That (File.Exists (Path.Combine (appFrameworksPath, "UnknownD.framework", "UnknownD")), "UnknownD");
			Assert.That (File.Exists (Path.Combine (appFrameworksPath, "UnknownE.framework", "UnknownE")), "UnknownE");

			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);
		}


		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		// [TestCase ("MacCatalyst", "")] - No extension support yet
		public void BuildTrimmedExtensionProject (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var project_path = GetProjectPath ("ExtensionProject", platform: platform);

			Clean (project_path);

			var properties = GetDefaultProperties ();
			properties ["MtouchLink"] = "Full";
			properties ["LinkMode"] = "Full";
			DotNet.AssertBuild (project_path, properties);
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64;iossimulator-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void AppWithGenericLibraryReference (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "AppWithGenericLibraryReference";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			if (platform == ApplePlatform.MacOSX) {
				Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS); // This test requires iOS as well when testing macOS
			} else {
				Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX); // This test requires macOS as well (for all other platforms except macOS).
			}

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			DotNet.AssertBuild (project_path, GetDefaultProperties (runtimeIdentifiers));

			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);
		}

		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void OlderCSharpLanguage (ApplePlatform platform, string runtimeIdentifier)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			properties ["LangVersion"] = "8";
			properties ["ExcludeTouchUnitReference"] = "true";
			DotNet.AssertBuild (project_path, properties);
		}

		// This test can be removed in .NET 7
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void CentralPackageVersionsApp (ApplePlatform platform)
		{
			var project = "CentralPackageVersionsApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties ();
			DotNet.AssertBuild (project_path, properties);
		}

		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", false)]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", true)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64;osx-arm64", true)]
		public void CatalystAppOptimizedForMacOS (ApplePlatform platform, string runtimeIdentifier, bool failureExpected)
		{
			var project = "CatalystAppOptimizedForMacOS";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			if (failureExpected) {
				var rv = DotNet.AssertBuildFailure (project_path, properties);
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
				Assert.AreEqual (1, errors.Length, "Error count");
				Assert.AreEqual ($"The UIDeviceFamily value '6' is not valid for this platform. It's only valid for Mac Catalyst.", errors [0].Message, "Error message");
			} else {
				DotNet.AssertBuild (project_path, properties);
			}
		}

		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", "13.3")]
		public void CatalystAppOptimizedForMacOS_InvalidMinOS (ApplePlatform platform, string runtimeIdentifier, string minOS)
		{
			var project = "CatalystAppOptimizedForMacOS";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			properties ["SupportedOSPlatformVersion"] = minOS;
			var rv = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			Assert.AreEqual (1, errors.Length, "Error count");
			Assert.AreEqual ($"The UIDeviceFamily value '6' requires macOS 11.0. Please set the 'SupportedOSPlatformVersion' in the project file to at least 14.0 (the Mac Catalyst version equivalent of macOS 11.0). The current value is {minOS} (equivalent to macOS 10.15.2).", errors [0].Message, "Error message");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void BuildNet6_0App (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "Net6_0SimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, netVersion: "net6.0");
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			var result = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (result.BinLogPath).ToList ();
			Assert.AreEqual (1, errors.Count, "Error Count");
			Assert.That (errors [0].Message, Does.Contain ("To build this project, the following workloads must be installed: "), "Error message");

			// With multi targeting, this happens instead:
			// // Due to an implementation detail in .NET, the same error message is shown twice.
			// Assert.AreEqual (2, errors.Count, "Error Count");
			// Assert.AreEqual (errors [0].Message, $"The workload 'net6.0-{platform.AsString ().ToLowerInvariant ()}' is out of support and will not receive security updates in the future. Please refer to https://aka.ms/maui-support-policy for more information about the support policy.", "Error message 1");
			// Assert.AreEqual (errors [1].Message, $"The workload 'net6.0-{platform.AsString ().ToLowerInvariant ()}' is out of support and will not receive security updates in the future. Please refer to https://aka.ms/maui-support-policy for more information about the support policy.", "Error message 2");
		}

		[Test]
		[Ignore ("Ignore due to issue: https://github.com/xamarin/xamarin-macios/issues/18655")]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void BuildNet7_0App (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "Net7_0SimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, netVersion: "net7.0");
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			// This property is needed until https://github.com/xamarin/xamarin-macios/pull/18411 has been merged and we can use the resulting packages.
			properties ["_RequiresILLinkPack"] = "true";

			var result = DotNet.AssertBuild (project_path, properties);
			AssertThatLinkerExecuted (result);
			var infoPlistPath = GetInfoPListPath (platform, appPath);
			Assert.That (infoPlistPath, Does.Exist, "Info.plist");
			var infoPlist = PDictionary.FromFile (infoPlistPath)!;
			Assert.AreEqual ("com.xamarin.mysimpleapp", infoPlist.GetString ("CFBundleIdentifier").Value, "CFBundleIdentifier");
			Assert.AreEqual ("MySimpleApp", infoPlist.GetString ("CFBundleDisplayName").Value, "CFBundleDisplayName");
			Assert.AreEqual ("7.0", infoPlist.GetString ("CFBundleVersion").Value, "CFBundleVersion");
			Assert.AreEqual ("7.0", infoPlist.GetString ("CFBundleShortVersionString").Value, "CFBundleShortVersionString");

			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		// [TestCase (ApplePlatform.TVOS, "tvos-arm64")] // Currently doesn't work because we overwrite the required MtouchExtraArgs in tests/nunit.frameworks.target in this test.
		// [TestCase (ApplePlatform.TVOS, "tvossimulator-x64")] // Currently doesn't work because we emit signatures with structs from the MetalPerformanceShaders framework, which isn't available in the tvOS simulator.
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void PInvokeWrapperGenerator (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			var extraArgs = "--require-pinvoke-wrappers:true --registrar:static"; // enable the static registrar too, see https://github.com/xamarin/xamarin-macios/issues/15190.
			properties ["MonoBundlingExtraArgs"] = extraArgs;
			properties ["MtouchExtraArgs"] = extraArgs;

			DotNet.AssertBuild (project_path, properties);
		}

		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", true)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", false)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64", true)]
		public void AutoDetectEntitlements (ApplePlatform platform, string runtimeIdentifiers, bool exclude)
		{
			var project = "AutoDetectEntitlements";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			if (exclude) {
				properties ["EnableDefaultCodesignEntitlements"] = "false";
				DotNet.AssertBuild (project_path, properties);
			} else {
				var rv = DotNet.AssertBuildFailure (project_path, properties);
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToList ();
				Assert.That (errors [0].Message, Does.Contain ("Error loading Entitlements.plist template 'Entitlements.plist'"), "Message");
			}
		}

		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		public void CustomAppBundleDir (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			var customAppBundleDir = Path.Combine (Cache.CreateTemporaryDirectory (), project + ".app");
			properties ["AppBundleDir"] = customAppBundleDir;
			var result = DotNet.AssertBuild (project_path, properties);
		}

		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void CustomizedCodeSigning (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "CustomizedCodeSigning";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);

			DotNet.AssertBuild (project_path, properties);

			var codesignDirectory = GetRelativeCodesignDirectory (platform);
			var sharedSupportDir = string.Empty;
			var dylibDir = GetRelativeDylibDirectory (platform);

			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				sharedSupportDir = "SharedSupport";
				break;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				sharedSupportDir = Path.Combine ("Contents", "SharedSupport");
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			var appBundleContents = Directory
				.GetFileSystemEntries (appPath, "*", SearchOption.AllDirectories)
				.Select (v => v.Substring (appPath.Length + 1))
				.ToHashSet ();

			// Assert that some apps are signed
			var directoriesThatMustExist = new string [] {
				Path.Combine (codesignDirectory, "_CodeSignature"),
				Path.Combine (sharedSupportDir, "app1.app", codesignDirectory, "_CodeSignature"),
			};

			foreach (var mustExist in directoriesThatMustExist)
				Assert.That (appBundleContents, Does.Contain (mustExist), "Must exist");

			appBundleContents.ExceptWith (directoriesThatMustExist);

			// And that there are no other signed apps
			var signatures = appBundleContents.Where (v => v.EndsWith ("_CodeSignature", StringComparison.Ordinal));
			Assert.That (signatures, Is.Empty, "No other signed app budnles");

			// Assert that some dylibs are signed
			var dylibs = appBundleContents.Where (v => Path.GetExtension (v) == ".dylib").ToList ();
			var signedDylibs = new List<string> {
				Path.Combine (sharedSupportDir, "app2.app", dylibDir, "lib2.dylib"),
			};

			foreach (var dylib in signedDylibs) {
				var path = Path.Combine (appPath, dylib);
				Assert.That (path, Does.Exist, "dylib exists");
				Assert.IsTrue (IsDylibSigned (path), $"Signed: {path}");
			}
			appBundleContents.ExceptWith (signedDylibs);
			// And that there are unsigned dylibs, but not the system ones
			var remainingDylibs = appBundleContents
				.Where (v => Path.GetExtension (v) == ".dylib")
				.ToArray ();
			foreach (var unsignedDylib in remainingDylibs) {
				var path = Path.Combine (appPath, unsignedDylib);
				Assert.That (path, Does.Exist, "unsigned dylib existence");
				Assert.IsFalse (IsDylibSigned (path), $"Unsigned: {path}");
			}
			Assert.AreEqual (1, remainingDylibs.Length, "Unsigned count");
		}

		bool IsDylibSigned (string dylib)
		{
			var file = MachO.Read (dylib).Single ();
			foreach (var lc in file.load_commands) {
				if (lc.cmd == (int) MachO.LoadCommands.CodeSignature)
					return true;
			}

			return false;
		}

		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", "Release")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64", "Debug")]
		public void AutoAllowJitEntitlements (ApplePlatform platform, string runtimeIdentifiers, string configuration)
		{
			var project = "Entitlements";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			DotNet.AssertBuild (project_path, properties);

			var executable = GetNativeExecutable (platform, appPath);
			var foundEntitlements = TryGetEntitlements (executable, out var entitlements);
			if (configuration == "Release") {
				Assert.IsTrue (foundEntitlements, "Found in Release");
				Assert.IsTrue (entitlements!.Get<PBoolean> ("com.apple.security.cs.allow-jit")?.Value, "Jit Allowed");
			} else {
				var jitNotAllowed = !foundEntitlements || !entitlements!.ContainsKey ("com.apple.security.cs.allow-jit");
				Assert.True (jitNotAllowed, "Jit Not Allowed");
			}
		}

		// [TestCase (ApplePlatform.MacCatalyst, null, "Release")]
		[TestCase (ApplePlatform.MacOSX, null, "Release")]
		public void NoWarnCodesign (ApplePlatform platform, string runtimeIdentifiers, string configuration)
		{
			var project = "Entitlements";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			properties ["EnableCodeSigning"] = "true";
			properties ["ExcludeNUnitLiteReference"] = "true";
			properties ["ExcludeTouchUnitReference"] = "true";
			var rv = DotNet.AssertBuild (project_path, properties);
			rv.AssertNoWarnings ();
		}

		[Test]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", true)]
		public void BuildAndExecuteAppWithNativeDynamicLibrariesInPackageReference (ApplePlatform platform, string runtimeIdentifier, bool isNativeAot)
		{
			var project = "AppWithNativeDynamicLibrariesInPackageReference";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifier, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);
			if (isNativeAot) {
				properties ["PublishAot"] = "true";
				properties ["_IsPublishing"] = "true";
			}
			DotNet.AssertBuild (project_path, properties);

			var appExecutable = Path.Combine (appPath, "Contents", "MacOS", Path.GetFileNameWithoutExtension (project_path));
			Assert.That (appExecutable, Does.Exist, "There is an executable");

			AssertThatDylibExistsAndIsReidentified (appPath, "libtest.dylib");
			AssertThatDylibExistsAndIsReidentified (appPath, "/subdir/libtest.dylib");
			AssertThatDylibExistsAndIsReidentified (appPath, "/subdir/libtest.so");

			ExecuteWithMagicWordAndAssert (appExecutable);
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void BuildAndExecuteAppWithXCFrameworkWithStaticLibraryInRuntimesNativeDirectory (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "AppWithXCFrameworkWithStaticLibraryInPackageReference";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			DotNet.AssertBuild (project_path, properties);

			var appExecutable = Path.Combine (appPath, "Contents", "MacOS", Path.GetFileNameWithoutExtension (project_path));
			Assert.That (appExecutable, Does.Exist, "There is an executable");

			if (CanExecute (platform, runtimeIdentifiers)) {
				var output = ExecuteWithMagicWordAndAssert (appExecutable);
				Assert.That (output, Does.Contain ("42"), "Execution");
			}
		}

		[Test]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		public void BuildAndExecuteAppWithWinExeOutputType (ApplePlatform platform, string runtimeIdentifier)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var project = "AppWithWinExeOutputType";
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifier, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifier);

			var rv = DotNet.AssertBuildFailure (project_path, properties);

			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			Assert.AreEqual (1, errors.Length, "Error count");
			Assert.AreEqual ($"WinExe is not a valid output type for macOS", errors [0].Message, "Error message");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void PublishAotDuringBuild (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["PublishAot"] = "true";
			DotNet.AssertBuild (project_path, properties);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64")]
		public void PublishAot (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["PublishAot"] = "true";
			properties ["_IsPublishing"] = "true"; // quack like "dotnet publish"
			properties ["ExcludeNUnitLiteReference"] = "true"; // we're asserting no warnings, and NUnitLite produces a lot of them, so ignore NUnitLite
			properties ["ExcludeTouchUnitReference"] = "true"; // we're asserting no warnings, and Touch.Unit produces a lot of them, so ignore Touch.Unit
			properties ["TrimmerSingleWarn"] = "false"; // don't be shy, we want to know what the problem is
			var rv = DotNet.AssertBuild (project_path, properties);

			// Verify that we have no warnings, but unfortunately we still have some we haven't fixed yet.
			// Ignore those, and fail the test if we stop getting them (so that we can update the test to not ignore them anymore).
			rv.AssertNoWarnings ((evt) => {
				if (platform == ApplePlatform.iOS && evt.Message?.Trim () == "Supported iPhone orientations have not been set")
					return false;

				return true;
			});
		}

		void AssertThatDylibExistsAndIsReidentified (string appPath, string dylibRelPath)
		{
			var dylibPath = Path.Join (appPath, "Contents", "MonoBundle", dylibRelPath);
			Assert.That (dylibPath, Does.Exist, "There is a library");

			var shared_libraries = ExecutionHelper.Execute ("otool", new [] { "-L", dylibPath }, hide_output: true);
			Assert.That (shared_libraries, Does.Contain (dylibPath), "The library ID is correct");
			Assert.That (shared_libraries, Does.Contain ($"@executable_path/../../Contents/MonoBundle/{dylibRelPath}"),
				"The dependent bundled shared library install name is relative to @executable_path");
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[Ignore ("Multi-targeting support has been temporarily reverted/postponed")]
		public void MultiTargetLibrary (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			// Get all the supported API versions
			var supportedApiVersion = Configuration.GetVariableArray ($"SUPPORTED_API_VERSIONS_{platform.AsString ().ToUpperInvariant ()}");
			supportedApiVersion = RemovePostCurrentOnMacCatalyst (supportedApiVersion, platform);
			var targetFrameworks = string.Join (";", supportedApiVersion.Select (v => v.Replace ("-", "-" + platform.AsString ().ToLowerInvariant ())));

			var project = "MultiTargetingLibrary";
			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties ();
			properties ["cmdline:AllTheTargetFrameworks"] = targetFrameworks;
			var rv = DotNet.AssertBuild (project_path, properties);
			rv.AssertNoWarnings ();
		}

		// Mac Catalyst projects can't be built with an earlier version of Xcode (even library projects),
		// which means that we can't build for target frameworks > than the current one (because we'll only have
		// the Xcode installed for the current target framework, especially on bots).
		// So filter out any target framework on Mac Catalyst that's after the current one.
		internal static IList<string> RemovePostCurrentOnMacCatalyst (IList<string> self, ApplePlatform platform)
		{
			if (platform == ApplePlatform.MacCatalyst) {
				var current = Configuration.DotNetTfm + "_" + Configuration.GetNuGetOsVersion (platform);
				return self.Where (v => string.Compare (v, current, StringComparison.Ordinal) <= 0).ToList ();
			}
			return self;
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		public void InvalidTargetPlatformVersion (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			// Pick a target platform version we don't support (such as iOS 6.66).
			var targetFrameworks = Configuration.DotNetTfm + "-" + platform.AsString ().ToLowerInvariant () + "6.66";
			var supportedApiVersions = GetSupportedApiVersions (platform);

			var project = "MultiTargetingLibrary";
			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties ();
			properties ["cmdline:AllTheTargetFrameworks"] = targetFrameworks;
			var rv = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			AssertErrorMessages (errors, $"6.66 is not a valid TargetPlatformVersion for {platform.AsString ()}. Valid versions include:\n{string.Join ('\n', supportedApiVersions)}");
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		public void UnsupportedTargetPlatformVersion (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			// Pick a target platform version that we don't really support,
			// but don't show an error in .NET 8 because of backwards compat.
			// The earliest target OS version should do.
			var minSupportedOSVersion = GetSupportedTargetPlatformVersions (platform).First ();
			var targetFrameworks = Configuration.DotNetTfm + "-" + platform.AsString ().ToLowerInvariant () + minSupportedOSVersion;
			var supportedApiVersions = GetSupportedApiVersions (platform, isCompat: false);

			var project = "MultiTargetingLibrary";
			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties ();
			properties ["cmdline:AllTheTargetFrameworks"] = targetFrameworks;

			if (IsTargetPlatformVersionCompatEnabled) {
				var rv = DotNet.AssertBuild (project_path, properties);
				var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
				AssertWarningMessages (warnings, $"{minSupportedOSVersion} is not a valid TargetPlatformVersion for {platform.AsString ()}. This warning will become an error in future versions of the {platform.AsString ()} workload. Valid versions include:\n{string.Join ('\n', supportedApiVersions)}");
			} else {
				var rv = DotNet.AssertBuildFailure (project_path, properties);
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
				AssertErrorMessages (errors, $"{minSupportedOSVersion} is not a valid TargetPlatformVersion for {platform.AsString ()}. Valid versions include:\n{string.Join ('\n', supportedApiVersions)}");
			}
		}

		bool IsTargetPlatformVersionCompatEnabled {
			get => Version.Parse (Configuration.DotNetTfm.Replace ("net", "")).Major < 9;
		}

		string [] GetSupportedApiVersions (ApplePlatform platform, bool? isCompat = null)
		{
			if (isCompat is null)
				isCompat = IsTargetPlatformVersionCompatEnabled;
			if (isCompat.Value)
				return GetSupportedTargetPlatformVersions (platform);

			var supportedApiVersions = Configuration.GetVariableArray ($"SUPPORTED_API_VERSIONS_{platform.AsString ().ToUpperInvariant ()}");
			return supportedApiVersions
				.Where (v => v.StartsWith (Configuration.DotNetTfm + "-", StringComparison.Ordinal))
				.Select (v => v.Substring (Configuration.DotNetTfm.Length + 1))
				.ToArray ();
		}

		string [] GetSupportedTargetPlatformVersions (ApplePlatform platform)
		{
			var plistPath = Path.Combine (Configuration.SourceRoot, "builds", $"Versions-{platform.AsString ()}.plist.in");
			var doc = new XmlDocument ();
			doc.Load (plistPath);
			var query = $"/plist/dict/key[text()='SupportedTargetPlatformVersions']/following-sibling::dict[1]/key[text()='{platform.AsString ()}']/following-sibling::array[1]/string";
			return doc
				.SelectNodes (query)!
				.Cast<XmlNode> ()
				.Select (v => v.InnerText)
				.ToArray ();
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "MtouchArch", "x86_64")]
		[TestCase (ApplePlatform.iOS, "MtouchArch", "ARMv7s")]
		[TestCase (ApplePlatform.TVOS, "MtouchArch", "ARM64")]
		[TestCase (ApplePlatform.MacOSX, "XamMacArch", "x86_64")]
		public void InvalidArchProperty (ApplePlatform platform, string property, string value)
		{
			// Only keep this test around for .NET 9+10, after that we'll just assume everyone has removed the MtouchArch/XamMacArch properties from their project files,
			// and at that point we can remove this test to save some testing time on the bots.
			if (Version.Parse (Configuration.DotNetTfm.Replace ("net", "")).Major > 10)
				Assert.Ignore ("This test can be removed, we shouldn't need it anymore.");
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties ();
			properties [property] = value;
			var rv = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			AssertErrorCount (errors, 1, "Error count");
			AssertErrorMessages (errors, $"The property '{property}' is deprecated, please remove it from the project file. Use 'RuntimeIdentifier' or 'RuntimeIdentifiers' instead to specify the target architecture.");
		}

		[Test]
		// The trailing semi-colon for single-arch platforms is significant:
		// it means we'll use "RuntimeIdentifiers" (plural) instead of "RuntimeIdentifier" (singular)
		[TestCase (ApplePlatform.iOS, "ios-arm64;")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64;")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64;")]
		public void StrippedRuntimeIdentifiers (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["NoSymbolStrip"] = "false";
			DotNet.AssertBuild (project_path, properties);

			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			var symbols = Configuration.GetNativeSymbols (appExecutable);
			Assert.That (symbols, Does.Contain ("_xamarin_release_managed_ref"), "_xamarin_release_managed_ref");
		}

	}
}
