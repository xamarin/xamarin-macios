#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Xamarin.Utils;

namespace Xamarin.Tests {
	[TestFixture]
	public class BundleStructureTest : TestBaseClass {
		// Returns true if the assembly name is _any_ of our platform assemblies (Microsoft.iOS/tvOS/macOS/MacCatalyst/watchOS.dll)
		bool IsPlatformAssembly (string assemblyName)
		{
			if (assemblyName.EndsWith (".dll", StringComparison.Ordinal) || assemblyName.EndsWith (".pdb", StringComparison.Ordinal))
				assemblyName = Path.GetFileNameWithoutExtension (assemblyName);
			foreach (var platform in Enum.GetValues<ApplePlatform> ()) {
				if (platform == ApplePlatform.None)
					continue;
				var platformAssembly = Path.GetFileNameWithoutExtension (Configuration.GetBaseLibraryName (platform, true));
				if (platformAssembly == assemblyName)
					return true;
			}
			return false;
		}

		void CheckAppBundleContents (ApplePlatform platform, string appPath, string[] runtimeIdentifiers, CodeSignature isSigned, bool isReleaseBuild)
		{
			// Directory.GetFileSystemEntries will enter symlink directories and iterate inside :/
			Console.WriteLine ($"App bundle: {appPath}");
			Assert.That (appPath, Does.Exist, "App bundle existence");
			var output = AssertExecute ("find", appPath);

			var isCoreCLR = platform == ApplePlatform.MacOSX;
			var includeDebugFiles = !isReleaseBuild;
			var allFiles = output.ToString ().
								Split ('\n', StringSplitOptions.RemoveEmptyEntries).
								Where (v => v.Length > appPath.Length).
								Select (v => v.Substring (appPath.Length + 1)).ToList ();

			// Remove various files we don't care about (for this test) from the list of files in the app bundle.
			Predicate<string?> predicate = (v) => {
				var fn = Path.GetFileName (v!);

				switch (fn) {
				case "libclrjit.dylib":
				case "libcoreclr.dylib":
				case "libdbgshim.dylib":
				case "libhostfxr.dylib":
				case "libhostpolicy.dylib":
				case "libmscordaccore.dylib":
				case "libmscordbi.dylib":
					return platform == ApplePlatform.MacOSX;
				case "libmono-component-debugger.dylib":
				case "libmono-component-diagnostics_tracing.dylib":
				case "libmono-component-hot_reload.dylib":
				case "libmonosgen-2.0.dylib":
					return platform != ApplePlatform.MacOSX;
				case "libSystem.Native.dylib":
				case "libSystem.Net.Security.Native.dylib":
				case "libSystem.IO.Compression.Native.dylib":
				case "libSystem.Security.Cryptography.Native.Apple.dylib":
				case "mscorlib.dll":
				case "WindowsBase.dll":
				case "netstandard.dll":
				case "libxamarin-dotnet-debug.dylib":
				case "libxamarin-dotnet.dylib":
					return true;

				case "embedded.mobileprovision":
				case "archived-expanded-entitlements.xcent":
					return true;
				}

				if (fn.EndsWith (".aotdata.arm64", StringComparison.Ordinal) || fn.EndsWith (".aotdata.armv7", StringComparison.Ordinal))
					return true;

				if (fn.StartsWith ("System.", StringComparison.Ordinal) && (fn.EndsWith (".dll", StringComparison.Ordinal) || fn.EndsWith (".pdb", StringComparison.Ordinal)))
					return true;

				if (!IsPlatformAssembly (fn) && fn.StartsWith ("Microsoft.", StringComparison.Ordinal) && (fn.EndsWith (".dll", StringComparison.Ordinal) || fn.EndsWith (".pdb", StringComparison.Ordinal)))
					return true;

				if (fn.StartsWith ("libSystem.", StringComparison.Ordinal) && fn.EndsWith (".dylib", StringComparison.Ordinal))
					return platform == ApplePlatform.MacOSX;

				return false;
			};

			allFiles.RemoveAll (predicate);

			var expectedFiles = new List<string> ();

			var assemblyDirectory = string.Empty;
			var resourcesDirectory = string.Empty;
			var frameworksDirectory = "Frameworks";
			var pluginsDirectory = "PlugIns";
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				assemblyDirectory = "Contents/MonoBundle/";
				resourcesDirectory = "Contents/Resources/";
				frameworksDirectory = "Contents/Frameworks";
				pluginsDirectory = "Contents/PlugIns";
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			// Create a list of all the files we expect in the app bundle
			// The files here are listed in the same order they show up in shared.csproj

			// NoneA.txt is not bundled
			expectedFiles.Add ($"{assemblyDirectory}NoneB.dll");
			if (includeDebugFiles) {
				expectedFiles.Add ($"{assemblyDirectory}NoneB.pdb");
				expectedFiles.Add ($"{assemblyDirectory}NoneB.dll.mdb");
			}
			expectedFiles.Add ($"{assemblyDirectory}NoneB.config");
			if (includeDebugFiles)
				expectedFiles.Add ($"{assemblyDirectory}NoneC.pdb");
			expectedFiles.Add ($"{assemblyDirectory}NoneD.exe");
			expectedFiles.Add ($"{assemblyDirectory}libNoneE.dylib");
			// NoneF.a is not bundled
			// Sub/NoneG.txt is not bundled
			// Sub/NoneH.txt is not bundled
			// NoneI.txt is not bundled
			// NoneJ.txt is not bundled
			// NoneK.txt is not bundled
			expectedFiles.Add ($"{assemblyDirectory}NoneL.config");
			// NoneM.unknown is not bundled
			expectedFiles.Add ($"{assemblyDirectory}libSkipInstallNameTool.dylib");

			expectedFiles.Add ($"{resourcesDirectory}basn3p08.png");
			expectedFiles.Add ($"{resourcesDirectory}iTunesArtwork.jpg");

			// UnknownA.bin: None
			expectedFiles.Add ($"{assemblyDirectory}UnknownB.bin"); // UnknownB.bin: Assembly
			expectedFiles.Add ($"{resourcesDirectory}UnknownC.bin"); // UnknownC.bin: Resource
			AddExpectedFrameworkFiles (platform, expectedFiles, "UnknownD", isSigned); // UnknownD: AppleFramework
			AddExpectedFrameworkFiles (platform, expectedFiles, "UnknownE", isSigned); // UnknownE: CompressedAppleFramework
			AddExpectedFrameworkFiles (platform, expectedFiles, "UnknownF1", isSigned); // UnknownF1.bin: AppleBindingResource 
			AddExpectedFrameworkFiles (platform, expectedFiles, "UnknownF2", isSigned); // UnknownF2.bin: AppleBindingResource (compressed)
			if (isSigned == CodeSignature.None) { // we don't support signing apps with plugins (yet)
				AddExpectedPlugInFiles (platform, expectedFiles, "PlugInA", isSigned); // PlugIns
				AddExpectedPlugInFiles (platform, expectedFiles, "CompressedPlugInB", isSigned); // CompressedPlugIns
			}
																				   // UnknownI.bin: Unknown -- this should show a warning
			// SomewhatUnknownI.bin: Unknown -- this should show a warning

			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedFiles.Add ($"UnknownJ.bin"); // UnknownJ.bin: RootDirectory
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				if (isSigned == CodeSignature.None)
					expectedFiles.Add ($"UnknownJ.bin"); // UnknownJ.bin: RootDirectory
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			// SomewhatUnknownA.bin: None
			expectedFiles.Add ($"{assemblyDirectory}Subfolder");
			expectedFiles.Add ($"{assemblyDirectory}Subfolder/SomewhatUnknownB.bin"); // SomewhatUnknownB.bin: Assembly
			expectedFiles.Add ($"{resourcesDirectory}Subfolder");
			expectedFiles.Add ($"{resourcesDirectory}Subfolder/SomewhatUnknownC.bin"); // SomewhatUnknownC.bin: Resource

			AddExpectedFrameworkFiles (platform, expectedFiles, "SomewhatUnknownD", isSigned); // SomewhatUnknownD.bin: AppleFramework
			AddExpectedFrameworkFiles (platform, expectedFiles, "SomewhatUnknownE", isSigned); // SomewhatUnknownE.bin: CompressedAppleFramework
			AddExpectedFrameworkFiles (platform, expectedFiles, "SomewhatUnknownF1", isSigned); // SomewhatUnknownF1.bin: AppleBindingResource
			AddExpectedFrameworkFiles (platform, expectedFiles, "SomewhatUnknownF2", isSigned); // SomewhatUnknownF2.bin: AppleBindingResource (compressed)
			if (isSigned == CodeSignature.None) {
				AddExpectedPlugInFiles (platform, expectedFiles, "PlugInC", isSigned, "Subfolder"); // PlugIns
				AddExpectedPlugInFiles (platform, expectedFiles, "CompressedPlugInD", isSigned); // CompressedPlugIns - the Link metadata has no effect, so no subfolder.
			}
																								// SomewhatUnknownI.bin: Unknown -- this should show a warning
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedFiles.Add ($"Subfolder");
				expectedFiles.Add ($"Subfolder/SomewhatUnknownJ.bin"); // SomewhatUnknownJ.bin: RootDirectory
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				if (isSigned == CodeSignature.None) {
					expectedFiles.Add ($"Subfolder");
					expectedFiles.Add ($"Subfolder/SomewhatUnknownJ.bin"); // SomewhatUnknownJ.bin: RootDirectory
				}
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			expectedFiles.Add ($"{resourcesDirectory}ContentA.txt");
			expectedFiles.Add ($"{resourcesDirectory}ContentB.txt");
			expectedFiles.Add ($"{resourcesDirectory}ContentC.txt");
			expectedFiles.Add ($"{resourcesDirectory}ContentD.txt");
			expectedFiles.Add ($"{resourcesDirectory}ContentE.txt");
			expectedFiles.Add ($"{resourcesDirectory}ContentI.txt");

			// expectedFiles.Add ($"{resourcesDirectory}EmbeddedResourceA.txt");
			expectedFiles.Add ($"{resourcesDirectory}EmbeddedResourceB.txt");
			expectedFiles.Add ($"{resourcesDirectory}EmbeddedResourceC.txt");
			// expectedFiles.Add ($"{resourcesDirectory}EmbeddedResourceD.txt");
			// expectedFiles.Add ($"{resourcesDirectory}EmbeddedResourceE.txt");

			expectedFiles.Add ($"{resourcesDirectory}BundleResourceA.txt");
			expectedFiles.Add ($"{resourcesDirectory}BundleResourceB.txt");
			expectedFiles.Add ($"{resourcesDirectory}BundleResourceC.txt");
			expectedFiles.Add ($"{resourcesDirectory}BundleResourceD.txt");
			expectedFiles.Add ($"{resourcesDirectory}BundleResourceE.txt");
			expectedFiles.Add ($"{resourcesDirectory}BundleResourceI.txt");

			expectedFiles.Add ($"{resourcesDirectory}AutoIncluded.txt");
			expectedFiles.Add ($"{resourcesDirectory}SubDirectory");
			expectedFiles.Add ($"{resourcesDirectory}SubDirectory/AutoIncluded2.txt");

			expectedFiles.Add ($"{assemblyDirectory}FrameworksInRuntimesNativeDirectory.dll");
			AddExpectedFrameworkFiles (platform, expectedFiles, "FrameworksInRuntimesNativeDirectory1", isSigned);
			AddExpectedFrameworkFiles (platform, expectedFiles, "FrameworksInRuntimesNativeDirectory2", isSigned);

			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				AddExpectedFrameworkFiles (platform, expectedFiles, "FrameworkTest2", isSigned);
				AddExpectedFrameworkFiles (platform, expectedFiles, "FrameworkTest3", isSigned);
				break;
			}

