//
// Unit tests for MSMessage
//
// Authors:
//	Vincent Dondain <vincent@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if HAS_MESSAGE

using System;
using Foundation;
using Messages;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Messages
{

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MSMessageTest
	{
		[SetUp]
		public void MinimumSdkCheck ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
		}

		[Test]
		public void InitWithSession ()
		{
			var session = new MSSession ();
			using (var msg = new MSMessage (session)) {
				Assert.That (msg.Session, Is.EqualTo (session), "Session");
				Assert.That (msg.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}
	}
}

#endif // HAS_MESSAGE
