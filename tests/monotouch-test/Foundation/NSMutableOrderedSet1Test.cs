//
// Unit tests for NSMutableOrderedSet Generic support
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using Foundation;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSMutableOrderedSet1Test {

		[Test]
		public void Ctor ()
		{
			var oset = new NSMutableOrderedSet<NSData> ();

			Assert.AreEqual ((nint) 0, oset.Count, "NSMutableOrderedSet Count");
		}

		[Test]
		public void Ctor_Capacity ()
		{
			var oset = new NSMutableOrderedSet<NSData> (10);

			Assert.AreEqual ((nint) 0, oset.Count, "NSMutableOrderedSet Count");
		}

		[Test]
		public void Ctor_Start ()
		{
			var oSet = new NSMutableOrderedSet<NSString> (start: (NSString) "foo");

			Assert.AreEqual ((nint) 1, oSet.Count, "NSMutableOrderedSet Count");
		}

		[Test]
		public void Ctor_Params ()
		{
			var oSet = new NSMutableOrderedSet<NSString> ((NSString) "foo", (NSString) "bar", (NSString) "xyz");

			Assert.AreEqual ((nint) 3, oSet.Count, "NSMutableOrderedSet Count");
		}

		[Test]
		public void Ctor_NSSet ()
		{
			var set = new NSSet<NSString> ((NSString) "foo", (NSString) "bar", (NSString) "xyz");
			var oSet = new NSMutableOrderedSet<NSString> (set);

			Assert.AreEqual ((nint) set.Count, oSet.Count, "NSMutableOrderedSet Count");
		}

		[Test]
		public void Ctor_NSOrderedSet ()
		{
			var oSetSource = new NSOrderedSet<NSString> ((NSString) "foo", (NSString) "bar", (NSString) "xyz");
			var oSet = new NSMutableOrderedSet<NSString> (oSetSource);

			Assert.AreEqual (oSetSource.Count, oSet.Count, "NSOrderedSet1Test Count");
		}

		[Test]
		public void Ctor_NSMutableOrderedSet ()
		{
			var oMutableSet = new NSMutableOrderedSet<NSString> ((NSString) "foo", (NSString) "bar", (NSString) "xyz");
			var oSet = new NSMutableOrderedSet<NSString> (oMutableSet);

			Assert.AreEqual (oMutableSet.Count, oSet.Count, "NSOrderedSet1Test Count");
		}

		[Test]
		public void IndexerTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var oSet = new NSMutableOrderedSet<NSString> (str1, str2, str3);

			Assert.AreEqual ((nint) 3, oSet.Count, "NSOrderedSet1Test Count");
			Assert.AreSame (str2, oSet [1], "NSOrderedSet1Test IndexOf");
			Assert.Throws<ArgumentNullException> (() => oSet [1] = null);
		}

		[Test]
		public void AsSetTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var oSet = new NSMutableOrderedSet<NSString> (str1, str2, str3);
			NSSet<NSString> set = oSet.AsSet ();

			Assert.AreEqual ((nint) 3, oSet.Count, "NSOrderedSet1Test Count");
			Assert.AreEqual ((nuint) 3, set.Count, "NSOrderedSet1Test Count");
			Assert.AreSame (str3, set.LookupMember (str3), "NSOrderedSet1Test IndexOf");
		}

		[Test]
		public void InsertTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var oSet = new NSMutableOrderedSet<NSString> ();
			Assert.AreEqual ((nint) 0, oSet.Count, "InsertTest Count");

			oSet.Insert (str1, 0);
			oSet.Insert (str2, 1);
			oSet.Insert (str3, 2);
			Assert.AreEqual ((nint) 3, oSet.Count, "InsertTest Count");
		}

		[Test]
		public void ReplaceTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";
			var oSet = new NSMutableOrderedSet<NSString> (str1, str2, str3);

			oSet.Replace (0, str4);

			Assert.IsTrue (oSet.Contains (str4), "ReplaceTesr Contains 4");
			Assert.IsFalse (oSet.Contains (str1), "ReplaceTesr Contains 4");
		}

		[Test]
		public void AddTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var oSet = new NSMutableOrderedSet<NSString> {
				str1, str2, str3
			};

			Assert.AreEqual ((nint) 3, oSet.Count, "AddTest Count");
			Assert.IsTrue (oSet.Contains (str1), "AddTest Contains 1");
			Assert.IsTrue (oSet.Contains (str2), "AddTest Contains 2");
			Assert.IsTrue (oSet.Contains (str3), "AddTest Contains 3");
		}

		[Test]
		public void AddObjectsTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var oSet = new NSMutableOrderedSet<NSString> ();
			oSet.AddObjects (str1, str2, str3);

			Assert.AreEqual ((nint) 3, oSet.Count, "AddObjectsTest Count");
			Assert.IsTrue (oSet.Contains (str1), "AddObjectsTest Contains 1");
			Assert.IsTrue (oSet.Contains (str2), "AddObjectsTest Contains 2");
			Assert.IsTrue (oSet.Contains (str3), "AddObjectsTest Contains 3");
		}

		[Test]
		public void InsertObjectsTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";
			var oSet = new NSMutableOrderedSet<NSString> (str4);
			oSet.InsertObjects (new [] { str1, str2, str3 }, NSIndexSet.FromNSRange (new NSRange (0, 3)));

			Assert.AreEqual ((nint) 4, oSet.Count, "InsertObjectsTest Count");
			Assert.IsTrue (oSet.Contains (str1), "InsertObjectsTest Contains 1");
			Assert.IsTrue (oSet.Contains (str2), "InsertObjectsTest Contains 2");
			Assert.IsTrue (oSet.Contains (str3), "InsertObjectsTest Contains 3");
			Assert.IsTrue (oSet.Contains (str4), "InsertObjectsTest Contains 4");
			Assert.AreSame (str1, oSet [0], "InsertObjectsTest 1 == 1");
			Assert.AreSame (str4, oSet [3], "InsertObjectsTest 4 == 4");
		}

		[Test]
		public void ReplaceObjectsTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";

			var oSet = new NSMutableOrderedSet<NSString> (str1, str2);
			Assert.AreEqual ((nint) 2, oSet.Count, "ReplaceObjectsTest Count");
			Assert.AreSame (str1, oSet [0], "ReplaceObjectsTest 1 == 1");
			Assert.AreSame (str2, oSet [1], "ReplaceObjectsTest 2 == 2");

			oSet.ReplaceObjects (NSIndexSet.FromNSRange (new NSRange (0, 2)), str3, str4);
			Assert.AreSame (str3, oSet [0], "ReplaceObjectsTest 3 == 3");
			Assert.AreSame (str4, oSet [1], "ReplaceObjectsTest 4 == 4");
		}

		[Test]
		public void RemoveObjectTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var oSet = new NSMutableOrderedSet<NSString> (str1, str2, str3);
			Assert.AreEqual ((nint) 3, oSet.Count, "RemoveObjectTest Count");

			oSet.RemoveObject (str2);
			Assert.AreEqual ((nint) 2, oSet.Count, "RemoveObjectTest Count");
			Assert.IsFalse (oSet.Contains (str2), "RemoveObjectTest must not contain 2");
			Assert.IsTrue (oSet.Contains (str1), "RemoveObjectTest Contains 1");
			Assert.IsTrue (oSet.Contains (str3), "RemoveObjectTest Contains 3");
		}

		[Test]
		public void RemoveObjectsTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var oSet = new NSMutableOrderedSet<NSString> (str1, str2, str3);
			Assert.AreEqual ((nint) 3, oSet.Count, "RemoveObjectsTest Count");

			oSet.RemoveObjects (str1, str2);
			Assert.AreEqual ((nint) 1, oSet.Count, "RemoveObjectsTest Count");
			Assert.IsFalse (oSet.Contains (str1), "RemoveObjectsTest must not contain 1");
			Assert.IsFalse (oSet.Contains (str2), "RemoveObjectsTest must not contain 2");
			Assert.IsTrue (oSet.Contains (str3), "RemoveObjectsTest Contains 3");
		}

		[Test]
		public void IEnumerable1Test ()
		{
			const int C = 16 * 2 + 3; // NSFastEnumerator has a array of size 16, use more than that, and not an exact multiple.
			var values = new NSString [C];
			for (int i = 0; i < C; i++)
				values [i] = (NSString) i.ToString ();

			var st = new NSMutableOrderedSet<NSString> (values);
			Assert.AreEqual ((nint) C, st.Count, "Count 1");

			var lst = new List<NSString> ();
			foreach (var a in (IEnumerable<NSString>) st) {
				Assert.IsNotNull (a, "null item iterator");
				Assert.IsFalse (lst.Contains (a), "duplicated item iterator");
				lst.Add (a);
				Assert.IsTrue (Array.IndexOf (values, a) >= 0, "different object");
			}
			Assert.AreEqual (C, lst.Count, "iterator count");
		}

		[Test]
		public void IEnumerableTest ()
		{
			const int C = 16 * 2 + 3; // NSFastEnumerator has a array of size 16, use more than that, and not an exact multiple.
			var values = new NSString [C];
			for (int i = 0; i < C; i++)
				values [i] = (NSString) i.ToString ();

			var st = new NSMutableOrderedSet<NSString> (values);
			Assert.AreEqual ((nint) C, st.Count, "Count 1");

			var lst = new List<NSString> ();
			foreach (NSString a in (IEnumerable) st) {
				Assert.IsNotNull (a, "null item iterator");
				Assert.IsFalse (lst.Contains (a), "duplicated item iterator");
				lst.Add (a);
				Assert.IsTrue (Array.IndexOf (values, a) >= 0, "different object");
			}
			Assert.AreEqual (C, lst.Count, "iterator count");
		}

		[Test]
		public void OperatorAddTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";

			var first = new NSMutableOrderedSet<NSString> (str1, str2);
			var second = new NSMutableOrderedSet<NSString> (str3, str4);
			var third = first + second;
			Assert.AreEqual ((nint) 4, third.Count, "OperatorAdd Count");
			Assert.IsTrue (third.Contains (str1), "OperatorAdd 1");
			Assert.IsTrue (third.Contains (str2), "OperatorAdd 2");
			Assert.IsTrue (third.Contains (str3), "OperatorAdd 3");
			Assert.IsTrue (third.Contains (str4), "OperatorAdd 4");
		}

		[Test]
		public void OperatorAddTest2 ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";

			var first = new NSMutableOrderedSet<NSString> (str1, str2);
			var second = new NSSet<NSString> (str3, str4);
			var third = first + second;
			Assert.AreEqual ((nint) 4, third.Count, "OperatorAdd Count");
			Assert.IsTrue (third.Contains (str1), "OperatorAdd 1");
			Assert.IsTrue (third.Contains (str2), "OperatorAdd 2");
			Assert.IsTrue (third.Contains (str3), "OperatorAdd 3");
			Assert.IsTrue (third.Contains (str4), "OperatorAdd 4");
		}

		[Test]
		public void OperatorAddTest3 ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";

			var first = new NSMutableOrderedSet<NSString> (str1, str2);
			var second = new NSOrderedSet<NSString> (str3, str4);
			var third = first + second;
			Assert.AreEqual ((nint) 4, third.Count, "OperatorAdd Count");
			Assert.IsTrue (third.Contains (str1), "OperatorAdd 1");
			Assert.IsTrue (third.Contains (str2), "OperatorAdd 2");
			Assert.IsTrue (third.Contains (str3), "OperatorAdd 3");
			Assert.IsTrue (third.Contains (str4), "OperatorAdd 4");
		}

		[Test]
		public void OperatorSubtractTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";

			var first = new NSMutableOrderedSet<NSString> (str1, str2, str3, str4);
			var second = new NSMutableOrderedSet<NSString> (str3, str4);
			var third = first - second;

			Assert.AreEqual ((nint) 2, third.Count, "OperatorSubtract Count");
			Assert.IsTrue (third.Contains (str1), "OperatorSubtract 1");
			Assert.IsTrue (third.Contains (str2), "OperatorSubtract 2");
			Assert.IsFalse (third.Contains (str3), "OperatorSubtract 3");
			Assert.IsFalse (third.Contains (str4), "OperatorSubtract 4");
		}

		[Test]
		public void OperatorSubtractTest2 ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";

			var first = new NSMutableOrderedSet<NSString> (str1, str2, str3, str4);
			var second = new NSSet<NSString> (str3, str4);
			var third = first - second;

			Assert.AreEqual ((nint) 2, third.Count, "OperatorSubtract Count");
			Assert.IsTrue (third.Contains (str1), "OperatorSubtract 1");
			Assert.IsTrue (third.Contains (str2), "OperatorSubtract 2");
			Assert.IsFalse (third.Contains (str3), "OperatorSubtract 3");
			Assert.IsFalse (third.Contains (str4), "OperatorSubtract 4");
		}

		[Test]
		public void OperatorSubtractTest3 ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";

			var first = new NSMutableOrderedSet<NSString> (str1, str2, str3, str4);
			var second = new NSOrderedSet<NSString> (str3, str4);
			var third = first - second;

			Assert.AreEqual ((nint) 2, third.Count, "OperatorSubtract Count");
			Assert.IsTrue (third.Contains (str1), "OperatorSubtract 1");
			Assert.IsTrue (third.Contains (str2), "OperatorSubtract 2");
			Assert.IsFalse (third.Contains (str3), "OperatorSubtract 3");
			Assert.IsFalse (third.Contains (str4), "OperatorSubtract 4");
		}

		[Test]
		public void OperatorPlusReferenceTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";
			var str4 = (NSString) "4";
			var str5 = (NSString) "5";
			var str6 = (NSString) "6";

			var one = new NSMutableOrderedSet<NSString> (str1, str2, str3);
			var two = new NSMutableOrderedSet<NSString> (str4, str5, str6);
			NSMutableOrderedSet<NSString> nil = null;
			using (var sum = one + nil)
			using (var sum2 = two + one)
			using (var sum3 = one + two) {

			}
			Assert.AreNotEqual (IntPtr.Zero, one.Handle, "Handle must be != IntPtr.Zero");
			Assert.AreNotEqual (IntPtr.Zero, two.Handle, "Handle must be != IntPtr.Zero");
		}
	}
}
