//
// Unit tests for NSArray Generic support
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
	public class NSArray1Test {
		[Test]
		public void Ctor ()
		{
			var arr = new NSArray<NSData> ();

			Assert.AreEqual ((nuint) 0, arr.Count, "NSArray Count");
		}

		[Test]
		public void FromNSObjectsTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";

			using (var arr = NSArray<NSString>.FromNSObjects (str1, str2, str3)) {
				Assert.AreEqual ((nuint) 3, arr.Count, "NSArray Count");
				Assert.AreSame (str1, arr [0], "NSArray indexer");
				Assert.AreSame (str2, arr [1], "NSArray indexer");
				Assert.AreSame (str3, arr [2], "NSArray indexer");
			}
		}

		[Test]
		public void FromNSObjectsCountTest ()
		{
			var str1 = (NSString) "1";
			var str2 = (NSString) "2";
			var str3 = (NSString) "3";

			using (var arr = NSArray<NSString>.FromNSObjects (3, str1, str2, str3)) {
				Assert.AreEqual ((nuint) 3, arr.Count, "NSArray Count");
				Assert.AreSame (str1, arr [0], "NSArray indexer");
				Assert.AreSame (str2, arr [1], "NSArray indexer");
				Assert.AreSame (str3, arr [2], "NSArray indexer");
			}
		}

		[Test]
		public void IEnumerableTest ()
		{
			const int C = 16 * 2 + 3; // NSFastEnumerator has a array of size 16, use more than that, and not an exact multiple.
			var values = new NSString [C];
			for (int i = 0; i < C; i++)
				values [i] = (NSString) i.ToString ();

			var st = NSArray<NSString>.FromNSObjects (values);
			Assert.AreEqual ((nuint) C, st.Count, "Count 1");

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
		public void FromNSObjectsNullTest ()
		{
			var str1 = (NSString) "1";
			NSString str2 = null;
			var str3 = (NSString) "3";

			using (var arr = NSArray<NSString>.FromNSObjects (str1, str2, str3)) {
				Assert.AreEqual ((nuint) 3, arr.Count, "NSArray Count");
				Assert.AreSame (str1, arr [0], "NSArray indexer");
				Assert.IsNull (arr [1], "NSArray null indexer");
				Assert.AreSame (str3, arr [2], "NSArray indexer");
			}
		}
	}
}
