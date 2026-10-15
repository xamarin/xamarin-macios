/*
 * CGEvenTypes.cs: bindings to the ApplicationServices framework's CoreGraphics APIs
 * 
 * Copyright 2013, 2014 Xamarin Inc
 * All Rights Reserved
 * 
 * Authors:
 *    Miguel de Icaza
 */

#nullable enable

#if MONOMAC || __MACCATALYST__

using System.Drawing;

using CoreFoundation;

namespace CoreGraphics {

	// CGEventTypes.h:typedef uint32_t CGEventTapLocation;
	/// <summary>Specifies the point in the event system where an event tap observes events.</summary>
	/// <remarks>Event taps can observe or filter events at different stages, depending on the selected location and tap options.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGEventTapLocation : int {
		/// <summary>Places the event tap at the HID system level.</summary>
		HID,
		/// <summary>Places the event tap in the current login session.</summary>
		Session,
		/// <summary>Places the event tap at the annotated session point.</summary>
		AnnotatedSession,
	}

	// CGEventTypes.h:typedef uint32_t CGEventTapPlacement;
	/// <summary>Specifies where a new event tap is inserted in the active event tap list.</summary>
	/// <remarks>The placement determines the tap's position relative to other active event taps.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGEventTapPlacement : uint {
		/// <summary>Inserts the event tap at the head of the active tap list.</summary>
		HeadInsert,
		/// <summary>Appends the event tap at the tail of the active tap list.</summary>
		TailAppend,
	}

	// CGEventTypes.h:typedef uint32_t CGEventTapOptions;
	/// <summary>Specifies whether an event tap filters events or only observes them.</summary>
	/// <remarks>The default option creates an active filter; listen-only taps cannot modify events.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGEventTapOptions : uint {
		/// <summary>Creates an active event filter.</summary>
		Default,
		/// <summary>Creates a passive listener that observes events without modifying them.</summary>
		ListenOnly,
	}

	// CGEventTypes.h:typedef uint32_t CGMouseButton;
	/// <summary>Identifies a standard button on a mouse.</summary>
	/// <remarks>The enumeration provides button identifiers for mice with up to three buttons.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGMouseButton : uint {
		/// <summary>The left mouse button.</summary>
		Left,
		/// <summary>The right mouse button.</summary>
		Right,
		/// <summary>The center mouse button.</summary>
		Center,
	}

	// CGEventTypes.h:typedef uint32_t CGScrollEventUnit;
	/// <summary>Specifies the unit used to express a scrolling event's delta.</summary>
	/// <remarks>Use this value to indicate whether scroll deltas are measured in pixels or lines.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGScrollEventUnit : uint {
		/// <summary>Measures scroll deltas in pixels.</summary>
		Pixel,
		/// <summary>Measures scroll deltas in lines.</summary>
		Line,
	}

	// CGEventTypes.h:typedef uint64_t CGEventMask;
	/// <summary>A mask that identifies Quartz event types for an event tap to observe.</summary>
	/// <remarks>Combine values to select the events delivered to the tap's callback.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[Flags]
	public enum CGEventMask : ulong {
		/// <summary>The mask bit for a null event.</summary>
		Null = 0x00000001,
		/// <summary>The mask bit for a left mouse button press.</summary>
		LeftMouseDown = 0x00000002,
		/// <summary>The mask bit for a left mouse button release.</summary>
		LeftMouseUp = 0x00000004,
		/// <summary>The mask bit for a right mouse button press.</summary>
		RightMouseDown = 0x00000008,
		/// <summary>The mask bit for a right mouse button release.</summary>
		RightMouseUp = 0x00000010,
		/// <summary>The mask bit for mouse movement without a button drag.</summary>
		MouseMoved = 0x00000020,
		/// <summary>The mask bit for mouse movement while the left button is held down.</summary>
		LeftMouseDragged = 0x00000040,
		/// <summary>The mask bit for mouse movement while the right button is held down.</summary>
		RightMouseDragged = 0x00000080,
		/// <summary>The mask bit for a keyboard key press.</summary>
		KeyDown = 0x00000400,
		/// <summary>The mask bit for a keyboard key release.</summary>
		KeyUp = 0x00000800,
		/// <summary>The mask bit for a change in modifier-key state.</summary>
		FlagsChanged = 0x00001000,
		/// <summary>The mask bit for a scroll-wheel event.</summary>
		ScrollWheel = 0x00400000,
		/// <summary>The mask bit for a tablet pointer event.</summary>
		TabletPointer = 0x00800000,
		/// <summary>The mask bit for a tablet proximity event.</summary>
		TabletProximity = 0x01000000,
		/// <summary>The mask bit for a non-left or non-right mouse button press.</summary>
		OtherMouseDown = 0x02000000,
		/// <summary>The mask bit for a non-left or non-right mouse button release.</summary>
		OtherMouseUp = 0x04000000,
		/// <summary>The mask bit for movement while a non-left or non-right mouse button is held down.</summary>
		OtherMouseDragged = 0x08000000,
	}

	// CGEventTypes.h:typedef uint64_t CGEventFlags;
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[Flags]
	public enum CGEventFlags : ulong {
		/// <summary>To be added.</summary>
		NonCoalesced = 0x00000100,
		/// <summary>To be added.</summary>
		AlphaShift = 0x00010000,
		/// <summary>To be added.</summary>
		Shift = 0x00020000,
		/// <summary>To be added.</summary>
		Control = 0x00040000,
		/// <summary>To be added.</summary>
		Alternate = 0x00080000,
		/// <summary>To be added.</summary>
		Command = 0x00100000,
		/// <summary>To be added.</summary>
		NumericPad = 0x00200000,
		/// <summary>To be added.</summary>
		Help = 0x00400000,
		/// <summary>To be added.</summary>
		SecondaryFn = 0x00800000,
	}

	// CGEventTypes.h:typedef uint32_t CGEventField;
	/// <summary>This enum represents constants used to access specialized fields in low-level events.</summary>
	/// <remarks>Call <see cref="CGEvent.GetLongValueField" /> or <see cref="CGEvent.GetDoubleValueField" /> to fetch the field values, and <see cref="CGEvent.SetValueField(CGEventField,long)" /> or <see cref="CGEvent.SetValueField(CGEventField,double)" /> to set the field values represented by the constants in this enum.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGEventField : int {
		/// <summary>The mouse button event number. This is an integer field. Matching mouse-down and mouse-up events will have the same event number.</summary>
		MouseEventNumber = 0,

		/// <summary>The mouse button click state. This is an integer field. A value of 1 is a single click, a value of 2 is a double click, and so on.</summary>
		MouseEventClickState = 1,

		/// <summary>The mouse button pressure state, ranging from 0 (mouse being up) to 1. This is an integer field.</summary>
		MouseEventPressure = 2,

		/// <summary>The mouse button number. This is an integer field.</summary>
		MouseEventButtonNumber = 3,

		/// <summary>The horizontal delta since the last mouse movement event. This is an integer field.</summary>
		MouseEventDeltaX = 4,

		/// <summary>The vertical delta since the last mouse movement event. This is an integer field.</summary>
		MouseEventDeltaY = 5,

		/// <summary>A value indicating whether the event should be ignored by the Inkwell subsystem. This is an integer field. A non-zero indicates the event should be ignored.</summary>
		MouseEventInstantMouser = 6,

		/// <summary>The mouse event subtype. This is an integer field.</summary>
		MouseEventSubtype = 7,

		/// <summary>A value indicating whether the key-down event is an autorepeat of a previous key-down event. This is an integer field. Zero indicates not an autorepeat, non-zero not an autorepeat.</summary>
		KeyboardEventAutorepeat = 8,

		/// <summary>The virtual keycode of the key-down or key-up event. This is an integer field.</summary>
		KeyboardEventKeycode = 9,

		/// <summary>The keyboard ype identifier This is an integer field.</summary>
		KeyboardEventKeyboardType = 10,

		/// <summary>A value that represents scrolling data. Typically this is the vertical position change since the last scrolling event. This is an integer field.</summary>
		ScrollWheelEventDeltaAxis1 = 11,

		/// <summary>A value that represents scrolling data. Typically this is the horizontal position change since the last scrolling event. This is an integer field.</summary>
		ScrollWheelEventDeltaAxis2 = 12,

		/// <summary>This value is unused.</summary>
		ScrollWheelEventDeltaAxis3 = 13,

		/// <summary>A value that represents scrolling data. Typically this is the line-based or pixel-based vertical position change since the last scrolling event. This field is a fixed-point 16.16 signed integer, but can also be retrieved as a double and will be automatically converted.</summary>
		ScrollWheelEventFixedPtDeltaAxis1 = 93,

		/// <summary>A value that represents scrolling data. Typically this is the line-based or pixel-based horizontal position change since the last scrolling event. This field is a fixed-point 16.16 signed integer, but can also be retrieved as a double and will be automatically converted.</summary>
		ScrollWheelEventFixedPtDeltaAxis2 = 94,

		/// <summary>This value is unused.</summary>
		ScrollWheelEventFixedPtDeltaAxis3 = 95,

		/// <summary>A value that represents pixel-based scrolling data. Typically this is the vertical position change since the last scrolling event. This is an integer field.</summary>
		ScrollWheelEventPointDeltaAxis1 = 96,

		/// <summary>A value that represents pixel-based scrolling data. Typically this is the horizontal position change since the last scrolling event. This is an integer field.</summary>
		ScrollWheelEventPointDeltaAxis2 = 97,

		/// <summary>This value is unused.</summary>
		ScrollWheelEventPointDeltaAxis3 = 98,

		/// <summary>A value indicating whether the event should be ignored by the Inkwell subsystem. This is an integer field. A non-zero indicates the event should be ignored.</summary>
		ScrollWheelEventInstantMouser = 14,

		/// <summary>A value that represents the absolute X coordinate in table space at full tablet resolution. This is an integer field.</summary>
		TabletEventPointX = 15,

		/// <summary>A value that represents the absolute Y coordinate in table space at full tablet resolution. This is an integer field.</summary>
		TabletEventPointY = 16,

		/// <summary>A value that represents the absolute Z coordinate in table space at full tablet resolution. This is an integer field.</summary>
		TabletEventPointZ = 17,

		/// <summary>A value that represents the tablet button state. This is a bitfield represented by an integer field. Bit 0 is the first button, and a set bit represents a closed or pressed button. A maximum of 16 buttons are supported.</summary>
		TabletEventPointButtons = 18,

		/// <summary>A value that represents the tablet pen pressure. This is a double field, with values ranging from 0.0 (no pressure) and 1.0 (maximum pressure).</summary>
		TabletEventPointPressure = 19,

		/// <summary>A value that represents the horizontal tablet pen tilt. This is a double field, with values ranging from 0.0 (no tilt) and 1.0 (maximum tilt).</summary>
		TabletEventTiltX = 20,

		/// <summary>A value that represents the vertical tablet pen tilt. This is a double field, with values ranging from 0.0 (no tilt) and 1.0 (maximum tilt).</summary>
		TabletEventTiltY = 21,

		/// <summary>A value that represents the tablet pen rotation. This is a double field.</summary>
		TabletEventRotation = 22,

		/// <summary>A value that represents the tangential pressure on a device. This is a double field, with values ranging from 0.0 (no pressure) and 1.0 (maximum pressure).</summary>
		TabletEventTangentialPressure = 23,

		/// <summary>A value that represents the system-assigned unique device id. This is an integer field.</summary>
		TabletEventDeviceId = 24,

		/// <summary>This is a vendor-specific value. This is an integer field.</summary>
		TabletEventVendor1 = 25,

		/// <summary>This is a vendor-specific value. This is an integer field.</summary>
		TabletEventVendor2 = 26,

		/// <summary>This is a vendor-specific value. This is an integer field.</summary>
		TabletEventVendor3 = 27,

		/// <summary>A value that represents the vendor-defined id, typically the USB vendor id. This is an integer field.</summary>
		TabletProximityEventVendorId = 28,

		/// <summary>A value that represents the vendor-defined tabled id, typically the USB product id. This is an integer field.</summary>
		TabletProximityEventTabletId = 29,

		/// <summary>A value that represents the vendor-defined id the pointing device. This is an integer field.</summary>
		TabletProximityEventPointerId = 30,

		/// <summary>A system-assigned device id for the device. This is an integer field.</summary>
		TabletProximityEventDeviceId = 31,

		/// <summary>A system-assigned unique tablet id for the device. This is an integer field.</summary>
		TabletProximityEventSystemTabletId = 32,

		/// <summary>The vendor-assigned pointer type. This is an integer field.</summary>
		TabletProximityEventVendorPointerType = 33,

		/// <summary>The vendor-defined pointer serial number. This is an integer field.</summary>
		TabletProximityEventVendorPointerSerialNumber = 34,

		/// <summary>The vendor-defined unique id. This is an integer field.</summary>
		TabletProximityEventVendorUniqueId = 35,

		/// <summary>A value that represents the device capabilities mask. This is an integer field.</summary>
		TabletProximityEventCapabilityMask = 36,

		/// <summary>A value that represents the pointer type. This is an integer field.</summary>
		TabletProximityEventPointerType = 37,

		/// <summary>A value that indicates whether the pen is in proximity to the tablet. This is an integer field. The value is non-zero if the pen is in proximity to the tablet, and zero otherwise.</summary>
		TabletProximityEventEnterProximity = 38,

		/// <summary>The process serial number for the target process. This is an integer field.</summary>
		EventTargetProcessSerialNumber = 39,

		/// <summary>The process id (pid) of the target process. This is an integer field.</summary>
		EventTargetUnixProcessId = 40,

		/// <summary>The process id (pid) of the target process. This is an integer field.</summary>
		EventSourceUnixProcessId = 41,

		/// <summary>User-supplied data. This is an integer field.</summary>
		EventSourceUserData = 42,

		/// <summary>The Unix effective UID for the event source. This is an integer field.</summary>
		EventSourceUserId = 43,

		/// <summary>The Unix effective GID for the event source. This is an integer field.</summary>
		EventSourceGroupId = 44,

		/// <summary>The state id for the event source used to create this event. This is an integer field.</summary>
		EventSourceStateId = 45,

		/// <summary>A value that indicates whether a scrolling event contains continuous, pixel-based scrolling data. This is an integer field. The value is non-zero when the scrolling data is continuous and pixel-based, and zero otherwise (when the scrolling data is line-based).</summary>
		ScrollWheelEventIsContinuous = 88,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the event window under the mouse pointer.</summary>
		EventWindowUnderMousePointer = 91,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the event window under the mouse pointer that can handle this event.</summary>
		EventWindowUnderMousePointerThatCanHandleThisEvent = 92,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the scroll wheel's scroll phase.</summary>
		ScrollWheelEventScrollPhase = 99,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the scroll wheel's scroll count.</summary>
		ScrollWheelEventScrollCount = 100,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the scroll wheel's scroll momentum.</summary>
		ScrollWheelEventMomentumPhase = 123,

		// the header file doesn't say whether this is an integer or a double
		/// <summary>The unaccelerated pointer movement for the X axis.</summary>
		EventUnacceleratedPointerMovementX = 170,

		// the header file doesn't say whether this is an integer or a double
		/// <summary>The unaccelerated pointer movement for the Y axis.</summary>
		EventUnacceleratedPointerMovementY = 171,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the scroll wheel's scroll momentum.</summary>
		ScrollWheelEventMomentumOptionPhase = 173,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the accelerated delta value for axis 1.</summary>
		ScrollWheelEventAcceleratedDeltaAxis1 = 176,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the accelerated delta value for axis 2.</summary>
		ScrollWheelEventAcceleratedDeltaAxis2 = 175,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the raw delta value for axis 1.</summary>
		ScrollWheelEventRawDeltaAxis1 = 178,

		// no documentation for this value in the header file.
		/// <summary>A value that represents the raw delta value for axis 2.</summary>
		ScrollWheelEventRawDeltaAxis2 = 177,
	}

	// CGEventTypes.h:typedef uint32_t CGEventType;
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGEventType : uint {
		/// <summary>To be added.</summary>
		Null = 0x0,
		/// <summary>To be added.</summary>
		LeftMouseDown = 0x1,
		/// <summary>To be added.</summary>
		LeftMouseUp = 0x2,
		/// <summary>To be added.</summary>
		RightMouseDown = 0x3,
		/// <summary>To be added.</summary>
		RightMouseUp = 0x4,
		/// <summary>To be added.</summary>
		MouseMoved = 0x5,
		/// <summary>To be added.</summary>
		LeftMouseDragged = 0x6,
		/// <summary>To be added.</summary>
		RightMouseDragged = 0x7,
		/// <summary>To be added.</summary>
		KeyDown = 0xa,
		/// <summary>To be added.</summary>
		KeyUp = 0xb,
		/// <summary>To be added.</summary>
		FlagsChanged = 0xc,
		/// <summary>To be added.</summary>
		ScrollWheel = 0x16,
		/// <summary>To be added.</summary>
		TabletPointer = 0x17,
		/// <summary>To be added.</summary>
		TabletProximity = 0x18,
		/// <summary>To be added.</summary>
		OtherMouseDown = 0x19,
		/// <summary>To be added.</summary>
		OtherMouseUp = 0x1a,
		/// <summary>To be added.</summary>
		OtherMouseDragged = 0x1b,
		TapDisabledByTimeout = 4294967294,
		TapDisabledByUserInput = 4294967295,
	}

	// CGEventTypes.h:typedef uint32_t CGEventMouseSubtype;
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGEventMouseSubtype : uint {
		/// <summary>To be added.</summary>
		Default,
		/// <summary>To be added.</summary>
		TabletPoint,
		/// <summary>To be added.</summary>
		TabletProximity,
	}

	// CGEventTypes.h:typedef uint32_t CGEventSourceStateID;
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGEventSourceStateID : int {
		/// <summary>To be added.</summary>
		Private = -1,
		/// <summary>To be added.</summary>
		CombinedSession = 0,
		/// <summary>To be added.</summary>
		HidSystem = 1,
	}

	// CGRemoteOperation.h:typedef uint32_t CGEventFilterMask;
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[Flags]
	public enum CGEventFilterMask : uint {
		/// <summary>To be added.</summary>
		PermitLocalMouseEvents = 1,
		/// <summary>To be added.</summary>
		PermitLocalKeyboardEvents = 2,
		/// <summary>To be added.</summary>
		PermitSystemDefinedEvents = 4,
	}

	// CGRemoteOperation.h:typedef uint32_t CGEventSuppressionState;
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGEventSuppressionState : int {
		/// <summary>To be added.</summary>
		SuppressionInterval,
		/// <summary>To be added.</summary>
		RemoteMouseDrag,
		NumberOfEventSuppressionStates,
	}

}

#endif // MONOMAC
