using System;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class DocumentPickerTests : ExtensionTestBase {

		public DocumentPickerTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			this.BuildExtension ("MyWebViewApp", "MyDocumentPickerExtension");
			this.TestStoryboardC (AppBundlePath);
		}
	}
}
