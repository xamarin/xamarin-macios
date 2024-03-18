using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using Foundation;

using NUnit.Framework;
using MonoTests.System.Net.Http;

namespace LinkAnyTest {
	// This test is included in both the LinkAll and LinkSdk projects.
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LinkAnyTest {
		[Test]
		public void AES ()
		{
			Assert.NotNull (Aes.Create (), "AES");
		}

		static bool waited;
		static bool requestError;
		static HttpStatusCode statusCode;

		void TimedWait (Task task)
		{
			var rv = task.Wait (TimeSpan.FromMinutes (1));
			if (rv)
				return;

			TestRuntime.IgnoreInCI ("This test times out randomly in CI due to bad network.");
			Assert.Fail ("Test timed out");
		}

		// http://blogs.msdn.com/b/csharpfaq/archive/2012/06/26/understanding-a-simple-async-program.aspx
		// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=7114
		static async Task GetWebPageAsync ()
		{
			// do not use GetStringAsync, we are going to miss useful data, such as the resul code
			using (var client = new HttpClient ()) {
				HttpResponseMessage response = await client.GetAsync ("http://example.com");
				if (!response.IsSuccessStatusCode) {
					requestError = true;
					statusCode = response.StatusCode;
				} else {
					string content = await response.Content.ReadAsStringAsync ();
					waited = true;
					bool success = !String.IsNullOrEmpty (content);
					Assert.IsTrue (success, $"received {content.Length} bytes");
				}
			}
		}

		[Test]
		public void GetWebPageAsyncTest ()
		{
			var current_sc = SynchronizationContext.Current;
			try {
				// we do not want the async code to get back to the AppKit thread, hanging the process
				SynchronizationContext.SetSynchronizationContext (null);
				TimedWait (GetWebPageAsync ());
				if (requestError) {
					Assert.Inconclusive ($"Test cannot be trusted. Issues performing the request. Status code '{statusCode}'");
				} else {
					Assert.IsTrue (waited, "async/await worked");
				}
			} finally {
				SynchronizationContext.SetSynchronizationContext (current_sc);
			}
		}

		void WebClientTest (string [] urls)
		{
			var exceptions = new List<string> ();
			foreach (var url in urls) {
				try {
					var wc = new WebClient ();
					var data = wc.DownloadString (url);

					Assert.That (data, Is.Not.Empty, "Downloaded content");
					return; // one url succeeded, that's enough
				} catch (Exception e) {
					var msg = $"Url '{url}' failed: {e.ToString ()}";
					Console.WriteLine (msg); // If this keeps occurring locally for the same url, we might have to take it off the list of urls to test.
					exceptions.Add (msg);
				}
			}
			Assert.That (exceptions, Is.Empty, "At least one url should work");
		}

		[Test]
		public void WebClientTest_Http ()
		{
			WebClientTest (NetworkResources.HttpUrls);
		}

		[Test]
		public void WebClientTest_Https ()
		{
			WebClientTest (NetworkResources.HttpsUrls);
		}

		[Test]
		public void WebClientTest_Async ()
		{
			var current_sc = SynchronizationContext.Current;
			try {
				// we do not want the async code to get back to the AppKit thread, hanging the process
				SynchronizationContext.SetSynchronizationContext (null);

				string data = null;

				var exceptions = new List<string> ();
				foreach (var url in NetworkResources.HttpsUrls) {
					try {
						async Task GetWebPage (string url)
						{
							var wc = new WebClient ();
							var task = wc.DownloadStringTaskAsync (new Uri (url));
							data = await task;
						}

						TimedWait (GetWebPage (url));
						Assert.That (data, Is.Not.Empty, "Downloaded content");
						return; // one url succeeded, that's enough
					} catch (Exception e) {
						var msg = $"Url '{url}' failed: {e.ToString ()}";
						Console.WriteLine (msg); // If this keeps occurring locally for the same url, we might have to take it off the list of urls to test.
						exceptions.Add (msg);
					}
				}
				Assert.That (exceptions, Is.Empty, "At least one url should work");
			} finally {
				SynchronizationContext.SetSynchronizationContext (current_sc);

			}
		}
	}
}