			AddExpectedFrameworkFiles (platform, expectedFiles, "FrameworkTest4", isSigned);
			AddExpectedFrameworkFiles (platform, expectedFiles, "FrameworkTest5", isSigned);

			expectedFiles.Add ($"{assemblyDirectory}bindings-framework-test.dll");
			if (includeDebugFiles)
				expectedFiles.Add ($"{assemblyDirectory}bindings-framework-test.pdb");
			AddExpectedFrameworkFiles (platform, expectedFiles, "XTest", isSigned);

			// various directories
			expectedFiles.Add (frameworksDirectory);
			if (isSigned == CodeSignature.None) {
				expectedFiles.Add (pluginsDirectory);
				expectedFiles.Add ($"{pluginsDirectory}/Subfolder");
			}

			// misc other files not directly related to the test itself
			if (!isCoreCLR)
				expectedFiles.Add ($"{assemblyDirectory}icudt.dat");
			AddMultiRidAssembly (platform, expectedFiles, assemblyDirectory, "BundleStructure", runtimeIdentifiers, addConfig: true, includeDebugFiles: includeDebugFiles);
			if (platform != ApplePlatform.MacOSX)
				AddMultiRidAssembly (platform, expectedFiles, assemblyDirectory, "MonoTouch.Dialog", runtimeIdentifiers, forceSingleRid: true, includeDebugFiles: includeDebugFiles);
			expectedFiles.Add ($"{assemblyDirectory}nunit.framework.dll");
			expectedFiles.Add ($"{assemblyDirectory}nunitlite.dll");
			expectedFiles.Add ($"{assemblyDirectory}Touch.Client.dll");
			if (includeDebugFiles)
				expectedFiles.Add ($"{assemblyDirectory}Touch.Client.pdb");
			AddMultiRidAssembly (platform, expectedFiles, assemblyDirectory, Path.GetFileNameWithoutExtension (Configuration.GetBaseLibraryName (platform, true)), runtimeIdentifiers, forceSingleRid: (platform == ApplePlatform.MacCatalyst && !isReleaseBuild) || platform == ApplePlatform.MacOSX, hasPdb: false, includeDebugFiles: includeDebugFiles);
			expectedFiles.Add ($"{assemblyDirectory}runtimeconfig.bin");

