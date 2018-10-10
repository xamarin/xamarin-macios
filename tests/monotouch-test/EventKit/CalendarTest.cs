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
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);
		}

		void RequestPermission ()
		{
#if __MACOS__ && __UNIFIED__
			TestRuntime.RequestEventStorePermission (EKEntityType.Event, true);
			TestRuntime.RequestEventStorePermission (EKEntityType.Reminder, true);
#endif
		}

		// note: default .ctor disable since it would thrown an objective-c exception telling us to use 'calendarWithEventStore:'
		[Test]
		public void FromEventStore ()
		{
			RequestPermission ();

			EKEventStore store = new EKEventStore ();
#if MONOMAC
			var c = EKCalendar.Create (EKEntityType.Event, store);
#else
			var c = EKCalendar.FromEventStore (store);
#endif
			// defaults
#if __WATCHOS__
			Assert.False (c.AllowsContentModifications, "AllowsContentModifications");
#else
			Assert.True (c.AllowsContentModifications, "AllowsContentModifications");
#endif
			Assert.NotNull (c.CalendarIdentifier, "CalendarIdentifier");
#if MONOMAC
			Assert.Null (c.Color, "Color");
#else
			Assert.Null (c.CGColor, "CGColor");
#endif

			if (TestRuntime.CheckXcodeVersion (4, 5)) {
				// default value changed for iOS 6.0 beta 1
#if __WATCHOS__
				Assert.True (c.Immutable, "Immutable");
#else
				Assert.False (c.Immutable, "Immutable");
#endif
				// new in 6.0
				Assert.AreEqual (EKEntityMask.Event, c.AllowedEntityTypes, "AllowedEntityTypes");
			} else {
				Assert.True (c.Immutable, "Immutable");
			}

			Assert.Null (c.Source, "Source");
			Assert.False (c.Subscribed, "Subscribed");
#if MONOMAC
			Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.Busy | EKCalendarEventAvailability.Free), "SupportedEventAvailabilities");
			Assert.That (c.Title, Is.EqualTo (string.Empty), "Title");
#else
			Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.None), "SupportedEventAvailabilities");
			Assert.Null (c.Title, "Title");
#endif
			Assert.That (c.Type, Is.EqualTo (EKCalendarType.Local), "Type");
		}

		[Test]
		public void FromEventStoreWithReminder ()
		{
			RequestPermission ();

			if (!TestRuntime.CheckXcodeVersion (4, 5))
				Assert.Inconclusive ("+[EKCalendar calendarForEntityType:eventStore:]: unrecognized selector before 6.0");

			var c = EKCalendar.Create (EKEntityType.Reminder, new EKEventStore ());
			// defaults
#if __WATCHOS__
			Assert.False (c.AllowsContentModifications, "AllowsContentModifications");
#else
			Assert.True (c.AllowsContentModifications, "AllowsContentModifications");
#endif
			Assert.NotNull (c.CalendarIdentifier, "CalendarIdentifier");
#if MONOMAC
			Assert.Null (c.Color, "Color");
#else
			Assert.Null (c.CGColor, "CGColor");
#endif

#if __WATCHOS__
			Assert.True (c.Immutable, "Immutable");
#else
			Assert.False (c.Immutable, "Immutable");
#endif
			Assert.Null (c.Source, "Source");
			Assert.False (c.Subscribed, "Subscribed");
#if MONOMAC
			Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.Busy | EKCalendarEventAvailability.Free), "SupportedEventAvailabilities");
			Assert.That (c.Title, Is.EqualTo (string.Empty), "Title");
#else
			Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.None), "SupportedEventAvailabilities");
			Assert.Null (c.Title, "Title");
#endif
			Assert.That (c.Type, Is.EqualTo (EKCalendarType.Local), "Type");
			Assert.AreEqual (EKEntityMask.Reminder, c.AllowedEntityTypes, "AllowedEntityTypes");
			Assert.IsNotNull (c.CalendarIdentifier, "CalendarIdentifier");
		}

		[Test]
		public void Title ()
		{
			RequestPermission ();

			EKEventStore store = new EKEventStore ();
#if MONOMAC
			var c = EKCalendar.Create (EKEntityType.Event, store);
#else
			var c = EKCalendar.FromEventStore (store);
#endif
			c.Title = "my title";
			Assert.That (c.Title, Is.EqualTo ("my title"), "Title");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void FromEventStore_Null ()
		{
#if MONOMAC
			EKCalendar.Create (EKEntityType.Event, null);
#else
			EKCalendar.FromEventStore (null);
#endif
		}
	}
}

#endif // !__TVOS__
