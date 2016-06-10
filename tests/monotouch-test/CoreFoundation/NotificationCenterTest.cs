//
// Unit tests for CFNotificationCenter
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NotificationCenterTest {

		[Test]
		public void Static ()
		{
			Assert.NotNull (CFNotificationCenter.Darwin, "Darwin");
			Assert.NotNull (CFNotificationCenter.Local, "Local");
		}
	}
}