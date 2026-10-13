
using System.Globalization;
using System.Threading.Tasks;

using CoreFoundation;
using Network;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWWebSocketOptionsTest {

		NWWebSocketOptions options;

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			options = new NWWebSocketOptions (NWWebSocketVersion.Version13);
		}

		[TearDown]
		public void TearDown ()
		{
			options.Dispose ();
		}

		[Test]
		public void TestConstructorInvalidVersion ()
		{
			Assert.DoesNotThrow (() => {
				using (var otherOptions = new NWWebSocketOptions (NWWebSocketVersion.Invalid))
					Assert.That (otherOptions.Handle, Is.Not.EqualTo (IntPtr.Zero));
			});
		}

		[Test]
		public void TestSetHeader () => Assert.DoesNotThrow (() => options.SetHeader ("CustomHeader", "hola"));

		[Test]
		public void TestSetHeaderNullName () => Assert.Throws<ArgumentNullException> (() => options.SetHeader (null, "hola"));


		[Test]
		public void TestSetHeaderNullValue () => Assert.DoesNotThrow (() => options.SetHeader ("CustomHeader", null));

		[Test]
		public void TestAddSubprotocol () => Assert.DoesNotThrow (() => options.AddSubprotocol ("Protobuf"));

		[Test]
		public void TestAddSubprotocolNullValue () => Assert.Throws<ArgumentNullException> (() => options.AddSubprotocol (null));

		[Test]
		public void TestAutoReplyPing ()
		{
			var defaultValue = options.AutoReplyPing;
			Assert.That (defaultValue, Is.False, "defaultValue");
			options.AutoReplyPing = true;
			Assert.That (options.AutoReplyPing, Is.True, "new value");
		}

		[Test]
		public void TestMaxMessageSize ()
		{
			var defaultValue = options.MaximumMessageSize;
			Assert.That (defaultValue, Is.EqualTo ((nuint) 0), "defaultValue");
			nuint newValue = 40;
			options.MaximumMessageSize = newValue;
			Assert.That (options.MaximumMessageSize, Is.EqualTo (newValue), "new value");
		}

		[Test]
		public void TestSkipHandShake ()
		{
			Assert.That (options.SkipHandShake, Is.False, "defaultValue");
			options.SkipHandShake = true;
			Assert.That (options.SkipHandShake, Is.True, "new value");
		}

		[Test]
		public void TestClientRequenHandlerNullQ () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (null, (r) => { }));

		[Test]
		public void TestClientRequestHandlerNullCallback () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (DispatchQueue.CurrentQueue, null));

		[Test]
		public void TestClientRequestHandler2NullQueue () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler2 (null, request => new NWWebSocketResponse (NWWebSocketResponseStatus.Reject, "ignored")));

		[Test]
		public void TestClientRequestHandler2NullCallback () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler2 (DispatchQueue.CurrentQueue, null));

		[Test]
		public void ClientRequestHandler ()
		{
			var listenerState = new TaskCompletionSource<string?> (TaskCreationOptions.RunContinuationsAsynchronously);
			var clientState = new TaskCompletionSource<string?> (TaskCreationOptions.RunContinuationsAsynchronously);
			var requestValues = new TaskCompletionSource<(Dictionary<string, string> Headers, List<string> Subprotocols)> (TaskCreationOptions.RunContinuationsAsynchronously);
			var acceptedConnection = new TaskCompletionSource<NWConnection> (TaskCreationOptions.RunContinuationsAsynchronously);

			using var serverOptions = new NWWebSocketOptions (NWWebSocketVersion.Version13);
			serverOptions.SetClientRequestHandler2 (DispatchQueue.DefaultGlobalQueue, request => {
				try {
					var headers = new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase);
					request.EnumerateAdditionalHeaders ((name, value) => {
						if (name is not null && value is not null)
							headers.Add (name, value);
					});
					var subprotocols = new List<string> ();
					request.EnumerateSubprotocols (subprotocol => {
						if (subprotocol is not null)
							subprotocols.Add (subprotocol);
					});
					requestValues.TrySetResult ((headers, subprotocols));
					return new NWWebSocketResponse (NWWebSocketResponseStatus.Accept, "second");
				} catch (Exception e) {
					requestValues.TrySetException (e);
					return new NWWebSocketResponse (NWWebSocketResponseStatus.Reject, "ignored");
				}
			});

			using var serverParameters = NWParameters.CreateTcp ();
			using (var stack = serverParameters.ProtocolStack)
				stack.PrependApplicationProtocol (serverOptions);
			var listener = NWListener.Create ("0", serverParameters);
			Assert.That (listener, Is.Not.Null, "Listener");
			if (listener is null)
				return;

			using (listener) {
				listener.SetQueue (DispatchQueue.DefaultGlobalQueue);
				listener.SetStateChangedHandler ((state, error) => {
					switch (state) {
					case NWListenerState.Ready:
						listenerState.TrySetResult (null);
						break;
					case NWListenerState.Invalid:
					case NWListenerState.Failed:
						listenerState.TrySetResult (error?.ToString () ?? $"Listener entered the {state} state.");
						break;
					}
				});
				listener.SetNewConnectionHandler (connection => {
					connection.SetQueue (DispatchQueue.DefaultGlobalQueue);
					connection.Start ();
					acceptedConnection.TrySetResult (connection);
				});
				listener.Start ();

				Assert.That (listenerState.Task.Wait (TimeSpan.FromSeconds (10)), Is.True, "Listener did not become ready.");
				Assert.That (listenerState.Task.Result, Is.Null, "Listener state");

				using var clientOptions = new NWWebSocketOptions (NWWebSocketVersion.Version13);
				clientOptions.SetHeader ("X-First", "first");
				clientOptions.SetHeader ("X-Second", "second");
				clientOptions.AddSubprotocol ("first");
				clientOptions.AddSubprotocol ("second");
				using var clientParameters = NWParameters.CreateTcp ();
				using (var stack = clientParameters.ProtocolStack)
					stack.PrependApplicationProtocol (clientOptions);
				using var endpoint = NWEndpoint.Create ("127.0.0.1", listener.Port.ToString (CultureInfo.InvariantCulture));
				using var client = new NWConnection (endpoint, clientParameters);
				client.SetQueue (DispatchQueue.DefaultGlobalQueue);
				client.SetStateChangeHandler ((state, error) => {
					switch (state) {
					case NWConnectionState.Ready:
						clientState.TrySetResult (null);
						break;
					case NWConnectionState.Invalid:
					case NWConnectionState.Failed:
						clientState.TrySetResult (error?.ToString () ?? $"Client entered the {state} state.");
						break;
					}
				});
				client.Start ();

				try {
					Assert.That (requestValues.Task.Wait (TimeSpan.FromSeconds (10)), Is.True, "Request handler was not called.");
					var values = requestValues.Task.Result;
					Assert.That (values.Headers.Count, Is.EqualTo (2), "Header count");
					Assert.That (values.Headers ["X-First"], Is.EqualTo ("first"), "First header");
					Assert.That (values.Headers ["X-Second"], Is.EqualTo ("second"), "Second header");
					Assert.That (values.Subprotocols, Is.EqualTo (new [] { "first", "second" }), "Subprotocols");
					Assert.That (clientState.Task.Wait (TimeSpan.FromSeconds (10)), Is.True, "Client did not become ready.");
					Assert.That (clientState.Task.Result, Is.Null, "Client state");
				} finally {
					client.Cancel ();
					listener.Cancel ();
					if (acceptedConnection.Task.IsCompletedSuccessfully) {
						acceptedConnection.Task.Result.Cancel ();
						acceptedConnection.Task.Result.Dispose ();
					}
				}
			}
		}

	}
}
