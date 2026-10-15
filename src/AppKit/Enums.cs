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
using System.ComponentModel;

#nullable enable

namespace AppKit {

	/// <summary>Specifies a return response from an application modal session.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSRunResponse : long {
		/// <summary>Indicates that the modal session stopped.</summary>
		Stopped = -1000,
		/// <summary>Indicates that the modal session was aborted.</summary>
		Aborted = -1001,
		/// <summary>Indicates that the modal session should continue.</summary>
		Continues = -1002,
	}

	/// <summary>Specifies options for activating an application.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSApplicationActivationOptions : ulong {
		/// <summary>Activates the application using the default behavior.</summary>
		Default = 0,
		/// <summary>Brings all the application's windows forward.</summary>
		ActivateAllWindows = 1,
		/// <summary>Activates the application regardless of which application is currently active.</summary>
		ActivateIgnoringOtherWindows = 2,
	}

	/// <summary>Specifies how an application participates in the user interface.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSApplicationActivationPolicy : long {
		/// <summary>The application appears in the Dock and can have a menu bar.</summary>
		Regular,
		/// <summary>The application does not appear in the Dock, but can present a user interface.</summary>
		Accessory,
		/// <summary>The application does not appear in the Dock and cannot create windows or be activated.</summary>
		Prohibited,
	}

	/// <summary>Specifies options that control the visibility of system UI, such as the Dock and menu bar, while the application is active.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSApplicationPresentationOptions : ulong {
		/// <summary>No special presentation options; the Dock and menu bar behave normally.</summary>
		Default = 0,
		/// <summary>The Dock is hidden and appears only when the pointer moves to its screen edge.</summary>
		AutoHideDock = (1 << 0),
		/// <summary>The Dock is entirely unavailable to the user.</summary>
		HideDock = (1 << 1),

		/// <summary>The menu bar is hidden and appears only when the pointer moves to the top of the screen.</summary>
		AutoHideMenuBar = (1 << 2),
		/// <summary>The menu bar is entirely unavailable to the user.</summary>
		HideMenuBar = (1 << 3),

		/// <summary>All Apple menu items are disabled.</summary>
		DisableAppleMenu = (1 << 4),
		/// <summary>The process switching user interface (such as Command-Tab) is disabled.</summary>
		DisableProcessSwitching = (1 << 5),
		/// <summary>The Force Quit panel is disabled.</summary>
		DisableForceQuit = (1 << 6),
		/// <summary>Logging out and shutting down are disabled.</summary>
		DisableSessionTermination = (1 << 7),
		/// <summary>The "Hide" and "Hide Others" menu commands, as well as their keyboard shortcuts, are disabled.</summary>
		DisableHideApplication = (1 << 8),
		/// <summary>The menu bar does not display its usual translucent background.</summary>
		DisableMenuBarTransparency = (1 << 9),

		/// <summary>The application's windows are displayed in full-screen mode, hiding the Dock and menu bar.</summary>
		FullScreen = (1 << 10),
		/// <summary>The toolbar is hidden and shown automatically together with the menu bar.</summary>
		AutoHideToolbar = (1 << 11),
		/// <summary>The floating overlay that assists in locating the pointer is disabled.</summary>
		DisableCursorLocationAssistance = (1 << 12),
		[Mac (27, 0)]
		DisableScreenCornerInteractions = (1 << 15),
	}

	/// <summary>Specifies the result of an application delegate operation.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSApplicationDelegateReply : ulong {
		/// <summary>The operation completed successfully.</summary>
		Success,
		/// <summary>The operation was canceled.</summary>
		Cancel,
		/// <summary>The operation failed.</summary>
		Failure,
	}

	/// <summary>Specifies the urgency of a request for the user's attention.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSRequestUserAttentionType : ulong {
		/// <summary>Requests the user's attention for a critical event.</summary>
		CriticalRequest = 0,
		/// <summary>Requests the user's attention for an informational event.</summary>
		InformationalRequest = 10,
	}

	/// <summary>Specifies how an application responds to a termination request.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSApplicationTerminateReply : ulong {
		/// <summary>Cancels application termination.</summary>
		Cancel,
		/// <summary>Terminates the application immediately.</summary>
		Now,
		/// <summary>Defers the termination decision.</summary>
		Later,
	}

	/// <summary>Specifies the result of an application print request.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSApplicationPrintReply : ulong {
		/// <summary>The print request was canceled.</summary>
		Cancelled,
		/// <summary>The print request completed successfully.</summary>
		Success,
		/// <summary>The print request failed.</summary>
		Failure,
		/// <summary>The application will reply after completing the print request asynchronously.</summary>
		ReplyLater,
	}

	/// <summary>Specifies the level of interpolation quality used when an image is scaled or transformed.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageInterpolation : ulong {
		/// <summary>Uses the interpolation quality most appropriate for the current context.</summary>
		Default,
		/// <summary>Performs no interpolation; the image is drawn using nearest-neighbor sampling.</summary>
		None,
		/// <summary>Uses a fast, low-quality interpolation algorithm.</summary>
		Low,
		/// <summary>Uses an interpolation algorithm that balances speed and quality.</summary>
		Medium,
		/// <summary>Uses the slowest, highest-quality interpolation algorithm available.</summary>
		High,
	}

	/// <summary>Specifies a Porter-Duff compositing operator, or a Core Image-style blend mode, used to combine a source image with a destination image.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSComposite : ulong {
		/// <summary>The result is transparent black; neither the source nor the destination contributes.</summary>
		Clear,
		/// <summary>The source image replaces the destination image.</summary>
		Copy,
		/// <summary>The source image is composited over the destination image.</summary>
		SourceOver,
		/// <summary>The part of the source image that lies within the destination image replaces the destination; everything else is discarded.</summary>
		SourceIn,
		/// <summary>The part of the source image that lies outside the destination image is displayed; everything else is discarded.</summary>
		SourceOut,
		/// <summary>The source image is composited over the destination image, but only the part of the source that lies within the destination is displayed.</summary>
		SourceAtop,
		/// <summary>The destination image is composited over the source image.</summary>
		DestinationOver,
		/// <summary>The part of the destination image that lies within the source image replaces the destination; everything else is discarded.</summary>
		DestinationIn,
		/// <summary>The part of the destination image that lies outside the source image is displayed; everything else is discarded.</summary>
		DestinationOut,
		/// <summary>The destination image is composited over the source image, but only the part of the destination that lies within the source is displayed.</summary>
		DestinationAtop,
		/// <summary>The parts of the source and destination images that do not overlap are displayed; the overlapping region is discarded.</summary>
		XOR,
		/// <summary>The source and destination color values are summed and the result approaches 0 (black) as the limit.</summary>
		PlusDarker,
		/// <summary>The source image is composited over the destination image using the same behavior as <see cref="F:AppKit.NSComposite.SourceOver" />.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use NSCompositeSourceOver instead.")]
		Highlight,
		/// <summary>The source and destination color values are summed and the result approaches 1 (white) as the limit.</summary>
		PlusLighter,
		/// <summary>The source and destination color values are multiplied together, always producing a darker or equal color.</summary>
		Multiply,
		/// <summary>The source and destination color values are inverted, multiplied, and inverted again, always producing a lighter or equal color.</summary>
		Screen,
		/// <summary>Multiplies or screens the color values, depending on the destination color, preserving highlights and shadows.</summary>
		Overlay,
		/// <summary>The darker of the source and destination color values is used at each pixel.</summary>
		Darken,
		/// <summary>The lighter of the source and destination color values is used at each pixel.</summary>
		Lighten,
		/// <summary>The destination color is brightened to reflect the source color, increasing the contrast between the two.</summary>
		ColorDodge,
		/// <summary>The destination color is darkened to reflect the source color, increasing the contrast between the two.</summary>
		ColorBurn,
		/// <summary>Darkens or lightens the colors, depending on the source color value, producing an effect similar to shining a diffuse spotlight on the destination.</summary>
		SoftLight,
		/// <summary>Multiplies or screens the color values, depending on the source color, producing an effect similar to shining a harsh spotlight on the destination.</summary>
		HardLight,
		/// <summary>Subtracts either the source color from the destination color or vice versa, whichever produces a positive value, producing an inversion effect.</summary>
		Difference,
		/// <summary>Produces an effect similar to <see cref="F:AppKit.NSComposite.Difference" /> but with lower contrast.</summary>
		Exclusion,
		/// <summary>Uses the luminance and saturation of the destination and the hue of the source.</summary>
		Hue,
		/// <summary>Uses the luminance and hue of the destination and the saturation of the source.</summary>
		Saturation,
		/// <summary>Uses the luminance of the destination and the hue and saturation of the source.</summary>
		Color,
		/// <summary>Uses the hue and saturation of the destination and the luminance of the source.</summary>
		Luminosity,
	}

	/// <summary>Specifies how a window's drawing is buffered before being flushed to the screen.</summary>
	[NoMacCatalyst]
	[NativeName ("NSBackingStoreType")]
	[Native]
	public enum NSBackingStore : ulong {
		/// <summary>The window renders visible portions directly to the screen and retains obscured portions in a buffer.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'Buffered' instead.")]
		Retained,
		/// <summary>The window renders directly into display memory, without retaining its contents.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'Buffered' instead.")]
		Nonretained,
		/// <summary>The window renders into an off-screen buffer, which is then flushed to the display in a single operation.</summary>
		Buffered,
	}

	/// <summary>Specifies how a window is ordered relative to other windows.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowOrderingMode : long {
		/// <summary>Orders the window below another window.</summary>
		Below = -1,
		/// <summary>Removes the window from the screen.</summary>
		Out,
		/// <summary>Orders the window above another window.</summary>
		Above,
	}

	/// <summary>Specifies where a focus ring is drawn relative to content.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSFocusRingPlacement : ulong {
		/// <summary>Draws only the focus ring.</summary>
		RingOnly,
		/// <summary>Draws the focus ring below the content.</summary>
		RingBelow,
		/// <summary>Draws the focus ring above the content.</summary>
		RingAbove,
	}

	/// <summary>Specifies the focus ring displayed by a control.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSFocusRingType : ulong {
		/// <summary>Uses the default focus ring.</summary>
		Default,
		/// <summary>Does not display a focus ring.</summary>
		None,
		/// <summary>Displays a focus ring outside the control.</summary>
		Exterior,
	}

	/// <summary>Specifies how out-of-gamut colors are mapped to the destination color space.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSColorRenderingIntent : long {
		/// <summary>Uses the rendering intent embedded in the source profile, or <see cref="F:AppKit.NSColorRenderingIntent.Perceptual" /> if none is specified.</summary>
		Default,
		/// <summary>Preserves the exact color values that fall within both color spaces' gamuts and clips out-of-gamut colors to the nearest reproducible color.</summary>
		AbsoluteColorimetric,
		/// <summary>Preserves the exact color values that fall within both color spaces' gamuts, and rescales white points to compensate for differing white points between the color spaces.</summary>
		RelativeColorimetric,
		/// <summary>Compresses the entire source gamut into the destination gamut, preserving the overall visual relationship between colors.</summary>
		Perceptual,
		/// <summary>Preserves the relative saturation of colors, sacrificing color accuracy to maintain vivid colors.</summary>
		Saturation,

	}

	/// <summary>Specifies one of the four edges of a rectangle.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSRectEdge : ulong {
		/// <summary>The minimum edge on the x-axis (the left edge).</summary>
		MinXEdge,
		/// <summary>The minimum edge on the y-axis (the bottom edge, in a non-flipped coordinate system).</summary>
		MinYEdge,
		/// <summary>The maximum edge on the x-axis (the right edge).</summary>
		MaxXEdge,
		/// <summary>The maximum edge on the y-axis (the top edge, in a non-flipped coordinate system).</summary>
		MaxYEdge,
	}

	/// <summary>Specifies the layout direction of the user interface.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSUserInterfaceLayoutDirection : long {
		/// <summary>Arranges interface elements from left to right.</summary>
		LeftToRight,
		/// <summary>Arranges interface elements from right to left.</summary>
		RightToLeft,
	}

	#region NSColorSpace
	/// <summary>Specifies the fundamental type of color space, describing the number and meaning of its color components.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSColorSpaceModel : long {
		/// <summary>The color space model is not known.</summary>
		Unknown = -1,
		/// <summary>A grayscale color space, with a single component representing intensity.</summary>
		Gray,
		/// <summary>A red-green-blue color space.</summary>
		RGB,
		/// <summary>A cyan-magenta-yellow-black color space.</summary>
		CMYK,
		/// <summary>A CIE L*a*b* color space.</summary>
		LAB,
		/// <summary>A color space with an arbitrary number of color components, such as those used by spot-color or multi-ink printing.</summary>
		DeviceN,
		/// <summary>A color space whose colors are looked up by index in a fixed palette.</summary>
		Indexed,
		/// <summary>A color space whose colors are defined by a repeating pattern image.</summary>
		Pattern,
	}
	#endregion

	#region NSFileWrapper
	#endregion

	#region NSParagraphStyle
	/// <summary>Specifies the alignment of text at a tab stop.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextTabType : ulong {
		/// <summary>Left-aligns text at the tab stop.</summary>
		Left,
		/// <summary>Right-aligns text at the tab stop.</summary>
		Right,
		/// <summary>Centers text at the tab stop.</summary>
		Center,
		/// <summary>Aligns decimal characters at the tab stop.</summary>
		Decimal,
	}

	/// <summary>Specifies how a line of text is wrapped or truncated when it does not fit within its container.</summary>
	[Native]
	[NoMacCatalyst]
	public enum NSLineBreakMode : ulong {
		/// <summary>Wraps lines at word boundaries, splitting a word only when it cannot fit on a single line.</summary>
		ByWordWrapping,
		/// <summary>Wraps lines at character boundaries, splitting words if necessary.</summary>
		CharWrapping,
		/// <summary>Clips text that does not fit, without wrapping to a new line.</summary>
		Clipping,
		/// <summary>Truncates the beginning of the line, inserting an ellipsis, so the end of the text remains visible.</summary>
		TruncatingHead,
		/// <summary>Truncates the end of the line, inserting an ellipsis, so the beginning of the text remains visible.</summary>
		TruncatingTail,
		/// <summary>Truncates the middle of the line, inserting an ellipsis, so both the beginning and end of the text remain visible.</summary>
		TruncatingMiddle,
	}

	#endregion

	#region NSCell Defines 

	/// <summary>Specifies the type of content displayed by a cell.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSCellType : ulong {
		/// <summary>The cell has no content type.</summary>
		Null,
		/// <summary>The cell displays text.</summary>
		Text,
		/// <summary>The cell displays an image.</summary>
		Image,
	}

	/// <summary>Specifies a generic attribute of a cell that can be queried or modified using the legacy <c>GetCellAttribute</c> and <c>SetCellAttribute</c> APIs.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSCellAttribute : ulong {
		/// <summary>The cell does not respond to mouse events or other user actions.</summary>
		CellDisabled,
		/// <summary>The current on/off/mixed state of the cell.</summary>
		CellState,
		/// <summary>The cell is drawn with a pushed-in (pressed) appearance.</summary>
		PushInCell,
		/// <summary>The cell's contents can be edited by the user.</summary>
		CellEditable,
		/// <summary>The cell darkens its image when its state is on or mixed.</summary>
		ChangeGrayCell,
		/// <summary>The cell is currently highlighted.</summary>
		CellHighlighted,
		/// <summary>The cell indicates its highlighted state by changing its contents rather than its background.</summary>
		CellLightsByContents,
		/// <summary>The cell indicates its highlighted state by changing to a grayed appearance.</summary>
		CellLightsByGray,
		/// <summary>The cell changes its background when its state is on or mixed.</summary>
		ChangeBackgroundCell,
		/// <summary>The cell indicates its highlighted state by lightening or darkening its background.</summary>
		CellLightsByBackground,
		/// <summary>The cell draws a border around itself.</summary>
		CellIsBordered,
		/// <summary>The cell's image is drawn overlapping its title text.</summary>
		CellHasOverlappingImage,
		/// <summary>The cell's image and title are arranged horizontally, side by side.</summary>
		CellHasImageHorizontal,
		/// <summary>The cell's image is positioned to the left of, or below, its title.</summary>
		CellHasImageOnLeftOrBottom,
		/// <summary>The cell's displayed contents change to reflect its current state.</summary>
		CellChangesContents,
		/// <summary>The degree to which the cell's bezel is inset, expressed as an integer level.</summary>
		CellIsInsetButton,
		/// <summary>The cell supports a third, mixed state in addition to its on and off states.</summary>
		CellAllowsMixedState,
	}

	/// <summary>Specifies how an image and title text are arranged within a cell.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSCellImagePosition : ulong {
		/// <summary>The cell displays no image.</summary>
		NoImage,
		/// <summary>The cell displays only its image; any title is not shown.</summary>
		ImageOnly,
		/// <summary>The image appears to the left of the title.</summary>
		ImageLeft,
		/// <summary>The image appears to the right of the title.</summary>
		ImageRight,
		/// <summary>The image appears below the title.</summary>
		ImageBelow,
		/// <summary>The image appears above the title.</summary>
		ImageAbove,
		/// <summary>The image is drawn directly on top of the title.</summary>
		ImageOverlaps,
		/// <summary>The image appears at the leading edge of the title, honoring the current layout direction.</summary>
		ImageLeading,
		/// <summary>The image appears at the trailing edge of the title, honoring the current layout direction.</summary>
		ImageTrailing,
	}

	/// <summary>Specifies how an image is scaled to fit the space allotted to it.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageScale : ulong {
		/// <summary>Scales the image proportionally down to fit the space, but never scales it up.</summary>
		ProportionallyDown = 0,
		/// <summary>Scales each dimension of the image independently, without regard to the image's original aspect ratio, so it fills the space exactly.</summary>
		AxesIndependently,
		/// <summary>Does not scale the image.</summary>
		None,
		/// <summary>Scales the image proportionally, either up or down, to fit the space as closely as possible.</summary>
		ProportionallyUpOrDown,
	}

	/// <summary>Specifies the state of a cell.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSCellStateValue : long {
		/// <summary>The cell has a mixed state.</summary>
		Mixed = -1,
		/// <summary>The cell is off.</summary>
		Off,
		/// <summary>The cell is on.</summary>
		On,
	}

	/// <summary>Specifies legacy style-mask flags that describe the appearance and behavior of a cell.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSCellStyleMask : ulong {
		/// <summary>No style flags are set.</summary>
		NoCell = 0,
		/// <summary>The button cell displays its alternate image, title, or both.</summary>
		ContentsCell = 1 << 0,
		/// <summary>The cell is drawn with a pushed-in (pressed) appearance.</summary>
		PushInCell = 1 << 1,
		/// <summary>The cell darkens its image when its state is on or mixed.</summary>
		ChangeGrayCell = 1 << 2,
		/// <summary>The cell changes its background when its state is on or mixed.</summary>
		ChangeBackgroundCell = 1 << 3,
	}

	/// <summary>Specifies which part of a cell, if any, a hit-test point falls within.</summary>
	[NoMacCatalyst]
	[Flags]
	[NativeName ("NSCellHitResult")]
	[Native]
	public enum NSCellHit : ulong {
		/// <summary>The point does not fall within any part of the cell.</summary>
		None,
		/// <summary>The point falls within the cell's content area.</summary>
		ContentArea = 1,
		/// <summary>The point falls within an editable and selectable text area of the cell.</summary>
		EditableTextArea = 2,
		/// <summary>The point falls within a trackable area of the cell, such as one that responds to mouse tracking.</summary>
		TrackableArea = 4,
#if !XAMCORE_5_0
		/// <summary>Obsolete alias kept for backward compatibility. Use <see cref="F:AppKit.NSCellHit.TrackableArea" /> instead.</summary>
		[Obsolete ("Use 'TrackableArea' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		TrackableArae = TrackableArea,
#endif
	}

	/// <summary>Specifies the tint color applied to certain controls, following the user's system-wide color preference.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSControlTint : ulong {
		/// <summary>Uses the system's default control tint.</summary>
		Default = 0,    // system 'default'
		/// <summary>Uses a blue control tint.</summary>
		Blue = 1,
		/// <summary>Uses a graphite (gray) control tint.</summary>
		Graphite = 6,
		/// <summary>No control tint is applied.</summary>
		Clear = 7,
	}

	/// <summary>Specifies the physical size at which a control is drawn.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSControlSize : ulong {
		/// <summary>The standard, full-size control.</summary>
		Regular = 0,
		/// <summary>A smaller variant of the control.</summary>
		Small = 1,
		/// <summary>The smallest variant of the control.</summary>
		Mini = 2,
		/// <summary>A control size that is larger than <see cref="F:AppKit.NSControlSize.Regular" />.</summary>
		Large = 3,
		/// <summary>A control size that is larger than <see cref="F:AppKit.NSControlSize.Large" />.</summary>
		[Mac (26, 0)]
		ExtraLarge = 4,
	}

	/// <summary>Specifies the visual context in which content, such as a cell's text or image, is being drawn, so it can adjust its appearance for legibility.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSBackgroundStyle : long {
		/// <summary>The content is drawn against a normal, unemphasized background.</summary>
		Normal = 0,
		/// <summary>The content is drawn against a light background.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Normal' instead.")]
		Light = Normal,
		/// <summary>The content is drawn against a dark, selected, or otherwise emphasized background, and should adjust its appearance for contrast.</summary>
		Emphasized,
		/// <summary>The content is drawn against a dark background.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Emphasized' instead.")]
		Dark = Emphasized,
		/// <summary>The content is drawn on a raised, embossed surface.</summary>
		Raised,
		/// <summary>The content is drawn on a lowered, engraved surface.</summary>
		Lowered,
	}
	#endregion

	#region NSImage

	/// <summary>Specifies the outcome of an incremental image-loading operation.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageLoadStatus : ulong {
		/// <summary>The image finished loading successfully.</summary>
		Completed,
		/// <summary>The image load operation was canceled.</summary>
		Cancelled,
		/// <summary>The image data was invalid or malformed.</summary>
		InvalidData,
		/// <summary>The image data ended unexpectedly before loading completed.</summary>
		UnexpectedEOF,
		/// <summary>An error occurred while reading the image data.</summary>
		ReadError,
	}

	/// <summary>Specifies when an <see cref="T:AppKit.NSImageRep" /> caches its rendered output as a bitmap.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageCacheMode : ulong {
		/// <summary>Lets the image decide the appropriate caching behavior.</summary>
		Default,
		/// <summary>Always caches the rendered image.</summary>
		Always,
		/// <summary>Caches the rendered image only when the cache would be smaller than the original image data.</summary>
		BySize,
		/// <summary>Never caches the rendered image; it is always redrawn from its source representation.</summary>
		Never,
	}

	/// <summary>Specifies how an image is resized to fill its destination.</summary>
	[NoMacCatalyst]
	[Native (ConvertToNative = "NSImageResizingModeExtensions.ToNative", ConvertToManaged = "NSImageResizingModeExtensions.ToManaged")]
	public enum NSImageResizingMode : long {
		/// <summary>Stretches the image to fill the destination.</summary>
		Stretch,
		/// <summary>Tiles the image to fill the destination.</summary>
		Tile,
	}

	#endregion

	#region NSAlert
	/// <summary>Specifies the visual style of an alert.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSAlertStyle : ulong {
		/// <summary>Displays a warning alert.</summary>
		Warning,
		/// <summary>Displays an informational alert.</summary>
		Informational,
		/// <summary>Displays an alert for a critical condition.</summary>
		Critical,
	}

	/// <summary>Specifies a response returned by a modal session.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSModalResponse : long {
		/// <summary>Indicates that the user accepted the modal session.</summary>
		OK = 1,
		/// <summary>Indicates that the user canceled the modal session.</summary>
		Cancel = 0,
		/// <summary>Indicates that the modal session should stop.</summary>
		Stop = -1000,
		/// <summary>Indicates that the modal session should abort.</summary>
		Abort = -1001,
		/// <summary>Indicates that the modal session should continue.</summary>
		Continue = -1002,
	}
	#endregion

	#region NSEvent
	/// <summary>Specifies the kind of event represented by an <see cref="T:AppKit.NSEvent" />, such as a mouse, keyboard, or gesture event.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSEventType : ulong {
		/// <summary>The left (primary) mouse button was pressed.</summary>
		LeftMouseDown = 1,
		/// <summary>The left (primary) mouse button was released.</summary>
		LeftMouseUp = 2,
		/// <summary>The right (secondary) mouse button was pressed.</summary>
		RightMouseDown = 3,
		/// <summary>The right (secondary) mouse button was released.</summary>
		RightMouseUp = 4,
		/// <summary>The mouse moved without any mouse button pressed.</summary>
		MouseMoved = 5,
		/// <summary>The mouse moved while the left mouse button was held down.</summary>
		LeftMouseDragged = 6,
		/// <summary>The mouse moved while the right mouse button was held down.</summary>
		RightMouseDragged = 7,
		/// <summary>The pointer entered a tracking rectangle or tracking area.</summary>
		MouseEntered = 8,
		/// <summary>The pointer exited a tracking rectangle or tracking area.</summary>
		MouseExited = 9,
		/// <summary>A key on the keyboard was pressed.</summary>
		KeyDown = 10,
		/// <summary>A key on the keyboard was released.</summary>
		KeyUp = 11,
		/// <summary>The state of one or more modifier keys (such as Shift, Control, or Command) changed.</summary>
		FlagsChanged = 12,
		/// <summary>An event generated internally by AppKit, not directly caused by user interaction.</summary>
		AppKitDefined = 13,
		/// <summary>An event generated internally by the system, not directly caused by user interaction.</summary>
		SystemDefined = 14,
		/// <summary>A custom event defined and posted by the application, typically used to wake up the run loop.</summary>
		ApplicationDefined = 15,
		/// <summary>A periodic event generated by a repeating timer, typically used during mouse-tracking loops.</summary>
		Periodic = 16,
		/// <summary>The pointer moved over a region whose cursor should be updated, such as when entering a cursor rectangle.</summary>
		CursorUpdate = 17,

		/// <summary>The scroll wheel, or a trackpad scrolling gesture, was moved.</summary>
		ScrollWheel = 22,

		/// <summary>A point event generated by a graphics tablet stylus.</summary>
		TabletPoint = 23,
		/// <summary>A proximity event indicating that a graphics tablet stylus entered or left proximity to the tablet.</summary>
		TabletProximity = 24,

		/// <summary>A mouse button other than the left or right button was pressed.</summary>
		OtherMouseDown = 25,
		/// <summary>A mouse button other than the left or right button was released.</summary>
		OtherMouseUp = 26,
		/// <summary>The mouse moved while a mouse button other than the left or right button was held down.</summary>
		OtherMouseDragged = 27,

		/// <summary>A general, non-specific touch or trackpad gesture event.</summary>
		Gesture = 29,
		/// <summary>A pinch-to-zoom (magnify) gesture on a trackpad.</summary>
		Magnify = 30,
		/// <summary>A swipe gesture on a trackpad.</summary>
		Swipe = 31,
		/// <summary>A rotation gesture on a trackpad.</summary>
		Rotate = 18,
		/// <summary>Indicates the start of a sequence of gesture-tracking events.</summary>
		BeginGesture = 19,
		/// <summary>Indicates the end of a sequence of gesture-tracking events.</summary>
		EndGesture = 20,

		/// <summary>A "smart zoom" gesture, such as a two-finger double-tap on a trackpad.</summary>
		SmartMagnify = 32,
		/// <summary>A Quick Look, or "look up", gesture, such as a force click on a trackpad.</summary>
		QuickLook = 33,
		/// <summary>A change in the amount of force applied by the user on a pressure-sensitive (Force Touch) trackpad.</summary>
		Pressure = 34, // 10.10.3, 64-bit-only
		/// <summary>A touch event received directly from a touch-capable surface, such as the Touch Bar.</summary>
		DirectTouch = 37, // 10.10
		/// <summary>Indicates a device mode-change request, such as an Apple Pencil double-tap while using an iPad as an additional display.</summary>
		ChangeMode = 38,
		/// <summary>Indicates that a mouse event in progress was cancelled.</summary>
		[Mac (26, 0)]
		MouseCancelled = 40,
	}

	/// <summary>Specifies a bitmask of one or more <see cref="T:AppKit.NSEventType" /> values, used to filter which kinds of events are retrieved or handled.</summary>
	[NoMacCatalyst]
	[Flags]
	public enum NSEventMask : ulong {
		/// <summary>Matches <see cref="F:AppKit.NSEventType.LeftMouseDown" /> events.</summary>
		LeftMouseDown = 1UL << (int) NSEventType.LeftMouseDown,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.LeftMouseUp" /> events.</summary>
		LeftMouseUp = 1UL << (int) NSEventType.LeftMouseUp,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.RightMouseDown" /> events.</summary>
		RightMouseDown = 1UL << (int) NSEventType.RightMouseDown,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.RightMouseUp" /> events.</summary>
		RightMouseUp = 1UL << (int) NSEventType.RightMouseUp,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.MouseMoved" /> events.</summary>
		MouseMoved = 1UL << (int) NSEventType.MouseMoved,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.LeftMouseDragged" /> events.</summary>
		LeftMouseDragged = 1UL << (int) NSEventType.LeftMouseDragged,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.RightMouseDragged" /> events.</summary>
		RightMouseDragged = 1UL << (int) NSEventType.RightMouseDragged,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.MouseEntered" /> events.</summary>
		MouseEntered = 1UL << (int) NSEventType.MouseEntered,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.MouseExited" /> events.</summary>
		MouseExited = 1UL << (int) NSEventType.MouseExited,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.KeyDown" /> events.</summary>
		KeyDown = 1UL << (int) NSEventType.KeyDown,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.KeyUp" /> events.</summary>
		KeyUp = 1UL << (int) NSEventType.KeyUp,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.FlagsChanged" /> events.</summary>
		FlagsChanged = 1UL << (int) NSEventType.FlagsChanged,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.AppKitDefined" /> events.</summary>
		AppKitDefined = 1UL << (int) NSEventType.AppKitDefined,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.SystemDefined" /> events.</summary>
		SystemDefined = 1UL << (int) NSEventType.SystemDefined,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.ApplicationDefined" /> events.</summary>
		ApplicationDefined = 1UL << (int) NSEventType.ApplicationDefined,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.Periodic" /> events.</summary>
		Periodic = 1UL << (int) NSEventType.Periodic,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.CursorUpdate" /> events.</summary>
		CursorUpdate = 1UL << (int) NSEventType.CursorUpdate,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.ScrollWheel" /> events.</summary>
		ScrollWheel = 1UL << (int) NSEventType.ScrollWheel,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.TabletPoint" /> events.</summary>
		TabletPoint = 1UL << (int) NSEventType.TabletPoint,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.TabletProximity" /> events.</summary>
		TabletProximity = 1UL << (int) NSEventType.TabletProximity,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.OtherMouseDown" /> events.</summary>
		OtherMouseDown = 1UL << (int) NSEventType.OtherMouseDown,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.OtherMouseUp" /> events.</summary>
		OtherMouseUp = 1UL << (int) NSEventType.OtherMouseUp,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.OtherMouseDragged" /> events.</summary>
		OtherMouseDragged = 1UL << (int) NSEventType.OtherMouseDragged,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.Gesture" /> events.</summary>
		EventGesture = 1UL << (int) NSEventType.Gesture,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.Magnify" /> events.</summary>
		EventMagnify = 1UL << (int) NSEventType.Magnify,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.Swipe" /> events.</summary>
		EventSwipe = 1UL << (int) NSEventType.Swipe,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.Rotate" /> events.</summary>
		EventRotate = 1UL << (int) NSEventType.Rotate,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.BeginGesture" /> events.</summary>
		EventBeginGesture = 1UL << (int) NSEventType.BeginGesture,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.EndGesture" /> events.</summary>
		EventEndGesture = 1UL << (int) NSEventType.EndGesture,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.SmartMagnify" /> events.</summary>
		SmartMagnify = 1UL << (int) NSEventType.SmartMagnify,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.Pressure" /> events.</summary>
		Pressure = 1UL << (int) NSEventType.Pressure, // 10.10.3, 64-bit-only
		/// <summary>Matches <see cref="F:AppKit.NSEventType.DirectTouch" /> events.</summary>
		DirectTouch = 1UL << (int) NSEventType.DirectTouch, // 10.10
		/// <summary>Matches <see cref="F:AppKit.NSEventType.ChangeMode" /> events.</summary>
		ChangeMode = 1UL << (int) NSEventType.ChangeMode,
		/// <summary>Matches <see cref="F:AppKit.NSEventType.MouseCancelled" /> events.</summary>
		[Mac (26, 0)]
		MouseCancelled = 1UL << (int) NSEventType.MouseCancelled,
		/// <summary>Matches every kind of event.</summary>
		AnyEvent = unchecked((ulong) UInt64.MaxValue),
	}

	/// <summary>Specifies a bitmask of modifier keys, such as Shift, Control, or Command, that were held down when an event occurred.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSEventModifierMask : ulong {
		/// <summary>The Caps Lock key is engaged.</summary>
		AlphaShiftKeyMask = 1 << 16,
		/// <summary>A Shift key is held down.</summary>
		ShiftKeyMask = 1 << 17,
		/// <summary>A Control key is held down.</summary>
		ControlKeyMask = 1 << 18,
		/// <summary>An Option (Alternate) key is held down.</summary>
		AlternateKeyMask = 1 << 19,
		/// <summary>A Command key is held down.</summary>
		CommandKeyMask = 1 << 20,
		/// <summary>The key is located on the numeric keypad.</summary>
		NumericPadKeyMask = 1 << 21,
		/// <summary>The Help key is held down.</summary>
		HelpKeyMask = 1 << 22,
		/// <summary>The key is a function key, such as F1 through F12, or an arrow key.</summary>
		FunctionKeyMask = 1 << 23,
		/// <summary>A mask covering all of the device-independent modifier flags.</summary>
		DeviceIndependentModifierFlagsMask = 0xffff0000,
	}

	/// <summary>Specifies the type of pointing device that generated a tablet event.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPointingDeviceType : ulong {
		/// <summary>The device type is not known.</summary>
		Unknown,
		/// <summary>The device is the tip (writing point) of a stylus.</summary>
		Pen,
		/// <summary>The device is a puck-style pointing device, such as a mouse used with a graphics tablet.</summary>
		Cursor,
		/// <summary>The device is the eraser end of a stylus.</summary>
		Eraser,
	}

	/// <summary>Specifies a bitmask of stylus buttons on a graphics tablet pen.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSEventButtonMask : ulong {
		/// <summary>The tip of the pen is in contact with the tablet.</summary>
		Pen = 1,
		/// <summary>The lower side button of the pen is pressed.</summary>
		PenLower = 2,
		/// <summary>The upper side button of the pen is pressed.</summary>
		PenUpper = 4,
	}

	// This enum is defined as an untyped enum in MacOSX.sdk/System/Library/Frameworks/Carbon.framework/Versions/A/Frameworks/HIToolbox.framework/Versions/A/Headers/Events.h
	// It represents values that may be returned by NSEvent.KeyCode (which isn't typed as 'NSKey' because it may be many other values as well).
	/// <summary>Specifies a virtual key code identifying a physical key on the keyboard, independent of the current keyboard layout.</summary>
	[NoMacCatalyst]
	public enum NSKey {
		/// <summary>The virtual key code for the 'A' key.</summary>
		A = 0x00,
		/// <summary>The virtual key code for the 'S' key.</summary>
		S = 0x01,
		/// <summary>The virtual key code for the 'D' key.</summary>
		D = 0x02,
		/// <summary>The virtual key code for the 'F' key.</summary>
		F = 0x03,
		/// <summary>The virtual key code for the 'H' key.</summary>
		H = 0x04,
		/// <summary>The virtual key code for the 'G' key.</summary>
		G = 0x05,
		/// <summary>The virtual key code for the 'Z' key.</summary>
		Z = 0x06,
		/// <summary>The virtual key code for the 'X' key.</summary>
		X = 0x07,
		/// <summary>The virtual key code for the 'C' key.</summary>
		C = 0x08,
		/// <summary>The virtual key code for the 'V' key.</summary>
		V = 0x09,
		/// <summary>The virtual key code for the 'B' key.</summary>
		B = 0x0B,
		/// <summary>The virtual key code for the 'Q' key.</summary>
		Q = 0x0C,
		/// <summary>The virtual key code for the 'W' key.</summary>
		W = 0x0D,
		/// <summary>The virtual key code for the 'E' key.</summary>
		E = 0x0E,
		/// <summary>The virtual key code for the 'R' key.</summary>
		R = 0x0F,
		/// <summary>The virtual key code for the 'Y' key.</summary>
		Y = 0x10,
		/// <summary>The virtual key code for the 'T' key.</summary>
		T = 0x11,
		/// <summary>The virtual key code for the '1' key on the top row.</summary>
		D1 = 0x12,
		/// <summary>The virtual key code for the '2' key on the top row.</summary>
		D2 = 0x13,
		/// <summary>The virtual key code for the '3' key on the top row.</summary>
		D3 = 0x14,
		/// <summary>The virtual key code for the '4' key on the top row.</summary>
		D4 = 0x15,
		/// <summary>The virtual key code for the '6' key on the top row.</summary>
		D6 = 0x16,
		/// <summary>The virtual key code for the '5' key on the top row.</summary>
		D5 = 0x17,
		/// <summary>The virtual key code for the '=' (equals) key.</summary>
		Equal = 0x18,
		/// <summary>The virtual key code for the '9' key on the top row.</summary>
		D9 = 0x19,
		/// <summary>The virtual key code for the '7' key on the top row.</summary>
		D7 = 0x1A,
		/// <summary>The virtual key code for the '-' (minus/hyphen) key.</summary>
		Minus = 0x1B,
		/// <summary>The virtual key code for the '8' key on the top row.</summary>
		D8 = 0x1C,
		/// <summary>The virtual key code for the '0' key on the top row.</summary>
		D0 = 0x1D,
		/// <summary>The virtual key code for the ']' (right bracket) key.</summary>
		RightBracket = 0x1E,
		/// <summary>The virtual key code for the 'O' key.</summary>
		O = 0x1F,
		/// <summary>The virtual key code for the 'U' key.</summary>
		U = 0x20,
		/// <summary>The virtual key code for the '[' (left bracket) key.</summary>
		LeftBracket = 0x21,
		/// <summary>The virtual key code for the 'I' key.</summary>
		I = 0x22,
		/// <summary>The virtual key code for the 'P' key.</summary>
		P = 0x23,
		/// <summary>The virtual key code for the 'L' key.</summary>
		L = 0x25,
		/// <summary>The virtual key code for the 'J' key.</summary>
		J = 0x26,
		/// <summary>The virtual key code for the quote (') key.</summary>
		Quote = 0x27,
		/// <summary>The virtual key code for the 'K' key.</summary>
		K = 0x28,
		/// <summary>The virtual key code for the ';' (semicolon) key.</summary>
		Semicolon = 0x29,
		/// <summary>The virtual key code for the backslash ('\') key.</summary>
		Backslash = 0x2A,
		/// <summary>The virtual key code for the ',' (comma) key.</summary>
		Comma = 0x2B,
		/// <summary>The virtual key code for the '/' (slash) key.</summary>
		Slash = 0x2C,
		/// <summary>The virtual key code for the 'N' key.</summary>
		N = 0x2D,
		/// <summary>The virtual key code for the 'M' key.</summary>
		M = 0x2E,
		/// <summary>The virtual key code for the '.' (period) key.</summary>
		Period = 0x2F,
		/// <summary>The virtual key code for the grave accent/tilde ('`') key.</summary>
		Grave = 0x32,
		/// <summary>The virtual key code for the decimal point key on the numeric keypad.</summary>
		KeypadDecimal = 0x41,
		/// <summary>The virtual key code for the multiply ('*') key on the numeric keypad.</summary>
		KeypadMultiply = 0x43,
		/// <summary>The virtual key code for the plus ('+') key on the numeric keypad.</summary>
		KeypadPlus = 0x45,
		/// <summary>The virtual key code for the Clear key on the numeric keypad.</summary>
		KeypadClear = 0x47,
		/// <summary>The virtual key code for the divide ('/') key on the numeric keypad.</summary>
		KeypadDivide = 0x4B,
		/// <summary>The virtual key code for the Enter key on the numeric keypad.</summary>
		KeypadEnter = 0x4C,
		/// <summary>The virtual key code for the minus ('-') key on the numeric keypad.</summary>
		KeypadMinus = 0x4E,
		/// <summary>The virtual key code for the equals ('=') key on the numeric keypad.</summary>
		KeypadEquals = 0x51,
		/// <summary>The virtual key code for the '0' key on the numeric keypad.</summary>
		Keypad0 = 0x52,
		/// <summary>The virtual key code for the '1' key on the numeric keypad.</summary>
		Keypad1 = 0x53,
		/// <summary>The virtual key code for the '2' key on the numeric keypad.</summary>
		Keypad2 = 0x54,
		/// <summary>The virtual key code for the '3' key on the numeric keypad.</summary>
		Keypad3 = 0x55,
		/// <summary>The virtual key code for the '4' key on the numeric keypad.</summary>
		Keypad4 = 0x56,
		/// <summary>The virtual key code for the '5' key on the numeric keypad.</summary>
		Keypad5 = 0x57,
		/// <summary>The virtual key code for the '6' key on the numeric keypad.</summary>
		Keypad6 = 0x58,
		/// <summary>The virtual key code for the '7' key on the numeric keypad.</summary>
		Keypad7 = 0x59,
		/// <summary>The virtual key code for the '8' key on the numeric keypad.</summary>
		Keypad8 = 0x5B,
		/// <summary>The virtual key code for the '9' key on the numeric keypad.</summary>
		Keypad9 = 0x5C,
		/// <summary>The virtual key code for the Return key.</summary>
		Return = 0x24,
		/// <summary>The virtual key code for the Tab key.</summary>
		Tab = 0x30,
		/// <summary>The virtual key code for the Space bar.</summary>
		Space = 0x31,
		/// <summary>The virtual key code for the Delete (Backspace) key.</summary>
		Delete = 0x33,
		/// <summary>The virtual key code for the Escape key.</summary>
		Escape = 0x35,
		/// <summary>The virtual key code for the (left) Command key.</summary>
		Command = 0x37,
		/// <summary>The virtual key code for the (left) Shift key.</summary>
		Shift = 0x38,
		/// <summary>The virtual key code for the Caps Lock key.</summary>
		CapsLock = 0x39,
		/// <summary>The virtual key code for the (left) Option key.</summary>
		Option = 0x3A,
		/// <summary>The virtual key code for the (left) Control key.</summary>
		Control = 0x3B,
		/// <summary>The virtual key code for the right Command key.</summary>
		RightCommand = 0x36,
		/// <summary>The virtual key code for the right Shift key.</summary>
		RightShift = 0x3C,
		/// <summary>The virtual key code for the right Option key.</summary>
		RightOption = 0x3D,
		/// <summary>The virtual key code for the right Control key.</summary>
		RightControl = 0x3E,
		/// <summary>The virtual key code for the Fn (Function) key.</summary>
		Function = 0x3F,
		/// <summary>The virtual key code for the F17 function key.</summary>
		F17 = 0x40,
		/// <summary>The virtual key code for the volume up media key.</summary>
		VolumeUp = 0x48,
		/// <summary>The virtual key code for the volume down media key.</summary>
		VolumeDown = 0x49,
		/// <summary>The virtual key code for the mute media key.</summary>
		Mute = 0x4A,
		/// <summary>The virtual key code for the Forward Delete key (deletes the character in front of the insertion point).</summary>
		ForwardDelete = 0x75,
		/// <summary>The virtual key code for the extra key found between Shift and 'Z' on ISO keyboard layouts.</summary>
		ISOSection = 0x0A,
		/// <summary>The virtual key code for the Yen ('¥') key found on JIS keyboard layouts.</summary>
		JISYen = 0x5D,
		/// <summary>The virtual key code for the underscore ('_') key found on JIS keyboard layouts.</summary>
		JISUnderscore = 0x5E,
		/// <summary>The virtual key code for the comma key on the numeric keypad, found on JIS keyboard layouts.</summary>
		JISKeypadComma = 0x5F,
		/// <summary>The virtual key code for the Eisu (alphanumeric input) key found on JIS keyboard layouts.</summary>
		JISEisu = 0x66,
		/// <summary>The virtual key code for the Kana (Japanese kana input) key found on JIS keyboard layouts.</summary>
		JISKana = 0x68,
		/// <summary>The virtual key code for the F18 function key.</summary>
		F18 = 0x4F,
		/// <summary>The virtual key code for the F19 function key.</summary>
		F19 = 0x50,
		/// <summary>The virtual key code for the F20 function key.</summary>
		F20 = 0x5A,
		/// <summary>The virtual key code for the F5 function key.</summary>
		F5 = 0x60,
		/// <summary>The virtual key code for the F6 function key.</summary>
		F6 = 0x61,
		/// <summary>The virtual key code for the F7 function key.</summary>
		F7 = 0x62,
		/// <summary>The virtual key code for the F3 function key.</summary>
		F3 = 0x63,
		/// <summary>The virtual key code for the F8 function key.</summary>
		F8 = 0x64,
		/// <summary>The virtual key code for the F9 function key.</summary>
		F9 = 0x65,
		/// <summary>The virtual key code for the F11 function key.</summary>
		F11 = 0x67,
		/// <summary>The virtual key code for the F13 function key.</summary>
		F13 = 0x69,
		/// <summary>The virtual key code for the F16 function key.</summary>
		F16 = 0x6A,
		/// <summary>The virtual key code for the F14 function key.</summary>
		F14 = 0x6B,
		/// <summary>The virtual key code for the F10 function key.</summary>
		F10 = 0x6D,
		/// <summary>The virtual key code for the F12 function key.</summary>
		F12 = 0x6F,
		/// <summary>The virtual key code for the F15 function key.</summary>
		F15 = 0x71,
		/// <summary>The virtual key code for the Help key.</summary>
		Help = 0x72,
		/// <summary>The virtual key code for the Home key.</summary>
		Home = 0x73,
		/// <summary>The virtual key code for the Page Up key.</summary>
		PageUp = 0x74,
		/// <summary>The virtual key code for the F4 function key.</summary>
		F4 = 0x76,
		/// <summary>The virtual key code for the End key.</summary>
		End = 0x77,
		/// <summary>The virtual key code for the F2 function key.</summary>
		F2 = 0x78,
		/// <summary>The virtual key code for the Page Down key.</summary>
		PageDown = 0x79,
		/// <summary>The virtual key code for the F1 function key.</summary>
		F1 = 0x7A,
		/// <summary>The virtual key code for the left arrow key.</summary>
		LeftArrow = 0x7B,
		/// <summary>The virtual key code for the right arrow key.</summary>
		RightArrow = 0x7C,
		/// <summary>The virtual key code for the down arrow key.</summary>
		DownArrow = 0x7D,
		/// <summary>The virtual key code for the up arrow key.</summary>
		UpArrow = 0x7E,
	}

	// This is an untyped enum in AppKit's NSEvent.h
	/// <summary>Specifies a Unicode private-use-area code point representing a non-printable function key, such as an arrow key or F-key.</summary>
	[NoMacCatalyst]
	public enum NSFunctionKey : int {
		/// <summary>A unicode private-use-area code point representing the up arrow key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		UpArrow = 0xF700,
		/// <summary>A unicode private-use-area code point representing the down arrow key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		DownArrow = 0xF701,
		/// <summary>A unicode private-use-area code point representing the left arrow key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		LeftArrow = 0xF702,
		/// <summary>A unicode private-use-area code point representing the right arrow key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		RightArrow = 0xF703,
		/// <summary>A unicode private-use-area code point representing the F1 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F1 = 0xF704,
		/// <summary>A unicode private-use-area code point representing the F2 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F2 = 0xF705,
		/// <summary>A unicode private-use-area code point representing the F3 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F3 = 0xF706,
		/// <summary>A unicode private-use-area code point representing the F4 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F4 = 0xF707,
		/// <summary>A unicode private-use-area code point representing the F5 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F5 = 0xF708,
		/// <summary>A unicode private-use-area code point representing the F6 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F6 = 0xF709,
		/// <summary>A unicode private-use-area code point representing the F7 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F7 = 0xF70A,
		/// <summary>A unicode private-use-area code point representing the F8 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F8 = 0xF70B,
		/// <summary>A unicode private-use-area code point representing the F9 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F9 = 0xF70C,
		/// <summary>A unicode private-use-area code point representing the F10 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F10 = 0xF70D,
		/// <summary>A unicode private-use-area code point representing the F11 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F11 = 0xF70E,
		/// <summary>A unicode private-use-area code point representing the F12 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F12 = 0xF70F,
		/// <summary>A unicode private-use-area code point representing the F13 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F13 = 0xF710,
		/// <summary>A unicode private-use-area code point representing the F14 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F14 = 0xF711,
		/// <summary>A unicode private-use-area code point representing the F15 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F15 = 0xF712,
		/// <summary>A unicode private-use-area code point representing the F16 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F16 = 0xF713,
		/// <summary>A unicode private-use-area code point representing the F17 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F17 = 0xF714,
		/// <summary>A unicode private-use-area code point representing the F18 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F18 = 0xF715,
		/// <summary>A unicode private-use-area code point representing the F19 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F19 = 0xF716,
		/// <summary>A unicode private-use-area code point representing the F20 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F20 = 0xF717,
		/// <summary>A unicode private-use-area code point representing the F21 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F21 = 0xF718,
		/// <summary>A unicode private-use-area code point representing the F22 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F22 = 0xF719,
		/// <summary>A unicode private-use-area code point representing the F23 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F23 = 0xF71A,
		/// <summary>A unicode private-use-area code point representing the F24 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F24 = 0xF71B,
		/// <summary>A unicode private-use-area code point representing the F25 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F25 = 0xF71C,
		/// <summary>A unicode private-use-area code point representing the F26 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F26 = 0xF71D,
		/// <summary>A unicode private-use-area code point representing the F27 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F27 = 0xF71E,
		/// <summary>A unicode private-use-area code point representing the F28 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F28 = 0xF71F,
		/// <summary>A unicode private-use-area code point representing the F29 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F29 = 0xF720,
		/// <summary>A unicode private-use-area code point representing the F30 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F30 = 0xF721,
		/// <summary>A unicode private-use-area code point representing the F31 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F31 = 0xF722,
		/// <summary>A unicode private-use-area code point representing the F32 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F32 = 0xF723,
		/// <summary>A unicode private-use-area code point representing the F33 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F33 = 0xF724,
		/// <summary>A unicode private-use-area code point representing the F34 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F34 = 0xF725,
		/// <summary>A unicode private-use-area code point representing the F35 function key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		F35 = 0xF726,
		/// <summary>A unicode private-use-area code point representing the Insert key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Insert = 0xF727,
		/// <summary>A unicode private-use-area code point representing the Delete key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Delete = 0xF728,
		/// <summary>A unicode private-use-area code point representing the Home key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Home = 0xF729,
		/// <summary>A unicode private-use-area code point representing the Begin key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Begin = 0xF72A,
		/// <summary>A unicode private-use-area code point representing the End key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		End = 0xF72B,
		/// <summary>A unicode private-use-area code point representing the Page Up key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		PageUp = 0xF72C,
		/// <summary>A unicode private-use-area code point representing the Page Down key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		PageDown = 0xF72D,
		/// <summary>A unicode private-use-area code point representing the Print Screen key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		PrintScreen = 0xF72E,
		/// <summary>A unicode private-use-area code point representing the Scroll Lock key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		ScrollLock = 0xF72F,
		/// <summary>A unicode private-use-area code point representing the Pause key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Pause = 0xF730,
		/// <summary>A unicode private-use-area code point representing the System Request key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		SysReq = 0xF731,
		/// <summary>A unicode private-use-area code point representing the Break key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Break = 0xF732,
		/// <summary>A unicode private-use-area code point representing the Reset key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Reset = 0xF733,
		/// <summary>A unicode private-use-area code point representing the Stop key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Stop = 0xF734,
		/// <summary>A unicode private-use-area code point representing the Menu key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Menu = 0xF735,
		/// <summary>A unicode private-use-area code point representing the User key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		User = 0xF736,
		/// <summary>A unicode private-use-area code point representing the System key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		System = 0xF737,
		/// <summary>A unicode private-use-area code point representing the Print key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Print = 0xF738,
		/// <summary>A unicode private-use-area code point representing the Clear Line key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		ClearLine = 0xF739,
		/// <summary>A unicode private-use-area code point representing the Clear Display key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		ClearDisplay = 0xF73A,
		/// <summary>A unicode private-use-area code point representing the Insert Line key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		InsertLine = 0xF73B,
		/// <summary>A unicode private-use-area code point representing the Delete Line key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		DeleteLine = 0xF73C,
		/// <summary>A unicode private-use-area code point representing the Insert Character key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		InsertChar = 0xF73D,
		/// <summary>A unicode private-use-area code point representing the Delete Character key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		DeleteChar = 0xF73E,
		/// <summary>A unicode private-use-area code point representing the Previous key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Prev = 0xF73F,
		/// <summary>A unicode private-use-area code point representing the Next key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Next = 0xF740,
		/// <summary>A unicode private-use-area code point representing the Select key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Select = 0xF741,
		/// <summary>A unicode private-use-area code point representing the Execute key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Execute = 0xF742,
		/// <summary>A unicode private-use-area code point representing the Undo key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Undo = 0xF743,
		/// <summary>A unicode private-use-area code point representing the Redo key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Redo = 0xF744,
		/// <summary>A unicode private-use-area code point representing the Find key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Find = 0xF745,
		/// <summary>A unicode private-use-area code point representing the Help key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		Help = 0xF746,
		/// <summary>A unicode private-use-area code point representing the Mode Switch key, used when reporting the <see cref="P:AppKit.NSEvent.CharactersIgnoringModifiers" /> for a non-printable key.</summary>
		ModeSwitch = 0xF747,
	}

	/// <summary>Specifies additional information about certain kinds of events; the meaning of each value depends on the event's <see cref="T:AppKit.NSEventType" />.</summary>
	[NoMacCatalyst]
	public enum NSEventSubtype : short {
		/* event subtypes for NSEventTypeAppKitDefined events */
		/// <summary>For an <see cref="F:AppKit.NSEventType.AppKitDefined" /> event, indicates that a window was exposed.</summary>
		WindowExposed = 0,
		/// <summary>For an <see cref="F:AppKit.NSEventType.AppKitDefined" /> event, indicates that the application was activated.</summary>
		ApplicationActivated = 1,
		/// <summary>For an <see cref="F:AppKit.NSEventType.AppKitDefined" /> event, indicates that the application was deactivated.</summary>
		ApplicationDeactivated = 2,
		/// <summary>For an <see cref="F:AppKit.NSEventType.AppKitDefined" /> event, indicates that a window was moved.</summary>
		WindowMoved = 4,
		/// <summary>For an <see cref="F:AppKit.NSEventType.AppKitDefined" /> event, indicates that the screen configuration changed.</summary>
		ScreenChanged = 8,
		/* event subtypes for NSEventTypeSystemDefined events */
		/* the value is repeated from above */
		/// <summary>For a <see cref="F:AppKit.NSEventType.SystemDefined" /> event, indicates that the system is powering off.</summary>
		PowerOff = 1,

		/* event subtypes for mouse events */
		/* the values are repeated from above */
		/// <summary>For a mouse event, indicates a standard mouse event.</summary>
		MouseEvent = 0, /* NX_SUBTYPE_DEFAULT */
		/// <summary>For a mouse event, indicates that the event originated from a graphics tablet stylus point.</summary>
		TabletPoint = 1, /* NX_SUBTYPE_TABLET_POINT */
		/// <summary>For a mouse event, indicates that the event originated from a graphics tablet proximity change.</summary>
		TabletProximity = 2, /* NX_SUBTYPE_TABLET_PROXIMITY */
		/// <summary>For a mouse event, indicates that the event originated from a touch on a touch-sensitive surface.</summary>
		Touch = 3, /* NX_SUBTYPE_MOUSE_TOUCH */
	}

	#endregion

	#region NSView
	/// <summary>Specifies a bitmask of options that describe how a view resizes relative to its superview.</summary>
	[NoMacCatalyst]
	[Flags]
	[NativeName ("NSAutoresizingMaskOptions")]
	[Native]
	public enum NSViewResizingMask : ulong {
		/// <summary>The view does not resize with its superview.</summary>
		NotSizable = 0,
		/// <summary>The left margin between the view and its superview can grow or shrink.</summary>
		MinXMargin = 1,
		/// <summary>The view's width can grow or shrink.</summary>
		WidthSizable = 2,
		/// <summary>The right margin between the view and its superview can grow or shrink.</summary>
		MaxXMargin = 4,
		/// <summary>The bottom margin between the view and its superview can grow or shrink.</summary>
		MinYMargin = 8,
		/// <summary>The view's height can grow or shrink.</summary>
		HeightSizable = 16,
		/// <summary>The top margin between the view and its superview can grow or shrink.</summary>
		MaxYMargin = 32,
	}

	/// <summary>Specifies the style of border drawn around a view.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSBorderType : ulong {
		/// <summary>No border is drawn.</summary>
		NoBorder,
		/// <summary>A line border is drawn.</summary>
		LineBorder,
		/// <summary>A bezel border is drawn.</summary>
		BezelBorder,
		/// <summary>A groove border is drawn.</summary>
		GrooveBorder,
	}

	/// <summary>Specifies the shape of a text field's bezel.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextFieldBezelStyle : ulong {
		/// <summary>A square bezel.</summary>
		Square,
		/// <summary>A rounded bezel.</summary>
		Rounded,
	}

	/// <summary>Specifies when a view's backing layer redraws its contents in response to the view being resized.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSViewLayerContentsRedrawPolicy : long {
		/// <summary>The layer never redraws its contents automatically; the view is responsible for updating it.</summary>
		Never,
		/// <summary>The layer redraws its contents only when the view calls <c>SetNeedsDisplay</c>.</summary>
		OnSetNeedsDisplay,
		/// <summary>The layer redraws its contents continuously while the view is being resized.</summary>
		DuringViewResize,
		/// <summary>The layer redraws its contents once, immediately before the view is resized.</summary>
		BeforeViewResize,
		/// <summary>The layer redraws its contents once, immediately after the view is resized, and cross-fades from the previous contents.</summary>
		Crossfade = 4,
	}

	/// <summary>Specifies how a view's backing layer positions its contents within the layer's bounds when the layer's <see cref="T:AppKit.NSViewLayerContentsRedrawPolicy" /> does not redraw on every resize.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSViewLayerContentsPlacement : long {
		/// <summary>Scales the contents independently along each axis to exactly fill the layer's bounds.</summary>
		ScaleAxesIndependently,
		/// <summary>Scales the contents proportionally so the entire contents fit within the layer's bounds.</summary>
		ScaleProportionallyToFit,
		/// <summary>Scales the contents proportionally so they completely fill the layer's bounds, cropping if necessary.</summary>
		ScaleProportionallyToFill,
		/// <summary>Centers the contents within the layer's bounds, without scaling.</summary>
		Center,
		/// <summary>Aligns the contents to the top edge of the layer's bounds, without scaling.</summary>
		Top,
		/// <summary>Aligns the contents to the top-right corner of the layer's bounds, without scaling.</summary>
		TopRight,
		/// <summary>Aligns the contents to the right edge of the layer's bounds, without scaling.</summary>
		Right,
		/// <summary>Aligns the contents to the bottom-right corner of the layer's bounds, without scaling.</summary>
		BottomRight,
		/// <summary>Aligns the contents to the bottom edge of the layer's bounds, without scaling.</summary>
		Bottom,
		/// <summary>Aligns the contents to the bottom-left corner of the layer's bounds, without scaling.</summary>
		BottomLeft,
		/// <summary>Aligns the contents to the left edge of the layer's bounds, without scaling.</summary>
		Left,
		/// <summary>Aligns the contents to the top-left corner of the layer's bounds, without scaling.</summary>
		TopLeft,
	}

	#endregion

	#region NSWindow
	/// <summary>Specifies a bitmask of stylistic and behavioral traits for a window's title bar, borders, and behavior.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native ("NSWindowStyleMask")]
	public enum NSWindowStyle : ulong {
		/// <summary>The window has no title bar and no border.</summary>
		Borderless = 0 << 0,
		/// <summary>The window displays a title bar.</summary>
		Titled = 1 << 0,
		/// <summary>The window displays a close button in its title bar.</summary>
		Closable = 1 << 1,
		/// <summary>The window displays a minimize button in its title bar.</summary>
		Miniaturizable = 1 << 2,
		/// <summary>The window can be resized by the user.</summary>
		Resizable = 1 << 3,
		/// <summary>The window is drawn using the "utility" panel appearance (a thinner title bar), typically used for auxiliary windows.</summary>
		Utility = 1 << 4,
		/// <summary>The panel behaves like a document-modal sheet.</summary>
		DocModal = 1 << 6,
		/// <summary>The panel does not activate its application when the user interacts with it.</summary>
		NonactivatingPanel = 1 << 7,
		/// <summary>The window has a textured background, such as the appearance historically used by metal-style windows.</summary>
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Don't use 'TexturedBackground' anymore.")]
		TexturedBackground = 1 << 8,
		/// <summary>The title bar and toolbar are drawn as a single, unified area.</summary>
		UnifiedTitleAndToolbar = 1 << 12,
		/// <summary>The window is drawn using the dark, translucent heads-up display (HUD) panel appearance.</summary>
		Hud = 1 << 13,
		/// <summary>The window is currently participating in full-screen mode.</summary>
		FullScreenWindow = 1 << 14,
		/// <summary>The window's content view extends to fill the entire window frame, including the area behind the title bar.</summary>
		FullSizeContentView = 1 << 15,
	}

	/// <summary>Specifies whether, and how, a window's contents can be read by other processes, such as screen-recording or accessibility clients.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowSharingType : ulong {
		/// <summary>The window's contents cannot be read by another process.</summary>
		None,
		/// <summary>The window's contents can be read, but not modified, by another process.</summary>
		ReadOnly,
		/// <summary>The window's contents can be both read and modified by another process.</summary>
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'ReadOnly' instead.")]
		ReadWrite,
	}

	/// <summary>Specifies where a window's backing store is located.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowBackingLocation : ulong {
		/// <summary>The system determines the backing store location.</summary>
		Default,
		/// <summary>The backing store is located in video memory.</summary>
		VideoMemory,
		/// <summary>The backing store is located in main memory.</summary>
		MainMemory,
	}

	/// <summary>Specifies a bitmask that describes how a window behaves with respect to Spaces and Exposé/Mission Control.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSWindowCollectionBehavior : ulong {
		/// <summary>The window uses the default Spaces and Exposé behavior.</summary>
		Default = 0,
		/// <summary>The window can be visible on every Space simultaneously.</summary>
		CanJoinAllSpaces = 1 << 0,
		/// <summary>The window moves to the currently active Space when it is ordered to the front.</summary>
		MoveToActiveSpace = 1 << 1,
		/// <summary>The window participates in Spaces and Exposé, and can be managed like an ordinary application window.</summary>
		Managed = 1 << 2,
		/// <summary>The window floats above the active Space and is hidden from the Mission Control window overview.</summary>
		Transient = 1 << 3,
		/// <summary>The window remains visible and stationary during Mission Control, like the desktop window.</summary>
		Stationary = 1 << 4,
		/// <summary>The window participates in the Cycle Through Windows window-cycling order.</summary>
		ParticipatesInCycle = 1 << 5,
		/// <summary>The window does not participate in the Cmd-`/Cmd-Shift-` window-cycling order.</summary>
		IgnoresCycle = 1 << 6,
		/// <summary>The window is the primary content window in a full-screen session.</summary>
		FullScreenPrimary = 1 << 7,
		/// <summary>The window can appear alongside a full-screen window as an auxiliary, such as a panel or inspector.</summary>
		FullScreenAuxiliary = 1 << 8,
		/// <summary>The window does not support full-screen mode.</summary>
		FullScreenNone = 1 << 9,
		/// <summary>The window supports being tiled side-by-side with another window in full-screen mode.</summary>
		FullScreenAllowsTiling = 1 << 11,
		/// <summary>The window does not support being tiled side-by-side with another window in full-screen mode.</summary>
		FullScreenDisallowsTiling = 1 << 12,
		/// <summary>The window is a primary window in a Stage Manager-style set, and other secondary windows can be grouped with it.</summary>
		Primary = 1 << 16,
		/// <summary>The window is an auxiliary window that is grouped together with a primary window.</summary>
		Auxiliary = 1 << 17,
		/// <summary>The window can join a set of windows that spans multiple applications.</summary>
		CanJoinAllApplications = 1 << 18,
	}

	/// <summary>Specifies which windows to include in a window-number list.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSWindowNumberListOptions : ulong {
		/// <summary>Includes windows from all applications, instead of only the calling application.</summary>
		AllApplication = 1 << 0,
		/// <summary>Includes windows from all spaces, instead of only the active space.</summary>
		AllSpaces = 1 << 4,
	}

	/// <summary>Specifies the direction in which a window traverses its key-view loop.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSelectionDirection : ulong {
		/// <summary>Indicates that the window is not traversing the key-view loop.</summary>
		Direct = 0,
		/// <summary>Proceeds to the next valid key view.</summary>
		Next,
		/// <summary>Proceeds to the previous valid key view.</summary>
		Previous,
	}

	/// <summary>Specifies one of the standard buttons that can appear in a window's title bar.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowButton : ulong {
		/// <summary>The button that closes the window.</summary>
		CloseButton,
		/// <summary>The button that minimizes the window to the Dock.</summary>
		MiniaturizeButton,
		/// <summary>The button that zooms the window between its standard and user-defined sizes.</summary>
		ZoomButton,
		/// <summary>The button used to toggle the visibility of the window's toolbar.</summary>
		ToolbarButton,
		/// <summary>The button that displays the document icon in the title bar.</summary>
		DocumentIconButton,
		/// <summary>The button that displays the document's version history.</summary>
		DocumentVersionsButton = 6,
		/// <summary>The button that toggles full-screen mode.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "The standard window button for FullScreenButton is always null; use ZoomButton instead.")]
		FullScreenButton,
	}

	/// <summary>Specifies a bitmask describing the phase of a touch during a multi-touch tracking sequence.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSTouchPhase : ulong {
		/// <summary>The touch has just begun.</summary>
		Began = 1 << 0,
		/// <summary>The position of the touch has changed.</summary>
		Moved = 1 << 1,
		/// <summary>The touch has not moved since the previous event.</summary>
		Stationary = 1 << 2,
		/// <summary>The touch has ended.</summary>
		Ended = 1 << 3,
		/// <summary>The touch was cancelled and did not end normally.</summary>
		Cancelled = 1 << 4,

		/// <summary>A combination of the phases that indicate an ongoing touch: <see cref="F:AppKit.NSTouchPhase.Began" />, <see cref="F:AppKit.NSTouchPhase.Moved" />, and <see cref="F:AppKit.NSTouchPhase.Stationary" />.</summary>
		Touching = Began | Moved | Stationary,
		/// <summary>Matches every touch phase.</summary>
		Any = unchecked((ulong) UInt64.MaxValue),
	}
	#endregion
	#region NSAnimation

	/// <summary>Specifies the timing curve of an animation.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSAnimationCurve : ulong {
		/// <summary>The animation accelerates at the beginning and decelerates at the end.</summary>
		EaseInOut,
		/// <summary>The animation accelerates from a slow start.</summary>
		EaseIn,
		/// <summary>The animation decelerates toward the end.</summary>
		EaseOut,
		/// <summary>The animation proceeds at a constant rate.</summary>
		Linear,
	};

	/// <summary>Specifies how an animation runs relative to the application.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSAnimationBlockingMode : ulong {
		/// <summary>Runs the animation synchronously and blocks user interaction until it completes.</summary>
		Blocking,
		/// <summary>Runs the animation asynchronously on the main thread while allowing user interaction.</summary>
		Nonblocking,
		/// <summary>Runs the animation asynchronously on a separate thread.</summary>
		NonblockingThreaded,
	};
	#endregion

	#region NSBox

	/// <summary>Specifies where a box's title is positioned relative to its border.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTitlePosition : ulong {
		/// <summary>No title is displayed.</summary>
		NoTitle,
		/// <summary>The title is centered above the box's top border.</summary>
		AboveTop,
		/// <summary>The title is centered on the box's top border.</summary>
		AtTop,
		/// <summary>The title is centered just below the box's top border, inside the box.</summary>
		BelowTop,
		/// <summary>The title is centered just above the box's bottom border, inside the box.</summary>
		AboveBottom,
		/// <summary>The title is centered on the box's bottom border.</summary>
		AtBottom,
		/// <summary>The title is centered below the box's bottom border.</summary>
		BelowBottom,
	};

	/// <summary>Specifies the visual style of an <see cref="T:AppKit.NSBox" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSBoxType : ulong {
		/// <summary>A box with a simple border and an optional title, used as a general-purpose grouping box.</summary>
		NSBoxPrimary,
		/// <summary>A box style identical to <see cref="F:AppKit.NSBoxType.NSBoxPrimary" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 15, message: "Identical to 'NSBoxPrimary'.")]
		NSBoxSecondary,
		/// <summary>A box drawn as a thin separator line, typically used to divide sections of a user interface.</summary>
		NSBoxSeparator,
		/// <summary>A box drawn with the appearance used in older versions of macOS.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 15, message: "'NSBoxOldStyle' is discouraged. Use 'NSBoxPrimary' or 'NSBoxCustom'.")]
		NSBoxOldStyle,
		/// <summary>A box whose border, background, and title appearance can be fully customized.</summary>
		NSBoxCustom,
	};
	#endregion

	#region NSButtonCell
	/// <summary>Specifies the highlighting and state-tracking behavior of a button.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSButtonType : ulong {
		/// <summary>The button highlights while pressed and reverts to its normal appearance when released, without retaining any state; used for simple action buttons.</summary>
		MomentaryLightButton,
		/// <summary>The button alternates between a pushed-in "on" appearance and its normal "off" appearance each time it is clicked.</summary>
		PushOnPushOff,
		/// <summary>The button alternates between its normal image or title and its alternate image or title each time it is clicked.</summary>
		Toggle,
		/// <summary>The button behaves like a checkbox, toggling between an unchecked and checked appearance each time it is clicked.</summary>
		Switch,
		/// <summary>The button behaves like a radio button; selecting it deselects the other radio buttons in the same group.</summary>
		Radio,
		/// <summary>The button displays its alternate image or title only while the mouse button is held down, reverting when released, without retaining any state.</summary>
		MomentaryChange,
		/// <summary>The button toggles between an "on" and "off" state each time it is clicked, remaining highlighted while "on".</summary>
		OnOff,
		/// <summary>The button appears pushed in only while the mouse button is held down, reverting when released, without retaining any state.</summary>
		MomentaryPushIn,
		/// <summary>The pressure-sensitive button sends actions as pressure changes and, when continuous, repeats them at an accelerating rate.</summary>
		Accelerator, // 10.10.3
		/// <summary>The pressure-sensitive button supports configurable pressure levels with tactile feedback and, when continuous, repeats actions according to the applied pressure.</summary>
		MultiLevelAccelerator, // 10.10.3
	}

	/// <summary>Specifies the appearance of a button's bezel (its border and background).</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSBezelStyle : ulong {
		/// <summary>The system chooses the most appropriate bezel style for the button's current configuration.</summary>
		Automatic = 0,
		/// <summary>A standard push-button bezel, typically used for the default button in a dialog.</summary>
		Push = 1,
		/// <summary>A push-button bezel whose height adapts to the button's content, unlike the fixed-height <see cref="F:AppKit.NSBezelStyle.Push" /> style.</summary>
		FlexiblePush = 2,
		/// <summary>An unbezeled button with a disclosure triangle, used to reveal or hide additional content.</summary>
		Disclosure = 5,
		/// <summary>A round bezel, typically used for buttons that contain only an image.</summary>
		Circular = 7,
		/// <summary>A round bezel showing a question mark, used for standard Help buttons.</summary>
		HelpButton = 9,
		/// <summary>A small, square bezel with square corners.</summary>
		SmallSquare = 10,
		/// <summary>A bezel appropriate for buttons placed in a window's toolbar.</summary>
		Toolbar = 11,
		/// <summary>A bezel appropriate for action buttons placed in an accessory bar, such as the bottom bar of a table or scroll view.</summary>
		AccessoryBarAction = 12,
		/// <summary>A bezel appropriate for buttons placed in an accessory bar, such as the bottom bar of a table or scroll view.</summary>
		AccessoryBar = 13,
		/// <summary>A push-button bezel that also displays a disclosure triangle.</summary>
		PushDisclosure = 14,
		/// <summary>A small, rounded bezel typically used for buttons that display a badge or count.</summary>
		Badge = 15,
		/// <summary>A bezel using the Liquid Glass material appearance.</summary>
		[Mac (20, 0)]
		Glass = 16,
