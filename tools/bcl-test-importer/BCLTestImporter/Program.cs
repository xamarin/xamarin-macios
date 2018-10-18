using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

using Mono.Options;

namespace BCLTestImporter {
	class MainClass {

		static string NUnitPattern = "MONOTOUCH_*_test.dll"; 
		static string xUnitPattern = "MONOTOUCH_*_xunit-test.dll";

		// returns a list with all the paths of the test assemblies
		static List<string> GetTestAssemblies (string dir, bool isXUnit, bool verbose = false)
		{
			var assemblies = new List <string> ();
			var pattern = isXUnit ? xUnitPattern : NUnitPattern;

			if (verbose)
				Console.WriteLine ($"Searching for assemblies that match the patter '{pattern}'");

			foreach (var a in Directory.GetFiles(dir, pattern)) {
				var fullAssemblyPath = Path.Combine (dir, a);
				if (verbose)
					Console.WriteLine ($"Found assembly {fullAssemblyPath}");
				assemblies.Add (fullAssemblyPath);
			}
			return assemblies;
		}

		// returns a dict with a type found in each assembly so that we can add the reference later.
		static Dictionary <string, Type> GetTypeForAssemblies (List<string> assemblyPaths, bool verbose = false) {
			var oldColor = Console.ForegroundColor;
			var dict = new Dictionary <string, Type> (assemblyPaths.Count);
			// loop over the paths, grab the assembly, find a type and then add it
			foreach (var path in assemblyPaths) {
				if (verbose) 
					Console.WriteLine ($"Loading assembly from path {path}");
				var a = Assembly.LoadFile (path);
				try {
					var types = a.ExportedTypes;
					if (!types.Any ()) {
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						Console.WriteLine ($"WARNING: The following assembly had no types: '{path}'");
						Console.ForegroundColor = oldColor;
					}
					if (verbose)
						Console.WriteLine ($"Adding to dll {Path.GetFileName (path)} type {types.ElementAt(0).FullName}");
					dict[Path.GetFileName (path)] = types.First (t => !t.IsAbstract && !t.IsGenericType && t.FullName.Contains ("Test"));
				} catch (ReflectionTypeLoadException e) {
					// we did get an exception, possible reason, the type comes from an assebly not loaded, but 
					// nevertheless we can do something about it, get all the not null types in the exception
					// and use one of them
					var types = e.Types.Where (t => t != null).Where (t => !t.IsAbstract && !t.IsGenericType && t.FullName.Contains ("Test"));
					if (types.Any()) {
						dict[Path.GetFileName (path)] = types.First ();
					} else {
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						Console.WriteLine ($"WARNING: The following assembly had no types: '{path}'");
						Console.ForegroundColor = oldColor;
						continue;
					}
				}
			}
			return dict;
		}
		
		// return the required assemblies for the given app
		static IEnumerable<string> GetAssemblyReferences (string assemblyPath, bool verbose = false) {
			var result = new List<string> ();
			if (verbose)
				Console.WriteLine ($"Loading assembly from path {assemblyPath}");
			var a = Assembly.LoadFile (assemblyPath);
			return a.GetReferencedAssemblies ().Select ((arg) => arg.Name);
		}

