#if !__WATCHOS__
using System;
using Foundation;
using Network;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolMetadataTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10,0);
		}

		[Test]
		public void IP ()
		{
			using (var m = NWProtocolMetadata.CreateIPMetadata ()) {
				Assert.That (m.IPMetadataEcnFlag, Is.EqualTo (NWIPEcnFlag.NonEct), "IPMetadataEcnFlag");
				Assert.That (m.IPMetadataReceiveTime, Is.EqualTo (0), "IPMetadataReceiveTime");
				Assert.True (m.IsIP, "IsIP");
				Assert.False (m.IsTcp, "IsTcp");
				Assert.False (m.IsUdp, "IsUdp");
				Assert.NotNull (m.ProtocolDefinition, "ProtocolDefinition");
				Assert.NotNull (m.SecProtocolMetadata, "SecProtocolMetadata");
				Assert.That (m.ServiceClass, Is.EqualTo (NWServiceClass.BestEffort), "ServiceClass");
			}
		}

		[Test]
		public void Udp ()
		{
			using (var m = NWProtocolMetadata.CreateUdpMetadata ()) {
				Assert.That (m.IPMetadataEcnFlag, Is.EqualTo (NWIPEcnFlag.NonEct), "IPMetadataEcnFlag");
				Assert.That (m.IPMetadataReceiveTime, Is.EqualTo (0), "IPMetadataReceiveTime");
				Assert.False (m.IsIP, "IsIP");
				Assert.False (m.IsTcp, "IsTcp");
				Assert.True (m.IsUdp, "IsUdp");
				Assert.NotNull (m.ProtocolDefinition, "ProtocolDefinition");
				Assert.NotNull (m.SecProtocolMetadata, "SecProtocolMetadata");
				Assert.That (m.ServiceClass, Is.EqualTo (NWServiceClass.BestEffort), "ServiceClass");
			}
		}
	}
}
#endif