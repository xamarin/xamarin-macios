//
// Unit tests for EKReminder
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using System;
#if XAMCORE_2_0
using Foundation;
using EventKit;
using ObjCRuntime;
#else
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
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

namespace MonoTouchFixtures.EventKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ReminderTest
	{
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);
		}

		[Test]
		public void DefaultProperties ()
		{
			using (var rem = new EKReminder ()) {
				Assert.AreEqual (0, rem.Priority, "Priority");
				Assert.IsFalse (rem.Completed, "Completed");
				Assert.IsNull (rem.CompletionDate, "CompletionDate");
				Assert.IsNull (rem.StartDateComponents, "StartDateComponents");
				Assert.IsNull (rem.DueDateComponents, "DueDateComponents");

				rem.Completed = true;
				Assert.IsTrue (rem.Completed, "Completed - Changed");
			}
		}

		[Test]
		public void NullableProperties ()
		{
			using (var rem = new EKReminder ()) {
				rem.StartDateComponents = null;
				rem.DueDateComponents = null;
				rem.CompletionDate = null;
			}
		}

		[Test]
		public void Range ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);

			using (var rem = new EKReminder ()) {
				// priority is documented to have a range of 0-9 but there's no validation in ObjC
				// this test is here to ensure Apple does not start throwing native exceptions at some points
				rem.Priority = -1;
				Assert.That (rem.Priority, Is.EqualTo ((nint) (-1)), "-1");
				rem.Priority = 10;
				Assert.That (rem.Priority, Is.EqualTo ((nint) 10), "10");

				// The following tests fail randomly.
//				// at some point values are ?normalized? but no exception is thrown
//				rem.Priority = nint.MinValue;
//				Assert.That (rem.Priority, Is.EqualTo ((nint) 0), "MinValue");
//				// exposed as an NSInteger but internal storage looks different
//				rem.Priority = nint.MaxValue;
//				Assert.That (rem.Priority, Is.EqualTo ((nint) (-1)), "MaxValue");
			}
		}
	}
}

#endif // !__TVOS__
