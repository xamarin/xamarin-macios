//
// Unit tests for NSUrlSessionTask
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
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
	public class UrlSessionTaskTest {

		[Test]
		public void Properties ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			using (var ur = new NSUrlRequest ())
			using (var task = NSUrlSession.SharedSession.CreateDownloadTask (ur)) {
				// in iOS9 those selectors do not respond - but they do work (forwarded to __NSCFLocalDownloadTask type ?)
				// * countOfBytesExpectedToReceive, countOfBytesExpectedToSend, countOfBytesReceived, countOfBytesSent,
				// currentRequest, originalRequest, response, state, taskDescription, setTaskDescription:, taskIdentifier
				Assert.That (task.BytesExpectedToReceive, Is.EqualTo (0), "countOfBytesExpectedToReceive");
				Assert.That (task.BytesExpectedToSend, Is.EqualTo (0), "countOfBytesExpectedToSend");
				Assert.That (task.BytesReceived, Is.EqualTo (0), "countOfBytesReceived");
				Assert.That (task.BytesSent, Is.EqualTo (0), "countOfBytesSent");
				Assert.NotNull (task.CurrentRequest, "currentRequest");
				Assert.Null (task.Error, "error");
				Assert.NotNull (task.OriginalRequest, "originalRequest");
				Assert.Null (task.Response, "response");
				Assert.That (task.State, Is.EqualTo (NSUrlSessionTaskState.Suspended), "state");
				Assert.Null (task.TaskDescription, "taskDescription");
				task.TaskDescription = "descriptive label";
				Assert.That ((string) task.TaskDescription, Is.EqualTo ("descriptive label"), "setTaskDescription:");
				Assert.That (task.TaskIdentifier, Is.GreaterThanOrEqualTo ((nuint) 0), "taskIdentifier");

				if (TestRuntime.CheckXcodeVersion (9, 0)) {
					Assert.Null (task.EarliestBeginDate, "earliestBeginDate");
					Assert.That (task.CountOfBytesClientExpectsToSend, Is.EqualTo (-1), "countOfBytesClientExpectsToSend");
					Assert.That (task.CountOfBytesClientExpectsToReceive, Is.EqualTo (-1), "countOfBytesClientExpectsToReceive");
					Assert.NotNull (task.Progress, "progress");
				}
			}
		}

		[Test]
		public void NSUrlSessionDownloadTaskTest ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);

			using (var ur = new NSUrlRequest ()) {
				NSUrlSessionDownloadTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateDownloadTask (ur), "Should not throw InvalidCastException");
				Assert.IsNotNull (task, "task should not be null");
				Assert.IsInstanceOf (typeof (NSUrlSessionDownloadTask), task, "task should be an instance of NSUrlSessionDownloadTask");
			}

			using (var ur = new NSUrlRequest ()) {
				NSUrlSessionDownloadTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateDownloadTask (ur, null), "Should not throw InvalidCastException 2");
				Assert.IsNotNull (task, "task should not be null 2");
				Assert.IsInstanceOf (typeof (NSUrlSessionDownloadTask), task, "task should be an instance of NSUrlSessionDownloadTask 2");
			}

			using (var ur = new NSUrl (NetworkResources.MicrosoftUrl)) {
				NSUrlSessionDownloadTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateDownloadTask (ur), "Should not throw InvalidCastException 3");
				Assert.IsNotNull (task, "task should not be null 3");
				Assert.IsInstanceOf (typeof (NSUrlSessionDownloadTask), task, "task should be an instance of NSUrlSessionDownloadTask 3");
			}

			using (var ur = new NSUrl (NetworkResources.MicrosoftUrl)) {
				NSUrlSessionDownloadTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateDownloadTask (ur, null), "Should not throw InvalidCastException 4");
				Assert.IsNotNull (task, "task should not be null 4");
				Assert.IsInstanceOf (typeof (NSUrlSessionDownloadTask), task, "task should be an instance of NSUrlSessionDownloadTask 4");
			}
		}

		[Test]
		public void NSUrlSessionDataTaskTest ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);

			using (var ur = new NSUrlRequest ()) {
				NSUrlSessionDataTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateDataTask (ur), "Should not throw InvalidCastException");
				Assert.IsNotNull (task, "task should not be null");
				Assert.IsInstanceOf (typeof (NSUrlSessionDataTask), task, "task should be an instance of NSUrlSessionDataTask");
			}

			using (var ur = new NSUrlRequest ()) {
				NSUrlSessionDataTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateDataTask (ur, null), "Should not throw InvalidCastException 2");
				Assert.IsNotNull (task, "task should not be null 2");
				Assert.IsInstanceOf (typeof (NSUrlSessionDataTask), task, "task should be an instance of NSUrlSessionDataTask 2");
			}

			using (var ur = new NSUrl (NetworkResources.MicrosoftUrl)) {
				NSUrlSessionDataTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateDataTask (ur), "Should not throw InvalidCastException 3");
				Assert.IsNotNull (task, "task should not be null 3");
				Assert.IsInstanceOf (typeof (NSUrlSessionDataTask), task, "task should be an instance of NSUrlSessionDataTask 3");
			}

			using (var ur = new NSUrl (NetworkResources.MicrosoftUrl)) {
				NSUrlSessionDataTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateDataTask (ur, null), "Should not throw InvalidCastException 4");
				Assert.IsNotNull (task, "task should not be null 4");
				Assert.IsInstanceOf (typeof (NSUrlSessionDataTask), task, "task should be an instance of NSUrlSessionDataTask 4");
			}
		}

		[Test]
		public void NSUrlSessionUploadTaskTest ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);

			using (var ur = new NSUrlRequest ()) {
				NSUrlSessionUploadTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateUploadTask (ur), "Should not throw InvalidCastException");
				Assert.IsNotNull (task, "task should not be null");
				Assert.IsInstanceOf (typeof (NSUrlSessionUploadTask), task, "task should be an instance of NSUrlSessionUploadTask");
			}

			using (var data = NSData.FromString ("Hola"))
			using (var ur = new NSUrlRequest ()) {
				NSUrlSessionUploadTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateUploadTask (ur, data), "Should not throw InvalidCastException 2");
				Assert.IsNotNull (task, "task should not be null 2");
				Assert.IsInstanceOf (typeof (NSUrlSessionUploadTask), task, "task should be an instance of NSUrlSessionUploadTask 2");
			}

			using (var ur = new NSUrlRequest ())
			using (var url = new NSUrl (NetworkResources.MicrosoftUrl)) {
				NSUrlSessionUploadTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateUploadTask (ur, url), "Should not throw InvalidCastException 3");
				Assert.IsNotNull (task, "task should not be null 3");
				Assert.IsInstanceOf (typeof (NSUrlSessionUploadTask), task, "task should be an instance of NSUrlSessionUploadTask 3");
			}

			using (var ur = new NSUrlRequest ())
			using (var url = new NSUrl (NetworkResources.MicrosoftUrl)) {
				NSUrlSessionUploadTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateUploadTask (ur, url, (data, response, error) => { }), "Should not throw InvalidCastException 4");
				Assert.IsNotNull (task, "task should not be null 4");
				Assert.IsInstanceOf (typeof (NSUrlSessionUploadTask), task, "task should be an instance of NSUrlSessionUploadTask 4");
			}

			using (var ur = new NSUrlRequest ())
			using (var data = NSData.FromString ("Hola")) {
				NSUrlSessionUploadTask task = null;
				Assert.DoesNotThrow (() => task = NSUrlSession.SharedSession.CreateUploadTask (ur, data, (d, response, error) => { }), "Should not throw InvalidCastException 5");
				Assert.IsNotNull (task, "task should not be null 5");
				Assert.IsInstanceOf (typeof (NSUrlSessionUploadTask), task, "task should be an instance of NSUrlSessionUploadTask 5");
			}
		}
	}
}
