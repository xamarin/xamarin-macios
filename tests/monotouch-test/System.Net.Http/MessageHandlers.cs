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
using Newtonsoft.Json;
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
			Exception ex = null;

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{
				try {
					HttpClient client = new HttpClient (GetHandler (handlerType));
					var s = await client.GetStringAsync ("http://doesnotexist.xamarin.com");
					Console.WriteLine (s);
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
				}
			}, () => done);

			Assert.IsTrue (done, "Did not time out");
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
		[Test]
#if !__WATCHOS__
		[TestCase (typeof (HttpClientHandler))]
		[TestCase (typeof (CFNetworkHandler))]
#endif
		[TestCase (typeof (NSUrlSessionHandler))]
		public void RedirectionWithAuthorizationHeaders (Type handlerType)
		{
			// anonymous type that represent the response
			var definition = new { 
				headers = new Dictionary<string, string> (),
				origin = "",
				url = ""
			};
			
			PrintHandlerToTest ();

			bool containsAuthorizarion = false;
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
					// get the data returned from httbin which contains the details of the requested performed.
					json = await result.Content.ReadAsStringAsync ();
					var httpRequest = JsonConvert.DeserializeAnonymousType (json, definition);
					containsAuthorizarion = httpRequest.headers.ContainsKey ("Authorization");
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
				}
			}, () => done);

			Assert.IsTrue (done, "Request timedout");
			Assert.IsFalse (containsAuthorizarion, $"Authorization header did reach the final destination. {json}");
			Assert.IsNull (ex, $"Exception {ex} for {json}");
		}
	}
}
