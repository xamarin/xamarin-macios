//
// Unit tests for CFNotificationCenter
//
// Authors:
//	Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Threading;

using Foundation;
using CoreFoundation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CFNotificationCenterTest {
		[Test]
		public void TestObservers ()
		{
			var target = new NSObject ();
			var d = CFNotificationCenter.Local;
			int count = 0;
			int count2 = 0;
			CFNotificationObserverToken o2 = null;
			var o1 = d.AddObserver ("hello", target, (x, dd) => {
				count++;
				//				Console.WriteLine ("Here");

				if (count == 1)
					o2 = d.AddObserver ("hello", target, (y, ee) => {
						//						Console.WriteLine ("There");
						count2++;
					});
			});
			d.PostNotification ("hello", target, null, deliverImmediately: true);
			Assert.AreEqual (1, count);
			d.PostNotification ("hello", target, null, deliverImmediately: true);
			Assert.AreEqual (2, count);
			Assert.AreEqual (1, count2);

			// Remove the first observer, count should not be updated
			d.RemoveObserver (o1);
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (2, count);
			Assert.AreEqual (2, count2);

			// Remove the last observer, there should be no change in count
			d.RemoveObserver (o2);
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (2, count);
			Assert.AreEqual (2, count2);

			// Test removing all observers
			count = 0;
			o1 = d.AddObserver ("hello", target, (x, dd) => {
				count++;
				Console.WriteLine ("Here");
			});
			o2 = d.AddObserver ("hello", target, (y, ee) => { count++; });
			d.RemoveEveryObserver ();
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (0, count);

			// Test removing from a callback
			count = 0;
			o2 = d.AddObserver ("hello", target, (y, ee) => { count++; d.RemoveObserver (o2); });
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (1, count);
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (1, count);
		}

		[Test]
		public void TestNullNameAndObserver ()
		{
			var d = CFNotificationCenter.Local;
			var mornNotification = new ManualResetEvent (false);

			var token = d.AddObserver (null, null, (n, i) => {
				if (n == "MornNotification")
					mornNotification.Set ();
			});

			// When not listening for a specific name nor observing an specific object
			// we will get all notifications posted to NSNotificationCenter/Local CFNotificationCenter
			NSNotificationCenter.DefaultCenter.PostNotificationName ("MornNotification", null);

			d.RemoveObserver (token);
			Assert.IsTrue (mornNotification.WaitOne (TimeSpan.FromSeconds (10)), "Didn't get a notification after waiting 10 seconds.");
		}

		[Test]
		public void TestObservers2 ()
		{
			var d = CFNotificationCenter.Local;
			int count = 0;
			int count2 = 0;
			CFNotificationObserverToken o2 = null;
			var o1 = d.AddObserver (null, null, (x, dd) => {
				count++;
				if (count == 1)
					o2 = d.AddObserver (null, null, (y, ee) => {
						count2++;
					});
			});
			d.PostNotification ("hello", null, null, deliverImmediately: true);
			Assert.AreEqual (1, count);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("hello", null);
			Assert.AreEqual (2, count);
			Assert.AreEqual (1, count2);

			// Remove the first observer, count should not be updated
			d.RemoveObserver (o1);
			d.PostNotification ("hello", null, null);
			Assert.AreEqual (2, count);
			Assert.AreEqual (2, count2);

			// Remove the last observer, there should be no change in count
			d.RemoveObserver (o2);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("hello", null);
			Assert.AreEqual (2, count);
			Assert.AreEqual (2, count2);

			// Test removing all observers
			count = 0;
			o1 = d.AddObserver (null, null, (x, dd) => {
				count++;
			});
			o2 = d.AddObserver (null, null, (y, ee) => { count++; });
			d.RemoveEveryObserver ();
			NSNotificationCenter.DefaultCenter.PostNotificationName ("hello", null);
			Assert.AreEqual (0, count);

			// Test removing from a callback
			count = 0;
			o2 = d.AddObserver (null, null, (y, ee) => { count++; d.RemoveObserver (o2); });
			d.PostNotification ("hello", null, null);
			Assert.AreEqual (1, count);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("hello", null);
			Assert.AreEqual (1, count);
		}
	}
}
