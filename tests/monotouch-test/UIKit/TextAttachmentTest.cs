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
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TextAttachmentTest {

		[Test]
		public void CtorNull ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

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
