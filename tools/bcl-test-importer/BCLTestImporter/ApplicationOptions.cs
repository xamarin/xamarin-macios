using System;
using System.IO;
using System.Collections.Generic;

using Mono.Options;

namespace BCLTestImporter {

	// struct used to store the options of the command line after they have been parsed
	// it provides ways to validate that the combinations are correct.
	public class ApplicationOptions {

		#region static vars
		static readonly string partialPath = "mcs/class/lib";
		static readonly Dictionary <Platform, string> PlatformPathMatch = new Dictionary <Platform, string> {
			{Platform.iOS, "monotouch"},
			{Platform.WatchOS, "monotouch_watch"},
			{Platform.TvOS, "monotouch_tv"},
			{Platform.MacOS, "xammac"},
		};
		
		#endregion

		#region properties 
		
		// bool flags
		internal bool ShouldShowHelp { get; set; }
		internal bool Verbose { get; set; }
		internal bool ShowDict { get; set; }
		internal bool ListAssemblies { get; set; }
		internal bool GenerateProject { get; set; }
		internal bool GenerateAllProjects { get; set; }
		internal bool GenerateTypeRegister { get; set; }
		internal bool IsXUnit { get; set; }
		internal bool Override { get; set; }
		internal bool ClearAll{ get; set; }

		// path options
		string monoPath;
		public string MonoPath { 
			get => monoPath;
			set => monoPath = FixPath (value);
		}
		public Platform Platform { get; set; }
		string output;
		public string Output {
			get => output;
			set => output = FixPath (value);
		}
		string registerTypeTemplate;
		public string RegisterTypeTemplate {
			get => registerTypeTemplate;
			set => registerTypeTemplate = FixPath (value);
		} 
		string projectTemplate;
		public string ProjectTemplate {
			get => projectTemplate;
			set => projectTemplate = FixPath (value);
		} 
		string assembly;
		public string Assembly {
			get => assembly;
			set => assembly = FixPath (value);
		}
		string registerTypesPath;
		public string RegisterTypesPath {
			get => registerTypesPath;
			set => registerTypesPath = FixPath (value);
		}

		public string TestsDirectory => GetTestsDirectoryPath (monoPath, Platform);

		// multiple value options
		public List<string> TestAssemblies { get; private set; }
		public string ProjectName { get; set; }
		
		#endregion

		public ApplicationOptions ()
		{
			TestAssemblies = new List<string> ();
		}

		// get a path, and make sure that is abs and that ~ is expanded
		string FixPath (string path)
		{
			var result = path;
			if (Path.IsPathRooted (result))
				return result;
			if (result.StartsWith ("~", StringComparison.Ordinal)) {
				if (Verbose)
					Console.WriteLine ($"Expanding home dir for path {result}.");
				result = result.Replace ("~", Environment.GetEnvironmentVariable("HOME"));
			}

			if (Verbose)
				Console.WriteLine ($"Converting path {result} to absolute.");
			result = Path.GetFullPath (result);
			if (Verbose)
				Console.WriteLine ($"New output path is {result}");
			return result;
		}

		bool MonoPathIsValid (string path, out string message)
		{
			if (string.IsNullOrEmpty (path)) {
				message = "Mono checkout is missing.";
				return false;
			}
			if (!Directory.Exists (monoPath)) {
				message = $"Could not find mono checkout: '{path}'";
				return false;
			}
			message = "";
			return true;
		}

		static string GetTestsDirectoryPath (string monoPath, Platform platform) 
			=> Path.Combine (monoPath, partialPath, PlatformPathMatch[platform], "tests");

		bool GenerateTypeRegisterOptionsAreValid (out string message)
		{
			var cmd = "--generate-type-register";
			if (!MonoPathIsValid (monoPath, out message)) {
				message = $"{cmd} {message}"; // let the user the param he used
				return false;
			}
			if (string.IsNullOrEmpty (registerTypeTemplate)) {
				message = $"{cmd} Template must be provided.";
				return false;
			}

			if (!File.Exists (registerTypeTemplate)) {
				message = $"{cmd} Template is missing.";
				return false;
			}

			if (string.IsNullOrEmpty (output)) {
				message = $"{cmd} output path must be provided.";
				return false;
			}

			if (!Override && File.Exists (output)) {
				message = $"{cmd} Output path already exists.";
				return false;
			}
			
			// we need other data depending on what is being generated
			if (TestAssemblies.Count == 0) {
				message = $"{cmd} test assemblies must be passed for code generation.";
				return false;
			}
			message = "";
			return true;
		}

