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
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;

namespace AppKit {

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSRunResponse : long {
		Stopped = -1000,
		Aborted = -1001,
		Continues = -1002
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSApplicationActivationOptions : ulong {
		Default = 0,
		ActivateAllWindows = 1,
		ActivateIgnoringOtherWindows = 2
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSApplicationActivationPolicy : long {
		Regular, Accessory, Prohibited
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSApplicationPresentationOptions : ulong {
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
		AutoHideToolbar            = (1 << 11),
#if NET
		[SupportedOSPlatform ("macos10.11.2")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,11,2)]
#endif
		DisableCursorLocationAssistance = (1 << 12),
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSApplicationDelegateReply : ulong {
		Success,
		Cancel,
		Failure
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSRequestUserAttentionType : ulong {
		CriticalRequest = 0,
		InformationalRequest = 10
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSApplicationTerminateReply : ulong {
		Cancel, Now, Later
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSApplicationPrintReply : ulong {
		Cancelled, Success, Failure, ReplyLater
	}

#if !XAMCORE_4_0
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSApplicationLayoutDirection : long {
		LeftToRight = 0,
		RightToLeft = 1
	}
#endif

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSImageInterpolation : ulong {
		Default, None, Low, Medium, High
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSComposite : ulong {
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
#if NET
		[UnsupportedOSPlatform ("macos10.10")]
#if MONOMAC
		[Obsolete ("Starting with macos10.10 use NSCompositeSourceOver instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use NSCompositeSourceOver instead.")]
#endif
		Highlight,
		PlusLighter,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Multiply,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Screen,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Overlay,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Darken,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Lighten,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
ColorDodge,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
ColorBurn,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
SoftLight,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
HardLight,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Difference,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Exclusion,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Hue,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Saturation,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Color,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Luminosity
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSBackingStore : ulong {
#if NET
		[UnsupportedOSPlatform ("macos10.13")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13 use 'Buffered' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'Buffered' instead.")]
#endif
		Retained, 
#if NET
		[UnsupportedOSPlatform ("macos10.13")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13 use 'Buffered' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'Buffered' instead.")]
#endif
		Nonretained, 
		Buffered,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSWindowOrderingMode : long {
		Below = -1, Out, Above,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSFocusRingPlacement : ulong {
		RingOnly, RingBelow, RingAbove,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSFocusRingType : ulong {
		Default, None, Exterior
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSColorRenderingIntent : long {
		Default,
		AbsoluteColorimetric,
		RelativeColorimetric,
		Perceptual,
		Saturation
		
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[MacCatalyst (13, 0)]
#endif
	[Native]
	public enum NSRectEdge : ulong {
		MinXEdge, MinYEdge, MaxXEdge, MaxYEdge
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSUserInterfaceLayoutDirection : long {
		LeftToRight, RightToLeft
	}

#region NSColorSpace
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSColorSpaceModel : long {
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
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	[Obsolete ("Use NSFileWrapperReadingOptions in Foundation instead.")]
	public enum NSFileWrapperReadingOptions : ulong {
		Immediate = 1, WithoutMapping = 2
	}
#endif
#endregion
	
#region NSParagraphStyle
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTextTabType : ulong {
		Left, Right, Center, Decimal
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSLineBreakMode : ulong {
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
#if NET
	[UnsupportedOSPlatform ("macos10.10")]
#if MONOMAC
	[Obsolete ("Starting with macos10.10 use formatters instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use formatters instead.")]
#endif
	[Native]
	public enum NSType : ulong {
	    Any			= 0,
	    Int			= 1,
	    PositiveInt		= 2,
	    Float		= 3,
	    PositiveFloat	= 4,
	    Double		= 6,
	    PositiveDouble	= 7
	}
#endif
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSCellType : ulong {
	    Null,
	    Text,
	    Image
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSCellAttribute : ulong {
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
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSCellImagePosition : ulong {
		NoImage,
		ImageOnly,
		ImageLeft,
		ImageRight,
		ImageBelow,
		ImageAbove,
		ImageOverlaps,
#if NET
		[SupportedOSPlatform ("macos10.12")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,12)]
#endif
		ImageLeading,
#if NET
		[SupportedOSPlatform ("macos10.12")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,12)]
#endif
		ImageTrailing,
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSImageScale : ulong {
		ProportionallyDown = 0,
		AxesIndependently,     
		None,                 
		ProportionallyUpOrDown
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSCellStateValue : long {
		Mixed = -1,
		Off,
		On
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSCellStyleMask : ulong {
		NoCell = 0,
		ContentsCell = 1 << 0,
		PushInCell = 1 << 1, 
		ChangeGrayCell = 1 << 2,
		ChangeBackgroundCell = 1 << 3
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSCellHit : ulong {
		None,
		ContentArea = 1,
		EditableTextArea = 2,
		TrackableArae = 4
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSControlTint : ulong {
		Default  = 0,	// system 'default'
		Blue     = 1,
		Graphite = 6,
		Clear    = 7
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSControlSize : ulong {
		Regular = 0, 
		Small = 1,
		Mini = 2,
#if NET
		[SupportedOSPlatform ("macos11.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (11,0)]
#endif
		Large = 3,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSBackgroundStyle : long {
		Normal = 0,
#if NET
		[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
		[Obsolete ("Starting with macos10.14 use 'Normal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Normal' instead.")]
#endif
		Light = Normal,
		Emphasized,
#if NET
		[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
		[Obsolete ("Starting with macos10.14 use 'Emphasized' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Emphasized' instead.")]
#endif
		Dark = Emphasized, 
		Raised, 
		Lowered,
	}
#endregion

#region NSImage
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSImageLoadStatus : ulong {
	    		Completed,
	    		Cancelled,
	    		InvalidData,
	    		UnexpectedEOF,
	    		ReadError
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSImageCacheMode : ulong {
		Default, 
		Always,  
		BySize,  
		Never    
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,10)]
#endif
	[Native (ConvertToNative = "NSImageResizingModeExtensions.ToNative", ConvertToManaged = "NSImageResizingModeExtensions.ToManaged")]
	public enum NSImageResizingMode : long {
		Stretch,
		Tile
	}
		
#endregion
	
#region NSAlert
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSAlertStyle : ulong {
		Warning, Informational, Critical
	}

#if NET
	[SupportedOSPlatform ("macos10.9")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,9)]
#endif
	[Native]
	public enum NSModalResponse : long {
		OK = 1,
		Cancel = 0,
		Stop = -1000,
		Abort = -1001,
		Continue = -1002
	}
#endregion

#region NSEvent
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSEventType : ulong {
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
		DirectTouch = 37, // 10.10
#if NET
		[SupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,15)]
#endif
		ChangeMode = 38,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
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
#if NET
		[SupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,15)]
#endif
		ChangeMode            = 1UL << (int)NSEventType.ChangeMode,
		AnyEvent              = unchecked ((ulong)UInt64.MaxValue)
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSEventModifierMask : ulong {
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

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPointingDeviceType : ulong {
		Unknown, Pen, Cursor, Eraser
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSEventButtonMask : ulong {
		Pen = 1, PenLower = 2, PenUpper = 4
	}

	// This enum is defined as an untyped enum in MacOSX.sdk/System/Library/Frameworks/Carbon.framework/Versions/A/Frameworks/HIToolbox.framework/Versions/A/Headers/Events.h
	// It represents values that may be returned by NSEvent.KeyCode (which isn't typed as 'NSKey' because it may be many other values as well).
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSKey : ulong {
// #endif
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
	}

	// This is an untyped enum in AppKit's NSEvent.h
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSFunctionKey : ulong {
// #else
	// public enum NSFunctionKey : int {
// #endif
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

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSEventSubtype : ulong {
// #else
// 	public enum NSEventSubtype : short {
// #endif
		/* event subtypes for NSEventTypeAppKitDefined events */
		WindowExposed = 0,
		ApplicationActivated = 1,
		ApplicationDeactivated = 2,
		WindowMoved = 4,
		ScreenChanged = 8,
#if !NET
		[Obsolete ("This API is not available on this platform.")]
		AWT = 16,
#endif
		/* event subtypes for NSEventTypeSystemDefined events */
		/* the value is repeated from above */
		PowerOff = 1,

		/* event subtypes for mouse events */
		/* the values are repeated from above */
		MouseEvent = 0, /* NX_SUBTYPE_DEFAULT */
		TabletPoint = 1, /* NX_SUBTYPE_TABLET_POINT */
		TabletProximity = 2, /* NX_SUBTYPE_TABLET_PROXIMITY */
		Touch = 3, /* NX_SUBTYPE_MOUSE_TOUCH */
	}

#if !NET
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSystemDefinedEvents : ulong {
#if NET
		[UnsupportedOSPlatform ("macos10.12")]
#if MONOMAC
		[Obsolete ("Starting with macos10.12 use 'NSEventSubtype.PowerOff' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'NSEventSubtype.PowerOff' instead.")]
#endif
		NSPowerOffEventType = 1
	}
#endif // !NET

#if !NET
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSEventMouseSubtype : ulong {
#if NET
		[UnsupportedOSPlatform ("macos10.12")]
#if MONOMAC
		[Obsolete ("Starting with macos10.12 use 'NSEventSubtype.MouseEvent' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'NSEventSubtype.MouseEvent' instead.")]
#endif
		Mouse, 
#if NET
		[UnsupportedOSPlatform ("macos10.12")]
#if MONOMAC
		[Obsolete ("Starting with macos10.12 use 'NSEventSubtype.TabletPoint' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'NSEventSubtype.TabletPoint' instead.")]
#endif
		TablePoint, 
#if NET
		[UnsupportedOSPlatform ("macos10.12")]
#if MONOMAC
		[Obsolete ("Starting with macos10.12 use 'NSEventSubtype.TabletProximity' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'NSEventSubtype.TabletProximity' instead.")]
#endif
		TabletProximity,
#if NET
		[UnsupportedOSPlatform ("macos10.12")]
#if MONOMAC
		[Obsolete ("Starting with macos10.12 use 'NSEventSubtype.Touch' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'NSEventSubtype.Touch' instead.")]
#endif
		Touch,
	}
#endif // !NET
	
#endregion

#region NSView
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSViewResizingMask : ulong {
		NotSizable		=  0,
		MinXMargin		=  1,
		WidthSizable		=  2,
		MaxXMargin		=  4,
		MinYMargin		=  8,
		HeightSizable		= 16,
		MaxYMargin		= 32
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSBorderType : ulong {
		NoBorder, LineBorder, BezelBorder, GrooveBorder
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTextFieldBezelStyle : ulong {
		Square, Rounded
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSViewLayerContentsRedrawPolicy : long {
		Never,
		OnSetNeedsDisplay,
		DuringViewResize, 
		BeforeViewResize,
#if NET
		[SupportedOSPlatform ("macos10.9")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,9)]
#endif
		Crossfade = 4,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSViewLayerContentsPlacement : long {
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
	// This enum is called NSWindowStyleMask in the headers.
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSWindowStyle : ulong {
		Borderless	       					= 0 << 0,
		Titled		       					= 1 << 0,
		Closable	       					= 1 << 1,
		Miniaturizable	      				= 1 << 2,
		Resizable	       					= 1 << 3,
		Utility		       					= 1 << 4,
		DocModal	       					= 1 << 6,
		NonactivatingPanel     				= 1 << 7,
#if NET
		[UnsupportedOSPlatform ("macos11.0")]
#if MONOMAC
		[Obsolete ("Starting with macos11.0 don't use 'TexturedBackground' anymore.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Don't use 'TexturedBackground' anymore.")]
#endif
		TexturedBackground     				= 1 << 8,
#if !NET
#if NET
		[UnsupportedOSPlatform ("macos10.9")]
#if MONOMAC
		[Obsolete ("Starting with macos10.9 don't use, this value has no effect.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Don't use, this value has no effect.")]
#endif
		Unscaled	       					= 1 << 11,
#endif
		UnifiedTitleAndToolbar 				= 1 << 12,
		Hud		       						= 1 << 13,
		FullScreenWindow       				= 1 << 14,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
FullSizeContentView   = 1 << 15 
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSWindowSharingType : ulong {
		None, ReadOnly, ReadWrite
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSWindowBackingLocation : ulong {
		Default, VideoMemory, MainMemory,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSWindowCollectionBehavior : ulong {
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
#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 11)]
#endif
FullScreenAllowsTiling = 1 << 11,
#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 11)]
#endif
FullScreenDisallowsTiling = 1 << 12
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSWindowNumberListOptions : ulong {
		AllApplication = 1 << 0,
		AllSpaces = 1 << 4
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSelectionDirection : ulong {
		Direct = 0,
		Next,
		Previous
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSWindowButton : ulong {
		CloseButton, MiniaturizeButton, ZoomButton, ToolbarButton, DocumentIconButton, DocumentVersionsButton = 6, 
#if NET
		[UnsupportedOSPlatform ("macos10.12")]
#if MONOMAC
		[Obsolete ("Starting with macos10.12 the standard window button for FullScreenButton is always null; use ZoomButton instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "The standard window button for FullScreenButton is always null; use ZoomButton instead.")]
#endif
		FullScreenButton
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSTouchPhase : ulong {
		Began           = 1 << 0,
		Moved           = 1 << 1,
		Stationary      = 1 << 2,
		Ended           = 1 << 3,
		Cancelled       = 1 << 4,
		
		Touching        = Began | Moved | Stationary,
		Any             = unchecked ((ulong)UInt64.MaxValue)
	}
#endregion
#region NSAnimation
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSAnimationCurve : ulong {
		EaseInOut,
		EaseIn,
		EaseOut,
		Linear
	};
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSAnimationBlockingMode : ulong {
		Blocking,
		Nonblocking,
		NonblockingThreaded
	};
#endregion

#region NSBox
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTitlePosition : ulong {
		NoTitle,
		AboveTop,
		AtTop,
		BelowTop,
		AboveBottom,
		AtBottom,
		BelowBottom
	};

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSBoxType : ulong {
		NSBoxPrimary,
#if NET
		[UnsupportedOSPlatform ("maccatalyst")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 identical to 'NSBoxPrimary'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Obsoleted (PlatformName.MacOSX, 10,15, message: "Identical to 'NSBoxPrimary'.")]
#endif
		NSBoxSecondary,
		NSBoxSeparator,
#if NET
		[UnsupportedOSPlatform ("maccatalyst")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 'NSBoxOldStyle' is discouraged. Use 'NSBoxPrimary' or 'NSBoxCustom'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Obsoleted (PlatformName.MacOSX, 10,15, message: "'NSBoxOldStyle' is discouraged. Use 'NSBoxPrimary' or 'NSBoxCustom'.")]
#endif
		NSBoxOldStyle,
		NSBoxCustom
	};
#endregion

#region NSButtonCell
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSButtonType : ulong {
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
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSBezelStyle : ulong {
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

#if NET
	[UnsupportedOSPlatform ("macos10.12")]
#if MONOMAC
	[Obsolete ("Starting with macos10.12 the GradientType property is unused, and setting it has no effect.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 12, message : "The GradientType property is unused, and setting it has no effect.")]
#endif
	[Native]
	public enum NSGradientType : ulong {
		None,
		ConcaveWeak,
		ConcaveStrong,
		ConvexWeak,
		ConvexStrong
	}
	
#endregion

#region NSGraphics
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	// NSGraphics.h:typedef int NSWindowDepth;
	public enum NSWindowDepth : int {
		TwentyfourBitRgb = 0x208,
		SixtyfourBitRgb = 0x210,
		OneHundredTwentyEightBitRgb = 0x220	
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSCompositingOperation : ulong {
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

#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Multiply,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Screen,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Overlay,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Darken,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Lighten,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		ColorDodge,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		ColorBurn,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		SoftLight,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		HardLight,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Difference,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Exclusion,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Hue,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Saturation,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Color,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 10)]
#endif
		Luminosity
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSAnimationEffect : ulong {
		DissapearingItemDefault = 0,
		EffectPoof = 10
	}
#endregion
	
#region NSMatrix
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSMatrixMode : ulong {
		Radio, Highlight, List, Track
	}
#endregion

#region NSBrowser
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSBrowserColumnResizingType : ulong {
		None, Auto, User
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSBrowserDropOperation : ulong {
		On, Above
	}
#endregion

#region NSColorPanel
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSColorPanelMode : long {
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

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSColorPanelFlags : ulong {
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

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSDocumentChangeType : ulong  {
		Done, Undone, Cleared, ReadOtherContents, Autosaved, Redone,
		Discardable = 256 /* New in Lion */
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSaveOperationType : ulong  {
		Save, SaveAs, SaveTo,
		Autosave = 3,	/* Deprecated name in Lion */
		Elsewhere = 3,	/* New Lion name */
		InPlace = 4,	/* New in Lion */
		AutoSaveAs = 5	/* New in Mountain Lion */
	}

#endregion

#region NSBezelPath
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSLineCapStyle : ulong {
		Butt, Round, Square
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSLineJoinStyle : ulong {
		Miter, Round, Bevel
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSWindingRule : ulong {
		NonZero, EvenOdd
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSBezierPathElement : ulong {
		MoveTo, LineTo, CurveTo, ClosePath
	}
#endregion

#region NSRulerView
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSRulerOrientation : ulong {
		Horizontal, Vertical
	}
#endregion

#region NSGestureRecognizer
#if NET
	[SupportedOSPlatform ("macos10.10")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,10)]
#endif
	[Native]
	public enum NSGestureRecognizerState : long {
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
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSUserInterfaceLayoutOrientation : long {
		Horizontal = 0,
		Vertical = 1
	}

	// NSStackView.h:typedef float NSStackViewVisibilityPriority
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	public enum NSStackViewVisibilityPriority : int {
		MustHold = 1000,
#if !XAMCORE_4_0
		[Obsolete ("Use 'MustHold' instead.")]
		Musthold = MustHold,
#endif
		DetachOnlyIfNecessary = 900,
		NotVisible = 0
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSStackViewGravity : long {
		Top = 1,
		Leading = 1,
		Center = 2,
		Bottom = 3,
		Trailing = 3
	}
#endregion

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSStackViewDistribution : long
	{
		GravityAreas = -1,
		Fill = 0,
		FillEqually,
		FillProportionally,
		EqualSpacing,
		EqualCentering
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSDragOperation : ulong {
		None,
		Copy = 1,
		Link = 2,
		Generic = 4,
		Private = 8,
		AllObsolete = 15,
		Move = 16,
		Delete = 32,
		All = ulong.MaxValue,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native (ConvertToNative = "NSTextAlignmentExtensions.ToNative", ConvertToManaged = "NSTextAlignmentExtensions.ToManaged")]
	public enum NSTextAlignment : ulong {
		Left = 0,
		Right = 1, 
		Center = 2,
		Justified = 3, 
		Natural = 4
	}

#if !NET && MONOMAC
	// Use Foundation.NSWritingDirection in .NET.
	// see: https://github.com/xamarin/xamarin-macios/issues/6573
	[Flags]
	[Native]
	public enum NSWritingDirection : long {
		Natural = -1, LeftToRight, RightToLeft,
		[Obsolete ("Use 'LeftToRight' instead.")]
		Embedding = 0,
		[Obsolete ("This API is not available on this platform.")]
		Override = 2,
	}
#endif // !NET && MONOMAC

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTextMovement : long {
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
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSMenuProperty : ulong {
		Title = 1 << 0,
		AttributedTitle = 1 << 1,
		KeyEquivalent = 1 << 2,
		Image = 1 << 3,
		Enabled = 1 << 4,
		AccessibilityDescription = 1 << 5
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSFontRenderingMode : ulong {
		Default, Antialiased, IntegerAdvancements, AntialiasedIntegerAdvancements
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSPasteboardReadingOptions : ulong {
		AsData = 0,
		AsString = 1,
		AsPropertyList = 2,
		AsKeyedArchive = 4
	}

#if !NET && MONOMAC // Use the one in Foundation instead, only keep this in macOS until .NET.
	[Native]
	public enum NSUnderlineStyle : long {
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
#endif

	// Convenience enum, untyped in ObjC
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	public enum NSUnderlinePattern : int {
		Solid             = 0x0000,
		Dot               = 0x0100,
		Dash              = 0x0200,
		DashDot           = 0x0300,
		DashDotDot        = 0x0400
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSelectionAffinity : ulong {
		Upstream, Downstream
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSelectionGranularity : ulong {
		Character, Word, Paragraph
	}

#region NSTrackingArea
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSTrackingAreaOptions : ulong {
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

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSLineSweepDirection : ulong {
		NSLineSweepLeft,
		NSLineSweepRight,
		NSLineSweepDown,
		NSLineSweepUp
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSLineMovementDirection : ulong {
		None, Left, Right, Down, Up
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum  NSTiffCompression : ulong {
		None = 1,
		CcittFax3 = 3,
		CcittFax4 = 4,
		Lzw = 5,

#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		Jpeg		= 6,
		Next		= 32766,
		PackBits	= 32773,

#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		OldJpeg		= 32865
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSBitmapImageFileType : ulong {
		Tiff,
		Bmp,
		Gif,
		Jpeg,
		Png,
		Jpeg2000
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSImageRepLoadStatus : long {
		UnknownType     = -1,
		ReadingHeader   = -2,
		WillNeedAllData = -3,
		InvalidData     = -4,
		UnexpectedEOF   = -5,
		Completed       = -6 
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSBitmapFormat : ulong {
		AlphaFirst = 1,
		AlphaNonpremultiplied = 2,
		FloatingPointSamples = 4,

		LittleEndian16Bit = 1 << 8,
		LittleEndian32Bit = 1 << 9,
		BigEndian16Bit = 1 << 10,
		BigEndian32Bit = 1 << 11
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPrintingOrientation : ulong {
		Portrait, Landscape
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPrintingPaginationMode : ulong {
		Auto, Fit, Clip
	}

#if !NET
#if NET
	[UnsupportedOSPlatform ("macos10.11")]
#if MONOMAC
	[Obsolete ("Starting with macos10.11 use 'NSGlyphProperty' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'NSGlyphProperty' instead.")]
#endif
	[Flags]
	[Native]
	public enum NSGlyphStorageOptions : ulong {
		ShowControlGlyphs = 1,
		ShowInvisibleGlyphs = 2,
		WantsBidiLevels = 4
	}
#endif // !NET

#if !XAMCORE_4_0
#if NET
	[UnsupportedOSPlatform ("macos10.11")]
#if MONOMAC
	[Obsolete ("Starting with macos10.11 use NSTextStorageEditActions instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use NSTextStorageEditActions instead.")]
#endif
	[Flags]
	[Native]
	public enum NSTextStorageEditedFlags : ulong {
		EditedAttributed = 1,
		EditedCharacters = 2
	}
#endif

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPrinterTableStatus : ulong {
		Ok, NotFound, Error
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14)]
#endif
	[Native]
	public enum NSScrollArrowPosition : ulong {
		MaxEnd = 0,
		MinEnd = 1,
		DefaultSetting = 0,
		None = 2,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSUsableScrollerParts : ulong {
		NoScroller, 
#if NET
		[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
		[Obsolete ("Starting with macos10.14.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 14)]
#endif
		OnlyArrows, 
		All,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSScrollerPart : ulong {
		None,
		DecrementPage,
		Knob,
		IncrementPage,
#if NET
		[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
		[Obsolete ("Starting with macos10.14.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 14)]
#endif
		DecrementLine,
#if NET
		[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
		[Obsolete ("Starting with macos10.14.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 14)]
#endif
		IncrementLine,
		KnobSlot,
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14)]
#endif
	[Native]
	public enum NSScrollerArrow : ulong {
		IncrementArrow, DecrementArrow
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPrintingPageOrder : long {
		Descending = -1,
		Special,
		Ascending,
		Unknown
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSPrintPanelOptions : long {
		ShowsCopies = 1,
		ShowsPageRange = 2,
		ShowsPaperSize = 4,
		ShowsOrientation = 8,
		ShowsScaling = 16,
		ShowsPrintSelection = 32,
		ShowsPageSetupAccessory = 256,
		ShowsPreview = 131072
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTextBlockValueType : ulong {
		Absolute, Percentage
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTextBlockDimension : ulong {
		Width = 0,
		MinimumWidth = 1,
		MaximumWidth = 2, 
		Height = 4,
		MinimumHeight = 5, 
		MaximumHeight = 6,
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTextBlockLayer : long {
		Padding = -1, Border, Margin
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTextBlockVerticalAlignment : ulong {
		Top, Middle, Bottom, Baseline
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTextTableLayoutAlgorithm : ulong {
		Automatic, Fixed
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSTextListOptions : ulong {
		PrependEnclosingMarker = 1
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	public enum NSFontSymbolicTraits : int { // uint32_t NSFontSymbolicTraits
		ItalicTrait = (1 << 0),
		BoldTrait = (1 << 1),
		ExpandedTrait = (1 << 5),
		CondensedTrait = (1 << 6),
		MonoSpaceTrait = (1 << 10),
		VerticalTrait = (1 << 11), 
		UIOptimizedTrait = (1 << 12),
#if NET
		[SupportedOSPlatform ("macos10.13")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,13)]
#endif
		TraitTightLeading = 1 << 15,
#if NET
		[SupportedOSPlatform ("macos10.13")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,13)]
#endif
		TraitLooseLeading = 1 << 16,
		TraitEmphasized = BoldTrait,
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

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSFontTraitMask : ulong {
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
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSPasteboardWritingOptions : ulong	 {
		WritingPromised = 1 << 9
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[MacCatalyst (13, 0)]
#endif
	[Native]
	public enum NSToolbarDisplayMode : ulong {
		Default, IconAndLabel, Icon, Label
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[MacCatalyst (13, 0)]
#endif
	[Native]
	public enum NSToolbarSizeMode : ulong {
		Default, Regular, Small
	}

#if !NET
#if NET
	[UnsupportedOSPlatform ("macos10.10")]
#if MONOMAC
	[Obsolete ("Starting with macos10.10 use NSAlertButtonReturn instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use NSAlertButtonReturn instead.")]
#endif
	[Native]
	public enum NSAlertType : long {
		ErrorReturn = -2,
		OtherReturn,
		AlternateReturn,
		DefaultReturn
	}
#endif // !NET

#if !XAMCORE_4_0
#if NET
	[UnsupportedOSPlatform ("macos10.10")]
#if MONOMAC
	[Obsolete ("Starting with macos10.10 use NSModalResponse instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use NSModalResponse instead.")]
#endif
	[Native]
	public enum NSPanelButtonType : long {
		Cancel, Ok
	}
#endif

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTableViewColumnAutoresizingStyle : ulong {
		None = 0,
		Uniform,
		Sequential,
		ReverseSequential,
		LastColumnOnly,
		FirstColumnOnly
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTableViewSelectionHighlightStyle : long {
		None = -1,
		Regular = 0,
#if NET
		[UnsupportedOSPlatform ("macos11.0")]
#if MONOMAC
		[Obsolete ("Starting with macos11.0 set 'NSTableView.Style' to 'NSTableViewStyle.SourceList' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Set 'NSTableView.Style' to 'NSTableViewStyle.SourceList' instead.")]
#endif
		SourceList = 1,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTableViewDraggingDestinationFeedbackStyle : long {
		None = -1,
		Regular = 0,
		SourceList = 1,
		FeedbackStyleGap = 2,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTableViewDropOperation : ulong {
		On,
		Above
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSTableColumnResizing : long {
		None = -1,
		Autoresizing = ( 1 << 0 ),
		UserResizingMask = ( 1 << 1 )
	} 

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSTableViewGridStyle : ulong {
		None = 0,
		SolidVerticalLine   = 1 << 0,
		SolidHorizontalLine = 1 << 1,
		DashedHorizontalGridLine = 1 << 3
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSGradientDrawingOptions : ulong {
		None = 0,
		BeforeStartingLocation =   (1 << 0),
		AfterEndingLocation =    (1 << 1)
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSImageAlignment : ulong {
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
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSImageFrameStyle : ulong {
		None = 0,
		Photo,
		GrayBezel,
		Groove,
		Button
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSpeechBoundary : ulong {
		Immediate =  0,
#if !XAMCORE_4_0
		[Obsolete ("Use 'Word' instead.")]
		hWord,
#endif
		Word = 1,
		Sentence
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSplitViewDividerStyle : long {
		Thick = 1,
		Thin = 2,
		PaneSplitter = 3
	}

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSSplitViewItemBehavior : long
	{
		Default,
		Sidebar,
		ContentList
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSImageScaling : ulong {
		ProportionallyDown = 0,
		AxesIndependently,
		None,
		ProportionallyUpOrDown
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSegmentStyle : long {
		Automatic = 0,
		Rounded = 1,
		TexturedRounded = 2,
		RoundRect = 3,
		TexturedSquare = 4,
		Capsule = 5,
		SmallSquare = 6,
#if NET
		[SupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,10)]
#endif
Separated = 8
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSegmentSwitchTracking : ulong {
		SelectOne = 0,
		SelectAny = 1,
		Momentary = 2,
		MomentaryAccelerator // 10.10.3
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTickMarkPosition : ulong {
		Below,
		Above,
		Left,
		Right,
		Leading = Left,
		Trailing = Right
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSliderType : ulong {
		Linear   = 0,
		Circular = 1
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTokenStyle : ulong {
		Default,
		PlainText,
		Rounded,
		Squared = 3,
		PlainSquared = 4,
	}

#if NET
	[UnsupportedOSPlatform ("macos11.0")]
#if MONOMAC
	[Obsolete ("Starting with macos11.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
	[Flags]
	[Native]
	public enum NSWorkspaceLaunchOptions : ulong {
		Print = 2,
		WithErrorPresentation = 0x40,
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

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSWorkspaceIconCreationOptions : ulong {
		NSExcludeQuickDrawElements   = 1 << 1,
		NSExclude10_4Elements       = 1 << 2
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPathStyle : long {
		Standard,
#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		NavigationBar,
		PopUp
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTabViewType : ulong {
		NSTopTabsBezelBorder,
		NSLeftTabsBezelBorder,
		NSBottomTabsBezelBorder,
		NSRightTabsBezelBorder,
		NSNoTabsBezelBorder,
		NSNoTabsLineBorder,
		NSNoTabsNoBorder,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTabState : ulong {
		Selected, Background, Pressed
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTabViewControllerTabStyle : long {
		SegmentedControlOnTop = 0,
		SegmentedControlOnBottom,
		Toolbar,
		Unspecified = -1
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSLevelIndicatorStyle : ulong {
		Relevancy, ContinuousCapacity, DiscreteCapacity, RatingLevel
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSFontCollectionOptions : long {
		ApplicationOnlyMask = 1
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.1")]
#else
	[MacCatalyst (13,1)]
#endif
	[Native]
	public enum NSCollectionViewDropOperation : long {
		On = 0, Before = 1
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.1")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[MacCatalyst (13,1)]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSCollectionViewItemHighlightState : long
	{
		None = 0,
		ForSelection = 1,
		ForDeselection = 2,
		AsDropTarget = 3
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[MacCatalyst (13,0)]
	[Mac (10,11)] // Not marked 10.11 in the headers, but doesn't exist in the 10.10 headers
#endif
	[Native]
	[Flags]
	public enum NSCollectionViewScrollPosition : ulong
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

#if NET
	[SupportedOSPlatform ("maccatalyst13.1")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[MacCatalyst (13,1)]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSCollectionElementCategory : long
	{
		Item,
		SupplementaryView,
		DecorationView,
		InterItemGap
	}

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSCollectionUpdateAction : long
	{
		Insert,
		Delete,
		Reload,
		Move,
		None
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.1")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[MacCatalyst (13,1)]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSCollectionViewScrollDirection : long
	{
		Vertical,
		Horizontal
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSDatePickerStyle : ulong {
		TextFieldAndStepper,
		ClockAndCalendar,
		TextField
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSDatePickerMode : ulong {
		Single, Range
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSDatePickerElementFlags : ulong {
		HourMinute = 0xc,
		HourMinuteSecond = 0xe,
		TimeZone = 0x10,

		YearMonthDate = 0xc0,
		YearMonthDateDay = 0xe0,
		Era = 0x100
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 use 'Metal' Framework instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
#endif
	[Native]
	public enum NSOpenGLContextParameter : ulong {
#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		SwapRectangle = 200,
#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		SwapRectangleEnable = 201,
#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		RasterizationEnable = 221,
#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		StateValidation = 301,
#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		SurfaceSurfaceVolatile = 306,

		SwapInterval = 222,
		SurfaceOrder = 235,
		SurfaceOpacity = 236,

		SurfaceBackingSize = 304,
		ReclaimResources = 308,
		CurrentRendererID = 309,
		GpuVertexProcessing = 310,
		GpuFragmentProcessing = 311,
		HasDrawable = 314,
		MpsSwapsInFlight = 315
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	public enum NSSurfaceOrder {
		AboveWindow = 1,
		BelowWindow = -1
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 use 'Metal' Framework instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
#endif
	public enum NSOpenGLPixelFormatAttribute : uint { // uint32_t NSOpenGLPixelFormatAttribute
		AllRenderers       =   1,
		DoubleBuffer       =   5,
		TripleBuffer = 3,
#if !XAMCORE_4_0
		[Obsolete ("Use 'TripleBuffer' instead.")]
		TrippleBuffer = TripleBuffer,
#endif
		Stereo             =   6,
		AuxBuffers         =   7,
		ColorSize          =   8,
		AlphaSize          =  11,
		DepthSize          =  12,
		StencilSize        =  13,
		AccumSize          =  14,
		MinimumPolicy      =  51,
		MaximumPolicy      =  52,
#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		OffScreen          =  53,
#if NET
		[UnsupportedOSPlatform ("macos10.6")]
#if MONOMAC
		[Obsolete ("Starting with macos10.6.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 6)]
#endif
		FullScreen         =  54,
		SampleBuffers      =  55,
		Samples            =  56,
		AuxDepthStencil    =  57,
		ColorFloat         =  58,
		Multisample        =  59,
		Supersample        =  60,
		SampleAlpha        =  61,
		RendererID         =  70,
#if NET
		[UnsupportedOSPlatform ("macos10.9")]
#if MONOMAC
		[Obsolete ("Starting with macos10.9.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		SingleRenderer     =  71,
		NoRecovery         =  72,
		Accelerated        =  73,
		ClosestPolicy      =  74,
		BackingStore       =  76,
#if NET
		[UnsupportedOSPlatform ("macos10.9")]
#if MONOMAC
		[Obsolete ("Starting with macos10.9.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		Window             =  80,
#if NET
		[UnsupportedOSPlatform ("macos10.9")]
#if MONOMAC
		[Obsolete ("Starting with macos10.9.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		Compliant          =  83,
		ScreenMask         =  84,
#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		PixelBuffer        =  90,
#if NET
		[UnsupportedOSPlatform ("macos10.7")]
#if MONOMAC
		[Obsolete ("Starting with macos10.7.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		RemotePixelBuffer  =  91,
		AllowOfflineRenderers = 96,
		AcceleratedCompute =  97,

		// Specify the profile
		OpenGLProfile = 99,
		VirtualScreenCount = 128,

#if NET
		[UnsupportedOSPlatform ("macos10.5")]
#if MONOMAC
		[Obsolete ("Starting with macos10.5.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 5)]
#endif
		Robust  =  75,
#if NET
		[UnsupportedOSPlatform ("macos10.5")]
#if MONOMAC
		[Obsolete ("Starting with macos10.5.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 5)]
#endif
		MPSafe  =  78,
#if NET
		[UnsupportedOSPlatform ("macos10.5")]
#if MONOMAC
		[Obsolete ("Starting with macos10.5.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 5)]
#endif
		MultiScreen =  81
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 use 'Metal' Framework instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
#endif
	public enum NSOpenGLProfile : int {
		VersionLegacy   = 0x1000, // Legacy
		Version3_2Core  = 0x3200,  // 3.2 or better
		Version4_1Core  = 0x4100
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSAlertButtonReturn : long {
		First = 1000,
		Second = 1001,
		Third = 1002,
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 use 'Metal' Framework instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
#endif
	public enum NSOpenGLGlobalOption : uint {
		FormatCacheSize = 501,
		ClearFormatCache = 502,
		RetainRenderers = 503,
		UseBuildCache = 506,
#if NET
		[UnsupportedOSPlatform ("macos10.4")]
#if MONOMAC
		[Obsolete ("Starting with macos10.4.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 4)]
#endif
		ResetLibrary = 504
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 use 'Metal' Framework instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
#endif
	public enum NSGLTextureTarget : uint {
		T2D = 0x0de1,
		CubeMap = 0x8513,
		RectangleExt = 0x84F5,
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 use 'Metal' Framework instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
#endif
	public enum NSGLFormat : uint {
		RGB = 0x1907,
		RGBA = 0x1908,
		DepthComponent = 0x1902,
	}
	
#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 use 'Metal' Framework instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
#endif
	public enum NSGLTextureCubeMap : uint {
		None = 0,
		PositiveX = 0x8515,
		PositiveY = 0x8517,
		PositiveZ = 0x8519,
		NegativeX = 0x8516,
		NegativeY = 0x8517,
		NegativeZ = 0x851A
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 use 'Metal' Framework instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
#endif
	public enum NSGLColorBuffer : uint {
		Front = 0x0404,
		Back = 0x0405,
		Aux0 = 0x0409
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14)]
#endif
	[Native]
	public enum NSProgressIndicatorThickness : ulong {
		Small = 10,
		Regular = 14,
		Aqua = 12,
		Large = 18
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSProgressIndicatorStyle : ulong {
		Bar, Spinning
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPopUpArrowPosition : ulong {
		None,
		Center,
		Bottom
	}

	// FileType 4cc values to use with NSFileTypeForHFSTypeCode.
#if NET
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[MacCatalyst (15, 0)]
#endif
	public enum HfsTypeCode : uint
	{
		/* Generic Finder icons */
		ClipboardIcon                 = 0x434C4950,   //'CLIP'
		ClippingUnknownTypeIcon       = 0x636C7075,   //'clpu'
		ClippingPictureTypeIcon       = 0x636C7070,   //'clpp'
		ClippingTextTypeIcon          = 0x636C7074,   //'clpt'
		ClippingSoundTypeIcon         = 0x636C7073,   //'clps'
		DesktopIcon                   = 0x6465736B,   //'desk'
		FinderIcon                    = 0x464E4452,   //'FNDR'
		ComputerIcon                  = 0x726F6F74,   //'root'
		FontSuitcaseIcon              = 0x4646494C,   //'FFIL'
		FullTrashIcon                 = 0x66747268,   //'ftrh'
		GenericApplicationIcon        = 0x4150504C,   //'APPL'
		GenericCdromIcon              = 0x63646472,   //'cddr'
		GenericControlPanelIcon       = 0x41505043,   //'APPC'
		GenericControlStripModuleIcon = 0x73646576,   //'sdev'
		GenericComponentIcon          = 0x74686E67,   //'thng'
		GenericDeskAccessoryIcon      = 0x41505044,   //'APPD'
		GenericDocumentIcon           = 0x646F6375,   //'docu'
		GenericEditionFileIcon        = 0x65647466,   //'edtf'
		GenericExtensionIcon          = 0x494E4954,   //'INIT'
		GenericFileServerIcon         = 0x73727672,   //'srvr'
		GenericFontIcon               = 0x6666696C,   //'ffil'
		GenericFontScalerIcon         = 0x73636C72,   //'sclr'
		GenericFloppyIcon             = 0x666C7079,   //'flpy'
		GenericHardDiskIcon           = 0x6864736B,   //'hdsk'
		GenericIDiskIcon              = 0x6964736B,   //'idsk'
		GenericRemovableMediaIcon     = 0x726D6F76,   //'rmov'
		GenericMoverObjectIcon        = 0x6D6F7672,   //'movr'
		GenericPCCardIcon             = 0x70636D63,   //'pcmc'
		GenericPreferencesIcon        = 0x70726566,   //'pref'
		GenericQueryDocumentIcon      = 0x71657279,   //'qery'
		GenericRamDiskIcon            = 0x72616D64,   //'ramd'
#if !XAMCORE_4_0
		[Obsolete ("Use 'GenericSharedLibraryIcon' instead.")]
		GenericSharedLibaryIcon       = 0x73686C62,   //'shlb'
#endif
		GenericSharedLibraryIcon      = 0x73686C62,   //'shlb'
		GenericStationeryIcon         = 0x73646F63,   //'sdoc'
		GenericSuitcaseIcon           = 0x73756974,   //'suit'
		GenericUrlIcon                = 0x6775726C,   //'gurl'
		GenericWormIcon               = 0x776F726D,   //'worm'
		InternationalResourcesIcon    = 0x6966696C,   //'ifil'
		KeyboardLayoutIcon            = 0x6B66696C,   //'kfil'
		SoundFileIcon                 = 0x7366696C,   //'sfil'
		SystemSuitcaseIcon            = 0x7A737973,   //'zsys'
		TrashIcon                     = 0x74727368,   //'trsh'
		TrueTypeFontIcon              = 0x7466696C,   //'tfil'
		TrueTypeFlatFontIcon          = 0x73666E74,   //'sfnt'
		TrueTypeMultiFlatFontIcon     = 0x74746366,   //'ttcf'
		UserIDiskIcon                 = 0x7564736B,   //'udsk'
		UnknownFSObjectIcon           = 0x756E6673,   //'unfs'

		/* Internet locations */
		InternetLocationHttpIcon            = 0x696C6874,   //'ilht'
		InternetLocationFtpIcon             = 0x696C6674,   //'ilft'
		InternetLocationAppleShareIcon      = 0x696C6166,   //'ilaf'
		InternetLocationAppleTalkZoneIcon   = 0x696C6174,   //'ilat'
		InternetLocationFileIcon            = 0x696C6669,   //'ilfi'
		InternetLocationMailIcon            = 0x696C6D61,   //'ilma'
		InternetLocationNewsIcon            = 0x696C6E77,   //'ilnw'
		InternetLocationNslNeighborhoodIcon = 0x696C6E73,   //'ilns'
		InternetLocationGenericIcon         = 0x696C6765,   //'ilge'

		/* Folders */
		GenericFolderIcon = 0x666C6472,   //'fldr'
		DropFolderIcon    = 0x64626F78,   //'dbox'
		MountedFolderIcon = 0x6D6E7464,   //'mntd'
		OpenFolderIcon    = 0x6F666C64,   //'ofld'
		OwnedFolderIcon   = 0x6F776E64,   //'ownd'
		PrivateFolderIcon = 0x70727666,   //'prvf'
		SharedFolderIcon  = 0x7368666C,   //'shfl'

		/* Sharingprivileges icons */
		SharingPrivsNotApplicableIcon = 0x73686E61,   //'shna'
		SharingPrivsReadOnlyIcon      = 0x7368726F,   //'shro'
		SharingPrivsReadWriteIcon     = 0x73687277,   //'shrw'
		SharingPrivsUnknownIcon       = 0x7368756B,   //'shuk'
		SharingPrivsWritableIcon      = 0x77726974,   //'writ'

		/* Users and Groups icons */
		UserFolderIcon      = 0x75666C64,   //'ufld'
		WorkgroupFolderIcon = 0x77666C64,   //'wfld'
		GuestUserIcon       = 0x67757372,   //'gusr'
		UserIcon            = 0x75736572,   //'user'
		OwnerIcon           = 0x73757372,   //'susr'
		GroupIcon           = 0x67727570,   //'grup'

		/* Special folders */
		AppearanceFolderIcon              = 0x61707072,   //'appr'
		AppleMenuFolderIcon               = 0x616D6E75,   //'amnu'
		ApplicationsFolderIcon            = 0x61707073,   //'apps'
		ApplicationSupportFolderIcon      = 0x61737570,   //'asup'
		ColorSyncFolderIcon               = 0x70726F66,   //'prof'
		ContextualMenuItemsFolderIcon     = 0x636D6E75,   //'cmnu'
		ControlPanelDisabledFolderIcon    = 0x63747244,   //'ctrD'
		ControlPanelFolderIcon            = 0x6374726C,   //'ctrl'
		DocumentsFolderIcon               = 0x646F6373,   //'docs'
		ExtensionsDisabledFolderIcon      = 0x65787444,   //'extD'
		ExtensionsFolderIcon              = 0x6578746E,   //'extn'
		FavoritesFolderIcon               = 0x66617673,   //'favs'
		FontsFolderIcon                   = 0x666F6E74,   //'font'
		InternetSearchSitesFolderIcon     = 0x69737366,   //'issf'
		PublicFolderIcon                  = 0x70756266,   //'pubf'
		PrinterDescriptionFolderIcon      = 0x70706466,   //'ppdf'
		PrintMonitorFolderIcon            = 0x70726E74,   //'prnt'
		RecentApplicationsFolderIcon      = 0x72617070,   //'rapp'
		RecentDocumentsFolderIcon         = 0x72646F63,   //'rdoc'
		RecentServersFolderIcon           = 0x72737276,   //'rsrv'
		ShutdownItemsDisabledFolderIcon   = 0x73686444,   //'shdD'
		ShutdownItemsFolderIcon           = 0x73686466,   //'shdf'
		SpeakableItemsFolder              = 0x73706B69,   //'spki'
		StartupItemsDisabledFolderIcon    = 0x73747244,   //'strD'
		StartupItemsFolderIcon            = 0x73747274,   //'strt'
		SystemExtensionDisabledFolderIcon = 0x6D616344,   //'macD'
		SystemFolderIcon                  = 0x6D616373,   //'macs'
		VoicesFolderIcon                  = 0x66766F63,   //'fvoc'

		/* Badges */
		AppleScriptBadgeIcon  = 0x73637270,   //'scrp'
		LockedBadgeIcon       = 0x6C626467,   //'lbdg'
		MountedBadgeIcon      = 0x6D626467,   //'mbdg'
		SharedBadgeIcon       = 0x73626467,   //'sbdg'
		AliasBadgeIcon        = 0x61626467,   //'abdg'
		AlertCautionBadgeIcon = 0x63626467,   //'cbdg'

		/* Alert icons */
		AlertNoteIcon    = 0x6E6F7465,   //'note'
		AlertCautionIcon = 0x63617574,   //'caut'
		AlertStopIcon    = 0x73746F70,   //'stop'

		/* Networking icons */
		AppleTalkIcon      = 0x61746C6B,   //'atlk'
		AppleTalkZoneIcon  = 0x61747A6E,   //'atzn'
		AfpServerIcon      = 0x61667073,   //'afps'
		FtpServerIcon      = 0x66747073,   //'ftps'
		HttpServerIcon     = 0x68747073,   //'htps'
		GenericNetworkIcon = 0x676E6574,   //'gnet'
		IPFileServerIcon   = 0x69737276,   //'isrv'

		/* Toolbar icons */
		ToolbarCustomizeIcon          = 0x74637573,   //'tcus'
		ToolbarDeleteIcon             = 0x7464656C,   //'tdel'
		ToolbarFavoritesIcon          = 0x74666176,   //'tfav'
		ToolbarHomeIcon               = 0x74686F6D,   //'thom'
		ToolbarAdvancedIcon           = 0x74626176,   //'tbav'
		ToolbarInfoIcon               = 0x7462696E,   //'tbin'
		ToolbarLabelsIcon             = 0x74626C62,   //'tblb'
		ToolbarApplicationsFolderIcon = 0x74417073,   //'tAps'
		ToolbarDocumentsFolderIcon    = 0x74446F63,   //'tDoc'
		ToolbarMovieFolderIcon        = 0x744D6F76,   //'tMov'
		ToolbarMusicFolderIcon        = 0x744D7573,   //'tMus'
		ToolbarPicturesFolderIcon     = 0x74506963,   //'tPic'
		ToolbarPublicFolderIcon       = 0x74507562,   //'tPub'
		ToolbarDesktopFolderIcon      = 0x7444736B,   //'tDsk'
		ToolbarDownloadsFolderIcon    = 0x7444776E,   //'tDwn'
		ToolbarLibraryFolderIcon      = 0x744C6962,   //'tLib'
		ToolbarUtilitiesFolderIcon    = 0x7455746C,   //'tUtl'
		ToolbarSitesFolderIcon        = 0x74537473,   //'tSts'

		/* Other icons */
		AppleLogoIcon                  = 0x6361706C,   //'capl'
		AppleMenuIcon                  = 0x7361706C,   //'sapl'
		BackwardArrowIcon              = 0x6261726F,   //'baro'
		FavoriteItemsIcon              = 0x66617672,   //'favr'
		ForwardArrowIcon               = 0x6661726F,   //'faro'
		GridIcon                       = 0x67726964,   //'grid'
		HelpIcon                       = 0x68656C70,   //'help'
		KeepArrangedIcon               = 0x61726E67,   //'arng'
		LockedIcon                     = 0x6C6F636B,   //'lock'
		NoFilesIcon                    = 0x6E66696C,   //'nfil'
		NoFolderIcon                   = 0x6E666C64,   //'nfld'
		NoWriteIcon                    = 0x6E777274,   //'nwrt'
		ProtectedApplicationFolderIcon = 0x70617070,   //'papp'
		ProtectedSystemFolderIcon      = 0x70737973,   //'psys'
		RecentItemsIcon                = 0x72636E74,   //'rcnt'
		ShortcutIcon                   = 0x73687274,   //'shrt'
		SortAscendingIcon              = 0x61736E64,   //'asnd'
		SortDescendingIcon             = 0x64736E64,   //'dsnd'
		UnlockedIcon                   = 0x756C636B,   //'ulck'
		ConnectToIcon                  = 0x636E6374,   //'cnct'
		GenericWindowIcon              = 0x6777696E,   //'gwin'
		QuestionMarkIcon               = 0x71756573,   //'ques'
		DeleteAliasIcon                = 0x64616C69,   //'dali'
		EjectMediaIcon                 = 0x656A6563,   //'ejec'
		BurningIcon                    = 0x6275726E,   //'burn'
		RightContainerArrowIcon        = 0x72636172,   //'rcar'
	}
	
	// These constants specify the possible states of a drawer.
#if NET
	[UnsupportedOSPlatform ("macos10.13")]
#if MONOMAC
	[Obsolete ("Starting with macos10.13 use 'NSSplitViewController' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSSplitViewController' instead.")]
#endif
	[Native]
	public enum NSDrawerState : ulong {
		Closed = 0,
		Opening = 1,
		Open = 2,
		Closing = 3
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSWindowLevel : long {
		Normal = 0,
#if NET
		[UnsupportedOSPlatform ("macos10.13")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 13)]
#endif
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
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSRuleEditorRowType : ulong {
		Simple = 0,
		Compound
	}
   
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSRuleEditorNestingMode : ulong {
		Single,
		List,
		Compound,
		Simple
	}

#if NET
	[UnsupportedOSPlatform ("macos10.11")]
#if MONOMAC
	[Obsolete ("Starting with macos10.11 use 'NSGlyphProperty' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'NSGlyphProperty' instead.")]
#endif
	[Native]
	public enum NSGlyphInscription : ulong {
		Base, Below, Above, Overstrike, OverBelow
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTypesetterBehavior : long {
		Latest = -1,
		Original = 0,
		Specific_10_2_WithCompatibility = 1,
		Specific_10_2 = 2,
		Specific_10_3 = 3,
		Specific_10_4 = 4,
			
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSRemoteNotificationType : ulong {
		None = 0,
		Badge = 1 << 0,
		Sound = 1 << 1,
		Alert = 1 << 2
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSScrollViewFindBarPosition : long {
		AboveHorizontalRuler = 0,
		AboveContent,
		BelowContent
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSScrollerStyle : long {
   		Legacy = 0,
		Overlay
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum  NSScrollElasticity : long {
		Automatic = 0,
   		None,
		Allowed
	}
	
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum  NSScrollerKnobStyle : long {
		Default  = 0,
		Dark     = 1,
		Light    = 2
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSEventPhase : ulong {
		None,
		Began = 1,
		Stationary = 2,
		Changed = 4,
		Ended = 8,
		Cancelled = 16,
		MayBegin = 32
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSEventSwipeTrackingOptions : ulong {
		LockDirection = 1,
		ClampGestureAmount = 2
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSEventGestureAxis : long {
		None, Horizontal, Vertical
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSLayoutConstraintOrientation : long {
		Horizontal, Vertical
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	public enum NSLayoutPriority : int /*float*/ {
		Required = 1000,
		DefaultHigh = 750,
		DragThatCanResizeWindow = 510,
		WindowSizeStayPut = 500,
		DragThatCannotResizeWindow = 490,
		DefaultLow = 250,
		FittingSizeCompression = 50,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPopoverAppearance : long {
		Minimal, HUD
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPopoverBehavior : long {
		ApplicationDefined, Transient, Semitransient
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTableViewRowSizeStyle : long {
		Default = -1,
		Custom = 0,
		Small, Medium, Large
	}

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSTableRowActionEdge : long
	{
		Leading,
		Trailing
	}

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSTableViewRowActionStyle : long
	{
		Regular,
		Destructive
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSTableViewAnimation : ulong {
		None, Fade = 1, Gap = 2,
		SlideUp = 0x10, SlideDown = 0x20, SlideLeft = 0x30, SlideRight = 0x40
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSDraggingItemEnumerationOptions : ulong {
		Concurrent = 1 << 0,
		ClearNonenumeratedImages = 1 << 16
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSDraggingFormation : long {
		Default, None, Pile, List, Stack
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSDraggingContext : long {
		OutsideApplication, WithinApplication
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSWindowAnimationBehavior : long {
		Default = 0, None = 2, DocumentWindow, UtilityWindow, AlertPanel
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSTextFinderAction : long {
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

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSFontPanelMode : ulong {
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
		AllModesMask = unchecked ((ulong)UInt32.MaxValue)
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSFontCollectionVisibility : ulong {
		Process = 1 << 0,
		User = 1 << 1,
		Computer = 1 << 2,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSSharingContentScope : long {
		Item,
		Partial,
		Full
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSTypesetterControlCharacterAction : ulong {
		ZeroAdvancement = 1 << 0,
		Whitespace = 1 << 1,
		HorizontalTab = 1 << 2,
		LineBreak = 1 << 3,
		ParagraphBreak = 1 << 4,
		ContainerBreak = 1 << 5,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSPageControllerTransitionStyle : long {
		StackHistory,
		StackBook,
		HorizontalStrip
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSWindowTitleVisibility : long {
		Visible = 0,
		Hidden = 1,
#if !XAMCORE_4_0
		[Obsolete ("This API is not available on this platform.")]
		HiddenWhenActive = 2,
#endif
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSViewControllerTransitionOptions : ulong {
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

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSApplicationOcclusionState  : ulong {
		Visible = 1 << 1
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSWindowOcclusionState  : ulong {
		Visible = 1 << 1
	}

	
	
#region NSVisualEffectView
#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSVisualEffectMaterial : long {
		[Advice ("Use a specific material instead.")]
		AppearanceBased,
		[Advice ("Use a semantic material instead.")]
		Light,
		[Advice ("Use a semantic material instead.")]
		Dark,
		Titlebar,
		Selection,
#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,11)]
#endif
		Menu,
#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,11)]
#endif
		Popover,
#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,11)]
#endif
		Sidebar,
#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,11)]
#endif
		[Advice ("Use a semantic material instead.")]
		MediumLight,
#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,11)]
#endif
		[Advice ("Use a semantic material instead.")]
		UltraDark,
#if NET
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,14)]
#endif
		HeaderView = 10,
#if NET
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,14)]
#endif
		Sheet = 11,
#if NET
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,14)]
#endif
		WindowBackground = 12,
#if NET
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,14)]
#endif
		HudWindow = 13,
#if NET
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,14)]
#endif
		FullScreenUI = 15,
#if NET
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,14)]
#endif
		ToolTip = 17,
#if NET
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,14)]
#endif
		ContentBackground = 18,
#if NET
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,14)]
#endif
		UnderWindowBackground = 21,
#if NET
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,14)]
#endif
		UnderPageBackground = 22,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSVisualEffectBlendingMode : long {
		BehindWindow,
		WithinWindow
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Native]
	public enum NSVisualEffectState : long {
		FollowsWindowActiveState,
		Active,
		Inactive
	}
#endregion

#if NET
	[SupportedOSPlatform ("macos10.10.3")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,10,3)]
#endif
	[Native]
	public enum NSPressureBehavior : long
	{
		Unknown = -1,
		PrimaryDefault = 0,
		PrimaryClick = 1,
		PrimaryGeneric = 2,
		PrimaryAccelerator = 3,
		PrimaryDeepClick = 5,
		PrimaryDeepDrag = 6
	}

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSHapticFeedbackPattern : long
	{
		Generic = 0,
		Alignment,
		LevelChange
	}

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSHapticFeedbackPerformanceTime : ulong
	{
		Default = 0,
		Now,
		DrawCompleted
	}

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,11)]
#endif
	[Native]
	public enum NSSpringLoadingHighlight : long
	{
		None = 0,
		Standard,
		Emphasized
	}

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,11)]
#endif
	[Flags]
	[Native]
	public enum NSSpringLoadingOptions : ulong
	{
		Disabled = 0,
		Enabled = 1 << 0,
		ContinuousActivation = 1 << 1,
		NoHover = 1 << 3
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,12)]
#endif
	[Flags]
	[Native]
	public enum NSWindowListOptions : long {
		OrderedFrontToBack = (1 << 0)
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,12)]
#endif
	[Native]
	public enum NSStatusItemBehavior : ulong
	{
		RemovalAllowed = (1 << 1),
		TerminationOnRemoval = (1 << 2)
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,12)]
#endif
	[Native]
	public enum NSWindowTabbingMode : long
	{
		Automatic,
		Preferred,
		Disallowed
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,12)]
#endif
	[Native]
	public enum NSWindowUserTabbingPreference : long
	{
		Manual,
		Always,
		InFullScreen
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10, 12)]
#endif
	[Native]
	public enum NSGridCellPlacement : long
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

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10, 12)]
#endif
	[Native]
	public enum NSGridRowAlignment : long
	{
		Inherited = 0,
		None,
		FirstBaseline,
		LastBaseline
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10, 12)]
#endif
	[Native]
	public enum NSImageLayoutDirection : long
	{
		Unspecified = -1,
		LeftToRight = 2,
		RightToLeft = 3
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10, 12)]
#endif
	[Native]
	[Flags]
	public enum NSCloudKitSharingServiceOptions : ulong
	{
		Standard = 0,
		AllowPublic = 1 << 0,
		AllowPrivate = 1 << 1,
		AllowReadOnly = 1 << 4,
		AllowReadWrite = 1 << 5
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10, 12)]
#endif
	[Native]
	public enum NSDisplayGamut : long
	{
		Srgb = 1,
		P3,
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10, 12)]
#endif
	[Native]
	public enum NSTabPosition : ulong
	{
		None = 0,
		Top,
		Left,
		Bottom,
		Right,
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10, 12)]
#endif
	[Native]
	public enum NSTabViewBorderType : ulong
	{
		None = 0,
		Line,
		Bezel,
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10, 12)]
#endif
	[Native]
	public enum NSPasteboardContentsOptions : ulong
	{
		CurrentHostOnly = 1,
	}

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,12,2)]
#endif
	[Native]
	public enum NSTouchType : long
	{
		Direct,
		Indirect
	}

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,12,2)]
#endif
	[Native]
	[Flags]
	public enum NSTouchTypeMask : ulong
	{
		Direct = (1 << (int)NSTouchType.Direct),
		Indirect = (1 << (int)NSTouchType.Indirect)
	}

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,12,2)]
#endif
	[Native]
	public enum NSScrubberMode : long
	{
		Fixed = 0,
		Free
	}

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,12,2)]
#endif
	[Native]
	public enum NSScrubberAlignment : long
	{
		None = 0,
		Leading,
		Trailing,
		Center
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,13)]
#endif
	public enum NSFontError : int {
		AssetDownloadError = 66304,
		ErrorMinimum = 66304,
		ErrorMaximum = 66335,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,13)]
#endif
	[Native]
	public enum NSAccessibilityAnnotationPosition : long {
		FullRange,
		Start,
		End,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,13)]
#endif
	[Native]
	public enum NSAccessibilityCustomRotorSearchDirection : long {
		Previous,
		Next,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,13)]
#endif
	[Native]
	public enum NSAccessibilityCustomRotorType : long {
		Custom = 0,
		Any = 1,
		Annotation,
		BoldText,
		Heading,
		HeadingLevel1,
		HeadingLevel2,
		HeadingLevel3,
		HeadingLevel4,
		HeadingLevel5,
		HeadingLevel6,
		Image,
		ItalicText,
		Landmark,
		Link,
		List,
		MisspelledWord,
		Table,
		TextField,
		UnderlinedText,
		VisitedLink,
		Audiograph,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10, 13)]
#endif
	[Native]
	public enum NSColorType : long {
		ComponentBased,
		Pattern,
		Catalog,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,13)]
#endif
	[Native]
	[Flags]
	public enum NSFontAssetRequestOptions : ulong {
		UsesStandardUI = 1 << 0,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,13)]
#endif
	[Native]
	[Flags]
	public enum NSFontPanelModeMask : ulong {
		Face = 1 << 0,
		Size = 1 << 1,
		Collection = 1 << 2,
		UnderlineEffect = 1 << 8,
		StrikethroughEffect = 1 << 9,
		TextColorEffect = 1 << 10,
		DocumentColorEffect = 1 << 11,
		ShadowEffect = 1 << 12,
		AllEffects = (ulong)0XFFF00,
		StandardModes = (ulong)0XFFFF,
		AllModes = (ulong)0XFFFFFFFF,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,13)]
#endif
	[Native]
	public enum NSLevelIndicatorPlaceholderVisibility : long {
		Automatic = 0,
		Always = 1,
		WhileEditing = 2,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,13)]
#endif
	[Native]
	public enum NSSegmentDistribution : long {
		Fit = 0,
		Fill,
		FillEqually,
		FillProportionally,
	}

#if NET
	[SupportedOSPlatform ("macos10.14")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,14)]
#endif
	[Native]
	public enum NSColorSystemEffect : long {
		None,
		Pressed,
		DeepPressed,
		Disabled,
		Rollover,
	}

#if NET
	[SupportedOSPlatform ("macos10.14")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (10,14)]
#endif
	[Native]
	public enum NSWorkspaceAuthorizationType : long  {
		CreateSymbolicLink,
		SetAttributes,
		ReplaceFile,
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (11,0)]
#endif
	[Native]
	public enum NSTableViewStyle : long
	{
		Automatic,
		FullWidth,
		Inset,
		SourceList,
		Plain,
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (11,0)]
#endif
	[Native]
	public enum NSTitlebarSeparatorStyle : long
	{
		Automatic,
		None,
		Line,
		Shadow,
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (11,0)]
#endif
	[Native]
	public enum NSWindowToolbarStyle : long
	{
		Automatic,
		Expanded,
		Preference,
		Unified,
		UnifiedCompact,
	}

#if NET
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
#endif
	[Flags]
	[Native]
	public enum NSTableViewAnimationOptions : ulong
	{
		EffectNone = 0x0,
		EffectFade = 0x1,
		EffectGap = 0x2,
		SlideUp = 0x10,
		SlideDown = 0x20,
		SlideLeft = 0x30,
		SlideRight = 0x40,
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoMacCatalyst]
	[Mac (11,0)]
#endif
	[Native]
	public enum NSImageSymbolScale : long
	{
		Small = 1,
		Medium = 2,
		Large = 3,
	}
}
