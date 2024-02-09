using System;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreGraphics;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CGEventTests {
#if MONOMAC
		bool tapCalled = false;

		IntPtr callBack (IntPtr tapProxyEvent, CGEventType eventType,
			IntPtr eventRef, IntPtr userInfo)
		{
			tapCalled = true;
			return eventRef;
		}

		[Test]
		public void CreateTap ()
		{
			tapCalled = false;
			using var tapPort = CGEvent.CreateTap (CGEventTapLocation.AnnotatedSession, CGEventTapPlacement.HeadInsert, CGEventTapOptions.Default, CGEventMask.KeyDown, callBack, IntPtr.Zero);
			Assert.IsFalse (tapCalled, "tap was mistakenly called.");
		}

		[Test]
		public void CreateTapPSN ()
		{
			tapCalled = false;
			var psn = (IntPtr) 2; // kCurrentProcess
			using var tapPort = CGEvent.CreateTap (psn, CGEventTapPlacement.HeadInsert, CGEventTapOptions.Default, CGEventMask.KeyDown, callBack, IntPtr.Zero);
			Assert.IsFalse (tapCalled, "tap was mistakenly called.");
		}
#endif
	}
}
