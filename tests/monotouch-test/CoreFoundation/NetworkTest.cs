//
// Unit tests for CFNetwork
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Foundation;
using CoreFoundation;
using NUnit.Framework;
// Mac tries to use CFNetwork Namespace instead of Class for calls without this:
#if !__WATCHOS__
using PlatformCFNetwork = CoreFoundation.CFNetwork;
#endif
using MonoTests.System.Net.Http;


namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NetworkTest {

		const string bug4715_url = "http://192.168.1.94:8080/telehealth/Services/External/Recipient.svc/List?OrgId={FDB521B6-1ADA-40D3-8AE9-0F59B9F2DB11}&Ticket=84DDB35C66B7EEF59C8B31D072A71C01E2F81158E98827C983FAF18C8B9D261A2D75680BFD6050B975E9F77EEEF1E9B235E631B957BC31D6C84CBDA6219DB11B2BC9F6BD39546158683F67A86947B034326A48B6E9F50C77D9A1578F50F26C861E514D1CE4721D011F037A1D2B0C91B7D60736B1021B7AC1A387BE28256794C7CF907B57CF2CA30F5D5D26CDAB55A986EDD8D00B9A6BD25FBADA1C583D6A13326851A92137F35DC69D4C565519E95365E6CA37FB60A8480B2297B106CE6DF9AC2A082B90D2755C2F4D73074CAFE1030512FC3A35";
#if !__TVOS__ && !__WATCHOS__
		CFProxySettings settings = PlatformCFNetwork.GetSystemProxySettings ();
#endif
		Uri uri = new Uri (bug4715_url);

#if !__TVOS__ && !__WATCHOS__
		[Test]
		public void WebProxy ()
		{
			IWebProxy proxy = PlatformCFNetwork.GetDefaultProxy ();
			Assert.True (proxy.IsBypassed (uri), "IsBypassed");
			Assert.That (proxy.GetProxy (uri), Is.SameAs (uri), "GetProxy");
		}

		[Test]
		public void GetProxiesForUri ()
		{
			var proxies = PlatformCFNetwork.GetProxiesForUri (uri, settings);
			Assert.That (proxies.Length, Is.EqualTo (1), "single");
			var p = proxies [0];
			Assert.Null (p.AutoConfigurationJavaScript, "AutoConfigurationJavaScript");
			Assert.Null (p.AutoConfigurationUrl, "AutoConfigurationUrl");
			Assert.Null (p.HostName, "HostName");
			Assert.That (p.Port, Is.EqualTo (0), "Port");
			Assert.Null (p.Password, "Password");
			Assert.That (p.ProxyType, Is.EqualTo (CFProxyType.None), "Type");
			Assert.Null (p.Username, "Username");
		}

		[Test]
		public void Bug_7923 ()
		{
			// Bug #7923 - crash when proxy is in effect.
			var uri = NetworkResources.MicrosoftUri;

			if (PlatformCFNetwork.GetProxiesForUri (uri, settings).Length <= 1)
				Assert.Ignore ("Only run when proxy is configured.");

			var req = WebRequest.CreateHttp (uri);
			using (var rsp = req.GetResponse ())
			using (var str = new StreamReader (rsp.GetResponseStream ()))
				Console.WriteLine (str.ReadToEnd ());
		}
#endif // !__TVOS__
	}
}
