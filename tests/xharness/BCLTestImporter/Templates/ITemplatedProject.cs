using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xharness.BCLTestImporter.Templates {
	// interface that represent a project that is created from a template.
	// The interface should be able to generate a project that will later be
	// used by the AppRunner to execute tests.
	public interface ITemplatedProject {
		string OutputDirectoryPath { get; set; }
		string IgnoreFilesRootDirectory { get; set; }

		IAssemblyLocator AssemblyLocator { get; set; }
		IProjectFilter ProjectFilter { get; set; }
		public Func<string, Guid> GuidGenerator { get; set; }

		/// <summary>
		/// Generates all the project files for the given projects and platform
		/// </summary>
		/// <param name="projects">The list of projects to be generated.</param>
		/// <param name="platform">The platform to which the projects have to be generated. Each platform
		/// has its own details.</param>
		/// <param name="generatedDir">The dir where the projects will be saved.</param>
		/// <returns></returns>
		Task<List<BclTestProject>> GenerateTestProjectsAsync (IEnumerable<BclTestProjectInfo> projects, Platform platform);
	}
}
