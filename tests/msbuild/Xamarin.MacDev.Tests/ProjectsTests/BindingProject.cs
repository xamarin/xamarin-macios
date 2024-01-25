using NUnit.Framework;

using System.IO;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class BindingProject : TestBase {
		public BindingProject (string platform)
			: base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var proj = SetupProjectPaths ("MyiOSFrameworkBinding", includePlatform: false);

			MonoTouchProject = proj;
			var dllPath = Path.Combine (proj.ProjectBinPath, "MyiOSFrameworkBinding.dll");

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);

			RunTarget (proj, "Restore", 0);

			RunTarget (proj, "Build", 0);
			Assert.IsTrue (File.Exists (dllPath), "{1} binding dll does not exist: {0} ", dllPath, Platform);

			RunTarget (proj, "Clean");
			Assert.IsFalse (File.Exists (dllPath), "{1}: binding dll exists after cleanup: {0} ", dllPath, Platform);
		}
	}
}
