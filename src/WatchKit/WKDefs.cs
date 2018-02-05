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
	[iOS (8,2)]
	[Native]
	public enum WKInterfaceMapPinColor : long {
		Red,
		Green,
		Purple
	}

	[iOS (8,2)]
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
		
	[iOS (8,2)]
	[Native]
	public enum WKUserNotificationInterfaceType : long {
		Default,
		Custom
	}

	[iOS (8,2)]
	[Native]
	public enum WKTextInputMode : long {
		Plain,
		AllowEmoji,
		AllowAnimatedEmoji
	}

	[iOS (8,2)]
	[Native]
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
		Click
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

	[Watch (2,0), NoiOS]
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

	[Watch (2,0), NoiOS]
	[Native]
	public enum WKAlertControllerStyle : long {
		Alert,
		SideBySideButtonsAlert,
		ActionSheet
	}

	[Watch (2,0), NoiOS]
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

	[Watch (2,1), NoiOS]
	[Native]
	public enum WKInterfaceLayoutDirection : long
	{
		LeftToRight,
		RightToLeft
	}

	[Watch (2,1), NoiOS]
	[Native]
	public enum WKInterfaceSemanticContentAttribute : long
	{
		Unspecified,
		Playback,
		Spatial,
		ForceLeftToRight,
		ForceRightToLeft
	}

	[Watch (3,0)][NoiOS]
	[Native]
	public enum WKApplicationState : long {
		Active,
		Inactive,
		Background
	}

	[Watch (3,0)][NoiOS]
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

	[Watch (3,0)][NoiOS]
	[Native]
	[Flags]
	public enum WKSwipeGestureRecognizerDirection : ulong {
		Right = 1 << 0,
		Left = 1 << 1,
		Up = 1 << 2,
		Down = 1 << 3
	}

	[Watch (3,0)][NoiOS]
	[Native]
	public enum WKInterfaceDeviceWristLocation : long {
		Left,
		Right
	}

	[Watch (3,0)][NoiOS]
	[Native]
	public enum WKInterfaceDeviceCrownOrientation : long {
		Left,
		Right
	}

	[Watch (3,0)][NoiOS]
	[Native]
	public enum WKWaterResistanceRating : long {
		Ipx7,
		Wr50,
	}

	[Watch (4,0)][NoiOS]
	[Native]
	public enum WKSnapshotReason : long {
		AppScheduled = 0,
		ReturnToDefaultState,
		ComplicationUpdate,
		Prelaunch,
		AppBackgrounded,
	}

	[Watch (4,0)][NoiOS]
	[Native]
	public enum WKPageOrientation : long {
		Horizontal,
		Vertical,
	}

	[Watch (4,0)][NoiOS]
	[Native]
	public enum WKInterfaceScrollPosition : long {
		Top,
		CenteredVertically,
		Bottom,
	}

	[Watch (4,0)][NoiOS]
	[Native]
	public enum WKInterfaceDeviceBatteryState : long {
		Unknown,
		Unplugged,
		Charging,
		Full,
	}
}
