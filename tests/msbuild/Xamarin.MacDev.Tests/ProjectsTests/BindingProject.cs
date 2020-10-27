using NUnit.Framework;
using System.IO;
using Xamarin.Tests;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class BindingProject : TestBase
	{
		public BindingProject (string platform)
			: base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			var mtouchPaths = SetupProjectPaths ("MyiOSFrameworkBinding", includePlatform: false);

			var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);

			AppBundlePath = mtouchPaths.AppBundlePath;
			var dllPath = Path.Combine (mtouchPaths.ProjectBinPath, "MyiOSFrameworkBinding.dll");

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);

			RunTarget (proj, "Restore", 0);

			RunTarget (proj, "Build", 0);
			Assert.IsTrue (File.Exists (dllPath), "{1} binding dll does not exist: {0} ", dllPath, Platform);

			RunTarget (proj, "Clean");
			Assert.IsFalse (File.Exists (dllPath), "{1}: binding dll exists after cleanup: {0} ", dllPath, Platform);
		}
	}
}