#if !XAMCORE_5_0
		/// <summary>A push-button bezel with rounded corners. Superseded by <see cref="F:AppKit.NSBezelStyle.Push" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "Use 'Push' instead.")]
		Rounded = 1,
		/// <summary>A square button bezel. Superseded by <see cref="F:AppKit.NSBezelStyle.FlexiblePush" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "Use 'FlexiblePush' instead.")]
		RegularSquare = 2,
		/// <summary>A thick square button bezel. Superseded by <see cref="F:AppKit.NSBezelStyle.FlexiblePush" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 12, message: "Use 'FlexiblePush' instead.")]
		ThickSquare = 3,
		/// <summary>An even thicker square button bezel. Superseded by <see cref="F:AppKit.NSBezelStyle.FlexiblePush" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 12, message: "Use 'FlexiblePush' instead.")]
		ThickerSquare = 4,
		/// <summary>A square bezel drawn without a shadow. Superseded by <see cref="F:AppKit.NSBezelStyle.SmallSquare" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "Use 'SmallSquare' instead.")]
		ShadowlessSquare = 6,
		/// <summary>A square bezel with a textured appearance. Superseded by <see cref="F:AppKit.NSBezelStyle.SmallSquare" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "Use 'SmallSquare' instead.")]
		TexturedSquare = 8,
		/// <summary>A rounded bezel with a textured appearance. Superseded by <see cref="F:AppKit.NSBezelStyle.Toolbar" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "Use 'Toolbar' instead.")]
		TexturedRounded = 11,
		/// <summary>A bezel with rounded rectangle corners. Superseded by <see cref="F:AppKit.NSBezelStyle.AccessoryBarAction" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "Use 'AccessoryBarAction' instead.")]
		RoundRect = 12,
		/// <summary>A bezel with a recessed appearance. Superseded by <see cref="F:AppKit.NSBezelStyle.AccessoryBar" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "Use 'AccessoryBar' instead.")]
		Recessed = 13,
		/// <summary>A push-button bezel with rounded corners that also displays a disclosure triangle. Superseded by <see cref="F:AppKit.NSBezelStyle.PushDisclosure" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "Use 'PushDisclosure' instead.")]
		RoundedDisclosure = 14,
		/// <summary>A bezel drawn inline with surrounding content. Superseded by <see cref="F:AppKit.NSBezelStyle.Badge" />.</summary>
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "Use 'Badge' instead.")]
		Inline = 15,
