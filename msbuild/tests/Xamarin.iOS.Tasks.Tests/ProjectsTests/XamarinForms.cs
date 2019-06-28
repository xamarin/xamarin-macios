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
		public void BasicTest ()
		{
			NugetRestore ("../MyXamarinFormsApp/MyXamarinFormsApp.csproj");
			NugetRestore ("../MyXamarinFormsApp/MyXamarinFormsAppNS/MyXamarinFormsAppNS.csproj");
			this.BuildProject ("MyXamarinFormsApp", Platform, "Debug", clean: false);
		}
	}
}
