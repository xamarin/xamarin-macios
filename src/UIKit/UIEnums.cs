//
// UIEnums.cs:
//
// Copyright 2009-2011 Novell, Inc.
// Copyright 2011-2012, Xamarin Inc.
//
// Author:
//  Miguel de Icaza
//

namespace UIKit {
	// NSInteger -> UIImagePickerController.h
	/// <summary>An enumeration of values used to specify video quality.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIImagePickerControllerQualityType : long {
		/// <summary>High quality.</summary>
		High,
		/// <summary>Medium quality.</summary>
		Medium,
		/// <summary>Low quality.</summary>
		Low,
		/// <summary>VGA-quality video recording.</summary>
		At640x480,
		/// <summary>The 1280x720 iFrame format.</summary>
		At1280x720,
		/// <summary>The 960x540 iFrame format.</summary>
		At960x540,
	}

	// NSInteger -> UIActivityIndicatorView.h
	/// <summary>The visual style for a <see cref="UIKit.UIActivityIndicatorView" />.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UIActivityIndicatorView" />
	/// <altmember cref="UIKit.UIActivityIndicatorView.UIActivityIndicatorViewAppearance" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIActivityIndicatorViewStyle : long {
		/// <summary>The indicator is large and white.</summary>
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Large' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Large' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Large' instead.")]
		WhiteLarge,
		/// <summary>The indicator is white.</summary>
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Medium' instead.")]
		White,
		/// <summary>The indicator is gray.</summary>
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Medium' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Medium' instead.")]
		Gray,

		[MacCatalyst (13, 1)]
		Medium = 100,
		[MacCatalyst (13, 1)]
		Large = 101,
	}

	// NSInteger -> UIAlertView.h
	/// <summary>The visual style for a <see cref="UIKit.UIAlertView" />.</summary>
	///     <remarks>
	///       <para>
	///         The values other than <see cref="UIKit.UIAlertViewStyle.Default" /> allow the user to enter text, including the option to obscure the text as it is being entered.
	///       </para>
	///     </remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIAlertViewStyle : long {
		/// <summary>A standard alert.</summary>
		Default,
		/// <summary>Allows the user to enter text, but the text field is obscured.</summary>
		SecureTextInput,
		/// <summary>Allows the user to enter text.</summary>
		PlainTextInput,
		/// <summary>Allows the user to enter a login id and a password.</summary>
		LoginAndPasswordInput,
	}

	// NSInteger -> UIBarButtonItem.h
	/// <summary>The visual style of a <see cref="UIKit.UIBarButtonItem" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIBarButtonItemStyle : long {
		/// <summary>Plain style, will glow when tapped.</summary>
		Plain,

		/// <summary>Developers should not use this deprecated field. Developers should use 'UIBarButtonItemStyle.Plain' instead.</summary>
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UIBarButtonItemStyle.Plain' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UIBarButtonItemStyle.Plain' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarButtonItemStyle.Plain' instead.")]
		Bordered,

		/// <summary>Style for a done button.   This should be used if the screen will be dismissed upon tapping.</summary>
		Done,
	}

	// NSInteger -> UIBarButtonItem.h
	/// <summary>An enumeration of the predefined <see cref="UIKit.UIBarButtonItem" />s.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIBarButtonSystemItem : long {
		/// <summary>Done text, localized.</summary>
		Done,
		/// <summary>Cancel text, localized.</summary>
		Cancel,
		/// <summary>Edit text, localized.</summary>
		Edit,
		/// <summary>Save text, localized.</summary>
		Save,
		/// <summary>Add image.</summary>
		Add,
		/// <summary>Flexible space inserted among all elements.</summary>
		FlexibleSpace,
		/// <summary>Fixed space used to insert padding between items. </summary>
		FixedSpace,
		/// <summary>Compose image.</summary>
		Compose,
		/// <summary>Reply image</summary>
		Reply,
		/// <summary>Action image.</summary>
		Action,
		/// <summary>Organize image.</summary>
		Organize,
		/// <summary>Bookmark image.</summary>
		Bookmarks,
		/// <summary>Search image.</summary>
		Search,
		/// <summary>Refresh image.</summary>
		Refresh,
		/// <summary>Stop image.</summary>
		Stop,
		/// <summary>Camera image.</summary>
		Camera,
		/// <summary>Trash image.</summary>
		Trash,
		/// <summary>Play image.</summary>
		Play,
		/// <summary>Pause image.</summary>
		Pause,
		/// <summary>Remind image.</summary>
		Rewind,
		/// <summary>Fast forward image.</summary>
		FastForward,
		/// <summary>Undo image.</summary>
		Undo,
		/// <summary>Redo image.</summary>
		Redo,
		/// <summary>Developers should not use this deprecated field. </summary>
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		PageCurl,
		[NoTV]
		[MacCatalyst (13, 1)]
		Close,
		[iOS (18, 2), NoTV, MacCatalyst (18, 2)]
		WritingTools,
	}

	// NSUInteger -> UIControl.h
	/// <summary>An enumeration indicating various types of events.</summary>
	/// <remarks>The list of events for UIControl objects.</remarks>
	[Native ("UIControlEvents")]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UIControlEvent : ulong {
		/// <summary>Touch down event.</summary>
		TouchDown = 1 << 0,
		/// <summary>Repeated touch-down event.   The UITouch.TapCount property will be greater than one.</summary>
		TouchDownRepeat = 1 << 1,
		/// <summary>A finger is being dragged within the control.</summary>
		TouchDragInside = 1 << 2,
		/// <summary>A finger is being dragged outside of the bounds of the control, but close to it.</summary>
		TouchDragOutside = 1 << 3,
		/// <summary>A dragging finger has entered the control.</summary>
		TouchDragEnter = 1 << 4,
		/// <summary>A dragging finger has left the bounds of the control.</summary>
		TouchDragExit = 1 << 5,
		/// <summary>Touch-up event within the control.</summary>
		TouchUpInside = 1 << 6,
		/// <summary>Touch-up event outside the control.</summary>
		TouchUpOutside = 1 << 7,
		/// <summary>The system is cancelling the touch event.</summary>
		TouchCancel = 1 << 8,

		/// <summary>The value changed, emitted by various controls.</summary>
		ValueChanged = 1 << 12,
		/// <summary>To be added.</summary>
		PrimaryActionTriggered = 1 << 13,
		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		MenuActionTriggered = 1 << 14,

		/// <summary>UITextField event: editing started.</summary>
		EditingDidBegin = 1 << 16,
		/// <summary>UITextField event: the contents changed.</summary>
		EditingChanged = 1 << 17,
		/// <summary>UITextField event: editing finished.</summary>
		EditingDidEnd = 1 << 18,
		/// <summary>UITextField event: editing ended.</summary>
		EditingDidEndOnExit = 1 << 19,

		/// <summary>All touch events.</summary>
		AllTouchEvents = 0x00000FFF,
		/// <summary>All editing events for the UITextField.</summary>
		AllEditingEvents = 0x000F0000,
		/// <summary>This mask describes the range of bytes available for application events.   Any values within [0x01000000,0x0f000000] can be used as application specific events.</summary>
		ApplicationReserved = 0x0F000000,
		/// <summary>Mask of events reserved for system use.</summary>
		SystemReserved = 0xF0000000,
		/// <summary>All events</summary>
		AllEvents = 0xFFFFFFFF,
	}

	// NSInteger -> UIEvent.h
	/// <summary>An enumeration of event types.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIEventType : long {
		/// <summary>The event relates to touches to the screen.</summary>
		Touches,
		/// <summary>A motion event, such as when the user shakes the device.</summary>
		Motion,
		/// <summary>A remote-control event originating from a headset or external accessory, for the purpose of controlling multimedia.</summary>
		RemoteControl,
		/// <summary>Indicates that a physical button was pressed.</summary>
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
	/// <summary>An enumeration of event subtypes.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIEventSubtype : long {
		/// <summary>The event has no subtype.</summary>
		None,
		/// <summary>An event relating to the user shaking the device.</summary>
		MotionShake,

		/// <summary>A remote-control event for playing audio or video.</summary>
		RemoteControlPlay = 100,
		/// <summary>A remote-control event for pausing audio or video.</summary>
		RemoteControlPause = 101,
		/// <summary>A remote-control event for stopping audio or video. </summary>
		RemoteControlStop = 102,
		/// <summary>A remote-control event for toggling play/pause of audio or video.</summary>
		RemoteControlTogglePlayPause = 103,
		/// <summary>A remote-controler event for skipping to the next track.</summary>
		RemoteControlNextTrack = 104,
		/// <summary>A remote-control event for skipping to the previous track.</summary>
		RemoteControlPreviousTrack = 105,
		/// <summary>A remote-control event to start seeking backward through audio or video.</summary>
		RemoteControlBeginSeekingBackward = 106,
		/// <summary>A remote-control event to end seeking backward through audio or video.</summary>
		RemoteControlEndSeekingBackward = 107,
		/// <summary>A remote-control event to begin seeking forward through audio or video.</summary>
		RemoteControlBeginSeekingForward = 108,
		/// <summary>A remote-control event to end seeking forward through audio or video.</summary>
		RemoteControlEndSeekingForward = 109,
	}

	// NSInteger -> UIControl.h
	/// <summary>An enumeration of vertical alignments available to text and images.</summary>
	/// <remarks>An enumeration of valid vertical alignment values.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIControlContentVerticalAlignment : long {
		/// <summary>Aligns the content vertically in the middle of the control.</summary>
		Center = 0,
		/// <summary>Aligns the content vertically at the top of the control.</summary>
		Top = 1,
		/// <summary>Aligns the content vertically at the bottom of the control.</summary>
		Bottom = 2,
		/// <summary>Aligns the content vertically to fill the content rectangle, potentially stretching content.</summary>
		Fill = 3,
	}

	// NSInteger -> UIControl.h
	/// <summary>An enumeration of horizontal alignments available to text and images.</summary>
	/// <remarks>An enumeration of valid horizontal alignment values.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIControlContentHorizontalAlignment : long {
		/// <summary>Indicates that the content will be horizontally centered.</summary>
		Center = 0,
		/// <summary>Indicates that the content will be horizontally aligned from the left edge.</summary>
		Left = 1,
		/// <summary>Indicates that the content will be horizontally aligned from the right edge.</summary>
		Right = 2,
		/// <summary>Indicates that the content will fill the control, stretching and wrapping as necessary.</summary>
		Fill = 3,
		/// <summary>Indicates that the content will be horizontally aligned from the leading edge.</summary>
		Leading = 4,
		/// <summary>Indicates that the content will be horizontally aligned from the trailing edge.</summary>
		Trailing = 5,
	}

	// NSUInteger -> UIControl.h
	/// <summary>An enumeration of possible states for a <see cref="UIKit.UIControl" />.</summary>
	/// <remarks>Flags representing the state of a control.</remarks>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UIControlState : ulong {
		/// <summary>The normal state of the control (not disabled and not higlighted)</summary>
		Normal = 0,
		/// <summary>Control is highlighted.  You can change this through the Highlighted property of the control.</summary>
		Highlighted = 1 << 0,
		/// <summary>Control is in the disabled state.   You can change this through the Enabled property of the control.</summary>
		Disabled = 1 << 1,
		/// <summary>Selected state of the control.  You can change this value by accessing the Selected property of the UIControl.</summary>
		Selected = 1 << 2,
		/// <summary>Indicates that the control has the focus.</summary>
		[MacCatalyst (13, 1)]
		Focused = 1 << 3,
		/// <summary>Mask for application defined states for a control.   Possible application-reservedd values are 0x00010000 to 0x00ff0000.</summary>
		Application = 0x00FF0000,
		/// <summary>Reserved mask, no states should be defined in this range by the application.</summary>
		Reserved = 0xFF000000,
	}

	// NSInteger -> UIImage.h
	/// <summary>An enumeration of values used to specify the orientation of a <see cref="UIKit.UIImage" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum UIImageOrientation : long {
		/// <summary>Default orientation. <img href="~/xml/UIKit/_images/UIKit.UIImageOrientation_Up.png" alt="Image showing the specified image orientation." /></summary>
		Up,
		/// <summary>Rotated 180 degrees. <img href="~/xml/UIKit/_images/UIKit.UIImageOrientation_Down.png" alt="Image showing the specified image orientation." /></summary>
		Down,
		/// <summary>Rotated 90 degrees counterclockwise. <img href="~/xml/UIKit/_images/UIKit.UIImageOrientation_Left.png" alt="Image showing the specified image orientation." /></summary>
		Left,
		/// <summary>Rotated 90 degrees clockwise. <img href="~/xml/UIKit/_images/UIKit.UIImageOrientation_Right.png" alt="Image showing the specified image orientation." /></summary>
		Right,
		/// <summary>Flipped about its vertical axis. <img href="~/xml/UIKit/_images/UIKit.UIImageOrientation_UpMirrored.png" alt="Image showing the specified image orientation." /></summary>
		UpMirrored,
		/// <summary>Flipped about its vertical axis and then rotated 180 degrees. <img href="~/xml/UIKit/_images/UIKit.UIImage_DownMirrored.png" alt="Image showing the orientation for down and mirrored" /></summary>
		DownMirrored,
		/// <summary>Flipped about its horizontal axis and then rotated 90 degrees counterclockwise. <img href="~/xml/UIKit/_images/UIKit.UIImageOrientation_LeftMirrored.png" alt="Image showing the specified image orientation." /></summary>
		LeftMirrored,
		/// <summary>Flipped about its horizontal axis and then rotated 90 degrees clockwise. <img href="~/xml/UIKit/_images/UIKit.UIImageOrientation_RightMirrored.png" alt="Image showing the specified image orientation." /></summary>
		RightMirrored,
	}

	// NSUInteger -> UIView.h
	/// <summary>An enumeration indicating the resizing style for <see cref="UIKit.UIView" />s.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UIViewAutoresizing : ulong {
		/// <summary>Indicates that the view does not resize.</summary>
		None = 0,
		/// <summary>Resizing is performed by expanding or shrinking the UIView in the direction of the left margin.</summary>
		FlexibleLeftMargin = 1 << 0,
		/// <summary>Resizing is performed by expanding or shrinking the UIView's width.</summary>
		FlexibleWidth = 1 << 1,
		/// <summary>Resizing is performed by expanding or shrinking the UIView in the direction of the right margin.</summary>
		FlexibleRightMargin = 1 << 2,
		/// <summary>Resizing is performed by expanding or shrinking the UIView in the direction of the top margin.</summary>
		FlexibleTopMargin = 1 << 3,
		/// <summary>Resizing is performed by expanding or shrinking the UIView's height.</summary>
		FlexibleHeight = 1 << 4,
		/// <summary>Resizing is performed by expanding or shrinking the UIView in the direction of the bottom margin.</summary>
		FlexibleBottomMargin = 1 << 5,
		/// <summary>Combination of all flexible margin values.</summary>
		FlexibleMargins = FlexibleBottomMargin | FlexibleTopMargin | FlexibleLeftMargin | FlexibleRightMargin,
		/// <summary>Combines <see cref="UIKit.UIViewAutoresizing.FlexibleHeight" /> and <see cref="UIKit.UIViewAutoresizing.FlexibleWidth" />.</summary>
		FlexibleDimensions = FlexibleHeight | FlexibleWidth,
		/// <summary>The UIView resizes on all sides.</summary>
		All = 63,
	}

	// NSInteger -> UIView.h
	/// <summary>An enumeration of animation curve styles.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIViewAnimationCurve : long {
		/// <summary>The animatione begins and ends slowly. This is the default for most animations.</summary>
		EaseInOut,
		/// <summary>Causes the animation to begin slowly and speed up as it progresses.</summary>
		EaseIn,
		/// <summary>Causes the animation to begin quickly and slow down as it progresses.</summary>
		EaseOut,
		/// <summary>Causes the animation to proceed evenly.</summary>
		Linear,
	}

	// NSInteger -> UIView.h
	/// <include file="../../docs/api/UIKit/UIViewContentMode.xml" path="/Documentation/Docs[@DocId='T:UIKit.UIViewContentMode']/*" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIViewContentMode : long {
		/// <summary>Scales the contents to fit the new bounds, this might distort the contents.</summary>
		ScaleToFill,
		/// <summary>Scales the contents so that everything is visible, while preserving the aspect ration.   Any areas that are not filled become transparent.</summary>
		ScaleAspectFit,
		/// <summary>Scales the contents to fill the new bounaries of the view, while preserving the aspect ratio.   This means that the contents might be clipped.</summary>
		ScaleAspectFill,
		/// <summary>This forces a redraw when the <see cref="UIKit.UIView.Frame" /> of an object changes.</summary>
		Redraw,
		/// <summary>Centers the contents in the view</summary>
		Center,
		/// <summary>Aligns the content to the top of the view.</summary>
		Top,
		/// <summary>Aligns the content to the bottom of the view.</summary>
		Bottom,
		/// <summary>Aligns the content to the left of the view.</summary>
		Left,
		/// <summary>Aligns the content to the right of the view..</summary>
		Right,
		/// <summary>Aligns the content to the top left of the view.</summary>
		TopLeft,
		/// <summary>Aligns the content to the top right of the view.</summary>
		TopRight,
		/// <summary>Aligns the content to the bottom left of the view.</summary>
		BottomLeft,
		/// <summary>Aligns the content to the bottom right side of the view.</summary>
		BottomRight,
	}

	// NSInteger -> UIView.h
	/// <summary>An enumeration of predefined animated transitions.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIViewAnimationTransition : long {
		/// <summary>The option for indicating that no transition is specified.</summary>
		None,
		/// <summary>A transition that flips a UIView around a vertical axis from left to right. The left moves forward and the right backward.</summary>
		FlipFromLeft,
		/// <summary>A transition that flips a UIView around a vertical axis from right to left The right moves forward and the left backward.</summary>
		FlipFromRight,
		/// <summary>A transition that curls a UIView up from the bottom.</summary>
		CurlUp,
		/// <summary>A transition that curls a UIView down from the top.</summary>
		CurlDown,
	}

	// NSInteger -> UIBarCommon.h
	/// <summary>Enumerates layout bar metrics.</summary>
	/// <remarks>To be added.</remarks>
	/// <!-- TODO:  How does this behave with an iPad app? -->
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIBarMetrics : long {
		/// <summary>The default metrics for the device.</summary>
		Default,
		/// <summary>Metrics for the phone idiom.</summary>
		Compact,
		/// <summary>Default metrics for the device for bars with the prompt property, e.g., UINavigationBar and UISearchBar.</summary>
		DefaultPrompt = 101,
		/// <summary>Default metrics for bars with prompts on the phone idiom.</summary>
		CompactPrompt,

		/// <summary>Developers should not use this deprecated field. Developers should use 'UIBarMetrics.Compat' instead.</summary>
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UIBarMetrics.Compat' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UIBarMetrics.Compat' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarMetrics.Compat' instead.")]
		LandscapePhone = Compact,

		/// <summary>Metrics for landscape orientation for the phone idiom, for bar with the prompt property.</summary>
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UIBarMetrics.CompactPrompt' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UIBarMetrics.CompactPrompt' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarMetrics.CompactPrompt' instead.")]
		LandscapePhonePrompt = CompactPrompt,
	}

	// NSInteger -> UIButton.h
	/// <summary>An enumeration of predefined button types.</summary>
	/// <remarks>The type of a <see cref="UIKit.UIButton" />.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIButtonType : long {
		/// <summary>No button style.</summary>
		Custom,
		/// <summary>Rounded rectangle style. This style is deprecated as of iOS 7; developers should use <see cref="UIKit.UIButtonType.System" />.</summary>
		RoundedRect,
		/// <summary>Uses a detail disclosure button (arrow).</summary>
		DetailDisclosure,
		/// <summary>Information button, light background.</summary>
		InfoLight,
		/// <summary>Information button, dark background.</summary>
		InfoDark,
		/// <summary>The contact add button.</summary>
		ContactAdd,
		/// <summary>A standard system button.</summary>
		[NoiOS]
		[NoMacCatalyst]
		Plain,
		[NoTV]
		[MacCatalyst (13, 1)]
		Close,
		/// <summary>Added in iOS 7, this is the preferred default style. It lacks visible edges, background, etc.</summary>
		System = RoundedRect,
	}

	// NSInteger -> UIStringDrawing.h
	/// <summary>An enumeration of values used to specify line break mode.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	// note: __TVOS_PROHIBITED -> because it uses NSLineBreakMode (but we need this because we don't expose the later)
	public enum UILineBreakMode : long {
		/// <summary>Wraps at the first word that does not fit.</summary>
		WordWrap = 0,
		/// <summary>Wraps at the first character that doesn't fit.</summary>
		CharacterWrap,
		/// <summary>That which does not fit is not rendered.</summary>
		Clip,
		/// <summary>The end of the text is shown, the head is truncated to an ellipse.</summary>
		HeadTruncation,
		/// <summary>The start of the text is shown, the rest is indicated with an ellipse.</summary>
		TailTruncation,
		/// <summary>The start and end of the text is shown, with an ellipse in the middle.</summary>
		MiddleTruncation,
	}

	// NSInteger -> UIStringDrawing.h
	/// <summary>An enumeration that specifies text baseline alignment.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIBaselineAdjustment : long {
		/// <summary>Aligns using the font baselines.</summary>
		AlignBaselines = 0,
		/// <summary>Aligns on the center.</summary>
		AlignCenters,
		/// <summary>No alignment.</summary>
		None,
	}

	// NSInteger -> UIDatePicker.h
	/// <summary>An enumeration of modes available to the <see cref="UIKit.UIDatePicker" />.</summary>
	///     <remarks>Valid modes for the <see cref="UIKit.UIDatePicker" />.</remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIDatePickerMode : long {
		/// <summary>Time selector.</summary>
		Time,
		/// <summary>Date selector.</summary>
		Date,
		/// <summary>Date and time selector.</summary>
		DateAndTime,
		/// <summary>A count-down timer.</summary>
		CountDownTimer,
		[iOS (17, 4), MacCatalyst (17, 4)]
		YearAndMonth,
	}

	// NSInteger -> UIDevice.h
	/// <summary>An enumeration of device orientations.</summary>
	///     <remarks>The current device orientation.</remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIDeviceOrientation : long {
		/// <summary>The orientation of the device cannot be determined.</summary>
		Unknown,
		/// <summary>The bottom of the device is pointing downward.</summary>
		Portrait,
		/// <summary>The bottom of the device is pointing upward.</summary>
		PortraitUpsideDown,
		/// <summary>The bottom of the device is pointing to the left.</summary>
		LandscapeLeft,
		/// <summary>The bottom of the device is pointing to the right.</summary>
		LandscapeRight,
		/// <summary>The device is facing upward.</summary>
		FaceUp,
		/// <summary>The device is facing downward.</summary>
		FaceDown,
	}

	// NSInteger -> UIDevice.h
	/// <summary>An enumeration of values indicating the state of the battery.</summary>
	///     <remarks>
	///       <para>This is a qualitative description of the battery state (e.g., <see cref="UIKit.UIDeviceBatteryState.Charging" />). For a quantitative view of battery charge level, use <see cref="UIKit.UIDevice.BatteryLevel" />.</para>
	///     </remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIDeviceBatteryState : long {
		/// <summary>Can not determine the state of the battery.</summary>
		Unknown,
		/// <summary>The device is unplugged.</summary>
		Unplugged,
		/// <summary>The device's battery is currently charging.</summary>
		Charging,
		/// <summary>The device's battery is at full capacity.</summary>
		Full,
	}

	// NSInteger -> UIDocument.h
	/// <summary>An enumeration indicating what kind of change has occurred to a <see cref="UIKit.UIDocument" />.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIDocumentChangeKind : long {
		/// <summary>A change has been made.</summary>
		Done,
		/// <summary>A change has been undone.</summary>
		Undone,
		/// <summary>An undone change has been re-applied.</summary>
		Redone,
		/// <summary>The document has been cleared of outstanding changes.</summary>
		Cleared,
	}

	// NSInteger -> UIDocument.h
	/// <summary>An enumeration indicating whether the <see cref="UIKit.UIDocument" /> is being saved for the first time or should overwrite an existing save.</summary>
	///     <remarks>
	///       <para>One of these enumerated values is used as a parameter to <see cref="UIKit.UIDocument" />s various save-oriented functions (e.g., <see cref="UIKit.UIDocument.Save(Foundation.NSUrl,UIKit.UIDocumentSaveOperation,UIKit.UIOperationHandler)" />, <see cref="UIKit.UIDocument.GetFileAttributesToWrite(Foundation.NSUrl,UIKit.UIDocumentSaveOperation,out Foundation.NSError)" />, etc.).</para>
	///     </remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIDocumentSaveOperation : long {
		/// <summary>The <see cref="UIKit.UIDocument" /> is being saved for the first time.</summary>
		ForCreating,
		/// <summary>The existing version of the <see cref="UIKit.UIDocument" /> is intended to be overwritten.</summary>
		ForOverwriting,
	}

	// NSUInteger -> UIDocument.h
	/// <summary>An enumeration indicating the editing/saving state of a <see cref="UIKit.UIDocument" />.</summary>
	[Native]
	[Flags]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIDocumentState : ulong {
		/// <summary>The <see cref="UIKit.UIDocument" /> is open, editing is allowed, and there are no detected conflicts.</summary>
		Normal = 0,
		/// <summary>Either the document did not open successfully or has been closed.</summary>
		Closed = 1 << 0,
		/// <summary>A conflict exists. The application developer should resolve these by examining the results of <see cref="Foundation.NSFileVersion.GetOtherVersions(Foundation.NSUrl)" />.</summary>
		InConflict = 1 << 1,
		/// <summary>Something has interfered with the proper saving of the <see cref="UIKit.UIDocument" />.</summary>
		SavingError = 1 << 2,
		/// <summary>The document is busy and the application developer must not allow the application user to introduce changes.</summary>
		EditingDisabled = 1 << 3,
		/// <summary>Indicates the progress information is available for the downloading document.</summary>
		ProgressAvailable = 1 << 4,
	}

	// NSInteger -> UIImagePickerController.h
	/// <summary>An enumeration of values used to specify the source used by the <see cref="UIKit.UIImagePickerController" />.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIImagePickerControllerSourceType : long {
		/// <summary>The device's photo library.</summary>
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'PHPicker' instead.")]
		PhotoLibrary,
		/// <summary>One of the cameras on the device.</summary>
		Camera,
		/// <summary>The device's "Camera Roll" album or, if the device does not have a camera, the "Saved Photos" album.</summary>
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'PHPicker' instead.")]
		SavedPhotosAlbum,
	}

	// NSInteger -> UIImagePickerController.h
	/// <summary>An enumeration of values used by <see cref="UIKit.UIImagePickerControllerCameraCaptureMode" />.</summary>
	///     <remarks>
	///       <para>Specifies whether the camera will capture still images or video.</para>
	///     </remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIImagePickerControllerCameraCaptureMode : long {
		/// <summary>The camera will capture still images.</summary>
		Photo,
		/// <summary>The camera will capture video.</summary>
		Video,
	}

	// NSInteger -> UIImagePickerController.h
	/// <summary>An enumeration of values used by <see cref="UIKit.UIImagePickerController.CameraDevice" /> and related functions.</summary>
	///     <remarks>Used to differentiate between front (screen-side) and rear cameras.</remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIImagePickerControllerCameraDevice : long {
		/// <summary>The side of the device away from the screen.</summary>
		Rear,
		/// <summary>The side of the device that has the screen.</summary>
		Front,
	}

	// NSInteger -> UIImagePickerController.h
	/// <summary>An enumeration of values used by <see cref="UIKit.UIImagePickerController.CameraFlashMode" /></summary>
	///     <remarks>Determines whether the flash will be on, off, or as-needed.</remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIImagePickerControllerCameraFlashMode : long {
		/// <summary>The flash is always off, no matter the ambient conditions.</summary>
		Off = -1,
		/// <summary>The device takes into consideration ambient light when determining flash.</summary>
		Auto = 0,
		/// <summary>The flash will fire, no matter ambient light conditions.</summary>
		On = 1,
	}

	// NSInteger -> UIInterface.h
	/// <summary>The visual style for a navigation, search, or tool bar.</summary>
	///     <remarks>
	///       <para>The style used for <see cref="UIKit.UINavigationBar" />, <see cref="UIKit.UISearchBar" />, and <see cref="UIKit.UIToolbar" />.</para>
	///     </remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIBarStyle : long {
		/// <summary>The system default</summary>
		Default,
		/// <summary>Black</summary>
		Black,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
		/// <summary>Developers should not use this deprecated field. Developers should use 'UIBarStyle.Black'.</summary>
		[Deprecated (PlatformName.iOS, 5, 1, message: "Use 'UIBarStyle.Black'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarStyle.Black'.")]
		BlackOpaque = 1,

		// The header doesn't say when it was deprecated, but the earliest headers I have (iOS 5.1) it is already deprecated.
		/// <summary>Black translucent</summary>
		[Deprecated (PlatformName.iOS, 5, 1, message: "Use 'UIBarStyle.Black' and set the translucency property to true.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIBarStyle.Black' and set the translucency property to true.")]
		BlackTranslucent = 2,
	}

	// NSInteger -> UIProgressView.h
	/// <summary>The visual style for a <see cref="UIKit.UIProgressView" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIProgressViewStyle : long {
		/// <summary>The standard progress-view style. Default value.</summary>
		Default,
		/// <summary>The style of progress view that is used in a toolbar.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		Bar,
	}

	// NSInteger -> UIScrollView.h
	/// <include file="../../docs/api/UIKit/UIScrollViewIndicatorStyle.xml" path="/Documentation/Docs[@DocId='T:UIKit.UIScrollViewIndicatorStyle']/*" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIScrollViewIndicatorStyle : long {
		/// <summary>A black scroll indicator with a narrow white border. Slightly wider than either <see cref="UIKit.UIScrollViewIndicatorStyle.Black" /> or <see cref="UIKit.UIScrollViewIndicatorStyle.White" /></summary>
		Default,
		/// <summary>A black, borderless scroll indicator. Slightly narrower than <see cref="UIKit.UIScrollViewIndicatorStyle.Default" />.</summary>
		Black,
		/// <summary>A white, borderless scroll indicator. Slightly narrower than <see cref="UIKit.UIScrollViewIndicatorStyle.Default" />.</summary>
		White,
	}

	// NSInteger -> UITextInputTraits.h
	/// <summary>An enumeration of auto-capitalization styles.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITextAutocapitalizationType : long {
		/// <summary>No text is capitalized automatically.</summary>
		None,
		/// <summary>Capitalizes the first letter of each word automatically.</summary>
		Words,
		/// <summary>Capitalizes the first letter of each sentence automatically.</summary>
		Sentences,
		/// <summary>Capitalizes all characters automatically.</summary>
		AllCharacters,
	}

	// NSInteger -> UITextInputTraits.h
	/// <summary>An enumeration of auto-correction types.</summary>
	/// <remarks>To be added.</remarks>
	/// <!-- Bummed <del>typos</del> didn't render -->
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITextAutocorrectionType : long {
		/// <summary>The default behavior, based on the current script system.</summary>
		Default,
		/// <summary>Disables auto-correction.</summary>
		No,
		/// <summary>Enables auto-correction.</summary>
		Yes,
	}

	// NSInteger -> UITextInputTraits.h
	/// <summary>An enumeration of keyboard types.</summary>
	/// <remarks>
	///       <para>In order to change the keyboard appearance, the currently displaying keyboard must be dismissed. This is achieved by having the <see cref="UIKit.UIControl" /> associated with the keyboard resigning as first responder, changing the keyboard type, and then re-subscribed as the first subscriber, as shown in the following code:</para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// void AddKeyboardTypeButton (UIKeyboardType kbType, RectangleF frame)
	/// {
	/// 	  var kbButton = UIButton.FromType (UIButtonType.RoundedRect);
	/// 	  kbButton.Frame = frame;
	/// 	  kbButton.SetTitle (kbType.ToString (), UIControlState.Normal);
	/// 	  View.AddSubview (kbButton);
	/// 	  kbButton.TouchUpInside += (sender, e) => {
	/// 		    myTextField.ResignFirstResponder ();
	/// 		    myTextField.KeyboardType = kbType;
	/// 		    myTextField.BecomeFirstResponder ();
	/// 	  };
	/// }
	///         ]]></code>
	///       </example>
	///     </remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIKeyboardType : long {
		/// <summary>The default keyboard for the current input type.</summary>
		Default,
		/// <summary>Displays standard ASCII characters.</summary>
		ASCIICapable,
		/// <summary>Displays standard ASCII characters.</summary>
		AsciiCapable = ASCIICapable,
		/// <summary>Numbers and punctuation.</summary>
		NumbersAndPunctuation,
		/// <summary>Characters, '.', '/', and '.com' keys, and access to numbers and punctuation.</summary>
		Url,
		/// <summary>Numbers.</summary>
		NumberPad,
		/// <summary>Numbers plus access to #, *, 'pause', and 'wait'.</summary>
		PhonePad,
		/// <summary>Characters plus access to numbers.</summary>
		NamePhonePad,
		/// <summary>Characters, an @ symbol, and access to numbers and punctuation.</summary>
		EmailAddress,
		/// <summary>Displays numbers and decimal point.</summary>
		DecimalPad,
		/// <summary>Characters, @ and # keys, and access to numbers and punctuation.</summary>
		Twitter,
		/// <summary>Optimized for Web search terms and URL entry. </summary>
		WebSearch,
		/// <summary>Displays numbers and decimal point by using standard ASCII characters.</summary>
		[MacCatalyst (13, 1)]
		AsciiCapableNumberPad,
	}

	// NSInteger -> UISegmentedControl.h
	/// <summary>The visual style for a <see cref="UIKit.UISegmentedControl" />.</summary>
	[Native]
	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "This no longer has any effect.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This no longer has any effect.")]
	public enum UISegmentedControlStyle : long {
		/// <summary>The large plain style for segmented controls. This is the default.</summary>
		Plain,
		/// <summary>The large bordered style for segmented controls.</summary>
		Bordered,
		/// <summary>The small toolbar style for segmented controls.</summary>
		Bar,
		/// <summary>The large bezeled style for segmented controls.</summary>
		Bezeled,
	}

	// NSInteger -> UITabBarItem.h
	/// <summary>An enumeration of predefined <see cref="UIKit.UITabBarSystemItem" />s.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITabBarSystemItem : long {
		/// <summary>The "more" system item.</summary>
		More,
		/// <summary>The "Favorites" system item.</summary>
		Favorites,
		/// <summary>The "Featured" system item.</summary>
		Featured,
		/// <summary>The "Top Rated" system item.</summary>
		TopRated,
		/// <summary>The "Recents" system item.</summary>
		Recents,
		/// <summary>The "Contacts" system item.</summary>
		Contacts,
		/// <summary>The "History" system item.</summary>
		History,
		/// <summary>The "Bookmarks" system item.</summary>
		Bookmarks,
		/// <summary>The "Search" system item.</summary>
		Search,
		/// <summary>The "Downloads" system item.</summary>
		Downloads,
		/// <summary>The "Most Recent" system item.</summary>
		MostRecent,
		/// <summary>The "Most Viewed" system item.</summary>
		MostViewed,
	}

	// NSInteger -> UITableView.h
	/// <summary>The visual style for a <see cref="UIKit.UITableView" />. A table view's style can only be set when it is instantiated.</summary>
	/// <remarks>Some table view features are only available for a specific style, for example Plain tables can provide an index to help scroll through long lists but Grouped tables should not.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITableViewStyle : long {
		/// <summary>Cells in the plain style take the entire width of the table view - there is no rounded-rectangle grouping. Section headers and footers 'float' (stick to the top/bottom of the table view) as the user scrolls through the section. An index may be implemented to make scrolling through long lists faster.</summary>
		Plain,
		/// <summary>Table style where each section is grouped into a rounded-rectangle. The table view's background can be see behind the rounded-rectangle groupings. Section headers and footers do not 'float' while scrolling.</summary>
		Grouped,
		[NoTV]
		[MacCatalyst (13, 1)]
		InsetGrouped,
	}

	// NSInteger -> UITableView.h
	/// <summary>An enumeration of predefined scroll positions.</summary>
	/// <remarks>This is used by the <see cref="UIKit.UITableView.ScrollToRow(Foundation.NSIndexPath,UIKit.UITableViewScrollPosition,System.Boolean)" /> method</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITableViewScrollPosition : long {
		/// <summary>Minimal scrolling to make the requested cell visible.</summary>
		None,
		/// <summary>Scrolls the row of interest to the top of the view.</summary>
		Top,
		/// <summary>Scrolls the row of interest to the middle of the view.</summary>
		Middle,
		/// <summary>Scrolls the cell to the bottom of the view.</summary>
		Bottom,
	}

	// NSInteger -> UITableView.h
	/// <summary>An enumeration of animations used when rows are inserted or deleted from a table view.</summary>
	/// <remarks>
	///     </remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITableViewRowAnimation : long {
		/// <summary>Affected rows fade in/out of view.</summary>
		Fade,
		/// <summary>Inserted row/s slide in from the right, deleted row/s slide out to the right.</summary>
		Right,
		/// <summary>Inserted row/s slide in from the left, deleted row/s slide out to the left.</summary>
		Left,
		/// <summary>Inserted row/s slide down from the top, deleted row/s slide up.</summary>
		Top,
		/// <summary>Inserted row/s slide up from the bottom, deleted row/s slide down.</summary>
		Bottom,
		/// <summary>There is no animation when cells are added or removed. The cell appears immediately, as if the table view had been reloaded.</summary>
		None,
		/// <summary>The table view tries to keep the location of affected cells centered in the table view.</summary>
		Middle,
		/// <summary>Allows the table view to choose an appropriate animation.</summary>
		Automatic = 100,
	}

	// #defines over UIBarPosition -> NSInteger -> UIBarCommon.h
	/// <summary>An enumeration of the valid positions for a <see cref="UIKit.UIToolbar" />.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIToolbarPosition : long {
		/// <summary>The UIToolbar may be in any position.</summary>
		Any,
		/// <summary>The UIToolbar is at the top of its containing UIView.</summary>
		Bottom,
		/// <summary>The UIToolbar is at the bottom of its containing UIView.</summary>
		Top,
	}

	// NSInteger -> UITouch.h
	/// <summary>An enumeration of phases associated with a <see cref="UIKit.UITouch" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITouchPhase : long {
		/// <summary>The touch has begun.</summary>
		Began,
		/// <summary>The finger has moved.</summary>
		Moved,
		/// <summary>The finger is stationary.</summary>
		Stationary,
		/// <summary>The touch has ended.</summary>
		Ended,
		/// <summary>The touch has been cancelled.</summary>
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

	/// <summary>Enumerates different kinds of <see cref="UIKit.UITouch" /> objects.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITouchType : long {
		/// <summary>A direct touch caused by a finger on the screen.</summary>
		Direct,
		/// <summary>A touch that was not caused by a finger or stylus.</summary>
		Indirect,
		/// <summary>A touch that was caused by a stylus (e.g., the Apple Pencil).</summary>
		Stylus,
		[iOS (13, 4), TV (13, 4)]
		[MacCatalyst (13, 1)]
		IndirectPointer,
	}

	/// <summary>A flagging enumeration of the properties of a <see cref="UIKit.UITouch" /> that may change.</summary>
	/// <remarks>
	///       <para>Different hardware devices have different capabilities that can vary over the duration of a touch. For instance, all current hardware allows the <see cref="UIKit.UITouchProperties.Location" /> property to vary, but only the Apple Pencil currently supports the <see cref="UIKit.UITouchProperties.Altitude" /> and <see cref="UIKit.UITouchProperties.Azimuth" /> values to change over the course of a touch.</para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UITouchProperties : long {
		/// <summary>The device allows for the possibility that the <see cref="UIKit.UITouch.Force" /> will vary over the course of a touch.</summary>
		Force = (1 << 0),
		/// <summary>The device allows for the possibility that the <see cref="UIKit.UITouch.GetAzimuthAngle(UIKit.UIView)" /> and <see cref="UIKit.UITouch.GetAzimuthUnitVector(UIKit.UIView)" /> will vary over the course of a touch.</summary>
		Azimuth = (1 << 1),
		/// <summary>The device allows for the possibility that the <see cref="UIKit.UITouch.AltitudeAngle" /> will vary over the course of a touch.</summary>
		Altitude = (1 << 2),
		/// <summary>The device allows for the possibility that the <see cref="UIKit.UITouch.LocationInView(UIKit.UIView)" /> will vary over the course of a touch.</summary>
		Location = (1 << 3),
		[iOS (17, 5), MacCatalyst (17, 5), NoTV]
		Roll = (1L << 4),
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
	/// <summary>An enumeration of text alignments.</summary>
	/// <remarks>To be added.</remarks>
	[Native (ConvertToNative = "UITextAlignmentExtensions.ToNative", ConvertToManaged = "UITextAlignmentExtensions.ToManaged")]
