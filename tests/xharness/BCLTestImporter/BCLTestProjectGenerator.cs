using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xharness.BCLTestImporter.Templates;
using Xharness.BCLTestImporter.Templates.Managed;
using Xharness.BCLTestImporter.Xamarin;

namespace Xharness.BCLTestImporter {
	/// <summary>
	/// Class that knows how to generate .csproj files based on a BCLTestProjectDefinition.
	/// </summary>
	public class BCLTestProjectGenerator {

		static string NUnitPattern = "monotouch_*_test.dll"; 
		static string xUnitPattern = "monotouch_*_xunit-test.dll";
		
		// we have two different types of list, those that are for the iOS like projects (ios, tvos and watch os) and those 
		// for mac
		static readonly List<BclTestProjectInfo> commoniOSTestProjects = new List<BclTestProjectInfo> {
			// NUNIT TESTS

			// BCL tests group 1
			new BclTestProjectInfo { Name = "SystemCoreTests", assemblies = new [] { "monotouch_System.Core_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemNumericsTests", assemblies = new [] { "monotouch_System.Numerics_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemRuntimeSerializationTests", assemblies = new [] { "monotouch_System.Runtime.Serialization_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemTransactionsTests", assemblies = new [] { "monotouch_System.Transactions_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemXmlTests", assemblies = new [] { "monotouch_System.Xml_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemXmlLinqTests", assemblies = new [] { "monotouch_System.Xml.Linq_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "MonoSecurityTests", assemblies = new [] { "monotouch_Mono.Security_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemComponentModelDataAnnotationsTests", assemblies = new [] { "monotouch_System.ComponentModel.DataAnnotations_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemJsonTests", assemblies = new [] { "monotouch_System.Json_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemServiceModelWebTests", assemblies = new [] { "monotouch_System.ServiceModel.Web_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemIOCompressionTests", assemblies = new [] { "monotouch_System.IO.Compression_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemIOCompressionFileSystemTests", assemblies = new [] { "monotouch_System.IO.Compression.FileSystem_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "MonoCSharpTests", assemblies = new [] { "monotouch_Mono.CSharp_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemSecurityTests", assemblies = new [] { "monotouch_System.Security_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "MonoDataSqliteTests", assemblies = new [] { "monotouch_Mono.Data.Sqlite_test.dll" }, Group = "BCL tests group 1" },
			new BclTestProjectInfo { Name = "MonoRuntimeTests", assemblies = new [] { "monotouch_Mono.Runtime.Tests_test.dll" }, Group = "BCL tests group 1" },

			// BCL tests group 2
			new BclTestProjectInfo { Name = "SystemTests", assemblies = new[] {"monotouch_System_test.dll" }, Group = "BCL tests group 2" },
			new BclTestProjectInfo { Name = "SystemDataTests", assemblies = new [] { "monotouch_System.Data_test.dll" }, Group = "BCL tests group 2" },
			new BclTestProjectInfo { Name = "SystemDataDataSetExtensionsTests", assemblies = new [] { "monotouch_System.Data.DataSetExtensions_test.dll" }, Group = "BCL tests group 2" },
			new BclTestProjectInfo { Name = "SystemNetHttpTests", assemblies = new [] { "monotouch_System.Net.Http_test.dll" }, Group = "BCL tests group 2" },
			new BclTestProjectInfo { Name = "MonoDataTdsTests", assemblies = new [] { "monotouch_Mono.Data.Tds_test.dll" }, Group = "BCL tests group 2" },
			new BclTestProjectInfo { Name = "SystemServiceModelTests", assemblies = new [] { "monotouch_System.ServiceModel_test.dll" }, Group = "BCL tests group 2" },
			new BclTestProjectInfo { Name = "CorlibTests", assemblies = new [] { "monotouch_corlib_test.dll" }, Group = "BCL tests group 2", TimeoutMultiplier = 2 },
			new BclTestProjectInfo { Name = "SystemWebServicesTests", assemblies = new [] { "monotouch_System.Web.Services_test.dll" }, Group = "BCL tests group 2" },

			// XUNIT TESTS 

			// BCL tests group 3
			new BclTestProjectInfo { Name = "SystemDataXunit", assemblies = new [] { "monotouch_System.Data_xunit-test.dll" }, Group = "BCL tests group 3" },
			new BclTestProjectInfo { Name = "SystemJsonXunit", assemblies = new [] { "monotouch_System.Json_xunit-test.dll" }, Group = "BCL tests group 3" },
			new BclTestProjectInfo { Name = "SystemSecurityXunit", assemblies = new [] { "monotouch_System.Security_xunit-test.dll" }, Group = "BCL tests group 3" },
			new BclTestProjectInfo { Name = "SystemLinqXunit", assemblies = new [] { "monotouch_System.Xml.Linq_xunit-test.dll" }, Group = "BCL tests group 3" },
			new BclTestProjectInfo { Name = "SystemComponentModelCompositionXunit", assemblies = new [] { "monotouch_System.ComponentModel.Composition_xunit-test.dll" }, Group = "BCL tests group 3" },
			new BclTestProjectInfo { Name = "SystemRuntimeSerializationXunit", assemblies = new [] { "monotouch_System.Runtime.Serialization_xunit-test.dll" }, Group = "BCL tests group 3" },
			new BclTestProjectInfo { Name = "SystemXmlXunit", assemblies = new [] { "monotouch_System.Xml_xunit-test.dll" }, Group = "BCL tests group 3" },
			new BclTestProjectInfo { Name = "SystemRuntimeCompilerServicesUnsafeXunit", assemblies = new [] { "monotouch_System.Runtime.CompilerServices.Unsafe_xunit-test.dll" }, Group = "BCL tests group 3" },

			// BCL tests group 4
			new BclTestProjectInfo { Name = "SystemNumericsXunit", assemblies = new [] { "monotouch_System.Numerics_xunit-test.dll" }, Group = "BCL tests group 4" },
			new BclTestProjectInfo { Name = "MicrosoftCSharpXunit", assemblies = new [] { "monotouch_Microsoft.CSharp_xunit-test.dll" }, Group = "BCL tests group 4" },

			// BCL tests group 5
			new BclTestProjectInfo { Name = "SystemNetHttpUnitTestsXunit", assemblies = new [] { "monotouch_System.Net.Http.UnitTests_xunit-test.dll" }, Group = "BCL tests group 5" },
			new BclTestProjectInfo { Name = "SystemNetHttpFunctionalTestsXunit", assemblies = new [] { "monotouch_System.Net.Http.FunctionalTests_xunit-test.dll" }, Group = "BCL tests group 5" },

			// Special assemblies that are in a single application due to their size being to large for the iOS 32b.
			new BclTestProjectInfo { Name = "mscorlib Part 1", assemblies = new [] { "monotouch_corlib_xunit-test.part1.dll" }, Group = "mscorlib Part 1" }, // special testcase for the corlib which is later used in xHarness for diff config options
			new BclTestProjectInfo { Name = "mscorlib Part 2", assemblies = new [] { "monotouch_corlib_xunit-test.part2.dll" }, Group = "mscorlib Part 2" },
			new BclTestProjectInfo { Name = "mscorlib Part 3", assemblies = new [] { "monotouch_corlib_xunit-test.part3.dll" }, Group = "mscorlib Part 3" },
			new BclTestProjectInfo { Name = "SystemCoreXunit Part 1", assemblies = new [] { "monotouch_System.Core_xunit-test.part1.dll" }, Group = "SystemCoreXunit Part 1" },
			new BclTestProjectInfo { Name = "SystemCoreXunit Part 2", assemblies = new [] { "monotouch_System.Core_xunit-test.part2.dll" }, Group = "SystemCoreXunit Part 2" },
			new BclTestProjectInfo { Name = "SystemXunit", assemblies = new [] { "monotouch_System_xunit-test.dll" }, ExtraArgs = $"--xml={Path.Combine (Harness.RootDirectory, "bcl-test", "SystemXunitLinker.xml")} --optimize=-custom-attributes-removal", Group = "SystemXunit" }, // special case due to the need of the extra args
		};

		private static readonly List<BclTestProjectInfo> macTestProjects = new List<BclTestProjectInfo> {
		
			// NUNIT Projects
			new BclTestProjectInfo { Name = "MonoDataSqliteTests", assemblies = new [] { "xammac_net_4_5_Mono.Data.Sqlite_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "MonoDataTdsTests", assemblies = new [] { "xammac_net_4_5_Mono.Data.Tds_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "MonoMessagingTests", assemblies = new [] { "xammac_net_4_5_Mono.Messaging_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "MonoPoxisTests", assemblies = new [] { "xammac_net_4_5_Mono.Posix_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "MonoSecurityTests", assemblies = new [] { "xammac_net_4_5_Mono.Security_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemConfigurationTests", assemblies = new [] { "xammac_net_4_5_System.Configuration_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemDataLinqTests", assemblies = new [] { "xammac_net_4_5_System.Data.Linq_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemDataTests", assemblies = new [] { "xammac_net_4_5_System.Data_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemIOCompressionFileSystemTests", assemblies = new [] { "xammac_net_4_5_System.IO.Compression.FileSystem_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemIOCompressionTests", assemblies = new [] { "xammac_net_4_5_System.IO.Compression_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemIdentityModelTests", assemblies = new [] { "xammac_net_4_5_System.IdentityModel_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemJsonTests", assemblies = new [] { "xammac_net_4_5_System.Json_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemMessagingTests", assemblies = new [] { "xammac_net_4_5_System.Messaging_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemNetHttpWebRequestTests", assemblies = new [] { "xammac_net_4_5_System.Net.Http.WebRequest_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemNumericsTests", assemblies = new [] { "xammac_net_4_5_System.Numerics_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemRuntimeSerializationFormattersSoapTests", assemblies = new [] { "xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemRuntimeSerializationTests", assemblies = new [] { "xammac_net_4_5_System.Runtime.Serialization_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemServiceModelWebTest", assemblies = new [] { "xammac_net_4_5_System.ServiceModel.Web_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemServiceModelTests", assemblies = new [] { "xammac_net_4_5_System.ServiceModel_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemTransactionsTests", assemblies = new [] { "xammac_net_4_5_System.Transactions_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemWebServicesTests", assemblies = new [] { "xammac_net_4_5_System.Web.Services_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemXmlLinqTests", assemblies = new [] { "xammac_net_4_5_System.Xml.Linq_test.dll" }, Group = "Mac OS X BCL tests group 1" },
			new BclTestProjectInfo { Name = "SystemDataDataSetExtensionsTests", assemblies = new [] { "xammac_net_4_5_System.Data.DataSetExtensions_test.dll" }, Group = "Mac OS X BCL tests group 1" },

			new BclTestProjectInfo { Name = "SystemNetHttpTests", assemblies = new [] { "xammac_net_4_5_System.Net.Http_test.dll" }, Group = "Mac OS X BCL tests group 2" }, // do not mix with SystemNetHttpWebRequestTests since there is a type collision
			new BclTestProjectInfo { Name = "SystemComponentModelDataAnnotationsTests", assemblies = new [] { "xammac_net_4_5_System.ComponentModel.DataAnnotations_test.dll" }, Group = "Mac OS X BCL tests group 2" },
			new BclTestProjectInfo { Name = "SystemCoreTests", assemblies = new [] { "xammac_net_4_5_System.Core_test.dll" }, Group = "Mac OS X BCL tests group 2" },
			new BclTestProjectInfo { Name = "SystemSecurityTests", assemblies = new [] { "xammac_net_4_5_System.Security_test.dll" }, Group = "Mac OS X BCL tests group 2" },
			new BclTestProjectInfo { Name = "SystemXmlTests", assemblies = new [] { "xammac_net_4_5_System.Xml_test.dll" }, Group = "Mac OS X BCL tests group 2" },
			new BclTestProjectInfo { Name = "SystemTests", assemblies = new [] { "xammac_net_4_5_System_test.dll" }, Group = "Mac OS X BCL tests group 2" },
			new BclTestProjectInfo { Name = "MonoCSharpTests", assemblies = new [] { "xammac_net_4_5_Mono.CSharp_test.dll" }, Group = "Mac OS X BCL tests group 2" }, // if add to the first group, it blocks 'til a timeout, mono issue
			
			new BclTestProjectInfo { Name = "CorlibTests", assemblies = new [] { "xammac_net_4_5_corlib_test.dll" }, Group = "Mac OS X BCL tests group 3" },
			
			// xUnit Projects
			new BclTestProjectInfo { Name = "MicrosoftCSharpXunit", assemblies = new [] { "xammac_net_4_5_Microsoft.CSharp_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemComponentModelCompositionXunit", assemblies = new [] { "xammac_net_4_5_System.ComponentModel.Composition_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemCoreXunit", assemblies = new [] { "xammac_net_4_5_System.Core_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemDataXunit", assemblies = new [] { "xammac_net_4_5_System.Data_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemJsonXunit", assemblies = new [] { "xammac_net_4_5_System.Json_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemNetHttpFunctionalTestsXunit", assemblies = new [] { "xammac_net_4_5_System.Net.Http.FunctionalTests_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemNetHttpUnitTestsXunit", assemblies = new [] { "xammac_net_4_5_System.Net.Http.UnitTests_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemNumericsXunit", assemblies = new [] { "xammac_net_4_5_System.Numerics_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemRuntimeCompilerServicesUnsafeXunit", assemblies = new [] { "xammac_net_4_5_System.Runtime.CompilerServices.Unsafe_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemSecurityXunit", assemblies = new [] { "xammac_net_4_5_System.Security_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemXmlLinqXunit", assemblies = new [] { "xammac_net_4_5_System.Xml.Linq_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemXmlXunit", assemblies = new [] { "xammac_net_4_5_System.Xml_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemXunit", assemblies = new [] { "xammac_net_4_5_System_xunit-test.dll" }, Group = "Mac OS X BCL tests group 4" },
			
			new BclTestProjectInfo { Name = "mscorlib", assemblies = new [] { "xammac_net_4_5_corlib_xunit-test.dll" }, Group = "mscorlib" },// special testcase for the corlib which is later used in xHarness for diff config options

		};

		public bool GroupTests { get; set; }
		public string OutputDirectoryPath { get; private  set; }
		public string MonoRootPath { get; private set; }

		public Func<string, Guid> GuidGenerator {
			get => TemplatedProject.GuidGenerator;
			set => TemplatedProject.GuidGenerator = value;
		}

		public string iOSMonoSDKPath {
			get {
				var locator = AssemblyLocator as AssemblyLocator;
				return locator?.iOSMonoSDKPath;
			}
			set {
				var locator = AssemblyLocator as AssemblyLocator;
				if (locator != null)
					locator.iOSMonoSDKPath = value;
			}
		}

		public string MacMonoSDKPath {
			get {
				var locator = AssemblyLocator as AssemblyLocator;
				return locator?.MacMonoSDKPath;
			}
			set {
				var locator = TemplatedProject.AssemblyLocator as AssemblyLocator;
				if (locator != null)
					locator.MacMonoSDKPath = value;
			}
		}

		public IAssemblyLocator AssemblyLocator { get; set; }
		public IProjectFilter ProjectFilter { get; set; }
		public ITemplatedProject TemplatedProject { get; set; }

		public BCLTestProjectGenerator (string outputDirectory)
		{
			OutputDirectoryPath = outputDirectory ?? throw new ArgumentNullException (nameof (outputDirectory));
			AssemblyLocator = new AssemblyLocator ();
			ProjectFilter = new ProjectFilter { AssemblyLocator = AssemblyLocator, IgnoreFilesRootDir = outputDirectory };
			TemplatedProject = new XamariniOSTemplate {
				AssemblyLocator = AssemblyLocator,
				OutputDirectoryPath = outputDirectory,
				IgnoreFilesRootDirectory = outputDirectory,
				ProjectFilter = ProjectFilter,
				//public Func<string, Guid> GuidGenerator { get; set; }
			};
		}
		
		public BCLTestProjectGenerator (string outputDirectory, string monoRootPath) : this (outputDirectory)
		{
			MonoRootPath = monoRootPath ?? throw new ArgumentNullException (nameof (monoRootPath));
		}

		/// <summary>
		/// Generates all the project files for the given projects and platform
		/// </summary>
		/// <param name="projects">The list of projects to be generated.</param>
		/// <param name="platform">The platform to which the projects have to be generated. Each platform
		/// has its own details.</param>
		/// <param name="generatedDir">The dir where the projects will be saved.</param>
		/// <returns></returns>
		public Task<List<BclTestProject>> GenerateTestProjectsAsync (IEnumerable<BclTestProjectInfo> projects, Platform platform)
			=> TemplatedProject.GenerateTestProjectsAsync (projects, platform);
		
		List<BclTestProjectInfo> GetProjectDefinitions (List<BclTestProjectInfo> definitions, Platform platform)
		{
			var testProjects = new List<BclTestProjectInfo> ();
			if (GroupTests && platform != Platform.WatchOS) {
				// build the grouped apps 
				var groupedApps = new Dictionary<string, List<string>> ();
				var groupedAppsExtraArgs = new Dictionary<string, List<string>> ();
				double timeoutMultiplier = 1;
				foreach (var def in definitions) {
					var validAssemblies = new List<string> ();
					foreach (var a in def.assemblies) { // filter ignored assemblies
						if (ProjectFilter == null || !ProjectFilter.ExcludeDll (platform, a))
							validAssemblies.Add (a);
					}
					if (groupedApps.ContainsKey (def.Group)) {
						groupedApps [def.Group].AddRange (validAssemblies);
						if (def.ExtraArgs != null)
							groupedAppsExtraArgs [def.Group].Add (def.ExtraArgs);
					} else {
						groupedApps [def.Group] = new List<string> (validAssemblies);
						groupedAppsExtraArgs [def.Group] = new List<string> ();
						if (def.ExtraArgs != null)
							groupedAppsExtraArgs [def.Group].Add (def.ExtraArgs);
					}
					timeoutMultiplier += (def.TimeoutMultiplier - 1);
				}
				foreach (var group in groupedApps.Keys) {
					var cleanedExtraArgs = groupedAppsExtraArgs [group].Distinct ();
					testProjects.Add (new BclTestProjectInfo { Name = group, assemblies = groupedApps [group].ToArray (), ExtraArgs = string.Join (" ", cleanedExtraArgs), TimeoutMultiplier = timeoutMultiplier });
				}
			} else {
				foreach (var def in definitions) {
					testProjects.Add (new BclTestProjectInfo { Name = def.Name, assemblies = def.assemblies, ExtraArgs = def.ExtraArgs, TimeoutMultiplier = def.TimeoutMultiplier });
				}
			}
			return testProjects;
		}

		async Task<List<iOSBclTestProject>> GenerateAllCommonTestProjectsAsync ()
		{
			var projectPaths = new List<iOSBclTestProject> ();
			var projects = new Dictionary<string, iOSBclTestProject> ();
			foreach (var platform in new [] { Platform.iOS, Platform.TvOS, Platform.WatchOS }) {
				var generated = await GenerateTestProjectsAsync (GetProjectDefinitions (commoniOSTestProjects, platform), platform);
				foreach (var tp in generated) {
					if (!projects.ContainsKey (tp.Name)) {
						projects [tp.Name] = new iOSBclTestProject { Path = tp.Path, XUnit = tp.XUnit, ExtraArgs = tp.ExtraArgs, Platforms = new List<Platform> { platform }, Failure = tp.Failure, TimeoutMultiplier = tp.TimeoutMultiplier };
					} else {
						projects [tp.Name].Platforms.Add (platform);
						projects [tp.Name].TimeoutMultiplier += (tp.TimeoutMultiplier - 1);
					}
				}
			} // foreach platform

			// return the grouped projects
			foreach (var name in projects.Keys) {
				projectPaths.Add (new iOSBclTestProject { Name = name, Path = projects [name].Path, XUnit = projects [name].XUnit, ExtraArgs = projects [name].ExtraArgs, Platforms = projects [name].Platforms, Failure = projects [name].Failure, TimeoutMultiplier = projects [name].TimeoutMultiplier });
			}
			return projectPaths;
		}

		public Task<List<iOSBclTestProject>> GenerateAlliOSTestProjectsAsync () => GenerateAllCommonTestProjectsAsync ();

		public List<iOSBclTestProject> GenerateAlliOSTestProjects () => GenerateAlliOSTestProjectsAsync ().Result;

		public Task<List<BclTestProject>> GenerateAllMacTestProjectsAsync (Platform platform) => GenerateTestProjectsAsync (GetProjectDefinitions (macTestProjects, platform), platform);

		public List<BclTestProject> GenerateAllMacTestProjects (Platform platform) => GenerateAllMacTestProjectsAsync (platform).Result;
	}
}
