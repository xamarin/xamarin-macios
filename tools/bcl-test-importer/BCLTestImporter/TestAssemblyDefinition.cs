using System;
using System.IO;
using System.Collections.Generic;

namespace BCLTestImporter {
	public struct TestAssemblyDefinition {
		
		#region static vars
		
		static string partialPath = "mcs/class/lib";
		static Dictionary <string, string> platformPathMatch = new Dictionary <string, string> {
			{"iOS", "monotouch"},
			{"WatchOS", "monotouch_watch"},
			{"TvOS", "monotouch_tv"},
			{"MacOS", "xammac"},
		};
		#endregion
		
		#region properties
		
		public string Name { get; set; }
		public bool IsXUnit { get; set; }
		
		#endregion
		
		public TestAssemblyDefinition (string name)
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
		public static string GetTestDirectory (string monoRootPath, string platform)
		{
			if (monoRootPath == null)
				throw new ArgumentNullException (nameof (monoRootPath));
			if (platform == null)
				throw new ArgumentNullException (nameof (platform));
			var fullPath = monoRootPath;
			switch (platform) {
			case "iOS":
				fullPath = Path.Combine (fullPath, partialPath, platformPathMatch["iOS"], "tests");
			break;
			case "WatchOS":
				fullPath = Path.Combine (fullPath, partialPath, platformPathMatch["WatchOS"], "tests");
			break;
			case "TvOS":
				fullPath = Path.Combine (fullPath, partialPath, platformPathMatch["TvOS"], "tests");
			break;
			case "MacOS":
				fullPath = Path.Combine (fullPath, partialPath, platformPathMatch["MacOS"], "tests");
			break;
			default:
			fullPath = null;
			break;
			}
			return fullPath;
		}

		/// <summary>
		/// Returns the path of the test assembly within the mono checkout.
		/// </summary>
		/// <param name="monoRootPath">The root path of the mono checkout.</param>
		/// <param name="platform">The platform we are working with.</param>
		/// <returns>The full path of the assembly.</returns>
		public string GetPath (string monoRootPath, string platform) => Path.Combine (GetTestDirectory (monoRootPath, platform), Name);
	}
}
