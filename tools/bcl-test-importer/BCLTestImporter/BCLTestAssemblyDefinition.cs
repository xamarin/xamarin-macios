using System;
using System.IO;
using System.Collections.Generic;

namespace BCLTestImporter {
	public struct BCLTestAssemblyDefinition {
		
		#region static vars
		
		static string partialPath = "mcs/class/lib";
		static readonly Dictionary<Platform, string> downloadPartialPath = new Dictionary<Platform, string> {
			{Platform.iOS, "ios-bcl"},
			{Platform.WatchOS, "ios-bcl"},
			{Platform.TvOS, "ios-bcl"},
			{Platform.MacOSFull, "mac-bcl"},
			{Platform.MacOSModern, "mac-bcl"},
		};
		static readonly Dictionary <Platform, string> platformPathMatch = new Dictionary <Platform, string> {
			{Platform.iOS, "monotouch"},
			{Platform.WatchOS, "monotouch_watch"},
			{Platform.TvOS, "monotouch_tv"},
			{Platform.MacOSFull, "xammac_net_4_5"},
			{Platform.MacOSModern, "xammac"},
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
		
		public string GetName (Platform platform)
		{
			switch (platform) {
			case Platform.WatchOS:
				return Name.Replace ("monotouch_", "monotouch_watch_");
			case Platform.TvOS:
				return Name.Replace ("monotouch_", "monotouch_tv_");
			case Platform.MacOSModern:
				return Name.Replace ("xammac_net_4_5", "xammac");
			default:
				return Name;
			}
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

			switch (platform) {
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				return Path.Combine (downloadsPath, "mac-bcl", platformPathMatch [platform], "tests");
			case Platform.iOS:
			case Platform.TvOS:
			case Platform.WatchOS:
				return Path.Combine (downloadsPath, "ios-bcl", platformPathMatch [platform], "tests"); 
			}

			return null;
		}

		public static string GetHintPathForRefenreceAssembly (string assembly, string monoRootPath, Platform platform, bool isDownload)
		{
			var hintPath = Path.Combine (monoRootPath, isDownload? downloadPartialPath [platform] : partialPath, platformPathMatch [platform], $"{assembly}.dll");
			if (File.Exists (hintPath)) {
				return hintPath;
			} else {
				// we could be referencing a dll in the test dir, lets test that
				hintPath = Path.Combine (monoRootPath, isDownload? downloadPartialPath [platform] : partialPath, platformPathMatch [platform], "tests", $"{assembly}.dll");
			}
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
			return Path.Combine (testsRootPath, GetName (platform));
		}
	}
}
