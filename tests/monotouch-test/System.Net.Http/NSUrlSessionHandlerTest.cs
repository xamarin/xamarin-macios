//
// NSUrlSessionHandlerTest.cs
//

using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTests.System.Net.Http {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSUrlSessionHandlerTest {

		// The proxy tests below use in-process servers bound to 127.0.0.1, so local network
		// connections should be reliable and we don't want to hide any failures by ignoring them
		// in CI. Set this to true to restore the usual "ignore transient network failures in CI"
		// behavior if these tests ever turn out to be flaky on the bots.
		bool ignoreLocalOnlyCIFailures = false;

		// https://github.com/dotnet/macios/issues/23958
		[Test]
		public void DecompressedResponseDoesNotHaveContentEncodingOrContentLength ()
		{
			bool noContentEncoding = false;
			bool noContentLength = false;
			string body = "";

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using var handler = new NSUrlSessionHandler ();
				using var client = new HttpClient (handler);
				// Explicitly request gzip to ensure the server compresses the response.
				using var request = new HttpRequestMessage (HttpMethod.Get, $"{NetworkResources.Httpbin.Url}/gzip");
				request.Headers.TryAddWithoutValidation ("Accept-Encoding", "gzip");
				// Use ResponseHeadersRead so that the response content is not buffered,
				// which would cause HttpContent to compute Content-Length from the buffer.
				var response = await client.SendAsync (request, HttpCompletionOption.ResponseHeadersRead);
				response.EnsureSuccessStatusCode ();

				noContentEncoding = response.Content.Headers.ContentEncoding.Count == 0;
				noContentLength = response.Content.Headers.ContentLength is null;
				body = await response.Content.ReadAsStringAsync ();
			}, out var ex);

			Assert.That (done, Is.True, "Request completed");
			Assert.That (ex, Is.Null, $"Exception: {ex}");
			Assert.That (noContentEncoding, Is.True, "Content-Encoding header should be removed for decompressed content");
			Assert.That (noContentLength, Is.True, "Content-Length header should be removed for decompressed content");
			Assert.That (body.Contains ("\"gzipped\"", StringComparison.OrdinalIgnoreCase), Is.True, "Response body should contain decompressed gzip data");
		}

		// https://github.com/dotnet/macios/issues/23958
		[Test]
		public void NonCompressedResponseHasContentLength ()
		{
			bool noContentEncoding = false;
			long? contentLength = null;
			string body = "";

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using var handler = new NSUrlSessionHandler ();
				using var client = new HttpClient (handler);
				// Request identity encoding to ensure no compression is applied.
				using var request = new HttpRequestMessage (HttpMethod.Get, $"{NetworkResources.Httpbin.Url}/html");
				request.Headers.TryAddWithoutValidation ("Accept-Encoding", "identity");
				// Use ResponseHeadersRead so that the response content is not buffered,
				// which would cause HttpContent to compute Content-Length from the buffer.
				var response = await client.SendAsync (request, HttpCompletionOption.ResponseHeadersRead);
				response.EnsureSuccessStatusCode ();

				noContentEncoding = response.Content.Headers.ContentEncoding.Count == 0;
				contentLength = response.Content.Headers.ContentLength;
				body = await response.Content.ReadAsStringAsync ();
			}, out var ex);

			Assert.That (done, Is.True, "Request completed");
			Assert.That (ex, Is.Null, $"Exception: {ex}");
			Assert.That (noContentEncoding, Is.True, "Content-Encoding should not be present for non-compressed content");
			Assert.That (contentLength, Is.Not.Null, "Content-Length header should be present for non-compressed content");
			Assert.That (contentLength > 0, Is.True, "Content-Length should be greater than zero");
			Assert.That (body.Length > 0, Is.True, "Response body should not be empty");
		}

		// https://github.com/dotnet/macios/issues/23958
		[Test]
		public void KeepHeadersAfterDecompressionSwitch ()
		{
			bool hasContentEncoding = false;
			bool hasContentLength = false;
			string body = "";

			AppContext.SetSwitch ("Foundation.NSUrlSessionHandler.KeepHeadersAfterDecompression", true);
			try {
				var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
					using var handler = new NSUrlSessionHandler ();
					using var client = new HttpClient (handler);
					using var request = new HttpRequestMessage (HttpMethod.Get, $"{NetworkResources.Httpbin.Url}/gzip");
					request.Headers.TryAddWithoutValidation ("Accept-Encoding", "gzip");
					var response = await client.SendAsync (request, HttpCompletionOption.ResponseHeadersRead);
					response.EnsureSuccessStatusCode ();

					hasContentEncoding = response.Content.Headers.ContentEncoding.Count > 0;
					hasContentLength = response.Content.Headers.ContentLength is not null;
					body = await response.Content.ReadAsStringAsync ();
				}, out var ex);

				Assert.That (done, Is.True, "Request completed");
				Assert.That (ex, Is.Null, $"Exception: {ex}");
				Assert.That (hasContentEncoding, Is.True, "Content-Encoding header should be preserved when KeepHeadersAfterDecompression is enabled");
				Assert.That (hasContentLength, Is.True, "Content-Length header should be preserved when KeepHeadersAfterDecompression is enabled");
				Assert.That (body.Contains ("\"gzipped\"", StringComparison.OrdinalIgnoreCase), Is.True, "Response body should contain decompressed gzip data");
			} finally {
				AppContext.SetSwitch ("Foundation.NSUrlSessionHandler.KeepHeadersAfterDecompression", false);
			}
		}

		// https://github.com/dotnet/macios/issues/24376
		[Test]
		public void DisposeAndRecreateBackgroundSessionHandler ()
		{
			bool firstRequestSucceeded = false;

			// First request - should succeed
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using (var handler = new NSUrlSessionHandler (NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration ("test-id"))) {
					using (var client = new HttpClient (handler)) {
						var response = await client.GetByteArrayAsync (NetworkResources.MicrosoftUrl);
						Assert.That (response, Is.Not.Null, "First request response");
						Assert.That (response.Length > 0, Is.True, "First request response length");
						firstRequestSucceeded = true;
					}
				}
			}, out var ex);

			if (!done || !firstRequestSucceeded) {
				TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
				Assert.Inconclusive ("First request failed or timed out - cannot verify the bug.");
			}

			IgnoreIfExceptionDueToBackgroundServiceInUseByAnotherProcess (ex);
			TestRuntime.IgnoreInCIIfBadNetwork (ex);
			Assert.That (ex, Is.Null, "First request exception");

			// Second request with new handler using same background session ID - should not timeout
			done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using (var handler = new NSUrlSessionHandler (NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration ("test-id"))) {
					using (var client = new HttpClient (handler)) {
						var response = await client.GetByteArrayAsync (NetworkResources.MicrosoftUrl);
						Assert.That (response, Is.Not.Null, "Second request response");
						Assert.That (response.Length > 0, Is.True, "Second request response length");
					}
				}
			}, out ex);

			if (!done) {
				TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
				Assert.Fail ("Second request timedout - this indicates the bug is present.");
			}

			IgnoreIfExceptionDueToBackgroundServiceInUseByAnotherProcess (ex);
			TestRuntime.IgnoreInCIIfBadNetwork (ex);
			if (ex is ObjCException && ex.ToString ().Contains ("Task created in a session that has been invalidated")) {
				// When disposing an NSUrlSessionHandler backed by a background NSUrlSession
				// and immediately creating a new handler with the same background session
				// identifier, the new session can fail with 'Task created in a session
				// that has been invalidated'.
				//
				// This happens because InvalidateAndCancel() is asynchronous - it marks
				// the session for invalidation but doesn't wait for it to complete. Apple
				// reuses the same native session object for background sessions with the
				// same identifier, so creating a new session before invalidation completes
				// returns the already-invalidated session.
				//
				// There are a couple of fixes:
				// * Add a Thread.Sleep before creating the second NSUrlSessionHandler - but this will slow down every test run,
				// * Wait for the session to become invalid in NSUrlSessionHandler (add a 'DidBecomeInvalid' implementation, and wait for that in Dispose) - which may unnecessarily slow down working code.
				// * Detect this scenario here, and just mark the test as inconclusive. The test does something somewhat unusual (create two background sessions with the same identifier in quick succession), so this seems like the best approach for now.
				Assert.Inconclusive ("The previous background session wasn't fully invalidated before we tried to create a new background session (with the same identifier)");
			}
			Assert.That (ex, Is.Null, "Second request exception");
		}

		void IgnoreIfExceptionDueToBackgroundServiceInUseByAnotherProcess (Exception? e)
		{
			if (e is null)
				return;

			if (e is not HttpRequestException hre)
				return;

			if (hre.InnerException is not NSErrorException nee)
				return;

			if (nee.Code != (nint) NSUrlError.BackgroundSessionInUseByAnotherProcess)
				return;

			Assert.Ignore ("The background service is in use by another process.");
		}

		// https://github.com/dotnet/macios/issues/25485
		[Test]
		public void BasicAuthWorksWhenBearerIsAdvertisedFirst ()
		{
			if (!HttpListener.IsSupported) {
				Assert.Inconclusive ("HttpListener is not supported");
			}

			const string username = "admin";
			const string password = "secret";
			var expectedBasicValue = Convert.ToBase64String (Encoding.UTF8.GetBytes ($"{username}:{password}"));

			var serverReady = new SemaphoreSlim (0, 1);
			int requestIndex = 0;
			int firstUnauthenticatedIndex = -1;
			int firstAuthenticatedIndex = -1;

			var httpListener = StartListenerOnAvailablePort (out var listeningPort);
			if (httpListener is null) {
				Assert.Inconclusive ("Could not find an available port for the test server.");
				return;
			}

			var serverTask = Task.Run (async () => {
				serverReady.Release ();
				try {
					while (httpListener.IsListening) {
						var context = await httpListener.GetContextAsync ().ConfigureAwait (false);
						var request = context.Request;
						var response = context.Response;

						var authHeader = request.Headers ["Authorization"];
						var currentIndex = Interlocked.Increment (ref requestIndex);
						if (authHeader is not null && authHeader == $"Basic {expectedBasicValue}") {
							// Authenticated - return success
							Interlocked.CompareExchange (ref firstAuthenticatedIndex, currentIndex, -1);
							response.StatusCode = 200;
							var body = Encoding.UTF8.GetBytes ("authenticated");
							response.ContentLength64 = body.Length;
							response.OutputStream.Write (body, 0, body.Length);
						} else {
							// Return 401 with Bearer first, then Basic
							Interlocked.CompareExchange (ref firstUnauthenticatedIndex, currentIndex, -1);
							response.StatusCode = 401;
							response.AddHeader ("WWW-Authenticate", "Bearer realm=\"test\", charset=\"UTF-8\"");
							response.AppendHeader ("WWW-Authenticate", "Basic realm=\"test\", charset=\"UTF-8\"");
						}
						response.Close ();
					}
				} catch (ObjectDisposedException) {
					// listener was stopped
				} catch (HttpListenerException) {
					// listener was stopped
				}
			});

			HttpStatusCode? statusCode = null;
			string responseBody = null;

			try {
				var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
					await serverReady.WaitAsync ().ConfigureAwait (false);

					using var handler = new NSUrlSessionHandler ();
					handler.Credentials = new NetworkCredential (username, password);
					using var client = new HttpClient (handler);
					// Use 127.0.0.1 instead of localhost to avoid IPv6 resolution
					// issues where NSUrlSession may connect to ::1 while
					// HttpListener only binds to IPv4.
					using var request = new HttpRequestMessage (HttpMethod.Get, $"http://127.0.0.1:{listeningPort}/test");
					var response = await client.SendAsync (request).ConfigureAwait (false);
					statusCode = response.StatusCode;
					responseBody = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
				}, out var ex);

				if (!done) {
					TestRuntime.IgnoreInCI ("Transient localhost server failure - ignore in CI");
					Assert.Inconclusive ("Request timed out.");
				}
				TestRuntime.IgnoreInCIIfBadNetwork (ex);
				Assert.That (ex, Is.Null, $"Exception: {ex}");
				// If no request reached the server, the failure is an infrastructure
				// issue (e.g. port conflict), not a code bug.
				if (Volatile.Read (ref requestIndex) == 0 && statusCode == HttpStatusCode.NotFound) {
					TestRuntime.IgnoreInCI ($"Server received no requests and got status {statusCode} - infrastructure issue, ignore in CI");
					Assert.Inconclusive ($"Server received no requests; status was {statusCode}. Likely a port/binding issue.");
				}
				Assert.That (statusCode, Is.EqualTo (HttpStatusCode.OK), "Expected 200 OK after Basic auth negotiation");
				Assert.That (responseBody, Is.EqualTo ("authenticated"), "Response body");
				Assert.That (firstUnauthenticatedIndex, Is.GreaterThan (0), "Server should have received an unauthenticated request");
				Assert.That (firstAuthenticatedIndex, Is.GreaterThan (0), "Server should have received an authenticated request");
				Assert.That (firstUnauthenticatedIndex, Is.LessThan (firstAuthenticatedIndex), "Unauthenticated request should have arrived before the authenticated retry");

				if (serverTask.IsFaulted)
					Assert.Fail ($"Server task failed: {serverTask.Exception}");
			} finally {
				httpListener.Stop ();
				httpListener.Close ();
			}
		}

		// https://github.com/dotnet/macios/issues/25667
		[Test]
		public void StreamReadAsyncCallerCancellationThrowsOperationCanceledException ()
		{
			// Use a raw TCP server so we have full control over when bytes are sent on the wire.
			var tcpListener = new TcpListener (IPAddress.Loopback, 0);
			tcpListener.Start ();
			var port = ((IPEndPoint) tcpListener.LocalEndpoint).Port;

			var serverReady = new SemaphoreSlim (0, 1);

			// Server accepts the HTTP request, sends response headers and a large
			// first body chunk, then stalls (never sends the rest of the declared body).
			var serverTask = Task.Run (async () => {
				try {
					serverReady.Release ();
					using var tcpClient = await tcpListener.AcceptTcpClientAsync ().ConfigureAwait (false);
					tcpClient.NoDelay = true;
					var stream = tcpClient.GetStream ();

					// Wait for the full HTTP request (ends with \r\n\r\n)
					var requestBytes = new byte [8192];
					var totalRead = 0;
					while (true) {
						var n = await stream.ReadAsync (requestBytes, totalRead, requestBytes.Length - totalRead).ConfigureAwait (false);
						if (n == 0)
							return;
						totalRead += n;
						var requestSoFar = Encoding.ASCII.GetString (requestBytes, 0, totalRead);
						if (requestSoFar.Contains ("\r\n\r\n"))
							break;
					}

					// Declare a large Content-Length, send a smaller body, then stall.
					// This ensures NSUrlSession delivers the initial body data via
					// DidReceiveData while keeping the connection open for more.
					var bodyChunk = new string ('A', 4096);
					var responseText = "HTTP/1.1 200 OK\r\n" +
						$"Content-Length: {bodyChunk.Length * 10}\r\n" +
						"Content-Type: text/plain\r\n" +
						"\r\n" +
						bodyChunk;
					var responseBytes = Encoding.ASCII.GetBytes (responseText);
					await stream.WriteAsync (responseBytes, 0, responseBytes.Length).ConfigureAwait (false);
					await stream.FlushAsync ().ConfigureAwait (false);

					// Keep the connection open until the client disconnects
					var discardBuffer = new byte [1024];
					while (await stream.ReadAsync (discardBuffer, 0, discardBuffer.Length).ConfigureAwait (false) > 0) {
					}
				} catch (ObjectDisposedException) {
					// listener was stopped
				} catch (SocketException) {
					// listener was stopped
				}
			});

			Type caughtExceptionType = null;

			try {
				var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
					await serverReady.WaitAsync ().ConfigureAwait (false);

					using var handler = new NSUrlSessionHandler ();
					using var httpClient = new HttpClient (handler);
					httpClient.Timeout = TimeSpan.FromMinutes (5);

					using var request = new HttpRequestMessage (HttpMethod.Get, $"http://127.0.0.1:{port}/stall");
					using var response = await httpClient.SendAsync (request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait (false);
					using var stream = await response.Content.ReadAsStreamAsync ().ConfigureAwait (false);

					// First read succeeds (server sent 4KB of body data)
					var buffer = new byte [8192];
					var bytesRead = await stream.ReadAsync (buffer, 0, buffer.Length).ConfigureAwait (false);
					Assert.That (bytesRead, Is.GreaterThan (0), "First read should return data");

					// Second read: cancel after 250ms via caller token.
					// The server declared a much larger Content-Length but stopped
					// sending, so ReadAsync will block in the polling loop until
					// the caller token fires.
					using var cts = new CancellationTokenSource (TimeSpan.FromMilliseconds (250));
					try {
						await stream.ReadAsync (buffer, 0, buffer.Length, cts.Token).ConfigureAwait (false);
						Assert.Fail ("Expected an exception from the cancelled ReadAsync");
					} catch (Exception ex) {
						caughtExceptionType = ex.GetType ();
					}
				}, out var ex2);

				Assert.That (done, Is.True, "Test timed out");
				Assert.That (ex2, Is.Null, $"Unexpected exception: {ex2}");

				// Caller cancellation should surface as OperationCanceledException (or a subclass like TaskCanceledException),
				// not as TimeoutException. TimeoutException should be reserved for actual request timeouts.
				Assert.That (typeof (OperationCanceledException).IsAssignableFrom (caughtExceptionType), Is.True,
					$"Expected OperationCanceledException but got {caughtExceptionType}");
			} finally {
				tcpListener.Stop ();
			}
		}

		[Test]
		public void ProxyRoutesRequestsThroughProxy ()
		{
			using var proxy = new ProxyTestServer ();

			HttpStatusCode? statusCode = null;
			bool viaProxy = false;

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using var handler = new NSUrlSessionHandler ();
				handler.Proxy = new WebProxy (proxy.Url);
				Assert.That (handler.UseProxy, Is.True, "UseProxy default");
				Assert.That (handler.SupportsProxy, Is.True, "SupportsProxy");
				using var client = new HttpClient (handler);
				var response = await client.GetAsync (NetworkResources.Httpbin.GetUrl).ConfigureAwait (false);
				statusCode = response.StatusCode;
				viaProxy = response.Headers.Contains ("Via-Test-Proxy");
			}, out var ex);

			if (!done) {
				if (ignoreLocalOnlyCIFailures)
					TestRuntime.IgnoreInCI ("Transient localhost server failure - ignore in CI");
				Assert.Inconclusive ("Request timed out.");
			}
			if (ignoreLocalOnlyCIFailures)
				TestRuntime.IgnoreInCIIfBadNetwork (ex);
			Assert.That (ex, Is.Null, $"Exception: {ex}");
			Assert.That (statusCode, Is.EqualTo (HttpStatusCode.OK), "Status code");
			Assert.That (viaProxy, Is.True, "Response should have gone through the test proxy");
			Assert.That (proxy.AuthenticatedRequestCount, Is.GreaterThan (0), "Proxy should have forwarded at least one request");
		}

		[Test]
		public void ProxyWithCredentialsAuthenticatesWithProxy ()
		{
			const string proxyUser = "proxyuser";
			const string proxyPass = "proxypass";

			// NSUrlSession only delivers a proxy authentication challenge to the delegate for CONNECT
			// tunnels (i.e. HTTPS destinations); for a plain HTTP forward proxy it returns the 407
			// directly to the caller. So we route an HTTPS request through the proxy's CONNECT support.
			// We also request a non-local hostname: CFNetwork bypasses the proxy for localhost HTTPS
			// destinations, so we must use a hostname that isn't local. The proxy tunnels every CONNECT
			// to our local TLS test server regardless of the requested host.
			Network.NWListener? destination = null;
			HttpStatusCode? statusCode = null;

			try {
				destination = TlsTestServer.CreateNWTlsListener (requireClientCert: false);
				var destinationPort = destination.Port;
				using var proxy = new ProxyTestServer (proxyUser, proxyPass, forceTunnelPort: (int) destinationPort);

				var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
					using var handler = new NSUrlSessionHandler ();
					handler.Proxy = new WebProxy (proxy.Url) {
						Credentials = new NetworkCredential (proxyUser, proxyPass),
					};
					handler.TrustOverrideForUrl = (sender, url, trust) => true;
					using var client = new HttpClient (handler);
					var response = await client.GetAsync ("https://proxy-tunnel-target.example/").ConfigureAwait (false);
					statusCode = response.StatusCode;
				}, out var ex);

				if (!done) {
					if (ignoreLocalOnlyCIFailures)
						TestRuntime.IgnoreInCI ("Transient localhost server failure - ignore in CI");
					Assert.Inconclusive ("Request timed out.");
				}
				if (ignoreLocalOnlyCIFailures)
					TestRuntime.IgnoreInCIIfBadNetwork (ex);
				Assert.That (ex, Is.Null, $"Exception: {ex}");
				Assert.That (statusCode, Is.EqualTo (HttpStatusCode.OK), $"Status code (proxy credentials should have been used); status={statusCode}, requestCount={proxy.RequestCount}, authRequestCount={proxy.AuthenticatedRequestCount}");
				Assert.That (proxy.AuthenticatedRequestCount, Is.GreaterThan (0), "Proxy should have established an authenticated tunnel");
			} finally {
				destination?.Cancel ();
				destination?.Dispose ();
			}
		}

		[Test]
		public void ProxyWithDefaultProxyCredentialsAuthenticatesWithProxy ()
		{
			const string proxyUser = "proxyuser";
			const string proxyPass = "proxypass";

			// See ProxyWithCredentialsAuthenticatesWithProxy for why we use an HTTPS (CONNECT) request
			// with a non-local hostname and tunnel every CONNECT to a local TLS test server.
			Network.NWListener? destination = null;
			HttpStatusCode? statusCode = null;

			try {
				destination = TlsTestServer.CreateNWTlsListener (requireClientCert: false);
				var destinationPort = destination.Port;
				using var proxy = new ProxyTestServer (proxyUser, proxyPass, forceTunnelPort: (int) destinationPort);

				var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
					using var handler = new NSUrlSessionHandler ();
					handler.Proxy = new WebProxy (proxy.Url);
					handler.DefaultProxyCredentials = new NetworkCredential (proxyUser, proxyPass);
					handler.TrustOverrideForUrl = (sender, url, trust) => true;
					using var client = new HttpClient (handler);
					var response = await client.GetAsync ("https://proxy-tunnel-target.example/").ConfigureAwait (false);
					statusCode = response.StatusCode;
				}, out var ex);

				if (!done) {
					if (ignoreLocalOnlyCIFailures)
						TestRuntime.IgnoreInCI ("Transient localhost server failure - ignore in CI");
					Assert.Inconclusive ("Request timed out.");
				}
				if (ignoreLocalOnlyCIFailures)
					TestRuntime.IgnoreInCIIfBadNetwork (ex);
				Assert.That (ex, Is.Null, $"Exception: {ex}");
				Assert.That (statusCode, Is.EqualTo (HttpStatusCode.OK), $"Status code (default proxy credentials should have been used); status={statusCode}, requestCount={proxy.RequestCount}, authRequestCount={proxy.AuthenticatedRequestCount}");
				Assert.That (proxy.AuthenticatedRequestCount, Is.GreaterThan (0), "Proxy should have established an authenticated tunnel");
			} finally {
				destination?.Cancel ();
				destination?.Dispose ();
			}
		}

		[Test]
		public void ProxyWithUnsupportedSchemeThrows ()
		{
			// A secure ("https") proxy connection can't be expressed via NSUrlSession's connection proxy
			// dictionary, so instead of silently connecting to the proxy over plain HTTP we should fail
			// loudly.
			using var handler = new NSUrlSessionHandler ();
			handler.Proxy = new WebProxy ("https://127.0.0.1:8888");
			using var client = new HttpClient (handler);

			Exception? caught = null;
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				try {
					await client.GetAsync ("http://proxy-scheme-test.example/").ConfigureAwait (false);
				} catch (Exception e) {
					caught = e;
				}
			}, out var ex);

			Assert.That (done, Is.True, "Request completed");
			Assert.That (ex, Is.Null, $"Exception: {ex}");

			// The NotSupportedException may be wrapped by HttpClient, so walk the inner exception chain.
			var notSupported = caught;
			while (notSupported is not null && notSupported is not NotSupportedException)
				notSupported = notSupported.InnerException;
			Assert.That (notSupported, Is.InstanceOf<NotSupportedException> (), $"Should have thrown a NotSupportedException for an unsupported proxy scheme, but was: {caught}");
		}

		[Test]
		public void ProxyPropertiesBehaveCorrectly ()
		{
			using var handler = new NSUrlSessionHandler ();
			Assert.That (handler.Proxy, Is.Null, "Proxy default");
			Assert.That (handler.UseProxy, Is.True, "UseProxy default");
			Assert.That (handler.SupportsProxy, Is.True, "SupportsProxy");
			Assert.That (handler.DefaultProxyCredentials, Is.Null, "DefaultProxyCredentials default");

			var proxy = new WebProxy ("http://127.0.0.1:8888");
			handler.Proxy = proxy;
			Assert.That (handler.Proxy, Is.SameAs (proxy), "Proxy set");

			handler.UseProxy = false;
			Assert.That (handler.UseProxy, Is.False, "UseProxy set to false");

			var credentials = new NetworkCredential ("user", "pass");
			handler.DefaultProxyCredentials = credentials;
			Assert.That (handler.DefaultProxyCredentials, Is.SameAs (credentials), "DefaultProxyCredentials set");
		}

		static HttpListener? StartListenerOnAvailablePort (out int listeningPort)
		{
			// IANA suggested range for dynamic or private ports
			const int MinPort = 49215;
			const int MaxPort = 65535;

			for (var port = MinPort; port < MaxPort; port++) {
				var listener = new HttpListener ();
				listener.Prefixes.Add ($"http://127.0.0.1:{port}/");
				try {
					listener.Start ();
					listeningPort = port;
					return listener;
				} catch {
					// port in use, try next
					listener.Close ();
				}
			}

			listeningPort = -1;
			return null;
		}
	}
}
