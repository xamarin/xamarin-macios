using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Xharness.TestImporter {
	/// <summary>
	/// Class that defines a bcl test project. A bcl test project by definition is the combination of the name
	/// of the project and a set on assemblies to be tested.
	/// </summary>
	public class BCLTestProjectDefinition {
		public string Name { get; set; }
		public string ExtraArgs { get; private set; }
		public IAssemblyLocator AssemblyLocator { get; set; }
		public ITestAssemblyDefinitionFactory AssemblyDefinitionFactory { get; set; }
		public List<ITestAssemblyDefinition> TestAssemblies {get; private set; }

		public bool IsXUnit {
			get {
				if (TestAssemblies.Count > 0)
					return TestAssemblies [0].IsXUnit;
				return false;
			}
		}

		public BCLTestProjectDefinition (string name, IAssemblyLocator locator, ITestAssemblyDefinitionFactory factory, string [] assemblies, string extraArgs)
		{
			if (assemblies.Length == 0)
				throw new ArgumentException ("Most provide at least an assembly.");

			Name = name ?? throw new ArgumentNullException (nameof (name));
			TestAssemblies = new List<ITestAssemblyDefinition> (assemblies.Length);
			AssemblyLocator = locator ?? throw new ArgumentNullException (nameof (locator));
			AssemblyDefinitionFactory = factory ?? throw new ArgumentNullException (nameof (factory));
			ExtraArgs = extraArgs;
			foreach (var assembly in assemblies) {
				TestAssemblies.Add (factory.Create (assembly, AssemblyLocator));
			}
		}
		
		public BCLTestProjectDefinition (string name, IAssemblyLocator locator, ITestAssemblyDefinitionFactory factory, List<ITestAssemblyDefinition> assemblies, string extraArgs)
		{
			Name = name ?? throw new ArgumentNullException (nameof (locator));
			AssemblyLocator = locator ?? throw new ArgumentNullException (nameof (locator));
			TestAssemblies = assemblies ?? throw new ArgumentNullException (nameof (assemblies));
			AssemblyDefinitionFactory = factory ?? throw new ArgumentNullException (nameof (factory));
			foreach (var a in TestAssemblies) {
				a.AssemblyLocator = AssemblyLocator;
			}
			ExtraArgs = extraArgs;
		}
		
		static (string FailureMessage, IEnumerable<string> References) GetAssemblyReferences (string assemblyPath) {
			if (!File.Exists (assemblyPath))
				return ($"The file {assemblyPath} does not exist.", null);
			var a = Assembly.LoadFile (assemblyPath);
			return (null, a.GetReferencedAssemblies ().Select ((arg) => arg.Name));
		}

		/// <summary>
		/// Ensures that the project is correctly defined and does not mix NUnit and xUnit.
		/// </summary>
		/// <returns></returns>
		public bool Validate ()
		{
			// what a lame way to test this!
			var xUnitAssemblies = new List<ITestAssemblyDefinition> ();
			var nUnitAssemblies = new List<ITestAssemblyDefinition> ();
			
			foreach (var assemblyDefinition in TestAssemblies) {
				if (assemblyDefinition.IsXUnit)
					xUnitAssemblies.Add (assemblyDefinition);
				else
					nUnitAssemblies.Add (assemblyDefinition);
			}
			return TestAssemblies.Count == xUnitAssemblies.Count || TestAssemblies.Count == nUnitAssemblies.Count;
		}

		/// <summary>
		/// Returns the assemblies that a referenced by the given test assembly.
		/// </summary>
		/// <returns></returns>
		(string FailureMessage, IEnumerable<string> References) GetProjectAssemblyReferences (string rootPath, Platform platform)
		{
			var set = new HashSet<string> ();
			string failureMessage = null;
			foreach (var definition in TestAssemblies) {
				var references = GetAssemblyReferences (definition.GetPath (platform));
				if (references.FailureMessage != null) {
					failureMessage = references.FailureMessage;
				} else {
					set.UnionWith (references.References);
				}
			}
			return (failureMessage, set);
		}
		
		public (string FailureMessage, Dictionary <string, Type> Types) GetTypeForAssemblies (string monoRootPath, Platform platform) {
			if (monoRootPath == null)
				throw new ArgumentNullException (nameof (monoRootPath));
			var dict = new Dictionary <string, Type> ();
			// loop over the paths, grab the assembly, find a type and then add it
			foreach (var definition in TestAssemblies) {
				var path = definition.GetPath (platform);
				if (!File.Exists (path))
					return ($"The assembly {path} does not exist. Please make sure it exists, then re-generate the project files by executing 'git clean -xfd && make' in the tests/ directory.", null);
				var a = Assembly.LoadFile (path);
				try {
					var types = a.ExportedTypes;
					if (!types.Any ()) {
						continue;
					}
					dict[Path.GetFileName (path)] = types.First (t => !t.IsGenericType && (t.FullName.EndsWith ("Test") || t.FullName.EndsWith ("Tests")) && t.Namespace != null);
				} catch (ReflectionTypeLoadException e) { // ReflectionTypeLoadException
					// we did get an exception, possible reason, the type comes from an assebly not loaded, but 
					// nevertheless we can do something about it, get all the not null types in the exception
					// and use one of them
					var types = e.Types.Where (t => t != null).Where (t => !t.IsGenericType && (t.FullName.EndsWith ("Test") || t.FullName.EndsWith ("Tests")) && t.Namespace != null);
					if (types.Any()) {
						dict[Path.GetFileName (path)] = types.First ();
					}
				}
			}
			return (null, dict);
		}

		/// <summary>
		/// Returns a list of tuples that contains the name of the assembly and the required hint path. If the
		/// path is null it means that the assembly is part of the distribution.
		/// </summary>
		/// <param name="platform">The platform we are working with.</param>
		/// <returns>The list of tuples (assembly name, path hint) for all the assemblies in the project.</returns>
		public (string FailureMessage, List<(string assembly, string hintPath)> Assemblies) GetAssemblyInclusionInformation (Platform platform)
		{
			var references = GetProjectAssemblyReferences (AssemblyLocator.GetAssembliesRootLocation (platform), platform);
			if (!string.IsNullOrEmpty (references.FailureMessage))
				return (references.FailureMessage, null);
			var asm = references.References.Select (
					a => (assembly: a, 
						hintPath: AssemblyLocator.GetHintPathForReferenceAssembly (a, platform))).Union (
					TestAssemblies.Select (
						definition => (assembly: definition.GetName (platform),
							hintPath: definition.GetPath (platform))))
				.ToList ();

			return (null, asm);
		}

	}
}
