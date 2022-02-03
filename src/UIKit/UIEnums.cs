//
// UIEnums.cs:
//
// Copyright 2009-2011 Novell, Inc.
// Copyright 2011-2012, Xamarin Inc.
//
// Author:
//  Miguel de Icaza
//

using System;
using Foundation;
using ObjCRuntime;

namespace UIKit {
	// NSInteger -> UIImagePickerController.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIImagePickerControllerQualityType : long {
		High,
		Medium,
		Low,
		At640x480,
		At1280x720,
		At960x540
	}

	// NSInteger -> UIActivityIndicatorView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIActivityIndicatorViewStyle : long {
#if NET
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0 use 'Large' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'Large' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Large' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Large' instead.")]
#endif
		WhiteLarge,
#if NET
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0 use 'Medium' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'Medium' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Medium' instead.")]
#endif
		White,
#if NET
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0 use 'Medium' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'Medium' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Medium' instead.")]
#endif
		Gray,

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
#else
		[iOS (13,0)]
		[TV (13,0)]
#endif
		Medium = 100,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
#else
		[iOS (13,0)]
		[TV (13,0)]
#endif
		Large = 101,
	}

	// NSInteger -> UIAlertView.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIAlertViewStyle : long {
		Default,
		SecureTextInput,
		PlainTextInput,
		LoginAndPasswordInput
	}

	// NSInteger -> UIBarButtonItem.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIBarButtonItemStyle : long {
		Plain,

#if NET
		[UnsupportedOSPlatform ("ios8.0")]
#if IOS
		[Obsolete ("Starting with ios8.0 use 'UIBarButtonItemStyle.Plain' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'UIBarButtonItemStyle.Plain' instead.")]
#endif
		Bordered,
			
		Done,
	}

	// NSInteger -> UIBarButtonItem.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIBarButtonSystemItem : long {
		Done,
		Cancel,
		Edit,  
		Save,  
		Add,
		FlexibleSpace,
		FixedSpace,
		Compose,
		Reply,
		Action,
		Organize,
		Bookmarks,
		Search,
		Refresh,
		Stop,
		Camera,
		Trash,
		Play,
		Pause,
		Rewind,
		FastForward,
		Undo,
		Redo,
#if NET
		[UnsupportedOSPlatform ("ios11.0")]
#if IOS
		[Obsolete ("Starting with ios11.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 11, 0)]
#endif
		PageCurl,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		Close,
	} 

	// NSUInteger -> UIControl.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIControlEvent : ulong {
		TouchDown           = 1 <<  0,
		TouchDownRepeat     = 1 <<  1,
		TouchDragInside     = 1 <<  2,
		TouchDragOutside    = 1 <<  3,
		TouchDragEnter      = 1 <<  4,
		TouchDragExit       = 1 <<  5,
		TouchUpInside       = 1 <<  6,
		TouchUpOutside      = 1 <<  7,
		TouchCancel         = 1 <<  8,
		
		ValueChanged        = 1 << 12,
		PrimaryActionTriggered = 1 << 13,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
#else
		[iOS (14,0)]
		[TV (14,0)]
#endif
		MenuActionTriggered = 1 << 14,
		
		EditingDidBegin     = 1 << 16,
		EditingChanged      = 1 << 17,
		EditingDidEnd       = 1 << 18,
		EditingDidEndOnExit = 1 << 19,
		
		AllTouchEvents      = 0x00000FFF,
		AllEditingEvents    = 0x000F0000,
		ApplicationReserved = 0x0F000000,
		SystemReserved      = 0xF0000000,
		AllEvents           = 0xFFFFFFFF
	}

	// NSInteger -> UIEvent.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIEventType : long {
		Touches,
		Motion,
		RemoteControl,
#if NET
		[SupportedOSPlatform ("ios9.0")]
#else
		[iOS (9,0)]
#endif
		Presses,
#if NET
		[SupportedOSPlatform ("ios13.4")]
		[SupportedOSPlatform ("tvos13.4")]
#else
		[iOS (13,4)]
		[TV (13,4)]
#endif
		Scroll = 10,
#if NET
		[SupportedOSPlatform ("ios13.4")]
		[SupportedOSPlatform ("tvos13.4")]
#else
		[iOS (13,4)]
		[TV (13,4)]
#endif
		Hover = 11,
#if NET
		[SupportedOSPlatform ("ios13.4")]
		[SupportedOSPlatform ("tvos13.4")]
#else
		[iOS (13,4)]
		[TV (13,4)]
#endif
		Transform = 14,
	}

	// NSInteger -> UIEvent.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIEventSubtype : long {
		None,
		MotionShake,

		RemoteControlPlay                 = 100,
		RemoteControlPause                = 101,
		RemoteControlStop                 = 102,
		RemoteControlTogglePlayPause      = 103,
		RemoteControlNextTrack            = 104,
		RemoteControlPreviousTrack        = 105,
		RemoteControlBeginSeekingBackward = 106,
		RemoteControlEndSeekingBackward   = 107,
		RemoteControlBeginSeekingForward  = 108,
		RemoteControlEndSeekingForward    = 109,
	}			
	
	// NSInteger -> UIControl.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIControlContentVerticalAlignment : long {
		Center  = 0,
		Top     = 1,
		Bottom  = 2,
		Fill    = 3,
	}

	// NSInteger -> UIControl.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIControlContentHorizontalAlignment : long {
		Center = 0,
		Left   = 1,
		Right  = 2,
		Fill   = 3,
		Leading = 4,
		Trailing = 5
	}

	// NSUInteger -> UIControl.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIControlState : ulong {
		Normal       = 0,
		Highlighted  = 1 << 0,
		Disabled     = 1 << 1,
		Selected     = 1 << 2,
#if NET
		[SupportedOSPlatform ("ios9.0")]
#else
		[iOS (9,0)]
#endif
		Focused      = 1 << 3,
		Application  = 0x00FF0000,
		Reserved     = 0xFF000000
	}

	// NSInteger -> UIImage.h
	[Native]
	public enum UIImageOrientation : long {
		Up,
		Down,
		Left, 
		Right,
		UpMirrored,
		DownMirrored,
		LeftMirrored,
		RightMirrored,
	}
	
	// NSUInteger -> UIView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIViewAutoresizing : ulong {
		None                 = 0,
		FlexibleLeftMargin   = 1 << 0,
		FlexibleWidth        = 1 << 1,
		FlexibleRightMargin  = 1 << 2,
		FlexibleTopMargin    = 1 << 3,
		FlexibleHeight       = 1 << 4,
		FlexibleBottomMargin = 1 << 5,
		FlexibleMargins      = FlexibleBottomMargin|FlexibleTopMargin|FlexibleLeftMargin|FlexibleRightMargin,
		FlexibleDimensions   = FlexibleHeight | FlexibleWidth,
		All = 63
	}

	// NSInteger -> UIView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIViewAnimationCurve : long {
		EaseInOut,
		EaseIn,
		EaseOut,
		Linear
	}

	// NSInteger -> UIView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIViewContentMode : long {
		ScaleToFill,
		ScaleAspectFit,
		ScaleAspectFill,
		Redraw,
		Center,
		Top,
		Bottom,
		Left,
		Right,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
	}

	// NSInteger -> UIView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIViewAnimationTransition : long {
		None,
		FlipFromLeft,
		FlipFromRight,
		CurlUp,
		CurlDown,
	}

	// NSInteger -> UIBarCommon.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIBarMetrics : long {
		Default,
		Compact,
		DefaultPrompt = 101,
		CompactPrompt,

#if NET
		[UnsupportedOSPlatform ("ios8.0")]
#if IOS
		[Obsolete ("Starting with ios8.0 use 'UIBarMetrics.Compat' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'UIBarMetrics.Compat' instead.")]
#endif
		LandscapePhone = Compact,

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[UnsupportedOSPlatform ("ios8.0")]
#if IOS
		[Obsolete ("Starting with ios8.0 use 'UIBarMetrics.CompactPrompt' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[iOS (7, 0)]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'UIBarMetrics.CompactPrompt' instead.")]
#endif
		LandscapePhonePrompt = CompactPrompt
	}

	// NSInteger -> UIButton.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIButtonType : long {
		Custom,
		RoundedRect,
		DetailDisclosure,
		InfoLight,
		InfoDark,
		ContactAdd,
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios")]
#else
		[TV (11,0)]
		[NoiOS]
#endif
		Plain,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
		[iOS (13,0)]
