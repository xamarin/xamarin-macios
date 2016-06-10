//
// Unit tests for NSUrlSessionTask
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
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
	public class UrlSessionTaskTest {

		[Test]
		public void Properties ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Inconclusive ("requires iOS7");
			
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
				Assert.That ((string)task.TaskDescription, Is.EqualTo ("descriptive label"), "setTaskDescription:");
				Assert.That (task.TaskIdentifier, Is.GreaterThanOrEqualTo (0), "taskIdentifier");
			}
		}
	}
}