#if __MACOS__
using System;
using System.Threading;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSObjectGCTest {
		[Test]
		public void NSObject_GCResurrectTest ()
		{
			// Run GC and pump run loop to ensure that NSObject_Disposer has processed everything
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			PumpLoop ();

			// Create a single object and immediately lose a reference to it, save the handle
			// to immitate getting the same handle from a native API at later point in time
			IntPtr nativeHandle = CreateNativeObject ();

			// Ensure that the GC is run and the one new object gets collected
			GC.Collect ();
			GC.WaitForPendingFinalizers ();

			// The object should now be resurrected through `NSObject_Disposer.Add(this)`
			// and so `GCHandle` will return its reference:
			var o = ObjCRuntime.Runtime.GetNSObject<NSString> (nativeHandle);

			Assert.AreNotEqual (IntPtr.Zero, (IntPtr) o.Handle);

			// Pump the run loop and thus drain the NSObject_Disposer
			PumpLoop ();

			// The object is invalid now
			Assert.AreNotEqual (IntPtr.Zero, (IntPtr) o.Handle);
		}

		[System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private static IntPtr CreateNativeObject ()
		{
			var foo = new NSString ("foo");
			foo.DangerousRetain ();
			return foo.Handle;
		}

		private static void PumpLoop ()
		{
			using (new NSAutoreleasePool ()) {
				NSEvent? evnt;
				while ((evnt = NSApplication.SharedApplication.NextEvent ((NSEventMask) NSEventMask.AnyEvent, NSDate.DistantPast, NSRunLoopMode.Default, true)) is not null) {
					NSApplication.SharedApplication.SendEvent (evnt);
				}
			}
		}
	}
}
#endif // __MACOS__
