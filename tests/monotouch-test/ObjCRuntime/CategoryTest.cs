using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using Foundation;
#if !MONOMAC
using UIKit;
#if NET
using NativeException = ObjCRuntime.ObjCException;
#else
using NativeException = Foundation.MonoTouchException;
#endif
#endif
using Bindings.Test;
using ObjCRuntime;
#if !__TVOS__
using MapKit;
#endif
#if !__WATCHOS__
using CoreAnimation;
#endif
using CoreGraphics;
using CoreLocation;
using PlatformException = ObjCRuntime.RuntimeException;
using NUnit.Framework;

using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;
using CatAttrib = ObjCRuntime.CategoryAttribute;

namespace MonoTouchFixtures {
	[CatAttrib (typeof (NSString))]
	[Preserve (AllMembers = true)]
	public static class MyStringCategory {
		[Export ("toUpper")]
		static string ToUpper ()
		{
			return "TOUPPER";
		}

		[Export ("toUpper:")]
		static string ToUpper (string str)
		{
			return str.ToUpper ();
		}

		[Export ("toLower")]
		static string ToLower (this NSString str)
		{
			return ((string) str).ToLower ();
		}

		[Export ("joinLower:")]
		static string JoinLower (this NSString str, string str2)
		{
			return (((string) str) + str2).ToLower ();
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CategoryTest {
		[Test]
		public void Static ()
		{
			using (var str = Runtime.GetNSObject (Messaging.IntPtr_objc_msgSend (new NSString ("").ClassHandle, Selector.GetHandle ("toUpper")))) {
				Assert.AreEqual ("TOUPPER", str.ToString (), "#1");
			}

			using (var istr = (NSString) "test-string") {
				using (var str = Runtime.GetNSObject (Messaging.IntPtr_objc_msgSend_IntPtr (new NSString ("").ClassHandle, Selector.GetHandle ("toUpper:"), istr.Handle))) {
					Assert.AreEqual ("TEST-STRING", str.ToString (), "#2");
				}
			}
		}

		[Test]
		public void Instance ()
		{
			using (var istr = (NSString) "SoMeUpPeRcAsElEtTeRs") {
				using (var str = Runtime.GetNSObject (Messaging.IntPtr_objc_msgSend (istr.Handle, Selector.GetHandle ("toLower")))) {
					Assert.AreEqual ("someuppercaseletters", str.ToString (), "#1");
				}
			}
			using (var istr = (NSString) "fIrSt") {
				using (var istr2 = (NSString) "secOND") {
					using (var str = Runtime.GetNSObject (Messaging.IntPtr_objc_msgSend_IntPtr (istr.Handle, Selector.GetHandle ("joinLower:"), istr2.Handle))) {
						Assert.AreEqual ("firstsecond", str.ToString (), "#2");
					}
				}
			}
		}

#if !__WATCHOS__ && !MONOMAC
		[Test]
		public void NavigationControllerOverride ()
		{
			TestRuntime.IgnoreOnTVOS (); // shouldAutorotate is not available on TVOS.
			TestRuntime.IgnoreOnMacCatalyst (); // rotation is not available on Mac Catalyst

			try {
				bool category_invoked = false;
				var vc = new UIViewController ();
				vc.View.BackgroundColor = UIColor.Yellow;
				var nc = new UINavigationController (vc);
				Rotation_IOS6.ShouldAutoRotateCallback = () => {
					category_invoked = true;
					vc.View.BackgroundColor = UIColor.Green;
				};
				AppDelegate.PresentModalViewController (nc, 0.5);
				if (TestRuntime.CheckXcodeVersion (14, 0)) {
					// The 'shouldAutorotate' selector isn't called anymore in iOS 16+ (this is documented in Xcode 14's release notes)
					Assert.That (!category_invoked);
				} else {
					Assert.That (category_invoked);
				}
			} finally {
				Rotation_IOS6.ShouldAutoRotateCallback = null;
			}
		}
#endif // !__WATCHOS__
	}

#if !__WATCHOS__ && !MONOMAC
	[CatAttrib (typeof (UINavigationController))]
	[Preserve (AllMembers = true)]
	static class Rotation_IOS6 {
		public static Action ShouldAutoRotateCallback;
		[Export ("shouldAutorotate")]
		static bool ShouldAutoRotate (this UINavigationController self)
		{
			if (ShouldAutoRotateCallback is not null)
				ShouldAutoRotateCallback ();
			return true;
		}
	}
#endif // !__WATCHOS__
}
