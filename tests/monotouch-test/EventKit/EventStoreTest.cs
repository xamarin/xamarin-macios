//
// Unit tests for EKEventStore
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using System;
using Foundation;
using ObjCRuntime;
using EventKit;
using NUnit.Framework;
using System.Threading;
using System.Linq;

namespace MonoTouchFixtures.EventKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EventStoreTest {
		[Test]
		[Ignore ("fail on a cleaned iOS 6 simulator and (differently) on devices")]
		public void DefaultCalendar ()
		{
			var store = new EKEventStore ();
			Assert.AreEqual ("Calendar", store.DefaultCalendarForNewEvents.Title, "DefaultCalendarForNewEvents");
			Assert.IsNull (store.DefaultCalendarForNewReminders, "DefaultCalendarForNewReminders");
#if !MONOMAC // Not available on Mac
			Assert.IsNotNull (store.Calendars, "Calendars");
#endif
			Assert.IsNotNull (store.Sources, "Sources");
		}

#if false
		// The EKEventStore constructor is no longer availble on iOS 6 Beta 4

		[Test]
		[Ignore ("fail on a cleaned iOS 6 simulator and (differently) on devices")]
		public void DefaultReminder ()
		{
			var store = new EKEventStore (EKEntityMask.Reminder);
			Assert.AreEqual ("Reminders", store.DefaultCalendarForNewReminders.Title, "DefaultCalendarForNewReminders");
			Assert.IsNull (store.DefaultCalendarForNewEvents, "DefaultCalendarForNewEvents");
			Assert.IsNotNull (store.Calendars, "Calendars");
			Assert.IsNotNull (store.Sources, "Sources");
		}

		[Test]
		[Ignore ("fail on a cleaned iOS 6 simulator and (differently) on devices")]
		public void GetCalendars ()
		{
			var store = new EKEventStore (EKEntityMask.Reminder);
			var calendars = store.GetCalendars (EKEntityType.Reminder);
			Assert.AreEqual ("Reminders", calendars[0].Title, "#1");

			calendars = store.GetCalendars (EKEntityType.Event);
			Assert.AreEqual (0, calendars.Length, "#2");
		}

		[Test]
		public void Predicates()
		{
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Inconclusive ("defaults are different on devices");

			var store = new EKEventStore (EKEntityMask.Reminder);
			var rem = EKReminder.Create (store);
			rem.Calendar = store.DefaultCalendarForNewReminders;

			NSError error;
			Assert.IsTrue (store.SaveReminder (rem, true, out error), "SaveReminder");

			var predicate = store.PredicateForIncompleteReminders (null, null, new [] { rem.Calendar });
			var mre = new ManualResetEvent (false);
			bool found = false;
			store.FetchReminders (predicate, l => {
				found = l.Any (ll => ll.ClassHandle == rem.ClassHandle);
				mre.Set ();
			});

			Assert.IsTrue (mre.WaitOne (3000), "#1");
			Assert.IsTrue (found, "#2");

			mre.Reset ();
			predicate = store.PredicateForReminders (null);

			store.FetchReminders (predicate, l => mre.Set ());
			Assert.IsTrue (mre.WaitOne (3000), "#10");

			mre.Reset ();
			predicate = store.PredicateForCompleteReminders (null, null, null);

			store.FetchReminders (predicate, l => mre.Set ());
			Assert.IsTrue (mre.WaitOne (3000), "#20");

			Assert.IsTrue (store.RemoveReminder (rem, true, out error), "RemoveReminder");
		}
#endif
	}
}

#endif // !__TVOS__
