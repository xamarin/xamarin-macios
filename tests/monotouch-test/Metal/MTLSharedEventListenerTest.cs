#if !__WATCHOS__

using System;

using CoreFoundation;
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLSharedEventListenerTest {
		MTLSharedEventListener listener = null;
		DispatchQueue queue = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (10, 0);
			queue = new DispatchQueue ("myQueue");
			listener = new MTLSharedEventListener (queue);
		}

		[TearDown]
		public void TearDown ()
		{
			if (listener is not null)
				listener.Dispose ();
			if (queue is not null)
				queue.Dispose ();
			listener = null;
			queue = null;
		}

		[Test]
		public void GetSetCommandTypesTest ()
		{
			Assert.AreEqual (queue, listener.DispatchQueue);
		}
	}
}

#endif // !__WATCHOS__
