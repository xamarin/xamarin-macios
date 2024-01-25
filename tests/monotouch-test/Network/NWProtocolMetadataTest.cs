#if !__WATCHOS__
using System;

using Foundation;

using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolMetadataTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void IP ()
		{
#if NET
			using (var m = new NWIPMetadata ()) {
				Assert.That (m.EcnFlag, Is.EqualTo (NWIPEcnFlag.NonEct), "IPMetadataEcnFlag");
				Assert.That (m.ReceiveTime, Is.EqualTo (TimeSpan.Zero), "IPMetadataReceiveTime");
#else
			using (var m = NWProtocolMetadata.CreateIPMetadata ()) {
				Assert.That (m.IPMetadataEcnFlag, Is.EqualTo (NWIPEcnFlag.NonEct), "IPMetadataEcnFlag");
				Assert.That (m.IPMetadataReceiveTime, Is.EqualTo (0), "IPMetadataReceiveTime");
#endif
				Assert.True (m.IsIP, "IsIP");
				Assert.False (m.IsTcp, "IsTcp");
				Assert.False (m.IsUdp, "IsUdp");
				Assert.NotNull (m.ProtocolDefinition, "ProtocolDefinition");
#if !NET
				Assert.Throws<InvalidOperationException> (() => { var x = m.SecProtocolMetadata; }, "SecProtocolMetadata");
				Assert.Throws<InvalidOperationException> (() => { var x = m.TlsSecProtocolMetadata; }, "TlsSecProtocolMetadata");
#endif
				Assert.That (m.ServiceClass, Is.EqualTo (NWServiceClass.BestEffort), "ServiceClass");
#if !NET
				Assert.That (m.IPServiceClass, Is.EqualTo (NWServiceClass.BestEffort), "IPServiceClass");
#endif
			}
		}

		[Test]
		public void Udp ()
		{
#if NET
			using (var m = new NWUdpMetadata ()) {
#else
			using (var m = NWProtocolMetadata.CreateUdpMetadata ()) {
				Assert.Throws<InvalidOperationException> (() => { var x = m.IPMetadataEcnFlag; }, "IPMetadataEcnFlag");
				Assert.Throws<InvalidOperationException> (() => { var x = m.IPMetadataReceiveTime; }, "IPMetadataReceiveTime");
#endif
				Assert.False (m.IsIP, "IsIP");
				Assert.False (m.IsTcp, "IsTcp");
				Assert.True (m.IsUdp, "IsUdp");
				Assert.NotNull (m.ProtocolDefinition, "ProtocolDefinition");
#if !NET
				Assert.Throws<InvalidOperationException> (() => { var x = m.SecProtocolMetadata; }, "SecProtocolMetadata");
				Assert.Throws<InvalidOperationException> (() => { var x = m.TlsSecProtocolMetadata; }, "TlsSecProtocolMetadata");
				Assert.Throws<InvalidOperationException> (() => { var x = m.ServiceClass; }, "ServiceClass");
				Assert.Throws<InvalidOperationException> (() => { var x = m.IPServiceClass; }, "IPServiceClass");
#endif
			}
		}

		[Test]
		public void Quic ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
#if NET
			using (var m = new NWIPMetadata ()) {
				Assert.That (m.EcnFlag, Is.EqualTo (NWIPEcnFlag.NonEct), "IPMetadataEcnFlag");
				Assert.That (m.ReceiveTime, Is.EqualTo (TimeSpan.Zero), "IPMetadataReceiveTime");
#else
			using (var m = NWProtocolMetadata.CreateIPMetadata ()) {
				Assert.That (m.IPMetadataEcnFlag, Is.EqualTo (NWIPEcnFlag.NonEct), "IPMetadataEcnFlag");
				Assert.That (m.IPMetadataReceiveTime, Is.EqualTo (0), "IPMetadataReceiveTime");
#endif
				Assert.True (m.IsIP, "IsIP");
				Assert.False (m.IsTcp, "IsTcp");
				Assert.False (m.IsUdp, "IsUdp");
				Assert.False (m.IsQuic, "IsQuic");
				Assert.NotNull (m.ProtocolDefinition, "ProtocolDefinition");
#if !NET
				Assert.Throws<InvalidOperationException> (() => { var x = m.SecProtocolMetadata; }, "SecProtocolMetadata");
				Assert.Throws<InvalidOperationException> (() => { var x = m.TlsSecProtocolMetadata; }, "TlsSecProtocolMetadata");
#endif
				Assert.That (m.ServiceClass, Is.EqualTo (NWServiceClass.BestEffort), "ServiceClass");
#if !NET
				Assert.That (m.IPServiceClass, Is.EqualTo (NWServiceClass.BestEffort), "IPServiceClass");
#endif
			}
		}
	}
}
#endif
