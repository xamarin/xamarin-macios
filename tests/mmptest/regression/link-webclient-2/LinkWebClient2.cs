using System;
using System.IO;
using System.Net;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * application use WebClient which uses HttpWeb[Request|Response] which depends on System.Configuration
// * application use HTTPS to ensure the SSL/TLS stack in included
// * dependencies check will include Mono.Security.dll for SSL/TLS support
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class WebClient2 {

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);
			
			bool success = false;
			string message;
			
			try {
				string url = "https://mail.google.com";
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