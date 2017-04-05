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
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.UIKit {
	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoTV][NoWatch]
	public enum UIImagePickerControllerQualityType : nint {
		High,
		Medium,
		Low,

		[iOS (4,0)]
		At640x480,

		[iOS (5,0)]
		At1280x720,

		[iOS (5,0)]
		At960x540
	}

	// NSInteger -> UIActivityIndicatorView.h
	[Native]
	[NoWatch]
	public enum UIActivityIndicatorViewStyle : nint {
		WhiteLarge,
		White,
		Gray
	}

	// NSInteger -> UIAlertView.h
	[Native]
	[NoTV][NoWatch]
	[iOS (5,0)]
	public enum UIAlertViewStyle : nint {
		Default,
		SecureTextInput,
		PlainTextInput,
		LoginAndPasswordInput
	}

	// NSInteger -> UIBarButtonItem.h
	[Native]
	[NoWatch]
	public enum UIBarButtonItemStyle : nint {
		Plain,

		[Availability (Deprecated = Platform.iOS_8_0, Message = "Use UIBarButtonItemStyle.Plain when the minimum deployment target is iOS 7")]
		Bordered,
			
		Done,
	}

	// NSInteger -> UIBarButtonItem.h
	[Native]
	[NoWatch]
	public enum UIBarButtonSystemItem : nint {
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

		[iOS (4,0)]
		PageCurl
	} 

	// NSUInteger -> UIControl.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UIControlEvent : nuint {
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
	[Native]
	[NoWatch]
	public enum UIEventType : nint {
		Touches,
		Motion,
		RemoteControl,
		[iOS (9,0)]
		Presses
	}

	// NSInteger -> UIEvent.h
	[Native]
	[NoWatch]
	public enum UIEventSubtype : nint {
		None,
		MotionShake,

		[iOS (4,0)]
		RemoteControlPlay                 = 100,
		[iOS (4,0)]
		RemoteControlPause                = 101,
		[iOS (4,0)]
		RemoteControlStop                 = 102,
		[iOS (4,0)]
		RemoteControlTogglePlayPause      = 103,
		[iOS (4,0)]
		RemoteControlNextTrack            = 104,
		[iOS (4,0)]
		RemoteControlPreviousTrack        = 105,
		[iOS (4,0)]
		RemoteControlBeginSeekingBackward = 106,
		[iOS (4,0)]
		RemoteControlEndSeekingBackward   = 107,
		[iOS (4,0)]
		RemoteControlBeginSeekingForward  = 108,
		[iOS (4,0)]
		RemoteControlEndSeekingForward    = 109,
	}			
	
	// NSInteger -> UIControl.h
	[Native]
	[NoWatch]
	public enum UIControlContentVerticalAlignment : nint {
		Center  = 0,
		Top     = 1,
		Bottom  = 2,
		Fill    = 3,
	}

	// NSInteger -> UIControl.h
	[Native]
	[NoWatch]
	public enum UIControlContentHorizontalAlignment : nint {
		Center = 0,
		Left   = 1,
		Right  = 2,
		Fill   = 3,
	}

	// NSUInteger -> UIControl.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UIControlState : nuint {
		Normal       = 0,
		Highlighted  = 1 << 0,
		Disabled     = 1 << 1,
		Selected     = 1 << 2,
		[iOS (9,0)]
		Focused      = 1 << 3,
		Application  = 0x00FF0000,
		Reserved     = 0xFF000000
	}

	// NSInteger -> UIImage.h
	[Native]
	public enum UIImageOrientation : nint {
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
	[Native]
	[Flags]
	[NoWatch]
	public enum UIViewAutoresizing : nuint_compat_int {
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
	[Native]
	[NoWatch]
	public enum UIViewAnimationCurve : nint {
		EaseInOut,
		EaseIn,
		EaseOut,
		Linear
	}

	// NSInteger -> UIView.h
	[Native]
	[NoWatch]
	public enum UIViewContentMode : nint {
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
	[Native]
	[NoWatch]
	public enum UIViewAnimationTransition : nint {
		None,
		FlipFromLeft,
		FlipFromRight,
		CurlUp,
		CurlDown,
	}

	// NSInteger -> UIBarCommon.h
	[Native]
	[NoWatch]
	[iOS (5,0)]
	public enum UIBarMetrics : nint {
		Default,
		Compact,
		DefaultPrompt = 101,
		CompactPrompt,

		[Availability (Introduced = Platform.iOS_5_0, Deprecated = Platform.iOS_8_0, Message = "Use UIBarMetrics.Compat instead")]
		LandscapePhone = Compact,

		[Availability (Introduced = Platform.iOS_7_0, Deprecated = Platform.iOS_8_0, Message = "Use UIBarMetrics.CompactPrompt instead")]
		LandscapePhonePrompt = CompactPrompt
	}

	// NSInteger -> UIButton.h
	[Native]
	[NoWatch]
	public enum UIButtonType : nint {
		Custom,
		RoundedRect,
		DetailDisclosure,
		InfoLight,
		InfoDark,
		ContactAdd,
		System = RoundedRect
	}

	// NSInteger -> UIStringDrawing.h
	[Native]
	// note: __TVOS_PROHIBITED -> because it uses NSLineBreakMode (but we need this because we don't expose the later)
	public enum UILineBreakMode : nint {
		WordWrap = 0,
		CharacterWrap,
		Clip,          
		HeadTruncation,
		TailTruncation,
		MiddleTruncation,
	}

	// NSInteger -> UIStringDrawing.h
	[Native]
	[NoWatch]
	public enum UIBaselineAdjustment : nint {
		AlignBaselines = 0,
		AlignCenters,
		None,
	}

	// NSInteger -> UIDatePicker.h
	[Native]
	[NoTV][NoWatch]
	public enum UIDatePickerMode : nint {
		Time,         
		Date,         
		DateAndTime,  
		CountDownTimer 
	}

	// NSInteger -> UIDevice.h
	[Native]
	[NoTV][NoWatch]
	public enum UIDeviceOrientation : nint {
		Unknown,
		Portrait,
		PortraitUpsideDown,
		LandscapeLeft,     
		LandscapeRight,    
		FaceUp,            
		FaceDown
	}

	// NSInteger -> UIDevice.h
	[Native]
	[NoTV][NoWatch]
	public enum UIDeviceBatteryState : nint {
		Unknown,
		Unplugged,
		Charging, 
		Full,     
	}

	// NSInteger -> UIDocument.h
	[Native]
	[NoTV][NoWatch]
	[iOS (5,0)]
	public enum UIDocumentChangeKind : nint {
		Done, Undone, Redone, Cleared
	}

	// NSInteger -> UIDocument.h
	[Native]
	[NoTV][NoWatch]
	[iOS (5,0)]
	public enum UIDocumentSaveOperation : nint {
		ForCreating, ForOverwriting
	}

	// NSUInteger -> UIDocument.h
	[Native]
	[Flags]
	[NoTV][NoWatch]
	[iOS (5,0)]
	public enum UIDocumentState : nuint_compat_int {
		Normal = 0,
		Closed = 1 << 0,
		InConflict = 1 << 1,
		SavingError = 1 << 2,
		EditingDisabled = 1 << 3,
		ProgressAvailable = 1 << 4
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch][NoTV]
	public enum UIImagePickerControllerSourceType : nint {
		PhotoLibrary,
		Camera,
		SavedPhotosAlbum
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch][NoTV]
	public enum UIImagePickerControllerCameraCaptureMode : nint {
		Photo, Video
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch][NoTV]
	public enum UIImagePickerControllerCameraDevice : nint {
		Rear,
		Front
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch][NoTV]
	public enum UIImagePickerControllerCameraFlashMode : nint {
		Off = -1, Auto = 0, On = 1
	}

	// NSInteger -> UIInterface.h
	[Native]
	[NoWatch][NoTV]
	public enum UIBarStyle : nint {
		Default,
		Black,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
		[Availability (Deprecated = Platform.iOS_5_1, Message = "Use UIBarStyle.Black")]
		BlackOpaque      = 1,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
		[Availability (Deprecated = Platform.iOS_5_1, Message = "Use UIBarStyle.Black and set the translucency property to true")]
		BlackTranslucent = 2,
	}

	// NSInteger -> UIProgressView.h
	[Native]
	[NoWatch]
	public enum UIProgressViewStyle : nint {
		Default,
		[NoTV]
		Bar,
	}
	
	// NSInteger -> UIScrollView.h
	[Native]
	[NoWatch]
	public enum UIScrollViewIndicatorStyle : nint {
		Default,
		Black,
		White
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UITextAutocapitalizationType : nint {
		None,
		Words,
		Sentences,
		AllCharacters,
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UITextAutocorrectionType : nint {
		Default,
		No,
		Yes,
	}
	
	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UIKeyboardType : nint {
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
		[iOS (10, 0)]
		AsciiCapableNumberPad
	} 

	// NSInteger -> UISegmentedControl.h
	[Native]
	[NoTV][NoWatch]
	[Availability (Deprecated = Platform.iOS_7_0, Message = "Deprecated in iOS 7, this no longer has any effect")]
	public enum UISegmentedControlStyle : nint {
		Plain,
		Bordered,
		Bar,

		[iOS (4,0)]
		Bezeled
	}

	// NSInteger -> UITabBarItem.h
	[Native]
	[NoWatch]
	public enum UITabBarSystemItem : nint {
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
	[Native]
	[NoWatch]
	public enum UITableViewStyle : nint {
		Plain,
		Grouped
	}

	// NSInteger -> UITableView.h
	[Native]
	[NoWatch]
	public enum UITableViewScrollPosition : nint {
		None,        
		Top,    
		Middle,   
		Bottom
	}
	
	// NSInteger -> UITableView.h
	[Native]
	[NoWatch]
	public enum UITableViewRowAnimation : nint {
		Fade,
		Right,
		Left,
		Top,
		Bottom,
		None,
		Middle,

		[iOS (5,0)]
		Automatic = 100
	}

	// #defines over UIBarPosition -> NSInteger -> UIBarCommon.h
	[Native]
	[NoTV][NoWatch]
	[iOS (5,0)]
	public enum UIToolbarPosition : nint {
		Any, Bottom, Top
	}
	
	// NSInteger -> UITouch.h
	[Native]
	[NoWatch]
	public enum UITouchPhase : nint {
		Began,
		Moved,
		Stationary,
		Ended,
		Cancelled,      
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UITouchType : nint
	{
		Direct,
		Indirect,
		Stylus
	}

	[NoWatch]
	[iOS (9,1)]
	[Native]
	[Flags]
	public enum UITouchProperties : nint
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
	[Native]
	public enum UITextAlignment : nint {
		Left,
		Center,
		Right, 

		[iOS (6,0)]
		Justified,
		[iOS (6,0)]
		Natural	
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoWatch]
	public enum UITableViewCellStyle : nint {
		Default,
		Value1,	
		Value2,	
		Subtitle
	}                 

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoTV][NoWatch]
	public enum UITableViewCellSeparatorStyle : nint {
		None,
		SingleLine,
		SingleLineEtched,
		DoubleLineEtched = SingleLineEtched
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoWatch]
	public enum UITableViewCellSelectionStyle : nint {
		None,
		Blue,
		Gray,
		Default
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoWatch]
	public enum UITableViewCellEditingStyle : nint {
		None,
		Delete,
		Insert
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoWatch]
	public enum UITableViewCellAccessory : nint {
		None,                
		DisclosureIndicator,
		[NoTV]
		DetailDisclosureButton,
		Checkmark,
		[NoTV][iOS (7,0)]
		DetailButton
	}

	// NSUInteger -> UITableViewCell.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UITableViewCellState : nuint_compat_int {
		DefaultMask                     = 0,
		ShowingEditControlMask          = 1 << 0,
		ShowingDeleteConfirmationMask   = 1 << 1
	}

	// NSInteger -> UITextField.h
	[Native]
	[NoWatch]
	public enum UITextBorderStyle : nint {
		None,
		Line,
		Bezel,
		RoundedRect
	}

	// NSInteger -> UITextField.h
	[Native]
	[NoWatch]
	public enum UITextFieldViewMode : nint {
		Never,
		WhileEditing,
		UnlessEditing,
		Always
	}

	// NSInteger -> UIViewController.h
	[Native]
	[NoWatch]
	public enum UIModalTransitionStyle : nint {
		CoverVertical = 0,
		[NoTV]
		FlipHorizontal,
		CrossDissolve,
		[NoTV]
		PartialCurl
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoTV][NoWatch]
	public enum UIInterfaceOrientation : nint {
		[iOS (8,0)]
		Unknown            = UIDeviceOrientation.Unknown,
		Portrait           = UIDeviceOrientation.Portrait,
		PortraitUpsideDown = UIDeviceOrientation.PortraitUpsideDown,
		LandscapeLeft      = UIDeviceOrientation.LandscapeRight,
		LandscapeRight     = UIDeviceOrientation.LandscapeLeft
	}

	// NSUInteger -> UIApplication.h
	[Native]
	[Flags]
	[NoTV][NoWatch]
	public enum UIInterfaceOrientationMask : nuint_compat_int {
		Portrait = 1 << (int) UIInterfaceOrientation.Portrait,
		LandscapeLeft = 1 << (int) UIInterfaceOrientation.LandscapeLeft,
		LandscapeRight = 1 << (int) UIInterfaceOrientation.LandscapeRight,
		PortraitUpsideDown = 1 << (int) UIInterfaceOrientation.PortraitUpsideDown,

		Landscape = LandscapeLeft | LandscapeRight,
		All = PortraitUpsideDown | Portrait | LandscapeRight | LandscapeLeft,
		AllButUpsideDown = Portrait | LandscapeRight | LandscapeLeft,
	}
	
	// NSInteger -> UIApplication.h
	[Native]
	[NoWatch][NoTV]
	public enum UIWebViewNavigationType : nint {
		LinkClicked,
		FormSubmitted,
		BackForward,
		Reload,
		FormResubmitted,
		Other
	}

	// NSUInteger -> UIApplication.h
	[Native]
	[Flags]
	[NoTV][NoWatch]
	public enum UIDataDetectorType : nuint {
		PhoneNumber            = 1 << 0,
		Link                   = 1 << 1,

		[iOS (4,0)]
		Address                = 1 << 2,
		[iOS (4,0)]
		CalendarEvent          = 1 << 3,

		[iOS (10,0)]
		ShipmentTrackingNumber = 1 << 4,
		[iOS (10,0)]
		FlightNumber           = 1 << 5,
		[iOS (10,0)]
		LookupSuggestion       = 1 << 6,

		None          = 0,
#if XAMCORE_2_0
		All           = UInt64.MaxValue
#else
		All           = UInt32.MaxValue
#endif
	}

	// NSInteger -> UIActionSheet.h
	[Native]
	[NoTV][NoWatch]
	public enum UIActionSheetStyle : nint {
		Automatic        = -1,
		Default          = UIBarStyle.Default,
		BlackTranslucent = 2, // UIBarStyle.BlackTranslucent,
		BlackOpaque      = 1 // UIBarStyle.BlackOpaque,
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoTV][NoWatch]
	public enum UIStatusBarStyle : nint {
		Default,

		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use LightContent instead")]
		BlackTranslucent = 1,

		LightContent = 1,

		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use LightContent instead")]
		BlackOpaque = 2,
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoTV][NoWatch]
	public enum UIStatusBarAnimation : nint {
		None, 
		Fade,
		Slide
	}
	
	// NSInteger -> UIGestureRecognizer.h
	[Native]
	[NoWatch]
	public enum UIGestureRecognizerState : nint {
		Possible,
		Began,
		Changed,
		Ended,
		Cancelled,
		Failed,
    
		Recognized = Ended
	}

	// NSUInteger -> UIApplication.h
	[Native]
	[Flags]
	[NoTV][NoWatch]
	public enum UIRemoteNotificationType : nuint_compat_int {
		None    = 0,
		Badge   = 1 << 0,
		Sound   = 1 << 1,
		Alert   = 1 << 2,

		[iOS (5,0)]
		NewsstandContentAvailability = 1 << 3
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UIKeyboardAppearance : nint {
		Default,
		Alert,
		Dark = Alert,
		Light
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UIReturnKeyType : nint {
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
	[Native]
	[NoWatch]
	public enum UIModalPresentationStyle : nint {
		FullScreen = 0,
		[NoTV]
		PageSheet,
		[NoTV]
		FormSheet,
		CurrentContext,
		Custom,
		OverFullScreen,
		OverCurrentContext,
		[NoTV]
		Popover,
		None = -1
	}
	
	// NSUInteger -> UISwipeGestureRecognizer.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UISwipeGestureRecognizerDirection : nuint_compat_int {
		Right = 1 << 0,
		Left = 1 << 1,
		Up = 1 << 2,
		Down = 1 << 3,
	}

	// NSUInteger -> UIPopoverController.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UIPopoverArrowDirection : nuint {
		Up = 1 << 0,
		Down = 1 << 1,
		Left = 1 << 2,
		Right = 1 << 3,
		Any = Up | Down | Left | Right,
#if XAMCORE_2_0
		Unknown = UInt64.MaxValue
#else
		Unknown = UInt32.MaxValue
#endif
	};

	// NSInteger -> UIMenuController.h
	[Native]
	[NoTV][NoWatch]
	public enum UIMenuControllerArrowDirection : nint {
		Default,
		Up,
		Down,
		Left,
		Right,
	}

	// NSUInteger -> UIPopoverController.h
	[Native]
	[Flags]
	public enum UIRectCorner : nuint {
		TopLeft     = 1 << 0,
		TopRight    = 1 << 1,
		BottomLeft  = 1 << 2,
		BottomRight = 1 << 3,
		AllCorners  = ~(uint)0
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoWatch]
	[iOS (5,0)]
	public enum UIUserInterfaceLayoutDirection : nint {
		LeftToRight, RightToLeft
	}
	
	// NSInteger -> UIDevice.h
	[Native]
	[NoWatch]
	public enum UIUserInterfaceIdiom : nint {
		Unspecified = -1,
		Phone,
		Pad,
		TV,
		CarPlay,
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoWatch]
	[iOS (4,0)]
	public enum UIApplicationState : nint {
		Active,
		Inactive,
		Background
	}

	// NSInteger -> UIView.h
	[Native]
	[Flags]
	[NoWatch]
	[iOS (4,0)]
	public enum UIViewAnimationOptions : nuint_compat_int {
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

		[iOS (10,3)]
		PreferredFramesPerSecondDefault = 0 << 24,
		[iOS (10,3)]
		PreferredFramesPerSecond60 = 3 << 24,
		[iOS (10,3)]
		PreferredFramesPerSecond30 = 7 << 24,
	}

#if !WATCH
	delegate void UIPrintInteractionCompletionHandler (UIPrintInteractionController printInteractionController, bool completed, NSError error);
#endif

	// untyped (and unamed) enum -> UIPrintError.h
	// note: it looks unused by any API
	[NoTV][NoWatch]
	[iOS (4,2)]
	[ErrorDomain ("UIPrintErrorDomain")]
	public enum UIPrintError {
		NotAvailable = 1,
		NoContent,
		UnknownImageFormat,
		JobFailed,
	}

	// NSInteger -> UIPrintInfo.h
	[Native]
	[NoTV][NoWatch]
	[iOS (4,2)]
	public enum UIPrintInfoDuplex : nint {
		None,
		LongEdge,
		ShortEdge,
	}

	// NSInteger -> UIPrintInfo.h
	[Native]
	[NoTV][NoWatch]
	[iOS (4,2)]
	public enum UIPrintInfoOrientation : nint {
		Portrait,
		Landscape,
	}

	// NSInteger -> UIPrintInfo.h
	[Native]
	[NoTV][NoWatch]
	[iOS (4,2)]
	public enum UIPrintInfoOutputType : nint {
		General,
		Photo,
		Grayscale,
		PhotoGrayscale
	}

	// NSInteger -> UIAccessibility.h
	[Native]
	[NoWatch]
	[iOS (4,2)]
	public enum UIAccessibilityScrollDirection : nint {
		Right = 1,
		Left,
		Up,
		Down,
		Next,
		Previous
	}

	// NSInteger -> UIScreen.h
	[Native]
	[NoWatch]
	[iOS (5,0)]
	public enum UIScreenOverscanCompensation : nint {
		Scale, InsetBounds,
		None,
		[Obsolete ("Use UIScreenOverscanCompensation.None instead")]
		InsetApplicationFrame = None
	}

	// NSInteger -> UISegmentedControl.h
	[Native]
	[NoWatch]
	[iOS (5,0)]
	public enum UISegmentedControlSegment : nint {
		Any, Left, Center, Right, Alone
	}

	// NSInteger -> UISearchBar.h
	[Native]
	[iOS (5,0)]
	[NoWatch]
	public enum UISearchBarIcon : nint {
		Search,
		[NoTV]
		Clear,
		[NoTV]
		Bookmark,
		[NoTV]
		ResultsList
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	[iOS (5,0)]
	public enum UIPageViewControllerNavigationOrientation : nint {
		Horizontal, Vertical
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	[iOS (5,0)]
	public enum UIPageViewControllerSpineLocation : nint {
		None, Min, Mid, Max
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	[iOS (5,0)]
	public enum UIPageViewControllerNavigationDirection : nint {
		Forward, Reverse
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	[iOS (5,0)]
	public enum UIPageViewControllerTransitionStyle : nint {
		PageCurl, Scroll
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UITextSpellCheckingType : nint {
		Default, No, Yes, 
	}

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextStorageDirection : nint {
		Forward, Backward
	}

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextLayoutDirection : nint {
		Right = 2,
		Left,
		Up,
		Down
	}

	// Sum of UITextStorageDirection and UITextLayoutDirection 
	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextDirection : nint {
		Forward, Backward, Right, Left, Up, Down
	}
	
	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextWritingDirection : nint {
		Natural = -1,
		LeftToRight,
		RightToLeft,
	}

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextGranularity : nint {
		Character,
		Word,
		Sentence,
		Paragraph,
		Line,
		Document
	}

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum NSLayoutRelation : nint {
		LessThanOrEqual = -1,
		Equal = 0,
		GreaterThanOrEqual = 1
	}

	// NSInteger -> NSLayoutConstraint.h
	[Native]
	[NoWatch]
	public enum NSLayoutAttribute : nint {
		NoAttribute = 0,
		Left = 1,
		Right,
		Top,
		Bottom,
		Leading,
		Trailing,
		Width,
		Height,
		CenterX,
		CenterY,
		Baseline,
		LastBaseline = Baseline,
		FirstBaseline,
   
		[iOS (8,0)]
		LeftMargin,
		[iOS (8,0)]
		RightMargin,
		[iOS (8,0)]
		TopMargin,
		[iOS (8,0)]
		BottomMargin,
		[iOS (8,0)]
		LeadingMargin,
		[iOS (8,0)]
		TrailingMargin,
		[iOS (8,0)]
		CenterXWithinMargins,
		[iOS (8,0)]
		CenterYWithinMargins,
	}

	// NSUInteger -> NSLayoutConstraint.h
	[Native]
	[Flags]
	[NoWatch]
	public enum NSLayoutFormatOptions : nuint_compat_int {
		AlignAllLeft = (1 << (int) NSLayoutAttribute.Left),
		AlignAllRight = (1 << (int) NSLayoutAttribute.Right),
		AlignAllTop = (1 << (int) NSLayoutAttribute.Top),
		AlignAllBottom = (1 << (int) NSLayoutAttribute.Bottom),
		AlignAllLeading = (1 << (int) NSLayoutAttribute.Leading),
		AlignAllTrailing = (1 << (int) NSLayoutAttribute.Trailing),
		AlignAllCenterX = (1 << (int) NSLayoutAttribute.CenterX),
		AlignAllCenterY = (1 << (int) NSLayoutAttribute.CenterY),
		AlignAllBaseline = (1 << (int) NSLayoutAttribute.Baseline),
		AlignAllLastBaseline = (1 << (int) NSLayoutAttribute.LastBaseline),
		AlignAllFirstBaseline = (1 << (int) NSLayoutAttribute.FirstBaseline),
		
		AlignmentMask = 0xFFFF,
		
		/* choose only one of these three
		 */
		DirectionLeadingToTrailing = 0 << 16, // default
		DirectionLeftToRight = 1 << 16,
		DirectionRightToLeft = 2 << 16,
		
		DirectionMask = 0x3 << 16,
	}

	// float (and not even a CGFloat) -> NSLayoutConstraint.h
	// the API were fixed (a long time ago to use `float`) and the enum
	// values can still be used (and useful) since they will be casted
	[NoWatch]
	public enum UILayoutPriority {
		Required = 1000,
		DefaultHigh = 750,
		DefaultLow = 250,
		FittingSizeLevel = 50,
	}

	// NSInteger -> NSLayoutConstraint.h
	[Native]
	[NoWatch]
	public enum UICollectionUpdateAction : nint {
		Insert, Delete, Reload, Move, None
	}

	// NSUInteger -> UICollectionView.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UICollectionViewScrollPosition : nuint_compat_int {
		None,
		Top = 1 << 0,
		CenteredVertically = 1 << 1,
		Bottom = 1 << 2,
		Left = 1 << 3,
		CenteredHorizontally = 1 << 4,
		Right = 1 << 5
	}

	// NSInteger -> UICollectionViewFlowLayout.h
	[Native]
	[NoWatch]
	public enum UICollectionViewScrollDirection : nint {
		Vertical, Horizontal
	}

	// NSInteger -> UICollectionViewFlowLayout.h
	[Native]
	[NoWatch]
	public enum UILayoutConstraintAxis : nint {
		Horizontal, Vertical
	}

	// NSInteger -> UIImage.h
	[Native]
	public enum UIImageResizingMode : nint {
		Tile, Stretch
	}

	// NSUInteger -> UICollectionViewLayout.h
	[Native]
	[NoWatch]
	public enum UICollectionElementCategory : nuint_compat_int {
		Cell, SupplementaryView, DecorationView
	}

	// that's a convenience enum that maps to UICollectionElementKindSection[Footer|Header] which are NSString
	[NoWatch]
	public enum UICollectionElementKindSection {
		Header,
		Footer
	}

	// uint64_t -> UIAccessibilityConstants.h
	// note: IMO not really worth changing to ulong in XAMCORE_2_0
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
	public enum UIImageRenderingMode : nint {
		Automatic,
		AlwaysOriginal,
		AlwaysTemplate
	}

	// NSInteger -> UIMotionEffect.h
	[Native]
	[NoWatch]
	public enum UIInterpolatingMotionEffectType : nint {
		TiltAlongHorizontalAxis,
		TiltAlongVerticalAxis
	}

	// NSInteger -> UINavigationController.h
	[Native]
	[NoWatch]
	public enum UINavigationControllerOperation : nint {
		None, Push, Pop
	}

	// NSInteger -> NSLayoutManager.h
	[Native]
	[NoWatch]
	public enum NSTextLayoutOrientation : nint {
		Horizontal, Vertical
	}

	// NSUInteger -> NSTextStorage.h
	[Native]
	[Flags]
	[NoWatch]
	public enum NSTextStorageEditActions : nuint_compat_int {
		Attributes = 1,
		Characters = 2
	}

	// NSInteger -> UIActivity.h
	[Native]
	[NoTV][NoWatch]
	public enum UIActivityCategory : nint {
		Action, Share
	}

	// NSInteger -> UIAttachmentBehavior.h
	[Native]
	[NoWatch]
	public enum UIAttachmentBehaviorType : nint {
		Items, Anchor
	}

	// NSInteger -> UIBarCommon.h
	[Native]
	[NoWatch]
	public enum UIBarPosition : nint {
		Any, Bottom, Top, TopAttached, 
	}

	// NSUInteger -> UICollisionBehavior.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UICollisionBehaviorMode : nuint  {
		Items = 1,
		Boundaries = 2,
#if XAMCORE_2_0
		Everything = UInt64.MaxValue
#else
		Everything = UInt32.MaxValue
#endif
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
	[Native]
	[Flags]
	[NoWatch]
	public enum UIKeyModifierFlags : nint {
		AlphaShift     = 1 << 16,  // This bit indicates CapsLock
		Shift          = 1 << 17,
		Control        = 1 << 18,
		Alternate      = 1 << 19,
		Command        = 1 << 20,
		NumericPad     = 1 << 21,
	}

	// NSInteger -> UIScrollView.h
	[Native]
	[NoWatch]
	public enum UIScrollViewKeyboardDismissMode : nint {
		None, OnDrag, Interactive
	}

	// NSInteger -> UIWebView.h
	[NoWatch][NoTV]
	[Native]
	public enum UIWebPaginationBreakingMode : nint {
		Page, Column
	}

	// NSInteger -> UIWebView.h
	[NoWatch][NoTV]
	[Native]
	public enum UIWebPaginationMode : nint {
		Unpaginated,
		LeftToRight,
		TopToBottom,
		BottomToTop,
		RightToLeft
	}

	// NSInteger -> UIPushBehavior.h
	[Native]
	[NoWatch]
	public enum UIPushBehaviorMode : nint {
		Continuous,
		Instantaneous
	}

	// NSInteger -> NSLayoutManager.h
	[Native]
	[NoWatch]
	public enum NSGlyphProperty : nint {
		Null = (1 << 0),
		ControlCharacter = (1 << 1),
		Elastic = (1 << 2),
		NonBaseCharacter = (1 << 3)
	}
	
	// NSInteger -> NSLayoutManager.h
	[Native]
	[NoWatch]
	public enum NSControlCharacterAction : nint {
		ZeroAdvancementAction = (1 << 0),
		WhitespaceAction = (1 << 1),
		HorizontalTabAction = (1 << 2),
		LineBreakAction = (1 << 3),
		ParagraphBreakAction = (1 << 4),
		ContainerBreakAction = (1 << 5)
	}

	// NSInteger -> UITabBar.h
	[Native]
	[NoWatch]
	public enum UITabBarItemPositioning : nint {
		Automatic,
		Fill,
		Centered
	}

	// NSUInteger -> UIView.h
	[Native]
	[iOS (7,0)]
	[NoWatch]
	public enum UIViewKeyframeAnimationOptions : nuint {
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
	[Native]
	[NoWatch]
	public enum UIViewTintAdjustmentMode : nint {
		Automatic,
		Normal,
		Dimmed
	}

	// NSUInteger -> UIView.h
	[Native]
	[NoWatch]
	public enum UISystemAnimation : nuint_compat_int {
		Delete
	}

	// NSUInteger -> UIGeometry.h
	[Native]
	public enum UIRectEdge : nuint_compat_int {
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
	[Native]
	[NoWatch]
	[iOS (7,0)]
	public enum UISearchBarStyle : nuint {
		Default,
		Prominent,
		Minimal
	}

	// NSInteger -> UIInputView.h
	[Native]
	[NoWatch]
	[iOS (7,0)]
	public enum UIInputViewStyle : nint {
		Default,
		Keyboard
	}

	[Native]
	[NoWatch]
	[iOS (8,0)]
	public enum UIUserInterfaceSizeClass : nint {
		Unspecified = 0,
		Compact = 1,
		Regular = 2
	}

	[Native]
	[NoWatch]
	[iOS (8,0)]
	public enum UIAlertActionStyle : nint {
		Default, Cancel, Destructive
	}

	[Native]
	[NoWatch]
	[iOS (8,0)]
	public enum UIAlertControllerStyle : nint {
		ActionSheet,
		Alert
	}

	[Native]
	[NoWatch]
	[iOS (8,0)]
	public enum UIBlurEffectStyle : nint {
		ExtraLight, Light, Dark,
		[TV (10,0), NoiOS, NoWatch]
		ExtraDark,
		[iOS (10,0)]
		Regular = 4,
		[iOS (10,0)]
		Prominent = 5,
	}

	[Native]
	[NoTV][NoWatch]
	[iOS (8,0)]
	public enum UIPrinterJobTypes : nint {
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

	[NoTV][NoWatch]
	[iOS (8,0)]
	[Deprecated (PlatformName.iOS, 10, 0, message:"Use UNAuthorizationOptions instead")]
	[Native]
	[Flags]
	public enum UIUserNotificationType : nuint {
		None       = 0,
		Badge      = 1 << 0,
		Sound      = 1 << 1, 
		Alert      = 1 << 2
	}
	
	[NoTV][NoWatch]
	[iOS (8, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use UNNotificationActionOptions instead")]
	[Native]
	public enum UIUserNotificationActivationMode : nuint {
		Foreground,
		Background
	}

	[NoTV][NoWatch]
	[iOS (8, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use UNNotificationCategory.Actions instead")]
	[Native]
	public enum UIUserNotificationActionContext : nuint {
		Default,
		Minimal
	}

	[NoTV][NoWatch]
	[iOS (8, 0)]
	[Native]
	public enum UIDocumentMenuOrder : nuint {
		First,
		Last
	}

	[NoTV][NoWatch]
	[iOS (8, 0)]
	[Native]
	public enum UIDocumentPickerMode : nuint {
		Import,
		Open,
		ExportToService,
		MoveToService
	}

	[iOS (8, 0)]
	[Native]
	public enum UIAccessibilityNavigationStyle : nint {

		Automatic = 0,
		Separate = 1,
		Combined = 2
	}

	[Native]
	[NoWatch]
	public enum UISplitViewControllerDisplayMode : nint {
		Automatic,
		PrimaryHidden,
		AllVisible,
		PrimaryOverlay
	}

	[Native]
	[NoTV][NoWatch]
	public enum UITableViewRowActionStyle : nint {
		Default, Destructive = Default, Normal
	}

	// Utility enum for UITransitionContext[To|From]ViewKey
	[NoWatch]
	public enum UITransitionViewControllerKind {
		ToView, FromView
	}

	// note [Native] since it maps to UIFontWeightConstants fields (CGFloat)
	[iOS (8,2)]
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

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UIStackViewDistribution : nint {
		Fill,
		FillEqually,
		FillProportionally,
		EqualSpacing,
		EqualCentering
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UIStackViewAlignment : nint {
		Fill,
		Leading,
		Top = Leading,
		FirstBaseline,
		Center,
		Trailing,
		Bottom = Trailing,
		LastBaseline
	}

	[iOS (9,0)]
	[Native]
	[Flags]
	public enum NSWritingDirectionFormatType : nint {
		Embedding = 0 << 1,
		Override = 1 << 1
	}

	[NoTV][NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UIPrinterCutterBehavior : nint
	{
		NoCut,
		PrinterDefault,
		CutAfterEachPage,
		CutAfterEachCopy,
		CutAfterEachJob
	}

	[NoTV][NoWatch]
	[iOS (9,0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use UNNotificationAction or UNTextInputNotificationAction instead")]
	[Native]
	public enum UIUserNotificationActionBehavior : nuint
	{
		Default,
		TextInput
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UISemanticContentAttribute : nint
	{
		Unspecified = 0,
		Playback,
		Spatial,
		ForceLeftToRight,
		ForceRightToLeft
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UIDynamicItemCollisionBoundsType : nuint
	{
		Rectangle,
		Ellipse,
		Path
	}

	[Native]
	[NoWatch]
	[iOS(9,0)]
	public enum UIForceTouchCapability : nint {
		Unknown = 0,
		Unavailable = 1,
		Available = 2
	}

	[Native]
	[NoWatch]
	[iOS(9,0)]
	public enum UIPreviewActionStyle : nint {
		Default, Selected, Destructive
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UIPressPhase : nint {
		Began,
		Changed,
		Stationary,
		Ended,
		Cancelled
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UIPressType : nint {
		UpArrow,
		DownArrow,
		LeftArrow,
		RightArrow,
		Select,
		Menu,
		PlayPause
	}

	[NoWatch]
	[iOS (9,0)] // introduced in Xcode 7.1 SDK (iOS 9.1 but hidden in 9.0)
	[Native]
	public enum UITableViewCellFocusStyle : nint {
		Default,
		Custom
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UIDisplayGamut : nint
	{
		Unspecified = -1,
		Srgb,
		P3
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UITraitEnvironmentLayoutDirection : nint
	{
		Unspecified = -1,
		LeftToRight = UIUserInterfaceLayoutDirection.LeftToRight,
		RightToLeft = UIUserInterfaceLayoutDirection.RightToLeft
	}

	[TV (10,0), NoWatch, NoiOS]
	[Native]
	public enum UIUserInterfaceStyle : nint
	{
		Unspecified,
		Light,
		Dark
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UITextItemInteraction : nint
	{
		InvokeDefaultAction,
		PresentActions,
		Preview
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UIViewAnimatingState : nint
	{
		Inactive,
		Active,
		Stopped
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UIViewAnimatingPosition : nint
	{
		End,
		Start,
		Current
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UITimingCurveType : nint
	{
		Builtin,
		Cubic,
		Spring,
		Composed
	}

	[NoWatch]
	[NoTV]
	[iOS(10,0)]
	[Native]
	public enum UIAccessibilityHearingDeviceEar : nuint {
		None = 0,
		Left = 1 << 1,
		Right = 1 << 2,
		Both = Left | Right
	}

	[NoWatch]
	[iOS(10,0)]
	[Native]
	public enum UIAccessibilityCustomRotorDirection : nint
	{
		Previous,
		Next
	}

#if XAMCORE_4_0
	[NoTV]
#else
	// Xcode 8.2 beta 1 added __TVOS_PROHIBITED but we need to keep it for binary compatibility
	[TV (10,0)]
#endif
	[iOS (10,0)][NoWatch]
	[Native]
	[Flags]
	public enum UICloudSharingPermissionOptions : nuint {
		Standard = 0,
		AllowPublic = 1 << 0,
		AllowPrivate = 1 << 1,
		AllowReadOnly = 1 << 2,
		AllowReadWrite = 1 << 3
	}

	[iOS (10,0), TV (10,0), NoWatch]
	[Native]
	public enum UITextFieldDidEndEditingReason : nint {
		Unknown = -1, // helper value (not in headers)
		Committed,
		[NoiOS]
		Cancelled
	}

	[iOS (10,3), TV (10,2), NoWatch]
	[Native]
	public enum UIScrollViewIndexDisplayMode : nint {
		Automatic,
		AlwaysHidden
	}
}