#else
	[Native]
#endif
	public enum UITextAlignment : long {
		/// <summary>Text is left-aligned.</summary>
		Left,
		/// <summary>Text is centered.</summary>
		Center,
		/// <summary>Text is right-aligned.</summary>
		Right,

		/// <summary>Text spreads to the margins.</summary>
		Justified,
		/// <summary>Alignment is based on the text's script.</summary>
		Natural,
	}

	// NSInteger -> UITableViewCell.h
	/// <summary>The visual style of a <see cref="UIKit.UITableViewCell" />.</summary>
	/// <remarks>
	///     </remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellStyle : long {
		/// <summary>Plain style with a black, left-aligned <see cref="UIKit.UITableViewCell.TextLabel" /> and an optional <see cref="UIKit.UITableViewCell.ImageView" /> (that will appear to the left of the text).</summary>
		Default,
		/// <summary>Style with two text labels. The <see cref="UIKit.UITableViewCell.TextLabel" /> appears on the left, and is left-aligned with black text. <see cref="UIKit.UITableViewCell.DetailTextLabel" /> appears to the right, and is right-aligned with smaller blue text. The Settings app uses this style.</summary>
		Value1,
		/// <summary>Style with two text labels. The <see cref="UIKit.UITableViewCell.TextLabel" /> appears on the left, but is right-aligned with blue text. <see cref="UIKit.UITableViewCell.DetailTextLabel" /> appears to the right, but is left-aligned with black text. The detailed contact information in the Contacts app is an example of this cell style.</summary>
		Value2,
		/// <summary>Style with two text labels. They are both left-aligned, the top <see cref="UIKit.UITableViewCell.TextLabel" /> is large black text and the bottom <see cref="UIKit.UITableViewCell.DetailTextLabel" /> uses smaller gray text. The Music app's Albums listing is an example of this cell style.</summary>
		Subtitle,
	}

	// NSInteger -> UITableViewCell.h
	/// <summary>The visual style of a <see cref="UIKit.UITableView" />'s cell separator.</summary>
	///     <remarks>The cell separator style is set via a table view's <see cref="UIKit.UITableView.SeparatorStyle" /> property.</remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellSeparatorStyle : long {
		/// <summary>No separator is displayed between cells.</summary>
		None,
		/// <summary>A single line is displayed between each cell. This is the default.</summary>
		SingleLine,
		/// <summary>Developers should not use this deprecated field. Developers should use 'SingleLine' for a single line separator.</summary>
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SingleLine' for a single line separator.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SingleLine' for a single line separator.")]
		SingleLineEtched,
		/// <summary>A single etched line (made up of two different colored lines) is displayed between each cell (same as SingleLineEtched). This style can only be used in grouped-style table views.</summary>
		DoubleLineEtched = SingleLineEtched,
	}

	// NSInteger -> UITableViewCell.h
	/// <summary>The visual appearance of a <see cref="UIKit.UITableViewCell" /> when it is selected.</summary>
	/// <remarks>Used to set the <see cref="UIKit.UITableViewCell.SelectionStyle" /> of a cell.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellSelectionStyle : long {
		/// <summary>There is no change to the cell's appearance when it is selected.</summary>
		None,
		/// <summary>The cell background turns blue when it is selected. This is the default behavior.</summary>
		Blue,
		/// <summary>The cell background turns gray when it is selected.</summary>
		Gray,
		/// <summary>The default selection style used by tables.</summary>
		Default,
	}

	// NSInteger -> UITableViewCell.h
	/// <summary>An enumeration of editing styles for a <see cref="UIKit.UITableView" /> cell.</summary>
	/// <remarks>The editing style of a cell is set on the <see cref="UIKit.UITableViewCell.EditingStyle" /> property.  The editing control is displayed on the left hand side of the cell when it is in editing mode.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellEditingStyle : long {
		/// <summary>No editing control is displayed in the cell (this is the default).</summary>
		None,
		/// <summary>A red circle with a white minus sign is displayed, to indicate the cell can be deleted.</summary>
		Delete,
		/// <summary>A gree circle with a white plus sign is displayed, indicating a new row can be inserted.</summary>
		Insert,
	}

	// NSInteger -> UITableViewCell.h
	/// <summary>An enumeration of standard accessory controls that can be used by a <see cref="UIKit.UITableViewCell" />.</summary>
	/// <remarks>Set the type of accessory to display in a <see cref="UIKit.UITableViewCell" /> using the <see cref="UIKit.UITableViewCell.Accessory" /> property.</remarks>
	[Native ("UITableViewCellAccessoryType")]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellAccessory : long {
		/// <summary>No accessory is displayed. This is the default. Use this value to remove a previously-assigned accessory.</summary>
		None,
		/// <summary>A chevron (right-pointing arrow) is displayed on the right side of the cell. This accessory does not track touches.</summary>
		DisclosureIndicator,
		/// <summary>A blue circular button containing a chevron (right-pointing arrow) is displayed on the right side of the cell. This accessory tracks touches separately from the rest of the cell.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		DetailDisclosureButton,
		/// <summary>A tick is displayed on the right side of the cell. This accessory does not track touches. The table view's <see cref="UIKit.UITableViewSource" /> can manage check marks (possibly limiting the check mark to a single row) in the <see cref="UIKit.UITableViewSource.RowSelected(UIKit.UITableView,Foundation.NSIndexPath)" /> method.</summary>
		Checkmark,
		/// <summary>A standard button indicating additional detail.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		DetailButton,
	}

	// NSUInteger -> UITableViewCell.h
	/// <summary>An enumeration of states for a <see cref="UIKit.UITableViewCell" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native ("UITableViewCellStateMask")]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UITableViewCellState : ulong {
		/// <summary>The normal state of a UITableViewCell.</summary>
		DefaultMask = 0,
		/// <summary>The state of a UITableViewCell when the table is in editing mode.</summary>
		ShowingEditControlMask = 1 << 0,
		/// <summary>The state of a UITableViewCell that shows a button requesting confirmation of a delete gesture.</summary>
		ShowingDeleteConfirmationMask = 1 << 1,
	}

	// NSInteger -> UITextField.h
	/// <summary>An enumeration of visual styles for text borders.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITextBorderStyle : long {
		/// <summary>The text field does not have a visible border.</summary>
		None,
		/// <summary>Displays a thin rectangle around the text field.</summary>
		Line,
		/// <summary>Displays a bezel around the text field. Generally used for standard data-entry fields.</summary>
		Bezel,
		/// <summary>Displays a rounded rectangle border around the text field.</summary>
		RoundedRect,
	}

	// NSInteger -> UITextField.h
	/// <summary>An enumeration indicating the behavior of the clear button on a <see cref="UIKit.UITextField" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITextFieldViewMode : long {
		/// <summary>The overlay view never appears.</summary>
		Never,
		/// <summary>The overly view is displayed only while text is being edited.</summary>
		WhileEditing,
		/// <summary>The overlaw view is displayed only while text is not being edited.</summary>
		UnlessEditing,
		/// <summary>The overlay view is always displayed.</summary>
		Always,
	}

	// NSInteger -> UIViewController.h
	/// <summary> An enumeration of values used to specify the transition style of presented <see cref="UIKit.UIViewController" />s.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIModalTransitionStyle : long {
		/// <summary>Slides up from the bottom of the screen.</summary>
		CoverVertical = 0,
		/// <summary>A horizontal right-to-left flip. On dismissal, the transition flips left-to-right.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		FlipHorizontal,
		/// <summary>The old view fades out while the new view simultaneously fades in.</summary>
		CrossDissolve,
		/// <summary>A corner of the presented <see cref="UIKit.UIViewController" /> is "curled up" like a book page. On transition, the curl peels up the screen. Application developers must only use this style when the <see cref="UIKit.UIViewController" /> is being presented with <see cref="UIKit.UIModalPresentationStyle.FullScreen" />.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		PartialCurl,
	}

	// NSInteger -> UIApplication.h
	/// <summary>An enumeration of values that specify the orientation of the user interface.</summary>
	///     <remarks>
	///       <para>The <see cref="UIKit.UIInterfaceOrientation" /> represents the rotation of the user interface and, in portrait modes, is the opposite of the <see cref="UIKit.UIDevice.Orientation" /> property. For instance, if the device is rotated 90 degrees clockwise from normal portrait mode such that the home button is to the left, the <see cref="UIKit.UIViewController.InterfaceOrientation" /> property will be  <see cref="UIKit.UIInterfaceOrientation.LandscapeLeft" /> but the <see cref="UIKit.UIDevice.Orientation" /> property will be <see cref="UIKit.UIDeviceOrientation.LandscapeRight" />.
	///         </para>
	///     </remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIInterfaceOrientation : long {
		/// <summary>The orientation is unknown.</summary>
		Unknown = UIDeviceOrientation.Unknown,
		/// <summary>The home button is at the bottom.</summary>
		Portrait = UIDeviceOrientation.Portrait,
		/// <summary>The home button is at the top.</summary>
		PortraitUpsideDown = UIDeviceOrientation.PortraitUpsideDown,
		/// <summary>The home button is to the left.</summary>
		LandscapeLeft = UIDeviceOrientation.LandscapeRight,
		/// <summary>The home button is to the right.</summary>
		LandscapeRight = UIDeviceOrientation.LandscapeLeft,
	}

	// NSUInteger -> UIApplication.h
	/// <summary>An enumeration of values that specify the available orientations of the user interface.</summary>
	///     <remarks>
	///       <para>A <see cref="UIKit.UIViewController" /> may support more than one <see cref="UIKit.UIInterfaceOrientation" />. This value, returned from <see cref="UIKit.UIViewController.GetSupportedInterfaceOrientations" /> and <see cref="UIKit.UIApplication.SupportedInterfaceOrientationsForWindow(UIKit.UIWindow)" />, specifies which orientations are supported by the application.</para>
	///     </remarks>
	[Native]
	[Flags]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIInterfaceOrientationMask : ulong {
		/// <summary>The long side is vertical.</summary>
		Portrait = 1 << (int) UIInterfaceOrientation.Portrait,
		/// <summary>The UIViewController supports landscape-left orientation.</summary>
		LandscapeLeft = 1 << (int) UIInterfaceOrientation.LandscapeLeft,
		/// <summary>The UIViewController supports landscape-right orientation.</summary>
		LandscapeRight = 1 << (int) UIInterfaceOrientation.LandscapeRight,
		/// <summary>The UIViewController supports upside-down portrait orientation.</summary>
		PortraitUpsideDown = 1 << (int) UIInterfaceOrientation.PortraitUpsideDown,

		/// <summary>The UIViewController supports both landscape-left and landscape-right orientations.</summary>
		Landscape = LandscapeLeft | LandscapeRight,
		/// <summary>The UIViewController supports all interface orientations.</summary>
		All = PortraitUpsideDown | Portrait | LandscapeRight | LandscapeLeft,
		/// <summary>The UIViewController supports all orientations except upside-down portrait.</summary>
		AllButUpsideDown = Portrait | LandscapeRight | LandscapeLeft,
	}

	// NSInteger -> UIApplication.h
	/// <summary>An enumeration of navigation types for use in a <see cref="UIKit.UIWebView" />.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIWebViewNavigationType : long {
		/// <summary>The user tapped a link.</summary>
		LinkClicked,
		/// <summary>The app user has submitted a form.</summary>
		FormSubmitted,
		/// <summary>The app user has tapped either the back or forward button.</summary>
		BackForward,
		/// <summary>The app user has tapped the reload button.</summary>
		Reload,
		/// <summary>The app user has resubmitted a form.</summary>
		FormResubmitted,
		/// <summary>The app user has performed some other action.</summary>
		Other,
	}

	// NSUInteger -> UIApplication.h
	/// <summary>An enumeration of values that are automatically detected in text.</summary>
	///     <remarks>The types of text that are automatically detected in text content.   If the types are detected in the content, the text is converted into clickable content.   When clicked the application that is bound to handle that kind of data type is invoked (Web browser for HTTP links, Phone application for phone numbers).</remarks>
	[Native ("UIDataDetectorTypes")]
	[Flags]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIDataDetectorType : ulong {
		/// <summary>Detects phone numbers.</summary>
		PhoneNumber = 1 << 0,
		/// <summary>Detects web Urls.</summary>
		Link = 1 << 1,
		/// <summary>Addresses.</summary>
		Address = 1 << 2,
		/// <summary>Detects calendar events.</summary>
		CalendarEvent = 1 << 3,

		/// <summary>A tracking number for a parcel.</summary>
		[MacCatalyst (13, 1)]
		ShipmentTrackingNumber = 1 << 4,
		/// <summary>An airplane flight identifier.</summary>
		[MacCatalyst (13, 1)]
		FlightNumber = 1 << 5,
		/// <summary>A word or phrase that may be the intended final value.</summary>
		[MacCatalyst (13, 1)]
		LookupSuggestion = 1 << 6,
		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		Money = 1 << 7,
		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		PhysicalValue = 1uL << 8,

		/// <summary>Do not perform any content detection.</summary>
		None = 0,
		/// <summary>All supported detection types are activated.</summary>
		All = UInt64.MaxValue,
	}

	// NSInteger -> UIActionSheet.h
	/// <summary>The visual style for a <see cref="UIKit.UIActionSheet" />.</summary>
	///     <remarks>
	///       <para>The values of <see cref="UIKit.UIActionSheetStyle.Automatic" /> and <see cref="UIKit.UIActionSheetStyle.Default" /> are the same.</para>
	///       <para>In this image, the left phone is displaying with <see cref="UIKit.UIActionSheetStyle.BlackTranslucent" /> style and the right with <see cref="UIKit.UIActionSheetStyle.BlackOpaque" /> style.</para>
	///       <para>
	///         <img href="~/xml/UIKit/_images/UIKit.UIActionSheetStyle.png" alt="Screenshot showing alternate styles for the UIActionSheet" />
	///       </para>
	///     </remarks>
	[Native]
	[NoTV]
	[Deprecated (PlatformName.iOS, 13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	public enum UIActionSheetStyle : long {
		/// <summary>Same as <see cref="UIKit.UIActionSheetStyle.Default" /> default.</summary>
		Automatic = -1,
		/// <summary>The default style takes appearance of the bottom bar.</summary>
		Default = UIBarStyle.Default,
		/// <summary>A background with some amount of translucence.</summary>
		BlackTranslucent = 2, // UIBarStyle.BlackTranslucent,
		/// <summary>A black, opaque style.</summary>
		BlackOpaque = 1, // UIBarStyle.BlackOpaque,
	}

	// NSInteger -> UIApplication.h
	/// <summary>The visual style of the status bar.</summary>
	///     <remarks>
	///       <para>Starting in iOS 7, the status bar does not have background content, so application developers must ensure that the style of the status bar works with the content of the <see cref="UIKit.UIView" /> that will appear behind it. The <see cref="UIKit.UIStatusBarStyle.Default" /> style of the status bar presents dark content (i.e., its text and content are dark). If the underlying view has dark content, the status bar will be more legible if the application developer overrides <see cref="UIKit.UIViewController.PreferredStatusBarStyle" /> so that it returns <see cref="UIKit.UIStatusBarStyle.LightContent" />.</para>
	///     </remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIStatusBarStyle : long {
		/// <summary>The default, dark, value for content in the status bar. Preferable for use with lighter-colored content views.</summary>
		Default,

		/// <summary>Application developers should not use this deprecated style.</summary>
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LightContent' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LightContent' instead.")]
		BlackTranslucent = 1,

		/// <summary>Content in the status bar is drawn with light values. Preferable for use wth darker-colored content views.</summary>
		LightContent = 1,

		/// <summary>Developers should not use this deprecated field. Developers should use 'LightContent' instead.</summary>
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LightContent' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LightContent' instead.")]
		BlackOpaque = 2,

		[MacCatalyst (13, 1)]
		DarkContent = 3,
	}

	// NSInteger -> UIApplication.h
	/// <summary>An enumeration of animations available when the status bar is hidden or made visible.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIStatusBarAnimation : long {
		/// <summary>No animation.</summary>
		None,
		/// <summary>The status bar fades and and out as it is shown or hidden.</summary>
		Fade,
		/// <summary>The status bad slides in or out as it is shown or hidden.</summary>
		Slide,
	}

	// NSInteger -> UIGestureRecognizer.h
	/// <summary>An enumeration of states for a <see cref="UIKit.UIGestureRecognizer" />.</summary>
	/// <remarks>
	///       <para>
	/// 	This describes the state of a <see cref="UIKit.UIGestureRecognizer" />.  All of
	/// 	UIGestureRecognizers start in the Possible state.  Once one or
	/// 	more touches has been received, the recognizers transition to
	/// 	the Began state.  For one-shot patterns (like Tap), this will
	/// 	either transition into the Recognized state or the Failed
	/// 	state.  For continuous gestures (like panning, pinching,
	/// 	rotating) the recognizer will transition to the Changed state
	/// 	and emit multiple calls back to the action and finally
	/// 	transition to either the Ended or Cancelled states.
	/// 
	///       </para>
	///     </remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIGestureRecognizerState : long {
		/// <summary>The default state: no gesture is recognized, but the recognizer may be evaluating touch events.</summary>
		Possible,
		/// <summary>Touch object have begun that are recognized as a continuous gesture.</summary>
		Began,
		/// <summary>Touch objects that are part of a continuous gesture have changed.</summary>
		Changed,
		/// <summary>Touch objects that are part of a continuous gesture have ended.</summary>
		Ended,
		/// <summary>Touches have cancelled a continuous gesture.</summary>
		Cancelled,
		/// <summary>An unrecognized multi-touch gesture has occurred. </summary>
		Failed,

		/// <summary>A multi-touch has been recognized. The state is changed to UIGestureRecognizerState.Possible.</summary>
		Recognized = Ended,
	}

	// NSUInteger -> UIApplication.h
	/// <summary>An enumeration of remote notification types.</summary>
	[Native]
	[Flags]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIRemoteNotificationType : ulong {
		/// <summary>The app accepts no notifications.</summary>
		None = 0,
		/// <summary>The app accepts notifications that badge the app's icon.</summary>
		Badge = 1 << 0,
		/// <summary>The app accepts alert sounds as notifications.</summary>
		Sound = 1 << 1,
		/// <summary>The app accepts alert messages as notifications.</summary>
		Alert = 1 << 2,
		/// <summary>The app accepts notifications that trigger the downloading of issue assets in a Newsstand app.</summary>
		NewsstandContentAvailability = 1 << 3,
	}

	// NSInteger -> UITextInputTraits.h
	/// <summary>The keyboard appearance.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIKeyboardAppearance : long {
		/// <summary>The normal keyboard appearance.</summary>
		Default,
		/// <summary>The keyboard used in alert panels.</summary>
		Alert,
		/// <summary>A keyboard with a dark theme.</summary>
		Dark = Alert,
		/// <summary>A keyboard with a light theme.</summary>
		Light,
	}

	// NSInteger -> UITextInputTraits.h
	/// <summary>An enumeration of styles used for rendering the return key.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIReturnKeyType : long {
		/// <summary>The return key is labeled "Return"</summary>
		Default,
		/// <summary>The return key is labeled "Go".</summary>
		Go,
		/// <summary>The return key is labeled "Google".</summary>
		Google,
		/// <summary>The return key is labeled "Join".</summary>
		Join,
		/// <summary>The return key is labeled "Next".</summary>
		Next,
		/// <summary>The return key is labeled "Route".</summary>
		Route,
		/// <summary>The return key is labeled "Search".</summary>
		Search,
		/// <summary>The return key is labeled "Send".</summary>
		Send,
		/// <summary>The return key is labeled "Yahoo".</summary>
		Yahoo,
		/// <summary>The return key is labeled "Done".</summary>
		Done,
		/// <summary>The return key is labeled "Emergency Call".</summary>
		EmergencyCall,
		/// <summary>The return key is labeled "Continue".</summary>
		Continue,
	}

	// NSInteger -> UIViewController.h
	/// <summary>An enumeration of values used by <see cref="UIKit.UIViewController.ModalPresentationStyle" /></summary>
	/// <remarks>iPhones and iTouches should always use <see cref="UIKit.UIModalPresentationStyle.FullScreen" />. iPads may use any value.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIModalPresentationStyle : long {
		/// <summary>A non-modal presentation or dismissal.</summary>
		None = -1,
		[MacCatalyst (13, 1)]
		Automatic = -2,
		/// <summary>A UIModalPresentationStyle that encompasses the whole screen.</summary>
		FullScreen = 0,
		/// <summary>Set to the height and width of the screen in portrait orientation.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		PageSheet,
		/// <summary>Centered on and smaller than the screen.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		FormSheet,
		/// <summary>The same UIModalPresentationStyle used by the view's parent UIViewController.</summary>
		CurrentContext,
		/// <summary>Managed by a custom animator and an optional interative controller.</summary>
		Custom,
		/// <summary>Display the modal content over the full screen on top of the current view hierarchy.</summary>
		OverFullScreen,
		/// <summary>Display the modal content over only the parent view controller's content area.</summary>
		OverCurrentContext,
		/// <summary>Display the modal content in a popover view for horizontally regular environments, and in full screen mode for horizontally compact environments.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		Popover,
		/// <summary>Blur the previous content and then overlay the new content.</summary>
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		BlurOverFullScreen,
	}

	// NSUInteger -> UISwipeGestureRecognizer.h
	/// <summary>An enumeration of values specifying the directin of a swipe gesture .</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UISwipeGestureRecognizerDirection : ulong {
		/// <summary>The touch or touches swipe to the right. This is the default.</summary>
		Right = 1 << 0,
		/// <summary>The touch or touches swipe to the left.</summary>
		Left = 1 << 1,
		/// <summary>The touch or touches swipe to the top.</summary>
		Up = 1 << 2,
		/// <summary>The touch or touches swipe toward the bottom.</summary>
		Down = 1 << 3,
	}

	// NSUInteger -> UIPopoverController.h
	/// <summary>An enumeration indicating the direction of the arrow attached to a <see cref="UIKit.UIPopoverController" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UIPopoverArrowDirection : ulong {
		/// <summary>The arrow will be on top.</summary>
		Up = 1 << 0,
		/// <summary>The arrow will be at the bottom.</summary>
		Down = 1 << 1,
		/// <summary>The arrow will be on the left.</summary>
		Left = 1 << 2,
		/// <summary>The arrow will be on the right.</summary>
		Right = 1 << 3,
		/// <summary>This lets the system decide the best position for the arrow.</summary>
		Any = Up | Down | Left | Right,
		/// <summary>The direction of the arrow is unknown.</summary>
		Unknown = UInt64.MaxValue,
	};

	// NSInteger -> UIMenuController.h
	/// <summary>An enumeration indicating in which direction the arrow of the <see cref="UIKit.UIMenuController" /> points.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIMenuControllerArrowDirection : long {
		/// <summary>The system decides the arrow location based on the menu size.</summary>
		Default,
		/// <summary>The arrow will be at the top of the menu.</summary>
		Up,
		/// <summary>The arrow will be at the bottom of the menu.</summary>
		Down,
		/// <summary>The arrow will be at the left of the menu.</summary>
		Left,
		/// <summary>The arrow will be at the right of the menu.</summary>
		Right,
	}

	// NSUInteger -> UIPopoverController.h
	/// <summary>An enumeration of the corners of a rectangle.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	public enum UIRectCorner : ulong {
		/// <summary>The top-left corner of the rectangle.</summary>
		TopLeft = 1 << 0,
		/// <summary>The top-right corner of the rectangle.</summary>
		TopRight = 1 << 1,
		/// <summary>The bottom-left corner of the rectangle.</summary>
		BottomLeft = 1 << 2,
		/// <summary>The bottom-right corner of the rectangle.</summary>
		BottomRight = 1 << 3,
		/// <summary>All the corners of the rectangle.</summary>
		AllCorners = ~(ulong) 0,
	}

	// NSInteger -> UIApplication.h
	/// <summary>An enumeration of values specifying the layout direction of the UI.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIUserInterfaceLayoutDirection : long {
		/// <summary>Indicates a left-to-right layout.</summary>
		LeftToRight,
		/// <summary>Layout direction flows right to left. Used most commonly in localizations such as Arabic or Hebrew.</summary>
		RightToLeft,
	}

	// NSInteger -> UIDevice.h
	/// <summary>An enumeration indicating on what kind of device the UI is running.</summary>
	/// <remarks>
	///     </remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIUserInterfaceIdiom : long {
		/// <summary>Not specified.</summary>
		Unspecified = -1,
		/// <summary>The UI should be designed for the iPhone and iPod Touch.</summary>
		Phone,
		/// <summary>The UI should be designed for an iPad.</summary>
		Pad,
		/// <summary>The UI should be designed for display on a television.</summary>
		TV,
		/// <summary>The UI should be designed for CarPlay.</summary>
		CarPlay,
		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Mac = 5,
		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		Vision = 6,
	}

	// NSInteger -> UIApplication.h
	/// <include file="../../docs/api/UIKit/UIApplicationState.xml" path="/Documentation/Docs[@DocId='T:UIKit.UIApplicationState']/*" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIApplicationState : long {
		/// <summary>The application is currently active and on the foreground</summary>
		Active,
		/// <summary>The application is in the foreground but not receiving events.   The application is placed in this state when moving in and out of the foreground state and also when the application is interrupted by a system notification.</summary>
		Inactive,
		/// <summary>The application is executing in the background.   It does not receive events.</summary>
		Background,
	}

	// NSInteger -> UIView.h
	/// <summary>An enumeration indicating animation options.</summary>
	/// <remarks>
	///     </remarks>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UIViewAnimationOptions : ulong {
		/// <summary>Lays out subviews at commit time so they are animated along with their parent.</summary>
		LayoutSubviews = 1 << 0,
		/// <summary>This flag instructs the system to keep sending input events to the view during the animation.   By default input events are disabled when an animation is taking place.</summary>
		AllowUserInteraction = 1 << 1,
		/// <summary>Starts the animation from the current view state.</summary>
		BeginFromCurrentState = 1 << 2,
		/// <summary>If set, the animation will repeat.</summary>
		Repeat = 1 << 3,
		/// <summary>If set, the animation will automatically reverse once it completes.</summary>
		Autoreverse = 1 << 4,
		/// <summary>If set, the animation will use the original duration value, rather than the remaining duration of the in-flight animation.</summary>
		OverrideInheritedDuration = 1 << 5,
		/// <summary>If set, the animation will use the original curve specified when the animation was submitted, not the curve of the in-flight animation.</summary>
		OverrideInheritedCurve = 1 << 6,
		/// <summary>If set, views are animated by changing their properties and redrawing. If not set, the views are animated using a snapshot image.</summary>
		AllowAnimatedContent = 1 << 7,
		/// <summary>If set, views are hidden and shown (not removed or added) during transition. Both views must already be in the parent view's hierarchy.</summary>
		ShowHideTransitionViews = 1 << 8,
		/// <summary>The option to not inherit the animation type or any other options.</summary>
		OverrideInheritedOptions = 1 << 9,

		/// <summary>Uses an EasyInOut animation.</summary>
		CurveEaseInOut = 0 << 16,
		/// <summary>Uses an EaseIn animation.</summary>
		CurveEaseIn = 1 << 16,
		/// <summary>Uses an EaseOut animation.</summary>
		CurveEaseOut = 2 << 16,
		/// <summary>Uses a linear animation.</summary>
		CurveLinear = 3 << 16,

		/// <summary>No transition.</summary>
		TransitionNone = 0 << 20,
		/// <summary>A transition that flips a view around its vertical axis from left to right. The left side comes forward and the right moves backward.</summary>
		TransitionFlipFromLeft = 1 << 20,
		/// <summary>A transition that flips a view around its vertical axis from right to left. The right side comes forward and the left moves backward.</summary>
		TransitionFlipFromRight = 2 << 20,
		/// <summary>A transition that curls a view up from the bottom.</summary>
		TransitionCurlUp = 3 << 20,
		/// <summary>A transition that curls a view down from the top.</summary>
		TransitionCurlDown = 4 << 20,
		/// <summary>A transition that dissolves between views.</summary>
		TransitionCrossDissolve = 5 << 20,
		/// <summary>A transition that flips a view around its horizontal axis from top to bottom. The top moves forward and the bottom moves back.</summary>
		TransitionFlipFromTop = 6 << 20,
		/// <summary>A transition that flips a view around its horizontal axis from bottom to top. The bottom moves forward and the top moves back.</summary>
		TransitionFlipFromBottom = 7 << 20,

		/// <summary>Constant that indicates that the default frame rate is preferred for animations.</summary>
		[MacCatalyst (13, 1)]
		PreferredFramesPerSecondDefault = 0 << 24,
		/// <summary>Constant that indicates that 60 frames per second are preferred for animations.</summary>
		[MacCatalyst (13, 1)]
		PreferredFramesPerSecond60 = 3 << 24,
		/// <summary>Constant that indicates that 30 frames per second are preferred for animations.</summary>
		[MacCatalyst (13, 1)]
		PreferredFramesPerSecond30 = 7 << 24,

		/// <summary>Forces layout updates to flush immediately after animation changes.</summary>
		[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		FlushUpdates = 1 << 28,
	}

	// untyped (and unamed) enum -> UIPrintError.h
	// note: it looks unused by any API
	/// <summary>An enumeration of error codes from the printing system.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("UIPrintErrorDomain")]
	public enum UIPrintError {
		/// <summary>The printer was not available.</summary>
		NotAvailable = 1,
		/// <summary>The job contained no content.</summary>
		NoContent,
		/// <summary>The print job image was not in a recognized format.</summary>
		UnknownImageFormat,
		/// <summary>The job failed.</summary>
		JobFailed,
	}

	// NSInteger -> UIPrintInfo.h
	/// <summary>An enumeration of duplex modes supported by the printer.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIPrintInfoDuplex : long {
		/// <summary>Single-sided printing only.</summary>
		None,
		/// <summary>Flips the back page along the long edge of the paper.</summary>
		LongEdge,
		/// <summary>Flips the back page along the short edge of the paper.</summary>
		ShortEdge,
	}

	// NSInteger -> UIPrintInfo.h
	/// <summary>An enumeration of printing orientations.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIPrintInfoOrientation : long {
		/// <summary>Portrait mode, in which the longer side of the page is vertical.</summary>
		Portrait,
		/// <summary>Landscape mode, in which the longer side of the page is horizontal.</summary>
		Landscape,
	}

	// NSInteger -> UIPrintInfo.h
	/// <summary>An enumeration of print content types (e.g., general, photo, grayscale).</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIPrintInfoOutputType : long {
		/// <summary>A mixture of text, graphics, and images.</summary>
		General,
		/// <summary>Black-and-white or color image.</summary>
		Photo,
		/// <summary>Print job contains no color.</summary>
		Grayscale,
		/// <summary>A grayscale image.</summary>
		PhotoGrayscale,
	}

	// NSInteger -> UIAccessibility.h
	/// <summary>An enumeration indicating the scrolling direction desired.</summary>
	/// <remarks>Used as the argument to <see cref="UIResponder.AccessibilityScroll" /> to generate a scrolling action.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIAccessibilityScrollDirection : long {
		/// <summary>Indicates a scroll to the right.</summary>
		Right = 1,
		/// <summary>Indicates a scroll to the left.</summary>
		Left,
		/// <summary>Indicates a scroll upwards.</summary>
		Up,
		/// <summary>Indicates a scroll downwards.</summary>
		Down,
		/// <summary>Indicates a scroll to the next logical position.</summary>
		Next,
		/// <summary>Indicates a scroll to the previous position.</summary>
		Previous,
	}

	// NSInteger -> UIScreen.h
	/// <summary>An enumeration of strategies for dealing with pixels lost at the edge of the screen.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UIScreen.OverscanCompensation" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIScreenOverscanCompensation : long {
		/// <summary>The final screenbuffer is scaled so that all pixels are visible.</summary>
		Scale,
		/// <summary>The screen bounds are reduced so that all pixels are visible.</summary>
		InsetBounds,
		/// <summary>No scaling is performed.</summary>
		None,
		/// <summary>The application frame is reduced to compensate for overscan.</summary>
		[Obsolete ("Use 'UIScreenOverscanCompensation.None' instead.")]
		InsetApplicationFrame = None,
	}

	// NSInteger -> UISegmentedControl.h
	/// <summary>An enumeration of locations in a <see cref="UIKit.UISegmentedControl" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UISegmentedControlSegment : long {
		/// <summary>Any segment.</summary>
		Any,
		/// <summary>The capped, left-most segment.</summary>
		Left,
		/// <summary>Any segment between the left- and right-most segments.</summary>
		Center,
		/// <summary>The capped, right-most segment.</summary>
		Right,
		/// <summary>The standalone segment, capped on both ends.</summary>
		Alone,
	}

	// NSInteger -> UISearchBar.h
	/// <summary>An enumeration indicating icons available for the search bar.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UISearchBarIcon : long {
		/// <summary>The search icon.</summary>
		Search,
		/// <summary>The icon used for the clear action.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		Clear,
		/// <summary>The icon used for a bookmark.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		Bookmark,
		/// <summary>The icon used for the results list.</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		ResultsList,
	}

	// NSInteger -> UIPageViewController.h
	/// <summary>An enumeration indicating the orientation of <see cref="UIKit.UIPageViewController" /> page turns.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIPageViewControllerNavigationOrientation : long {
		/// <summary>Horizontal orientation.</summary>
		Horizontal,
		/// <summary>Vertical orientation.</summary>
		Vertical,
	}

	// NSInteger -> UIPageViewController.h
	/// <summary>An enumeration indicating the location of the spine around which the <see cref="UIKit.UIPageViewController" /> transitions occur.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIPageViewControllerSpineLocation : long {
		/// <summary>No spine.</summary>
		None,
		/// <summary>The left or top edge of the screen.</summary>
		Min,
		/// <summary>The middle of the screen.</summary>
		Mid,
		/// <summary>At the right or bottom edge of the screen.</summary>
		Max,
	}

	// NSInteger -> UIPageViewController.h
	/// <summary>An enumeration indicating the direction of <see cref="UIKit.UIPageViewController" /> page turns.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIPageViewControllerNavigationDirection : long {
		/// <summary>Navigation forward.</summary>
		Forward,
		/// <summary>Navigation backward.</summary>
		Reverse,
	}

	// NSInteger -> UIPageViewController.h
	/// <summary>An enumeration indicating the transition style of a <see cref="UIKit.UIPageViewController" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIPageViewControllerTransitionStyle : long {
		/// <summary>A transition in which the page curls up.</summary>
		PageCurl,
		/// <summary>A transition in which the page scrolls away.</summary>
		Scroll,
	}

	// NSInteger -> UITextInputTraits.h
	/// <summary>An enumeration specifying whether spell-checking is on or off.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITextSpellCheckingType : long {
		/// <summary>The default behavior: spell-checking is enabled when auto-correction is enabled.</summary>
		Default,
		/// <summary>Disables spell-checking.</summary>
		No,
		/// <summary>Enables spell-checking.</summary>
		Yes,
	}

	// NSInteger -> UITextInput.h
	/// <summary>An enumeation indicating the direction in which text is stored.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UITextField" />
	/// <altmember cref="UIKit.UITextView" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITextStorageDirection : long {
		/// <summary>The text is stored in a forward direction.</summary>
		Forward,
		/// <summary>The text is stored in a backward direction.</summary>
		Backward,
	}

	// NSInteger -> UITextInput.h
	/// <summary>An enumeration indicating the direction of text layout.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITextLayoutDirection : long {
		/// <summary>The text is laid out from left-to-right.</summary>
		Right = 2,
		/// <summary>The text is laid out from right-to-left.</summary>
		Left,
		/// <summary>The text is laid out upward.</summary>
		Up,
		/// <summary>The text is laid out downward.</summary>
		Down,
	}

	// Sum of UITextStorageDirection and UITextLayoutDirection 
	// NSInteger -> UITextInput.h
	/// <summary>An enumeration of values that specify text direction.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UITextStorageDirection" />
	/// <altmember cref="UIKit.UITextLayoutDirection" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITextDirection : long {
		/// <summary>The text direction is forward.</summary>
		Forward,
		/// <summary>Backward text direction.</summary>
		Backward,
		/// <summary>The text direction is to the right.</summary>
		Right,
		/// <summary>The text direction is to the left.</summary>
		Left,
		/// <summary>The text direction is upward.</summary>
		Up,
		/// <summary>The text direction is downward.</summary>
		Down,
	}

	// NSInteger -> UITextInput.h
	/// <summary>An enumeration of values that specify the granularity of a text range .</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UITextInputStringTokenizer" />
	/// <altmember cref="UIKit.UITextInputTokenizer" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITextGranularity : long {
		/// <summary>The unit of text is a character.</summary>
		Character,
		/// <summary>The unit of text is a word.</summary>
		Word,
		/// <summary>The unit of text is a sentence.</summary>
		Sentence,
		/// <summary>The unit of text is a paragraph.</summary>
		Paragraph,
		/// <summary>The unit of text is a line.</summary>
		Line,
		/// <summary>The unit of text is a document.</summary>
		Document,
	}

	// float (and not even a CGFloat) -> NSLayoutConstraint.h
	// the API were fixed (a long time ago to use `float`) and the enum
	// values can still be used (and useful) since they will be casted
	/// <summary>An enumeration of values used by flow layouts to prioritize constraints.</summary>
	/// <remarks>
	///       <para>Higher values are considered more important by the flow layout constraint engine. Application developers should not specify a layout priority greater than the value of <see cref="UIKit.UILayoutPriority.Required" /></para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	public enum UILayoutPriority {
		/// <summary>Indicates a required constraint. The underlying value of this is 1000.</summary>
		Required = 1000,
		/// <summary>The resistance by which a button resists compressing its content.</summary>
		DefaultHigh = 750,
		/// <summary>The priority at which a button hugs its content horizontally.</summary>
		DefaultLow = 250,
		/// <summary>Generally not used; the priority at which a view wants to conform to the value of UIView.SystemLayoutSizeFitting.</summary>
		FittingSizeLevel = 50,
		[MacCatalyst (13, 1)]
		DragThatCanResizeScene = 510,
		[MacCatalyst (13, 1)]
		SceneSizeStayPut = 500,
		[MacCatalyst (13, 1)]
		DragThatCannotResizeScene = 490,
	}

	// NSInteger -> NSLayoutConstraint.h
	/// <summary>An enumeration of valid <see cref="UIKit.UICollectionViewUpdateItem.UpdateAction" /> properties.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UICollectionViewUpdateItem.UpdateAction" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UICollectionUpdateAction : long {
		/// <summary>Inserts the item into the collection view.</summary>
		Insert,
		/// <summary>Removes the item from the collection view.</summary>
		Delete,
		/// <summary>Reloads the item by deleting and then inserting it into the collection view.</summary>
		Reload,
		/// <summary>Moves the item to a new location.</summary>
		Move,
		/// <summary>Take no action on the item.</summary>
		None,
	}

	// NSUInteger -> UICollectionView.h
	/// <summary>An enumeration of values used to specify to where a <see cref="UIKit.UIView" /> should end up after a scroll into a <see cref="UIKit.UICollectionView" />.</summary>
	/// <remarks>To be added.</remarks>
	/// <!--TODO : Confirm that it's UIView and not a specialized type -->
	/// <altmember cref="UIKit.UICollectionView" />
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UICollectionViewScrollPosition : ulong {
		/// <summary>Do not scroll the item into the view.</summary>
		None,
		/// <summary>Scrolls so that the item is positioned at the top of the view's bounds.</summary>
		Top = 1 << 0,
		/// <summary>Scrolls so that the item is centered vertically in the collection view.</summary>
		CenteredVertically = 1 << 1,
		/// <summary>Scrolls so that the item is positioned at the bottom of the collection view.</summary>
		Bottom = 1 << 2,
		/// <summary>Scrolls so that the item is positioned at the left edge of the collection view.</summary>
		Left = 1 << 3,
		/// <summary>Scrolls so that the item is centered horizontally in the collection view.</summary>
		CenteredHorizontally = 1 << 4,
		/// <summary>Scrolls so that the item is positioned at the right edge of the collection view.</summary>
		Right = 1 << 5,
	}

	// NSInteger -> UICollectionViewFlowLayout.h
	/// <summary>An enumeration of values used by the <see cref="UIKit.UICollectionViewFlowLayout.ScrollDirection" /> property.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UICollectionViewFlowLayout.ScrollDirection" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UICollectionViewScrollDirection : long {
		/// <summary>Elements are placed top-to-bottom and then wrap horizontally to the right.</summary>
		Vertical,
		/// <summary>Elements are placed left-to-right and then wrap vertically downward.</summary>
		Horizontal,
	}

	// NSInteger -> UICollectionViewFlowLayout.h
	/// <summary>An enumeration of values used in flow layouts to specify which axis is being constrained.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UILayoutConstraintAxis : long {
		/// <summary>The constraint applies to the horizontal axis.</summary>
		Horizontal,
		/// <summary>The constraint applies to the vertical axis.</summary>
		Vertical,
	}

	// NSInteger -> UIImage.h
