using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using Foundation;

namespace MonoTouchFixtures.Foundation {
#if false // https://github.com/xamarin/xamarin-macios/issues/15577
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSOrderedCollectionDifference1Test {

		[Test]
		public void InsertionsAndRemovalsTest ()
		{
			using var data = new NSString ("Foo");
			using var change = NSOrderedCollectionChange<NSString>.ChangeWithObject (data, NSCollectionChangeType.Insert, 0);
			var changes = new NSOrderedCollectionChange<NSString> [] { change };
			using var diff = new NSOrderedCollectionDifference<NSString> (changes);
			Assert.DoesNotThrow (() => {
				var i = diff.Insertions;
			});
			Assert.DoesNotThrow (() => {
				var r = diff.Removals;
			});
			// https://github.com/xamarin/xamarin-macios/issues/15577 - Did not rewrite tests that were disabled
			// Any reason for not asserting on the returned value?
			// Assert.AreEqual (1, diff.Insertions.Length, "insertions");
			// (or whatever it's supposed to return)
		}
	}
#endif
}
