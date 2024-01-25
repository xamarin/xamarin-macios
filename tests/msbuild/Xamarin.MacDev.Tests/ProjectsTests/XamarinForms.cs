using System.IO;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class XamarinForms : ProjectTest {
		public XamarinForms (string platform) : base (platform)
		{
		}

		[Test]
		public void IncrementalBuilds ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var testdir = GetTestDirectory ();
			NugetRestore (Path.Combine (testdir, "MyXamarinFormsApp", "MyXamarinFormsApp.csproj"));
			NugetRestore (Path.Combine (testdir, "MyXamarinFormsApp", "MyXamarinFormsAppNS", "MyXamarinFormsAppNS.csproj"));

			// First build
			BuildProject ("MyXamarinFormsApp");
			Assert.IsFalse (IsTargetSkipped ("_CompileToNative"), "_CompileToNative should *not* be skipped on first build.");

			// Build with no changes
			BuildProject ("MyXamarinFormsApp", clean: false);
			Assert.IsTrue (IsTargetSkipped ("_CompileToNative"), "_CompileToNative should be skipped on a build with no changes.");

			// Build with XAML change
			Touch (Path.Combine (testdir, "MyXamarinFormsApp", "MyXamarinFormsAppNS", "App.xaml"));
			BuildProject ("MyXamarinFormsApp", clean: false);

			Assert.IsFalse (IsTargetSkipped ("_CompileToNative"), "_CompileToNative should *not* be skipped on a build with a XAML change.");
		}
	}
}
