using System;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	[Ignore ("The PhotoUI framework is broken in Xcode 11 beta 1 and the generated registrar code doesn't compile.")]
	public class PhotoEditingTests : ExtensionTestBase {
		public PhotoEditingTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			this.BuildExtension ("MySpriteKitGame", "MyPhotoEditingExtension", Platform, "Debug");
		}
	}
}