#endif
		Close,
		System = RoundedRect,
	}

	// NSInteger -> UIStringDrawing.h
	[Native]
	// note: __TVOS_PROHIBITED -> because it uses NSLineBreakMode (but we need this because we don't expose the later)
	public enum UILineBreakMode : long {
		WordWrap = 0,
		CharacterWrap,
		Clip,          
		HeadTruncation,
		TailTruncation,
		MiddleTruncation,
	}

	// NSInteger -> UIStringDrawing.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIBaselineAdjustment : long {
		AlignBaselines = 0,
		AlignCenters,
		None,
	}

	// NSInteger -> UIDatePicker.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIDatePickerMode : long {
		Time,         
		Date,         
		DateAndTime,  
		CountDownTimer 
	}

	// NSInteger -> UIDevice.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIDeviceOrientation : long {
		Unknown,
		Portrait,
		PortraitUpsideDown,
		LandscapeLeft,     
		LandscapeRight,    
		FaceUp,            
		FaceDown
	}

	// NSInteger -> UIDevice.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIDeviceBatteryState : long {
		Unknown,
		Unplugged,
		Charging, 
		Full,     
	}

	// NSInteger -> UIDocument.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIDocumentChangeKind : long {
		Done, Undone, Redone, Cleared
	}

	// NSInteger -> UIDocument.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIDocumentSaveOperation : long {
		ForCreating, ForOverwriting
	}

	// NSUInteger -> UIDocument.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIDocumentState : ulong {
		Normal = 0,
		Closed = 1 << 0,
		InConflict = 1 << 1,
		SavingError = 1 << 2,
		EditingDisabled = 1 << 3,
		ProgressAvailable = 1 << 4
	}

	// NSInteger -> UIImagePickerController.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[Native]
	public enum UIImagePickerControllerSourceType : long {
#if NET
		[UnsupportedOSPlatform ("ios14.0")]
#if IOS
		[Obsolete ("Starting with ios14.0 use 'PHPicker' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
#endif
		PhotoLibrary,
		Camera,
#if NET
		[UnsupportedOSPlatform ("ios14.0")]
#if IOS
		[Obsolete ("Starting with ios14.0 use 'PHPicker' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
#endif
		SavedPhotosAlbum,
	}

	// NSInteger -> UIImagePickerController.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[Native]
	public enum UIImagePickerControllerCameraCaptureMode : long {
		Photo, Video
	}

	// NSInteger -> UIImagePickerController.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[Native]
	public enum UIImagePickerControllerCameraDevice : long {
		Rear,
		Front
	}

	// NSInteger -> UIImagePickerController.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[Native]
	public enum UIImagePickerControllerCameraFlashMode : long {
		Off = -1, Auto = 0, On = 1
	}

	// NSInteger -> UIInterface.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[Native]
	public enum UIBarStyle : long {
		Default,
		Black,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
#if NET
		[UnsupportedOSPlatform ("ios5.1")]
#if IOS
		[Obsolete ("Starting with ios5.1 use 'UIBarStyle.Black'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 5, 1, message : "Use 'UIBarStyle.Black'.")]
#endif
		BlackOpaque      = 1,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
#if NET
		[UnsupportedOSPlatform ("ios5.1")]
#if IOS
		[Obsolete ("Starting with ios5.1 use 'UIBarStyle.Black' and set the translucency property to true.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 5, 1, message : "Use 'UIBarStyle.Black' and set the translucency property to true.")]
#endif
		BlackTranslucent = 2,
	}

	// NSInteger -> UIProgressView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIProgressViewStyle : long {
		Default,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		Bar,
	}
	
	// NSInteger -> UIScrollView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIScrollViewIndicatorStyle : long {
		Default,
		Black,
		White
	}

	// NSInteger -> UITextInputTraits.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextAutocapitalizationType : long {
		None,
		Words,
		Sentences,
		AllCharacters,
	}

	// NSInteger -> UITextInputTraits.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextAutocorrectionType : long {
		Default,
		No,
		Yes,
	}
	
	// NSInteger -> UITextInputTraits.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIKeyboardType : long {
		Default,
		ASCIICapable,
		AsciiCapable = ASCIICapable,
		NumbersAndPunctuation,
		Url,
		NumberPad,
		PhonePad,
		NamePhonePad,
		EmailAddress,
		DecimalPad,
		Twitter,
		WebSearch,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10, 0)]
#endif
		AsciiCapableNumberPad
	} 

	// NSInteger -> UISegmentedControl.h
#if NET
	[UnsupportedOSPlatform ("ios7.0")]
#if IOS
	[Obsolete ("Starting with ios7.0 this no longer has any effect.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 7, 0, message : "This no longer has any effect.")]
#endif
	[Native]
	public enum UISegmentedControlStyle : long {
		Plain,
		Bordered,
		Bar,
		Bezeled
	}

	// NSInteger -> UITabBarItem.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITabBarSystemItem : long {
		More,
		Favorites,
		Featured,
		TopRated,
		Recents,
		Contacts,
		History,
		Bookmarks,
		Search,
		Downloads,
		MostRecent,
		MostViewed,
	}

	// NSInteger -> UITableView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITableViewStyle : long {
		Plain,
		Grouped,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
		[iOS (13,0)]
#endif
		InsetGrouped,
	}

	// NSInteger -> UITableView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITableViewScrollPosition : long {
		None,        
		Top,    
		Middle,   
		Bottom
	}
	
	// NSInteger -> UITableView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITableViewRowAnimation : long {
		Fade,
		Right,
		Left,
		Top,
		Bottom,
		None,
		Middle,
		Automatic = 100
	}

	// #defines over UIBarPosition -> NSInteger -> UIBarCommon.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIToolbarPosition : long {
		Any, Bottom, Top
	}
	
	// NSInteger -> UITouch.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITouchPhase : long {
		Began,
		Moved,
		Stationary,
		Ended,
		Cancelled,
#if NET
		[SupportedOSPlatform ("ios13.4")]
		[SupportedOSPlatform ("tvos13.4")]
#else
		[iOS (13,4)]
		[TV (13,4)]
#endif
		RegionEntered,
#if NET
		[SupportedOSPlatform ("ios13.4")]
		[SupportedOSPlatform ("tvos13.4")]
#else
		[iOS (13,4)]
		[TV (13,4)]
#endif
		RegionMoved,
#if NET
		[SupportedOSPlatform ("ios13.4")]
		[SupportedOSPlatform ("tvos13.4")]
#else
		[iOS (13,4)]
		[TV (13,4)]
#endif
		RegionExited,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UITouchType : long
	{
		Direct,
		Indirect,
		Stylus,
#if NET
		[SupportedOSPlatform ("ios13.4")]
		[SupportedOSPlatform ("tvos13.4")]
#else
		[iOS (13,4)]
		[TV (13,4)]
#endif
		IndirectPointer,
	}

#if NET
	[SupportedOSPlatform ("ios9.1")]
#else
	[NoWatch]
	[iOS (9,1)]
#endif
	[Native]
	[Flags]
	public enum UITouchProperties : long
	{
		Force = (1 << 0),
		Azimuth = (1 << 1),
		Altitude = (1 << 2),
		Location = (1 << 3),
	}
	
	//
	// UITextAlignment is deprecated in iOS6+ (inside the header file)
	// in favor of NSTextAlignment - but that would be a breaking change
	// so we introduced the new members here. 
	//
	// note: __TVOS_PROHIBITED -> because it uses NSLineBreakMode (but we need this because we don't expose the later)
	//
	// NSInteger -> UIStringDrawing.h
#if __MACCATALYST__
	[Native (ConvertToNative = "UITextAlignmentExtensions.ToNative", ConvertToManaged = "UITextAlignmentExtensions.ToManaged")]
#else
	[Native]
#endif
	public enum UITextAlignment : long {
		Left,
		Center,
		Right, 

		Justified,
		Natural	
	}

	// NSInteger -> UITableViewCell.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITableViewCellStyle : long {
		Default,
		Value1,	
		Value2,	
		Subtitle
	}                 

	// NSInteger -> UITableViewCell.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UITableViewCellSeparatorStyle : long {
		None,
		SingleLine,
#if NET
		[UnsupportedOSPlatform ("ios11.0")]
#if IOS
		[Obsolete ("Starting with ios11.0 se 'SingleLine' for a single line separator.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 11, 0, message:"Use 'SingleLine' for a single line separator.")]
#endif
		SingleLineEtched,
		DoubleLineEtched = SingleLineEtched
	}

	// NSInteger -> UITableViewCell.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITableViewCellSelectionStyle : long {
		None,
		Blue,
		Gray,
		Default
	}

	// NSInteger -> UITableViewCell.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITableViewCellEditingStyle : long {
		None,
		Delete,
		Insert
	}

	// NSInteger -> UITableViewCell.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITableViewCellAccessory : long {
		None,                
		DisclosureIndicator,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		DetailDisclosureButton,
		Checkmark,
#if NET
		[SupportedOSPlatform ("ios7.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
		[iOS (7,0)]
#endif
		DetailButton
	}

	// NSUInteger -> UITableViewCell.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UITableViewCellState : ulong {
		DefaultMask                     = 0,
		ShowingEditControlMask          = 1 << 0,
		ShowingDeleteConfirmationMask   = 1 << 1
	}

	// NSInteger -> UITextField.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextBorderStyle : long {
		None,
		Line,
		Bezel,
		RoundedRect
	}

	// NSInteger -> UITextField.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextFieldViewMode : long {
		Never,
		WhileEditing,
		UnlessEditing,
		Always
	}

	// NSInteger -> UIViewController.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIModalTransitionStyle : long {
		CoverVertical = 0,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		FlipHorizontal,
		CrossDissolve,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		PartialCurl
	}

	// NSInteger -> UIApplication.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[iOS (8,0)]
#endif
	[Native]
	public enum UIInterfaceOrientation : long {
		Unknown            = UIDeviceOrientation.Unknown,
		Portrait           = UIDeviceOrientation.Portrait,
		PortraitUpsideDown = UIDeviceOrientation.PortraitUpsideDown,
		LandscapeLeft      = UIDeviceOrientation.LandscapeRight,
		LandscapeRight     = UIDeviceOrientation.LandscapeLeft
	}

	// NSUInteger -> UIApplication.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIInterfaceOrientationMask : ulong {
		Portrait = 1 << (int) UIInterfaceOrientation.Portrait,
		LandscapeLeft = 1 << (int) UIInterfaceOrientation.LandscapeLeft,
		LandscapeRight = 1 << (int) UIInterfaceOrientation.LandscapeRight,
		PortraitUpsideDown = 1 << (int) UIInterfaceOrientation.PortraitUpsideDown,

		Landscape = LandscapeLeft | LandscapeRight,
		All = PortraitUpsideDown | Portrait | LandscapeRight | LandscapeLeft,
		AllButUpsideDown = Portrait | LandscapeRight | LandscapeLeft,
	}
	
	// NSInteger -> UIApplication.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[Native]
	public enum UIWebViewNavigationType : long {
		LinkClicked,
		FormSubmitted,
		BackForward,
		Reload,
		FormResubmitted,
		Other
	}

	// NSUInteger -> UIApplication.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIDataDetectorType : ulong {
		PhoneNumber            = 1 << 0,
		Link                   = 1 << 1,
		Address                = 1 << 2,
		CalendarEvent          = 1 << 3,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (10,0)]
