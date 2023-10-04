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
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UIActivityIndicatorViewStyle : long {
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Large' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Large' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Large' instead.")]
		WhiteLarge,
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Medium' instead.")]
		White,
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Medium' instead.")]
		Gray,

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		Medium = 100,
		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		Large = 101,
	}

	// NSInteger -> UIAlertView.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIAlertViewStyle : long {
		Default,
		SecureTextInput,
		PlainTextInput,
		LoginAndPasswordInput
	}

	// NSInteger -> UIBarButtonItem.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIBarButtonItemStyle : long {
		Plain,

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UIBarButtonItemStyle.Plain' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UIBarButtonItemStyle.Plain' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarButtonItemStyle.Plain' instead.")]
		Bordered,

		Done,
	}

	// NSInteger -> UIBarButtonItem.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		PageCurl,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		Close,
	}

	// NSUInteger -> UIControl.h
	[Native ("UIControlEvents")]
	[Flags]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIControlEvent : ulong {
		TouchDown = 1 << 0,
		TouchDownRepeat = 1 << 1,
		TouchDragInside = 1 << 2,
		TouchDragOutside = 1 << 3,
		TouchDragEnter = 1 << 4,
		TouchDragExit = 1 << 5,
		TouchUpInside = 1 << 6,
		TouchUpOutside = 1 << 7,
		TouchCancel = 1 << 8,

		ValueChanged = 1 << 12,
		PrimaryActionTriggered = 1 << 13,
		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		MenuActionTriggered = 1 << 14,

		EditingDidBegin = 1 << 16,
		EditingChanged = 1 << 17,
		EditingDidEnd = 1 << 18,
		EditingDidEndOnExit = 1 << 19,

		AllTouchEvents = 0x00000FFF,
		AllEditingEvents = 0x000F0000,
		ApplicationReserved = 0x0F000000,
		SystemReserved = 0xF0000000,
		AllEvents = 0xFFFFFFFF
	}

	// NSInteger -> UIEvent.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIEventType : long {
		Touches,
		Motion,
		RemoteControl,
		[iOS (9, 0)]
		[MacCatalyst (13, 1)]
		Presses,
		[iOS (13, 4), TV (13, 4)]
		[MacCatalyst (13, 1)]
		Scroll = 10,
		[iOS (13, 4), TV (13, 4)]
		[MacCatalyst (13, 1)]
		Hover = 11,
		[iOS (13, 4), TV (13, 4)]
		[MacCatalyst (13, 1)]
		Transform = 14,
	}

	// NSInteger -> UIEvent.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIEventSubtype : long {
		None,
		MotionShake,

		RemoteControlPlay = 100,
		RemoteControlPause = 101,
		RemoteControlStop = 102,
		RemoteControlTogglePlayPause = 103,
		RemoteControlNextTrack = 104,
		RemoteControlPreviousTrack = 105,
		RemoteControlBeginSeekingBackward = 106,
		RemoteControlEndSeekingBackward = 107,
		RemoteControlBeginSeekingForward = 108,
		RemoteControlEndSeekingForward = 109,
	}

	// NSInteger -> UIControl.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIControlContentVerticalAlignment : long {
		Center = 0,
		Top = 1,
		Bottom = 2,
		Fill = 3,
	}

	// NSInteger -> UIControl.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIControlContentHorizontalAlignment : long {
		Center = 0,
		Left = 1,
		Right = 2,
		Fill = 3,
		Leading = 4,
		Trailing = 5
	}

	// NSUInteger -> UIControl.h
	[Native]
	[Flags]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIControlState : ulong {
		Normal = 0,
		Highlighted = 1 << 0,
		Disabled = 1 << 1,
		Selected = 1 << 2,
		[iOS (9, 0)]
		[MacCatalyst (13, 1)]
		Focused = 1 << 3,
		Application = 0x00FF0000,
		Reserved = 0xFF000000
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
	[MacCatalyst (13, 1)]
	public enum UIViewAutoresizing : ulong {
		None = 0,
		FlexibleLeftMargin = 1 << 0,
		FlexibleWidth = 1 << 1,
		FlexibleRightMargin = 1 << 2,
		FlexibleTopMargin = 1 << 3,
		FlexibleHeight = 1 << 4,
		FlexibleBottomMargin = 1 << 5,
		FlexibleMargins = FlexibleBottomMargin | FlexibleTopMargin | FlexibleLeftMargin | FlexibleRightMargin,
		FlexibleDimensions = FlexibleHeight | FlexibleWidth,
		All = 63
	}

	// NSInteger -> UIView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIViewAnimationCurve : long {
		EaseInOut,
		EaseIn,
		EaseOut,
		Linear
	}

	// NSInteger -> UIView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UIBarMetrics : long {
		Default,
		Compact,
		DefaultPrompt = 101,
		CompactPrompt,

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UIBarMetrics.Compat' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UIBarMetrics.Compat' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarMetrics.Compat' instead.")]
		LandscapePhone = Compact,

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UIBarMetrics.CompactPrompt' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UIBarMetrics.CompactPrompt' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarMetrics.CompactPrompt' instead.")]
		LandscapePhonePrompt = CompactPrompt
	}

	// NSInteger -> UIButton.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIButtonType : long {
		Custom,
		RoundedRect,
		DetailDisclosure,
		InfoLight,
		InfoDark,
		ContactAdd,
		[TV (11, 0)]
		[NoiOS]
		[NoMacCatalyst]
		Plain,
		[NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
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
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIBaselineAdjustment : long {
		AlignBaselines = 0,
		AlignCenters,
		None,
	}

	// NSInteger -> UIDatePicker.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIDatePickerMode : long {
		Time,
		Date,
		DateAndTime,
		CountDownTimer
	}

	// NSInteger -> UIDevice.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIDeviceBatteryState : long {
		Unknown,
		Unplugged,
		Charging,
		Full,
	}

	// NSInteger -> UIDocument.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIDocumentChangeKind : long {
		Done, Undone, Redone, Cleared
	}

	// NSInteger -> UIDocument.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIDocumentSaveOperation : long {
		ForCreating, ForOverwriting
	}

	// NSUInteger -> UIDocument.h
	[Native]
	[Flags]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIImagePickerControllerSourceType : long {
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'PHPicker' instead.")]
		PhotoLibrary,
		Camera,
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'PHPicker' instead.")]
		SavedPhotosAlbum,
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIImagePickerControllerCameraCaptureMode : long {
		Photo, Video
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIImagePickerControllerCameraDevice : long {
		Rear,
		Front
	}

	// NSInteger -> UIImagePickerController.h
	[Native]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIImagePickerControllerCameraFlashMode : long {
		Off = -1, Auto = 0, On = 1
	}

	// NSInteger -> UIInterface.h
	[Native]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIBarStyle : long {
		Default,
		Black,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
		[Deprecated (PlatformName.iOS, 5, 1, message: "Use 'UIBarStyle.Black'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarStyle.Black'.")]
		BlackOpaque = 1,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
		[Deprecated (PlatformName.iOS, 5, 1, message: "Use 'UIBarStyle.Black' and set the translucency property to true.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarStyle.Black' and set the translucency property to true.")]
		BlackTranslucent = 2,
	}

	// NSInteger -> UIProgressView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIProgressViewStyle : long {
		Default,
		[NoTV]
		[MacCatalyst (13, 1)]
		Bar,
	}

	// NSInteger -> UIScrollView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIScrollViewIndicatorStyle : long {
		Default,
		Black,
		White
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITextAutocapitalizationType : long {
		None,
		Words,
		Sentences,
		AllCharacters,
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITextAutocorrectionType : long {
		Default,
		No,
		Yes,
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		AsciiCapableNumberPad
	}

	// NSInteger -> UISegmentedControl.h
	[Native]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 7, 0, message: "This no longer has any effect.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This no longer has any effect.")]
	public enum UISegmentedControlStyle : long {
		Plain,
		Bordered,
		Bar,
		Bezeled
	}

	// NSInteger -> UITabBarItem.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UITableViewStyle : long {
		Plain,
		Grouped,
		[NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		InsetGrouped,
	}

	// NSInteger -> UITableView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITableViewScrollPosition : long {
		None,
		Top,
		Middle,
		Bottom
	}

	// NSInteger -> UITableView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIToolbarPosition : long {
		Any, Bottom, Top
	}

	// NSInteger -> UITouch.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITouchPhase : long {
		Began,
		Moved,
		Stationary,
		Ended,
		Cancelled,
		[iOS (13, 4), TV (13, 4)]
		[MacCatalyst (13, 1)]
		RegionEntered,
		[iOS (13, 4), TV (13, 4)]
		[MacCatalyst (13, 1)]
		RegionMoved,
		[iOS (13, 4), TV (13, 4)]
		[MacCatalyst (13, 1)]
		RegionExited,
	}

	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITouchType : long {
		Direct,
		Indirect,
		Stylus,
		[iOS (13, 4), TV (13, 4)]
		[MacCatalyst (13, 1)]
		IndirectPointer,
	}

	[NoWatch]
	[iOS (9, 1)]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UITouchProperties : long {
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
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellStyle : long {
		Default,
		Value1,
		Value2,
		Subtitle
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellSeparatorStyle : long {
		None,
		SingleLine,
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SingleLine' for a single line separator.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SingleLine' for a single line separator.")]
		SingleLineEtched,
		DoubleLineEtched = SingleLineEtched
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellSelectionStyle : long {
		None,
		Blue,
		Gray,
		Default
	}

	// NSInteger -> UITableViewCell.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellEditingStyle : long {
		None,
		Delete,
		Insert
	}

	// NSInteger -> UITableViewCell.h
	[Native ("UITableViewCellAccessoryType")]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellAccessory : long {
		None,
		DisclosureIndicator,
		[NoTV]
		[MacCatalyst (13, 1)]
		DetailDisclosureButton,
		Checkmark,
		[NoTV]
		[MacCatalyst (13, 1)]
		DetailButton
	}

	// NSUInteger -> UITableViewCell.h
	[Native ("UITableViewCellStateMask")]
	[Flags]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellState : ulong {
		DefaultMask = 0,
		ShowingEditControlMask = 1 << 0,
		ShowingDeleteConfirmationMask = 1 << 1
	}

	// NSInteger -> UITextField.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITextBorderStyle : long {
		None,
		Line,
		Bezel,
		RoundedRect
	}

	// NSInteger -> UITextField.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITextFieldViewMode : long {
		Never,
		WhileEditing,
		UnlessEditing,
		Always
	}

	// NSInteger -> UIViewController.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIModalTransitionStyle : long {
		CoverVertical = 0,
		[NoTV]
		[MacCatalyst (13, 1)]
		FlipHorizontal,
		CrossDissolve,
		[NoTV]
		[MacCatalyst (13, 1)]
		PartialCurl
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
	[MacCatalyst (13, 1)]
	public enum UIInterfaceOrientation : long {
		Unknown = UIDeviceOrientation.Unknown,
		Portrait = UIDeviceOrientation.Portrait,
		PortraitUpsideDown = UIDeviceOrientation.PortraitUpsideDown,
		LandscapeLeft = UIDeviceOrientation.LandscapeRight,
		LandscapeRight = UIDeviceOrientation.LandscapeLeft
	}

	// NSUInteger -> UIApplication.h
	[Native]
	[Flags]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIWebViewNavigationType : long {
		LinkClicked,
		FormSubmitted,
		BackForward,
		Reload,
		FormResubmitted,
		Other
	}

	// NSUInteger -> UIApplication.h
	[Native ("UIDataDetectorTypes")]
	[Flags]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIDataDetectorType : ulong {
		PhoneNumber = 1 << 0,
		Link = 1 << 1,
		Address = 1 << 2,
		CalendarEvent = 1 << 3,

		[iOS (10, 0)]
		[MacCatalyst (13, 1)]
		ShipmentTrackingNumber = 1 << 4,
		[iOS (10, 0)]
		[MacCatalyst (13, 1)]
		FlightNumber = 1 << 5,
		[iOS (10, 0)]
		[MacCatalyst (13, 1)]
		LookupSuggestion = 1 << 6,
		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		Money = 1 << 7,
		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		PhysicalValue = 1uL << 8,

		None = 0,
		All = UInt64.MaxValue
	}

	// NSInteger -> UIActionSheet.h
	[Native]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	public enum UIActionSheetStyle : long {
		Automatic = -1,
		Default = UIBarStyle.Default,
		BlackTranslucent = 2, // UIBarStyle.BlackTranslucent,
		BlackOpaque = 1 // UIBarStyle.BlackOpaque,
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIStatusBarStyle : long {
		Default,

		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LightContent' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LightContent' instead.")]
		BlackTranslucent = 1,

		LightContent = 1,

		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LightContent' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LightContent' instead.")]
		BlackOpaque = 2,

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		DarkContent = 3,
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIStatusBarAnimation : long {
		None,
		Fade,
		Slide
	}

	// NSInteger -> UIGestureRecognizer.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIRemoteNotificationType : ulong {
		None = 0,
		Badge = 1 << 0,
		Sound = 1 << 1,
		Alert = 1 << 2,
		NewsstandContentAvailability = 1 << 3
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIKeyboardAppearance : long {
		Default,
		Alert,
		Dark = Alert,
		Light
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UIModalPresentationStyle : long {
		None = -1,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Automatic = -2,
		FullScreen = 0,
		[NoTV]
		[MacCatalyst (13, 1)]
		PageSheet,
		[NoTV]
		[MacCatalyst (13, 1)]
		FormSheet,
		CurrentContext,
		Custom,
		OverFullScreen,
		OverCurrentContext,
		[NoTV]
		[MacCatalyst (13, 1)]
		Popover,
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		BlurOverFullScreen,
	}

	// NSUInteger -> UISwipeGestureRecognizer.h
	[Native]
	[Flags]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UIPopoverArrowDirection : ulong {
		Up = 1 << 0,
		Down = 1 << 1,
		Left = 1 << 2,
		Right = 1 << 3,
		Any = Up | Down | Left | Right,
		Unknown = UInt64.MaxValue
	};

	// NSInteger -> UIMenuController.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
		TopLeft = 1 << 0,
		TopRight = 1 << 1,
		BottomLeft = 1 << 2,
		BottomRight = 1 << 3,
		AllCorners = ~(ulong) 0
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIUserInterfaceLayoutDirection : long {
		LeftToRight, RightToLeft
	}

	// NSInteger -> UIDevice.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIUserInterfaceIdiom : long {
		Unspecified = -1,
		Phone,
		Pad,
		TV,
		CarPlay,
		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Mac = 5,
	}

	// NSInteger -> UIApplication.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIApplicationState : long {
		Active,
		Inactive,
		Background
	}

	// NSInteger -> UIView.h
	[Native]
	[Flags]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIViewAnimationOptions : ulong {
		LayoutSubviews = 1 << 0,
		AllowUserInteraction = 1 << 1,
		BeginFromCurrentState = 1 << 2,
		Repeat = 1 << 3,
		Autoreverse = 1 << 4,
		OverrideInheritedDuration = 1 << 5,
		OverrideInheritedCurve = 1 << 6,
		AllowAnimatedContent = 1 << 7,
		ShowHideTransitionViews = 1 << 8,
		OverrideInheritedOptions = 1 << 9,

		CurveEaseInOut = 0 << 16,
		CurveEaseIn = 1 << 16,
		CurveEaseOut = 2 << 16,
		CurveLinear = 3 << 16,

		TransitionNone = 0 << 20,
		TransitionFlipFromLeft = 1 << 20,
		TransitionFlipFromRight = 2 << 20,
		TransitionCurlUp = 3 << 20,
		TransitionCurlDown = 4 << 20,
		TransitionCrossDissolve = 5 << 20,
		TransitionFlipFromTop = 6 << 20,
		TransitionFlipFromBottom = 7 << 20,

		[iOS (10, 3)]
		[MacCatalyst (13, 1)]
		PreferredFramesPerSecondDefault = 0 << 24,
		[iOS (10, 3)]
		[MacCatalyst (13, 1)]
		PreferredFramesPerSecond60 = 3 << 24,
		[iOS (10, 3)]
		[MacCatalyst (13, 1)]
		PreferredFramesPerSecond30 = 7 << 24,
	}

	// untyped (and unamed) enum -> UIPrintError.h
	// note: it looks unused by any API
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("UIPrintErrorDomain")]
	public enum UIPrintError {
		NotAvailable = 1,
		NoContent,
		UnknownImageFormat,
		JobFailed,
	}

	// NSInteger -> UIPrintInfo.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIPrintInfoDuplex : long {
		None,
		LongEdge,
		ShortEdge,
	}

	// NSInteger -> UIPrintInfo.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIPrintInfoOrientation : long {
		Portrait,
		Landscape,
	}

	// NSInteger -> UIPrintInfo.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIPrintInfoOutputType : long {
		General,
		Photo,
		Grayscale,
		PhotoGrayscale
	}

	// NSInteger -> UIAccessibility.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UIScreenOverscanCompensation : long {
		Scale, InsetBounds,
		None,
		[Obsolete ("Use 'UIScreenOverscanCompensation.None' instead.")]
		InsetApplicationFrame = None
	}

	// NSInteger -> UISegmentedControl.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UISegmentedControlSegment : long {
		Any, Left, Center, Right, Alone
	}

	// NSInteger -> UISearchBar.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UISearchBarIcon : long {
		Search,
		[NoTV]
		[MacCatalyst (13, 1)]
		Clear,
		[NoTV]
		[MacCatalyst (13, 1)]
		Bookmark,
		[NoTV]
		[MacCatalyst (13, 1)]
		ResultsList
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIPageViewControllerNavigationOrientation : long {
		Horizontal, Vertical
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIPageViewControllerSpineLocation : long {
		None, Min, Mid, Max
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIPageViewControllerNavigationDirection : long {
		Forward, Reverse
	}

	// NSInteger -> UIPageViewController.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIPageViewControllerTransitionStyle : long {
		PageCurl, Scroll
	}

	// NSInteger -> UITextInputTraits.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITextSpellCheckingType : long {
		Default, No, Yes,
	}

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITextStorageDirection : long {
		Forward, Backward
	}

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UITextDirection : long {
		Forward, Backward, Right, Left, Up, Down
	}

