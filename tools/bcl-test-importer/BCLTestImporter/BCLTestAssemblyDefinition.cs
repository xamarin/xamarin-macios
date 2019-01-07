using System;
using System.IO;
using System.Collections.Generic;

namespace BCLTestImporter {
	public struct BCLTestAssemblyDefinition {
		
		#region static vars
		
		static string partialPath = "mcs/class/lib";
		static Dictionary <Platform, string> platformPathMatch = new Dictionary <Platform, string> {
			{Platform.iOS, "monotouch"},
			{Platform.WatchOS, "monotouch"},
			{Platform.TvOS, "monotouch"},
			{Platform.MacOSFull, "xammac_net_4_5"},
			{Platform.MacOSModern, "xammac_net_4_5"},
		};
		#endregion
		
		#region properties
		
		public string Name { get; set; }
		public bool IsXUnit { get; set; }
		
		#endregion
		
		public BCLTestAssemblyDefinition (string name)
		{
			Name = name;
			// the following pattern is used when generating xunit test
			// assemblies
			IsXUnit = name.Contains ("_xunit-test");
		}

		/// <summary>
		/// Returns the mono directory where test can be found.
		/// </summary>
		/// <param name="monoRootPath">The root path of the mono checkout.</param>
		/// <param name="platform">The platform whose test directory we need.</param>
		/// <returns>The full path of the test directory.</returns>
		/// <exception cref="ArgumentNullException">Raised when one of the parameters is null.</exception>
		public static string GetTestDirectoryFromMonoPath (string monoRootPath, Platform platform)
		{
			if (string.IsNullOrEmpty (monoRootPath))
				throw new ArgumentNullException (nameof (monoRootPath));
			var fullPath = monoRootPath;
			return Path.Combine (fullPath, partialPath, platformPathMatch[platform], "tests");
		}

		/// <summary>
		/// Returns the directory from the downloads where tests can be found.
		/// </summary>
		/// <returns>The test directory from downloads path.</returns>
		/// <param name="downloadsPath">Downloads path.</param>
		/// <param name="platform">Platform whose tests we require.</param>
		public static string GetTestDirectoryFromDownloadsPath (string downloadsPath, Platform platform)
		{
			if (string.IsNullOrEmpty (downloadsPath))
				throw new ArgumentNullException (nameof (downloadsPath));
			return Path.Combine (downloadsPath, "ios-bcl", platformPathMatch [platform], "tests"); 
		}

		public static string GetHintPathForRefenreceAssembly (string assembly, string monoRootPath, Platform plaform)
		{
			var hintPath = Path.Combine (monoRootPath, partialPath, platformPathMatch[plaform], $"{assembly}.dll");
			return File.Exists (hintPath) ? hintPath : null;
		}
		
		/// <summary>
		/// Returns the path of the test assembly within the mono checkout.
		/// </summary>
		/// <param name="rootPath">The root path of the mono checkout.</param>
		/// <param name="platform">The platform we are working with.</param>
		/// <returns>The full path of the assembly.</returns>
		public string GetPath (string rootPath, Platform platform, bool wasDownloaded)
		{
			var testsRootPath = wasDownloaded? GetTestDirectoryFromDownloadsPath (rootPath, platform) : 
				GetTestDirectoryFromMonoPath (rootPath, platform);
			return Path.Combine (testsRootPath, Name);
		}
	}
}
