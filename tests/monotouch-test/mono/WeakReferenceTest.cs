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
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.ObjCRuntime;
#endif

using NUnit.Framework;
using System.Threading;

namespace MonoTouchFixtures {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class WeakReferenceTest {
		const int totalTestObjects = 5000;

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.WatchOS, 3, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);
		}

		[Test]
		public void NoRetainCyclesExpectedTest ()
		{
			var thread = new Thread (delegate () {
				MyParentView.weakcount = 0;
				for (int i = 0; i < totalTestObjects; i++) {
					var parent = new MyParentView (useWeak: true);
					parent.TouchButton ();
				}
			});

			thread.Start ();
			thread.Join ();

			GC.Collect (0);
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			GC.WaitForPendingFinalizers ();

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), () => { }, () => MyParentView.weakcount < totalTestObjects);
			Assert.That (MyParentView.weakcount, Is.LessThan (totalTestObjects), "No retain cycles expected");
		}

		[Test]
		public void RetainCyclesExpectedTest ()
		{
			var cache = new IntPtr [totalTestObjects];
			MyParentView.noweakcount = 0;
			for (int i = 0; i < totalTestObjects; i++) {
				var parent = new MyParentView (useWeak: false);
				parent.TouchButton ();
				cache [i] = parent.Handle;
			}

			GC.Collect (0);
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Assert.That (MyParentView.noweakcount, Is.EqualTo (totalTestObjects), "Retain cycles expected");

			for (int i = 0; i < totalTestObjects; i++) {
				using (var parent = Runtime.GetNSObject<MyParentView> (cache[i])) {
					var child = (MyButton) parent.Children [0];
					child.RemoveStrongRef ();
				}
			}
		}
	}

	class MyParentView : SKScene {
		public static int weakcount;
		public static int noweakcount;
		bool useWeak;

		~MyParentView ()
		{
			if (useWeak)
				weakcount--;
			else
				noweakcount--;
		}

		public MyParentView (bool useWeak)
		{
			this.useWeak = useWeak;
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
			if (useWeak)
				weakcount++;
			else
				noweakcount++;
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
		public void RemoveStrongRef () => strong = null;
	}
}
