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
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using EventKit;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.EventKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EventKitCalendarTest {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
		}

		void RequestPermission ()
		{
#if __MACOS__ || __MACCATALYST__
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
#if MONOMAC || __MACCATALYST__
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

			Assert.That (c.Immutable, Is.EqualTo (true).Or.EqualTo (false), "Immutable");
			Assert.AreEqual (EKEntityMask.Event, c.AllowedEntityTypes, "AllowedEntityTypes");

			Assert.Null (c.Source, "Source");
			Assert.False (c.Subscribed, "Subscribed");
#if MONOMAC || __MACCATALYST__
			if (TestRuntime.CheckXcodeVersion (14, 0))
				Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.None), "SupportedEventAvailabilities");
			else
				Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.Busy | EKCalendarEventAvailability.Free), "SupportedEventAvailabilities");

			Assert.That (c.Title, Is.EqualTo (string.Empty), "Title");
#else
			Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.None), "SupportedEventAvailabilities");
			if (TestRuntime.CheckXcodeVersion (13, 2))
				Assert.That (c.Title, Is.EqualTo (string.Empty), "Title");
			else
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
#if MONOMAC || __MACCATALYST__
			if (TestRuntime.CheckXcodeVersion (14, 0))
				Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.None), "SupportedEventAvailabilities");
			else
				Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.Busy | EKCalendarEventAvailability.Free), "SupportedEventAvailabilities");

			Assert.That (c.Title, Is.EqualTo (string.Empty), "Title");
#else
			Assert.That (c.SupportedEventAvailabilities, Is.EqualTo (EKCalendarEventAvailability.None), "SupportedEventAvailabilities");
			if (TestRuntime.CheckXcodeVersion (13, 2))
				Assert.That (c.Title, Is.EqualTo (string.Empty), "Title");
			else
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
#if MONOMAC || __MACCATALYST__
			var c = EKCalendar.Create (EKEntityType.Event, store);
#else
			var c = EKCalendar.FromEventStore (store);
#endif
			c.Title = "my title";
			Assert.That (c.Title, Is.EqualTo ("my title"), "Title");
		}

		[Test]
		public void FromEventStore_Null ()
		{
#if MONOMAC || __MACCATALYST__
			Assert.Throws<ArgumentNullException> (() => EKCalendar.Create (EKEntityType.Event, null));
#else
			Assert.Throws<ArgumentNullException> (() => EKCalendar.FromEventStore (null));
#endif
		}
	}
}

#endif // !__TVOS__
