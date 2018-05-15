//
// Unit tests for CTParagraphStyle
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using NUnit.Framework;
using System.Linq;

#if XAMCORE_2_0
using Foundation;
using CoreText;
#else
using MonoTouch.CoreText;
using MonoTouch.Foundation;
#endif


#if XAMCORE_2_0
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;
#else
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace MonoTouchFixtures.CoreText {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CTParagraphStyleTests {

		[Test]
		public void StylePropertiesTest ()
		{
			var settings = new CTParagraphStyleSettings () {
				TailIndent = 5,
				ParagraphSpacingBefore = 5,
				ParagraphSpacing = 5,
				LineSpacing = 5,
				MinimumLineHeight = 5,
				MaximumLineHeight = 5,
				LineHeightMultiple = 5,
				DefaultTabInterval = 5,
				HeadIndent = 5,
				FirstLineHeadIndent = 5,
				LineBreakMode = CTLineBreakMode.TruncatingHead,
				BaseWritingDirection = CTWritingDirection.Natural,
				Alignment = CTTextAlignment.Justified,
				TabStops = new [] {
					new CTTextTab (CTTextAlignment.Justified, 2),
					new CTTextTab (CTTextAlignment.Natural, 1)
				}
			};

			var style = new CTParagraphStyle (settings);
			Assert.DoesNotThrow (() => {
				Assert.AreEqual (settings.TailIndent, style.TailIndent, "TailIndent");
				Assert.AreEqual (settings.ParagraphSpacingBefore, style.ParagraphSpacingBefore, "ParagraphSpacingBefore");
				Assert.AreEqual (settings.ParagraphSpacing, style.ParagraphSpacing, "ParagraphSpacing");
				Assert.AreEqual (settings.LineSpacing, style.LineSpacing, "LineSpacing");
				Assert.AreEqual (settings.MinimumLineHeight, style.MinimumLineHeight, "MinimumLineHeight");
				Assert.AreEqual (settings.MaximumLineHeight, style.MaximumLineHeight, "MaximumLineHeight");
				Assert.AreEqual (settings.LineHeightMultiple, style.LineHeightMultiple, "LineHeightMultiple");
				Assert.AreEqual (settings.DefaultTabInterval, style.DefaultTabInterval, "DefaultTabInterval");
				Assert.AreEqual (settings.HeadIndent, style.HeadIndent, "HeadIndent");
				Assert.AreEqual (settings.FirstLineHeadIndent, style.FirstLineHeadIndent, "FirstLineHeadIndent");
				Assert.AreEqual (settings.LineBreakMode, style.LineBreakMode, "LineBreakMode");
				Assert.AreEqual (settings.BaseWritingDirection, style.BaseWritingDirection, "LineBreakMode");
				Assert.AreEqual (settings.Alignment, style.Alignment, "Alignment");

				var styleTabStops = style.GetTabStops ();
				Assert.AreEqual (settings.TabStops.Count (), styleTabStops.Length, "TabStops");
				Assert.True (styleTabStops.Any (t => t.Location == 2 && t.TextAlignment == CTTextAlignment.Justified));
				Assert.True (styleTabStops.Any (t => t.Location == 1 && t.TextAlignment == CTTextAlignment.Natural));
			});
		}
	}
}
