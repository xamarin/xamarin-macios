//
// Unit tests for NSUrlRequest
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlRequestTest {

		[Test]
		public void Mutability_30744 ()
		{
			using (var s1 = new NSString ("Authorization"))
			using (var s2 = new NSString ("ok"))
			using (var s3 = new NSString ("fail"))
			using (var md = NSMutableDictionary.FromObjectAndKey (s2, s1))
			using (var ur = new NSUrlRequest ())
			using (var mur = (NSMutableUrlRequest) ur.MutableCopy ()) {
				Assert.Null (ur.Headers, "NSUrlRequest / Headers / null");
				Assert.Null (mur.Headers, "NSMutableUrlRequest / Headers / null");

				mur.Headers = md;

				// that a bit like lying, we still consider it an NSMutableDictionary but it't not mutable
				Assert.That (mur.Headers, Is.TypeOf (typeof (NSMutableDictionary)), "NSMutableDictionary");

				var isActuallyMutable = TestRuntime.CheckXcodeVersion (8, 0);
				if (isActuallyMutable) {
					mur.Headers.SetValueForKey (s3, s1);
				} else {
					// In older OSes, the Headers is an instance of a class that's somehow immutable,
					// even though it's an NSMutableDictionary subclass (specifically __NSCFDictionary).
					// This feels like a bug that Apple fixed at some point.
#if __MACOS__ || NET
					Assert.Throws<ObjCException> (() => mur.Headers.SetValueForKey (s3, s1));
#else
					Assert.Throws<MonoTouchException> (() => mur.Headers.SetValueForKey (s3, s1));
#endif
				}

				// the original NSMutableDictionary is fine - but it's not what's being used, i.e. property is "copy"
				md.Remove (s1);
				Assert.That (md.Count, Is.EqualTo ((nuint) 0), "1");
				Assert.That (mur.Headers.Count, Is.EqualTo ((nuint) 1), "2");
				md.SetValueForKey (s3, s1);
				Assert.That (md.Count, Is.EqualTo ((nuint) 1), "3");
				Assert.That (mur.Headers.Count, Is.EqualTo ((nuint) 1), "40");

				Assert.AreNotSame (md, mur.Headers, "!same");

				// https://www.bignerdranch.com/blog/about-mutability/
				Assert.That (mur.Headers.Class.Name, Is.EqualTo ("__NSCFDictionary"), "__NSCFDictionary");
			}
		}
	}
}
