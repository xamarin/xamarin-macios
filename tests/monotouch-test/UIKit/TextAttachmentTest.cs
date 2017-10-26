//
// NSTextAttachment Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TextAttachmentTest {

		[Test]
		public void CtorNull ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7,0))
				Assert.Inconclusive ("requires iOS7+");

			using (var ta = new NSTextAttachment (null, null)) {
				Assert.IsTrue (ta.Bounds.IsEmpty, "Bounds");
				Assert.Null (ta.Contents, "Contents");
				Assert.Null (ta.FileType, "FileType");
				Assert.Null (ta.FileWrapper, "FileWrapper");
				Assert.Null (ta.Image, "Image");
			}
		}
	}
}

#endif // !__WATCHOS__
