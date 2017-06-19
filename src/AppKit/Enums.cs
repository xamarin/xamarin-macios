//
// Copyright 2010, 2011 Novell, Inc.
// Copyright 2011, Xamarin, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using XamCore.ObjCRuntime;

namespace XamCore.AppKit {

	[Native]
	public enum NSRunResponse : nint {
		Stopped = -1000,
		Aborted = -1001,
		Continues = -1002
	}

	[Native]
	public enum NSApplicationActivationOptions : nuint_compat_int {
		Default = 0,
		ActivateAllWindows = 1,
		ActivateIgnoringOtherWindows = 2
	}

	[Native]
	public enum NSApplicationActivationPolicy : nint {
		Regular, Accessory, Prohibited
	}
	
	[Flags]
	[Native]
	public enum NSApplicationPresentationOptions : nuint_compat_int {
		Default                    = 0,
		AutoHideDock               = (1 <<  0),
		HideDock                   = (1 <<  1),

		AutoHideMenuBar            = (1 <<  2),
		HideMenuBar                = (1 <<  3),

		DisableAppleMenu           = (1 <<  4),
		DisableProcessSwitching    = (1 <<  5),
		DisableForceQuit           = (1 <<  6),
		DisableSessionTermination  = (1 <<  7),
		DisableHideApplication     = (1 <<  8),
		DisableMenuBarTransparency = (1 <<  9),

		FullScreen                 = (1 << 10),
		AutoHideToolbar            = (1 << 11)
	}

	[Native]
	public enum NSApplicationDelegateReply : nuint_compat_int {
		Success,
		Cancel,
		Failure
	}

	[Native]
	public enum NSRequestUserAttentionType : nuint_compat_int {
		CriticalRequest = 0,
		InformationalRequest = 10
	}

	[Native]
	public enum NSApplicationTerminateReply : nuint_compat_int {
		Cancel, Now, Later
	}

	[Native]
	public enum NSApplicationPrintReply : nuint_compat_int {
		Cancelled, Success, Failure, ReplyLater
	}

#if !XAMCORE_4_0
	[Native]
	public enum NSApplicationLayoutDirection : nint {
		LeftToRight = 0,
		RightToLeft = 1
	}
#endif

	[Native]
	public enum NSImageInterpolation : nuint_compat_int {
		Default, None, Low, Medium, High
	}

	[Native]
	public enum NSComposite : nuint_compat_int {
		Clear,
		Copy,
		SourceOver,
		SourceIn,
		SourceOut,
		SourceAtop,
		DestinationOver,
		DestinationIn,
		DestinationOut,
		DestinationAtop,
		XOR,
		PlusDarker,
		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_10, Message = "Use NSCompositeSourceOver instead")]
		Highlight,
		PlusLighter,
		[Mac (10,10)] Multiply,
		[Mac (10,10)] Screen,
		[Mac (10,10)] Overlay,
		[Mac (10,10)] Darken,
		[Mac (10,10)] Lighten,
		[Mac (10,10)] ColorDodge,
		[Mac (10,10)] ColorBurn,
		[Mac (10,10)] SoftLight,
		[Mac (10,10)] HardLight,
		[Mac (10,10)] Difference,
		[Mac (10,10)] Exclusion,
		[Mac (10,10)] Hue,
		[Mac (10,10)] Saturation,
		[Mac (10,10)] Color,
		[Mac (10,10)] Luminosity
	}

	[Native]
	public enum NSBackingStore : nuint_compat_int {
		Retained, Nonretained, Buffered
	}

	[Native]
	public enum NSWindowOrderingMode : nint {
		Below = -1, Out, Above,
	}

	[Native]
	public enum NSFocusRingPlacement : nuint_compat_int {
		RingOnly, RingBelow, RingAbove,
	}

	[Native]
	public enum NSFocusRingType : nuint_compat_int {
		Default, None, Exterior
	}
	
	[Native]
	public enum NSColorRenderingIntent : nint {
		Default,
		AbsoluteColorimetric,
		RelativeColorimetric,
		Perceptual,
		Saturation
		
	}

	[Native]
	public enum NSRectEdge : nuint_compat_int {
		MinXEdge, MinYEdge, MaxXEdge, MaxYEdge
	}

	[Native]
	public enum NSUserInterfaceLayoutDirection : nint {
		LeftToRight, RightToLeft
	}

#region NSColorSpace
	[Native]
	public enum NSColorSpaceModel : nint {
		Unknown = -1,
		Gray,
		RGB,
		CMYK,
		LAB,
		DeviceN,
		Indexed,
		Pattern
    }
#endregion

#region NSFileWrapper
#if !XAMCORE_3_0
	[Flags]
	[Native]
	[Obsolete ("Use NSFileWrapperReadingOptions in Foundation instead")]
	public enum NSFileWrapperReadingOptions : nuint_compat_int {
		Immediate = 1, WithoutMapping = 2
	}
#endif
#endregion
	
#region NSParagraphStyle
	[Native]
	public enum NSTextTabType : nuint_compat_int {
		Left, Right, Center, Decimal
	}

	[Native]
	public enum NSLineBreakMode : nuint_compat_int {
		ByWordWrapping,
		CharWrapping,
		Clipping,
		TruncatingHead,
		TruncatingTail,
		TruncatingMiddle
	}
	
#endregion
	
#region NSCell Defines 

#if !XAMCORE_4_0
	[Native]
	[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_10, Message = "Use formatters instead")]
	public enum NSType : nuint_compat_int {
	    Any			= 0,
	    Int			= 1,
	    PositiveInt		= 2,
	    Float		= 3,
	    PositiveFloat	= 4,
	    Double		= 6,
	    PositiveDouble	= 7
	}
#endif
	
	[Native]
	public enum NSCellType : nuint_compat_int {
	    Null,
	    Text,
	    Image
	}
	
	[Native]
	public enum NSCellAttribute : nuint_compat_int {
		CellDisabled,
		CellState,
		PushInCell,
		CellEditable,
		ChangeGrayCell,
		CellHighlighted,
		CellLightsByContents,
		CellLightsByGray,
		ChangeBackgroundCell,
		CellLightsByBackground,
		CellIsBordered,
		CellHasOverlappingImage,
		CellHasImageHorizontal,
		CellHasImageOnLeftOrBottom,
		CellChangesContents,
		CellIsInsetButton,
		CellAllowsMixedState,
	}
	
	[Native]
	public enum NSCellImagePosition : nuint_compat_int {
		NoImage,
		ImageOnly,
		ImageLeft,
		ImageRight,
		ImageBelow,
		ImageAbove,
		ImageOverlaps,
		[Mac (10,12)]
		ImageLeading,
		[Mac (10,12)]
		ImageTrailing,
	}
	
	[Native]
	public enum NSImageScale : nuint_compat_int {
		ProportionallyDown = 0,
		AxesIndependently,     
		None,                 
		ProportionallyUpOrDown
	}
	
	[Native]
	public enum NSCellStateValue : nint {
		Mixed = -1,
		Off,
		On
	}

	[Flags]
	[Native]