		bool GenerateProjectOptionsAreValid (out string message)
		{
			// we are dealing with two possible options, the project generation or the type registration
			// generation, both need the mono path and the platform to be correct
			var cmd = "--generate-project";
			if (!MonoPathIsValid (monoPath, out message)) {
				message = $"{cmd} {message}"; // let the user the param he used
				return false;
			}
			if (string.IsNullOrEmpty (projectTemplate)) {
				message = $"{cmd} Template must be provided.";
				return false;
			}

			if (!File.Exists (projectTemplate)) {
				message = $"{cmd} Template is missing.";
				return false;
			}
			if (string.IsNullOrEmpty (output)) {
				message = $"{cmd} output path must be provided.";
				return false;
			}

			if (!Override && File.Exists (output)) {
				message = $"{cmd} Output path already exists.";
				return false;
			}
			
			// we need other data depending on what is being generated
			if (TestAssemblies.Count == 0) {
				message = $"{cmd} test assemblies must be passed for project generation.";
				return false;
			}
			if (string.IsNullOrEmpty (ProjectName)) {
				message = $"{cmd} a project name is needed when generating a new test app.";
				return false;
			}
			if (string.IsNullOrEmpty (registerTypesPath)) {
				message = $"{cmd} the path to the generated class is needed.";
				return false;
			}
			if (!File.Exists (registerTypesPath)) {
				message = $"{cmd} the path to the generated class could not be found.";
				return false;
			}
			message = "";
			return false;
		}

		bool GenerateAllProjectsOptionsAreValid (out string message)
		{
			const string cmd = "--generate-all-projects";
			if (ClearAll) { // special case in which we only need the output
				if (string.IsNullOrEmpty (output)) {
					message = $"{cmd} output path must be provided.";
					return false;
				}
				if (!Directory.Exists (output)) {
					message = $"{cmd} Output path must be an existing directory.";
					return false;
				}

				message = "";
				return true;
			}
			if (!MonoPathIsValid (monoPath, out message)) {
				message = $"{cmd} {message}"; // let the user the param he used
				return false;
			}
			if (string.IsNullOrEmpty (registerTypeTemplate)) {
				message = $"{cmd} Register type template must be provided.";
				return false;
			}

			if (string.IsNullOrEmpty (projectTemplate)) {
				message = $"{cmd} Project template must be provided.";
				return false;
			}
			if (!File.Exists (registerTypeTemplate)) {
				message = $"{cmd} Template is missing.";
				return false;
			}

			if (!File.Exists (projectTemplate)) {
				message = $"{cmd} Template is missing.";
				return false;
			}
			if (string.IsNullOrEmpty (output)) {
				message = $"{cmd} output path must be provided.";
				return false;
			}

			if (!Directory.Exists (output)) {
				message = $"{cmd} Output path must be an existing directory.";
				return false;
			}
			message = "";
			return true;
		}
		