#endif // !XAMCORE_5_0
	}

	/// <summary>Specifies a gradient style that was previously drawn on certain bezeled controls; no longer has any visual effect.</summary>
	[NoMacCatalyst]
	[Native]
	[Deprecated (PlatformName.MacOSX, 10, 12, message: "The GradientType property is unused, and setting it has no effect.")]
	public enum NSGradientType : ulong {
		/// <summary>No gradient is drawn.</summary>
		None,
		/// <summary>A subtle concave (inward-curving) gradient.</summary>
		ConcaveWeak,
		/// <summary>A pronounced concave (inward-curving) gradient.</summary>
		ConcaveStrong,
		/// <summary>A subtle convex (outward-curving) gradient.</summary>
		ConvexWeak,
		/// <summary>A pronounced convex (outward-curving) gradient.</summary>
		ConvexStrong,
	}

	#endregion

	#region NSGraphics
	/// <summary>Specifies the color depth of a window's backing store.</summary>
	[NoMacCatalyst]
	// NSGraphics.h:typedef int NSWindowDepth;
	public enum NSWindowDepth : int {
		/// <summary>A 24-bit RGB color depth.</summary>
		TwentyfourBitRgb = 0x208,
		/// <summary>A 64-bit RGB color depth.</summary>
		SixtyfourBitRgb = 0x210,
		/// <summary>A 128-bit RGB color depth.</summary>
		OneHundredTwentyEightBitRgb = 0x220,
	}

	/// <summary>Specifies a Porter-Duff compositing operator, or a Core Image-style blend mode, used to combine a source image with a destination image.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSCompositingOperation : ulong {
		/// <summary>The result is transparent black; neither the source nor the destination contributes.</summary>
		Clear,
		/// <summary>The source image replaces the destination image.</summary>
		Copy,
		/// <summary>The source image is composited over the destination image.</summary>
		SourceOver,
		/// <summary>The part of the source image that lies within the destination image replaces the destination; everything else is discarded.</summary>
		SourceIn,
		/// <summary>The part of the source image that lies outside the destination image is displayed; everything else is discarded.</summary>
		SourceOut,
		/// <summary>The source image is composited over the destination image, but only the part of the source that lies within the destination is displayed.</summary>
		SourceAtop,
		/// <summary>The destination image is composited over the source image.</summary>
		DestinationOver,
		/// <summary>The part of the destination image that lies within the source image replaces the destination; everything else is discarded.</summary>
		DestinationIn,
		/// <summary>The part of the destination image that lies outside the source image is displayed; everything else is discarded.</summary>
		DestinationOut,
		/// <summary>The destination image is composited over the source image, but only the part of the destination that lies within the source is displayed.</summary>
		DestinationAtop,
		/// <summary>The parts of the source and destination images that do not overlap are displayed; the overlapping region is discarded.</summary>
		Xor,
		/// <summary>The source and destination color values are summed and the result approaches 0 (black) as the limit.</summary>
		PlusDarker,
		/// <summary>The source image is composited over the destination image using the same behavior as <see cref="F:AppKit.NSCompositingOperation.SourceOver" />.</summary>
		Highlight,
		/// <summary>The source and destination color values are summed and the result approaches 1 (white) as the limit.</summary>
		PlusLighter,

		/// <summary>The source and destination color values are multiplied together, always producing a darker or equal color.</summary>
		Multiply,
		/// <summary>The source and destination color values are inverted, multiplied, and inverted again, always producing a lighter or equal color.</summary>
		Screen,
		/// <summary>Multiplies or screens the color values, depending on the destination color, preserving highlights and shadows.</summary>
		Overlay,
		/// <summary>The darker of the source and destination color values is used at each pixel.</summary>
		Darken,
		/// <summary>The lighter of the source and destination color values is used at each pixel.</summary>
		Lighten,
		/// <summary>The destination color is brightened to reflect the source color, increasing the contrast between the two.</summary>
		ColorDodge,
		/// <summary>The destination color is darkened to reflect the source color, increasing the contrast between the two.</summary>
		ColorBurn,
		/// <summary>Darkens or lightens the colors, depending on the source color value, producing an effect similar to shining a diffuse spotlight on the destination.</summary>
		SoftLight,
		/// <summary>Multiplies or screens the color values, depending on the source color, producing an effect similar to shining a harsh spotlight on the destination.</summary>
		HardLight,
		/// <summary>Subtracts either the source color from the destination color or vice versa, whichever produces a positive value, producing an inversion effect.</summary>
		Difference,
		/// <summary>Produces an effect similar to <see cref="F:AppKit.NSCompositingOperation.Difference" /> but with lower contrast.</summary>
		Exclusion,
		/// <summary>Uses the luminance and saturation of the destination and the hue of the source.</summary>
		Hue,
		/// <summary>Uses the luminance and hue of the destination and the saturation of the source.</summary>
		Saturation,
		/// <summary>Uses the luminance of the destination and the hue and saturation of the source.</summary>
		Color,
		/// <summary>Uses the hue and saturation of the destination and the luminance of the source.</summary>
		Luminosity,
	}

	/// <summary>Specifies an animation effect to display at a screen location.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSAnimationEffect : ulong {
		/// <summary>The default effect for a disappearing item.</summary>
		DisappearingItemDefault = 0,