#if !NET
	// NSInteger -> UITextInput.h
	// Use Foundation.NSWritingDirection in .NET.
	// see: https://github.com/xamarin/xamarin-macios/issues/6573
	[Native]
	[NoWatch]
	public enum UITextWritingDirection : long {
		Natural = -1,
		LeftToRight,
		RightToLeft,
	}
#endif

	// NSInteger -> UITextInput.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UILayoutPriority {
		Required = 1000,
		DefaultHigh = 750,
		DefaultLow = 250,
		FittingSizeLevel = 50,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		DragThatCanResizeScene = 510,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		SceneSizeStayPut = 500,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		DragThatCannotResizeScene = 490,
	}

	// NSInteger -> NSLayoutConstraint.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UICollectionUpdateAction : long {
		Insert, Delete, Reload, Move, None
	}

	// NSUInteger -> UICollectionView.h
	[Native]
	[Flags]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UICollectionViewScrollDirection : long {
		Vertical, Horizontal
	}

	// NSInteger -> UICollectionViewFlowLayout.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UICollectionElementCategory : ulong {
		Cell, SupplementaryView, DecorationView
	}

	// that's a convenience enum that maps to UICollectionElementKindSection[Footer|Header] which are NSString
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIInterpolatingMotionEffectType : long {
		TiltAlongHorizontalAxis,
		TiltAlongVerticalAxis
	}

	// NSInteger -> UINavigationController.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UINavigationControllerOperation : long {
		None, Push, Pop
	}

	// NSInteger -> UIActivity.h
	[Native]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIActivityCategory : long {
		Action, Share
	}

	// NSInteger -> UIAttachmentBehavior.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIAttachmentBehaviorType : long {
		Items, Anchor
	}

	// NSInteger -> UIBarCommon.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIBarPosition : long {
		Any, Bottom, Top, TopAttached,
	}

	// NSUInteger -> UICollisionBehavior.h
	[Native]
	[Flags]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UICollisionBehaviorMode : ulong {
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
	[Native]
	[Flags]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIKeyModifierFlags : long {
		AlphaShift = 1 << 16,  // This bit indicates CapsLock
		Shift = 1 << 17,
		Control = 1 << 18,
		Alternate = 1 << 19,
		Command = 1 << 20,
		NumericPad = 1 << 21,
	}

	// NSInteger -> UIScrollView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIScrollViewKeyboardDismissMode : long {
		None,
		OnDrag,
		Interactive,
		[TV (16, 0)] // Added in Xcode 14.0, but headers and documentation say it's available in iOS 7+ and Mac Catalyst 13.1+ (and tvOS 16.0)
		OnDragWithAccessory,
		[TV (16, 0)] // Added in Xcode 14.0, but headers and documentation say it's available in iOS 7+ and Mac Catalyst 13.1+ (and tvOS 16.0)
		InteractiveWithAccessory,
	}

	// NSInteger -> UIWebView.h
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIWebPaginationBreakingMode : long {
		Page, Column
	}

	// NSInteger -> UIWebView.h
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UIPushBehaviorMode : long {
		Continuous,
		Instantaneous
	}

	// NSInteger -> UITabBar.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITabBarItemPositioning : long {
		Automatic,
		Fill,
		Centered
	}

	// NSUInteger -> UIView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	public enum UIViewTintAdjustmentMode : long {
		Automatic,
		Normal,
		Dimmed
	}

	// NSUInteger -> UIView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	public enum NSTextEffect {
		None,
		LetterPressStyle,

		// An unkonwn value, the real value can be fetched using the WeakTextEffect: Apple added a new effect and the bindings are old.
		UnknownUseWeakEffect
	}

	// NSUInteger -> UISearchBar.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UISearchBarStyle : ulong {
		Default,
		Prominent,
		Minimal
	}

	// NSInteger -> UIInputView.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIInputViewStyle : long {
		Default,
		Keyboard
	}

	[Native]
	[NoWatch]
	[iOS (8, 0)]
	[MacCatalyst (13, 1)]
	public enum UIUserInterfaceSizeClass : long {
		Unspecified = 0,
		Compact = 1,
		Regular = 2
	}

	[Native]
	[NoWatch]
	[iOS (8, 0)]
	[MacCatalyst (13, 1)]
	public enum UIAlertActionStyle : long {
		Default, Cancel, Destructive
	}

	[Native]
	[NoWatch]
	[iOS (8, 0)]
	[MacCatalyst (13, 1)]
	public enum UIAlertControllerStyle : long {
		ActionSheet,
		Alert
	}

	[Native]
	[NoWatch]
	[iOS (8, 0)]
	[MacCatalyst (13, 1)]
	public enum UIBlurEffectStyle : long {
		ExtraLight, Light, Dark,
		[TV (10, 0), NoiOS, NoWatch]
		[NoMacCatalyst]
		ExtraDark,
		[iOS (10, 0)]
		[MacCatalyst (13, 1)]
		Regular = 4,
		[iOS (10, 0)]
		[MacCatalyst (13, 1)]
		Prominent = 5,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemUltraThinMaterial,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemThinMaterial,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemMaterial,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemThickMaterial,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemChromeMaterial,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemUltraThinMaterialLight,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemThinMaterialLight,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemMaterialLight,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemThickMaterialLight,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemChromeMaterialLight,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemUltraThinMaterialDark,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemThinMaterialDark,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemMaterialDark,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemThickMaterialDark,
		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		SystemChromeMaterialDark,
	}

	[Native]
	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
	[MacCatalyst (13, 1)]
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

	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNAuthorizationOptions' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNAuthorizationOptions' instead.")]
	[Native]
	[Flags]
	public enum UIUserNotificationType : ulong {
		None = 0,
		Badge = 1 << 0,
		Sound = 1 << 1,
		Alert = 1 << 2
	}

	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationActionOptions' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNNotificationActionOptions' instead.")]
	[Native]
	public enum UIUserNotificationActivationMode : ulong {
		Foreground,
		Background
	}

	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationCategory.Actions' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNNotificationCategory.Actions' instead.")]
	[Native]
	public enum UIUserNotificationActionContext : ulong {
		Default,
		Minimal
	}

	[Deprecated (PlatformName.iOS, 11, 0)]
	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum UIDocumentMenuOrder : ulong {
		First,
		Last
	}

	[Deprecated (PlatformName.iOS, 14, 0, message: "Use the designated constructors instead.")]
	[NoTV]
	[NoWatch]
	[iOS (8, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the designated constructors instead.")]
	[Native]
	public enum UIDocumentPickerMode : ulong {
		Import,
		Open,
		ExportToService,
		MoveToService
	}

	[iOS (8, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityNavigationStyle : long {

		Automatic = 0,
		Separate = 1,
		Combined = 2
	}

	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UISplitViewControllerDisplayMode : long {
		Automatic,
		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		SecondaryOnly,
		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		OneBesideSecondary,
		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		OneOverSecondary,
		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		TwoBesideSecondary,
		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		TwoOverSecondary,
		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		TwoDisplaceSecondary,

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'SecondaryOnly' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'SecondaryOnly' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'SecondaryOnly' instead.")]
		PrimaryHidden = SecondaryOnly,

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'OneBesideSecondary' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'OneBesideSecondary' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'OneBesideSecondary' instead.")]
		AllVisible = OneBesideSecondary,

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'OneOverSecondary' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'OneOverSecondary' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'OneOverSecondary' instead.")]
		PrimaryOverlay = OneOverSecondary,
	}

	[Native]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'UIContextualActionStyle' and corresponding APIs instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIContextualActionStyle' and corresponding APIs instead.")]
	public enum UITableViewRowActionStyle : long {
		Default, Destructive = Default, Normal
	}

	// Utility enum for UITransitionContext[To|From]ViewKey
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UITransitionViewControllerKind {
		ToView, FromView
	}

	// note [Native] since it maps to UIFontWeightConstants fields (CGFloat)
	[iOS (8, 2)]
	[MacCatalyst (13, 1)]
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

	[Watch (9, 0), TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	public enum UIFontWidth {
		Condensed,
		Standard,
		Expanded,
		Compressed,
	}

	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIStackViewDistribution : long {
		Fill,
		FillEqually,
		FillProportionally,
		EqualSpacing,
		EqualCentering
	}

	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum NSWritingDirectionFormatType : long {
		Embedding = 0 << 1,
		Override = 1 << 1
	}

	[NoTV]
	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPrinterCutterBehavior : long {
		NoCut,
		PrinterDefault,
		CutAfterEachPage,
		CutAfterEachCopy,
		CutAfterEachJob
	}

	[NoTV]
	[NoWatch]
	[iOS (9, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationAction' or 'UNTextInputNotificationAction' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNNotificationAction' or 'UNTextInputNotificationAction' instead.")]
	[Native]
	public enum UIUserNotificationActionBehavior : ulong {
		Default,
		TextInput
	}

	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UISemanticContentAttribute : long {
		Unspecified = 0,
		Playback,
		Spatial,
		ForceLeftToRight,
		ForceRightToLeft
	}

	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDynamicItemCollisionBoundsType : ulong {
		Rectangle,
		Ellipse,
		Path
	}

	[Native]
	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	public enum UIForceTouchCapability : long {
		Unknown = 0,
		Unavailable = 1,
		Available = 2
	}

	[Native]
	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	public enum UIPreviewActionStyle : long {
		Default, Selected, Destructive
	}

	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPressPhase : long {
		Began,
		Changed,
		Stationary,
		Ended,
		Cancelled
	}

	[NoWatch]
	[iOS (9, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPressType : long {
		UpArrow,
		DownArrow,
		LeftArrow,
		RightArrow,
		Select,
		Menu,
		PlayPause,
		[TV (14, 3)]
		[NoiOS]
		[NoMacCatalyst]
		PageUp = 30,
		[TV (14, 3)]
		[NoiOS]
		[NoMacCatalyst]
		PageDown = 31,
	}

	[NoWatch]
	[iOS (9, 0)] // introduced in Xcode 7.1 SDK (iOS 9.1 but hidden in 9.0)
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITableViewCellFocusStyle : long {
		Default,
		Custom
	}

	[NoWatch]
	[iOS (10, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDisplayGamut : long {
		Unspecified = -1,
		Srgb,
		P3
	}

	[NoWatch]
	[iOS (10, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITraitEnvironmentLayoutDirection : long {
		Unspecified = -1,
		LeftToRight = UIUserInterfaceLayoutDirection.LeftToRight,
		RightToLeft = UIUserInterfaceLayoutDirection.RightToLeft
	}

	[TV (10, 0), NoWatch, iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIUserInterfaceStyle : long {
		Unspecified,
		Light,
		Dark
	}

	[NoWatch]
	[iOS (10, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextItemInteraction : long {
		InvokeDefaultAction,
		PresentActions,
		Preview
	}

	[NoWatch]
	[iOS (10, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIViewAnimatingState : long {
		Inactive,
		Active,
		Stopped
	}

	[NoWatch]
	[iOS (10, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIViewAnimatingPosition : long {
		End,
		Start,
		Current
	}

	[NoWatch]
	[iOS (10, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITimingCurveType : long {
		Builtin,
		Cubic,
		Spring,
		Composed
	}

	[NoWatch]
	[NoTV]
	[iOS (10, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityHearingDeviceEar : ulong {
		None = 0,
		Left = 1 << 1,
		Right = 1 << 2,
		Both = Left | Right
	}

	[NoWatch]
	[iOS (10, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityCustomRotorDirection : long {
		Previous,
		Next
	}

#if NET
	[NoTV]
#else
	// Xcode 8.2 beta 1 added __TVOS_PROHIBITED but we need to keep it for binary compatibility
	[TV (10, 0)]
#endif
	[iOS (10, 0)]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UICloudSharingPermissionOptions : ulong {
		Standard = 0,
		AllowPublic = 1 << 0,
		AllowPrivate = 1 << 1,
		AllowReadOnly = 1 << 2,
		AllowReadWrite = 1 << 3
	}

	[iOS (10, 0), TV (10, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextFieldDidEndEditingReason : long {
		Unknown = -1, // helper value (not in headers)
		Committed,
		[NoiOS]
		[NoMacCatalyst]
		Cancelled
	}

	[iOS (10, 3), TV (10, 2), NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIScrollViewIndexDisplayMode : long {
		Automatic,
		AlwaysHidden
	}

	[NoWatch]
	[TV (11, 0), iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIScrollViewContentInsetAdjustmentBehavior : long {
		Automatic,
		ScrollableAxes,
		Never,
		Always
	}

	[iOS (11, 0), TV (11, 0), Watch (4, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityContainerType : long {
		None = 0,
		DataTable,
		List,
		Landmark,
		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		SemanticGroup,
	}

	[NoWatch]
	[iOS (11, 0), TV (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextSmartQuotesType : long {
		Default,
		No,
		Yes
	}

	[NoWatch]
	[iOS (11, 0), TV (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextSmartDashesType : long {
		Default,
		No,
		Yes
	}

	[NoWatch]
	[iOS (11, 0), TV (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextSmartInsertDeleteType : long {
		Default,
		No,
		Yes
	}

	[NoWatch]
	[iOS (11, 0), TV (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityCustomSystemRotorType : long {
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

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDropOperation : ulong {
		Cancel = 0,
		Forbidden = 1,
		Copy = 2,
		Move = 3
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UITextDragOptions : long {
		None = 0,
		StripTextColorFromPreviews = (1 << 0)
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextDropAction : ulong {
		Insert = 0,
		ReplaceSelection,
		ReplaceAll
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextDropProgressMode : ulong {
		System = 0,
		Custom
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextDropEditability : ulong {
		No = 0,
		Temporary,
		Yes
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UICollectionViewReorderingCadence : long {
		Immediate,
		Fast,
		Slow
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UICollectionViewDropIntent : long {
		Unspecified,
		InsertAtDestinationIndexPath,
		InsertIntoDestinationIndexPath
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UICollectionViewCellDragState : long {
		None,
		Lifting,
		Dragging
	}

	[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
	[NoWatch]
	[NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'PHPicker' instead.")]
	[Native]
	public enum UIImagePickerControllerImageUrlExportPreset : long {
		Compatible = 0,
		Current
	}

	[NoWatch]
	[NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIContextualActionStyle : long {
		Normal,
		Destructive
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITableViewCellDragState : long {
		None,
		Lifting,
		Dragging
	}

	[NoWatch]
	[TV (11, 0), iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITableViewSeparatorInsetReference : long {
		CellEdges,
		AutomaticInsets
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITableViewDropIntent : long {
		Unspecified,
		InsertAtDestinationIndexPath,
		InsertIntoDestinationIndexPath,
		Automatic
	}

	[NoWatch]
	[TV (11, 0), iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UISplitViewControllerPrimaryEdge : long {
		Leading,
		Trailing
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDropSessionProgressIndicatorStyle : ulong {
		None,
		Default
	}

	[NoWatch, NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UISpringLoadedInteractionEffectState : long {
		Inactive,
		Possible,
		Activating,
		Activated
	}

	[NoWatch]
	[NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDocumentBrowserImportMode : ulong {
		None,
		Copy,
		Move
	}

	[NoWatch]
	[NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDocumentBrowserUserInterfaceStyle : ulong {
		White = 0,
		Light,
		Dark
	}

	[NoWatch]
	[NoTV, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UIDocumentBrowserActionAvailability : long {
		Menu = 1,
		NavigationBar = 1 << 1
	}

	[NoWatch, NoTV]
	[iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextDropPerformer : ulong {
		View = 0,
		Delegate,
	}

	[NoWatch]
	[iOS (11, 0), TV (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UINavigationItemLargeTitleDisplayMode : long {
		Automatic,
		Always,
		Never,
	}

	[NoWatch]
	[iOS (11, 0), TV (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UICollectionViewFlowLayoutSectionInsetReference : long {
		ContentInset,
		SafeArea,
		LayoutMargins,
	}

	[NoWatch]
	[NoTV]
	[iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPreferredPresentationStyle : long {
		Unspecified = 0,
		Inline,
		Attachment,
	}

	[NoWatch, NoTV, NoMac, iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("UIDocumentBrowserErrorDomain")]
	public enum UIDocumentBrowserErrorCode : long {
		Generic = 1,
		NoLocationAvailable = 2,
	}

	[iOS (12, 0), TV (12, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIGraphicsImageRendererFormatRange : long {
		Unspecified = -1,
		Automatic = 0,
		Extended,
		Standard,
	}

	[iOS (12, 0), NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPrintErrorCode : long {
		NotAvailableError = 1,
		NoContentError,
		UnknownImageFormatError,
		JobFailedError
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("UISceneErrorDomain")]
	[Native]
	public enum UISceneErrorCode : long {
		MultipleScenesNotSupported,
		RequestDenied,
		GeometryRequestUnsupported = 100,
		GeometryRequestDenied,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIImageSymbolScale : long {
		Default = -1,
		Unspecified = 0,
		Small = 1,
		Medium,
		Large,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIImageSymbolWeight : long {
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

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UISceneActivationState : long {
		Unattached = -1,
		ForegroundActive,
		ForegroundInactive,
		Background,
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIMenuElementState : long {
		Off,
		On,
		Mixed,
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIMenuElementAttributes : ulong {
		Disabled = 1uL << 0,
		Destructive = 1uL << 1,
		Hidden = 1uL << 2,
		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		KeepsMenuPresented = 1uL << 3,
	}

	[Flags]
	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIMenuOptions : ulong {
		DisplayInline = 1uL << 0,
		Destructive = 1uL << 1,
		[iOS (15, 0), TV (15, 0), NoWatch, MacCatalyst (15, 0)]
		SingleSelection = 1uL << 5,
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIContextMenuInteractionCommitStyle : long {
		Dismiss = 0,
		Pop,
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIWindowSceneSessionRole {
		[Field ("UIWindowSceneSessionRoleApplication")]
		Application,

		[Field ("UIWindowSceneSessionRoleExternalDisplay")]
		ExternalDisplay,

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
#if HAS_CARPLAY
		[Field ("CPTemplateApplicationSceneSessionRoleApplication", "CarPlay")]
#endif
		CarTemplateApplication,

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Field ("UIWindowSceneSessionRoleExternalDisplayNonInteractive")]
		ExternalDisplayNonInteractive,
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
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
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
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

		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("UIMenuOpenRecent")]
		OpenRecent,

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Field ("UIMenuDocument")]
		Document,

	}

	[iOS (13, 0), TV (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
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

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UICollectionLayoutSectionOrthogonalScrollingBehavior : long {
		None,
		Continuous,
		ContinuousGroupLeadingBoundary,
		Paging,
		GroupPaging,
		GroupPagingCentered,
	}

	[TV (13, 0), NoWatch, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityContrast : long {
		Unspecified = -1,
		Normal,
		High,
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UILegibilityWeight : long {
		Unspecified = -1,
		Regular,
		Bold,
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIUserInterfaceLevel : long {
		Unspecified = -1,
		Base,
		Elevated,
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIEditingInteractionConfiguration : long {
		None = 0,
		Default = 1,
	}

	[NoTV, NoWatch, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UISplitViewControllerBackgroundStyle : long {
		None,
		Sidebar,
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITabBarItemAppearanceStyle : long {
		Stacked,
		Inline,
		CompactInline,
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextAlternativeStyle : long {
		None,
		LowConfidence,
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextInteractionMode : long {
		Editable,
		NonEditable,
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
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

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIWindowSceneDismissalAnimation : long {
		Standard = 1,
		Commit = 2,
		Decline = 3,
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	public enum UIActivityItemsConfigurationInteraction {
		[Field ("UIActivityItemsConfigurationInteractionShare")]
		Share,
		[iOS (16, 4), MacCatalyst (16, 4)]
		[Field ("UIActivityItemsConfigurationInteractionCopy")]
		Copy,
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	public enum UIActivityItemsConfigurationPreviewIntent {
		[Field ("UIActivityItemsConfigurationPreviewIntentFullSize")]
		FullSize,
		[Field ("UIActivityItemsConfigurationPreviewIntentThumbnail")]
		Thumbnail,
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDatePickerStyle : long {
		Automatic,
		Wheels,
		Compact,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Inline,
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[iOS (13, 4), NoWatch, TV (13, 4)]
	[Native ("UIKeyboardHIDUsage")]
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

	[Flags, NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIEventButtonMask : ulong {
		Primary = 1L << 0,
		Secondary = 1L << 1,
	}

	[Flags, TV (13, 4), NoWatch, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAxis : ulong {
		Neither = 0uL,
		Horizontal = 1uL << 0,
		Vertical = 1uL << 1,
		Both = (Horizontal | Vertical),
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIScrollType : long {
		Discrete,
		Continuous,
	}

	[Flags, NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIScrollTypeMask : ulong {
		Discrete = 1L << 0,
		Continuous = 1L << 1,
		All = Discrete | Continuous,
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPointerEffectTintMode : long {
		None = 0,
		Overlay,
		Underlay,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIButtonRole : long {
		Normal,
		Primary,
		Cancel,
		Destructive,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellAccessoryDisplayedState : long {
		Always,
		WhenEditing,
		WhenNotEditing,
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellAccessoryOutlineDisclosureStyle : long {
		Automatic,
		Header,
		Cell,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellAccessoryPlacement : long {
		Leading,
		Trailing,
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellConfigurationDragState : long {
		None,
		Lifting,
		Dragging,
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellConfigurationDropState : long {
		None,
		NotTargeted,
		Targeted,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICollectionLayoutListAppearance : long {
		Plain,
		Grouped,
#if !TVOS
		[NoTV]
		[MacCatalyst (14, 0)]
		InsetGrouped,
		[NoTV]
		[MacCatalyst (14, 0)]
		Sidebar,
		[NoTV]
		[MacCatalyst (14, 0)]
		SidebarPlain,
#endif
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICollectionLayoutListHeaderMode : long {
		None,
		Supplementary,
		FirstItemInSection,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIContentInsetsReference : long {
		Automatic,
		None,
		SafeArea,
		LayoutMargins,
		ReadableContent,
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIContextMenuInteractionAppearance : long {
		Unknown = 0,
		Rich,
		Compact,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIUserInterfaceActiveAppearance : long {
		Unspecified = -1,
		Inactive,
		Active,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIListContentTextAlignment : long {
		Natural,
		Center,
		Justified,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIPageControlInteractionState : long {
		None = 0,
		Discrete = 1,
		Continuous = 2,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIPageControlBackgroundStyle : long {
		Automatic = 0,
		Prominent = 1,
		Minimal = 2,
	}

	[iOS (14, 0), TV (14, 0), NoWatch]
	[MacCatalyst (14, 0)]
	public enum UIPasteboardDetectionPattern {
		[Field ("UIPasteboardDetectionPatternProbableWebURL")]
		ProbableWebUrl,
		[Field ("UIPasteboardDetectionPatternProbableWebSearch")]
		ProbableWebSearch,
		[Field ("UIPasteboardDetectionPatternNumber")]
		Number,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("UIPasteboardDetectionPatternLink")]
		Link,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("UIPasteboardDetectionPatternPhoneNumber")]
		PhoneNumber,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("UIPasteboardDetectionPatternEmailAddress")]
		EmailAddress,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("UIPasteboardDetectionPatternPostalAddress")]
		PostalAddress,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("UIPasteboardDetectionPatternCalendarEvent")]
		CalendarEvent,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("UIPasteboardDetectionPatternShipmentTrackingNumber")]
		ShipmentTrackingNumber,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("UIPasteboardDetectionPatternFlightNumber")]
		FlightNumber,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("UIPasteboardDetectionPatternMoneyAmount")]
		MoneyAmount,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, NoiOS]
	[Native]
	public enum UISceneCollectionJoinBehavior : long {
		Automatic,
		Preferred,
		Disallowed,
		PreferredWithoutActivating,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UISplitViewControllerStyle : long {
		Unspecified,
		DoubleColumn,
		TripleColumn,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UISplitViewControllerColumn : long {
		Primary,
		Supplementary,
		Secondary,
		Compact,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UISplitViewControllerSplitBehavior : long {
		Automatic,
		Tile,
		Overlay,
		Displace,
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UISwitchStyle : long {
		Automatic = 0,
		Checkbox,
		Sliding,
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICollectionLayoutListFooterMode : long {
		None,
		Supplementary,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, NoiOS]
	[Native]
	public enum UITitlebarSeparatorStyle : long {
		Automatic,
		None,
		Line,
		Shadow,
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UINavigationItemBackButtonDisplayMode : long {
		Default = 0,
		Generic = 1,
		Minimal = 2,
	}

	// NSInteger -> UIGuidedAccessRestrictions.h
	[Native]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIGuidedAccessRestrictionState : long {
		Allow,
		Deny,
	}

	[TV (15, 0), iOS (15, 0), NoWatch, MacCatalyst (15, 0)]
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

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIBandSelectionInteractionState : long {
		Possible = 0,
		Began,
		Selecting,
		Ended,
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIBehavioralStyle : ulong {
		Automatic = 0,
		Pad,
		Mac,
	}

	[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIButtonConfigurationSize : long {
		Medium,
		Small,
		Mini,
		Large,
	}

	[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIButtonConfigurationTitleAlignment : long {
		Automatic,
		Leading,
		Center,
		Trailing,
	}

	[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIButtonConfigurationCornerStyle : long {
		Fixed = -1,
		Dynamic,
		Small,
		Medium,
		Large,
		Capsule,
	}

	[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIButtonConfigurationMacIdiomStyle : long {
		Automatic,
		Bordered,
		Borderless,
		BorderlessTinted,
	}

	[NoTV, NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIFocusGroupPriority : long {
		Ignored = 0,
		PreviouslyFocused = 1000,
		Prioritized = 2000,
		CurrentlyFocused = Int64.MaxValue,
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIFocusHaloEffectPosition : long {
		Automatic = 0,
		Outside,
		Inside,
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	public enum UISheetPresentationControllerDetentIdentifier {
		[DefaultEnumValue]
		[Field (null)]
		Unknown = -1,

		[Field ("UISheetPresentationControllerDetentIdentifierMedium")]
		Medium,

		[Field ("UISheetPresentationControllerDetentIdentifierLarge")]
		Large,
	}

	[NoWatch, TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIWindowScenePresentationStyle : ulong {
		Automatic,
		Standard,
		Prominent,
	}
}
