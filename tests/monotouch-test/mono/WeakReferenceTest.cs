//
// Unit tests for WeakAttribute
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2018 Xamarin Inc. All rights reserved.
//

using System;

#if XAMCORE_2_0
using Foundation;
using SpriteKit;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class WeakReferenceTest {
		const int totalTestObjects = 100;

		[SetUp]
		public void Setup ()
		{
#if __WATCHOS__
			// watchOS 3.0+
			//TestRuntime.CheckWatchOSSystemVersion (3, 0);
			Assert.Ignore ("WeakAttribute is not working on watchOS yet");
#else
			// iOS 7.0+ macOS 10.9+ tvOS 9.0+
			TestRuntime.AssertXcodeVersion (5, 0);
#endif
		}

		[Test]
		public void NoRetainCyclesExpectedTest ()
		{
			for (int i = 0; i < totalTestObjects; i++) {
				var parent = new MyParentView (useWeak: true);
				parent.TouchButton ();
			}

			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Assert.That (MyParentView.count, Is.LessThan (totalTestObjects), "No retain cycles expected");
		}

		[Test]
		public void RetainCyclesExpectedTest ()
		{
			for (int i = 0; i < totalTestObjects; i++) {
				var parent = new MyParentView (useWeak: false);
				parent.TouchButton ();
			}

			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Assert.That (MyParentView.count, Is.EqualTo (totalTestObjects), "Retain cycles expected");
		}
	}

	class MyParentView : SKScene {
		public static int count;

		~MyParentView () => count--;

		public MyParentView (bool useWeak)
		{
			var child = new MyButton (this, useWeak);
			child.TouchUpInside += Child_TouchUpInside;
			AddChild (child);
		}

		public void TouchButton ()
		{
			((MyButton) Children [0]).FireTouch ();
		}

		void Child_TouchUpInside (object sender, EventArgs e)
		{
			count++;
			((MyButton) sender).TouchUpInside -= Child_TouchUpInside;
		}
	}

	class MyButton : SKNode {
		MyParentView strong;
		[Weak] MyParentView weak;

		public MyButton (MyParentView parent, bool useWeak)
		{
			if (useWeak)
				weak = parent;
			else
				strong = parent;
		}

		public event EventHandler<EventArgs> TouchUpInside;
		public void FireTouch () => TouchUpInside?.Invoke (this, EventArgs.Empty);
	}
}
