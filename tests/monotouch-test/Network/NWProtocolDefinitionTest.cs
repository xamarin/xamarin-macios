#if !__WATCHOS__
using Foundation;

using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolDefinitionTest {

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (10, 0);



		[Test]
		public void IPDefinitionTest ()
		{
			using (var definition = NWProtocolDefinition.CreateIPDefinition ())
				Assert.NotNull (definition);
		}

		[Test]
		public void TcpDefinitionTest ()
		{
			using (var definition = NWProtocolDefinition.CreateTcpDefinition ())
				Assert.NotNull (definition);
		}

		[Test]
		public void TlsDefinitionTest ()
		{
			using (var definition = NWProtocolDefinition.CreateTlsDefinition ())
				Assert.NotNull (definition);
		}

		[Test]
		public void UdpDefinitionTest ()
		{
			using (var definition = NWProtocolDefinition.CreateUdpDefinition ())
				Assert.NotNull (definition);
		}

		[Test]
		public void WebSocketDefinitionTest ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var definition = NWProtocolDefinition.CreateWebSocketDefinition ())
				Assert.NotNull (definition);
		}
	}
}
#endif