			if (platform == ApplePlatform.MacOSX)
				expectedFiles.Add ("Contents/MonoBundle/createdump");

			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedFiles.Add ("BundleStructure");
				expectedFiles.Add ("Info.plist");
				if (!isReleaseBuild)
					expectedFiles.Add ("MonoTouchDebugConfiguration.txt");
				expectedFiles.Add ("PkgInfo");
				if (!isReleaseBuild) {
					expectedFiles.Add ("Settings.bundle");
					expectedFiles.Add ("Settings.bundle/Root.plist");
				}
				break;
			case ApplePlatform.MacCatalyst:
				if (!isReleaseBuild)
					expectedFiles.Add ("Contents/Resources/MonoTouchDebugConfiguration.txt");
				goto case ApplePlatform.MacOSX;
			case ApplePlatform.MacOSX:
				expectedFiles.Add ("Contents");
				expectedFiles.Add ("Contents/Info.plist");
				expectedFiles.Add ("Contents/MacOS");
				expectedFiles.Add ("Contents/MacOS/BundleStructure");
				expectedFiles.Add ("Contents/MonoBundle");
				expectedFiles.Add ("Contents/PkgInfo");
				expectedFiles.Add ("Contents/Resources");
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			if (isSigned == CodeSignature.All) {
				switch (platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					expectedFiles.Add ($"_CodeSignature");
					expectedFiles.Add ($"_CodeSignature/CodeResources");
					break;
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.MacOSX:
					expectedFiles.Add ($"Contents/_CodeSignature");
					expectedFiles.Add ($"Contents/_CodeSignature/CodeResources");
					break;
				default:
					throw new NotImplementedException ($"Unknown platform: {platform}");
				}
			}

