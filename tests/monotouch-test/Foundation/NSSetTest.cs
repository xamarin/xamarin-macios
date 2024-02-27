
using System;
using NUnit.Framework;

using Foundation;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSSetTest {
		[Test]
		public void SetCtors ()
		{
			// The NSSet (params object [] args)
			var s = new NSSet (1);
			Assert.AreEqual (s.Count, (nuint) 1);
			s = new NSSet (1, 2, 3);
			Assert.AreEqual (s.Count, (nuint) 3);

			// The NSSet (params [] NSObject args)
			var objs = new NSObject [5];
			for (int i = 0; i < objs.Length; i++)
				objs [i] = new NSNumber (i);

			s = new NSSet (objs [0], objs [1], objs [2], objs [3], objs [4]);
			Assert.AreEqual (s.Count, (nuint) 5);

			// Repeat the values
			s = new NSSet (objs [0], objs [1], objs [2], objs [0], objs [1]);
			Assert.AreEqual (s.Count, (nuint) 3);
		}

		[Test]
		public void OperatorPlus ()
		{
			var one = new NSSet (1, 2, 3);
			var two = new NSSet (4, 5, 6);
			var sum = one + two;
			Assert.AreEqual (sum.Count, (nuint) 6);

			var objs = new NSObject [5];
			for (int i = 0; i < objs.Length; i++)
				objs [i] = new NSNumber (i * 100);

			sum = new NSSet (objs) + one + two;
			Assert.AreEqual (sum.Count, (nuint) 11);
			sum = new NSSet (objs) + new NSSet (objs);
			Assert.AreEqual (sum.Count, (nuint) 5);

			Assert.AreEqual ((one + one).Count, (nuint) 3);
			var sub = one - one;
			Assert.AreEqual (sub.Count, (nuint) 0);

			var three = new NSSet (1, 2, 3, 4, 5, 6);
			var subt = three - two;
			Assert.AreEqual (subt.Count, (nuint) 3);
			Assert.True (three.Contains (1));
			Assert.True (three.Contains (2));
			Assert.True (three.Contains (3));
			subt = three - one;
			Assert.AreEqual (subt.Count, (nuint) 3);
			Assert.True (three.Contains (4));
			Assert.True (three.Contains (5));
			Assert.True (three.Contains (6));

		}

		[Test]
		public void OperatorPlusReferenceTest ()
		{
			var one = new NSSet (1, 2, 3);
			var two = new NSSet (4, 5, 6);
			NSSet nil = null;
			using (var sum = one + nil)
			using (var sum2 = two + one)
			using (var sum3 = one + two) {

			}
			Assert.AreNotEqual (IntPtr.Zero, one.Handle, "Handle must be != IntPtr.Zero");
			Assert.AreNotEqual (IntPtr.Zero, two.Handle, "Handle must be != IntPtr.Zero");
		}

		[Test]
		public void OperatorAddTest ()
		{
			var str1 = "1";
			var str2 = "2";
			var str3 = "3";

			using (var set1 = new NSSet (str1))
			using (var set2 = new NSOrderedSet (str2, str3))
			using (var result = set1 + set2) {
				Assert.AreEqual ((nuint) 3, result.Count, "AddTest Count");
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

			using (var set1 = new NSSet (str1))
			using (var set2 = new NSMutableOrderedSet (str2, str3))
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

			using (var first = new NSSet (str1, str2, str3, str4))
			using (var second = new NSOrderedSet (str3, str4))
			using (var third = first - second) {

				Assert.AreEqual ((nuint) 2, third.Count, "OperatorSubtract Count");
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

			using (var first = new NSSet (str1, str2, str3, str4))
			using (var second = new NSMutableOrderedSet (str3, str4))
			using (var third = first - second) {

				Assert.AreEqual ((nuint) 2, third.Count, "OperatorSubtract Count");
				Assert.IsTrue (third.Contains (str1), "OperatorSubtract 1");
				Assert.IsTrue (third.Contains (str2), "OperatorSubtract 2");
				Assert.IsFalse (third.Contains (str3), "OperatorSubtract 3");
				Assert.IsFalse (third.Contains (str4), "OperatorSubtract 4");
			}
		}
	}
}
