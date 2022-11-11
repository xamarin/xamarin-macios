//
// Unit tests for NotificationCenter
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Diagnostics;
using System.Threading;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NotificationCenterTest {

		[Test]
		[Ignore ("This test is 'randomly' failing the first time it's executed with debugging disabled (if executed with the rest of the tests) - CWLS show that the TestNotification instance is freed at the end of the test run.")]
		public void Free ()
		{
			bool freed = false;
			bool a = false, b = false;
			Action<TestNotification> destroyedCallback = delegate (TestNotification n) { freed = true; a = n.A; b = n.B; };
			FreeLeaf (destroyedCallback);
			Stopwatch watch = new Stopwatch ();
			watch.Start ();
			while (!freed && watch.ElapsedMilliseconds < 1000) {
				GC.Collect (GC.MaxGeneration);
				Thread.Sleep (10);
			}
			Assert.True (a, "a");
			Assert.True (b, "b");
			Assert.True (freed, "freed");
		}

		void FreeLeaf (Action<TestNotification> destroyedCallback)
		{
			var testNotification = new TestNotification (destroyedCallback);

			NSNotificationCenter.DefaultCenter.PostNotificationName ("notifyA", null);
			NSNotificationCenter.DefaultCenter.PostNotificationName ("notifyB", null);

			testNotification.StopObserving ();
		}

		public class TestNotification : NSObject {
			public bool A;
			public bool B;
			Action<TestNotification> destroyed_callback;

			public TestNotification (Action<TestNotification> destroyedCallback)
			{
				destroyed_callback = destroyedCallback;

				NSNotificationCenter.DefaultCenter.AddObserver (this,
															   new Selector ("a:"),
															   new NSString ("notifyA"),
															   null);

				NSNotificationCenter.DefaultCenter.AddObserver (this,
															   new Selector ("b:"),
															   new NSString ("notifyB"),
															   null);
			}

			[Export ("a:")]
			public void a (NSNotification notification)
			{
				A = true;
			}

			[Export ("b:")]
			public void b (NSNotification notification)
			{
				B = true;
			}

			public void StopObserving ()
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver (this);
			}

			~TestNotification ()
			{
				destroyed_callback (this);
			}

		}

#if __IOS__
		[Test]
		public void TargetedNotificationsTest ()
		{
			bool called = false;
			using (var txt = new global::UIKit.UITextField ())
			using (var notification = global::UIKit.UITextField.Notifications.ObserveTextFieldTextDidChange (txt, (sender, e) => called = true)) {
				NSNotificationCenter.DefaultCenter.PostNotificationName (global::UIKit.UITextField.TextFieldTextDidChangeNotification, null);
				Assert.False (called, "Notification should not be called");

				NSNotificationCenter.DefaultCenter.PostNotificationName (global::UIKit.UITextField.TextFieldTextDidChangeNotification, txt);
				Assert.True (called, "Notification should have been called");
			}
		}
#endif

		[Test]
		public void ThreadSafe ()
		{
			var threadCount = 10;
			var threads = new Thread [threadCount];
			var startSignal = new ManualResetEvent (false);
			var stopSignal = new ManualResetEvent (false);
			var center = NSNotificationCenter.DefaultCenter;
			var name = (NSString) "dummyKey";
			var callback = new Action<NSNotification> ((v) => { });
			Exception ex = null;

			// Add and Remove observers from multiple threads at the same time.
			for (var i = 0; i < threadCount; i++) {
				threads [i] = new Thread ((id) => {
					startSignal.WaitOne ();
					try {
						while (!stopSignal.WaitOne (0)) {
							var obj = center.AddObserver (name, callback);
							center.RemoveObserver (obj);
						}
					} catch (Exception e) {
						ex = e;
					}
				});
				threads [i].IsBackground = true;
				threads [i].Start (i);
			}
			// Ready, GO!
			startSignal.Set ();
			// Full sprint for 0.1s, no need to run longer since the bug triggers easily enough.
			Thread.Sleep (TimeSpan.FromSeconds (0.1));
			// OK, we're done now, time to stop.
			stopSignal.Set ();
			for (var i = 0; i < threadCount; i++) {
				threads [i].Join ();
			}
			Assert.IsNull (ex, "Exception");
		}
	}
}
