using System;
using System.IO;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	public class BindingReferences : TestBase {

		[Test]
		public void BuildTest ()
		{
			var mtouchPathsLibraryA = SetupProjectPaths ("LibraryA", "LibraryA", "../MyBindingsReferences/", false);
			var mtouchPathsLibraryB = SetupProjectPaths ("LibraryB", "LibraryB", "../MyBindingsReferences/", false);

			var projA = SetupProject (Engine, mtouchPathsLibraryA ["project_csprojpath"]);
			var dllAPath = Path.Combine (mtouchPathsLibraryA.ProjectBinPath, "LibraryA.dll");

			RunTarget (projA, "Build", 0);
			Assert.IsTrue (File.Exists (dllAPath), "LibraryA dll does not exist: {0} ", dllAPath);

			var projB = SetupProject (Engine, mtouchPathsLibraryB ["project_csprojpath"]);
			var dllBPath = Path.Combine (mtouchPathsLibraryB.ProjectBinPath, "LibraryB.dll");

			RunTarget (projB, "Build", 0);
			Assert.IsTrue (File.Exists (dllBPath), "LibraryB binding dll does not exist: {0} ", dllBPath);
		}
	}
}
