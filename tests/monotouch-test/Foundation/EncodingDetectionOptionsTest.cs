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
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Inconclusive ("Request iOS8+");

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