//
// Unit tests CFHTTPMessage
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Net;
#if NET
using CFNetwork;
#else
using CoreServices;
#endif
using Foundation;
using CoreFoundation;
using NUnit.Framework;
using MonoTests.System.Net.Http;
using System.Threading;
using static CoreFoundation.CFStream;
using System.Threading.Tasks;

namespace MonoTouchFixtures.CoreServices {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HttpMessageTest {

		[Test]
		public void CreateEmptyTrue ()
		{
			using (var m = CFHTTPMessage.CreateEmpty (true)) {
				Assert.That (m.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.False (m.IsHeaderComplete, "IsHeaderComplete");
				Assert.True (m.IsRequest, "IsRequest");
				Assert.Throws<InvalidOperationException> (delegate { var x = m.ResponseStatusCode; }, "ResponseStatusCode");
				Assert.Throws<InvalidOperationException> (delegate { var x = m.ResponseStatusLine; }, "ResponseStatusLine");
				Assert.That (m.Version.ToString (), Is.EqualTo ("1.1"), "Version");
				Assert.That (TestRuntime.CFGetRetainCount (m.Handle), Is.EqualTo ((nint) 1), "RetainCount");
			}
		}

		[Test]
		public void CreateEmptyFalse ()
		{
			using (var m = CFHTTPMessage.CreateEmpty (false)) {
				Assert.That (m.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.False (m.IsHeaderComplete, "IsHeaderComplete");
				Assert.False (m.IsRequest, "IsRequest");
				Assert.That (m.ResponseStatusCode, Is.EqualTo (HttpStatusCode.OK), "ResponseStatusCode");
				Assert.That (m.ResponseStatusLine, Is.Empty, "ResponseStatusLine");
				Assert.That (m.Version.ToString (), Is.EqualTo ("1.1"), "Version");
				Assert.That (TestRuntime.CFGetRetainCount (m.Handle), Is.EqualTo ((nint) 1), "RetainCount");
			}
		}

		[Test]
		public void CreateRequest10 ()
		{
			using (var m = CFHTTPMessage.CreateRequest (NetworkResources.XamarinUri, "GET", new Version (1, 0))) {
				Assert.That (m.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.False (m.IsHeaderComplete, "IsHeaderComplete");
				Assert.True (m.IsRequest, "IsRequest");
				Assert.Throws<InvalidOperationException> (delegate { var x = m.ResponseStatusCode; }, "ResponseStatusCode");
				Assert.Throws<InvalidOperationException> (delegate { var x = m.ResponseStatusLine; }, "ResponseStatusLine");
				Assert.That (m.Version.ToString (), Is.EqualTo ("1.0"), "Version");
				Assert.That (TestRuntime.CFGetRetainCount (m.Handle), Is.EqualTo ((nint) 1), "RetainCount");
			}
		}

		[Test]
		public void CreateResponseAuth ()
		{
			CFHTTPMessage response = null;
			var done = false;
			var taskCompletionSource = new TaskCompletionSource<CFHTTPMessage> ();
			// the following code has to be in a diff thread, else, we are blocking the current loop, not cool
			// perform a request so that we fail in the auth, then create the auth object and check the count
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				using (var request = CFHTTPMessage.CreateRequest (
					new Uri (NetworkResources.Httpbin.GetStatusCodeUrl (HttpStatusCode.Unauthorized)), "GET", null)) {
					request.SetBody (Array.Empty<byte> ()); // empty body, we are not interested
					using (var stream = CFStream.CreateForHTTPRequest (request)) {
						Assert.IsNotNull (stream, "Null stream");
						// we are only interested in the completed event
						stream.ClosedEvent += (sender, e) => {
							taskCompletionSource.SetResult (stream.GetResponseHeader ());
							done = true;
						};
						// enable events and run in the current loop
						stream.EnableEvents (CFRunLoop.Main, CFRunLoop.ModeDefault);
						stream.Open ();
						response = await taskCompletionSource.Task;
					}
				}
			}, () => done);
			if (!done)
				TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
			Assert.IsTrue (done, "Network request completed");
			using (var auth = CFHTTPAuthentication.CreateFromResponse (response)) {
				Assert.NotNull (auth, "Null Auth");
				Assert.IsTrue (auth.IsValid, "Auth is valid");
				Assert.That (TestRuntime.CFGetRetainCount (auth.Handle), Is.EqualTo ((nint) 1), "RetainCount");
			}
		}
	}
}

#endif // !__WATCHOS__
