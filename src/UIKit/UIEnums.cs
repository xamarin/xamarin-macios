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
	[Native]
	[NoTV][NoWatch]
	public enum UIImagePickerControllerQualityType : long {
		High,
		Medium,
		Low,
		At640x480,
		At1280x720,
		At960x540
	}

	// NSInteger -> UIActivityIndicatorView.h
	[Native]
	[NoWatch]
	public enum UIActivityIndicatorViewStyle : long {
		WhiteLarge,
		White,
		Gray
	}

	// NSInteger -> UIAlertView.h
	[Native]
	[NoTV][NoWatch]
	public enum UIAlertViewStyle : long {
		Default,
		SecureTextInput,
		PlainTextInput,
		LoginAndPasswordInput
	}

	// NSInteger -> UIBarButtonItem.h
	[Native]
	[NoWatch]
	public enum UIBarButtonItemStyle : long {
		Plain,

		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'UIBarButtonItemStyle.Plain' instead.")]
		Bordered,
			
		Done,
	}

	// NSInteger -> UIBarButtonItem.h
	[Native]
	[NoWatch]
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
		[Deprecated (PlatformName.iOS, 11, 0)]
		PageCurl
	} 

	// NSUInteger -> UIControl.h
	[Native]
	[Flags]
	[NoWatch]
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
	public enum UIEventType : long {
		Touches,
		Motion,
		RemoteControl,
		[iOS (9,0)]
		Presses
	}

	// NSInteger -> UIEvent.h
	[Native]
	[NoWatch]
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
	[Native]
	[NoWatch]
	public enum UIControlContentVerticalAlignment : long {
		Center  = 0,
		Top     = 1,
		Bottom  = 2,
		Fill    = 3,
	}

	// NSInteger -> UIControl.h
	[Native]
	[NoWatch]
	public enum UIControlContentHorizontalAlignment : long {
		Center = 0,
		Left   = 1,
		Right  = 2,
		Fill   = 3,
		Leading = 4,
		Trailing = 5
	}

	// NSUInteger -> UIControl.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UIControlState : ulong {
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
	[Native]
	[Flags]
	[NoWatch]
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
	[Native]
	[NoWatch]
	public enum UIViewAnimationCurve : long {
		EaseInOut,
		EaseIn,
		EaseOut,
		Linear
	}

	// NSInteger -> UIView.h
	[Native]
	[NoWatch]
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
	[Native]
	[NoWatch]
	public enum UIViewAnimationTransition : long {
		None,
		FlipFromLeft,
		FlipFromRight,
		CurlUp,
		CurlDown,
	}

	// NSInteger -> UIBarCommon.h
	[Native]
	[NoWatch]
	public enum UIBarMetrics : long {
		Default,
		Compact,
		DefaultPrompt = 101,
		CompactPrompt,

		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'UIBarMetrics.Compat' instead.")]
		LandscapePhone = Compact,

		[iOS (7, 0)]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'UIBarMetrics.CompactPrompt' instead.")]
		LandscapePhonePrompt = CompactPrompt
	}

	// NSInteger -> UIButton.h
	[Native]
	[NoWatch]
	public enum UIButtonType : long {
		Custom,
		RoundedRect,
		DetailDisclosure,
		InfoLight,
		InfoDark,
		ContactAdd,
		[TV (11,0)][NoiOS]
		Plain,
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
	[Native]
	[NoWatch]
	public enum UIBaselineAdjustment : long {
		AlignBaselines = 0,
		AlignCenters,
		None,
	}

	// NSInteger -> UIDatePicker.h
	[Native]
	[NoTV][NoWatch]
	public enum UIDatePickerMode : long {
		Time,         
		Date,         
		DateAndTime,  
		CountDownTimer 
	}

	// NSInteger -> UIDevice.h
	[Native]
	[NoTV][NoWatch]
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
	[Native]
	[NoTV][NoWatch]
	public enum UIDeviceBatteryState : long {
		Unknown,
		Unplugged,
		Charging, 
		Full,     
	}

	// NSInteger -> UIDocument.h
	[Native]
	[NoTV][NoWatch]
	public enum UIDocumentChangeKind : long {
		Done, Undone, Redone, Cleared
	}

	// NSInteger -> UIDocument.h
	[Native]
	[NoTV][NoWatch]
	public enum UIDocumentSaveOperation : long {
		ForCreating, ForOverwriting
	}

	// NSUInteger -> UIDocument.h
	[Native]
	[Flags]
	[NoTV][NoWatch]
	public enum UIDocumentState : ulong {
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
	public enum UIImagePickerControllerSourceType : long {
		PhotoLibrary,
		Camera,
		SavedPhotosAlbum
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch][NoTV]
	public enum UIImagePickerControllerCameraCaptureMode : long {
		Photo, Video
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch][NoTV]
	public enum UIImagePickerControllerCameraDevice : long {
		Rear,
		Front
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch][NoTV]
	public enum UIImagePickerControllerCameraFlashMode : long {
		Off = -1, Auto = 0, On = 1
	}

	// NSInteger -> UIInterface.h
	[Native]
	[NoWatch][NoTV]
	public enum UIBarStyle : long {
		Default,
		Black,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
		[Deprecated (PlatformName.iOS, 5, 1, message : "Use 'UIBarStyle.Black'.")]
		BlackOpaque      = 1,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
		[Deprecated (PlatformName.iOS, 5, 1, message : "Use 'UIBarStyle.Black' and set the translucency property to true.")]
		BlackTranslucent = 2,
	}

	// NSInteger -> UIProgressView.h
	[Native]
	[NoWatch]
	public enum UIProgressViewStyle : long {
		Default,
		[NoTV]
		Bar,
	}
	
	// NSInteger -> UIScrollView.h
	[Native]
	[NoWatch]
	public enum UIScrollViewIndicatorStyle : long {
		Default,
		Black,
		White
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UITextAutocapitalizationType : long {
		None,
		Words,
		Sentences,
		AllCharacters,
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UITextAutocorrectionType : long {
		Default,
		No,
		Yes,
	}
	
	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
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
		[iOS (10, 0)]
		AsciiCapableNumberPad
	} 

	// NSInteger -> UISegmentedControl.h
	[Native]
	[NoTV][NoWatch]
	[Deprecated (PlatformName.iOS, 7, 0, message : "This no longer has any effect.")]
	public enum UISegmentedControlStyle : long {
		Plain,
		Bordered,
		Bar,
		Bezeled
	}

	// NSInteger -> UITabBarItem.h
	[Native]
	[NoWatch]
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
	[Native]
	[NoWatch]
	public enum UITableViewStyle : long {
		Plain,
		Grouped
	}

	// NSInteger -> UITableView.h
	[Native]
	[NoWatch]
	public enum UITableViewScrollPosition : long {
		None,        
		Top,    
		Middle,   
		Bottom
	}
	
	// NSInteger -> UITableView.h
	[Native]
	[NoWatch]
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
	[Native]
	[NoTV][NoWatch]
	public enum UIToolbarPosition : long {
		Any, Bottom, Top
	}
	
	// NSInteger -> UITouch.h
	[Native]
	[NoWatch]
	public enum UITouchPhase : long {
		Began,
		Moved,
		Stationary,
		Ended,
		Cancelled,      
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UITouchType : long
	{
		Direct,
		Indirect,
		Stylus
	}

	[NoWatch]
	[iOS (9,1)]
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
	[Native]
	public enum UITextAlignment : long {
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
	public enum UITableViewCellStyle : long {
		Default,
		Value1,	
		Value2,	
		Subtitle
	}                 

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoTV][NoWatch]
	public enum UITableViewCellSeparatorStyle : long {
		None,
		SingleLine,
		[Deprecated (PlatformName.iOS, 11, 0, message:"Use 'SingleLine' for a single line separator.")]
		SingleLineEtched,
		DoubleLineEtched = SingleLineEtched
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoWatch]
	public enum UITableViewCellSelectionStyle : long {
		None,
		Blue,
		Gray,
		Default
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoWatch]
	public enum UITableViewCellEditingStyle : long {
		None,
		Delete,
		Insert
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoWatch]
	public enum UITableViewCellAccessory : long {
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
	public enum UITableViewCellState : ulong {
		DefaultMask                     = 0,
		ShowingEditControlMask          = 1 << 0,
		ShowingDeleteConfirmationMask   = 1 << 1
	}

	// NSInteger -> UITextField.h
	[Native]
	[NoWatch]
	public enum UITextBorderStyle : long {
		None,
		Line,
		Bezel,
		RoundedRect
	}

	// NSInteger -> UITextField.h
	[Native]
	[NoWatch]
	public enum UITextFieldViewMode : long {
		Never,
		WhileEditing,
		UnlessEditing,
		Always
	}

	// NSInteger -> UIViewController.h
	[Native]
	[NoWatch]
	public enum UIModalTransitionStyle : long {
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
		[iOS (8,0)]
	public enum UIInterfaceOrientation : long {
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
	[Native]
	[NoWatch][NoTV]
	public enum UIWebViewNavigationType : long {
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
	public enum UIDataDetectorType : ulong {
		PhoneNumber            = 1 << 0,
		Link                   = 1 << 1,
		Address                = 1 << 2,
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
	public enum UIActionSheetStyle : long {
		Automatic        = -1,
		Default          = UIBarStyle.Default,
		BlackTranslucent = 2, // UIBarStyle.BlackTranslucent,
		BlackOpaque      = 1 // UIBarStyle.BlackOpaque,
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoTV][NoWatch]
	public enum UIStatusBarStyle : long {
		Default,

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'LightContent' instead.")]
		BlackTranslucent = 1,

		LightContent = 1,

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'LightContent' instead.")]
		BlackOpaque = 2,
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoTV][NoWatch]
	public enum UIStatusBarAnimation : long {
		None, 
		Fade,
		Slide
	}
	
	// NSInteger -> UIGestureRecognizer.h
	[Native]
	[NoWatch]
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
	[Native]
	[Flags]
	[NoTV][NoWatch]
	public enum UIRemoteNotificationType : ulong {
		None    = 0,
		Badge   = 1 << 0,
		Sound   = 1 << 1,
		Alert   = 1 << 2,
		NewsstandContentAvailability = 1 << 3
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UIKeyboardAppearance : long {
		Default,
		Alert,
		Dark = Alert,
		Light
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
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
	[Native]
	[NoWatch]
	public enum UIModalPresentationStyle : long {
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
		BlurOverFullScreen,
		None = -1
	}
	
	// NSUInteger -> UISwipeGestureRecognizer.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UISwipeGestureRecognizerDirection : ulong {
		Right = 1 << 0,
		Left = 1 << 1,
		Up = 1 << 2,
		Down = 1 << 3,
	}

	// NSUInteger -> UIPopoverController.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UIPopoverArrowDirection : ulong {
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
		AllCorners  = ~(uint)0
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoWatch]
	public enum UIUserInterfaceLayoutDirection : long {
		LeftToRight, RightToLeft
	}
	
	// NSInteger -> UIDevice.h
	[Native]
	[NoWatch]
	public enum UIUserInterfaceIdiom : long {
		Unspecified = -1,
		Phone,
		Pad,
		TV,
		CarPlay,
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoWatch]
	public enum UIApplicationState : long {
		Active,
		Inactive,
		Background
	}

	// NSInteger -> UIView.h
	[Native]
	[Flags]
	[NoWatch]
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
	public enum UIPrintInfoDuplex : long {
		None,
		LongEdge,
		ShortEdge,
	}

	// NSInteger -> UIPrintInfo.h
	[Native]
	[NoTV][NoWatch]
	public enum UIPrintInfoOrientation : long {
		Portrait,
		Landscape,
	}

	// NSInteger -> UIPrintInfo.h
	[Native]
	[NoTV][NoWatch]
	public enum UIPrintInfoOutputType : long {
		General,
		Photo,
		Grayscale,
		PhotoGrayscale
	}

	// NSInteger -> UIAccessibility.h
	[Native]
	[NoWatch]
	public enum UIAccessibilityScrollDirection : long {
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
	public enum UIScreenOverscanCompensation : long {
		Scale, InsetBounds,
		None,
		[Obsolete ("Use 'UIScreenOverscanCompensation.None' instead.")]
		InsetApplicationFrame = None
	}

	// NSInteger -> UISegmentedControl.h
	[Native]
	[NoWatch]
	public enum UISegmentedControlSegment : long {
		Any, Left, Center, Right, Alone
	}

	// NSInteger -> UISearchBar.h
	[Native]
	[NoWatch]
	public enum UISearchBarIcon : long {
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
	public enum UIPageViewControllerNavigationOrientation : long {
		Horizontal, Vertical
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	public enum UIPageViewControllerSpineLocation : long {
		None, Min, Mid, Max
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	public enum UIPageViewControllerNavigationDirection : long {
		Forward, Reverse
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	public enum UIPageViewControllerTransitionStyle : long {
		PageCurl, Scroll
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	public enum UITextSpellCheckingType : long {
		Default, No, Yes, 
	}

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextStorageDirection : long {
		Forward, Backward
	}

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextLayoutDirection : long {
		Right = 2,
		Left,
		Up,
		Down
	}

	// Sum of UITextStorageDirection and UITextLayoutDirection 
	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextDirection : long {
		Forward, Backward, Right, Left, Up, Down
	}
	
	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextWritingDirection : long {
		Natural = -1,
		LeftToRight,
		RightToLeft,
	}

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	public enum UITextGranularity : long {
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
	public enum NSLayoutRelation : long {
		LessThanOrEqual = -1,
		Equal = 0,
		GreaterThanOrEqual = 1
	}

	// NSInteger -> NSLayoutConstraint.h
	[Native]
	[NoWatch]
	public enum NSLayoutAttribute : long {
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
	public enum NSLayoutFormatOptions : ulong {
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

		SpacingEdgeToEdge = 0 << 19,
		SpacingBaselineToBaseline = 1 << 19,
		SpacingMask = 1 << 19,
		
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
	public enum UICollectionUpdateAction : long {
		Insert, Delete, Reload, Move, None
	}

	// NSUInteger -> UICollectionView.h
	[Native]
	[Flags]
	[NoWatch]
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
	[Native]
	[NoWatch]
	public enum UICollectionViewScrollDirection : long {
		Vertical, Horizontal
	}

	// NSInteger -> UICollectionViewFlowLayout.h
	[Native]
	[NoWatch]
	public enum UILayoutConstraintAxis : long {
		Horizontal, Vertical
	}

	// NSInteger -> UIImage.h
	[Native]
	public enum UIImageResizingMode : long {
		Tile, Stretch
	}

	// NSUInteger -> UICollectionViewLayout.h
	[Native]
	[NoWatch]
	public enum UICollectionElementCategory : ulong {
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
	public enum UIImageRenderingMode : long {
		Automatic,
		AlwaysOriginal,
		AlwaysTemplate
	}

	// NSInteger -> UIMotionEffect.h
	[Native]
	[NoWatch]
	public enum UIInterpolatingMotionEffectType : long {
		TiltAlongHorizontalAxis,
		TiltAlongVerticalAxis
	}

	// NSInteger -> UINavigationController.h
	[Native]
	[NoWatch]
	public enum UINavigationControllerOperation : long {
		None, Push, Pop
	}

	// NSInteger -> NSLayoutManager.h
	[Native]
	[NoWatch]
	public enum NSTextLayoutOrientation : long {
		Horizontal, Vertical
	}

	// NSUInteger -> NSTextStorage.h
	[Native]
	[Flags]
	[NoWatch]
	public enum NSTextStorageEditActions : ulong {
		Attributes = 1,
		Characters = 2
	}

	// NSInteger -> UIActivity.h
	[Native]
	[NoTV][NoWatch]
	public enum UIActivityCategory : long {
		Action, Share
	}

	// NSInteger -> UIAttachmentBehavior.h
	[Native]
	[NoWatch]
	public enum UIAttachmentBehaviorType : long {
		Items, Anchor
	}

	// NSInteger -> UIBarCommon.h
	[Native]
	[NoWatch]
	public enum UIBarPosition : long {
		Any, Bottom, Top, TopAttached, 
	}

	// NSUInteger -> UICollisionBehavior.h
	[Native]
	[Flags]
	[NoWatch]
	public enum UICollisionBehaviorMode : ulong  {
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
	public enum UIKeyModifierFlags : long {
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
	public enum UIScrollViewKeyboardDismissMode : long {
		None, OnDrag, Interactive
	}

	// NSInteger -> UIWebView.h
	[NoWatch][NoTV]
	[Native]
	public enum UIWebPaginationBreakingMode : long {
		Page, Column
	}

	// NSInteger -> UIWebView.h
	[NoWatch][NoTV]
	[Native]
	public enum UIWebPaginationMode : long {
		Unpaginated,
		LeftToRight,
		TopToBottom,
		BottomToTop,
		RightToLeft
	}

	// NSInteger -> UIPushBehavior.h
	[Native]
	[NoWatch]
	public enum UIPushBehaviorMode : long {
		Continuous,
		Instantaneous
	}

	// NSInteger -> NSLayoutManager.h
	[Native]
	[NoWatch]
	public enum NSGlyphProperty : long {
		Null = (1 << 0),
		ControlCharacter = (1 << 1),
		Elastic = (1 << 2),
		NonBaseCharacter = (1 << 3)
	}
	
	// NSInteger -> NSLayoutManager.h
	[Native]
	[NoWatch]
	public enum NSControlCharacterAction : long {
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
	public enum UITabBarItemPositioning : long {
		Automatic,
		Fill,
		Centered
	}

	// NSUInteger -> UIView.h
	[Native]
	[iOS (7,0)]
	[NoWatch]
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
	[Native]
	[NoWatch]
	public enum UIViewTintAdjustmentMode : long {
		Automatic,
		Normal,
		Dimmed
	}

	// NSUInteger -> UIView.h
	[Native]
	[NoWatch]
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
	[Native]
	[NoWatch]
	[iOS (7,0)]
	public enum UISearchBarStyle : ulong {
		Default,
		Prominent,
		Minimal
	}

	// NSInteger -> UIInputView.h
	[Native]
	[NoWatch]
	[iOS (7,0)]
	public enum UIInputViewStyle : long {
		Default,
		Keyboard
	}

	[Native]
	[NoWatch]
	[iOS (8,0)]
	public enum UIUserInterfaceSizeClass : long {
		Unspecified = 0,
		Compact = 1,
		Regular = 2
	}

	[Native]
	[NoWatch]
	[iOS (8,0)]
	public enum UIAlertActionStyle : long {
		Default, Cancel, Destructive
	}

	[Native]
	[NoWatch]
	[iOS (8,0)]
	public enum UIAlertControllerStyle : long {
		ActionSheet,
		Alert
	}

	[Native]
	[NoWatch]
	[iOS (8,0)]
	public enum UIBlurEffectStyle : long {
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

	[NoTV][NoWatch]
	[iOS (8,0)]
	[Deprecated (PlatformName.iOS, 10, 0, message:"Use 'UNAuthorizationOptions' instead.")]
	[Native]
	[Flags]
	public enum UIUserNotificationType : ulong {
		None       = 0,
		Badge      = 1 << 0,
		Sound      = 1 << 1, 
		Alert      = 1 << 2
	}
	
	[NoTV][NoWatch]
	[iOS (8, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationActionOptions' instead.")]
	[Native]
	public enum UIUserNotificationActivationMode : ulong {
		Foreground,
		Background
	}

	[NoTV][NoWatch]
	[iOS (8, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationCategory.Actions' instead.")]
	[Native]
	public enum UIUserNotificationActionContext : ulong {
		Default,
		Minimal
	}

	[Deprecated (PlatformName.iOS, 11, 0)]
	[NoTV][NoWatch]
	[iOS (8, 0)]
	[Native]
	public enum UIDocumentMenuOrder : ulong {
		First,
		Last
	}

	[NoTV][NoWatch]
	[iOS (8, 0)]
	[Native]
	public enum UIDocumentPickerMode : ulong {
		Import,
		Open,
		ExportToService,
		MoveToService
	}

	[iOS (8, 0)]
	[Native]
	public enum UIAccessibilityNavigationStyle : long {

		Automatic = 0,
		Separate = 1,
		Combined = 2
	}

	[Native]
	[NoWatch]
	public enum UISplitViewControllerDisplayMode : long {
		Automatic,
		PrimaryHidden,
		AllVisible,
		PrimaryOverlay
	}

	[Native]
	[NoTV][NoWatch]
	public enum UITableViewRowActionStyle : long {
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
	public enum UIStackViewDistribution : long {
		Fill,
		FillEqually,
		FillProportionally,
		EqualSpacing,
		EqualCentering
	}

	[NoWatch]
	[iOS (9,0)]
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

	[iOS (9,0)]
	[Native]
	[Flags]
	public enum NSWritingDirectionFormatType : long {
		Embedding = 0 << 1,
		Override = 1 << 1
	}

	[NoTV][NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UIPrinterCutterBehavior : long
	{
		NoCut,
		PrinterDefault,
		CutAfterEachPage,
		CutAfterEachCopy,
		CutAfterEachJob
	}

	[NoTV][NoWatch]
	[iOS (9,0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationAction' or 'UNTextInputNotificationAction' instead.")]
	[Native]
	public enum UIUserNotificationActionBehavior : ulong
	{
		Default,
		TextInput
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UISemanticContentAttribute : long
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
	public enum UIDynamicItemCollisionBoundsType : ulong
	{
		Rectangle,
		Ellipse,
		Path
	}

	[Native]
	[NoWatch]
	[iOS(9,0)]
	public enum UIForceTouchCapability : long {
		Unknown = 0,
		Unavailable = 1,
		Available = 2
	}

	[Native]
	[NoWatch]
	[iOS(9,0)]
	public enum UIPreviewActionStyle : long {
		Default, Selected, Destructive
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UIPressPhase : long {
		Began,
		Changed,
		Stationary,
		Ended,
		Cancelled
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum UIPressType : long {
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
	public enum UITableViewCellFocusStyle : long {
		Default,
		Custom
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UIDisplayGamut : long
	{
		Unspecified = -1,
		Srgb,
		P3
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UITraitEnvironmentLayoutDirection : long
	{
		Unspecified = -1,
		LeftToRight = UIUserInterfaceLayoutDirection.LeftToRight,
		RightToLeft = UIUserInterfaceLayoutDirection.RightToLeft
	}

	[TV (10,0), NoWatch, iOS (12,0)]
	[Native]
	public enum UIUserInterfaceStyle : long
	{
		Unspecified,
		Light,
		Dark
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UITextItemInteraction : long
	{
		InvokeDefaultAction,
		PresentActions,
		Preview
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UIViewAnimatingState : long
	{
		Inactive,
		Active,
		Stopped
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UIViewAnimatingPosition : long
	{
		End,
		Start,
		Current
	}

	[NoWatch]
	[iOS (10,0)]
	[Native]
	public enum UITimingCurveType : long
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
	public enum UIAccessibilityHearingDeviceEar : ulong {
		None = 0,
		Left = 1 << 1,
		Right = 1 << 2,
		Both = Left | Right
	}

	[NoWatch]
	[iOS(10,0)]
	[Native]
	public enum UIAccessibilityCustomRotorDirection : long
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
	public enum UICloudSharingPermissionOptions : ulong {
		Standard = 0,
		AllowPublic = 1 << 0,
		AllowPrivate = 1 << 1,
		AllowReadOnly = 1 << 2,
		AllowReadWrite = 1 << 3
	}

	[iOS (10,0), TV (10,0), NoWatch]
	[Native]
	public enum UITextFieldDidEndEditingReason : long {
		Unknown = -1, // helper value (not in headers)
		Committed,
		[NoiOS]
		Cancelled
	}

	[iOS (10,3), TV (10,2), NoWatch]
	[Native]
	public enum UIScrollViewIndexDisplayMode : long {
		Automatic,
		AlwaysHidden
	}

	[NoWatch]
	[TV (11,0), iOS (11,0)]
	[Native]
	public enum UIScrollViewContentInsetAdjustmentBehavior : long
	{
		Automatic,
		ScrollableAxes,
		Never,
		Always
	}
	
	[iOS (11,0), TV (11,0), Watch (4,0)]
	[Native]
	public enum UIAccessibilityContainerType : long
	{
		None = 0,
		DataTable,
		List,
		Landmark
	}
	
	[NoWatch]
	[iOS (11,0), TV (11,0)]
	[Native]
	public enum UITextSmartQuotesType : long
	{
		Default,
		No,
		Yes
	}
	
	[NoWatch]
	[iOS (11,0), TV (11,0)]
	[Native]
	public enum UITextSmartDashesType : long
	{
		Default,
		No,
		Yes
	}
	
	[NoWatch]
	[iOS (11,0), TV (11,0)]
	[Native]
	public enum UITextSmartInsertDeleteType : long
	{
		Default,
		No,
		Yes
	}
	
	[NoWatch]
	[iOS (11,0), TV (11,0)]
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
	
	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UIDropOperation : ulong
	{
		Cancel = 0,
		Forbidden = 1,
		Copy = 2,
		Move = 3
	}

	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	[Flags]
	public enum UITextDragOptions : long
	{
		None = 0,
		StripTextColorFromPreviews = (1 << 0)
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UITextDropAction : ulong
	{
		Insert = 0,
		ReplaceSelection,
		ReplaceAll
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UITextDropProgressMode : ulong
	{
		System = 0,
		Custom
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UITextDropEditability : ulong
	{
		No = 0,
		Temporary,
		Yes
	}

	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UICollectionViewReorderingCadence : long
	{
		Immediate,
		Fast,
		Slow
	}

	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UICollectionViewDropIntent : long
	{
		Unspecified,
		InsertAtDestinationIndexPath,
		InsertIntoDestinationIndexPath
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UICollectionViewCellDragState : long
	{
		None,
		Lifting,
		Dragging
	}
	
	[NoWatch]
	[NoTV, iOS (11,0)]
	[Native]
	public enum UIImagePickerControllerImageUrlExportPreset : long
	{
		Compatible = 0,
		Current
	}
	
	[NoWatch]
	[NoTV, iOS (11,0)]
	[Native]
	public enum UIContextualActionStyle : long
	{
		Normal,
		Destructive
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UITableViewCellDragState : long
	{
		None,
		Lifting,
		Dragging
	}
	
	[NoWatch]
	[TV (11,0), iOS (11,0)]
	[Native]
	public enum UITableViewSeparatorInsetReference : long
	{
		CellEdges,
		AutomaticInsets
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UITableViewDropIntent : long
	{
		Unspecified,
		InsertAtDestinationIndexPath,
		InsertIntoDestinationIndexPath,
		Automatic
	}
	
	[NoWatch]
	[TV (11,0), iOS (11,0)]
	[Native]
	public enum UISplitViewControllerPrimaryEdge : long
	{
		Leading,
		Trailing
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UIDropSessionProgressIndicatorStyle : ulong
	{
		None,
		Default
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum UISpringLoadedInteractionEffectState : long
	{
		Inactive,
		Possible,
		Activating,
		Activated
	}

	[NoWatch]
	[NoTV, iOS (11,0)]
	[Native]
	public enum UIDocumentBrowserImportMode : ulong
	{
		None,
		Copy,
		Move
	}
	
	[NoWatch]
	[NoTV, iOS (11,0)]
	[Native]
	public enum UIDocumentBrowserUserInterfaceStyle : ulong
	{
		White = 0,
		Light,
		Dark
	}
	
	[NoWatch]
	[NoTV, iOS (11,0)]
	[Native]
	[Flags]
	public enum UIDocumentBrowserActionAvailability : long
	{
		Menu = 1,
		NavigationBar = 1 << 1
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[Native]
	public enum UITextDropPerformer : ulong
	{
		View = 0,
		Delegate,
	}

	[NoWatch]
	[iOS (11,0), TV (11,0)]
	[Native]
	public enum UINavigationItemLargeTitleDisplayMode : long
	{
		Automatic,
		Always,
		Never,
	}

	[NoWatch]
	[iOS (11,0), TV (11,0)]
	[Native]
	public enum UICollectionViewFlowLayoutSectionInsetReference : long
	{
		ContentInset,
		SafeArea,
		LayoutMargins,
	}

	[NoWatch][NoTV]
	[iOS (11,0)]
	[Native]
	public enum UIPreferredPresentationStyle : long
	{
		Unspecified = 0,
		Inline,
		Attachment,
	}

	[NoWatch, NoTV, NoMac, iOS (11,0)]
	[Native]
	[ErrorDomain ("UIDocumentBrowserErrorDomain")]
	public enum UIDocumentBrowserErrorCode : long
	{
		Generic = 1,
	}
	
	[iOS (12,0), TV (12,0), NoWatch]
	[Native]
	public enum UIGraphicsImageRendererFormatRange : long
	{
		Unspecified = -1,
		Automatic = 0,
		Extended,
		Standard,
	}

	[iOS (12,0), NoTV, NoWatch]
	[Native]
	public enum UIPrintErrorCode : long
	{
		ingNotAvailableError = 1,
		NoContentError,
		UnknownImageFormatError,
		JobFailedError
	}
	
}
