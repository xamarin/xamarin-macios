#if !__WATCHOS__
using System;
using System.Collections.Generic;
using System.Threading;
#if XAMCORE_2_0
using CoreFoundation;
using Foundation;
using Network;
using ObjCRuntime;
using Security;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.Network;
using MonoTouch.Security;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWConnection {

		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the Report
		string host;
		NWConnection connection;

		void ConnectionStateHandler (NWConnectionState state, NWError error)
		{
			switch (state){
			case NWConnectionState.Ready:
				connectedEvent.Set ();
				break;
			case NWConnectionState.Invalid:
			case NWConnectionState.Failed:
				Assert.Inconclusive ("Network connection could not be performed.");
				break;
			}
		}

		[TestFixtureSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (10, 0);

		[SetUp]
		public void SetUp ()
		{
			// connect and once the connection is done, deal with the diff tests
			connectedEvent = new AutoResetEvent(false);
			host = "www.google.com";
			// we create a connection which we are going to use to get the availabe
			// interfaces, that way we can later test protperties of the NWParameters class.
			using (var parameters = NWParameters.CreateUdp ())
			using (var endpoint = NWEndpoint.Create (host, "80"))
			{
				using (var protocolStack = parameters.ProtocolStack) {
					var ipOptions = protocolStack.InternetProtocol;
					ipOptions.IPSetVersion (NWIPVersion.Version4);
				}
				connection = new NWConnection (endpoint, parameters);
				connection.SetQueue (DispatchQueue.DefaultGlobalQueue); // important, else we will get blocked
				connection.SetStateChangeHandler (ConnectionStateHandler);
				connection.Start (); 
				Assert.True (connectedEvent.WaitOne (20000), "Connection timed out.");
			}

		}

		[TearDown]
		public void TearDown () => connection.Dispose ();

		[Test]
		public void TestEndpointProperty () => Assert.IsNotNull (connection.Endpoint);

		[Test]
		public void TestParametersProperty () => Assert.IsNotNull (connection.Parameters);

		[Test]
		public void TestSetQPropertyNull () => Assert.Fail ("Not implemented.");

		[Test]
		public void TestCancel () => Assert.Fail ("Not implemented");

		[Test]
		public void TestForceCancel () => Assert.Fail ("Not implemented.");
	}
}
#endif