#endif
		ShipmentTrackingNumber = 1 << 4,
#if NET
		[SupportedOSPlatform ("ios10.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (10,0)]
#endif
		FlightNumber           = 1 << 5,
#if NET
		[SupportedOSPlatform ("ios10.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (10,0)]
#endif
		LookupSuggestion       = 1 << 6,

		None          = 0,
		All           = UInt64.MaxValue
	}

	// NSInteger -> UIActionSheet.h
#if NET
	[UnsupportedOSPlatform ("ios13.0")]
#if IOS
	[Obsolete ("Starting with ios13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 13,0)]
#endif
	[Native]
	public enum UIActionSheetStyle : long {
		Automatic        = -1,
		Default          = UIBarStyle.Default,
		BlackTranslucent = 2, // UIBarStyle.BlackTranslucent,
		BlackOpaque      = 1 // UIBarStyle.BlackOpaque,
	}

	// NSInteger -> UIApplication.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIStatusBarStyle : long {
		Default,

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use 'LightContent' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'LightContent' instead.")]
#endif
		BlackTranslucent = 1,

		LightContent = 1,

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use 'LightContent' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'LightContent' instead.")]
#endif
		BlackOpaque = 2,

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
#endif
		DarkContent = 3,
	}

	// NSInteger -> UIApplication.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIStatusBarAnimation : long {
		None, 
		Fade,
		Slide
	}
	
	// NSInteger -> UIGestureRecognizer.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIGestureRecognizerState : long {
		Possible,
		Began,
		Changed,
		Ended,
		Cancelled,
		Failed,
    
		Recognized = Ended
	}

	// NSUInteger -> UIApplication.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIRemoteNotificationType : ulong {
		None    = 0,
		Badge   = 1 << 0,
		Sound   = 1 << 1,
		Alert   = 1 << 2,
		NewsstandContentAvailability = 1 << 3
	}

	// NSInteger -> UITextInputTraits.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIKeyboardAppearance : long {
		Default,
		Alert,
		Dark = Alert,
		Light
	}

	// NSInteger -> UITextInputTraits.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIReturnKeyType : long {
		Default,
		Go,
		Google,
		Join,
		Next,
		Route,
		Search,
		Send,
		Yahoo,
		Done,
		EmergencyCall,
		Continue
	} 	

	// NSInteger -> UIViewController.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIModalPresentationStyle : long {
		None = -1,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13,0)]
#endif
		Automatic = -2,
		FullScreen = 0,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		PageSheet,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		FormSheet,
		CurrentContext,
		Custom,
		OverFullScreen,
		OverCurrentContext,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		Popover,
		BlurOverFullScreen,
	}
	
	// NSUInteger -> UISwipeGestureRecognizer.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UISwipeGestureRecognizerDirection : ulong {
		Right = 1 << 0,
		Left = 1 << 1,
		Up = 1 << 2,
		Down = 1 << 3,
	}

	// NSUInteger -> UIPopoverController.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIPopoverArrowDirection : ulong {
		Up = 1 << 0,
		Down = 1 << 1,
		Left = 1 << 2,
		Right = 1 << 3,
		Any = Up | Down | Left | Right,
		Unknown = UInt64.MaxValue
	};

	// NSInteger -> UIMenuController.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIMenuControllerArrowDirection : long {
		Default,
		Up,
		Down,
		Left,
		Right,
	}

	// NSUInteger -> UIPopoverController.h
	[Native]
	[Flags]
	public enum UIRectCorner : ulong {
		TopLeft     = 1 << 0,
		TopRight    = 1 << 1,
		BottomLeft  = 1 << 2,
		BottomRight = 1 << 3,
		AllCorners  = ~(ulong)0
	}

	// NSInteger -> UIApplication.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIUserInterfaceLayoutDirection : long {
		LeftToRight, RightToLeft
	}
	
	// NSInteger -> UIDevice.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIUserInterfaceIdiom : long {
		Unspecified = -1,
		Phone,
		Pad,
		TV,
		CarPlay,
#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
#else
		[Watch (7,0)]
		[TV (14,0)]
		[iOS (14,0)]
#endif
		Mac = 5,
	}

	// NSInteger -> UIApplication.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIApplicationState : long {
		Active,
		Inactive,
		Background
	}

	// NSInteger -> UIView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIViewAnimationOptions : ulong {
		LayoutSubviews            = 1 <<  0,
		AllowUserInteraction      = 1 <<  1,
		BeginFromCurrentState     = 1 <<  2,
		Repeat                    = 1 <<  3,
		Autoreverse               = 1 <<  4,
		OverrideInheritedDuration = 1 <<  5,
		OverrideInheritedCurve    = 1 <<  6,
		AllowAnimatedContent      = 1 <<  7,
		ShowHideTransitionViews   = 1 <<  8,
		OverrideInheritedOptions  = 1 <<  9,
		
		CurveEaseInOut            = 0 << 16,
		CurveEaseIn               = 1 << 16,
		CurveEaseOut              = 2 << 16,
		CurveLinear               = 3 << 16,
		
		TransitionNone            = 0 << 20,
		TransitionFlipFromLeft    = 1 << 20,
		TransitionFlipFromRight   = 2 << 20,
		TransitionCurlUp          = 3 << 20,
		TransitionCurlDown        = 4 << 20,
		TransitionCrossDissolve   = 5 << 20,
		TransitionFlipFromTop     = 6 << 20,
		TransitionFlipFromBottom  = 7 << 20,

#if NET
		[SupportedOSPlatform ("ios10.3")]
#else
		[iOS (10,3)]
#endif
		PreferredFramesPerSecondDefault = 0 << 24,
#if NET
		[SupportedOSPlatform ("ios10.3")]
#else
		[iOS (10,3)]
#endif
		PreferredFramesPerSecond60 = 3 << 24,
#if NET
		[SupportedOSPlatform ("ios10.3")]
#else
		[iOS (10,3)]
#endif
		PreferredFramesPerSecond30 = 7 << 24,
	}

#if !WATCH
	delegate void UIPrintInteractionCompletionHandler (UIPrintInteractionController printInteractionController, bool completed, NSError error);
#endif

	// untyped (and unamed) enum -> UIPrintError.h
	// note: it looks unused by any API
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[ErrorDomain ("UIPrintErrorDomain")]
	public enum UIPrintError {
		NotAvailable = 1,
		NoContent,
		UnknownImageFormat,
		JobFailed,
	}

	// NSInteger -> UIPrintInfo.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIPrintInfoDuplex : long {
		None,
		LongEdge,
		ShortEdge,
	}

	// NSInteger -> UIPrintInfo.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIPrintInfoOrientation : long {
		Portrait,
		Landscape,
	}

	// NSInteger -> UIPrintInfo.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIPrintInfoOutputType : long {
		General,
		Photo,
		Grayscale,
		PhotoGrayscale
	}

	// NSInteger -> UIAccessibility.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIAccessibilityScrollDirection : long {
		Right = 1,
		Left,
		Up,
		Down,
		Next,
		Previous
	}

	// NSInteger -> UIScreen.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIScreenOverscanCompensation : long {
		Scale, InsetBounds,
		None,
		[Obsolete ("Use 'UIScreenOverscanCompensation.None' instead.")]
		InsetApplicationFrame = None
	}

	// NSInteger -> UISegmentedControl.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UISegmentedControlSegment : long {
		Any, Left, Center, Right, Alone
	}

	// NSInteger -> UISearchBar.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UISearchBarIcon : long {
		Search,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		Clear,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		Bookmark,
#if NET
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		ResultsList
	}

	// NSInteger -> UIPageViewController.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIPageViewControllerNavigationOrientation : long {
		Horizontal, Vertical
	}

	// NSInteger -> UIPageViewController.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIPageViewControllerSpineLocation : long {
		None, Min, Mid, Max
	}

	// NSInteger -> UIPageViewController.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIPageViewControllerNavigationDirection : long {
		Forward, Reverse
	}

	// NSInteger -> UIPageViewController.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIPageViewControllerTransitionStyle : long {
		PageCurl, Scroll
	}

	// NSInteger -> UITextInputTraits.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextSpellCheckingType : long {
		Default, No, Yes, 
	}

	// NSInteger -> UITextInput.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextStorageDirection : long {
		Forward, Backward
	}

	// NSInteger -> UITextInput.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextLayoutDirection : long {
		Right = 2,
		Left,
		Up,
		Down
	}

	// Sum of UITextStorageDirection and UITextLayoutDirection 
	// NSInteger -> UITextInput.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextDirection : long {
		Forward, Backward, Right, Left, Up, Down
	}
	
#if !NET
	// NSInteger -> UITextInput.h
	// Use Foundation.NSWritingDirection in .NET.
	// see: https://github.com/xamarin/xamarin-macios/issues/6573
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextWritingDirection : long {
		Natural = -1,
		LeftToRight,
		RightToLeft,
	}
#endif

	// NSInteger -> UITextInput.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITextGranularity : long {
		Character,
		Word,
		Sentence,
		Paragraph,
		Line,
		Document
	}

	// float (and not even a CGFloat) -> NSLayoutConstraint.h
	// the API were fixed (a long time ago to use `float`) and the enum
	// values can still be used (and useful) since they will be casted
#if !NET
	[NoWatch]
#endif
	public enum UILayoutPriority {
		Required = 1000,
		DefaultHigh = 750,
		DefaultLow = 250,
		FittingSizeLevel = 50,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13,0)]
#endif
		DragThatCanResizeScene = 510,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13,0)]
#endif
		SceneSizeStayPut = 500,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13,0)]
