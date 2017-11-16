//
// Unit tests for MSMessage
//
// Authors:
//	Vincent Dondain <vincent@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using Messages;
#else
using MonoTouch.Foundation;
using MonoTouch.Messages;
#endif
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

#endif // !__TVOS__ && !__WATCHOS__
