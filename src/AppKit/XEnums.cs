//
// Enums.cs: enums for AppKit
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2012 Xamarin Inc

using System;

using ObjCRuntime;

#nullable enable

namespace AppKit {

	[NoMacCatalyst]
	[Native]
	public enum NSPrintRenderingQuality : long {
		Best,
		Responsive
	}

	[NoMacCatalyst]
	[Native]
	public enum NSCorrectionIndicatorType : long {
		Default = 0,
		Reversion,
		Guesses
	}

	[NoMacCatalyst]
	[Native]
	public enum NSCorrectionResponse : long {
		None,
		Accepted,
		Rejected,
		Ignored,
		Edited,
		Reverted
	}

	[NoMacCatalyst]
	[Native]
	public enum NSTextFinderMatchingType : long {
		Contains = 0,
		StartsWith = 1,
		FullWord = 2,
		EndsWith = 3
	}

	[NoMacCatalyst]
	[Native]
	public enum NSCharacterCollection : ulong {
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

	[NoMacCatalyst]
	[Flags]
#if NET
	[Native]
	public enum NSSpellingState : long
#else
	public enum NSSpellingState : int
#endif
	{
		None = 0x0,
		Spelling = 0x1,
		Grammar = 0x2
	}
}
