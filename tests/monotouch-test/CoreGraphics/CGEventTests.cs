using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

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

#if __MACOS__ || __MACCATALYST__
		[Test]
		public void Constructor_CGEventSourceStateID_0 ()
		{
			var ex = Assert.Throws<ArgumentException> (() => new CGEvent (null, CGScrollEventUnit.Pixel), "ArgumentException");
			Assert.AreEqual ("At least one wheel must be provided", ex.Message, "Message");
		}

		[Test]
		public void Constructor_CGEventSourceStateID_1 ()
		{
			using var evt = new CGEvent (null, CGScrollEventUnit.Pixel, 0);
			Assert.AreEqual (CGEventType.ScrollWheel, evt.EventType, "EventType");
			Assert.AreEqual (0, evt.Timestamp, "Timestamp");
			// There doesn't seem to be any way to validate any creation
			// arguments, except using CGEvent.ToData which returns an opaque
			// byte array. Unfortunately the byte array changes randomly
			// (timestamps in it maybe?), so it's not reliable enough for a
			// test.
		}

		[Test]
		public void Constructor_CGEventSourceStateID_2 ()
		{
			using var evt = new CGEvent (null, CGScrollEventUnit.Pixel, 0, 3);
			Assert.AreEqual (CGEventType.ScrollWheel, evt.EventType, "EventType");
			Assert.AreEqual (0, evt.Timestamp, "Timestamp");
			// There doesn't seem to be any way to validate any creation
			// arguments, except using CGEvent.ToData which returns an opaque
			// byte array. Unfortunately the byte array changes randomly
			// (timestamps in it maybe?), so it's not reliable enough for a
			// test.
		}

		[Test]
		public void Constructor_CGEventSourceStateID_3 ()
		{
			using var evt = new CGEvent (null, CGScrollEventUnit.Pixel, 0, 3, 9);
			Assert.AreEqual (CGEventType.ScrollWheel, evt.EventType, "EventType");
			Assert.AreEqual (0, evt.Timestamp, "Timestamp");
			// There doesn't seem to be any way to validate any creation
			// arguments, except using CGEvent.ToData which returns an opaque
			// byte array. Unfortunately the byte array changes randomly
			// (timestamps in it maybe?), so it's not reliable enough for a
			// test.
		}

		[Test]
		public void Constructor_CGEventSourceStateID_4 ()
		{
			var ex = Assert.Throws<ArgumentException> (() => new CGEvent (null, CGScrollEventUnit.Pixel, 0, 3, 9, 42), "ArgumentException");
			Assert.AreEqual ("Only one to three wheels are supported on this constructor", ex.Message, "Message");
		}

		public void PostToPid ()
		{
			var pid = Process.GetCurrentProcess ().Id;
			using var evt = new CGEvent (null, (ushort) 1, true);
			evt.PostToPid (pid);
			CGEvent.PostToPid (evt, pid);
		}

		[Test]
		public void PostToPSN ()
		{
			var pid = Process.GetCurrentProcess ().Id;
			Assert.AreEqual (0, GetProcessForPID (pid, out var psn), "GetProcessForPID");
			using var evt = new CGEvent (null, (ushort) 1, true);
			unsafe {
				IntPtr* psnPtr = &psn;
				evt.PostToPSN ((IntPtr) psnPtr);
				CGEvent.PostToPSN (evt, (IntPtr) psnPtr);
			}
		}

		[DllImport ("/System/Library/Frameworks/ApplicationServices.framework/Versions/A/ApplicationServices")]
		static extern int GetProcessForPID (int pid, out IntPtr psn);
#endif // __MACOS__ || __MACCATALYST__
	}
}
