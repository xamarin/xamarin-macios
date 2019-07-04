using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using Foundation;

using NUnit.Framework;

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
		// http://blogs.msdn.com/b/csharpfaq/archive/2012/06/26/understanding-a-simple-async-program.aspx
		// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=7114
		static async Task GetWebPageAsync ()
		{
			Task<string> getWebPageTask = new HttpClient ().GetStringAsync ("http://msdn.microsoft.com");
			string content = await getWebPageTask;
			waited = true;
			bool success = !String.IsNullOrEmpty (content);
			Assert.IsTrue (success, $"received {content.Length} bytes");
		}

		[Test]
		public void GetWebPageAsyncTest ()
		{
			var current_sc = SynchronizationContext.Current;
			try {
				// we do not want the async code to get back to the AppKit thread, hanging the process
				SynchronizationContext.SetSynchronizationContext (null);
				GetWebPageAsync ().Wait ();
				Assert.IsTrue (waited, "async/await worked");
			} finally {
				SynchronizationContext.SetSynchronizationContext (current_sc);
			}
		}
	}
}
