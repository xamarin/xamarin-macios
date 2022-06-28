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
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SimpleTextPrintFormatterTest {

#if !XAMCORE_3_0 // The default ctor is not available in XAMCORE_3_0+
		[Test]
		public void DefaultCtor ()
		{
			using (var stpf = new UISimpleTextPrintFormatter ()) {
				Assert.That (stpf.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				if (TestRuntime.CheckXcodeVersion (11, 0)) {
					Assert.NotNull (stpf.Color, "Color");
					Assert.Null (stpf.Font, "Font");
					Assert.That (stpf.TextAlignment, Is.EqualTo (UITextAlignment.Natural), "TextAlignment");
				} else if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false)) {
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
#endif // !XAMCORE_3_0

		[Test]
		public void StringCtor ()
		{
			using (var stpf = new UISimpleTextPrintFormatter ("Xamarin")) {
				if (TestRuntime.CheckXcodeVersion (11, 0)) {
					Assert.NotNull (stpf.Color, "Color");
					Assert.That (stpf.TextAlignment, Is.EqualTo (UITextAlignment.Natural), "TextAlignment");
				} else if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false)) {
					Assert.Null (stpf.Color, "Color");
					Assert.That (stpf.TextAlignment, Is.EqualTo (UITextAlignment.Natural), "TextAlignment");
				} else {
					Assert.That (stpf.Color, Is.EqualTo (UIColor.Black), "Color");
					Assert.That (stpf.TextAlignment, Is.EqualTo (UITextAlignment.Left), "TextAlignment");
				}
				if (TestRuntime.CheckXcodeVersion (14, 0)) {
					Assert.Null (stpf.Font, "Font");
				} else {
					Assert.NotNull (stpf.Font, "Font");
				}
				Assert.That (stpf.Text, Is.EqualTo ("Xamarin"), "Text");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
