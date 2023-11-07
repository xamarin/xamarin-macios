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
using Foundation;
using UIKit;
using NUnit.Framework;
using System.Threading;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class GestureRecognizerTest {

		[Test]
		public void DoubleDispose ()
		{
			using (var gr = new UIGestureRecognizer ()) {
				gr.Dispose ();
			}
		}

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
		[Ignore ("Issue: https://github.com/xamarin/maccore/issues/1345 && WIP PR: https://github.com/xamarin/xamarin-macios/pull/5462")]
		public void NoStrongCycles ()
		{
			bool finalizedAnyCtor = false;
			bool finalizedAnyAddTarget1 = false;
			bool finalizedAnyAddTarget2 = false;

			// Add the gesture recognizers to a list so that they're not collected until after the test
			// This is to avoid false positives (the callback should be collectible already after disposing the gesture recognizer).
			var list = new List<UIGestureRecognizer> ();

			var pool = new NSAutoreleasePool ();
			for (var k = 0; k < 10; k++) {
				{
					var notifier = new FinalizerNotifier (() => finalizedAnyCtor = true);
					using (var gr = new UIGestureRecognizer (() => {
						GC.KeepAlive (notifier); // Make sure the 'notifier' instance is only collected if the delegate to UIGestureRecognizer is collectable.
					})) {
						list.Add (gr);
					}
				}
				{
					var notifier = new FinalizerNotifier (() => finalizedAnyAddTarget1 = true);
					using (var gr = new UIGestureRecognizer ()) {
						gr.AddTarget (() => { GC.KeepAlive (notifier); });
						list.Add (gr);
					}
				}
				{
					var notifier = new FinalizerNotifier (() => finalizedAnyAddTarget2 = true);
					using (var gr = new UIGestureRecognizer ()) {
						gr.AddTarget ((obj) => { GC.KeepAlive (notifier); });
						list.Add (gr);
					}
				}
			}
			pool.Dispose ();

			TestRuntime.RunAsync (TimeSpan.FromSeconds (1), () => { GC.Collect (); }, () => finalizedAnyCtor && finalizedAnyAddTarget1 && finalizedAnyAddTarget2);
			Assert.IsTrue (finalizedAnyCtor, "Any finalized");
			Assert.IsTrue (finalizedAnyAddTarget1, "AddTarget1 finalized");
			Assert.IsTrue (finalizedAnyAddTarget2, "AddTarget2 finalized");

			GC.KeepAlive (list);
		}

		[Test]
		public void GenericCallbackTest ()
		{
			var didRun = false;
			var callbackEvent = new AutoResetEvent (false);
			Action<UITapGestureRecognizer> callback = (UITapGestureRecognizer _) => {
				didRun = true;
				callbackEvent.Set ();
			};

			using var recognizer = new UITapGestureRecognizer (callback);

			// add gesture recognizer to UI view
			using UIView view = new UIView ();
			view.AddGestureRecognizer (recognizer);
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), () => {
				// change state of gesture recognizer to execute callback
				recognizer.State = UIGestureRecognizerState.Changed;
				recognizer.State = UIGestureRecognizerState.Ended;
			}, () => didRun);

			// blocks main thread until event is trigerred
			callbackEvent.WaitOne (30000);
			Assert.IsTrue (didRun, "didRun");
		}

		class FinalizerNotifier {
			public Action Action;
			public FinalizerNotifier (Action action)
			{
				Action = action;
			}
			~FinalizerNotifier ()
			{
				if (Action is not null)
					Action ();
			}
		}
	} //end of class
} //end of namespace

#endif // !__WATCHOS__
