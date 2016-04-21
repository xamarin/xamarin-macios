//
// Enums.cs: enums for AppKit
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2012 Xamarin Inc

using System;
using XamCore.ObjCRuntime;

namespace XamCore.AppKit {

	[Lion]
	[Native]
	public enum NSTextLayoutOrientation : nint {
		Horizontal,
		Vertical
	}

#if !XAMCORE_2_0
	[Lion, Flags]
	[Native]
	public enum NSTableViewAnimationOptions : nuint_compat_int {
		EffectFade = 0x1,
		EffectGap = 0x2,

		// these cannot be combined
		SlideUp = 0x10,
		SlideDown = 0x20,
		SlideLeft = 0x30,
		SlideRight = 0x40,
	}
#endif

	[Lion]
	[Native]
	public enum NSPrintRenderingQuality : nint {
		Best,
		Responsive
	}

	[Lion]
	[Native]
	public enum NSCorrectionIndicatorType : nint {
		Default = 0,
		Reversion,
		Guesses
	}

	[Lion]
	[Native]
	public enum NSCorrectionResponse : nint {
		None,
		Accepted,
		Rejected,
		Ignored,
		Edited,
		Reverted
	}

	[Lion]
	[Native]
	public enum NSTextFinderMatchingType : nint {
		Contains = 0,
		StartsWith = 1,
		FullWord = 2,
		EndsWith = 3
	}

	[Native]
	public enum NSCharacterCollection : nuint_compat_int {
		/// <summary>Identity mapping (CID == NSGlyph)</summary>
		IdentityMapping = 0,

		/// <summary>Adobe-CNS1</summary>
		AdobeCns1 = 1,

		/// <summary>Adobe-GB1</summary>
		AdobeGb1 = 2,

		/// <summary>Adobe-Japan1</summary>
		AdobeJapan1 = 3,

		/// <summary>Adobe-Japan2</summary>
		AdobeJapan2 = 4,

		/// <summary>Adobe-Korea1</summary>
		AdobeKorea1 = 5
	}

	// Untyped enum (NSAttributedString.h). Only used as a convience enum in our API.
	[Flags]
	public enum NSSpellingState : int {
		None = 0x0,
		Spelling = 0x1,
		Grammar = 0x2
	}
}

