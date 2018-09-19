//
// Unit tests for EncodingDetectionOptions
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EncodingDetectionOptionsTest {

		[Test]
		public void SetValueEnumArray ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);

			var encodings = new NSStringEncoding [] { NSStringEncoding.ISOLatin1, NSStringEncoding.ISOLatin2 };
			var edo = new EncodingDetectionOptions () {
				EncodingDetectionDisallowedEncodings = encodings
			};

			using (var d = edo.Dictionary) {
				Assert.That ((int) d.Count, Is.EqualTo (1), "Count");
				var values = d [d.Keys [0]] as NSArray;
				Assert.That (values.GetItem<NSNumber> (0), Is.EqualTo ((NSNumber) (int) NSStringEncoding.ISOLatin1), "0");
				Assert.That (values.GetItem<NSNumber> (1), Is.EqualTo ((NSNumber) (int) NSStringEncoding.ISOLatin2), "1");
			}
		}
	}
}