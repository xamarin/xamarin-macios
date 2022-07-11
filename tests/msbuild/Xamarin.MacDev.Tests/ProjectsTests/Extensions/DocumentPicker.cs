using System;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class DocumentPickerTests : ExtensionTestBase {

		public DocumentPickerTests (string platform) : base(platform)
		{
		}

		[Test]
		public void BasicTest () 
		{
			this.BuildExtension ("MyWebViewApp", "MyDocumentPickerExtension");
			this.TestStoryboardC (AppBundlePath);
		}
	}
}
