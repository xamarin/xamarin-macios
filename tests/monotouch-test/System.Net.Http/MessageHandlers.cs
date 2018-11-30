//
// MessageHandlers.cs
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.IO;

using NUnit.Framework;
#if MONOMAC
using Foundation;
#endif
using ObjCRuntime;

namespace MonoTests.System.Net.Http
{
	[TestFixture]
	public class MessageHandlerTest
	{
		void PrintHandlerToTest ()
		{
#if !__WATCHOS__
			Console.WriteLine (new HttpClientHandler ());
			Console.WriteLine (new CFNetworkHandler ());
#endif
			Console.WriteLine (new NSUrlSessionHandler ());
		}

		HttpMessageHandler GetHandler (Type handler_type)
		{
			return (HttpMessageHandler) Activator.CreateInstance (handler_type);
		}

		[Test]
#if !__WATCHOS__
		[TestCase (typeof (HttpClientHandler))]
		[TestCase (typeof (CFNetworkHandler))]
#endif
		[TestCase (typeof (NSUrlSessionHandler))]
		public void DnsFailure (Type handlerType)
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);

			PrintHandlerToTest ();

			bool done = false;
			Exception ex = null;

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{
				try {
					HttpClient client = new HttpClient (GetHandler (handlerType));
					var s = await client.GetStringAsync ("http://doesnotexist.xamarin.com");
					Console.WriteLine (s);
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
				}
			}, () => done);

			Assert.IsTrue (done, "Did not time out");
			Assert.IsNotNull (ex, "Exception");
			// The handlers throw different types of exceptions, so we can't assert much more than that something went wrong.			
		}

	}}