		public static int Main (string [] args)
		{
			var appOptions = new ApplicationOptions ();
			var options = appOptions.CreateCommandLineOptionSet ();
			
			var extra = options.Parse (args);
			var console = new ConsoleWriter (appOptions);
			
			if (extra.Count > 0) {
				console.WriteWarning ($"The following extra parameters will be ignored: '{ string.Join (",", extra) }'");
			}
			// lets validate the options and return any possible error
			if (!appOptions.OptionsAreValid (out var validationMessage)) {
				console.WriteError (validationMessage);
				return 1;
			}

			if (appOptions.ShouldShowHelp) {
				options.WriteOptionDescriptions (Console.Out);
			} else if (!string.IsNullOrWhiteSpace (appOptions.Assembly)) {
				foreach (var a in GetAssemblyReferences (appOptions.Assembly)) {
					Console.WriteLine (a);
				}
			} else if (appOptions.ShowDict || appOptions.ListAssemblies) {
				// got the full path, we need to look for diff types of tests, since that changes the glob rule
				var assemblies = GetTestAssemblies (appOptions.TestsDirectory, appOptions.IsXUnit, appOptions.Verbose);
				if (assemblies.Count == 0) {
					Console.WriteLine ("No test assemblies have been found.");
					return 0;
				}
				
				console.WriteLine ("Using reflection to retrieve a type per assembly.");

				var typesPerAssembly = GetTypeForAssemblies (assemblies, appOptions.Verbose);
				if (appOptions.ShowDict) {
					Console.WriteLine ("The list of test assemblies are the types is:");
					foreach (var a in typesPerAssembly.Keys) {
						Console.WriteLine ($"\tAssembly '{a}' => Type '{typesPerAssembly[a].Namespace}.{typesPerAssembly[a].FullName}'");
					}
					return 0;
				}

				Console.WriteLine ("Found test assemblies are:");
				foreach (var a in typesPerAssembly.Keys) {
					Console.WriteLine ($"\t{a}");
				}
				return 0;

			} else if (appOptions.GenerateProject) {
				// we need to have at least a sigle assembly to use for the project generation
				// loop over the assemblies to get the full path.
				var fixedTestAssemblies = new List<string> ();
				foreach (var a in appOptions.TestAssemblies) {
					fixedTestAssemblies.Add (Path.Combine (appOptions.TestsDirectory, a));
				}
				// we need to know the assemblies that are used by the test assemblies, otherwise we cannot add them to the project
				// and we wont be able to load the diff tests
				var refAssemblies = new HashSet<string> ();
				foreach (var a in fixedTestAssemblies) {
					foreach (var refA in GetAssemblyReferences (a)) {
						refAssemblies.Add (refA);
					}
				}
				
				console.WriteLine ("Generating test projet:");
				console.WriteLine ($"\tProject name: {appOptions.ProjectName}");
				console.WriteLine ("\tTest Assemblies:");
				if (appOptions.Verbose)
					foreach (var a in appOptions.TestAssemblies) {
						Console.WriteLine ($"\t\t{a}");
					}
				console.WriteLine ("Reference Assemblies:");
				if (appOptions.Verbose)
					foreach (var a in refAssemblies) {
						Console.WriteLine ($"\t\t{a}");
					}
				
				// got the required info to generate the project, create the value types for the references
				var info = new List<(string assembly, string hintPath)> (); 
				foreach (var a in fixedTestAssemblies) {
					var assemblyInfo = (assembly: Path.GetFileName (a), hintPath: (string) a);
					info.Add (assemblyInfo);
					console.WriteLine ($"Ref will be added for assembly: '{assemblyInfo.assembly}' hintPath: '{assemblyInfo.hintPath}'");
				}
				foreach (var a in refAssemblies) {
					var assemblyInfo = (assembly: a, hintPath: (string) null);
					info.Add (assemblyInfo);
					console.WriteLine ($"Ref will be added for assembly: '{assemblyInfo.assembly}' hintPath: '{assemblyInfo.hintPath}'");
				}
				var generatedProject = BCLTestProjectGenerator.Generate (appOptions.ProjectName, appOptions.RegisterTypesPath, info, appOptions.RegisterTypeTemplate);
				console.WriteLine ("Generated project is:");
				console.WriteLine (generatedProject);
				
				using (var file = new StreamWriter (appOptions.Output, !appOptions.Override)) { // falso is do not append
					file.Write (generatedProject);
				}
				return 0;
			} else if (appOptions.GenerateTypeRegister) {
				console.WriteLine ("Generating type register.");
				var fixedTestAssemblies = new List<string> ();
				foreach (var a in appOptions.TestAssemblies) {
					var path = Path.Combine (appOptions.TestsDirectory, a);
					console.WriteLine ($"Assembly path is {path}");
					fixedTestAssemblies.Add (path);
				}
				var typesPerAssembly = GetTypeForAssemblies (fixedTestAssemblies, appOptions.Verbose);
				var generatedCode = RegisterTypeGenerator.GenerateCode (typesPerAssembly, appOptions.IsXUnit, appOptions.RegisterTypeTemplate);
				console.WriteLine ("Generated code is:");
				console.WriteLine (generatedCode);
				using (var file = new StreamWriter (appOptions.Output, !appOptions.Override)) { // falso is do not append
					file.Write (generatedCode);
				}
				return 0;
			} else if (appOptions.GenerateAllProjects) {
				if (appOptions.ClearAll) {
					var projectGenerator = new BCLTestProjectGenerator (appOptions.Output);
					projectGenerator.CleanOutput ();
					return 0;
				}
				else {
					var projectGenerator = new BCLTestProjectGenerator (appOptions.Output, appOptions.MonoPath,
						appOptions.ProjectTemplate, appOptions.RegisterTypeTemplate);
					console.WriteLine ("Verifying if all the test assemblies have been added.");
					if (!projectGenerator.AllTestAssembliesAreRan (out var missingAssemblies)) {
						console.WriteLine ("The following test assemblies should be added to a test project or ignored.");
						foreach (var assembly in missingAssemblies) {
							console.WriteLine ($"\t{assembly}");
						}

						return 1;
					}
						
					console.WriteLine ("Generating all the registered test projects");
					projectGenerator.GenerateAllTestProjects ().Wait ();
					return 0;
				}
			}
			else {
				return 1;
			}
			return 0;
		}
	}
}
