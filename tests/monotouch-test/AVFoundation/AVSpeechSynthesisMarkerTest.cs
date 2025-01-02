//
// Unit tests for AVSpeechSynthesisMarker

using System;

using AVFoundation;
using Foundation;

using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVSpeechSynthesisMarkerTest {
		[Test]
		public void NSRangeCtor ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var range = new NSRange (1, 2);
			nint byteOffset = 314;
			Assert.Multiple (() => {
				{
					using var marker = new AVSpeechSynthesisMarker (range, byteOffset, AVSpeechSynthesisMarkerRangeOption.Word);
					Assert.AreEqual (range, marker.TextRange, "TextRange W");
					Assert.AreEqual (byteOffset, (nint) marker.ByteSampleOffset, "ByteSampleOffset W");
					Assert.AreEqual (AVSpeechSynthesisMarkerMark.Word, marker.Mark, "AVSpeechSynthesisMarkerMark W");
					Assert.IsNull (marker.BookmarkName, "BookmarkName W");
					Assert.IsNull (marker.Phoneme, "Phoneme W");
				}
				{
					using var marker = new AVSpeechSynthesisMarker (range, byteOffset, AVSpeechSynthesisMarkerRangeOption.Sentence);
					Assert.AreEqual (range, marker.TextRange, "TextRange S");
					Assert.AreEqual (byteOffset, (nint) marker.ByteSampleOffset, "ByteSampleOffset S");
					Assert.AreEqual (AVSpeechSynthesisMarkerMark.Sentence, marker.Mark, "AVSpeechSynthesisMarkerMark S");
					Assert.IsNull (marker.BookmarkName, "BookmarkName S");
					Assert.IsNull (marker.Phoneme, "Phoneme S");
				}
				{
					using var marker = new AVSpeechSynthesisMarker (range, byteOffset, AVSpeechSynthesisMarkerRangeOption.Paragraph);
					Assert.AreEqual (range, marker.TextRange, "TextRange P");
					Assert.AreEqual (byteOffset, (nint) marker.ByteSampleOffset, "ByteSampleOffset P");
					Assert.AreEqual (AVSpeechSynthesisMarkerMark.Paragraph, marker.Mark, "AVSpeechSynthesisMarkerMark P");
					Assert.IsNull (marker.BookmarkName, "BookmarkName P");
					Assert.IsNull (marker.Phoneme, "Phoneme P");
				}
			});
		}

		[Test]
		public void StringCtor ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var range = new NSRange (0, 0);
			var value = "hello world";
			nint byteOffset = 314;
			Assert.Multiple (() => {
				{
					using var marker = new AVSpeechSynthesisMarker (value, byteOffset, AVSpeechSynthesisMarkerStringOption.Phoneme);
					Assert.AreEqual (range, marker.TextRange, "TextRange P");
					Assert.AreEqual (byteOffset, (nint) marker.ByteSampleOffset, "ByteSampleOffset P");
					Assert.AreEqual (AVSpeechSynthesisMarkerMark.Phoneme, marker.Mark, "AVSpeechSynthesisMarkerMark P");
					Assert.IsNull (marker.BookmarkName, "BookmarkName P");
					Assert.AreEqual (value, marker.Phoneme, "Phoneme P");
				}
				{
					using var marker = new AVSpeechSynthesisMarker (value, byteOffset, AVSpeechSynthesisMarkerStringOption.Bookmark);
					Assert.AreEqual (range, marker.TextRange, "TextRange B");
					Assert.AreEqual (byteOffset, (nint) marker.ByteSampleOffset, "ByteSampleOffset B");
					Assert.AreEqual (AVSpeechSynthesisMarkerMark.Bookmark, marker.Mark, "AVSpeechSynthesisMarkerMark B");
					Assert.IsNull (marker.Phoneme, "Phoneme B");
				}
			});
		}
	}
}
