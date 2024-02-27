//
// MessageHandlers.cs
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
#if NET
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif
using System.Linq;
using System.IO;

using NUnit.Framework;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using Foundation;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTests.System.Net.Http {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MessageHandlerTest {
		public MessageHandlerTest ()
		{
			// Https seems broken on our macOS 10.9 bot, so skip this test.
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
		}

		HttpMessageHandler GetHandler (Type handler_type)
		{
#if !__WATCHOS__
			if (handler_type == typeof (HttpClientHandler))
				return new HttpClientHandler ();
			if (handler_type == typeof (CFNetworkHandler))
				return new CFNetworkHandler ();
#endif
#if NET
			if (handler_type == typeof (SocketsHttpHandler))
				return new SocketsHttpHandler ();
#endif
			if (handler_type == typeof (NSUrlSessionHandler))
				return new NSUrlSessionHandler ();

			throw new NotImplementedException ($"Unknown handler type: {handler_type}");
		}

		[Test]
#if !__WATCHOS__
		[TestCase (typeof (HttpClientHandler))]
		[TestCase (typeof (CFNetworkHandler))]
#if NET
		[TestCase (typeof (SocketsHttpHandler))]
#endif
#endif
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

			Assert.IsTrue (done, "Did not time out");
			Assert.IsNull (response, $"Response is not null {response}");
			Assert.IsInstanceOf (typeof (HttpRequestException), ex, "Exception");
		}

#if !__WATCHOS__
		// ensure that we do get the same cookies as the managed handler
		[Test]
		public void TestNSUrlSessionHandlerCookies ()
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

				var nativeHandler = new NSUrlSessionHandler () {
					AllowAutoRedirect = false,
				};
				nativeHandler.AllowAutoRedirect = true;
				var nativeClient = new HttpClient (nativeHandler);
				var nativeResponse = await nativeClient.GetAsync (url);
				nativeCookieResult = nativeResponse.Headers.TryGetValues ("Set-Cookie", out nativeCookies);
			}, out var ex);

			if (!completed || !managedCookieResult || !nativeCookieResult)
				TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
			Assert.IsTrue (completed, "Network request completed");
			Assert.IsNull (ex, "Exception");
			Assert.IsTrue (managedCookieResult, $"Failed to get managed cookies");
			Assert.IsTrue (nativeCookieResult, $"Failed to get native cookies");
			Assert.AreEqual (1, managedCookies.Count (), $"Managed Cookie Count");
			Assert.AreEqual (1, nativeCookies.Count (), $"Native Cookie Count");
			Assert.That (nativeCookies.First (), Does.StartWith ("cookie=chocolate-chip;"), $"Native Cookie Value");
			Assert.That (managedCookies.First (), Does.StartWith ("cookie=chocolate-chip;"), $"Managed Cookie Value");
		}

		// ensure that we can use a cookie container to set the cookies for a url
		[Test]
		public void TestNSUrlSessionHandlerCookieContainer ()
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

				var nativeHandler = new NSUrlSessionHandler () {
					AllowAutoRedirect = true,
					CookieContainer = cookieContainer,
				};
				var nativeClient = new HttpClient (nativeHandler);
				var nativeResponse = await nativeClient.GetAsync (url);
				nativeCookieResult = await nativeResponse.Content.ReadAsStringAsync ();
			}, out var ex);

			if (!completed || managedCookieResult.Contains ("502 Bad Gateway") || nativeCookieResult.Contains ("502 Bad Gateway") || managedCookieResult.Contains ("504 Gateway Time-out") || nativeCookieResult.Contains ("504 Gateway Time-out"))
				TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
			Assert.IsTrue (completed, "Network request completed");
			Assert.IsNull (ex, "Exception");
			Assert.IsNotNull (managedCookieResult, "Managed cookies result");
			Assert.IsNotNull (nativeCookieResult, "Native cookies result");
			Assert.AreEqual (managedCookieResult, nativeCookieResult, "Cookies");
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

			if (!completed)
				TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
			Assert.IsTrue (completed, "Network request completed");
			Assert.IsNull (ex, "Exception");
			Assert.IsNotNull (nativeCookieResult, "Native cookies result");
			var cookiesFromServer = cookieContainer.GetCookies (new Uri (url));
			if (cookiesFromServer.Count != 1)
				TestRuntime.IgnoreInCI ("Unexpected network failure in CI");
			Assert.AreEqual (1, cookiesFromServer.Count, "Cookies received from server.");
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

			if (!completed)
				TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
			Assert.IsTrue (completed, "Network request completed");
			Assert.IsNull (ex, "Exception");
			Assert.IsNotNull (nativeSetCookieResult, "Native set-cookies result");
			Assert.IsNotNull (nativeCookieResult, "Native cookies result");
			Assert.IsFalse (nativeCookieResult.Contains ("chocolate-chip"));
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

			if (!completed)
				TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
			Assert.IsTrue (completed, "Network request completed");
			Assert.IsNull (ex, "Exception");
			Assert.IsNotNull (nativeSetCookieResult, "Native set-cookies result");
			Assert.IsNotNull (nativeCookieResult, "Native cookies result");
			Assert.IsFalse (nativeCookieResult.Contains ("chocolate-chip"));
			var cookiesFromServer = cookieContainer.GetCookies (new Uri (url));
			Assert.AreEqual (0, cookiesFromServer.Count, "Cookies received from server.");
		}

		[Test]
		public void TestNSUrlSessionEphemeralDisabledCookies ()
		{
			// assert we do throw an exception with ephmeral configs.
			using (var config = NSUrlSessionConfiguration.EphemeralSessionConfiguration) {
				Assert.True (config.SessionType == NSUrlSessionConfiguration.SessionConfigurationType.Ephemeral, "Session type.");
				var nativeHandler = new NSUrlSessionHandler (config);
				Assert.Throws<InvalidOperationException> (() => {
					nativeHandler.UseCookies = false;
				});
			}
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

			Assert.IsNull (ex, "Exception");

			if (!timeoutExceptionShouldHaveBeenThrown) {
				Assert.Inconclusive ("Failed to produce a timeout. The response content was streamed completely.");
			} else {
				Assert.IsTrue (timeoutExceptionWasThrown, "Timeout exception is thrown.");
			}
		}

