#if !__WATCHOS__
using System;
using System.Threading;

using CoreFoundation;

using Foundation;

using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWBrowserTest {

		NWBrowserDescriptor descriptor;
		NWBrowser browser;

		string type = "_tictactoe._tcp";
		string domain = "local.";

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			descriptor = NWBrowserDescriptor.CreateBonjourService (type, domain);
			using (var parameters = new NWParameters { IncludePeerToPeer = true })
				browser = new NWBrowser (descriptor);
			browser.SetDispatchQueue (DispatchQueue.DefaultGlobalQueue);
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor.Dispose ();
			browser.Dispose ();
		}

		[Test]
		public void TestConstructorNullParameters ()
		{
			using (var otherBrowser = new NWBrowser (descriptor)) {
				Assert.IsNotNull (otherBrowser.Descriptor, "Descriptor");
				// we expect the default parameters
				Assert.IsNotNull (otherBrowser.Parameters, "Parameters");
			}
		}

		[Test]
		public void TestDispatchQueuPropertyNull () => Assert.Throws<ArgumentNullException> (() => { browser.SetDispatchQueue (null); });

		[Test]
		public void TestStart ()
		{
			Assert.IsFalse (browser.IsActive, "Idle");
			browser.Start ();
			Assert.IsTrue (browser.IsActive, "Active");
			browser.Cancel ();
			Assert.IsFalse (browser.IsActive, "Cancel");
		}

		[Test]
		public void TestStartNoQ ()
		{
			using (var newBrowser = new NWBrowser (descriptor))
				Assert.Throws<InvalidOperationException> (() => newBrowser.Start ());
		}

		[Test]
		public void TestStateChangesHandler ()
		{
			// In the test we are doing the following:
			//
			// 1. Start a browser. At this point, we have no listeners (unless someone is exposing it in the lab)
			// and therefore the browser cannot find any services/listeners.
			// 2. Start a listener that is using the same type/domain pair that the browser expects.
			// 3. Browser picks up the new listener, and sends an event (service found).
			// 4. Listener stops, and the service disappears.
			// 5. The browser is not yet canceled, so it picks up that the service/listener is not longer then and returns it.
			// 
			// The test will block until the different events are set by the callbacks that are executed in a diff thread.
			bool didRun = false;
			bool receivedNotNullChange = false;
			bool eventsDone = false;
			bool listeningDone = false;
			Exception ex = null;
			NWError? errorState = null;
			var changesEvent = new AutoResetEvent (false);
			var browserReady = new AutoResetEvent (false);
			var finalEvent = new AutoResetEvent (false);
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), () => {
				// start the browser, before the listener
				browser.SetStateChangesHandler ((st, er) => {
					// assert here with a `st` of `Fail`
					errorState ??= er;
					if (st == NWBrowserState.Ready)
						browserReady.Set ();
				});
#if NET
				browser.IndividualChangesDelegate = (oldResult, newResult) => {
#else
				browser.SetChangesHandler ((oldResult, newResult) => {
#endif
					didRun = true;
					try {
						receivedNotNullChange = oldResult is not null || newResult is not null;
					} catch (Exception e) {
						ex = e;
					} finally {
						changesEvent.Set ();
						eventsDone = true;
					}
#if NET
				};
#else
				});
#endif
				browser.Start ();
				browserReady.WaitOne (30000);
				using (var advertiser = NWAdvertiseDescriptor.CreateBonjourService ("MonoTouchFixtures.Network", type))
#if NET
				using (var tcpOptions = new NWProtocolTcpOptions ())
				using (var tlsOptions = new NWProtocolTlsOptions ())
#else
				using (var tcpOptions = NWProtocolOptions.CreateTcp ())
				using (var tlsOptions = NWProtocolOptions.CreateTls ())
#endif
				using (var paramenters = NWParameters.CreateTcp ()) {
					paramenters.ProtocolStack.PrependApplicationProtocol (tlsOptions);
					paramenters.ProtocolStack.PrependApplicationProtocol (tcpOptions);
					paramenters.IncludePeerToPeer = true;
					using (var listener = NWListener.Create ("1234", paramenters)) {
						listener.SetQueue (DispatchQueue.CurrentQueue);
						listener.SetAdvertiseDescriptor (advertiser);
						// we need the connection handler, else we will get an exception
						listener.SetNewConnectionHandler ((c) => { });
						listener.SetStateChangedHandler ((s, e) => {
							if (e is not null) {
								Console.WriteLine ($"Got error {e.ErrorCode} {e.ErrorDomain} '{e.CFError.FailureReason}' {e.ToString ()}");
							}
						});
						listener.Start ();
						changesEvent.WaitOne (30000);
						listener.Cancel ();
						listeningDone = true;
						finalEvent.Set ();
					}
				}

			}, () => eventsDone);

			finalEvent.WaitOne (30000);
			Assert.IsNull (errorState, "Error");
			Assert.IsTrue (eventsDone, "eventDone");
			Assert.IsTrue (listeningDone, "listeningDone");
			Assert.IsNull (ex, "Exception");
			Assert.IsTrue (didRun, "didRan");
			Assert.IsTrue (receivedNotNullChange, "receivedNotNullChange");
			browser.Cancel ();
		}
	}
}
#endif
