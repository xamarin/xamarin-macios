//
// Unit tests for EKCalendar
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using CoreGraphics;
using ObjCRuntime;
using EventKit;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.EventKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EventKitCalendarTest {

		// note: default .ctor disable since it would thrown an objective-c exception telling us to use 'calendarWithEventStore:'
		[Test]
		public void FromEventStore ()
		{
			EKEventStore store = new EKEventStore ();
			var c = EKCalendar.FromEventStore (store);
			// defaults
			Assert.True (c.AllowsContentModifications, "AllowsContentModifications");
			Assert.NotNull (c.CalendarIdentifier, "CalendarIdentifier");
			Assert.Null (c.CGColor, "CGColor");

			if (TestRuntime.CheckXcodeVersion (4, 5)) {
				// default value changed for iOS 6.0 beta 1
				Assert.False (c.Immutable, "Immutable");
				// new in 6.0
				Assert.AreEqual (EKEntityMask.Event, c.AllowedEntityTypes, "AllowedEntityTypes");
			} else {
				Assert.True (c.Immutable, "Immutable");
			}

			Assert.Null (c.Source, "Source");
			Assert.False (c.Subscribed, "Subscribed");
			Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.None), "SupportedEventAvailabilities");
			Assert.Null (c.Title, "Title");
			Assert.That (c.Type, Is.EqualTo (EKCalendarType.Local), "Type");
		}

		[Test]
		public void FromEventStoreWithReminder ()
		{
			if (!TestRuntime.CheckXcodeVersion (4, 5))
				Assert.Inconclusive ("+[EKCalendar calendarForEntityType:eventStore:]: unrecognized selector before 6.0");

			var c = EKCalendar.Create (EKEntityType.Reminder, new EKEventStore ());
			// defaults
			Assert.True (c.AllowsContentModifications, "AllowsContentModifications");
			Assert.NotNull (c.CalendarIdentifier, "CalendarIdentifier");
			Assert.Null (c.CGColor, "CGColor");

			Assert.False (c.Immutable, "Immutable");
			Assert.Null (c.Source, "Source");
			Assert.False (c.Subscribed, "Subscribed");
			Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.None), "SupportedEventAvailabilities");
			Assert.Null (c.Title, "Title");
			Assert.That (c.Type, Is.EqualTo (EKCalendarType.Local), "Type");
			Assert.AreEqual (EKEntityMask.Reminder, c.AllowedEntityTypes, "AllowedEntityTypes");
			Assert.IsNotNull (c.CalendarIdentifier, "CalendarIdentifier");
		}

		[Test]
		public void Title ()
		{
			EKEventStore store = new EKEventStore ();
			var c = EKCalendar.FromEventStore (store);
			c.Title = "my title";
			Assert.That (c.Title, Is.EqualTo ("my title"), "Title");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void FromEventStore_Null ()
		{
			EKCalendar.FromEventStore (null);
		}
	}
}

#endif // !__TVOS__
