//
// Unit tests for NSMutableOrderedSet
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using NUnit.Framework;

using Foundation;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSMutableOrderedSetTest {

		[Test]
		public void OperatorAddTest ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";

			using (var set1 = new NSMutableOrderedSet (str1))
			using (var set2 = new NSMutableOrderedSet (str2, str3))
			using (var result = set1 + set2) {
				Assert.AreEqual ((nint) 3, result.Count, "AddTest Count");
				Assert.IsTrue (result.Contains (str1), "AddTest Contains 1");
				Assert.IsTrue (result.Contains (str2), "AddTest Contains 2");
				Assert.IsTrue (result.Contains (str3), "AddTest Contains 3");
			}
		}

		[Test]
		public void OperatorSubtractTest ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";
			var str4 = "4";

			using (var first = new NSMutableOrderedSet (str1, str2, str3, str4))
			using (var second = new NSMutableOrderedSet (str3, str4))
			using (var third = first - second) {

				Assert.AreEqual ((nint) 2, third.Count, "OperatorSubtract Count");
				Assert.IsTrue (third.Contains (str1), "OperatorSubtract 1");
				Assert.IsTrue (third.Contains (str2), "OperatorSubtract 2");
				Assert.IsFalse (third.Contains (str3), "OperatorSubtract 3");
				Assert.IsFalse (third.Contains (str4), "OperatorSubtract 4");
			}
		}

		[Test]
		public void OperatorPlusReferenceTest ()
		{
			var one = new NSMutableOrderedSet ("1", "2", "3");
			var two = new NSMutableOrderedSet ("4", "5", "6");
			NSMutableOrderedSet nil = null;
			using (var sum = one + nil)
			using (var sum2 = two + one)
			using (var sum3 = one + two) {

			}
			Assert.AreNotEqual (IntPtr.Zero, one.Handle, "Handle must be != IntPtr.Zero");
			Assert.AreNotEqual (IntPtr.Zero, two.Handle, "Handle must be != IntPtr.Zero");
		}

		[Test]
		public void OperatorEqualTest ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";

			using (var oSet = new NSMutableOrderedSet (str1, str2, str3))
			using (var oSet2 = new NSMutableOrderedSet (str1, str2, str3)) {
				Assert.IsTrue (oSet == oSet2, "NSMutableOrderedSetTest == must be true");
				Assert.IsTrue (oSet.Equals (oSet2), "NSMutableOrderedSetTest Equals must be true");
			}
		}

		[Test]
		public void OperatorDifferentTest ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";

			using (var oSet = new NSMutableOrderedSet (str1, str2, str3))
			using (var oSet2 = new NSMutableOrderedSet (str3, str2, str1)) {
				Assert.IsTrue (oSet != oSet2, "NSMutableOrderedSetTest != must be true");
				Assert.IsFalse (oSet.Equals (oSet2), "NSMutableOrderedSetTest Equals must be false");
			}
		}
	}
}