#endif
		DragThatCannotResizeScene = 490,
	}

	// NSInteger -> NSLayoutConstraint.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UICollectionUpdateAction : long {
		Insert, Delete, Reload, Move, None
	}

	// NSUInteger -> UICollectionView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UICollectionViewScrollPosition : ulong {
		None,
		Top = 1 << 0,
		CenteredVertically = 1 << 1,
		Bottom = 1 << 2,
		Left = 1 << 3,
		CenteredHorizontally = 1 << 4,
		Right = 1 << 5
	}

	// NSInteger -> UICollectionViewFlowLayout.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UICollectionViewScrollDirection : long {
		Vertical, Horizontal
	}

	// NSInteger -> UICollectionViewFlowLayout.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UILayoutConstraintAxis : long {
		Horizontal, Vertical
	}

	// NSInteger -> UIImage.h
#if __MACCATALYST__
	[Native (ConvertToNative = "UIImageResizingModeExtensions.ToNative", ConvertToManaged = "UIImageResizingModeExtensions.ToManaged")]
#else
	[Native]
#endif
	public enum UIImageResizingMode : long {
		Tile, Stretch
	}

	// NSUInteger -> UICollectionViewLayout.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UICollectionElementCategory : ulong {
		Cell, SupplementaryView, DecorationView
	}

	// that's a convenience enum that maps to UICollectionElementKindSection[Footer|Header] which are NSString
#if !NET
	[NoWatch]
#endif
	public enum UICollectionElementKindSection {
		Header,
		Footer
	}

	// uint64_t -> UIAccessibilityConstants.h
	// note: IMO not really worth changing to ulong for backwards compatibility concerns
	// This is not an enum in ObjC but several fields exported (and we have them too)
	// Unit tests (ViewTest.cs) already ensure we expose the same value as iOS returns
	[Flags]
	public enum UIAccessibilityTrait : long {
		None = 0,
		Button = 1,
		Link = 2,
		Image = 4,
		Selected = 8,
		PlaysSound = 16,
		KeyboardKey = 32,
		StaticText = 64,
		SummaryElement = 128,
		NotEnabled = 256,
		UpdatesFrequently = 512,
		SearchField = 1024,
		StartsMediaSession = 2048,
		Adjustable = 4096,
		AllowsDirectInteraction = 8192,
		CausesPageTurn = 16384,
		Header = 65536,
	}

	// NSInteger -> UIImage.h
	[Native]
	public enum UIImageRenderingMode : long {
		Automatic,
		AlwaysOriginal,
		AlwaysTemplate
	}

	// NSInteger -> UIMotionEffect.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIInterpolatingMotionEffectType : long {
		TiltAlongHorizontalAxis,
		TiltAlongVerticalAxis
	}

	// NSInteger -> UINavigationController.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UINavigationControllerOperation : long {
		None, Push, Pop
	}

	// NSInteger -> UIActivity.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIActivityCategory : long {
		Action, Share
	}

	// NSInteger -> UIAttachmentBehavior.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIAttachmentBehaviorType : long {
		Items, Anchor
	}

	// NSInteger -> UIBarCommon.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIBarPosition : long {
		Any, Bottom, Top, TopAttached, 
	}

	// NSUInteger -> UICollisionBehavior.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UICollisionBehaviorMode : ulong  {
		Items = 1,
		Boundaries = 2,
		Everything = UInt64.MaxValue
	}

	// uint32_t -> UIFontDescriptor.h
	[Flags]
	public enum UIFontDescriptorSymbolicTraits : uint {
		Italic = 1 << 0,
		Bold = 1 << 1,
		Expanded = 1 << 5,
		Condensed = 1 << 6,
		MonoSpace = 1 << 10,
		Vertical = 1 << 11,
		UIOptimized = 1 << 12,
		TightLeading = 1 << 15,
		LooseLeading = 1 << 16,

		ClassMask = 0xF0000000,
		
		ClassUnknown = 0,
		ClassOldStyleSerifs = 1 << 28,
		ClassTransitionalSerifs = 2 << 28,
		ClassModernSerifs = 3 << 28,
		ClassClarendonSerifs = 4 << 28,
		ClassSlabSerifs = 5 << 28,
		ClassFreeformSerifs = 7 << 28,
		ClassSansSerif = 8U << 28,
		ClassOrnamentals = 9U << 28,
		ClassScripts = 10U << 28,
		ClassSymbolic = 12U << 28
	}

	// NSInteger -> UIResponder.h
#if !NET
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UIKeyModifierFlags : long {
		AlphaShift     = 1 << 16,  // This bit indicates CapsLock
		Shift          = 1 << 17,
		Control        = 1 << 18,
		Alternate      = 1 << 19,
		Command        = 1 << 20,
		NumericPad     = 1 << 21,
	}

	// NSInteger -> UIScrollView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIScrollViewKeyboardDismissMode : long {
		None, OnDrag, Interactive
	}

	// NSInteger -> UIWebView.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[Native]
	public enum UIWebPaginationBreakingMode : long {
		Page, Column
	}

	// NSInteger -> UIWebView.h
#if NET
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[Native]
	public enum UIWebPaginationMode : long {
		Unpaginated,
		LeftToRight,
		TopToBottom,
		BottomToTop,
		RightToLeft
	}

	// NSInteger -> UIPushBehavior.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIPushBehaviorMode : long {
		Continuous,
		Instantaneous
	}

	// NSInteger -> UITabBar.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UITabBarItemPositioning : long {
		Automatic,
		Fill,
		Centered
	}

	// NSUInteger -> UIView.h
#if NET
	[SupportedOSPlatform ("ios7.0")]
#else
	[iOS (7,0)]
	[NoWatch]
#endif
	[Native]
	public enum UIViewKeyframeAnimationOptions : ulong {
		LayoutSubviews = UIViewAnimationOptions.LayoutSubviews,
		AllowUserInteraction = UIViewAnimationOptions.AllowUserInteraction,
		BeginFromCurrentState = UIViewAnimationOptions.BeginFromCurrentState,
		Repeat = UIViewAnimationOptions.Repeat,
		Autoreverse = UIViewAnimationOptions.Autoreverse,
		OverrideInheritedDuration = UIViewAnimationOptions.OverrideInheritedDuration,
		OverrideInheritedOptions = UIViewAnimationOptions.OverrideInheritedOptions,

		CalculationModeLinear = 0 << 10,
		CalculationModeDiscrete = 1 << 10,
		CalculationModePaced = 2 << 10,
		CalculationModeCubic = 3 << 10,
		CalculationModeCubicPaced = 4 << 10
	}

	// NSInteger -> UIView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UIViewTintAdjustmentMode : long {
		Automatic,
		Normal,
		Dimmed
	}

	// NSUInteger -> UIView.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UISystemAnimation : ulong {
		Delete
	}

	// NSUInteger -> UIGeometry.h
	[Native]
	public enum UIRectEdge : ulong {
		None = 0,
		Top = 1 << 0,
		Left = 1 << 1,
		Bottom = 1 << 2,
		Right = 1 << 3,
		All = Top | Left | Bottom | Right
	}

	// Xamarin.iOS home-grown define
	public enum  NSTextEffect {
		None,
		LetterPressStyle,

		// An unkonwn value, the real value can be fetched using the WeakTextEffect: Apple added a new effect and the bindings are old.
		UnknownUseWeakEffect
	}

	// NSUInteger -> UISearchBar.h
#if NET
	[SupportedOSPlatform ("ios7.0")]
#else
	[NoWatch]
	[iOS (7,0)]
#endif
	[Native]
	public enum UISearchBarStyle : ulong {
		Default,
		Prominent,
		Minimal
	}

	// NSInteger -> UIInputView.h
#if NET
	[SupportedOSPlatform ("ios7.0")]
#else
	[NoWatch]
	[iOS (7,0)]
