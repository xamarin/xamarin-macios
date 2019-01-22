//
// UIGestureRecognizer Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Collections.Generic;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class GestureRecognizerTest {

		[Test]
		public void Null ()
		{
			using (var gr = new UIGestureRecognizer (Null)) {
				// ensure documented null-friendly methods actually are before releasing them in the wild
				gr.LocationInView (null);
				// can't call LocationOfTouch, 0 is not valid if there's no touch event
				// gr.LocationOfTouch (0, null);
				gr.RemoveTarget (null, null);
			}
		}

		[Test]
		public void NoStrongCycles ()
		{
			bool disposeAnyCtor = false;
			bool disposeAnyAddTarget1 = false;
			bool disposeAnyAddTarget2 = false;

			// Add the gesture recognizers to a list so that they're not collected until after the test
			// This is to avoid false positives (the callback should be collectible already after disposing the gesture recognizer).
			var list = new List<UIGestureRecognizer> ();

			// 'NSAutoreleasePool' makes test happy on 32 bit
			var pool = new NSAutoreleasePool ();
			for (var k = 0; k < 10; k++) {
				{
					var notifier = new FinalizerNotifier (() => disposeAnyCtor = true);
					using (var gr = new UIGestureRecognizer (() => {
						GC.KeepAlive (notifier); // Make sure the 'notifier' instance is only collected if the delegate to UIGestureRecognizer is collectable.
					})) {
						list.Add (gr);
					}
				}
				{
					var notifier = new FinalizerNotifier (() => disposeAnyAddTarget1 = true);
					using (var gr = new UIGestureRecognizer ()) {
						gr.AddTarget (() => { GC.KeepAlive (notifier); });
						list.Add (gr);
					}
				}
				{
					var notifier = new FinalizerNotifier (() => disposeAnyAddTarget2 = true);
					using (var gr = new UIGestureRecognizer ()) {
						gr.AddTarget ((obj) => { GC.KeepAlive (notifier); });
						list.Add (gr);
					}
				}
			}
			pool.Dispose ();

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (1), () => { GC.Collect (); }, () => disposeAnyCtor && disposeAnyAddTarget1 && disposeAnyAddTarget2);
			Assert.IsTrue (disposeAnyCtor, "Any finalized");
			Assert.IsTrue (disposeAnyAddTarget1, "AddTarget1 finalized");
			Assert.IsTrue (disposeAnyAddTarget2, "AddTarget2 finalized");

			GC.KeepAlive (list);
		}

		class FinalizerNotifier : IDisposable
		{
			public Action Action;
			public FinalizerNotifier (Action action)
			{
				Action = action;
			}

			public void Dispose ()
			{
				if (Action != null)
					Action ();
			}

			// Might not be called since 'Dispose ()' is calling 'GC.SuppressFinalize ()'
			~FinalizerNotifier ()
			{
				if (Action != null)
					Action ();
			}
		}
	}
}

#endif // !__WATCHOS__
