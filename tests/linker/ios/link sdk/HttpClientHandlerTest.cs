// Tests to ensure consistency of our handlers across updates
//
// Copyright 2016 Xamarin Inc.

using System;
using System.Net;
using System.Net.Http;
using Foundation;
using NUnit.Framework;

namespace LinkSdk.Net.Http {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HttpClientHandlerTest {

#if !__WATCHOS__
		[Test]
		public void HttpClient ()
		{
			using (var handler = new HttpClientHandler ()) {
				Assert.True (handler.AllowAutoRedirect, "AllowAutoRedirect");
				Assert.NotNull (handler.CookieContainer, "CookieContainer");
				Assert.Null (handler.Credentials, "Credentials");
				// (so far) not exposed in other, native handlers
				Assert.That (handler.AutomaticDecompression, Is.EqualTo (DecompressionMethods.None), "AutomaticDecompression");
				Assert.That (handler.ClientCertificateOptions, Is.EqualTo (ClientCertificateOption.Manual), "ClientCertificateOptions");
				Assert.That (handler.MaxAutomaticRedirections, Is.EqualTo (50), "MaxAutomaticRedirections");
				Assert.Null (handler.Proxy, "Proxy");
				Assert.True (handler.SupportsAutomaticDecompression, "SupportsAutomaticDecompression");
				Assert.True (handler.SupportsProxy, "SupportsProxy");
				Assert.True (handler.SupportsRedirectConfiguration, "SupportsRedirectConfiguration");
				Assert.True (handler.UseCookies, "UseCookies");
				Assert.False (handler.UseDefaultCredentials, "UseDefaultCredentials");
				Assert.True (handler.UseProxy, "UseProxy");
			}
		}

		[Test]
		public void CFNetwork ()
		{
			using (var handler = new CFNetworkHandler ()) {
				Assert.True (handler.AllowAutoRedirect, "AllowAutoRedirect");
				Assert.NotNull (handler.CookieContainer, "CookieContainer");
				// custom, not in HttpClientHandler
				Assert.False (handler.UseSystemProxy, "UseSystemProxy");
			}
		}
#endif

		[Test]
		public void NSUrlSession ()
		{
			using (var handler = new NSUrlSessionHandler ()) {
				Assert.True (handler.AllowAutoRedirect, "AllowAutoRedirect");
				Assert.Null (handler.Credentials, "Credentials");
				// custom, not in HttpClientHandler
				Assert.False (handler.DisableCaching, "DisableCaching");
			}
		}
	}
}
