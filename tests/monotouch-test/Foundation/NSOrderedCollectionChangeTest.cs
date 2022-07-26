using System;

using NUnit.Framework;

using Foundation;

namespace MonoTouchFixtures.Foundation {
#if false // https://github.com/xamarin/xamarin-macios/issues/15577
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSOrderedCollectionChangeTest {

		[Test]
		public void ChangeWithObjectTest ()
		{
			TestRuntime.AssertXcodeVersion (13,0);

			using var str = new NSString ("Test");
			using var change = NSOrderedCollectionChange.ChangeWithObject (str, NSCollectionChangeType.Insert, 0);
			Assert.AreEqual (str, change.Object, "Content");
			Assert.AreEqual ((nuint)0, change.Index, "Index");
		}

		[Test]
		public void ChangeWithObjectTestWithAssociatedindex ()
		{
			TestRuntime.AssertXcodeVersion (13,0);

			using var str = new NSString ("Test");
			using var change = NSOrderedCollectionChange.ChangeWithObject (str, NSCollectionChangeType.Insert, 0, 1);
			Assert.AreEqual (str, change.Object, "Content");
			Assert.AreEqual ((nuint)0, change.Index, "Index");
			Assert.AreEqual ((nuint)1, change.AssociatedIndex);
		}
	}
#endif
}
