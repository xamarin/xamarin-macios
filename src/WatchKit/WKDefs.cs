//
// WatchKit Enums
//
// Copyright 2014-2015 Xamarin Inc.
//
// Author:
//  Miguel de Icaza
//

using System;
using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[NoiOS]
	[Native]
	public enum WKInterfaceMapPinColor : long {
		Red,
		Green,
		Purple
	}

	[NoiOS]
	[Deprecated (PlatformName.WatchOS, 7, 0)]
	[Native]
	public enum WKMenuItemIcon : long {
		Accept,
		Add,
		Block,
		Decline,
		Info,
		Maybe,
		More,
		Mute,
		Pause,
		Play,
		Repeat,
		Resume,
		Share,
		Shuffle,
		Speaker,
		Trash
	}

	[NoiOS]
	[Native]
	public enum WKUserNotificationInterfaceType : long {
		Default,
		Custom
	}

	[NoiOS]
	[Native]
	public enum WKTextInputMode : long {
		Plain,
		AllowEmoji,
		AllowAnimatedEmoji
	}

	[NoiOS]
	[Native ("WatchKitErrorCode")]
	[ErrorDomain ("WatchKitErrorDomain")]
	public enum WKErrorCode : long {
		None = 0,
		UnknownError = 1,
		RequestReplyNotCalledError = 2,
		InvalidArgumentError = 3,
		MediaPlayerError = 4,
		DownloadError = 5,
		RecordingFailedError = 6,
	}

	[NoiOS]
	[Native]
	public enum WKHapticType : long {
		Notification,
		DirectionUp,
		DirectionDown,
		Success,
		Failure,
		Retry,
		Start,
		Stop,
		Click,
		[Watch (7, 0)]
		NavigationLeftTurn,
		[Watch (7, 0)]
		NavigationRightTurn,
		[Watch (7, 0)]
		NavigationGenericManeuver,
		[Watch (9, 0)]
		UnderwaterDepthPrompt,
		[Watch (9, 0)]
		UnderwaterDepthCriticalPrompt,
	}

	[NoiOS]
	[Native]
	public enum WKAudioFilePlayerStatus : long {
		Unknown,
		ReadyToPlay,
		Failed
	}

	[NoiOS]
	[Native]
	public enum WKAudioFilePlayerItemStatus : long {
		Unknown,
		ReadyToPlay,
		Failed
	}

	[NoiOS]
	[Native]
	public enum WKAudioRecorderPreset : long {
		NarrowBandSpeech,
		WideBandSpeech,
		HighQualityAudio
	}

	[NoiOS]
	[Native]
	public enum WKAlertActionStyle : long {
		Default = 0,
		Cancel,
		Destructive
	}

	[NoiOS]
	[Native]
	public enum WKAlertControllerStyle : long {
		Alert,
		SideBySideButtonsAlert,
		ActionSheet
	}

	[NoiOS]
	[Native]
	public enum WKVideoGravity : long {
		Aspect,
		AspectFill,
		Resize
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceObjectHorizontalAlignment : long {
		Left,
		Center,
		Right
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceObjectVerticalAlignment : long {
		Top,
		Center,
		Bottom
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceLayoutDirection : long {
		LeftToRight,
		RightToLeft
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceSemanticContentAttribute : long {
		Unspecified,
		Playback,
		Spatial,
		ForceLeftToRight,
		ForceRightToLeft
	}

	[NoiOS]
	[Native]
	public enum WKApplicationState : long {
		Active,
		Inactive,
		Background
	}

	[NoiOS]
	[Native]
	public enum WKGestureRecognizerState : long {
		Possible,
		Began,
		Changed,
		Ended,
		Cancelled,
		Failed,
		Recognized
	}

	[NoiOS]
	[Native]
	[Flags]
	public enum WKSwipeGestureRecognizerDirection : ulong {
		Right = 1 << 0,
		Left = 1 << 1,
		Up = 1 << 2,
		Down = 1 << 3
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceDeviceWristLocation : long {
		Left,
		Right
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceDeviceCrownOrientation : long {
		Left,
		Right
	}

	[NoiOS]
	[Native]
	public enum WKWaterResistanceRating : long {
		Ipx7,
		Wr50,
		[Watch (9, 0)]
		WR100,
	}

	[NoiOS]
	[Native]
	public enum WKSnapshotReason : long {
		AppScheduled = 0,
		ReturnToDefaultState,
		ComplicationUpdate,
		Prelaunch,
		AppBackgrounded,
	}

	[NoiOS]
	[Native]
	public enum WKPageOrientation : long {
		Horizontal,
		Vertical,
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceScrollPosition : long {
		Top,
		CenteredVertically,
		Bottom,
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceDeviceBatteryState : long {
		Unknown,
		Unplugged,
		Charging,
		Full,
	}

	[Watch (9, 0), NoiOS]
	[Native]
	enum WKExtendedRuntimeSessionAutoLaunchAuthorizationStatus : long {
		Unknown,
		Inactive,
		Active,
	}
}
