using System;
using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhoneSimulator")]
	[TestFixture ("iPhone")]
	public class ShareTests : ExtensionTestBase {
		public ShareTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			this.BuildExtension ("MyMasterDetailApp", "MyShareExtension");
		}
	}
}
