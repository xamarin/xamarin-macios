using System;
using System.IO;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class ProjectWithSpacesTests : ProjectTest
	{
		public ProjectWithSpacesTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			this.BuildProject ("My Spaced App", Platform, "Debug", clean: false);
		}
	}
}

