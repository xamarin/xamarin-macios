using System;
using System.IO;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public class BindingReferences : TestBase {

		[Test]
		public void BuildTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var projA = SetupProjectPaths ("MyBindingsReferences/LibraryA");
			var dllAPath = Path.Combine (projA.ProjectBinPath, "LibraryA.dll");

			RunTarget (projA, "Build", 0);
			Assert.IsTrue (File.Exists (dllAPath), "LibraryA dll does not exist: {0} ", dllAPath);

			var projB = SetupProjectPaths ("MyBindingsReferences/LibraryB");
			var dllBPath = Path.Combine (projB.ProjectBinPath, "LibraryB.dll");

			RunTarget (projB, "Build", 0);
			Assert.IsTrue (File.Exists (dllBPath), "LibraryB binding dll does not exist: {0} ", dllBPath);
		}

		// https://bugzilla.xamarin.com/show_bug.cgi?id=56317
		[Test]
		public void SatelliteAssembliesBug ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var proj = SetupProjectPaths ("MySatelliteAssembliesBug/iOSBinding");
			var dll = Path.Combine (proj.ProjectBinPath, "iOSBinding.dll");

			RunTarget (proj, "Build", 0);
			Assert.IsTrue (File.Exists (dll), "iOSBinding dll does not exist: {0} ", dll);
		}
	}
}
