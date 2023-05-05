//
// Unit tests for NSThread
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;
using System.Threading;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ThreadTest {

		[Test]
		public void MainThread ()
		{
			Assert.True (NSThread.IsMain, "IsMain");
			Assert.True (NSThread.MainThread.IsMainThread, "IsMainThread");
		}

		[Test]
		public void GetEntryAssemblyReturnsOk ()
		{
#if __WATCHOS__
			Assert.IsNull (Assembly.GetEntryAssembly ());
#else
			Assert.IsNotNull (Assembly.GetEntryAssembly ());
			Assert.IsTrue (NSThread.IsMain);
			int rv = -1;
			var t = new Thread (() => {
				if (NSThread.IsMain)
					rv = 1;
				else if (Assembly.GetEntryAssembly () is null)
					rv = 2;
				else
					rv = 0;
			}) {
				IsBackground = true,
			};
			t.Start ();
			t.Join ();
			Assert.AreEqual (0, rv);
#endif
		}

		[Test]
		public void InitWithDataTest ()
		{
			var obj = new InitWithDataObject ();
			var thread = new NSThread (obj, new Selector ("start:"), null);
			thread.Start ();
			Assert.IsTrue (obj.StartedEvent.WaitOne (TimeSpan.FromSeconds (5)), "thread start");
			GC.Collect ();
		}

		class InitWithDataObject : NSObject {
			public ManualResetEvent StartedEvent = new ManualResetEvent (false);

			[Export ("start:")]
			public void Start (NSObject obj)
			{
				StartedEvent.Set ();
			}
		}
	}
}
