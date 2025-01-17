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

using System;
using System.Runtime.InteropServices;
#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif
using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace CoreGraphics {

	// CGEventTypes.h:typedef uint32_t CGEventTapLocation;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGEventTapLocation : int {
		HID,
		Session,
		AnnotatedSession,
	}

	// CGEventTypes.h:typedef uint32_t CGEventTapPlacement;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGEventTapPlacement : uint {
		HeadInsert,
		TailAppend,
	}

	// CGEventTypes.h:typedef uint32_t CGEventTapOptions;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGEventTapOptions : uint {
		Default,
		ListenOnly,
	}

	// CGEventTypes.h:typedef uint32_t CGMouseButton;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGMouseButton : uint {
		Left,
		Right,
		Center,
	}

	// CGEventTypes.h:typedef uint32_t CGScrollEventUnit;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGScrollEventUnit : uint {
		Pixel,
		Line,
	}

	// CGEventTypes.h:typedef uint64_t CGEventMask;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	[Flags]
	public enum CGEventMask : ulong {
		Null = 0x00000001,
		LeftMouseDown = 0x00000002,
		LeftMouseUp = 0x00000004,
		RightMouseDown = 0x00000008,
		RightMouseUp = 0x00000010,
		MouseMoved = 0x00000020,
		LeftMouseDragged = 0x00000040,
		RightMouseDragged = 0x00000080,
		KeyDown = 0x00000400,
		KeyUp = 0x00000800,
		FlagsChanged = 0x00001000,
		ScrollWheel = 0x00400000,
		TabletPointer = 0x00800000,
		TabletProximity = 0x01000000,
		OtherMouseDown = 0x02000000,
		OtherMouseUp = 0x04000000,
		OtherMouseDragged = 0x08000000,
	}

	// CGEventTypes.h:typedef uint64_t CGEventFlags;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	[Flags]
	public enum CGEventFlags : ulong {
		NonCoalesced = 0x00000100,
		AlphaShift = 0x00010000,
		Shift = 0x00020000,
		Control = 0x00040000,
		Alternate = 0x00080000,
		Command = 0x00100000,
		NumericPad = 0x00200000,
		Help = 0x00400000,
		SecondaryFn = 0x00800000,
	}

	// CGEventTypes.h:typedef uint32_t CGEventField;
	/// <summary>This enum represents constants used to access specialized fields in low-level events.</summary>
	/// <remarks>Call <see cref="CGEvent.GetLongValueField" /> or <see cref="CGEvent.GetDoubleValueField" /> to fetch the field values, and <see cref="CGEvent.SetValueField(CGEventField,long)" /> or <see cref="CGEvent.SetValueField(CGEventField,double)" /> to set the field values represented by the constants in this enum.</remarks>
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGEventField : int {
		/// <summary>The mouse button event number.</summary>
		/// <remarks>This is an integer field. Matching mouse-down and mouse-up events will have the same event number.</remarks>
		MouseEventNumber = 0,

		/// <summary>The mouse button click state.</summary>
		/// <remarks>This is an integer field. A value of 1 is a single click, a value of 2 is a double click, and so on.</remarks>
		MouseEventClickState = 1,

		/// <summary>The mouse button pressure state, ranging from 0 (mouse being up) to 1.</summary>
		/// <remarks>This is an integer field.</remarks>
		MouseEventPressure = 2,

		/// <summary>The mouse button number.</summary>
		/// <remarks>This is an integer field.</remarks>
		MouseEventButtonNumber = 3,

		/// <summary>The horizontal delta since the last mouse movement event.</summary>
		/// <remarks>This is an integer field.</remarks>
		MouseEventDeltaX = 4,

		/// <summary>The vertical delta since the last mouse movement event.</summary>
		/// <remarks>This is an integer field.</remarks>
		MouseEventDeltaY = 5,

		/// <summary>A value indicating whether the event should be ignored by the Inkwell subsystem.</summary>
		/// <remarks>This is an integer field. A non-zero indicates the event should be ignored.</remarks>
		MouseEventInstantMouser = 6,

		/// <summary>The mouse event subtype.</summary>
		/// <remarks>This is an integer field.</remarks>
		MouseEventSubtype = 7,

		/// <summary>A value indicating whether the key-down event is an autorepeat of a previous key-down event.</summary>
		/// <remarks>This is an integer field. Zero indicates not an autorepeat, non-zero not an autorepeat.</remarks>
		KeyboardEventAutorepeat = 8,

		/// <summary>The virtual keycode of the key-down or key-up event.</summary>
		/// <remarks>This is an integer field.</remarks>
		KeyboardEventKeycode = 9,

		/// <summary>The keyboard ype identifier</summary>
		/// <remarks>This is an integer field.</remarks>
		KeyboardEventKeyboardType = 10,

		/// <summary>A value that represents scrolling data. Typically this is the vertical position change since the last scrolling event.</summary>
		/// <remarks>This is an integer field.</remarks>
		ScrollWheelEventDeltaAxis1 = 11,

		/// <summary>A value that represents scrolling data. Typically this is the horizontal position change since the last scrolling event.</summary>
		/// <remarks>This is an integer field.</remarks>
		ScrollWheelEventDeltaAxis2 = 12,

		/// <summary>This value is unused.</summary>
		ScrollWheelEventDeltaAxis3 = 13,

		/// <summary>A value that represents scrolling data. Typically this is the line-based or pixel-based vertical position change since the last scrolling event.</summary>
		/// <remarks>This field is a fixed-point 16.16 signed integer, but can also be retrieved as a double and will be automatically converted.</remarks>
		ScrollWheelEventFixedPtDeltaAxis1 = 93,

		/// <summary>A value that represents scrolling data. Typically this is the line-based or pixel-based horizontal position change since the last scrolling event.</summary>
		/// <remarks>This field is a fixed-point 16.16 signed integer, but can also be retrieved as a double and will be automatically converted.</remarks>
		ScrollWheelEventFixedPtDeltaAxis2 = 94,

		/// <summary>This value is unused.</summary>
		ScrollWheelEventFixedPtDeltaAxis3 = 95,

		/// <summary>A value that represents pixel-based scrolling data. Typically this is the vertical position change since the last scrolling event.</summary>
		/// <remarks>This is an integer field.</remarks>
		ScrollWheelEventPointDeltaAxis1 = 96,

		/// <summary>A value that represents pixel-based scrolling data. Typically this is the horizontal position change since the last scrolling event.</summary>
		/// <remarks>This is an integer field.</remarks>
		ScrollWheelEventPointDeltaAxis2 = 97,

		/// <summary>This value is unused.</summary>
		ScrollWheelEventPointDeltaAxis3 = 98,

		/// <summary>A value indicating whether the event should be ignored by the Inkwell subsystem.</summary>
		/// <remarks>This is an integer field. A non-zero indicates the event should be ignored.</remarks>
		ScrollWheelEventInstantMouser = 14,

		/// <summary>A value that represents the absolute X coordinate in table space at full tablet resolution.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletEventPointX = 15,

		/// <summary>A value that represents the absolute Y coordinate in table space at full tablet resolution.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletEventPointY = 16,

		/// <summary>A value that represents the absolute Z coordinate in table space at full tablet resolution.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletEventPointZ = 17,

		/// <summary>A value that represents the tablet button state.</summary>
		/// <remarks>This is a bitfield represented by an integer field. Bit 0 is the first button, and a set bit represents a closed or pressed button. A maximum of 16 buttons are supported.</remarks>
		TabletEventPointButtons = 18,

		/// <summary>A value that represents the tablet pen pressure.</summary>
		/// <remarks>This is a double field, with values ranging from 0.0 (no pressure) and 1.0 (maximum pressure).</remarks>
		TabletEventPointPressure = 19,

		/// <summary>A value that represents the horizontal tablet pen tilt.</summary>
		/// <remarks>This is a double field, with values ranging from 0.0 (no tilt) and 1.0 (maximum tilt).</remarks>
		TabletEventTiltX = 20,

		/// <summary>A value that represents the vertical tablet pen tilt.</summary>
		/// <remarks>This is a double field, with values ranging from 0.0 (no tilt) and 1.0 (maximum tilt).</remarks>
		TabletEventTiltY = 21,

		/// <summary>A value that represents the tablet pen rotation.</summary>
		/// <remarks>This is a double field.</remarks>
		TabletEventRotation = 22,

		/// <summary>A value that represents the tangential pressure on a device.</summary>
		/// <remarks>This is a double field, with values ranging from 0.0 (no pressure) and 1.0 (maximum pressure).</remarks>
		TabletEventTangentialPressure = 23,

		/// <summary>A value that represents the system-assigned unique device id.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletEventDeviceId = 24,

		/// <summary>This is a vendor-specific value.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletEventVendor1 = 25,

		/// <summary>This is a vendor-specific value.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletEventVendor2 = 26,

		/// <summary>This is a vendor-specific value.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletEventVendor3 = 27,

		/// <summary>A value that represents the vendor-defined id, typically the USB vendor id.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventVendorId = 28,

		/// <summary>A value that represents the vendor-defined tabled id, typically the USB product id.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventTabletId = 29,

		/// <summary>A value that represents the vendor-defined id the pointing device.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventPointerId = 30,

		/// <summary>A system-assigned device id for the device.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventDeviceId = 31,

		/// <summary>A system-assigned unique tablet id for the device.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventSystemTabletId = 32,

		/// <summary>The vendor-assigned pointer type.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventVendorPointerType = 33,

		/// <summary>The vendor-defined pointer serial number.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventVendorPointerSerialNumber = 34,

		/// <summary>The vendor-defined unique id.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventVendorUniqueId = 35,

		/// <summary>A value that represents the device capabilities mask.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventCapabilityMask = 36,

		/// <summary>A value that represents the pointer type.</summary>
		/// <remarks>This is an integer field.</remarks>
		TabletProximityEventPointerType = 37,

		/// <summary>A value that indicates whether the pen is in proximity to the tablet.</summary>
		/// <remarks>This is an integer field. The value is non-zero if the pen is in proximity to the tablet, and zero otherwise.</remarks>
		TabletProximityEventEnterProximity = 38,

		/// <summary>The process serial number for the target process.</summary>
		/// <remarks>This is an integer field.</remarks>
		EventTargetProcessSerialNumber = 39,

		/// <summary>The process id (pid) of the target process.</summary>
		/// <remarks>This is an integer field.</remarks>
		EventTargetUnixProcessId = 40,

		/// <summary>The process id (pid) of the target process.</summary>
		/// <remarks>This is an integer field.</remarks>
		EventSourceUnixProcessId = 41,

		/// <summary>User-supplied data.</summary>
		/// <remarks>This is an integer field.</remarks>
		EventSourceUserData = 42,

		/// <summary>The Unix effective UID for the event source.</summary>
		/// <remarks>This is an integer field.</remarks>
		EventSourceUserId = 43,

		/// <summary>The Unix effective GID for the event source.</summary>
		/// <remarks>This is an integer field.</remarks>
		EventSourceGroupId = 44,

		/// <summary>The state id for the event source used to create this event.</summary>
		/// <remarks>This is an integer field.</remarks>
		EventSourceStateId = 45,

		/// <summary>A value that indicates whether a scrolling event contains continuous, pixel-based scrolling data.</summary>
		/// <remarks>This is an integer field. The value is non-zero when the scrolling data is continuous and pixel-based, and zero otherwise (when the scrolling data is line-based).</remarks>
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
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGEventType : uint {
		Null = 0x0,
		LeftMouseDown = 0x1,
		LeftMouseUp = 0x2,
		RightMouseDown = 0x3,
		RightMouseUp = 0x4,
		MouseMoved = 0x5,
		LeftMouseDragged = 0x6,
		RightMouseDragged = 0x7,
		KeyDown = 0xa,
		KeyUp = 0xb,
		FlagsChanged = 0xc,
		ScrollWheel = 0x16,
		TabletPointer = 0x17,
		TabletProximity = 0x18,
		OtherMouseDown = 0x19,
		OtherMouseUp = 0x1a,
		OtherMouseDragged = 0x1b,
		TapDisabledByTimeout = 4294967294,
		TapDisabledByUserInput = 4294967295,
	}

	// CGEventTypes.h:typedef uint32_t CGEventMouseSubtype;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGEventMouseSubtype : uint {
		Default,
		TabletPoint,
		TabletProximity,
	}

	// CGEventTypes.h:typedef uint32_t CGEventSourceStateID;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGEventSourceStateID : int {
		Private = -1,
		CombinedSession = 0,
		HidSystem = 1,
	}

	// CGRemoteOperation.h:typedef uint32_t CGEventFilterMask;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	[Flags]
	public enum CGEventFilterMask : uint {
		PermitLocalMouseEvents = 1,
		PermitLocalKeyboardEvents = 2,
		PermitSystemDefinedEvents = 4,
	}

	// CGRemoteOperation.h:typedef uint32_t CGEventSuppressionState;
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	public enum CGEventSuppressionState : int {
		SuppressionInterval,
		RemoteMouseDrag,
		NumberOfEventSuppressionStates,
	}

}

#endif // MONOMAC
