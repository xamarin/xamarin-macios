using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using xharness;

namespace BCLTestImporter {
	/// <summary>
	/// Class that knows how to generate .csproj files based on a BCLTestProjectDefinition.
	/// </summary>
	public class BCLTestProjectGenerator {

		static string NUnitPattern = "monotouch_*_test.dll"; 
		static string xUnitPattern = "monotouch_*_xunit-test.dll";
		internal static readonly string ProjectGuidKey = "%PROJECT GUID%";
		internal static readonly string NameKey = "%NAME%";
		internal static readonly string ReferencesKey = "%REFERENCES%";
		internal static readonly string RegisterTypeKey = "%REGISTER TYPE%";
		internal static readonly string ContentKey = "%CONTENT RESOURCES%";
		internal static readonly string PlistKey = "%PLIST PATH%";
		internal static readonly string WatchOSTemplatePathKey = "%TEMPLATE PATH%";
		internal static readonly string WatchOSCsporjAppKey = "%WATCH APP PROJECT PATH%";
		internal static readonly string WatchOSCsporjExtensionKey  ="%WATCH EXTENSION PROJECT PATH%";
		internal static readonly string TargetFrameworkVersionKey = "%TARGET FRAMEWORK VERSION%";
		internal static readonly string TargetExtraInfoKey = "%TARGET EXTRA INFO%";
		internal static readonly string DefineConstantsKey = "%DEFINE CONSTANTS%";
		internal static readonly string DownloadPathKey = "%DOWNLOAD PATH%";

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
			"Xunit.NetCore.Extensions",
		};

		public class BclTestProjectInfo {
			public string Name;
			public string [] assemblies;
			public string ExtraArgs;
			public string Group;
			public double TimeoutMultiplier = 1;
		}

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
			new BclTestProjectInfo { Name = "SystemCoreXunit", assemblies = new [] { "monotouch_System.Core_xunit-test.dll" }, Group = "BCL tests group 4" },
			new BclTestProjectInfo { Name = "SystemXunit", assemblies = new [] { "monotouch_System_xunit-test.dll" }, ExtraArgs = $"--xml={Path.Combine (Harness.RootDirectory, "bcl-test", "SystemXunitLinker.xml")} --optimize=-custom-attributes-removal", Group = "BCL tests group 4" },
			new BclTestProjectInfo { Name = "MicrosoftCSharpXunit", assemblies = new [] { "monotouch_Microsoft.CSharp_xunit-test.dll" }, Group = "BCL tests group 4" },

			// BCL tests group 5
			new BclTestProjectInfo { Name = "mscorlib", assemblies = new [] { "monotouch_corlib_xunit-test.dll" }, Group = "mscorlib" }, // special testcase for the corlib which is later used in xHarness for diff config options

