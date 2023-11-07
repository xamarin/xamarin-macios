using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Foundation;

using NUnit.Framework;

namespace MonoTests.System.Net.Http {
	[Preserve (AllMembers = true)]
	public static class NetworkResources {
		public static string MicrosoftUrl => AssertNetworkConnection ("https://www.microsoft.com");
		public static Uri MicrosoftUri => new Uri (MicrosoftUrl);
		public static string MicrosoftHttpUrl => AssertNetworkConnection ("http://www.microsoft.com");
		public static string XamarinUrl => AssertNetworkConnection ("https://dotnet.microsoft.com/apps/xamarin");
		public static string XamarinHttpUrl => AssertNetworkConnection ("http://dotnet.microsoft.com/apps/xamarin");
		public static Uri XamarinUri => new Uri (XamarinUrl);
		public static string StatsUrl => AssertNetworkConnection ("https://api.imgur.com/2/stats");

		public static string [] HttpsUrls => new [] {
			MicrosoftUrl,
			XamarinUrl,
			Httpbin.Url,
		};

		public static string [] HttpUrls => new [] {
			MicrosoftHttpUrl,
			XamarinHttpUrl,
			Httpbin.HttpUrl,
		};

		// Robots urls, useful when we want to get a small file
		public static string MicrosoftRobotsUrl => AssertNetworkConnection ("https://www.microsoft.com/robots.txt");
		public static string XamarinRobotsUrl => AssertNetworkConnection ("https://www.xamarin.com/robots.txt");
		public static string BingRobotsUrl => AssertNetworkConnection ("http://www.bing.com/robots.txt");
		public static string XboxRobotsUrl => AssertNetworkConnection ("https://www.xbox.com/robots.txt");
		public static string MSNRobotsUrl => AssertNetworkConnection ("https://www.msn.com/robots.txt");
		public static string VisualStudioRobotsUrl => AssertNetworkConnection ("https://visualstudio.microsoft.com/robots.txt");

		public static string [] RobotsUrls => new [] {
			MicrosoftRobotsUrl,
			XamarinRobotsUrl,
			BingRobotsUrl,
			XboxRobotsUrl,
			MSNRobotsUrl,
			VisualStudioRobotsUrl,
		};

		static Dictionary<string, bool> sites = new Dictionary<string, bool> ();
		static bool VerifyNetworkConnection (string url, int timeoutInSeconds = 5)
		{
			bool value;
			lock (sites) {
				if (sites.TryGetValue (url, out value)) {
					return value;
				}
			}
			value = VerifyNetworkConnectionImpl (url, timeoutInSeconds);
			lock (sites) {
				sites [url] = value;
			}
			return value;
		}

		static bool VerifyNetworkConnectionImpl (string url, int timeoutInSeconds = 5)
		{
			try {
				var client = new HttpClient ();
				var task = client.GetStringAsync (url);
				if (task.Wait (TimeSpan.FromSeconds (timeoutInSeconds)))
					return task.IsCompletedSuccessfully;
				TestRuntime.NSLog ($"VerifyNetworkConnection ({url}) => task timed out, status: {task.Status}");
				return false;
			} catch (Exception e) {
				TestRuntime.NSLog ($"VerifyNetworkConnection ({url}) => {e}");
				return false;
			}
		}

		static string AssertNetworkConnection (string url)
		{
			if (!VerifyNetworkConnection (url)) {
				TestRuntime.NSLog ($"AssertNetworkConnection ({url}): failure");
				Assert.Ignore ($"The site {url} is not accessible. This test will be ignored.");
			}
			return url;
		}

		public static class Httpbin {
			static bool? isHttpbinDown;

			public static string Url => AssertNetworkConnection ("https://httpbin.org");
			public static Uri Uri => new Uri ($"{Url}");
			public static string DeleteUrl => $"{Url}/delete";
			public static string GetUrl => $"{Url}/get";
			public static string PatchUrl => $"{Url}/patch";
			public static string PostUrl => $"{Url}/post";
			public static string PutUrl => $"{Url}/put";
			public static string CookiesUrl => $"{Url}/cookies";
			public static string HttpUrl => AssertNetworkConnection ("http://httpbin.org");

			public static string GetAbsoluteRedirectUrl (int count) => $"{Url}/absolute-redirect/{count}";
			public static string GetRedirectUrl (int count) => $"{Url}/redirect/{count}";
			public static string GetRelativeRedirectUrl (int count) => $"{Url}/relative-redirect/{count}";
			public static string GetStatusCodeUrl (HttpStatusCode status) => $"{HttpUrl}/status/{(int) status}";
			public static string GetSetCookieUrl (string cookie, string value) => $"{Url}/cookies/set?{cookie}={value}";
			public static string GetBasicAuthUrl (string username, string password) => $"{Url}/basic-auth/{username}/{password}";

		}
	}
}
