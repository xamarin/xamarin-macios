using System;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class XamarinForms : ProjectTest
	{
		public XamarinForms (string platform) : base (platform)
		{
		}

		[Test]
		public void IncrementalBuilds ()
		{
			NugetRestore ("../MyXamarinFormsApp/MyXamarinFormsApp.csproj");
			NugetRestore ("../MyXamarinFormsApp/MyXamarinFormsAppNS/MyXamarinFormsAppNS.csproj");

			// First build
			BuildProject ("MyXamarinFormsApp", Platform, "Debug");
			Assert.IsFalse (IsTargetSkipped ("_CompileToNative"), "_CompileToNative should *not* be skipped on first build.");

			// Build with no changes
			Engine.Logger.Clear ();
			BuildProject ("MyXamarinFormsApp", Platform, "Debug", clean: false);
			Assert.IsTrue (IsTargetSkipped ("_CompileToNative"), "_CompileToNative should be skipped on a build with no changes.");

			// Build with XAML change
			Touch ("../MyXamarinFormsApp/MyXamarinFormsAppNS/App.xaml");
			Engine.Logger.Clear ();
			BuildProject ("MyXamarinFormsApp", Platform, "Debug", clean: false);

			Assert.IsFalse (IsTargetSkipped ("_CompileToNative"), "_CompileToNative should *not* be skipped on a build with a XAML change.");
		}
	}
}
