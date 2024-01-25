using System;
using System.Linq;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class ProjectWithSpacesTests : ProjectTest {
		public ProjectWithSpacesTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			this.BuildProject ("My Spaced App", clean: false);

			// Message of the form:
			// Property reassignment: $(AssemblySearchPaths)="..." (previous value: "...") at Xamarin.iOS.Common.props (106,3)
			var assemblySearchPaths = Engine.Logger.MessageEvents.FirstOrDefault (m => m.Message.Contains ("Property reassignment: $(AssemblySearchPaths)=\""));
			Assert.IsNotNull (assemblySearchPaths, "$(AssemblySearchPaths) should be modified");
			var split = assemblySearchPaths.Message.Split ('"');
			Assert.GreaterOrEqual (split.Length, 1, "Unexpected string contents");
			Assert.IsFalse (split [1].Contains ("{GAC}"), "$(AssemblySearchPaths) should not contain {GAC}");
		}
	}
}
