using System;
using System.Net.Http;
using System.Threading.Tasks;
using Foundation;
using NUnit.Framework;
using MonoTests.System.Net.Http;

namespace LinkSdk {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AsyncTests {

		public Task<string> LoadCategories ()
		{
			return Task.Run (async () => await (new HttpClient ()).GetStringAsync (NetworkResources.MicrosoftUrl));
		}

#if NET
		[Ignore ("System.EntryPointNotFoundException: AppleCryptoNative_SecKeychainItemCopyKeychain")] // https://github.com/dotnet/runtime/issues/36897
#endif
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