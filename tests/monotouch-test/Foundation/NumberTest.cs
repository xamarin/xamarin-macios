//
// Unit tests for NSNumber
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NumberTest {

		[Test]
		public void CompareTo ()
		{
			NSNumber n0 = (NSNumber) 0;
			NSNumber n1 = (NSNumber) 1;
			NSNumber n1f = (NSNumber) 1.0f;
			NSNumber n2 = (NSNumber) 2.0f;

			Assert.That (n1.Compare (n1f), Is.EqualTo ((nint) 0), "Compare-a");
			Assert.That (n0.Compare (n1f), Is.LessThan ((nint) 0), "Compare-b");
			Assert.That (n2.Compare (n0), Is.GreaterThan ((nint) 0), "Compare-c");

			// IComparable
			Assert.That (n1.CompareTo ((object) n1f), Is.EqualTo (0), "CompareTo-a");
			Assert.That (n0.CompareTo ((object) n1f), Is.LessThan (0), "CompareTo-b");
			Assert.That (n2.CompareTo ((object) n0), Is.GreaterThan (0), "CompareTo-c");

			// IComparable<NSNumber>
			Assert.That (n1.CompareTo (n1f), Is.EqualTo (0), "CompareTo<NSNumber>-a");
			Assert.That (n0.CompareTo (n1f), Is.LessThan (0), "CompareTo<NSNumber>-b");
			Assert.That (n2.CompareTo (n0), Is.GreaterThan (0), "CompareTo<NSNumber>-c");
		}

		[Test]
		public void CtorNSCoder ()
		{
			// NSNumber conforms to NSCoding - so it's .ctor(NSCoder) is usable
			using (var n = new NSNumber (-1))
			using (var d = new NSMutableData ()) {
				using (var a = new NSKeyedArchiver (d)) {
					n.EncodeTo (a);
					a.FinishEncoding ();
				}
				using (var u = new NSKeyedUnarchiver (d))
				using (var n2 = new NSNumber (u)) {
					// so we can re-create an instance from it
					Assert.That (n.Int32Value, Is.EqualTo (-1), "Value");
				}
			}
		}

		[Test]
		public void Singleton ()
		{
			var a = new NSNumber (true);
			var b = new NSNumber (true);
			a.Dispose ();
			Assert.That (b.Handle, Is.Not.EqualTo (IntPtr.Zero));
		}

		[Test]
		public void Equals ()
		{
			using (var a = new NSNumber (1))
			using (var b = new NSNumber (1d)) {
				// Two objects that are equal return hash codes that are equal.
				Assert.True (a.Equals (b), "Equals(NSNumber)");
				Assert.True (b.Equals ((object) a), "Equals(Object)");
				Assert.That (a.GetHashCode (), Is.EqualTo (b.GetHashCode ()), "GetHashCode");
			}
		}
	}
}
