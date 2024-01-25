using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Xharness.TestImporter.Templates;

namespace Xharness.TestImporter.Xamarin {
	public class ProjectFilter : IProjectFilter {

		static readonly string splitPattern = ".part";

		// all could be private, but we allow them to be public since this file will not be shared aoutside
		// the xamarin-macios repo and is very specific to how we run the tests. Making them public makes testing
		// the filger easier and more robuts, we don't want to be looking at reflection in the tests
		public static readonly List<string> CommonIgnoredAssemblies = new List<string> {
			// we already have them in the templates, do not re-add them
			"mscorlib",
			"nunitlite",
			"System",
			"System.Xml",
			"System.Xml.Linq",
			"System.Core",
			"xunit.core",
			"xunit.abstractions",
			"xunit.assert",
			"Xunit.NetCore.Extensions",
			// specific to tests
			"monotouch_Commons.Xml.Relaxng_test.dll", // not supported by xamarin
			"monotouch_Cscompmgd_test.dll", // not supported by xamarin
			"monotouch_I18N.CJK_test.dll",
			"monotouch_I18N.MidEast_test.dll",
			"monotouch_I18N.Other_test.dll",
			"monotouch_I18N.Rare_test.dll",
			"monotouch_I18N.West_test.dll",
			"monotouch_Mono.C5_test.dll", // not supported by xamarin
			"monotouch_Mono.CodeContracts_test.dll", // not supported by xamarin
			"monotouch_Novell.Directory.Ldap_test.dll", // not supported by xamarin
			"monotouch_Mono.Profiler.Log_xunit-test.dll", // special tests that need an extra app to connect as a profiler
			"monotouch_System.ComponentModel.Composition_xunit-test.dll", // has no test classes, all test have been removed by mono
			"monotouch_System.Net.Http.FunctionalTests_xunit-test.dll", // has no test classes, all test have been removed by mono
			"monotouch_System.Runtime.Serialization_xunit-test.dll", // has no test classes, all test have been removed by mono
		};

		// list of assemblies that are going to be ignored, any project with an assemblies that is ignored will
		// be ignored

		public static readonly List<string> iOSIgnoredAssemblies = new List<string> { };

		public static readonly List<string> tvOSIgnoredAssemblies = new List<string> {
		};

		public static readonly List<string> watcOSIgnoredAssemblies = new List<string> {
			"monotouch_Mono.Security_test.dll",
			"monotouch_Mono.Data.Tds_test.dll", // not present in the watch tests dlls
		};

		public static readonly List<(string assembly, Platform [] platforms)> macIgnoredAssemblies = new List<(string assembly, Platform [] platforms)> {
			(assembly: "xammac_net_4_5_I18N.CJK_test.dll", platforms: new [] { Platform.MacOSFull, Platform.MacOSModern }),
			(assembly: "xammac_net_4_5_I18N.MidEast_test.dll", platforms: new [] { Platform.MacOSFull, Platform.MacOSModern }),
			(assembly: "xammac_net_4_5_I18N.Other_test.dll", platforms: new [] { Platform.MacOSFull, Platform.MacOSModern }),
			(assembly: "xammac_net_4_5_I18N.Rare_test.dll", platforms: new [] { Platform.MacOSFull, Platform.MacOSModern }),
			(assembly: "xammac_net_4_5_I18N.West_test.dll", platforms: new [] { Platform.MacOSFull, Platform.MacOSModern }),
			(assembly: "xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll", platforms: new [] { Platform.MacOSModern}), // not present 
			(assembly: "xammac_net_4_5_System.Net.Http.WebRequest_test.dll", platforms: new [] { Platform.MacOSModern}), // not present 
			(assembly: "xammac_net_4_5_System.Messaging_test.dll", platforms: new [] { Platform.MacOSModern}), // not present 
			(assembly: "xammac_net_4_5_System.IdentityModel_test.dll", platforms: new [] { Platform.MacOSModern}), // not present 
			(assembly: "xammac_net_4_5_System.Data.Linq_test.dll", platforms: new [] { Platform.MacOSModern}), // not present 
			(assembly: "xammac_net_4_5_Mono.Posix_test.dll", platforms: new [] { Platform.MacOSModern}), // not present 
			(assembly: "xammac_net_4_5_Mono.Messaging_test.dll", platforms: new [] { Platform.MacOSModern}), // not present 
			(assembly: "xammac_net_4_5_System.Data_test.dll", platforms: new [] { Platform.MacOSModern }), // tests use 'System.Configuration.IConfigurationSectionHandler' not present in modern 
			(assembly: "xammac_net_4_5_System.Configuration_test.dll", platforms: new [] { Platform.MacOSModern }), // Not present in modern, ergo all tests will fail
		};

		public string IgnoreFilesRootDir { get; set; }
		public IAssemblyLocator AssemblyLocator { get; set; }

		public bool ExcludeDll (Platform platform, string assembly)
		{
			if (CommonIgnoredAssemblies.Contains (assembly))
				return true;
			switch (platform) {
			case Platform.iOS:
				return iOSIgnoredAssemblies.Contains (assembly);
			case Platform.TvOS:
				return tvOSIgnoredAssemblies.Contains (assembly);
			case Platform.WatchOS:
				return watcOSIgnoredAssemblies.Contains (assembly);
			case Platform.MacOSFull:
				return macIgnoredAssemblies.Any ((ignored) => (ignored.assembly == assembly) && ignored.platforms.Contains (Platform.MacOSFull));
			case Platform.MacOSModern:
				return macIgnoredAssemblies.Any ((ignored) => (ignored.assembly == assembly) && ignored.platforms.Contains (Platform.MacOSModern));
			}
			return false;
		}