			var unexpectedFiles = allFiles.Except (expectedFiles).OrderBy (v => v).ToArray ();
			var missingFiles = expectedFiles.Except (allFiles).OrderBy (v => v).ToArray ();

#if false
			// Debug code to print out what we got
			if (unexpectedFiles.Any () || missingFiles.Any ()) {
				Console.WriteLine ($"All files in the bundle ({allFiles.Count ()})");
				foreach (var file in allFiles.OrderBy (v => v)) {
					Console.WriteLine ($"    {file}");
				}
				Console.WriteLine ("---------------------------------------");
			}
			Console.WriteLine ($"Found {unexpectedFiles.Count ()} unexpected files");
			foreach (var file in unexpectedFiles)
				Console.WriteLine ($"Unexpected file: {file}");
			Console.WriteLine ($"Found {missingFiles.Count ()} missing files");
			foreach (var file in missingFiles)
				Console.WriteLine ($"Missing file: {file}");
#endif

			Assert.That (unexpectedFiles, Is.Empty, "No unexpected files");
			Assert.That (missingFiles, Is.Empty, "No missing files");

			AssertDynamicLibraryId (platform, appPath, assemblyDirectory, "libSkipInstallNameTool.dylib");
		}

		void AssertDynamicLibraryId (ApplePlatform platform, string appPath, string dylibDirectory, string library)
		{
			var dylibPath = Path.Combine (appPath, dylibDirectory, library);
			Assert.That (dylibPath, Does.Exist, "dylib existence");

			var invalidLoadCommands = new List<string> ();

			var appExecutable = GetNativeExecutable (platform, appPath);
			foreach (var file in MachO.Read (appExecutable)) {
				foreach (var lc in file.load_commands) {
					if (lc is DylibLoadCommand loadCommand) {
						if (!IsValidLoadLibrary (loadCommand.name)) {
							invalidLoadCommands.Add ($"Invalid load library '{loadCommand.name}' in '{file.Filename}'");
						}
					}
				}
			}

			var dylibs = Directory.GetFiles (Path.Combine (appPath, dylibDirectory), "*.dylib");
			foreach (var dylib in dylibs) {
				foreach (var file in MachO.Read (dylib)) {
					foreach (var lc in file.load_commands) {
						if (lc is DylibIdCommand loadCommand) {
							if (!IsValidLoadLibrary (loadCommand.name)) {
								invalidLoadCommands.Add ($"Invalid id '{loadCommand.name}' for library '{file.Filename}'");
							}
						}
					}
				}
			}

			Assert.That (invalidLoadCommands, Is.Empty);
		}

