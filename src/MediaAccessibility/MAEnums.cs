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
		/// <summary>To be added.</summary>
		Default = 0,
		/// <summary>To be added.</summary>
		User = 1,
	}

	/// <summary>Enumerates values that indicate whether to display captions only for translation, always, or only if an audio track language differs from the system.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum MACaptionAppearanceDisplayType : long {
		/// <summary>To be added.</summary>
		ForcedOnly = 0,
		/// <summary>To be added.</summary>
		Automatic = 1,
		/// <summary>To be added.</summary>
		AlwaysOn = 2,
	}

	/// <summary>Enumerates values that indicate whether to override a setting with the value that is supplied by the media, if present.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum MACaptionAppearanceBehavior : long {
		/// <summary>To be added.</summary>
		UseValue = 0,
		/// <summary>To be added.</summary>
		UseContentIfAvailable = 1,
	}

	/// <summary>Enumerated values that control the font for the captions.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum MACaptionAppearanceFontStyle : long {
		/// <summary>To be added.</summary>
		Default = 0,
		/// <summary>To be added.</summary>
		MonospacedWithSerif = 1,
		/// <summary>To be added.</summary>
		ProportionalWithSerif = 2,
		/// <summary>To be added.</summary>
		MonospacedWithoutSerif = 3,
		/// <summary>To be added.</summary>
		ProportionalWithoutSerif = 4,
		/// <summary>To be added.</summary>
		Casual = 5,
		/// <summary>To be added.</summary>
		Cursive = 6,
		/// <summary>To be added.</summary>
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
