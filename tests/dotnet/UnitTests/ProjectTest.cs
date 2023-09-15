using System.Diagnostics;
using System.Xml;

using Mono.Cecil;

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
			var expectedFSharpResources = new string [] {
				"FSharpOptimizationCompressedData.fsharplibrary",
				"FSharpSignatureCompressedData.fsharplibrary",
				"FSharpSignatureCompressedDataB.fsharplibrary",
			};
			var actualFSharpResources = ad.MainModule.Resources.Select (v => v.Name).OrderBy (v => v).ToArray ();
			Assert.That (actualFSharpResources, Is.EqualTo (expectedFSharpResources), "F# resources:"); // There are some embedded resources by default by the F# compiler.
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
			var resourceZip = resourceBundle + ".zip";
			if (!Directory.Exists (resourceBundle) && !File.Exists (resourceZip))
				Assert.Fail ($"Neither the sidecar {resourceBundle} or the zipped sidecar {resourceZip} exists.");
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
		// [Category ("AllPlatforms")]
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
		// [Category ("AllPlatforms")] // builds a complete app, not just the binding projects, so can only be built on macOS
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
			var assemblies = FilterToAssembly (lines, assemblyName, true);
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
				Assert.That (Path.Combine (appPath, Configuration.GetBaseLibraryName (platform)), Does.Not.Exist, "Platform assembly is in the bundle");
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
		[TestCase (ApplePlatform.iOS, "tvos-arm64", false)] // valid RID for another platform
		[TestCase (ApplePlatform.TVOS, "tvos-x64", false)] // valid RID in a previous preview (and common mistake)
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x46", true)] // it's x64, not x46
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm", true)] // we don't support this
		[TestCase (ApplePlatform.TVOS, "helloworld", true)] // random text
		[TestCase (ApplePlatform.TVOS, "iossimulator-x64", false)] // valid RID for another platform
		[TestCase (ApplePlatform.MacOSX, "osx-x46", true)] // it's x64, not x46
		[TestCase (ApplePlatform.MacOSX, "macos-arm64", true)] // it's osx, not macos
		[TestCase (ApplePlatform.MacOSX, "helloworld", true)] // random text
		[TestCase (ApplePlatform.MacOSX, "ios-arm64", false)] // valid RID for another platform
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x46", true)] // it's x64, not x46
		[TestCase (ApplePlatform.MacCatalyst, "helloworld", true)] // random text
		[TestCase (ApplePlatform.MacCatalyst, "ios-arm64", false)] // valid RID for another platform
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

		[Category ("Windows")]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.iOS, null)]
		public void LibraryWithResourcesOnWindows (ApplePlatform platform, bool? bundleOriginalResources)
		{
			Configuration.IgnoreIfNotOnWindows ();

			// This should all execute locally on Windows when BundleOriginalResources=true
			LibraryWithResources (platform, anyLibraryResources: bundleOriginalResources == true, bundleOriginalResources: bundleOriginalResources);
		}


		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		public void LibraryWithResourcesOnRemoteWindows (ApplePlatform platform, bool? bundleOriginalResources)
		{
			Configuration.IgnoreIfNotOnWindows ();

			// This should all execute locally on Windows when BundleOriginalResources=true, but either should work
			LibraryWithResources (platform, bundleOriginalResources);
		}

		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.iOS, null)]
		[TestCase (ApplePlatform.TVOS, true)]
		[TestCase (ApplePlatform.TVOS, false)]
		[TestCase (ApplePlatform.TVOS, null)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		[TestCase (ApplePlatform.MacCatalyst, false)]
		[TestCase (ApplePlatform.MacCatalyst, null)]
		[TestCase (ApplePlatform.MacOSX, true)]
		[TestCase (ApplePlatform.MacOSX, false)]
		[TestCase (ApplePlatform.MacOSX, null)]
		public void LibraryWithResources (ApplePlatform platform, bool? bundleOriginalResources, bool anyLibraryResources = true)
		{
			var project = "LibraryWithResources";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var actualBundleOriginalResources = bundleOriginalResources ?? Version.Parse (Configuration.DotNetTfm.Replace ("net", "")).Major >= 10;
			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);

			var properties = GetDefaultProperties ();
			if (bundleOriginalResources.HasValue)
				properties ["BundleOriginalResources"] = bundleOriginalResources.Value ? "true" : "false";

			var rv = DotNet.AssertBuild (project_path, properties);

			var allTargets = BinLog.GetAllTargets (rv.BinLogPath).Where (v => !v.Skipped).Select (v => v.TargetName);
			// https://github.com/xamarin/xamarin-macios/issues/15031
			if (actualBundleOriginalResources) {
				Assert.That (allTargets, Does.Not.Contain ("_CompileAppManifest"), "Didn't execute '_CompileAppManifest'");
				Assert.That (allTargets, Does.Not.Contain ("_DetectSdkLocations"), "Didn't execute '_DetectSdkLocations'");
				Assert.That (allTargets, Does.Not.Contain ("_SayHello"), "Didn't execute '_SayHello'");
			} else {
				Assert.That (allTargets, Does.Contain ("_CompileAppManifest"), "Did execute '_CompileAppManifest'");
				Assert.That (allTargets, Does.Contain ("_DetectSdkLocations"), "Did execute '_DetectSdkLocations'");
				if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform (System.Runtime.InteropServices.OSPlatform.Windows))
					Assert.That (allTargets, Does.Contain ("_SayHello"), "Did execute '_SayHello'");
			}

			var lines = BinLog.PrintToLines (rv.BinLogPath);
			// Find the resulting binding assembly from the build log
			var assemblies = FilterToAssembly (lines, project);
			Assert.That (assemblies, Is.Not.Empty, "Assemblies");
			// Make sure there's no other assembly confusing our logic
			Assert.That (assemblies.Distinct ().Count (), Is.EqualTo (1), "Unique assemblies");
			var asm = assemblies.First ();
			Assert.That (asm, Does.Exist, "Assembly existence");

			using var ad = AssemblyDefinition.ReadAssembly (asm, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			var actualResources = ad.MainModule.Resources.Select (v => v.Name).OrderBy (v => v).ToArray ();

			string [] expectedResources;

			if (anyLibraryResources) {
				var platformPrefix = (platform == ApplePlatform.MacOSX) ? "xammac" : "monotouch";
				if (actualBundleOriginalResources) {
					expectedResources = new string [] {
						$"__{platformPrefix}_content_A.ttc",
						$"__{platformPrefix}_content_B.otf",
						$"__{platformPrefix}_content_C.ttf",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0001.png",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0002.png",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0003.png",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0004.png",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0005.png",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0006.png",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0007.png",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0008.png",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0009.png",
						$"__{platformPrefix}_item_AtlasTexture_Archer__Attack.atlas_sarcher__attack__0010.png",
						$"__{platformPrefix}_item_BundleResource_A.ttc",
						$"__{platformPrefix}_item_BundleResource_B.otf",
						$"__{platformPrefix}_item_BundleResource_C.ttf",
						$"__{platformPrefix}_item_Collada_scene.dae",
						$"__{platformPrefix}_item_CoreMLModel_SqueezeNet.mlmodel",
						$"__{platformPrefix}_item_ImageAsset_Images.xcassets_sContents.json",
						$"__{platformPrefix}_item_ImageAsset_Images.xcassets_sImage.imageset_sContents.json",
						$"__{platformPrefix}_item_ImageAsset_Images.xcassets_sImage.imageset_sIcon16.png",
						$"__{platformPrefix}_item_ImageAsset_Images.xcassets_sImage.imageset_sIcon32.png",
						$"__{platformPrefix}_item_ImageAsset_Images.xcassets_sImage.imageset_sIcon64.png",
						$"__{platformPrefix}_item_InterfaceDefinition_Main.storyboard",
						$"__{platformPrefix}_item_PartialAppManifest_shared.plist",
						$"__{platformPrefix}_item_SceneKitAsset_art.scnassets_sscene.scn",
						$"__{platformPrefix}_item_SceneKitAsset_art.scnassets_stexture.png",
						$"__{platformPrefix}_item_SceneKitAsset_DirWithResources_slinkedArt.scnassets_sscene.scn",
						$"__{platformPrefix}_item_SceneKitAsset_DirWithResources_slinkedArt.scnassets_stexture.png",
					};
				} else {
					var expectedList = new List<string> ();
					expectedList.Add ($"__{platformPrefix}_content_A.ttc");
					expectedList.Add ($"__{platformPrefix}_content_Archer__Attack.atlasc_sArcher__Attack.plist");
					expectedList.Add ($"__{platformPrefix}_content_art.scnassets_sscene.scn");
					expectedList.Add ($"__{platformPrefix}_content_art.scnassets_stexture.png");
					expectedList.Add ($"__{platformPrefix}_content_Assets.car");
					expectedList.Add ($"__{platformPrefix}_content_B.otf");
					expectedList.Add ($"__{platformPrefix}_content_C.ttf");
					expectedList.Add ($"__{platformPrefix}_content_DirWithResources_slinkedArt.scnassets_sscene.scn");
					expectedList.Add ($"__{platformPrefix}_content_DirWithResources_slinkedArt.scnassets_stexture.png");
					expectedList.Add ($"__{platformPrefix}_content_scene.dae");
					switch (platform) {
					case ApplePlatform.iOS:
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sBYZ-38-t0r-view-8bC-Xf-vdC.nib");
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sInfo.plist");
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sUIViewController-BYZ-38-t0r.nib");
						break;
					case ApplePlatform.TVOS:
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sBYZ-38-t0r-view-8bC-Xf-vdC.nib");
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sInfo.plist");
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sUIViewController-BYZ-38-t0r.nib");
						break;
					case ApplePlatform.MacCatalyst:
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_s1-view-2.nib");
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sInfo.plist");
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sUIViewController-1.nib");
						break;
					case ApplePlatform.MacOSX:
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sInfo.plist");
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sMainMenu.nib");
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sNSWindowController-B8D-0N-5wS.nib");
						expectedList.Add ($"__{platformPrefix}_content_Main.storyboardc_sXfG-lQ-9wD-view-m2S-Jp-Qdl.nib");
						break;
					}
					expectedList.Add ($"__{platformPrefix}_content_SqueezeNet.mlmodelc_sanalytics_scoremldata.bin");
					expectedList.Add ($"__{platformPrefix}_content_SqueezeNet.mlmodelc_scoremldata.bin");
					expectedList.Add ($"__{platformPrefix}_content_SqueezeNet.mlmodelc_smetadata.json");
					expectedList.Add ($"__{platformPrefix}_content_SqueezeNet.mlmodelc_smodel.espresso.net");
					expectedList.Add ($"__{platformPrefix}_content_SqueezeNet.mlmodelc_smodel.espresso.shape");
					expectedList.Add ($"__{platformPrefix}_content_SqueezeNet.mlmodelc_smodel.espresso.weights");
					expectedList.Add ($"__{platformPrefix}_content_SqueezeNet.mlmodelc_smodel_scoremldata.bin");
					expectedList.Add ($"__{platformPrefix}_content_SqueezeNet.mlmodelc_sneural__network__optionals_scoremldata.bin");
					expectedResources = expectedList.ToArray ();
				}
			} else {
				expectedResources = new string [0];
			}
			CollectionAssert.AreEquivalent (expectedResources, actualResources, "Resources");
		}

		[TestCase (ApplePlatform.iOS, "ios-arm64", false)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64", false)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64", true)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64", false)]
		public void AppWithLibraryWithResourcesReference (ApplePlatform platform, string runtimeIdentifiers, bool bundleOriginalResources)
		{
			AppWithLibraryWithResourcesReferenceImpl (platform, runtimeIdentifiers, bundleOriginalResources, false, false);
		}


		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, "ios-arm64", false)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true)]
		public void AppWithLibraryWithResourcesReferenceOnRemoteWindows (ApplePlatform platform, string runtimeIdentifiers, bool bundleOriginalResources)
		{
			Configuration.IgnoreIfNotOnWindows ();

			AppWithLibraryWithResourcesReferenceImpl (platform, runtimeIdentifiers, bundleOriginalResources, true, false);
		}

		[Category ("Windows")]
		[TestCase (ApplePlatform.iOS, "ios-arm64", false)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true)]
		public void AppWithLibraryWithResourcesReferenceWithHotRestart (ApplePlatform platform, string runtimeIdentifiers, bool bundleOriginalResources)
		{
			Configuration.IgnoreIfNotOnWindows ();

			AppWithLibraryWithResourcesReferenceImpl (platform, runtimeIdentifiers, bundleOriginalResources, false, isUsingHotRestart: true);
		}

		void AppWithLibraryWithResourcesReferenceImpl (ApplePlatform platform, string runtimeIdentifiers, bool bundleOriginalResources, bool remoteWindows, bool isUsingHotRestart)
		{
			var project = "AppWithLibraryWithResourcesReference";
			var config = bundleOriginalResources ? "DebugOriginal" : "DebugCompiled";

			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var library_project = GetProjectPath ("LibraryWithResources", platform: platform);
			Clean (library_project);
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: config);
			Clean (project_path);

			Dictionary<string, string>? extraProperties = null;
			string? tmpdir;
			string? hotRestartOutputDir = null;
			string? hotRestartAppBundlePath = null;
			if (isUsingHotRestart) {
				tmpdir = Cache.CreateTemporaryDirectory ();
				extraProperties = GetHotRestartProperties (tmpdir, out hotRestartOutputDir, out hotRestartAppBundlePath);
			}

			var properties = GetDefaultProperties (runtimeIdentifiers, extraProperties);
			properties ["Configuration"] = config;
			properties ["BundleOriginalResources"] = bundleOriginalResources ? "true" : "false";
			if (remoteWindows) {
				// Copy the app bundle to Windows so that we can inspect the results.
				properties ["CopyAppBundleToWindows"] = "true";
			}

			var rv = DotNet.AssertBuild (project_path, properties);

			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			var appBundleInfo = new AppBundleInfo (platform, appPath, project_path, remoteWindows, runtimeIdentifiers, config, isUsingHotRestart, hotRestartOutputDir, hotRestartAppBundlePath);
			var appBundleContents = appBundleInfo.GetAppBundleFiles (true).ToHashSet ();

			appBundleInfo.DumpAppBundleContents ();

			Assert.Multiple (() => {
				var resourcesDirectory = GetResourcesDirectory (platform, "");

				var fontDirectory = resourcesDirectory;
				var fontAFile = Path.Combine (fontDirectory, "A.ttc");
				var fontBFile = Path.Combine (fontDirectory, "B.otf");
				var fontCFile = Path.Combine (fontDirectory, "C.ttf");

				Assert.That (appBundleContents, Does.Contain (fontAFile), "A.ttc existence");
				Assert.That (appBundleContents, Does.Contain (fontBFile), "B.otf existence");
				Assert.That (appBundleContents, Does.Contain (fontCFile), "C.ttf existence");

				var atlasTexture = Path.Combine (resourcesDirectory, "Archer_Attack.atlasc", "Archer_Attack.plist");
				AssertExistsOrUsingHotRestart (atlasTexture, "AtlasTexture - Archer_Attack");

				var scnAssetsDir = Path.Combine (resourcesDirectory, "art.scnassets");
				AssertExistsOrUsingHotRestart (Path.Combine (scnAssetsDir, "scene.scn"), "scene.scn");
				AssertExistsOrUsingHotRestart (Path.Combine (scnAssetsDir, "texture.png"), "texture.png");

				AssertExistsOrUsingHotRestart (Path.Combine (resourcesDirectory, "Assets.car"), "Assets.car");

				AssertExistsOrUsingHotRestart (Path.Combine (resourcesDirectory, "DirWithResources", "linkedArt.scnassets", "scene.scn"), "DirWithResources/linkedArt.scnassets/scene.scn");
				AssertExistsOrUsingHotRestart (Path.Combine (resourcesDirectory, "DirWithResources", "linkedArt.scnassets", "texture.png"), "DirWithResources/linkedArt.scnassets/texture.png");

				var mainStoryboard = Path.Combine (resourcesDirectory, "Main.storyboardc");
				AssertExistsOrUsingHotRestart (mainStoryboard, "Main.storyboardc");
				AssertExistsOrUsingHotRestart (Path.Combine (mainStoryboard, "Info.plist"), "Main.storyboardc/Info.plist");

				var colladaScene = Path.Combine (resourcesDirectory, "scene.dae");
				AssertExistsOrUsingHotRestart (colladaScene, "Collada - scene.dae");

				var mlModel = Path.Combine (resourcesDirectory, "SqueezeNet.mlmodelc");
				AssertExistsOrUsingHotRestart (mlModel, "CoreMLModel");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "analytics"), "CoreMLModel/analytics");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "analytics", "coremldata.bin"), "CoreMLModel/analytics/coremldata.bin");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "coremldata.bin"), "CoreMLModel/coremldata.bin");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "metadata.json"), "CoreMLModel/metadata.json");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "model"), "CoreMLModel/model");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "model.espresso.net"), "CoreMLModel/model.espresso.net");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "model.espresso.shape"), "CoreMLModel/model.espresso.shape");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "model.espresso.weights"), "CoreMLModel/model.espresso.weights");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "model", "coremldata.bin"), "CoreMLModel/model/coremldata.bin");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "neural_network_optionals"), "CoreMLModel/neural_network_optionals");
				AssertExistsOrUsingHotRestart (Path.Combine (mlModel, "neural_network_optionals", "coremldata.bin"), "CoreMLModel/neural_network_optionals/coremldata.bin");

				if (bundleOriginalResources) {
					var infoPlist = appBundleInfo.GetFile (GetInfoPListPath (platform, ""));
					var appManifest = PDictionary.FromByteArray (infoPlist, out var _)!;
					Assert.AreEqual ("Here I am", appManifest.GetString ("LibraryWithResources").Value, "Partial plist entry");
				}
			});

			void AssertExistsOrUsingHotRestart (string path, string message)
			{
				var exists = appBundleContents.Contains (path);
				if (exists ^ isUsingHotRestart)
					return;
				Assert.Fail ($"Expected either hot restart to be enabled ({isUsingHotRestart}) or the file '{path}' to be in the app bundle ({exists}): {message}");
			}
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
			foreach (var rx in new string [] { "BindingWithDefaultCompileInclude", "bindings-framework-test" }) {
				var zip = Path.Combine (bindir, $"{rx}.resources.zip");
				var hasCompressedResources = File.Exists (zip);
				var hasDirectoryResources = Directory.Exists (Path.Combine (bindir, $"{rx}.resources"));
				if (!hasDirectoryResources && !hasCompressedResources)
					Assert.Fail ($"Could not find either {rx}.resources.zip or {rx}.resources in {bindir}");


				if (hasDirectoryResources)
					continue;

				var zipContents = ZipHelpers.List (zip).ToHashSet ();
				var mustHaveContents = new List<string> {
					"manifest",
				};
				var mayHaveContents = new List<string> ();
				List<string> addHere;

				if (rx == "bindings-framework-test") {
					foreach (var lib in new string [] { "XStaticArTest", "XStaticObjectTest" }) {
						addHere = Configuration.include_tvos ? mustHaveContents : mayHaveContents;
						addHere.AddRange (new string [] {
							$"{lib}.xcframework/tvos-arm64",
							$"{lib}.xcframework/tvos-arm64/{lib}.framework",
							$"{lib}.xcframework/tvos-arm64/{lib}.framework/{lib}",
							$"{lib}.xcframework/tvos-arm64_x86_64-simulator",
							$"{lib}.xcframework/tvos-arm64_x86_64-simulator/{lib}.framework",
							$"{lib}.xcframework/tvos-arm64_x86_64-simulator/{lib}.framework/{lib}",
						});

						addHere = Configuration.include_mac ? mustHaveContents : mayHaveContents;
						addHere.AddRange (new string [] {
							$"{lib}.xcframework/macos-arm64_x86_64",
							$"{lib}.xcframework/macos-arm64_x86_64/{lib}.framework",
							$"{lib}.xcframework/macos-arm64_x86_64/{lib}.framework/{lib}",
						});

						addHere = Configuration.include_maccatalyst ? mustHaveContents : mayHaveContents;
						addHere.AddRange (new string [] {
							$"{lib}.xcframework/ios-arm64_x86_64-maccatalyst",
							$"{lib}.xcframework/ios-arm64_x86_64-maccatalyst/{lib}.framework",
							$"{lib}.xcframework/ios-arm64_x86_64-maccatalyst/{lib}.framework/{lib}",
						});

						addHere = Configuration.include_ios ? mustHaveContents : mayHaveContents;
						addHere.AddRange (new string [] {
							$"{lib}.xcframework/ios-arm64_x86_64-simulator",
							$"{lib}.xcframework/ios-arm64_x86_64-simulator/{lib}.framework",
							$"{lib}.xcframework/ios-arm64_x86_64-simulator/{lib}.framework/{lib}",
							$"{lib}.xcframework/ios-arm64",
							$"{lib}.xcframework/ios-arm64/{lib}.framework",
							$"{lib}.xcframework/ios-arm64/{lib}.framework/{lib}",
						});

						mustHaveContents.AddRange (new string [] {
							$"{lib}.xcframework",
							$"{lib}.xcframework/Info.plist",
						});
					}
				}

				mustHaveContents.AddRange (new string [] {
					"XTest.xcframework",
					"XTest.xcframework/Info.plist",
				});

				addHere = Configuration.include_ios ? mustHaveContents : mayHaveContents;
				addHere.AddRange (new string [] {
					"XTest.xcframework/ios-arm64",
					"XTest.xcframework/ios-arm64/XTest.framework",
					"XTest.xcframework/ios-arm64/XTest.framework/Info.plist",
					"XTest.xcframework/ios-arm64/XTest.framework/XTest",
					"XTest.xcframework/ios-arm64_x86_64-simulator",
					"XTest.xcframework/ios-arm64_x86_64-simulator/XTest.framework",
					"XTest.xcframework/ios-arm64_x86_64-simulator/XTest.framework/Info.plist",
					"XTest.xcframework/ios-arm64_x86_64-simulator/XTest.framework/XTest",
				});

				addHere = Configuration.include_maccatalyst ? mustHaveContents : mayHaveContents;
				addHere.AddRange (new string [] {
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst",
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework",
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Resources",
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Versions",
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Versions/A",
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Versions/A/Resources",
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Versions/A/Resources/Info.plist",
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Versions/A/XTest",
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Versions/Current",
					"XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/XTest",
				});

				addHere = Configuration.include_mac ? mustHaveContents : mayHaveContents;
				addHere.AddRange (new string [] {
					"XTest.xcframework/macos-arm64_x86_64",
					"XTest.xcframework/macos-arm64_x86_64/XTest.framework",
					"XTest.xcframework/macos-arm64_x86_64/XTest.framework/Resources",
					"XTest.xcframework/macos-arm64_x86_64/XTest.framework/Versions",
					"XTest.xcframework/macos-arm64_x86_64/XTest.framework/Versions/A",
					"XTest.xcframework/macos-arm64_x86_64/XTest.framework/Versions/A/Resources",
					"XTest.xcframework/macos-arm64_x86_64/XTest.framework/Versions/A/Resources/Info.plist",
					"XTest.xcframework/macos-arm64_x86_64/XTest.framework/Versions/A/XTest",
					"XTest.xcframework/macos-arm64_x86_64/XTest.framework/Versions/Current",
					"XTest.xcframework/macos-arm64_x86_64/XTest.framework/XTest",
				});

				addHere = Configuration.include_tvos ? mustHaveContents : mayHaveContents;
				addHere.AddRange (new string [] {
					"XTest.xcframework/tvos-arm64",
					"XTest.xcframework/tvos-arm64/XTest.framework",
					"XTest.xcframework/tvos-arm64/XTest.framework/Info.plist",
					"XTest.xcframework/tvos-arm64/XTest.framework/XTest",
					"XTest.xcframework/tvos-arm64_x86_64-simulator",
					"XTest.xcframework/tvos-arm64_x86_64-simulator/XTest.framework",
					"XTest.xcframework/tvos-arm64_x86_64-simulator/XTest.framework/Info.plist",
					"XTest.xcframework/tvos-arm64_x86_64-simulator/XTest.framework/XTest",
				});

				var missing = mustHaveContents.ToHashSet ().Except (zipContents);
				Assert.That (missing, Is.Empty, "No missing files");

				var extra = zipContents.Except (mustHaveContents).Except (mayHaveContents);
				Assert.That (extra, Is.Empty, "No extra files");
			}
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

		IEnumerable<string> FilterToAssembly (IEnumerable<string> lines, string assemblyName, bool doAppCheckInsteadOfRefCheck = false)
		{
			var rv = FilterToAssembly2 (lines, assemblyName, false, doAppCheckInsteadOfRefCheck);
			if (!rv.Any ()) {
				Console.WriteLine ($"Could not find any matching lines of {lines.Count ()} lines matching {assemblyName}");
				rv = FilterToAssembly2 (lines, assemblyName, true, doAppCheckInsteadOfRefCheck);
			}
			return rv;
		}

		IEnumerable<string> FilterToAssembly2 (IEnumerable<string> lines, string assemblyName, bool log, bool doAppCheckInsteadOfRefCheck = false)
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
					if (!(v.Contains ("/bin/", StringComparison.Ordinal) || v.Contains ("\\bin\\", StringComparison.Ordinal))) {
						if (log)
							Console.WriteLine ($"    4: {v}");
						return false;
					}
					if (!doAppCheckInsteadOfRefCheck && v.Contains (Path.DirectorySeparatorChar + "ref" + Path.DirectorySeparatorChar, StringComparison.Ordinal)) {
						if (log)
							Console.WriteLine ($"    5a: {v}");
						return false; // Skip reference assemblies
					}
					if (doAppCheckInsteadOfRefCheck && !v.Contains ($"{assemblyName}.app", StringComparison.Ordinal)) {
						if (log)
							Console.WriteLine ($"    5b: {v}");
						return false;
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


		[TestCase (ApplePlatform.iOS, "ios-arm64", false)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true)]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", false)]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", true)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", false)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64;osx-arm64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64;osx-arm64", true)]
		// [TestCase ("MacCatalyst", "")] - No extension support yet
		public void BuildProjectsWithExtensions (ApplePlatform platform, string runtimeIdentifier, bool isNativeAot)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var consumingProjectDir = GetProjectPath ("ExtensionConsumer", runtimeIdentifier, platform, out var appPath);
			var extensionProjectDir = GetProjectPath ("ExtensionProject", platform: platform);

			Clean (extensionProjectDir);
			Clean (consumingProjectDir);

			var properties = GetDefaultProperties (runtimeIdentifier);

			if (isNativeAot) {
				properties ["PublishAot"] = "true";
				properties ["_IsPublishing"] = "true";
			}

			DotNet.AssertBuild (consumingProjectDir, properties);

			var extensionPath = Path.Combine (appPath, GetPlugInsRelativePath (platform), "ExtensionProject.appex");
			Assert.That (Directory.Exists (extensionPath), $"App extension directory does not exist: {extensionPath}");

			var pathToSearch = Path.Combine (Path.GetDirectoryName (consumingProjectDir)!, "bin", "Debug");
			string [] configFiles = Directory.GetFiles (pathToSearch, "*.runtimeconfig.*", SearchOption.AllDirectories);
			Assert.AreNotEqual (0, configFiles.Length, "runtimeconfig.json file does not exist");
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-x64", false)]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", true)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", false)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64;osx-arm64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-x64;osx-arm64", true)]
		// [TestCase ("MacCatalyst", "")] - No extension support yet
		public void BuildProjectsWithExtensionsAndFrameworks (ApplePlatform platform, string runtimeIdentifier, bool isNativeAot)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var consumingProjectDir = GetProjectPath ("ExtensionConsumerWithFrameworks", runtimeIdentifiers: runtimeIdentifier, platform: platform, out var appPath);
			var extensionProjectDir = GetProjectPath ("ExtensionProjectWithFrameworks", platform: platform);

			Clean (extensionProjectDir);
			Clean (consumingProjectDir);

			var properties = GetDefaultProperties (runtimeIdentifier);

			if (isNativeAot) {
				properties ["PublishAot"] = "true";
				properties ["_IsPublishing"] = "true";
			}

			DotNet.AssertBuild (consumingProjectDir, properties);

			var extensionPath = Path.Combine (appPath, GetPlugInsRelativePath (platform), "ExtensionProjectWithFrameworks.appex");
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
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifier, appExecutable);
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

		[Category ("Multiplatform")]
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

			// We want RuntimeIdentifier(s) so to be specified in a file, otherwise they're applied to the library
			// project as well, and that might not be valid.
			var properties = GetDefaultProperties (runtimeIdentifiers);
			if (properties.ContainsKey ("RuntimeIdentifiers")) {
				properties ["file:RuntimeIdentifiers"] = properties ["RuntimeIdentifiers"];
				properties.Remove ("RuntimeIdentifiers");
			}
			if (properties.ContainsKey ("RuntimeIdentifier")) {
				properties ["file:RuntimeIdentifier"] = properties ["RuntimeIdentifier"];
				properties.Remove ("RuntimeIdentifier");
			}

			DotNet.AssertBuild (project_path, properties);

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
			properties ["ExcludeNUnitLiteReference"] = "true";
			properties ["ExcludeTouchUnitReference"] = "true";
			// This is to prevent this type of errors:
			//     Unable to find package Microsoft.NETCore.App.Ref with version (= 6.0.27)
			// which happens when we don't have a feed for the Microsoft.NETCore.App.Ref version in question
			// (the specific package version may in fact not exist yet - which happens sometimes for maestro bumps,
			// and if that happens, we can do nothing but wait, which may take a while).
			// This works around the problem by just skipping the reference to the Microsoft.NETCore.App.Ref package,
			// which we don't need for this test anyway.
			properties ["DisableImplicitFrameworkReferences"] = "true";

			var result = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (result.BinLogPath).ToList ();

			// Due to an implementation detail in .NET, the same error message is shown twice.
			var targetFramework = $"net6.0-{platform.AsString ().ToLowerInvariant ()}";
			AssertErrorMessages (errors,
				$"The workload '{targetFramework}' is out of support and will not receive security updates in the future. Please refer to https://aka.ms/maui-support-policy for more information about the support policy.",
				$"The workload '{targetFramework}' is out of support and will not receive security updates in the future. Please refer to https://aka.ms/maui-support-policy for more information about the support policy.");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void BuildNetFutureApp (ApplePlatform platform, string runtimeIdentifiers)
		{
			// Builds an app with a higher .NET version than we support (for instance 'net9.0-ios' when we support 'net8.0-ios')
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var majorNetVersion = Version.Parse (Configuration.DotNetTfm.Replace ("net", "")).Major;
			var netVersion = $"net{majorNetVersion + 1}.0";
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, netVersion: netVersion);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			var targetFramework = platform.ToFramework (netVersion);
			properties ["TargetFramework"] = targetFramework;
			properties ["ExcludeNUnitLiteReference"] = "true";
			properties ["ExcludeTouchUnitReference"] = "true";

			var result = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (result.BinLogPath).ToList ();

			AssertErrorMessages (errors,
				$"The current .NET SDK does not support targeting .NET {majorNetVersion + 1}.0.  Either target .NET {majorNetVersion}.0 or lower, or use a version of the .NET SDK that supports .NET {majorNetVersion + 1}.0. Download the .NET SDK from https://aka.ms/dotnet/download");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void BuildNet7_0App (ApplePlatform platform, string runtimeIdentifiers)
		{
			var tfm = "net7.0";
			var project = "Net7_0SimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, netVersion: tfm);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			var result = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (result.BinLogPath).ToList ();
			// Due to an implementation detail in .NET, the same error message is shown twice.
			var targetFramework = $"{tfm}-{platform.AsString ().ToLowerInvariant ()}";
			AssertErrorMessages (errors,
				$"The workload '{targetFramework}' is out of support and will not receive security updates in the future. Please refer to https://aka.ms/maui-support-policy for more information about the support policy.",
				$"The workload '{targetFramework}' is out of support and will not receive security updates in the future. Please refer to https://aka.ms/maui-support-policy for more information about the support policy.");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void BuildNet8_0App (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "Net8_0SimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, netVersion: "net8.0");
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			var result = DotNet.AssertBuild (project_path, properties);
			AssertThatLinkerExecuted (result);
			var infoPlistPath = GetInfoPListPath (platform, appPath);
			Assert.That (infoPlistPath, Does.Exist, "Info.plist");
			var infoPlist = PDictionary.FromFile (infoPlistPath)!;
			Assert.AreEqual ("com.xamarin.mysimpleapp", infoPlist.GetString ("CFBundleIdentifier").Value, "CFBundleIdentifier");
			Assert.AreEqual ("MySimpleApp", infoPlist.GetString ("CFBundleDisplayName").Value, "CFBundleDisplayName");
			Assert.AreEqual ("8.0", infoPlist.GetString ("CFBundleVersion").Value, "CFBundleVersion");
			Assert.AreEqual ("8.0", infoPlist.GetString ("CFBundleShortVersionString").Value, "CFBundleShortVersionString");

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

		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void PluralRuntimeIdentifiers (ApplePlatform platform, string runtimeIdentifiers)
		{
			PluralRuntimeIdentifiersImpl (platform, runtimeIdentifiers);
		}

		internal static void PluralRuntimeIdentifiersImpl (ApplePlatform platform, string runtimeIdentifiers, Dictionary<string, string>? extraProperties = null, bool isUsingHotRestart = false)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (extraProperties: extraProperties);
			properties ["RuntimeIdentifiers"] = runtimeIdentifiers;
			if (isUsingHotRestart) {
				var rv = DotNet.AssertBuildFailure (project_path, properties);
				var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
				AssertErrorMessages (errors, $"Hot Restart is not supported when 'RuntimeIdentifiers' (plural) is set. Use 'RuntimeIdentifier' (singular) instead.");
			} else {
				DotNet.AssertBuild (project_path, properties);
			}
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

			Clean (project_path);
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

			// Verify that a Resources subdirectory causes the build to fail.
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				var invalidDir = Path.Combine (appPath, "Resources");
				Directory.CreateDirectory (invalidDir);

				// First verify that we get our customized error message
				var buildFailure = DotNet.AssertBuildFailure (project_path, properties);
				var errors = BinLog.GetBuildLogErrors (buildFailure.BinLogPath).ToArray ();
				AssertErrorMessages (errors, $"The app bundle '{appPath}' contains a subdirectory named 'Resources'. This is not allowed on this platform. Typically resource files should be in the root directory of the app bundle (or a custom subdirectory, but named anything other than 'Resources').");

				// Then disable our customized error message, but the build will still fail, now with codesign's error message
				properties ["CodesignDisallowResourcesSubdirectoryInAppBundle"] = "false";
				buildFailure = DotNet.AssertBuildFailure (project_path, properties);
				errors = BinLog.GetBuildLogErrors (buildFailure.BinLogPath).ToArray ();
				var errorMessagePrefixes = new string []
				{
					$"/usr/bin/codesign exited with code 1:\n" +
					$"{appPath}: replacing existing signature\n" +
					$"{appPath}: code object is not signed at all\n",

					$"Failed to codesign '{appPath}': {appPath}: replacing existing signature\n" +
					$"{appPath}: code object is not signed at all\n",
				};

				AssertErrorMessages (errors,
					errorMessagePrefixes.Select (prefix => new Func<string, bool> ((msg) => msg.StartsWith (prefix))).ToArray (),
					errorMessagePrefixes.Select (prefix => new Func<string> (() => prefix)).ToArray ()
				);

				// Remove the dir, and now the build should succeed again.
				properties.Remove ("CodesignDisallowResourcesSubdirectoryInAppBundle");
				Directory.Delete (invalidDir);
				DotNet.AssertBuild (project_path, properties);
				break;
			}
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

			AssertThatDylibExistsAndIsReidentified (appPath, "libframework.dylib");
			AssertThatDylibExistsAndIsReidentified (appPath, "/subdir/libframework.dylib");
			AssertThatDylibExistsAndIsReidentified (appPath, "/subdir/libframework.so");

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
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		public void CompressedXCFrameworkInBindingProjectApp (ApplePlatform platform)
		{
			var project = "CompressedXCFrameworkInBindingProjectApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			DotNet.AssertBuild (project_path, properties);

			var appExecutable = GetNativeExecutable (platform, appPath);
			Assert.That (appExecutable, Does.Exist, "There is an executable");

			if (CanExecute (platform, properties)) {
				ExecuteWithMagicWordAndAssert (appExecutable);
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
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void BuildMyNativeAotAppWithTrimAnalysisWarning (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MyNativeAotAppWithTrimAnalysisWarning";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			var rv = DotNet.AssertBuild (project_path, properties);

			// We expect to get a warning from the trim analzyer in Debug build
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();

			// Ignore warnings we haven't fixed yet
			if (platform == ApplePlatform.iOS) {
				warnings = warnings.Where (w => w.Message?.Trim () != "Supported iPhone orientations have not been set").ToArray ();
			}

			Assert.AreEqual (1, warnings.Length, "Warning count");
			Assert.AreEqual (warnings [0].Code, "IL2075", "Warning code");
			Assert.AreEqual (warnings [0].Message, "'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicProperties' in call to 'System.Type.GetProperties()'. The return value of method 'System.Object.GetType()' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "Debug")]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "Release")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", "Debug")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", "Release")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64", "Debug")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64", "Release")]
		[TestCase (ApplePlatform.MacOSX, "", "Release")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", "Debug")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", "Release")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", "Debug")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", "Release")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", "Debug")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", "Release")]
		public void PublishAot (ApplePlatform platform, string runtimeIdentifiers, string configuration)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			properties ["PublishAot"] = "true";
			properties ["_IsPublishing"] = "true"; // quack like "dotnet publish"
			properties ["ExcludeNUnitLiteReference"] = "true"; // we're asserting no warnings, and NUnitLite produces a lot of them, so ignore NUnitLite
			properties ["ExcludeTouchUnitReference"] = "true"; // we're asserting no warnings, and Touch.Unit produces a lot of them, so ignore Touch.Unit
			properties ["TrimmerSingleWarn"] = "false"; // don't be shy, we want to know what the problem is
			var rv = DotNet.AssertBuild (project_path, properties);

			Assert.True (Directory.Exists (appPath), $"App file expected at: {appPath}");

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
			var minSupportedOSVersion = GetMinSupportedOSPlatformVersion (platform);
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
				.OrderBy (v => v)
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

		string GetMinSupportedOSPlatformVersion (ApplePlatform platform)
		{
			return Configuration.GetVariable ($"DOTNET_MIN_{platform.AsString ().ToUpperInvariant ()}_SDK_VERSION", "unknown MinSupportedOSPlatformVersion");
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

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64;", "iOS")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64", "macOS")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", "MacCatalyst")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64;", "tvOS")]
		public void SourcelinkTest (ApplePlatform platform, string runtimeIdentifiers, string platformName)
		{
			// Sourcelink uses the latest commit and tests to see if
			// it is valid which will not pass until the commit has
			// been merged in and actually exists on github.

			if (!IsInCI || IsPullRequest)
				Assert.Ignore ("This test is disabled for local runs and Pull Requests.");

			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			DotNet.AssertBuild (project_path, properties);

			var pdbFile = Directory
				.GetFiles (Path.GetDirectoryName (project_path)!, $"Microsoft.{platformName}.pdb", SearchOption.AllDirectories)
				.FirstOrDefault ();

			Assert.NotNull (pdbFile, "No PDB file found");

			var tool = "sourcelink";
			var toolPath = Directory.GetCurrentDirectory ();
			DotNet.InstallTool (tool, toolPath);
			var test = DotNet.RunTool (Path.Combine (toolPath, tool), "test", pdbFile!);

			Assert.AreEqual ($"sourcelink test passed: {pdbFile}", test.StandardOutput.ToString ().TrimEnd ('\n'));
		}


		[Test]
		// [TestCase (ApplePlatform.iOS)] // Skipping because we're not executing tvOS apps anyway (but it should work)
		// [TestCase (ApplePlatform.TVOS)] // Skipping because we're not executing tvOS apps anyway (but it should work)
		[TestCase (ApplePlatform.MacOSX)] // https://github.com/dotnet/runtime/issues/102730
		[TestCase (ApplePlatform.MacCatalyst)]
		public void RaisesAppDomainUnhandledExceptionEvent (ApplePlatform platform)
		{
			var project = "ExceptionalTestApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties ();
			DotNet.AssertBuild (project_path, properties);

			if (CanExecute (platform, runtimeIdentifiers)) {
				var env = new Dictionary<string, string?> {
					{ "EXCEPTIONAL_TEST_CASE", "1" },
				};
				var appExecutable = GetNativeExecutable (platform, appPath);
				var output = ExecuteWithMagicWordAndAssert (appExecutable, env);
			}
		}

		bool FindAOTedAssemblyFile (string path, string dllName)
		{
			var aotedAssemblyFileName = $"{dllName}.o";
			foreach (string file in Directory.GetFiles (path, "*.o", SearchOption.AllDirectories)) {
				if (Path.GetFileName (file).Equals (aotedAssemblyFileName, StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
			}

			return false;
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "-all,System.Private.CoreLib")]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "all,-System.Private.CoreLib")]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64", "-all,System.Private.CoreLib")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64", "all,-System.Private.CoreLib")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64", "")]
		public void DedupEnabledTest (ApplePlatform platform, string runtimeIdentifiers, string mtouchInterpreter)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["MtouchInterpreter"] = $"\"{mtouchInterpreter}\"";

			DotNet.AssertBuild (project_path, properties);

			var objDir = GetObjDir (project_path, platform, runtimeIdentifiers);
			Assert.True (FindAOTedAssemblyFile (objDir, "aot-instances.dll"), $"Dedup optimization should be enabled for AOT compilation on: {platform} with RID: {runtimeIdentifiers}");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", "-all,System.Private.CoreLib")]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", "all,-System.Private.CoreLib")]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", "")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", "-all,System.Private.CoreLib")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", "all,-System.Private.CoreLib")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", "")]
		public void DedupDisabledTest (ApplePlatform platform, string runtimeIdentifiers, string mtouchInterpreter)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["MtouchInterpreter"] = $"\"{mtouchInterpreter}\"";

			DotNet.AssertBuild (project_path, properties);

			var objDir = GetObjDir (project_path, platform, runtimeIdentifiers);
			Assert.False (FindAOTedAssemblyFile (objDir, "aot-instances.dll"), $"Dedup optimization should not be enabled for AOT compilation on: {platform} with RID: {runtimeIdentifiers}");
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", "-all,System.Private.CoreLib")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", "all,-System.Private.CoreLib")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64", "")]
		public void DedupUniversalAppTest (ApplePlatform platform, string runtimeIdentifiers, string mtouchInterpreter)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["MtouchInterpreter"] = $"\"{mtouchInterpreter}\"";

			DotNet.AssertBuild (project_path, properties);

			var objDir = GetObjDir (project_path, platform, runtimeIdentifiers);
			var objDirMacCatalystArm64 = Path.Combine (objDir, "maccatalyst-arm64");
			Assert.True (FindAOTedAssemblyFile (objDirMacCatalystArm64, "aot-instances.dll"), $"Dedup optimization should be enabled for AOT compilation on: {platform} with RID: maccatalyst-arm64");

			var objDirMacCatalystx64 = Path.Combine (objDir, "maccatalyst-x64");
			Assert.False (FindAOTedAssemblyFile (objDirMacCatalystx64, "aot-instances.dll"), $"Dedup optimization should not be enabled for AOT compilation on: {platform} with RID: maccatalyst-x64");

			var appExecutable = GetNativeExecutable (platform, appPath);

			if (CanExecute (platform, runtimeIdentifiers)) {
				ExecuteWithMagicWordAndAssert (appExecutable);
			}
		}

		[Test]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		public void AppendRuntimeIdentifierToOutputPath_DisableCommandLine (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["cmdline:AppendRuntimeIdentifierToOutputPath"] = "false";
			DotNet.AssertBuild (project_path, properties);
		}

		[Test]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		public void AppendRuntimeIdentifierToOutputPath_DisableDirectoryBuildProps (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["SetAppendRuntimeIdentifierToOutputPathToFalse"] = "true";
			DotNet.AssertBuild (project_path, properties);
		}

		static string [] expectedFrameworks_iOS_None = [
			"/System/Library/Frameworks/Accelerate.framework/Accelerate",
			"/System/Library/Frameworks/Accessibility.framework/Accessibility",
			"/System/Library/Frameworks/AccessorySetupKit.framework/AccessorySetupKit",
			"/System/Library/Frameworks/Accounts.framework/Accounts",
			"/System/Library/Frameworks/AddressBook.framework/AddressBook",
			"/System/Library/Frameworks/AddressBookUI.framework/AddressBookUI",
			"/System/Library/Frameworks/AdServices.framework/AdServices",
			"/System/Library/Frameworks/AdSupport.framework/AdSupport",
			"/System/Library/Frameworks/AppClip.framework/AppClip",
			"/System/Library/Frameworks/AppTrackingTransparency.framework/AppTrackingTransparency",
			"/System/Library/Frameworks/ARKit.framework/ARKit",
			"/System/Library/Frameworks/AudioToolbox.framework/AudioToolbox",
			"/System/Library/Frameworks/AuthenticationServices.framework/AuthenticationServices",
			"/System/Library/Frameworks/AutomaticAssessmentConfiguration.framework/AutomaticAssessmentConfiguration",
			"/System/Library/Frameworks/AVFoundation.framework/AVFoundation",
			"/System/Library/Frameworks/AVKit.framework/AVKit",
			"/System/Library/Frameworks/AVRouting.framework/AVRouting",
			"/System/Library/Frameworks/BackgroundAssets.framework/BackgroundAssets",
			"/System/Library/Frameworks/BackgroundTasks.framework/BackgroundTasks",
			"/System/Library/Frameworks/BusinessChat.framework/BusinessChat",
			"/System/Library/Frameworks/CallKit.framework/CallKit",
			"/System/Library/Frameworks/CarPlay.framework/CarPlay",
			"/System/Library/Frameworks/CFNetwork.framework/CFNetwork",
			"/System/Library/Frameworks/Cinematic.framework/Cinematic",
			"/System/Library/Frameworks/ClassKit.framework/ClassKit",
			"/System/Library/Frameworks/CloudKit.framework/CloudKit",
			"/System/Library/Frameworks/Contacts.framework/Contacts",
			"/System/Library/Frameworks/ContactsUI.framework/ContactsUI",
			"/System/Library/Frameworks/CoreAudioKit.framework/CoreAudioKit",
			"/System/Library/Frameworks/CoreBluetooth.framework/CoreBluetooth",
			"/System/Library/Frameworks/CoreData.framework/CoreData",
			"/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation",
			"/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics",
			"/System/Library/Frameworks/CoreHaptics.framework/CoreHaptics",
			"/System/Library/Frameworks/CoreImage.framework/CoreImage",
			"/System/Library/Frameworks/CoreLocation.framework/CoreLocation",
			"/System/Library/Frameworks/CoreLocationUI.framework/CoreLocationUI",
			"/System/Library/Frameworks/CoreMedia.framework/CoreMedia",
			"/System/Library/Frameworks/CoreMIDI.framework/CoreMIDI",
			"/System/Library/Frameworks/CoreML.framework/CoreML",
			"/System/Library/Frameworks/CoreMotion.framework/CoreMotion",
			"/System/Library/Frameworks/CoreNFC.framework/CoreNFC",
			"/System/Library/Frameworks/CoreSpotlight.framework/CoreSpotlight",
			"/System/Library/Frameworks/CoreTelephony.framework/CoreTelephony",
			"/System/Library/Frameworks/CoreText.framework/CoreText",
			"/System/Library/Frameworks/CoreVideo.framework/CoreVideo",
			"/System/Library/Frameworks/CryptoTokenKit.framework/CryptoTokenKit",
			"/System/Library/Frameworks/DeviceCheck.framework/DeviceCheck",
			"/System/Library/Frameworks/DeviceDiscoveryExtension.framework/DeviceDiscoveryExtension",
			"/System/Library/Frameworks/EventKit.framework/EventKit",
			"/System/Library/Frameworks/EventKitUI.framework/EventKitUI",
			"/System/Library/Frameworks/ExternalAccessory.framework/ExternalAccessory",
			"/System/Library/Frameworks/FileProvider.framework/FileProvider",
			"/System/Library/Frameworks/FileProviderUI.framework/FileProviderUI",
			"/System/Library/Frameworks/Foundation.framework/Foundation",
			"/System/Library/Frameworks/GameController.framework/GameController",
			"/System/Library/Frameworks/GameKit.framework/GameKit",
			"/System/Library/Frameworks/GameplayKit.framework/GameplayKit",
			"/System/Library/Frameworks/GLKit.framework/GLKit",
			"/System/Library/Frameworks/GSS.framework/GSS",
			"/System/Library/Frameworks/HealthKit.framework/HealthKit",
			"/System/Library/Frameworks/HealthKitUI.framework/HealthKitUI",
			"/System/Library/Frameworks/HomeKit.framework/HomeKit",
			"/System/Library/Frameworks/IdentityLookup.framework/IdentityLookup",
			"/System/Library/Frameworks/IdentityLookupUI.framework/IdentityLookupUI",
			"/System/Library/Frameworks/ImageIO.framework/ImageIO",
			"/System/Library/Frameworks/Intents.framework/Intents",
			"/System/Library/Frameworks/IntentsUI.framework/IntentsUI",
			"/System/Library/Frameworks/IOSurface.framework/IOSurface",
			"/System/Library/Frameworks/JavaScriptCore.framework/JavaScriptCore",
			"/System/Library/Frameworks/LinkPresentation.framework/LinkPresentation",
			"/System/Library/Frameworks/LocalAuthentication.framework/LocalAuthentication",
			"/System/Library/Frameworks/MapKit.framework/MapKit",
			"/System/Library/Frameworks/MediaAccessibility.framework/MediaAccessibility",
			"/System/Library/Frameworks/MediaPlayer.framework/MediaPlayer",
			"/System/Library/Frameworks/MediaSetup.framework/MediaSetup",
			"/System/Library/Frameworks/MediaToolbox.framework/MediaToolbox",
			"/System/Library/Frameworks/Messages.framework/Messages",
			"/System/Library/Frameworks/MessageUI.framework/MessageUI",
			"/System/Library/Frameworks/Metal.framework/Metal",
			"/System/Library/Frameworks/MetalFX.framework/MetalFX",
			"/System/Library/Frameworks/MetalKit.framework/MetalKit",
			"/System/Library/Frameworks/MetalPerformanceShaders.framework/MetalPerformanceShaders",
			"/System/Library/Frameworks/MetalPerformanceShadersGraph.framework/MetalPerformanceShadersGraph",
			"/System/Library/Frameworks/MetricKit.framework/MetricKit",
			"/System/Library/Frameworks/MLCompute.framework/MLCompute",
			"/System/Library/Frameworks/MobileCoreServices.framework/MobileCoreServices",
			"/System/Library/Frameworks/ModelIO.framework/ModelIO",
			"/System/Library/Frameworks/MultipeerConnectivity.framework/MultipeerConnectivity",
			"/System/Library/Frameworks/NaturalLanguage.framework/NaturalLanguage",
			"/System/Library/Frameworks/NearbyInteraction.framework/NearbyInteraction",
			"/System/Library/Frameworks/Network.framework/Network",
			"/System/Library/Frameworks/NetworkExtension.framework/NetworkExtension",
			"/System/Library/Frameworks/NotificationCenter.framework/NotificationCenter",
			"/System/Library/Frameworks/OpenGLES.framework/OpenGLES",
			"/System/Library/Frameworks/OSLog.framework/OSLog",
			"/System/Library/Frameworks/PassKit.framework/PassKit",
			"/System/Library/Frameworks/PDFKit.framework/PDFKit",
			"/System/Library/Frameworks/PencilKit.framework/PencilKit",
			"/System/Library/Frameworks/PHASE.framework/PHASE",
			"/System/Library/Frameworks/Photos.framework/Photos",
			"/System/Library/Frameworks/PhotosUI.framework/PhotosUI",
			"/System/Library/Frameworks/PushKit.framework/PushKit",
			"/System/Library/Frameworks/PushToTalk.framework/PushToTalk",
			"/System/Library/Frameworks/QuartzCore.framework/QuartzCore",
			"/System/Library/Frameworks/QuickLook.framework/QuickLook",
			"/System/Library/Frameworks/QuickLookThumbnailing.framework/QuickLookThumbnailing",
			"/System/Library/Frameworks/ReplayKit.framework/ReplayKit",
			"/System/Library/Frameworks/SafariServices.framework/SafariServices",
			"/System/Library/Frameworks/SafetyKit.framework/SafetyKit",
			"/System/Library/Frameworks/SceneKit.framework/SceneKit",
			"/System/Library/Frameworks/ScreenTime.framework/ScreenTime",
			"/System/Library/Frameworks/Security.framework/Security",
			"/System/Library/Frameworks/SensitiveContentAnalysis.framework/SensitiveContentAnalysis",
			"/System/Library/Frameworks/SensorKit.framework/SensorKit",
			"/System/Library/Frameworks/SharedWithYou.framework/SharedWithYou",
			"/System/Library/Frameworks/SharedWithYouCore.framework/SharedWithYouCore",
			"/System/Library/Frameworks/ShazamKit.framework/ShazamKit",
			"/System/Library/Frameworks/Social.framework/Social",
			"/System/Library/Frameworks/SoundAnalysis.framework/SoundAnalysis",
			"/System/Library/Frameworks/Speech.framework/Speech",
			"/System/Library/Frameworks/SpriteKit.framework/SpriteKit",
			"/System/Library/Frameworks/StoreKit.framework/StoreKit",
			"/System/Library/Frameworks/Symbols.framework/Symbols",
			"/System/Library/Frameworks/SystemConfiguration.framework/SystemConfiguration",
			"/System/Library/Frameworks/ThreadNetwork.framework/ThreadNetwork",
			"/System/Library/Frameworks/Twitter.framework/Twitter",
			"/System/Library/Frameworks/UIKit.framework/UIKit",
			"/System/Library/Frameworks/UniformTypeIdentifiers.framework/UniformTypeIdentifiers",
			"/System/Library/Frameworks/UserNotifications.framework/UserNotifications",
			"/System/Library/Frameworks/UserNotificationsUI.framework/UserNotificationsUI",
			"/System/Library/Frameworks/VideoSubscriberAccount.framework/VideoSubscriberAccount",
			"/System/Library/Frameworks/VideoToolbox.framework/VideoToolbox",
			"/System/Library/Frameworks/Vision.framework/Vision",
			"/System/Library/Frameworks/VisionKit.framework/VisionKit",
			"/System/Library/Frameworks/WatchConnectivity.framework/WatchConnectivity",
			"/System/Library/Frameworks/WebKit.framework/WebKit",
			"/usr/lib/libc++.1.dylib",
			"/usr/lib/libcompression.dylib",
			"/usr/lib/libiconv.2.dylib",
			"/usr/lib/libobjc.A.dylib",
			"/usr/lib/libSystem.B.dylib",
			"/usr/lib/libz.1.dylib",
			"/System/Library/Frameworks/CryptoKit.framework/CryptoKit",
			"/usr/lib/libicucore.A.dylib",
			"/usr/lib/swift/libswiftCore.dylib",
			"/usr/lib/swift/libswiftCoreFoundation.dylib",
			"/usr/lib/swift/libswiftCoreImage.dylib",
			"/usr/lib/swift/libswiftDarwin.dylib",
			"/usr/lib/swift/libswiftDataDetection.dylib",
			"/usr/lib/swift/libswiftDispatch.dylib",
			"/usr/lib/swift/libswiftFileProvider.dylib",
			"/usr/lib/swift/libswiftFoundation.dylib",
			"/usr/lib/swift/libswiftMetal.dylib",
			"/usr/lib/swift/libswiftObjectiveC.dylib",
			"/usr/lib/swift/libswiftos.dylib",
			"/usr/lib/swift/libswiftQuartzCore.dylib",
			"/usr/lib/swift/libswiftUIKit.dylib",
			"/usr/lib/swift/libswiftUniformTypeIdentifiers.dylib",
			"/usr/lib/swift/libswiftXPC.dylib",
		];

		static string [] expectedFrameworks_iOS_Full = [
			"/System/Library/Frameworks/CFNetwork.framework/CFNetwork",
			"/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation",
			"/System/Library/Frameworks/Foundation.framework/Foundation",
			"/System/Library/Frameworks/GSS.framework/GSS",
			"/System/Library/Frameworks/Security.framework/Security",
			"/System/Library/Frameworks/UIKit.framework/UIKit",
			"/usr/lib/libc++.1.dylib",
			"/usr/lib/libcompression.dylib",
			"/usr/lib/libiconv.2.dylib",
			"/usr/lib/libicucore.A.dylib",
			"/usr/lib/libobjc.A.dylib",
			"/usr/lib/libSystem.B.dylib",
			"/usr/lib/libz.1.dylib",
		];

		static string [] expectedFrameworks_tvOS_None = [
			"/System/Library/Frameworks/Accelerate.framework/Accelerate",
			"/System/Library/Frameworks/Accessibility.framework/Accessibility",
			"/System/Library/Frameworks/AdSupport.framework/AdSupport",
			"/System/Library/Frameworks/AppTrackingTransparency.framework/AppTrackingTransparency",
			"/System/Library/Frameworks/AudioToolbox.framework/AudioToolbox",
			"/System/Library/Frameworks/AuthenticationServices.framework/AuthenticationServices",
			"/System/Library/Frameworks/AVFoundation.framework/AVFoundation",
			"/System/Library/Frameworks/AVKit.framework/AVKit",
			"/System/Library/Frameworks/BackgroundTasks.framework/BackgroundTasks",
			"/System/Library/Frameworks/CFNetwork.framework/CFNetwork",
			"/System/Library/Frameworks/Cinematic.framework/Cinematic",
			"/System/Library/Frameworks/CloudKit.framework/CloudKit",
			"/System/Library/Frameworks/CoreBluetooth.framework/CoreBluetooth",
			"/System/Library/Frameworks/CoreData.framework/CoreData",
			"/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation",
			"/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics",
			"/System/Library/Frameworks/CoreHaptics.framework/CoreHaptics",
			"/System/Library/Frameworks/CoreImage.framework/CoreImage",
			"/System/Library/Frameworks/CoreLocation.framework/CoreLocation",
			"/System/Library/Frameworks/CoreMedia.framework/CoreMedia",
			"/System/Library/Frameworks/CoreMIDI.framework/CoreMIDI",
			"/System/Library/Frameworks/CoreML.framework/CoreML",
			"/System/Library/Frameworks/CoreSpotlight.framework/CoreSpotlight",
			"/System/Library/Frameworks/CoreText.framework/CoreText",
			"/System/Library/Frameworks/CoreVideo.framework/CoreVideo",
			"/System/Library/Frameworks/CryptoKit.framework/CryptoKit",
			"/System/Library/Frameworks/CryptoTokenKit.framework/CryptoTokenKit",
			"/System/Library/Frameworks/DeviceCheck.framework/DeviceCheck",
			"/System/Library/Frameworks/DeviceDiscoveryUI.framework/DeviceDiscoveryUI",
			"/System/Library/Frameworks/ExternalAccessory.framework/ExternalAccessory",
			"/System/Library/Frameworks/Foundation.framework/Foundation",
			"/System/Library/Frameworks/GameController.framework/GameController",
			"/System/Library/Frameworks/GameKit.framework/GameKit",
			"/System/Library/Frameworks/GameplayKit.framework/GameplayKit",
			"/System/Library/Frameworks/GLKit.framework/GLKit",
			"/System/Library/Frameworks/HomeKit.framework/HomeKit",
			"/System/Library/Frameworks/ImageIO.framework/ImageIO",
			"/System/Library/Frameworks/Intents.framework/Intents",
			"/System/Library/Frameworks/IOSurface.framework/IOSurface",
			"/System/Library/Frameworks/JavaScriptCore.framework/JavaScriptCore",
			"/System/Library/Frameworks/LinkPresentation.framework/LinkPresentation",
			"/System/Library/Frameworks/MapKit.framework/MapKit",
			"/System/Library/Frameworks/MediaAccessibility.framework/MediaAccessibility",
			"/System/Library/Frameworks/MediaPlayer.framework/MediaPlayer",
			"/System/Library/Frameworks/MediaToolbox.framework/MediaToolbox",
			"/System/Library/Frameworks/Metal.framework/Metal",
			"/System/Library/Frameworks/MetalKit.framework/MetalKit",
			"/System/Library/Frameworks/MetalPerformanceShaders.framework/MetalPerformanceShaders",
			"/System/Library/Frameworks/MetalPerformanceShadersGraph.framework/MetalPerformanceShadersGraph",
			"/System/Library/Frameworks/MLCompute.framework/MLCompute",
			"/System/Library/Frameworks/MobileCoreServices.framework/MobileCoreServices",
			"/System/Library/Frameworks/ModelIO.framework/ModelIO",
			"/System/Library/Frameworks/MultipeerConnectivity.framework/MultipeerConnectivity",
			"/System/Library/Frameworks/NaturalLanguage.framework/NaturalLanguage",
			"/System/Library/Frameworks/Network.framework/Network",
			"/System/Library/Frameworks/NetworkExtension.framework/NetworkExtension",
			"/System/Library/Frameworks/OpenGLES.framework/OpenGLES",
			"/System/Library/Frameworks/OSLog.framework/OSLog",
			"/System/Library/Frameworks/PDFKit.framework/PDFKit",
			"/System/Library/Frameworks/PHASE.framework/PHASE",
			"/System/Library/Frameworks/Photos.framework/Photos",
			"/System/Library/Frameworks/PhotosUI.framework/PhotosUI",
			"/System/Library/Frameworks/QuartzCore.framework/QuartzCore",
			"/System/Library/Frameworks/ReplayKit.framework/ReplayKit",
			"/System/Library/Frameworks/SceneKit.framework/SceneKit",
			"/System/Library/Frameworks/Security.framework/Security",
			"/System/Library/Frameworks/SharedWithYou.framework/SharedWithYou",
			"/System/Library/Frameworks/ShazamKit.framework/ShazamKit",
			"/System/Library/Frameworks/SoundAnalysis.framework/SoundAnalysis",
			"/System/Library/Frameworks/SpriteKit.framework/SpriteKit",
			"/System/Library/Frameworks/StoreKit.framework/StoreKit",
			"/System/Library/Frameworks/Symbols.framework/Symbols",
			"/System/Library/Frameworks/SystemConfiguration.framework/SystemConfiguration",
			"/System/Library/Frameworks/TVMLKit.framework/TVMLKit",
			"/System/Library/Frameworks/TVServices.framework/TVServices",
			"/System/Library/Frameworks/TVUIKit.framework/TVUIKit",
			"/System/Library/Frameworks/UIKit.framework/UIKit",
			"/System/Library/Frameworks/UniformTypeIdentifiers.framework/UniformTypeIdentifiers",
			"/System/Library/Frameworks/UserNotifications.framework/UserNotifications",
			"/System/Library/Frameworks/VideoSubscriberAccount.framework/VideoSubscriberAccount",
			"/System/Library/Frameworks/VideoToolbox.framework/VideoToolbox",
			"/System/Library/Frameworks/Vision.framework/Vision",
			"/usr/lib/libc++.1.dylib",
			"/usr/lib/libcompression.dylib",
			"/usr/lib/libiconv.2.dylib",
			"/usr/lib/libicucore.A.dylib",
			"/usr/lib/libobjc.A.dylib",
			"/usr/lib/libSystem.B.dylib",
			"/usr/lib/libz.1.dylib",
			"/usr/lib/swift/libswiftCore.dylib",
			"/usr/lib/swift/libswiftCoreFoundation.dylib",
			"/usr/lib/swift/libswiftCoreImage.dylib",
			"/usr/lib/swift/libswiftDarwin.dylib",
			"/usr/lib/swift/libswiftDispatch.dylib",
			"/usr/lib/swift/libswiftFoundation.dylib",
			"/usr/lib/swift/libswiftMetal.dylib",
			"/usr/lib/swift/libswiftObjectiveC.dylib",
			"/usr/lib/swift/libswiftos.dylib",
			"/usr/lib/swift/libswiftQuartzCore.dylib",
			"/usr/lib/swift/libswiftUIKit.dylib",
			"/usr/lib/swift/libswiftUniformTypeIdentifiers.dylib",
		];

		static string [] expectedFrameworks_tvOS_Full = [
			"/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation",
			"/System/Library/Frameworks/Foundation.framework/Foundation",
			"/System/Library/Frameworks/Security.framework/Security",
			"/System/Library/Frameworks/UIKit.framework/UIKit",
			"/usr/lib/libc++.1.dylib",
			"/usr/lib/libcompression.dylib",
			"/usr/lib/libiconv.2.dylib",
			"/usr/lib/libicucore.A.dylib",
			"/usr/lib/libobjc.A.dylib",
			"/usr/lib/libSystem.B.dylib",
			"/usr/lib/libz.1.dylib",
		];

		static string [] expectedFrameworks_macOS_None = [
			"@executable_path/../../Contents/MonoBundle/libclrgc.dylib",
			"@executable_path/../../Contents/MonoBundle/libclrgcexp.dylib",
			"@executable_path/../../Contents/MonoBundle/libclrjit.dylib",
			"@executable_path/../../Contents/MonoBundle/libcoreclr.dylib",
			"@executable_path/../../Contents/MonoBundle/libhostfxr.dylib",
			"@executable_path/../../Contents/MonoBundle/libhostpolicy.dylib",
			"@executable_path/../../Contents/MonoBundle/libmscordaccore.dylib",
			"@executable_path/../../Contents/MonoBundle/libmscordbi.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Globalization.Native.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.IO.Compression.Native.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Native.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Net.Security.Native.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Security.Cryptography.Native.Apple.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Security.Cryptography.Native.OpenSsl.dylib",
			"/System/Library/Frameworks/Accelerate.framework/Versions/A/Accelerate",
			"/System/Library/Frameworks/Accessibility.framework/Versions/A/Accessibility",
			"/System/Library/Frameworks/Accounts.framework/Versions/A/Accounts",
			"/System/Library/Frameworks/AdServices.framework/Versions/A/AdServices",
			"/System/Library/Frameworks/AdSupport.framework/Versions/A/AdSupport",
			"/System/Library/Frameworks/AppKit.framework/Versions/C/AppKit",
			"/System/Library/Frameworks/ApplicationServices.framework/Versions/A/ApplicationServices",
			"/System/Library/Frameworks/AppTrackingTransparency.framework/Versions/A/AppTrackingTransparency",
			"/System/Library/Frameworks/AudioToolbox.framework/Versions/A/AudioToolbox",
			"/System/Library/Frameworks/AudioUnit.framework/Versions/A/AudioUnit",
			"/System/Library/Frameworks/AuthenticationServices.framework/Versions/A/AuthenticationServices",
			"/System/Library/Frameworks/AutomaticAssessmentConfiguration.framework/Versions/A/AutomaticAssessmentConfiguration",
			"/System/Library/Frameworks/AVFoundation.framework/Versions/A/AVFoundation",
			"/System/Library/Frameworks/AVKit.framework/Versions/A/AVKit",
			"/System/Library/Frameworks/AVRouting.framework/Versions/A/AVRouting",
			"/System/Library/Frameworks/BackgroundAssets.framework/Versions/A/BackgroundAssets",
			"/System/Library/Frameworks/BusinessChat.framework/Versions/A/BusinessChat",
			"/System/Library/Frameworks/CallKit.framework/Versions/A/CallKit",
			"/System/Library/Frameworks/CFNetwork.framework/Versions/A/CFNetwork",
			"/System/Library/Frameworks/Cinematic.framework/Versions/A/Cinematic",
			"/System/Library/Frameworks/ClassKit.framework/Versions/A/ClassKit",
			"/System/Library/Frameworks/CloudKit.framework/Versions/A/CloudKit",
			"/System/Library/Frameworks/Contacts.framework/Versions/A/Contacts",
			"/System/Library/Frameworks/ContactsUI.framework/Versions/A/ContactsUI",
			"/System/Library/Frameworks/CoreAudio.framework/Versions/A/CoreAudio",
			"/System/Library/Frameworks/CoreAudioKit.framework/Versions/A/CoreAudioKit",
			"/System/Library/Frameworks/CoreBluetooth.framework/Versions/A/CoreBluetooth",
			"/System/Library/Frameworks/CoreData.framework/Versions/A/CoreData",
			"/System/Library/Frameworks/CoreFoundation.framework/Versions/A/CoreFoundation",
			"/System/Library/Frameworks/CoreImage.framework/Versions/A/CoreImage",
			"/System/Library/Frameworks/CoreLocation.framework/Versions/A/CoreLocation",
			"/System/Library/Frameworks/CoreMedia.framework/Versions/A/CoreMedia",
			"/System/Library/Frameworks/CoreMIDI.framework/Versions/A/CoreMIDI",
			"/System/Library/Frameworks/CoreML.framework/Versions/A/CoreML",
			"/System/Library/Frameworks/CoreMotion.framework/Versions/A/CoreMotion",
			"/System/Library/Frameworks/CoreServices.framework/Versions/A/CoreServices",
			"/System/Library/Frameworks/CoreSpotlight.framework/Versions/A/CoreSpotlight",
			"/System/Library/Frameworks/CoreText.framework/Versions/A/CoreText",
			"/System/Library/Frameworks/CoreVideo.framework/Versions/A/CoreVideo",
			"/System/Library/Frameworks/CoreWLAN.framework/Versions/A/CoreWLAN",
			"/System/Library/Frameworks/CryptoTokenKit.framework/Versions/A/CryptoTokenKit",
			"/System/Library/Frameworks/DeviceCheck.framework/Versions/A/DeviceCheck",
			"/System/Library/Frameworks/DeviceDiscoveryExtension.framework/Versions/A/DeviceDiscoveryExtension",
			"/System/Library/Frameworks/EventKit.framework/Versions/A/EventKit",
			"/System/Library/Frameworks/ExecutionPolicy.framework/Versions/A/ExecutionPolicy",
			"/System/Library/Frameworks/ExtensionKit.framework/Versions/A/ExtensionKit",
			"/System/Library/Frameworks/ExternalAccessory.framework/ExternalAccessory",
			"/System/Library/Frameworks/FileProvider.framework/Versions/A/FileProvider",
			"/System/Library/Frameworks/FileProviderUI.framework/Versions/A/FileProviderUI",
			"/System/Library/Frameworks/FinderSync.framework/Versions/A/FinderSync",
			"/System/Library/Frameworks/Foundation.framework/Versions/C/Foundation",
			"/System/Library/Frameworks/GameController.framework/Versions/A/GameController",
			"/System/Library/Frameworks/GameKit.framework/Versions/A/GameKit",
			"/System/Library/Frameworks/GameplayKit.framework/Versions/A/GameplayKit",
			"/System/Library/Frameworks/GLKit.framework/Versions/A/GLKit",
			"/System/Library/Frameworks/HealthKit.framework/Versions/A/HealthKit",
			"/System/Library/Frameworks/ImageCaptureCore.framework/Versions/A/ImageCaptureCore",
			"/System/Library/Frameworks/ImageIO.framework/Versions/A/ImageIO",
			"/System/Library/Frameworks/Intents.framework/Versions/A/Intents",
			"/System/Library/Frameworks/IntentsUI.framework/Versions/A/IntentsUI",
			"/System/Library/Frameworks/IOSurface.framework/Versions/A/IOSurface",
			"/System/Library/Frameworks/iTunesLibrary.framework/Versions/A/iTunesLibrary",
			"/System/Library/Frameworks/JavaScriptCore.framework/Versions/A/JavaScriptCore",
			"/System/Library/Frameworks/LinkPresentation.framework/Versions/A/LinkPresentation",
			"/System/Library/Frameworks/LocalAuthentication.framework/Versions/A/LocalAuthentication",
			"/System/Library/Frameworks/LocalAuthenticationEmbeddedUI.framework/Versions/A/LocalAuthenticationEmbeddedUI",
			"/System/Library/Frameworks/MailKit.framework/Versions/A/MailKit",
			"/System/Library/Frameworks/MapKit.framework/Versions/A/MapKit",
			"/System/Library/Frameworks/MediaAccessibility.framework/Versions/A/MediaAccessibility",
			"/System/Library/Frameworks/MediaExtension.framework/Versions/A/MediaExtension",
			"/System/Library/Frameworks/MediaLibrary.framework/Versions/A/MediaLibrary",
			"/System/Library/Frameworks/MediaPlayer.framework/Versions/A/MediaPlayer",
			"/System/Library/Frameworks/MediaToolbox.framework/Versions/A/MediaToolbox",
			"/System/Library/Frameworks/Metal.framework/Versions/A/Metal",
			"/System/Library/Frameworks/MetalFX.framework/Versions/A/MetalFX",
			"/System/Library/Frameworks/MetalKit.framework/Versions/A/MetalKit",
			"/System/Library/Frameworks/MetalPerformanceShaders.framework/Versions/A/MetalPerformanceShaders",
			"/System/Library/Frameworks/MetalPerformanceShadersGraph.framework/Versions/A/MetalPerformanceShadersGraph",
			"/System/Library/Frameworks/MetricKit.framework/Versions/A/MetricKit",
			"/System/Library/Frameworks/MLCompute.framework/Versions/A/MLCompute",
			"/System/Library/Frameworks/ModelIO.framework/Versions/A/ModelIO",
			"/System/Library/Frameworks/MultipeerConnectivity.framework/Versions/A/MultipeerConnectivity",
			"/System/Library/Frameworks/NaturalLanguage.framework/Versions/A/NaturalLanguage",
			"/System/Library/Frameworks/Network.framework/Versions/A/Network",
			"/System/Library/Frameworks/NetworkExtension.framework/Versions/A/NetworkExtension",
			"/System/Library/Frameworks/NotificationCenter.framework/Versions/A/NotificationCenter",
			"/System/Library/Frameworks/OpenGL.framework/Versions/A/OpenGL",
			"/System/Library/Frameworks/OSLog.framework/Versions/A/OSLog",
			"/System/Library/Frameworks/PassKit.framework/Versions/A/PassKit",
			"/System/Library/Frameworks/PencilKit.framework/Versions/A/PencilKit",
			"/System/Library/Frameworks/PHASE.framework/Versions/A/PHASE",
			"/System/Library/Frameworks/Photos.framework/Versions/A/Photos",
			"/System/Library/Frameworks/PhotosUI.framework/Versions/A/PhotosUI",
			"/System/Library/Frameworks/PushKit.framework/Versions/A/PushKit",
			"/System/Library/Frameworks/Quartz.framework/Versions/A/Quartz",
			"/System/Library/Frameworks/QuartzCore.framework/Versions/A/QuartzCore",
			"/System/Library/Frameworks/QuickLook.framework/Versions/A/QuickLook",
			"/System/Library/Frameworks/QuickLookThumbnailing.framework/Versions/A/QuickLookThumbnailing",
			"/System/Library/Frameworks/ReplayKit.framework/Versions/A/ReplayKit",
			"/System/Library/Frameworks/SafariServices.framework/Versions/A/SafariServices",
			"/System/Library/Frameworks/SafetyKit.framework/Versions/A/SafetyKit",
			"/System/Library/Frameworks/SceneKit.framework/Versions/A/SceneKit",
			"/System/Library/Frameworks/ScreenCaptureKit.framework/Versions/A/ScreenCaptureKit",
			"/System/Library/Frameworks/ScreenTime.framework/Versions/A/ScreenTime",
			"/System/Library/Frameworks/ScriptingBridge.framework/Versions/A/ScriptingBridge",
			"/System/Library/Frameworks/Security.framework/Versions/A/Security",
			"/System/Library/Frameworks/SensitiveContentAnalysis.framework/Versions/A/SensitiveContentAnalysis",
			"/System/Library/Frameworks/ServiceManagement.framework/Versions/A/ServiceManagement",
			"/System/Library/Frameworks/SharedWithYou.framework/Versions/A/SharedWithYou",
			"/System/Library/Frameworks/SharedWithYouCore.framework/Versions/A/SharedWithYouCore",
			"/System/Library/Frameworks/ShazamKit.framework/Versions/A/ShazamKit",
			"/System/Library/Frameworks/Social.framework/Versions/A/Social",
			"/System/Library/Frameworks/SoundAnalysis.framework/Versions/A/SoundAnalysis",
			"/System/Library/Frameworks/Speech.framework/Versions/A/Speech",
			"/System/Library/Frameworks/SpriteKit.framework/Versions/A/SpriteKit",
			"/System/Library/Frameworks/StoreKit.framework/Versions/A/StoreKit",
			"/System/Library/Frameworks/Symbols.framework/Versions/A/Symbols",
			"/System/Library/Frameworks/SystemConfiguration.framework/Versions/A/SystemConfiguration",
			"/System/Library/Frameworks/ThreadNetwork.framework/Versions/A/ThreadNetwork",
			"/System/Library/Frameworks/UniformTypeIdentifiers.framework/Versions/A/UniformTypeIdentifiers",
			"/System/Library/Frameworks/UserNotifications.framework/Versions/A/UserNotifications",
			"/System/Library/Frameworks/UserNotificationsUI.framework/Versions/A/UserNotificationsUI",
			"/System/Library/Frameworks/VideoSubscriberAccount.framework/Versions/A/VideoSubscriberAccount",
			"/System/Library/Frameworks/VideoToolbox.framework/Versions/A/VideoToolbox",
			"/System/Library/Frameworks/Vision.framework/Versions/A/Vision",
			"/System/Library/Frameworks/WebKit.framework/Versions/A/WebKit",
			"/usr/lib/libc++.1.dylib",
			"/usr/lib/libcompression.dylib",
			"/usr/lib/libiconv.2.dylib",
			"/usr/lib/libicucore.A.dylib",
			"/usr/lib/libobjc.A.dylib",
			"/usr/lib/libSystem.B.dylib",
			"/usr/lib/libz.1.dylib",
			"/usr/lib/swift/libswiftCore.dylib",
			"/usr/lib/swift/libswiftCoreFoundation.dylib",
			"/usr/lib/swift/libswiftCoreImage.dylib",
			"/usr/lib/swift/libswiftDarwin.dylib",
			"/usr/lib/swift/libswiftDispatch.dylib",
			"/usr/lib/swift/libswiftFoundation.dylib",
			"/usr/lib/swift/libswiftIOKit.dylib",
			"/usr/lib/swift/libswiftMetal.dylib",
			"/usr/lib/swift/libswiftObjectiveC.dylib",
			"/usr/lib/swift/libswiftos.dylib",
			"/usr/lib/swift/libswiftOSLog.dylib",
			"/usr/lib/swift/libswiftQuartzCore.dylib",
			"/usr/lib/swift/libswiftUniformTypeIdentifiers.dylib",
			"/usr/lib/swift/libswiftXPC.dylib",
		];

		static string [] expectedFrameworks_macOS_Full = [
			"@executable_path/../../Contents/MonoBundle/libclrgc.dylib",
			"@executable_path/../../Contents/MonoBundle/libclrgcexp.dylib",
			"@executable_path/../../Contents/MonoBundle/libclrjit.dylib",
			"@executable_path/../../Contents/MonoBundle/libcoreclr.dylib",
			"@executable_path/../../Contents/MonoBundle/libhostfxr.dylib",
			"@executable_path/../../Contents/MonoBundle/libhostpolicy.dylib",
			"@executable_path/../../Contents/MonoBundle/libmscordaccore.dylib",
			"@executable_path/../../Contents/MonoBundle/libmscordbi.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Globalization.Native.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.IO.Compression.Native.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Native.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Net.Security.Native.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Security.Cryptography.Native.Apple.dylib",
			"@executable_path/../../Contents/MonoBundle/libSystem.Security.Cryptography.Native.OpenSsl.dylib",
			"/System/Library/Frameworks/AppKit.framework/Versions/C/AppKit",
			"/System/Library/Frameworks/ApplicationServices.framework/Versions/A/ApplicationServices",
			"/System/Library/Frameworks/CloudKit.framework/Versions/A/CloudKit",
			"/System/Library/Frameworks/CoreData.framework/Versions/A/CoreData",
			"/System/Library/Frameworks/CoreFoundation.framework/Versions/A/CoreFoundation",
			"/System/Library/Frameworks/Foundation.framework/Versions/C/Foundation",
			"/System/Library/Frameworks/Quartz.framework/Versions/A/Quartz",
			"/System/Library/Frameworks/QuartzCore.framework/Versions/A/QuartzCore",
			"/System/Library/Frameworks/Security.framework/Versions/A/Security",
			"/usr/lib/libc++.1.dylib",
			"/usr/lib/libcompression.dylib",
			"/usr/lib/libiconv.2.dylib",
			"/usr/lib/libicucore.A.dylib",
			"/usr/lib/libobjc.A.dylib",
			"/usr/lib/libSystem.B.dylib",
			"/usr/lib/libz.1.dylib",
		];

		static string [] expectedFrameworks_MacCatalyst_None = [
			"/System/iOSSupport/System/Library/Frameworks/AddressBook.framework/Versions/A/AddressBook",
			"/System/iOSSupport/System/Library/Frameworks/AppClip.framework/Versions/A/AppClip",
			"/System/iOSSupport/System/Library/Frameworks/AuthenticationServices.framework/Versions/A/AuthenticationServices",
			"/System/iOSSupport/System/Library/Frameworks/AVKit.framework/Versions/A/AVKit",
			"/System/iOSSupport/System/Library/Frameworks/BusinessChat.framework/Versions/A/BusinessChat",
			"/System/iOSSupport/System/Library/Frameworks/ContactsUI.framework/Versions/A/ContactsUI",
			"/System/iOSSupport/System/Library/Frameworks/CoreAudioKit.framework/Versions/A/CoreAudioKit",
			"/System/iOSSupport/System/Library/Frameworks/CoreLocationUI.framework/Versions/A/CoreLocationUI",
			"/System/iOSSupport/System/Library/Frameworks/CoreNFC.framework/Versions/A/CoreNFC",
			"/System/iOSSupport/System/Library/Frameworks/EventKitUI.framework/Versions/A/EventKitUI",
			"/System/iOSSupport/System/Library/Frameworks/GameController.framework/Versions/A/GameController",
			"/System/iOSSupport/System/Library/Frameworks/GameKit.framework/Versions/A/GameKit",
			"/System/iOSSupport/System/Library/Frameworks/GameplayKit.framework/Versions/A/GameplayKit",
			"/System/iOSSupport/System/Library/Frameworks/HealthKitUI.framework/Versions/A/HealthKitUI",
			"/System/iOSSupport/System/Library/Frameworks/HomeKit.framework/Versions/A/HomeKit",
			"/System/iOSSupport/System/Library/Frameworks/IdentityLookupUI.framework/Versions/A/IdentityLookupUI",
			"/System/iOSSupport/System/Library/Frameworks/IntentsUI.framework/Versions/A/IntentsUI",
			"/System/iOSSupport/System/Library/Frameworks/JavaScriptCore.framework/Versions/A/JavaScriptCore",
			"/System/iOSSupport/System/Library/Frameworks/LinkPresentation.framework/Versions/A/LinkPresentation",
			"/System/iOSSupport/System/Library/Frameworks/MapKit.framework/Versions/A/MapKit",
			"/System/iOSSupport/System/Library/Frameworks/MediaPlayer.framework/Versions/A/MediaPlayer",
			"/System/iOSSupport/System/Library/Frameworks/Messages.framework/Versions/A/Messages",
			"/System/iOSSupport/System/Library/Frameworks/MessageUI.framework/Versions/A/MessageUI",
			"/System/iOSSupport/System/Library/Frameworks/MetalKit.framework/Versions/A/MetalKit",
			"/System/iOSSupport/System/Library/Frameworks/MobileCoreServices.framework/Versions/A/MobileCoreServices",
			"/System/iOSSupport/System/Library/Frameworks/MultipeerConnectivity.framework/Versions/A/MultipeerConnectivity",
			"/System/iOSSupport/System/Library/Frameworks/PassKit.framework/Versions/A/PassKit",
			"/System/iOSSupport/System/Library/Frameworks/PDFKit.framework/Versions/A/PDFKit",
			"/System/iOSSupport/System/Library/Frameworks/PencilKit.framework/Versions/A/PencilKit",
			"/System/iOSSupport/System/Library/Frameworks/PhotosUI.framework/Versions/A/PhotosUI",
			"/System/iOSSupport/System/Library/Frameworks/QuickLook.framework/Versions/A/QuickLook",
			"/System/iOSSupport/System/Library/Frameworks/ReplayKit.framework/Versions/A/ReplayKit",
			"/System/iOSSupport/System/Library/Frameworks/SafariServices.framework/Versions/A/SafariServices",
			"/System/iOSSupport/System/Library/Frameworks/SceneKit.framework/Versions/A/SceneKit",
			"/System/iOSSupport/System/Library/Frameworks/ScreenCaptureKit.framework/Versions/A/ScreenCaptureKit",
			"/System/iOSSupport/System/Library/Frameworks/ScreenTime.framework/Versions/A/ScreenTime",
			"/System/iOSSupport/System/Library/Frameworks/SharedWithYou.framework/Versions/A/SharedWithYou",
			"/System/iOSSupport/System/Library/Frameworks/Social.framework/Versions/A/Social",
			"/System/iOSSupport/System/Library/Frameworks/SpriteKit.framework/Versions/A/SpriteKit",
			"/System/iOSSupport/System/Library/Frameworks/StoreKit.framework/Versions/A/StoreKit",
			"/System/iOSSupport/System/Library/Frameworks/UIKit.framework/Versions/A/UIKit",
			"/System/iOSSupport/System/Library/Frameworks/UserNotificationsUI.framework/Versions/A/UserNotificationsUI",
			"/System/iOSSupport/System/Library/Frameworks/VisionKit.framework/Versions/A/VisionKit",
			"/System/iOSSupport/System/Library/Frameworks/WebKit.framework/Versions/A/WebKit",
			"/System/Library/Frameworks/Accelerate.framework/Versions/A/Accelerate",
			"/System/Library/Frameworks/Accessibility.framework/Versions/A/Accessibility",
			"/System/Library/Frameworks/Accounts.framework/Versions/A/Accounts",
			"/System/Library/Frameworks/AdServices.framework/Versions/A/AdServices",
			"/System/Library/Frameworks/AdSupport.framework/Versions/A/AdSupport",
			"/System/Library/Frameworks/AppKit.framework/Versions/C/AppKit",
			"/System/Library/Frameworks/AppTrackingTransparency.framework/Versions/A/AppTrackingTransparency",
			"/System/Library/Frameworks/AudioToolbox.framework/Versions/A/AudioToolbox",
			"/System/Library/Frameworks/AutomaticAssessmentConfiguration.framework/Versions/A/AutomaticAssessmentConfiguration",
			"/System/Library/Frameworks/AVFoundation.framework/Versions/A/AVFoundation",
			"/System/Library/Frameworks/AVRouting.framework/Versions/A/AVRouting",
			"/System/Library/Frameworks/BackgroundAssets.framework/Versions/A/BackgroundAssets",
			"/System/Library/Frameworks/BackgroundTasks.framework/Versions/A/BackgroundTasks",
			"/System/Library/Frameworks/CallKit.framework/Versions/A/CallKit",
			"/System/Library/Frameworks/CFNetwork.framework/Versions/A/CFNetwork",
			"/System/Library/Frameworks/ClassKit.framework/Versions/A/ClassKit",
			"/System/Library/Frameworks/CloudKit.framework/Versions/A/CloudKit",
			"/System/Library/Frameworks/Contacts.framework/Versions/A/Contacts",
			"/System/Library/Frameworks/CoreAudio.framework/Versions/A/CoreAudio",
			"/System/Library/Frameworks/CoreBluetooth.framework/Versions/A/CoreBluetooth",
			"/System/Library/Frameworks/CoreData.framework/Versions/A/CoreData",
			"/System/Library/Frameworks/CoreFoundation.framework/Versions/A/CoreFoundation",
			"/System/Library/Frameworks/CoreGraphics.framework/Versions/A/CoreGraphics",
			"/System/Library/Frameworks/CoreHaptics.framework/Versions/A/CoreHaptics",
			"/System/Library/Frameworks/CoreImage.framework/Versions/A/CoreImage",
			"/System/Library/Frameworks/CoreLocation.framework/Versions/A/CoreLocation",
			"/System/Library/Frameworks/CoreMedia.framework/Versions/A/CoreMedia",
			"/System/Library/Frameworks/CoreMIDI.framework/Versions/A/CoreMIDI",
			"/System/Library/Frameworks/CoreML.framework/Versions/A/CoreML",
			"/System/Library/Frameworks/CoreMotion.framework/Versions/A/CoreMotion",
			"/System/Library/Frameworks/CoreServices.framework/Versions/A/CoreServices",
			"/System/Library/Frameworks/CoreSpotlight.framework/Versions/A/CoreSpotlight",
			"/System/Library/Frameworks/CoreTelephony.framework/Versions/A/CoreTelephony",
			"/System/Library/Frameworks/CoreText.framework/Versions/A/CoreText",
			"/System/Library/Frameworks/CoreVideo.framework/Versions/A/CoreVideo",
			"/System/Library/Frameworks/CoreWLAN.framework/Versions/A/CoreWLAN",
			"/System/Library/Frameworks/CryptoKit.framework/Versions/A/CryptoKit",
			"/System/Library/Frameworks/CryptoTokenKit.framework/Versions/A/CryptoTokenKit",
			"/System/Library/Frameworks/DeviceCheck.framework/Versions/A/DeviceCheck",
			"/System/Library/Frameworks/DeviceDiscoveryExtension.framework/Versions/A/DeviceDiscoveryExtension",
			"/System/Library/Frameworks/EventKit.framework/Versions/A/EventKit",
			"/System/Library/Frameworks/ExecutionPolicy.framework/Versions/A/ExecutionPolicy",
			"/System/Library/Frameworks/ExternalAccessory.framework/ExternalAccessory",
			"/System/Library/Frameworks/FileProvider.framework/Versions/A/FileProvider",
			"/System/Library/Frameworks/Foundation.framework/Versions/C/Foundation",
			"/System/Library/Frameworks/GSS.framework/Versions/A/GSS",
			"/System/Library/Frameworks/HealthKit.framework/Versions/A/HealthKit",
			"/System/Library/Frameworks/IdentityLookup.framework/Versions/A/IdentityLookup",
			"/System/Library/Frameworks/ImageIO.framework/Versions/A/ImageIO",
			"/System/Library/Frameworks/Intents.framework/Versions/A/Intents",
			"/System/Library/Frameworks/IOSurface.framework/Versions/A/IOSurface",
			"/System/Library/Frameworks/LocalAuthentication.framework/Versions/A/LocalAuthentication",
			"/System/Library/Frameworks/MediaAccessibility.framework/Versions/A/MediaAccessibility",
			"/System/Library/Frameworks/MediaToolbox.framework/Versions/A/MediaToolbox",
			"/System/Library/Frameworks/Metal.framework/Versions/A/Metal",
			"/System/Library/Frameworks/MetalPerformanceShaders.framework/Versions/A/MetalPerformanceShaders",
			"/System/Library/Frameworks/MetalPerformanceShadersGraph.framework/Versions/A/MetalPerformanceShadersGraph",
			"/System/Library/Frameworks/MetricKit.framework/Versions/A/MetricKit",
			"/System/Library/Frameworks/MLCompute.framework/Versions/A/MLCompute",
			"/System/Library/Frameworks/ModelIO.framework/Versions/A/ModelIO",
			"/System/Library/Frameworks/NaturalLanguage.framework/Versions/A/NaturalLanguage",
			"/System/Library/Frameworks/NearbyInteraction.framework/Versions/A/NearbyInteraction",
			"/System/Library/Frameworks/Network.framework/Versions/A/Network",
			"/System/Library/Frameworks/NetworkExtension.framework/Versions/A/NetworkExtension",
			"/System/Library/Frameworks/OSLog.framework/Versions/A/OSLog",
			"/System/Library/Frameworks/PHASE.framework/Versions/A/PHASE",
			"/System/Library/Frameworks/Photos.framework/Versions/A/Photos",
			"/System/Library/Frameworks/PushKit.framework/Versions/A/PushKit",
			"/System/Library/Frameworks/QuartzCore.framework/Versions/A/QuartzCore",
			"/System/Library/Frameworks/QuickLookThumbnailing.framework/Versions/A/QuickLookThumbnailing",
			"/System/Library/Frameworks/Security.framework/Versions/A/Security",
			"/System/Library/Frameworks/SensitiveContentAnalysis.framework/Versions/A/SensitiveContentAnalysis",
			"/System/Library/Frameworks/SensorKit.framework/Versions/A/SensorKit",
			"/System/Library/Frameworks/ServiceManagement.framework/Versions/A/ServiceManagement",
			"/System/Library/Frameworks/SharedWithYouCore.framework/Versions/A/SharedWithYouCore",
			"/System/Library/Frameworks/ShazamKit.framework/Versions/A/ShazamKit",
			"/System/Library/Frameworks/SoundAnalysis.framework/Versions/A/SoundAnalysis",
			"/System/Library/Frameworks/Speech.framework/Versions/A/Speech",
			"/System/Library/Frameworks/Symbols.framework/Versions/A/Symbols",
			"/System/Library/Frameworks/SystemConfiguration.framework/Versions/A/SystemConfiguration",
			"/System/Library/Frameworks/ThreadNetwork.framework/Versions/A/ThreadNetwork",
			"/System/Library/Frameworks/UniformTypeIdentifiers.framework/Versions/A/UniformTypeIdentifiers",
			"/System/Library/Frameworks/UserNotifications.framework/Versions/A/UserNotifications",
			"/System/Library/Frameworks/VideoToolbox.framework/Versions/A/VideoToolbox",
			"/System/Library/Frameworks/Vision.framework/Versions/A/Vision",
			"/usr/lib/libc++.1.dylib",
			"/usr/lib/libcompression.dylib",
			"/usr/lib/libiconv.2.dylib",
			"/usr/lib/libicucore.A.dylib",
			"/usr/lib/libobjc.A.dylib",
			"/usr/lib/libSystem.B.dylib",
			"/usr/lib/libz.1.dylib",
			"/System/iOSSupport/usr/lib/swift/libswiftUIKit.dylib",
			"/usr/lib/swift/libswiftCore.dylib",
			"/usr/lib/swift/libswiftCoreFoundation.dylib",
			"/usr/lib/swift/libswiftCoreImage.dylib",
			"/usr/lib/swift/libswiftDarwin.dylib",
			"/usr/lib/swift/libswiftDataDetection.dylib",
			"/usr/lib/swift/libswiftDispatch.dylib",
			"/usr/lib/swift/libswiftFileProvider.dylib",
			"/usr/lib/swift/libswiftFoundation.dylib",
			"/usr/lib/swift/libswiftIOKit.dylib",
			"/usr/lib/swift/libswiftMetal.dylib",
			"/usr/lib/swift/libswiftObjectiveC.dylib",
			"/usr/lib/swift/libswiftos.dylib",
			"/usr/lib/swift/libswiftOSLog.dylib",
			"/usr/lib/swift/libswiftQuartzCore.dylib",
			"/usr/lib/swift/libswiftUniformTypeIdentifiers.dylib",
			"/usr/lib/swift/libswiftXPC.dylib",
		];

		static string [] expectedFrameworks_MacCatalyst_Full = [
			"/System/iOSSupport/System/Library/Frameworks/UIKit.framework/Versions/A/UIKit",
			"/System/Library/Frameworks/AppKit.framework/Versions/C/AppKit",
			"/System/Library/Frameworks/CloudKit.framework/Versions/A/CloudKit",
			"/System/Library/Frameworks/CoreData.framework/Versions/A/CoreData",
			"/System/Library/Frameworks/CoreFoundation.framework/Versions/A/CoreFoundation",
			"/System/Library/Frameworks/CoreGraphics.framework/Versions/A/CoreGraphics",
			"/System/Library/Frameworks/Foundation.framework/Versions/C/Foundation",
			"/System/Library/Frameworks/GSS.framework/Versions/A/GSS",
			"/System/Library/Frameworks/QuartzCore.framework/Versions/A/QuartzCore",
			"/System/Library/Frameworks/Security.framework/Versions/A/Security",
			"/usr/lib/libc++.1.dylib",
			"/usr/lib/libcompression.dylib",
			"/usr/lib/libiconv.2.dylib",
			"/usr/lib/libicucore.A.dylib",
			"/usr/lib/libobjc.A.dylib",
			"/usr/lib/libSystem.B.dylib",
			"/usr/lib/libz.1.dylib",
		];

		static IEnumerable<TestCaseData> GetLinkedWithNativeLibrariesTestCases ()
		{
			// Generally speaking, whenever we bind a new framework, we'll have to adjust the LinkMode="None" test cases,
			// but we shouldn't have to adjust the LinkMode="Full" test cases (which would typically mean that we'll end
			// up linking with said framework in every app - it's also an indication that we're not trimming away as much
			// as we want, because just adding an (unused) framework shouldn't make it impossible to trim away all the
			// code in that framework).
			//
			// However, new .NET versions often require updates to both the "None" and "Full lists of frameworks and libraries.
			//

			yield return new TestCaseData (ApplePlatform.iOS, "ios-arm64", "None", expectedFrameworks_iOS_None);
			yield return new TestCaseData (ApplePlatform.iOS, "ios-arm64", "Full", expectedFrameworks_iOS_Full);
			yield return new TestCaseData (ApplePlatform.TVOS, "tvos-arm64", "None", expectedFrameworks_tvOS_None);
			yield return new TestCaseData (ApplePlatform.TVOS, "tvos-arm64", "Full", expectedFrameworks_tvOS_Full);
			yield return new TestCaseData (ApplePlatform.MacOSX, "osx-arm64", "None", expectedFrameworks_macOS_None);
			yield return new TestCaseData (ApplePlatform.MacOSX, "osx-arm64", "Full", expectedFrameworks_macOS_Full);
			yield return new TestCaseData (ApplePlatform.MacCatalyst, "maccatalyst-x64", "None", expectedFrameworks_MacCatalyst_None);
			yield return new TestCaseData (ApplePlatform.MacCatalyst, "maccatalyst-x64", "Full", expectedFrameworks_MacCatalyst_Full);
		}

		[TestCaseSource (nameof (GetLinkedWithNativeLibrariesTestCases))]
		public void LinkedWithNativeLibraries (ApplePlatform platform, string runtimeIdentifiers, string linkMode, string [] expectedFrameworks)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["MtouchLink"] = linkMode;
			properties ["LinkMode"] = linkMode;
			if (platform != ApplePlatform.MacOSX)
				properties ["UseInterpreter"] = "true"; // just to speed up the build
			DotNet.AssertBuild (project_path, properties);

			var appExecutable = GetNativeExecutable (platform, appPath);
			var actualFrameworks = GetLinkedWithFrameworks (appExecutable);
			CollectionAssert.AreEquivalent (
				expectedFrameworks.OrderBy (v => v).ToArray (),
				actualFrameworks.OrderBy (v => v).ToArray (),
				"Frameworks");
		}

		static HashSet<string> GetLinkedWithFrameworks (string path)
		{
			var rv = new HashSet<string> ();
			foreach (var file in MachO.Read (path)) {
				foreach (var lc in file.load_commands) {
					if (lc is DylibLoadCommand loadCommand) {
						rv.Add (loadCommand.name);
					}
				}
			}
			return rv;
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", "13.1")]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "10.0")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64", "10.0")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64", "10.0")]
		public void InvalidSupportedOSPlatformVersion (ApplePlatform platform, string runtimeIdentifiers, string version)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var minVersion = GetMinSupportedOSPlatformVersion (platform);
			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["SupportedOSPlatformVersion"] = version;
			properties ["ExcludeTouchUnitReference"] = "true";
			properties ["ExcludeNUnitLiteReference"] = "true";
			var rv = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			AssertErrorMessages (errors, $"The SupportedOSPlatformVersion value '{version}' in the project file is lower than the minimum value '{minVersion}'.");
		}
	}
}
