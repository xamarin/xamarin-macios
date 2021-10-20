#if !__WATCHOS__
using System;
using Foundation;
using Network;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolQuicOptionsTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (13,0);
		}

		[Test]
		public void AddTlsApplicationProtocolTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void SecProtocolOptionsPropertyTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void StreamIsUnidirectionalPropertyTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void InitialMaxDataTest ()
		{
			Assert.Fail ("Not implemented");
		}
		
		[Test]
		public void MaxUdpPayloadSizeTest ()
		{
			Assert.Fail ("Not implemented");
		} 
		
		[Test]
		public void IdleTimeout ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void InitialMaxStreamsBidirectionalTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void InitialMaxStreamsUnidirectionalTest ()
		{
			Assert.Fail ("Not implemented");
		} 
		
		[Test]
		public void InitialMaxStreamDataBidirectionalLocalTest ()
		{
			Assert.Fail ("Not implemented");
		}
		
		[Test]
		public void InitialMaxStreamDataBidirectionalRemoteTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void InitialMaxStreamDataUnidirectionalTest ()
		{
			Assert.Fail ("Not implemented");
		}
	}
}
#endif