#endif

		// ensure that if we have a redirect, we do not have the auth headers in the following requests
#if !__WATCHOS__
		[TestCase (typeof (HttpClientHandler))]
		[TestCase (typeof (CFNetworkHandler))]
#endif
		[TestCase (typeof (NSUrlSessionHandler))]
		public void RedirectionWithAuthorizationHeaders (Type handlerType)
		{

			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			bool containsAuthorizarion = false;
			bool containsHeaders = false;
			string json = "";

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				HttpClient client = new HttpClient (GetHandler (handlerType));
				client.BaseAddress = NetworkResources.Httpbin.Uri;
				var byteArray = new UTF8Encoding ().GetBytes ("username:password");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String (byteArray));
				var result = await client.GetAsync (NetworkResources.Httpbin.GetRedirectUrl (3));
				// get the data returned from httpbin which contains the details of the requested performed.
				json = await result.Content.ReadAsStringAsync ();
				containsAuthorizarion = json.Contains ("Authorization");
				containsHeaders = json.Contains ("headers");  // ensure we do have the headers in the response
			}, out var ex);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else if (!containsHeaders) {
				Assert.Inconclusive ("Response from httpbin does not contain headers, therefore we cannot ensure that if the authoriation is present.");
			} else {
				Assert.IsFalse (containsAuthorizarion, $"Authorization header did reach the final destination. {json}");
				Assert.IsNull (ex, $"Exception {ex} for {json}");
			}
		}

#if !__WATCHOS__
#if !NET // By default HttpClientHandler redirects to a NSUrlSessionHandler, so no need to test that here.
		[TestCase (typeof (HttpClientHandler))]
