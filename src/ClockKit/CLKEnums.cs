//
// ClockKit Enums
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015-2016 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.ObjCRuntime;

namespace XamCore.ClockKit {

	[Native]
	public enum CLKComplicationFamily : nint {
		ModularSmall,
		ModularLarge,
		UtilitarianSmall,
		UtilitarianLarge,
		CircularSmall,
		// nothing has the value of 5
		[Watch (3,0)]
		UtilitarianSmallFlat = 6,
		[Watch (3,0)]
		ExtraLarge = 7,
	}

	[Native]
	public enum CLKComplicationTimelineAnimationBehavior : nuint {
		Never,
		Grouped,
		Always
	}

	[Native]
	[Flags]
	public enum CLKComplicationTimeTravelDirections : nuint {
		None = 0,
		Forward = 1 << 0,
		Backward = 1 << 1
	}

	[Native]
	public enum CLKComplicationPrivacyBehavior : nuint {
		ShowOnLockScreen,
		HideOnLockScreen
	}

	[Native]
	public enum CLKComplicationColumnAlignment : nint {
		Leading,	// renamed from Left in watchOS 2.1
		Trailing,	// renamed from Right in watchOS 2.1
	}

	[Native]
	public enum CLKComplicationRingStyle : nint {
		Closed,
		Open
	}

	[Native]
	public enum CLKRelativeDateStyle : nint {
		Natural,
		Offset,
		Timer
	}

}

