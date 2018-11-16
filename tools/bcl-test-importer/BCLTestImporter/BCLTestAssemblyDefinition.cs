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
			{Platform.MacOS, "xammac"},
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
		public static string GetTestDirectory (string monoRootPath, Platform platform)
		{
			if (monoRootPath == null)
				throw new ArgumentNullException (nameof (monoRootPath));
			var fullPath = monoRootPath;
			return Path.Combine (fullPath, partialPath, platformPathMatch[platform], "tests");
		}

		public static string GetHintPathForRefenreceAssembly (string assembly, string monoRootPath, Platform plaform)
		{
			var hintPath = Path.Combine (monoRootPath, partialPath, platformPathMatch[plaform], $"{assembly}.dll");
			return File.Exists (hintPath) ? hintPath : null;
		}
		
		/// <summary>
		/// Returns the path of the test assembly within the mono checkout.
		/// </summary>
		/// <param name="monoRootPath">The root path of the mono checkout.</param>
		/// <param name="platform">The platform we are working with.</param>
		/// <returns>The full path of the assembly.</returns>
		public string GetPath (string monoRootPath, Platform platform) => Path.Combine (GetTestDirectory (monoRootPath, platform), Name);
	}
}
