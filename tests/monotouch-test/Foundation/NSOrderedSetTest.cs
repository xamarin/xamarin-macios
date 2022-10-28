//
// Unit tests for NSOrderedSet
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
	public class NSOrderedSetTest {

		[Test]
		public void OperatorAddTest ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";

			using (var set1 = new NSOrderedSet (str1))
			using (var set2 = new NSOrderedSet (str2, str3))
			using (var result = set1 + set2) {
				Assert.AreEqual ((nint) 3, result.Count, "AddTest Count");
				Assert.IsTrue (result.Contains (str1), "AddTest Contains 1");
				Assert.IsTrue (result.Contains (str2), "AddTest Contains 2");
				Assert.IsTrue (result.Contains (str3), "AddTest Contains 3");
			}
		}

		[Test]
		public void OperatorAddTest2 ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";

			using (var set1 = new NSOrderedSet (str1))
			using (var set2 = new NSSet (str2, str3))
			using (var result = set1 + set2) {
				Assert.AreEqual ((nint) 3, result.Count, "AddTest Count");
				Assert.IsTrue (result.Contains (str1), "AddTest Contains 1");
				Assert.IsTrue (result.Contains (str2), "AddTest Contains 2");
				Assert.IsTrue (result.Contains (str3), "AddTest Contains 3");
			}
		}

		[Test]
		public void OperatorAddTest3 ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";

			using (var set1 = new NSOrderedSet (str1))
			using (var set2 = new NSMutableSet (str2, str3))
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

			using (var first = new NSOrderedSet (str1, str2, str3, str4))
			using (var second = new NSOrderedSet (str3, str4))
			using (var third = first - second) {

				Assert.AreEqual ((nint) 2, third.Count, "OperatorSubtract Count");
				Assert.IsTrue (third.Contains (str1), "OperatorSubtract 1");
				Assert.IsTrue (third.Contains (str2), "OperatorSubtract 2");
				Assert.IsFalse (third.Contains (str3), "OperatorSubtract 3");
				Assert.IsFalse (third.Contains (str4), "OperatorSubtract 4");
			}
		}

		[Test]
		public void OperatorSubtractTest2 ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";
			var str4 = "4";

			using (var first = new NSOrderedSet (str1, str2, str3, str4))
			using (var second = new NSSet (str3, str4))
			using (var third = first - second) {

				Assert.AreEqual ((nint) 2, third.Count, "OperatorSubtract Count");
				Assert.IsTrue (third.Contains (str1), "OperatorSubtract 1");
				Assert.IsTrue (third.Contains (str2), "OperatorSubtract 2");
				Assert.IsFalse (third.Contains (str3), "OperatorSubtract 3");
				Assert.IsFalse (third.Contains (str4), "OperatorSubtract 4");
			}
		}

		[Test]
		public void OperatorSubtractTest3 ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";
			var str4 = "4";

			using (var first = new NSOrderedSet (str1, str2, str3, str4))
			using (var second = new NSMutableSet (str3, str4))
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
			var one = new NSOrderedSet ("1", "2", "3");
			var two = new NSOrderedSet ("4", "5", "6");
			NSOrderedSet nil = null;
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

			using (var oSet = new NSOrderedSet (str1, str2, str3))
			using (var oSet2 = new NSOrderedSet (str1, str2, str3)) {
				Assert.IsTrue (oSet == oSet2, "NSOrderedSetTest == must be true");
				Assert.IsTrue (oSet.Equals (oSet2), "NSOrderedSetTest Equals must be true");
			}
		}

		[Test]
		public void OperatorDifferentTest ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";

			using (var oSet = new NSOrderedSet (str1, str2, str3))
			using (var oSet2 = new NSOrderedSet (str3, str2, str1)) {
				Assert.IsTrue (oSet != oSet2, "NSOrderedSetTest != must be true");
				Assert.IsFalse (oSet.Equals (oSet2), "NSOrderedSetTest Equals must be false");
			}
		}
	}
}
