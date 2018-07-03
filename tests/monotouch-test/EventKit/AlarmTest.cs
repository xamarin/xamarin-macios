//
// Unit tests for EKAlarm
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using System;
#if XAMCORE_2_0
using Foundation;
using CoreGraphics;
using EventKit;
using ObjCRuntime;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.EventKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AlarmTest {
		
		[Test]
		public void NullAllowedTest ()
		{
			TestRuntime.AssertMacSystemVersion (10, 8, throwIfOtherPlatform: false);

			using (var alarm = EKAlarm.FromTimeInterval (1234)) {
				alarm.AbsoluteDate = null;
				alarm.StructuredLocation = null;
			}
		}
	}
}

#endif // !__TVOS__
