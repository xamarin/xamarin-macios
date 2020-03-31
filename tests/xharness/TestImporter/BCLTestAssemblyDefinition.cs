using System;
using System.IO;
using System.Collections.Generic;
using Xharness.BCLTestImporter.Templates;

namespace Xharness.BCLTestImporter {
	public class BCLTestAssemblyDefinition {

		public IAssemblyLocator AssemblyLocator { get; set; }

		#region static vars
		
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
		
		public BCLTestAssemblyDefinition (string name, IAssemblyLocator locator)
		{
			Name = name ?? throw new ArgumentNullException (nameof (name));
			AssemblyLocator = locator ?? throw new ArgumentNullException (nameof (locator));
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
		/// Returns the directory from the downloads where tests can be found.
		/// </summary>
		/// <returns>The test directory from downloads path.</returns>
		/// <param name="downloadsPath">Downloads path.</param>
		/// <param name="platform">Platform whose tests we require.</param>
		public string GetTestDirectoryFromDownloadsPath (Platform platform)
		{
			var downloadsPath = AssemblyLocator.GetAssembliesRootLocation (platform);

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

		public static string GetHintPathForReferenceAssembly (string assembly, string monoRootPath, Platform platform)
		{
			var hintPath = Path.Combine (monoRootPath, downloadPartialPath [platform], platformPathMatch [platform], $"{assembly}.dll");
			if (File.Exists (hintPath)) {
				return hintPath;
			} else {
				// we could be referencing a dll in the test dir, lets test that
				hintPath = Path.Combine (monoRootPath, downloadPartialPath [platform], platformPathMatch [platform], "tests", $"{assembly}.dll");
			}
			return File.Exists (hintPath) ? hintPath : null;
		}
		
		/// <summary>
		/// Returns the path of the test assembly within the mono checkout.
		/// </summary>
		/// <param name="platform">The platform we are working with.</param>
		/// <returns>The full path of the assembly.</returns>
		public string GetPath (Platform platform)
		{
			var testsRootPath = GetTestDirectoryFromDownloadsPath (platform);
			return Path.Combine (testsRootPath, GetName (platform));
		}
	}
}
