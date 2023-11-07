//
// Unit tests for NSArray
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Linq;
using Foundation;
using ObjCRuntime;
using Security;
using NUnit.Framework;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSArrayTest {

		[Test]
		public void FromStrings_Null ()
		{
			Assert.Throws<ArgumentNullException> (() => NSArray.FromStrings (null), "null");

			using (var a = NSArray.FromStrings (new string [1])) {
				Assert.That (a.Count, Is.EqualTo ((nuint) 1), "null item");
				Assert.IsNull (a.GetItem<NSString> (0), "0");
			}
		}

		[Test]
		public void Null ()
		{
			using (var a = NSArray.FromNSObjects (NSNull.Null)) {
				Assert.That (a.Count, Is.EqualTo ((nuint) 1), "Count");
				Assert.IsNull (a.GetItem<NSNull> (0), "0");
			}
		}

		int comparator_count;

		// the new NSObject are often, but not always, in ascending order 
		// (because of how we allocate them) so we sort the other way
		NSComparisonResult Comparator (NSObject obj1, NSObject obj2)
		{
			comparator_count++;
#if NET
			return (NSComparisonResult) (((long) (IntPtr) obj2.Handle - (long) (IntPtr) obj1.Handle));
#else
			return (NSComparisonResult) (long) ((nint) obj2.Handle - (nint) obj1.Handle);
#endif
		}

		[Test]
		public void Sort ()
		{
			comparator_count = 0;
			using (var obj1 = new NSObject ())
			using (var obj2 = new NSObject ())
			using (var a = new NSMutableArray ()) {
				a.Add (obj1);
				a.Add (a);
				a.Add (obj2);
				using (var s = a.Sort (Comparator)) {
					Assert.That ((long) (IntPtr) s.ValueAt (0), Is.GreaterThan ((long) (IntPtr) s.ValueAt (1)), "0");
					Assert.That ((long) (IntPtr) s.ValueAt (1), Is.GreaterThan ((long) (IntPtr) s.ValueAt (2)), "1");
				}
			}
			Assert.That (comparator_count, Is.GreaterThanOrEqualTo (2), "2+");
		}

		int evaluator_count;

		bool Evaluator (NSObject evaluatedObject, NSDictionary bindings)
		{
			evaluator_count++;
			return (evaluatedObject is NSMutableArray);
		}

		[Test]
		public void Filter ()
		{
			evaluator_count = 0;
			using (var obj1 = new NSObject ())
			using (var obj2 = new NSObject ())
			using (var a = new NSMutableArray ()) {
				a.Add (obj1);
				a.Add (a);
				a.Add (obj2);
				using (NSPredicate p = NSPredicate.FromExpression (Evaluator))
				using (var f = a.Filter (p)) {
					Assert.That (f.ValueAt (0), Is.EqualTo (a.Handle), "0");
					Assert.That (f.Count, Is.EqualTo ((nuint) 1), "Count");
				}
			}
			Assert.That (evaluator_count, Is.EqualTo (3), "3");
		}

		[Test]
		public void INativeObjects ()
		{
			using (var policy = SecPolicy.CreateSslPolicy (true, "mail.xamarin.com")) {
				using (var a = NSArray.FromObjects (policy)) {
					var b = NSArray.ArrayFromHandle<SecPolicy> (a.Handle);
					Assert.AreNotSame (a, b);
				}
			}
		}

		[Test]
		public void FromNSObjects ()
		{
			using (var a = NSArray.FromNSObjects (null)) {
				// on the managed side we have an empty array
				Assert.That (a.Count, Is.EqualTo ((nuint) 0), "Count");
				// and a valid native instance (or some other API might fail)
				Assert.That (a.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void ToArray ()
		{
			using (var a = NSArray.FromStrings (new string [1] { "abc" })) {
				var arr = a.ToArray ();
				Assert.AreEqual (1, arr.Length, "Length");
				Assert.AreEqual ("abc", arr [0].ToString (), "Value");
			}
		}

		[Test]
		public void ToArray_T ()
		{
			using (var a = NSArray.FromStrings (new string [1] { "abc" })) {
				var arr = a.ToArray<NSString> ();
				Assert.AreEqual (1, arr.Length, "Length");
				Assert.AreEqual ("abc", arr [0].ToString (), "Value");
			}
		}

		[Test]
		public void Enumerator ()
		{
			using (var a = NSArray.FromStrings (new string [1] { "abc" })) {
				foreach (var item in a)
					Assert.AreEqual ("abc", item.ToString (), "Value");
				var list = a.ToList ();
				Assert.AreEqual (1, list.Count (), "Length");
				Assert.AreEqual ("abc", list [0].ToString (), "Value");
			}
		}
	}
}
