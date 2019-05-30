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
using System.Linq;
using System.IO;

using NUnit.Framework;
using System.Net.Http.Headers;
using System.Text;
#if MONOMAC
using Foundation;
#endif
using ObjCRuntime;

namespace MonoTests.System.Net.Http
{
	[TestFixture]
	public class MessageHandlerTest
	{
		void PrintHandlerToTest ()
		{
#if !__WATCHOS__
			Console.WriteLine (new HttpClientHandler ());
			Console.WriteLine (new CFNetworkHandler ());
#endif
			Console.WriteLine (new NSUrlSessionHandler ());
		}

		HttpMessageHandler GetHandler (Type handler_type)
		{
			return (HttpMessageHandler) Activator.CreateInstance (handler_type);
		}

		[Test]
#if !__WATCHOS__
		[TestCase (typeof (HttpClientHandler))]
		[TestCase (typeof (CFNetworkHandler))]
#endif
		[TestCase (typeof (NSUrlSessionHandler))]
		public void DnsFailure (Type handlerType)
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);

			PrintHandlerToTest ();

			bool done = false;
			string response = null;
			Exception ex = null;

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{
				try {
					HttpClient client = new HttpClient (GetHandler (handlerType));
					response = await client.GetStringAsync ("http://doesnotexist.xamarin.com");
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
				}
			}, () => done);

			Assert.IsTrue (done, "Did not time out");
			Assert.IsNull (response, $"Response is not null {response}");
			Assert.IsNotNull (ex, "Exception");
			// The handlers throw different types of exceptions, so we can't assert much more than that something went wrong.			
		}

#if !__WATCHOS__
		// ensure that we do get the same number of cookies as the managed handler
		[TestCase]
		public void TestNSUrlSessionHandlerCookies ()
		{
			bool areEqual = false;
			var manageCount = 0;
			var nativeCount = 0;
			Exception ex = null;

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{
				try {
					var managedClient = new HttpClient (new HttpClientHandler ());
					var managedResponse = await managedClient.GetAsync ("https://google.com");
					if (managedResponse.Headers.TryGetValues ("Set-Cookie", out var managedCookies)) {
						var nativeClient = new HttpClient (new NSUrlSessionHandler ());
						var nativeResponse = await nativeClient.GetAsync ("https://google.com");
						if (managedResponse.Headers.TryGetValues ("Set-Cookie", out var nativeCookies)) {
							manageCount = managedCookies.Count ();
							nativeCount = nativeCookies.Count ();
							areEqual = manageCount == nativeCount;
						} else {
							manageCount = -1;
							nativeCount = -1;
							areEqual = false;
						}
					}
					
				} catch (Exception e) {
					ex = e;
				} 
			}, () => areEqual);

			Assert.IsTrue (areEqual, $"Cookies are different - Managed {manageCount} vs Native {nativeCount}");
			Assert.IsNull (ex, "Exception");
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

			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false); 

			bool containsAuthorizarion = false;
			bool containsHeaders = false;
			string json = "";
			bool done = false;
			Exception ex = null;

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{
				try {
					HttpClient client = new HttpClient (GetHandler (handlerType));
					client.BaseAddress = new Uri ("https://httpbin.org");
					var byteArray = new UTF8Encoding ().GetBytes ("username:password");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String(byteArray));
					var result = await client.GetAsync ("https://httpbin.org/redirect/3");
					// get the data returned from httpbin which contains the details of the requested performed.
					json = await result.Content.ReadAsStringAsync ();
					containsAuthorizarion = json.Contains ("Authorization");
					containsHeaders = json.Contains ("headers");  // ensure we do have the headers in the response
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
				}
			}, () => done);

			if (!done) { // timeouts happen in the bost due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else if (!containsHeaders) {
				Assert.Inconclusive ("Response from httpbin does not contain headers, therefore we cannot ensure that if the authoriation is present.");
			} else {
				Assert.IsFalse (containsAuthorizarion, $"Authorization header did reach the final destination. {json}");
				Assert.IsNull (ex, $"Exception {ex} for {json}");
			}
		}

#if !__WATCHOS__
		[TestCase (typeof (HttpClientHandler))]
#endif
		[TestCase (typeof (NSUrlSessionHandler))]
		public void RejectSslCertificatesServicePointManager (Type handlerType)
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);

#if __MACOS__
			if (handlerType == typeof (NSUrlSessionHandler) && TestRuntime.CheckSystemVersion (PlatformName.MacOSX, 10, 10, 0) && !TestRuntime.CheckSystemVersion (PlatformName.MacOSX, 10, 11, 0))
				Assert.Ignore ("Fails on macOS 10.10: https://github.com/xamarin/maccore/issues/1645");
#endif

			bool servicePointManagerCbWasExcuted = false;
			bool done = false;
			Exception ex = null;
			HttpResponseMessage result = null;

			var handler = GetHandler (handlerType);
			if (handler is NSUrlSessionHandler ns) {
				ns.TrustOverride += (a,b) => {
					servicePointManagerCbWasExcuted = true;
					// return false, since we want to test that the exception is raised
					return false;
				};
			} else {
				ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => {
					servicePointManagerCbWasExcuted = true;
					// return false, since we want to test that the exception is raised
					return false;
				};
			}

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{
				try {
					HttpClient client = new HttpClient (handler);
					client.BaseAddress = new Uri ("https://httpbin.org");
					var byteArray = new UTF8Encoding ().GetBytes ("username:password");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String(byteArray));
					result = await client.GetAsync ("https://httpbin.org/redirect/3");
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
					ServicePointManager.ServerCertificateValidationCallback = null;
				}
			}, () => done);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				// assert the exception type
				Assert.IsNotNull (ex, (result == null)? "Expected exception is missing and got no result" : $"Expected exception but got {result.Content.ReadAsStringAsync ().Result}");
				Assert.IsInstanceOfType (typeof (HttpRequestException), ex);
				Assert.IsNotNull (ex.InnerException);
				Assert.IsInstanceOfType (typeof (WebException), ex.InnerException);
			}
		}

#if !__WATCHOS__
		[TestCase (typeof (HttpClientHandler))]
#endif
		[TestCase (typeof (NSUrlSessionHandler))]
		public void AcceptSslCertificatesServicePointManager (Type handlerType)
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);

			bool servicePointManagerCbWasExcuted = false;
			bool done = false;
			Exception ex = null;

			var handler = GetHandler (handlerType);
			if (handler is NSUrlSessionHandler ns) {
				ns.TrustOverride += (a,b) => {
					servicePointManagerCbWasExcuted = true;
					return true;
				};
			} else {
				ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => {
					servicePointManagerCbWasExcuted = true;
					return true;
				};
			}

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{
				try {
					HttpClient client = new HttpClient (handler);
					client.BaseAddress = new Uri ("https://httpbin.org");
					var byteArray = new UTF8Encoding ().GetBytes ("username:password");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String(byteArray));
					var result = await client.GetAsync ("https://httpbin.org/redirect/3");
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
					ServicePointManager.ServerCertificateValidationCallback = null;
				}
			}, () => done);

			if (!done) { // timeouts happen in the bots due to dns issues, connection issues etc.. we do not want to fail
				Assert.Inconclusive ("Request timedout.");
			} else {
				// assert that we did not get an exception
				if (ex != null && ex.InnerException != null) {
					// we could get here.. if we have a diff issue, in that case, lets get the exception message and assert is not the trust issue
					Assert.AreNotEqual (ex.InnerException.Message, "Error: TrustFailure");
				}
			}
		}
	}
}
