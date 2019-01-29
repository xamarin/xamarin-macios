using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BCLTestImporter {
	/// <summary>
	/// Class that defines a bcl test project. A bcl test project by definition is the combination of the name
	/// of the project and a set on assemblies to be tested.
	/// </summary>
	public partial struct BCLTestProjectDefinition {
		public string Name { get; set; }
		public List<BCLTestAssemblyDefinition> TestAssemblies {get; private set;}
		public bool IsXUnit {
			get {
				if (TestAssemblies.Count > 0)
					return TestAssemblies [0].IsXUnit;
				return false;
			}
		}

		public BCLTestProjectDefinition (string name, string[] assemblies)
		{
			if (assemblies.Length == 0)
				throw new ArgumentException ("Most provide at least an assembly.");
			Name = name;
			TestAssemblies = new List<BCLTestAssemblyDefinition> (assemblies.Length);
			foreach (var assembly in assemblies) {
				TestAssemblies.Add (new BCLTestAssemblyDefinition (assembly));
			}
		}
		
		public BCLTestProjectDefinition (string name, List<BCLTestAssemblyDefinition> assemblies)
		{
			Name = name;
			TestAssemblies = assemblies;
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
			var xUnitAssemblies = new List<BCLTestAssemblyDefinition> ();
			var nUnitAssemblies = new List<BCLTestAssemblyDefinition> ();
			
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
		(string FailureMessage, IEnumerable<string> References) GetProjectAssemblyReferences (string rootPath, Platform platform, bool wasDownloaded)
		{
			var set = new HashSet<string> ();
			string failureMessage = null;
			foreach (var definition in TestAssemblies) {
				var references = GetAssemblyReferences (definition.GetPath (rootPath, platform, wasDownloaded));
				if (references.FailureMessage != null) {
					failureMessage = references.FailureMessage;
				} else {
					set.UnionWith (references.References);
				}
			}
			return (failureMessage, set);
		}
		
		public (string FailureMessage, Dictionary <string, Type> Types) GetTypeForAssemblies (string monoRootPath, Platform platform, bool wasDownloaded) {
			if (monoRootPath == null)
				throw new ArgumentNullException (nameof (monoRootPath));
			var dict = new Dictionary <string, Type> ();
			// loop over the paths, grab the assembly, find a type and then add it
			foreach (var definition in TestAssemblies) {
				var path = definition.GetPath (monoRootPath, platform, wasDownloaded);
				if (!File.Exists (path))
					return ($"The assembly {path} does not exist. Please make sure it exists, then re-generate the project files by executing 'git clean -xfd && make' in the tests/ directory.", null);
				var a = Assembly.LoadFile (path);
				try {
					var types = a.ExportedTypes;
					if (!types.Any ()) {
						continue;
					}
					dict[Path.GetFileName (path)] = types.First (t => !t.IsGenericType && t.FullName.Contains ("Test"));
				} catch (ReflectionTypeLoadException e) { // ReflectionTypeLoadException
					// we did get an exception, possible reason, the type comes from an assebly not loaded, but 
					// nevertheless we can do something about it, get all the not null types in the exception
					// and use one of them
					var types = e.Types.Where (t => t != null).Where (t => !t.IsGenericType && t.FullName.Contains ("Test"));
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
		/// <param name="monoRootPath">The root path of the mono checkout.</param>
		/// <param name="platform">The platform we are working with.</param>
		/// <returns>The list of tuples (assembly name, path hint) for all the assemblies in the project.</returns>
		public (string FailureMessage, List<(string assembly, string hintPath)> Assemblies) GetAssemblyInclusionInformation (string monoRootPath,
			Platform platform, bool wasDownloaded)
		{
			if (monoRootPath == null)
				throw new ArgumentNullException (nameof (monoRootPath));

			var references = GetProjectAssemblyReferences (monoRootPath, platform, wasDownloaded);
			if (!string.IsNullOrEmpty (references.FailureMessage))
				return (references.FailureMessage, null);
			var asm = references.References.Select (
					a => (assembly: a, 
						hintPath: BCLTestAssemblyDefinition.GetHintPathForRefenreceAssembly (a, monoRootPath, platform))).Union (
					TestAssemblies.Select (
						definition => (assembly: definition.Name,
							hintPath: definition.GetPath (monoRootPath, platform, wasDownloaded))))
				.ToList ();

			return (null, asm);
		}

	}
}
