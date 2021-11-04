using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using Foundation;

namespace MonoTouchFixtures.Foundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSOrderedCollectionDifferenceTest {
		
		[Test]
		public void InsertionsAndRemovalsTest ()
		{
			using var data = new NSString ("Foo");
			using var change = NSOrderedCollectionChange.ChangeWithObject (data, NSCollectionChangeType.Insert, 0);
			var changes = new NSOrderedCollectionChange [] { change };
			using var diff = new NSOrderedCollectionDifference (changes);
			Assert.DoesNotThrow (() => {
				var i = diff.Insertions;
			});
			Assert.DoesNotThrow (() => {
				var r = diff.Removals;
			});
		}
	}
}
