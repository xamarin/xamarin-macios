#if !__WATCHOS__
using System;
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using Network;
using ObjCRuntime;
using CoreFoundation;
#else
using MonoTouch.Foundation;
using MonoTouch.Network;
using MonoTouch.CoreFoundation;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolTcpOptionsTest {
		NWProtocolTcpOptions options;

		[TestFixtureSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			options = new NWProtocolTcpOptions ();
		}

		[TearDown]
		public void TearDown () => options.Dispose ();

		[Test]
		public void NoDelayTest ()
		{
			Assert.IsFalse (options.NoDelay, "default");
			options.NoDelay = true;
			Assert.IsTrue (options.NoDelay, "new");
		}

		[Test]
		public void NoPushTest ()
		{
			Assert.IsFalse (options.NoPush, "default");
			options.NoPush = true;
			Assert.IsTrue (options.NoPush, "new");
		}

		[Test]
		public void NoOptionsTest ()
		{
			Assert.IsFalse (options.NoOptions, "default");
			options.NoOptions = true;
			Assert.IsTrue (options.NoOptions, "new");
		}

		[Test]
		public void EnableKeepAliveTest ()
		{
			Assert.IsFalse (options.EnableKeepAlive, "default");
			options.EnableKeepAlive = true;
			Assert.IsTrue (options.EnableKeepAlive, "new");
		}

		[Test]
		public void KeepAliveCountTest ()
		{
			Assert.AreEqual (0, options.KeepAliveCount, "default");
			options.KeepAliveCount = 10;
			Assert.AreEqual (10, options.KeepAliveCount, "new");
		}

		[Test]
		public void KeepAliveIdleTimeTest ()
		{
			Assert.AreEqual (TimeSpan.Zero, options.KeepAliveIdleTime, "default");
			options.KeepAliveIdleTime = TimeSpan.FromSeconds (10);
			Assert.AreEqual (TimeSpan.FromSeconds (10), options.KeepAliveIdleTime, "new");
		}

		[Test]
		public void MaximumSegmentSizeTest ()
		{
			Assert.AreEqual (0, options.MaximumSegmentSize, "default");
			options.MaximumSegmentSize = 10;
			Assert.AreEqual (10, options.MaximumSegmentSize, "new");
		}

		[Test]
		public void ConnectionTimeoutTest ()
		{
			Assert.AreEqual (TimeSpan.Zero, options.ConnectionTimeout, "default");
			options.ConnectionTimeout = TimeSpan.FromSeconds (10);
			Assert.AreEqual (TimeSpan.FromSeconds (10), options.ConnectionTimeout, "new");
		}

		[Test]
		public void PersistTimeoutTest ()
		{
			Assert.AreEqual (TimeSpan.Zero, options.PersistTimeout, "default");
			options.PersistTimeout = TimeSpan.FromSeconds (10);
			Assert.AreEqual (TimeSpan.FromSeconds (10), options.PersistTimeout, "new");
		}

		[Test]
		public void RetransmitConnectionDropTimeTest ()
		{
			Assert.AreEqual (TimeSpan.Zero, options.RetransmitConnectionDropTime, "default");
			options.RetransmitConnectionDropTime = TimeSpan.FromSeconds (10);
			Assert.AreEqual (TimeSpan.FromSeconds (10), options.RetransmitConnectionDropTime, "new");
		}

		[Test]
		public void RetransmitFinDropTest ()
		{
			Assert.IsFalse (options.RetransmitFinDrop, "default");
			options.RetransmitFinDrop = true;
			Assert.IsTrue (options.RetransmitFinDrop, "new");
		}

		[Test]
		public void DisableAckStretchingTest ()
		{
			Assert.IsFalse (options.DisableAckStretching, "default");
			options.DisableAckStretching = true;
			Assert.IsTrue (options.DisableAckStretching, "new");
		}

		[Test]
		public void EnableFastOpenTest ()
		{
			Assert.IsFalse (options.EnableFastOpen, "default");
			options.EnableFastOpen = true;
			Assert.IsTrue (options.EnableFastOpen);
		}

		[Test]
		public void DisableEcnTest ()
		{
			Assert.IsFalse (options.DisableEcn, "default");
			options.DisableEcn = true;
			Assert.IsTrue (options.DisableEcn);
		}
	}
}
#endif
