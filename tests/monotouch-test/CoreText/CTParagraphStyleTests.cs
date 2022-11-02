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

using Foundation;
using CoreText;
using ObjCRuntime;

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
				Assert.AreEqual (settings.TailIndent, (nfloat) style.TailIndent, "TailIndent");
				Assert.AreEqual (settings.ParagraphSpacingBefore, (nfloat) style.ParagraphSpacingBefore, "ParagraphSpacingBefore");
				Assert.AreEqual (settings.ParagraphSpacing, (nfloat) style.ParagraphSpacing, "ParagraphSpacing");
				Assert.AreEqual (settings.LineSpacing, (nfloat) style.LineSpacing, "LineSpacing");
				Assert.AreEqual (settings.MinimumLineHeight, (nfloat) style.MinimumLineHeight, "MinimumLineHeight");
				Assert.AreEqual (settings.MaximumLineHeight, (nfloat) style.MaximumLineHeight, "MaximumLineHeight");
				Assert.AreEqual (settings.LineHeightMultiple, (nfloat) style.LineHeightMultiple, "LineHeightMultiple");
				Assert.AreEqual (settings.DefaultTabInterval, (nfloat) style.DefaultTabInterval, "DefaultTabInterval");
				Assert.AreEqual (settings.HeadIndent, (nfloat) style.HeadIndent, "HeadIndent");
				Assert.AreEqual (settings.FirstLineHeadIndent, (nfloat) style.FirstLineHeadIndent, "FirstLineHeadIndent");
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
