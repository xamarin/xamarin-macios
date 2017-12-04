using System;
using System.IO;

namespace InstallSources
{
	public class MonoPathMangler : IPathMangler
	{
		static readonly string iOSFramework = "Xamarin.iOS/";
		static readonly string MacFramework = "Xamarin.Mac/";
		/// <summary>
		/// Gets and sets the location of the mono sources.
		/// </summary>
		/// <value>The mono source path.</value>
		public string MonoSourcePath { get; set; }
		
		/// <summary>
		/// Gets or sets the frame work dir.
		/// </summary>
		/// <value>The frame work dir.</value>
		public string DestinationDir { get; set; }

		/// <summary>
		/// Gets or sets the install dir.
		/// </summary>
		/// <value>The install dir.</value>
		public string InstallDir { get; set; }

		public string GetSourcePath (string path)
		{
			bool iosFramework = true;
			if (path.StartsWith(MonoSourcePath, StringComparison.Ordinal)) 
				return path;
			// we are dealing with a package build
			var index = path.IndexOf (iOSFramework, StringComparison.Ordinal);
			if (index < 0) {// we are dealing with mac sources
				iosFramework = false;
				index = path.IndexOf(MacFramework, StringComparison.Ordinal);
			}
			path = path.Remove (0, index + ((iosFramework)?iOSFramework.Length : MacFramework.Length)); // + length framework
			return Path.Combine (MonoSourcePath, path);
		}

		public string GetTargetPath (string path)
		{
			var relativePath = path.Substring (MonoSourcePath.Length);
			if (relativePath.StartsWith ("/", StringComparison.Ordinal))
				relativePath = relativePath.Remove (0, 1);
			var target = Path.Combine (DestinationDir, "src", (InstallDir.Contains("Xamarin.iOS")?"Xamarin.iOS":"Xamarin.Mac"), relativePath);
			return target;
		}
	}
}
