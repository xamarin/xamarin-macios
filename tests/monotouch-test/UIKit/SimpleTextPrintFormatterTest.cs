//
// Unit tests for UISimpleTextPrintFormatter
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.IO;
using System.Threading;
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

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SimpleTextPrintFormatterTest {

		[Test]
		public void DefaultCtor ()
		{
			using (var stpf = new UISimpleTextPrintFormatter ()) {
				Assert.That (stpf.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				if (TestRuntime.CheckSystemAndSDKVersion (7,0)) {
					Assert.Null (stpf.Color, "Color");
					Assert.Null (stpf.Font, "Font");
					Assert.That (stpf.TextAlignment, Is.EqualTo (UITextAlignment.Natural), "TextAlignment");
				} else {
					Assert.That (stpf.Color, Is.EqualTo (UIColor.Black), "Color");
					Assert.NotNull (stpf.Font, "Font");
					Assert.That (stpf.TextAlignment, Is.EqualTo (UITextAlignment.Left), "TextAlignment");
				}
				Assert.That (stpf.Text, Is.Empty, "Text");
			}
		}

		[Test]
		public void StringCtor ()
		{
			using (var stpf = new UISimpleTextPrintFormatter ("Xamarin")) {
				if (TestRuntime.CheckSystemAndSDKVersion (7, 0)) {
					Assert.Null (stpf.Color, "Color");
					Assert.That (stpf.TextAlignment, Is.EqualTo (UITextAlignment.Natural), "TextAlignment");
				} else {
					Assert.That (stpf.Color, Is.EqualTo (UIColor.Black), "Color");
					Assert.That (stpf.TextAlignment, Is.EqualTo (UITextAlignment.Left), "TextAlignment");
				}
				Assert.NotNull (stpf.Font, "Font");
				Assert.That (stpf.Text, Is.EqualTo ("Xamarin"), "Text");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