#if __MACCATALYST__
	/// <summary>An enumeration of values that specify how a <see cref="UIKit.UIImage" /> ought to be resized.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UIImage" />
	[Native (ConvertToNative = "UIImageResizingModeExtensions.ToNative", ConvertToManaged = "UIImageResizingModeExtensions.ToManaged")]
#else
	[Native]
#endif
	public enum UIImageResizingMode : long {
		/// <summary>The contents of the original image are repeated as necessary to fill the interior of the new image.</summary>
		Tile,
		/// <summary>The contents of the original image are scaled to fill the interior of the new image.</summary>
		Stretch,
	}

	// NSUInteger -> UICollectionViewLayout.h
	/// <summary>An enumeration of values used by <see cref="UIKit.UICollectionViewLayoutAttributes.RepresentedElementCategory" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UICollectionElementCategory : ulong {
		/// <summary>The related <see cref="UIKit.UIView" /> is a cell in a <see cref="UIKit.UICollectionView" />.</summary>
		Cell,
		/// <summary>The related <see cref="UIKit.UIView" /> is a field in a <see cref="UIKit.UICollectionView" />.</summary>
		SupplementaryView,
		/// <summary>The related <see cref="UIKit.UIView" /> is a decoration in a <see cref="UIKit.UICollectionView" />.</summary>
		DecorationView,
	}

	// that's a convenience enum that maps to UICollectionElementKindSection[Footer|Header] which are NSString
	/// <summary>An enumeration of view types that are supported in flow layouts.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UICollectionViewFlowLayout" />
	[MacCatalyst (13, 1)]
	public enum UICollectionElementKindSection {
		/// <summary>The <see cref="UIKit.UIView" /> should be treated as a header.</summary>
		Header,
		/// <summary>The <see cref="UIKit.UIView" /> should be treated as a footer.</summary>
		Footer,
	}

	// uint64_t -> UIAccessibilityConstants.h
	// note: IMO not really worth changing to ulong for backwards compatibility concerns
	// This is not an enum in ObjC but several fields exported (and we have them too)
	// Unit tests (ViewTest.cs) already ensure we expose the same value as iOS returns
	/// <summary>An enumeration whose values can be used as flags for <see cref="UIKit.UIView.AccessibilityTraits" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	public enum UIAccessibilityTrait : long {
		/// <summary>The accessibility element has no traits.</summary>
		None = 0,
		/// <summary>The accessibility element should be treated as a button.</summary>
		Button = 1,
		/// <summary>The accessibility element should be treated as a link.</summary>
		Link = 2,
		/// <summary>The accessibility element should be treated as an image.</summary>
		Image = 4,
		/// <summary>The accessibility element is selected.</summary>
		Selected = 8,
		/// <summary>The accessibility element plays a sound when it is activated.</summary>
		PlaysSound = 16,
		/// <summary>The accessibility element behaves like a keyboard key.</summary>
		KeyboardKey = 32,
		/// <summary>The accessibility element should be treated as static, immutable text.</summary>
		StaticText = 64,
		/// <summary>The accessibility element provides summary information when the app starts.</summary>
		SummaryElement = 128,
		/// <summary>The accessibility element is not enabled.</summary>
		NotEnabled = 256,
		/// <summary>Indicates that the accessibility element frequently changes its label or value.</summary>
		UpdatesFrequently = 512,
		/// <summary>The accessibility element should be treated as a search field.</summary>
		SearchField = 1024,
		/// <summary>The accessibility element starts a media session when activated.</summary>
		StartsMediaSession = 2048,
		/// <summary>The accessibility element allows a continuous adjustment over a range of values.</summary>
		Adjustable = 4096,
		/// <summary>The accessibility element allows direct touch interaction for VoiceOver users.</summary>
		AllowsDirectInteraction = 8192,
		/// <summary>The accessibility element causes an automatic page turn when VoiceOver finishes reading the text within the element.</summary>
		CausesPageTurn = 16384,
		/// <summary>The accessibility element is a header that divides content into sections.</summary>
		Header = 65536,
	}

	// NSInteger -> UIImage.h
	/// <summary>An enumeration whose values specify rendering modes for a <see cref="UIKit.UIImage" />.</summary>
	/// <remarks>
	///   <para>
	///     A template image is used as a mask to create the final image. A template image inherits the <see cref="UIView.TintColor" />
	///     of its parent. Application developers who do not want this behavior should use <see cref="UIKit.UIImageRenderingMode.AlwaysOriginal" />.
	///   </para>
	/// </remarks>
	[Native]
	public enum UIImageRenderingMode : long {
		/// <summary>The default rendering mode for the context.</summary>
		Automatic,
		/// <summary>Always draws the original image, without treating it as a template.</summary>
		AlwaysOriginal,
		/// <summary>Always draws the image as a template, ignoring its color information.</summary>
		AlwaysTemplate,
	}

	// NSInteger -> UIMotionEffect.h
	/// <summary>An enumeration whose values specify the axis being monitored by a <see cref="UIKit.UIInterpolatingMotionEffect" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIInterpolatingMotionEffectType : long {
		/// <summary>Interpolates values along the horizontal axis, involving the device facing left or right relative to the user's viewpoint.</summary>
		TiltAlongHorizontalAxis,
		/// <summary>Interpolates along the vertical axis, involving the device facing above or below theuser's viewpoint.</summary>
		TiltAlongVerticalAxis,
	}

	// NSInteger -> UINavigationController.h
	/// <summary>An enumeration whose values specify operations on <see cref="UIKit.UINavigationController" />.</summary>
	/// <remarks>This enumeration is used in calls to <see cref="UINavigationControllerDelegate.GetAnimationControllerForOperation" />.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UINavigationControllerOperation : long {
		/// <summary>No navigation operation is happening.</summary>
		None,
		/// <summary>A UIViewController is being pushed onto the navigation stack.</summary>
		Push,
		/// <summary>The top UIViewController is being removed from the navigation stack.</summary>
		Pop,
	}

	// NSInteger -> UIActivity.h
	/// <summary>An enumeration whose values specify a category for a <see cref="UIKit.UIActivity" />.</summary>
	///     <remarks>
	///       <para>Application developers may choose to group activities in their UI based on this value.</para>
	///     </remarks>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIActivityCategory : long {
		/// <summary>Activities whose primary purpose is to take an action (other than sharing) on the selected item.</summary>
		Action,
		/// <summary>Activities whose purpose is to share the selected item.</summary>
		Share,
	}

	// NSInteger -> UIAttachmentBehavior.h
	/// <summary>An enumeration whose values specify whether a <see cref="UIKit.UIAttachmentBehavior" /> is anchored to a fixed point or to an <see cref="UIKit.IUIDynamicItem" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIAttachmentBehaviorType : long {
		/// <summary>Two dynamic items are attached.</summary>
		Items,
		/// <summary>Connects a dynamic item to an anchor point.</summary>
		Anchor,
	}

	// NSInteger -> UIBarCommon.h
	/// <summary>An enumeration whose values specify locations for <see cref="UIKit.UISearchBar" />, <see cref="UIKit.UINavigationBar" />, or <see cref="UIKit.UIToolbar" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIBarPosition : long {
		/// <summary>Specifies that the position is unspecified.</summary>
		Any,
		/// <summary>The bar is at the bottom of its containing UIView.</summary>
		Bottom,
		/// <summary>The bar is at the top of its containing UIView.</summary>
		Top,
		/// <summary>The bar is at the top of the screen, as well as its containing UIView.</summary>
		TopAttached,
	}

	// NSUInteger -> UICollisionBehavior.h
	/// <summary>An enumeration whose values specify whether a <see cref="UIKit.UICollisionBehavior" /> detects collisions to boundaries, items, or everything.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UICollisionBehaviorMode : ulong {
		/// <summary>The dynamic items collide only with each and not with collision boundaries.</summary>
		Items = 1,
		/// <summary>The dynamic items will not collide with each other, only the specified collision boundaries.</summary>
		Boundaries = 2,
		/// <summary>The dynamic items will collide with each other or the specified collision boundaries.</summary>
		Everything = UInt64.MaxValue,
	}

	// uint32_t -> UIFontDescriptor.h
	/// <summary>Describes some stylistic properties of a typeface (lower 16 bits), and font appearance (top 16 bits), used with UIFontDescriptor.</summary>
	/// <remarks>
	///     </remarks>
	[Flags]
	public enum UIFontDescriptorSymbolicTraits : uint {
		/// <summary>Typeface: italic.</summary>
		Italic = 1 << 0,
		/// <summary>Style is bold</summary>
		Bold = 1 << 1,
		/// <summary>Typeface: Expanded (can not be used with Condensed).</summary>
		Expanded = 1 << 5,
		/// <summary>Typeface: Condensed (can not be used with expanded).</summary>
		Condensed = 1 << 6,
		/// <summary>Typeface: is monospace.</summary>
		MonoSpace = 1 << 10,
		/// <summary>Typeface: contains vertical glyphs and metrics.</summary>
		Vertical = 1 << 11,
		/// <summary>Typeface: is optimized for rendering UI controls.</summary>
		UIOptimized = 1 << 12,
		/// <summary>Typeface: tight leading values (spacing between text lines).</summary>
		TightLeading = 1 << 15,
		/// <summary>Typeface: uses looser leading values (spacing between text lines).</summary>
		LooseLeading = 1 << 16,

		/// <summary>Bitmask that can be used to isolate the font appearance from the typeface information.</summary>
		ClassMask = 0xF0000000,

		/// <summary>Unknown font appearance.</summary>
		ClassUnknown = 0,
		/// <summary>Font appearance: Old style serifs.</summary>
		ClassOldStyleSerifs = 1 << 28,
		/// <summary>Font appearance: transitional serifs.</summary>
		ClassTransitionalSerifs = 2 << 28,
		/// <summary>Font appearance: Modern serifs.</summary>
		ClassModernSerifs = 3 << 28,
		/// <summary>Font appearance: Clarendon style of slab serifs (examples include fonts like Clarendon and Egyptienne).</summary>
		ClassClarendonSerifs = 4 << 28,
		/// <summary>Font appearance: Slab serifs.</summary>
		ClassSlabSerifs = 5 << 28,
		/// <summary>Font appearance: Includes some serifs</summary>
		ClassFreeformSerifs = 7 << 28,
		/// <summary>Font appearance: Sans serifs.</summary>
		ClassSansSerif = 8U << 28,
		/// <summary>Font appearance: Ornamental</summary>
		ClassOrnamentals = 9U << 28,
		/// <summary>Font appearance: Scripts.</summary>
		ClassScripts = 10U << 28,
		/// <summary>Font appearance: symbolic.</summary>
		ClassSymbolic = 12U << 28,
	}

	// NSInteger -> UIResponder.h
	/// <summary>An enumeration whose values flag the hardware modifier keys associated with a <see cref="UIKit.UIKeyCommand" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum UIKeyModifierFlags : long {
		/// <summary>The caps lock is pressed.</summary>
		AlphaShift = 1 << 16,  // This bit indicates CapsLock
		/// <summary>The shift key is pressed.</summary>
		Shift = 1 << 17,
		/// <summary>The control key is pressed.</summary>
		Control = 1 << 18,
		/// <summary>The option key is pressed.</summary>
		Alternate = 1 << 19,
		/// <summary>The command key is pressed.</summary>
		Command = 1 << 20,
		/// <summary>The pressed key is on the numeric pad.</summary>
		NumericPad = 1 << 21,
	}

	// NSInteger -> UIScrollView.h
	/// <summary>An enumeration whose values specify the mode in which the keyboard is dismissed in a scrollview.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UIScrollView.KeyboardDismissMode" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIScrollViewKeyboardDismissMode : long {
		/// <summary>The keyboard does not get dismissed with a drag.</summary>
		None,
		/// <summary>The keyboard is dismissed when a drag begins.</summary>
		OnDrag,
		/// <summary>The keyboard follows the dragging touch offscreen, and can be pulled up again to cancel the dismissal.</summary>
		Interactive,
		[TV (16, 0)] // Added in Xcode 14.0, but headers and documentation say it's available in iOS 7+ and Mac Catalyst 13.1+ (and tvOS 16.0)
		OnDragWithAccessory,
		[TV (16, 0)] // Added in Xcode 14.0, but headers and documentation say it's available in iOS 7+ and Mac Catalyst 13.1+ (and tvOS 16.0)
		InteractiveWithAccessory,
	}

	// NSInteger -> UIWebView.h
	/// <summary>An enumeration whose values specify whether a <see cref="UIKit.UIWebView" />'s <see cref="UIKit.UIWebView.PaginationMode" /> should break by columns or page.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIWebPaginationBreakingMode : long {
		/// <summary>Content respects CSS properties controlling page-breaking.</summary>
		Page,
		/// <summary>Contents respects CSS properties relation to column-breaking.</summary>
		Column,
	}

	// NSInteger -> UIWebView.h
	/// <summary>An enumeration whose values specify valid page-break modes for the <see cref="UIKit.UIWebView" />'s <see cref="UIKit.UIWebView.PaginationMode" /> property.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIWebPaginationMode : long {
		/// <summary>Content appears as one long scrolling view with no pages.</summary>
		Unpaginated,
		/// <summary>The content will be broken into pages that flow from left to right.</summary>
		LeftToRight,
		/// <summary>The content will be broken into pages that flow from top to bottom.</summary>
		TopToBottom,
		/// <summary>The content will be broken into pages that flow from bottom to top.</summary>
		BottomToTop,
		/// <summary>The content will be broken into pages that flow from right to left.</summary>
		RightToLeft,
	}

	// NSInteger -> UIPushBehavior.h
	/// <summary>An enumeration whose values specify whether a <see cref="UIKit.UIPushBehavior" /> force is applied continuously or instantaneously.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UIPushBehavior" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIPushBehaviorMode : long {
		/// <summary>The force is continuous.</summary>
		Continuous,
		/// <summary>The force is applied instantaneously.</summary>
		Instantaneous,
	}

	// NSInteger -> UITabBar.h
	/// <summary>An enumeration whose values specify how a <see cref="UIKit.UITabBarItem" /> is positioned.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UITabBar.ItemPositioning" />
	/// <altmember cref="UIKit.UITabBarAppearance.StackedItemPositioning" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UITabBarItemPositioning : long {
		/// <summary>The item positioning is controlled by the user interface idiom.</summary>
		Automatic,
		/// <summary>The tab bar items are distributed across the width of the tab bar. Default value on iPhone.</summary>
		Fill,
		/// <summary>The tab bar items are centered in the tab bar. Default on iPad.</summary>
		Centered,
	}

	// NSUInteger -> UIView.h
	/// <summary>An enumeration whose values specify valid options for the <see cref="UIKit.UIView.AnimateKeyframes(System.Double,System.Double,UIKit.UIViewKeyframeAnimationOptions,System.Action,UIKit.UICompletionHandler)" /> method.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIViewKeyframeAnimationOptions : ulong {
		/// <summary>The option to layout subviews at commit time so they animate with their parent.</summary>
		LayoutSubviews = UIViewAnimationOptions.LayoutSubviews,
		/// <summary>Whether the user can interact with the views while they are being animated.</summary>
		AllowUserInteraction = UIViewAnimationOptions.AllowUserInteraction,
		/// <summary>Whether to start an animation from the current setting of the in-flight animation. If not set, in-flight animations are allowed to finish before the new animation is started.</summary>
		BeginFromCurrentState = UIViewAnimationOptions.BeginFromCurrentState,
		/// <summary>Whether to repeat the animation indefinitely.</summary>
		Repeat = UIViewAnimationOptions.Repeat,
		/// <summary>Whether to run the animation in both directions. Must be combined with the Repeat option.</summary>
		Autoreverse = UIViewAnimationOptions.Autoreverse,
		/// <summary>Whether to force an animation to use the original duration value specified when the animation was submitted. If not set, the animation inherits the remaining duration of the in-flight animation.</summary>
		OverrideInheritedDuration = UIViewAnimationOptions.OverrideInheritedDuration,
		/// <summary>Whether to not inherit the animation type or any options.</summary>
		OverrideInheritedOptions = UIViewAnimationOptions.OverrideInheritedOptions,

		/// <summary>Use a simple linear calculation for interpolating between keyframe values.</summary>
		CalculationModeLinear = 0 << 10,
		/// <summary>Does not interpolate keyframe values; jumps directly to each keyframe value.</summary>
		CalculationModeDiscrete = 1 << 10,
		/// <summary>Use a simple even-pacing algorithm to interpolate between keyframe values.</summary>
		CalculationModePaced = 2 << 10,
		/// <summary>Use a Catmull-Rom spline to interpolate between keyframe values. The Catmull-Rom parameter is not available for manipulation.</summary>
		CalculationModeCubic = 3 << 10,
		/// <summary>Use a cubic scheme to compute intermediate frames, ignoring timing properties. Timing parameters are implicitly calculated to give the animation a constant velocity.</summary>
		CalculationModeCubicPaced = 4 << 10,
	}

	// NSInteger -> UIView.h
	/// <summary>An enumeration whose values specify adjustment modes for <see cref="UIKit.UIView.TintAdjustmentMode" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIViewTintAdjustmentMode : long {
		/// <summary>Automatic tint adjustment.</summary>
		Automatic,
		/// <summary>Normal tint adjustment.</summary>
		Normal,
		/// <summary>Dimmed tint adjustment.</summary>
		Dimmed,
	}

	// NSUInteger -> UIView.h
	/// <summary>An enumeration specifying system animations, i.e., Delete.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UISystemAnimation : ulong {
		/// <summary>Option to remove views from the view hierarchy when the animation completes.</summary>
		Delete,
	}

	// NSUInteger -> UIGeometry.h
	/// <summary>An enumeration whose values specify screen edges, for use with <see cref="UIKit.UIScreenEdgePanGestureRecognizer.Edges" /> and <see cref="UIKit.UIViewController.EdgesForExtendedLayout" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	public enum UIRectEdge : ulong {
		/// <summary>No edges.</summary>
		None = 0,
		/// <summary>The top edge of the rectangle.</summary>
		Top = 1 << 0,
		/// <summary>The left edge of the rectangle.</summary>
		Left = 1 << 1,
		/// <summary>The bottom edge of the rectangle.</summary>
		Bottom = 1 << 2,
		/// <summary>The right edge of the rectangle.</summary>
		Right = 1 << 3,
		/// <summary>All edges of the rectangle.</summary>
		All = Top | Left | Bottom | Right,
	}

	// Xamarin.iOS home-grown define
	/// <summary>An enumeration whose values specify text effects (e.g., Letterpress). Used with <see cref="UIKit.UIStringAttributes.TextEffect" />.</summary>
	/// <remarks>To be added.</remarks>
	public enum NSTextEffect {
		/// <summary>No style.</summary>
		None,
		/// <summary>A style that looks like paper that has been layered and pressed onto the page.</summary>
		LetterPressStyle,

		// An unkonwn value, the real value can be fetched using the WeakTextEffect: Apple added a new effect and the bindings are old.
		/// <summary>This value is returned when the underlying <see cref="UIKit.UIStringAttributes.WeakTextEffect" />
		/// 	can not be mapped to one of the strongly typed known
		/// 	enumeration values.   In that case use the <see cref="UIKit.UIStringAttributes.WeakTextEffect" />
		/// 	value to determine what kind of effect is being applied. </summary>
		UnknownUseWeakEffect,
	}

	// NSUInteger -> UISearchBar.h
	/// <summary>An enumeration whose values specify the prominence of the <see cref="UIKit.UISearchBar" />.</summary>
	/// <remarks>To be added.</remarks>
	/// <altmember cref="UIKit.UISearchBar.SearchBarStyle" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UISearchBarStyle : ulong {
		/// <summary>The default search bar style.</summary>
		Default,
		/// <summary>A translucent background and the search field is opaque.</summary>
		Prominent,
		/// <summary>No background and the search field is translucent.</summary>
		Minimal,
	}

	// NSInteger -> UIInputView.h
	/// <summary>An enumeration whose value specify the blurring and tinting effects applied to a <see cref="UIKit.UIInputView" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIInputViewStyle : long {
		/// <summary>Applies blurring, but not tinting. This style is appropriate for <see cref="UIKit.UIInputView" />s that should be themed like, but do not look like, the keyboard.</summary>
		Default,
		/// <summary>Applies both blurring and tinting. This style is appropriate for <see cref="UIKit.UIInputView" />s that look like the keyboard (extensions or replacements).</summary>
		Keyboard,
	}

	/// <summary>Enumerates the various interface sizes.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIUserInterfaceSizeClass : long {
		/// <summary>Unspecified interface.</summary>
		Unspecified = 0,
		/// <summary>Compact interface.</summary>
		Compact = 1,
		/// <summary>Regular interface.</summary>
		Regular = 2,
	}

	/// <summary>Enumeration of the styles showing the effect of a <see cref="UIKit.UIAlertAction" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIAlertActionStyle : long {
		/// <summary>A style indicating default behavior.</summary>
		Default,
		/// <summary>A style indicating a <see cref="UIKit.UIAlertAction" /> that cancels the operation associated with the alert.</summary>
		Cancel,
		/// <summary>A style indicating the <see cref="UIKit.UIAlertAction" /> might change or delete data.</summary>
		Destructive,
	}

	/// <summary>Enumerates whether a <see cref="UIKit.UIAlertController" /> is displaying an action sheet or an alert.</summary>
	/// <remarks>
	///       <para>This is how an action sheet is displayed:</para>
	///       <para>
	///         <img href="~/xml/UIKit/_images/actioncontroller-action-sheet.png" alt="Screenshot of the action sheet." />
	///       </para>
	///       <para>
	///       </para>
	///       <para>This is how an alert is displayed:</para>
	///       <para>
	///         <img href="~/xml/UIKit/_images/actioncontroller-alert.png" alt="Image of the alert dialog" />
	///       </para>
	///     </remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIAlertControllerStyle : long {
		/// <summary>Displays the alert controller as an action sheet.</summary>
		ActionSheet,
		/// <summary>Displays the alert controller as an alert.</summary>
		Alert,
	}

	/// <summary>Enumerates the types of blur effect supported by <see cref="UIKit.UIBlurEffect" />.</summary>
	/// <remarks>To be added.</remarks>
	/// <related type="article" href="https://blog.xamarin.com/adding-view-effects-in-ios-8/">Adding View Effects in iOS 8</related>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIBlurEffectStyle : long {
		/// <summary>The blur effect is much lighter than the view.</summary>
		ExtraLight,
		/// <summary>The blur effect is a little lighter than the view.</summary>
		Light,
		/// <summary>The blur effect is darker than the view.</summary>
		Dark,
		/// <summary>The blur effect is much darker than the view.</summary>
		[NoiOS]
		[NoMacCatalyst]
		ExtraDark,
		/// <summary>The normal blur effect for the UI style.</summary>
		[MacCatalyst (13, 1)]
		Regular = 4,
		/// <summary>The blur effect adapts to the style of UI so that it is noticeable.</summary>
		[MacCatalyst (13, 1)]
		Prominent = 5,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemUltraThinMaterial,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemThinMaterial,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemMaterial,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemThickMaterial,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemChromeMaterial,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemUltraThinMaterialLight,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemThinMaterialLight,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemMaterialLight,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemThickMaterialLight,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemChromeMaterialLight,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemUltraThinMaterialDark,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemThinMaterialDark,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemMaterialDark,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemThickMaterialDark,
		[NoTV]
		[MacCatalyst (13, 1)]
		SystemChromeMaterialDark,
	}

	/// <summary>Enumerates various types of printing tasks. Used with <see cref="UIKit.UIPrinter.SupportedJobTypes" />.</summary>
	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIPrinterJobTypes : long {
		/// <summary>Printer support is unknown.</summary>
		Unknown = 0,
		/// <summary>Supports standard printing of documents.</summary>
		Document = 1 << 0,
		/// <summary>Supports printing upon envelopes.</summary>
		Envelope = 1 << 1,
		/// <summary>Supports printing upon cut labels.</summary>
		Label = 1 << 2,
		/// <summary>Supports photographic print quality printing.</summary>
		Photo = 1 << 3,
		/// <summary>Supports printing of receipts.</summary>
		Receipt = 1 << 4,
		/// <summary>Supports the printing of documents or photos on a continuous paper roll.</summary>
		Roll = 1 << 5,
		/// <summary>Supports printing in formats larger than the ISO A3 size.</summary>
		LargeFormat = 1 << 6,
		/// <summary>Supports postcard printing.</summary>
		Postcard = 1 << 7,
	}

	/// <summary>Enumerates the various types of user notification. Can be "OR"ed together as flags.</summary>
	[NoTV]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNAuthorizationOptions' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNAuthorizationOptions' instead.")]
	[Native]
	[Flags]
	public enum UIUserNotificationType : ulong {
		/// <summary>No notifications types are allowed.</summary>
		None = 0,
		/// <summary>Modifications to the application icon's badge.</summary>
		Badge = 1 << 0,
		/// <summary>Plays a sound.</summary>
		Sound = 1 << 1,
		/// <summary>Text alerts.</summary>
		Alert = 1 << 2,
	}

	/// <summary>Whether a <see cref="UIKit.UIUserNotificationAction" /> should run the app in foreground or background mode.</summary>
	[NoTV]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationActionOptions' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNNotificationActionOptions' instead.")]
	[Native]
	public enum UIUserNotificationActivationMode : ulong {
		/// <summary>The activated app should be placed in the foreground.</summary>
		Foreground,
		/// <summary>The activated app should be placed in the background.</summary>
		Background,
	}

	/// <summary>Enumerates the amount of space available for a <see cref="UIKit.UIUserNotificationAction" />. Used with <see cref="UIKit.UIMutableUserNotificationCategory.SetActions(UIKit.UIUserNotificationAction[],UIKit.UIUserNotificationActionContext)" /></summary>
	[NoTV]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationCategory.Actions' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNNotificationCategory.Actions' instead.")]
	[Native]
	public enum UIUserNotificationActionContext : ulong {
		/// <summary>The full UI is displayed for the notification's alert. Allows up to four <see cref="UIKit.UIUserNotificationAction" />s.</summary>
		Default,
		/// <summary>Only minimal space is available for the notification's alert. Allows up to two <see cref="UIKit.UIUserNotificationAction" />s.</summary>
		Minimal,
	}

	/// <summary>Enumerates the insertion points for custom menu items.</summary>
	[Deprecated (PlatformName.iOS, 11, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum UIDocumentMenuOrder : ulong {
		/// <summary>Custom menu items will be inserted at the start of the menu.</summary>
		First,
		/// <summary>Custom menu items will be inserted at the end of the menu.</summary>
		Last,
	}

	/// <summary>Enumerates the types of file transfer operations used by the document picker.</summary>
	[Deprecated (PlatformName.iOS, 14, 0, message: "Use the designated constructors instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the designated constructors instead.")]
	[Native]
	public enum UIDocumentPickerMode : ulong {
		/// <summary>Imports a file from a specified destination outside the sandbox for the app.</summary>
		Import,
		/// <summary>Opens an external file that is located outside the sandobox for the app.</summary>
		Open,
		/// <summary>Exports a local file to a specified destination outside the sandbox for the app.</summary>
		ExportToService,
		/// <summary>Moves a local file outside of the sandbox for the app, providing access to it as an external file.</summary>
		MoveToService,
	}

	/// <summary>Enumerates how elements should be navigated by the assistive technology.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityNavigationStyle : long {

		/// <summary>The system guesses the best way to navigate.</summary>
		Automatic = 0,
		/// <summary>The components of this object should be treated as separate items.</summary>
		Separate = 1,
		/// <summary>The components of this object should be combined and navigated as a single item.</summary>
		Combined = 2,
	}

	/// <summary>Enumerates valid display modes for an expanded <see cref="UIKit.UISplitViewController" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UISplitViewControllerDisplayMode : long {
		/// <summary>The system decides the most appropriate display mode.</summary>
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

		/// <summary>The primary <see cref="UIKit.UISplitViewController" /> is hidden.</summary>
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'SecondaryOnly' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'SecondaryOnly' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'SecondaryOnly' instead.")]
		PrimaryHidden = SecondaryOnly,

		/// <summary>The primary and secondary <see cref="UIKit.UIViewController" />s are displayed side-by-side.</summary>
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'OneBesideSecondary' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'OneBesideSecondary' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'OneBesideSecondary' instead.")]
		AllVisible = OneBesideSecondary,

		/// <summary>The primary <see cref="UIKit.UISplitViewController" /> overlays the secondary, which is partially visible.</summary>
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'OneOverSecondary' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'OneOverSecondary' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'OneOverSecondary' instead.")]
		PrimaryOverlay = OneOverSecondary,
	}

	/// <summary>Enumerates characteristics of the button displayed in a table row. Used with the <see cref="UITableViewRowAction.Create(UITableViewRowActionStyle,string,Action{UITableViewRowAction,NSIndexPath})" /> factory method.</summary>
	[Native]
	[NoTV]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'UIContextualActionStyle' and corresponding APIs instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIContextualActionStyle' and corresponding APIs instead.")]
	public enum UITableViewRowActionStyle : long {
		/// <summary>The default button appearance.</summary>
		Default,
		/// <summary>The appearance for a button that may delete data.</summary>
		Destructive = Default,
		/// <summary>The appearance for a nondestructive button.</summary>
		Normal,
	}

	// Utility enum for UITransitionContext[To|From]ViewKey
	/// <summary>Enumerates whether a <see cref="UIKit.UIViewController" /> for a transition is associated with the "from" <see cref="UIKit.UIView" /> or the "to" <see cref="UIKit.UIView" />. Used with <see cref="UIKit.UIViewControllerTransitionCoordinatorContext_Extensions.GetTransitionViewController(UIKit.IUIViewControllerTransitionCoordinatorContext,UIKit.UITransitionViewControllerKind)" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	public enum UITransitionViewControllerKind {
		/// <summary>Specifies a transition to a specified view.</summary>
		ToView,
		/// <summary>Specifies a transition from a specified view.</summary>
		FromView,
	}

	// note [Native] since it maps to UIFontWeightConstants fields (CGFloat)
	/// <summary>Enumerates font weights.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	public enum UIFontWeight {
		/// <summary>A very light weight.</summary>
		UltraLight,
		/// <summary>A thin weight.</summary>
		Thin,
		/// <summary>A light weight.</summary>
		Light,
		/// <summary>The regular weight of the font.</summary>
		Regular,
		/// <summary>A medium weight.</summary>
		Medium,
		/// <summary>A semibold weight.</summary>
		Semibold,
		/// <summary>A bold weight.</summary>
		Bold,
		/// <summary>A heavy weight.</summary>
		Heavy,
		/// <summary>To be added.</summary>
		Black,
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	public enum UIFontWidth {
		Condensed,
		Standard,
		Expanded,
		Compressed,
	}

	/// <summary>How the views in a <see cref="UIKit.UIStackView" /> are distributed along the view's alignment axis.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIStackViewDistribution : long {
		/// <summary>Attempts to fill along the <see cref="UIKit.UIStackView.Axis" /> according to the <see cref="UIKit.UIStackView.ArrangedSubviews" />' <see cref="UIKit.UIView.ContentCompressionResistancePriority(UIKit.UILayoutConstraintAxis)" /> and <see cref="UIKit.UIView.ContentHuggingPriority(UIKit.UILayoutConstraintAxis)" /> properties.</summary>
		Fill,
		/// <summary>Attempts to fill along the <see cref="UIKit.UIStackView.Axis" /> by giving the <see cref="UIKit.UIStackView.ArrangedSubviews" /> the same space.</summary>
		FillEqually,
		/// <summary>Attempts to fill along the <see cref="UIKit.UIStackView.Axis" /> by giving the <see cref="UIKit.UIStackView.ArrangedSubviews" /> the space required by their <see cref="UIKit.UIView.IntrinsicContentSize" /> property.</summary>
		FillProportionally,
		/// <summary>Attempts to maintain equal spacing between <see cref="UIKit.UIStackView.ArrangedSubviews" />.</summary>
		EqualSpacing,
		/// <summary>Attempts to maintain equal center-to-center spacing between <see cref="UIKit.UIStackView.ArrangedSubviews" />.</summary>
		EqualCentering,
	}

	/// <include file="../../docs/api/UIKit/UIStackViewAlignment.xml" path="/Documentation/Docs[@DocId='T:UIKit.UIStackViewAlignment']/*" />
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIStackViewAlignment : long {
		/// <summary>Views are arranged to fill available space perpendicular to the <see cref="UIKit.UIStackView.Axis" />.</summary>
		Fill,
		/// <summary>Views are aligned based on the leading edge of the first <see cref="UIKit.UIStackView.ArrangedSubviews" />. (Vertical only.)
		/// </summary>
		Leading,
		/// <summary>Horizontal layout, top edges aligned to the top of the <see cref="UIKit.UIStackView" />.</summary>
		Top = Leading,
		/// <summary>Views are aligned based on the baseline of the first <see cref="UIKit.UIStackView.ArrangedSubviews" />. (Horizontal only.)</summary>
		FirstBaseline,
		/// <summary>Views are aligned along the <see cref="UIKit.UIStackView.Axis" />, in the center of the <see cref="UIKit.UIStackView" />.</summary>
		Center,
		/// <summary>Views are aligned based on the trailing edge of the first <see cref="UIKit.UIStackView.ArrangedSubviews" />. (Vertical only.)</summary>
		Trailing,
		/// <summary>Horizontal layout, bottom edges aligned to the bottom of the <see cref="UIKit.UIStackView" />.</summary>
		Bottom = Trailing,
		/// <summary>Views are aligned based on the baseline of the last <see cref="UIKit.UIStackView.ArrangedSubviews" />. (Horizontal only.)</summary>
		LastBaseline,
	}

	/// <summary>Flagging enumeration that can specify overriding of writing direction.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum NSWritingDirectionFormatType : long {
		/// <summary>Indicates text embedded within text of a different writing direction.</summary>
		Embedding = 0 << 1,
		/// <summary>Overrides the default writing direction of the text.</summary>
		Override = 1 << 1,
	}

	/// <summary>Enumerates desired page cutting behavior for roll-feed printers.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPrinterCutterBehavior : long {
		/// <summary>Indicates that the printer should not cut pages.</summary>
		NoCut,
		/// <summary>Indicates that the printer's default behavior should be used.</summary>
		PrinterDefault,
		/// <summary>Indicates that the paper is cut after each page is printed.</summary>
		CutAfterEachPage,
		/// <summary>Indicates that the paper is cut after each copy of the document is printed.</summary>
		CutAfterEachCopy,
		/// <summary>Indicates that the paper is cut after each print job is completed.</summary>
		CutAfterEachJob,
	}

	/// <summary>Enumerates whether the user may respond to a user notification with text input.</summary>
	[NoTV]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationAction' or 'UNTextInputNotificationAction' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNNotificationAction' or 'UNTextInputNotificationAction' instead.")]
	[Native]
	public enum UIUserNotificationActionBehavior : ulong {
		/// <summary>The user may not respond to the notification with text input.</summary>
		Default,
		/// <summary>The user may not respond to the notification with text input.</summary>
		TextInput,
	}

	/// <summary>Describes a view's contents so that the app dev can control if it should be flipped between left-to-right and right-to-left layouts.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UISemanticContentAttribute : long {
		/// <summary>Indicates a default left-right view that is flipped when necessary.</summary>
		Unspecified = 0,
		/// <summary>Indicates a view that contains playback controls, which are not left-right flipped.</summary>
		Playback,
		/// <summary>Indicates a view that contains directional controls, which are not left-right flipped.</summary>
		Spatial,
		/// <summary>Forces the contents to be laid out left to right.</summary>
		ForceLeftToRight,
		/// <summary>Forces the contents to be laid out right to left.</summary>
		ForceRightToLeft,
	}

	/// <summary>Enumerates descriptions of collision boundary geometries for dynamic items.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDynamicItemCollisionBoundsType : ulong {
		/// <summary>Indicates that the collision boundary is a rectangle.</summary>
		Rectangle,
		/// <summary>Indicates that the collision boundary is an ellipse.</summary>
		Ellipse,
		/// <summary>Indicates that the collision boundary is a closed path.</summary>
		Path,
	}

	/// <summary>Enumerates the 3D Touch capabilities on a device.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIForceTouchCapability : long {
		/// <summary>It is not known whether 3D Touch capabilities are available. For example, a view that has not been added to a view hierarchy has an unknown 3D Touch capability.</summary>
		Unknown = 0,
		/// <summary>3D Touch capabilities are unavailable on the device..</summary>
		Unavailable = 1,
		/// <summary>3D Touch capabilities are available on the device.</summary>
		Available = 2,
	}

	/// <summary>Enumeration that defines the various styles of <see cref="UIKit.UIPreviewAction" /> objects.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIPreviewActionStyle : long {
		/// <summary>Indicates the default style.</summary>
		Default,
		/// <summary>Indicates the style for a selected peek action.</summary>
		Selected,
		/// <summary>Indicates the style for a peek action that is destructive.</summary>
		Destructive,
	}

	/// <summary>Enumerates the phases of the button-press life-cycle.</summary>
	/// <remarks>
	///       <para>
	///         <see cref="UIKit.UIPress" /> objects model not just digital presses but, for instance, trackpads, so a <see cref="UIKit.UIPress" /> may have both location and force data. Additionally, the system may cancel tracking of a button press at any time. This leads to the following state-machine:</para>
	///       <para>
	///         <img href="~/xml/UIKit/_images/UIKit.UIPressPhase_States.png" alt="Statechart showing states and transitions occurring during a press" />
	///       </para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPressPhase : long {
		/// <summary>The initial state of a button. Indicates that a press has begun.</summary>
		Began,
		/// <summary>Indicates that either the location of the button press or it's <see cref="UIKit.UIPress.Force" /> has changed.</summary>
		Changed,
		/// <summary>Indicates that the button is still down, with the same location and force as previously.</summary>
		Stationary,
		/// <summary>Indicates that the button has been released.</summary>
		Ended,
		/// <summary>Indicates that the system has canceled tracking of this button-press sequence.</summary>
		Cancelled,
	}

	/// <summary>Enumerates button types for <see cref="UIKit.UIPress" /> objects (see <see cref="UIKit.UIPress.Type" />).</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPressType : long {
		/// <summary>Arrow pointing up.</summary>
		UpArrow,
		/// <summary>Arrow pointing down.</summary>
		DownArrow,
		/// <summary>Arrow pointing left.</summary>
		LeftArrow,
		/// <summary>Arrow pointing right.</summary>
		RightArrow,
		/// <summary>Button indicating selection.</summary>
		Select,
		/// <summary>The dedicated Menu button.</summary>
		Menu,
		/// <summary>The button dedicated to toggling playback.</summary>
		PlayPause,
		[TV (14, 3)]
		[NoiOS]
		[NoMacCatalyst]
		PageUp = 30,
		[TV (14, 3)]
		[NoiOS]
		[NoMacCatalyst]
		PageDown = 31,
		[TV (18, 1), NoiOS, NoMacCatalyst]
		TVRemoteOneTwoThree = 32,
		[TV (18, 1), NoiOS, NoMacCatalyst]
		TVRemoteFourColors = 33,
	}

	/// <summary>Enumeration whose values define how a <see cref="UIKit.UITableViewCell" /> displays when it is focused.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITableViewCellFocusStyle : long {
		/// <summary>Indicates that the cell will have the default visual response when it receives the focus.</summary>
		Default,
		/// <summary>Indicates that the cell will not alter its appearance in when it receives the focus.</summary>
		Custom,
	}

	/// <summary>Enumerates display gamuts.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDisplayGamut : long {
		/// <summary>An indeterminate display gamut.</summary>
		Unspecified = -1,
		/// <summary>The Srgb display gamut.</summary>
		Srgb,
		/// <summary>The P3 display gamut ("wide-color").</summary>
		P3,
	}

	/// <summary>Enumerates layout directions.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITraitEnvironmentLayoutDirection : long {
		/// <summary>The layout direction is unknown.</summary>
		Unspecified = -1,
		/// <summary>Layout is done from the left to the right.</summary>
		LeftToRight = UIUserInterfaceLayoutDirection.LeftToRight,
		/// <summary>Layout is done from the right to the left.</summary>
		RightToLeft = UIUserInterfaceLayoutDirection.RightToLeft,
	}

	/// <summary>Enumerates CarPlay and tvOS UI themes.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIUserInterfaceStyle : long {
		/// <summary>Indicates that the theme is not known.</summary>
		Unspecified,
		/// <summary>Indicates a light theme.</summary>
		Light,
		/// <summary>Indicates a dark theme.</summary>
		Dark,
	}

	/// <summary>Enumerates activities that a user might use with a URL or text attachment.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextItemInteraction : long {
		/// <summary>The user wants to execute the default action.</summary>
		InvokeDefaultAction,
		/// <summary>The user should be presented a list of possible actions.</summary>
		PresentActions,
		/// <summary>The user wishes to see a preview of the action.</summary>
		Preview,
	}

	/// <summary>Enumerates animation states.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIViewAnimatingState : long {
		/// <summary>Animations have not yet started.</summary>
		Inactive,
		/// <summary>Animations are either running or paused.</summary>
		Active,
		/// <summary>The animation is stopped and cannot be resume.</summary>
		Stopped,
	}

	/// <summary>Enumerates the endpoints and current position of an animation.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIViewAnimatingPosition : long {
		/// <summary>The ending position.</summary>
		End,
		/// <summary>The starting position.</summary>
		Start,
		/// <summary>The most-recent position.</summary>
		Current,
	}

	/// <summary>Enumerates the various types of timing curves.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITimingCurveType : long {
		/// <summary>The built-in UIKit timing curves.</summary>
		Builtin,
		/// <summary>A timing curve based on a cubic Bezier curve.</summary>
		Cubic,
		/// <summary>A timing curve based on custom spring parameters.</summary>
		Spring,
		/// <summary>A type of timing curve that starts with cubic timing but has spring behavior as well.</summary>
		Composed,
	}

	/// <summary>Enumerates the ear (or ears) for which the accessibility device is paired.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityHearingDeviceEar : ulong {
		/// <summary>The accessibility device is not currently associated with either or both bears.</summary>
		None = 0,
		/// <summary>The accessibility device is associated with the left ear.</summary>
		Left = 1 << 1,
		/// <summary>The accessibility device is associated with the right ear.</summary>
		Right = 1 << 2,
		/// <summary>The accessibility device is associated with both ears.</summary>
		Both = Left | Right,
	}

	/// <summary>Enmumerates search directions.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityCustomRotorDirection : long {
		/// <summary>The previous search result.</summary>
		Previous,
		/// <summary>The next search result.</summary>
		Next,
	}

	/// <summary>Flagging enumeration for the permission options for Cloud sharing.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UICloudSharingPermissionOptions : ulong {
		/// <summary>Default options.</summary>
		Standard = 0,
		/// <summary>Allows access to anyone.</summary>
		AllowPublic = 1 << 0,
		/// <summary>Allows access only to those who have been invited.</summary>
		AllowPrivate = 1 << 1,
		/// <summary>The shared data is read-only.</summary>
		AllowReadOnly = 1 << 2,
		/// <summary>Users may read and modify the data.</summary>
		AllowReadWrite = 1 << 3,
	}

	/// <summary>Enumerates reasons that an editing session ends.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextFieldDidEndEditingReason : long {
		/// <summary>Indicates that the reason for ending the edit are unknown.</summary>
		Unknown = -1, // helper value (not in headers)
		/// <summary>Indicates that the user saved the edit.</summary>
		Committed,
		/// <summary>Indicates that the user cancelled the edit.</summary>
		[NoiOS]
		[NoMacCatalyst]
		Cancelled,
	}

	/// <summary>Enumerates index display behavior during scrolling.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIScrollViewIndexDisplayMode : long {
		/// <summary>Indicates that the index is automatically hidden or displayed.</summary>
		Automatic,
		/// <summary>Indicates that the index is always hidden.</summary>
		AlwaysHidden,
	}

	/// <summary>Enumerates safe area inset adjustment behaviors.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIScrollViewContentInsetAdjustmentBehavior : long {
		/// <summary>Indicates that safe area insets are automatically adjusted when content is adjusted.</summary>
		Automatic,
		/// <summary>Indicates that safe area insets are adjusted only in scroll directions.</summary>
		ScrollableAxes,
		/// <summary>Indicates that safe area insets are never included in content adjustment.</summary>
		Never,
		/// <summary>Indicates that safe area insets are always included in content adjustment.</summary>
		Always,
	}

	/// <summary>Enumerates the types that implement the <see cref="UIKit.IUIAccessibilityContainer" /> interface.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityContainerType : long {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		DataTable,
		/// <summary>To be added.</summary>
		List,
		/// <summary>To be added.</summary>
		Landmark,
		[MacCatalyst (13, 1)]
		SemanticGroup,
	}

	/// <summary>Enumerates smart quote conversion behavior.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextSmartQuotesType : long {
		/// <summary>Indicates the default behavior.</summary>
		Default,
		/// <summary>Indicates that smart quote conversion is never performed.</summary>
		No,
		/// <summary>Indicates that smart quote conversion is enabled.</summary>
		Yes,
	}

	/// <summary>Enumerates behaviors for converting hyphens to en or em dashes.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextSmartDashesType : long {
		/// <summary>Indicates the default behavior.</summary>
		Default,
		/// <summary>Indicates that conversion is never performed.</summary>
		No,
		/// <summary>Indicates that conversion is enabled.</summary>
		Yes,
	}

	/// <summary>Enumerates behaviors for padding insertions and unpadding deletions.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextSmartInsertDeleteType : long {
		/// <summary>Indicates the default behavior.</summary>
		Default,
		/// <summary>Indicates that padding and unpadding is never performed.</summary>
		No,
		/// <summary>Indicates that padding and unpadding is enabled.</summary>
		Yes,
	}

	/// <summary>Enumerates the types of content that may be represented by a <see cref="UIKit.UIAccessibilityCustomRotor" /> object.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityCustomSystemRotorType : long {
		/// <summary>An unknown or non-specific type.</summary>
		None = 0,
		/// <summary>A non-visited link.</summary>
		Link,
		/// <summary>A link that has been visited.</summary>
		VisitedLink,
		/// <summary>Heading text of any level.</summary>
		Heading,
		/// <summary>The highest heading level.</summary>
		HeadingLevel1,
		/// <summary>Heading level 2 text.</summary>
		HeadingLevel2,
		/// <summary>Heading level 3 text.</summary>
		HeadingLevel3,
		/// <summary>Heading level 4 text.</summary>
		HeadingLevel4,
		/// <summary>Heading level 5 text.</summary>
		HeadingLevel5,
		/// <summary>Heading level 5 text.</summary>
		HeadingLevel6,
		/// <summary>Text that is bolded.</summary>
		BoldText,
		/// <summary>Italicized text.</summary>
		ItalicText,
		/// <summary>Text that has been underlined.</summary>
		UnderlineText,
		/// <summary>A word that has been misspelled.</summary>
		MisspelledWord,
		/// <summary>A graphic element.</summary>
		Image,
		/// <summary>A text field.</summary>
		TextField,
		/// <summary>A table.</summary>
		Table,
		/// <summary>A sequential list.</summary>
		List,
		/// <summary>A landmark.</summary>
		Landmark,
	}

	/// <summary>Enumerates drag activity drop resolutions.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDropOperation : ulong {
		/// <summary>Indicates that dropping items will not result in data transfer.</summary>
		Cancel = 0,
		/// <summary>Indicates that dropping items will result in the items being rejected, though they would normall be accepted.</summary>
		Forbidden = 1,
		/// <summary>Indicates that dropping items will result in the data being copied.</summary>
		Copy = 2,
		/// <summary>Indicates that dropping items will result in the data being moved.</summary>
		Move = 3,
	}

	/// <summary>Enumerates behaviors when dragging text.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UITextDragOptions : long {
		/// <summary>Indicates that preview text contains the original colors.</summary>
		None = 0,
		/// <summary>Indicates that preview text has its original colors removed.</summary>
		StripTextColorFromPreviews = (1 << 0),
	}

	/// <summary>Enumerates behaviors in response to dropped text.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextDropAction : ulong {
		/// <summary>Indicates that dropped text is inserted at the insertion point, regardless of whether there is a text selection.</summary>
		Insert = 0,
		/// <summary>Indicates that dropped text replaces the selected text in the target, or is inserted at the drop point if there is no selection.</summary>
		ReplaceSelection,
		/// <summary>Indicates that dropped text replaces all text in the target.</summary>
		ReplaceAll,
	}

	/// <summary>Enumerates text drop progress notification styles.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextDropProgressMode : ulong {
		/// <summary>Indicates that the system's blocking alert will be used.</summary>
		System = 0,
		/// <summary>Indicates that the developer will provide a custom notification.</summary>
		Custom,
	}

	/// <summary>Enumerates behaviors for noneditable views when receiving dropped text.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextDropEditability : ulong {
		/// <summary>Indicates that dropped text is not accepted.</summary>
		No = 0,
		/// <summary>Indicates that dropped text is accepted, but the text is not editable after it is dropped.</summary>
		Temporary,
		/// <summary>Indicates that dropped text is accepted, and the dropped text is editable after it is dropped.</summary>
		Yes,
	}

	/// <summary>Enumerates collection view reorganization speeds for drop animations.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UICollectionViewReorderingCadence : long {
		/// <summary>Indicates immediate reorganization.</summary>
		Immediate,
		/// <summary>Indicates a short delay followed by a quick reordering.</summary>
		Fast,
		/// <summary>Indicates a short delay before reorganizing.</summary>
		Slow,
	}

	/// <summary>Enumerates drop actions.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UICollectionViewDropIntent : long {
		/// <summary>Indicates that no action was specified.</summary>
		Unspecified,
		/// <summary>Indicates that the dropped items will be inserted at the destination index path.</summary>
		InsertAtDestinationIndexPath,
		/// <summary>Indicates that the dropped items will be inserted into the destination index path item.</summary>
		InsertIntoDestinationIndexPath,
	}

	/// <summary>Enumerates drag operation states.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UICollectionViewCellDragState : long {
		/// <summary>Indicates that the cell is not in a drag operation.</summary>
		None,
		/// <summary>Indicates that the cel is being animated to the drag point.</summary>
		Lifting,
		/// <summary>Indicates that the cell is being dragged.</summary>
		Dragging,
	}

	/// <summary>Enumerates presets for exporting images.</summary>
	[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'PHPicker' instead.")]
	[Native]
	public enum UIImagePickerControllerImageUrlExportPreset : long {
		/// <summary>Indicates the preset for converting HEIF images to JPEG.</summary>
		Compatible = 0,
		/// <summary>Indicates the preset for passing unconverted image data.</summary>
		Current,
	}

	/// <summary>Enumerates action button styles.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIContextualActionStyle : long {
		/// <summary>Indicates the style for a button that causes a nondestructive action.</summary>
		Normal,
		/// <summary>Indicates the style for a button that causes a destructive button.</summary>
		Destructive,
	}

	/// <summary>Enumerates cell states while the cell is involved in a drag operation.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITableViewCellDragState : long {
		/// <summary>Indicates that the cell is not lifting or dragging.</summary>
		None,
		/// <summary>Indicates that the cell is lifting.</summary>
		Lifting,
		/// <summary>Indicates that the cell is dragging.</summary>
		Dragging,
	}

	/// <summary>Enumerates inset behaviors in a table view.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITableViewSeparatorInsetReference : long {
		/// <summary>Indicates that insets are calculated from the edge of the cell.</summary>
		CellEdges,
		/// <summary>Indicates that insets are calculated from the default separator insets.</summary>
		AutomaticInsets,
	}

	/// <summary>Enumerates drop handling options.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITableViewDropIntent : long {
		/// <summary>Indicates the lack of a drop proposal.</summary>
		Unspecified,
		/// <summary>Indicates that the content will be placed at the dropped location.</summary>
		InsertAtDestinationIndexPath,
		/// <summary>Indicates that the content will be placed into the item that is at the dropped location.</summary>
		InsertIntoDestinationIndexPath,
		/// <summary>Indicates that the drop will be automatically handled based on drop location.</summary>
		Automatic,
	}

	/// <summary>Enumerates primary view controller locations.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UISplitViewControllerPrimaryEdge : long {
		/// <summary>Indicates that the primary view controller is on the leading edge.</summary>
		Leading,
		/// <summary>Indicates that the primary view controller is on the trailing edge.</summary>
		Trailing,
	}

	/// <summary>Enumerates drop progress indicator styles.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDropSessionProgressIndicatorStyle : ulong {
		/// <summary>Indicates no progress indicator.</summary>
		None,
		/// <summary>Indicates the default progress indicator.</summary>
		Default,
	}

	/// <summary>Enumerates states for spring-loaded interactions.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UISpringLoadedInteractionEffectState : long {
		/// <summary>Indicates a view state for a view that is not currently available for spring loading. (The view's default visual style.)</summary>
		Inactive,
		/// <summary>Indicates a view state for a view that can be spring loaded.</summary>
		Possible,
		/// <summary>Indicates a view state for a view that is about to spring load.</summary>
		Activating,
		/// <summary>Indicates a view state for a view that has been spring loaded.</summary>
		Activated,
	}

	/// <summary>Enumerates the import modes of the document browser.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDocumentBrowserImportMode : ulong {
		/// <summary>The document cannot be imported.</summary>
		None,
		/// <summary>Copies the file, leaving the original unchanged.</summary>
		Copy,
		/// <summary>Moves the file.</summary>
		Move,
	}

	/// <summary>Enumerates document browser styles.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIDocumentBrowserUserInterfaceStyle : ulong {
		/// <summary>Indicates a white background.</summary>
		White = 0,
		/// <summary>Indicates a light background.</summary>
		Light,
		/// <summary>Indicates a dark background.</summary>
		Dark,
	}

	/// <summary>Enumerates allowable action locations.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UIDocumentBrowserActionAvailability : long {
		/// <summary>Indicates an action that appears in the Edit Menu after a long press in a supported document.</summary>
		Menu = 1,
		/// <summary>Indicates an action that appears when the document browser is in Select mode.</summary>
		NavigationBar = 1 << 1,
	}

	/// <summary>Enumerates the type responsible for the drop operation.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextDropPerformer : ulong {
		/// <summary>Indicates that the drop operation should be performed by the text view.</summary>
		View = 0,
		/// <summary>Indicates that the drop operation should be performed by the delegate object.</summary>
		Delegate,
	}

	/// <summary>Enumerates whether and when large titles are displayed.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum UINavigationItemLargeTitleDisplayMode : long {
		/// <summary>Indicates that large title text is displayed if it was displayed in the previous navigation item.</summary>
		Automatic,
		/// <summary>Indicates that large titles are always displayed.</summary>
		Always,
		/// <summary>Indicates that large title text is always displayed.</summary>
		Never,
		[iOS (17, 0), NoTV, MacCatalyst (17, 0)]
		Inline,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UICollectionViewFlowLayoutSectionInsetReference : long {
		/// <summary>To be added.</summary>
		ContentInset,
		/// <summary>To be added.</summary>
		SafeArea,
		/// <summary>To be added.</summary>
		LayoutMargins,
	}

	/// <summary>Enumerates how the data from a <see cref="Foundation.NSItemProvider" /> should be presented.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPreferredPresentationStyle : long {
		/// <summary>There is no preferred presentation style.</summary>
		Unspecified = 0,
		/// <summary>The data should be shown inline.</summary>
		Inline,
		/// <summary>The data should be shown as an attachment.</summary>
		Attachment,
	}

	/// <summary>Enumerates the document browser error code.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("UIDocumentBrowserErrorDomain")]
	public enum UIDocumentBrowserErrorCode : long {
		/// <summary>Indicates an error.</summary>
		Generic = 1,
		NoLocationAvailable = 2,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UIGraphicsImageRendererFormatRange : long {
		/// <summary>To be added.</summary>
		Unspecified = -1,
		/// <summary>To be added.</summary>
		Automatic = 0,
		/// <summary>To be added.</summary>
		Extended,
		/// <summary>To be added.</summary>
		Standard,
	}

	/// <summary>Enumerates print failure reasons.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPrintErrorCode : long {
		/// <summary>Indicates that the selected device cannot print.</summary>
		NotAvailableError = 1,
		/// <summary>Indicates that printable content was supplied.</summary>
		NoContentError,
		/// <summary>Indicates that UIKit does not recognize the image format.</summary>
		UnknownImageFormatError,
		/// <summary>Indicates that an internal print error occurred.</summary>
		JobFailedError,
	}

	[MacCatalyst (13, 1)]
	[ErrorDomain ("UISceneErrorDomain")]
	[Native]
	public enum UISceneErrorCode : long {
		MultipleScenesNotSupported,
		RequestDenied,
		GeometryRequestUnsupported = 100,
		GeometryRequestDenied,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UIImageSymbolScale : long {
		Default = -1,
		Unspecified = 0,
		Small = 1,
		Medium,
		Large,
	}

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

	[MacCatalyst (13, 1)]
	[Native]
	public enum UISceneActivationState : long {
		Unattached = -1,
		ForegroundActive,
		ForegroundInactive,
		Background,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UIMenuElementState : long {
		Off,
		On,
		Mixed,
	}

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
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIMenuOptions : ulong {
		DisplayInline = 1uL << 0,
		Destructive = 1uL << 1,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		SingleSelection = 1uL << 5,
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		DisplayAsPalette = 1uL << 7,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIContextMenuInteractionCommitStyle : long {
		Dismiss = 0,
		Pop,
	}

	[MacCatalyst (13, 1)]
	public enum UIWindowSceneSessionRole {
		[Field ("UIWindowSceneSessionRoleApplication")]
		Application,

		[Field ("UIWindowSceneSessionRoleExternalDisplay")]
		ExternalDisplay,

		[NoTV]
		[MacCatalyst (13, 1)]
#if HAS_CARPLAY
		[Field ("CPTemplateApplicationSceneSessionRoleApplication", "CarPlay")]
#endif
		CarTemplateApplication,

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Field ("UIWindowSceneSessionRoleExternalDisplayNonInteractive")]
		ExternalDisplayNonInteractive,

		[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Field ("UIWindowSceneSessionRoleAssistiveAccessApplication")]
		AssistiveAccessApplication,

		[NoTV, NoMacCatalyst, iOS (27, 1)]
		[Field ("UIWindowSceneSessionRoleCameraCaptureAccessory")]
		CameraCaptureAccessory,
	}

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
		[Deprecated (PlatformName.iOS, 26, 0, message: "Use 'UIMenuNewItem' instead.")]
		[Deprecated (PlatformName.TvOS, 26, 0, message: "Use 'UIMenuNewItem' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 26, 0, message: "Use 'UIMenuNewItem' instead.")]
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

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("UIMenuAutoFill")]
		AutoFill,
		[TV (18, 1), iOS (18, 1), MacCatalyst (18, 1)]
		[Field ("UIMenuOpen")]
		Open,

		[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Field ("UIMenuFindPanel")]
		FindPanel,

		[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Field ("UIMenuNewItem")]
		NewItem,
	}

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

	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAccessibilityContrast : long {
		Unspecified = -1,
		Normal,
		High,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UILegibilityWeight : long {
		Unspecified = -1,
		Regular,
		Bold,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIUserInterfaceLevel : long {
		Unspecified = -1,
		Base,
		Elevated,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UIEditingInteractionConfiguration : long {
		None = 0,
		Default = 1,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UISplitViewControllerBackgroundStyle : long {
		None,
		Sidebar,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UITabBarItemAppearanceStyle : long {
		Stacked,
		Inline,
		CompactInline,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextAlternativeStyle : long {
		None,
		LowConfidence,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum UITextInteractionMode : long {
		Editable,
		NonEditable,
	}

	[NoTV]
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

	[MacCatalyst (13, 1)]
	[Native]
	public enum UIWindowSceneDismissalAnimation : long {
		Standard = 1,
		Commit = 2,
		Decline = 3,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIActivityItemsConfigurationInteraction {
		[Field ("UIActivityItemsConfigurationInteractionShare")]
		Share,
		[iOS (16, 4), MacCatalyst (16, 4)]
		[Field ("UIActivityItemsConfigurationInteractionCopy")]
		Copy,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	public enum UIActivityItemsConfigurationPreviewIntent {
		[Field ("UIActivityItemsConfigurationPreviewIntentFullSize")]
		FullSize,
		[Field ("UIActivityItemsConfigurationPreviewIntentThumbnail")]
		Thumbnail,
	}

	[NoTV, iOS (13, 4)]
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
	[iOS (13, 4), TV (13, 4)]
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

	[Flags, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIEventButtonMask : ulong {
		Primary = 1L << 0,
		Secondary = 1L << 1,
	}

	[Flags, TV (13, 4), iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIAxis : ulong {
		Neither = 0uL,
		Horizontal = 1uL << 0,
		Vertical = 1uL << 1,
		Both = (Horizontal | Vertical),
	}

	[NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIScrollType : long {
		Discrete,
		Continuous,
	}

	[Flags, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIScrollTypeMask : ulong {
		Discrete = 1L << 0,
		Continuous = 1L << 1,
		All = Discrete | Continuous,
	}

	[NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPointerEffectTintMode : long {
		None = 0,
		Overlay,
		Underlay,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIButtonRole : long {
		Normal,
		Primary,
		Cancel,
		Destructive,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellAccessoryDisplayedState : long {
		Always,
		WhenEditing,
		WhenNotEditing,
	}

	[NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellAccessoryOutlineDisclosureStyle : long {
		Automatic,
		Header,
		Cell,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellAccessoryPlacement : long {
		Leading,
		Trailing,
	}

	[NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellConfigurationDragState : long {
		None,
		Lifting,
		Dragging,
	}

	[NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICellConfigurationDropState : long {
		None,
		NotTargeted,
		Targeted,
	}

	[TV (14, 0), iOS (14, 0)]
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

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICollectionLayoutListHeaderMode : long {
		None,
		Supplementary,
		FirstItemInSection,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIContentInsetsReference : long {
		Automatic,
		None,
		SafeArea,
		LayoutMargins,
		ReadableContent,
	}

	[TV (17, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIContextMenuInteractionAppearance : long {
		Unknown = 0,
		Rich,
		Compact,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIUserInterfaceActiveAppearance : long {
		Unspecified = -1,
		Inactive,
		Active,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIListContentTextAlignment : long {
		Natural,
		Center,
		Justified,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIPageControlInteractionState : long {
		None = 0,
		Discrete = 1,
		Continuous = 2,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UIPageControlBackgroundStyle : long {
		Automatic = 0,
		Prominent = 1,
		Minimal = 2,
	}

#if XAMCORE_5_0
	[iOS (14, 0), TV (14, 0), NoMacCatalyst]
#else
	[iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Obsoleted (PlatformName.MacCatalyst, 17, 4, message: Constants.ApiRemovedGeneral)]
#endif
	public enum UIPasteboardDetectionPattern {
		[Field ("UIPasteboardDetectionPatternProbableWebURL")]
		ProbableWebUrl,
		[Field ("UIPasteboardDetectionPatternProbableWebSearch")]
		ProbableWebSearch,
		[Field ("UIPasteboardDetectionPatternNumber")]
		Number,
#if XAMCORE_5_0
		[iOS (15, 0), TV (15, 0), NoMacCatalyst]
#else
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 17, 4, message: Constants.ApiRemovedGeneral)]
#endif
		[Field ("UIPasteboardDetectionPatternLink")]
		Link,
#if XAMCORE_5_0
		[iOS (15, 0), TV (15, 0), NoMacCatalyst]
#else
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 17, 4, message: Constants.ApiRemovedGeneral)]
#endif
		[Field ("UIPasteboardDetectionPatternPhoneNumber")]
		PhoneNumber,
#if XAMCORE_5_0
		[iOS (15, 0), TV (15, 0), NoMacCatalyst]
#else
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 17, 4, message: Constants.ApiRemovedGeneral)]
#endif
		[Field ("UIPasteboardDetectionPatternEmailAddress")]
		EmailAddress,
#if XAMCORE_5_0
		[iOS (15, 0), TV (15, 0), NoMacCatalyst]
#else
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 17, 4, message: Constants.ApiRemovedGeneral)]
#endif
		[Field ("UIPasteboardDetectionPatternPostalAddress")]
		PostalAddress,
#if XAMCORE_5_0
		[iOS (15, 0), TV (15, 0), NoMacCatalyst]
#else
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 17, 4, message: Constants.ApiRemovedGeneral)]
#endif
		[Field ("UIPasteboardDetectionPatternCalendarEvent")]
		CalendarEvent,
#if XAMCORE_5_0
		[iOS (15, 0), TV (15, 0), NoMacCatalyst]
#else
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 17, 4, message: Constants.ApiRemovedGeneral)]
#endif
		[Field ("UIPasteboardDetectionPatternShipmentTrackingNumber")]
		ShipmentTrackingNumber,
#if XAMCORE_5_0
		[iOS (15, 0), TV (15, 0), NoMacCatalyst]
#else
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 17, 4, message: Constants.ApiRemovedGeneral)]
#endif
		[Field ("UIPasteboardDetectionPatternFlightNumber")]
		FlightNumber,
#if XAMCORE_5_0
		[iOS (15, 0), TV (15, 0), NoMacCatalyst]
#else
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Obsoleted (PlatformName.MacCatalyst, 17, 4, message: Constants.ApiRemovedGeneral)]
#endif
		[Field ("UIPasteboardDetectionPatternMoneyAmount")]
		MoneyAmount,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, NoiOS]
	[Native]
	public enum UISceneCollectionJoinBehavior : long {
		Automatic,
		Preferred,
		Disallowed,
		PreferredWithoutActivating,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UISplitViewControllerStyle : long {
		Unspecified,
		DoubleColumn,
		TripleColumn,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UISplitViewControllerColumn : long {
		Primary,
		Supplementary,
		Secondary,
		Compact,
		[NoTV, iOS (26, 0), MacCatalyst (26, 0)]
		Inspector,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UISplitViewControllerSplitBehavior : long {
		Automatic,
		Tile,
		Overlay,
		Displace,
	}

	[NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UISwitchStyle : long {
		Automatic = 0,
		Checkbox,
		Sliding,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UICollectionLayoutListFooterMode : long {
		None,
		Supplementary,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, NoiOS]
	[Native]
	public enum UITitlebarSeparatorStyle : long {
		Automatic,
		None,
		Line,
		Shadow,
	}

	[NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum UINavigationItemBackButtonDisplayMode : long {
		Default = 0,
		Generic = 1,
		Minimal = 2,
	}

	// NSInteger -> UIGuidedAccessRestrictions.h
	/// <summary>An enumeration whose values specify whether a Guided Access restriction is in an allow or deny state.</summary>
	/// <remarks>
	///     </remarks>
	/// <altmember cref="UIKit.UIGuidedAccessRestriction.GetState" />
	[Native]
	[MacCatalyst (13, 1)]
	public enum UIGuidedAccessRestrictionState : long {
		/// <summary>The application should allow the behavior in question.</summary>
		Allow,
		/// <summary>The application should not allow the behavior in question.</summary>
		Deny,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

		[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Field ("UIActionNewFromPasteboard")]
		NewFromPasteboard,
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIBandSelectionInteractionState : long {
		Possible = 0,
		Began,
		Selecting,
		Ended,
	}

	[TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIBehavioralStyle : ulong {
		Automatic = 0,
		Pad,
		Mac,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIButtonConfigurationSize : long {
		Medium,
		Small,
		Mini,
		Large,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIButtonConfigurationTitleAlignment : long {
		Automatic,
		Leading,
		Center,
		Trailing,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIButtonConfigurationCornerStyle : long {
		Fixed = -1,
		Dynamic,
		Small,
		Medium,
		Large,
		Capsule,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIButtonConfigurationMacIdiomStyle : long {
		Automatic,
		Bordered,
		Borderless,
		BorderlessTinted,
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIFocusGroupPriority : long {
		Ignored = 0,
		PreviouslyFocused = 1000,
		Prioritized = 2000,
		CurrentlyFocused = Int64.MaxValue,
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIFocusHaloEffectPosition : long {
		Automatic = 0,
		Outside,
		Inside,
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	public enum UISheetPresentationControllerDetentIdentifier {
		[DefaultEnumValue]
		[Field (null)]
		Unknown = -1,

		[Field ("UISheetPresentationControllerDetentIdentifierMedium")]
		Medium,

		[Field ("UISheetPresentationControllerDetentIdentifierLarge")]
		Large,
	}

	/// <summary>Specifies where a sheet is placed within its presenting view.</summary>
	[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum UISheetPresentationControllerPlacement : long {
		/// <summary>The system determines the sheet placement.</summary>
		Automatic,

		/// <summary>Places the sheet on the leading edge.</summary>
		[NoTV]
		Leading,

		/// <summary>Centers the sheet within the presenting view.</summary>
		[NoTV]
		Center,

		/// <summary>Places the sheet on the trailing edge.</summary>
		[NoTV]
		Trailing,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum UIWindowScenePresentationStyle : ulong {
		Automatic,
		Standard,
		Prominent,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UIHdrHeadroomUsageLimit : long {
		Unspecified = -1,
		Active,
		Inactive,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UIImageSymbolColorRenderingMode : long {
		Automatic = 0,
		Flat,
		Gradient,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UIImageSymbolVariableValueMode : long {
		Automatic = 0,
		Color,
		Draw,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UIMenuElementRepeatBehavior : long {
		Automatic = 0,
		Repeatable,
		NonRepeatable,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UIMenuSystemElementGroupPreference : long {
		Automatic = 0,
		Removed,
		Included,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UIMenuSystemFindElementGroupConfigurationStyle : long {
		Automatic = 0,
		Search,
		NonEditableText,
		EditableText,
	}

	[NoTV, iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UISliderStyle : long {
		Default,
		Thumbless,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UISplitViewControllerLayoutEnvironment : long {
		None,
		Expanded,
		Collapsed,
	}

	[NoTV, iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UITabAccessoryEnvironment : long {
		Unspecified,
		None,
		Regular,
		Inline,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UITabBarMinimizeBehavior : long {
		Automatic = 0,
		[NoTV]
		Never,
		[NoTV]
		OnScrollDown,
		[NoTV]
		OnScrollUp,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UIViewLayoutRegionAdaptivityAxis : long {
		None,
		Horizontal,
		Vertical,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum UIGlassEffectStyle : long {
		Regular,
		Clear,
	}

	// NSInteger -> UIMenuElement.h
	[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum UIMenuElementImageVisibility : long {
		Automatic = 0,
		Visible = 1,
		Hidden = 2,
	}

	// NSInteger -> UIBarMinimization.h
	[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum UIBarMinimizationBehavior : long {
		Automatic = 0,
		[NoTV]
		Never,
		[NoTV]
		OnScrollDown,
		[NoTV]
		OnScrollUp,
	}

	// NSInteger -> UIBarMinimization.h
	[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum UIBarMinimizationSafeAreaAdjustment : long {
		Automatic = 0,
		[NoTV]
		Enabled,
		[NoTV]
		Disabled,
	}

	// NSInteger -> UIBarMinimization.h
	[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum UIBarMinimizationRestorationBehavior : long {
		Automatic = 0,
		[NoTV]
		AtScrollEdge,
	}

	// NSInteger -> UITabBarControllerSidebar.h
	[NoTV, iOS (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum UITabBarControllerSidebarPlacement : long {
		Automatic = 0,
		Sidebar = 1,
		TabBar = 2,
	}

	// NSUInteger -> UIDragInteraction.h
	[NoTV, iOS (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum UIDragLiftBehavior : ulong {
		Default = 0,
		Extended = 1,
	}

	// NSUInteger -> UITextView.h
	[Flags]
	[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum UITextAttachmentViewProviderReusePolicy : ulong {
		ScrollingOutOfViewport = 1uL << 0,
		EditingInlineParagraphs = 1uL << 1,
	}

	[NoTV, NoMacCatalyst, iOS (27, 1)]
	[Native]
	public enum UIArrangementViewControllerViewPlacement : long {
		None = 0,
		Primary = 1,
		Secondary = 2,
	}

	[NoTV, NoMacCatalyst, iOS (27, 1)]
	[Native]
	public enum UIBarButtonItemAxisBehavior : long {
		Automatic = 0,
		[NoTV]
		HorizontalOnly = 1,
		[NoTV]
		VerticalPreferred = 2,
	}

	[NoTV, NoMacCatalyst, iOS (27, 1)]
	[Native]
	public enum UIHingeStatus : long {
		Unknown = 0,
		Closed = 1,
		PartiallyOpen = 2,
		FullyOpen = 3,
	}

	[NoTV, NoMacCatalyst, iOS (27, 1)]
	[Native]
	public enum UIVerticalBarBehavior : long {
		Automatic = 0,
		Disabled = 1,
	}

	[NoTV, NoMacCatalyst, iOS (27, 1)]
	[Native]
	public enum UIVerticalBarCompressionBehavior : long {
		Automatic = 0,
		[NoTV]
		PrefersBarItems = 1,
		[NoTV]
		PrefersTabBar = 2,
	}

	[NoTV, NoMacCatalyst, iOS (27, 1)]
	[Native]
	public enum UIVerticalBarEdge : long {
		Unspecified = 0,
		[NoTV]
		Leading = 1,
		[NoTV]
		Trailing = 2,
	}

	[Flags]
	[NoTV, NoMacCatalyst, iOS (27, 1)]
	[Native]
	public enum UIViewReservedRegionQueryOptions : ulong {
		None = 0,
		IncludeInactive = 1uL << 0,
	}

	// NSInteger -> NSTextTable.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSTextBlockValueType : long {
		Absolute = 0,
		Percentage = 1,
	}

	// NSInteger -> NSTextTable.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSTextBlockDimension : long {
		Width = 0,
		MinimumWidth = 1,
		MaximumWidth = 2,
		Height = 4,
		MinimumHeight = 5,
		MaximumHeight = 6,
	}

	// NSInteger -> NSTextTable.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSTextBlockLayer : long {
		Padding = -1,
		Border = 0,
		Margin = 1,
	}

	// NSInteger -> NSTextTable.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSTextBlockVerticalAlignment : long {
		Top = 0,
		Middle = 1,
		Bottom = 2,
		Baseline = 3,
	}

	// NSInteger -> NSTextTable.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSTextTableLayoutAlgorithm : long {
		Automatic = 0,
		Fixed = 1,
	}
}
