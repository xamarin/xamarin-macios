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

#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
#else
	[iOS (7,0)]
	[Mac (10,9)]
#endif
	[Native]
	public enum MACaptionAppearanceDomain : long {
		Default = 0,
		User = 1,
	}

#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
#else
	[iOS (7,0)]
	[Mac (10,9)]
#endif
	[Native]
	public enum MACaptionAppearanceDisplayType : long {
		ForcedOnly = 0,
		Automatic = 1,
		AlwaysOn = 2,
	}

#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
#else
	[iOS (7,0)]
	[Mac (10,9)]
#endif
	[Native]
	public enum MACaptionAppearanceBehavior : long {
		UseValue = 0,
		UseContentIfAvailable = 1,
	}

#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
#else
	[iOS (7,0)]
	[Mac (10,9)]
#endif
	[Native]
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

#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
#else
	[iOS (7,0)]
	[Mac (10,9)]
#endif
	[Native]
	public enum MACaptionAppearanceTextEdgeStyle : long {
		Undefined = 0,
		None = 1,
		Raised = 2,
		Depressed = 3,
		Uniform = 4,
		DropShadow = 5,
	}
}
