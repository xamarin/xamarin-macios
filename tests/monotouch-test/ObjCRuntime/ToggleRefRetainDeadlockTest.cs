using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ToggleRefRetainDeadlockTest {

		class CustomObject : NSObject {
			[Export ("getObject")]
			public NSObject GetObject ()
			{
				return new CustomObject ();
			}
		}

		[Test]
		public void RunTest ()
		{
			// A thread running the GC.
			var done = new ManualResetEvent (false);
			var gcThread = new Thread (() =>
			{
				while (!done.WaitOne (0)) {
					Thread.Sleep (100);
					GC.Collect ();
				}
			}) {
				IsBackground = true,
			};
			gcThread.Start ();

			// A thread calling retain/release on an object
			var rrThread = new Thread (() =>
			{
				var obj = new CustomObject ();
				while (!done.WaitOne (0)) {
					obj.DangerousRetain ();
					obj.DangerousRelease ();
				}
			}) {
				IsBackground = true,
			};
			rrThread.Start ();

			// A thread that calls a method returning a custom object
			var itThread = new Thread (() =>
			{
				var obj = new CustomObject ();
				while (!done.WaitOne (0)) {
					Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("getObject"));
				}
			}) {
				IsBackground = true,
			};
			itThread.Start ();

			// Let the threads run for little bit. 500ms is enough to trigger the deadlock for me pretty much always.
			// If the process deadlocks, the bug is there.
			Thread.Sleep (500);

			done.Set ();
			gcThread.Join ();
			rrThread.Join ();
			itThread.Join ();
		}
	}
}
