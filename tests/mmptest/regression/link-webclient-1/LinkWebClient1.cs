// Copyright 2012-2013 Xamarin Inc. All rights reserved.

using System;
using System.Net;
using MonoMac.AppKit;

// Test
// * application use WebClient which uses HttpWeb[Request|Response] which depends on System.Configuration
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	class WebClient1 {

		static void Main (string[] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			bool success = false;
			string message;

			try {
				string url = "http://www.google.com";
				WebClient wc = new WebClient ();
				string data = wc.DownloadString (url);
			
				success = !String.IsNullOrEmpty (data);
				message = String.Format ("WebClient on '{0}' {1}.", url, success ? "succeeded" : "FAILED");
			}
			catch (Exception e) {
				message = e.ToString ();
			}

			Test.Log.WriteLine ("{0}\t{1}", success ? "[PASS]" : "[FAIL]", message);
			Test.Terminate ();
		}
	}
}