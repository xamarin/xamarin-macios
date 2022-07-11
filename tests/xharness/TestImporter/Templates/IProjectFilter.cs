using System.Collections.Generic;

namespace Xharness.TestImporter.Templates {

	/// <summary>
	/// There are cases in which projects are ignored in certain platforms, either because
	/// we do know we have issues in the plafrom or because certain dlls are not present. The interface allows
	/// to ignore projects and dlls per platform. Template objects should honour this filter.
	/// </summary>
	public interface IProjectFilter {

		/// <summary>
		/// True if a specific dlls should be ignored from a project in a platform.
		/// </summary>
		/// <param name="platform">The platform whose project is going to be generated.</param>
		/// <param name="assembly">The assembly that is part of the test project.</param>
		/// <returns>True if the dll was to be excluded, false otherwhise.</returns>
		bool ExcludeDll (Platform platform, string assembly);

		/// <summary>
		/// True if a specific project has to be ignored in a platform.
		/// </summary>
		/// <param name="project">The project that is being generated.</param>
		/// <param name="platform">The target platform of the project.</param>
		/// <returns>True if the project should be ignored, false otherwise.</returns>
		bool ExludeProject (ProjectDefinition project, Platform platform);

		/// <summary>
		/// Returns the list of ignore files that will be added to a project so that certain tests are ignored. This
		/// is useful when we know that there are flacky tests that we want to ignore but they have not yet been ignored
		/// by the project that builds the test dlls.
		/// </summary>
		/// <param name="projectName">The name of the project being generated.</param>
		/// <param name="assemblies">The assemblies information.</param>
		/// <param name="platform">The plaform targented by the generated project.</param>
		/// <returns>An enumerable with all the ignore text files to use.</returns>
		IEnumerable<string> GetIgnoreFiles (string projectName, List<(string assembly, string hintPath)> assemblies, Platform platform);


		/// <summary>
		/// Return the files that contain the traits to be ignored in a specific platform. Allows to configure different
		/// traits per platform so that certain tests are not ran.
		/// </summary>
		/// <param name="platform">The platform of the project that is being generated.</param>
		/// <returns>IEnumerable of string with the paths of the trait files to include in the project.</returns>
		IEnumerable<string> GetTraitsFiles (Platform platform);
	}
}
