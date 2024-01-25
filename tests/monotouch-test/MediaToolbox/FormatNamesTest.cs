
#if !__WATCHOS__

using System;

using CoreMedia;

using Foundation;
#if !MONOMAC
using UIKit;
#endif
using MediaToolbox;

using NUnit.Framework;

namespace MonoTouchFixtures.MediaToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FormatNamesTest {

		[Test]
		[Culture ("en")]
		public void LocalizedNameForMediaType ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS 9.0 or macOS 10.11");

			Assert.That (CMMediaType.Audio.GetLocalizedName (), Is.EqualTo ("Sound"), "Audio");
			Assert.That (CMMediaType.ClosedCaption.GetLocalizedName (), Is.EqualTo ("Closed Caption"), "ClosedCaption");
			if (TestRuntime.CheckXcodeVersion (9, 0)) {
				Assert.That (CMMediaType.Metadata.GetLocalizedName (), Is.EqualTo ("Metadata"), "Metadata");
			} else {
				Assert.That (CMMediaType.Metadata.GetLocalizedName (), Is.EqualTo ("meta"), "Metadata");
			}
			Assert.That (CMMediaType.Muxed.GetLocalizedName (), Is.EqualTo ("Muxed"), "Muxed");
			Assert.That (CMMediaType.Subtitle.GetLocalizedName (), Is.EqualTo ("Subtitle"), "Subtitle");
			Assert.That (CMMediaType.Text.GetLocalizedName (), Is.EqualTo ("Text"), "Text");
			Assert.That (CMMediaType.TimeCode.GetLocalizedName (), Is.EqualTo ("Timecode"), "TimeCode");
			Assert.That (CMMediaType.Video.GetLocalizedName (), Is.EqualTo ("Video"), "Video");

			// incorrect
			Assert.That (((CMMediaType) 1).GetLocalizedName (), Is.EqualTo (String.Empty), "-1");
		}

		[Test]
		[Culture ("en")]
		public void LocalizedNameForMediaSubType ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS 9.0 or macOS 10.11");

			Assert.That (CMMediaType.ClosedCaption.GetLocalizedName ((uint) CMClosedCaptionFormatType.ATSC), Is.EqualTo ("ATSC/52 part-4"), "ATSC");
			Assert.That (CMMediaType.ClosedCaption.GetLocalizedName ((uint) CMClosedCaptionFormatType.CEA608), Is.EqualTo ("CEA 608"), "CEA608");
			Assert.That (CMMediaType.ClosedCaption.GetLocalizedName ((uint) CMClosedCaptionFormatType.CEA708), Is.EqualTo ("CEA 708"), "CEA708");

			// sounds incorrect - maybe it gets mapped to another value (like a default dictionary of 4CC values)
			Assert.That (CMMediaType.Audio.GetLocalizedName ((uint) CMClosedCaptionFormatType.ATSC), Is.EqualTo ("atcc"), "incorrect");
		}
	}
}

#endif // !__WATCHOS__
