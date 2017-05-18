using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#if XAMCORE_2_0
using Foundation;
#else
using MonoTouch.Foundation;
#endif
using NUnit.Framework;


namespace MonoTouchFixtures.HttpClientTests
{
	[TestFixture]
	public class HttpClientTest
	{
		const int WaitTimeout = 5000;

		[Test]
		public void CFNetworkEnsureModifiabilityPostSend ()
		{
			var handler = new CFNetworkHandler ();
			using (var client = new HttpClient (handler))
			using (var request = new HttpRequestMessage (HttpMethod.Get, "http://xamarin.com")) {
				Assert.DoesNotThrow (() => handler.AllowAutoRedirect = !handler.AllowAutoRedirect);
				Task.Factory.StartNew (() => {
					var token = new CancellationTokenSource ();
					client.SendAsync (request, token.Token).Wait (WaitTimeout);
					Assert.Throws<InvalidOperationException> (() => handler.AllowAutoRedirect = !handler.AllowAutoRedirect);
					// cancel to ensure that we do not have side effects
					token.Cancel ();
				});
			}
		}

		[Test]
		public void NSUrlSessionEnsureModifiabilityPostSend ()
		{
			var handler = new NSUrlSessionHandler ();
			using (var client = new HttpClient (handler))
			using (var request = new HttpRequestMessage (HttpMethod.Get, "http://xamarin.com")) {
				Assert.DoesNotThrow (() => handler.AllowAutoRedirect = !handler.AllowAutoRedirect);
				Task.Factory.StartNew (() => {
					var token = new CancellationTokenSource ();
					client.SendAsync (request, token.Token).Wait (WaitTimeout);
					Assert.Throws<InvalidOperationException> (() => handler.AllowAutoRedirect = !handler.AllowAutoRedirect);
					// cancel to ensure that we do not have side effects
					token.Cancel ();
				});
			}
		}
	}
}
