//
// MAEnums.cs: enumarations for iOS (7+) MediaAccessibility framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013, 2015 Xamarin Inc.

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using CoreGraphics;
using CoreText;
using Foundation;

namespace MediaAccessibility {

	/// <summary>Enumerates values that indicate whether to use the system's or the user's caption appearance settings.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum MACaptionAppearanceDomain : long {
		Default = 0,
		User = 1,
	}

	/// <summary>Enumerates values that indicate whether to display captions only for translation, always, or only if an audio track language differs from the system.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum MACaptionAppearanceDisplayType : long {
		ForcedOnly = 0,
		Automatic = 1,
		AlwaysOn = 2,
	}

	/// <summary>Enumerates values that indicate whether to override a setting with the value that is supplied by the media, if present.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum MACaptionAppearanceBehavior : long {
		UseValue = 0,
		UseContentIfAvailable = 1,
	}

	/// <summary>Enumerated values that control the font for the captions.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum MACaptionAppearanceFontStyle : long {
		Default = 0,
		MonospacedWithSerif = 1,
		ProportionalWithSerif = 2,
		MonospacedWithoutSerif = 3,
		ProportionalWithoutSerif = 4,
		Casual = 5,
		Cursive = 6,
		SmallCapital = 7,
	}

	/// <summary>Enumerates values that indicate whether to raise caption text, use drop shadows on them, or etc.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum MACaptionAppearanceTextEdgeStyle : long {
		Undefined = 0,
		None = 1,
		Raised = 2,
		Depressed = 3,
		Uniform = 4,
		DropShadow = 5,
	}
}