#if !XAMCORE_5_0
		/// <summary>The obsolete, misspelled name for <see cref="DisappearingItemDefault" />.</summary>
		[Obsolete ("Use 'DisappearingItemDefault' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		DissapearingItemDefault = DisappearingItemDefault,
#endif
		/// <summary>A poof animation effect.</summary>
		EffectPoof = 10,
	}
	#endregion

	#region NSMatrix
	/// <summary>Specifies how a matrix tracks and selects its cells.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSMatrixMode : ulong {
		/// <summary>Allows a single cell to be selected at a time.</summary>
		Radio,
		/// <summary>Highlights a cell while asking it to track the mouse.</summary>
		Highlight,
		/// <summary>Highlights cells without asking them to track the mouse.</summary>
		List,
		/// <summary>Allows individual cells to track the mouse.</summary>
		Track,
	}
	#endregion

	#region NSBrowser
	/// <summary>Specifies how browser columns can be resized.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSBrowserColumnResizingType : ulong {
		/// <summary>Prevents columns from being resized.</summary>
		None,
		/// <summary>Automatically resizes columns.</summary>
		Auto,
		/// <summary>Allows the user to resize columns.</summary>
		User,
	}

	/// <summary>Specifies where a browser accepts a drag-and-drop operation.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSBrowserDropOperation : ulong {
		/// <summary>The drop occurs on an item.</summary>
		On,
		/// <summary>The drop occurs above an item.</summary>
		Above,
	}
	#endregion

	#region NSColorPanel
	/// <summary>Specifies which color-picking mode is active in the shared color panel.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSColorPanelMode : long {
		/// <summary>No specific mode; the color panel's default picker is shown.</summary>
		None = -1,
		/// <summary>The grayscale picker.</summary>
		Gray = 0,
		/// <summary>The red-green-blue (RGB) sliders picker.</summary>
		RGB,
		/// <summary>The cyan-magenta-yellow-black (CMYK) sliders picker.</summary>
		CMYK,
		/// <summary>The hue-saturation-brightness (HSB) sliders picker.</summary>
		HSB,
		/// <summary>The custom color list/palette picker.</summary>
		CustomPalette,
		/// <summary>The named color list picker.</summary>
		ColorList,
		/// <summary>The color wheel picker.</summary>
		Wheel,
		/// <summary>The crayon-style picker.</summary>
		Crayon,
	};

	/// <summary>Specifies a bitmask of the color-picking modes available in the shared color panel.</summary>
	[NoMacCatalyst]
	[Flags]
	[NativeName ("NSColorPanelOptions")]
	[Native]
	public enum NSColorPanelFlags : ulong {
		/// <summary>The grayscale picker.</summary>
		Gray = 0x00000001,
		/// <summary>The red-green-blue (RGB) sliders picker.</summary>
		RGB = 0x00000002,
		/// <summary>The cyan-magenta-yellow-black (CMYK) sliders picker.</summary>
		CMYK = 0x00000004,
		/// <summary>The hue-saturation-brightness (HSB) sliders picker.</summary>
		HSB = 0x00000008,
		/// <summary>The custom color list/palette picker.</summary>
		CustomPalette = 0x00000010,
		/// <summary>The named color list picker.</summary>
		ColorList = 0x00000020,
		/// <summary>The color wheel picker.</summary>
		Wheel = 0x00000040,
		/// <summary>The crayon-style picker.</summary>
		Crayon = 0x00000080,
		/// <summary>All of the available color-picking modes.</summary>
		All = 0x0000ffff,
	}


	#endregion
	#region NSDocument

	/// <summary>Specifies the kind of change that was made to a document, reported when updating the document's change count.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSDocumentChangeType : ulong {
		/// <summary>A change was made to the document.</summary>
		Done,
		/// <summary>A change was undone.</summary>
		Undone,
		/// <summary>The document was synchronized with its file, resetting the change count.</summary>
		Cleared,
		/// <summary>The document's contents were read from another file, such as during a revert-to-saved or duplicate operation.</summary>
		ReadOtherContents,
		/// <summary>The document was saved automatically.</summary>
		Autosaved,
		/// <summary>A previously undone change was redone.</summary>
		Redone,
		/// <summary>The change can be discarded instead of prompting the user to save it, but still marks the document as edited.</summary>
		Discardable = 256, /* New in Lion */
	}

	/// <summary>Specifies the kind of save operation being performed on a document.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSaveOperationType : ulong {
		/// <summary>Saves the document to its existing location.</summary>
		Save,
		/// <summary>Saves the document to a new location chosen by the user, which becomes the document's new location.</summary>
		SaveAs,
		/// <summary>Saves a copy of the document to a new location chosen by the user, without changing the document's current location.</summary>
		SaveTo,
		/// <summary>The document is being saved automatically. Deprecated in favor of <see cref="F:AppKit.NSSaveOperationType.Elsewhere" />.</summary>
		Autosave = 3,   /* Deprecated name in Lion */
		/// <summary>The document is being saved automatically to a location other than its current location.</summary>
		Elsewhere = 3,  /* New Lion name */
		/// <summary>The document is being saved automatically, in place, to its current location.</summary>
		InPlace = 4,    /* New in Lion */
		/// <summary>The document is being saved automatically to a new location chosen by the system, such as during version browsing.</summary>
		AutoSaveAs = 5, /* New in Mountain Lion */
	}

	#endregion

	#region NSBezelPath

	/// <summary>Specifies the shape of the endpoints of an open path.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSLineCapStyle : ulong {
		/// <summary>Ends the path at the endpoint.</summary>
		Butt,
		/// <summary>Uses a semicircular cap centered on the endpoint.</summary>
		Round,
		/// <summary>Uses a square cap that extends beyond the endpoint.</summary>
		Square,
	}

	/// <summary>Specifies the shape used to join connected path segments.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSLineJoinStyle : ulong {
		/// <summary>Joins segments by extending their outer edges until they meet.</summary>
		Miter,
		/// <summary>Joins segments with a rounded corner.</summary>
		Round,
		/// <summary>Joins segments with a beveled corner.</summary>
		Bevel,
	}

	/// <summary>Specifies the rule used to determine which areas of a path are filled.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindingRule : ulong {
		/// <summary>Fills areas using the nonzero winding rule.</summary>
		NonZero,
		/// <summary>Fills areas using the even-odd winding rule.</summary>
		EvenOdd,
	}

	/// <summary>Specifies the kind of path segment represented by an element of a <see cref="T:AppKit.NSBezierPath" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSBezierPathElement : ulong {
		/// <summary>Begins a new subpath at the given point, without drawing a segment.</summary>
		MoveTo,
		/// <summary>Draws a straight line segment to the given point.</summary>
		LineTo,
		/// <summary>Draws a cubic Bézier curve segment to the given point, using two control points.</summary>
		CurveTo,
		/// <summary>Closes the current subpath by drawing a straight line back to its starting point.</summary>
		ClosePath,
		/// <summary>Draws a quadratic Bézier curve segment to the given point, using a single control point.</summary>
		QuadraticCurveTo,
	}
	#endregion

	#region NSRulerView
	/// <summary>Specifies the orientation of a ruler view.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSRulerOrientation : ulong {
		/// <summary>A horizontal ruler.</summary>
		Horizontal,
		/// <summary>A vertical ruler.</summary>
		Vertical,
	}
	#endregion

	#region NSGestureRecognizer
	/// <summary>Specifies the current state of a gesture recognizer's recognition state machine.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSGestureRecognizerState : long {
		/// <summary>The gesture recognizer has not yet recognized its gesture, but may still do so.</summary>
		Possible,
		/// <summary>The gesture recognizer has recognized the start of its gesture.</summary>
		Began,
		/// <summary>The gesture recognizer has recognized a change in a continuous gesture that is already in progress.</summary>
		Changed,
		/// <summary>The gesture recognizer has recognized the end of its gesture and reported a successful completion.</summary>
		Ended,
		/// <summary>The gesture recognizer's recognition of a continuous gesture was cancelled, without ending normally.</summary>
		Cancelled,
		/// <summary>The gesture recognizer failed to recognize its gesture.</summary>
		Failed,
		/// <summary>The gesture recognizer recognized a discrete gesture. Equivalent to <see cref="F:AppKit.NSGestureRecognizerState.Ended" />.</summary>
		Recognized = NSGestureRecognizerState.Ended,
	}
	#endregion

	#region NSStackLayout
	/// <summary>Specifies the axis along which user interface elements are laid out.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSUserInterfaceLayoutOrientation : long {
		/// <summary>Elements are laid out along the horizontal axis.</summary>
		Horizontal = 0,
		/// <summary>Elements are laid out along the vertical axis.</summary>
		Vertical = 1,
	}

	// NSStackView.h:typedef float NSStackViewVisibilityPriority
	/// <summary>Provides predefined visibility-priority values that can be cast to <see cref="float"/> for use with <see cref="NSStackView"/> APIs.</summary>
	[NoMacCatalyst]
	public enum NSStackViewVisibilityPriority : int {
		/// <summary>The view must remain attached to the stack view.</summary>
		MustHold = 1000,
		/// <summary>The view is detached only when necessary.</summary>
		DetachOnlyIfNecessary = 900,
		/// <summary>The view is not visible.</summary>
		NotVisible = 0,
	}

	/// <summary>Specifies the "gravity area" of an <see cref="T:AppKit.NSStackView" /> to which a view is added, controlling its position relative to other views.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSStackViewGravity : long {
		/// <summary>Places the view in the leading gravity area of a vertical stack view, at its top.</summary>
		Top = 1,
		/// <summary>Places the view in the leading gravity area of the stack view.</summary>
		Leading = 1,
		/// <summary>Places the view in the center gravity area of the stack view.</summary>
		Center = 2,
		/// <summary>Places the view in the trailing gravity area of a vertical stack view, at its bottom.</summary>
		Bottom = 3,
		/// <summary>Places the view in the trailing gravity area of the stack view.</summary>
		Trailing = 3,
	}
	#endregion

	/// <summary>Specifies how the arranged views of an <see cref="T:AppKit.NSStackView" /> are sized and spaced along the stack's axis.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSStackViewDistribution : long {
		/// <summary>Views are positioned according to their assigned gravity areas, rather than an automatic distribution.</summary>
		GravityAreas = -1,
		/// <summary>Views are resized to fill the available space based on their content-hugging and compression-resistance priorities.</summary>
		Fill = 0,
		/// <summary>Views are resized equally to fill the available space.</summary>
		FillEqually,
		/// <summary>Views are resized proportionally, based on their initial sizes, to fill the available space.</summary>
		FillProportionally,
		/// <summary>Views keep their intrinsic size, and any extra space is distributed equally between them.</summary>
		EqualSpacing,
		/// <summary>Views keep their intrinsic size, and any extra space is distributed so that the centers of the views are equally spaced.</summary>
		EqualCentering,
	}

	/// <summary>Specifies a bitmask of the drag-and-drop operations that a dragging source allows a destination to perform.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSDragOperation : ulong {
		/// <summary>No operation is allowed; the drop is refused.</summary>
		None,
		/// <summary>The dragged data is copied to the destination.</summary>
		Copy = 1,
		/// <summary>A link is created from the destination to the original data.</summary>
		Link = 2,
		/// <summary>The destination performs a generic operation defined by the destination application.</summary>
		Generic = 4,
		/// <summary>The destination performs an operation defined privately between the source and destination.</summary>
		Private = 8,
		/// <summary>An obsolete mask combining the <see cref="F:AppKit.NSDragOperation.Copy" />, <see cref="F:AppKit.NSDragOperation.Link" />, <see cref="F:AppKit.NSDragOperation.Generic" />, and <see cref="F:AppKit.NSDragOperation.Private" /> operations.</summary>
		AllObsolete = 15,
		/// <summary>The dragged data is moved from the source to the destination.</summary>
		Move = 16,
		/// <summary>The dragged data is deleted.</summary>
		Delete = 32,
		/// <summary>All drag operations are allowed; the destination chooses the most appropriate one.</summary>
		All = ulong.MaxValue,
	}

	/// <summary>Specifies the horizontal alignment of text within its container.</summary>
	[NoMacCatalyst]
	[Native (ConvertToNative = "NSTextAlignmentExtensions.ToNative", ConvertToManaged = "NSTextAlignmentExtensions.ToManaged")]
	public enum NSTextAlignment : ulong {
		/// <summary>Aligns text to the left edge of its container.</summary>
		Left = 0,
		/// <summary>Aligns text to the right edge of its container.</summary>
		Right = 1,
		/// <summary>Centers text within its container.</summary>
		Center = 2,
		/// <summary>Justifies text so that both edges align with the container, except for the last line.</summary>
		Justified = 3,
		/// <summary>Aligns text according to the natural writing direction of its script (left for left-to-right languages, right for right-to-left languages).</summary>
		Natural = 4,
	}

	/// <summary>Specifies the kind of action, such as pressing Return or Tab, that ended editing in a text field.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextMovement : long {
		/// <summary>Editing ended for a reason other than one of the other defined movements.</summary>
		Other = 0,
		/// <summary>Editing ended because the user pressed Return.</summary>
		Return = 0x10,
		/// <summary>Editing ended because the user pressed Tab.</summary>
		Tab = 0x11,
		/// <summary>Editing ended because the user pressed Shift-Tab, moving backward.</summary>
		Backtab = 0x12,
		/// <summary>Editing ended because the user pressed the left arrow key.</summary>
		Left = 0x13,
		/// <summary>Editing ended because the user pressed the right arrow key.</summary>
		Right = 0x14,
		/// <summary>Editing ended because the user pressed the up arrow key.</summary>
		Up = 0x15,
		/// <summary>Editing ended because the user pressed the down arrow key.</summary>
		Down = 0x16,
		/// <summary>Editing ended because the user pressed Escape, cancelling the edit.</summary>
		Cancel = 0x17,
	}

	/// <summary>Specifies a bitmask of the properties of a menu item that should be observed for changes, or included in comparisons.</summary>
	[NoMacCatalyst]
	[Flags]
	[NativeName ("NSMenuProperties")]
	[Native]
	public enum NSMenuProperty : ulong {
		/// <summary>The menu item's title.</summary>
		Title = 1 << 0,
		/// <summary>The menu item's attributed title.</summary>
		AttributedTitle = 1 << 1,
		/// <summary>The menu item's key equivalent.</summary>
		KeyEquivalent = 1 << 2,
		/// <summary>The menu item's image.</summary>
		Image = 1 << 3,
		/// <summary>Whether the menu item is enabled.</summary>
		Enabled = 1 << 4,
		/// <summary>The menu item's accessibility description.</summary>
		AccessibilityDescription = 1 << 5,
	}

	/// <summary>Specifies how a font renders glyphs and computes glyph advancements.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSFontRenderingMode : ulong {
		/// <summary>Uses the default rendering mode for the current context.</summary>
		Default,
		/// <summary>Renders glyphs with antialiasing.</summary>
		Antialiased,
		/// <summary>Renders glyphs using integer advancements between characters, without antialiasing.</summary>
		IntegerAdvancements,
		/// <summary>Renders glyphs with antialiasing, using integer advancements between characters.</summary>
		AntialiasedIntegerAdvancements,
	}

	/// <summary>Specifies a bitmask of options for reading an object from the pasteboard.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSPasteboardReadingOptions : ulong {
		/// <summary>Reads the pasteboard item as raw data.</summary>
		AsData = 0,
		/// <summary>Reads the pasteboard item as a string.</summary>
		AsString = 1,
		/// <summary>Reads the pasteboard item as a property list.</summary>
		AsPropertyList = 2,
		/// <summary>Reads the pasteboard item as a keyed archive.</summary>
		AsKeyedArchive = 4,
	}

	// Convenience enum, untyped in ObjC
	/// <summary>Specifies the dash pattern used when drawing an underline or strikethrough beneath text.</summary>
	[NoMacCatalyst]
	public enum NSUnderlinePattern : int {
		/// <summary>A solid line.</summary>
		Solid = 0x0000,
		/// <summary>A dotted line.</summary>
		Dot = 0x0100,
		/// <summary>A dashed line.</summary>
		Dash = 0x0200,
		/// <summary>A line alternating dashes and dots.</summary>
		DashDot = 0x0300,
		/// <summary>A line alternating dashes and pairs of dots.</summary>
		DashDotDot = 0x0400,
	}

	/// <summary>Specifies the direction associated with a selection at a line boundary.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSelectionAffinity : ulong {
		/// <summary>The selection is associated with the preceding line.</summary>
		Upstream,
		/// <summary>The selection is associated with the following line.</summary>
		Downstream,
	}

	/// <summary>Specifies the unit used when modifying a text selection.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSelectionGranularity : ulong {
		/// <summary>Selects text by character.</summary>
		Character,
		/// <summary>Selects text by word.</summary>
		Word,
		/// <summary>Selects text by paragraph.</summary>
		Paragraph,
	}

	#region NSTrackingArea
	/// <summary>Specifies a bitmask of options that control when and how an <see cref="T:AppKit.NSTrackingArea" /> generates tracking events.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSTrackingAreaOptions : ulong {
		/// <summary>Generates events when the pointer enters or exits the tracking area.</summary>
		MouseEnteredAndExited = 0x01,
		/// <summary>Generates events when the pointer moves within the tracking area.</summary>
		MouseMoved = 0x02,
		/// <summary>Generates a cursor-update event when the pointer enters the tracking area.</summary>
		CursorUpdate = 0x04,
		/// <summary>The tracking area is active only while its owning view is the first responder.</summary>
		ActiveWhenFirstResponder = 0x10,
		/// <summary>The tracking area is active only while its owning window is the key window.</summary>
		ActiveInKeyWindow = 0x20,
		/// <summary>The tracking area is active only while its owning application is the active application.</summary>
		ActiveInActiveApp = 0x40,
		/// <summary>The tracking area is always active, regardless of the key window or active application.</summary>
		ActiveAlways = 0x80,
		/// <summary>Assumes the pointer starts inside the tracking area, allowing a mouse-exited event when the pointer leaves without a preceding mouse-entered event.</summary>
		AssumeInside = 0x100,
		/// <summary>Automatically updates the tracking area's rectangle to always match the visible portion of its owning view.</summary>
		InVisibleRect = 0x200,
		/// <summary>Continues to generate tracking events even while the mouse button is held down and being dragged.</summary>
		EnabledDuringMouseDrag = 0x400,
	}
	#endregion

	/// <summary>Specifies the direction in which text layout sweeps across a line.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSLineSweepDirection : ulong {
		/// <summary>Sweeps toward the left.</summary>
		NSLineSweepLeft,
		/// <summary>Sweeps toward the right.</summary>
		NSLineSweepRight,
		/// <summary>Sweeps downward.</summary>
		NSLineSweepDown,
		/// <summary>Sweeps upward.</summary>
		NSLineSweepUp,
	}

	/// <summary>Specifies the direction in which text layout moves between lines.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSLineMovementDirection : ulong {
		/// <summary>Does not move between lines.</summary>
		None,
		/// <summary>Moves to the left.</summary>
		Left,
		/// <summary>Moves to the right.</summary>
		Right,
		/// <summary>Moves downward.</summary>
		Down,
		/// <summary>Moves upward.</summary>
		Up,
	}

	/// <summary>Specifies the compression scheme used when writing TIFF image data.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTiffCompression : ulong {
		/// <summary>No compression.</summary>
		None = 1,
		/// <summary>CCITT Group 3 fax compression.</summary>
		CcittFax3 = 3,
		/// <summary>CCITT Group 4 fax compression.</summary>
		CcittFax4 = 4,
		/// <summary>LZW compression.</summary>
		Lzw = 5,

		/// <summary>A legacy JPEG-in-TIFF compression code that is no longer supported for input or output.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		Jpeg = 6,
		/// <summary>NeXT's proprietary 2-bit run-length encoding scheme.</summary>
		Next = 32766,
		/// <summary>PackBits (Macintosh RLE) compression.</summary>
		PackBits = 32773,

		/// <summary>A legacy JPEG-in-TIFF compression code that is no longer supported for input or output.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		OldJpeg = 32865,
	}

	/// <summary>Specifies the file format used to store bitmap image data.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSBitmapImageFileType : ulong {
		/// <summary>The TIFF (Tagged Image File Format) format.</summary>
		Tiff,
		/// <summary>The BMP (Windows bitmap) format.</summary>
		Bmp,
		/// <summary>The GIF (Graphics Interchange Format) format.</summary>
		Gif,
		/// <summary>The JPEG format.</summary>
		Jpeg,
		/// <summary>The PNG (Portable Network Graphics) format.</summary>
		Png,
		/// <summary>The JPEG 2000 format.</summary>
		Jpeg2000,
	}

	/// <summary>Specifies the progress of an incremental image-representation loading operation.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageRepLoadStatus : long {
		/// <summary>The image data's type could not be determined.</summary>
		UnknownType = -1,
		/// <summary>The image representation is still reading the image's header.</summary>
		ReadingHeader = -2,
		/// <summary>The image representation requires all of the remaining data before it can continue loading.</summary>
		WillNeedAllData = -3,
		/// <summary>The image data is invalid or malformed.</summary>
		InvalidData = -4,
		/// <summary>The image data ended unexpectedly before loading completed.</summary>
		UnexpectedEOF = -5,
		/// <summary>The image finished loading successfully.</summary>
		Completed = -6,
	}

	/// <summary>Specifies a bitmask describing the pixel layout and sample encoding of an <see cref="T:AppKit.NSBitmapImageRep" />.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSBitmapFormat : ulong {
		/// <summary>The alpha component is stored first in each pixel, rather than last.</summary>
		AlphaFirst = 1,
		/// <summary>The color components are not premultiplied by the alpha component.</summary>
		AlphaNonpremultiplied = 2,
		/// <summary>Each sample is stored as a floating-point value, rather than an integer.</summary>
		FloatingPointSamples = 4,

		/// <summary>16-bit samples are stored in little-endian byte order.</summary>
		LittleEndian16Bit = 1 << 8,
		/// <summary>32-bit samples are stored in little-endian byte order.</summary>
		LittleEndian32Bit = 1 << 9,
		/// <summary>16-bit samples are stored in big-endian byte order.</summary>
		BigEndian16Bit = 1 << 10,
		/// <summary>32-bit samples are stored in big-endian byte order.</summary>
		BigEndian32Bit = 1 << 11,
	}

	/// <summary>Specifies the orientation of printed pages.</summary>
	[NoMacCatalyst]
	[NativeName ("NSPaperOrientation")]
	[Native]
	public enum NSPrintingOrientation : ulong {
		/// <summary>Prints pages in portrait orientation.</summary>
		Portrait,
		/// <summary>Prints pages in landscape orientation.</summary>
		Landscape,
	}

	/// <summary>Specifies how printed content is paginated along one axis.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPrintingPaginationMode : ulong {
		/// <summary>Divides the content into equal-sized page rectangles.</summary>
		Auto,
		/// <summary>Scales the content to produce one row or column of pages.</summary>
		Fit,
		/// <summary>Clips the content to produce one row or column of pages.</summary>
		Clip,
	}

	/// <summary>Specifies the status of a table in a printer's PostScript Printer Description.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPrinterTableStatus : ulong {
		/// <summary>The table is available.</summary>
		Ok,
		/// <summary>The table was not found.</summary>
		NotFound,
		/// <summary>An error occurred while accessing the table.</summary>
		Error,
	}

	/// <summary>Specifies where a scroll view's scroll arrows are positioned.</summary>
	[NoMacCatalyst]
	[Native]
	[Deprecated (PlatformName.MacOSX, 10, 14)]
	public enum NSScrollArrowPosition : ulong {
		/// <summary>Places the scroll arrows at the maximum end (bottom or right) of the scroller.</summary>
		MaxEnd = 0,
		/// <summary>Places the scroll arrows at the minimum end (top or left) of the scroller.</summary>
		MinEnd = 1,
		/// <summary>Uses the system's default scroll arrow position.</summary>
		DefaultSetting = 0,
		/// <summary>Does not display scroll arrows.</summary>
		None = 2,
	}

	/// <summary>Specifies which parts of a scroller are usable.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSUsableScrollerParts : ulong {
		/// <summary>No part of the scroller is usable; the scroller is disabled.</summary>
		NoScroller,
		/// <summary>Only the scroll arrows are usable.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		OnlyArrows,
		/// <summary>All parts of the scroller, including the knob and arrows, are usable.</summary>
		All,
	}

	/// <summary>Specifies a part of a scroller, such as an arrow, the knob, or the knob slot.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSScrollerPart : ulong {
		/// <summary>No part of the scroller.</summary>
		None,
		/// <summary>The area of the knob slot above or to the left of the knob, which pages up or left when clicked.</summary>
		DecrementPage,
		/// <summary>The scroller's knob, which is dragged to scroll continuously.</summary>
		Knob,
		/// <summary>The area of the knob slot below or to the right of the knob, which pages down or right when clicked.</summary>
		IncrementPage,
		/// <summary>The arrow that scrolls up or left by a small increment.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		DecrementLine,
		/// <summary>The arrow that scrolls down or right by a small increment.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		IncrementLine,
		/// <summary>The track (slot) within which the knob moves.</summary>
		KnobSlot,
	}

	/// <summary>Specifies one of a scroller's two scroll arrows.</summary>
	[NoMacCatalyst]
	[Native]
	[Deprecated (PlatformName.MacOSX, 10, 14)]
	public enum NSScrollerArrow : ulong {
		/// <summary>The arrow that increments the scroll position.</summary>
		IncrementArrow,
		/// <summary>The arrow that decrements the scroll position.</summary>
		DecrementArrow,
	}

	/// <summary>Specifies the order in which pages are printed.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPrintingPageOrder : long {
		/// <summary>Prints pages from front to back.</summary>
		Descending = -1,
		/// <summary>Prints pages in the order received by the spooler without rearranging them.</summary>
		Special,
		/// <summary>Prints pages from back to front.</summary>
		Ascending,
		/// <summary>The page order is unknown.</summary>
		Unknown,
	}

	/// <summary>Specifies a bitmask of the accessory panels shown in the standard print panel.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSPrintPanelOptions : long {
		/// <summary>Shows the control for choosing the number of copies to print.</summary>
		ShowsCopies = 1,
		/// <summary>Shows the control for choosing which pages to print.</summary>
		ShowsPageRange = 2,
		/// <summary>Shows the control for choosing the paper size.</summary>
		ShowsPaperSize = 4,
		/// <summary>Shows the control for choosing the page orientation.</summary>
		ShowsOrientation = 8,
		/// <summary>Shows the control for choosing the scale factor.</summary>
		ShowsScaling = 16,
		/// <summary>Shows the control for choosing whether to print the entire document or only the current selection.</summary>
		ShowsPrintSelection = 32,
		/// <summary>Shows the page setup accessory controls.</summary>
		ShowsPageSetupAccessory = 256,
		/// <summary>Shows a preview of the document being printed.</summary>
		ShowsPreview = 131072,
	}

	/// <summary>Specifies how a text block value is interpreted.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextBlockValueType : ulong {
		/// <summary>The value is an absolute number of points.</summary>
		Absolute,
		/// <summary>The value is a percentage of the containing text block.</summary>
		Percentage,
	}

	/// <summary>Identifies a dimension of a text block.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextBlockDimension : ulong {
		/// <summary>The text block's width.</summary>
		Width = 0,
		/// <summary>The minimum width of the text block.</summary>
		MinimumWidth = 1,
		/// <summary>The maximum width of the text block.</summary>
		MaximumWidth = 2,
		/// <summary>The text block's height.</summary>
		Height = 4,
		/// <summary>The minimum height of the text block.</summary>
		MinimumHeight = 5,
		/// <summary>The maximum height of the text block.</summary>
		MaximumHeight = 6,
	}

	/// <summary>Identifies a layer of a text block.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextBlockLayer : long {
		/// <summary>The padding layer between the text and the border.</summary>
		Padding = -1,
		/// <summary>The border layer surrounding the padding.</summary>
		Border,
		/// <summary>The margin layer outside the border.</summary>
		Margin,
	}

	/// <summary>Specifies the vertical alignment of content in a text block.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextBlockVerticalAlignment : ulong {
		/// <summary>Aligns content with the top of the text block.</summary>
		Top,
		/// <summary>Centers content vertically in the text block.</summary>
		Middle,
		/// <summary>Aligns content with the bottom of the text block.</summary>
		Bottom,
		/// <summary>Aligns content with the text block's baseline.</summary>
		Baseline,
	}

	/// <summary>Specifies how a text table calculates column widths.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextTableLayoutAlgorithm : ulong {
		/// <summary>Calculates column widths based on the table's contents.</summary>
		Automatic,
		/// <summary>Uses the table's specified column widths without considering its contents.</summary>
		Fixed,
	}

	/// <summary>Specifies a bitmask of stylistic traits, such as weight, slant, and font-family classification, associated with a font descriptor.</summary>
	[NoMacCatalyst]
	[Flags]
	[NativeName ("NSFontDescriptorSymbolicTraits")]
