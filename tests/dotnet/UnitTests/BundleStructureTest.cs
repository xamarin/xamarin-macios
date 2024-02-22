#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class BundleStructureTest : TestBaseClass {
		// Returns true if the assembly name is _any_ of our platform assemblies (Microsoft.iOS/tvOS/macOS/MacCatalyst/watchOS.dll)
		static bool IsPlatformAssembly (string assemblyName)
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

		public static List<string> Find (string appPath)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				var dir = new DirectoryInfo (appPath);
				var managedFiles = dir.GetFileSystemInfos ("*", SearchOption.AllDirectories)
									.Select (v => v.FullName)
									.Select (v => v.Substring (appPath.Length + 1))
									.Order ()
									.ToList ();
				return managedFiles;
			}

			// Directory.GetFileSystemEntries will enter symlink directories and iterate inside :/
			var output = AssertExecute ("find", appPath);
			var allFiles = output.ToString ()
								.Split ('\n', StringSplitOptions.RemoveEmptyEntries)
								.Where (v => v.Length > appPath.Length)
								.Select (v => v.Substring (appPath.Length + 1))
								.Order ()
								.ToList ();

			return allFiles;
		}

		internal static void CheckAppBundleContents (ApplePlatform platform, string appPath, string [] runtimeIdentifiers, CodeSignature isSigned, bool isReleaseBuild)
		{
			Console.WriteLine ($"App bundle: {appPath}");
			Assert.That (appPath, Does.Exist, "App bundle existence");
			var allFiles = Find (appPath);
			CheckAppBundleContents (platform, allFiles, runtimeIdentifiers, isSigned, isReleaseBuild, appPath);
		}

		internal static void CheckAppBundleContents (ApplePlatform platform, IEnumerable<string> allFiles, string [] runtimeIdentifiers, CodeSignature isSigned, bool isReleaseBuild, string? appPath = null)
		{
			var isCoreCLR = platform == ApplePlatform.MacOSX;
			var includeDebugFiles = !isReleaseBuild;

			// Remove various files we don't care about (for this test) from the list of files in the app bundle.
			Predicate<string?> predicate = (v) => {
				var fn = Path.GetFileName (v!);

				switch (fn) {
				case "libclrgc.dylib":
				case "libclrgcexp.dylib":
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
				case "libmono-component-marshal-ilgen.dylib":
				case "libmonosgen-2.0.dylib":
					return platform != ApplePlatform.MacOSX;
				case "libSystem.Native.dylib":
				case "libSystem.Net.Security.Native.dylib":
				case "libSystem.Globalization.Native.dylib":
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

			allFiles = allFiles.Where (v => !predicate (v));

			var expectedFiles = new List<string> ();

			var assemblyDirectory = string.Empty;
			var resourcesDirectory = string.Empty;
			var frameworksDirectory = "Frameworks";
			var pluginsDirectory = "PlugIns";
			var xpcServicesDirectory = "XPCServices";
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				assemblyDirectory = Path.Combine ("Contents", "MonoBundle");
				resourcesDirectory = Path.Combine ("Contents", "Resources");
				frameworksDirectory = Path.Combine ("Contents", "Frameworks");
				pluginsDirectory = Path.Combine ("Contents", "PlugIns");
				xpcServicesDirectory = Path.Combine ("Contents", "XPCServices");
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			// Create a list of all the files we expect in the app bundle
			// The files here are listed in the same order they show up in shared.csproj

			// NoneA.txt is not bundled
			expectedFiles.Add (Path.Combine (assemblyDirectory, "NoneB.dll"));
			if (includeDebugFiles) {
				expectedFiles.Add (Path.Combine (assemblyDirectory, "NoneB.pdb"));
				expectedFiles.Add (Path.Combine (assemblyDirectory, "NoneB.dll.mdb"));
			}
			expectedFiles.Add (Path.Combine (assemblyDirectory, "NoneB.config"));
			if (includeDebugFiles)
				expectedFiles.Add (Path.Combine (assemblyDirectory, "NoneC.pdb"));
			expectedFiles.Add (Path.Combine (assemblyDirectory, "NoneD.exe"));
			expectedFiles.Add (Path.Combine (assemblyDirectory, "libNoneE.dylib"));
			expectedFiles.Add (Path.Combine (assemblyDirectory, "libNoneE.so"));
			// NoneF.a is not bundled
			// Sub/NoneG.txt is not bundled
			// Sub/NoneH.txt is not bundled
			// NoneI.txt is not bundled
			// NoneJ.txt is not bundled
			// NoneK.txt is not bundled
			expectedFiles.Add (Path.Combine (assemblyDirectory, "NoneL.config"));
			// NoneM.unknown is not bundled
			expectedFiles.Add (Path.Combine (assemblyDirectory, "libSkipInstallNameTool.dylib"));
			expectedFiles.Add (Path.Combine (assemblyDirectory, "libSkipInstallNameTool.so"));

			expectedFiles.Add (Path.Combine (resourcesDirectory, "basn3p08.png"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "basn3p08_with_loc.png"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "iTunesArtwork.jpg"));

			// NoneN.dll: bundled (assembly) - but the xml file next to it should not be bundled.
			expectedFiles.Add (Path.Combine (assemblyDirectory, "NoneN.dll"));

			// NoneP.dll: bundled (assembly) - but the xml file with the same base name should not be bundled.
			expectedFiles.Add (Path.Combine (assemblyDirectory, "NoneP.dll"));

			// UnknownA.bin: None
			expectedFiles.Add (Path.Combine (assemblyDirectory, "UnknownB.bin")); // UnknownB.bin: Assembly
			expectedFiles.Add (Path.Combine (resourcesDirectory, "UnknownC.bin")); // UnknownC.bin: Resource
			AddExpectedFrameworkFiles (platform, expectedFiles, "UnknownD", isSigned); // UnknownD: AppleFramework
			AddExpectedFrameworkFiles (platform, expectedFiles, "UnknownE", isSigned); // UnknownE: CompressedAppleFramework
			AddExpectedFrameworkFiles (platform, expectedFiles, "UnknownF1", isSigned); // UnknownF1.bin: AppleBindingResource 
			AddExpectedFrameworkFiles (platform, expectedFiles, "UnknownF2", isSigned); // UnknownF2.bin: AppleBindingResource (compressed)
			if (isSigned == CodeSignature.None) { // we don't support signing apps with plugins (yet)
				AddExpectedPlugInFiles (platform, expectedFiles, "PlugInA", isSigned); // PlugIns
				AddExpectedPlugInFiles (platform, expectedFiles, "CompressedPlugInB", isSigned); // CompressedPlugIns
			}
			AddExpectedXpcServicesFiles (platform, expectedFiles, "XpcServiceE", isSigned); // XPCServices
			AddExpectedXpcServicesFiles (platform, expectedFiles, "CompressedXpcServiceF", isSigned); // CompressedXPCServices

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

			expectedFiles.Add (Path.Combine (assemblyDirectory, "Subfolder"));
			expectedFiles.Add (Path.Combine (assemblyDirectory, "Subfolder", "SomewhatUnknownB.bin")); // SomewhatUnknownB.bin: Assembly
			expectedFiles.Add (Path.Combine (resourcesDirectory, "Subfolder"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "Subfolder", "SomewhatUnknownC.bin")); // SomewhatUnknownC.bin: Resource

			AddExpectedFrameworkFiles (platform, expectedFiles, "SomewhatUnknownD", isSigned); // SomewhatUnknownD.bin: AppleFramework
			AddExpectedFrameworkFiles (platform, expectedFiles, "SomewhatUnknownE", isSigned); // SomewhatUnknownE.bin: CompressedAppleFramework
			AddExpectedFrameworkFiles (platform, expectedFiles, "SomewhatUnknownF1", isSigned); // SomewhatUnknownF1.bin: AppleBindingResource
			AddExpectedFrameworkFiles (platform, expectedFiles, "SomewhatUnknownF2", isSigned); // SomewhatUnknownF2.bin: AppleBindingResource (compressed)
			if (isSigned == CodeSignature.None) {
				AddExpectedPlugInFiles (platform, expectedFiles, "PlugInC", isSigned, "Subfolder"); // PlugIns
				AddExpectedPlugInFiles (platform, expectedFiles, "CompressedPlugInD", isSigned); // CompressedPlugIns - the Link metadata has no effect, so no subfolder.
			}
			AddExpectedXpcServicesFiles (platform, expectedFiles, "XpcServiceG", isSigned, "Subfolder"); // XPCServices
			AddExpectedXpcServicesFiles (platform, expectedFiles, "CompressedXpcServiceH", isSigned); // CompressedXPCServices - the Link metadata has no effect, so no subfolder.

			// SomewhatUnknownI.bin: Unknown -- this should show a warning

			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedFiles.Add ($"Subfolder");
				expectedFiles.Add (Path.Combine ($"Subfolder", "SomewhatUnknownJ.bin")); // SomewhatUnknownJ.bin: RootDirectory
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				if (isSigned == CodeSignature.None) {
					expectedFiles.Add ($"Subfolder");
					expectedFiles.Add (Path.Combine ($"Subfolder", "SomewhatUnknownJ.bin")); // SomewhatUnknownJ.bin: RootDirectory
				}
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			AddExpectedFrameworkFiles (platform, expectedFiles, "Framework.With.Dots", isSigned); // https://github.com/xamarin/xamarin-macios/issues/15727

			expectedFiles.Add (Path.Combine (assemblyDirectory, "NoLibPrefix.dylib")); // https://github.com/xamarin/xamarin-macios/issues/15044

			expectedFiles.Add (Path.Combine (resourcesDirectory, "ContentA.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "ContentB.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "ContentC.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "ContentD.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "ContentE.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "ContentI.txt"));

			// expectedFiles.Add (Path.Combine (resourcesDirectory, "EmbeddedResourceA.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "EmbeddedResourceB.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "EmbeddedResourceC.txt"));
			// expectedFiles.Add (Path.Combine (resourcesDirectory, "EmbeddedResourceD.txt"));
			// expectedFiles.Add (Path.Combine (resourcesDirectory, "EmbeddedResourceE.txt"));

			expectedFiles.Add (Path.Combine (resourcesDirectory, "BundleResourceA.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "BundleResourceB.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "BundleResourceC.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "BundleResourceD.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "BundleResourceE.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "BundleResourceI.txt"));

			expectedFiles.Add (Path.Combine (resourcesDirectory, "AutoIncluded.txt"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "SubDirectory"));
			expectedFiles.Add (Path.Combine (resourcesDirectory, "SubDirectory", "AutoIncluded2.txt"));

			expectedFiles.Add (Path.Combine (assemblyDirectory, "FrameworksInRuntimesNativeDirectory.dll"));
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

			expectedFiles.Add (Path.Combine (assemblyDirectory, "bindings-framework-test.dll"));
			if (includeDebugFiles)
				expectedFiles.Add (Path.Combine (assemblyDirectory, "bindings-framework-test.pdb"));
			AddExpectedFrameworkFiles (platform, expectedFiles, "XTest", isSigned);

			// various directories
			expectedFiles.Add (frameworksDirectory);
			if (isSigned == CodeSignature.None) {
				expectedFiles.Add (pluginsDirectory);
				expectedFiles.Add (Path.Combine (pluginsDirectory, "Subfolder"));
			}
			expectedFiles.Add (xpcServicesDirectory);
			expectedFiles.Add (Path.Combine (xpcServicesDirectory, "Subfolder"));

			// misc other files not directly related to the test itself
			AddMultiRidAssembly (platform, expectedFiles, assemblyDirectory, "BundleStructure", runtimeIdentifiers, addConfig: true, includeDebugFiles: includeDebugFiles);
			if (platform != ApplePlatform.MacOSX)
				AddMultiRidAssembly (platform, expectedFiles, assemblyDirectory, "MonoTouch.Dialog", runtimeIdentifiers, forceSingleRid: (platform == ApplePlatform.MacCatalyst && !isReleaseBuild), includeDebugFiles: includeDebugFiles);
			expectedFiles.Add (Path.Combine (assemblyDirectory, "nunit.framework.dll"));
			expectedFiles.Add (Path.Combine (assemblyDirectory, "nunitlite.dll"));
			bool forceSingleRid = (platform == ApplePlatform.MacCatalyst && !isReleaseBuild) || platform == ApplePlatform.MacOSX;
			AddMultiRidAssembly (platform, expectedFiles, assemblyDirectory, "Touch.Client", runtimeIdentifiers, forceSingleRid, includeDebugFiles: includeDebugFiles);
			AddMultiRidAssembly (platform, expectedFiles, assemblyDirectory, Path.GetFileNameWithoutExtension (Configuration.GetBaseLibraryName (platform, true)), runtimeIdentifiers, forceSingleRid, includeDebugFiles: includeDebugFiles);
			expectedFiles.Add (Path.Combine (assemblyDirectory, "runtimeconfig.bin"));

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
					expectedFiles.Add (Path.Combine ("Settings.bundle", "Root.plist"));
				}
				break;
			case ApplePlatform.MacCatalyst:
				if (!isReleaseBuild)
					expectedFiles.Add (Path.Combine ("Contents", "Resources", "MonoTouchDebugConfiguration.txt"));
				goto case ApplePlatform.MacOSX;
			case ApplePlatform.MacOSX:
				expectedFiles.Add ("Contents");
				expectedFiles.Add (Path.Combine ("Contents", "Info.plist"));
				expectedFiles.Add (Path.Combine ("Contents", "MacOS"));
				expectedFiles.Add (Path.Combine ("Contents", "MacOS/BundleStructure"));
				expectedFiles.Add (Path.Combine ("Contents", "MonoBundle"));
				expectedFiles.Add (Path.Combine ("Contents", "PkgInfo"));
				expectedFiles.Add (Path.Combine ("Contents", "Resources"));
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			if (isSigned == CodeSignature.All) {
				switch (platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					expectedFiles.Add ("_CodeSignature");
					expectedFiles.Add (Path.Combine ("_CodeSignature", "CodeResources"));
					break;
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.MacOSX:
					expectedFiles.Add (Path.Combine ("Contents", "_CodeSignature"));
					expectedFiles.Add (Path.Combine ("Contents", "_CodeSignature", "CodeResources"));
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

			if (appPath is not null) {
				AssertDynamicLibraryId (platform, appPath, assemblyDirectory, "libSkipInstallNameTool.dylib");
				AssertDynamicLibraryId (platform, appPath, assemblyDirectory, "libSkipInstallNameTool.so");
				AssertLibraryArchitectures (appPath, runtimeIdentifiers);
			}
		}

		static void AssertDynamicLibraryId (ApplePlatform platform, string appPath, string dylibDirectory, string library)
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


		static void AddMultiRidAssembly (ApplePlatform platform, List<string> expectedFiles, string assemblyDirectory, string assemblyName, string [] runtimeIdentifiers, bool forceSingleRid = false, bool addConfig = false, bool includeDebugFiles = false)
		{
			if (forceSingleRid || runtimeIdentifiers.Length == 1) {
				expectedFiles.Add (Path.Combine (assemblyDirectory, $"{assemblyName}.dll"));
				if (includeDebugFiles)
					expectedFiles.Add (Path.Combine (assemblyDirectory, $"{assemblyName}.pdb"));
				if (addConfig)
					expectedFiles.Add (Path.Combine (assemblyDirectory, $"{assemblyName}.dll.config"));
			} else {
				expectedFiles.Add (Path.Combine (assemblyDirectory, ".xamarin"));
				foreach (var rid in runtimeIdentifiers) {
					expectedFiles.Add (Path.Combine (Path.Combine (assemblyDirectory, ".xamarin", $"{rid}")));
					expectedFiles.Add (Path.Combine (Path.Combine (assemblyDirectory, ".xamarin", $"{rid}", $"{assemblyName}.dll")));
					if (includeDebugFiles)
						expectedFiles.Add (Path.Combine (Path.Combine (assemblyDirectory, ".xamarin", $"{rid}", $"{assemblyName}.pdb")));
					if (addConfig)
						expectedFiles.Add (Path.Combine (Path.Combine (assemblyDirectory, ".xamarin", $"{rid}", $"{assemblyName}.dll.config")));
				}
			}
		}

		static void AddExpectedPlugInFiles (ApplePlatform platform, List<string> expectedFiles, string pluginName, CodeSignature signature, string subdirectory = "")
		{
			AddExpectedExtensionFiles (platform, expectedFiles, pluginName, signature, subdirectory, "PlugIns", "bundle");
		}

		static void AddExpectedXpcServicesFiles (ApplePlatform platform, List<string> expectedFiles, string xpcName, CodeSignature signature, string subdirectory = "")
		{
			AddExpectedExtensionFiles (platform, expectedFiles, xpcName, signature, subdirectory, "XPCServices", "xpc");
		}

		static void AddExpectedExtensionFiles (ApplePlatform platform, List<string> expectedFiles, string extensionName, CodeSignature signature, string subdirectory, string extensionType, string extensionExtension)
		{
			var isSigned = signature != CodeSignature.None;
			var extensionDirectory = extensionType;
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				extensionDirectory = Path.Combine ("Contents", extensionType);
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			extensionDirectory = Path.Combine (extensionDirectory, subdirectory);

			expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}"));
			expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", extensionName));
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Info.plist"));
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Resources"));
				expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Versions"));
				expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Versions", "A"));
				expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Versions", "A", "Resources"));
				expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Versions", "A", "Resources", "Info.plist"));
				expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Versions", "A", extensionName));
				expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Versions", "Current"));
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			if (isSigned) {
				switch (platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "_CodeSignature"));
					expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "_CodeSignature", "CodeResources"));
					break;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Versions", "A", "_CodeSignature"));
					expectedFiles.Add (Path.Combine (extensionDirectory, $"{extensionName}.{extensionExtension}", "Versions", "A", "_CodeSignature", "CodeResources"));
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
				frameworksDirectory = Path.Combine ("Contents", "Frameworks");
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework"));
			expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", frameworkName));
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Info.plist"));
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Resources"));
				expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Versions"));
				expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Versions", "A"));
				expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Versions", "A", "Resources"));
				expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Versions", "A", "Resources", "Info.plist"));
				expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Versions", "A", frameworkName));
				expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Versions", "Current"));
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			if (isSigned) {
				switch (platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "_CodeSignature"));
					expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "_CodeSignature", "CodeResources"));
					break;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Versions", "A", "_CodeSignature"));
					expectedFiles.Add (Path.Combine (frameworksDirectory, $"{frameworkName}.framework", "Versions", "A", "_CodeSignature", "CodeResources"));
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
		[TestCase (ApplePlatform.iOS, "ios-arm64", CodeSignature.All, "Debug")]
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm", CodeSignature.All, "Debug")]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64", CodeSignature.Frameworks, "Debug")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64", CodeSignature.All, "Debug")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64;maccatalyst-arm64", CodeSignature.All, "Debug")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", CodeSignature.Frameworks, "Debug")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64;osx-arm64", CodeSignature.Frameworks, "Debug")]
		// [TestCase (ApplePlatform.TVOS, "tvos-arm64", CodeSignature.All, "Debug")]
		// Release
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm", CodeSignature.All, "Release")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64;maccatalyst-arm64", CodeSignature.All, "Release")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64", CodeSignature.Frameworks, "Release")]
		// [TestCase (ApplePlatform.TVOS, "tvos-arm64", CodeSignature.All, "Release")]
		public void Build (ApplePlatform platform, string runtimeIdentifiers, CodeSignature signature, string configuration)
		{
			var project = "BundleStructure";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

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
			var testsDirectory = Path.GetDirectoryName (Path.GetDirectoryName (project_dir))!;
			var expectedWarnings = new string [] {
				$"The 'PublishFolderType' metadata value 'Unknown' on the item '{Path.Combine (project_dir, platformString, "SomewhatUnknownI.bin")}' is not recognized. The file will not be copied to the app bundle. If the file is not supposed to be copied to the app bundle, remove the 'CopyToOutputDirectory' metadata on the item.",
				$"The 'PublishFolderType' metadata value 'Unknown' on the item '{Path.Combine (project_dir, platformString, "UnknownI.bin")}' is not recognized. The file will not be copied to the app bundle. If the file is not supposed to be copied to the app bundle, remove the 'CopyToOutputDirectory' metadata on the item.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneA.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneI.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneJ.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneK.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneM.unknown")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "Sub", "NoneG.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, "NoneH.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, "NoneO.xml")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
			}.ToList ();

			var rids = runtimeIdentifiers.Split (';');
			if (rids.Length > 1) {
				// All warnings show up twice if we're building for multiple architectures
				expectedWarnings.AddRange (expectedWarnings);
			}

			var zippedFrameworks = platform == ApplePlatform.MacCatalyst || platform == ApplePlatform.MacOSX;
			foreach (var rid in rids) {
				if (zippedFrameworks) {
					expectedWarnings.Add ($"The framework {Path.Combine ("obj", configuration, tfm, rid, "bindings-framework-test.resources.zip", "XStaticObjectTest.framework")} is a framework of static libraries, and will not be copied to the app.");
					expectedWarnings.Add ($"The framework {Path.Combine ("obj", configuration, tfm, rid, "bindings-framework-test.resources.zip", "XStaticArTest.framework")} is a framework of static libraries, and will not be copied to the app.");
				} else {
					expectedWarnings.Add ($"The framework {Path.Combine (testsDirectory, "bindings-framework-test", "dotnet", platformString, "bin", configuration, tfm, "bindings-framework-test.resources", "XStaticObjectTest.framework")} is a framework of static libraries, and will not be copied to the app.");
					expectedWarnings.Add ($"The framework {Path.Combine (testsDirectory, "bindings-framework-test", "dotnet", platformString, "bin", configuration, tfm, "bindings-framework-test.resources", "XStaticArTest.framework")} is a framework of static libraries, and will not be copied to the app.");
				}
			}

			if (signature == CodeSignature.None && (platform == ApplePlatform.MacCatalyst || platform == ApplePlatform.MacOSX)) {
				expectedWarnings.Add ($"Found files in the root directory of the app bundle. This will likely cause codesign to fail. Files:\n{Path.Combine ("bin", configuration, tfm, runtimeIdentifiers.IndexOf (';') >= 0 ? string.Empty : runtimeIdentifiers, "BundleStructure.app", "UnknownJ.bin")}");
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

		string [] FilterWarnings (IEnumerable<BuildLogEvent> warnings)
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

		static void AssertLibraryArchitectures (string appBundle, string [] runtimeIdentifiers)
		{
			var renderArchitectures = (IEnumerable<Abi> architectures) => {
				return string.Join (", ",
					architectures.
						// ARMv7s is kind of special in that we don't target it by default for ios-arm
						Where (v => v != Abi.ARMv7s).
						// Sort to get stable results
						OrderBy (v => v).
						// Render to a string to make it easy to understand what's going on in test failures
						Select (v => v.ToString ()));
			};
			var expectedArchitectures = renderArchitectures (
				runtimeIdentifiers.
					Select (rid => Configuration.GetArchitectures (rid)).
					SelectMany (v => v).
					Select (v => {
						if (v == "x86")
							return Abi.i386;
						return Enum.Parse<Abi> (v, true);
					})
			);
			var libraries = Directory.EnumerateFiles (appBundle, "*", SearchOption.AllDirectories)
				.Where (file => {
					// dylibs
					if (file.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase))
						return true;
					// frameworks
					if (Path.GetFileName (Path.GetDirectoryName (file)) == Path.GetFileName (file) + ".framework")
						return true;
					// nothing else
					return false;
				});
			foreach (var lib in libraries) {
				var libArchitectures = renderArchitectures (MachO.GetArchitectures (lib));
				Assert.AreEqual (expectedArchitectures, libArchitectures, $"Architectures in {lib}");
			}
		}
	}
}
