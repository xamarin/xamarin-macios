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

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			connectedEvent = new AutoResetEvent(false);
			interfaces = new List<NWInterface> ();
			host = "wwww.google.com";
			// we create a connection which we are going to use to get the availabe
			// interfaces, that way we can later test protperties of the NWParameters class.
			using (var parameters = NWParameters.CreateUdp ())
			using (var endpoint = NWEndpoint.Create (host, "80"))
			using (var connection = new NWConnection (endpoint, parameters)) {
				connection.SetStateChangeHandler (ConnectionStateHandler);
				connection.SetQueue (DispatchQueue.MainQueue);
				connection.Start ();
				connectedEvent.WaitOne (500);
				path = connection.CurrentPath;
				path.EnumerateInterfaces (EnumerateInterfacesHandler);
			}
		}

		void ConnectionStateHandler (NWConnectionState state, NWError error)
		{
			var errno = (SslStatus)(error != null ? error.ErrorCode : 0);
			switch (state){
			case NWConnectionState.Ready:
				connectedEvent.Set ();
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
			if (path != null)
				path.Dispose ();
			foreach (var i in interfaces)
				i.Dispose ();
		}

		[Test]
		public void StatusPropertyTest ()
		{
			Assert.AreEqual (NWPathStatus.Satisfied, path.Status, $"Unexpected status {path.Status}");
		}

		[Test]
		public void IsExpensivePropertyTest ()
		{
			Assert.False (path.IsExpensive, "Path was not expected to be expensive.");
		} 

		[Test]
		public void HasIPV4PropertyTest ()
		{
			Assert.False (path.HasIPV4);
		}

		[Test]
		public void HasIPV6PropertyTest ()
		{
			Assert.False (path.HasIPV6);
		}

		[Test]
		public void HasDnsPropertyTest ()
		{
			Assert.False (path.HasDns);
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
			Assert.AreNotEqual (0, interfaces.Count, "We should have at least on interface.");
		}
	}
}

#endif