
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
		public void TestClientRequenHandlerNullQ () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (null, (Action<NWWebSocketRequest>) (r => { })));

		[Test]
		public void TestClientRequestHandlerNullCallback () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (DispatchQueue.CurrentQueue, (Action<NWWebSocketRequest>) null));

		[Test]
		public void TestClientRequestHandlerNullQueue () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (null, request => new NWWebSocketResponse (NWWebSocketResponseStatus.Reject, "ignored")));

		[Test]
		public void TestClientRequestHandlerNullResponseCallback () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (DispatchQueue.CurrentQueue, (NWWebSocketClientRequestHandler) null));

		[TestCase (false)]
		[TestCase (true)]
		public void ClientRequestHandler (bool returnNull)
		{
			var listenerState = new TaskCompletionSource<string?> (TaskCreationOptions.RunContinuationsAsynchronously);
			var listenerCancelled = new TaskCompletionSource<bool> (TaskCreationOptions.RunContinuationsAsynchronously);
			var clientState = new TaskCompletionSource<string?> (TaskCreationOptions.RunContinuationsAsynchronously);
			var requestHandled = new TaskCompletionSource<bool> (TaskCreationOptions.RunContinuationsAsynchronously);
			var responseStatus = new TaskCompletionSource<NWWebSocketResponseStatus> (TaskCreationOptions.RunContinuationsAsynchronously);
			var acceptedConnection = new TaskCompletionSource<NWConnection> (TaskCreationOptions.RunContinuationsAsynchronously);

			using var serverOptions = new NWWebSocketOptions (NWWebSocketVersion.Version13);
			using var acceptResponse = new NWWebSocketResponse (NWWebSocketResponseStatus.Accept, "accepted");
#pragma warning disable CS8603 // Intentionally validate the null-response rejection fallback.
			serverOptions.SetClientRequestHandler (DispatchQueue.DefaultGlobalQueue, request => {
				requestHandled.TrySetResult (true);
				return returnNull ? null : acceptResponse;
			});
#pragma warning restore CS8603

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
					case NWListenerState.Cancelled:
						listenerCancelled.TrySetResult (true);
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
				clientOptions.AddSubprotocol ("accepted");
				using var clientParameters = NWParameters.CreateTcp ();
				using (var stack = clientParameters.ProtocolStack)
					stack.PrependApplicationProtocol (clientOptions);
				using var endpoint = NWEndpoint.Create ($"ws://127.0.0.1:{listener.Port.ToString (CultureInfo.InvariantCulture)}/")
					?? throw new InvalidOperationException ("Could not create the WebSocket endpoint.");
				using var client = new NWConnection (endpoint, clientParameters);
				using var webSocketDefinition = NWProtocolDefinition.CreateWebSocketDefinition ();
				client.SetQueue (DispatchQueue.DefaultGlobalQueue);
				client.SetStateChangeHandler ((state, error) => {
					using var metadata = client.GetProtocolMetadata<NWWebSocketMetadata> (webSocketDefinition);
					using var response = metadata?.ServerResponse;
					if (response is not null)
						responseStatus.TrySetResult (response.Status);

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
					Assert.That (requestHandled.Task.Wait (TimeSpan.FromSeconds (10)), Is.True, "Request handler was not called.");
					Assert.That (responseStatus.Task.Wait (TimeSpan.FromSeconds (10)), Is.True, "Client did not receive the server response.");
					Assert.That (responseStatus.Task.Result, Is.EqualTo (returnNull ? NWWebSocketResponseStatus.Reject : NWWebSocketResponseStatus.Accept), "Response status");
					if (!returnNull) {
						Assert.That (clientState.Task.Wait (TimeSpan.FromSeconds (10)), Is.True, "Client did not reach a terminal handshake state.");
						Assert.That (clientState.Task.Result, Is.Null, "Accepted client state");
					}
				} finally {
					client.Cancel ();
					listener.Cancel ();
					listenerCancelled.Task.Wait (TimeSpan.FromSeconds (5));
					if (acceptedConnection.Task.Wait (TimeSpan.FromSeconds (5))) {
						acceptedConnection.Task.Result.Cancel ();
						acceptedConnection.Task.Result.Dispose ();
					}
				}
			}
		}

	}
}
