using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BCLTestImporter {
	/// <summary>
	/// Class that knows how to generate .csproj files based on a BCLTestProjectDefinition.
	/// </summary>
	public class BCLTestProjectGenerator {

		static string NUnitPattern = "MONOTOUCH_*_test.dll"; 
		static string xUnitPattern = "MONOTOUCH_*_xunit-test.dll";
		static readonly string NameKey = "%NAME%";
		static readonly string ReferencesKey = "%REFERENCES%";
		static readonly string RegisterTypeKey = "%REGISTER TYPE%";
		static readonly string PlistKey = "%PLIST PATH%";

		//list of reference that we are already adding, and we do not want to readd (although it is just a warning)
		static readonly List<string> excludeDlls = new List<string> {
			"mscorlib",
			"nunitlite",
			"System",
			"System.Xml",
			"System.Xml.Linq",
		};

		// this can be grouped TODO
		static readonly List <(string name, string[] assemblies)> iOSTestProjects = new List <(string name, string [] assemblies)> {
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

		readonly bool isCodeGeneration;
		public bool Override { get; set; }
		public string OutputDirectoryPath { get; private  set; }
		public string MonoRootPath { get; private set; }
		public string ProjectTemplatePath { get; private set; }
		public string PlistTemplatePath{ get; private set; }
		public string RegisterTypesTemplatePath { get; private set; }
		string GeneratedCodePathRoot => Path.Combine (OutputDirectoryPath, "generated");

		public BCLTestProjectGenerator (string outputDirectory)
		{
			OutputDirectoryPath = outputDirectory ?? throw new ArgumentNullException (nameof (outputDirectory));
		}
		
		public BCLTestProjectGenerator (string outputDirectory, string monoRootPath, string projectTemplatePath, string registerTypesTemplatePath, string plistTemplatePath)
		{
			isCodeGeneration = true;
			OutputDirectoryPath = outputDirectory ?? throw new ArgumentNullException (nameof (outputDirectory));
			MonoRootPath = monoRootPath ?? throw new ArgumentNullException (nameof (monoRootPath));
			ProjectTemplatePath = projectTemplatePath ?? throw new ArgumentNullException (nameof (projectTemplatePath));
			PlistTemplatePath = plistTemplatePath ?? throw new ArgumentNullException (nameof (plistTemplatePath));
			RegisterTypesTemplatePath = registerTypesTemplatePath ?? throw new ArgumentNullException (nameof (registerTypesTemplatePath));
		}

		string GetProjectPath (string projectName) => Path.Combine (OutputDirectoryPath, $"{projectName}.csproj");
		
		// creates the reference node
		static string GetReferenceNode (string assemblyName, string hintPath = null)
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

		static string GetRegisterTypeNode (string registerPath)
		{
			var sb = new StringBuilder ();
			sb.AppendLine ($"<Compile Include=\"{registerPath}\">");
			sb.AppendLine ($"<Link>{Path.GetFileName (registerPath)}</Link>");
			sb.AppendLine ("</Compile>");
			return sb.ToString ();
		}

		// creates all the projects that have already been defined
		public async Task<List<(string name, string path, bool xunit)>> GenerateAllTestProjectsAsync ()
		{
			var projectPaths = new List<(string name, string path, bool xunit)> ();
			if (!isCodeGeneration)
				throw new InvalidOperationException ("Project generator was instantiated to delete the generated code.");
			// TODO: Do this per platform
			var platform = "iOS";
			var generatedCodePathRoot = GeneratedCodePathRoot;
			if (!Directory.Exists (generatedCodePathRoot)) {
				Directory.CreateDirectory (generatedCodePathRoot);
			}

			foreach (var def in iOSTestProjects) {
				var projectDefinition = new BCLTestProjectDefinition (def.name, def.assemblies);
				if (!projectDefinition.Validate ())
					throw new InvalidOperationException ("xUnit and NUnit assemblies cannot be mixed in a test project.");
				// generate the required type registration info
				var generatedCodeDir = Path.Combine (generatedCodePathRoot, projectDefinition.Name);
				if (!Directory.Exists (generatedCodeDir)) {
					Directory.CreateDirectory (generatedCodeDir);
				}

				var typesPerAssembly = projectDefinition.GetTypeForAssemblies (MonoRootPath, "iOS");
				var registerCode = await RegisterTypeGenerator.GenerateCodeAsync (typesPerAssembly,
					projectDefinition.IsXUnit, RegisterTypesTemplatePath);

				var registerTypePath = Path.Combine (generatedCodeDir, "RegisterType.cs");
				using (var file = new StreamWriter (registerTypePath, !Override)) { // false is do not append
					await file.WriteAsync (registerCode);
				}

				var plist = await BCLTestInfoPlistGenerator.GenerateCodeAsync (PlistTemplatePath,
					projectDefinition.Name);
				var infoPlistPath = Path.Combine (generatedCodeDir, "Info.plist");
				using (var file = new StreamWriter (infoPlistPath, !Override)) { // false is do not append
					await file.WriteAsync (plist);
				}

				var generatedProject = await GenerateAsync (projectDefinition.Name, registerTypePath,
					projectDefinition.GetAssemblyInclusionInformation (MonoRootPath, platform), ProjectTemplatePath, infoPlistPath);
				var projectPath = GetProjectPath (projectDefinition.Name);
				projectPaths.Add ((name: projectDefinition.Name, path: projectPath, xunit: projectDefinition.IsXUnit));
				using (var file = new StreamWriter (projectPath, !Override)) { // false is do not append
					await file.WriteAsync (generatedProject);
				}
			}

			return projectPaths;
		}

		public List<(string name, string path, bool xunit)> GenerateAllTestProjects () => GenerateAllTestProjectsAsync ().Result;
		
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
			// delete each of the generated project files
			foreach (var projectDefinition in iOSTestProjects) {
				var projectPath = GetProjectPath (projectDefinition.name);
				if (File.Exists (projectPath))
					File.Delete (projectPath);
			}	
		}

		/// <summary>
		/// Returns if all the test assemblies found in the mono path 
		/// </summary>
		/// <param name="missingAssemblies"></param>
		/// <returns></returns>
		public bool AllTestAssembliesAreRan (out List<string> missingAssemblies)
		{
			// TODO: do this for all platforms
			var platform = "iOS";
			
			// loop over the mono root path and grab all the assemblies, then intersect the found ones with the added
			// and ignored ones.
			var testDir = BCLTestAssemblyDefinition.GetTestDirectory (MonoRootPath, platform);

			// get all the present assemblies
			missingAssemblies = Directory.GetFiles (testDir, NUnitPattern).Select (Path.GetFileName).Union (
				Directory.GetFiles (testDir, xUnitPattern).Select (Path.GetFileName)).ToList ();
			
			// remove the ignored ones
			foreach (var assembly in CommonIgnoredAssemblies) {
				missingAssemblies.Remove (assembly);
			}
			
			// remove the added ones
			foreach (var projectDefinition in iOSTestProjects) {
				foreach (var testAssembly in projectDefinition.assemblies) {
					missingAssemblies.Remove (testAssembly);
				}
			}	

			return missingAssemblies.Count == 0;
		}
	}
}