		static bool IsValidLoadLibrary (string library)
		{
			var valid_prefixes = new string [] {
				"/System/Library/",
				"/System/iOSSupport/System/Library/",
				"/usr/lib/",
				"@rpath",
				"@executable_path",
			};
			foreach (var valid_prefix in valid_prefixes) {
				if (library.StartsWith (valid_prefix, StringComparison.Ordinal))
					return true;
			}
			return false;
		}


		static void AddMultiRidAssembly (ApplePlatform platform, List<string> expectedFiles, string assemblyDirectory, string assemblyName, string[] runtimeIdentifiers, bool forceSingleRid = false, bool hasPdb = true, bool addConfig = false, bool includeDebugFiles = false)
		{
			if (forceSingleRid || runtimeIdentifiers.Length == 1) {
				expectedFiles.Add ($"{assemblyDirectory}{assemblyName}.dll");
				if (hasPdb && includeDebugFiles)
					expectedFiles.Add ($"{assemblyDirectory}{assemblyName}.pdb");
				if (addConfig)
					expectedFiles.Add ($"{assemblyDirectory}{assemblyName}.dll.config");
			} else {
				expectedFiles.Add ($"{assemblyDirectory}.xamarin");
				foreach (var rid in runtimeIdentifiers) {
					expectedFiles.Add ($"{assemblyDirectory}.xamarin/{rid}");
					expectedFiles.Add ($"{assemblyDirectory}.xamarin/{rid}/{assemblyName}.dll");
					if (hasPdb && includeDebugFiles)
						expectedFiles.Add ($"{assemblyDirectory}.xamarin/{rid}/{assemblyName}.pdb");
					if (addConfig)
						expectedFiles.Add ($"{assemblyDirectory}.xamarin/{rid}/{assemblyName}.dll.config");
				}
			}
		}

		static void AddExpectedPlugInFiles (ApplePlatform platform, List<string> expectedFiles, string pluginName, CodeSignature signature, string subdirectory = "")
		{
			var isSigned = signature != CodeSignature.None;
			var pluginsDirectory = "PlugIns";
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				pluginsDirectory = "Contents/PlugIns";
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			pluginsDirectory = Path.Combine (pluginsDirectory, subdirectory);

			expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle");
			expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/{pluginName}");
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Info.plist");
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Resources");
				expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Versions");
				expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Versions/A");
				expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Versions/A/Resources");
				expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Versions/A/Resources/Info.plist");
				expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Versions/A/{pluginName}");
				expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Versions/Current");
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			if (isSigned) {
				switch (platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/_CodeSignature");
					expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/_CodeSignature/CodeResources");
					break;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Versions/A/_CodeSignature");
					expectedFiles.Add ($"{pluginsDirectory}/{pluginName}.bundle/Versions/A/_CodeSignature/CodeResources");
					break;
				default:
					throw new NotImplementedException ($"Unknown platform: {platform}");
				}
			}
		}

