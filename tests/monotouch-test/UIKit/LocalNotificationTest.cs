//
// Unit tests for UILocalNotification
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LocalNotificationTest {

		[Test]
		public void DefaultValues ()
		{
			using (var def = new UILocalNotification ()) {
				Assert.IsNull (def.FireDate, "FireDate");
				Assert.IsNull (def.TimeZone, "TimeZone");
				Assert.That ((nuint) (ulong) def.RepeatInterval, Is.EqualTo ((nuint) 0), "RepeatInterval"); // documented to be 0, which is not in the enum.
				Assert.IsNull (def.RepeatCalendar, "RepeatCalendar");
				Assert.IsNull (def.AlertBody, "AlertBody");
				Assert.IsTrue (def.HasAction, "HasAction");
				Assert.IsNull (def.AlertAction, "AlertAction");
				Assert.IsNull (def.AlertLaunchImage, "AlertLaunchImage");
				Assert.IsNull (def.SoundName, "SoundName");
				Assert.That (def.ApplicationIconBadgeNumber, Is.EqualTo ((nint)0), "ApplicationIconBadgeNumber");
				Assert.IsNull (def.UserInfo, "UserInfo");
			}
		}

		[Test]
		public void NullValues ()
		{
			using (var def = new UILocalNotification ()) {
				def.FireDate = null;
				def.FireDate = new NSDate ();
				Assert.IsNotNull (def.FireDate, "FireDate NN");
				def.FireDate = null;
				Assert.IsNull (def.FireDate, "FireDate N");

				def.TimeZone = null;
				def.TimeZone = new NSTimeZone ("GMT");
				Assert.IsNotNull (def.TimeZone, "TimeZone NN");
				def.TimeZone = null;
				Assert.IsNull (def.TimeZone, "TimeZone N");

				def.RepeatInterval = NSCalendarUnit.Calendar;
				Assert.That (def.RepeatInterval, Is.EqualTo (NSCalendarUnit.Calendar), "RepeatInterval 1");
				def.RepeatInterval = (NSCalendarUnit)0;
				Assert.That (def.RepeatInterval, Is.EqualTo ((NSCalendarUnit)0), "RepeatInterval 2");

				def.RepeatCalendar = null;
				def.RepeatCalendar = new NSCalendar (NSCalendarType.Hebrew);
				Assert.IsNotNull (def.RepeatCalendar, "RepeatCalendar NN");
				def.RepeatCalendar = null;
				Assert.IsNull (def.RepeatCalendar, "RepeatCalendar N");

				def.AlertBody = null;
				def.AlertBody = "body";
				Assert.AreEqual ("body", def.AlertBody, "AlertBody NN");
				def.AlertBody = null;
				Assert.IsNull (def.AlertBody, "AlertBody N");

				def.AlertAction = null;
				def.AlertAction = "action";
				Assert.AreEqual ("action", def.AlertAction, "AlertAction NN");
				def.AlertAction = null;
				Assert.IsNull (def.AlertAction, "AlertAction N");

				def.AlertLaunchImage = null;
				def.AlertLaunchImage = "image";
				Assert.AreEqual ("image", def.AlertLaunchImage, "AlertLaunchImage NN");
				def.AlertLaunchImage = null;
				Assert.IsNull (def.AlertLaunchImage, "AlertLaunchImage N");

				def.SoundName = null;
				def.SoundName = "sound";
				Assert.AreEqual ("sound", def.SoundName, "SoundName NN");
				def.SoundName = null;
				Assert.IsNull (def.SoundName, "SoundName N");

				def.UserInfo = null;
				def.UserInfo = new NSDictionary ();
				Assert.IsNotNull (def.UserInfo, "UserInfo NN");
				def.UserInfo = null;
				Assert.IsNull (def.UserInfo, "UserInfo N");
			}
		}
	}
}

#endif // !__TVOS__
