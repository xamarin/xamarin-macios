#if !__WATCHOS__
using System;
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using Network;
using ObjCRuntime;
using CoreFoundation;
#else
using MonoTouch.Foundation;
using MonoTouch.Network;
using MonoTouch.CoreFoundation;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolIPOptionsTest {

		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the NWPath
		string host;
		NWConnection connection;
		NWProtocolStack stack;
		NWProtocolIPOptions options; 

		void ConnectionStateHandler (NWConnectionState state, NWError error)
		{
			switch (state){
			case NWConnectionState.Ready:
				connectedEvent.Set ();
				break;
			case NWConnectionState.Cancelled:
				connection?.Dispose ();
				connection = null;
				stack?.Dispose ();
				stack = null;
				break;
			case NWConnectionState.Invalid:
			case NWConnectionState.Failed:
				Assert.Inconclusive ("Network connection could not be performed.");
				break;
			}
		}

		[TestFixtureSetUp]
		public void Init () {
			TestRuntime.AssertXcodeVersion (11, 0);
			// we want to use a single connection, since it is expensive
			connectedEvent = new AutoResetEvent(false);
			host = "www.google.com";
			using (var parameters = NWParameters.CreateTcp ())
			using (var endpoint = NWEndpoint.Create (host, "80")) {
				connection = new NWConnection (endpoint, parameters);
				connection.SetQueue (DispatchQueue.DefaultGlobalQueue); // important, else we will get blocked
				connection.SetStateChangeHandler (ConnectionStateHandler);
				connection.Start (); 
				Assert.True (connectedEvent.WaitOne (20000), "Connection timed out.");
				stack = parameters.ProtocolStack;
				using (var ipOptions = stack.InternetProtocol) {
					if (ipOptions != null) {
						ipOptions.IPSetVersion (NWIPVersion.Version4);
						stack.PrependApplicationProtocol (ipOptions);
					}
				}
			}
		}

		[TestFixtureTearDown]
		public void Dispose()
		{
			connection.Cancel ();
		}

		[SetUp]
		public void SetUp ()
		{
			options = stack.InternetProtocol as NWProtocolIPOptions;
			Assert.NotNull (options, "options"); 
		}


		[Test]
		public void SetIPVersionTest ()
		{
			Assert.AreEqual (options.Version, NWIPVersion.Any, "defaulf");
			options.Version = NWIPVersion.Version6;
			Assert.AreEqual (options.Version, NWIPVersion.Version6, "new");
		}

		[Test]
		public void SetHopLimitTest ()
		{
			Assert.AreEqual (0, options.HopLimit, "default");
			options.HopLimit = 1;
			Assert.AreEqual (1, options.HopLimit, "new");
		}

		[Test]
		public void SetUseMinimumMtu ()
		{
			Assert.IsFalse (options.UseMinimumMtu, "default");
			options.UseMinimumMtu = true;
			Assert.IsTrue (options.UseMinimumMtu, "new");
		}

		[Test]
		public void SetDisableFragmentation ()
		{
			Assert.IsFalse (options.DisableFragmentation, "default");
			options.DisableFragmentation = true;
			Assert.IsTrue (options.DisableFragmentation, "new");
		}

		[Test]
		public void SetCaculateReceiveTimeTest ()
		{
			Assert.IsFalse (options.CalculateReceiveTime, "default");
			options.CalculateReceiveTime = true;
			Assert.IsTrue (options.CalculateReceiveTime, "new");
		}

		[Test]
		public void SetIPLocalAddressPreference ()
		{
			Assert.AreEqual (NWIPLocalAddressPreference.Default, options.IPLocalAddressPreference, "default");
			options.IPLocalAddressPreference = NWIPLocalAddressPreference.Temporary;
			Assert.AreEqual (NWIPLocalAddressPreference.Temporary, options.IPLocalAddressPreference, "new");
		}
	}
}
#endif