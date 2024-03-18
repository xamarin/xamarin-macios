using System;
using System.IO;
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

		[Test]
		public void Bug12221 ()
		{
#if __WATCHOS__
			Assert.Ignore ("WatchOS doesn't support BSD sockets, which our network stack currently requires.");
#endif
			try {
				LoadCategories ().GetAwaiter ().GetResult ();
			} catch (TaskCanceledException tce) {
				TestRuntime.IgnoreInCI ("Ignore any download timeouts");
				throw;
			} catch (HttpRequestException hre) {
				TestRuntime.IgnoreInCIIfForbidden (hre); // Ignore any 403 errors.
				throw;
			} catch (IOException ie) {
				/*
				 *  LinkSdk.AsyncTests
				 *  	[FAIL] Bug12221 : System.Net.Http.HttpRequestException : The SSL connection could not be established, see inner exception.
				 *    ----> System.IO.IOException :  Received an unexpected EOF or 0 bytes from the transport stream.
				 *  		   at System.Net.Http.ConnectHelper.EstablishSslConnectionAsync(SslClientAuthenticationOptions , HttpRequestMessage , Boolean , Stream , CancellationToken )
				 *  		   at System.Net.Http.HttpConnectionPool.ConnectAsync(HttpRequestMessage , Boolean , CancellationToken )
				 *  		   at System.Net.Http.HttpConnectionPool.CreateHttp11ConnectionAsync(HttpRequestMessage , Boolean , CancellationToken )
				 *  		   at System.Net.Http.HttpConnectionPool.AddHttp11ConnectionAsync(QueueItem )
				 *  		   at System.Threading.Tasks.TaskCompletionSourceWithCancellation`1.<WaitWithCancellationAsync>d__1[[System.Net.Http.HttpConnection, System.Net.Http, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]].MoveNext()
				 *  		   at System.Net.Http.HttpConnectionPool.HttpConnectionWaiter`1.<WaitForConnectionAsync>d__5[[System.Net.Http.HttpConnection, System.Net.Http, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]].MoveNext()
				 *  		   at System.Net.Http.HttpConnectionPool.SendWithVersionDetectionAndRetryAsync(HttpRequestMessage , Boolean , Boolean , CancellationToken )
				 *  		   at System.Net.Http.RedirectHandler.SendAsync(HttpRequestMessage , Boolean , CancellationToken )
				 *  		   at System.Net.Http.HttpClient.GetStringAsyncCore(HttpRequestMessage , CancellationToken )
				 *  		   at LinkSdk.AsyncTests.<>c.<<LoadCategories>b__0_0>d.MoveNext() in /Users/builder/azdo/_work/2/s/xamarin-macios/tests/linker/ios/link sdk/AsyncTest.cs:line 16
				 *  		--- End of stack trace from previous location ---
				 *  		   at LinkSdk.AsyncTests.Bug12221() in /Users/builder/azdo/_work/2/s/xamarin-macios/tests/linker/ios/link sdk/AsyncTest.cs:line 32
				 *  		   at System.Reflection.MethodInvoker.InterpretedInvoke(Object , Span`1 , BindingFlags )
				 *  		--IOException
				 *  		   at System.Net.Security.SslStream.<ReceiveBlobAsync>d__147`1[[System.Net.Security.AsyncReadWriteAdapter, System.Net.Security, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]].MoveNext()
				 *  		   at System.Net.Security.SslStream.<ForceAuthenticationAsync>d__146`1[[System.Net.Security.AsyncReadWriteAdapter, System.Net.Security, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]].MoveNext()
				 *  		   at System.Net.Http.ConnectHelper.EstablishSslConnectionAsync(SslClientAuthenticationOptions , HttpRequestMessage , Boolean , Stream , CancellationToken )
				 */
				TestRuntime.IgnoreInCI ("Ignore any IO exceptions");
				throw;
			}
		}
	}
}
