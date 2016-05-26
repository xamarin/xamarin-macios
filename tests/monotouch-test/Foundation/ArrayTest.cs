//
// Unit tests for NSArray
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using Security;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.Security;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.Foundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSArrayTest {
		
		[Test]
		public void FromStrings_NullItem ()
		{
			try {
				NSArray.FromStrings (new string [1]);
			}
			catch (Exception e) {
				Assert.Fail ("Unexpected exception {0}", e);
			}
		}
		
		int comparator_count;

		// the new NSObject are often, but not always, in ascending order 
		// (because of how we allocate them) so we sort the other way
#if XAMCORE_2_0
		NSComparisonResult
#else
		int 
#endif
		Comparator (NSObject obj1, NSObject obj2)
		{
			comparator_count++;
			return 
#if XAMCORE_2_0
			(NSComparisonResult)
#endif
				(int) ((nint) obj2.Handle - (nint) obj1.Handle);
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
					Assert.That ((nuint) s.ValueAt (0), Is.GreaterThan ((nuint) s.ValueAt (1)), "0");
					Assert.That ((nuint) s.ValueAt (1), Is.GreaterThan ((nuint) s.ValueAt (2)), "1");
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
				// if we provide the handle to the ObjC it will be `nil`
				Assert.That (a.Handle, Is.EqualTo (IntPtr.Zero), "Handle");
			}
		}
	}
}
