using System;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class PhotoEditingTests : ExtensionTestBase {
		public PhotoEditingTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			this.BuildExtension ("MySpriteKitGame", "MyPhotoEditingExtension");
		}
	}
}
