using System;

namespace InstallSources {
	/// <summary>
	/// Represents a class that knows how to change paths to ensure that the correct source path is used.
	/// </summary>
	public interface IPathMangler {
		/// <summary>
		/// Returns the real source path for the given path found in an mdb.
		/// </summary>
		/// <returns>The source path related to the mdb path.</returns>
		/// <param name="path">The path found in the mdb file.</param>
		string GetSourcePath (string path);

		/// <summary>
		/// Returns the target were a source path should be installed.
		/// </summary>
		/// <returns>The target path for the installation.</returns>
		/// <param name="path">The source path to install.</param>/
		string GetTargetPath (string path);
	}
}
