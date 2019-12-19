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

#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

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

		class CustomProtocol : NSUrlProtocol
		{
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
			Exception ex = null;
			var done = new ManualResetEvent (false);
			var success = false;

			Task.Run (async () => {
				try {
					var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
					config.WeakProtocolClasses = NSArray.FromNSObjects (new Class (typeof (CustomUrlProtocol)));
					var session = NSUrlSession.FromConfiguration (config);
					var custom_url = new NSUrl ("foo://server");
					using (var task = await session.CreateDownloadTaskAsync (custom_url)) {
						success = true;
					}
				} catch (Exception e) {
					ex = e;
				} finally {
					done.Set ();
				}
			});

			Assert.IsTrue (TestRuntime.RunAsync (DateTime.Now.AddSeconds (10), () => { }, () => done.WaitOne (0)), "Timed out");
			Assert.IsNull (ex, "Exception");
			Assert.IsTrue (custom_url_protocol_instance.Called_DidCompleteWithError, "DidCompleteWithError");
			// if DidReceiveChallenge is called or not seems to vary between test runs, so we can't assert it.
			//Assert.IsFalse (custom_url_protocol_instance.Called_DidReceiveChallenge, "DidReceiveChallenge");
			Assert.IsTrue (custom_url_protocol_instance.Called_DidReceiveData, "DidReceiveData");
			Assert.IsTrue (custom_url_protocol_instance.Called_DidReceiveResponse, "DidReceiveResponse");
			Assert.IsTrue (custom_url_protocol_instance.Called_StartLoading, "StartLoading");
			Assert.IsTrue (custom_url_protocol_instance.Called_StopLoading, "StopLoading");
			Assert.IsFalse (custom_url_protocol_instance.Called_WillPerformHttpRedirection, "WillPerformHttpRedirection");

			Assert.IsTrue (CustomUrlProtocol.Called_CanInitWithRequest, "CanInitWithRequest");
			Assert.IsTrue (CustomUrlProtocol.Called_GetCanonicalRequest, "GetCanonicalRequest");

			Assert.IsTrue (success, "Success");
		}

		static CustomUrlProtocol custom_url_protocol_instance;

		public class CustomUrlProtocol : NSUrlProtocol, INSUrlSessionDelegate, INSUrlSessionTaskDelegate, INSUrlSessionDataDelegate {
			[Export ("canInitWithRequest:")]
			public static new bool CanInitWithRequest (NSUrlRequest request)
			{
				Called_CanInitWithRequest = true;
				return true;
			}
			public static bool Called_CanInitWithRequest;

			[Export ("canonicalRequestForRequest:")]
			public static new NSUrlRequest GetCanonicalRequest (NSUrlRequest request)
			{
				Called_GetCanonicalRequest = true;
				return request;
			}
			public static bool Called_GetCanonicalRequest;

			[Export ("initWithRequest:cachedResponse:client:")]
			public CustomUrlProtocol (NSUrlRequest request, NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client)
			: base (request, cachedResponse, client)
			{
				custom_url_protocol_instance = this;
			}

			[Export ("startLoading")]
			public override void StartLoading ()
			{
				Called_StartLoading = true;
				var config = NSUrlSession.SharedSession.Configuration;
				var session = NSUrlSession.FromConfiguration (config, this, new NSOperationQueue ());

				var task = session.CreateDataTask (new NSUrlRequest (new NSUrl ("https://example.com")));
				task.Resume ();
			}
			public bool Called_StartLoading;

			[Export ("stopLoading")]
			public override void StopLoading ()
			{
				Called_StopLoading = true;
			}
			public bool Called_StopLoading;

			//NSURLSessionTaskDelegate
			[Export ("URLSession:task:willPerformHTTPRedirection:newRequest:completionHandler:")]
			public virtual void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest> completionHandler)
			{
				Called_WillPerformHttpRedirection = true;
				completionHandler (newRequest);
			}
			public bool Called_WillPerformHttpRedirection;

			[Export ("URLSession:task:didReceiveChallenge:completionHandler:")]
			public virtual void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
			{
				Called_DidReceiveChallenge = true;
				completionHandler (NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, null);
			}
			public bool Called_DidReceiveChallenge;

			//NSURLSessionDataDelegate
			[Export ("URLSession:dataTask:didReceiveResponse:completionHandler:")]
			public virtual void DidReceiveResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
			{
				Called_DidReceiveResponse = true;
				completionHandler (NSUrlSessionResponseDisposition.Allow);
				this.Client.ReceivedResponse (this, response, NSUrlCacheStoragePolicy.Allowed);
			}
			public bool Called_DidReceiveResponse;

			[Export ("URLSession:dataTask:didReceiveData:")]
			public virtual void DidReceiveData (NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data)
			{
				Called_DidReceiveData = true;
				this.Client.DataLoaded (this, data);
			}
			public bool Called_DidReceiveData;

			[Export ("URLSession:task:didCompleteWithError:")]
			public virtual void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error)
			{
				Called_DidCompleteWithError = true;
				if (error != null) {
					this.Client.FailedWithError (this, error);
				} else {
					this.Client.FinishedLoading (this);
				}
			}
			public bool Called_DidCompleteWithError;
		}
#endif // !__WATCHOS__
	}
}
