//
// UIStringAttributes Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#if !MONOMAC
using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
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
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class StringAttributesTest {

		[Test]
		public void RetainCount ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			var sa = new UIStringAttributes ();

			var bc = UIColor.FromRGBA (0.1f, 0.2f, 0.3f, 0.4f);
			Assert.That (bc.RetainCount, Is.EqualTo ((nint) 2), "BackgroundColor-new"); // bug
			sa.BackgroundColor = bc;
			Assert.That (bc.RetainCount, Is.EqualTo ((nint) 3), "BackgroundColor-set");

			sa.BaselineOffset = 0.0f;

			var fc = UIColor.FromRGBA (0.5f, 0.6f, 0.7f, 0.8f);
			Assert.That (fc.RetainCount, Is.EqualTo ((nint) 2), "ForegroundColor-new"); // bug
			sa.ForegroundColor = fc;
			Assert.That (fc.RetainCount, Is.EqualTo ((nint) 3), "ForegroundColor-set");

			var f = UIFont.FromName ("Helvetica", 12);
			var f_count = f.RetainCount; // lots of owner
			sa.Font = f;
			Assert.That (f.RetainCount, Is.EqualTo (++f_count), "Font-set");

			var ps = new NSParagraphStyle ();
			Assert.That (ps.RetainCount, Is.EqualTo ((nint) 1), "ParagraphStyle-new");
			sa.ParagraphStyle = ps;
			Assert.That (ps.RetainCount, Is.EqualTo ((nint) 2), "ParagraphStyle-set");

			for (int i=0; i < 16; i++) {
				Assert.NotNull (sa.BackgroundColor, "BackgroundColor-get");
				Assert.NotNull (sa.ForegroundColor, "ForegroundColor-get");
				Assert.NotNull (sa.Font, "Font-get");
				Assert.NotNull (sa.ParagraphStyle, "ParagraphStyle-get");
			}

			Assert.That (sa.BackgroundColor.RetainCount, Is.EqualTo ((nint) 3), "BackgroundColor");
			Assert.That (sa.ForegroundColor.RetainCount, Is.EqualTo ((nint) 3), "ForegroundColor");
			Assert.That (sa.Font.RetainCount, Is.EqualTo (f_count), "Font");
			Assert.That (sa.ParagraphStyle.RetainCount, Is.EqualTo ((nint) 2), "ParagraphStyle");

			GC.KeepAlive (bc);
			GC.KeepAlive (fc);
			GC.KeepAlive (f);
			GC.KeepAlive (ps);
		}

		[Test]
		public void RetainCount_7 ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			var sa = new UIStringAttributes ();
			sa.Expansion = 0.0f;

			var uc = UIColor.FromRGBA (0.1f, 0.2f, 0.3f, 0.4f);
			Assert.That (uc.RetainCount, Is.EqualTo ((nint) 2), "UnderlineColor-new");
			sa.UnderlineColor = uc;
			Assert.That (uc.RetainCount, Is.EqualTo ((nint) 3), "UnderlineColor-set");

			var sc = UIColor.FromRGBA (0.5f, 0.6f, 0.7f, 0.8f);
			Assert.That (sc.RetainCount, Is.EqualTo ((nint) 2), "StrikethroughColor-new");
			sa.StrikethroughColor = sc;
			Assert.That (sc.RetainCount, Is.EqualTo ((nint) 3), "StrikethroughColor-set");

			var u = new NSUrl ("http://xamarin.com");
			Assert.That (u.RetainCount, Is.EqualTo ((nint) 1), "Link-new");
			sa.Link = u;
			Assert.That (u.RetainCount, Is.EqualTo ((nint) 2), "Link-set");

#if !__WATCHOS__
			var ta = new NSTextAttachment ();
			Assert.That (ta.RetainCount, Is.EqualTo ((nint) 1), "TextAttachment-new");
			sa.TextAttachment = ta;
			Assert.That (ta.RetainCount, Is.EqualTo ((nint) 2), "TextAttachment-set");
#endif // !__WATCHOS__

			for (int i=0; i < 16; i++) {
				Assert.NotNull (sa.UnderlineColor, "UnderlineColor-get");
				Assert.NotNull (sa.StrikethroughColor, "StrikethroughColor-get");
				Assert.NotNull (sa.Link, "Link-get");
#if !__WATCHOS__
				Assert.NotNull (sa.TextAttachment, "TextAttachment-get");
#endif
			}

			Assert.That (sa.UnderlineColor.RetainCount, Is.EqualTo ((nint) 3), "UnderlineColor");
			Assert.That (sa.StrikethroughColor.RetainCount, Is.EqualTo ((nint) 3), "StrikethroughColor");
			Assert.That (sa.Link.RetainCount, Is.EqualTo ((nint) 2), "Link");
#if !__WATCHOS__
			Assert.That (sa.TextAttachment.RetainCount, Is.EqualTo ((nint) 2), "TextAttachment");
#endif

			GC.KeepAlive (uc);
			GC.KeepAlive (sc);
			GC.KeepAlive (u);
#if !__WATCHOS__
			GC.KeepAlive (ta);
#endif
		}

#if !__WATCHOS__
		[Test]
		public void MutableStringAttributesTest ()
		{
			// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=28158
			// issue: Properties of type UIStringAttributes produce immutable objects that crash when you try to modify them
			// This test proves that the bug is fixed

			using (var nb = new UINavigationBar ()) {
				Assert.Null (nb.TitleTextAttributes, "TitleTextAttributes should be null");
				nb.TitleTextAttributes = new UIStringAttributes { ForegroundColor = UIColor.Green };
				Assert.AreSame (UIColor.Green, nb.TitleTextAttributes.ForegroundColor, "TitleTextAttributes.ForegroundColor should match");

				var titleAttribtues = nb.TitleTextAttributes; // we now get a mutable dictionary for this DictionaryContainer
				titleAttribtues.ForegroundColor = UIColor.Red; // this used to throw unrecognized selector before fixing bug 28158
				nb.TitleTextAttributes = titleAttribtues;
				Assert.AreSame (UIColor.Red, nb.TitleTextAttributes.ForegroundColor, "TitleTextAttributes.ForegroundColor should match");
			}
		}
#endif // !__WATCHOS__
	}
}
#endif