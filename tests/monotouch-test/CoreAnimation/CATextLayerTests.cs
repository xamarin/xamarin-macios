//
// Unit tests for CATextLayerTests
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using CoreAnimation;
#else
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
#endif

namespace MonoTouchFixtures.CoreAnimation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CATextLayerTests {

		[Test]
		public void CATextLayerTruncationModeTest ()
		{
			var textLayer = new CATextLayer {
				String = "Hello",
				TextTruncationMode = CATextLayerTruncationMode.Middle
			};

			Assert.AreEqual (CATextLayerTruncationMode.Middle, textLayer.TextTruncationMode, "TextTruncationMode");
			Assert.AreEqual (textLayer.TruncationMode, (string) textLayer.TextTruncationMode.GetConstant (), "TruncationMode");

			textLayer.TruncationMode = CATextLayer.TruncantionEnd;
			Assert.AreEqual (CATextLayerTruncationMode.End, textLayer.TextTruncationMode, "TextTruncationMode 2");

			Assert.Throws<ArgumentNullException> (() => textLayer.TruncationMode = null);
		}

		[Test]
		public void CATextLayerAlignmentModeTest ()
		{
			var textLayer = new CATextLayer {
				String = "Hello",
				TextAlignmentMode = CATextLayerAlignmentMode.Justified
			};

			Assert.AreEqual (CATextLayerAlignmentMode.Justified, textLayer.TextAlignmentMode, "TextAlignmentMode");
			Assert.AreEqual (textLayer.AlignmentMode, (string) textLayer.TextAlignmentMode.GetConstant (), "AlignmentMode");

			textLayer.AlignmentMode = CATextLayer.AlignmentNatural;
			Assert.AreEqual (CATextLayerAlignmentMode.Natural, textLayer.TextAlignmentMode, "TextAlignmentMode 2");

			Assert.Throws<ArgumentNullException> (() => textLayer.AlignmentMode = null);
		}
	}
}
#endif // !__WATCHOS__
