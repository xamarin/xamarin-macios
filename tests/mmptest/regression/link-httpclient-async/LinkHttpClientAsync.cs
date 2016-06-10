using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MonoMac.Foundation;
using MonoMac.AppKit;

// Test
// * application references [CallerFilePath] which is new in .NET 4.5
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class HttpClientAsync {

		static bool waited = false;
		
		// http://blogs.msdn.com/b/csharpfaq/archive/2012/06/26/understanding-a-simple-async-program.aspx
		// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=7114
		// CS4009 entry point cannot be async
		static async Task GetWebPageAsync ()
		{
			Task<string> getWebPageTask = new HttpClient ().GetStringAsync ("http://msdn.microsoft.com");
			string content = await getWebPageTask;
			waited = true;
			bool success = !String.IsNullOrEmpty (content);
			Test.Log.WriteLine ("{0}\treceived {1} bytes", success ? "[PASS]" : "[FAIL]", success ? content.Length : 0);
		}

		static void Main (string[] args)
		{
			NSApplication.Init ();
			// we do not want the async code to get back to the AppKit thread, hanging the process
			SynchronizationContext.SetSynchronizationContext (null);

			Test.EnsureLinker (true);
			GetWebPageAsync ().Wait ();

			Test.Log.WriteLine ("{0}\tasync / await worked", waited ? "[PASS]" : "[FAIL]");
			Test.Terminate ();
		}
	}
}