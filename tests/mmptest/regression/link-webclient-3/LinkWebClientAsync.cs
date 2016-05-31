// Copyright 2012 Xamarin Inc. All rights reserved.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * application use WebClient which uses HttpWeb[Request|Response] which depends on System.Configuration
// * use the new, .NET 4.5, async API
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	class WebClientAsync {

		static bool success = false;

		static async Task GetWebPage (string url)
		{
			WebClient wc = new WebClient ();
			Task<string> task = wc.DownloadStringTaskAsync (new Uri (url));
			string data = await task;
			success = !String.IsNullOrEmpty (data);
		}

		static void Main (string[] args)
		{
			NSApplication.Init ();
			// we do not want the async code to get back to the AppKit thread, hanging the process
			SynchronizationContext.SetSynchronizationContext (null);

			Test.EnsureLinker (true);

			string url = "http://www.google.com";
			GetWebPage (url).Wait ();

			Test.Log.WriteLine ("{0}\tWebClient async on {1}", success ? "[PASS]" : "[FAIL]", url);
			Test.Terminate ();
		}
	}
}