#endif
#endif
#if NET
		[TestCase (typeof (SocketsHttpHandler))]
#endif
		[TestCase (typeof (NSUrlSessionHandler))]
		public void RejectSslCertificatesServicePointManager (Type handlerType)
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

#if __MACOS__
			if (handlerType == typeof (NSUrlSessionHandler) && TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 10, 0) && !TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 11, 0))
				Assert.Ignore ("Fails on macOS 10.10: https://github.com/xamarin/maccore/issues/1645");
#endif

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
				ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => {
					invalidServicePointManagerCbWasExcuted = true;
					return false;
				};
#if NET
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
#endif // NET
			} else if (handler is NSUrlSessionHandler ns) {
				expectedExceptionType = typeof (WebException);
#if NET
				ns.TrustOverrideForUrl += (a, b, c) => {
#else
				ns.TrustOverride += (a, b) => {
#endif
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
					client.BaseAddress = NetworkResources.Httpbin.Uri;
					var byteArray = new UTF8Encoding ().GetBytes ("username:password");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String (byteArray));
					result = await client.GetAsync (NetworkResources.Httpbin.GetRedirectUrl (3));
				} finally {
					ServicePointManager.ServerCertificateValidationCallback = null;
				}
			}, out var ex);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				// the ServicePointManager.ServerCertificateValidationCallback will never be executed.
				Assert.False (invalidServicePointManagerCbWasExcuted, "Invalid SPM executed");
				Assert.True (validationCbWasExecuted, "Validation Callback called");
				// assert the exception type
				Assert.IsNotNull (ex, (result is null) ? "Expected exception is missing and got no result" : $"Expected exception but got {result.Content.ReadAsStringAsync ().Result}");
				Assert.IsInstanceOf (typeof (HttpRequestException), ex, "Exception type");
				Assert.IsNotNull (ex.InnerException, "InnerException");
				Assert.IsInstanceOf (expectedExceptionType, ex.InnerException, "InnerException type");
			}
		}

#if !__WATCHOS__
		[TestCase (typeof (HttpClientHandler))]
#endif
		[TestCase (typeof (NSUrlSessionHandler))]
		public void AcceptSslCertificatesServicePointManager (Type handlerType)
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			// bool servicePointManagerCbWasExcuted = false;

			var handler = GetHandler (handlerType);
			if (handler is NSUrlSessionHandler ns) {
#if NET
				ns.TrustOverrideForUrl += (a, b, c) => {
#else
				ns.TrustOverride += (a, b) => {
#endif
					// servicePointManagerCbWasExcuted = true;
					return true;
				};
			} else {
				ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => {
					// servicePointManagerCbWasExcuted = true;
					return true;
				};
			}

			var done = TestRuntime.TryRunAsync (TimeSpan.FromSeconds (30), async () => {
				try {
					HttpClient client = new HttpClient (handler);
					client.BaseAddress = NetworkResources.Httpbin.Uri;
					var byteArray = new UTF8Encoding ().GetBytes ("username:password");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String (byteArray));
					var result = await client.GetAsync (NetworkResources.Httpbin.GetRedirectUrl (3));
				} finally {
					ServicePointManager.ServerCertificateValidationCallback = null;
				}
			}, out var ex);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				// assert that we did not get an exception
				if (ex is not null && ex.InnerException is not null) {
					// we could get here.. if we have a diff issue, in that case, lets get the exception message and assert is not the trust issue
					Assert.AreNotEqual (ex.InnerException.Message, "Error: TrustFailure");
				}
			}
			Assert.IsNull (ex);
			// Assert.IsTrue (servicePointManagerCbWasExcuted, "Executed");
		}

