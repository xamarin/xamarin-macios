using System;
using System.Net.Http;
using System.Threading.Tasks;
#if XAMCORE_2_0
using Foundation;
#else
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace LinkSdk {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AsyncTests {

		public Task<string> LoadCategories ()
		{
			return Task.Run (async () => await (new HttpClient ()).GetStringAsync ("https://google.com"));
		}

		[Test]
		public void Bug12221 ()
		{
#if __WATCHOS__
			Assert.Ignore ("WatchOS doesn't support BSD sockets, which our network stack currently requires.");
#endif
			LoadCategories ().Wait ();
		}
	}
}