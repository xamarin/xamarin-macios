// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ControlTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UIControl c = new UIControl (frame)) {
				Assert.That (c.Frame, Is.EqualTo (frame), "Frame");
			}
		}
		
		[Test]
		public void CancelTrackingTest ()
		{
			using (var c = new UIControl ()) {
				c.CancelTracking (null);
				c.CancelTracking (new UIEvent ());
			}
		}

		[Test]
		public void AddTargetTable ()
		{
			const int items = 100;
			var handles = new GCHandle [items];
			var handler = new EventHandler (delegate(object sender, EventArgs e) {
				
			});
			for (int i = 0; i < items; i++) {
				var ctrl = new UIControl ();
				ctrl.AddTarget (handler, UIControlEvent.EditingChanged);
				handles [i] = GCHandle.Alloc (ctrl, GCHandleType.Weak);
			}
			GC.Collect ();

			// If at least one object was collected, then we know the static ConditionalWeakTable
			// of object -> event handlers doesn't keep strong references.
			var any_collected = false;
			for (int i = 0; i < items; i++) {
				if (handles [i].Target == null)
					any_collected = true;
				handles [i].Free ();
			}
			Assert.IsTrue (any_collected, "Nothing collected");
		}

		[Test]
		public void AddTargetMakeDirty ()
		{
			using (var ctrl = new UIControl ()) {
				ctrl.AddTarget ((a, b) => { }, UIControlEvent.EditingDidBegin);
				Assert.IsTrue ((GetFlags (ctrl) & 0x8) /* RegisteredToggleRef */ == 0x8, "RegisteredToggleRef");
			}
		}


		byte GetFlags (NSObject obj)
		{
			return (byte) typeof (NSObject).GetField ("flags", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic).GetValue (obj);
		}

	}
}

#endif // !__WATCHOS__