#if XAMCORE_2_0
	public enum NSCellStyleMask : nuint_compat_int {
#else
	public enum NSCellMask : nuint_compat_int {
#endif
		NoCell = 0,
		ContentsCell = 1 << 0,
		PushInCell = 1 << 1, 
		ChangeGrayCell = 1 << 2,
		ChangeBackgroundCell = 1 << 3
	}

	[Flags]
	[Native]
	public enum NSCellHit : nuint_compat_int {
		None,
		ContentArea = 1,
		EditableTextArea = 2,
		TrackableArae = 4
	}
	
	[Native]
	public enum NSControlTint : nuint {
		Default  = 0,	// system 'default'
		Blue     = 1,
		Graphite = 6,
		Clear    = 7
	}
	
	[Native]
	public enum NSControlSize : nuint_compat_int {
		Regular, 
		Small,
		Mini
	}

	[Native]
	public enum NSBackgroundStyle : nint {
		Light, Dark, Raised, Lowered
	}
#endregion

#region NSImage
	
	[Native]
	public enum NSImageLoadStatus : nuint_compat_int {
	    		Completed,
	    		Cancelled,
	    		InvalidData,
	    		UnexpectedEOF,
	    		ReadError
	}
	
	[Native]
	public enum NSImageCacheMode : nuint_compat_int {
		Default, 
		Always,  
		BySize,  
		Never    
	}

	[Mac (10,10)]
	[Native]
	public enum NSImageResizingMode : nint {
		Stretch,
		Tile
	}
		
#endregion
	
#region NSAlert
	[Native]
	public enum NSAlertStyle : nuint_compat_int {
		Warning, Informational, Critical
	}

	[Mac (10,9)]
	[Native]
	public enum NSModalResponse : nint {
		OK = 1,
		Cancel = 0,
		Stop = -1000,
		Abort = -1001,
		Continue = -1002
	}
#endregion

#region NSEvent
	[Native]
	public enum NSEventType : nuint_compat_int {
		LeftMouseDown = 1,            
		LeftMouseUp = 2,
		RightMouseDown = 3,
		RightMouseUp = 4,
		MouseMoved = 5,
		LeftMouseDragged = 6,
		RightMouseDragged = 7,
		MouseEntered = 8,
		MouseExited = 9,
		KeyDown = 10,
		KeyUp = 11,
		FlagsChanged = 12,
		AppKitDefined = 13,
		SystemDefined = 14,
		ApplicationDefined = 15,
		Periodic = 16,
		CursorUpdate = 17,

		ScrollWheel = 22,

		TabletPoint = 23,
		TabletProximity = 24,

		OtherMouseDown = 25,
		OtherMouseUp = 26,
		OtherMouseDragged = 27,

		Gesture = 29,
		Magnify = 30,
		Swipe = 31,
		Rotate = 18,
		BeginGesture = 19,
		EndGesture = 20,

		SmartMagnify = 32,
		QuickLook = 33,
		Pressure = 34, // 10.10.3, 64-bit-only
		DirectTouch = 37 // 10.10
	}

	[Flags]
	public enum NSEventMask : ulong {
		LeftMouseDown         = 1UL << (int)NSEventType.LeftMouseDown,
		LeftMouseUp           = 1UL << (int)NSEventType.LeftMouseUp,
		RightMouseDown        = 1UL << (int)NSEventType.RightMouseDown,
		RightMouseUp          = 1UL << (int)NSEventType.RightMouseUp,
		MouseMoved            = 1UL << (int)NSEventType.MouseMoved,
		LeftMouseDragged      = 1UL << (int)NSEventType.LeftMouseDragged,
		RightMouseDragged     = 1UL << (int)NSEventType.RightMouseDragged,
		MouseEntered          = 1UL << (int)NSEventType.MouseEntered,
		MouseExited           = 1UL << (int)NSEventType.MouseExited,
		KeyDown               = 1UL << (int)NSEventType.KeyDown,
		KeyUp                 = 1UL << (int)NSEventType.KeyUp,
		FlagsChanged          = 1UL << (int)NSEventType.FlagsChanged,
		AppKitDefined         = 1UL << (int)NSEventType.AppKitDefined,
		SystemDefined         = 1UL << (int)NSEventType.SystemDefined,
		ApplicationDefined    = 1UL << (int)NSEventType.ApplicationDefined,
		Periodic              = 1UL << (int)NSEventType.Periodic,
		CursorUpdate          = 1UL << (int)NSEventType.CursorUpdate,
		ScrollWheel           = 1UL << (int)NSEventType.ScrollWheel,
		TabletPoint           = 1UL << (int)NSEventType.TabletPoint,
		TabletProximity       = 1UL << (int)NSEventType.TabletProximity,
		OtherMouseDown        = 1UL << (int)NSEventType.OtherMouseDown,
		OtherMouseUp          = 1UL << (int)NSEventType.OtherMouseUp,
		OtherMouseDragged     = 1UL << (int)NSEventType.OtherMouseDragged,
		EventGesture          = 1UL << (int)NSEventType.Gesture,
		EventMagnify          = 1UL << (int)NSEventType.Magnify,
		EventSwipe            = 1UL << (int)NSEventType.Swipe,
		EventRotate           = 1UL << (int)NSEventType.Rotate,
		EventBeginGesture     = 1UL << (int)NSEventType.BeginGesture,
		EventEndGesture       = 1UL << (int)NSEventType.EndGesture,
		SmartMagnify          = 1UL << (int)NSEventType.SmartMagnify,
		Pressure              = 1UL << (int)NSEventType.Pressure, // 10.10.3, 64-bit-only
		DirectTouch           = 1UL << (int)NSEventType.DirectTouch, // 10.10
		AnyEvent              = unchecked ((nuint)UInt64.MaxValue)
	}

	[Flags]
	[Native]
	public enum NSEventModifierMask : nuint {
		AlphaShiftKeyMask         = 1 << 16,
		ShiftKeyMask              = 1 << 17,
		ControlKeyMask            = 1 << 18,
		AlternateKeyMask          = 1 << 19,
		CommandKeyMask            = 1 << 20,
		NumericPadKeyMask         = 1 << 21,
		HelpKeyMask               = 1 << 22,
		FunctionKeyMask           = 1 << 23,
		DeviceIndependentModifierFlagsMask    = 0xffff0000
	}

	[Native]
	public enum NSPointingDeviceType : nuint_compat_int {
		Unknown, Pen, Cursor, Eraser
	}

	[Flags]
	[Native]
#if XAMCORE_2_0
	public enum NSEventButtonMask : nuint_compat_int {
#else
	public enum NSPointingDeviceMask : nuint_compat_int {
#endif
		Pen = 1, PenLower = 2, PenUpper = 4
	}

#if !XAMCORE_4_0
	[Native]
	public enum NSKey : nuint_compat_int {
#else
	public enum NSKey : int
#endif
		A              = 0x00,
		S              = 0x01,
		D              = 0x02,
		F              = 0x03,
		H              = 0x04,
		G              = 0x05,
		Z              = 0x06,
		X              = 0x07,
		C              = 0x08,
		V              = 0x09,
		B              = 0x0B,
		Q              = 0x0C,
		W              = 0x0D,
		E              = 0x0E,
		R              = 0x0F,
		Y              = 0x10,
		T              = 0x11,
		D1             = 0x12,
		D2             = 0x13,
		D3             = 0x14,
		D4             = 0x15,
		D6             = 0x16,
		D5             = 0x17,
		Equal          = 0x18,
		D9             = 0x19,
		D7             = 0x1A,
		Minus          = 0x1B,
		D8             = 0x1C,
		D0             = 0x1D,
		RightBracket   = 0x1E,
		O              = 0x1F,
		U              = 0x20,
		LeftBracket    = 0x21,
		I              = 0x22,
		P              = 0x23,
		L              = 0x25,
		J              = 0x26,
		Quote          = 0x27,
		K              = 0x28,
		Semicolon      = 0x29,
		Backslash      = 0x2A,
		Comma          = 0x2B,
		Slash          = 0x2C,
		N              = 0x2D,
		M              = 0x2E,
		Period         = 0x2F,
		Grave          = 0x32,
		KeypadDecimal  = 0x41,
		KeypadMultiply = 0x43,
		KeypadPlus     = 0x45,
		KeypadClear    = 0x47,
		KeypadDivide   = 0x4B,
		KeypadEnter    = 0x4C,
		KeypadMinus    = 0x4E,
		KeypadEquals   = 0x51,
		Keypad0        = 0x52,
		Keypad1        = 0x53,
		Keypad2        = 0x54,
		Keypad3        = 0x55,
		Keypad4        = 0x56,
		Keypad5        = 0x57,
		Keypad6        = 0x58,
		Keypad7        = 0x59,
		Keypad8        = 0x5B,
		Keypad9        = 0x5C,
		Return         = 0x24,
		Tab            = 0x30,
		Space          = 0x31,
		Delete         = 0x33,
		Escape         = 0x35,
		Command        = 0x37,
		Shift          = 0x38,
		CapsLock       = 0x39,
		Option         = 0x3A,
		Control        = 0x3B,
		RightShift     = 0x3C,
		RightOption    = 0x3D,
		RightControl   = 0x3E,
		Function       = 0x3F,
		VolumeUp       = 0x48,
		VolumeDown     = 0x49,
		Mute           = 0x4A,
		ForwardDelete  = 0x75,
		ISOSection     = 0x0A,
		JISYen         = 0x5D,
		JISUnderscore  = 0x5E,
		JISKeypadComma = 0x5F,
		JISEisu        = 0x66,
		JISKana        = 0x68,
#if XAMCORE_2_0
		F18            = 0x4F,
		F19            = 0x50,
		F20            = 0x5A,
		F5             = 0x60,
		F6             = 0x61,
		F7             = 0x62,
		F3             = 0x63,
		F8             = 0x64,
		F9             = 0x65,
		F11            = 0x67,
		F13            = 0x69,
		F16            = 0x6A,
		F14            = 0x6B,
		F10            = 0x6D,
		F12            = 0x6F,
		F15            = 0x71,
		Help           = 0x72,
		Home           = 0x73,
		PageUp         = 0x74,
		F4             = 0x76,
		End            = 0x77,
		F2             = 0x78,
		PageDown       = 0x79,
		F1             = 0x7A,
		LeftArrow      = 0x7B,
		RightArrow     = 0x7C,
		DownArrow      = 0x7D,
		UpArrow        = 0x7E
#else
		UpArrow        = 0xF700,
		DownArrow      = 0xF701,
		LeftArrow      = 0xF702,
		RightArrow     = 0xF703,
		F1             = 0xF704,
		F2             = 0xF705,
		F3             = 0xF706,
		F4             = 0xF707,
		F5             = 0xF708,
		F6             = 0xF709,
		F7             = 0xF70A,
		F8             = 0xF70B,
		F9             = 0xF70C,
		F10            = 0xF70D,
		F11            = 0xF70E,
		F12            = 0xF70F,
		F13            = 0xF710,
		F14            = 0xF711,
		F15            = 0xF712,
		F16            = 0xF713,
		F17            = 0xF714,
		F18            = 0xF715,
		F19            = 0xF716,
		F20            = 0xF717,
		F21            = 0xF718,
		F22            = 0xF719,
		F23            = 0xF71A,
		F24            = 0xF71B,
		F25            = 0xF71C,
		F26            = 0xF71D,
		F27            = 0xF71E,
		F28            = 0xF71F,
		F29            = 0xF720,
		F30            = 0xF721,
		F31            = 0xF722,
		F32            = 0xF723,
		F33            = 0xF724,
		F34            = 0xF725,
		F35            = 0xF726,
		Insert         = 0xF727,
		Home           = 0xF729,
		Begin          = 0xF72A,
		End            = 0xF72B,
		PageUp         = 0xF72C,
		PageDown       = 0xF72D,
		PrintScreen    = 0xF72E,
		ScrollLock     = 0xF72F,
		Pause          = 0xF730,
		SysReq         = 0xF731,
		Break          = 0xF732,
		Reset          = 0xF733,
		Stop           = 0xF734,
		Menu           = 0xF735,
		User           = 0xF736,
		System         = 0xF737,
		Print          = 0xF738,
		ClearLine      = 0xF739,
		ClearDisplay   = 0xF73A,
		InsertLine     = 0xF73B,
		DeleteLine     = 0xF73C,
		InsertChar     = 0xF73D,
		DeleteChar     = 0xF73E,
		Prev           = 0xF73F,
		Next           = 0xF740,
		Select         = 0xF741,
		Execute        = 0xF742,
		Undo           = 0xF743,
		Redo           = 0xF744,
		Find           = 0xF745,
		Help           = 0xF746,
		ModeSwitch     = 0xF747
#endif
	}

#if !XAMCORE_4_0
	[Native]
	public enum NSFunctionKey : nuint_compat_int {
#else
	public enum NSFunctionKey : int {
#endif
		UpArrow        = 0xF700,
		DownArrow      = 0xF701,
		LeftArrow      = 0xF702,
		RightArrow     = 0xF703,
		F1             = 0xF704,
		F2             = 0xF705,
		F3             = 0xF706,
		F4             = 0xF707,
		F5             = 0xF708,
		F6             = 0xF709,
		F7             = 0xF70A,
		F8             = 0xF70B,
		F9             = 0xF70C,
		F10            = 0xF70D,
		F11            = 0xF70E,
		F12            = 0xF70F,
		F13            = 0xF710,
		F14            = 0xF711,
		F15            = 0xF712,
		F16            = 0xF713,
		F17            = 0xF714,
		F18            = 0xF715,
		F19            = 0xF716,
		F20            = 0xF717,
		F21            = 0xF718,
		F22            = 0xF719,
		F23            = 0xF71A,
		F24            = 0xF71B,
		F25            = 0xF71C,
		F26            = 0xF71D,
		F27            = 0xF71E,
		F28            = 0xF71F,
		F29            = 0xF720,
		F30            = 0xF721,
		F31            = 0xF722,
		F32            = 0xF723,
		F33            = 0xF724,
		F34            = 0xF725,
		F35            = 0xF726,
		Insert         = 0xF727,
		Delete         = 0xF728,
		Home           = 0xF729,
		Begin          = 0xF72A,
		End            = 0xF72B,
		PageUp         = 0xF72C,
		PageDown       = 0xF72D,
		PrintScreen    = 0xF72E,
		ScrollLock     = 0xF72F,
		Pause          = 0xF730,
		SysReq         = 0xF731,
		Break          = 0xF732,
		Reset          = 0xF733,
		Stop           = 0xF734,
		Menu           = 0xF735,
		User           = 0xF736,
		System         = 0xF737,
		Print          = 0xF738,
		ClearLine      = 0xF739,
		ClearDisplay   = 0xF73A,
		InsertLine     = 0xF73B,
		DeleteLine     = 0xF73C,
		InsertChar     = 0xF73D,
		DeleteChar     = 0xF73E,
		Prev           = 0xF73F,
		Next           = 0xF740,
		Select         = 0xF741,
		Execute        = 0xF742,
		Undo           = 0xF743,
		Redo           = 0xF744,
		Find           = 0xF745,
		Help           = 0xF746,
		ModeSwitch     = 0xF747
	}

#if !XAMCORE_4_0
	[Native]
	public enum NSEventSubtype : nuint_compat_int {
#else
	public enum NSEventSubtype : short {
#endif
		WindowExposed = 0,
		ApplicationActivated = 1,
		ApplicationDeactivated = 2,
		WindowMoved = 4,
		ScreenChanged = 8,
		AWT = 16
	}

#if !XAMCORE_4_0
	[Native]
	public enum NSSystemDefinedEvents : nuint_compat_int {
#else
	public enum NSSystemDefinedEvents : short {
#endif
		NSPowerOffEventType = 1
	}

#if !XAMCORE_4_0
	[Native]
	public enum NSEventMouseSubtype : nuint_compat_int {
#else
	public enum NSEventMouseSubtype : short {
#endif
		Mouse, 
#if !XAMCORE_4_0
		TablePoint, 
#else
		TabletPoint, 
#endif
		TabletProximity, Touch
	}
	
#endregion

#region NSView
	[Flags]
	[Native]
	public enum NSViewResizingMask : nuint_compat_int {
		NotSizable		=  0,
		MinXMargin		=  1,
		WidthSizable		=  2,
		MaxXMargin		=  4,
		MinYMargin		=  8,
		HeightSizable		= 16,
		MaxYMargin		= 32
	}
	
	[Native]
	public enum NSBorderType : nuint_compat_int {
		NoBorder, LineBorder, BezelBorder, GrooveBorder
	}

	[Native]
	public enum NSTextFieldBezelStyle : nuint_compat_int {
		Square, Rounded
	}
	
	[Native]
	public enum NSViewLayerContentsRedrawPolicy : nint {
		Never, OnSetNeedsDisplay, DuringViewResize, BeforeViewResize
	}

	[Native]
	public enum NSViewLayerContentsPlacement : nint {
		ScaleAxesIndependently,
		ScaleProportionallyToFit,
		ScaleProportionallyToFill,
		Center,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		BottomLeft,
		Left,
		TopLeft,
	}

#endregion
	
#region NSWindow
	[Flags]
#if !XAMCORE_4_0
	[Native]
	public enum NSWindowStyle : nuint_compat_int {
#else
	public enum NSWindowStyle : int {
#endif
		Borderless	       					= 0 << 0,
		Titled		       					= 1 << 0,
		Closable	       					= 1 << 1,
		Miniaturizable	      				= 1 << 2,
		Resizable	       					= 1 << 3,
		Utility		       					= 1 << 4,
		DocModal	       					= 1 << 6,
		NonactivatingPanel     				= 1 << 7,
		TexturedBackground     				= 1 << 8,
		Unscaled	       					= 1 << 11,
		UnifiedTitleAndToolbar 				= 1 << 12,
		Hud		       						= 1 << 13,
		FullScreenWindow       				= 1 << 14,
		[Mac (10,10)] FullSizeContentView   = 1 << 15 
	}

	[Native]
	public enum NSWindowSharingType : nuint_compat_int {
		None, ReadOnly, ReadWrite
	}

	[Native]
	public enum NSWindowBackingLocation : nuint_compat_int {
		Default, VideoMemory, MainMemory,
	}

	[Flags]
	[Native]
	public enum NSWindowCollectionBehavior : nuint_compat_int {
		Default = 0,
		CanJoinAllSpaces = 1 << 0,
		MoveToActiveSpace = 1 << 1,
		Managed = 1 << 2,
		Transient = 1 << 3,
		Stationary = 1 << 4,
		ParticipatesInCycle = 1 << 5,
		IgnoresCycle = 1 << 6,
		FullScreenPrimary = 1 << 7,
		FullScreenAuxiliary = 1 << 8,
		FullScreenNone = 1 << 9,
		[Mac (10, 11)] FullScreenAllowsTiling = 1 << 11,
		[Mac (10, 11)] FullScreenDisallowsTiling = 1 << 12
	}

	[Flags]
#if !XAMCORE_4_0
	[Native]
	public enum NSWindowNumberListOptions : nuint_compat_int {
#else
	public enum NSWindowNumberListOptions : int {
#endif
		AllApplication = 1 << 0,
		AllSpaces = 1 << 4
	}

	[Native]
	public enum NSSelectionDirection : nuint_compat_int {
		Direct = 0,
		Next,
		Previous
	}

	[Native]
	public enum NSWindowButton : nuint_compat_int {
		CloseButton, MiniaturizeButton, ZoomButton, ToolbarButton, DocumentIconButton, DocumentVersionsButton = 6, 
		[Availability (Deprecated = Platform.Mac_10_12, Message = "The standard window button for FullScreenButton is always null; use ZoomButton instead")]
		FullScreenButton
	}

	[Flags]
	[Native]
	public enum NSTouchPhase : nuint_compat_int {
		Began           = 1 << 0,
		Moved           = 1 << 1,
		Stationary      = 1 << 2,
		Ended           = 1 << 3,
		Cancelled       = 1 << 4,
		
		Touching        = Began | Moved | Stationary,
		Any             = unchecked ((nuint_compat_int)UInt64.MaxValue)
	}
#endregion
#region NSAnimation
	
	[Native]
	public enum NSAnimationCurve : nuint_compat_int {
		EaseInOut,
		EaseIn,
		EaseOut,
		Linear
	};
	
	[Native]
	public enum NSAnimationBlockingMode : nuint_compat_int {
		Blocking,
		Nonblocking,
		NonblockingThreaded
	};
#endregion

#region NSBox
	
	[Native]
	public enum NSTitlePosition : nuint_compat_int {
		NoTitle,
		AboveTop,
		AtTop,
		BelowTop,
		AboveBottom,
		AtBottom,
		BelowBottom
	};

	[Native]
	public enum NSBoxType : nuint_compat_int {
		NSBoxPrimary,
		NSBoxSecondary,
		NSBoxSeparator,
		NSBoxOldStyle,
		NSBoxCustom
	};
#endregion

#region NSButtonCell
	[Native]
	public enum NSButtonType : nuint_compat_int {
		MomentaryLightButton,
		PushOnPushOff,
		Toggle,
		Switch,
		Radio,
		MomentaryChange,
		OnOff,
		MomentaryPushIn,
		Accelerator, // 10.10.3
		MultiLevelAccelerator // 10.10.3
	}
	
	[Native]
	public enum NSBezelStyle : nuint_compat_int {
		Rounded = 1,
		RegularSquare,
		ThickSquare,
		ThickerSquare,
		Disclosure,
		ShadowlessSquare,
		Circular,
		TexturedSquare,
		HelpButton,
		SmallSquare,
		TexturedRounded,
		RoundRect,
		Recessed,
		RoundedDisclosure,
		Inline
	}

	[Native]
	[Availability (Deprecated = Platform.Mac_10_12, Message = "The GradientType property is unused, and setting it has no effect.")]
	public enum NSGradientType : nuint_compat_int {
		None,
		ConcaveWeak,
		ConcaveStrong,
		ConvexWeak,
		ConvexStrong
	}
	
#endregion

#region NSGraphics
	// NSGraphics.h:typedef int NSWindowDepth;
	public enum NSWindowDepth : int {
		TwentyfourBitRgb = 0x208,
		SixtyfourBitRgb = 0x210,
		OneHundredTwentyEightBitRgb = 0x220	
	}

	[Native]
	public enum NSCompositingOperation : nuint_compat_int {
		Clear,
		Copy,
		SourceOver,
		SourceIn,
		SourceOut,
		SourceAtop,
		DestinationOver,
		DestinationIn,
		DestinationOut,
		DestinationAtop,
		Xor,
		PlusDarker,
		Highlight,
		PlusLighter,

		[Mac (10, 10)]
		Multiply,
		[Mac (10, 10)]
		Screen,
		[Mac (10, 10)]
		Overlay,
		[Mac (10, 10)]
		Darken,
		[Mac (10, 10)]
		Lighten,
		[Mac (10, 10)]
		ColorDodge,
		[Mac (10, 10)]
		ColorBurn,
		[Mac (10, 10)]
		SoftLight,
		[Mac (10, 10)]
		HardLight,
		[Mac (10, 10)]
		Difference,
		[Mac (10, 10)]
		Exclusion,
		[Mac (10, 10)]
		Hue,
		[Mac (10, 10)]
		Saturation,
		[Mac (10, 10)]
		Color,
		[Mac (10, 10)]
		Luminosity
	}

	[Native]
	public enum NSAnimationEffect : nuint_compat_int {
		DissapearingItemDefault = 0,
		EffectPoof = 10
	}
#endregion
	
#region NSMatrix
	[Native]
	public enum NSMatrixMode : nuint_compat_int {
		Radio, Highlight, List, Track
	}
#endregion

#region NSBrowser
	[Native]
	public enum NSBrowserColumnResizingType : nuint_compat_int {
		None, Auto, User
	}

	[Native]
	public enum NSBrowserDropOperation : nuint_compat_int {
		On, Above
	}
#endregion

#region NSColorPanel
	[Native]
	public enum NSColorPanelMode : nint {
		None = -1,
		Gray = 0,
		RGB,
		CMYK,
		HSB,
		CustomPalette,
		ColorList,
		Wheel,
		Crayon
	};


	[Flags]
	[Native]
	public enum NSColorPanelFlags : nuint_compat_int {
		Gray			= 0x00000001,
		RGB				= 0x00000002,
		CMYK			= 0x00000004,
		HSB				= 0x00000008,
		CustomPalette	= 0x00000010,
		ColorList		= 0x00000020,
		Wheel			= 0x00000040,
		Crayon			= 0x00000080,
		All				= 0x0000ffff
	}


#endregion
#region NSDocument

	[Native]
	public enum NSDocumentChangeType : nuint_compat_int  {
		Done, Undone, Cleared, ReadOtherContents, Autosaved, Redone,
		Discardable = 256 /* New in Lion */
	}

	[Native]
	public enum NSSaveOperationType : nuint_compat_int  {
		Save, SaveAs, SaveTo,
		Autosave = 3,	/* Deprecated name in Lion */
		Elsewhere = 3,	/* New Lion name */
		InPlace = 4,	/* New in Lion */
		AutoSaveAs = 5	/* New in Mountain Lion */
	}

#endregion

#region NSBezelPath
	
	[Native]
	public enum NSLineCapStyle : nuint_compat_int {
		Butt, Round, Square
	}
	
	[Native]
	public enum NSLineJoinStyle : nuint_compat_int {
		Miter, Round, Bevel
	}
	
	[Native]
	public enum NSWindingRule : nuint_compat_int {
		NonZero, EvenOdd
	}
	
	[Native]
	public enum NSBezierPathElement : nuint_compat_int {
		MoveTo, LineTo, CurveTo, ClosePath
	}
#endregion

#region NSRulerView
	[Native]
	public enum NSRulerOrientation : nuint_compat_int {
		Horizontal, Vertical
	}
#endregion

#region NSGestureRecognizer
	[Mac (10,10)]
	[Native]
	public enum NSGestureRecognizerState : nint {
		Possible,
		Began,
		Changed,
		Ended,
		Cancelled,
		Failed,
		Recognized = NSGestureRecognizerState.Ended
	}
#endregion

#region NSStackLayout
	[Native]
	public enum NSUserInterfaceLayoutOrientation : nint {
		Horizontal = 0,
		Vertical = 1
	}

	// NSStackView.h:typedef float NSStackViewVisibilityPriority
	public enum NSStackViewVisibilityPriority : int {
		Musthold = 1000,
		DetachOnlyIfNecessary = 900,
		NotVisible = 0
	}

	[Native]
	public enum NSStackViewGravity : nint {
		Top = 1,
		Leading = 1,
		Center = 2,
		Bottom = 3,
		Trailing = 3
	}
#endregion

	[Mac (10,11)]
	[Native]
	public enum NSStackViewDistribution : nint
	{
		GravityAreas = -1,
		Fill = 0,
		FillEqually,
		FillProportionally,
		EqualSpacing,
		EqualCentering
	}

	[Flags]
	[Native]
	public enum NSDragOperation : nuint {
		None,
		Copy = 1,
		Link = 2,
		Generic = 4,
		Private = 8,
		AllObsolete = 15,
		Move = 16,
		Delete = 32,
		All = UInt32.MaxValue
	}

	[Native]
	public enum NSTextAlignment : nuint_compat_int {
		Left = 0,
		Right = 1, 
		Center = 2,
		Justified = 3, 
		Natural = 4
	}

	[Flags]
	[Native]
	public enum NSWritingDirection : nint {
		Natural = -1, LeftToRight, RightToLeft,
		Embedding = 0,
		Override = 2,
	}

#if !XAMCORE_4_0
	[Native]
	public enum NSTextMovement : nint {
#else
	public enum NSTextMovement : int {
#endif
		Other = 0,
		Return = 0x10,
		Tab = 0x11,
		Backtab = 0x12,
		Left = 0x13,
		Right = 0x14,
		Up = 0x15,
		Down = 0x16,
		Cancel = 0x17
	}
	
	[Flags]
	[Native]
	public enum NSMenuProperty : nuint_compat_int {
		Title = 1 << 0,
		AttributedTitle = 1 << 1,
		KeyEquivalent = 1 << 2,
		Image = 1 << 3,
		Enabled = 1 << 4,
		AccessibilityDescription = 1 << 5
	}

	[Native]
	public enum NSFontRenderingMode : nuint_compat_int {
		Default, Antialiased, IntegerAdvancements, AntialiasedIntegerAdvancements
	}

	[Flags]
	[Native]
	public enum NSPasteboardReadingOptions : nuint_compat_int {
		AsData = 0,
		AsString = 1,
		AsPropertyList = 2,
		AsKeyedArchive = 4
	}

	[Native]
	public enum NSUnderlineStyle : nint {
		None                = 0x00,
		Single              = 0x01,
		Thick               = 0x02,
		Double              = 0x09,
		PatternSolid 		= 0x0000,
		PatternDot 			= 0x0100,
		PatternDash 		= 0x0200,
		PatternDashDot 		= 0x0300,
		PatternDashDotDot 	= 0x0400,
		ByWord 				= 0x8000
	}

	// Convenience enum, untyped in ObjC
	public enum NSUnderlinePattern : int {
		Solid             = 0x0000,
		Dot               = 0x0100,
		Dash              = 0x0200,
		DashDot           = 0x0300,
		DashDotDot        = 0x0400
	}

	[Native]
	public enum NSSelectionAffinity : nuint_compat_int {
		Upstream, Downstream
	}

	[Native]
	public enum NSSelectionGranularity : nuint_compat_int {
		Character, Word, Paragraph
	}

#region NSTrackingArea
	[Flags]
	[Native]
	public enum NSTrackingAreaOptions : nuint_compat_int {
		MouseEnteredAndExited     = 0x01,
		MouseMoved                = 0x02,
		CursorUpdate 		  = 0x04,
		ActiveWhenFirstResponder  = 0x10,
		ActiveInKeyWindow         = 0x20,
		ActiveInActiveApp 	  = 0x40,
		ActiveAlways 		  = 0x80,
		AssumeInside              = 0x100,
		InVisibleRect             = 0x200,
		EnabledDuringMouseDrag    = 0x400 	
	}
#endregion

	[Native]
	public enum NSLineSweepDirection : nuint_compat_int {
		NSLineSweepLeft,
		NSLineSweepRight,
		NSLineSweepDown,
		NSLineSweepUp
	}

	[Native]
	public enum NSLineMovementDirection : nuint_compat_int {
		None, Left, Right, Down, Up
	}

	[Native]
	public enum  NSTiffCompression : nuint_compat_int {
		None = 1,
		CcittFax3 = 3,
		CcittFax4 = 4,
		Lzw = 5,

		[Availability (Deprecated = Platform.Mac_10_7)]
		Jpeg		= 6,
		Next		= 32766,
		PackBits	= 32773,

		[Availability (Deprecated = Platform.Mac_10_7)]
		OldJpeg		= 32865
	}

	[Native]
	public enum NSBitmapImageFileType : nuint_compat_int {
		Tiff,
		Bmp,
		Gif,
		Jpeg,
		Png,
		Jpeg2000
	}

	[Native]
	public enum NSImageRepLoadStatus : nint {
		UnknownType     = -1,
		ReadingHeader   = -2,
		WillNeedAllData = -3,
		InvalidData     = -4,
		UnexpectedEOF   = -5,
		Completed       = -6 
	}

	[Flags]
	[Native]
	public enum NSBitmapFormat : nuint_compat_int {
		AlphaFirst = 1,
		AlphaNonpremultiplied = 2,
		FloatingPointSamples = 4,

		LittleEndian16Bit = 1 << 8,
		LittleEndian32Bit = 1 << 9,
		BigEndian16Bit = 1 << 10,
		BigEndian32Bit = 1 << 11
	}

	[Native]
	public enum NSPrintingOrientation : nuint_compat_int {
		Portrait, Landscape
	}
	
	[Native]
	public enum NSPrintingPaginationMode : nuint_compat_int {
		Auto, Fit, Clip
	}

	[Flags]
#if !XAMCORE_4_0
	[Native]
	public enum NSGlyphStorageOptions : nuint_compat_int {
#else
	public enum NSGlyphStorageOptions : int
#endif
		ShowControlGlyphs = 1,
		ShowInvisibleGlyphs = 2,
		WantsBidiLevels = 4
	}

#if !XAMCORE_4_0
	[Availability (Deprecated = Platform.Mac_10_11, Message = "Use NSTextStorageEditActions instead")]
	[Flags]
	[Native]
	public enum NSTextStorageEditedFlags : nuint_compat_int {
		EditedAttributed = 1,
		EditedCharacters = 2
	}
#endif

	[Mac (10,11)]
	[Native]
	[Flags]
	public enum NSTextStorageEditActions : nuint
	{
		Attributes = (1 << 0),
		Characters = (1 << 1)
	}

	[Native]
	public enum NSPrinterTableStatus : nuint_compat_int {
		Ok, NotFound, Error
	}

	[Native]
	public enum NSScrollArrowPosition : nuint_compat_int {
		MaxEnd, MinEnd, DefaultSetting, None
	}

	[Native]
	public enum NSUsableScrollerParts : nuint_compat_int {
		NoScroller, OnlyArrows, All
	}

	[Native]
	public enum NSScrollerPart : nuint_compat_int {
		None, DecrementPage, Knob, IncrementPage, DecrementLine, IncrementLine, KnobSlot
	}

	[Native]
	public enum NSScrollerArrow : nuint_compat_int {
		IncrementArrow, DecrementArrow
	}

	[Native]
	public enum NSPrintingPageOrder : nint {
		Descending = -1,
		Special,
		Ascending,
		Unknown
	}

	[Flags]
	[Native]
	public enum NSPrintPanelOptions : nint {
		ShowsCopies = 1,
		ShowsPageRange = 2,
		ShowsPaperSize = 4,
		ShowsOrientation = 8,
		ShowsScaling = 16,
		ShowsPrintSelection = 32,
		ShowsPageSetupAccessory = 256,
		ShowsPreview = 131072
	}

	[Native]
	public enum NSTextBlockValueType : nuint_compat_int {
		Absolute, Percentage
	}

	[Native]
	public enum NSTextBlockDimension : nuint_compat_int {
		Width, MinimumWidth, MaximumWidth, Height, MinimumHeight, MaximumHeight
	}
	
	[Native]
	public enum NSTextBlockLayer : nint {
		Padding = -1, Border, Margin
	}

	[Native]
	public enum NSTextBlockVerticalAlignment : nuint_compat_int {
		Top, Middle, Bottom, Baseline
	}

	[Native]
	public enum NSTextTableLayoutAlgorithm : nuint_compat_int {
		Automatic, Fixed
	}

	[Flags]
	[Native]
	public enum NSTextListOptions : nuint_compat_int {
		PrependEnclosingMarker = 1
	}

	[Flags]
	public enum NSFontSymbolicTraits : int { // uint32_t NSFontSymbolicTraits
		ItalicTrait = (1 << 0),
		BoldTrait = (1 << 1),
		ExpandedTrait = (1 << 5),
		CondensedTrait = (1 << 6),
		MonoSpaceTrait = (1 << 10),
		VerticalTrait = (1 << 11), 
		UIOptimizedTrait = (1 << 12),
		
		UnknownClass = 0 << 28,
		OldStyleSerifsClass = 1 << 28,
		TransitionalSerifsClass = 2 << 28,
		ModernSerifsClass = 3 << 28,
		ClarendonSerifsClass = 4 << 28,
		SlabSerifsClass = 5 << 28,
		FreeformSerifsClass = 7 << 28,
		SansSerifClass = 8 << 28,
		OrnamentalsClass = 9 << 28,
		ScriptsClass = 10 << 28,
		SymbolicClass = 12 << 28,

		FamilyClassMask = (int) -268435456,
	}

	[Flags]
	[Native]
	public enum NSFontTraitMask : nuint_compat_int {
		Italic = 1,
		Bold = 2,
		Unbold = 4,
		NonStandardCharacterSet = 8,
		Narrow = 0x10,
		Expanded = 0x20,
		Condensed = 0x40,
		SmallCaps = 0x80,
		Poster = 0x100,
		Compressed = 0x200,
		FixedPitch = 0x400,
		Unitalic = 0x1000000
	}
	
	[Flags]
	[Native]
	public enum NSPasteboardWritingOptions : nuint_compat_int	 {
		WritingPromised = 1 << 9
	}


	[Native]
	public enum NSToolbarDisplayMode : nuint_compat_int {
		Default, IconAndLabel, Icon, Label
	}

	[Native]
	public enum NSToolbarSizeMode : nuint_compat_int {
		Default, Regular, Small
	}

	[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_10, Message = "Use NSAlertButtonReturn instead")]
#if !XAMCORE_4_0
	[Native]
	public enum NSAlertType : nint {
#else
	public enum NSAlertType : int {
#endif
		ErrorReturn = -2,
		OtherReturn,
		AlternateReturn,
		DefaultReturn
	}

#if !XAMCORE_4_0
	[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_10, Message = "Use NSModalResponse instead")]
	[Native]
	public enum NSPanelButtonType : nint {
		Cancel, Ok
	}
#endif

	[Native]
	public enum NSTableViewColumnAutoresizingStyle : nuint_compat_int {
		None = 0,
		Uniform,
		Sequential,
		ReverseSequential,
		LastColumnOnly,
		FirstColumnOnly
	}

	[Native]
	public enum NSTableViewSelectionHighlightStyle : nint {
		None = -1,
		Regular = 0,
		SourceList = 1
	}

	[Native]
	public enum NSTableViewDraggingDestinationFeedbackStyle : nint {
		None = -1,
		Regular = 0,
		SourceList = 1
	}

	[Native]
	public enum NSTableViewDropOperation : nuint_compat_int {
		On,
		Above
	}

	[Flags]
	[Native]
	public enum NSTableColumnResizing : nint {
		None = -1,
		Autoresizing = ( 1 << 0 ),
		UserResizingMask = ( 1 << 1 )
	} 

	[Flags]
	[Native]
	public enum NSTableViewGridStyle : nuint_compat_int {
		None = 0,
		SolidVerticalLine   = 1 << 0,
		SolidHorizontalLine = 1 << 1,
		DashedHorizontalGridLine = 1 << 3
	}

	[Flags]
	[Native]
	public enum NSGradientDrawingOptions : nuint_compat_int {
		None = 0,
		BeforeStartingLocation =   (1 << 0),
		AfterEndingLocation =    (1 << 1)
	}
	
	[Native]
	public enum NSImageAlignment : nuint_compat_int {
		Center = 0,
		Top,
		TopLeft,
		TopRight,
		Left,
		Bottom,
		BottomLeft,
		BottomRight,
		Right
	}
	
	[Native]
	public enum NSImageFrameStyle : nuint_compat_int {
		None = 0,
		Photo,
		GrayBezel,
		Groove,
		Button
	}
	
	[Native]
	public enum NSSpeechBoundary : nuint_compat_int {
		Immediate =  0,
		hWord,
		Sentence
	}

	[Native]
	public enum NSSplitViewDividerStyle : nint {
		Thick = 1,
		Thin = 2,
		PaneSplitter = 3
	}

	[Mac (10,11)]
	[Native]
	public enum NSSplitViewItemBehavior : nint
	{
		Default,
		Sidebar,
		ContentList
	}
	
	[Native]
	public enum NSImageScaling : nuint_compat_int {
		ProportionallyDown = 0,
		AxesIndependently,
		None,
		ProportionallyUpOrDown
	}
	
	[Native]
	public enum NSSegmentStyle : nint {
		Automatic = 0,
		Rounded = 1,
		TexturedRounded = 2,
		RoundRect = 3,
		TexturedSquare = 4,
		Capsule = 5,
		SmallSquare = 6,
		[Mac (10,10)] Separated = 8
	}
	
	[Native]
	public enum NSSegmentSwitchTracking : nuint_compat_int {
		SelectOne = 0,
		SelectAny = 1,
		Momentary = 2,
		MomentaryAccelerator // 10.10.3
	}
	
	[Native]
	public enum NSTickMarkPosition : nuint_compat_int {
		Below,
		Above,
		Left,
		Right,
		Leading = Left,
		Trailing = Right
	}
	
	[Native]
	public enum NSSliderType : nuint_compat_int {
		Linear   = 0,
		Circular = 1
	}
	
	[Native]
	public enum NSTokenStyle : nuint_compat_int {
		Default,
		PlainText,
		Rounded
	}

	[Flags]
	[Native]
	public enum NSWorkspaceLaunchOptions : nuint_compat_int {
		Print = 2,
		InhibitingBackgroundOnly = 0x80,
		WithoutAddingToRecents = 0x100,
		WithoutActivation = 0x200,
		Async = 0x10000,
		AllowingClassicStartup = 0x20000,
		PreferringClassic = 0x40000,
		NewInstance = 0x80000,
		Hide = 0x100000,
		HideOthers = 0x200000,
		Default = Async | AllowingClassicStartup
	}

	[Flags]
	[Native]
	public enum NSWorkspaceIconCreationOptions : nuint_compat_int {
		NSExcludeQuickDrawElements   = 1 << 1,
		NSExclude10_4Elements       = 1 << 2
	}

	[Native]
	public enum NSPathStyle : nint {
#if XAMCORE_2_0
		Standard,
		[Availability (Introduced = Platform.Mac_10_5, Deprecated = Platform.Mac_10_7)]
		NavigationBar,
		PopUp
#else
		NSPathStyleStandard,
		[Availability (Introduced = Platform.Mac_10_5, Deprecated = Platform.Mac_10_7)]
		NSPathStyleNavigationBar,
		NSPathStylePopUp
#endif
	}

	[Native]
	public enum NSTabViewType : nuint_compat_int {
		NSTopTabsBezelBorder,
		NSLeftTabsBezelBorder,
		NSBottomTabsBezelBorder,
		NSRightTabsBezelBorder,
		NSNoTabsBezelBorder,
		NSNoTabsLineBorder,
		NSNoTabsNoBorder,
	}

	[Native]
	public enum NSTabState : nuint_compat_int {
		Selected, Background, Pressed
	}

	[Native]
	public enum NSTabViewControllerTabStyle : nint {
		SegmentedControlOnTop = 0,
		SegmentedControlOnBottom,
		Toolbar,
		Unspecified = -1
	}

	[Native]
	public enum NSLevelIndicatorStyle : nuint_compat_int {
		Relevancy, ContinuousCapacity, DiscreteCapacity, RatingLevel
	}

	[Flags]
	[Native]
	public enum NSFontCollectionOptions : nint {
		ApplicationOnlyMask = 1
	}

	[Native]
	public enum NSCollectionViewDropOperation : nint {
		On = 0, Before = 1
	}

	[Mac (10,11)]
	[Native]
	public enum NSCollectionViewItemHighlightState : nint
	{
		None = 0,
		ForSelection = 1,
		ForDeselection = 2,
		AsDropTarget = 3
	}

	[Mac (10,11)] // Not marked 10.11 in the headers, but doesn't exist in the 10.10 headers
	[Native]
	[Flags]
	public enum NSCollectionViewScrollPosition : nuint
	{
		None = 0,
		Top = 1 << 0,
		CenteredVertically = 1 << 1,
		Bottom = 1 << 2,
		NearestHorizontalEdge = 1 << 9,
		Left = 1 << 3,
		CenteredHorizontally = 1 << 4,
		Right = 1 << 5,
		LeadingEdge = 1 << 6,
		TrailingEdge = 1 << 7,
		NearestVerticalEdge = 1 << 8
	}

	[Mac (10,11)]
	[Native]
	public enum NSCollectionElementCategory : nint
	{
		Item,
		SupplementaryView,
		DecorationView,
		InterItemGap
	}

	[Mac (10,11)]
	[Native]
	public enum NSCollectionUpdateAction : nint
	{
		Insert,
		Delete,
		Reload,
		Move,
		None
	}

	[Mac (10,11)]
	[Native]
	public enum NSCollectionViewScrollDirection : nint
	{
		Vertical,
		Horizontal
	}

	[Native]
	public enum NSDatePickerStyle : nuint_compat_int {
		TextFieldAndStepper,
		ClockAndCalendar,
		TextField
	}

	[Native]
	public enum NSDatePickerMode : nuint_compat_int {
		Single, Range
	}

	[Flags]
	[Native]
	public enum NSDatePickerElementFlags : nuint_compat_int {
		HourMinute = 0xc,
		HourMinuteSecond = 0xe,
		TimeZone = 0x10,

		YearMonthDate = 0xc0,
		YearMonthDateDay = 0xe0,
		Era = 0x100
	}

	[Native]
	public enum NSOpenGLContextParameter : nuint_compat_int {
		[Availability (Deprecated = Platform.Mac_10_7)]
		SwapRectangle = 200,
		[Availability (Deprecated = Platform.Mac_10_7)]
		SwapRectangleEnable = 201,
		[Availability (Deprecated = Platform.Mac_10_7)]
		RasterizationEnable = 221,
		[Availability (Deprecated = Platform.Mac_10_7)]
		StateValidation = 301,
		[Availability (Deprecated = Platform.Mac_10_7)]
		SurfaceSurfaceVolatile = 306,

		SwapInterval = 222,
		SurfaceOrder = 235,
		SurfaceOpacity = 236,

		[Lion] SurfaceBackingSize = 304,
		[Lion] ReclaimResources = 308,
		[Lion] CurrentRendererID = 309,
		[Lion] GpuVertexProcessing = 310,
		[Lion] GpuFragmentProcessing = 311,
		[Lion] HasDrawable = 314,
		[Lion] MpsSwapsInFlight = 315
	}
	
	public enum NSSurfaceOrder {
		AboveWindow = 1,
		BelowWindow = -1
	}

	public enum NSOpenGLPixelFormatAttribute : uint_compat_int { // uint32_t NSOpenGLPixelFormatAttribute
		AllRenderers       =   1,
		DoubleBuffer       =   5,
		[Lion] TrippleBuffer = 3,
		Stereo             =   6,
		AuxBuffers         =   7,
		ColorSize          =   8,
		AlphaSize          =  11,
		DepthSize          =  12,
		StencilSize        =  13,
		AccumSize          =  14,
		MinimumPolicy      =  51,
		MaximumPolicy      =  52,
		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_7)]
		OffScreen          =  53,
		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_6)]
		FullScreen         =  54,
		SampleBuffers      =  55,
		Samples            =  56,
		AuxDepthStencil    =  57,
		ColorFloat         =  58,
		Multisample        =  59,
		Supersample        =  60,
		SampleAlpha        =  61,
		RendererID         =  70,
		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_9)]
		SingleRenderer     =  71,
		NoRecovery         =  72,
		Accelerated        =  73,
		ClosestPolicy      =  74,
		BackingStore       =  76,
		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_9)]
		Window             =  80,
		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_9)]
		Compliant          =  83,
		ScreenMask         =  84,
		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_7)]
		PixelBuffer        =  90,
		[Availability (Introduced = Platform.Mac_10_3, Deprecated = Platform.Mac_10_7)]
		RemotePixelBuffer  =  91,
		AllowOfflineRenderers = 96,
		AcceleratedCompute =  97,

		// Specify the profile
		[Lion] OpenGLProfile = 99,
		VirtualScreenCount = 128,

		[Availability (Deprecated = Platform.Mac_10_5)]
		Robust  =  75,
		[Availability (Deprecated = Platform.Mac_10_5)]
		MPSafe  =  78,
		[Availability (Deprecated = Platform.Mac_10_5)]
		MultiScreen =  81
	}

