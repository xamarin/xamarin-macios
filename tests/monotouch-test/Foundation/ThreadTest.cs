//
// Unit tests for NSThread
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System.Reflection;
using System.Threading;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ThreadTest {

		[Test]
		public void MainThread ()
		{
			Assert.That (NSThread.IsMain, Is.True, "IsMain");
			Assert.That (NSThread.MainThread.IsMainThread, Is.True, "IsMainThread");
		}

		[Test]
#if APP_EXTENSION
		[Ignore ("App extensions don't have an entry assembly.")]
#endif
		public void GetEntryAssemblyReturnsOk ()
		{
			Assert.That (Assembly.GetEntryAssembly (), Is.Not.Null);
			Assert.That (NSThread.IsMain, Is.True);
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
			Assert.That (t.Join (TimeSpan.FromSeconds (5)), Is.True, "Thread.Join timed out");
			Assert.That (rv, Is.EqualTo (0));
		}

		[Test]
		public void InitWithDataTest ()
		{
			var obj = new InitWithDataObject ();
			var thread = new NSThread (obj, new Selector ("start:"), null);
			thread.Start ();
			Assert.That (obj.StartedEvent.WaitOne (TimeSpan.FromSeconds (5)), Is.True, "thread start");
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
