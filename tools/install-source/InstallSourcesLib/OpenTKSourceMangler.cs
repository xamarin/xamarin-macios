using System;
using System.IO;

namespace InstallSources
{
	/// <summary>
	/// Path manipulator that knows how to deal with OpenTK paths.
	/// </summary>
	public class OpenTKSourceMangler : IPathMangler
	{
		/// <summary>
		/// Locations of the OpenTK source.
		/// </summary>
		/// <value>The OpenTK source path.</value>
		public string OpenTKSourcePath { get; set; }

		/// <summary>
		/// Gets or sets the install dir.
		/// </summary>
		/// <value>The install dir.</value>
		public string InstallDir { get; set;}

		public string GetSourcePath (string path) => path;

		public string GetTargetPath (string path)
		{
			var relativePath = path.Substring (OpenTKSourcePath.Length);
			if (relativePath.StartsWith ("/", StringComparison.InvariantCulture))
				relativePath = relativePath.Remove (0, 1);
			var target = Path.Combine (InstallDir, "src", relativePath);
			return target;
		}
	}
}