			// BCL tests group 6
			new BclTestProjectInfo { Name = "SystemNetHttpUnitTestsXunit", assemblies = new [] { "monotouch_System.Net.Http.UnitTests_xunit-test.dll" }, Group = "BCL tests group 6" },
			new BclTestProjectInfo { Name = "SystemNetHttpFunctionalTestsXunit", assemblies = new [] { "monotouch_System.Net.Http.FunctionalTests_xunit-test.dll" }, Group = "BCL tests group 6" },
		};
			
		static readonly List <string> CommonIgnoredAssemblies = new List <string> {
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

		static readonly List<string> iOSIgnoredAssemblies = new List<string> {};

		static readonly List<string> tvOSIgnoredAssemblies = new List<string> {
		};

		static readonly List<string> watcOSIgnoredAssemblies = new List<string> {
			"monotouch_Mono.Security_test.dll",
			"monotouch_Mono.Data.Tds_test.dll", // not present in the watch tests dlls
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
		
		static readonly List<(string assembly, Platform[] platforms)> macIgnoredAssemblies = new List<(string assembly, Platform[] platforms)> {
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

		readonly bool isCodeGeneration;
		readonly BCLPathManager pathManager;
		public bool Override { get; set; }
		public bool GroupTests { get; set; }
		public string OutputDirectoryPath { get; private  set; }
		public string MonoRootPath { get; private set; }
		public string iOSMonoSDKPath { get; set; }
		public string MacMonoSDKPath { get; set; }
		public string RegisterTypesTemplatePath { get; private set; }
		public Func<string, Guid> GuidGenerator { get; set; }

		public BCLTestProjectGenerator (string outputDirectory)
		{
			OutputDirectoryPath = outputDirectory ?? throw new ArgumentNullException (nameof (outputDirectory));
		}
		
		public BCLTestProjectGenerator (string outputDirectory, string monoRootPath, string projectTemplatePath, string registerTypesTemplatePath, string plistTemplatePath)
		{
			isCodeGeneration = true;
			pathManager = new BCLPathManager (outputDirectory, projectTemplatePath, plistTemplatePath);
			OutputDirectoryPath = outputDirectory ?? throw new ArgumentNullException (nameof (outputDirectory));
			MonoRootPath = monoRootPath ?? throw new ArgumentNullException (nameof (monoRootPath));
			RegisterTypesTemplatePath = registerTypesTemplatePath ?? throw new ArgumentNullException (nameof (registerTypesTemplatePath));
		}

		string GetReleaseDownload (Platform platform)
		{
			switch (platform) {
			case Platform.iOS:
			case Platform.TvOS:
			case Platform.WatchOS:
				// simply, try to find the dir with the pattern
				return iOSMonoSDKPath;
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				return MacMonoSDKPath;
			default:
				return null;
			}
		}

		static void CopyAssets (string targetDir)
		{
			var assetsPath = Path.Combine (Harness.RootDirectory, "bcl-test", "Assets.xcassets");
			var assetsTargetDir = Path.Combine (targetDir, "Assets.xcassets");
			if (!Directory.Exists (assetsTargetDir))
				Directory.CreateDirectory (assetsTargetDir);
			var contentDir = Path.Combine (assetsTargetDir, "AppIcon.appiconset");
			if (!Directory.Exists (contentDir))
				Directory.CreateDirectory (contentDir);
			File.Copy (Path.Combine (assetsPath, "AppIcon.appiconset", "Contents.json"), contentDir, true);
			File.Copy (Path.Combine (assetsPath, "Contents.json"), assetsTargetDir, true);
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

		internal static string GetContentNode (string resourcePath)
		{
			var sb = new StringBuilder ();
			sb.AppendLine ($"<Content Include=\"{resourcePath}\">");
			sb.AppendLine ($"<Link>{Path.GetFileName (resourcePath)}</Link>");
			sb.AppendLine ("<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>");
			sb.AppendLine ("</Content>");
			return sb.ToString ();
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
		
		internal static string[] GetIgnoreFileNames (string name, Platform platform)
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
		
		internal static IEnumerable<string> GetIgnoreFiles (string templatePath, string projectName, List<(string assembly, string hintPath)> assemblies, Platform platform)
		{
			// check if the common and plaform paths can be found in the template path, if they are, we return them
			var templateDir = Path.GetDirectoryName (templatePath);
			var commonIgnore = Path.Combine (templateDir, GetCommonIgnoreFileName (projectName, platform));
			if (File.Exists (commonIgnore))
				yield return commonIgnore;
			foreach (var platformFile in GetIgnoreFileNames (projectName, platform)) {
				var platformIgnore = Path.Combine (templateDir, platformFile);
				if (File.Exists (platformIgnore))
					yield return platformIgnore;
			}
			// do we have ignores per files and not the project name? Add them
			foreach (var (assembly, hintPath) in assemblies) {
				foreach (var platformFile in GetIgnoreFileNames (assembly, platform)) {
					var commonAssemblyIgnore = Path.Combine (templateDir, GetCommonIgnoreFileName (assembly, platform));
					if (File.Exists (commonAssemblyIgnore))
						yield return commonAssemblyIgnore;
					var platformAssemblyIgnore = Path.Combine (templateDir, platformFile);
					if (File.Exists (platformAssemblyIgnore))
						yield return platformAssemblyIgnore;
				}
			}
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
					return macIgnoredAssemblies.Any ((ignored) => (ignored.assembly == a.Name) && ignored.platforms.Contains (Platform.MacOSFull));
				case Platform.MacOSModern:
					return macIgnoredAssemblies.Any ((ignored) => (ignored.assembly == a.Name) && ignored.platforms.Contains (Platform.MacOSModern));
				}
			}
			return false;
		}
		
		bool IsIgnored (string a, Platform platform)
		{
			if (CommonIgnoredAssemblies.Contains (a))
				return true;
			switch (platform) {
			case Platform.iOS:
				return iOSIgnoredAssemblies.Contains (a);
			case Platform.TvOS:
				return tvOSIgnoredAssemblies.Contains (a);
			case Platform.WatchOS:
				return watcOSIgnoredAssemblies.Contains (a);
			case Platform.MacOSFull:
				return macIgnoredAssemblies.Any ((ignored) => (ignored.assembly == a) && ignored.platforms.Contains (Platform.MacOSFull));
			case Platform.MacOSModern:
				return macIgnoredAssemblies.Any ((ignored) => (ignored.assembly == a) && ignored.platforms.Contains (Platform.MacOSModern));
			}
			return false;
		}

		async Task<List<BclTestProject>> GenerateWatchOSTestProjectsAsync (IEnumerable<BclTestProjectInfo> projects)
		{
			var projectPaths = new List<BclTestProject> ();
			foreach (var def in projects) {
				// each watch os project requires 3 different ones:
				// 1. The app
				// 2. The container
				// 3. The extensions
				// TODO: The following is very similar to what is done in the iOS generation. Must be grouped
				var projectDefinition = new BCLTestProjectDefinition (def.Name, def.assemblies, def.ExtraArgs);
				if (IsIgnored (projectDefinition, Platform.WatchOS)) // if it is ignored, continue
					continue;

				if (!projectDefinition.Validate ())
					throw new InvalidOperationException ("xUnit and NUnit assemblies cannot be mixed in a test project.");

				var paths = pathManager.GetProjectPaths (projectDefinition, Platform.WatchOS);
				
				string failure = null;
				// copy the required assets
				CopyAssets (paths.AssetsDirectoryPath);
				try {
					// create the plist for each of the apps
					var projectData = new Dictionary<WatchAppType, (string plist, string project)> ();
					foreach (var appType in new [] { WatchAppType.Extension, WatchAppType.App }) {
						(string plist, string project) data;
						data.plist = paths.WatchOSProjectPaths.GetPListPath (appType);
						await BCLTestInfoPlistGenerator.GenerateCodeToFileAsync (paths.WatchOSProjectPaths.GetPListTemplatePath (appType), projectDefinition.Name, data.plist);
						
						var projetTemplate = pathManager.GetProjectTemplate (appType);
						switch (appType) {
						case WatchAppType.App:
							await GenerateWatchAppToFileAsync (projectDefinition.Name, paths);
							break;
						default:
							var info = projectDefinition.GetAssemblyInclusionInformation (GetReleaseDownload (Platform.WatchOS), Platform.WatchOS, true);
							await GenerateWatchExtensionToFileAsync (projectDefinition.Name, paths, info);
							failure = failure ?? info.FailureMessage;
							break;
						}
						
						data.project = paths.WatchOSProjectPaths.GetProjectPath (appType);

						projectData [appType] = data;
					} // foreach app type

					await BCLTestInfoPlistGenerator.GenerateCodeToFileAsync (pathManager.GetPListTemplatePath (Platform.WatchOS), projectDefinition.Name, BCLPathManager.GetPListPath (paths.GeneratedSubdirPath, Platform.WatchOS));
					await GenerateWatchProjectToFileAsync (projectDefinition.Name, paths);
					
					var typesPerAssembly = projectDefinition.GetTypeForAssemblies (GetReleaseDownload (Platform.iOS), Platform.WatchOS, true);
					await RegisterTypeGenerator.GenerateCodeToFileAsync (typesPerAssembly, projectDefinition.IsXUnit, RegisterTypesTemplatePath, paths.RegisterTypePath);

					failure = failure ?? typesPerAssembly.FailureMessage;
				} catch (Exception e) {
					failure = e.Message;
				}
				// we have the 3 projects we depend on, we need the root one, the one that will be used by harness
				projectPaths.Add (new BclTestProject { Name = projectDefinition.Name, Path = paths.ProjectPath, XUnit = projectDefinition.IsXUnit, ExtraArgs = projectDefinition.ExtraArgs, Failure = failure, TimeoutMultiplier = def.TimeoutMultiplier } );
			} // foreach project

			return projectPaths;
		}
		
		async Task<List<BclTestProject>> GenerateiOSTestProjectsAsync (IEnumerable<BclTestProjectInfo> projects, Platform platform)
		{
			if (platform == Platform.WatchOS) 
				throw new ArgumentException (nameof (platform));
			if (!projects.Any()) // return an empty list
				return new List<BclTestProject> ();
			var projectPaths = new List<BclTestProject> ();
			foreach (var def in projects) {
				if (def.assemblies.Length == 0)
					continue;
				var projectDefinition = new BCLTestProjectDefinition (def.Name, def.assemblies, def.ExtraArgs);
				if (IsIgnored (projectDefinition, platform)) // some projects are ignored, so we just continue
					continue;

				if (!projectDefinition.Validate ())
					throw new InvalidOperationException ("xUnit and NUnit assemblies cannot be mixed in a test project.");
					
				var paths = pathManager.GetProjectPaths (projectDefinition, platform);
				// generate the required type registration info
				// copy the require assets
				CopyAssets (paths.AssetsDirectoryPath);
				// projects have to be inthe same dir, not in the io/tvos ones
				string failure = null;
				try {
					var typesPerAssembly = projectDefinition.GetTypeForAssemblies (GetReleaseDownload (platform), platform, true);
					var info = projectDefinition.GetAssemblyInclusionInformation (GetReleaseDownload (platform), platform, true);
					
					await RegisterTypeGenerator.GenerateCodeToFileAsync (typesPerAssembly, projectDefinition.IsXUnit, RegisterTypesTemplatePath, paths.RegisterTypePath);
					await BCLTestInfoPlistGenerator.GenerateCodeToFileAsync (paths.PListTemplatePath, projectDefinition.Name, paths.PListPath);
					await GenerateAsync (projectDefinition.Name, paths, info, platform);

					failure = failure ?? info.FailureMessage;
					failure = failure ?? typesPerAssembly.FailureMessage;
				} catch (Exception e) {
					failure = e.Message;
				}
				projectPaths.Add (new BclTestProject { Name = projectDefinition.Name, Path = paths.ProjectPath, XUnit = projectDefinition.IsXUnit, ExtraArgs = projectDefinition.ExtraArgs, Failure = failure, TimeoutMultiplier = def.TimeoutMultiplier });
			} // foreach project

			return projectPaths;
		}
		
		async Task<List<BclTestProject>> GenerateMacTestProjectsAsync (IEnumerable<BclTestProjectInfo> projects, Platform platform)
		{
			var projectPaths = new List<BclTestProject> ();
			foreach (var def in projects) {
				if (!def.assemblies.Any ())
					continue;
				var projectDefinition = new BCLTestProjectDefinition (def.Name, def.assemblies, def.ExtraArgs);
				if (IsIgnored (projectDefinition, platform)) // some projects are ignored, so we just continue
					continue;

				if (!projectDefinition.Validate ())
					throw new InvalidOperationException ("xUnit and NUnit assemblies cannot be mixed in a test project.");
					
				var paths = pathManager.GetProjectPaths (projectDefinition, platform);
				var typesPerAssembly = projectDefinition.GetTypeForAssemblies (GetReleaseDownload (platform), platform, true);
				
				await RegisterTypeGenerator.GenerateCodeToFileAsync (typesPerAssembly, projectDefinition.IsXUnit, RegisterTypesTemplatePath, paths.RegisterTypePath);
				await BCLTestInfoPlistGenerator.GenerateCodeToFileAsync (paths.PListTemplatePath, projectDefinition.Name, paths.PListPath);

				var info = projectDefinition.GetAssemblyInclusionInformation (GetReleaseDownload (platform), platform, true);
				await GenerateMacToFileAsync (projectDefinition.Name, paths, info, platform);
				
				// copy the require assets
				CopyAssets (paths.AssetsDirectoryPath);
				projectPaths.Add (new BclTestProject { Name = projectDefinition.Name, Path = paths.ProjectPath, XUnit = projectDefinition.IsXUnit, ExtraArgs = projectDefinition.ExtraArgs, Failure = null, TimeoutMultiplier = def.TimeoutMultiplier });
			}
			return projectPaths;
		}

		/// <summary>
		/// Generates all the project files for the given projects and platform
		/// </summary>
		/// <param name="projects">The list of projects to be generated.</param>
		/// <param name="platform">The platform to which the projects have to be generated. Each platform
		/// has its own details.</param>
		/// <returns></returns>
		public async Task<List<BclTestProject>> GenerateTestProjectsAsync (
			IEnumerable<BclTestProjectInfo> projects, Platform platform)
		{
			var result = new List<BclTestProject> ();
			switch (platform) {
			case Platform.WatchOS:
				result = await GenerateWatchOSTestProjectsAsync (projects);
				break;
			case Platform.iOS:
			case Platform.TvOS:
				result = await GenerateiOSTestProjectsAsync (projects, platform);
				break;
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				result = await GenerateMacTestProjectsAsync (projects, platform);
				break;
			}
			return result;
		}
		
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
						if (!IsIgnored (a, platform)) 
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
		// generates a project per platform of the common projects. 
		async Task<List<iOSBclTestProject>> GenerateAllCommonTestProjectsAsync ()
		{
			var projectPaths = new List<iOSBclTestProject> ();
			if (!isCodeGeneration)
				throw new InvalidOperationException ("Project generator was instantiated to delete the generated code.");

			var projects = new Dictionary<string, iOSBclTestProject> ();
			foreach (var platform in new [] {Platform.iOS, Platform.TvOS, Platform.WatchOS}) {
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

		public class iOSBclTestProject {
			public string Name;
			public string Path;
			public bool XUnit;
			public string ExtraArgs;
			public List<Platform> Platforms;
			public string Failure;
			public double TimeoutMultiplier = 1;
		}

		public class BclTestProject {
			public string Name;
			public string Path;
			public bool XUnit;
			public string ExtraArgs;
			public string Failure;
			public double TimeoutMultiplier = 1;
		}

		// creates all the projects that have already been defined
		public async Task<List<iOSBclTestProject>> GenerateAlliOSTestProjectsAsync ()
		{
			var projectPaths = new List<iOSBclTestProject> ();
			if (!isCodeGeneration)
				throw new InvalidOperationException ("Project generator was instantiated to delete the generated code.");
			// generate all the common projects
			projectPaths.AddRange (await GenerateAllCommonTestProjectsAsync ());

			return projectPaths;
		}

		public List<iOSBclTestProject> GenerateAlliOSTestProjects () => GenerateAlliOSTestProjectsAsync ().Result;
		
		public async Task<List<BclTestProject>> GenerateAllMacTestProjectsAsync (Platform platform)
		{
			if (!isCodeGeneration)
				throw new InvalidOperationException ("Project generator was instantiated to delete the generated code.");
			var generated = await GenerateTestProjectsAsync (GetProjectDefinitions (macTestProjects, platform), platform);
			return generated;
		}

		public List<BclTestProject> GenerateAllMacTestProjects (Platform platform) => GenerateAllMacTestProjectsAsync (platform).Result;

		/// <summary>
		/// Generates an iOS project for testing purposes. The generated project will contain the references to the
		/// mono test assemblies to run.
		/// </summary>
		/// <param name="projectName">The name of the project under generation.</param>
		/// <param name="info">The list of assemblies to be added to the project and their hint paths.</param>
		/// <returns></returns>
		async Task GenerateAsync (string projectName, BCLProjectPaths paths, (string FailureMessage, List<(string assembly, string hintPath)> Assemblies) info, Platform platform)
		{
			var downloadPath = GetReleaseDownload (platform).Replace ("/", "\\");
			// fix possible issues with the paths to be included in the msbuild xml
			var sb = new StringBuilder ();
			if (!string.IsNullOrEmpty (info.FailureMessage)) {
				WriteReferenceFailure (sb, info.FailureMessage);
			} else {
				foreach (var assemblyInfo in info.Assemblies) {
					if (!excludeDlls.Contains (assemblyInfo.assembly))
						sb.AppendLine (GetReferenceNode (assemblyInfo.assembly, assemblyInfo.hintPath));
				}
			}
			
			var contentFiles = new StringBuilder ();
			foreach (var path in GetIgnoreFiles (paths.ProjectTemplatePath, projectName, info.Assemblies, platform)) {
				contentFiles.Append (GetContentNode (path));
			}
			var projectGuid = GuidGenerator?.Invoke (projectName) ?? Guid.NewGuid ();
			using (var reader = new StreamReader(paths.ProjectTemplatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (DownloadPathKey, downloadPath);
				result = result.Replace (ProjectGuidKey, projectGuid.ToString ().ToUpperInvariant ());
				result = result.Replace (NameKey, projectName);
				result = result.Replace (ReferencesKey, sb.ToString ());
				result = result.Replace (RegisterTypeKey, GetRegisterTypeNode (paths.RegisterTypePath));
				result = result.Replace (PlistKey, paths.PListPath.Replace ('/', '\\'));
				result = result.Replace (ContentKey, contentFiles.ToString ());
				using (var file = new StreamWriter (paths.ProjectPath, false)) { // false is do not append
					await file.WriteAsync (result);
				}
			}
		}
		
		async Task GenerateMacToFileAsync (string projectName, BCLProjectPaths paths,(string FailureMessage, List<(string assembly, string hintPath)> Assemblies) info, Platform platform)
		{
			var downloadPath = Path.Combine(GetReleaseDownload (platform), "mac-bcl", platform == Platform.MacOSFull? "xammac_net_4_5" : "xammac").Replace ("/", "\\");
			var sb = new StringBuilder ();
			if (!string.IsNullOrEmpty (info.FailureMessage)) {
				WriteReferenceFailure (sb, info.FailureMessage);
			} else {
				foreach (var assemblyInfo in info.Assemblies) {
					if (!excludeDlls.Contains (assemblyInfo.assembly))
						sb.AppendLine (GetReferenceNode (assemblyInfo.assembly, assemblyInfo.hintPath));
				}
			}

			var contentFiles = new StringBuilder ();
			foreach (var path in GetIgnoreFiles (paths.ProjectTemplatePath, projectName, info.Assemblies, platform)) {
				contentFiles.Append (GetContentNode (path));
			}
			var projectGuid = GuidGenerator?.Invoke (projectName) ?? Guid.NewGuid ();
			using (var reader = new StreamReader(paths.ProjectTemplatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (DownloadPathKey, downloadPath);
				result = result.Replace (ProjectGuidKey, projectGuid.ToString ().ToUpperInvariant ());
				result = result.Replace (NameKey, projectName);
				result = result.Replace (ReferencesKey, sb.ToString ());
				result = result.Replace (RegisterTypeKey, GetRegisterTypeNode (paths.RegisterTypePath));
				result = result.Replace (PlistKey, paths.PListPath.Replace ('/', '\\'));
				result = result.Replace (ContentKey, contentFiles.ToString ());
				switch (platform){
				case Platform.MacOSFull:
					result = result.Replace (TargetFrameworkVersionKey, "v4.5.2");
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
				using (var file = new StreamWriter (paths.ProjectPath, false)) { // false is do not append
					await file.WriteAsync (result);
				}
			}
		}

		async Task GenerateWatchProjectToFileAsync (string projectName, BCLProjectPaths paths)
		{
			var projectTemplatePath = pathManager.GetProjectTemplate (Platform.WatchOS);
			using (var file = new StreamWriter (paths.ProjectPath, false)) // false is do not append
			using (var reader = new StreamReader (projectTemplatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (NameKey, projectName);
				result = result.Replace (WatchOSTemplatePathKey, pathManager.WatchContainerTemplatePath);
				result = result.Replace (PlistKey, paths.PListPath);
				result = result.Replace (WatchOSCsporjAppKey, paths.WatchOSProjectPaths.GetProjectPath (WatchAppType.App).Replace ('/', '\\'));
				await file.WriteAsync (result);
			}
		}

		async Task GenerateWatchAppToFileAsync (string projectName, BCLProjectPaths paths)
		{
			using (var reader = new StreamReader(pathManager.GetProjectTemplate (WatchAppType.App))) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (NameKey, projectName);
				result = result.Replace (WatchOSTemplatePathKey, pathManager.WatchAppTemplatePath);
				result = result.Replace (PlistKey, paths.WatchOSProjectPaths.GetPListPath (WatchAppType.App));
				result = result.Replace (WatchOSCsporjExtensionKey, paths.WatchOSProjectPaths.GetProjectPath (WatchAppType.App));
				using (var file = new StreamWriter (paths.WatchOSProjectPaths.GetProjectPath (WatchAppType.App), false)) { // false is do not append
					await file.WriteAsync (result);
				}
			}
		}

		static void WriteReferenceFailure (StringBuilder sb, string failureMessage)
		{
			sb.AppendLine ($"<!-- Failed to load all assembly references; please 'git clean && make' the tests/ directory to re-generate the project files -->");
			sb.AppendLine ($"<!-- Failure message: {failureMessage} --> ");
			sb.AppendLine ("<Reference Include=\"ProjectGenerationFailure_PleaseRegenerateProjectFiles.dll\" />"); // Make sure the project fails to build.
		}

		async Task GenerateWatchExtensionToFileAsync (string projectName, BCLProjectPaths paths, (string FailureMessage, List<(string assembly, string hintPath)> Assemblies) info)
		{
			var templatePath = pathManager.GetProjectTemplate (WatchAppType.Extension);
			var downloadPath = GetReleaseDownload (Platform.WatchOS).Replace ("/", "\\");
			var sb = new StringBuilder ();
			if (!string.IsNullOrEmpty (info.FailureMessage)) {
				WriteReferenceFailure (sb, info.FailureMessage);
			} else {
				foreach (var assemblyInfo in info.Assemblies) {
					if (!excludeDlls.Contains (assemblyInfo.assembly))
						sb.AppendLine (GetReferenceNode (assemblyInfo.assembly, assemblyInfo.hintPath));
				}
			}
			
			var contentFiles = new StringBuilder ();
			foreach (var path in GetIgnoreFiles (templatePath, projectName, info.Assemblies, Platform.WatchOS)) {
				contentFiles.Append (GetContentNode (path));
			}
			
			using (var reader = new StreamReader(templatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (DownloadPathKey, downloadPath);
				result = result.Replace (NameKey, projectName);
				result = result.Replace (WatchOSTemplatePathKey, pathManager.WatchExtensionTemplatePath);
				result = result.Replace (PlistKey, paths.WatchOSProjectPaths.GetPListPath (WatchAppType.Extension));
				result = result.Replace (RegisterTypeKey, GetRegisterTypeNode (paths.RegisterTypePath));
				result = result.Replace (ReferencesKey, sb.ToString ());
				result = result.Replace (ContentKey, contentFiles.ToString ());
				using (var file = new StreamWriter (paths.WatchOSProjectPaths.GetProjectPath (WatchAppType.Extension), false)) { // false is do not append
					await file.WriteAsync (result);
				}
			}
		}

		/// <summary>
		/// Removes all the generated files by the tool.
		/// </summary>
		public void CleanOutput ()
		{
			if (isCodeGeneration)
				throw new InvalidOperationException ("Project generator was instantiated to project generation.");
			// remove the generated code dir
			if (Directory.Exists (pathManager.GeneratedCodePathRoot))
				Directory.Delete (pathManager.GeneratedCodePathRoot, true);
		}

		/// <summary>
		/// Returns if all the test assemblies found in the mono path 
		/// </summary>
		/// <param name="missingAssemblies"></param>
		/// <returns></returns>
		public bool AllTestAssembliesAreRan (out Dictionary<Platform, List<string>> missingAssemblies, bool wasDownloaded)
		{
			missingAssemblies = new Dictionary<Platform, List<string>> ();
			foreach (var platform in new [] {Platform.iOS, Platform.TvOS, Platform.WatchOS}) {
				var testDir = wasDownloaded ? BCLTestAssemblyDefinition.GetTestDirectoryFromDownloadsPath (GetReleaseDownload (platform), platform)
					: BCLTestAssemblyDefinition.GetTestDirectoryFromMonoPath (MonoRootPath, platform);
				var missingAssembliesPlatform = Directory.GetFiles (testDir, NUnitPattern).Select (Path.GetFileName).Union (
					Directory.GetFiles (testDir, xUnitPattern).Select (Path.GetFileName)).ToList ();
				
				foreach (var assembly in CommonIgnoredAssemblies) {
					missingAssembliesPlatform.Remove (new BCLTestAssemblyDefinition (assembly).GetName (platform));
				}
				
				// loop over the mono root path and grab all the assemblies, then intersect the found ones with the added
				// and ignored ones.
				foreach (var projectDefinition in commoniOSTestProjects) {
					foreach (var testAssembly in projectDefinition.assemblies) {
						missingAssembliesPlatform.Remove (new BCLTestAssemblyDefinition (testAssembly).GetName (platform));
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
