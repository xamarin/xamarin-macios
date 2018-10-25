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
	public struct BCLTestProjectDefinition {
		public string Name { get; set; }
		public List<BCLTestAssemblyDefinition> TestAssemblies {get; private set;}
		public bool IsXUnit {
			get {
				if (TestAssemblies.Count > 0)
					return TestAssemblies [0].IsXUnit;
				return false;
			}
		}

		public BCLTestProjectDefinition (string name, List<BCLTestAssemblyDefinition> assemblies)
		{
			Name = name;
			TestAssemblies = assemblies;
		}
		
		static IEnumerable<string> GetAssemblyReferences (string assemblyPath) {
			var a = Assembly.LoadFile (assemblyPath);
			return a.GetReferencedAssemblies ().Select ((arg) => arg.Name);
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
		IEnumerable<string> GetProjectAssemblyReferences (string monoRootPath, string platform)
		{
			var set = new HashSet<string> ();
			foreach (var definition in TestAssemblies) {
				foreach (var reference in GetAssemblyReferences (definition.GetPath (monoRootPath, platform))) {
					set.Add (reference);
				}
			}
			return set;
		}
		
		public Dictionary <string, Type> GetTypeForAssemblies (string monoRootPath, string platform) {
			if (monoRootPath == null)
				throw new ArgumentNullException (nameof (monoRootPath));
			if (platform == null)
				throw new ArgumentNullException (nameof (platform));
			var dict = new Dictionary <string, Type> ();
			// loop over the paths, grab the assembly, find a type and then add it
			foreach (var definition in TestAssemblies) {
				var path = definition.GetPath (monoRootPath, platform);
				var a = Assembly.LoadFile (path);
				try {
					var types = a.ExportedTypes;
					if (!types.Any ()) {
						continue;
					}
					dict[Path.GetFileName (path)] = types.First (t => !t.IsAbstract && !t.IsGenericType && t.FullName.Contains ("Test"));
				} catch (ReflectionTypeLoadException e) { // ReflectionTypeLoadException
					// we did get an exception, possible reason, the type comes from an assebly not loaded, but 
					// nevertheless we can do something about it, get all the not null types in the exception
					// and use one of them
					var types = e.Types.Where (t => t != null).Where (t => !t.IsAbstract && !t.IsGenericType && t.FullName.Contains ("Test"));
					if (types.Any()) {
						dict[Path.GetFileName (path)] = types.First ();
					}
				}
			}
			return dict;
		}

		/// <summary>
		/// Returns a list of tuples that contains the name of the assembly and the required hint path. If the
		/// path is null it means that the assembly is part of the distribution.
		/// </summary>
		/// <param name="monoRootPath">The root path of the mono checkout.</param>
		/// <param name="platform">The platform we are working with.</param>
		/// <returns>The list of tuples (assembly name, path hint) for all the assemblies in the project.</returns>
		public List<(string assembly, string hintPath)> GetAssemblyInclusionInformation (string monoRootPath,
			string platform)
		{
			if (monoRootPath == null)
				throw new ArgumentNullException (nameof (monoRootPath));
			if (platform == null)
				throw new ArgumentNullException (nameof (platform));
			
			return GetProjectAssemblyReferences (monoRootPath, platform).Select (
					a => (assembly: a, hintPath: (string) null)).Union (
					TestAssemblies.Select (
						definition => (assembly: definition.Name,
							hintPath: definition.GetPath (monoRootPath, platform))))
				.ToList ();
		}

	}
}
