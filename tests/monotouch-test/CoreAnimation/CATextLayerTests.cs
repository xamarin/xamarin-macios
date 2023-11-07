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

using Foundation;
using CoreAnimation;

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
#if !NET
			Assert.AreEqual (textLayer.TruncationMode, (string) textLayer.TextTruncationMode.GetConstant (), "TruncationMode");
#endif

#if NET
			textLayer.TextTruncationMode = CATextLayerTruncationMode.End;
#else
			textLayer.TruncationMode = CATextLayer.TruncantionEnd;
#endif
			Assert.AreEqual (CATextLayerTruncationMode.End, textLayer.TextTruncationMode, "TextTruncationMode 2");

#if !NET
			Assert.Throws<ArgumentNullException> (() => textLayer.TruncationMode = null);
#endif
		}

		[Test]
		public void CATextLayerAlignmentModeTest ()
		{
			var textLayer = new CATextLayer {
				String = "Hello",
				TextAlignmentMode = CATextLayerAlignmentMode.Justified
			};

			Assert.AreEqual (CATextLayerAlignmentMode.Justified, textLayer.TextAlignmentMode, "TextAlignmentMode");
#if !NET
			Assert.AreEqual (textLayer.AlignmentMode, (string) textLayer.TextAlignmentMode.GetConstant (), "AlignmentMode");
#endif

#if NET
			textLayer.TextAlignmentMode = CATextLayerAlignmentMode.Natural;
#else
			textLayer.AlignmentMode = CATextLayer.AlignmentNatural;
#endif
			Assert.AreEqual (CATextLayerAlignmentMode.Natural, textLayer.TextAlignmentMode, "TextAlignmentMode 2");

#if !NET
			Assert.Throws<ArgumentNullException> (() => textLayer.AlignmentMode = null);
#endif
		}
	}
}
#endif // !__WATCHOS__
