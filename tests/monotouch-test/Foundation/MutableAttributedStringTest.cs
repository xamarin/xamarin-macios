//
// Unit tests for NSMutableSAttributedString
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc. All rights reserved.
//

using System;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MutableSAttributedStringTest {

		[Test]
		public void InitWith ()
		{
			using (var s1 = new NSMutableAttributedString ("string")) {
				// initWithString: does not respond (see dontlink.app) but it works
				Assert.That (s1.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle-1");

				using (var d = new NSDictionary ())
				using (var s2 = new NSMutableAttributedString ("string", d)) {
					// initWithString:attributes: does not respond (see dontlink.app) but it works
					Assert.That (s2.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle-2");
				}

				using (var s3 = new NSMutableAttributedString (s1)) {
					// initWithAttributedString: does not respond (see dontlink.app) but it works
					Assert.That (s3.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle-3");
				}
			}
		}

		[Test]
		public void NullDictionary ()
		{
			using (var s = new NSMutableAttributedString ("string", (NSDictionary) null)) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero));
			}
		}

#if !__WATCHOS__ && !MONOMAC // No foregroundColor parameter for mac
		[Test]
		public void IndirectNullDictionary ()
		{
			// that will call NSAttributedString.ToDictionary which may return null (if empty)
			using (var s = new NSMutableAttributedString ("string", foregroundColor: null)) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero));
			}
		}
#endif // !__WATCHOS__
	}
}
