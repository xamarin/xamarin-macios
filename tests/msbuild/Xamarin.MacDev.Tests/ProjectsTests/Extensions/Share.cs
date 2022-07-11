using System;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhoneSimulator")]
	[TestFixture ("iPhone")]
	public class ShareTests : ExtensionTestBase {
		public ShareTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			this.BuildExtension ("MyMasterDetailApp", "MyShareExtension");
		}
	}
}
