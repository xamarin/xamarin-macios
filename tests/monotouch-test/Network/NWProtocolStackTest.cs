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
	public class NWProtocolStackTest {

		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the NWPath
		string host;
		NWConnection connection;
		NWProtocolStack stack;
		List<NWProtocolOptions> options;

		[TestFixtureSetUp]
		public void Init ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			// we want to use a single connection, since it is expensive
			connectedEvent = new AutoResetEvent(false);
			host = "www.google.com";
			using (var parameters = NWParameters.CreateUdp ())
			using (var endpoint = NWEndpoint.Create (host, "80"))
			{
				connection = new NWConnection (endpoint, parameters);
				connection.SetQueue (DispatchQueue.DefaultGlobalQueue); // important, else we will get blocked
				connection.SetStateChangeHandler (ConnectionStateHandler);
				connection.Start (); 
				Assert.True (connectedEvent.WaitOne (20000), "Connection timed out.");
				stack = parameters.ProtocolStack;
				using (var ipOptions = stack.InternetProtocol) {
					ipOptions.IPSetVersion (NWIPVersion.Version4);
					stack.PrependApplicationProtocol (ipOptions);
				}
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
			options = new List<NWProtocolOptions> (); 
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
				stack?.Dispose ();
				stack = null;
				foreach (var o in options)
					o.Dispose ();
				break;
			case NWConnectionState.Invalid:
			case NWConnectionState.Failed:
				Assert.Inconclusive ("Network connection could not be performed.");
				break;
			}
		}

		[Test]
		public void PrependApplicationProtocolNullOptionsTest ()
		{
			// not need to test the method with a valid argument since it is part of the setup.
			Assert.Throws <ArgumentNullException> (() => stack.PrependApplicationProtocol (null)); 
		}

		// handler to iterate over the app protocols
		public void InterateProtocolsHandler (NWProtocolOptions o)
		{
			options.Add (o);
		}

		[Test]
		public void ClearApplicationProtocolsTest ()
		{
			// test the clean and the iterate
			stack.IterateProtocols (InterateProtocolsHandler);
			Assert.That (options.Count, Is.GreaterThan (0), "options.Count");
			// remove present ones
			foreach (var o in options)
				o.Dispose ();
			options = new List<NWProtocolOptions> ();
			stack.ClearApplicationProtocols ();
			stack.IterateProtocols (InterateProtocolsHandler);
			Assert.AreEqual (0, options.Count, "Cleared options");
		}

		[Test]
		public void TransportProtocolPropertyTest ()
		{
			using (var options = stack.TransportProtocol)
			{
				Assert.IsNotNull (options, "Transport protocol should not be null.");
			}
			using (var options = NWProtocolOptions.CreateUdp ())
			{
				stack.TransportProtocol = options;
				using (var copyOptions = stack.TransportProtocol)
				{
					copyOptions.IPSetUseMinimumMtu (true); // should not crash
				}
			}
		}

		[Test]
		public void InternetProtocolTest ()
		{
			using (var o = stack.InternetProtocol)
			{
				o.IPSetUseMinimumMtu (true); // should not crash
			}
		}

	}
 }

 #endif 