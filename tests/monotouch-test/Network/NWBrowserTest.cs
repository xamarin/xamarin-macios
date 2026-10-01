using System.Threading;

using Network;

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
				Assert.That (otherBrowser.Descriptor, Is.Not.Null, "Descriptor");
				// we expect the default parameters
				Assert.That (otherBrowser.Parameters, Is.Not.Null, "Parameters");
			}
		}

		[Test]
		public void TestDispatchQueuPropertyNull () => Assert.Throws<ArgumentNullException> (() => { browser.SetDispatchQueue (null); });

		[Test]
		public void TestStart ()
		{
			Assert.That (browser.IsActive, Is.False, "Idle");
			browser.Start ();
			Assert.That (browser.IsActive, Is.True, "Active");
			browser.Cancel ();
			Assert.That (browser.IsActive, Is.False, "Cancel");
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
			// This test may cause cause a dialog asking for access to the local network. The test will work if access is either granted or
			// revoked, but it won't work _while the dialog is up_ (which can't be detected), which makes it rather annoying on the bots.
			// So just skip it there.
			TestRuntime.IgnoreInCI ("This test may pop up a dialog asking for access to the local network, which will cause the test to fail.");

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
			bool listeningDone = false;
			Exception ex = null;
			NWError? browserErrorState = null;
			NWBrowserState state = NWBrowserState.Invalid;
			var changesEvent = new ManualResetEventSlim (false, 0);
			var browserReady = new AutoResetEvent (false);
			var finalEvent = new AutoResetEvent (false);
			var logLines = new List<string> ();

			var log = new Action<string> ((v) => {
				var line = $"{DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss.fffffff")} {v}";
				lock (logLines)
					logLines.Add (line);
				Console.WriteLine (line);
			});
			var printLog = new Func<string> (() => {
				lock (logLines)
					return $"\n\t{string.Join ($"\n\t", logLines)}";
			});

			log ("Starting async...");
			var finishedBeforeTimeout = TestRuntime.RunAsync (TimeSpan.FromSeconds (30), () => {
				// start the browser, before the listener
				log ("Starting browser...");
				browser.SetStateChangesHandler ((st, er) => {
					log ($"browser.SetStateChangedHandler ({st}, {er} => {er?.CFError})");

					browserErrorState ??= er;
					state = st;
					browserReady.Set ();

					if (er is not null) {
						// we can get errors after reaching the NWBrowserState.Ready state,
						// and in that case we may not get any other callbacks, so signal
						// completion so that the test doesn't hang.
						changesEvent.Set ();
					}
				});
				browser.IndividualChangesDelegate = (oldResult, newResult) => {
					log ($"browser.IndividualChangesDelegate ({oldResult}, {newResult})");
					didRun = true;
					try {
						receivedNotNullChange = oldResult is not null || newResult is not null;
					} catch (Exception e) {
						ex = e;
					} finally {
						changesEvent.Set ();
					}
				};
				browser.Start ();
				Assert.That (browserReady.WaitOne (30000), Is.True, "Browser ready");

				using (var advertiser = NWAdvertiseDescriptor.CreateBonjourService ("MonoTouchFixtures.Network", type))
				using (var tcpOptions = new NWProtocolTcpOptions ())
				using (var tlsOptions = new NWProtocolTlsOptions ())
				using (var paramenters = NWParameters.CreateTcp ()) {
					paramenters.ProtocolStack.PrependApplicationProtocol (tlsOptions);
					paramenters.ProtocolStack.PrependApplicationProtocol (tcpOptions);
					paramenters.IncludePeerToPeer = true;
					using (var listener = NWListener.Create ("0", paramenters)) {
						listener.SetQueue (DispatchQueue.CurrentQueue);
						listener.SetAdvertiseDescriptor (advertiser);
						// we need the connection handler, else we will get an exception
						listener.SetNewConnectionHandler ((c) => {
							log ($"listener.SetNewConnectionHandler ()");
						});
						listener.SetStateChangedHandler ((s, e) => {
							log ($"listener.SetStateChangedHandler ({s}, {e} (ErrorCode = {e?.ErrorCode}, ErrorDomain = {e?.ErrorDomain}, CFError: {e?.CFError}, CFError.FailureReason: {e?.CFError?.FailureReason}))");
						});
						listener.Start ();
						Assert.That (changesEvent.Wait (30000), Is.True, $"changesEvent.Wait (){printLog ()}");
						listener.Cancel ();
						listeningDone = true;
						finalEvent.Set ();
					}
				}

			}, () => changesEvent.IsSet);
			log ($"Async done...");

			if (browserErrorState?.CFError?.Code == -65570/* kDNSServiceErr_PolicyDenied */ ) {
				// https://developer.apple.com/forums/thread/663852
				// "If you’re using Bonjour, you will get the kDNSServiceErr_PolicyDenied (-65570) error if your Bonjour operation failed because you don’t have local network access."
				Assert.Ignore ("This test requires access to the local network, and this has not been granted.");
			}

			Assert.That (browserErrorState, Is.Null, "Ready Error");
			Assert.That (state, Is.EqualTo (NWBrowserState.Ready), "NWBrowserState");

			Assert.That (finishedBeforeTimeout, Is.True, $"RunAsync timeout{printLog ()}");
			Assert.That (finalEvent.WaitOne (30000), Is.True, $"Final event{printLog ()}");
			Assert.That (browserErrorState?.CFError, Is.Null, $"Error.CFError{printLog ()}");
			Assert.That (browserErrorState, Is.Null, $"Error{printLog ()}");
			Assert.That (listeningDone, Is.True, $"listeningDone{printLog ()}");
			Assert.That (ex, Is.Null, $"Exception{printLog ()}");
			Assert.That (didRun, Is.True, $"didRan{printLog ()}");
			Assert.That (receivedNotNullChange, Is.True, $"receivedNotNullChange{printLog ()}");
			log ($"about to cancel...");
			browser.Cancel ();
			log ($"cancelled...");
		}
	}
}
