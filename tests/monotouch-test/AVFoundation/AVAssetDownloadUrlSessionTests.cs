//
// Unit tests for AVAssetImageGenerator
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !__TVOS__ && !MONOMAC

using System;
using ObjCRuntime;

using Foundation;
using AVFoundation;
using NUnit.Framework;

namespace monotouchtest {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVAssetDownloadUrlSessionTests {

		[Test]
		public void AVAssetDownloadUrlSessionStaticNotSupported ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			Assert.Throws<NotSupportedException> (() => { var x = AVAssetDownloadUrlSession.SharedSession; }, "SharedSession should throw NotSupportedException");
			Assert.Throws<NotSupportedException> (() => AVAssetDownloadUrlSession.FromConfiguration (NSUrlSessionConfiguration.DefaultSessionConfiguration), "FromConfiguration should throw NotSupportedException");
			Assert.Throws<NotSupportedException> (() => AVAssetDownloadUrlSession.FromConfiguration (NSUrlSessionConfiguration.DefaultSessionConfiguration, new NSUrlSessionDelegate (), null), "FromConfiguration should throw NotSupportedException");
			Assert.Throws<NotSupportedException> (() => AVAssetDownloadUrlSession.FromWeakConfiguration (NSUrlSessionConfiguration.DefaultSessionConfiguration, new NSObject (), null), "FromWeakConfiguration should throw NotSupportedException");
		}

		[Test]
		public void CreateSessionTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Ignoring AVAssetDownloadUrlSession tests: Requires iOS9+");

			TestRuntime.AssertNotDevice ("Ignoring CreateSessionTest tests: Requires com.apple.developer.media-asset-download entitlement");

			using (var backgroundConfiguration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration ("HLS-Identifier")) {
				Assert.DoesNotThrow (() => AVAssetDownloadUrlSession.CreateSession (backgroundConfiguration, null, NSOperationQueue.MainQueue), "Should not throw InvalidCastException");
			}
		}

		// FIXME: Disabling this test from now, will reenable once apple releases docs on what is ecpected to have this entitlement key
		// Reason: Creating an AVAssetDownloadURLSession requires the com.apple.developer.media-asset-download entitlement
		//		[Test]
		//		public void AVAssetDownloadUrlSessionNotSupported ()
		//		{
		//			if (!TestRuntime.CheckSystemAndSDKVersion (9, 0))
		//				Assert.Ignore ("Ignoring AVAssetDownloadUrlSession tests: Requires iOS9+");
		//
		//			var session = AVAssetDownloadUrlSession.CreateSession (NSUrlSessionConfiguration.BackgroundSessionConfiguration ("XamFoo"), null, null);
		//
		//			Assert.Throws <NotSupportedException> (() => session.CreateDataTask (new NSUrlRequest ()), "CreateDataTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateDataTask (new NSUrl (NetworkResources.MicrosoftUrl)), "CreateDataTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateUploadTask (new NSUrlRequest (), new NSUrl (NetworkResources.MicrosoftUrl)), "CreateUploadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateUploadTask (new NSUrlRequest (), new NSData ()), "CreateUploadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateUploadTask (new NSUrlRequest ()), "CreateUploadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateDownloadTask (new NSUrlRequest ()), "CreateDownloadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateDownloadTask (new NSUrl (NetworkResources.MicrosoftUrl)), "CreateDownloadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateDownloadTask (new NSData ()), "CreateDownloadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateDataTask (new NSUrlRequest (), (data, response, error) => {}), "CreateDataTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateDataTask (new NSUrl (NetworkResources.MicrosoftUrl), (data, response, error) => {}), "CreateDataTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateUploadTask (new NSUrlRequest (), new NSUrl (NetworkResources.MicrosoftUrl), (data, response, error) => {}), "CreateUploadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateUploadTask (new NSUrlRequest (), new NSData (), (data, response, error) => {}), "CreateUploadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateDownloadTask (new NSUrlRequest (), (NSUrl location, NSUrlResponse response, NSError error) => {}), "CreateDownloadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateDownloadTask (new NSUrl (NetworkResources.MicrosoftUrl), (NSUrl location, NSUrlResponse response, NSError error) => {}), "CreateDownloadTask should throw NotSupportedException");
		//			Assert.Throws <NotSupportedException> (() => session.CreateDownloadTaskFromResumeData (new NSData (), (NSUrl location, NSUrlResponse response, NSError error) => {}), "CreateDownloadTask should throw NotSupportedException");
		//
		//		}
	}
}

#endif // !__WATCHOS__
