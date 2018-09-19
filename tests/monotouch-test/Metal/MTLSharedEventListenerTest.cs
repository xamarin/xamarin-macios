#if !__WATCHOS__

using System;

#if XAMCORE_2_0
using CoreFoundation;
using Metal;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Metal;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
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
			if (listener != null)
				listener.Dispose ();
			if (queue != null)
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
