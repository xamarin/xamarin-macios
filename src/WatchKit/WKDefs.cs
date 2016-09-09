//
// WatchKit Enums
//
// Copyright 2014-2015 Xamarin Inc.
//
// Author:
//  Miguel de Icaza
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.WatchKit {
	[iOS (8,2)]
	[Native]
	public enum WKInterfaceMapPinColor : nint {
		Red,
		Green,
		Purple
	}

	[iOS (8,2)]
	[Native]
	public enum WKMenuItemIcon : nint {
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
	public enum WKUserNotificationInterfaceType : nint {
		Default,
		Custom
	}

	[iOS (8,2)]
	[Native]
	public enum WKTextInputMode : nint {
		Plain,
		AllowEmoji,
		AllowAnimatedEmoji
	}

	[iOS (8,2)]
	[Native]
	[ErrorDomain ("WatchKitErrorDomain")]
	public enum WKErrorCode : nint {
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
	public enum WKHapticType : nint {
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
	public enum WKAudioFilePlayerStatus : nint {
		Unknown,
		ReadyToPlay,
		Failed
	}

	[NoiOS]
	[Native]
	public enum WKAudioFilePlayerItemStatus : nint {
		Unknown,
		ReadyToPlay,
		Failed
	}

	[Watch (2,0), NoiOS]
	[Native]
	public enum WKAudioRecorderPreset : nint {
		NarrowBandSpeech,
		WideBandSpeech,
		HighQualityAudio
	}

	[NoiOS]
	[Native]
	public enum WKAlertActionStyle : nint {
		Default = 0,
		Cancel,
		Destructive
	}

	[Watch (2,0), NoiOS]
	[Native]
	public enum WKAlertControllerStyle : nint {
		Alert,
		SideBySideButtonsAlert,
		ActionSheet
	}

	[Watch (2,0), NoiOS]
	[Native]
	public enum WKVideoGravity : nint {
		Aspect,
		AspectFill,
		Resize
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceObjectHorizontalAlignment : nint {
		Left,
		Center,
		Right
	}

	[NoiOS]
	[Native]
	public enum WKInterfaceObjectVerticalAlignment : nint {
		Top,
		Center,
		Bottom
	}

	[Watch (2,1), NoiOS]
	[Native]
	public enum WKInterfaceLayoutDirection : nint
	{
		LeftToRight,
		RightToLeft
	}

	[Watch (2,1), NoiOS]
	[Native]
	public enum WKInterfaceSemanticContentAttribute : nint
	{
		Unspecified,
		Playback,
		Spatial,
		ForceLeftToRight,
		ForceRightToLeft
	}

	[Watch (3,0)][NoiOS]
	[Native]
	public enum WKApplicationState : nint {
		Active,
		Inactive,
		Background
	}

	[Watch (3,0)][NoiOS]
	[Native]
	public enum WKGestureRecognizerState : nint {
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
	public enum WKSwipeGestureRecognizerDirection : nuint {
		Right = 1 << 0,
		Left = 1 << 1,
		Up = 1 << 2,
		Down = 1 << 3
	}

	[Watch (3,0)][NoiOS]
	[Native]
	public enum WKInterfaceDeviceWristLocation : nint {
		Left,
		Right
	}

	[Watch (3,0)][NoiOS]
	[Native]
	public enum WKInterfaceDeviceCrownOrientation : nint {
		Left,
		Right
	}

	[Watch (3,0)][NoiOS]
	[Native]
	public enum WKWaterResistanceRating : nint {
		Ipx7,
		Wr50,
	}
}
