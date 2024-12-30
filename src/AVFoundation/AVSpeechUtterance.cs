using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace AVFoundation {
	/// <summary>This enum is used to select how to initialize a new <see cref="AVSpeechUtterance" /> instance.</summary>
	public enum AVSpeechUtteranceInitializationOption {
		/// <summary>The <c>string</c> parameter passed to the constructor is a plain text string.</summary>
		PlainText,
		/// <summary>The <c>string</c> parameter passed to the constructor is an SSML (Speech Synthesis Markup Language) string.</summary>
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
		SsmlRepresentation,
	}

	public partial class AVSpeechUtterance {
		/// <summary>Create a new <see cref="AVSpeechUtterance" /> instance for the specified string.</summary>
		/// <param name="string">The text to speak.</param>
		public AVSpeechUtterance (string @string)
			: this (@string, AVSpeechUtteranceInitializationOption.PlainText)
		{
		}

		/// <summary>Create a new <see cref="AVSpeechUtterance" /> instance for the specified string.</summary>
		/// <param name="string">The text to speak.</param>
		/// <param name="option">Use this option to specify how to interpret the <paramref name="string" /> parameter.</param>
		public AVSpeechUtterance (string @string, AVSpeechUtteranceInitializationOption option)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case AVSpeechUtteranceInitializationOption.PlainText:
				InitializeHandle (_InitWithString (@string));
				break;
			case AVSpeechUtteranceInitializationOption.SsmlRepresentation:
				InitializeHandle (_InitWithSsmlRepresentation (@string));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}
	}
}
