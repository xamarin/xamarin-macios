using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace BCLTestImporter {
	/// <summary>
	/// Class that knows how to generate .csproj files based on a BCLTestProjectDefinition.
	/// </summary>
	public class BCLTestProjectGenerator {

		static string NUnitPattern = "MONOTOUCH_*_test.dll"; 
		static string xUnitPattern = "MONOTOUCH_*_xunit-test.dll";
		internal static readonly string NameKey = "%NAME%";
		internal static readonly string ReferencesKey = "%REFERENCES%";
		internal static readonly string RegisterTypeKey = "%REGISTER TYPE%";
		internal static readonly string PlistKey = "%PLIST PATH%";
		internal static readonly string WatchOSTemplatePathKey = "%TEMPLATE PATH%";
		internal static readonly string WatchOSCsporjAppKey = "%WATCH APP PROJECT PATH%";
		internal static readonly string WatchOSCsporjExtensionKey  ="%WATCH EXTENSION PROJECT PATH%";
		internal static readonly string TargetFrameworkVersionKey = "%TARGET FRAMEWORK VERSION%";
		internal static readonly string TargetExtraInfoKey = "%TARGET EXTRA INFO%";
		internal static readonly string DefineConstantsKey = "%DEFINE CONSTANTS%";
		static readonly Dictionary<Platform, string> plistTemplateMatches = new Dictionary<Platform, string> {
			{Platform.iOS, "Info.plist.in"},
			{Platform.TvOS, "Info-tv.plist.in"},
			{Platform.WatchOS, "Info-watchos.plist.in"},
			{Platform.MacOSFull, "Info-mac.plist.in"},
			{Platform.MacOSModern, "Info-mac.plist.in"},
		};
		static readonly Dictionary<Platform, string> projectTemplateMatches = new Dictionary<Platform, string> {
			{Platform.iOS, "BCLTests.csproj.in"},
			{Platform.TvOS, "BCLTests-tv.csproj.in"},
			{Platform.WatchOS, "BCLTests-watchos.csproj.in"},
			{Platform.MacOSFull, "BCLTests-mac.csproj.in"},
			{Platform.MacOSModern, "BCLTests-mac.csproj.in"},
		};
		static readonly Dictionary<WatchAppType, string> watchOSProjectTemplateMatches = new Dictionary<WatchAppType, string>
		{
			{ WatchAppType.App, "BCLTests-watchos-app.csproj.in"},
			{ WatchAppType.Extension, "BCLTests-watchos-extension.csproj.in"}
		};

		public enum WatchAppType {
			App,
			Extension
		}

		static readonly Dictionary<WatchAppType, string> watchOSPlistTemplateMatches = new Dictionary<WatchAppType, string> {
			{WatchAppType.App, "Info-watchos-app.plist.in"},
			{WatchAppType.Extension, "Info-watchos-extension.plist.in"}
		};

		//list of reference that we are already adding, and we do not want to readd (although it is just a warning)
		static readonly List<string> excludeDlls = new List<string> {
			"mscorlib",
			"nunitlite",
			"System",
			"System.Xml",
			"System.Xml.Linq",
			"System.Core",
			"xunit.core",
			"xunit.abstractions",
			"xunit.assert",
		};

		// we have two different types of list, those that are for the iOS like projects (ios, tvos and watch os) and those 
		// for mac
		static readonly List<(string name, string[] assemblies)> commoniOSTestProjects = new List<(string name, string[] assemblies)> {
			// NUNIT TESTS

			(name:"SystemTests", assemblies: new[] {"MONOTOUCH_System_test.dll"}),
			(name:"SystemCoreTests", assemblies: new [] {"MONOTOUCH_System.Core_test.dll"}),
			(name:"SystemDataTests", assemblies: new [] {"MONOTOUCH_System.Data_test.dll"}),
			(name:"SystemNetHttpTests", assemblies: new [] {"MONOTOUCH_System.Net.Http_test.dll"}),
			(name:"SystemNumericsTests", assemblies: new [] {"MONOTOUCH_System.Numerics_test.dll"}),
			(name:"SystemRuntimeSerializationTests", assemblies: new [] {"MONOTOUCH_System.Runtime.Serialization_test.dll"}),
			(name:"SystemTransactionsTests", assemblies: new [] {"MONOTOUCH_System.Transactions_test.dll"}),
			(name:"SystemXmlTests", assemblies: new [] {"MONOTOUCH_System.Xml_test.dll"}),
			(name:"SystemXmlLinqTests", assemblies: new [] {"MONOTOUCH_System.Xml.Linq_test.dll"}),
			(name:"MonoSecurityTests", assemblies: new [] {"MONOTOUCH_Mono.Security_test.dll"}),
			(name:"SystemComponentModelDataAnnotationTests", assemblies: new [] {"MONOTOUCH_System.ComponentModel.DataAnnotations_test.dll"}),
			(name:"SystemJsonTests", assemblies: new [] {"MONOTOUCH_System.Json_test.dll"}),
			(name:"SystemServiceModelWebTests", assemblies: new [] {"MONOTOUCH_System.ServiceModel.Web_test.dll"}),
			(name:"MonoDataTdsTests", assemblies: new [] {"MONOTOUCH_Mono.Data.Tds_test.dll"}),
			(name:"SystemIOCompressionTests", assemblies: new [] {"MONOTOUCH_System.IO.Compression_test.dll"}),
			(name:"SystemIOCompressionFileSystemTests", assemblies: new [] {"MONOTOUCH_System.IO.Compression.FileSystem_test.dll"}),
			(name:"MonoCSharpTests", assemblies: new [] {"MONOTOUCH_Mono.CSharp_test.dll"}),
			(name:"SystemSecurityTests", assemblies: new [] {"MONOTOUCH_System.Security_test.dll"}),
			(name:"SystemServiceModelTests", assemblies: new [] {"MONOTOUCH_System.ServiceModel_test.dll"}),
			(name:"SystemJsonMicrosoftTests", assemblies: new [] {"MONOTOUCH_System.Json.Microsoft_test.dll"}),
			(name:"SystemDataDataSetExtensionTests", assemblies: new [] {"MONOTOUCH_System.Data.DataSetExtensions_test.dll"}),
			(name:"SystemRuntimeSerializationFormattersSoapTests", assemblies: new [] {"MONOTOUCH_System.Runtime.Serialization.Formatters.Soap_test.dll"}),
			(name:"CorlibTests", assemblies: new [] {"MONOTOUCH_corlib_test.dll"}),
			(name:"MonoParallelTests", assemblies: new [] {"MONOTOUCH_Mono.Parallel_test.dll"}),
			(name:"MonoRuntimeTests", assemblies: new [] {"MONOTOUCH_Mono.Runtime.Tests_test.dll"}),
			(name:"MonoTaskletsTests", assemblies: new [] {"MONOTOUCH_Mono.Tasklets_test.dll"}),
			(name:"SystemThreadingTasksDataflowTests", assemblies: new [] {"MONOTOUCH_System.Threading.Tasks.Dataflow_test.dll"}),

			// XUNIT TESTS 

			(name:"SystemDataXunit", assemblies: new [] {"MONOTOUCH_System.Data_xunit-test.dll"}),
			(name:"SystemJsonXunit", assemblies: new [] {"MONOTOUCH_System.Json_xunit-test.dll"}),
			(name:"SystemNumericsXunit", assemblies: new [] {"MONOTOUCH_System.Numerics_xunit-test.dll"}),
			(name:"SystemSecurityXunit", assemblies: new [] {"MONOTOUCH_System.Security_xunit-test.dll"}),
			(name:"SystemThreadingTaskXunit", assemblies: new [] {"MONOTOUCH_System.Threading.Tasks.Dataflow_xunit-test.dll"}),
			(name:"SystemLinqXunit", assemblies: new [] {"MONOTOUCH_System.Xml.Linq_xunit-test.dll"}),
			(name:"SystemRuntimeCompilerServicesUnsafeXunit", assemblies: new [] {"MONOTOUCH_System.Runtime.CompilerServices.Unsafe_xunit-test.dll"}),
		};
			
		static readonly List <string> CommonIgnoredAssemblies = new List <string> {
			"MONOTOUCH_System.Data_xunit-test.dll", // issue https://github.com/xamarin/maccore/issues/1131
			"MONOTOUCH_System.Security_xunit-test.dll",// issue https://github.com/xamarin/maccore/issues/1128
			"MONOTOUCH_System.Threading.Tasks.Dataflow_xunit-test.dll", // issue https://github.com/xamarin/maccore/issues/1132
			"MONOTOUCH_System.Xml_test.dll", // issue https://github.com/xamarin/maccore/issues/1133
			"MONOTOUCH_System.Transactions_test.dll", // issue https://github.com/xamarin/maccore/issues/1134
			"MONOTOUCH_System_test.dll", // issues https://github.com/xamarin/maccore/issues/1135
			"MONOTOUCH_System.ServiceModel.Web_test.dll", // issue https://github.com/xamarin/maccore/issues/1137
			"MONOTOUCH_System.ServiceModel_test.dll", // issue https://github.com/xamarin/maccore/issues/1138
			"MONOTOUCH_System.Security_test.dll", // issue https://github.com/xamarin/maccore/issues/1139
			"MONOTOUCH_System.Runtime.Serialization.Formatters.Soap_test.dll", // issue https://github.com/xamarin/maccore/issues/1140
			"MONOTOUCH_System.Net.Http_test.dll", // issue https://github.com/xamarin/maccore/issues/1144 and https://github.com/xamarin/maccore/issues/1145
			"MONOTOUCH_System.IO.Compression_test.dll", // issue https://github.com/xamarin/maccore/issues/1146
			"MONOTOUCH_System.IO.Compression.FileSystem_test.dll", // issue https://github.com/xamarin/maccore/issues/1147 and https://github.com/xamarin/maccore/issues/1148
			"MONOTOUCH_System.Data_test.dll", // issue https://github.com/xamarin/maccore/issues/1149
			"MONOTOUCH_System.Data.DataSetExtensions_test.dll", // issue https://github.com/xamarin/maccore/issues/1150 and https://github.com/xamarin/maccore/issues/1151
			"MONOTOUCH_System.Core_test.dll", // issue https://github.com/xamarin/maccore/issues/1143
			"MONOTOUCH_Mono.Runtime.Tests_test.dll", // issue https://github.com/xamarin/maccore/issues/1141
			"MONOTOUCH_corlib_test.dll", // issue https://github.com/xamarin/maccore/issues/1153
			"MONOTOUCH_Commons.Xml.Relaxng_test.dll", // not supported by xamarin
			"MONOTOUCH_Cscompmgd_test.dll", // not supported by xamarin
			"MONOTOUCH_I18N.CJK_test.dll",
			"MONOTOUCH_I18N.MidEast_test.dll",
			"MONOTOUCH_I18N.Other_test.dll",
			"MONOTOUCH_I18N.Rare_test.dll",
			"MONOTOUCH_I18N.West_test.dll",
			"MONOTOUCH_Mono.C5_test.dll", // not supported by xamarin
			"MONOTOUCH_Mono.CodeContracts_test.dll", // not supported by xamarin
			"MONOTOUCH_Novell.Directory.Ldap_test.dll", // not supported by xamarin
			"MONOTOUCH_Mono.Profiler.Log_xunit-test.dll", // special tests that need an extra app to connect as a profiler
		};
		
		// list of assemblies that are going to be ignored, any project with an assemblies that is ignored will
		// be ignored

		static readonly List<string> iOSIgnoredAssemblies = new List<string> {};

		static readonly List<string> tvOSIgnoredAssemblies = new List<string> {
			"MONOTOUCH_System.Xml.Linq_xunit-test.dll", // issue https://github.com/xamarin/maccore/issues/1130
			"MONOTOUCH_System.Numerics_xunit-test.dll", // issue https://github.com/xamarin/maccore/issues/1129
		};

		static readonly List<string> watcOSIgnoredAssemblies = new List<string> {
			"MONOTOUCH_System.Xml.Linq_xunit-test.dll", // issue https://github.com/xamarin/maccore/issues/1130
			"MONOTOUCH_System.Numerics_xunit-test.dll", // issue https://github.com/xamarin/maccore/issues/1129
			"MONOTOUCH_Mono.Security_test.dll", // issue https://github.com/xamarin/maccore/issues/1142
			"MONOTOUCH_Mono.Data.Tds_test.dll", // issue https://gist.github.com/mandel-macaque/d97fa28f8a73c3016d1328567da77a0b
		};

		private static readonly List<(string name, string[] assemblies)> macTestProjects = new List<(string name, string[] assemblies)> {
		
			// NUNIT Projects
			(name:"MonoCSharpTests", assemblies: new [] {"xammac_net_4_5_Mono.CSharp_test.dll"}),
			(name:"MonoDataSqilteTests", assemblies: new [] {"xammac_net_4_5_Mono.Data.Sqlite_test.dll"}),
			(name:"MonoDataTdsTests", assemblies: new [] {"xammac_net_4_5_Mono.Data.Tds_test.dll"}),
			(name:"MonoPoxisTests", assemblies: new [] {"xammac_net_4_5_Mono.Posix_test.dll"}),
			(name:"MonoSecurtiyTests", assemblies: new [] {"xammac_net_4_5_Mono.Security_test.dll"}),
			(name:"SystemComponentModelDataAnnotationsTests", assemblies: new [] {"xammac_net_4_5_System.ComponentModel.DataAnnotations_test.dll"}),
			(name:"SystemConfigurationTests", assemblies: new [] {"xammac_net_4_5_System.Configuration_test.dll"}),
			(name:"SystemCoreTests", assemblies: new [] {"xammac_net_4_5_System.Core_test.dll"}),
			(name:"SystemDataLinqTests", assemblies: new [] {"xammac_net_4_5_System.Data.Linq_test.dll"}),
			(name:"SystemDataTests", assemblies: new [] {"xammac_net_4_5_System.Data_test.dll"}),
			(name:"SystemIOCompressionFileSystemTests", assemblies: new [] {"xammac_net_4_5_System.IO.Compression.FileSystem_test.dll"}),
			(name:"SystemIOCompressionTests", assemblies: new [] {"xammac_net_4_5_System.IO.Compression_test.dll"}),
			(name:"SystemIdentityModelTests", assemblies: new [] {"xammac_net_4_5_System.IdentityModel_test.dll"}),
			(name:"SystemJsonTests", assemblies: new [] {"xammac_net_4_5_System.Json_test.dll"}),
			(name:"SystemNetHttpTests", assemblies: new [] {"xammac_net_4_5_System.Net.Http_test.dll"}),
			(name:"SystemNumericsTests", assemblies: new [] {"xammac_net_4_5_System.Numerics_test.dll"}),
			(name:"SystemRuntimeSerializationFormattersSoapTests", assemblies: new [] {"xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll"}),
			(name:"SystemSecurityTests", assemblies: new [] {"xammac_net_4_5_System.Security_test.dll"}),
			(name:"SystemServiceModelTests", assemblies: new [] {"xammac_net_4_5_System.ServiceModel_test.dll"}),
			(name:"SystemTransactionsTests", assemblies: new [] {"xammac_net_4_5_System.Transactions_test.dll"}),
			(name:"SystemXmlLinqTests", assemblies: new [] {"xammac_net_4_5_System.Xml.Linq_test.dll"}),
			(name:"SystemXmlTests", assemblies: new [] {"xammac_net_4_5_System.Xml_test.dll"}),
			(name:"SystemTests", assemblies: new [] {"xammac_net_4_5_System_test.dll"}),
			
			// xUnit Projects
			(name:"MicrosoftCSharpXunit", assemblies: new [] {"xammac_net_4_5_Microsoft.CSharp_xunit-test.dll"}),
			(name:"SystemCoreXunit", assemblies: new [] {"xammac_net_4_5_System.Core_xunit-test.dll"}),
			(name:"SystemDataXunit", assemblies: new [] {"xammac_net_4_5_System.Data_xunit-test.dll"}),
			(name:"SystemJsonXunit", assemblies: new [] {"xammac_net_4_5_System.Json_xunit-test.dll"}),
			(name:"SystemNumericsXunit", assemblies: new [] {"xammac_net_4_5_System.Numerics_xunit-test.dll"}),
			(name:"SystemRuntimeCompilerServicesXunit", assemblies: new [] {"xammac_net_4_5_System.Runtime.CompilerServices.Unsafe_xunit-test.dll"}),
			(name:"SystemSecurityXunit", assemblies: new [] {"xammac_net_4_5_System.Security_xunit-test.dll"}),
			(name:"SystemXmlLinqXunit", assemblies: new [] {"xammac_net_4_5_System.Xml.Linq_xunit-test.dll"}),
			(name:"SystemXunit", assemblies: new [] {"xammac_net_4_5_System_xunit-test.dll"}),
			(name:"CorlibXunit", assemblies: new [] {"xammac_net_4_5_corlib_xunit-test.dll"}),
		};
		
		static readonly List<string> macIgnoredAssemblies = new List<string> {
			"xammac_net_4_5_corlib_test.dll	", // exception when loading the image via refection
			"xammac_net_4_5_I18N.CJK_test.dll",
			"xammac_net_4_5_I18N.MidEast_test.dll",
			"xammac_net_4_5_I18N.Other_test.dll",
			"xammac_net_4_5_I18N.Rare_test.dll",
			"xammac_net_4_5_I18N.West_test.dll",
		};

		readonly bool isCodeGeneration;
		public bool Override { get; set; }
		public string OutputDirectoryPath { get; private  set; }
		public string MonoRootPath { get; private set; }
		public string ProjectTemplateRootPath { get; private set; }
		public string PlistTemplateRootPath{ get; private set; }
		public string RegisterTypesTemplatePath { get; private set; }
		string GeneratedCodePathRoot => Path.Combine (OutputDirectoryPath, "generated");
		string WatchContainerTemplatePath => Path.Combine (OutputDirectoryPath, "templates", "watchOS", "Container").Replace ("/", "\\");
		string WatchAppTemplatePath => Path.Combine (OutputDirectoryPath, "templates", "watchOS", "App").Replace ("/", "\\");
		string WatchExtensionTemplatePath => Path.Combine (OutputDirectoryPath, "templates", "watchOS", "Extension").Replace ("/", "\\");

		public BCLTestProjectGenerator (string outputDirectory)
		{
			OutputDirectoryPath = outputDirectory ?? throw new ArgumentNullException (nameof (outputDirectory));
		}
		
		public BCLTestProjectGenerator (string outputDirectory, string monoRootPath, string projectTemplatePath, string registerTypesTemplatePath, string plistTemplatePath)
		{
			isCodeGeneration = true;
			OutputDirectoryPath = outputDirectory ?? throw new ArgumentNullException (nameof (outputDirectory));
			MonoRootPath = monoRootPath ?? throw new ArgumentNullException (nameof (monoRootPath));
			ProjectTemplateRootPath = projectTemplatePath ?? throw new ArgumentNullException (nameof (projectTemplatePath));
			PlistTemplateRootPath = plistTemplatePath ?? throw new ArgumentNullException (nameof (plistTemplatePath));
			RegisterTypesTemplatePath = registerTypesTemplatePath ?? throw new ArgumentNullException (nameof (registerTypesTemplatePath));
		}

		/// <summary>
		/// Returns the path to be used to store the project file depending on the platform.
		/// </summary>
		/// <param name="projectName">The name of the project being generated.</param>
		/// <param name="platform">The supported platform by the project.</param>
		/// <returns></returns>
		internal string GetProjectPath (string projectName, Platform platform)
		{
			switch (platform) {
			case Platform.iOS:
				return Path.Combine (OutputDirectoryPath, $"{projectName}.csproj");
			case Platform.TvOS:
				return Path.Combine (OutputDirectoryPath, $"{projectName}-tvos.csproj");
			case Platform.WatchOS:
				return Path.Combine (OutputDirectoryPath, $"{projectName}-watchos.csproj");
			case Platform.MacOSFull:
				return Path.Combine (OutputDirectoryPath, $"{projectName}-mac-full.csproj");
			case Platform.MacOSModern:
				return Path.Combine (OutputDirectoryPath, $"{projectName}-mac-modern.csproj");
			default:
				return null;
			}
		}
		
		internal string GetProjectPath (string projectName, WatchAppType appType)
		{
			switch (appType) {
			case WatchAppType.App:
				return Path.Combine (OutputDirectoryPath, $"{projectName}-watchos-app.csproj");
			default:
				return Path.Combine (OutputDirectoryPath, $"{projectName}-watchos-extension.csproj");
			}
		}

		/// <summary>
		/// Returns the path to be used to store the projects plist file depending on the platform.
		/// </summary>
		/// <param name="rootDir">The root dir to use.</param>
		/// <param name="platform">The platform that is supported by the project.</param>
		/// <returns></returns>
		internal static string GetPListPath (string rootDir, Platform platform)
		{
			switch (platform) {
			case Platform.iOS:
				return Path.Combine (rootDir, "Info.plist");
			case Platform.TvOS:
				return Path.Combine (rootDir, "Info-tv.plist");
			case Platform.WatchOS:
				return Path.Combine (rootDir, "Info-watchos.plist");
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				return Path.Combine (rootDir, "Info-mac.plist");
			default:
				return Path.Combine (rootDir, "Info.plist");
			}
		}

		internal static string GetPListPath (string rootDir, WatchAppType appType)
		{
			switch (appType) {
				case WatchAppType.App:
					return Path.Combine (rootDir, "Info-watchos-app.plist");
				default:
					return Path.Combine (rootDir, "Info-watchos-extension.plist");
			}
		}
		
		// creates the reference node
		internal static string GetReferenceNode (string assemblyName, string hintPath = null)
		{
			// lets not complicate our life with Xml, we just need to replace two things
			if (string.IsNullOrEmpty (hintPath)) {
				return $"<Reference Include=\"{assemblyName}\" />";
			} else {
				// the hint path is using unix separators, we need to use windows ones
				hintPath = hintPath.Replace ('/', '\\');
				var sb = new StringBuilder ();
				sb.AppendLine ($"<Reference Include=\"{assemblyName}\" >");
				sb.AppendLine ($"<HintPath>{hintPath}</HintPath>");
				sb.AppendLine ("</Reference>");
				return sb.ToString ();
			}
		}

		internal static string GetRegisterTypeNode (string registerPath)
		{
			var sb = new StringBuilder ();
			sb.AppendLine ($"<Compile Include=\"{registerPath}\">");
			sb.AppendLine ($"<Link>{Path.GetFileName (registerPath)}</Link>");
			sb.AppendLine ("</Compile>");
			return sb.ToString ();
		}

		/// <summary>
		/// Returns is a project should be ignored in a platform. A project is ignored in one of the assemblies in the
		/// project is ignored in the platform.
		/// </summary>
		/// <param name="project">The project which is under test.</param>
		/// <param name="platform">The platform to which we are testing against.</param>
		/// <returns>If the project should be ignored in a platform or not.</returns>
		bool IsIgnored(BCLTestProjectDefinition project, Platform platform)
		{
			foreach (var a in project.TestAssemblies){
				if (CommonIgnoredAssemblies.Contains (a.Name))
					return true;
				switch (platform){
				case Platform.iOS:
					return iOSIgnoredAssemblies.Contains (a.Name);
				case Platform.TvOS:
					return tvOSIgnoredAssemblies.Contains (a.Name);
				case Platform.WatchOS:
					return watcOSIgnoredAssemblies.Contains (a.Name);
				case Platform.MacOSFull:
				case Platform.MacOSModern:
					return macIgnoredAssemblies.Contains (a.Name);
				}
			}
			return false;
		}

		async Task<List<(string name, string path, bool xunit)>> GenerateWatchOSTestProjectsAsync (
			IEnumerable<(string name, string[] assemblies)> projects, string generatedDir)
		{
			var projectPaths = new List<(string name, string path, bool xunit)> ();
			foreach (var def in projects) {
				// each watch os project requires 3 different ones:
				// 1. The app
				// 2. The container
				// 3. The extensions
				// TODO: The following is very similar to what is done in the iOS generation. Must be grouped
				var projectDefinition = new BCLTestProjectDefinition (def.name, def.assemblies);
				if (IsIgnored (projectDefinition, Platform.WatchOS)) // if it is ignored, continue
					continue;

				if (!projectDefinition.Validate ())
					throw new InvalidOperationException ("xUnit and NUnit assemblies cannot be mixed in a test project.");
				var generatedCodeDir = Path.Combine (generatedDir, projectDefinition.Name);
				if (!Directory.Exists (generatedCodeDir)) {
					Directory.CreateDirectory (generatedCodeDir);
				}
				var registerTypePath = Path.Combine (generatedCodeDir, "RegisterType.cs");
				var registerCode = await RegisterTypeGenerator.GenerateCodeAsync (def.name, projectDefinition.IsXUnit,
					RegisterTypesTemplatePath, Platform.WatchOS);
				using (var file = new StreamWriter (registerTypePath, false)) { // false is do not append
					await file.WriteAsync (registerCode);
				}
				// create the plist for each of the apps
				var projectData = new Dictionary<WatchAppType, (string plist, string project)> ();
				foreach (var appType in new [] {WatchAppType.Extension, WatchAppType.App}) {
					(string plist, string project) data;
					var plistTemplate = Path.Combine (PlistTemplateRootPath, watchOSPlistTemplateMatches[appType]);
					var plist = await BCLTestInfoPlistGenerator.GenerateCodeAsync (plistTemplate, projectDefinition.Name);
					data.plist = GetPListPath (generatedCodeDir, appType);
					using (var file = new StreamWriter (data.plist, false)) { // false is do not append
						await file.WriteAsync (plist);
					}

					string generatedProject;
					var projetTemplate = Path.Combine (ProjectTemplateRootPath, watchOSProjectTemplateMatches[appType]);
					switch (appType) {
						case WatchAppType.App:
							generatedProject = await GenerateWatchAppAsync (projectDefinition.Name, projetTemplate, data.plist);
							break;
						default:
							generatedProject = await GenerateWatchExtensionAsync (projectDefinition.Name, projetTemplate, data.plist, registerTypePath, projectDefinition.GetCachedAssemblyInclusionInformation (MonoRootPath, Platform.WatchOS));
							break;
					}
					data.project = GetProjectPath (projectDefinition.Name, appType);
					using (var file = new StreamWriter (data.project, false)) { // false is do not append
						await file.WriteAsync (generatedProject);
					}

					projectData[appType] = data;
				} // foreach app type
				
				var rootPlistTemplate = Path.Combine (PlistTemplateRootPath, plistTemplateMatches[Platform.WatchOS]);
				var rootPlist = await BCLTestInfoPlistGenerator.GenerateCodeAsync (rootPlistTemplate, projectDefinition.Name);
				var infoPlistPath = GetPListPath (generatedCodeDir, Platform.WatchOS);
				using (var file = new StreamWriter (infoPlistPath, false)) { // false is do not append
					await file.WriteAsync (rootPlist);
				}
				
				var projectTemplatePath = Path.Combine (ProjectTemplateRootPath, projectTemplateMatches[Platform.WatchOS]);
				var rootProjectPath = GetProjectPath (projectDefinition.Name, Platform.WatchOS);
				using (var file = new StreamWriter (rootProjectPath, false)) // false is do not append
				using (var reader = new StreamReader (projectTemplatePath)){
					var template = await reader.ReadToEndAsync ();
					var generatedRootProject = GenerateWatchProject (def.name, template, infoPlistPath);
					await file.WriteAsync (generatedRootProject);
				}

				// we have the 3 projects we depend on, we need the root one, the one that will be used by harness
				projectPaths.Add ((name: projectDefinition.Name, path: rootProjectPath, xunit: projectDefinition.IsXUnit));
			} // foreach project

			return projectPaths;
		}
		
		async Task<List<(string name, string path, bool xunit)>> GenerateiOSTestProjectsAsync (
			IEnumerable<(string name, string[] assemblies)> projects, Platform platform, string generatedDir)
		{
			if (platform == Platform.WatchOS) 
				throw new ArgumentException (nameof (platform));
			var projectPaths = new List<(string name, string path, bool xunit)> ();
			foreach (var def in projects) {
				var projectDefinition = new BCLTestProjectDefinition (def.name, def.assemblies);
				if (IsIgnored (projectDefinition, platform)) // some projects are ignored, so we just continue
					continue;

				if (!projectDefinition.Validate ())
					throw new InvalidOperationException ("xUnit and NUnit assemblies cannot be mixed in a test project.");
				// generate the required type registration info
				var generatedCodeDir = Path.Combine (generatedDir, projectDefinition.Name);
				if (!Directory.Exists (generatedCodeDir)) {
					Directory.CreateDirectory (generatedCodeDir);
				}
				var registerTypePath = Path.Combine (generatedCodeDir, "RegisterType.cs");

				var registerCode = await RegisterTypeGenerator.GenerateCodeAsync (def.name, projectDefinition.IsXUnit,
					RegisterTypesTemplatePath, Platform.iOS);

				using (var file = new StreamWriter (registerTypePath, false)) { // false is do not append
					await file.WriteAsync (registerCode);
				}

				var plistTemplate = Path.Combine (PlistTemplateRootPath, plistTemplateMatches[platform]);
				var plist = await BCLTestInfoPlistGenerator.GenerateCodeAsync (plistTemplate, projectDefinition.Name);
				var infoPlistPath = GetPListPath (generatedCodeDir, platform);
				using (var file = new StreamWriter (infoPlistPath, false)) { // false is do not append
					await file.WriteAsync (plist);
				}

				var projectTemplatePath = Path.Combine (ProjectTemplateRootPath, projectTemplateMatches[platform]);
				var generatedProject = await GenerateAsync (projectDefinition.Name, registerTypePath,
					projectDefinition.GetCachedAssemblyInclusionInformation (MonoRootPath, platform), projectTemplatePath, infoPlistPath);
				var projectPath = GetProjectPath (projectDefinition.Name, platform);
				projectPaths.Add ((name: projectDefinition.Name, path: projectPath, xunit: projectDefinition.IsXUnit));
				using (var file = new StreamWriter (projectPath, false)) { // false is do not append
					await file.WriteAsync (generatedProject);
				}
			} // foreach project

			return projectPaths;
		}
		
		async Task<List<(string name, string path, bool xunit)>> GenerateMacTestProjectsAsync (
			IEnumerable<(string name, string[] assemblies)> projects, string generatedDir, Platform platform)
		{
			var projectPaths = new List<(string name, string path, bool xunit)> ();
			foreach (var def in projects) {
				var projectDefinition = new BCLTestProjectDefinition (def.name, def.assemblies);
				if (IsIgnored (projectDefinition, platform)) // some projects are ignored, so we just continue
					continue;

				if (!projectDefinition.Validate ())
					throw new InvalidOperationException ("xUnit and NUnit assemblies cannot be mixed in a test project.");
				// generate the required type registration info
				var generatedCodeDir = Path.Combine (generatedDir, projectDefinition.Name);
				Directory.CreateDirectory (generatedCodeDir);
				var registerTypePath = Path.Combine (generatedCodeDir, "RegisterType.cs");

				var registerCode = await RegisterTypeGenerator.GenerateCodeAsync (def.name, projectDefinition.IsXUnit,
					RegisterTypesTemplatePath, platform);

				using (var file = new StreamWriter (registerTypePath, false)) { // false is do not append
					await file.WriteAsync (registerCode);
				}
				
				var plistTemplate = Path.Combine (PlistTemplateRootPath, plistTemplateMatches[platform]);
				var plist = await BCLTestInfoPlistGenerator.GenerateCodeAsync (plistTemplate, projectDefinition.Name);
				var infoPlistPath = GetPListPath (generatedCodeDir, platform);
				using (var file = new StreamWriter (infoPlistPath, false)) { // false is do not append
					await file.WriteAsync (plist);
				}

				var projectTemplatePath = Path.Combine (ProjectTemplateRootPath, projectTemplateMatches[platform]);
				var generatedProject = await GenerateMacAsync (projectDefinition.Name, registerTypePath,
					projectDefinition.GetCachedAssemblyInclusionInformation (MonoRootPath, platform), projectTemplatePath, infoPlistPath, platform);
				var projectPath = GetProjectPath (projectDefinition.Name, platform);
				projectPaths.Add ((name: projectDefinition.Name, path: projectPath, xunit: projectDefinition.IsXUnit));
				using (var file = new StreamWriter (projectPath, false)) { // false is do not append
					await file.WriteAsync (generatedProject);
				}
			}
			return projectPaths;
		}

		/// <summary>
		/// Generates all the project files for the given projects and platform
		/// </summary>
		/// <param name="projects">The list of projects to be generated.</param>
		/// <param name="platform">The platform to which the projects have to be generated. Each platform
		/// has its own details.</param>
		/// <param name="generatedDir">The dir where the projects will be saved.</param>
		/// <returns></returns>
		async Task<List<(string name, string path, bool xunit)>> GenerateTestProjectsAsync (
			IEnumerable<(string name, string[] assemblies)> projects, Platform platform, string generatedDir)
		{
			List<(string name, string path, bool xunit)> result = new List<(string name, string path, bool xunit)> ();
			switch (platform) {
			case Platform.WatchOS:
				result = await GenerateWatchOSTestProjectsAsync (projects, generatedDir);
				break;
			case Platform.iOS:
				result = await GenerateiOSTestProjectsAsync (projects, platform, generatedDir);
				break;
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				result = await GenerateMacTestProjectsAsync (projects, generatedDir, platform);
				break;
			}
			return result;
		}
		
		// generates a project per platform of the common projects. 
		async Task<List<(string name, string path, bool xunit, List<Platform> platforms)>> GenerateAllCommonTestProjectsAsync ()
		{
			var projectPaths = new List<(string name, string path, bool xunit, List<Platform> platforms)> ();
			if (!isCodeGeneration)
				throw new InvalidOperationException ("Project generator was instantiated to delete the generated code.");
			var generatedCodePathRoot = GeneratedCodePathRoot;
			if (!Directory.Exists (generatedCodePathRoot)) {
				Directory.CreateDirectory (generatedCodePathRoot);
			}

			var projects = new Dictionary<string, (string path, bool xunit, List<Platform> platforms)> ();
			foreach (var platform in new [] {Platform.iOS, Platform.TvOS, Platform.WatchOS}) {
				var generated = await GenerateTestProjectsAsync (commoniOSTestProjects, platform, generatedCodePathRoot);
				foreach (var (name, path, xunit) in generated) {
					if (!projects.ContainsKey (name)) {
						projects [name] = (path, xunit, new List<Platform> { platform });
					} else {
						projects [name].platforms.Add (platform);
					}
				}
			} // foreach platform
			
			// return the grouped projects
			foreach (var name in projects.Keys) {
				projectPaths.Add ((name, projects[name].path, projects[name].xunit, projects[name].platforms));
			}
			return projectPaths;
		}
		
		// creates all the projects that have already been defined
		public async Task<List<(string name, string path, bool xunit, List<Platform> platforms)>> GenerateAlliOSTestProjectsAsync ()
		{
			var projectPaths = new List<(string name, string path, bool xunit, List<Platform> platforms)> ();
			if (!isCodeGeneration)
				throw new InvalidOperationException ("Project generator was instantiated to delete the generated code.");
			var generatedCodePathRoot = GeneratedCodePathRoot;
			if (!Directory.Exists (generatedCodePathRoot)) {
				Directory.CreateDirectory (generatedCodePathRoot);
			}
			// generate all the common projects
			projectPaths.AddRange (await GenerateAllCommonTestProjectsAsync ());

			return projectPaths;
		}

		public List<(string name, string path, bool xunit, List<Platform> platforms)> GenerateAlliOSTestProjects () => GenerateAlliOSTestProjectsAsync ().Result;
		
		public async Task<List<(string name, string path, bool xunit)>> GenerateAllMacTestProjectsAsync (Platform platform)
		{
			if (!isCodeGeneration)
				throw new InvalidOperationException ("Project generator was instantiated to delete the generated code.");
			var generatedCodePathRoot = GeneratedCodePathRoot;
			if (!Directory.Exists (generatedCodePathRoot)) {
				Directory.CreateDirectory (generatedCodePathRoot);
			}
			
			var generated = await GenerateTestProjectsAsync (macTestProjects, platform, generatedCodePathRoot);
			return generated;
		}

		public List<(string name, string path, bool xunit)> GenerateAllMacTestProjects (Platform platform) => GenerateAllMacTestProjectsAsync (platform).Result;

		/// <summary>
		/// Generates an iOS project for testing purposes. The generated project will contain the references to the
		/// mono test assemblies to run.
		/// </summary>
		/// <param name="projectName">The name of the project under generation.</param>
		/// <param name="registerPath">The path to the code that register the types so that the assemblies are not linked.</param>
		/// <param name="info">The list of assemblies to be added to the project and their hint paths.</param>
		/// <param name="templatePath">A path to the template used to generate the path.</param>
		/// <param name="infoPlistPath">The path to the info plist of the project.</param>
		/// <returns></returns>
		static async Task<string> GenerateAsync (string projectName, string registerPath, List<(string assembly, string hintPath)> info, string templatePath, string infoPlistPath)
		{
			// fix possible issues with the paths to be included in the msbuild xml
			infoPlistPath = infoPlistPath.Replace ('/', '\\');
			var sb = new StringBuilder ();
			foreach (var assemblyInfo in info) {
				if (!excludeDlls.Contains (assemblyInfo.assembly))
				sb.AppendLine (GetReferenceNode (assemblyInfo.assembly, assemblyInfo.hintPath));
			}

			using (var reader = new StreamReader(templatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (NameKey, projectName);
				result = result.Replace (ReferencesKey, sb.ToString ());
				result = result.Replace (RegisterTypeKey, GetRegisterTypeNode (registerPath));
				result = result.Replace (PlistKey, infoPlistPath);
				return result;
			}
		}
		
		static async Task<string> GenerateMacAsync (string projectName, string registerPath, List<(string assembly, string hintPath)> info, string templatePath, string infoPlistPath, Platform platform)
		{
			infoPlistPath = infoPlistPath.Replace ('/', '\\');
			var sb = new StringBuilder ();
			foreach (var assemblyInfo in info) {
				if (!excludeDlls.Contains (assemblyInfo.assembly))
				sb.AppendLine (GetReferenceNode (assemblyInfo.assembly, assemblyInfo.hintPath));
			}

			using (var reader = new StreamReader(templatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (NameKey, projectName);
				result = result.Replace (ReferencesKey, sb.ToString ());
				result = result.Replace (RegisterTypeKey, GetRegisterTypeNode (registerPath));
				result = result.Replace (PlistKey, infoPlistPath);
				switch (platform){
				case Platform.MacOSFull:
					result = result.Replace (TargetFrameworkVersionKey, "v4.5");
					result = result.Replace (TargetExtraInfoKey,
						"<UseXamMacFullFramework>true</UseXamMacFullFramework>");
					result = result.Replace (DefineConstantsKey, "XAMCORE_2_0;ADD_BCL_EXCLUSIONS;XAMMAC_4_5");
					break;
				case Platform.MacOSModern:
					result = result.Replace (TargetFrameworkVersionKey, "v2.0");
					result = result.Replace (TargetExtraInfoKey,
						"<TargetFrameworkIdentifier>Xamarin.Mac</TargetFrameworkIdentifier>");
					result = result.Replace (DefineConstantsKey, "XAMCORE_2_0;ADD_BCL_EXCLUSIONS;MOBILE;XAMMAC");
					break;
				}
				return result;
			}
		}

		internal string GenerateWatchProject (string projectName, string template, string infoPlistPath)
		{
				var result = template.Replace (NameKey, projectName);
				result = result.Replace (WatchOSTemplatePathKey, WatchContainerTemplatePath);
				result = result.Replace (PlistKey, infoPlistPath);
				result = result.Replace (WatchOSCsporjAppKey, GetProjectPath (projectName, WatchAppType.App).Replace ("/", "\\"));
				return result;
		}

		async Task<string> GenerateWatchAppAsync (string projectName, string templatePath, string infoPlistPath)
		{
			using (var reader = new StreamReader(templatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (NameKey, projectName);
				result = result.Replace (WatchOSTemplatePathKey, WatchAppTemplatePath);
				result = result.Replace (PlistKey, infoPlistPath);
				result = result.Replace (WatchOSCsporjExtensionKey, GetProjectPath (projectName, WatchAppType.Extension).Replace ("/", "\\"));
				return result;
			}
		}

		async Task<string> GenerateWatchExtensionAsync (string projectName, string templatePath, string infoPlistPath, string registerPath, List<(string assembly, string hintPath)> info)
		{
			var sb = new StringBuilder ();
			foreach (var assemblyInfo in info) {
				if (!excludeDlls.Contains (assemblyInfo.assembly))
					sb.AppendLine (GetReferenceNode (assemblyInfo.assembly, assemblyInfo.hintPath));
			}
			
			using (var reader = new StreamReader(templatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (NameKey, projectName);
				result = result.Replace (WatchOSTemplatePathKey, WatchExtensionTemplatePath);
				result = result.Replace (PlistKey, infoPlistPath);
				result = result.Replace (RegisterTypeKey, GetRegisterTypeNode (registerPath));
				result = result.Replace (ReferencesKey, sb.ToString ());
				return result;
			}
		}

		public static string Generate (string projectName, string registerPath, List<(string assembly, string hintPath)> info, string templatePath, string infoPlistPath) =>
			GenerateAsync (projectName, registerPath, info, templatePath, infoPlistPath).Result;

		/// <summary>
		/// Removes all the generated files by the tool.
		/// </summary>
		public void CleanOutput ()
		{
			if (isCodeGeneration)
				throw new InvalidOperationException ("Project generator was instantiated to project generation.");
			if (Directory.Exists (GeneratedCodePathRoot))
				Directory.Delete (GeneratedCodePathRoot, true);
			// delete all the common projects
			foreach (var platform in new [] {Platform.iOS, Platform.TvOS}) {
				foreach (var testProject in commoniOSTestProjects) {
					var projectPath = GetProjectPath (testProject.name, platform);
					if (File.Exists (projectPath))
						File.Delete (projectPath);
				}
			}
			// delete each of the generated project files
			foreach (var projectDefinition in commoniOSTestProjects) {
				var projectPath = GetProjectPath (projectDefinition.name, Platform.iOS);
				if (File.Exists (projectPath))
					File.Delete (projectPath);
			}	
		}

		/// <summary>
		/// Returns if all the test assemblies found in the mono path 
		/// </summary>
		/// <param name="missingAssemblies"></param>
		/// <returns></returns>
		public bool AllTestAssembliesAreRan (out Dictionary<Platform, List<string>> missingAssemblies)
		{
			missingAssemblies = new Dictionary<Platform, List<string>> ();
			foreach (var platform in new [] {Platform.iOS, Platform.TvOS}) {
				var testDir = BCLTestAssemblyDefinition.GetTestDirectory (MonoRootPath, platform); 
				var missingAssembliesPlatform = Directory.GetFiles (testDir, NUnitPattern).Select (Path.GetFileName).Union (
					Directory.GetFiles (testDir, xUnitPattern).Select (Path.GetFileName)).ToList ();
				
				foreach (var assembly in CommonIgnoredAssemblies) {
					missingAssembliesPlatform.Remove (assembly);
				}
				
				// loop over the mono root path and grab all the assemblies, then intersect the found ones with the added
				// and ignored ones.
				foreach (var projectDefinition in commoniOSTestProjects) {
					foreach (var testAssembly in projectDefinition.assemblies) {
						missingAssembliesPlatform.Remove (testAssembly);
					}
				}

				if (missingAssembliesPlatform.Count != 0) {
					missingAssemblies[platform] = missingAssembliesPlatform;
				}
			}
			return missingAssemblies.Keys.Count == 0;
		}
	}
}
