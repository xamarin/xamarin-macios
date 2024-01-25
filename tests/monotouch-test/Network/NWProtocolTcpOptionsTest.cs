#if !__WATCHOS__
using System;

using Foundation;

using Network;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolTcpOptionsTest {
		NWProtocolTcpOptions options;

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			options = new NWProtocolTcpOptions ();
		}

		[TearDown]
		public void TearDown () => options.Dispose ();

		// properties do not have getters, but we know that if we call
		// the setter with the wrong pointer we do have a exception
		// thrown

		[Test]
		public void NoDelayTest () => Assert.DoesNotThrow (() => options.SetNoDelay (true));

		[Test]
		public void NoPushTest () => Assert.DoesNotThrow (() => options.SetNoPush (true));

		[Test]
		public void NoOptionsTest () => Assert.DoesNotThrow (() => options.SetNoOptions (true));

		[Test]
		public void EnableKeepAliveTest () => Assert.DoesNotThrow (() => options.SetEnableKeepAlive (true));

		[Test]
		public void KeepAliveCountTest () => Assert.DoesNotThrow (() => options.SetKeepAliveCount (10));

		[Test]
		public void KeepAliveIdleTimeTest () => Assert.DoesNotThrow (() => options.SetKeepAliveIdleTime (TimeSpan.FromSeconds (10)));

		[Test]
		public void MaximumSegmentSizeTest () => Assert.DoesNotThrow (() => options.SetMaximumSegmentSize (10));

		[Test]
		public void ConnectionTimeoutTest () => Assert.DoesNotThrow (() => options.SetConnectionTimeout (TimeSpan.FromSeconds (10)));

		[Test]
		public void PersistTimeoutTest () => Assert.DoesNotThrow (() => options.SetPersistTimeout (TimeSpan.FromSeconds (10)));

		[Test]
		public void RetransmitConnectionDropTimeTest ()
			=> Assert.DoesNotThrow (() => options.SetRetransmitConnectionDropTime (TimeSpan.FromSeconds (10)));

		[Test]
		public void RetransmitFinDropTest () => Assert.DoesNotThrow (() => options.SetRetransmitFinDrop (true));

		[Test]
		public void DisableAckStretchingTest () => Assert.DoesNotThrow (() => options.SetDisableAckStretching (true));

		[Test]
		public void EnableFastOpenTest () => Assert.DoesNotThrow (() => options.SetEnableFastOpen (true));

		[Test]
		public void DisableEcnTest () => Assert.DoesNotThrow (() => options.SetDisableEcn (true));

		[Test]
		public void ForceMultipathVersionTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			Assert.DoesNotThrow (() => options.ForceMultipathVersion (NWMultipathVersion.Version0));
		}

	}
}
#endif
