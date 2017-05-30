using System;
using System.IO;

namespace InstallSources
{
	public class MonoPathMangler : IPathMangler
	{
		/// <summary>
		/// Gets and sets the location of the mono sources.
		/// </summary>
		/// <value>The mono source path.</value>
		public string MonoSourcePath { get; set; }

		/// <summary>
		/// Gets or sets the install dir.
		/// </summary>
		/// <value>The install dir.</value>
		public string InstallDir { get; set; }

		public string GetSourcePath (string path) => path;

		public string GetTargetPath (string path)
		{
			var relativePath = path.Substring (MonoSourcePath.Length);
			if (relativePath.StartsWith ("/", StringComparison.Ordinal))
				relativePath = relativePath.Remove (0, 1);
			var target = Path.Combine (InstallDir, "src", "mono", relativePath);
			return target;
		}
	}
}