#if XAMCORE_5_0
	public enum NSFontSymbolicTraits : uint {
#else
	public enum NSFontSymbolicTraits : int { // uint32_t NSFontSymbolicTraits
#endif
		/// <summary>The font has an italic or oblique style.</summary>
		ItalicTrait = (1 << 0),
		/// <summary>The font has a bold weight.</summary>
		BoldTrait = (1 << 1),
		/// <summary>The font has an expanded (wider) character width.</summary>
		ExpandedTrait = (1 << 5),
		/// <summary>The font has a condensed (narrower) character width.</summary>
		CondensedTrait = (1 << 6),
		/// <summary>The font uses monospaced (fixed-pitch) glyphs.</summary>
		MonoSpaceTrait = (1 << 10),
		/// <summary>The font contains vertical glyph variants for vertical text layout.</summary>
		VerticalTrait = (1 << 11),
		/// <summary>The font has been optimized for rendering in user interfaces.</summary>
		UIOptimizedTrait = (1 << 12),
		/// <summary>The font is optimized for use with tight line spacing.</summary>
		TraitTightLeading = 1 << 15,
		/// <summary>The font is optimized for use with loose line spacing.</summary>
		TraitLooseLeading = 1 << 16,
		/// <summary>The font is emphasized, equivalent to <see cref="F:AppKit.NSFontSymbolicTraits.BoldTrait" />.</summary>
		TraitEmphasized = BoldTrait,
		/// <summary>The font's design classification is not known.</summary>
		UnknownClass = 0 << 28,
		/// <summary>The font belongs to the old-style serif design classification, such as Garamond.</summary>
		OldStyleSerifsClass = 1 << 28,
		/// <summary>The font belongs to the transitional serif design classification, such as Times.</summary>
		TransitionalSerifsClass = 2 << 28,
		/// <summary>The font belongs to the modern serif design classification, such as Bodoni.</summary>
		ModernSerifsClass = 3 << 28,
		/// <summary>The font belongs to the Clarendon (slab) serif design classification.</summary>
		ClarendonSerifsClass = 4 << 28,
		/// <summary>The font belongs to the slab serif design classification.</summary>
		SlabSerifsClass = 5 << 28,
		/// <summary>The font belongs to the freeform serif design classification.</summary>
		FreeformSerifsClass = 7 << 28,
		/// <summary>The font belongs to the sans-serif design classification.</summary>
#if XAMCORE_5_0
		SansSerifClass = 8u << 28,
#else
		SansSerifClass = 8 << 28,
#endif
		/// <summary>The font belongs to the ornamental design classification, such as display or decorative faces.</summary>
#if XAMCORE_5_0
		OrnamentalsClass = 9u << 28,
#else
		OrnamentalsClass = 9 << 28,
#endif
		/// <summary>The font belongs to the script design classification, resembling handwriting.</summary>
#if XAMCORE_5_0
		ScriptsClass = 10u << 28,
#else
		ScriptsClass = 10 << 28,
#endif
		/// <summary>The font belongs to the symbolic design classification, such as an icon or dingbat font.</summary>
#if XAMCORE_5_0
		SymbolicClass = 12u << 28,
#else
		SymbolicClass = 12 << 28,
#endif

		/// <summary>A mask that isolates the font-family classification bits.</summary>
#if XAMCORE_5_0
		FamilyClassMask = 15u << 28,
#else
		FamilyClassMask = (int) -268435456,
#endif
	}

	/// <summary>Specifies a bitmask of stylistic traits used with the legacy <see cref="T:AppKit.NSFontManager" /> font-selection APIs.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSFontTraitMask : ulong {
		/// <summary>The font has an italic or oblique style.</summary>
		Italic = 1,
		/// <summary>The font has a bold weight.</summary>
		Bold = 2,
		/// <summary>The font does not have a bold weight.</summary>
		Unbold = 4,
		/// <summary>The font does not use a standard character set.</summary>
		NonStandardCharacterSet = 8,
		/// <summary>The font has a narrow character width.</summary>
		Narrow = 0x10,
		/// <summary>The font has an expanded (wider) character width.</summary>
		Expanded = 0x20,
		/// <summary>The font has a condensed (narrower) character width.</summary>
		Condensed = 0x40,
		/// <summary>The font uses small capital letters.</summary>
		SmallCaps = 0x80,
		/// <summary>The font is designed for use at large, poster-like sizes.</summary>
		Poster = 0x100,
		/// <summary>The font has a compressed character width.</summary>
		Compressed = 0x200,
		/// <summary>The font uses monospaced (fixed-pitch) glyphs.</summary>
		FixedPitch = 0x400,
		/// <summary>The font does not have an italic or oblique style.</summary>
		Unitalic = 0x1000000,
	}

	/// <summary>Specifies options for writing objects to a pasteboard.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSPasteboardWritingOptions : ulong {
		/// <summary>The object promises to provide its pasteboard data later.</summary>
		WritingPromised = 1 << 9,
	}

	/// <summary>Specifies how a toolbar displays its items.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSToolbarDisplayMode : ulong {
		/// <summary>Uses the toolbar's default display mode.</summary>
		Default,
		/// <summary>Displays both the icon and label for each toolbar item.</summary>
		IconAndLabel,
		/// <summary>Displays only the icon for each toolbar item.</summary>
		Icon,
		/// <summary>Displays only the label for each toolbar item.</summary>
		Label,
	}

	/// <summary>Specifies the size of toolbar items.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSToolbarSizeMode : ulong {
		/// <summary>Uses the toolbar's default item size.</summary>
		Default,
		/// <summary>Uses regular-sized toolbar items.</summary>
		Regular,
		/// <summary>Uses small toolbar items.</summary>
		Small,
	}

	/// <summary>Specifies how a table view automatically resizes its columns to fill the available width.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTableViewColumnAutoresizingStyle : ulong {
		/// <summary>Columns are not automatically resized.</summary>
		None = 0,
		/// <summary>All resizable columns are resized uniformly.</summary>
		Uniform,
		/// <summary>Columns are resized one at a time, starting with the last column, in order.</summary>
		Sequential,
		/// <summary>Columns are resized one at a time, starting with the first column, in reverse order.</summary>
		ReverseSequential,
		/// <summary>Only the last column is resized.</summary>
		LastColumnOnly,
		/// <summary>Only the first column is resized.</summary>
		FirstColumnOnly,
	}

	/// <summary>Specifies how a table view highlights selected rows.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTableViewSelectionHighlightStyle : long {
		/// <summary>Does not highlight selected rows.</summary>
		None = -1,
		/// <summary>Uses the regular selection highlight style.</summary>
		Regular = 0,
		/// <summary>Uses the source-list selection highlight style.</summary>
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Set 'NSTableView.Style' to 'NSTableViewStyle.SourceList' instead.")]
		SourceList = 1,
	}

	/// <summary>Specifies the visual feedback style used to indicate a valid drop target while dragging over a table view.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTableViewDraggingDestinationFeedbackStyle : long {
		/// <summary>No drop-target feedback is drawn.</summary>
		None = -1,
		/// <summary>Draws the regular drop-target highlight.</summary>
		Regular = 0,
		/// <summary>Draws the source-list style drop-target highlight.</summary>
		SourceList = 1,
		/// <summary>Opens a gap between rows at the proposed drop location.</summary>
		FeedbackStyleGap = 2,
	}

	/// <summary>Specifies where a table view performs a drop operation.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTableViewDropOperation : ulong {
		/// <summary>Performs the drop on the specified row.</summary>
		On,
		/// <summary>Performs the drop above the specified row.</summary>
		Above,
	}

	/// <summary>Specifies how a table column can be resized.</summary>
	[NoMacCatalyst]
	[Flags]
	[NativeName ("NSTableColumnResizingOptions")]
	[Native]
	public enum NSTableColumnResizing : long {
		/// <summary>Represents -1, which sets all table column resizing option bits.</summary>
		None = -1,
		/// <summary>Allows the table view to resize the column automatically.</summary>
		Autoresizing = (1 << 0),
		/// <summary>Allows the user to resize the column.</summary>
		UserResizingMask = (1 << 1),
	}

	/// <summary>Specifies which grid lines a table view draws.</summary>
	[NoMacCatalyst]
	[Flags]
	[NativeName ("NSTableViewGridLineStyle")]
	[Native]
	public enum NSTableViewGridStyle : ulong {
		/// <summary>Does not draw grid lines.</summary>
		None = 0,
		/// <summary>Draws solid vertical grid lines.</summary>
		SolidVerticalLine = 1 << 0,
		/// <summary>Draws solid horizontal grid lines.</summary>
		SolidHorizontalLine = 1 << 1,
		/// <summary>Draws dashed horizontal grid lines.</summary>
		DashedHorizontalGridLine = 1 << 3,
	}

	/// <summary>Specifies how a gradient extends beyond its starting and ending locations.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSGradientDrawingOptions : ulong {
		/// <summary>Does not extend the gradient beyond its starting or ending location.</summary>
		None = 0,
		/// <summary>Extends the gradient before its starting location using the starting color.</summary>
		BeforeStartingLocation = (1 << 0),
		/// <summary>Extends the gradient after its ending location using the ending color.</summary>
		AfterEndingLocation = (1 << 1),
	}

	/// <summary>Specifies how an image is aligned within its frame when it is not scaled to fill the entire frame.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageAlignment : ulong {
		/// <summary>Centers the image within its frame.</summary>
		Center = 0,
		/// <summary>Aligns the image with the top edge of its frame, centered horizontally.</summary>
		Top,
		/// <summary>Aligns the image with the top-left corner of its frame.</summary>
		TopLeft,
		/// <summary>Aligns the image with the top-right corner of its frame.</summary>
		TopRight,
		/// <summary>Aligns the image with the left edge of its frame, centered vertically.</summary>
		Left,
		/// <summary>Aligns the image with the bottom edge of its frame, centered horizontally.</summary>
		Bottom,
		/// <summary>Aligns the image with the bottom-left corner of its frame.</summary>
		BottomLeft,
		/// <summary>Aligns the image with the bottom-right corner of its frame.</summary>
		BottomRight,
		/// <summary>Aligns the image with the right edge of its frame, centered vertically.</summary>
		Right,
	}

	/// <summary>Specifies the frame drawn around an image.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageFrameStyle : ulong {
		/// <summary>Draws no frame.</summary>
		None = 0,
		/// <summary>Draws a frame suitable for a photograph.</summary>
		Photo,
		/// <summary>Draws a gray bezel frame.</summary>
		GrayBezel,
		/// <summary>Draws a grooved frame.</summary>
		Groove,
		/// <summary>Draws a button-style frame.</summary>
		Button,
	}

	/// <summary>Specifies when a speech synthesizer pauses or stops speaking.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSpeechBoundary : ulong {
		/// <summary>Pauses or stops speaking immediately.</summary>
		Immediate = 0,
		/// <summary>Pauses or stops speaking at the next word boundary.</summary>
		Word = 1,
		/// <summary>Pauses or stops speaking at the next sentence boundary.</summary>
		Sentence,
	}

	/// <summary>Specifies the visual style of a split view divider.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSplitViewDividerStyle : long {
		/// <summary>Uses a thick divider.</summary>
		Thick = 1,
		/// <summary>Uses a thin divider.</summary>
		Thin = 2,
		/// <summary>Uses a pane-splitter divider.</summary>
		PaneSplitter = 3,
	}

	/// <summary>Specifies the standard behavior of a split view item, such as a sidebar or inspector pane, within a split view controller.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSplitViewItemBehavior : long {
		/// <summary>The item has no special behavior; it participates in the split view like an ordinary pane.</summary>
		Default,
		/// <summary>The item behaves as a sidebar, with the system-defined sidebar appearance and collapsing behavior.</summary>
		Sidebar,
		/// <summary>The item behaves as the primary content list pane alongside a sidebar.</summary>
		ContentList,
		/// <summary>The item behaves as an inspector pane, with the system-defined inspector appearance and collapsing behavior.</summary>
		Inspector,
	}

	/// <summary>Specifies how an image is scaled to fit its frame.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageScaling : ulong {
		/// <summary>Scales the image down proportionally when it is larger than the frame.</summary>
		ProportionallyDown = 0,
		/// <summary>Scales the image independently along each axis to fill the frame.</summary>
		AxesIndependently,
		/// <summary>Does not scale the image.</summary>
		None,
		/// <summary>Scales the image proportionally up or down to fit the frame.</summary>
		ProportionallyUpOrDown,
	}

	/// <summary>Specifies the visual appearance of a segmented control.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSegmentStyle : long {
		/// <summary>The system chooses the most appropriate style based on the control's context.</summary>
		Automatic = 0,
		/// <summary>A style with rounded ends, resembling the classic segmented control appearance.</summary>
		Rounded = 1,
		/// <summary>A textured, rounded style; superseded by <see cref="F:AppKit.NSSegmentStyle.TexturedSquare" />, which uses the same artwork.</summary>
		TexturedRounded = 2,
		/// <summary>A rounded-rectangle style appropriate for scope bars and title bar accessories.</summary>
		RoundRect = 3,
		/// <summary>A textured, square style appropriate for toolbars and title bar areas.</summary>
		TexturedSquare = 4,
		/// <summary>A style with fully rounded, pill-shaped ends.</summary>
		Capsule = 5,
		/// <summary>A small, square style suited to compact user interfaces.</summary>
		SmallSquare = 6,
		/// <summary>A style in which the segments are visually separated from one another while remaining grouped.</summary>
		Separated = 8,
	}

	/// <summary>Specifies how a segmented control's segments are selected when clicked.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSegmentSwitchTracking : ulong {
		/// <summary>Only one segment can be selected at a time, like a set of radio buttons.</summary>
		SelectOne = 0,
		/// <summary>Any number of segments can be selected simultaneously, like a set of checkboxes.</summary>
		SelectAny = 1,
		/// <summary>Segments are highlighted only while pressed and do not retain a selected state.</summary>
		Momentary = 2,
		/// <summary>Segments behave like <see cref="F:AppKit.NSSegmentSwitchTracking.Momentary" />, but repeatedly send their action message at an accelerating rate while held down.</summary>
		MomentaryAccelerator, // 10.10.3
	}

	/// <summary>Specifies the position of tick marks relative to a slider.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTickMarkPosition : ulong {
		/// <summary>Places tick marks below a horizontal slider.</summary>
		Below,
		/// <summary>Places tick marks above a horizontal slider.</summary>
		Above,
		/// <summary>Places tick marks to the left of a vertical slider.</summary>
		Left,
		/// <summary>Places tick marks to the right of a vertical slider.</summary>
		Right,
		/// <summary>Places tick marks at the leading edge of a vertical slider, honoring the current layout direction. Equivalent to <see cref="F:AppKit.NSTickMarkPosition.Left" />.</summary>
		Leading = Left,
		/// <summary>Places tick marks at the trailing edge of a vertical slider, honoring the current layout direction. Equivalent to <see cref="F:AppKit.NSTickMarkPosition.Right" />.</summary>
		Trailing = Right,
	}

	/// <summary>Specifies the shape of a slider.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSliderType : ulong {
		/// <summary>A horizontal or vertical linear slider.</summary>
		Linear = 0,
		/// <summary>A circular slider.</summary>
		Circular = 1,
	}

	/// <summary>Specifies the visual style of tokens displayed in an <see cref="T:AppKit.NSTokenField" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTokenStyle : ulong {
		/// <summary>Uses the default token style, currently equivalent to <see cref="F:AppKit.NSTokenStyle.Rounded" />.</summary>
		Default,
		/// <summary>Displays the token's text without any surrounding token background.</summary>
		PlainText,
		/// <summary>Displays the token with fully rounded ends.</summary>
		Rounded,
		/// <summary>Displays the token with square corners.</summary>
		Squared = 3,
		/// <summary>Displays the token with square edges and no background unless the token is selected or highlighted.</summary>
		PlainSquared = 4,
	}

	/// <summary>Specifies a bitmask of options that control how an application is launched.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	[Deprecated (PlatformName.MacOSX, 11, 0)]
	public enum NSWorkspaceLaunchOptions : ulong {
		/// <summary>Prints the specified file or files instead of opening them.</summary>
		Print = 2,
		/// <summary>Presents a user interface if an error occurs while launching.</summary>
		WithErrorPresentation = 0x40,
		/// <summary>Causes the launch to fail if the target application runs only in the background.</summary>
		InhibitingBackgroundOnly = 0x80,
		/// <summary>Launches the application, and opens its documents, without adding them to the Recent Items list.</summary>
		WithoutAddingToRecents = 0x100,
		/// <summary>Launches the application without activating it (bringing it to the foreground).</summary>
		WithoutActivation = 0x200,
		/// <summary>Launches the application asynchronously, without waiting for it to finish launching.</summary>
		Async = 0x10000,
		/// <summary>Allows the application to be launched in the legacy Classic environment if necessary.</summary>
		AllowingClassicStartup = 0x20000,
		/// <summary>Prefers launching the application in the legacy Classic environment over a native launch.</summary>
		PreferringClassic = 0x40000,
		/// <summary>Launches a new instance of the application, even if another instance is already running.</summary>
		NewInstance = 0x80000,
		/// <summary>Hides the application after it launches.</summary>
		Hide = 0x100000,
		/// <summary>Hides all other applications when this one launches.</summary>
		HideOthers = 0x200000,
		/// <summary>The default launch options, combining <see cref="F:AppKit.NSWorkspaceLaunchOptions.Async" /> and <see cref="F:AppKit.NSWorkspaceLaunchOptions.AllowingClassicStartup" />.</summary>
		Default = Async | AllowingClassicStartup,
	}

	/// <summary>Specifies legacy icon representations to exclude when creating a file icon.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSWorkspaceIconCreationOptions : ulong {
		/// <summary>Excludes QuickDraw icon representations.</summary>
		NSExcludeQuickDrawElements = 1 << 1,
		/// <summary>Excludes icon representations introduced in macOS 10.4.</summary>
		NSExclude10_4Elements = 1 << 2,
	}

	/// <summary>Specifies the visual style of an <see cref="T:AppKit.NSPathControl" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPathStyle : long {
		/// <summary>Displays the path using the standard bezeled control style.</summary>
		Standard,
		/// <summary>Displays the path using a navigation-bar style.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		NavigationBar,
		/// <summary>Displays the path as a pop-up button.</summary>
		PopUp,
	}

	/// <summary>Specifies the position and border style of the tabs in an <see cref="T:AppKit.NSTabView" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTabViewType : ulong {
		/// <summary>Displays tabs along the top edge, with a bezeled border around the content.</summary>
		NSTopTabsBezelBorder,
		/// <summary>Displays tabs along the left edge, with a bezeled border around the content.</summary>
		NSLeftTabsBezelBorder,
		/// <summary>Displays tabs along the bottom edge, with a bezeled border around the content.</summary>
		NSBottomTabsBezelBorder,
		/// <summary>Displays tabs along the right edge, with a bezeled border around the content.</summary>
		NSRightTabsBezelBorder,
		/// <summary>Displays no tabs, with a bezeled border around the content.</summary>
		NSNoTabsBezelBorder,
		/// <summary>Displays no tabs, with a plain line border around the content.</summary>
		NSNoTabsLineBorder,
		/// <summary>Displays no tabs and no border around the content.</summary>
		NSNoTabsNoBorder,
	}

	/// <summary>Specifies the visual state of a tab view item.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTabState : ulong {
		/// <summary>The tab is selected.</summary>
		Selected,
		/// <summary>The tab is displayed in the background.</summary>
		Background,
		/// <summary>The tab is being pressed.</summary>
		Pressed,
	}

	/// <summary>Specifies how an <see cref="T:AppKit.NSTabViewController" /> displays the controls used to switch between its tabbed items.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTabViewControllerTabStyle : long {
		/// <summary>Displays a segmented control above the tab view's content.</summary>
		SegmentedControlOnTop = 0,
		/// <summary>Displays a segmented control below the tab view's content.</summary>
		SegmentedControlOnBottom,
		/// <summary>Displays a toolbar, with one toolbar item per tab, above the tab view's content.</summary>
		Toolbar,
		/// <summary>No tab-switching control is displayed; the tab view controller does not manage the presentation of a control.</summary>
		Unspecified = -1,
	}

	/// <summary>Specifies the visual style of an <see cref="T:AppKit.NSLevelIndicator" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSLevelIndicatorStyle : ulong {
		/// <summary>Displays a series of horizontal tick marks indicating a relevancy level, such as in search results.</summary>
		Relevancy,
		/// <summary>Displays a continuous bar indicating a capacity level, such as a battery or disk usage indicator.</summary>
		ContinuousCapacity,
		/// <summary>Displays a series of discrete segments indicating a capacity level.</summary>
		DiscreteCapacity,
		/// <summary>Displays a row of shapes, such as stars, indicating a rating level.</summary>
		RatingLevel,
	}

	/// <summary>Specifies options for creating or modifying font collections.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSFontCollectionOptions : long {
		/// <summary>Limits the font collection to the current application.</summary>
		ApplicationOnlyMask = 1,
	}

	/// <summary>Specifies whether a drop occurs on an item or before it.</summary>
#if XAMCORE_5_0
	[NoMacCatalyst]
#else
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This enum doesn't exist on this platform.")]
#if __MACCATALYST__
	[EditorBrowsable (EditorBrowsableState.Never)]
#endif
#endif
	[Native]
	public enum NSCollectionViewDropOperation : long {
		/// <summary>The drop occurs on the item at the specified index path.</summary>
		On = 0,
		/// <summary>The drop occurs before the item at the specified index path.</summary>
		Before = 1,
	}

	/// <summary>Specifies the highlight state applied to an item in a collection view, such as during selection or a drag-and-drop operation.</summary>
#if XAMCORE_5_0
	[NoMacCatalyst]
#else
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This enum doesn't exist on this platform.")]
#if __MACCATALYST__
	[EditorBrowsable (EditorBrowsableState.Never)]
#endif
#endif
	[Native]
	public enum NSCollectionViewItemHighlightState : long {
		/// <summary>The item is not highlighted.</summary>
		None = 0,
		/// <summary>The item is highlighted because it is being selected.</summary>
		ForSelection = 1,
		/// <summary>The item is highlighted because it is being deselected.</summary>
		ForDeselection = 2,
		/// <summary>The item is highlighted because it is the target of a drag-and-drop operation.</summary>
		AsDropTarget = 3,
	}

	/// <summary>Specifies a bitmask describing where a scrolled-to item should be positioned within a collection view's visible area.</summary>
#if XAMCORE_5_0
	[NoMacCatalyst]
#else
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This enum doesn't exist on this platform.")]
#if __MACCATALYST__
	[EditorBrowsable (EditorBrowsableState.Never)]