#if NET
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
				Assert.True (callbackWasExecuted, "Validation Callback called");
				Assert.AreNotEqual (SslPolicyErrors.None, sslPolicyErrors, "Callback was called with unexpected SslPolicyErrors");
				Assert.IsNotNull (serverCertificate, "Server certificate is null");
				Assert.IsNull (ex, "Exception wasn't expected.");
				Assert.IsNotNull (result, "Result was null");
				Assert.IsTrue (result.IsSuccessStatusCode, "Status code was not success");
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
						Assert.IsNotNull (certificate);
						if (errors == SslPolicyErrors.RemoteCertificateChainErrors && TestRuntime.IsInCI)
							return false;
						Assert.AreEqual (SslPolicyErrors.None, errors);
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
				Assert.True (callbackWasExecuted, "Validation Callback called.");
				Assert.IsNotNull (ex, result is null ? "Expected exception is missing and got no result." : $"Expected exception but got {result.Content.ReadAsStringAsync ().Result}.");
				Assert.IsInstanceOf (typeof (HttpRequestException), ex, "Exception type");
				Assert.IsNotNull (ex.InnerException, "InnerException");
				Assert.IsInstanceOf (typeof (WebException), ex.InnerException, "InnerException type");
				Assert.IsNull (ex2, "Callback asserts");
			}
		}
#endif

		[Test]
		public void AssertDefaultValuesNSUrlSessionHandler ()
		{
			using (var handler = new NSUrlSessionHandler ()) {
				Assert.True (handler.AllowAutoRedirect, "Default redirects value");
				Assert.True (handler.AllowsCellularAccess, "Default cellular data value.");
			}
			using (var config = NSUrlSessionConfiguration.DefaultSessionConfiguration) {
				config.AllowsCellularAccess = false;
				using (var handler = new NSUrlSessionHandler (config)) {
					Assert.False (handler.AllowsCellularAccess, "Configuration cellular data value.");
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

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				TestRuntime.IgnoreInCIIfBadNetwork (httpStatus);
				Assert.IsNull (ex, "Exception not null");
				Assert.AreEqual (expectedStatus, httpStatus, "Status not ok");
			}
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

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("First request timedout.");
			} else {
				TestRuntime.IgnoreInCIIfBadNetwork (httpStatus);
				Assert.IsNull (ex, "First request exception not null");
				Assert.AreEqual (HttpStatusCode.OK, httpStatus, "First status not ok");
			}
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

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Second request timedout.");
			} else {
				TestRuntime.IgnoreInCIIfBadNetwork (httpStatus);
				Assert.IsNull (ex, "Second request exception not null");
				Assert.AreEqual (HttpStatusCode.Unauthorized, httpStatus, "Second status not ok");
			}
		}
		class TestDelegateHandler : DelegatingHandler {
			public bool FirstRequestCompleted { get; private set; } = false;
			public bool SecondRequestCompleted { get; private set; } = false;
			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				// test that we can perform a retry with the same request
				var _ = await base.SendAsync(request, cancellationToken);
				FirstRequestCompleted = true;
				var response = await base.SendAsync(request, cancellationToken);
				SecondRequestCompleted = true;
				return response;
			}
		}

		[TestCase]
		public void GHIssue16339 ()
		{
			// test that we can perform two diff requests with the same managed HttpRequestMessage
			bool done = false;
			bool firstRequestCompleted = false;
			bool secondRequestCompleted = false;
			Exception ex = null;
			var json = "{this:\"\", is:\"a\", test:2}";
			
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new (NetworkResources.Httpbin.PostUrl),
				Content = new StringContent (json, Encoding.UTF8, "application/json")
			};

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{
				try {
					using var delegatingHandler = new TestDelegateHandler {
						InnerHandler = new NSUrlSessionHandler (),
					};
					using HttpClient client = new(delegatingHandler);
					var _= await client.SendAsync(request);
					firstRequestCompleted = delegatingHandler.FirstRequestCompleted;
					secondRequestCompleted = delegatingHandler.SecondRequestCompleted;
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
				}
			}, () => done);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				Assert.IsNull (ex, "Exception");
				Assert.IsTrue (firstRequestCompleted, "First request");
				Assert.IsTrue (secondRequestCompleted, "Second request");
			} 
		}
	}
}
