using System;
using NUnit.Framework;
using System.IO;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class BindingProject : TestBase
	{
		string Platform;
		public BindingProject (string platform)
		{
			Platform = platform;
		}

		[Test]
		public void BuildTest ()
		{
			var mtouchPaths = SetupProjectPaths ("bindings-test", "bindings-test-unified", "../../../tests/", false, Platform, "Any CPU/Debug-unified");

			var proj = SetupProject (Engine, mtouchPaths ["project_csprojpath"]);

			AppBundlePath = mtouchPaths.AppBundlePath;
			var dllPath = Path.Combine(mtouchPaths.ProjectBinPath, "bindings-test.dll");

			Engine.GlobalProperties.SetProperty ("Platform", Platform);

			RunTarget (proj, "Build", 0);
			Assert.IsTrue (File.Exists (dllPath), "{1} binding dll does not exist: {0} ", dllPath, Platform);

			RunTarget (proj, "Clean");
			Assert.IsFalse (File.Exists (dllPath), "{1}: binding dll exists after cleanup: {0} ", dllPath, Platform);
		}


		[Test]
		public void FrameworkTest ()
		{
			var mtouchPaths = SetupProjectPaths ("bindings-test", "bindings-test-unified", "../../../tests/", false, Platform, "Any CPU/Debug-unified");

			var proj = SetupProject (Engine, mtouchPaths ["project_csprojpath"]);
			AppBundlePath = mtouchPaths.AppBundlePath;
			var dllPath = Path.Combine(mtouchPaths.ProjectBinPath, "bindings-test.dll");

			Engine.GlobalProperties.SetProperty ("Platform", Platform);

			RunTarget (proj, "Build", 0);
			Assert.IsTrue (File.Exists (dllPath), "{1} binding dll does not exist: {0} ", dllPath, Platform);

			RunTarget (proj, "Clean");
			Assert.IsFalse (File.Exists (dllPath), "{1}: binding dll exists after cleanup: {0} ", dllPath, Platform);
		}
	}
}

