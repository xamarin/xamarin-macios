//
// Unit tests for NSUrlProtocol
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
using NUnit.Framework;
using MonoTests.System.Net.Http;
using Xamarin.Utils;


namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlProtocolTest {
		[Test]
		public void Registration ()
		{
			Class c = new Class (typeof (CustomProtocol));
			bool res;

			res = NSUrlProtocol.RegisterClass (c);

			Assert.That (res, "#1");

			NSUrlProtocol.UnregisterClass (c);
		}

		class CustomProtocol : NSUrlProtocol {
		}

		// API disabled - see comments in src/foundation.cs
#if false
		[Test]
		public void CanInitWithTask ()
		{
			// NSInvalidArgumentException Reason: *** -canInitWithRequest: cannot be sent to an abstract object of class NSURLProtocol: Create a concrete instance!
			using (var t = new NSUrlSessionTask ()) {
				Assert.False (NSUrlProtocol.CanInitWithTask (t), "CanInitWithTask");
			}
		}

		[Test]
		public void Task ()
		{
			// NSInvalidArgumentException -[MonoTouchFixtures_Foundation_UrlProtocolTest_CustomProtocol task]: unrecognized selector sent to instance 0x7ff4c910
			using (var p = new CustomProtocol ()) {
				Assert.Null (p.Task, "Task");
			}
		}
#endif

#if !__WATCHOS__
		[Test]
		public void RegistrarTest ()
		{
			// Networking seems broken on our macOS 10.9 bot, so skip this test.
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			var success = false;

			var task = Task.Run (async () => {
				var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
				config.WeakProtocolClasses = NSArray.FromNSObjects (new Class (typeof (CustomUrlProtocol)));
				var session = NSUrlSession.FromConfiguration (config);
				var custom_url = new NSUrl ("foo://server");
				using (var task = await session.CreateDownloadTaskAsync (custom_url)) {
					success = true;
				}
			});

			Assert.IsTrue (TestRuntime.RunAsync (TimeSpan.FromSeconds (10), task), "Timed out");
			Assert.That (CustomUrlProtocol.State, Is.EqualTo (5), "State");
			Assert.IsTrue (success, "Success");
		}

		public class CustomUrlProtocol : NSUrlProtocol, INSUrlSessionDelegate, INSUrlSessionTaskDelegate, INSUrlSessionDataDelegate {

			public static int State;

			[Export ("canInitWithRequest:")]
			public static new bool CanInitWithRequest (NSUrlRequest request)
			{
				if (State == 0)
					State++;
				return true;
			}

			[Export ("canonicalRequestForRequest:")]
			public static new NSUrlRequest GetCanonicalRequest (NSUrlRequest request)
			{
				if (State == 1)
					State++;
				return request;
			}

			[Export ("initWithRequest:cachedResponse:client:")]
			public CustomUrlProtocol (NSUrlRequest request, NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client)
			: base (request, cachedResponse, client)
			{
				if (State == 2)
					State++;
			}

			[Export ("startLoading")]
			public override void StartLoading ()
			{
#if MONOMAC
				if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 10)) {
					if (State == 3)
						State++;
				} else {
					// looks like 10.9 is not calling `initWithRequest:cachedResponse:client:`
					if (State >= 2)
						State = 4;
				}
#else
				if (State == 3)
					State++;
#endif

				string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");

				using (var d = NSData.FromFile (file))
				using (var response = new NSUrlResponse (Request.Url, "image/png", (nint) d.Length, null)) {
					Client.ReceivedResponse (this, response, NSUrlCacheStoragePolicy.NotAllowed);
					Client.DataLoaded (this, d);
					Client.FinishedLoading (this);
				}
			}

			[Export ("stopLoading")]
			public override void StopLoading ()
			{
				if (State == 4)
					State++;
			}

			//NSURLSessionTaskDelegate
			[Export ("URLSession:task:willPerformHTTPRedirection:newRequest:completionHandler:")]
			public virtual void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest> completionHandler)
			{
				State = -1;
				completionHandler (newRequest);
			}

			[Export ("URLSession:task:didReceiveChallenge:completionHandler:")]
			public virtual void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
			{
				State = -2;
				completionHandler (NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, null);
			}

			//NSURLSessionDataDelegate
			[Export ("URLSession:dataTask:didReceiveResponse:completionHandler:")]
			public virtual void DidReceiveResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
			{
				State = -3;
				completionHandler (NSUrlSessionResponseDisposition.Allow);
				this.Client.ReceivedResponse (this, response, NSUrlCacheStoragePolicy.Allowed);
			}

			[Export ("URLSession:dataTask:didReceiveData:")]
			public virtual void DidReceiveData (NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data)
			{
				State = -4;
				this.Client.DataLoaded (this, data);
			}

			[Export ("URLSession:task:didCompleteWithError:")]
			public virtual void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error)
			{
				State = -5;
				if (error is not null) {
					this.Client.FailedWithError (this, error);
				} else {
					this.Client.FinishedLoading (this);
				}
			}
		}
#endif // !__WATCHOS__
	}
}
