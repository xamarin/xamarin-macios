#if __IOS__ && !__MACCATALYST__

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UITextFormattingViewControllerFormattingDescriptorTest {

		[Test]
		public void SetProperties ()
		{
			using var obj = new UITextFormattingViewControllerFormattingDescriptor ();

			Assert.Multiple (() => {
				var textAlignments = UITextFormattingViewControllerTextAlignment.Left | UITextFormattingViewControllerTextAlignment.Center;
				obj.TextAlignments = textAlignments;
				var weakTestAlignments = ((IEnumerable<NSString>) obj.WeakTextAlignments).Select (v => v.ToString ()).ToArray ();
				Assert.AreEqual (2, weakTestAlignments.Length, "WeakTextAlignments.Length");
				Assert.That (weakTestAlignments, Does.Contain ((string) UITextFormattingViewControllerTextAlignment.Left.GetConstant ()), "WeakTextAlignments #1");
				Assert.That (weakTestAlignments, Does.Contain ((string) UITextFormattingViewControllerTextAlignment.Center.GetConstant ()), "WeakTextAlignments #2");
				Assert.AreEqual (textAlignments, obj.TextAlignments, "TextAlignments");

				var textLists = UITextFormattingViewControllerTextList.Hyphen | UITextFormattingViewControllerTextList.Decimal;
				obj.TextLists = textLists;
				var weakTextLists = ((IEnumerable<NSString>) obj.WeakTextLists).Select (v => v.ToString ()).ToArray ();
				Assert.AreEqual (2, weakTextLists.Length, "WeakTextLists.Length");
				Assert.That (weakTextLists, Does.Contain ((string) UITextFormattingViewControllerTextList.Hyphen.GetConstant ()), "WeakTextLists #1");
				Assert.That (weakTextLists, Does.Contain ((string) UITextFormattingViewControllerTextList.Decimal.GetConstant ()), "WeakTextLists #2");
				Assert.AreEqual (textLists, obj.TextLists, "TextLists");

				var highlights = UITextFormattingViewControllerHighlight.Purple | UITextFormattingViewControllerHighlight.Pink;
				obj.Highlights = highlights;
				var weakHighlights = ((IEnumerable<NSString>) obj.WeakHighlights).Select (v => v.ToString ()).ToArray ();
				Assert.AreEqual (2, weakHighlights.Length, "WeakHighlights.Length");
				Assert.That (weakHighlights, Does.Contain ((string) UITextFormattingViewControllerHighlight.Purple.GetConstant ()), "WeakHighlights #1");
				Assert.That (weakHighlights, Does.Contain ((string) UITextFormattingViewControllerHighlight.Pink.GetConstant ()), "WeakHighlights #2");
				Assert.AreEqual (highlights, obj.Highlights, "Highlights");
			});
		}

		[Test]
		public void Convert_Null ()
		{
			using (UIWindow w = new UIWindow ()) {
				Assert.That (w.ConvertPointFromWindow (CGPoint.Empty, null), Is.EqualTo (CGPoint.Empty), "ConvertPointFromWindow");
				Assert.That (w.ConvertPointToWindow (CGPoint.Empty, null), Is.EqualTo (CGPoint.Empty), "ConvertPointToWindow");
				Assert.That (w.ConvertRectFromWindow (CGRect.Empty, null), Is.EqualTo (CGRect.Empty), "ConvertRectFromWindow");
				Assert.That (w.ConvertRectToWindow (CGRect.Empty, null), Is.EqualTo (CGRect.Empty), "ConvertRectToWindow");
			}
		}

		[Test]
		public void IsKeyWindow_5199 ()
		{
			using (UIWindow w = new UIWindow ()) {
				Assert.False (w.IsKeyWindow, "IsKeyWindow");
			}
		}

		[Test]
		public void Level ()
		{
			using (UIWindow w = new UIWindow ()) {
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 0f), "default");
				w.WindowLevel = UIWindowLevel.Normal;
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 0f), "Normal");
				w.WindowLevel = UIWindowLevel.Alert;
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 2000f), "Alert");
#if !__TVOS__
				w.WindowLevel = UIWindowLevel.StatusBar;
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 1000f), "StatusBar");
#endif // !__TVOS__
			}
		}
	}
}

#endif // __IOS__ && !__MACCATALYST__