		// validate the options and return a message with the possible issue.
		public bool OptionsAreValid (out string message)
		{
			if (!string.IsNullOrEmpty (assembly)) {
				// we need to ensure that no other options have been given
				if (ShouldShowHelp || ShowDict || ListAssemblies || GenerateProject || GenerateTypeRegister || IsXUnit
					|| !string.IsNullOrEmpty (monoPath) || !string.IsNullOrEmpty (output)
					|| !string.IsNullOrEmpty (registerTypeTemplate) || TestAssemblies.Count > 0) {
					message = "--assemblyref does not take any other arguments.";
					return false;
				}
			}
			if (ShowDict) {
				if (ShouldShowHelp || ListAssemblies || GenerateProject || GenerateTypeRegister 
				    || !string.IsNullOrEmpty (monoPath) || !string.IsNullOrEmpty (output)
				    || !string.IsNullOrEmpty (assembly) || !string.IsNullOrEmpty (registerTypeTemplate)
				    || TestAssemblies.Count > 0) {
					message = "--d received unrecognized parameters..";
					return false;
				}
				if (string.IsNullOrEmpty (monoPath)) {
					message = "(-d) Mono checkout is missing.";
					return false;
				}
				// assert that the platform is supported
				if (!PlatformPathMatch.ContainsKey (Platform)) {
					message = "(-d) Unrecognized platform.";
					return false;
				}
			}
			if (ListAssemblies) {
				if (!MonoPathIsValid (monoPath, out message)) {
					message = $"(-d) {message}"; // let the user the param he used
					return false;
				}
			}

			if (GenerateTypeRegister)
				return GenerateTypeRegisterOptionsAreValid (out message);
			if (GenerateProject)
				return GenerateProjectOptionsAreValid (out message);
			if (GenerateAllProjects)
				return GenerateAllProjectsOptionsAreValid (out message);
			message = "";
			return true;
		}

		/// <summary>
		/// Returns the option set to parse the cmd line of the application.
		/// </summary>
		/// <returns>A configured option set for the command line.</returns>
		public OptionSet CreateCommandLineOptionSet ()
		{
			return new OptionSet { 
				{ "mono-root=", "Root directory of the mono check out that will be use to search for the unit tests."
					+ " Example: '/Users/test/xamarin-macios/external/mono'.", p => MonoPath = p }, 
				{ "iOS", "Specifies that the platform to which the projects are going to be build is iOS.", i => Platform = Platform.iOS },
				{ "watchOS", "Specifies that the platform to which the projects are going to be build is watchOS.", w => Platform = Platform.WatchOS },
				{ "tvOS", "Specifies that the platform to which the projects are going to be build is tvOS.", t => Platform = Platform.TvOS },
				{ "macOS", "Specifies that the platform to which the projects are going to be build is macOS.", m => Platform = Platform.MacOS },
				{ "generate-project", "Flag used to state that a new test project should be generated."
					+ " Output path must be provided via -o.", f => GenerateProject = f != null },
				{ "generate-all-projects", "Flag used to state that all the test projects should be generated."
					+ " Output path must be provided via -o which should be a directory.", f => GenerateAllProjects = f != null },
				{ "clean", "Clear the output directory when used with the --generate-all-projects options.", c =>  ClearAll = c != null},
				{ "generate-type-register", "Flag used to state that a new type register should be generated."
					+ "Output path must be provided via -o." , f => GenerateTypeRegister = f != null },
				{ "override", "State if the output should be overriden or not.", o => Override = o != null},
				{ "o|output=", "Specifies the output of the generated code.", o => Output = o },
				{ "register-type-template=", "Specifies the template to be used for the code generation.", t => RegisterTypeTemplate = t},
				{ "project-template=", "Specifies the template to be used for the project generation.", t => ProjectTemplate = t},
				{ "assemblyref=", "Gets an assembly and returns all the references to it.", a => Assembly = a },
				{ "a|assembly=", "Allows to pass the test assemblies to be used for the code generation", a => TestAssemblies.Add (a) },
				{ "x|xunit", "Flag that states if the assemblies contain xunit tests.", x => IsXUnit = x != null},
				{ "d|dictionary", "Lists the assemblies found and a relation with a type found in it.", d => ShowDict = d != null},
				{ "l|list", "Lists the assemblies found.", l => ListAssemblies = l != null},
				{ "project-name=", "Allows to specify the name of the project to be generated.", p => ProjectName = p},
				{ "register-types-path=", "Allows to specify the location of the RegisterTypes generated class.", p => RegisterTypesPath = p},
				{ "h|?|help", "Show this message and exit", h => ShouldShowHelp = h != null },
				{ "v|verbose", "Be verbose when searching for the tests.", v => Verbose = v != null},
			};
		}
	}
}
