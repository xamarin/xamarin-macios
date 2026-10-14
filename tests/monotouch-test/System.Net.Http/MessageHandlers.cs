//
// MessageHandlers.cs
//

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.IO;

using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using Xamarin.Utils;

using Network;
using Security;

namespace MonoTests.System.Net.Http {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MessageHandlerTest {
		const string AllowSameOriginRedirectCredentialsSwitch = "Foundation.NSUrlSessionHandler.AllowSameOriginRedirectCredentials";
		const string UseSharedCredentialStorageSwitch = "Foundation.NSUrlSessionHandler.UseSharedCredentialStorage";

		public MessageHandlerTest ()
		{
			// Https seems broken on our macOS 10.9 bot, so skip this test.
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
		}

		HttpMessageHandler GetHandler (Type handler_type)
		{
			if (handler_type == typeof (HttpClientHandler))
				return new HttpClientHandler ();
			if (handler_type == typeof (CFNetworkHandler))
				return new CFNetworkHandler ();
			if (handler_type == typeof (SocketsHttpHandler))
				return new SocketsHttpHandler ();
			if (handler_type == typeof (NSUrlSessionHandler))
				return new NSUrlSessionHandler ();

			throw new NotImplementedException ($"Unknown handler type: {handler_type}");
		}


		[Test]
		[TestCase (typeof (HttpClientHandler))]
		// There are known issues (deadlocks) with CFNetworkHandler: https://github.com/dotnet/macios/issues/25634
		// CFNetworkHandler is obsolete, so we won't fix any such issues, so to avoid deadlocks, just avoid testing CFNetworkHandler.
		[TestCase (typeof (CFNetworkHandler), Ignore = "There are known issues (deadlocks) with CFNetworkHandler: https://github.com/dotnet/macios/issues/25634")]
		[TestCase (typeof (SocketsHttpHandler))]
		[TestCase (typeof (NSUrlSessionHandler))]
		public void DnsFailure (Type handlerType)
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			string response = null;

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				HttpClient client = new HttpClient (GetHandler (handlerType));
				response = await client.GetStringAsync ("http://doesnotexist.xamarin.com");
			}, out var ex);

			Assert.That (done, Is.True, "Did not time out");
			Assert.That (response, Is.Null, $"Response is not null {response}");
			Assert.That (ex, Is.InstanceOf (typeof (HttpRequestException)), "Exception");
		}

		// ensure that we do get the same cookies as the managed handler for the default session
		[Test]
		public void TestNSUrlSessionHandlerCookies ()
		{
			TestNSUrlSessionHandlerCookiesImpl (new NSUrlSessionHandler ());
		}

		// ensure that we do get the same cookies as the managed handler for ephemeral sessions
		[Test]
		public void TestNSUrlSessionEphemeralHandlerCookies ()
		{
			using var ephemeralConfig = NSUrlSessionConfiguration.EphemeralSessionConfiguration;
			TestNSUrlSessionHandlerCookiesImpl (new NSUrlSessionHandler (ephemeralConfig));
		}

		void TestNSUrlSessionHandlerCookiesImpl (NSUrlSessionHandler nativeHandler)
		{
			var managedCookieResult = false;
			var nativeCookieResult = false;
			IEnumerable<string> nativeCookies = null;
			IEnumerable<string> managedCookies = null;

			var completed = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var url = NetworkResources.Httpbin.GetSetCookieUrl ("cookie", "chocolate-chip");
				var managedHandler = new HttpClientHandler () {
					AllowAutoRedirect = false,
				};
				var managedClient = new HttpClient (managedHandler);
				var managedResponse = await managedClient.GetAsync (url);
				managedCookieResult = managedResponse.Headers.TryGetValues ("Set-Cookie", out managedCookies);

				nativeHandler.AllowAutoRedirect = true;
				var nativeClient = new HttpClient (nativeHandler);
				var nativeResponse = await nativeClient.GetAsync (url);
				nativeCookieResult = nativeResponse.Headers.TryGetValues ("Set-Cookie", out nativeCookies);

				Assert.That (nativeHandler.UseCookies, Is.EqualTo (true), "UseCookies");
			}, out var ex);

			var managedHasExpectedCookie = managedCookies?.Any (v => v.StartsWith ("cookie=chocolate-chip;", StringComparison.Ordinal)) == true;
			var nativeHasExpectedCookie = nativeCookies?.Any (v => v.StartsWith ("cookie=chocolate-chip;", StringComparison.Ordinal)) == true;

			Assert.That (completed, Is.True, "Network request completed");
			Assert.That (ex, Is.Null, "Exception");
			Assert.That (managedCookieResult, Is.True, $"Failed to get managed cookies");
			Assert.That (nativeCookieResult, Is.True, $"Failed to get native cookies");
			Assert.That (managedHasExpectedCookie, Is.True, $"Managed Cookie Value");
			Assert.That (nativeHasExpectedCookie, Is.True, $"Native Cookie Value");
		}

		// ensure that we can use a cookie container to set the cookies for a url
		[Test]
		public void TestNSUrlSessionHandlerCookieContainer ()
		{
			TestNSUrlSessionHandlerCookieContainerImpl (new NSUrlSessionHandler ());
		}

		[Test]
		public void TestNSUrlSessionEphemeralHandlerCookieContainer ()
		{
			using var ephemeralConfig = NSUrlSessionConfiguration.EphemeralSessionConfiguration;
			TestNSUrlSessionHandlerCookieContainerImpl (new NSUrlSessionHandler (ephemeralConfig));
		}

		void TestNSUrlSessionHandlerCookieContainerImpl (NSUrlSessionHandler nativeHandler)
		{
			var url = NetworkResources.Httpbin.CookiesUrl;
			var cookie = new Cookie ("cookie", "chocolate-chip");
			var cookieContainer = new CookieContainer ();
			cookieContainer.Add (new Uri (url), cookie);

			string managedCookieResult = null;
			string nativeCookieResult = null;

			var completed = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var managedHandler = new HttpClientHandler () {
					AllowAutoRedirect = false,
					CookieContainer = cookieContainer,
				};
				var managedClient = new HttpClient (managedHandler);
				var managedResponse = await managedClient.GetAsync (url);
				managedCookieResult = await managedResponse.Content.ReadAsStringAsync ();

				nativeHandler.AllowAutoRedirect = true;
				nativeHandler.CookieContainer = cookieContainer;
				var nativeClient = new HttpClient (nativeHandler);
				var nativeResponse = await nativeClient.GetAsync (url);
				nativeCookieResult = await nativeResponse.Content.ReadAsStringAsync ();
			}, out var ex);

			Assert.That (completed, Is.True, "Network request completed");
			Assert.That (ex, Is.Null, "Exception");
			Assert.That (managedCookieResult, Is.Not.Null, "Managed cookies result");
			Assert.That (nativeCookieResult, Is.Not.Null, "Native cookies result");
			Assert.That (managedCookieResult, Does.Contain ("\"cookie\": \"chocolate-chip\""), "Managed cookies");
			Assert.That (nativeCookieResult, Does.Contain ("\"cookie\": \"chocolate-chip\""), "Native cookies");
		}

		// ensure that the Set-Cookie headers do update the CookieContainer
		[Test]
		public void TestNSurlSessionHandlerCookieContainerSetCookie ()
		{
			var url = NetworkResources.Httpbin.GetSetCookieUrl ("cookie", "chocolate-chip");
			var cookieContainer = new CookieContainer ();

			string nativeCookieResult = null;

			var completed = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var nativeHandler = new NSUrlSessionHandler () {
					AllowAutoRedirect = true,
					CookieContainer = cookieContainer,
				};
				var nativeClient = new HttpClient (nativeHandler);
				var nativeResponse = await nativeClient.GetAsync (url);
				nativeCookieResult = await nativeResponse.Content.ReadAsStringAsync ();
			}, out var ex);

			Assert.That (completed, Is.True, "Network request completed");
			Assert.That (ex, Is.Null, "Exception");
			Assert.That (nativeCookieResult, Is.Not.Null, "Native cookies result");
			var cookiesFromServer = cookieContainer.GetCookies (new Uri (url));
			var hasExpectedCookie = cookiesFromServer.Cast<Cookie> ().Any (v => v.Name == "cookie" && v.Value == "chocolate-chip");
			if (!hasExpectedCookie)
				TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
			Assert.That (hasExpectedCookie, Is.True, "Cookies received from server.");
		}

		[Test]
		public void TestNSUrlSessionDefaultDisabledCookies ()
		{
			// simple test. send a request with a set-cookie url, get the data
			// and ensure that the second request does not send any cookies.
			var url = NetworkResources.Httpbin.GetSetCookieUrl ("cookie", "chocolate-chip");

			string nativeSetCookieResult = null;
			string nativeCookieResult = null;

			var completed = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var nativeHandler = new NSUrlSessionHandler () {
					AllowAutoRedirect = true,
					UseCookies = false,
				};
				var nativeClient = new HttpClient (nativeHandler);
				var nativeResponse = await nativeClient.GetAsync (url);
				nativeSetCookieResult = await nativeResponse.Content.ReadAsStringAsync ();

				// got the response, perofm a second queries to the cookies endpoint to get
				// the actual cookies sent from the storage
				nativeResponse = await nativeClient.GetAsync (NetworkResources.Httpbin.CookiesUrl);
				nativeCookieResult = await nativeResponse.Content.ReadAsStringAsync ();
			}, out var ex);

			Assert.That (completed, Is.True, "Network request completed");
			Assert.That (ex, Is.Null, "Exception");
			Assert.That (nativeSetCookieResult, Is.Not.Null, "Native set-cookies result");
			Assert.That (nativeCookieResult, Is.Not.Null, "Native cookies result");
			Assert.That (nativeCookieResult.Contains ("chocolate-chip"), Is.False);
		}

		[Test]
		public void TestNSUrlSessionDefaultDisableCookiesWithManagedContainer ()
		{
			// simple test. send a request with a set-cookie url, get the data
			// and ensure that the second request does not send any cookies.
			var url = NetworkResources.Httpbin.GetSetCookieUrl ("cookie", "chocolate-chip");

			string nativeSetCookieResult = null;
			string nativeCookieResult = null;
			var cookieContainer = new CookieContainer ();

			var completed = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var nativeHandler = new NSUrlSessionHandler () {
					AllowAutoRedirect = true,
					UseCookies = false,
				};
				var nativeClient = new HttpClient (nativeHandler);
				var nativeResponse = await nativeClient.GetAsync (url);
				nativeSetCookieResult = await nativeResponse.Content.ReadAsStringAsync ();

				// got the response, preform a second queries to the cookies endpoint to get
				// the actual cookies sent from the storage
				nativeResponse = await nativeClient.GetAsync (NetworkResources.Httpbin.CookiesUrl);
				nativeCookieResult = await nativeResponse.Content.ReadAsStringAsync ();
			}, out var ex);

			Assert.That (completed, Is.True, "Network request completed");
			Assert.That (ex, Is.Null, "Exception");
			Assert.That (nativeSetCookieResult, Is.Not.Null, "Native set-cookies result");
			Assert.That (nativeCookieResult, Is.Not.Null, "Native cookies result");
			Assert.That (nativeCookieResult.Contains ("chocolate-chip"), Is.False);
			var cookiesFromServer = cookieContainer.GetCookies (new Uri (url));
			Assert.That (cookiesFromServer.Count, Is.EqualTo (0), "Cookies received from server.");
		}

		[Test]
		public void TestNSUrlSessionEphemeralDisabledCookies ()
		{
			// assert that we can't re-enable cookies for ephemeral sessions
			using var config = NSUrlSessionConfiguration.EphemeralSessionConfiguration;
			Assert.That (config.SessionType, Is.EqualTo (NSUrlSessionConfiguration.SessionConfigurationType.Ephemeral), "Session type.");
			var nativeHandler = new NSUrlSessionHandler (config);
			Assert.That (nativeHandler.UseCookies, Is.EqualTo (true), "UseCookies #1");
			nativeHandler.UseCookies = true;
			Assert.That (nativeHandler.UseCookies, Is.EqualTo (true), "UseCookies #2");
			nativeHandler.UseCookies = false;
			Assert.That (nativeHandler.UseCookies, Is.EqualTo (false), "UseCookies #3");
			nativeHandler.UseCookies = false;
			Assert.That (nativeHandler.UseCookies, Is.EqualTo (false), "UseCookies #4");
			Assert.Throws<InvalidOperationException> (() => {
				nativeHandler.UseCookies = true;
			}, "UseCookies #5");
			Assert.That (nativeHandler.UseCookies, Is.EqualTo (false), "UseCookies #6");
			Assert.Throws<InvalidOperationException> (() => {
				nativeHandler.UseCookies = true;
			}, "UseCookies #7");
			Assert.That (nativeHandler.UseCookies, Is.EqualTo (false), "UseCookies #8");
		}

		[Test]
		public void TestNSUrlSessionTimeoutExceptionWhileStreamingContent ()
		{
			if (!HttpListener.IsSupported) {
				Assert.Inconclusive ("HttpListener is not supported");
			}

			// HTPP listener config
			IPEndPoint httpListenerEndPoint = null;
			var serverLaunchedSemaphore = new SemaphoreSlim (0, 1);
			const int expectedHttpResponseContentLength = 10;

			var serverCancellationTokenSource = new CancellationTokenSource ();
			var serverCancellationToken = serverCancellationTokenSource.Token;


			// NSUrlSession config
			var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
			config.TimeoutIntervalForResource = 3;

			Task.Run (async () => {
				// Trying to bing a HttpListener to the first available port
				// To avoid race condition, we cannot list available ports, then decide to bind to one of them
				HttpListener httpListener = null;

				// IANA suggested range for dynamic or private ports
				const int MinPort = 49215;
				const int MaxPort = 65535;

				int listeningPort = -1;
				for (var port = MinPort; port < MaxPort; port++) {
					httpListener = new HttpListener ();
					httpListener.Prefixes.Add ($"http://*:{port}/");
					try {
						httpListener.Start ();
						listeningPort = port;
						break;
					} catch {
						// nothing to do here -- the listener disposes itself when Start throws
					}
				}

				if (httpListener is null) {
					return;
				}

				httpListenerEndPoint = new IPEndPoint (IPAddress.Any, listeningPort);

				serverLaunchedSemaphore.Release ();

				try {
					while (true) {
						var contextTask = httpListener.GetContextAsync ();
						Task.WaitAny (
							new Task [] { contextTask },
							serverCancellationToken);

						var context = contextTask.Result;
						var request = context.Request;
						var response = context.Response;

						// Construct a response.
						response.ContentType = "application/octet-stream";
						response.StatusCode = 200;

						try {
							// Dripping response blocks, with increasing interval
							using (var output = response.OutputStream) {
								for (var i = 0; i < expectedHttpResponseContentLength; i++) {
									serverCancellationToken.ThrowIfCancellationRequested ();

									await output.WriteAsync (new byte [] { 0x42 });
									output.Flush ();

									await Task.Delay (TimeSpan.FromSeconds (i));
								}
							}
						} finally {
							response.Close ();
						}
					}
				} finally {
					httpListener.Stop ();
				}
			});

			var timeoutExceptionWasThrown = false;
			var timeoutExceptionShouldHaveBeenThrown = true;

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				HttpClient client = new HttpClient (new NSUrlSessionHandler (config));

				await serverLaunchedSemaphore.WaitAsync ();

				try {
					var responseMessage = await client.GetAsync (
													$"http://{httpListenerEndPoint.Address}:{httpListenerEndPoint.Port}",
													HttpCompletionOption.ResponseHeadersRead);

					using (var contentStream = await responseMessage.Content.ReadAsStreamAsync ())
					using (var outputStream = new global::System.IO.MemoryStream ()) {
						await contentStream.CopyToAsync (outputStream);

						timeoutExceptionWasThrown = false;
						timeoutExceptionShouldHaveBeenThrown = outputStream.ToArray ().Length < expectedHttpResponseContentLength;
					}
				} catch (Exception e)
					  when (e is TimeoutException || e is HttpRequestException) {
					timeoutExceptionWasThrown = true;
				}
			}, out var ex);

			serverCancellationTokenSource.Cancel ();

			if (!done) {
				Assert.Inconclusive ("Test run timedout.");
			}

			Assert.That (ex, Is.Null, "Exception");

			if (!timeoutExceptionShouldHaveBeenThrown) {
				Assert.Inconclusive ("Failed to produce a timeout. The response content was streamed completely.");
			} else {
				Assert.That (timeoutExceptionWasThrown, Is.True, "Timeout exception is thrown.");
			}
		}

		// ensure that if we have a redirect, we do not have the auth headers in the following requests
		[TestCase (typeof (HttpClientHandler))]
		// There are known issues (deadlocks) with CFNetworkHandler: https://github.com/dotnet/macios/issues/25634
		// CFNetworkHandler is obsolete, so we won't fix any such issues, so to avoid deadlocks, just avoid testing CFNetworkHandler.
		[TestCase (typeof (CFNetworkHandler), Ignore = "There are known issues (deadlocks) with CFNetworkHandler: https://github.com/dotnet/macios/issues/25634")]
		[TestCase (typeof (NSUrlSessionHandler))]
		public void RedirectionWithAuthorizationHeaders (Type handlerType)
		{

			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			bool containsAuthorizarion = false;
			bool containsHeaders = false;
			string json = "";

			using var handler = GetHandler (handlerType);
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using var client = new HttpClient (handler, disposeHandler: false) {
					Timeout = TimeSpan.FromSeconds (20),
				};
				client.BaseAddress = NetworkResources.Httpbin.Uri;
				var byteArray = new UTF8Encoding ().GetBytes ("username:password");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String (byteArray));
				using var result = await client.GetAsync (NetworkResources.Httpbin.GetRedirectUrl (3));
				// get the data returned from httpbin which contains the details of the requested performed.
				json = await result.Content.ReadAsStringAsync ();
				containsAuthorizarion = json.Contains ("Authorization");
				containsHeaders = json.Contains ("headers");  // ensure we do have the headers in the response
			}, out var ex);

			Assert.That (done, Is.True, "Request completed");
			Assert.That (ex, Is.Null, $"Exception {ex} for {json}");
			Assert.That (containsHeaders, Is.True, "Response does not contain headers");
			Assert.That (containsAuthorizarion, Is.False, $"Authorization header did reach the final destination. {json}");
		}

		[TestCase (true, false, HttpStatusCode.Unauthorized, false, TestName = "NSUrlSessionHandlerOriginCredentialCacheNotSentToCrossOriginRedirectTarget")]
		[TestCase (true, true, HttpStatusCode.OK, true, TestName = "NSUrlSessionHandlerTargetCredentialCacheSentToCrossOriginRedirectTarget")]
		[TestCase (false, false, HttpStatusCode.Unauthorized, false, TestName = "NSUrlSessionHandlerNetworkCredentialNotSentToCrossOriginRedirectTarget")]
		public void NSUrlSessionHandlerCredentialsCrossOriginRedirectTarget (bool useCredentialCache, bool cacheRedirectTarget, HttpStatusCode expectedStatusCode, bool expectAuthorizationHeader)
		{
			if (!HttpListener.IsSupported) {
				Assert.Inconclusive ("HttpListener is not supported");
			}

			using var server = new RedirectBasicAuthServer (crossOrigin: true);
			using var handler = new NSUrlSessionHandler ();
			var username = "origin-user";
			var password = "origin-password";

			if (useCredentialCache) {
				var cache = new CredentialCache ();
				var credentialUri = cacheRedirectTarget ? new Uri (server.TargetUri, "protected") : server.OriginUri;
				cache.Add (credentialUri, "basic", new NetworkCredential (username, password));
				handler.Credentials = cache;
			} else {
				handler.Credentials = new NetworkCredential (username, password);
			}

			using var client = new HttpClient (handler);
			using var response = GetResponseWithTimeout (client, new Uri (server.OriginUri, "start"));

			Assert.That (response.StatusCode, Is.EqualTo (expectedStatusCode), "StatusCode");
			Assert.That (server.TargetRequestCount, Is.GreaterThanOrEqualTo (1), "Target request count");
			Assert.That (server.TargetAuthorizationHeaders.Length > 0, Is.EqualTo (expectAuthorizationHeader), "Authorization header presence.");
		}

		[TestCase (false, HttpStatusCode.Unauthorized, TestName = "NSUrlSessionHandlerNetworkCredentialNotSentAfterSameOriginRedirectByDefault")]
		[TestCase (true, HttpStatusCode.OK, TestName = "NSUrlSessionHandlerNetworkCredentialSentAfterSameOriginRedirectWithAppContextSwitch")]
		public void NSUrlSessionHandlerNetworkCredentialSameOriginRedirectCredentials (bool allowSameOriginRedirectCredentials, HttpStatusCode expectedStatusCode)
		{
			if (!HttpListener.IsSupported) {
				Assert.Inconclusive ("HttpListener is not supported");
			}

			AppContext.TryGetSwitch (AllowSameOriginRedirectCredentialsSwitch, out var originalValue);
			try {
				AppContext.SetSwitch (AllowSameOriginRedirectCredentialsSwitch, allowSameOriginRedirectCredentials);

				using var server = new RedirectBasicAuthServer (crossOrigin: false);
				using var handler = new NSUrlSessionHandler {
					Credentials = new NetworkCredential ("origin-user", "origin-password"),
				};

				using var client = new HttpClient (handler);
				using var response = GetResponseWithTimeout (client, new Uri (server.OriginUri, "start"));

				Assert.That (response.StatusCode, Is.EqualTo (expectedStatusCode), "StatusCode");
				Assert.That (server.TargetRequestCount, Is.GreaterThanOrEqualTo (1), "Target request count");
				Assert.That (server.TargetAuthorizationHeaders.Length > 0, Is.EqualTo (allowSameOriginRedirectCredentials), "Authorization header presence.");
			} finally {
				AppContext.SetSwitch (AllowSameOriginRedirectCredentialsSwitch, originalValue);
			}
		}

		[TestCase (false, HttpStatusCode.Unauthorized, false, TestName = "NSUrlSessionHandlerNetworkCredentialNotSentToCrossOriginRedirectWithDefaultCredentialStorage")]
		[TestCase (true, HttpStatusCode.OK, true, TestName = "NSUrlSessionHandlerNetworkCredentialSentToCrossOriginRedirectWithSharedCredentialStorage")]
		public void NSUrlSessionHandlerUseSharedCredentialStorage (bool useSharedCredentialStorage, HttpStatusCode expectedStatusCode, bool expectSecondRequestAuthorizationHeader)
		{
			if (!HttpListener.IsSupported) {
				Assert.Inconclusive ("HttpListener is not supported");
			}

			AppContext.TryGetSwitch (UseSharedCredentialStorageSwitch, out var originalValue);
			try {
				AppContext.SetSwitch (UseSharedCredentialStorageSwitch, useSharedCredentialStorage);

				using var server = new RedirectBasicAuthServer (crossOrigin: true);

				// First, prime the shared credential storage by making a successful auth request
				// using a CredentialCache with the target URI. When shared storage is enabled,
				// NSUrlSession stores the credential with ForSession persistence in SharedCredentialStorage,
				// making it available to subsequent handlers targeting the same host:port.
				var cache = new CredentialCache ();
				cache.Add (new Uri (server.TargetUri, "protected"), "basic", new NetworkCredential ("origin-user", "origin-password"));

				using (var primingHandler = new NSUrlSessionHandler { Credentials = cache }) {
					using var primingClient = new HttpClient (primingHandler);
					using var primingResponse = GetResponseWithTimeout (primingClient, new Uri (server.OriginUri, "start"));
				}

				// Record how many auth headers the server received from the priming request
				var authHeadersAfterPriming = server.TargetAuthorizationHeaders.Length;

				// Now make a second request with a NetworkCredential (not CredentialCache) to the same server.
				// With shared storage enabled, NSUrlSession finds cached credentials in SharedCredentialStorage
				// and pre-emptively authenticates the redirect target without calling DidReceiveChallenge.
				// With shared storage disabled (default), no stored credentials exist, and the
				// DidReceiveChallenge delegate blocks the NetworkCredential after the cross-origin redirect.
				using var handler = new NSUrlSessionHandler {
					Credentials = new NetworkCredential ("origin-user", "origin-password"),
				};

				using var client = new HttpClient (handler);
				using var response = GetResponseWithTimeout (client, new Uri (server.OriginUri, "start"));

				Assert.That (response.StatusCode, Is.EqualTo (expectedStatusCode), "StatusCode");

				var authHeadersFromSecondRequest = server.TargetAuthorizationHeaders.Length - authHeadersAfterPriming;
				Assert.That (authHeadersFromSecondRequest > 0, Is.EqualTo (expectSecondRequestAuthorizationHeader), "Authorization header on second request.");
			} finally {
				AppContext.SetSwitch (UseSharedCredentialStorageSwitch, originalValue);
			}
		}

		[TestCase (typeof (SocketsHttpHandler))]
		[TestCase (typeof (NSUrlSessionHandler))]
		public void RejectSslCertificatesServicePointManager (Type handlerType)
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			bool validationCbWasExecuted = false;
			bool invalidServicePointManagerCbWasExcuted = false;
			Type expectedExceptionType = null;
			HttpResponseMessage result = null;

			var handler = GetHandler (handlerType);
			if (handler is HttpClientHandler ch) {
				expectedExceptionType = typeof (AuthenticationException);
				ch.ServerCertificateCustomValidationCallback = (sender, certificate, chain, errors) => {
					validationCbWasExecuted = true;
					// return false, since we want to test that the exception is raised
					return false;
				};
#pragma warning disable SM02184 // "Server certificate validation disabled" - the warning is incorrect, because it's supposed to detect when server validation always passes (i.e. the callback returns 'true'), but we return _false_ (i.e. we always fail the server validation).
#pragma warning disable SYSLIB0014 // 'ServicePointManager' is obsolete: 'WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead. Settings on ServicePointManager no longer affect SslStream or HttpClient.' (https://aka.ms/dotnet-warnings/SYSLIB0014)
				ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => {
#pragma warning restore SYSLIB0014
					invalidServicePointManagerCbWasExcuted = true;
					return false;
				};
#pragma warning restore SM02184
			} else if (handler is SocketsHttpHandler shh) {
				expectedExceptionType = typeof (AuthenticationException);
				var sslOptions = new SslClientAuthenticationOptions {
					// Leave certs unvalidated for debugging
					RemoteCertificateValidationCallback = delegate
					{
						validationCbWasExecuted = true;
						// return false, since we want to test that the exception is raised
						return false;
					},
				};
				shh.SslOptions = sslOptions;
			} else if (handler is NSUrlSessionHandler ns) {
				expectedExceptionType = typeof (WebException);
				ns.TrustOverrideForUrl += (a, b, c) => {
					validationCbWasExecuted = true;
					// return false, since we want to test that the exception is raised
					return false;
				};
			} else {
				Assert.Fail ($"Invalid HttpMessageHandler: '{handler.GetType ()}'.");
			}

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				try {
					HttpClient client = new HttpClient (handler);
					var byteArray = new UTF8Encoding ().GetBytes ("username:password");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String (byteArray));
					result = await client.GetAsync (NetworkResources.MicrosoftUrl);
				} finally {
#pragma warning disable SYSLIB0014 // 'ServicePointManager' is obsolete: 'WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead. Settings on ServicePointManager no longer affect SslStream or HttpClient.' (https://aka.ms/dotnet-warnings/SYSLIB0014)
					ServicePointManager.ServerCertificateValidationCallback = null;
#pragma warning restore SYSLIB0014
				}
			}, out var ex);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				// the ServicePointManager.ServerCertificateValidationCallback will never be executed.
				Assert.That (invalidServicePointManagerCbWasExcuted, Is.False, "Invalid SPM executed");
				Assert.That (validationCbWasExecuted, Is.True, "Validation Callback called");
				// assert the exception type
				Assert.That (ex, Is.Not.Null, (result is null) ? "Expected exception is missing and got no result" : $"Expected exception but got {result.Content.ReadAsStringAsync ().Result}");
				Assert.That (ex, Is.InstanceOf (typeof (HttpRequestException)), "Exception type");
				Assert.That (ex.InnerException, Is.Not.Null, "InnerException");
				Assert.That (ex.InnerException, Is.InstanceOf (expectedExceptionType), "InnerException type");
			}
		}

		[TestCase (typeof (HttpClientHandler))]
		[TestCase (typeof (NSUrlSessionHandler))]
		public void AcceptSslCertificatesServicePointManager (Type handlerType)
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			// bool servicePointManagerCbWasExcuted = false;

			var handler = GetHandler (handlerType);
			if (handler is NSUrlSessionHandler ns) {
				ns.TrustOverrideForUrl += (a, b, c) => {
					// servicePointManagerCbWasExcuted = true;
					return true;
				};
			} else {
#pragma warning disable SYSLIB0014 // 'ServicePointManager' is obsolete: 'WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead. Settings on ServicePointManager no longer affect SslStream or HttpClient.' (https://aka.ms/dotnet-warnings/SYSLIB0014)
				ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => {
#pragma warning restore SYSLIB0014
					// servicePointManagerCbWasExcuted = true;
					return true;
				};
			}

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				try {
					HttpClient client = new HttpClient (handler);
					var byteArray = new UTF8Encoding ().GetBytes ("username:password");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String (byteArray));
					var result = await client.GetAsync (NetworkResources.MicrosoftUrl);
				} finally {
#pragma warning disable SYSLIB0014 // 'ServicePointManager' is obsolete: 'WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead. Settings on ServicePointManager no longer affect SslStream or HttpClient.' (https://aka.ms/dotnet-warnings/SYSLIB0014)
					ServicePointManager.ServerCertificateValidationCallback = null;
#pragma warning restore SYSLIB0014
				}
			}, out var ex);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				// assert that we did not get an exception
				if (ex is not null && ex.InnerException is not null) {
					// we could get here.. if we have a diff issue, in that case, lets get the exception message and assert is not the trust issue
					Assert.That (ex.InnerException.Message, Is.Not.EqualTo ("Error: TrustFailure"));
				}
			}
			TestRuntime.IgnoreInCIIfBadNetwork (ex);
			Assert.That (ex, Is.Null);
			// Assert.That (servicePointManagerCbWasExcuted, Is.True, "Executed");
		}

		[Ignore ("https://github.com/dotnet/macios/issues/21912")]
		[TestCase ("https://self-signed.badssl.com/")]
		[TestCase ("https://wrong.host.badssl.com/")]
		public void AcceptSslCertificatesWithCustomValidationCallbackNSUrlSessionHandler (string url)
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			bool callbackWasExecuted = false;
			HttpResponseMessage result = null;
			X509Certificate2 serverCertificate = null;
			SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;

			var handler = new NSUrlSessionHandler {
				ServerCertificateCustomValidationCallback = (request, certificate, chain, errors) => {
					callbackWasExecuted = true;
					serverCertificate = certificate;
					sslPolicyErrors = errors;
					return true;
				}
			};

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var client = new HttpClient (handler);
				result = await client.GetAsync (url);
			}, out var ex);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				Assert.That (callbackWasExecuted, Is.True, "Validation Callback called");
				Assert.That (sslPolicyErrors, Is.Not.EqualTo (SslPolicyErrors.None), "Callback was called with unexpected SslPolicyErrors");
				Assert.That (serverCertificate, Is.Not.Null, "Server certificate is null");
				Assert.That (ex, Is.Null, "Exception wasn't expected.");
				Assert.That (result, Is.Not.Null, "Result was null");
				Assert.That (result.IsSuccessStatusCode, Is.True, $"Status code was not success: {result.StatusCode}");
			}
		}

		[TestCase ("https://www.microsoft.com/")]
		public void RejectSslCertificatesWithCustomValidationCallbackNSUrlSessionHandler (string url)
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			bool callbackWasExecuted = false;
			Exception ex2 = null;
			HttpResponseMessage result = null;

			var handler = new NSUrlSessionHandler {
				ServerCertificateCustomValidationCallback = (sender, certificate, chain, errors) => {
					callbackWasExecuted = true;
					try {
						Assert.That (certificate, Is.Not.Null);
						if (errors == SslPolicyErrors.RemoteCertificateChainErrors && TestRuntime.IsInCI)
							return false;
						Assert.That (errors, Is.EqualTo (SslPolicyErrors.None));
					} catch (ResultStateException) {
						throw;
					} catch (Exception e) {
						ex2 = e;
					}
					return false;
				}
			};

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var client = new HttpClient (handler);
				result = await client.GetAsync (url);
			}, out var ex);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				Assert.That (callbackWasExecuted, Is.True, "Validation Callback called.");
				Assert.That (ex, Is.Not.Null, result is null ? "Expected exception is missing and got no result." : $"Expected exception but got {result.Content.ReadAsStringAsync ().Result}.");
				Assert.That (ex, Is.InstanceOf (typeof (HttpRequestException)), "Exception type");
				Assert.That (ex.InnerException, Is.Not.Null, "InnerException");
				Assert.That (ex.InnerException, Is.InstanceOf (typeof (WebException)), "InnerException type");
				Assert.That (ex2, Is.Null, "Callback asserts");
			}
		}

		[Test]
		public void TestNSUrlSessionHandlerSendClientCertificate ()
		{
			string certificate_base64 = "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";
			// make the cert exportable so that the tests pass: ref: https://github.com/dotnet/runtime/issues/21581

#if __MACOS__
			// The test requires access to the default system keychain on macOS 14 or earlier, which is really
			// annoying on bots (a password dialog will pop up, blocking the tests). So just not run this test
			// on anything earlier than macOS 15.0,
			TestRuntime.AssertXcodeVersion (16, 0);
			var storageFlags = X509KeyStorageFlags.Exportable;
#else
			var storageFlags = X509KeyStorageFlags.DefaultKeySet;
#endif

			X509Certificate2 certificate = X509CertificateLoader.LoadPkcs12 (global::System.Convert.FromBase64String (certificate_base64), "test", storageFlags);
			string content = "";
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using var handler = new NSUrlSessionHandler ();
				handler.ClientCertificates.Add (certificate);
				using var client = new HttpClient (handler);
				var response = await client.GetAsync (NetworkResources.EchoClientCertificateUrl);
				content = await response.EnsureSuccessStatusCode ().Content.ReadAsStringAsync ();
			}, out var ex);
			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				TestRuntime.IgnoreInCIIfBadNetwork (ex);
				Assert.That (ex, Is.Null, "Exception wasn't expected.");
				X509Certificate2 certificate2 = X509CertificateLoader.LoadCertificate (global::System.Convert.FromBase64String (content));
				Assert.That (certificate2.Thumbprint, Is.EqualTo (certificate.Thumbprint));
			}
		}

		[Test]
		public void TestNSUrlSessionHandlerOptionalClientCertificate ()
		{
			NWListener? listener = null;
			try {
				listener = TlsTestServer.CreateNWTlsListener (requireClientCert: false);
				var port = listener.Port;

				var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
					using var handler = new NSUrlSessionHandler ();
					handler.TrustOverrideForUrl = (sender, url, trust) => true;
					using var client = new HttpClient (handler);
					var response = await client.GetAsync ($"https://localhost:{port}/");
					response.EnsureSuccessStatusCode ();
				}, out var ex);
				Assert.That (done, Is.True, "Request to localhost timed out.");
				Assert.That (ex, Is.Null, $"Exception wasn't expected, but got: {ex}");
			} finally {
				listener?.Cancel ();
				listener?.Dispose ();
			}
		}

		[Test]
		public void TestNSUrlSessionHandlerDetectMissingClientCertificate ()
		{
			NWListener? listener = null;
			try {
				listener = TlsTestServer.CreateNWTlsListener (requireClientCert: true);
				var port = listener.Port;

				var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
					using var handler = new NSUrlSessionHandler ();
					handler.TrustOverrideForUrl = (sender, url, trust) => true;
					using var client = new HttpClient (handler);
					await client.GetAsync ($"https://localhost:{port}/");
				}, out var ex);
				Assert.That (done, Is.True, "Request to localhost timed out.");
				Assert.That (ex, Is.Not.Null, "Exception was expected.");
				Assert.That (ex, Is.InstanceOf (typeof (HttpRequestException)), "Exception");
				Assert.That (ex!.InnerException, Is.InstanceOf (typeof (WebException)), "InnerException Type");
				Assert.That (((WebException) ex.InnerException!).Status, Is.EqualTo (WebExceptionStatus.SecureChannelFailure), "InnerException Status");
				Assert.That (ex.InnerException.InnerException, Is.InstanceOf (typeof (AuthenticationException)), "InnerException.InnerException Type");
			} finally {
				listener?.Cancel ();
				listener?.Dispose ();
			}
		}

		[Test]
		public void TestNSUrlSessionHandlerDetectMissingClientCertificateOptOut ()
		{
			AppContext.TryGetSwitch ("Foundation.NSUrlSessionHandler.NoMissingCertificateHandling", out var originalValue);
			NWListener? listener = null;
			try {
				AppContext.SetSwitch ("Foundation.NSUrlSessionHandler.NoMissingCertificateHandling", true);
				listener = TlsTestServer.CreateNWTlsListener (requireClientCert: true);
				var port = listener.Port;

				var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
					using var handler = new NSUrlSessionHandler ();
					handler.TrustOverrideForUrl = (sender, url, trust) => true;
					using var client = new HttpClient (handler);
					await client.GetAsync ($"https://localhost:{port}/");
				}, out var ex);
				Assert.That (done, Is.True, "Request to localhost timed out.");
				// With the opt-out switch enabled, the new specific exception is not thrown.
				// Instead we get a generic connection error (no WebException/AuthenticationException chain).
				Assert.That (ex, Is.Not.Null, "Exception was expected.");
				Assert.That (ex, Is.InstanceOf (typeof (HttpRequestException)), "Exception");
				if (ex!.InnerException is WebException we)
					Assert.That (we.Status, Is.Not.EqualTo (WebExceptionStatus.SecureChannelFailure), "Should not be SecureChannelFailure");
			} finally {
				AppContext.SetSwitch ("Foundation.NSUrlSessionHandler.NoMissingCertificateHandling", originalValue);
				listener?.Cancel ();
				listener?.Dispose ();
			}
		}

		static HttpResponseMessage GetResponseWithTimeout (HttpClient client, Uri uri)
		{
			HttpResponseMessage? response = null;
			var stopwatch = Stopwatch.StartNew ();
			Console.WriteLine ($"RedirectBasicAuthServer entering TestRuntime.TryRunAsync for {uri}.");
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				Console.WriteLine ($"RedirectBasicAuthServer starting HttpClient.GetAsync for {uri} after {stopwatch.ElapsedMilliseconds} ms.");
				var receivedResponse = await client.GetAsync (uri);
				response = receivedResponse;
				Console.WriteLine ($"RedirectBasicAuthServer completed HttpClient.GetAsync for {uri} with status {(int) receivedResponse.StatusCode} after {stopwatch.ElapsedMilliseconds} ms.");
			}, out var exception);
			Console.WriteLine ($"RedirectBasicAuthServer returned from TestRuntime.TryRunAsync for {uri}: done={done}, exception={exception?.GetType ().FullName ?? "<none>"}, elapsed={stopwatch.ElapsedMilliseconds} ms.");

			Assert.That (done, Is.True, $"Request to {uri} timed out.");
			Assert.That (exception, Is.Null, $"Exception while requesting {uri}.");
			return response ?? throw new InvalidOperationException ($"Request to {uri} completed without a response.");
		}

		sealed class RedirectBasicAuthServer : IDisposable {
			readonly bool crossOrigin;
			readonly HttpListener originListener;
			readonly HttpListener? targetListener;
			readonly Task originTask;
			readonly Task? targetTask;
			readonly object targetAuthorizationHeadersLock = new object ();
			readonly List<string> targetAuthorizationHeaders = new List<string> ();
			readonly string expectedBasicAuth;
			int targetRequestCount;

			public Uri OriginUri { get; }
			public Uri TargetUri { get; }
			public int TargetRequestCount => Volatile.Read (ref targetRequestCount);

			public string [] TargetAuthorizationHeaders {
				get {
					lock (targetAuthorizationHeadersLock)
						return targetAuthorizationHeaders.ToArray ();
				}
			}

			public RedirectBasicAuthServer (bool crossOrigin, string username = "origin-user", string password = "origin-password")
			{
				this.crossOrigin = crossOrigin;
				expectedBasicAuth = "Basic " + Convert.ToBase64String (global::System.Text.Encoding.UTF8.GetBytes ($"{username}:{password}"));
				originListener = CreateStartedHttpListener (out var originUri);
				OriginUri = originUri;

				if (crossOrigin) {
					targetListener = CreateStartedHttpListener (out var targetUri);
					TargetUri = targetUri;
				} else {
					TargetUri = OriginUri;
				}

				originTask = Task.Run (RunOrigin);
				if (targetListener is not null)
					targetTask = Task.Run (RunTarget);
			}

			async Task RunOrigin ()
			{
				while (true) {
					var context = await GetContextAsync (originListener);
					if (context is null)
						return;

					if (!crossOrigin && string.Equals (context.Request.Url?.AbsolutePath, "/protected", StringComparison.Ordinal)) {
						RespondToProtectedResource (context);
					} else {
						RespondWithRedirect (context);
					}
				}
			}

			async Task RunTarget ()
			{
				if (targetListener is null)
					return;

				while (true) {
					var context = await GetContextAsync (targetListener);
					if (context is null)
						return;

					RespondToProtectedResource (context);
				}
			}

			void RespondWithRedirect (HttpListenerContext context)
			{
				var response = context.Response;
				response.StatusCode = (int) HttpStatusCode.Redirect;
				response.RedirectLocation = new Uri (TargetUri, "protected").AbsoluteUri;
				response.ContentLength64 = 0;
				response.KeepAlive = false;
				response.Close ();
			}

			void RespondToProtectedResource (HttpListenerContext context)
			{
				Interlocked.Increment (ref targetRequestCount);

				var authorization = context.Request.Headers ["Authorization"];
				if (!string.IsNullOrEmpty (authorization)) {
					lock (targetAuthorizationHeadersLock)
						targetAuthorizationHeaders.Add (authorization);
				}

				var response = context.Response;
				if (string.Equals (authorization, expectedBasicAuth, StringComparison.Ordinal)) {
					response.StatusCode = (int) HttpStatusCode.OK;
				} else {
					response.StatusCode = (int) HttpStatusCode.Unauthorized;
					response.AddHeader ("WWW-Authenticate", "Basic realm=\"redirect-target\"");
				}
				response.ContentLength64 = 0;
				response.KeepAlive = false;
				response.Close ();
			}

			static async Task<HttpListenerContext?> GetContextAsync (HttpListener listener)
			{
				try {
					return await listener.GetContextAsync ();
				} catch (HttpListenerException) {
					return null;
				} catch (ObjectDisposedException) {
					return null;
				} catch (InvalidOperationException) {
					return null;
				}
			}

			static HttpListener CreateStartedHttpListener (out Uri uri)
			{
				const int MinPort = 49215;
				const int MaxPort = 65535;
				const int PortRange = MaxPort - MinPort;
				const int MaxAttempts = 50;
				Exception? lastException = null;
				var stopwatch = Stopwatch.StartNew ();

				// Pick a random start port, then probe sequentially (with wraparound) so
				// each attempt tries a distinct port while keeping the bounded retry behavior.
				var startPort = Random.Shared.Next (MinPort, MaxPort);
				for (var attempt = 0; attempt < MaxAttempts; attempt++) {
					var port = MinPort + (startPort - MinPort + attempt) % PortRange;
					var listener = new HttpListener ();
					var url = $"http://127.0.0.1:{port}/";
					listener.Prefixes.Add (url);
					try {
						listener.Start ();
						uri = new Uri (url);
						Console.WriteLine ($"RedirectBasicAuthServer listening on {uri} after {attempt + 1} attempt(s) in {stopwatch.ElapsedMilliseconds} ms.");
						return listener;
					} catch (Exception ex) {
						lastException = ex;
						listener.Close ();
					}
				}

				throw new InvalidOperationException ($"Could not start a local HTTP listener after {MaxAttempts} attempts.", lastException);
			}

			public void Dispose ()
			{
				Console.WriteLine ($"RedirectBasicAuthServer stopping origin {OriginUri} and target {TargetUri}.");
				originListener.Close ();
				targetListener?.Close ();

				try {
					if (targetTask is null)
						Task.WaitAll (new [] { originTask }, TimeSpan.FromSeconds (1));
					else
						Task.WaitAll (new [] { originTask, targetTask }, TimeSpan.FromSeconds (1));
				} catch {
					// Listener disposal wakes the request loops.
				}
				Console.WriteLine ($"RedirectBasicAuthServer stopped origin {OriginUri} and target {TargetUri}.");
			}
		}

		[Test]
		public void AssertDefaultValuesNSUrlSessionHandler ()
		{
			using (var handler = new NSUrlSessionHandler ()) {
				Assert.That (handler.AllowAutoRedirect, Is.True, "Default redirects value");
				Assert.That (handler.AllowsCellularAccess, Is.True, "Default cellular data value.");
			}
			using (var config = NSUrlSessionConfiguration.DefaultSessionConfiguration) {
				config.AllowsCellularAccess = false;
				using (var handler = new NSUrlSessionHandler (config)) {
					Assert.That (handler.AllowsCellularAccess, Is.False, "Configuration cellular data value.");
				}
			}
		}

		[TestCase (HttpStatusCode.OK, "mandel", "12345678", "mandel", "12345678")]
		[TestCase (HttpStatusCode.Unauthorized, "mandel", "12345678", "mandel", "87654321")]
		[TestCase (HttpStatusCode.Unauthorized, "mandel", "12345678", "", "")]
		public void GHIssue8342 (HttpStatusCode expectedStatus, string validUsername, string validPassword, string username, string password)
		{
			// create a http client to use with some creds that we do know are not valid
			var handler = new NSUrlSessionHandler () {
				Credentials = new NetworkCredential (username, password, "")
			};

			var client = new HttpClient (handler);

			HttpStatusCode httpStatus = HttpStatusCode.NotFound;
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var result = await client.GetAsync (NetworkResources.Httpbin.GetBasicAuthUrl (validUsername, validPassword));
				httpStatus = result.StatusCode;
			}, out var ex);

			Assert.That (done, Is.True, "Request completed");
			Assert.That (ex, Is.Null, "Exception not null");
			Assert.That (httpStatus, Is.EqualTo (expectedStatus), "Status not ok");
		}

		[TestCase (HttpStatusCode.OK, "mandel", "12345678", "mandel", "12345678")]
		[TestCase (HttpStatusCode.Unauthorized, "mandel", "12345678", "mandel", "87654321")]
		[TestCase (HttpStatusCode.Unauthorized, "mandel", "12345678", "", "")]
		public void SupportsDigestAuthentication (HttpStatusCode expectedStatus, string validUsername, string validPassword, string username, string password)
		{
			var handler = new NSUrlSessionHandler () {
				Credentials = new NetworkCredential (username, password, "")
			};

			var client = new HttpClient (handler);

			HttpStatusCode httpStatus = HttpStatusCode.NotFound;
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var result = await client.GetAsync (NetworkResources.Httpbin.GetDigestAuthUrl (validUsername, validPassword));
				httpStatus = result.StatusCode;
			}, out var ex);

			Assert.That (done, Is.True, "Request completed");
			Assert.That (ex, Is.Null, "Exception not null");
			Assert.That (httpStatus, Is.EqualTo (expectedStatus), "Status not ok");
		}

		[TestCase]
		public void GHIssue8344 ()
		{
			var username = "mandel";
			var password = "12345678";
			var url = NetworkResources.Httpbin.GetBasicAuthUrl (username, password);
			// perform two requests, one that will get a 200 with valid creds, one that wont and assert that
			// the second call does get a 401
			// create a http client to use with some creds that we do know are not valid
			var firstHandler = new NSUrlSessionHandler () {
				Credentials = new NetworkCredential (username, password, "")
			};

			var firstClient = new HttpClient (firstHandler);

			HttpStatusCode httpStatus = HttpStatusCode.NotFound;
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var result = await firstClient.GetAsync (url);
				httpStatus = result.StatusCode;
			}, out var ex);

			Assert.That (done, Is.True, "First request completed");
			Assert.That (ex, Is.Null, "First request exception not null");
			Assert.That (httpStatus, Is.EqualTo (HttpStatusCode.OK), "First status not ok");

			// exactly same operation, diff handler, wrong password, should fail

			var secondHandler = new NSUrlSessionHandler () {
				Credentials = new NetworkCredential (username, password + password, "")
			};

			var secondClient = new HttpClient (secondHandler);

			httpStatus = HttpStatusCode.NotFound;
			done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				var result = await secondClient.GetAsync (url);
				httpStatus = result.StatusCode;
			}, out ex);

			Assert.That (done, Is.True, "Second request completed");
			Assert.That (ex, Is.Null, "Second request exception not null");
			Assert.That (httpStatus, Is.EqualTo (HttpStatusCode.Unauthorized), "Second status not ok");
		}

		class TestDelegateHandler : DelegatingHandler {
			public int Iterations;
			public HttpResponseMessage [] Responses;

			public TestDelegateHandler (int iterations)
			{
				Responses = new HttpResponseMessage [iterations];
				Iterations = iterations;
			}

			public bool IsCompleted (int iteration)
			{
				return Responses [iteration] is not null;
			}

			protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
			{
				// test that we can perform a retry with the same request
				for (var i = 0; i < Iterations; i++)
					Responses [i] = await base.SendAsync (request, cancellationToken);
				return Responses.Last ();
			}
		}

		[TestCase]
		public void GHIssue16339 ()
		{
			// test that we can perform two diff requests with the same managed HttpRequestMessage
			var json = "{this:\"\", is:\"a\", test:2}";
			var iterations = 2;
			var bodies = new string [iterations];

			var request = new HttpRequestMessage {
				Method = HttpMethod.Post,
				RequestUri = new (NetworkResources.Httpbin.PostUrl),
				Content = new StringContent (json, Encoding.UTF8, "application/json")
			};

			using var delegatingHandler = new TestDelegateHandler (iterations) {
				InnerHandler = new NSUrlSessionHandler (),
			};

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using var client = new HttpClient (delegatingHandler);
				var _ = await client.SendAsync (request);
				for (var i = 0; i < iterations; i++) {
					if (delegatingHandler.IsCompleted (i))
						bodies [i] = await delegatingHandler.Responses [i].Content.ReadAsStringAsync ();
				}
			}, out var ex);

			Assert.That (done, Is.True, "Request completed");
			Assert.That (ex, Is.Null, "Exception");

			for (var i = 0; i < iterations; i++) {
				var rsp = delegatingHandler.Responses [i];
				Assert.That (delegatingHandler.IsCompleted (i), Is.True, $"Completed #{i}");
				Assert.That (rsp.ReasonPhrase, Is.EqualTo ("OK"), $"ReasonPhrase #{i}");
				Assert.That (rsp.StatusCode, Is.EqualTo (HttpStatusCode.OK), $"StatusCode #{i}");

				var body = bodies [i];
				// Poor-man's json parser
				var data = body.Split ('\n', '\r').Single (v => v.Contains ("\"data\": \""));
				data = data.Trim ().Replace ("\"data\": \"", "").TrimEnd ('"', ',');
				data = data.Replace ("\\\"", "\"");

				Assert.That (data, Is.EqualTo (json), $"Post data #{i}");
			}
		}

		[TestCase (typeof (NSUrlSessionHandler))]
		[TestCase (typeof (SocketsHttpHandler))]
		public void UpdateRequestUriAfterRedirect (Type handlerType)
		{
			// https://github.com/dotnet/macios/issues/20629

			using var handler = GetHandler (handlerType);
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using var client = new HttpClient (handler, disposeHandler: false) {
					Timeout = TimeSpan.FromSeconds (20),
				};
				var postRequestUri = NetworkResources.Httpbin.Url + "/";
				var initialRequestUri = NetworkResources.Httpbin.GetRedirectToUrl (postRequestUri);
				using var request = new HttpRequestMessage (HttpMethod.Get, initialRequestUri);
				Assert.That (request.RequestUri.ToString (), Is.EqualTo (initialRequestUri), "Initial RequestUri");
				using var response = await client.SendAsync (request);
				Assert.That (request.RequestUri.ToString (), Is.EqualTo (postRequestUri), "Post RequestUri");
			}, out var ex);

			Assert.That (done, Is.True, "Request completed");
			Assert.That (ex, Is.Null, "Exception");
		}

		[TestCase (typeof (NSUrlSessionHandler))]
		[TestCase (typeof (SocketsHttpHandler))]
		public void RequestUriNotUpdatedIfNotRedirect (Type handlerType)
		{
			// https://github.com/dotnet/macios/issues/20629

			using var handler = GetHandler (handlerType);
			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				using var client = new HttpClient (handler, disposeHandler: false) {
					Timeout = TimeSpan.FromSeconds (20),
				};
				var requestUri = NetworkResources.Httpbin.Uri + "?stuffHere=[]{}";
				using var request = new HttpRequestMessage (HttpMethod.Get, requestUri);
				Assert.That (request.RequestUri.ToString (), Is.EqualTo (requestUri), "Initial RequestUri");
				using var response = await client.SendAsync (request);
				Assert.That (request.RequestUri.ToString (), Is.EqualTo (requestUri), "Post RequestUri");
			}, out var ex);

			Assert.That (done, Is.True, "Request completed");
			Assert.That (ex, Is.Null, "Exception");
		}

		// https://github.com/dotnet/macios/issues/23764
		[TestCase ("https://sha256.badssl.com/")]
		public void SslCertificatesWithoutOCSPEndPointsNSUrlSessionHandler_AllowByDefault (string url)
		{
			SslCertificatesWithoutOCSPEndPointsNSUrlSessionHandler (url, null, SslPolicyErrors.None);
		}

		// https://github.com/dotnet/macios/issues/23764
		[TestCase ("https://sha256.badssl.com/")]
		public void SslCertificatesWithoutOCSPEndPointsNSUrlSessionHandler_Disallow (string url)
		{
			SslCertificatesWithoutOCSPEndPointsNSUrlSessionHandler (url, (X509VerificationFlags) 0, SslPolicyErrors.RemoteCertificateChainErrors);
		}

		void SslCertificatesWithoutOCSPEndPointsNSUrlSessionHandler (string url, X509VerificationFlags? verificationFlags, SslPolicyErrors expectedError)
		{
			bool callbackWasExecuted = false;
			HttpResponseMessage result = null;
			X509Certificate2 serverCertificate = null;
			SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
			Exception ex = null;

			var handler = new NSUrlSessionHandler {
				ServerCertificateCustomValidationCallback = (request, certificate, chain, errors) => {
					callbackWasExecuted = true;
					serverCertificate = certificate;
					sslPolicyErrors = errors;
					return true;
				},
			};
			if (verificationFlags.HasValue)
				handler.CertificateChainPolicy.VerificationFlags = verificationFlags.Value;

			Assert.That (handler.CheckCertificateRevocationList, Is.True, "CheckCertificateRevocationList");

			for (var i = 0; i < 3; i++) {
				callbackWasExecuted = false;
				result = null;
				serverCertificate = null;
				sslPolicyErrors = SslPolicyErrors.None;

				var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
					using var client = new HttpClient (handler);
					// Disable keep-alive and cache to force reconnection for each request
					client.DefaultRequestHeaders.ConnectionClose = true;
					client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true, NoStore = true };
					result = await client.GetAsync (url);
				}, out ex);

				if (!done) // timeouts happen in the bots due to dns issues, connection issues etc., we do not want to fail
					Assert.Inconclusive ("Request timedout.");

				if (callbackWasExecuted)
					break;

				TestRuntime.IgnoreInCIIfBadNetwork (ex);
				if (result is not null)
					TestRuntime.IgnoreInCIIfBadNetwork (result.StatusCode);

				if (ex is not null)
					break;
				if (result is null || !result.IsSuccessStatusCode)
					break;
			}

			if (!callbackWasExecuted)
				Assert.Inconclusive ("Validation callback was not called.");

			Assert.That (sslPolicyErrors, Is.EqualTo (expectedError), "Callback was called with unexpected SslPolicyErrors");
			Assert.That (serverCertificate, Is.Not.Null, "Server certificate is null");
			Assert.That (ex, Is.Null, "Exception wasn't expected.");
			Assert.That (result, Is.Not.Null, "Result was null");
			Assert.That (result.IsSuccessStatusCode, Is.True, $"Status code was not success: {result.StatusCode}");
		}
	}
}
