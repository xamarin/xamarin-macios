using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace AVFoundation {
	/// <summary>This enum is used to select how to initialize a new <see cref="AVSpeechSynthesisMarker" /> instance.</summary>
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("maccatalyst17.0")]
	[SupportedOSPlatform ("macos14.0")]
	[SupportedOSPlatform ("tvos17.0")]
	public enum AVSpeechSynthesisMarkerRangeOption {
		/// <summary>The <c>range</c> parameter passed to the constructor is a word range.</summary>
		Word,
		/// <summary>The <c>range</c> parameter passed to the constructor is a sentence range.</summary>
		Sentence,
		/// <summary>The <c>range</c> parameter passed to the constructor is a paragraph range.</summary>
		Paragraph,
	}

	/// <summary>This enum is used to select how to initialize a new <see cref="AVSpeechSynthesisMarker" /> instance.</summary>
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("maccatalyst17.0")]
	[SupportedOSPlatform ("macos14.0")]
	[SupportedOSPlatform ("tvos17.0")]
	public enum AVSpeechSynthesisMarkerStringOption {
		/// <summary>The <c>value</c> parameter passed to the constructor is a phoneme.</summary>
		Phoneme,
		/// <summary>The <c>value</c> parameter passed to the constructor is a bookmark name.</summary>
		Bookmark,
	}

	public partial class AVSpeechSynthesisMarker {
		/// <summary>Create a new <see cref="AVSpeechSynthesisMarker" /> instance for the specified range and byte offset.</summary>
		/// <param name="range">The range of the marker.</param>
		/// <param name="byteSampleOffset">The byte offset into the audio buffer.</param>
		/// <param name="option">Use this option to specify how to interpret the <paramref name="range" /> parameter.</param>
		public AVSpeechSynthesisMarker (NSRange range, nint byteSampleOffset, AVSpeechSynthesisMarkerRangeOption option)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case AVSpeechSynthesisMarkerRangeOption.Word:
				InitializeHandle (_InitWithWordRange (range, byteSampleOffset));
				break;
			case AVSpeechSynthesisMarkerRangeOption.Sentence:
				InitializeHandle (_InitWithSentenceRange (range, byteSampleOffset));
				break;
			case AVSpeechSynthesisMarkerRangeOption.Paragraph:
				InitializeHandle (_InitWithParagraphRange (range, byteSampleOffset));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}

		/// <summary>Create a new <see cref="AVSpeechSynthesisMarker" /> instance for the specified string value.</summary>
		/// <param name="value">The phoneme or bookmark name of the marker.</param>
		/// <param name="byteSampleOffset">The byte offset into the audio buffer.</param>
		/// <param name="option">Use this option to specify how to interpret the <paramref name="value" /> parameter.</param>
		public AVSpeechSynthesisMarker (string value, nint byteSampleOffset, AVSpeechSynthesisMarkerStringOption option)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case AVSpeechSynthesisMarkerStringOption.Phoneme:
				InitializeHandle (_InitWithPhonemeString (value, byteSampleOffset));
				break;
			case AVSpeechSynthesisMarkerStringOption.Bookmark:
				InitializeHandle (_InitWithBookmarkName (value, byteSampleOffset));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}
	}
}
