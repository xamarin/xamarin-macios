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
using UIKit;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

using NUnit.Framework;


namespace MonoTouchFixtures {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class WeakReferenceTest {
		const int totalTestObjects = 100;

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

	class MyParentView : UIView {
		public static int count;

		~MyParentView () => count--;

		public MyParentView (bool useWeak)
		{
			var child = new MyButton (this, useWeak);
			child.TouchUpInside += Child_TouchUpInside;
			AddSubview (child);
		}

		public void TouchButton ()
		{
			((MyButton) Subviews [0]).SendActionForControlEvents (UIControlEvent.TouchUpInside);
		}

		void Child_TouchUpInside (object sender, EventArgs e)
		{
			count++;
			((MyButton) sender).TouchUpInside -= Child_TouchUpInside;
		}
	}

	class MyButton : UIButton {
		MyParentView strong;
		[Weak] MyParentView weak;

		public MyButton (MyParentView parent, bool useWeak)
		{
			if (useWeak)
				weak = parent;
			else
				strong = parent;
		}
	}
}
