using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using Foundation;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSMutableArray1Test {

		[Test]
		public void Ctor ()
		{
			using (var arr = new NSMutableArray<NSDate> ()) {
				Assert.AreEqual ((nuint) 0, arr.Count, "Count");
			}
		}

		[Test]
		public void Ctor_Capacity ()
		{
			using (var arr = new NSMutableArray<NSString> (1)) {
				Assert.AreEqual ((nuint) 0, arr.Count, "Count");
			}
		}

		[Test]
		public void ContainsTest ()
		{
			var v = (NSString) "value";
			var v2 = (NSString) "value 2";
			using (var arr = new NSMutableArray<NSString> (v, v)) {
				Assert.Throws<ArgumentNullException> (() => arr.Contains (null), "Contains ANE");
				Assert.IsTrue (arr.Contains (v), "Contains 1");
				Assert.IsFalse (arr.Contains (v2), "Contains 2");
			}
		}

		[Test]
		public void IndexOfTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";

			using (var arr = new NSMutableArray<NSString> (v1)) {
				Assert.Throws<ArgumentNullException> (() => arr.IndexOf (null), "IndexOf ANE");
				Assert.AreEqual ((nuint) 0, arr.IndexOf (v1), "IndexOf 1");
				Assert.AreEqual ((nuint) nint.MaxValue, arr.IndexOf (v2), "IndxOf 2"); // [NSArray indexOfObject:] returns NSNotFound = NSIntegerMax when object isn't found in the array
			}
		}

		[Test]
		public void AddTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";

			using (var arr = new NSMutableArray<NSString> (v1)) {
				Assert.AreEqual ((nuint) 1, arr.Count, "Count 1");
				Assert.Throws<ArgumentNullException> (() => arr.Add (null), "Add ANE");
				arr.Add (v2);
				Assert.AreEqual ((nuint) 2, arr.Count, "Count 2");
				Assert.AreSame (v2, arr [1], "idx[1]");
			}
		}

		[Test]
		public void InsertTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";
			var v3 = (NSString) "3";

			using (var arr = new NSMutableArray<NSString> (v1, v3)) {
				Assert.AreEqual ((nuint) 2, arr.Count, "Insert 1");
				Assert.Throws<ArgumentNullException> (() => arr.Insert (null, 0), "Insert ANE");
				Assert.Throws<IndexOutOfRangeException> (() => arr.Insert (v2, -1), "Insert AOORE 1");
				Assert.Throws<IndexOutOfRangeException> (() => arr.Insert (v2, 3), "Insert AOORE 2");
				arr.Insert (v2, 1);
				Assert.AreEqual ((nuint) 3, arr.Count, "Insert 2");
				Assert.AreSame (v1, arr [0], "[0]");
				Assert.AreSame (v2, arr [1], "[1]");
				Assert.AreSame (v3, arr [2], "[2]");
			}

			using (var arr = new NSMutableArray<NSString> ()) {
				Assert.DoesNotThrow (() => arr.Insert (v1, 0), "Insert into empty array");
			}
		}

		[Test]
		public void ReplaceObjectTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";
			var v3 = (NSString) "3";

			using (var arr = new NSMutableArray<NSString> (v1, v3)) {
				Assert.AreEqual ((nuint) 2, arr.Count, "ReplaceObject 1");
				Assert.AreSame (v1, arr [0], "a [0]");
				Assert.AreSame (v3, arr [1], "a [1]");
				Assert.Throws<ArgumentNullException> (() => arr.ReplaceObject (0, null), "Insert ANE");
				Assert.Throws<IndexOutOfRangeException> (() => arr.ReplaceObject (-1, v2), "Insert AOORE 1");
				Assert.Throws<IndexOutOfRangeException> (() => arr.ReplaceObject (3, v2), "Insert AOORE 2");
				arr.ReplaceObject (1, v2);
				Assert.AreEqual ((nuint) 2, arr.Count, "ReplaceObject 2");
				Assert.AreSame (v1, arr [0], "b [0]");
				Assert.AreSame (v2, arr [1], "b [1]");
			}
		}

		[Test]
		public void AddObjectsTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";
			var v3 = (NSString) "3";

			using (var arr = new NSMutableArray<NSString> ()) {
				Assert.Throws<ArgumentNullException> (() => arr.AddObjects ((NSString []) null), "AddObjects ANE 1");
				Assert.AreEqual ((nuint) 0, arr.Count, "Count 1");

				Assert.Throws<ArgumentNullException> (() => arr.AddObjects (new NSString [] { null }), "AddObjects ANE 2");
				Assert.AreEqual ((nuint) 0, arr.Count, "Count 2");

				Assert.Throws<ArgumentNullException> (() => arr.AddObjects (new NSString [] { v1, null, v3 }), "AddObjects ANE 3");
				Assert.AreEqual ((nuint) 0, arr.Count, "Count 3");

				arr.AddObjects (v1, v2);
				Assert.AreEqual ((nuint) 2, arr.Count, "AddObjects 1");
				Assert.AreSame (v1, arr [0], "a [0]");
				Assert.AreSame (v2, arr [1], "a [1]");
			}
		}

		[Test]
		public void InsertObjectsTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";
			var v3 = (NSString) "3";
			var v4 = (NSString) "4";

			using (var arr = new NSMutableArray<NSString> (v1, v2)) {
				var iset = new NSMutableIndexSet ();
				iset.Add (1);
				iset.Add (2);

				Assert.Throws<ArgumentNullException> (() => arr.InsertObjects ((NSString []) null, iset), "InsertObjects ANE 1");
				Assert.AreEqual ((nuint) 2, arr.Count, "Count 1");

				Assert.Throws<ArgumentNullException> (() => arr.InsertObjects (new NSString [] { null, null }, iset), "InsertObjects ANE 2");
				Assert.AreEqual ((nuint) 2, arr.Count, "Count 2");

				Assert.Throws<ArgumentNullException> (() => arr.InsertObjects (new NSString [] { v1, null }, iset), "InsertObjects ANE 3");
				Assert.AreEqual ((nuint) 2, arr.Count, "Count 3");

				Assert.Throws<ArgumentNullException> (() => arr.InsertObjects (new NSString [] { v1 }, null), "InsertObjects ANE 4");
				Assert.AreEqual ((nuint) 2, arr.Count, "Count 4");

				arr.InsertObjects (new NSString [] { v3, v4 }, iset);

				Assert.AreEqual ((nuint) 4, arr.Count, "InsertObjects 1");
				Assert.AreSame (v1, arr [0], "a [0]");
				Assert.AreSame (v3, arr [1], "a [1]");
				Assert.AreSame (v4, arr [2], "a [2]");
				Assert.AreSame (v2, arr [3], "a [3]");

				iset.Clear ();
				iset.Add (9);
				Assert.Throws<IndexOutOfRangeException> (() => arr.InsertObjects (new NSString [] { v1 }, iset), "InsertObjects ANE 5");
			}
		}

		[Test]
		public void IndexerTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";
			var v3 = (NSString) "3";

			using (var arr = new NSMutableArray<NSString> (v1, v2)) {
				arr [1] = v3;
				Assert.AreEqual ((nuint) 2, arr.Count, "a 1");
				Assert.AreSame (v1, arr [0], "a [0]");
				Assert.AreSame (v3, arr [1], "a [1]");

				Assert.Throws<ArgumentNullException> (() => arr [0] = null, "ANE 1");
				Assert.Throws<IndexOutOfRangeException> (() => arr [2] = v3, "IOORE 1");
			}
		}

		[Test]
		public void IEnumerableTest ()
		{
			const int C = 16 * 2 + 3; // NSFastEnumerator has a array of size 16, use more than that, and not an exact multiple.
			using (var arr = new NSMutableArray<NSString> ()) {
				for (int i = 0; i < C; i++)
					arr.Add ((NSString) i.ToString ());
				Assert.AreEqual ((nuint) C, arr.Count, "Count 1");

				var lst = new List<NSString> ();
				foreach (NSString a in (IEnumerable) arr) {
					Assert.IsNotNull (a, "null item iterator");
					Assert.IsFalse (lst.Contains (a), "duplicated item iterator");
					Assert.AreEqual (lst.Count.ToString (), (string) a, "#" + lst.Count.ToString ());
					lst.Add (a);
				}
				Assert.AreEqual (C, lst.Count, "iterator count");
			}
		}

		[Test]
		public void IEnumerable1Test ()
		{
			const int C = 16 * 2 + 3; // NSFastEnumerator has a array of size 16, use more than that, and not an exact multiple.
			using (var arr = new NSMutableArray<NSString> ()) {
				for (int i = 0; i < C; i++)
					arr.Add ((NSString) i.ToString ());
				Assert.AreEqual ((nuint) C, arr.Count, "Count 1");

				var lst = new List<NSString> ();
				foreach (var a in (IEnumerable<NSString>) arr) {
					Assert.IsNotNull (a, "null item iterator");
					Assert.IsFalse (lst.Contains (a), "duplicated item iterator");
					Assert.AreEqual (lst.Count.ToString (), (string) a, "#" + lst.Count.ToString ());
					lst.Add (a);
				}
				Assert.AreEqual (C, lst.Count, "iterator count");
			}
		}
	}
}
