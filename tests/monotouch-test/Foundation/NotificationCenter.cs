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
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
#endif
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

		public class TestNotification : NSObject 
		{
			public bool A;
			public bool B;
			Action<TestNotification> destroyed_callback;

			public TestNotification (Action<TestNotification> destroyedCallback)
			{
				destroyed_callback = destroyedCallback;

				NSNotificationCenter.DefaultCenter.AddObserver(this,
				                                               new Selector("a:"),
				                                               new NSString("notifyA"),
				                                               null);
				
				NSNotificationCenter.DefaultCenter.AddObserver(this,
				                                               new Selector("b:"),
				                                               new NSString("notifyB"),
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
	
			~TestNotification() 
			{
				destroyed_callback (this);
			}
	
		}

#if XAMCORE_2_0 && __IOS__
		[Test]
		public void TargetedNotificationsTest ()
		{
			bool called = false;
			using (var txt = new global::UIKit.UITextField ())
			using (var notification = global::UIKit.UITextField.Notifications.ObserveTextFieldTextDidChange (txt, (sender, e) => called = true)) {
				NSNotificationCenter.DefaultCenter.PostNotificationName (global::UIKit.UITextField.TextFieldTextDidChangeNotification, null);
				Assert.False (called, "Notification should not be called");

				NSNotificationCenter.DefaultCenter.PostNotificationName (global::UIKit.UITextField.TextFieldTextDidChangeNotification, txt);
				Assert.True (called,  "Notification should have been called");
			}
		}
#endif
	}
}
