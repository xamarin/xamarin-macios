using System;
using System.IO;
using System.Collections.Generic;

namespace BCLTestImporter {

	// struct used to store the options of the command line after they have been parsed
	// it provides ways to validate that the combinations are correct.
	public class ApplicationOptions {

		static string partialPath = "mcs/class/lib";
		static Dictionary <string, string> platformPathMatch = new Dictionary <string, string> {
			{"iOS", "monotouch"},
			{"WatchOS", "monotouch_watch"},
			{"TvOS", "monotouch_tv"},
			{"MacOS", "xammac"},
		};

		// bool flags
		public bool ShouldShowHelp { get; set; }
		public bool Verbose { get; set; }
		public bool ShowDict { get; set; }
		public bool ListAssemblies { get; set; }
		public bool GenerateProject { get; set; }
		public bool GenerateTypeRegister { get; set; }
		public bool IsXUnit { get; set; }
		public bool Override { get; set; }

		// path options
		string monoPath;
		public string MonoPath { 
			get => monoPath;
			set {
				// ensure the path is abs etc..
				monoPath = FixPath (value);
			}
		}
		public string Platform { get; set; }
		string output;
		public string Output {
			get => output;
			set {
				output = FixPath (value);
			}
		}
		string template;
		public string Template {
			get => template;
			set {
				template = FixPath (value);
			}
		} 
		string assembly;
		public string Assembly {
			get => assembly;
			set {
				assembly = FixPath (assembly);
			}
		}
		string regiterTypesPath;
		public string RegiterTypesPath {
			get => regiterTypesPath;
			set {
				regiterTypesPath = FixPath (value);
			}
		}

		public string TestsDirectory {
			get {
				return GetTestsDirectoryPath (monoPath, Platform);
			}
		}

		// multiple value options
		public List<string> TestAssemblies { get; private set; }
		public string ProjectName { get; set; }

		public ApplicationOptions ()
		{
			TestAssemblies = new List<string> ();
		}

		// get a path, and make sure that is abs and that ~ is expanded
		public string FixPath (string path)
		{
			var result = path;
			if (!Path.IsPathRooted (result)) {
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
			}
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

		string GetTestsDirectoryPath (string monoPath, string platform)
		{
			var fullPath = monoPath;
			switch (platform) {
			case "iOS":
				fullPath = Path.Combine (fullPath, partialPath, platformPathMatch["iOS"], "tests");
			break;
			case "WatchOS":
				fullPath = Path.Combine (fullPath, partialPath, platformPathMatch["WatchOS"], "tests");
			break;
			case "TvOS":
				fullPath = Path.Combine (fullPath, partialPath, platformPathMatch["TvOS"], "tests");
			break;
			case "MacOS":
				fullPath = Path.Combine (fullPath, partialPath, platformPathMatch["MacOS"], "tests");
			break;
			default:
			fullPath = null;
			break;
			}
			return fullPath;
		}

		bool PlatformIsValid (string mPath, string platform, out string message)
		{
			// mono path should have been checked already
			if (string.IsNullOrEmpty (platform)) {
				message = "Platform must be provided.";
				return false;
			}
			if (!platformPathMatch.ContainsKey (platform)) {
				message = "Unrecognized platform.";
				return false;
			}
			var fullPath = GetTestsDirectoryPath (mPath, platform);

			if (!Directory.Exists (fullPath)) {
					message = $"Could not find path: '{fullPath}'";
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
					|| !string.IsNullOrEmpty (Platform) || !string.IsNullOrEmpty (monoPath) || !string.IsNullOrEmpty (output)
					|| !string.IsNullOrEmpty (template) || TestAssemblies.Count > 0) {
					message = "--assemblyref does not take any other arguments.";
					return false;
				}
			}
			if (ShowDict) {
				if (ShouldShowHelp || ListAssemblies || GenerateProject || GenerateTypeRegister || !string.IsNullOrEmpty (Platform)
					|| !string.IsNullOrEmpty (monoPath) || !string.IsNullOrEmpty (output) || !string.IsNullOrEmpty (assembly)
					|| !string.IsNullOrEmpty (template) || TestAssemblies.Count > 0) {
					message = "--d received unrecognized parameters..";
					return false;
				}
				if (string.IsNullOrEmpty (monoPath)) {
					message = "(-d) Mono checkout is missing.";
					return false;
				}
				if (string.IsNullOrEmpty (Platform)) {
					message = "(-d) Platform must be provided.";
					return false;
				}
				// assert that the platform is supported
				if (!platformPathMatch.ContainsKey (Platform)) {
					message = "(-d) Unrecognized platform.";
					return false;
				}
			}
			if (ListAssemblies) {
				if (!MonoPathIsValid (monoPath, out message)) {
					message = $"(-d) {message}"; // let the user the param he used
					return false;
				}
				if (!PlatformIsValid (monoPath, Platform, out message)) {
					message = $"(-d) {message}"; // let the user the param he used
					return false;
				}
			}
			if (!string.IsNullOrEmpty (output)) {
				// we are dealing with two possible options, the project generation or the type registration
				// generation, both need the mono path and the platform to be correct
				string cmd = "";
				if (GenerateProject)
					cmd = "--generate-project";
				if (GenerateTypeRegister) 
					cmd = "--generate-type-register";
				if (!MonoPathIsValid (monoPath, out message)) {
					message = $"{cmd} {message}"; // let the user the param he used
					return false;
				}
				if (!PlatformIsValid (monoPath, Platform, out message)) {
					message = $"{cmd} {message}"; // let the user the param he used
					return false;
				}
				if (string.IsNullOrEmpty (template)) {
					message = $"{cmd} Template must be provided.";
					return false;
				}
				if (!File.Exists (template)) {
					message = $"{cmd} Template is missing.";
					return false;
				}
				if (string.IsNullOrEmpty (output)) {
					message = $"{cmd} output path most be provided.";
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
				if (GenerateProject && string.IsNullOrEmpty (ProjectName)) {
					message = $"{cmd} a project name is needed when generating a new test app.";
					return false;
				}
				if (GenerateProject && string.IsNullOrEmpty (regiterTypesPath)) {
					message = $"{cmd} the path to the generated class is needed.";
					return false;
				}
				if (GenerateProject && !File.Exists (regiterTypesPath)) {
					message = $"{cmd} the path to the generated class could not be found.";
					return false;
				}
			}
			message = "";
			return true;
		}
	}
}