		static void AddExpectedFrameworkFiles (ApplePlatform platform, List<string> expectedFiles, string frameworkName, CodeSignature signature, string subdirectory = "")
		{
			var isSigned = signature != CodeSignature.None;
			var frameworksDirectory = "Frameworks";
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				frameworksDirectory = "Contents/Frameworks";
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework");
			expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/{frameworkName}");
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Info.plist");
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Resources");
				expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Versions");
				expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Versions/A");
				expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Versions/A/Resources");
				expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Versions/A/Resources/Info.plist");
				expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Versions/A/{frameworkName}");
				expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Versions/Current");
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			if (isSigned) {
				switch (platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/_CodeSignature");
					expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/_CodeSignature/CodeResources");
					break;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Versions/A/_CodeSignature");
					expectedFiles.Add ($"{frameworksDirectory}/{frameworkName}.framework/Versions/A/_CodeSignature/CodeResources");
					break;
				default:
					throw new NotImplementedException ($"Unknown platform: {platform}");
				}
			}
		}

		public enum CodeSignature {
			None,
			Frameworks,
			All,
		}

		[Test]
		// Debug
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm", CodeSignature.All, "Debug")]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", CodeSignature.Frameworks, "Debug")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", CodeSignature.All, "Debug")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64;maccatalyst-arm64", CodeSignature.All, "Debug")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", CodeSignature.None, "Debug")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64;osx-arm64", CodeSignature.None, "Debug")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64", CodeSignature.All, "Debug")]
		// Release
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm", CodeSignature.All, "Release")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64;maccatalyst-arm64", CodeSignature.All, "Release")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", CodeSignature.Frameworks, "Release")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64", CodeSignature.All, "Release")]
		public void Build (ApplePlatform platform, string runtimeIdentifiers, CodeSignature signature, string configuration)
		{
			var project = "BundleStructure";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			var project_dir = Path.GetDirectoryName (Path.GetDirectoryName (project_path))!;
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["_IsAppSigned"] = signature != CodeSignature.None ? "true" : "false";
			if (!string.IsNullOrWhiteSpace (configuration))
				properties ["Configuration"] = configuration;
			var rv = DotNet.AssertBuild (project_path, properties);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			var warningMessages = FilterWarnings (warnings);

			var isReleaseBuild = string.Equals (configuration, "Release", StringComparison.OrdinalIgnoreCase);
			var platformString = platform.AsString ();
			var tfm = platform.ToFramework ();
			var testsDirectory = Path.GetDirectoryName (Path.GetDirectoryName (project_dir));
			var expectedWarnings = new string [] {
				$"The 'PublishFolderType' metadata value 'Unknown' on the item '{project_dir}/{platformString}/SomewhatUnknownI.bin' is not recognized. The file will not be copied to the app bundle. If the file is not supposed to be copied to the app bundle, remove the 'CopyToOutputDirectory' metadata on the item.",
				$"The 'PublishFolderType' metadata value 'Unknown' on the item '{project_dir}/{platformString}/UnknownI.bin' is not recognized. The file will not be copied to the app bundle. If the file is not supposed to be copied to the app bundle, remove the 'CopyToOutputDirectory' metadata on the item.",
				$"The file '{project_dir}/{platformString}/NoneA.txt' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{project_dir}/{platformString}/NoneI.txt' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{project_dir}/{platformString}/NoneJ.txt' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{project_dir}/{platformString}/NoneK.txt' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{project_dir}/{platformString}/NoneM.unknown' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{project_dir}/{platformString}/Sub/NoneG.txt' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{project_dir}/NoneH.txt' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
			}.ToList ();

			var rids = runtimeIdentifiers.Split (';');
			if (rids.Length > 1) {
				// All warnings show up twice if we're building for multiple architectures
				expectedWarnings.AddRange (expectedWarnings);
			}

			var zippedFrameworks = platform == ApplePlatform.MacCatalyst || platform == ApplePlatform.MacOSX;
			foreach (var rid in rids) {
				if (zippedFrameworks) {
					expectedWarnings.Add ($"The framework obj/{configuration}/{tfm}/{rid}/bindings-framework-test.resources.zip/XStaticObjectTest.framework is a framework of static libraries, and will not be copied to the app.");
					expectedWarnings.Add ($"The framework obj/{configuration}/{tfm}/{rid}/bindings-framework-test.resources.zip/XStaticArTest.framework is a framework of static libraries, and will not be copied to the app.");
				} else {
					expectedWarnings.Add ($"The framework {testsDirectory}/bindings-framework-test/dotnet/{platformString}/bin/{configuration}/{tfm}/bindings-framework-test.resources/XStaticObjectTest.framework is a framework of static libraries, and will not be copied to the app.");
					expectedWarnings.Add ($"The framework {testsDirectory}/bindings-framework-test/dotnet/{platformString}/bin/{configuration}/{tfm}/bindings-framework-test.resources/XStaticArTest.framework is a framework of static libraries, and will not be copied to the app.");
				}
			}

			if (signature == CodeSignature.None && (platform == ApplePlatform.MacCatalyst || platform == ApplePlatform.MacOSX)) {
				expectedWarnings.Add ($"Found files in the root directory of the app bundle. This will likely cause codesign to fail. Files:\nbin/{configuration}/{tfm}{(runtimeIdentifiers.IndexOf (';') >= 0 ? string.Empty : "/" + runtimeIdentifiers)}/BundleStructure.app/UnknownJ.bin");
			}

			// Sort the messages so that comparison against the expected array is faster
			expectedWarnings = expectedWarnings
				.OrderBy (v => v)
				.ToList ();

			var appExecutable = GetNativeExecutable (platform, appPath);

			CheckAppBundleContents (platform, appPath, rids, signature, isReleaseBuild);
			CollectionAssert.AreEqual (expectedWarnings, warningMessages, "Warnings");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// touch AppDelegate.cs, and rebuild should succeed and do the right thing
			var appDelegatePath = Path.Combine (project_dir, "AppDelegate.cs");
			Configuration.Touch (appDelegatePath);

			rv = DotNet.AssertBuild (project_path, properties);
			warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			warningMessages = FilterWarnings (warnings);

			CheckAppBundleContents (platform, appPath, rids, signature, isReleaseBuild);
			CollectionAssert.AreEqual (expectedWarnings, warningMessages, "Warnings Rebuild 1");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// remove the bin directory, and rebuild should succeed and do the right thing
			var binDirectory = Path.Combine (Path.GetDirectoryName (project_path)!, "bin");
			Directory.Delete (binDirectory, true);

			rv = DotNet.AssertBuild (project_path, properties);
			warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			warningMessages = FilterWarnings (warnings);

			CheckAppBundleContents (platform, appPath, rids, signature, isReleaseBuild);
			CollectionAssert.AreEqual (expectedWarnings, warningMessages, "Warnings Rebuild 2");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// a simple rebuild should succeed
			rv = DotNet.AssertBuild (project_path, properties);
			warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			warningMessages = FilterWarnings (warnings);

			CheckAppBundleContents (platform, appPath, rids, signature, isReleaseBuild);
			CollectionAssert.AreEqual (expectedWarnings, warningMessages, "Warnings Rebuild 3");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);
		}

		string[] FilterWarnings (IEnumerable<BuildLogEvent> warnings)
		{
			return warnings
				.Select (v => v?.Message!).Where (v => !string.IsNullOrWhiteSpace (v))
				// Remove warnings of the form "This call site is reachable on: '...' and later. 'TheAPI' is only supported on: '...' and later."
				.Where (v => !v.StartsWith ("This call site is reachable on:"))
				// Remove CLSCompliant warnings
				.Where (v => !v.Contains ("does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute"))
				// Remove obsolete warnings
				.Where (v => !v.Contains (" is obsolete: "))
				// More obsolete warnings
				.Where (v => !v.Contains (" overrides obsolete member "))
				// Don't care about this
				.Where (v => !v.Contains ("Supported iPhone orientations have not been set"))
				// Sort the messages so that comparison against the expected array is faster
				.OrderBy (v => v)
				.ToArray ();

		}
	}
}
