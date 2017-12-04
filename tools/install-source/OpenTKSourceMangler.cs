using System;
using System.IO;

namespace InstallSources
{
	/// <summary>
	/// Path manipulator that knows how to deal with OpenTK paths.
	/// </summary>
	public class OpenTKSourceMangler : IPathMangler
	{
		static readonly string iOSFramework = "Xamarin.iOS/";
		static readonly string MacFramework = "Xamarin.Mac/";
		
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
		
		/// <summary>
		/// Gets or sets the frame work dir.
		/// </summary>
		/// <value>The frame work dir.</value>
		public string DestinationDir { get; set; }

		public string GetSourcePath (string path)
		{
			bool iosFramework = true;
			if (path.StartsWith (OpenTKSourcePath, StringComparison.Ordinal)) 
				return path;
			// we are dealing with a package build
			var index = path.IndexOf (iOSFramework, StringComparison.Ordinal);
			if (index < 0) {// we are dealing with mac sources
				iosFramework = false;
				index = path.IndexOf(MacFramework, StringComparison.Ordinal);
			}
			path = path.Remove (0, index + ((iosFramework)?iOSFramework.Length : MacFramework.Length)); // + length framework
			return Path.Combine (OpenTKSourcePath, path);
		}

		public string GetTargetPath (string path)
		{
			var relativePath = path.Substring (OpenTKSourcePath.Length);
			if (relativePath.StartsWith ("/", StringComparison.Ordinal))
				relativePath = relativePath.Remove (0, 1);
			var target = Path.Combine (DestinationDir, "src", (InstallDir.Contains ("Xamarin.iOS") ? "Xamarin.iOS" : "Xamarin.Mac"), relativePath);
			return target;
		}
	}
}