#endif
	[Native]
	public enum UIInputViewStyle : long {
		Default,
		Keyboard
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[NoWatch]
	[iOS (8,0)]
#endif
	[Native]
	public enum UIUserInterfaceSizeClass : long {
		Unspecified = 0,
		Compact = 1,
		Regular = 2
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[NoWatch]
	[iOS (8,0)]
#endif
	[Native]
	public enum UIAlertActionStyle : long {
		Default, Cancel, Destructive
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[NoWatch]
	[iOS (8,0)]
#endif
	[Native]
	public enum UIAlertControllerStyle : long {
		ActionSheet,
		Alert
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[NoWatch]
	[iOS (8,0)]
#endif
	[Native]
	public enum UIBlurEffectStyle : long {
		ExtraLight, Light, Dark,
#if NET
		[SupportedOSPlatform ("tvos10.0")]
		[UnsupportedOSPlatform ("ios")]
#else
		[TV (10,0)]
		[NoiOS]
		[NoWatch]
#endif
		ExtraDark,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
#endif
		Regular = 4,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
#endif
		Prominent = 5,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemUltraThinMaterial,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemThinMaterial,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemMaterial,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemThickMaterial,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemChromeMaterial,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemUltraThinMaterialLight,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemThinMaterialLight,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemMaterialLight,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemThickMaterialLight,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemChromeMaterialLight,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemUltraThinMaterialDark,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemThinMaterialDark,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemMaterialDark,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemThickMaterialDark,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[NoTV]
#endif
		SystemChromeMaterialDark,
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[iOS (8,0)]
#endif
	[Native]
	public enum UIPrinterJobTypes : long {
		Unknown = 0,
		Document = 1 << 0,
		Envelope = 1 << 1,
		Label = 1 << 2,
		Photo = 1 << 3,
		Receipt = 1 << 4,
		Roll = 1 << 5,
		LargeFormat = 1 << 6,
		Postcard = 1 << 7
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("ios10.0")]
#if IOS
	[Obsolete ("Starting with ios10.0 se 'UNAuthorizationOptions' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[iOS (8,0)]
	[Deprecated (PlatformName.iOS, 10, 0, message:"Use 'UNAuthorizationOptions' instead.")]
#endif
	[Native]
	[Flags]
	public enum UIUserNotificationType : ulong {
		None       = 0,
		Badge      = 1 << 0,
		Sound      = 1 << 1, 
		Alert      = 1 << 2
	}
	
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("ios10.0")]
#if IOS
	[Obsolete ("Starting with ios10.0 use 'UNNotificationActionOptions' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationActionOptions' instead.")]
#endif
	[Native]
	public enum UIUserNotificationActivationMode : ulong {
		Foreground,
		Background
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("ios10.0")]
#if IOS
	[Obsolete ("Starting with ios10.0 use 'UNNotificationCategory.Actions' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationCategory.Actions' instead.")]
#endif
	[Native]
	public enum UIUserNotificationActionContext : ulong {
		Default,
		Minimal
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("ios11.0")]
#if IOS
	[Obsolete ("Starting with ios11.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[Deprecated (PlatformName.iOS, 11, 0)]
	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
#endif
	[Native]
	public enum UIDocumentMenuOrder : ulong {
		First,
		Last
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("ios14.0")]
#if IOS
	[Obsolete ("Starting with ios14.0 use the designated constructors instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[Deprecated (PlatformName.iOS, 14, 0, message: "Use the designated constructors instead.")]
	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
#endif
	[Native]
	public enum UIDocumentPickerMode : ulong {
		Import,
		Open,
		ExportToService,
		MoveToService
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[iOS (8, 0)]
#endif
	[Native]
	public enum UIAccessibilityNavigationStyle : long {

		Automatic = 0,
		Separate = 1,
		Combined = 2
	}

#if !NET
	[NoWatch]
#endif
	[Native]
	public enum UISplitViewControllerDisplayMode : long {
		Automatic,
#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
#else
		[TV (14,0)]
		[iOS (14,0)]
#endif
		SecondaryOnly,
#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
#else
		[TV (14,0)]
		[iOS (14,0)]
#endif
		OneBesideSecondary,
#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
#else
		[TV (14,0)]
		[iOS (14,0)]
#endif
		OneOverSecondary,
#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
#else
		[TV (14,0)]
		[iOS (14,0)]
#endif
		TwoBesideSecondary,
#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
#else
		[TV (14,0)]
		[iOS (14,0)]
#endif
		TwoOverSecondary,
#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
#else
		[TV (14,0)]
		[iOS (14,0)]
#endif
		TwoDisplaceSecondary,

#if NET
		[UnsupportedOSPlatform ("tvos14.0")]
		[UnsupportedOSPlatform ("ios14.0")]
#if TVOS
		[Obsolete ("Starting with tvos14.0 use 'SecondaryOnly' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios14.0 use 'SecondaryOnly' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'SecondaryOnly' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'SecondaryOnly' instead.")]
#endif
		PrimaryHidden = SecondaryOnly,

#if NET
		[UnsupportedOSPlatform ("tvos14.0")]
		[UnsupportedOSPlatform ("ios14.0")]
#if TVOS
		[Obsolete ("Starting with tvos14.0 use 'OneBesideSecondary' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios14.0 use 'OneBesideSecondary' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'OneBesideSecondary' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'OneBesideSecondary' instead.")]
#endif
		AllVisible = OneBesideSecondary,

#if NET
		[UnsupportedOSPlatform ("tvos14.0")]
		[UnsupportedOSPlatform ("ios14.0")]
#if TVOS
		[Obsolete ("Starting with tvos14.0 use 'OneOverSecondary' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios14.0 use 'OneOverSecondary' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'OneOverSecondary' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'OneOverSecondary' instead.")]
#endif
		PrimaryOverlay = OneOverSecondary,
	}

#if NET
	[UnsupportedOSPlatform ("ios13.0")]
#if IOS
	[Obsolete ("Starting with ios13.0 use 'UIContextualActionStyle' and corresponding APIs instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'UIContextualActionStyle' and corresponding APIs instead.")]
#endif
	[Native]
	public enum UITableViewRowActionStyle : long {
		Default, Destructive = Default, Normal
	}

	// Utility enum for UITransitionContext[To|From]ViewKey
#if !NET
	[NoWatch]
#endif
	public enum UITransitionViewControllerKind {
		ToView, FromView
	}

	// note [Native] since it maps to UIFontWeightConstants fields (CGFloat)
#if NET
	[SupportedOSPlatform ("ios8.2")]
#else
	[iOS (8,2)]
#endif
	public enum UIFontWeight {
		UltraLight,
		Thin,
		Light,
		Regular,
		Medium,
		Semibold,
		Bold,
		Heavy,
		Black,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UIStackViewDistribution : long {
		Fill,
		FillEqually,
		FillProportionally,
		EqualSpacing,
		EqualCentering
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UIStackViewAlignment : long {
		Fill,
		Leading,
		Top = Leading,
		FirstBaseline,
		Center,
		Trailing,
		Bottom = Trailing,
		LastBaseline
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[iOS (9,0)]
#endif
	[Native]
	[Flags]
	public enum NSWritingDirectionFormatType : long {
		Embedding = 0 << 1,
		Override = 1 << 1
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UIPrinterCutterBehavior : long
	{
		NoCut,
		PrinterDefault,
		CutAfterEachPage,
		CutAfterEachCopy,
		CutAfterEachJob
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[UnsupportedOSPlatform ("ios10.0")]
#if IOS
	[Obsolete ("Starting with ios10.0 use 'UNNotificationAction' or 'UNTextInputNotificationAction' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[iOS (9,0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationAction' or 'UNTextInputNotificationAction' instead.")]
#endif
	[Native]
	public enum UIUserNotificationActionBehavior : ulong
	{
		Default,
		TextInput
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UISemanticContentAttribute : long
	{
		Unspecified = 0,
		Playback,
		Spatial,
		ForceLeftToRight,
		ForceRightToLeft
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UIDynamicItemCollisionBoundsType : ulong
	{
		Rectangle,
		Ellipse,
		Path
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UIForceTouchCapability : long {
		Unknown = 0,
		Unavailable = 1,
		Available = 2
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UIPreviewActionStyle : long {
		Default, Selected, Destructive
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UIPressPhase : long {
		Began,
		Changed,
		Stationary,
		Ended,
		Cancelled
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum UIPressType : long {
		UpArrow,
		DownArrow,
		LeftArrow,
		RightArrow,
		Select,
		Menu,
		PlayPause,
#if NET
		[SupportedOSPlatform ("tvos14.3")]
		[UnsupportedOSPlatform ("ios")]
#else
		[TV (14,3)]
		[NoiOS]
#endif
		PageUp = 30,
#if NET
		[SupportedOSPlatform ("tvos14.3")]
		[UnsupportedOSPlatform ("ios")]
#else
		[TV (14,3)]
		[NoiOS]
#endif
		PageDown = 31,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[NoWatch]
	[iOS (9,0)] // introduced in Xcode 7.1 SDK (iOS 9.1 but hidden in 9.0)
#endif
	[Native]
	public enum UITableViewCellFocusStyle : long {
		Default,
		Custom
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[NoWatch]
	[iOS (10,0)]
#endif
	[Native]
	public enum UIDisplayGamut : long
	{
		Unspecified = -1,
		Srgb,
		P3
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[NoWatch]
	[iOS (10,0)]
#endif
	[Native]
	public enum UITraitEnvironmentLayoutDirection : long
	{
		Unspecified = -1,
		LeftToRight = UIUserInterfaceLayoutDirection.LeftToRight,
		RightToLeft = UIUserInterfaceLayoutDirection.RightToLeft
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (10,0)]
	[NoWatch]
	[iOS (12,0)]
#endif
	[Native]
	public enum UIUserInterfaceStyle : long
	{
		Unspecified,
		Light,
		Dark
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[NoWatch]
	[iOS (10,0)]
#endif
	[Native]
	public enum UITextItemInteraction : long
	{
		InvokeDefaultAction,
		PresentActions,
		Preview
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[NoWatch]
	[iOS (10,0)]
#endif
	[Native]
	public enum UIViewAnimatingState : long
	{
		Inactive,
		Active,
		Stopped
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[NoWatch]
	[iOS (10,0)]
#endif
	[Native]
	public enum UIViewAnimatingPosition : long
	{
		End,
		Start,
		Current
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[NoWatch]
	[iOS (10,0)]
#endif
	[Native]
	public enum UITimingCurveType : long
	{
		Builtin,
		Cubic,
		Spring,
		Composed
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (10,0)]
#endif
	[Native]
	public enum UIAccessibilityHearingDeviceEar : ulong {
		None = 0,
		Left = 1 << 1,
		Right = 1 << 2,
		Both = Left | Right
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[NoWatch]
	[iOS (10,0)]
#endif
	[Native]
	public enum UIAccessibilityCustomRotorDirection : long
	{
		Previous,
		Next
	}

	// Xcode 8.2 beta 1 added __TVOS_PROHIBITED but we need to keep it for binary compatibility
#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
#else
	[TV (10,0)]
	[iOS (10,0)]
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum UICloudSharingPermissionOptions : ulong {
		Standard = 0,
		AllowPublic = 1 << 0,
		AllowPrivate = 1 << 1,
		AllowReadOnly = 1 << 2,
		AllowReadWrite = 1 << 3
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
#else
	[iOS (10,0)]
	[TV (10,0)]
	[NoWatch]
#endif
	[Native]
	public enum UITextFieldDidEndEditingReason : long {
		Unknown = -1, // helper value (not in headers)
		Committed,
#if NET
		[SupportedOSPlatform ("tvos10.0")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoiOS]
#endif
		Cancelled
	}

#if NET
	[SupportedOSPlatform ("ios10.3")]
	[SupportedOSPlatform ("tvos10.2")]
#else
	[iOS (10,3)]
	[TV (10,2)]
	[NoWatch]
#endif
	[Native]
	public enum UIScrollViewIndexDisplayMode : long {
		Automatic,
		AlwaysHidden
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[NoWatch]
	[TV (11,0)]
	[iOS (11,0)]
#endif
	[Native]
	public enum UIScrollViewContentInsetAdjustmentBehavior : long
	{
		Automatic,
		ScrollableAxes,
		Never,
		Always
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#else
	[iOS (11,0)]
	[TV (11,0)]
	[Watch (4,0)]
#endif
	[Native]
	public enum UIAccessibilityContainerType : long
	{
		None = 0,
		DataTable,
		List,
		Landmark,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
#else
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
#endif
		SemanticGroup,
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#else
	[NoWatch]
	[iOS (11,0)]
	[TV (11,0)]
#endif
	[Native]
	public enum UITextSmartQuotesType : long
	{
		Default,
		No,
		Yes
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#else
	[NoWatch]
	[iOS (11,0)]
	[TV (11,0)]
#endif
	[Native]
	public enum UITextSmartDashesType : long
	{
		Default,
		No,
		Yes
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#else
	[NoWatch]
	[iOS (11,0)]
	[TV (11,0)]
#endif
	[Native]
	public enum UITextSmartInsertDeleteType : long
	{
		Default,
		No,
		Yes
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#else
	[NoWatch]
	[iOS (11,0)]
	[TV (11,0)]
#endif
	[Native]
	public enum UIAccessibilityCustomSystemRotorType : long
	{
		None = 0,
		Link,
		VisitedLink,
		Heading,
		HeadingLevel1,
		HeadingLevel2,
		HeadingLevel3,
		HeadingLevel4,
		HeadingLevel5,
		HeadingLevel6,
		BoldText,
		ItalicText,
		UnderlineText,
		MisspelledWord,
		Image,
		TextField,
		Table,
		List,
		Landmark
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UIDropOperation : ulong
	{
		Cancel = 0,
		Forbidden = 1,
		Copy = 2,
		Move = 3
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	[Flags]
	public enum UITextDragOptions : long
	{
		None = 0,
		StripTextColorFromPreviews = (1 << 0)
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UITextDropAction : ulong
	{
		Insert = 0,
		ReplaceSelection,
		ReplaceAll
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UITextDropProgressMode : ulong
	{
		System = 0,
		Custom
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UITextDropEditability : ulong
	{
		No = 0,
		Temporary,
		Yes
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UICollectionViewReorderingCadence : long
	{
		Immediate,
		Fast,
		Slow
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UICollectionViewDropIntent : long
	{
		Unspecified,
		InsertAtDestinationIndexPath,
		InsertIntoDestinationIndexPath
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UICollectionViewCellDragState : long
	{
		None,
		Lifting,
		Dragging
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("ios14.0")]
#if IOS
	[Obsolete ("Starting with ios14.0 use 'PHPicker' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UIImagePickerControllerImageUrlExportPreset : long
	{
		Compatible = 0,
		Current
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UIContextualActionStyle : long
	{
		Normal,
		Destructive
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UITableViewCellDragState : long
	{
		None,
		Lifting,
		Dragging
	}
	
#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[NoWatch]
	[TV (11,0)]
	[iOS (11,0)]
#endif
	[Native]
	public enum UITableViewSeparatorInsetReference : long
	{
		CellEdges,
		AutomaticInsets
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UITableViewDropIntent : long
	{
		Unspecified,
		InsertAtDestinationIndexPath,
		InsertIntoDestinationIndexPath,
		Automatic
	}
	
#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[NoWatch]
	[TV (11,0)]
	[iOS (11,0)]
#endif
	[Native]
	public enum UISplitViewControllerPrimaryEdge : long
	{
		Leading,
		Trailing
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UIDropSessionProgressIndicatorStyle : ulong
	{
		None,
		Default
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UISpringLoadedInteractionEffectState : long
	{
		Inactive,
		Possible,
		Activating,
		Activated
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UIDocumentBrowserImportMode : ulong
	{
		None,
		Copy,
		Move
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UIDocumentBrowserUserInterfaceStyle : ulong
	{
		White = 0,
		Light,
		Dark
	}
	
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	[Flags]
	public enum UIDocumentBrowserActionAvailability : long
	{
		Menu = 1,
		NavigationBar = 1 << 1
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UITextDropPerformer : ulong
	{
		View = 0,
		Delegate,
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#else
	[NoWatch]
	[iOS (11,0)]
	[TV (11,0)]
#endif
	[Native]
	public enum UINavigationItemLargeTitleDisplayMode : long
	{
		Automatic,
		Always,
		Never,
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#else
	[NoWatch]
	[iOS (11,0)]
	[TV (11,0)]
#endif
	[Native]
	public enum UICollectionViewFlowLayoutSectionInsetReference : long
	{
		ContentInset,
		SafeArea,
		LayoutMargins,
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (11,0)]
#endif
	[Native]
	public enum UIPreferredPresentationStyle : long
	{
		Unspecified = 0,
		Inline,
		Attachment,
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("macos")]
#else
	[NoWatch]
	[NoTV]
	[NoMac]
	[iOS (11,0)]
#endif
	[Native]
	[ErrorDomain ("UIDocumentBrowserErrorDomain")]
	public enum UIDocumentBrowserErrorCode : long
	{
		Generic = 1,
		NoLocationAvailable = 2,
	}
	
#if NET
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("tvos12.0")]
#else
	[iOS (12,0)]
	[TV (12,0)]
	[NoWatch]
#endif
	[Native]
	public enum UIGraphicsImageRendererFormatRange : long
	{
		Unspecified = -1,
		Automatic = 0,
		Extended,
		Standard,
	}

#if NET
	[SupportedOSPlatform ("ios12.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[iOS (12,0)]
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum UIPrintErrorCode : long
	{
		NotAvailableError = 1,
		NoContentError,
		UnknownImageFormatError,
		JobFailedError
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[NoWatch]
#endif
	[ErrorDomain ("UISceneErrorDomain")]
	[Native]
	public enum UISceneErrorCode : long
	{
		MultipleScenesNotSupported,
		RequestDenied,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[Watch (6,0)]
	[TV (13,0)]
	[iOS (13,0)]
#endif
	[Native]
	public enum UIImageSymbolScale : long
	{
		Default = -1,
		Unspecified = 0,
		Small = 1,
		Medium,
		Large,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[Watch (6,0)]
	[TV (13,0)]
	[iOS (13,0)]
#endif
	[Native]
	public enum UIImageSymbolWeight : long
	{
		Unspecified = 0,
		UltraLight = 1,
		Thin,
		Light,
		Regular,
		Medium,
		Semibold,
		Bold,
		Heavy,
		Black,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[NoWatch]
#endif
	[Native]
	public enum UISceneActivationState : long {
		Unattached = -1,
		ForegroundActive,
		ForegroundInactive,
		Background,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[NoWatch]
#endif
	[Native]
	public enum UIMenuElementState : long {
		Off,
		On,
		Mixed,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[NoWatch]
#endif
	[Native]
	public enum UIMenuElementAttributes : ulong {
		Disabled = 1uL << 0,
		Destructive = 1uL << 1,
		Hidden = 1uL << 2,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[NoWatch]
#endif
	[Flags]
	[Native]
	public enum UIMenuOptions : ulong {
		DisplayInline = 1uL << 0,
		Destructive = 1uL << 1,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[NoWatch]
		[MacCatalyst (15,0)]
#endif
		SingleSelection = 1uL << 5,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13, 0)]
#endif
	[Native]
	public enum UIContextMenuInteractionCommitStyle : long {
		Dismiss = 0,
		Pop,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[NoWatch]
#endif
	public enum UIWindowSceneSessionRole {
		[Field ("UIWindowSceneSessionRoleApplication")]
		Application,

		[Field ("UIWindowSceneSessionRoleExternalDisplay")]
		ExternalDisplay,

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
		[NoWatch]
#endif
#if HAS_CARPLAY
		[Field ("CPTemplateApplicationSceneSessionRoleApplication", "CarPlay")]
#endif
		CarTemplateApplication,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[NoWatch]
#endif
	public enum UIMenuIdentifier {
		[DefaultEnumValue]
		[Field (null)]
		None,
		[Field ("UIMenuApplication")]
		Application,
		[Field ("UIMenuFile")]
		File,
		[Field ("UIMenuEdit")]
		Edit,
		[Field ("UIMenuView")]
		View,
		[Field ("UIMenuWindow")]
		Window,
		[Field ("UIMenuHelp")]
		Help,
		[Field ("UIMenuAbout")]
		About,
		[Field ("UIMenuPreferences")]
		Preferences,
		[Field ("UIMenuServices")]
		Services,
		[Field ("UIMenuHide")]
		Hide,
		[Field ("UIMenuQuit")]
		Quit,
		[Field ("UIMenuNewScene")]
		NewScene,
		[Field ("UIMenuClose")]
		Close,
		[Field ("UIMenuPrint")]
		Print,
		[Field ("UIMenuUndoRedo")]
		UndoRedo,
		[Field ("UIMenuStandardEdit")]
		StandardEdit,
		[Field ("UIMenuFind")]
		Find,
		[Field ("UIMenuReplace")]
		Replace,
		[Field ("UIMenuShare")]
		Share,
		[Field ("UIMenuTextStyle")]
		TextStyle,
		[Field ("UIMenuSpelling")]
		Spelling,
		[Field ("UIMenuSpellingPanel")]
		SpellingPanel,
		[Field ("UIMenuSpellingOptions")]
		SpellingOptions,
		[Field ("UIMenuSubstitutions")]
		Substitutions,
		[Field ("UIMenuSubstitutionsPanel")]
		SubstitutionsPanel,
		[Field ("UIMenuSubstitutionOptions")]
		SubstitutionOptions,
		[Field ("UIMenuTransformations")]
		Transformations,
		[Field ("UIMenuSpeech")]
		Speech,
		[Field ("UIMenuLookup")]
		Lookup,
		[Field ("UIMenuLearn")]
		Learn,
		[Field ("UIMenuFormat")]
		Format,
		[Field ("UIMenuFont")]
		Font,
		[Field ("UIMenuTextSize")]
		TextSize,
		[Field ("UIMenuTextColor")]
		TextColor,
		[Field ("UIMenuTextStylePasteboard")]
		TextStylePasteboard,
		[Field ("UIMenuText")]
		Text,
		[Field ("UIMenuWritingDirection")]
		WritingDirection,
		[Field ("UIMenuAlignment")]
		Alignment,
		[Field ("UIMenuToolbar")]
		Toolbar,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
#endif
		[Field ("UIMenuSidebar")]
		Sidebar,
		[Field ("UIMenuFullscreen")]
		Fullscreen,
		[Field ("UIMenuMinimizeAndZoom")]
		MinimizeAndZoom,
		[Field ("UIMenuBringAllToFront")]
		BringAllToFront,
		[Field ("UIMenuRoot")]
		Root,

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,0)]
		[TV (14,0)]
		[MacCatalyst (14,0)]
#endif
		[Field ("UIMenuOpenRecent")]
		OpenRecent,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[Watch (6,0)]
#endif
	public enum UIAccessibilityTextualContext {
		[Field ("UIAccessibilityTextualContextWordProcessing")]
		WordProcessing,
		[Field ("UIAccessibilityTextualContextNarrative")]
		Narrative,
		[Field ("UIAccessibilityTextualContextMessaging")]
		Messaging,
		[Field ("UIAccessibilityTextualContextSpreadsheet")]
		Spreadsheet,
		[Field ("UIAccessibilityTextualContextFileSystem")]
		FileSystem,
		[Field ("UIAccessibilityTextualContextSourceCode")]
		SourceCode,
		[Field ("UIAccessibilityTextualContextConsole")]
		Console,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[NoWatch]
	[TV (13,0)]
	[iOS (13,0)]
#endif
	[Native]
	public enum UICollectionLayoutSectionOrthogonalScrollingBehavior : long {
		None,
		Continuous,
		ContinuousGroupLeadingBoundary,
		Paging,
		GroupPaging,
		GroupPagingCentered,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[NoWatch]
	[iOS (13,0)]
#endif
	[Native]
	public enum UIAccessibilityContrast : long {
		Unspecified = -1,
		Normal,
		High,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[NoWatch]
	[TV (13,0)]
	[iOS (13,0)]
#endif
	[Native]
	public enum UILegibilityWeight : long {
		Unspecified = -1,
		Regular,
		Bold,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13,0)]
#endif
	[Native]
	public enum UIUserInterfaceLevel : long {
		Unspecified = -1,
		Base,
		Elevated,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[NoWatch]
	[TV (13,0)]
	[iOS (13,0)]
#endif
	[Native]
	public enum UIEditingInteractionConfiguration : long {
		None = 0,
		Default = 1,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[iOS (13,0)]
#endif
	[Native]
	public enum UISplitViewControllerBackgroundStyle : long {
		None,
		Sidebar,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[NoWatch]
	[TV (13,0)]
	[iOS (13,0)]
#endif
	[Native]
	public enum UITabBarItemAppearanceStyle : long {
		Stacked,
		Inline,
		CompactInline,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[NoWatch]
	[TV (13,0)]
	[iOS (13,0)]
#endif
	[Native]
	public enum UITextAlternativeStyle : long {
		None,
		LowConfidence,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[NoWatch]
	[TV (13,0)]
	[iOS (13,0)]
#endif
	[Native]
	public enum UITextInteractionMode : long {
		Editable,
		NonEditable,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13,0)]
#endif
	[Native]
	public enum UIVibrancyEffectStyle : long {
		Label,
		SecondaryLabel,
		TertiaryLabel,
		QuaternaryLabel,
		Fill,
		SecondaryFill,
		TertiaryFill,
		Separator,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[NoWatch]
	[TV (13,0)]
	[iOS (13,0)]
#endif
	[Native]
	public enum UIWindowSceneDismissalAnimation : long {
		Standard = 1,
		Commit = 2,
		Decline = 3,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13,0)]
#endif
	public enum UIActivityItemsConfigurationInteraction {
		[Field ("UIActivityItemsConfigurationInteractionShare")]
		Share,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13,0)]
#endif
	public enum UIActivityItemsConfigurationPreviewIntent {
		[Field ("UIActivityItemsConfigurationPreviewIntentFullSize")]
		FullSize,
		[Field ("UIActivityItemsConfigurationPreviewIntentThumbnail")]
		Thumbnail,
	}

#if NET
	[SupportedOSPlatform ("ios13.4")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13,4)]
#endif
	[Native]
	public enum UIDatePickerStyle : long {
		Automatic,
		Wheels,
		Compact,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (14,0)]
#endif
		Inline,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.4")]
	[SupportedOSPlatform ("ios13.4")]
	[SupportedOSPlatform ("tvos13.4")]
#else
	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[iOS (13,4)]
	[NoWatch]
	[TV (13,4)]
#endif
	[Native]
	public enum UIKeyboardHidUsage : long {
		KeyboardErrorRollOver = 1,
		KeyboardPostFail = 2,
		KeyboardErrorUndefined = 3,
		KeyboardA = 4,
		KeyboardB = 5,
		KeyboardC = 6,
		KeyboardD = 7,
		KeyboardE = 8,
		KeyboardF = 9,
		KeyboardG = 10,
		KeyboardH = 11,
		KeyboardI = 12,
		KeyboardJ = 13,
		KeyboardK = 14,
		KeyboardL = 15,
		KeyboardM = 16,
		KeyboardN = 17,
		KeyboardO = 18,
		KeyboardP = 19,
		KeyboardQ = 20,
		KeyboardR = 21,
		KeyboardS = 22,
		KeyboardT = 23,
		KeyboardU = 24,
		KeyboardV = 25,
		KeyboardW = 26,
		KeyboardX = 27,
		KeyboardY = 28,
		KeyboardZ = 29,
		Keyboard1 = 30,
		Keyboard2 = 31,
		Keyboard3 = 32,
		Keyboard4 = 33,
		Keyboard5 = 34,
		Keyboard6 = 35,
		Keyboard7 = 36,
		Keyboard8 = 37,
		Keyboard9 = 38,
		Keyboard0 = 39,
		KeyboardReturnOrEnter = 40,
		KeyboardEscape = 41,
		KeyboardDeleteOrBackspace = 42,
		KeyboardTab = 43,
		KeyboardSpacebar = 44,
		KeyboardHyphen = 45,
		KeyboardEqualSign = 46,
		KeyboardOpenBracket = 47,
		KeyboardCloseBracket = 48,
		KeyboardBackslash = 49,
		KeyboardNonUSPound = 50,
		KeyboardSemicolon = 51,
		KeyboardQuote = 52,
		KeyboardGraveAccentAndTilde = 53,
		KeyboardComma = 54,
		KeyboardPeriod = 55,
		KeyboardSlash = 56,
		KeyboardCapsLock = 57,
		KeyboardF1 = 58,
		KeyboardF2 = 59,
		KeyboardF3 = 60,
		KeyboardF4 = 61,
		KeyboardF5 = 62,
		KeyboardF6 = 63,
		KeyboardF7 = 64,
		KeyboardF8 = 65,
		KeyboardF9 = 66,
		KeyboardF10 = 67,
		KeyboardF11 = 68,
		KeyboardF12 = 69,
		KeyboardPrintScreen = 70,
		KeyboardScrollLock = 71,
		KeyboardPause = 72,
		KeyboardInsert = 73,
		KeyboardHome = 74,
		KeyboardPageUp = 75,
		KeyboardDeleteForward = 76,
		KeyboardEnd = 77,
		KeyboardPageDown = 78,
		KeyboardRightArrow = 79,
		KeyboardLeftArrow = 80,
		KeyboardDownArrow = 81,
		KeyboardUpArrow = 82,
		KeypadNumLock = 83,
		KeypadSlash = 84,
		KeypadAsterisk = 85,
		KeypadHyphen = 86,
		KeypadPlus = 87,
		KeypadEnter = 88,
		Keypad1 = 89,
		Keypad2 = 90,
		Keypad3 = 91,
		Keypad4 = 92,
		Keypad5 = 93,
		Keypad6 = 94,
		Keypad7 = 95,
		Keypad8 = 96,
		Keypad9 = 97,
		Keypad0 = 98,
		KeypadPeriod = 99,
		KeyboardNonUSBackslash = 100,
		KeyboardApplication = 101,
		KeyboardPower = 102,
		KeypadEqualSign = 103,
		KeyboardF13 = 104,
		KeyboardF14 = 105,
		KeyboardF15 = 106,
		KeyboardF16 = 107,
		KeyboardF17 = 108,
		KeyboardF18 = 109,
		KeyboardF19 = 110,
		KeyboardF20 = 111,
		KeyboardF21 = 112,
		KeyboardF22 = 113,
		KeyboardF23 = 114,
		KeyboardF24 = 115,
		KeyboardExecute = 116,
		KeyboardHelp = 117,
		KeyboardMenu = 118,
		KeyboardSelect = 119,
		KeyboardStop = 120,
		KeyboardAgain = 121,
		KeyboardUndo = 122,
		KeyboardCut = 123,
		KeyboardCopy = 124,
		KeyboardPaste = 125,
		KeyboardFind = 126,
		KeyboardMute = 127,
		KeyboardVolumeUp = 128,
		KeyboardVolumeDown = 129,
		KeyboardLockingCapsLock = 130,
		KeyboardLockingNumLock = 131,
		KeyboardLockingScrollLock = 132,
		KeypadComma = 133,
		KeypadEqualSignAS400 = 134,
		KeyboardInternational1 = 135,
		KeyboardInternational2 = 136,
		KeyboardInternational3 = 137,
		KeyboardInternational4 = 138,
		KeyboardInternational5 = 139,
		KeyboardInternational6 = 140,
		KeyboardInternational7 = 141,
		KeyboardInternational8 = 142,
		KeyboardInternational9 = 143,
		KeyboardLang1 = 144,
		KeyboardLang2 = 145,
		KeyboardLang3 = 146,
		KeyboardLang4 = 147,
		KeyboardLang5 = 148,
		KeyboardLang6 = 149,
		KeyboardLang7 = 150,
		KeyboardLang8 = 151,
		KeyboardLang9 = 152,
		KeyboardAlternateErase = 153,
		KeyboardSysReqOrAttention = 154,
		KeyboardCancel = 155,
		KeyboardClear = 156,
		KeyboardPrior = 157,
		KeyboardReturn = 158,
		KeyboardSeparator = 159,
		KeyboardOut = 160,
		KeyboardOper = 161,
		KeyboardClearOrAgain = 162,
		KeyboardCrSelOrProps = 163,
		KeyboardExSel = 164,
		KeyboardLeftControl = 224,
		KeyboardLeftShift = 225,
		KeyboardLeftAlt = 226,
		KeyboardLeftGui = 227,
		KeyboardRightControl = 228,
		KeyboardRightShift = 229,
		KeyboardRightAlt = 230,
		KeyboardRightGui = 231,
		KeyboardReserved = 65535,
		KeyboardHangul = KeyboardLang1,
		KeyboardHanja = KeyboardLang2,
		KeyboardKanaSwitch = KeyboardLang1,
		KeyboardAlphanumericSwitch = KeyboardLang2,
		KeyboardKatakana = KeyboardLang3,
		KeyboardHiragana = KeyboardLang4,
		KeyboardZenkakuHankakuKanji = KeyboardLang5,
	}

#if NET
	[SupportedOSPlatform ("ios13.4")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13,4)]
#endif
	[Flags]
	[Native]
	public enum UIEventButtonMask : ulong {
		Primary = 1L << 0,
		Secondary = 1L << 1,
	}

#if NET
	[SupportedOSPlatform ("tvos13.4")]
	[SupportedOSPlatform ("ios13.4")]
#else
	[TV (13,4)]
	[NoWatch]
	[iOS (13,4)]
#endif
	[Flags]
	[Native]
	public enum UIAxis : ulong {
		Neither = 0uL,
		Horizontal = 1uL << 0,
		Vertical = 1uL << 1,
		Both = (Horizontal | Vertical),
	}

#if NET
	[SupportedOSPlatform ("ios13.4")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13,4)]
#endif
	[Native]
	public enum UIScrollType : long {
		Discrete,
		Continuous,
	}

#if NET
	[SupportedOSPlatform ("ios13.4")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13,4)]
#endif
	[Flags]
	[Native]
	public enum UIScrollTypeMask : ulong {
		Discrete = 1L << 0,
		Continuous = 1L << 1,
		All = Discrete | Continuous,
	}

#if NET
	[SupportedOSPlatform ("ios13.4")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (13,4)]
#endif
	[Native]
	public enum UIPointerEffectTintMode : long {
		None = 0,
		Overlay,
		Underlay,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UIButtonRole : long {
		Normal,
		Primary,
		Cancel,
		Destructive,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UICellAccessoryDisplayedState : long {
		Always,
		WhenEditing,
		WhenNotEditing,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (14,0)]
#endif
	[Native]
	public enum UICellAccessoryOutlineDisclosureStyle : long {
		Automatic,
		Header,
		Cell,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UICellAccessoryPlacement : long {
		Leading,
		Trailing,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (14,0)]
#endif
	[Native]
	public enum UICellConfigurationDragState : long {
		None,
		Lifting,
		Dragging,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (14,0)]
#endif
	[Native]
	public enum UICellConfigurationDropState : long {
		None,
		NotTargeted,
		Targeted,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UICollectionLayoutListAppearance : long {
		Plain,
		Grouped,
#if !TVOS
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		InsetGrouped,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		Sidebar,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
#endif
		SidebarPlain,
#endif
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UICollectionLayoutListHeaderMode : long {
		None,
		Supplementary,
		FirstItemInSection,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UIContentInsetsReference : long {
		Automatic,
		None,
		SafeArea,
		LayoutMargins,
		ReadableContent,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (14,0)]
#endif
	[Native]
	public enum UIContextMenuInteractionAppearance : long {
		Unknown = 0,
		Rich,
		Compact,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UIUserInterfaceActiveAppearance : long {
		Unspecified = -1,
		Inactive,
		Active,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UIListContentTextAlignment : long {
		Natural,
		Center,
		Justified,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UIPageControlInteractionState : long {
		None = 0,
		Discrete = 1,
		Continuous = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UIPageControlBackgroundStyle : long {
		Automatic = 0,
		Prominent = 1,
		Minimal = 2,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (14,0)]
	[TV (14,0)]
	[NoWatch]
	[MacCatalyst (14,0)]
#endif
	public enum UIPasteboardDetectionPattern {
		[Field ("UIPasteboardDetectionPatternProbableWebURL")]
		ProbableWebUrl,
		[Field ("UIPasteboardDetectionPatternProbableWebSearch")]
		ProbableWebSearch,
		[Field ("UIPasteboardDetectionPatternNumber")]
		Number,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
#endif
		[Field ("UIPasteboardDetectionPatternLink")]
		Link,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
#endif
		[Field ("UIPasteboardDetectionPatternPhoneNumber")]
		PhoneNumber,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
#endif
		[Field ("UIPasteboardDetectionPatternEmailAddress")]
		EmailAddress,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
#endif
		[Field ("UIPasteboardDetectionPatternPostalAddress")]
		PostalAddress,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
#endif
		[Field ("UIPasteboardDetectionPatternCalendarEvent")]
		CalendarEvent,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
#endif
		[Field ("UIPasteboardDetectionPatternShipmentTrackingNumber")]
		ShipmentTrackingNumber,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
#endif
		[Field ("UIPasteboardDetectionPatternFlightNumber")]
		FlightNumber,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
#endif
		[Field ("UIPasteboardDetectionPatternMoneyAmount")]
		MoneyAmount,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
#else
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[NoiOS]
#endif
	[Native]
	public enum UISceneCollectionJoinBehavior : long {
		Automatic,
		Preferred,
		Disallowed,
		PreferredWithoutActivating,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UISplitViewControllerStyle : long {
		Unspecified,
		DoubleColumn,
		TripleColumn,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UISplitViewControllerColumn : long {
		Primary,
		Supplementary,
		Secondary,
		Compact,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UISplitViewControllerSplitBehavior : long {
		Automatic,
		Tile,
		Overlay,
		Displace,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (14,0)]
#endif
	[Native]
	public enum UISwitchStyle : long {
		Automatic = 0,
		Checkbox,
		Sliding,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[TV (14,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum UICollectionLayoutListFooterMode : long {
		None,
		Supplementary,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
#else
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[NoiOS]
#endif
	[Native]
	public enum UITitlebarSeparatorStyle : long {
		Automatic,
		None,
		Line,
		Shadow,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (14,0)]
#endif
	[Native]
	public enum UINavigationItemBackButtonDisplayMode : long {
		Default = 0,
		Generic = 1,
		Minimal = 2,
	}

	// NSInteger -> UIGuidedAccessRestrictions.h
#if NET
	[SupportedOSPlatform ("ios7.0")]
#else
	[NoWatch]
	[iOS (7,0)]
#endif
	[Native]
	public enum UIGuidedAccessRestrictionState : long {
		Allow,
		Deny,
	}

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[TV (15,0)]
	[iOS (15,0)]
	[NoWatch]
	[MacCatalyst (15,0)]
#endif
	public enum UIActionIdentifier {
		[DefaultEnumValue]
		[Field (null)]
		None = -1,

		[Field ("UIActionPaste")]
		Paste,

		[Field ("UIActionPasteAndMatchStyle")]
		PasteAndMatchStyle,

		[Field ("UIActionPasteAndGo")]
		PasteAndGo,

		[Field ("UIActionPasteAndSearch")]
		PasteAndSearch,
	}

#if NET
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum UIBandSelectionInteractionState : long {
		Possible = 0,
		Began,
		Selecting,
		Ended,
	}

#if NET
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum UIBehavioralStyle : ulong {
		Automatic = 0,
		Pad,
		Mac,
	}

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[TV (15,0)]
	[NoWatch]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum UIButtonConfigurationSize : long {
		Medium,
		Small,
		Mini,
		Large,
	}

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[TV (15,0)]
	[NoWatch]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum UIButtonConfigurationTitleAlignment : long {
		Automatic,
		Leading,
		Center,
		Trailing,
	}

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[TV (15,0)]
	[NoWatch]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum UIButtonConfigurationCornerStyle : long {
		Fixed = -1,
		Dynamic,
		Small,
		Medium,
		Large,
		Capsule,
	}

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[TV (15,0)]
	[NoWatch]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum UIButtonConfigurationMacIdiomStyle : long {
		Automatic,
		Bordered,
		Borderless,
		BorderlessTinted,
	}

#if NET
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV]
	[NoWatch]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum UIFocusGroupPriority : long {
		Ignored = 0,
		PreviouslyFocused = 1000,
		Prioritized = 2000,
		CurrentlyFocused = Int64.MaxValue,
	}

#if NET
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum UIFocusHaloEffectPosition : long {
		Automatic = 0,
		Outside,
		Inside,
	}

#if NET
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	public enum UISheetPresentationControllerDetentIdentifier {
		[DefaultEnumValue]
		[Field (null)]
		Unknown = -1,

		[Field ("UISheetPresentationControllerDetentIdentifierMedium")]
		Medium,

		[Field ("UISheetPresentationControllerDetentIdentifierLarge")]
		Large,
	}

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[NoWatch]
	[TV (15,0)]
	[iOS (15,0)]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum UIWindowScenePresentationStyle : ulong {
		Automatic,
		Standard,
		Prominent,
	}
}