#endif
#endif
	[Native]
	[Flags]
	public enum NSCollectionViewScrollPosition : ulong {
		/// <summary>No particular scroll position is requested.</summary>
		None = 0,
		/// <summary>Positions the item at the top of the visible area.</summary>
		Top = 1 << 0,
		/// <summary>Centers the item vertically within the visible area.</summary>
		CenteredVertically = 1 << 1,
		/// <summary>Positions the item at the bottom of the visible area.</summary>
		Bottom = 1 << 2,
		/// <summary>Scrolls the minimum amount necessary to bring the item's nearest horizontal edge into view.</summary>
		NearestHorizontalEdge = 1 << 9,
		/// <summary>Positions the item at the left of the visible area.</summary>
		Left = 1 << 3,
		/// <summary>Centers the item horizontally within the visible area.</summary>
		CenteredHorizontally = 1 << 4,
		/// <summary>Positions the item at the right of the visible area.</summary>
		Right = 1 << 5,
		/// <summary>Positions the item at the leading edge of the visible area, honoring the current layout direction.</summary>
		LeadingEdge = 1 << 6,
		/// <summary>Positions the item at the trailing edge of the visible area, honoring the current layout direction.</summary>
		TrailingEdge = 1 << 7,
		/// <summary>Scrolls the minimum amount necessary to bring the item's nearest vertical edge into view.</summary>
		NearestVerticalEdge = 1 << 8,
	}

	/// <summary>Specifies the category of an element in a collection view layout.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSCollectionElementCategory : long {
		/// <summary>A collection view item.</summary>
		Item,
		/// <summary>A supplementary view.</summary>
		SupplementaryView,
		/// <summary>A decoration view.</summary>
		DecorationView,
		/// <summary>An inter-item gap.</summary>
		InterItemGap,
	}

	/// <summary>Specifies an update applied to a collection view.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSCollectionUpdateAction : long {
		/// <summary>Inserts an item.</summary>
		Insert,
		/// <summary>Deletes an item.</summary>
		Delete,
		/// <summary>Reloads an item.</summary>
		Reload,
		/// <summary>Moves an item.</summary>
		Move,
		/// <summary>No update action.</summary>
		None,
	}

	/// <summary>Specifies the scrolling direction of a collection view layout.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSCollectionViewScrollDirection : long {
		/// <summary>The collection view scrolls vertically.</summary>
		Vertical,
		/// <summary>The collection view scrolls horizontally.</summary>
		Horizontal,
	}

	/// <summary>Specifies the visual style of an <see cref="T:AppKit.NSDatePicker" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSDatePickerStyle : ulong {
		/// <summary>Displays an editable text field with a stepper control for incrementing and decrementing its value.</summary>
		TextFieldAndStepper,
		/// <summary>Displays a graphical clock and calendar for choosing a time and date.</summary>
		ClockAndCalendar,
		/// <summary>Displays only an editable text field, without a stepper.</summary>
		TextField,
	}

	/// <summary>Specifies whether a date picker selects a single date or a date range.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSDatePickerMode : ulong {
		/// <summary>The date picker selects a single date.</summary>
		Single,
		/// <summary>The date picker selects a range of dates.</summary>
		Range,
	}

	/// <summary>Specifies a bitmask of the date and time components displayed by an <see cref="T:AppKit.NSDatePicker" />.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSDatePickerElementFlags : ulong {
		/// <summary>Displays the hour and minute.</summary>
		HourMinute = 0xc,
		/// <summary>Displays the hour, minute, and second.</summary>
		HourMinuteSecond = 0xe,
		/// <summary>Displays the time zone.</summary>
		TimeZone = 0x10,

		/// <summary>Displays the year and month.</summary>
		YearMonthDate = 0xc0,
		/// <summary>Displays the year, month, and day.</summary>
		YearMonthDateDay = 0xe0,
		/// <summary>Displays the era.</summary>
		Era = 0x100,
	}

	/// <summary>Specifies a parameter that can be set or queried on an <see cref="T:AppKit.NSOpenGLContext" />.</summary>
	[NoMacCatalyst]
	[Native]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' Framework instead.")]
	public enum NSOpenGLContextParameter : ulong {
		/// <summary>The rectangle used for a partial screen swap operation.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		SwapRectangle = 200,
		/// <summary>Enables or disables the use of a partial screen swap rectangle.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		SwapRectangleEnable = 201,
		/// <summary>Enables or disables rasterization.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		RasterizationEnable = 221,
		/// <summary>Validates the context's state before each rendering call, which is useful for debugging.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		StateValidation = 301,
		/// <summary>Indicates that the surface's contents are volatile and do not need to be preserved.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		SurfaceSurfaceVolatile = 306,

		/// <summary>Controls whether buffer swaps are synchronized with the display's vertical refresh rate.</summary>
		SwapInterval = 222,
		/// <summary>Controls whether the surface is displayed above or below its window, as specified by an <see cref="T:AppKit.NSSurfaceOrder" /> value.</summary>
		SurfaceOrder = 235,
		/// <summary>Controls whether the surface is opaque or supports transparency.</summary>
		SurfaceOpacity = 236,

		/// <summary>Sets the size of the surface's offscreen backing store.</summary>
		SurfaceBackingSize = 304,
		/// <summary>Reclaims graphics resources associated with the context that are no longer needed.</summary>
		ReclaimResources = 308,
		/// <summary>The identifier of the renderer currently used by the context.</summary>
		CurrentRendererID = 309,
		/// <summary>Indicates whether the current renderer performs vertex processing on the GPU.</summary>
		GpuVertexProcessing = 310,
		/// <summary>Indicates whether the current renderer performs fragment processing on the GPU.</summary>
		GpuFragmentProcessing = 311,
		/// <summary>Indicates whether the context currently has a drawable object attached.</summary>
		HasDrawable = 314,
		/// <summary>The number of buffer swaps that can be queued and not yet completed.</summary>
		MpsSwapsInFlight = 315,
	}

	/// <summary>Specifies the position of an OpenGL surface relative to its window.</summary>
	[NoMacCatalyst]
	public enum NSSurfaceOrder {
		/// <summary>The surface is displayed above the window.</summary>
		AboveWindow = 1,
		/// <summary>The surface is displayed below the window.</summary>
		BelowWindow = -1,
	}

	/// <summary>Specifies an attribute used to describe the capabilities requested when creating an <see cref="T:AppKit.NSOpenGLPixelFormat" />.</summary>
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' Framework instead.")]
	public enum NSOpenGLPixelFormatAttribute : uint { // uint32_t NSOpenGLPixelFormatAttribute
		/// <summary>Chooses from among all available renderers, instead of only accelerated ones.</summary>
		AllRenderers = 1,
		/// <summary>Requests a double-buffered context.</summary>
		DoubleBuffer = 5,
		/// <summary>Requests a triple-buffered context.</summary>
		TripleBuffer = 3,
		/// <summary>Requests a stereoscopic (left- and right-eye) rendering context.</summary>
		Stereo = 6,
		/// <summary>Specifies the number of auxiliary buffers.</summary>
		AuxBuffers = 7,
		/// <summary>Specifies the number of bits per color buffer.</summary>
		ColorSize = 8,
		/// <summary>Specifies the number of bits in the alpha component of the color buffer.</summary>
		AlphaSize = 11,
		/// <summary>Specifies the number of bits in the depth buffer.</summary>
		DepthSize = 12,
		/// <summary>Specifies the number of bits in the stencil buffer.</summary>
		StencilSize = 13,
		/// <summary>Specifies the number of bits per component in the accumulation buffer.</summary>
		AccumSize = 14,
		/// <summary>Chooses a renderer whose capabilities meet or exceed, but most closely match, the minimum requested values.</summary>
		MinimumPolicy = 51,
		/// <summary>Prefers the largest available color, depth, and accumulation buffers when a nonzero size is requested.</summary>
		MaximumPolicy = 52,
		/// <summary>Requests a renderer capable of rendering to an offscreen buffer.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		OffScreen = 53,
		/// <summary>Requests a renderer capable of rendering to the full screen.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 6)]
		FullScreen = 54,
		/// <summary>Specifies the number of multisample buffers.</summary>
		SampleBuffers = 55,
		/// <summary>Specifies the number of samples per pixel used for multisampling.</summary>
		Samples = 56,
		/// <summary>Requests that auxiliary buffers have their own depth and stencil buffers.</summary>
		AuxDepthStencil = 57,
		/// <summary>Requests floating-point color buffer components.</summary>
		ColorFloat = 58,
		/// <summary>Requests multisample antialiasing.</summary>
		Multisample = 59,
		/// <summary>Requests supersample antialiasing.</summary>
		Supersample = 60,
		/// <summary>Requests that the alpha channel be filtered, rather than only color components, when using multisample antialiasing.</summary>
		SampleAlpha = 61,
		/// <summary>Requests a renderer matching a specific renderer identifier.</summary>
		RendererID = 70,
		/// <summary>Requires that all displays use the same renderer.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		SingleRenderer = 71,
		/// <summary>Disallows falling back to a software renderer if a hardware-accelerated renderer is not available.</summary>
		NoRecovery = 72,
		/// <summary>Requests a hardware-accelerated renderer.</summary>
		Accelerated = 73,
		/// <summary>Chooses the renderer whose capabilities most closely match the requested attribute values.</summary>
		ClosestPolicy = 74,
		/// <summary>Requests that the contents of the back buffer be preserved after a buffer swap, rather than becoming undefined.</summary>
		BackingStore = 76,
		/// <summary>Requests a renderer capable of rendering to a window.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		Window = 80,
		/// <summary>Requests a fully OpenGL-compliant renderer.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		Compliant = 83,
		/// <summary>Specifies a bitmask of the displays on which the pixel format is usable.</summary>
		ScreenMask = 84,
		/// <summary>Requests a renderer capable of rendering to a pixel buffer.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		PixelBuffer = 90,
		/// <summary>Requests a renderer capable of rendering to a pixel buffer for use with a remote context.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		RemotePixelBuffer = 91,
		/// <summary>Allows the pixel format to use renderers that are not attached to a display.</summary>
		AllowOfflineRenderers = 96,
		/// <summary>Requests a renderer with hardware-accelerated compute support.</summary>
		AcceleratedCompute = 97,

		// Specify the profile
		/// <summary>Specifies the OpenGL profile (version) requested, as an <see cref="T:AppKit.NSOpenGLProfile" /> value.</summary>
		OpenGLProfile = 99,
		/// <summary>Specifies the number of virtual screens described by the pixel format.</summary>
		VirtualScreenCount = 128,

		/// <summary>Requests a renderer that guards against corruption caused by other running applications.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 5)]
		Robust = 75,
		/// <summary>Requests a multiprocessor-safe renderer.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 5)]
		MPSafe = 78,
		/// <summary>Requests a renderer capable of driving multiple displays.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 5)]
		MultiScreen = 81,
	}

	/// <summary>Specifies the version and feature profile of OpenGL requested for a context.</summary>
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' Framework instead.")]
	public enum NSOpenGLProfile : int {
		/// <summary>Requests the legacy (pre-3.0) OpenGL profile.</summary>
		VersionLegacy = 0x1000, // Legacy
		/// <summary>Requests the OpenGL 3.2 Core Profile or better.</summary>
		Version3_2Core = 0x3200,  // 3.2 or better
		/// <summary>Requests the OpenGL 4.1 Core Profile.</summary>
		Version4_1Core = 0x4100,
	}

	/// <summary>Identifies which button dismissed an alert.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSAlertButtonReturn : long {
		/// <summary>The first alert button.</summary>
		First = 1000,
		/// <summary>The second alert button.</summary>
		Second = 1001,
		/// <summary>The third alert button.</summary>
		Third = 1002,
	}

	/// <summary>Specifies a global option affecting all OpenGL contexts within the process.</summary>
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' Framework instead.")]
	public enum NSOpenGLGlobalOption : uint {
		/// <summary>Specifies the number of pixel format entries the format cache can hold.</summary>
		FormatCacheSize = 501,
		/// <summary>Clears the pixel format cache.</summary>
		ClearFormatCache = 502,
		/// <summary>Controls whether renderers are retained in memory between uses.</summary>
		RetainRenderers = 503,
		/// <summary>Controls whether the shader build cache is used.</summary>
		UseBuildCache = 506,
		/// <summary>Resets the internal OpenGL library state.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 4)]
		ResetLibrary = 504,
	}

	/// <summary>Specifies the OpenGL texture target used when binding a texture, such as with <c>glTexImage2D</c>.</summary>
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' Framework instead.")]
	public enum NSGLTextureTarget : uint {
		/// <summary>A standard two-dimensional texture (equivalent to <c>GL_TEXTURE_2D</c>).</summary>
		T2D = 0x0de1,
		/// <summary>A cube-map texture (equivalent to <c>GL_TEXTURE_CUBE_MAP</c>).</summary>
		CubeMap = 0x8513,
		/// <summary>A rectangular texture with non-normalized texture coordinates (equivalent to <c>GL_TEXTURE_RECTANGLE_EXT</c>).</summary>
		RectangleExt = 0x84F5,
	}

	/// <summary>Specifies the OpenGL pixel format used when reading or copying texture image data.</summary>
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' Framework instead.")]
	public enum NSGLFormat : uint {
		/// <summary>Red, green, and blue color components (equivalent to <c>GL_RGB</c>).</summary>
		RGB = 0x1907,
		/// <summary>Red, green, blue, and alpha color components (equivalent to <c>GL_RGBA</c>).</summary>
		RGBA = 0x1908,
		/// <summary>Depth buffer component data (equivalent to <c>GL_DEPTH_COMPONENT</c>).</summary>
		DepthComponent = 0x1902,
	}

	/// <summary>Specifies a face of a cube-map texture, or that the texture is not a cube map.</summary>
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' Framework instead.")]
	public enum NSGLTextureCubeMap : uint {
		/// <summary>The texture is not a cube map.</summary>
		None = 0,
		/// <summary>The positive x-axis face of the cube map.</summary>
		PositiveX = 0x8515,
		/// <summary>The positive y-axis face of the cube map.</summary>
		PositiveY = 0x8517,
		/// <summary>The positive z-axis face of the cube map.</summary>
		PositiveZ = 0x8519,
		/// <summary>The negative x-axis face of the cube map.</summary>
		NegativeX = 0x8516,
		/// <summary>A legacy value named for the negative y-axis face whose underlying OpenGL constant identifies the positive y-axis face.</summary>
		NegativeY = 0x8517,
		/// <summary>The negative z-axis face of the cube map.</summary>
		NegativeZ = 0x851A,
	}

	/// <summary>Specifies an OpenGL color buffer, such as the front, back, or an auxiliary buffer.</summary>
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' Framework instead.")]
	public enum NSGLColorBuffer : uint {
		/// <summary>The front (visible) color buffer.</summary>
		Front = 0x0404,
		/// <summary>The back (offscreen) color buffer.</summary>
		Back = 0x0405,
		/// <summary>The first auxiliary color buffer.</summary>
		Aux0 = 0x0409,
	}

	/// <summary>Specifies the standard thickness, in points, of a horizontal or vertical progress indicator.</summary>
	[NoMacCatalyst]
	[Native]
	[Deprecated (PlatformName.MacOSX, 10, 14)]
	public enum NSProgressIndicatorThickness : ulong {
		/// <summary>A small progress indicator, 10 points thick.</summary>
		Small = 10,
		/// <summary>A regular-sized progress indicator, 14 points thick.</summary>
		Regular = 14,
		/// <summary>An Aqua-style progress indicator, 12 points thick.</summary>
		Aqua = 12,
		/// <summary>A large progress indicator, 18 points thick.</summary>
		Large = 18,
	}

	/// <summary>Specifies the visual style of a progress indicator.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSProgressIndicatorStyle : ulong {
		/// <summary>Displays progress as a bar.</summary>
		Bar,
		/// <summary>Displays progress as a spinning indicator.</summary>
		Spinning,
	}

	/// <summary>Specifies where a pop-up button displays its indicator arrows.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPopUpArrowPosition : ulong {
		/// <summary>Does not display arrows.</summary>
		None,
		/// <summary>Displays arrows centered within the button.</summary>
		Center,
		/// <summary>Displays arrows near the bottom edge of the button.</summary>
		Bottom,
	}

	// FileType 4cc values to use with NSFileTypeForHFSTypeCode.
	/// <summary>Specifies a legacy four-character HFS file type code identifying one of the classic Mac OS Finder icons, for use with <see cref="M:AppKit.NSWorkspace.IconForFileType(AppKit.HfsTypeCode)" />.</summary>
	[MacCatalyst (15, 0)]
	public enum HfsTypeCode : uint {
		/* Generic Finder icons */
		/// <summary>The four-character code (FourCC) 'CLIP', identifying the clipboard icon.</summary>
		ClipboardIcon = 0x434C4950,   //'CLIP'
		/// <summary>The four-character code (FourCC) 'clpu', identifying the clipping unknown type icon.</summary>
		ClippingUnknownTypeIcon = 0x636C7075,   //'clpu'
		/// <summary>The four-character code (FourCC) 'clpp', identifying the clipping picture type icon.</summary>
		ClippingPictureTypeIcon = 0x636C7070,   //'clpp'
		/// <summary>The four-character code (FourCC) 'clpt', identifying the clipping text type icon.</summary>
		ClippingTextTypeIcon = 0x636C7074,   //'clpt'
		/// <summary>The four-character code (FourCC) 'clps', identifying the clipping sound type icon.</summary>
		ClippingSoundTypeIcon = 0x636C7073,   //'clps'
		/// <summary>The four-character code (FourCC) 'desk', identifying the desktop icon.</summary>
		DesktopIcon = 0x6465736B,   //'desk'
		/// <summary>The four-character code (FourCC) 'FNDR', identifying the finder icon.</summary>
		FinderIcon = 0x464E4452,   //'FNDR'
		/// <summary>The four-character code (FourCC) 'root', identifying the computer icon.</summary>
		ComputerIcon = 0x726F6F74,   //'root'
		/// <summary>The four-character code (FourCC) 'FFIL', identifying the font suitcase icon.</summary>
		FontSuitcaseIcon = 0x4646494C,   //'FFIL'
		/// <summary>The four-character code (FourCC) 'ftrh', identifying the full trash icon.</summary>
		FullTrashIcon = 0x66747268,   //'ftrh'
		/// <summary>The four-character code (FourCC) 'APPL', identifying the generic application icon.</summary>
		GenericApplicationIcon = 0x4150504C,   //'APPL'
		/// <summary>The four-character code (FourCC) 'cddr', identifying the generic cdrom icon.</summary>
		GenericCdromIcon = 0x63646472,   //'cddr'
		/// <summary>The four-character code (FourCC) 'APPC', identifying the generic control panel icon.</summary>
		GenericControlPanelIcon = 0x41505043,   //'APPC'
		/// <summary>The four-character code (FourCC) 'sdev', identifying the generic control strip module icon.</summary>
		GenericControlStripModuleIcon = 0x73646576,   //'sdev'
		/// <summary>The four-character code (FourCC) 'thng', identifying the generic component icon.</summary>
		GenericComponentIcon = 0x74686E67,   //'thng'
		/// <summary>The four-character code (FourCC) 'APPD', identifying the generic desk accessory icon.</summary>
		GenericDeskAccessoryIcon = 0x41505044,   //'APPD'
		/// <summary>The four-character code (FourCC) 'docu', identifying the generic document icon.</summary>
		GenericDocumentIcon = 0x646F6375,   //'docu'
		/// <summary>The four-character code (FourCC) 'edtf', identifying the generic edition file icon.</summary>
		GenericEditionFileIcon = 0x65647466,   //'edtf'
		/// <summary>The four-character code (FourCC) 'INIT', identifying the generic extension icon.</summary>
		GenericExtensionIcon = 0x494E4954,   //'INIT'
		/// <summary>The four-character code (FourCC) 'srvr', identifying the generic file server icon.</summary>
		GenericFileServerIcon = 0x73727672,   //'srvr'
		/// <summary>The four-character code (FourCC) 'ffil', identifying the generic font icon.</summary>
		GenericFontIcon = 0x6666696C,   //'ffil'
		/// <summary>The four-character code (FourCC) 'sclr', identifying the generic font scaler icon.</summary>
		GenericFontScalerIcon = 0x73636C72,   //'sclr'
		/// <summary>The four-character code (FourCC) 'flpy', identifying the generic floppy icon.</summary>
		GenericFloppyIcon = 0x666C7079,   //'flpy'
		/// <summary>The four-character code (FourCC) 'hdsk', identifying the generic hard disk icon.</summary>
		GenericHardDiskIcon = 0x6864736B,   //'hdsk'
		/// <summary>The four-character code (FourCC) 'idsk', identifying the generic idisk icon.</summary>
		GenericIDiskIcon = 0x6964736B,   //'idsk'
		/// <summary>The four-character code (FourCC) 'rmov', identifying the generic removable media icon.</summary>
		GenericRemovableMediaIcon = 0x726D6F76,   //'rmov'
		/// <summary>The four-character code (FourCC) 'movr', identifying the generic mover object icon.</summary>
		GenericMoverObjectIcon = 0x6D6F7672,   //'movr'
		/// <summary>The four-character code (FourCC) 'pcmc', identifying the generic PC card icon.</summary>
		GenericPCCardIcon = 0x70636D63,   //'pcmc'
		/// <summary>The four-character code (FourCC) 'pref', identifying the generic preferences icon.</summary>
		GenericPreferencesIcon = 0x70726566,   //'pref'
		/// <summary>The four-character code (FourCC) 'qery', identifying the generic query document icon.</summary>
		GenericQueryDocumentIcon = 0x71657279,   //'qery'
		/// <summary>The four-character code (FourCC) 'ramd', identifying the generic ram disk icon.</summary>
		GenericRamDiskIcon = 0x72616D64,   //'ramd'
		/// <summary>The four-character code (FourCC) 'shlb', identifying the generic shared library icon.</summary>
		GenericSharedLibraryIcon = 0x73686C62,   //'shlb'
		/// <summary>The four-character code (FourCC) 'sdoc', identifying the generic stationery icon.</summary>
		GenericStationeryIcon = 0x73646F63,   //'sdoc'
		/// <summary>The four-character code (FourCC) 'suit', identifying the generic suitcase icon.</summary>
		GenericSuitcaseIcon = 0x73756974,   //'suit'
		/// <summary>The four-character code (FourCC) 'gurl', identifying the generic URL icon.</summary>
		GenericUrlIcon = 0x6775726C,   //'gurl'
		/// <summary>The four-character code (FourCC) 'worm', identifying the generic worm icon.</summary>
		GenericWormIcon = 0x776F726D,   //'worm'
		/// <summary>The four-character code (FourCC) 'ifil', identifying the international resources icon.</summary>
		InternationalResourcesIcon = 0x6966696C,   //'ifil'
		/// <summary>The four-character code (FourCC) 'kfil', identifying the keyboard layout icon.</summary>
		KeyboardLayoutIcon = 0x6B66696C,   //'kfil'
		/// <summary>The four-character code (FourCC) 'sfil', identifying the sound file icon.</summary>
		SoundFileIcon = 0x7366696C,   //'sfil'
		/// <summary>The four-character code (FourCC) 'zsys', identifying the system suitcase icon.</summary>
		SystemSuitcaseIcon = 0x7A737973,   //'zsys'
		/// <summary>The four-character code (FourCC) 'trsh', identifying the trash icon.</summary>
		TrashIcon = 0x74727368,   //'trsh'
		/// <summary>The four-character code (FourCC) 'tfil', identifying the true type font icon.</summary>
		TrueTypeFontIcon = 0x7466696C,   //'tfil'
		/// <summary>The four-character code (FourCC) 'sfnt', identifying the true type flat font icon.</summary>
		TrueTypeFlatFontIcon = 0x73666E74,   //'sfnt'
		/// <summary>The four-character code (FourCC) 'ttcf', identifying the true type multi flat font icon.</summary>
		TrueTypeMultiFlatFontIcon = 0x74746366,   //'ttcf'
		/// <summary>The four-character code (FourCC) 'udsk', identifying the user idisk icon.</summary>
		UserIDiskIcon = 0x7564736B,   //'udsk'
		/// <summary>The four-character code (FourCC) 'unfs', identifying the unknown FS object icon.</summary>
		UnknownFSObjectIcon = 0x756E6673,   //'unfs'

		/* Internet locations */
		/// <summary>The four-character code (FourCC) 'ilht', identifying the internet location HTTP icon.</summary>
		InternetLocationHttpIcon = 0x696C6874,   //'ilht'
		/// <summary>The four-character code (FourCC) 'ilft', identifying the internet location FTP icon.</summary>
		InternetLocationFtpIcon = 0x696C6674,   //'ilft'
		/// <summary>The four-character code (FourCC) 'ilaf', identifying the internet location apple share icon.</summary>
		InternetLocationAppleShareIcon = 0x696C6166,   //'ilaf'
		/// <summary>The four-character code (FourCC) 'ilat', identifying the internet location apple talk zone icon.</summary>
		InternetLocationAppleTalkZoneIcon = 0x696C6174,   //'ilat'
		/// <summary>The four-character code (FourCC) 'ilfi', identifying the internet location file icon.</summary>
		InternetLocationFileIcon = 0x696C6669,   //'ilfi'
		/// <summary>The four-character code (FourCC) 'ilma', identifying the internet location mail icon.</summary>
		InternetLocationMailIcon = 0x696C6D61,   //'ilma'
		/// <summary>The four-character code (FourCC) 'ilnw', identifying the internet location news icon.</summary>
		InternetLocationNewsIcon = 0x696C6E77,   //'ilnw'
		/// <summary>The four-character code (FourCC) 'ilns', identifying the internet location NSL neighborhood icon.</summary>
		InternetLocationNslNeighborhoodIcon = 0x696C6E73,   //'ilns'
		/// <summary>The four-character code (FourCC) 'ilge', identifying the internet location generic icon.</summary>
		InternetLocationGenericIcon = 0x696C6765,   //'ilge'

		/* Folders */
		/// <summary>The four-character code (FourCC) 'fldr', identifying the generic folder icon.</summary>
		GenericFolderIcon = 0x666C6472,   //'fldr'
		/// <summary>The four-character code (FourCC) 'dbox', identifying the drop folder icon.</summary>
		DropFolderIcon = 0x64626F78,   //'dbox'
		/// <summary>The four-character code (FourCC) 'mntd', identifying the mounted folder icon.</summary>
		MountedFolderIcon = 0x6D6E7464,   //'mntd'
		/// <summary>The four-character code (FourCC) 'ofld', identifying the open folder icon.</summary>
		OpenFolderIcon = 0x6F666C64,   //'ofld'
		/// <summary>The four-character code (FourCC) 'ownd', identifying the owned folder icon.</summary>
		OwnedFolderIcon = 0x6F776E64,   //'ownd'
		/// <summary>The four-character code (FourCC) 'prvf', identifying the private folder icon.</summary>
		PrivateFolderIcon = 0x70727666,   //'prvf'
		/// <summary>The four-character code (FourCC) 'shfl', identifying the shared folder icon.</summary>
		SharedFolderIcon = 0x7368666C,   //'shfl'

		/* Sharingprivileges icons */
		/// <summary>The four-character code (FourCC) 'shna', identifying the sharing privs not applicable icon.</summary>
		SharingPrivsNotApplicableIcon = 0x73686E61,   //'shna'
		/// <summary>The four-character code (FourCC) 'shro', identifying the sharing privs read only icon.</summary>
		SharingPrivsReadOnlyIcon = 0x7368726F,   //'shro'
		/// <summary>The four-character code (FourCC) 'shrw', identifying the sharing privs read write icon.</summary>
		SharingPrivsReadWriteIcon = 0x73687277,   //'shrw'
		/// <summary>The four-character code (FourCC) 'shuk', identifying the sharing privs unknown icon.</summary>
		SharingPrivsUnknownIcon = 0x7368756B,   //'shuk'
		/// <summary>The four-character code (FourCC) 'writ', identifying the sharing privs writable icon.</summary>
		SharingPrivsWritableIcon = 0x77726974,   //'writ'

		/* Users and Groups icons */
		/// <summary>The four-character code (FourCC) 'ufld', identifying the user folder icon.</summary>
		UserFolderIcon = 0x75666C64,   //'ufld'
		/// <summary>The four-character code (FourCC) 'wfld', identifying the workgroup folder icon.</summary>
		WorkgroupFolderIcon = 0x77666C64,   //'wfld'
		/// <summary>The four-character code (FourCC) 'gusr', identifying the guest user icon.</summary>
		GuestUserIcon = 0x67757372,   //'gusr'
		/// <summary>The four-character code (FourCC) 'user', identifying the user icon.</summary>
		UserIcon = 0x75736572,   //'user'
		/// <summary>The four-character code (FourCC) 'susr', identifying the owner icon.</summary>
		OwnerIcon = 0x73757372,   //'susr'
		/// <summary>The four-character code (FourCC) 'grup', identifying the group icon.</summary>
		GroupIcon = 0x67727570,   //'grup'

		/* Special folders */
		/// <summary>The four-character code (FourCC) 'appr', identifying the appearance folder icon.</summary>
		AppearanceFolderIcon = 0x61707072,   //'appr'
		/// <summary>The four-character code (FourCC) 'amnu', identifying the apple menu folder icon.</summary>
		AppleMenuFolderIcon = 0x616D6E75,   //'amnu'
		/// <summary>The four-character code (FourCC) 'apps', identifying the applications folder icon.</summary>
		ApplicationsFolderIcon = 0x61707073,   //'apps'
		/// <summary>The four-character code (FourCC) 'asup', identifying the application support folder icon.</summary>
		ApplicationSupportFolderIcon = 0x61737570,   //'asup'
		/// <summary>The four-character code (FourCC) 'prof', identifying the color sync folder icon.</summary>
		ColorSyncFolderIcon = 0x70726F66,   //'prof'
		/// <summary>The four-character code (FourCC) 'cmnu', identifying the contextual menu items folder icon.</summary>
		ContextualMenuItemsFolderIcon = 0x636D6E75,   //'cmnu'
		/// <summary>The four-character code (FourCC) 'ctrD', identifying the control panel disabled folder icon.</summary>
		ControlPanelDisabledFolderIcon = 0x63747244,   //'ctrD'
		/// <summary>The four-character code (FourCC) 'ctrl', identifying the control panel folder icon.</summary>
		ControlPanelFolderIcon = 0x6374726C,   //'ctrl'
		/// <summary>The four-character code (FourCC) 'docs', identifying the documents folder icon.</summary>
		DocumentsFolderIcon = 0x646F6373,   //'docs'
		/// <summary>The four-character code (FourCC) 'extD', identifying the extensions disabled folder icon.</summary>
		ExtensionsDisabledFolderIcon = 0x65787444,   //'extD'
		/// <summary>The four-character code (FourCC) 'extn', identifying the extensions folder icon.</summary>
		ExtensionsFolderIcon = 0x6578746E,   //'extn'
		/// <summary>The four-character code (FourCC) 'favs', identifying the favorites folder icon.</summary>
		FavoritesFolderIcon = 0x66617673,   //'favs'
		/// <summary>The four-character code (FourCC) 'font', identifying the fonts folder icon.</summary>
		FontsFolderIcon = 0x666F6E74,   //'font'
		/// <summary>The four-character code (FourCC) 'issf', identifying the internet search sites folder icon.</summary>
		InternetSearchSitesFolderIcon = 0x69737366,   //'issf'
		/// <summary>The four-character code (FourCC) 'pubf', identifying the public folder icon.</summary>
		PublicFolderIcon = 0x70756266,   //'pubf'
		/// <summary>The four-character code (FourCC) 'ppdf', identifying the printer description folder icon.</summary>
		PrinterDescriptionFolderIcon = 0x70706466,   //'ppdf'
		/// <summary>The four-character code (FourCC) 'prnt', identifying the print monitor folder icon.</summary>
		PrintMonitorFolderIcon = 0x70726E74,   //'prnt'
		/// <summary>The four-character code (FourCC) 'rapp', identifying the recent applications folder icon.</summary>
		RecentApplicationsFolderIcon = 0x72617070,   //'rapp'
		/// <summary>The four-character code (FourCC) 'rdoc', identifying the recent documents folder icon.</summary>
		RecentDocumentsFolderIcon = 0x72646F63,   //'rdoc'
		/// <summary>The four-character code (FourCC) 'rsrv', identifying the recent servers folder icon.</summary>
		RecentServersFolderIcon = 0x72737276,   //'rsrv'
		/// <summary>The four-character code (FourCC) 'shdD', identifying the shutdown items disabled folder icon.</summary>
		ShutdownItemsDisabledFolderIcon = 0x73686444,   //'shdD'
		/// <summary>The four-character code (FourCC) 'shdf', identifying the shutdown items folder icon.</summary>
		ShutdownItemsFolderIcon = 0x73686466,   //'shdf'
		/// <summary>The four-character code (FourCC) 'spki', identifying the speakable items folder.</summary>
		SpeakableItemsFolder = 0x73706B69,   //'spki'
		/// <summary>The four-character code (FourCC) 'strD', identifying the startup items disabled folder icon.</summary>
		StartupItemsDisabledFolderIcon = 0x73747244,   //'strD'
		/// <summary>The four-character code (FourCC) 'strt', identifying the startup items folder icon.</summary>
		StartupItemsFolderIcon = 0x73747274,   //'strt'
		/// <summary>The four-character code (FourCC) 'macD', identifying the system extension disabled folder icon.</summary>
		SystemExtensionDisabledFolderIcon = 0x6D616344,   //'macD'
		/// <summary>The four-character code (FourCC) 'macs', identifying the system folder icon.</summary>
		SystemFolderIcon = 0x6D616373,   //'macs'
		/// <summary>The four-character code (FourCC) 'fvoc', identifying the voices folder icon.</summary>
		VoicesFolderIcon = 0x66766F63,   //'fvoc'

		/* Badges */
		/// <summary>The four-character code (FourCC) 'scrp', identifying the apple script badge icon.</summary>
		AppleScriptBadgeIcon = 0x73637270,   //'scrp'
		/// <summary>The four-character code (FourCC) 'lbdg', identifying the locked badge icon.</summary>
		LockedBadgeIcon = 0x6C626467,   //'lbdg'
		/// <summary>The four-character code (FourCC) 'mbdg', identifying the mounted badge icon.</summary>
		MountedBadgeIcon = 0x6D626467,   //'mbdg'
		/// <summary>The four-character code (FourCC) 'sbdg', identifying the shared badge icon.</summary>
		SharedBadgeIcon = 0x73626467,   //'sbdg'
		/// <summary>The four-character code (FourCC) 'abdg', identifying the alias badge icon.</summary>
		AliasBadgeIcon = 0x61626467,   //'abdg'
		/// <summary>The four-character code (FourCC) 'cbdg', identifying the alert caution badge icon.</summary>
		AlertCautionBadgeIcon = 0x63626467,   //'cbdg'

		/* Alert icons */
		/// <summary>The four-character code (FourCC) 'note', identifying the alert note icon.</summary>
		AlertNoteIcon = 0x6E6F7465,   //'note'
		/// <summary>The four-character code (FourCC) 'caut', identifying the alert caution icon.</summary>
		AlertCautionIcon = 0x63617574,   //'caut'
		/// <summary>The four-character code (FourCC) 'stop', identifying the alert stop icon.</summary>
		AlertStopIcon = 0x73746F70,   //'stop'

		/* Networking icons */
		/// <summary>The four-character code (FourCC) 'atlk', identifying the apple talk icon.</summary>
		AppleTalkIcon = 0x61746C6B,   //'atlk'
		/// <summary>The four-character code (FourCC) 'atzn', identifying the apple talk zone icon.</summary>
		AppleTalkZoneIcon = 0x61747A6E,   //'atzn'
		/// <summary>The four-character code (FourCC) 'afps', identifying the AFP server icon.</summary>
		AfpServerIcon = 0x61667073,   //'afps'
		/// <summary>The four-character code (FourCC) 'ftps', identifying the FTP server icon.</summary>
		FtpServerIcon = 0x66747073,   //'ftps'
		/// <summary>The four-character code (FourCC) 'htps', identifying the HTTP server icon.</summary>
		HttpServerIcon = 0x68747073,   //'htps'
		/// <summary>The four-character code (FourCC) 'gnet', identifying the generic network icon.</summary>
		GenericNetworkIcon = 0x676E6574,   //'gnet'
		/// <summary>The four-character code (FourCC) 'isrv', identifying the IP file server icon.</summary>
		IPFileServerIcon = 0x69737276,   //'isrv'

		/* Toolbar icons */
		/// <summary>The four-character code (FourCC) 'tcus', identifying the toolbar customize icon.</summary>
		ToolbarCustomizeIcon = 0x74637573,   //'tcus'
		/// <summary>The four-character code (FourCC) 'tdel', identifying the toolbar delete icon.</summary>
		ToolbarDeleteIcon = 0x7464656C,   //'tdel'
		/// <summary>The four-character code (FourCC) 'tfav', identifying the toolbar favorites icon.</summary>
		ToolbarFavoritesIcon = 0x74666176,   //'tfav'
		/// <summary>The four-character code (FourCC) 'thom', identifying the toolbar home icon.</summary>
		ToolbarHomeIcon = 0x74686F6D,   //'thom'
		/// <summary>The four-character code (FourCC) 'tbav', identifying the toolbar advanced icon.</summary>
		ToolbarAdvancedIcon = 0x74626176,   //'tbav'
		/// <summary>The four-character code (FourCC) 'tbin', identifying the toolbar info icon.</summary>
		ToolbarInfoIcon = 0x7462696E,   //'tbin'
		/// <summary>The four-character code (FourCC) 'tblb', identifying the toolbar labels icon.</summary>
		ToolbarLabelsIcon = 0x74626C62,   //'tblb'
		/// <summary>The four-character code (FourCC) 'tAps', identifying the toolbar applications folder icon.</summary>
		ToolbarApplicationsFolderIcon = 0x74417073,   //'tAps'
		/// <summary>The four-character code (FourCC) 'tDoc', identifying the toolbar documents folder icon.</summary>
		ToolbarDocumentsFolderIcon = 0x74446F63,   //'tDoc'
		/// <summary>The four-character code (FourCC) 'tMov', identifying the toolbar movie folder icon.</summary>
		ToolbarMovieFolderIcon = 0x744D6F76,   //'tMov'
		/// <summary>The four-character code (FourCC) 'tMus', identifying the toolbar music folder icon.</summary>
		ToolbarMusicFolderIcon = 0x744D7573,   //'tMus'
		/// <summary>The four-character code (FourCC) 'tPic', identifying the toolbar pictures folder icon.</summary>
		ToolbarPicturesFolderIcon = 0x74506963,   //'tPic'
		/// <summary>The four-character code (FourCC) 'tPub', identifying the toolbar public folder icon.</summary>
		ToolbarPublicFolderIcon = 0x74507562,   //'tPub'
		/// <summary>The four-character code (FourCC) 'tDsk', identifying the toolbar desktop folder icon.</summary>
		ToolbarDesktopFolderIcon = 0x7444736B,   //'tDsk'
		/// <summary>The four-character code (FourCC) 'tDwn', identifying the toolbar downloads folder icon.</summary>
		ToolbarDownloadsFolderIcon = 0x7444776E,   //'tDwn'
		/// <summary>The four-character code (FourCC) 'tLib', identifying the toolbar library folder icon.</summary>
		ToolbarLibraryFolderIcon = 0x744C6962,   //'tLib'
		/// <summary>The four-character code (FourCC) 'tUtl', identifying the toolbar utilities folder icon.</summary>
		ToolbarUtilitiesFolderIcon = 0x7455746C,   //'tUtl'
		/// <summary>The four-character code (FourCC) 'tSts', identifying the toolbar sites folder icon.</summary>
		ToolbarSitesFolderIcon = 0x74537473,   //'tSts'

		/* Other icons */
		/// <summary>The four-character code (FourCC) 'capl', identifying the apple logo icon.</summary>
		AppleLogoIcon = 0x6361706C,   //'capl'
		/// <summary>The four-character code (FourCC) 'sapl', identifying the apple menu icon.</summary>
		AppleMenuIcon = 0x7361706C,   //'sapl'
		/// <summary>The four-character code (FourCC) 'baro', identifying the backward arrow icon.</summary>
		BackwardArrowIcon = 0x6261726F,   //'baro'
		/// <summary>The four-character code (FourCC) 'favr', identifying the favorite items icon.</summary>
		FavoriteItemsIcon = 0x66617672,   //'favr'
		/// <summary>The four-character code (FourCC) 'faro', identifying the forward arrow icon.</summary>
		ForwardArrowIcon = 0x6661726F,   //'faro'
		/// <summary>The four-character code (FourCC) 'grid', identifying the grid icon.</summary>
		GridIcon = 0x67726964,   //'grid'
		/// <summary>The four-character code (FourCC) 'help', identifying the help icon.</summary>
		HelpIcon = 0x68656C70,   //'help'
		/// <summary>The four-character code (FourCC) 'arng', identifying the keep arranged icon.</summary>
		KeepArrangedIcon = 0x61726E67,   //'arng'
		/// <summary>The four-character code (FourCC) 'lock', identifying the locked icon.</summary>
		LockedIcon = 0x6C6F636B,   //'lock'
		/// <summary>The four-character code (FourCC) 'nfil', identifying the no files icon.</summary>
		NoFilesIcon = 0x6E66696C,   //'nfil'
		/// <summary>The four-character code (FourCC) 'nfld', identifying the no folder icon.</summary>
		NoFolderIcon = 0x6E666C64,   //'nfld'
		/// <summary>The four-character code (FourCC) 'nwrt', identifying the no write icon.</summary>
		NoWriteIcon = 0x6E777274,   //'nwrt'
		/// <summary>The four-character code (FourCC) 'papp', identifying the protected application folder icon.</summary>
		ProtectedApplicationFolderIcon = 0x70617070,   //'papp'
		/// <summary>The four-character code (FourCC) 'psys', identifying the protected system folder icon.</summary>
		ProtectedSystemFolderIcon = 0x70737973,   //'psys'
		/// <summary>The four-character code (FourCC) 'rcnt', identifying the recent items icon.</summary>
		RecentItemsIcon = 0x72636E74,   //'rcnt'
		/// <summary>The four-character code (FourCC) 'shrt', identifying the shortcut icon.</summary>
		ShortcutIcon = 0x73687274,   //'shrt'
		/// <summary>The four-character code (FourCC) 'asnd', identifying the sort ascending icon.</summary>
		SortAscendingIcon = 0x61736E64,   //'asnd'
		/// <summary>The four-character code (FourCC) 'dsnd', identifying the sort descending icon.</summary>
		SortDescendingIcon = 0x64736E64,   //'dsnd'
		/// <summary>The four-character code (FourCC) 'ulck', identifying the unlocked icon.</summary>
		UnlockedIcon = 0x756C636B,   //'ulck'
		/// <summary>The four-character code (FourCC) 'cnct', identifying the connect to icon.</summary>
		ConnectToIcon = 0x636E6374,   //'cnct'
		/// <summary>The four-character code (FourCC) 'gwin', identifying the generic window icon.</summary>
		GenericWindowIcon = 0x6777696E,   //'gwin'
		/// <summary>The four-character code (FourCC) 'ques', identifying the question mark icon.</summary>
		QuestionMarkIcon = 0x71756573,   //'ques'
		/// <summary>The four-character code (FourCC) 'dali', identifying the delete alias icon.</summary>
		DeleteAliasIcon = 0x64616C69,   //'dali'
		/// <summary>The four-character code (FourCC) 'ejec', identifying the eject media icon.</summary>
		EjectMediaIcon = 0x656A6563,   //'ejec'
		/// <summary>The four-character code (FourCC) 'burn', identifying the burning icon.</summary>
		BurningIcon = 0x6275726E,   //'burn'
		/// <summary>The four-character code (FourCC) 'rcar', identifying the right container arrow icon.</summary>
		RightContainerArrowIcon = 0x72636172,   //'rcar'
	}

	// These constants specify the possible states of a drawer.
	/// <summary>Specifies the current animation state of an <see cref="T:AppKit.NSDrawer" />.</summary>
	[NoMacCatalyst]
	[Native]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSSplitViewController' instead.")]
	public enum NSDrawerState : ulong {
		/// <summary>The drawer is fully closed.</summary>
		Closed = 0,
		/// <summary>The drawer is animating from closed to open.</summary>
		Opening = 1,
		/// <summary>The drawer is fully open.</summary>
		Open = 2,
		/// <summary>The drawer is animating from open to closed.</summary>
		Closing = 3,
	}

	/// <summary>Specifies the layering level of a window relative to other windows on the screen.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowLevel : long {
		/// <summary>The default level used by ordinary application windows.</summary>
		Normal = 0,
		/// <summary>The level used by the Dock.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		Dock = 20,
		/// <summary>The level used by floating panels, which appear above normal windows.</summary>
		Floating = 3,
		/// <summary>The level used by the application's main menu bar.</summary>
		MainMenu = 24,
		/// <summary>The level used by modal panels, which appear above floating windows.</summary>
		ModalPanel = 8,
		/// <summary>The level used by pop-up menus.</summary>
		PopUpMenu = 101,
		/// <summary>The level used by the screen saver, above nearly all other windows.</summary>
		ScreenSaver = 1000,
		/// <summary>The level used by status bar items and windows.</summary>
		Status = 25,
		/// <summary>The level used by submenus. Equivalent to <see cref="F:AppKit.NSWindowLevel.Floating" />.</summary>
		Submenu = 3,
		/// <summary>The level used by torn-off menus. Equivalent to <see cref="F:AppKit.NSWindowLevel.Floating" />.</summary>
		TornOffMenu = 3,
	}

	/// <summary>Specifies whether a rule editor row is a leaf condition or a group of subrows.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSRuleEditorRowType : ulong {
		/// <summary>A leaf row that represents a single condition.</summary>
		Simple = 0,
		/// <summary>A parent row that groups one or more subrows.</summary>
		Compound,
	}

	/// <summary>Specifies how deeply an <see cref="T:AppKit.NSRuleEditor" /> allows compound rows to be nested.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSRuleEditorNestingMode : ulong {
		/// <summary>Only a single row is allowed, with no compound rows.</summary>
		Single,
		/// <summary>A list of rows is allowed at the top level, with no further nesting.</summary>
		List,
		/// <summary>Compound rows can be nested to any depth.</summary>
		Compound,
		/// <summary>A single compound row is allowed at the top level, containing only simple (non-nested) rows.</summary>
		Simple,
	}

	/// <summary>Specifies how a glyph is positioned relative to its base glyph, such as for diacritical marks.</summary>
	[NoMacCatalyst]
	[Native]
	[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSGlyphProperty' instead.")]
	public enum NSGlyphInscription : ulong {
		/// <summary>The glyph is a standalone base glyph.</summary>
		Base,
		/// <summary>The glyph is positioned below its base glyph.</summary>
		Below,
		/// <summary>The glyph is positioned above its base glyph.</summary>
		Above,
		/// <summary>The glyph is drawn overstruck directly on top of its base glyph.</summary>
		Overstrike,
		/// <summary>The glyph is drawn overstruck below its base glyph.</summary>
		OverBelow,
	}

	/// <summary>Specifies the text layout compatibility behavior used by a typesetter, matching the behavior of a specific historical macOS release.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTypesetterBehavior : long {
		/// <summary>Uses the most recent typesetter behavior available.</summary>
		Latest = -1,
		/// <summary>Uses the original typesetter behavior, matching macOS 10.0 through 10.1.</summary>
		Original = 0,
		/// <summary>Uses the typesetter behavior introduced in macOS 10.2, with compatibility fixes for the original behavior.</summary>
		Specific_10_2_WithCompatibility = 1,
		/// <summary>Uses the typesetter behavior introduced in macOS 10.2.</summary>
		Specific_10_2 = 2,
		/// <summary>Uses the typesetter behavior introduced in macOS 10.3.</summary>
		Specific_10_3 = 3,
		/// <summary>Uses the typesetter behavior introduced in macOS 10.4.</summary>
		Specific_10_4 = 4,

	}

	/// <summary>Specifies a bitmask of the types of remote (push) notifications an application can receive.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSRemoteNotificationType : ulong {
		/// <summary>The application does not receive any remote notifications.</summary>
		None = 0,
		/// <summary>The application can update its icon badge in response to a remote notification.</summary>
		Badge = 1 << 0,
		/// <summary>The application can play a sound in response to a remote notification.</summary>
		Sound = 1 << 1,
		/// <summary>The application can display an alert in response to a remote notification.</summary>
		Alert = 1 << 2,
	}

	/// <summary>Specifies where a scroll view positions its find bar.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSScrollViewFindBarPosition : long {
		/// <summary>Positions the find bar above the horizontal ruler.</summary>
		AboveHorizontalRuler = 0,
		/// <summary>Positions the find bar above the scroll view's content.</summary>
		AboveContent,
		/// <summary>Positions the find bar below the scroll view's content.</summary>
		BelowContent,
	}

	/// <summary>Specifies how a scroller is displayed relative to its content.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSScrollerStyle : long {
		/// <summary>The scroller occupies space reserved for it in the layout.</summary>
		Legacy = 0,
		/// <summary>The scroller is drawn over the content without reserving layout space.</summary>
		Overlay,
	}

	/// <summary>Specifies whether a scroll view exhibits elastic (rubber-band) scrolling past its content bounds.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSScrollElasticity : long {
		/// <summary>Uses the system's default elasticity behavior for the scroll view's context.</summary>
		Automatic = 0,
		/// <summary>Disables elastic scrolling; the scroll view stops at its content bounds.</summary>
		None,
		/// <summary>Allows elastic scrolling past the scroll view's content bounds.</summary>
		Allowed,
	}

	/// <summary>Specifies the visual style of a scroller's knob.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSScrollerKnobStyle : long {
		/// <summary>Uses the default knob style for the current context.</summary>
		Default = 0,
		/// <summary>Uses a dark-colored knob, suited to light backgrounds.</summary>
		Dark = 1,
		/// <summary>Uses a light-colored knob, suited to dark backgrounds.</summary>
		Light = 2,
	}

	/// <summary>Specifies a bitmask describing the phase of a continuous gesture or scrolling event, such as a trackpad scroll.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSEventPhase : ulong {
		/// <summary>The event has no associated phase.</summary>
		None,
		/// <summary>The event marks the beginning of a continuous gesture.</summary>
		Began = 1,
		/// <summary>The gesture has not changed since the previous event.</summary>
		Stationary = 2,
		/// <summary>The gesture has changed since the previous event.</summary>
		Changed = 4,
		/// <summary>The event marks the end of a continuous gesture.</summary>
		Ended = 8,
		/// <summary>The gesture was cancelled and did not end normally.</summary>
		Cancelled = 16,
		/// <summary>The event indicates that a gesture may begin, but has not yet been confirmed.</summary>
		MayBegin = 32,
	}

	/// <summary>Specifies options for tracking a swipe gesture.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSEventSwipeTrackingOptions : ulong {
		/// <summary>Clamps the gesture amount to zero when the user reverses the initial swipe direction.</summary>
		LockDirection = 1,
		/// <summary>Clamps the gesture amount to the range from -1.0 through 1.0.</summary>
		ClampGestureAmount = 2,
	}

	/// <summary>Specifies the axis associated with a gesture event.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSEventGestureAxis : long {
		/// <summary>The gesture is not associated with an axis.</summary>
		None,
		/// <summary>The gesture is associated with the horizontal axis.</summary>
		Horizontal,
		/// <summary>The gesture is associated with the vertical axis.</summary>
		Vertical,
	}

	/// <summary>Specifies the axis affected by Auto Layout constraints.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSLayoutConstraintOrientation : long {
		/// <summary>The constraint has a horizontal orientation.</summary>
		Horizontal,
		/// <summary>The constraint has a vertical orientation.</summary>
		Vertical,
	}

	/// <summary>Provides standard priority values used with Auto Layout constraints to resolve conflicts between competing constraints.</summary>
	[NoMacCatalyst]
	public enum NSLayoutPriority : int /*float*/ {
		/// <summary>The highest priority; a constraint at this priority must always be satisfied and cannot be broken.</summary>
		Required = 1000,
		/// <summary>A high priority used for constraints that should be satisfied if at all possible, such as a view's preferred content size.</summary>
		DefaultHigh = 750,
		/// <summary>The priority at which a window can be resized by the user beyond its current drag operation.</summary>
		DragThatCanResizeWindow = 510,
		/// <summary>The priority at which a window's current size is preferred to remain unchanged.</summary>
		WindowSizeStayPut = 500,
		/// <summary>The priority at which a user's drag operation should not resize the window.</summary>
		DragThatCannotResizeWindow = 490,
		/// <summary>A low priority used for constraints that are satisfied only if higher-priority constraints allow it, such as a view's content-hugging priority.</summary>
		DefaultLow = 250,
		/// <summary>The priority at which a view is compressed to its minimum size that still fits its content.</summary>
		FittingSizeCompression = 50,
	}

	/// <summary>Specifies the visual appearance of a popover.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPopoverAppearance : long {
		/// <summary>A minimal popover appearance.</summary>
		Minimal,
		/// <summary>A heads-up display appearance.</summary>
		HUD,
	}

	/// <summary>Specifies how a popover is dismissed in response to user interaction.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPopoverBehavior : long {
		/// <summary>The application is responsible for closing the popover explicitly; it is not dismissed automatically.</summary>
		ApplicationDefined,
		/// <summary>The popover is closed automatically when the user interacts with a user interface element outside of it.</summary>
		Transient,
		/// <summary>The popover is closed automatically when the user interacts with a user interface element in the window containing its positioning view.</summary>
		Semitransient,
	}

	/// <summary>Specifies the row height used by a table view.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTableViewRowSizeStyle : long {
		/// <summary>Uses the default row size style for the table view's context.</summary>
		Default = -1,
		/// <summary>Uses the row height explicitly set on the table view, ignoring the standard size styles.</summary>
		Custom = 0,
		/// <summary>Uses a small, compact row height.</summary>
		Small,
		/// <summary>Uses a medium row height.</summary>
		Medium,
		/// <summary>Uses a large row height.</summary>
		Large,
	}

	/// <summary>Specifies the edge of a table row where row actions appear.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTableRowActionEdge : long {
		/// <summary>The leading edge in the current user interface layout direction.</summary>
		Leading,
		/// <summary>The trailing edge in the current user interface layout direction.</summary>
		Trailing,
	}

	/// <summary>Specifies whether a table row action is regular or destructive.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTableViewRowActionStyle : long {
		/// <summary>A regular, nondestructive action.</summary>
		Regular,
		/// <summary>An action that performs a destructive operation.</summary>
		Destructive,
	}

	/// <summary>Specifies the visual animation effect used when rows are inserted into or removed from a table view.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSTableViewAnimation : ulong {
		/// <summary>No animation effect.</summary>
		None,
		/// <summary>Fades the affected rows in or out.</summary>
		Fade = 1,
		/// <summary>Opens a gap for newly inserted rows.</summary>
		Gap = 2,
		/// <summary>Slides the affected rows upward.</summary>
		SlideUp = 0x10,
		/// <summary>Slides the affected rows downward.</summary>
		SlideDown = 0x20,
		/// <summary>Slides the affected rows to the left.</summary>
		SlideLeft = 0x30,
		/// <summary>Slides the affected rows to the right.</summary>
		SlideRight = 0x40,
	}

	/// <summary>Specifies a bitmask of options for enumerating the items participating in a drag session.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSDraggingItemEnumerationOptions : ulong {
		/// <summary>Enumerates the dragging items concurrently, rather than sequentially.</summary>
		Concurrent = 1 << 0,
		/// <summary>Clears the drag images of items that are not enumerated (matched) by the enumeration.</summary>
		ClearNonenumeratedImages = 1 << 16,
	}

	/// <summary>Specifies how multiple dragged items are visually arranged during a drag session.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSDraggingFormation : long {
		/// <summary>Uses the default formation for the current context.</summary>
		Default,
		/// <summary>Displays each item at its original location, without rearranging them.</summary>
		None,
		/// <summary>Stacks the items in an overlapping pile.</summary>
		Pile,
		/// <summary>Arranges the items in a list.</summary>
		List,
		/// <summary>Lays out the items overlapping diagonally.</summary>
		Stack,
	}

	/// <summary>Specifies whether a dragging operation occurs within or outside the application.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSDraggingContext : long {
		/// <summary>The dragging operation occurs outside the application.</summary>
		OutsideApplication,
		/// <summary>The dragging operation occurs within the application.</summary>
		WithinApplication,
	}

	/// <summary>Specifies the default animation behavior used when a window appears, closes, or is ordered onto the screen.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowAnimationBehavior : long {
		/// <summary>Uses the default animation behavior, chosen automatically based on the window's style.</summary>
		Default = 0,
		/// <summary>Disables default window animations.</summary>
		None = 2,
		/// <summary>Uses the animation behavior appropriate for a standard document window.</summary>
		DocumentWindow,
		/// <summary>Uses the animation behavior appropriate for a utility window or panel.</summary>
		UtilityWindow,
		/// <summary>Uses the animation behavior appropriate for an alert panel.</summary>
		AlertPanel,
	}

	/// <summary>Specifies an action to perform on an <see cref="T:AppKit.NSTextFinder" />, such as showing the find bar or navigating between matches.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextFinderAction : long {
		/// <summary>Shows the find interface.</summary>
		ShowFindInterface = 1,
		/// <summary>Selects and scrolls to the next match.</summary>
		NextMatch = 2,
		/// <summary>Selects and scrolls to the previous match.</summary>
		PreviousMatch = 3,
		/// <summary>Replaces all matches with the replacement text.</summary>
		ReplaceAll = 4,
		/// <summary>Replaces the current match with the replacement text.</summary>
		Replace = 5,
		/// <summary>Replaces the current match and advances to the next match.</summary>
		ReplaceAndFind = 6,
		/// <summary>Sets the search string from the current selection.</summary>
		SetSearchString = 7,
		/// <summary>Replaces all matches found within the current selection.</summary>
		ReplaceAllInSelection = 8,
		/// <summary>Selects all matches.</summary>
		SelectAll = 9,
		/// <summary>Selects all matches found within the current selection.</summary>
		SelectAllInSelection = 10,
		/// <summary>Hides the find interface.</summary>
		HideFindInterface = 11,
		/// <summary>Shows the find-and-replace interface.</summary>
		ShowReplaceInterface = 12,
		/// <summary>Hides the find-and-replace interface.</summary>
		HideReplaceInterface = 13,
	}

	/// <summary>Specifies a bitmask of the panels and controls displayed by the shared font panel.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSFontPanelMode : ulong {
		/// <summary>Shows the font family/typeface controls.</summary>
		FaceMask = 1 << 0,
		/// <summary>Shows the font size control.</summary>
		SizeMask = 1 << 1,
		/// <summary>Shows the font collection list.</summary>
		CollectionMask = 1 << 2,
		/// <summary>Shows the underline effect control.</summary>
		UnderlineEffectMask = 1 << 8,
		/// <summary>Shows the strikethrough effect control.</summary>
		StrikethroughEffectMask = 1 << 9,
		/// <summary>Shows the text color control.</summary>
		TextColorEffectMask = 1 << 10,
		/// <summary>Shows the document background color control.</summary>
		DocumentColorEffectMask = 1 << 11,
		/// <summary>Shows the shadow effect control.</summary>
		ShadowEffectMask = 1 << 12,
		/// <summary>Shows all of the available text effect controls.</summary>
		AllEffectsMask = 0XFFF00,
		/// <summary>Shows the standard font controls and text effect controls.</summary>
		StandardMask = 0xFFFF,
		/// <summary>Shows all available controls and effects.</summary>
		AllModesMask = unchecked((ulong) UInt32.MaxValue),
	}

	/// <summary>Specifies the scope in which a font collection is visible.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSFontCollectionVisibility : ulong {
		/// <summary>The collection is visible only to the current process and is not persistent.</summary>
		Process = 1 << 0,
		/// <summary>The collection is persisted and visible to all processes for the current user.</summary>
		User = 1 << 1,
		/// <summary>The collection is persisted and visible to all users of the computer.</summary>
		Computer = 1 << 2,
	}

	/// <summary>Specifies how much of the shared content a sharing service should consider when validating or presenting items.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSharingContentScope : long {
		/// <summary>Each item is shared as a full, standalone item.</summary>
		Item,
		/// <summary>The items represent only part of a larger piece of content, such as a portion of a document.</summary>
		Partial,
		/// <summary>The items together represent the complete content, such as an entire document.</summary>
		Full,
	}

	/// <summary>Specifies a bitmask of actions a typesetter should take when it encounters a control character while laying out text.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSTypesetterControlCharacterAction : ulong {
		/// <summary>The control character advances by zero width and is otherwise skipped during layout.</summary>
		ZeroAdvancement = 1 << 0,
		/// <summary>The control character is treated as whitespace.</summary>
		Whitespace = 1 << 1,
		/// <summary>The control character is treated as a horizontal tab.</summary>
		HorizontalTab = 1 << 2,
		/// <summary>The control character forces a line break.</summary>
		LineBreak = 1 << 3,
		/// <summary>The control character forces a paragraph break.</summary>
		ParagraphBreak = 1 << 4,
		/// <summary>The control character forces a break to a new text container.</summary>
		ContainerBreak = 1 << 5,
	}

	/// <summary>Specifies the visual transition style used by an <see cref="T:AppKit.NSPageController" /> when navigating between pages.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPageControllerTransitionStyle : long {
		/// <summary>Pages are stacked and animated similarly to navigating back and forward through history.</summary>
		StackHistory,
		/// <summary>Pages are stacked and animated similarly to turning the pages of a book.</summary>
		StackBook,
		/// <summary>Pages are arranged in a horizontal strip and animated by sliding.</summary>
		HorizontalStrip,
	}

	/// <summary>Specifies whether a window's title is displayed.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowTitleVisibility : long {
		/// <summary>The window title is displayed.</summary>
		Visible = 0,
		/// <summary>The window title is hidden.</summary>
		Hidden = 1,
	}

	/// <summary>Specifies a bitmask of options describing the transition animation used when one view controller replaces another.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSViewControllerTransitionOptions : ulong {
		/// <summary>No transition animation; the new view controller replaces the old one immediately.</summary>
		None = 0x0,
		/// <summary>Crossfades from the old view controller's view to the new one.</summary>
		Crossfade = 0x1,
		/// <summary>Slides the old view controller's view up while the new view comes in from the bottom.</summary>
		SlideUp = 0x10,
		/// <summary>Slides the old view controller's view down while the new view comes in from the top.</summary>
		SlideDown = 0x20,
		/// <summary>Slides the new view controller's view in from the right.</summary>
		SlideLeft = 0x40,
		/// <summary>Slides the new view controller's view in from the left.</summary>
		SlideRight = 0x80,
		/// <summary>Slides the new view controller's view in as if moving forward, honoring the current layout direction.</summary>
		SlideForward = 0x140,
		/// <summary>Slides the new view controller's view in as if moving backward, honoring the current layout direction.</summary>
		SlideBackward = 0x180,
		/// <summary>Allows the user to interact with the views while the transition animation is in progress.</summary>
		AllowUserInteraction = 0x1000,
	}

	/// <summary>Describes whether an application has visible content.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSApplicationOcclusionState : ulong {
		/// <summary>At least part of the application's content is visible.</summary>
		Visible = 1 << 1,
	}

	/// <summary>Describes whether a window is visible to the user.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSWindowOcclusionState : ulong {
		/// <summary>At least part of the window is visible.</summary>
		Visible = 1 << 1,
	}



	#region NSVisualEffectView
	/// <summary>Specifies a semantic vibrancy material used to render the background of an <see cref="T:AppKit.NSVisualEffectView" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSVisualEffectMaterial : long {
		/// <summary>Chooses a material automatically based on the view's appearance (light or dark).</summary>
		[Advice ("Use a specific material instead.")]
		AppearanceBased,
		/// <summary>A light-colored material.</summary>
		[Advice ("Use a semantic material instead.")]
		Light,
		/// <summary>A dark-colored material.</summary>
		[Advice ("Use a semantic material instead.")]
		Dark,
		/// <summary>The material used behind a window's title bar.</summary>
		Titlebar,
		/// <summary>The material used behind selected content, such as a selected table row.</summary>
		Selection,
		/// <summary>The material used behind menus.</summary>
		Menu,
		/// <summary>The material used behind popovers.</summary>
		Popover,
		/// <summary>The material used behind sidebars.</summary>
		Sidebar,
		/// <summary>A medium-light material.</summary>
		[Advice ("Use a semantic material instead.")]
		MediumLight,
		/// <summary>An ultra-dark material.</summary>
		[Advice ("Use a semantic material instead.")]
		UltraDark,
		/// <summary>The material used behind header views, such as table or outline view headers.</summary>
		HeaderView = 10,
		/// <summary>The material used behind sheets.</summary>
		Sheet = 11,
		/// <summary>The material used for a standard window's background.</summary>
		WindowBackground = 12,
		/// <summary>The material used behind heads-up display (HUD) windows.</summary>
		HudWindow = 13,
		/// <summary>The material used for full-screen user interface elements.</summary>
		FullScreenUI = 15,
		/// <summary>The material used behind tooltips.</summary>
		ToolTip = 17,
		/// <summary>The material used behind the background of a window's content area.</summary>
		ContentBackground = 18,
		/// <summary>The material that appears underneath a window's background, visible through translucent window areas.</summary>
		UnderWindowBackground = 21,
		/// <summary>The material that appears underneath a page's background, for use with paginated content.</summary>
		UnderPageBackground = 22,
	}

	/// <summary>Specifies which content a visual effect view blends with.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSVisualEffectBlendingMode : long {
		/// <summary>Blends with content behind the window.</summary>
		BehindWindow,
		/// <summary>Blends with content behind the view in the current window.</summary>
		WithinWindow,
	}

	/// <summary>Specifies whether a visual effect view appears active or inactive, affecting the vibrancy of its material.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSVisualEffectState : long {
		/// <summary>The visual effect view's appearance follows the active or inactive state of its window.</summary>
		FollowsWindowActiveState,
		/// <summary>The visual effect view always appears active, regardless of its window's state.</summary>
		Active,
		/// <summary>The visual effect view always appears inactive, regardless of its window's state.</summary>
		Inactive,
	}
	#endregion

	/// <summary>Specifies how a control responds to varying levels of pressure on a Force Touch trackpad.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPressureBehavior : long {
		/// <summary>The pressure behavior is not known.</summary>
		Unknown = -1,
		/// <summary>Uses the default pressure behavior for the control.</summary>
		PrimaryDefault = 0,
		/// <summary>Behaves like a standard click, with a single activation threshold.</summary>
		PrimaryClick = 1,
		/// <summary>Provides continuous, generic feedback proportional to the amount of pressure applied.</summary>
		PrimaryGeneric = 2,
		/// <summary>Repeats its action at a rate that accelerates as more pressure is applied.</summary>
		PrimaryAccelerator = 3,
		/// <summary>Requires a deep press to activate, similar to a "Force Click".</summary>
		PrimaryDeepClick = 5,
		/// <summary>Provides two-stage pressure sensing that remains active while dragging.</summary>
		PrimaryDeepDrag = 6,
	}

	/// <summary>Specifies a predefined haptic feedback pattern played on the Force Touch trackpad.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSHapticFeedbackPattern : long {
		/// <summary>A generic, general-purpose haptic pattern.</summary>
		Generic = 0,
		/// <summary>A haptic pattern indicating that an object aligned with a guide or another object.</summary>
		Alignment,
		/// <summary>A haptic pattern indicating a discrete change in a value, such as a slider snapping to a level.</summary>
		LevelChange,
	}

	/// <summary>Specifies when a haptic feedback pattern is played relative to the visual update it accompanies.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSHapticFeedbackPerformanceTime : ulong {
		/// <summary>Uses the default timing for the current context.</summary>
		Default = 0,
		/// <summary>Plays the haptic feedback immediately.</summary>
		Now,
		/// <summary>Plays the haptic feedback after the corresponding visual update has finished drawing.</summary>
		DrawCompleted,
	}

	/// <summary>Specifies the highlight style shown on a spring-loaded destination during a drag-and-drop operation.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSpringLoadingHighlight : long {
		/// <summary>No highlight is shown.</summary>
		None = 0,
		/// <summary>The standard spring-loading highlight is shown.</summary>
		Standard,
		/// <summary>An emphasized spring-loading highlight is shown.</summary>
		Emphasized,
	}

	/// <summary>Specifies a bitmask of options controlling whether, and how, a view participates in spring-loading during drag-and-drop.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSSpringLoadingOptions : ulong {
		/// <summary>Spring-loading is disabled.</summary>
		Disabled = 0,
		/// <summary>Spring-loading is enabled.</summary>
		Enabled = 1 << 0,
		/// <summary>Spring-loading activates when a force click begins or the hover timeout elapses, and deactivates when the force click is released or the drag exits the target.</summary>
		ContinuousActivation = 1 << 1,
		/// <summary>Spring-loading is enabled but cannot be activated by hovering; the user must force click the target.</summary>
		NoHover = 1 << 3,
	}

	/// <summary>Specifies how to order windows in a window list.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSWindowListOptions : long {
		/// <summary>Orders windows from front to back.</summary>
		OrderedFrontToBack = (1 << 0),
	}

	/// <summary>Specifies optional behaviors for a status item.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSStatusItemBehavior : ulong {
		/// <summary>Allows the user to remove the status item from the menu bar.</summary>
		RemovalAllowed = (1 << 1),
		/// <summary>Allows the user to remove the status item and terminates the application when they do so. This option implies <see cref="RemovalAllowed" />.</summary>
		TerminationOnRemoval = (1 << 2),
	}

	/// <summary>Specifies whether a window supports tabbing with other windows.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowTabbingMode : long {
		/// <summary>The system decides automatically whether the window supports tabbing, based on the user's tabbing preference.</summary>
		Automatic,
		/// <summary>The window prefers to open as a tab whenever possible.</summary>
		Preferred,
		/// <summary>The window never opens as a tab.</summary>
		Disallowed,
	}

	/// <summary>Specifies the user's system-wide preference for when new windows should be opened as tabs.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowUserTabbingPreference : long {
		/// <summary>The user prefers to open new windows as tabs only when explicitly requested.</summary>
		Manual,
		/// <summary>The user prefers to always open new windows as tabs.</summary>
		Always,
		/// <summary>The user prefers to open new windows as tabs only while in full-screen mode.</summary>
		InFullScreen,
	}

	/// <summary>Specifies how a view is aligned within its cell in an <see cref="T:AppKit.NSGridView" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSGridCellPlacement : long {
		/// <summary>Inherits the placement from the grid view's row or column.</summary>
		Inherited = 0,
		/// <summary>Does not constrain the view's placement within the cell.</summary>
		None,
		/// <summary>Aligns the view with the leading edge of the cell.</summary>
		Leading,
		/// <summary>Aligns the view with the top edge of the cell. Equivalent to <see cref="F:AppKit.NSGridCellPlacement.Leading" />.</summary>
		Top = Leading,
		/// <summary>Aligns the view with the trailing edge of the cell.</summary>
		Trailing,
		/// <summary>Aligns the view with the bottom edge of the cell. Equivalent to <see cref="F:AppKit.NSGridCellPlacement.Trailing" />.</summary>
		Bottom = Trailing,
		/// <summary>Centers the view within the cell.</summary>
		Center,
		/// <summary>Resizes the view to fill the entire cell.</summary>
		Fill,
	}

	/// <summary>Specifies how the content of the cells in a grid view row is aligned vertically.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSGridRowAlignment : long {
		/// <summary>Inherits the alignment from the grid view.</summary>
		Inherited = 0,
		/// <summary>Does not apply any special baseline alignment.</summary>
		None,
		/// <summary>Aligns the cells' content to the first baseline of the row.</summary>
		FirstBaseline,
		/// <summary>Aligns the cells' content to the last baseline of the row.</summary>
		LastBaseline,
	}

	/// <summary>Specifies the layout direction used when drawing a directional image.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageLayoutDirection : long {
		/// <summary>The image does not have a specific layout direction.</summary>
		Unspecified = -1,
		/// <summary>The image is drawn for a left-to-right layout direction.</summary>
		LeftToRight = 2,
		/// <summary>The image is drawn for a right-to-left layout direction.</summary>
		RightToLeft = 3,
	}

	/// <summary>Specifies a bitmask of options controlling which CloudKit sharing configurations are offered by the sharing service.</summary>
	[NoMacCatalyst]
	[Native]
	[Flags]
	public enum NSCloudKitSharingServiceOptions : ulong {
		/// <summary>Uses the standard set of sharing options.</summary>
		Standard = 0,
		/// <summary>Allows sharing with anyone who has the link (public sharing).</summary>
		AllowPublic = 1 << 0,
		/// <summary>Allows sharing only with specifically invited people (private sharing).</summary>
		AllowPrivate = 1 << 1,
		/// <summary>Allows participants to be granted read-only access.</summary>
		AllowReadOnly = 1 << 4,
		/// <summary>Allows participants to be granted read-write access.</summary>
		AllowReadWrite = 1 << 5,
	}

	/// <summary>Specifies a color gamut that a display can represent.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSDisplayGamut : long {
		/// <summary>The sRGB color gamut.</summary>
		Srgb = 1,
		/// <summary>The Display P3 color gamut.</summary>
		P3,
	}

	/// <summary>Specifies which edge of a tab view displays its tabs.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTabPosition : ulong {
		/// <summary>No tabs are displayed.</summary>
		None = 0,
		/// <summary>Tabs are displayed along the top edge.</summary>
		Top,
		/// <summary>Tabs are displayed along the left edge.</summary>
		Left,
		/// <summary>Tabs are displayed along the bottom edge.</summary>
		Bottom,
		/// <summary>Tabs are displayed along the right edge.</summary>
		Right,
	}

	/// <summary>Specifies the style of border drawn around a tab view.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTabViewBorderType : ulong {
		/// <summary>No border is drawn.</summary>
		None = 0,
		/// <summary>A plain line border is drawn.</summary>
		Line,
		/// <summary>A bezeled border is drawn.</summary>
		Bezel,
	}

	/// <summary>Specifies options for replacing the contents of a pasteboard.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPasteboardContentsOptions : ulong {
		/// <summary>Restricts the pasteboard contents to the current host.</summary>
		CurrentHostOnly = 1,
	}

	/// <summary>Specifies whether contact occurs directly on a display or through an indirect input device.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTouchType : long {
		/// <summary>Contact occurs directly on a display.</summary>
		Direct,
		/// <summary>Contact occurs on an indirect input device, such as a trackpad.</summary>
		Indirect,
	}

	/// <summary>Specifies the types of touch input to include.</summary>
	[NoMacCatalyst]
	[Native]
	[Flags]
	public enum NSTouchTypeMask : ulong {
		/// <summary>Includes direct touch input.</summary>
		Direct = (1 << (int) NSTouchType.Direct),
		/// <summary>Includes indirect touch input.</summary>
		Indirect = (1 << (int) NSTouchType.Indirect),
	}

	/// <summary>Specifies how a scrubber responds to touch input.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSScrubberMode : long {
		/// <summary>Items remain fixed while the user moves the selection across them.</summary>
		Fixed = 0,
		/// <summary>Items scroll freely in response to the user's touch.</summary>
		Free,
	}

	/// <summary>Specifies how a scrubber aligns its selected item within its visible bounds.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSScrubberAlignment : long {
		/// <summary>No particular alignment is applied.</summary>
		None = 0,
		/// <summary>Aligns the selected item with the leading edge of the scrubber.</summary>
		Leading,
		/// <summary>Aligns the selected item with the trailing edge of the scrubber.</summary>
		Trailing,
		/// <summary>Centers the selected item within the scrubber.</summary>
		Center,
	}

	/// <summary>Specifies an error code related to font asset requests.</summary>
	[NoMacCatalyst]
	public enum NSFontError : int {
		/// <summary>Indicates an error while downloading a font asset.</summary>
		AssetDownloadError = 66304,
		/// <summary>The smallest error code value reserved for font-related errors.</summary>
		ErrorMinimum = 66304,
		/// <summary>The largest error code value reserved for font-related errors.</summary>
		ErrorMaximum = 66335,
	}

	/// <summary>Specifies the position of an accessibility annotation within a range.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSAccessibilityAnnotationPosition : long {
		/// <summary>The annotation applies to the entire range.</summary>
		FullRange,
		/// <summary>The annotation applies to the start of the range.</summary>
		Start,
		/// <summary>The annotation applies to the end of the range.</summary>
		End,
	}

	/// <summary>Specifies the direction in which an accessibility custom rotor searches.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSAccessibilityCustomRotorSearchDirection : long {
		/// <summary>Searches for the previous item.</summary>
		Previous,
		/// <summary>Searches for the next item.</summary>
		Next,
	}

	/// <summary>Specifies a category of content that VoiceOver's accessibility rotor can navigate between.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSAccessibilityCustomRotorType : long {
		/// <summary>A custom, application-defined rotor category.</summary>
		Custom = 0,
		/// <summary>Any kind of accessibility element.</summary>
		Any = 1,
		/// <summary>An annotation, such as a comment or tracked change.</summary>
		Annotation,
		/// <summary>Text rendered in a bold font.</summary>
		BoldText,
		/// <summary>A heading of any level.</summary>
		Heading,
		/// <summary>A level-1 heading.</summary>
		HeadingLevel1,
		/// <summary>A level-2 heading.</summary>
		HeadingLevel2,
		/// <summary>A level-3 heading.</summary>
		HeadingLevel3,
		/// <summary>A level-4 heading.</summary>
		HeadingLevel4,
		/// <summary>A level-5 heading.</summary>
		HeadingLevel5,
		/// <summary>A level-6 heading.</summary>
		HeadingLevel6,
		/// <summary>An image.</summary>
		Image,
		/// <summary>Text rendered in an italic font.</summary>
		ItalicText,
		/// <summary>A landmark region, such as navigation or main content.</summary>
		Landmark,
		/// <summary>A hyperlink.</summary>
		Link,
		/// <summary>A list.</summary>
		List,
		/// <summary>A misspelled word.</summary>
		MisspelledWord,
		/// <summary>A table.</summary>
		Table,
		/// <summary>A text field.</summary>
		TextField,
		/// <summary>Text with an underline.</summary>
		UnderlinedText,
		/// <summary>A hyperlink that has already been visited.</summary>
		VisitedLink,
		/// <summary>An audio graph, describing a chart or graph through sound.</summary>
		Audiograph,
	}

	/// <summary>Specifies how a color represents its color data.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSColorType : long {
		/// <summary>A color defined by components in a color space.</summary>
		ComponentBased,
		/// <summary>A color defined by a repeating image pattern.</summary>
		Pattern,
		/// <summary>A color obtained from a named color catalog.</summary>
		Catalog,
	}

	/// <summary>Specifies options for requesting downloadable font assets.</summary>
	[NoMacCatalyst]
	[Native]
	[Flags]
	public enum NSFontAssetRequestOptions : ulong {
		/// <summary>Displays the standard user interface while downloading fonts.</summary>
		UsesStandardUI = 1 << 0,
	}

	/// <summary>Specifies a bitmask of the panels and controls displayed by the shared font panel.</summary>
	[NoMacCatalyst]
	[Native]
	[Flags]
	public enum NSFontPanelModeMask : ulong {
		/// <summary>Shows the font family/typeface controls.</summary>
		Face = 1 << 0,
		/// <summary>Shows the font size control.</summary>
		Size = 1 << 1,
		/// <summary>Shows the font collection list.</summary>
		Collection = 1 << 2,
		/// <summary>Shows the underline effect control.</summary>
		UnderlineEffect = 1 << 8,
		/// <summary>Shows the strikethrough effect control.</summary>
		StrikethroughEffect = 1 << 9,
		/// <summary>Shows the text color control.</summary>
		TextColorEffect = 1 << 10,
		/// <summary>Shows the document background color control.</summary>
		DocumentColorEffect = 1 << 11,
		/// <summary>Shows the shadow effect control.</summary>
		ShadowEffect = 1 << 12,
		/// <summary>Shows all of the available text effect controls.</summary>
		AllEffects = (ulong) 0XFFF00,
		/// <summary>Shows the standard font controls and text effect controls.</summary>
		StandardModes = (ulong) 0XFFFF,
		/// <summary>Shows all available controls and effects.</summary>
		AllModes = (ulong) 0XFFFFFFFF,
	}

	/// <summary>Specifies when a level indicator displays placeholder tick marks in place of missing values.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSLevelIndicatorPlaceholderVisibility : long {
		/// <summary>The system decides automatically when to show placeholders.</summary>
		Automatic = 0,
		/// <summary>Placeholders are always shown for missing values.</summary>
		Always = 1,
		/// <summary>Placeholders are shown only while the level indicator is being edited.</summary>
		WhileEditing = 2,
	}

	/// <summary>Specifies how a segmented control sizes and distributes its segments across its width.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSSegmentDistribution : long {
		/// <summary>Sizes each segment to fit its content, without necessarily filling the control's width.</summary>
		Fit = 0,
		/// <summary>Resizes segments to fill the control's width, distributing extra space equally among dynamically sized segments. This is the default distribution.</summary>
		Fill,
		/// <summary>Resizes all segments equally to fill the control's width.</summary>
		FillEqually,
		/// <summary>Resizes segments proportionally, based on their initial sizes, to fill the control's width.</summary>
		FillProportionally,
	}

	/// <summary>Specifies a system-defined visual effect applied to a color to reflect a control's interaction state.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSColorSystemEffect : long {
		/// <summary>No effect is applied to the color.</summary>
		None,
		/// <summary>The effect applied to a color while its control is pressed.</summary>
		Pressed,
		/// <summary>The effect applied to a color while its control is pressed deeply, such as with a Force Touch trackpad.</summary>
		DeepPressed,
		/// <summary>The effect applied to a color when its control is disabled.</summary>
		Disabled,
		/// <summary>The effect applied to a color while the pointer is hovering over its control.</summary>
		Rollover,
	}

	/// <summary>Specifies a type of privileged file-system operation that requires the user's authorization.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWorkspaceAuthorizationType : long {
		/// <summary>Authorization to create a symbolic link.</summary>
		CreateSymbolicLink,
		/// <summary>Authorization to set the attributes of a file or directory.</summary>
		SetAttributes,
		/// <summary>Authorization to replace the contents of a file.</summary>
		ReplaceFile,
	}

	/// <summary>Specifies the overall visual style of an <see cref="T:AppKit.NSTableView" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTableViewStyle : long {
		/// <summary>The system chooses the most appropriate style automatically, based on the table view's context.</summary>
		Automatic,
		/// <summary>The table view's rows and content extend the full width of the enclosing scroll view.</summary>
		FullWidth,
		/// <summary>The table view's content is inset from the edges of the enclosing scroll view.</summary>
		Inset,
		/// <summary>The table view uses the source-list appearance, as seen in Finder-style sidebars.</summary>
		SourceList,
		/// <summary>The table view uses a plain appearance, with no additional insets or styling.</summary>
		Plain,
	}

	/// <summary>Specifies the visual style of the separator drawn between a window's title bar and its content.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTitlebarSeparatorStyle : long {
		/// <summary>The system chooses the most appropriate separator style automatically, based on the window's content.</summary>
		Automatic,
		/// <summary>No separator is drawn.</summary>
		None,
		/// <summary>A plain line separator is drawn.</summary>
		Line,
		/// <summary>A shadow is drawn to separate the title bar from the content.</summary>
		Shadow,
	}

	/// <summary>Specifies how a window's toolbar is displayed relative to its title bar.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSWindowToolbarStyle : long {
		/// <summary>The system chooses the most appropriate toolbar style automatically, based on the window's configuration.</summary>
		Automatic,
		/// <summary>The toolbar is expanded below a larger title, with more vertical space for toolbar items.</summary>
		Expanded,
		/// <summary>The toolbar is styled to match a Preferences-style window.</summary>
		Preference,
		/// <summary>The toolbar and title bar are unified into a single bar.</summary>
		Unified,
		/// <summary>The toolbar and title bar are unified into a single, more compact bar.</summary>
		UnifiedCompact,
	}

	/// <summary>Specifies the visual animation effect used when a diffable data source applies a snapshot to a table view.</summary>
	[NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSTableViewAnimationOptions : ulong {
		/// <summary>No animation effect.</summary>
		EffectNone = 0x0,
		/// <summary>Fades the affected rows in or out.</summary>
		EffectFade = 0x1,
		/// <summary>Opens a gap for newly inserted rows.</summary>
		EffectGap = 0x2,
		/// <summary>Slides the affected rows upward.</summary>
		SlideUp = 0x10,
		/// <summary>Slides the affected rows downward.</summary>
		SlideDown = 0x20,
		/// <summary>Slides the affected rows to the left.</summary>
		SlideLeft = 0x30,
		/// <summary>Slides the affected rows to the right.</summary>
		SlideRight = 0x40,
	}

	/// <summary>Specifies the point size variant used to render an SF Symbols image.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageSymbolScale : long {
		/// <summary>A small-scale symbol variant.</summary>
		Small = 1,
		/// <summary>A medium-scale symbol variant.</summary>
		Medium = 2,
		/// <summary>A large-scale symbol variant.</summary>
		Large = 3,
	}

	/// <summary>Specifies the visual style of an <see cref="T:AppKit.NSColorWell" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSColorWellStyle : long {
		/// <summary>Uses the default color well style.</summary>
		Default = 0,
		/// <summary>Uses a minimal, compact color well style without a disclosure button.</summary>
		Minimal,
		/// <summary>Uses an expanded color well style with a built-in disclosure button for choosing recent or custom colors.</summary>
		Expanded,
	}

	/// <summary>Specifies the visual style of an <see cref="T:AppKit.NSComboButton" />.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSComboButtonStyle : long {
		/// <summary>Displays the button's action and menu-disclosure controls as visually separate segments.</summary>
		Split = 0,
		/// <summary>Displays the button's action and menu-disclosure controls as a single, unified control.</summary>
		Unified = 1,
	}

	/// <summary>Specifies the dynamic range of an image, describing whether it contains high dynamic range (HDR) content.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSImageDynamicRange : long {
		/// <summary>The image's dynamic range is not known.</summary>
		Unspecified = -1,
		/// <summary>The image uses the standard dynamic range (SDR).</summary>
		Standard = 0,
		/// <summary>The image uses a constrained high dynamic range, suitable for contexts with limited HDR headroom.</summary>
		ConstrainedHigh = 1,
		/// <summary>The image uses the full high dynamic range (HDR).</summary>
		High = 2,
	}

	/// <summary>Specifies the kind of badge displayed on a menu item.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSMenuItemBadgeType : long {
		/// <summary>The badge has no descriptive string; only its count is displayed.</summary>
		None = 0,
		/// <summary>A badge indicating that updates are available.</summary>
		Updates,
		/// <summary>A badge indicating that new items are available.</summary>
		NewItems,
		/// <summary>A badge indicating that alerts require attention.</summary>
		Alerts,
	}

	/// <summary>Specifies how a menu is visually presented.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSMenuPresentationStyle : long {
		/// <summary>Presents the menu using the regular, standard menu appearance.</summary>
		Regular = 0,
		/// <summary>Displays horizontally arranged menu items in place of the presenting menu item.</summary>
		Palette = 1,
	}

	/// <summary>Specifies how items within a menu can be selected.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSMenuSelectionMode : long {
		/// <summary>The system chooses the most appropriate selection mode automatically.</summary>
		Automatic = 0,
		/// <summary>Allows at most one selected menu item in each selection group.</summary>
		SelectOne = 1,
		/// <summary>Any number of menu items can be selected simultaneously.</summary>
		SelectAny = 2,
	}

	/// <summary>Specifies the outcome of a page layout panel session.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPageLayoutResult : long {
		/// <summary>The user cancelled the page layout panel.</summary>
		Cancelled = 0,
		/// <summary>The user made changes and accepted the page layout panel.</summary>
		Changed,
	}

	/// <summary>Specifies the outcome of a print panel session.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSPrintPanelResult : long {
		/// <summary>The user cancelled the print panel.</summary>
		Cancelled = 0,
		/// <summary>The user confirmed the print panel and printing proceeded.</summary>
		Printed,
	}

	/// <summary>Specifies where an accessory view is placed relative to the text insertion point (cursor).</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextCursorAccessoryPlacement : long {
		/// <summary>The placement is not specified; the system chooses an appropriate placement.</summary>
		Unspecified,
		/// <summary>The accessory is shown above horizontal text or to the right of vertical text.</summary>
		Backward,
		/// <summary>The accessory is shown below horizontal text or to the left of vertical text.</summary>
		Forward,
		/// <summary>The accessory is not shown.</summary>
		Invisible,
		/// <summary>The accessory is centered above the selected text.</summary>
		Center,
		/// <summary>The accessory is shown at the left edge of the visible rectangle with a left-pointing arrow when the cursor is off-screen to the left.</summary>
		OffscreenLeft,
		/// <summary>The accessory is shown at the top edge of the visible rectangle with an upward-pointing arrow when the cursor is off-screen above.</summary>
		OffscreenTop,
		/// <summary>The accessory is shown at the right edge of the visible rectangle with a right-pointing arrow when the cursor is off-screen to the right.</summary>
		OffscreenRight,
		/// <summary>The accessory is shown at the bottom edge of the visible rectangle with a downward-pointing arrow when the cursor is off-screen below.</summary>
		OffscreenBottom,
	}

	/// <summary>Specifies a bitmask of options controlling when the text insertion point automatically displays its animated appearance.</summary>
	[NoMacCatalyst]
	[Native]
	[Flags]
	public enum NSTextInsertionIndicatorAutomaticModeOptions : long {
		/// <summary>Shows the animated insertion point effects view.</summary>
		EffectsView = 1L << 0,
		/// <summary>Shows the animated insertion point while the user is actively tracking (typing or navigating) it.</summary>
		WhileTracking = 1L << 1,
	}

	/// <summary>Specifies whether the animated text insertion point indicator is displayed.</summary>
	[NoMacCatalyst]
	[Native]
	public enum NSTextInsertionIndicatorDisplayMode : long {
		/// <summary>The system decides automatically whether to display the animated insertion point indicator.</summary>
		Automatic = 0,
		/// <summary>The animated insertion point indicator is always hidden.</summary>
		Hidden,
		/// <summary>The animated insertion point indicator is always visible.</summary>
		Visible,
	}

	/// <summary>Specifies how much confirmation is required before an application can access the general pasteboard.</summary>
	[Native]
	[Mac (15, 4), NoMacCatalyst]
	public enum NSPasteboardAccessBehavior : ulong {
		/// <summary>Uses the system's default pasteboard access behavior.</summary>
		Default = 0,
		/// <summary>Prompts the user before access that is not part of a user-initiated paste operation.</summary>
		Ask = 1,
		/// <summary>Always allows the application to access the pasteboard without prompting.</summary>
		AlwaysAllow = 2,
		/// <summary>Denies access that is not part of a user-initiated paste operation without prompting.</summary>
		AlwaysDeny = 3,
	}

	/// <summary>Specifies a kind of content to detect in pasteboard items.</summary>
	[Mac (15, 4), NoMacCatalyst]
	enum NSPasteboardDetectionPattern {
		/// <summary>Detects text that is likely to be a web URL.</summary>
		[Field ("NSPasteboardDetectionPatternProbableWebURL")]
		ProbableWebUrl,

		/// <summary>Detects text that is likely to be a web search query.</summary>
		[Field ("NSPasteboardDetectionPatternProbableWebSearch")]
		ProbableWebSearch,

		/// <summary>Detects numbers.</summary>
		[Field ("NSPasteboardDetectionPatternNumber")]
		Number,

		/// <summary>Detects links.</summary>
		[Field ("NSPasteboardDetectionPatternLink")]
		Link,

		/// <summary>Detects phone numbers.</summary>
		[Field ("NSPasteboardDetectionPatternPhoneNumber")]
		PhoneNumber,

		/// <summary>Detects email addresses.</summary>
		[Field ("NSPasteboardDetectionPatternEmailAddress")]
		EmailAddress,

		/// <summary>Detects postal addresses.</summary>
		[Field ("NSPasteboardDetectionPatternPostalAddress")]
		PostalAddress,

		/// <summary>Detects calendar events.</summary>
		[Field ("NSPasteboardDetectionPatternCalendarEvent")]
		CalendarEvent,

		/// <summary>Detects shipment tracking numbers.</summary>
		[Field ("NSPasteboardDetectionPatternShipmentTrackingNumber")]
		ShipmentTrackingNumber,

		/// <summary>Detects flight numbers.</summary>
		[Field ("NSPasteboardDetectionPatternFlightNumber")]
		FlightNumber,

		/// <summary>Detects monetary amounts.</summary>
		[Field ("NSPasteboardDetectionPatternMoneyAmount")]
		MoneyAmount,
	}

	/// <summary>Specifies a key identifying a piece of metadata that can be detected within a pasteboard item.</summary>
	[Mac (15, 4), NoMacCatalyst]
	enum NSPasteboardMetadataType {
		/// <summary>The uniform type identifier (content type) of the pasteboard item.</summary>
		[Field ("NSPasteboardMetadataTypeContentType")]
		ContentType,
	}

	/// <summary>Specifies the visual prominence of a toolbar item.</summary>
	[MacCatalyst (26, 0), Mac (26, 0)]
	[Native]
	public enum NSToolbarItemStyle : long {
		/// <summary>The toolbar item uses the standard, unemphasized appearance.</summary>
		Plain,
		/// <summary>The toolbar item uses a visually emphasized, prominent appearance.</summary>
		Prominent,
	}

	/// <summary>Specifies how color is applied when rendering a multicolor or variable-value SF Symbol image.</summary>
	[NoMacCatalyst, Mac (26, 0)]
	[Native]
	public enum NSImageSymbolColorRenderingMode : long {
		/// <summary>Uses the default color rendering mode for the symbol's configuration.</summary>
		Automatic = 0,
		/// <summary>Renders the symbol using flat, solid colors.</summary>
		Flat,
		/// <summary>Renders the symbol using a gradient effect.</summary>
		Gradient,
	}

	/// <summary>Specifies how a variable-value SF Symbol, such as a signal-strength icon, visually represents its current value.</summary>
	[NoMacCatalyst, Mac (26, 0)]
	[Native]
	public enum NSImageSymbolVariableValueMode : long {
		/// <summary>Uses the default representation mode for the symbol.</summary>
		Automatic = 0,
		/// <summary>Represents the value by coloring a proportional part of the symbol.</summary>
		Color,
		/// <summary>Represents the value by drawing a proportional part of the symbol's layers.</summary>
		Draw,
	}

	/// <summary>Specifies how prominently a control's tint color is applied to its appearance.</summary>
	[NoMacCatalyst, Mac (26, 0)]
	[Native]
	public enum NSTintProminence : long {
		/// <summary>Uses the default tint prominence for the control.</summary>
		Automatic = 0,
		/// <summary>Does not apply a tint color.</summary>
		None,
		/// <summary>Applies the tint color as the primary, most prominent color.</summary>
		Primary,
		/// <summary>Applies the tint color with secondary, less prominent emphasis.</summary>
		Secondary,
	}

	/// <summary>Specifies the shape of the border drawn around a control.</summary>
	[NoMacCatalyst, Mac (26, 0)]
	[Native]
	public enum NSControlBorderShape : long {
		/// <summary>Uses the default border shape for the control.</summary>
		Automatic,
		/// <summary>Draws a capsule-shaped (fully rounded) border.</summary>
		Capsule,
		/// <summary>Draws a rounded-rectangle border.</summary>
		RoundedRectangle,
		/// <summary>Draws a circular border.</summary>
		Circle,
	}
}
