#if !__WATCHOS__
using System;
using System.Threading;
using Foundation;
using Network;
using ObjCRuntime;
using CoreFoundation;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWQuicMetadataTest {
		NWQuicMetadata metadata;
		[SetUp]
		public void SetUo ()
		{
			TestRuntime.AssertXcodeVersion (13,0);
			metadata = NWProtocolMetadata.CreateQuicMeta
		}
		
		[Test]
		public void KeepaliveIntervalTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void ApplicationErrorReasonTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void ApplicationErrorCodeTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void ApplicationErrorTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void SecProtocolMetadataTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void StreamIdTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void StreamApplicationErrorTest () 
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void MaxStreamsBidirectionalTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void LocalMaxStreamsUnidirectionalTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void RemoteMaxStreamsBidirectionalTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void RemoteMaxStreamsUnidirectionalTest ()
		{
			Assert.Fail ("Not implemented");
		}
	}
}
#endif
