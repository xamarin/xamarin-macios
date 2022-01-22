//
// Unit tests for NSMutableSet
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
	public class NSMutableSetTest {

		[Test]
		public void OperatorAddTest ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";

			using (var set1 = new NSMutableSet (str1))
			using (var set2 = new NSMutableSet (str2, str3))
			using (var result = set1 + set2) {
				Assert.AreEqual ((nuint) 3, result.Count, "AddTest Count");
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

			var first = new NSMutableSet (str1, str2, str3, str4);
			var second = new NSMutableSet (str3, str4);
			var third = first - second;

			Assert.AreEqual ((nuint) 2, third.Count, "OperatorSubtract Count");
			Assert.IsTrue (third.Contains (str1), "OperatorSubtract 1");
			Assert.IsTrue (third.Contains (str2), "OperatorSubtract 2");
			Assert.IsFalse (third.Contains (str3), "OperatorSubtract 3");
			Assert.IsFalse (third.Contains (str4), "OperatorSubtract 4");
		}

		[Test]
		public void OperatorPlusReferenceTest ()
		{
			var one = new NSMutableSet ("1", "2", "3");
			var two = new NSMutableSet ("4", "5", "6");
			NSMutableSet nil = null;
			using (var sum = one + nil)
			using (var sum2 = two + one)
			using (var sum3 = one + two) {

			}
			Assert.AreNotEqual (IntPtr.Zero, one.Handle, "Handle must be != IntPtr.Zero");
			Assert.AreNotEqual (IntPtr.Zero, two.Handle, "Handle must be != IntPtr.Zero");
		}
	}
}
