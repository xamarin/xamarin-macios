//
// Unit tests for NSUserDefaults
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
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

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.Foundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UserDefaultsTest {

		[Test]
		public void SetString ()
		{
			// confusing API for .NET developers since the parameters are 'value', 'key'
			// http://stackoverflow.com/q/12415054/220643
			NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
			defaults.RemoveObject ("spid");
			Assert.Null (defaults.StringForKey ("spid"), "StringForKey-1");
			defaults.SetString ("coucou", "spid");
			defaults.Synchronize ();
			Assert.That (defaults.StringForKey ("spid"), Is.EqualTo ("coucou"), "StringForKey-2");
		}

		[Test]
		public void Ctor_UserName ()
		{
			// initWithUser:
			using (var ud = new NSUserDefaults ("username")) {
				Assert.That (ud.RetainCount, Is.EqualTo ((nint) 1), "RetainCount");
				ud.SetString ("value", "key");
				ud.Synchronize ();
			}

			using (var ud = new NSUserDefaults ("username", NSUserDefaultsType.UserName)) {
				Assert.That (ud.RetainCount, Is.EqualTo ((nint) 1), "RetainCount");
				Assert.That (ud ["key"].ToString (), Is.EqualTo ("value"), "[key]-1");
				ud.RemoveObject ("key");
				ud.Synchronize ();
				Assert.Null (ud ["key"], "[key]-2");
			}
		}

		[Test]
		public void Ctor_SuiteName ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);

			// initWithSuiteName:
			using (var ud = new NSUserDefaults ("suitename", NSUserDefaultsType.SuiteName)) {
				Assert.That (ud.RetainCount, Is.EqualTo ((nint) 1), "RetainCount");
			}
		}
	}
}
