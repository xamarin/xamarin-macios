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
	public class NWPathTest {

		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the NWPath
		string host;
		NWPath path;
		List<NWInterface> interfaces = new List<NWInterface> ();
		NWConnection connection;

		[TestFixtureSetUp]
		public void Init ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			// we want to use a single connection, since it is expensive
			connectedEvent = new AutoResetEvent(false);
			interfaces = new List<NWInterface> ();
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

		[TestFixtureTearDown]
		public void Dispose()
		{
			connection?.Cancel ();
		}

		[SetUp]
		public void SetUp ()
		{

			path = connection.CurrentPath;
			path.EnumerateInterfaces (EnumerateInterfacesHandler);
			Assert.That (interfaces.Count, Is.GreaterThan (0), "interfaces.Count");
		}

		void ConnectionStateHandler (NWConnectionState state, NWError error)
		{
			Console.WriteLine ($"State is {state} and error {error}");
			switch (state){
			case NWConnectionState.Ready:
				connectedEvent.Set ();
				break;
			case NWConnectionState.Cancelled:
				connection?.Dispose ();
				connection = null;
				foreach (var i in interfaces)
					i.Dispose ();
				break;
			case NWConnectionState.Invalid:
			case NWConnectionState.Failed:
				Assert.Inconclusive ("Network connection could not be performed.");
				break;
			}
		}

		void EnumerateInterfacesHandler (NWInterface nwInterface)
		{
			interfaces.Add (nwInterface);
		}

		[TearDown]
		public void TearDown ()
		{
			path?.Dispose ();
		}

		[Test]
		public void StatusPropertyTest ()
		{
			Assert.AreEqual (NWPathStatus.Satisfied, path.Status, $"Unexpected status {path.Status}");
		}

		[Test]
		public void IsExpensivePropertyTest ()
		{
			Assert.False (path.IsExpensive, "Path was not expected to be expensive."); // To be tested as part of NWProtocolStack
		} 

		[Test]
		public void HasIPV4PropertyTest ()
		{
#if !MONOMAC	
			if (Runtime.Arch != Arch.DEVICE)
				Assert.False (path.HasIPV4, "By default the interface does not support IPV4 on the simulator"); 
			else
#endif
				Assert.True (path.HasIPV4, "By default the interface does support IPV4 on the device");
		}

		[Test]
		public void HasIPV6PropertyTest ()
		{
			Assert.False (path.HasIPV6, "By default the interface does not support IPV6"); // To be tested as part of NWProtocolStack
		}

		[Test]
		public void HasDnsPropertyTest ()
		{
#if !MONOMAC	
			if (Runtime.Arch != Arch.DEVICE)
				Assert.False (path.HasDns,  "By default the interface does not support DNS on the simulator");
			else
#endif
				Assert.True (path.HasDns,  "By default the interface does support DNS on the device");
		}

		[Test]
		public void UsesInterfaceTypeTest ()
		{
			foreach (var i in interfaces) {
				Assert.True (path.UsesInterfaceType (i.InterfaceType), $"Type {i.InterfaceType} should be in use.");
			}
		}

		[Test]
		public void IterateInterfacesTest ()
		{
			// clean interfaces from setup
			foreach (var i in interfaces)
				i.Dispose ();
			interfaces = new List<NWInterface> ();
			path.EnumerateInterfaces (EnumerateInterfacesHandler);
			Assert.That (interfaces.Count, Is.GreaterThan (0), "interfaces.Count");
		}
	}
}

#endif