		public bool ExludeProject (ProjectDefinition project, Platform platform)
		{
			foreach (var a in project.TestAssemblies) {
				if (CommonIgnoredAssemblies.Contains (a.Name))
					return true;
				switch (platform) {
				case Platform.iOS:
					return iOSIgnoredAssemblies.Contains (a.Name);
				case Platform.TvOS:
					return tvOSIgnoredAssemblies.Contains (a.Name);
				case Platform.WatchOS:
					return watcOSIgnoredAssemblies.Contains (a.Name);
				case Platform.MacOSFull:
					return macIgnoredAssemblies.Any ((ignored) => (ignored.assembly == a.Name) && ignored.platforms.Contains (Platform.MacOSFull));
				case Platform.MacOSModern:
					return macIgnoredAssemblies.Any ((ignored) => (ignored.assembly == a.Name) && ignored.platforms.Contains (Platform.MacOSModern));
				}
			}
			return false;
		}

		internal static string GetCommonIgnoreFileName (string name, Platform platform)
		{
			switch (platform) {
			case Platform.TvOS:
				return $"common-{name.Replace ("monotouch_tv_", "monotouch_")}.ignore";
			case Platform.WatchOS:
				return $"common-{name.Replace ("monotouch_watch_", "monotouch_")}.ignore";
			default:
				return $"common-{name}.ignore";
			}
		}

		internal static string [] GetIgnoreFileNames (string name, Platform platform)
		{
			switch (platform) {
			case Platform.iOS:
				return new string [] { $"iOS-{name}.ignore" };
			case Platform.MacOSFull:
				return new string [] { $"macOSFull-{name}.ignore", $"macOS-{name}.ignore" };
			case Platform.MacOSModern:
				return new string [] { $"macOSModern-{name.Replace ("xammac_", "xammac_net_4_5_")}.ignore", $"macOS-{name.Replace ("xammac_", "xammac_net_4_5_")}.ignore" };
			case Platform.TvOS:
				return new string [] { $"tvOS-{name.Replace ("monotouch_tv_", "monotouch_")}.ignore" };
			case Platform.WatchOS:
				return new string [] { $"watchOS-{name.Replace ("monotouch_watch_", "monotouch_")}.ignore" };
			default:
				return null;
			}
		}

		public IEnumerable<string> GetIgnoreFiles (string projectName, List<(string assembly, string hintPath)> assemblies, Platform platform)
		{
			// check if the common and plaform paths can be found in the template path, if they are, we return them
			var commonIgnore = Path.Combine (IgnoreFilesRootDir, GetCommonIgnoreFileName (projectName, platform));
			if (File.Exists (commonIgnore))
				yield return commonIgnore;
			foreach (var platformFile in GetIgnoreFileNames (projectName, platform)) {
				var platformIgnore = Path.Combine (IgnoreFilesRootDir, platformFile);
				if (File.Exists (platformIgnore))
					yield return platformIgnore;
			}
			// do we have ignores per files and not the project name? Add them
			foreach (var (assembly, hintPath) in assemblies) {
				// we could be looking at a splitted assembly, if that is the case, lets pass the name of the dll without the 'part{number}.dll
				// so that we have all the ignores in a single file
				var assemblyName = assembly;
				var index = assembly.IndexOf (splitPattern, StringComparison.Ordinal);

				if (index != -1)
					assemblyName = assembly.Substring (0, index) + ".dll";
				foreach (var platformFile in GetIgnoreFileNames (assemblyName, platform)) {
					var commonAssemblyIgnore = Path.Combine (IgnoreFilesRootDir, GetCommonIgnoreFileName (assemblyName, platform));
					if (File.Exists (commonAssemblyIgnore))
						yield return commonAssemblyIgnore;
					var platformAssemblyIgnore = Path.Combine (IgnoreFilesRootDir, platformFile);
					if (File.Exists (platformAssemblyIgnore))
						yield return platformAssemblyIgnore;
				}
			}
		}

		public IEnumerable<string> GetTraitsFiles (Platform platform)
		{
			var rootPath = AssemblyLocator.GetAssembliesRootLocation (platform);
			switch (platform) {
			case Platform.iOS:
				rootPath = Path.Combine (rootPath, "ios-bcl", "monotouch", "tests");
				break;
			case Platform.TvOS:
				rootPath = Path.Combine (rootPath, "ios-bcl", "monotouch_tv", "tests");
				break;
			case Platform.WatchOS:
				rootPath = Path.Combine (rootPath, "ios-bcl", "monotouch_watch", "tests");
				break;
			case Platform.MacOSFull:
				rootPath = Path.Combine (rootPath, "mac-bcl", "xammac_net_4_5", "tests");
				break;
			case Platform.MacOSModern:
				rootPath = Path.Combine (rootPath, "mac-bcl", "xammac", "tests");
				break;
			}
			return new [] {
 				Path.Combine (rootPath, "nunit-excludes.txt"),
				Path.Combine (rootPath, "xunit-excludes.txt")
			};
		}
	}
}
