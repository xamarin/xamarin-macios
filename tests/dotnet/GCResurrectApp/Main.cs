using System;
using Foundation;
using AppKit;

public class Program
{
	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
	public static IntPtr CreateNativeObject()
	{
		var foo = new NSString("foo");
		foo.DangerousRetain();
		return foo.Handle;
	}

	public static void PumpLoop()
	{
		using (new NSAutoreleasePool())
		{
			NSEvent? evnt;
			while ((evnt = NSApplication.SharedApplication.NextEvent((NSEventMask)NSEventMask.AnyEvent, NSDate.DistantPast, NSRunLoopMode.Default, true)) != null)
			{
				NSApplication.SharedApplication.SendEvent(evnt);
			}
		}
	}

	public static bool NSObjectFinalizeTest()
	{
		// Pump run loop to ensure that NSObject_Disposer has processed everything
		PumpLoop();

		// Create a single object and immediately lose a reference to it, save the handle
		// to immitate getting the same handle from a native API at later point in time
		IntPtr nativeHandle = CreateNativeObject();

		// Ensure that the GC is run and the one new object gets collected
		GC.Collect();
		GC.WaitForPendingFinalizers();

		// The object should now be resurrected through `NSObject_Disposer.Add(this)`
		// and so `GCHandle` will return its reference:
		var o = ObjCRuntime.Runtime.GetNSObject<NSString>(nativeHandle);

		Console.WriteLine("Handle (1): " + o.Handle);

		// Pump the run loop and thus drain the NSObject_Disposer
		PumpLoop();

		// The object is invalid now
		Console.WriteLine("Handle (2): " + o.Handle);

		return o.Handle != IntPtr.Zero;
	}

	public static void Main()
	{
		NSApplication.Init();
		if (NSObjectFinalizeTest())
		{
			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));
		}
	}
}
