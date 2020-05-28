//
// Unit tests for NSUrlRequest
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

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
			using (var mur = (NSMutableUrlRequest)ur.MutableCopy ()) {
				Assert.Null (ur.Headers, "NSUrlRequest / Headers / null");
				Assert.Null (mur.Headers, "NSMutableUrlRequest / Headers / null");

				mur.Headers = md;

				// that a bit like lying, we still consider it an NSMutableDictionary but it't not mutable
				Assert.That (mur.Headers, Is.TypeOf (typeof (NSMutableDictionary)), "NSMutableDictionary");

#if !MONOMAC // No Simulator for mac
				// that would crash on devices
				// NSInternalInconsistencyException -[__NSCFDictionary setObject:forKey:]: mutating method sent to immutable object
				if (Runtime.Arch == Arch.SIMULATOR) {
					bool native_exception = false;
					try {
						mur.Headers.SetValueForKey (s3, s1);
						Assert.Fail ("exception immutability");
					} catch {
						native_exception = true;
					}
					Assert.True (native_exception, "non-mutable NSDictionary");

					// the original NSMutableDictionary is fine - but it's not what's being used, i.e. property is "copy"
					md.Remove (s1);
					Assert.That (md.Count, Is.EqualTo (0), "1");
					Assert.That (mur.Headers.Count, Is.EqualTo (1), "2");
					md.SetValueForKey (s3, s1);
					Assert.That (md.Count, Is.EqualTo (1), "3");
					Assert.That (mur.Headers.Count, Is.EqualTo (1), "40");

					Assert.AreNotSame (md, mur.Headers, "!same");
				}
#endif

				// https://www.bignerdranch.com/blog/about-mutability/
				Assert.That (mur.Headers.Class.Name, Is.EqualTo ("__NSCFDictionary"), "__NSCFDictionary");
			}
		}
	}
}