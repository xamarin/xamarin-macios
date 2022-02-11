#if !__WATCHOS__
using System;

using CoreFoundation;
using Foundation;
using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWWebSocketMetadataTest {

		NWWebSocketMetadata metadata;

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			metadata = new NWWebSocketMetadata (NWWebSocketOpCode.Text);
		}

		[TearDown]
		public void TearDown ()
		{
			metadata.Dispose ();
		}

		[Test]
		public void TestConstructor ()
		{
			foreach (var opCode in new [] { NWWebSocketOpCode.Binary, NWWebSocketOpCode.Close, NWWebSocketOpCode.Cont, NWWebSocketOpCode.Invalid, NWWebSocketOpCode.Ping, NWWebSocketOpCode.Pong, NWWebSocketOpCode.Text }) {
				Assert.DoesNotThrow (() => {
					var newMetadata = new NWWebSocketMetadata (opCode);
					newMetadata.Dispose ();
				});
			}
		}

		[Test]
		public void TestPongHandlerNullQ () => Assert.Throws<ArgumentNullException> (() => metadata.SetPongHandler (null, (e) => { }));

		[Test]
		public void TestPongHandlerNullCallaback () => Assert.Throws<ArgumentNullException> (() => metadata.SetPongHandler (DispatchQueue.CurrentQueue, null));

		[Test]
		public void TestServerResponse ()
		{
			var resposne = metadata.ServerResponse;
			Assert.IsNull (resposne); // did not make a request, null is expected
		}
	}
}
#endif