#if XAMCORE_4_0
	[Native]
	public enum NSOpenGLProfile : nint {
#else
	public enum NSOpenGLProfile : int {
#endif
		VersionLegacy   = 0x1000, // Legacy
		Version3_2Core  = 0x3200,  // 3.2 or better
		Version4_1Core  = 0x4100
	}
	
#if !XAMCORE_4_0
	[Native]
	public enum NSAlertButtonReturn : nint {
#else
	public enum NSAlertButtonReturn : int {
#endif
		First = 1000,
		Second = 1001,
		Third = 1002,
	}

	public enum NSOpenGLGlobalOption : uint_compat_int {
		FormatCacheSize = 501,
		ClearFormatCache = 502,
		RetainRenderers = 503,
		[Lion] UseBuildCache = 506,
		[Availability (Deprecated = Platform.Mac_10_4)]
		ResetLibrary = 504
	}

	public enum NSGLTextureTarget : uint_compat_int {
		T2D = 0x0de1,
		CubeMap = 0x8513,
		RectangleExt = 0x84F5,
	}

	public enum NSGLFormat : uint_compat_int {
		RGB = 0x1907,
		RGBA = 0x1908,
		DepthComponent = 0x1902,
	}
	
	public enum NSGLTextureCubeMap : uint_compat_int {
		None = 0,
		PositiveX = 0x8515,
		PositiveY = 0x8517,
		PositiveZ = 0x8519,
		NegativeX = 0x8516,
		NegativeY = 0x8517,
		NegativeZ = 0x851A
	}

	public enum NSGLColorBuffer : uint_compat_int {
		Front = 0x0404,
		Back = 0x0405,
		Aux0 = 0x0409
	}

	[Native]
	public enum NSProgressIndicatorThickness : nuint_compat_int {
		Small = 10,
		Regular = 14,
		Aqua = 12,
		Large = 18
	}

	[Native]
	public enum NSProgressIndicatorStyle : nuint_compat_int {
		Bar, Spinning
	}

	[Native]
	public enum NSPopUpArrowPosition : nuint_compat_int {
		None,
		Center,
		Bottom
	}

	public static class NSFileTypeForHFSTypeCode {
		public static readonly string ComputerIcon = "root";
		public static readonly string DesktopIcon = "desk";
		public static readonly string FinderIcon = "FNDR";
	}
	
	// These constants specify the possible states of a drawer.
	[Native]
	public enum NSDrawerState : nuint_compat_int {
		Closed = 0,
		Opening = 1,
		Open = 2,
		Closing = 3
	}

	[Native]
	public enum NSWindowLevel : nint {
		Normal = 0,
		Dock = 20,
		Floating = 3,
		MainMenu = 24, 
		ModalPanel = 8,
		PopUpMenu = 101,
		ScreenSaver = 1000,
		Status = 25,
		Submenu = 3,
		TornOffMenu = 3
	}
	
	[Native]
	public enum NSRuleEditorRowType : nuint_compat_int {
		Simple = 0,
		Compound
	}
   
	[Native]
	public enum NSRuleEditorNestingMode : nuint_compat_int {
		Single,
		List,
		Compound,
		Simple
	}

	[Native]
	public enum NSGlyphInscription : nuint_compat_int {
		Base, Below, Above, Overstrike, OverBelow
	}

	[Native]
	public enum NSTypesetterBehavior : nint {
		Latest = -1,
		Original = 0,
		Specific_10_2_WithCompatibility = 1,
		Specific_10_2 = 2,
		Specific_10_3 = 3,
		Specific_10_4 = 4,
			
	}

	[Flags]
	[Native]
	public enum NSRemoteNotificationType : nuint_compat_int {
		None = 0,
		Badge = 1 << 0,
		Sound = 1 << 1,
		Alert = 1 << 2
	}
	
	[Native]
	public enum NSScrollViewFindBarPosition : nint {
		AboveHorizontalRuler = 0,
		AboveContent,
		BelowContent
	}
	
	[Native]
	public enum NSScrollerStyle : nint {
   		Legacy = 0,
		Overlay
	}
	
	[Native]
	public enum  NSScrollElasticity : nint {
		Automatic = 0,
   		None,
		Allowed
	}
	
	[Native]
	public enum  NSScrollerKnobStyle : nint {
		Default  = 0,
		Dark     = 1,
		Light    = 2
	}

	[Flags]
	[Native]
	public enum NSEventPhase : nuint_compat_int {
		None,
		Began = 1,
		Stationary = 2,
		Changed = 4,
		Ended = 8,
		Cancelled = 16,
		MayBegin = 32
	}

	[Flags]
	[Native]
	public enum NSEventSwipeTrackingOptions : nuint_compat_int {
		LockDirection = 1,
		ClampGestureAmount = 2
	}

	[Native]
	public enum NSEventGestureAxis : nint {
		None, Horizontal, Vertical
	}

	[Native]
	public enum NSLayoutRelation : nint {
		LessThanOrEqual = -1,
		Equal = 0,
		GreaterThanOrEqual = 1
	}

	[Native]
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
		[Mac (10,11)] LastBaseline = Baseline,
		[Mac (10,11)] FirstBaseline,
	}

	[Flags]
	[Native]
	public enum NSLayoutFormatOptions : nuint_compat_int {
		None = 0,

		AlignAllLeft = (1 << (int)NSLayoutAttribute.Left),
		AlignAllRight = (1 << (int)NSLayoutAttribute.Right),
		AlignAllTop = (1 << (int)NSLayoutAttribute.Top),
		AlignAllBottom = (1 << (int)NSLayoutAttribute.Bottom),
		AlignAllLeading = (1 << (int)NSLayoutAttribute.Leading),
		AlignAllTrailing = (1 << (int)NSLayoutAttribute.Trailing),
		AlignAllCenterX = (1 << (int)NSLayoutAttribute.CenterX),
		AlignAllCenterY = (1 << (int)NSLayoutAttribute.CenterY),
		AlignAllBaseline = (1 << (int)NSLayoutAttribute.Baseline),
		[Mac (10,11)] AlignAllLastBaseline = (int)AlignAllBaseline,
		[Mac (10,11)] AlignAllFirstBaseline = (1 << (int)NSLayoutAttribute.FirstBaseline),
		AlignmentMask = 0xFFFF,
		
		/* choose only one of these three
		 */
		DirectionLeadingToTrailing = 0 << 16, // default
		DirectionLeftToRight = 1 << 16,
		DirectionRightToLeft = 2 << 16,
		
		DirectionMask = 0x3 << 16,
	}

	[Native]
	public enum NSLayoutConstraintOrientation : nint {
		Horizontal, Vertical
	}

	public enum NSLayoutPriority : int /*float*/ {
		Required = 1000,
		DefaultHigh = 750,
		DragThatCanResizeWindow = 510,
		WindowSizeStayPut = 500,
		DragThatCannotResizeWindow = 490,
		DefaultLow = 250,
		FittingSizeCompression = 50,
	}

	[Native]
	public enum NSPopoverAppearance : nint {
		Minimal, HUD
	}

	[Native]
	public enum NSPopoverBehavior : nint {
		ApplicationDefined, Transient, Semitransient
	}

	[Native]
	public enum NSTableViewRowSizeStyle : nint {
		Default = -1,
		Custom = 0,
		Small, Medium, Large
	}

	[Mac (10,11)]
	[Native]
	public enum NSTableRowActionEdge : nint
	{
		Leading,
		Trailing
	}

	[Mac (10,11)]
	[Native]
	public enum NSTableViewRowActionStyle : nint
	{
		Regular,
		Destructive
	}

	[Flags]
	[Native]
	public enum NSTableViewAnimation : nuint_compat_int {
		None, Fade = 1, Gap = 2,
		SlideUp = 0x10, SlideDown = 0x20, SlideLeft = 0x30, SlideRight = 0x40
	}

	[Flags]
	[Native]
	public enum NSDraggingItemEnumerationOptions : nuint_compat_int {
		Concurrent = 1 << 0,
		ClearNonenumeratedImages = 1 << 16
	}

	[Native]
	public enum NSDraggingFormation : nint {
		Default, None, Pile, List, Stack
	}

	[Native]
	public enum NSDraggingContext : nint {
		OutsideApplication, WithinApplication
	}

	[Native]
	public enum NSWindowAnimationBehavior : nint {
		Default = 0, None = 2, DocumentWindow, UtilityWindow, AlertPanel
	}

	[Lion]
	[Native]
	public enum NSTextFinderAction : nint {
		ShowFindInterface = 1,
		NextMatch = 2,
		PreviousMatch = 3,
		ReplaceAll = 4,
		Replace = 5,
		ReplaceAndFind = 6,
		SetSearchString = 7,
		ReplaceAllInSelection = 8,
		SelectAll = 9,
		SelectAllInSelection = 10,
		HideFindInterface = 11,
		ShowReplaceInterface = 12,
		HideReplaceInterface = 13
	}

	[Flags]
#if !XAMCORE_4_0
	[Native]
	public enum NSFontPanelMode : nuint_compat_int {
#else
	public enum NSFontPanelMode : int {
#endif
		FaceMask = 1 << 0,
		SizeMask = 1 << 1,
		CollectionMask = 1 << 2,
		UnderlineEffectMask = 1<<8,
		StrikethroughEffectMask = 1<<9,
		TextColorEffectMask = 1<< 10,
		DocumentColorEffectMask = 1<<11,
		ShadowEffectMask = 1<<12,
		AllEffectsMask = 0XFFF00,
		StandardMask = 0xFFFF,
		AllModesMask = unchecked ((nuint_compat_int)UInt32.MaxValue)
	}

	[Flags]
	[Native]
	public enum NSFontCollectionVisibility : nuint_compat_int {
		Process = 1 << 0,
		User = 1 << 1,
		Computer = 1 << 2,
	}

	[Native]
	public enum NSSharingContentScope : nint {
		Item,
		Partial,
		Full
	}

	// Convenience enum for string values in ObjC
	public enum NSSharingServiceName : int {
		PostOnFacebook,
		PostOnTwitter,
		PostOnSinaWeibo,
		ComposeEmail,
		ComposeMessage,
		SendViaAirDrop,
		AddToSafariReadingList,
		AddToIPhoto,
		AddToAperture,
		UseAsTwitterProfileImage,
		UseAsDesktopPicture,
		PostImageOnFlickr,
		PostVideoOnVimeo,
		PostVideoOnYouku,
		PostVideoOnTudou,
		CloudSharing
	}

	[Flags]
	[Native]
	public enum NSTypesetterControlCharacterAction : nuint_compat_int {
		ZeroAdvancement = 1 << 0,
		Whitespace = 1 << 1,
		HorizontalTab = 1 << 2,
		LineBreak = 1 << 3,
		ParagraphBreak = 1 << 4,
		ContainerBreak = 1 << 5,
	}

	[Native]
	public enum NSPageControllerTransitionStyle : nint {
		StackHistory,
		StackBook,
		HorizontalStrip
	}

	[Native]
	public enum NSWindowTitleVisibility : nint {
		Visible = 0,
		Hidden = 1,
		HiddenWhenActive = 2
	}

	[Flags]
	[Native]
	public enum NSViewControllerTransitionOptions : nuint {
		None = 0x0,
		Crossfade = 0x1,
		SlideUp = 0x10,
		SlideDown = 0x20,
		SlideLeft = 0x40,
		SlideRight = 0x80,
		SlideForward = 0x140,
		SlideBackward = 0x180,
		AllowUserInteraction = 0x1000
	}

	[Flags]
	[Native]
	public enum NSApplicationOcclusionState  : nuint {
		Visible = 1 << 1
	}

	[Flags]
	[Native]
	public enum NSWindowOcclusionState  : nuint {
		Visible = 1 << 1
	}

	
	
#region NSVisualEffectView
	[Native]
	public enum NSVisualEffectMaterial : nint {
		AppearanceBased,
		Light,
		Dark,
		Titlebar,
		Selection,
		[Mac (10,11)]
		Menu,
		[Mac (10,11)]
		Popover,
		[Mac (10,11)]
		Sidebar,
		[Mac (10,11)]
		MediumLight,
		[Mac (10,11)]
		UltraDark,
	}

	[Native]
	public enum NSVisualEffectBlendingMode : nint {
		BehindWindow,
		WithinWindow
	}

	[Native]
	public enum NSVisualEffectState : nint {
		FollowsWindowActiveState,
		Active,
		Inactive
	}
#endregion

	[Mac (10,10,3)]
	[Native]
	public enum NSPressureBehavior : nint
	{
		Unknown = -1,
		PrimaryDefault = 0,
		PrimaryClick = 1,
		PrimaryGeneric = 2,
		PrimaryAccelerator = 3,
		PrimaryDeepClick = 5,
		PrimaryDeepDrag = 6
	}

	[Mac (10,11)]
	[Native]
	public enum NSHapticFeedbackPattern : nint
	{
		Generic = 0,
		Alignment,
		LevelChange
	}

	[Mac (10,11)]
	[Native]
	public enum NSHapticFeedbackPerformanceTime : nuint
	{
		Default = 0,
		Now,
		DrawCompleted
	}

	[Mac (10,11)]
	[Native]
	public enum NSSpringLoadingHighlight : nint
	{
		None = 0,
		Standard,
		Emphasized
	}

	[Flags]
	[Mac (10,11)]
	[Native]
	public enum NSSpringLoadingOptions : nuint
	{
		Disabled = 0,
		Enabled = 1 << 0,
		ContinuousActivation = 1 << 1,
		NoHover = 1 << 3
	}

	[Mac (10,11)]
	[Native]
	public enum NSGlyphProperty : nint
	{
		Null = (1 << 0),
		ControlCharacter = (1 << 1),
		Elastic = (1 << 2),
		NonBaseCharacter = (1 << 3)
	}

	[Flags]
	[Mac (10,11)]
	[Native]
	public enum NSControlCharacterAction : nint
	{
		ZeroAdvancement = (1 << 0),
		Whitespace = (1 << 1),
		HorizontalTab = (1 << 2),
		LineBreak = (1 << 3),
		ParagraphBreak = (1 << 4),
		ContainerBreak = (1 << 5)
	}

	[Flags]
	[Mac (10,12)]
	[Native]
	public enum NSWindowListOptions : nint {
		OrderedFrontToBack = (1 << 0)
	}

	[Mac (10,12)]
	[Native]
	public enum NSStatusItemBehavior : nuint
	{
		RemovalAllowed = (1 << 1),
		TerminationOnRemoval = (1 << 2)
	}

	[Mac (10,12)]
	[Native]
	public enum NSWindowTabbingMode : nint
	{
		Automatic,
		Preferred,
		Disallowed
	}

	[Mac (10,12)]
	[Native]
	public enum NSWindowUserTabbingPreference : nint
	{
		Manual,
		Always,
		InFullScreen
	}


	[Mac (10, 12)]
	[Native]
	public enum NSGridCellPlacement : nint
	{
		Inherited = 0,
		None,
		Leading,
		Top = Leading,
		Trailing,
		Bottom = Trailing,
		Center,
		Fill
	}

	[Mac (10, 12)]
	[Native]
	public enum NSGridRowAlignment : nint
	{
		Inherited = 0,
		None,
		FirstBaseline,
		LastBaseline
	}

	[Mac (10, 12)]
	[Native]
	public enum NSImageLayoutDirection : nint
	{
		Unspecified = -1,
		LeftToRight = 2,
		RightToLeft = 3
	}

	[Mac (10, 12)]
	[Native][Flags]
	public enum NSCloudKitSharingServiceOptions : nuint
	{
		Standard = 0,
		AllowPublic = 1 << 0,
		AllowPrivate = 1 << 1,
		AllowReadOnly = 1 << 4,
		AllowReadWrite = 1 << 5
	}

	[Mac (10, 12)]
	[Native]
	public enum NSDisplayGamut : nint
	{
		Srgb = 1,
		P3,
	}

	[Mac (10, 12)]
	[Native]
	public enum NSTabPosition : nuint
	{
		None = 0,
		Top,
		Left,
		Bottom,
		Right,
	}

	[Mac (10, 12)]
	[Native]
	public enum NSTabViewBorderType : nuint
	{
		None = 0,
		Line,
		Bezel,
	}

	[Mac (10, 12)]
	[Native]
	public enum NSPasteboardContentsOptions : nuint
	{
		CurrentHostOnly = 1,
	}


	[Mac (10,12,2)]
	[Native]
	public enum NSTouchType : nint
	{
		Direct,
		Indirect
	}

	[Mac (10,12,2)]
	[Native]
	[Flags]
	public enum NSTouchTypeMask : nuint
	{
		Direct = (1 << (int)NSTouchType.Direct),
		Indirect = (1 << (int)NSTouchType.Indirect)
	}

	[Mac (10,12,2)]
	[Native]
	public enum NSScrubberMode : nint
	{
		Fixed = 0,
		Free
	}

	[Mac (10,12,2)]
	[Native]
	public enum NSScrubberAlignment : nint
	{
		None = 0,
		Leading,
		Trailing,
		Center
	}

}
