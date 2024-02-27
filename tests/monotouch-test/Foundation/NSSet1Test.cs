using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using Foundation;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSSet1Test {

		[Test]
		public void Ctor ()
		{
			using (var arr = new NSSet<NSDate> ()) {
				Assert.AreEqual ((nuint) 0, arr.Count, "Count");
			}
		}

		[Test]
		public void Ctor_Params ()
		{
			using (var arr = new NSSet<NSString> ((NSString) "foo")) {
				Assert.AreEqual ((nuint) 1, arr.Count, "Count");
			}
			using (var arr = new NSSet<NSString> ((NSString) "foo", (NSString) "bar")) {
				Assert.AreEqual ((nuint) 2, arr.Count, "Count");
			}
		}

		[Test]
		public void Ctor_OtherSet ()
		{
			var v1 = (NSString) "1";

			using (var first = new NSSet<NSString> (v1)) {
				using (var second = new NSSet<NSString> (first)) {
					Assert.AreEqual ((nuint) 1, first.Count, "1 count");
					Assert.AreEqual ((nuint) 1, second.Count, "2 count");
				}
			}
		}

		[Test]
		public void Ctor_OtherMutableSet ()
		{
			var v1 = (NSString) "1";

			using (var first = new NSMutableSet<NSString> (v1)) {
				using (var second = new NSSet<NSString> (first)) {
					Assert.AreEqual ((nuint) 1, first.Count, "1 count");
					Assert.AreEqual ((nuint) 1, second.Count, "2 count");
				}
			}
		}

		[Test]
		public void LookupMemberTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";

			using (var st = new NSSet<NSString> (v1)) {
				Assert.Throws<ArgumentNullException> (() => st.LookupMember ((NSString) null), "LookupMember ANE 1");
				Assert.AreSame (v1, st.LookupMember (v1), "LookupMember 1");
				Assert.IsNull (st.LookupMember (v2), "LookupMember 2");
			}
		}

		[Test]
		public void AnyObjectTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";

			using (var st = new NSSet<NSString> ()) {
				Assert.IsNull (st.AnyObject, "AnyObject 1");
			}

			using (var st = new NSSet<NSString> (v1)) {
				Assert.AreSame (v1, st.AnyObject, "AnyObject 2");
			}
		}

		[Test]
		public void ContainsTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";

			using (var st = new NSSet<NSString> (v1)) {
				Assert.Throws<ArgumentNullException> (() => st.Contains ((NSString) null), "Contains ANE 1");
				Assert.IsTrue (st.Contains (v1), "Contains 1");
				Assert.IsFalse (st.Contains (v2), "Contains 2");
			}
		}

		[Test]
		public void ToArrayTest ()
		{
			var v1 = (NSString) "1";

			using (var st = new NSSet<NSString> (v1)) {
				var arr = st.ToArray ();
				Assert.AreEqual (1, arr.Length, "ToArray Length");
				Assert.AreSame (v1, arr [0], "ToArray () [0]");
			}
		}

		[Test]
		public void OperatorAddTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";

			using (var first = new NSSet<NSString> (v1)) {
				using (var second = new NSSet<NSString> (v2)) {
					using (var third = first + second) {
						Assert.AreEqual ((nuint) 2, third.Count, "+ Count");
						Assert.IsTrue (third.Contains (v1), "+ 1");
						Assert.IsTrue (third.Contains (v2), "+ 2");
					}
				}
			}
		}

		[Test]
		public void OperatorSubtractTest ()
		{
			var v1 = (NSString) "1";
			var v2 = (NSString) "2";

			using (var first = new NSSet<NSString> (v1, v2)) {
				using (var second = new NSSet<NSString> (v2)) {
					using (var third = first - second) {
						Assert.AreEqual ((nuint) 1, third.Count, "- Count");
						Assert.IsTrue (third.Contains (v1), "- 1");
					}
				}
			}
		}

		[Test]
		public void IEnumerable1Test ()
		{
			const int C = 16 * 2 + 3; // NSFastEnumerator has a array of size 16, use more than that, and not an exact multiple.
			var values = new NSString [C];
			for (int i = 0; i < C; i++)
				values [i] = (NSString) i.ToString ();

			using (var st = new NSSet<NSString> (values)) {
				Assert.AreEqual ((nuint) C, st.Count, "Count 1");

				var lst = new List<NSString> ();
				foreach (var a in (IEnumerable<NSString>) st) {
					Assert.IsNotNull (a, "null item iterator");
					Assert.IsFalse (lst.Contains (a), "duplicated item iterator");
					lst.Add (a);
					Assert.IsTrue (Array.IndexOf (values, a) >= 0, "different object");
				}
				Assert.AreEqual (C, lst.Count, "iterator count");
			}
		}

		[Test]
		public void IEnumerableTest ()
		{
			const int C = 16 * 2 + 3; // NSFastEnumerator has a array of size 16, use more than that, and not an exact multiple.
			var values = new NSString [C];
			for (int i = 0; i < C; i++)
				values [i] = (NSString) i.ToString ();

			using (var st = new NSSet<NSString> (values)) {
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
		}

		[Test]
		public void OperatorPlusReferenceTest ()
		{
			var one = new NSSet<NSString> ((NSString) "1", (NSString) "2", (NSString) "3");
			var two = new NSSet<NSString> ((NSString) "4", (NSString) "5", (NSString) "6");
			NSSet<NSString> nil = null;
			using (var sum = one + nil)
			using (var sum2 = two + one)
			using (var sum3 = one + two) {

			}
			Assert.AreNotEqual (IntPtr.Zero, one.Handle, "Handle must be != IntPtr.Zero");
			Assert.AreNotEqual (IntPtr.Zero, two.Handle, "Handle must be != IntPtr.Zero");
		}
	}
}
