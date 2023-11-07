using System;
using System.IO;

namespace Xharness.TestImporter.Xamarin {
	public class TestAssemblyDefinition : ITestAssemblyDefinition {

		#region properties

		public string Name { get; set; }
		public bool IsXUnit { get; set; }
		public IAssemblyLocator AssemblyLocator { get; set; }

		#endregion

		public TestAssemblyDefinition (string name, IAssemblyLocator locator)
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
		/// Returns the path of the test assembly within the mono checkout.
		/// </summary>
		/// <param name="platform">The platform we are working with.</param>
		/// <returns>The full path of the assembly.</returns>
		public string GetPath (Platform platform)
		{
			var testsRootPath = AssemblyLocator.GetAssembliesLocation (platform);
			return Path.Combine (testsRootPath, GetName (platform));
		}
	}
}
