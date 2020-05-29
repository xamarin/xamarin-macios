#if !__WATCHOS__
using System;
using System.Collections.Generic;
using System.Threading;
using CoreFoundation;
using Foundation;
using Network;
using ObjCRuntime;
using Security;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolDefinitionTest {

		[TestFixtureSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (10, 0);



		[Test]
		public void IPDefinitionTest ()
		{
			using (var definition = NWProtocolDefinition.IPDefinition)
				Assert.NotNull (definition);
		}

		[Test]
		public void TcpDefinitionTest ()
		{
			using (var definition = NWProtocolDefinition.TcpDefinition)
				Assert.NotNull (definition);
		}

		[Test]
		public void TlsDefinitionTest ()
		{
			using (var definition = NWProtocolDefinition.TlsDefinition)
				Assert.NotNull (definition);
		}

		[Test]
		public void UdpDefinitionTest ()
		{
			using (var definition = NWProtocolDefinition.UdpDefinition)
				Assert.NotNull (definition);
		}

		[Test]
		public void WebSocketDefinitionTest ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var definition = NWProtocolDefinition.WebSocketDefinition)
				Assert.NotNull (definition);
		}
	}
}
#endif
