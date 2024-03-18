#if !__WATCHOS__
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CoreFoundation;
using Foundation;
using Network;
using ObjCRuntime;
using NUnit.Framework;
using MonoTests.System.Net.Http;


namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWPathTest {

		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the NWPath
		string host;
		NWPath path;
		List<NWInterface> interfaces = new List<NWInterface> ();
		NWConnection connection;

		[OneTimeSetUp]
		public void Init ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			// we want to use a single connection, since it is expensive
			connectedEvent = new AutoResetEvent (false);
			interfaces = new List<NWInterface> ();
			host = NetworkResources.MicrosoftUri.Host;
			// we create a connection which we are going to use to get the availabe
			// interfaces, that way we can later test protperties of the NWParameters class.
			using (var parameters = NWParameters.CreateUdp ())
			using (var endpoint = NWEndpoint.Create (host, "80")) {
				using (var protocolStack = parameters.ProtocolStack) {
					var ipOptions = protocolStack.InternetProtocol;
#if NET
					ipOptions.SetVersion (NWIPVersion.Version4);
#else
					ipOptions.IPSetVersion (NWIPVersion.Version4);
#endif
				}
				connection = new NWConnection (endpoint, parameters);
				connection.SetQueue (DispatchQueue.DefaultGlobalQueue); // important, else we will get blocked
				connection.SetStateChangeHandler (ConnectionStateHandler);
				connection.Start ();
				Assert.True (connectedEvent.WaitOne (20000), "Connection timed out.");
			}
		}

		[OneTimeTearDown]
		public void Dispose ()
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
			switch (state) {
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
#if !MONOMAC && !__MACCATALYST__
			if (Runtime.Arch != Arch.DEVICE)
				Assert.False (path.HasIPV4, "By default the interface does not support IPV4 on the simulator");
			else
#endif
				Assert.True (path.HasIPV4, "By default the interface does support IPV4 on the device");
		}

		[Test]
		public void HasIPV6PropertyTest ()
		{
			Assert.Ignore ("We cannot test the use of IPV6 since it is different per machine configuraton and makes the test flaky.");
			Assert.False (path.HasIPV6, "By default the interface does not support IPV6"); // To be tested as part of NWProtocolStack
		}

		[Test]
		public void HasDnsPropertyTest ()
		{
#if !MONOMAC && !__MACCATALYST__
			if (Runtime.Arch != Arch.DEVICE)
				Assert.False (path.HasDns, "By default the interface does not support DNS on the simulator");
			else
#endif
				Assert.True (path.HasDns, "By default the interface does support DNS on the device");
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

		[Test]
		public void EnumerateGatewayNullCallbackTest ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			Assert.Throws<ArgumentNullException> (() => { path.EnumerateGateways ((Func<NWEndpoint, bool>) null); });
		}

		[Test]
		public void EnumerateGatewayTest ()
		{
			var e1 = new TaskCompletionSource<bool> ();
			var e2 = new TaskCompletionSource<bool> ();
			var monitor = new NWPathMonitor ();
			try {
				monitor.SetQueue (DispatchQueue.DefaultGlobalQueue);
				monitor.Start ();
				monitor.SnapshotHandler += path => {
					path.EnumerateGateways (gateway => {
						e1.TrySetResult (true);
						return true;
					});

					path.EnumerateInterfaces (@interface => {
						e2.TrySetResult (true);
						return true;
					});
				};
				var rv = TestRuntime.RunAsync (TimeSpan.FromSeconds (5),
						Task.CompletedTask,
						Task.WhenAll (e1.Task, e2.Task));
				if (!rv)
					TestRuntime.IgnoreInCI ("This test doesn't seem to be working on the bots, uncommon network setup?");
				Assert.IsTrue (rv, "Called back");
			} finally {
				monitor.Cancel ();
				monitor.Dispose ();
			}
		}

		[Test]
		public void GetUnsatisfiedReason ()
		{
			TestRuntime.AssertXcodeVersion (12, 2);
			Assert.That (path.GetUnsatisfiedReason (), Is.EqualTo (NWPathUnsatisfiedReason.NotAvailable));
		}
	}
}

#endif
