using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xharness.TestImporter.Templates {

	// less typing
	public class GeneratedProjects : List<(string Name, string Path, bool XUnit, string ExtraArgs, string Failure, double TimeoutMultiplier)> {
	}

	// interface that represent a project that is created from a template.
	// The interface should be able to generate a project that will later be
	// used by the AppRunner to execute tests.
	public interface ITemplatedProject {
		string OutputDirectoryPath { get; set; }
		string IgnoreFilesRootDirectory { get; set; }

		IAssemblyLocator AssemblyLocator { get; set; }
		IProjectFilter ProjectFilter { get; set; }
		ITestAssemblyDefinitionFactory AssemblyDefinitionFactory { get; set; }
		public Func<string, Guid> GuidGenerator { get; set; }

		/// <summary>
		/// Generates all the project files for the given projects and platform
		/// </summary>
		/// <param name="projects">The list of projects to be generated.</param>
		/// <param name="platform">The platform to which the projects have to be generated. Each platform
		/// has its own details.</param>
		/// <param name="generatedDir">The dir where the projects will be saved.</param>
		/// <returns></returns>
		Task<GeneratedProjects> GenerateTestProjectsAsync (IEnumerable<(string Name, string [] Assemblies, string ExtraArgs, double TimeoutMultiplier)> projects, Platform platform);
	}
}
