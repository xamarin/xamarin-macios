/*
 * CGEvenTypes.cs: bindings to the ApplicationServices framework's CoreGraphics APIs
 * 
 * Copyright 2013, 2014 Xamarin Inc
 * All Rights Reserved
 * 
 * Authors:
 *    Miguel de Icaza
 */

#if MONOMAC

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
	public enum CGEventTapLocation : int {
		HID,
		Session,
		AnnotatedSession
	}

	// CGEventTypes.h:typedef uint32_t CGEventTapPlacement;
	public enum CGEventTapPlacement : uint {
		HeadInsert,
		TailAppend
	}

	// CGEventTypes.h:typedef uint32_t CGEventTapOptions;
	public enum CGEventTapOptions : uint {
		Default, 
		ListenOnly
	}

	// CGEventTypes.h:typedef uint32_t CGMouseButton;
	public enum CGMouseButton : uint {
		Left, Right, Center
	}

	// CGEventTypes.h:typedef uint32_t CGScrollEventUnit;
	public enum CGScrollEventUnit : uint {
		Pixel, Line
	}

	// CGEventTypes.h:typedef uint64_t CGEventMask;
	[Flags]
	public enum CGEventMask : ulong {
		Null              = 0x00000001,
		LeftMouseDown     = 0x00000002,
		LeftMouseUp       = 0x00000004,
		RightMouseDown    = 0x00000008,
		RightMouseUp      = 0x00000010,
		MouseMoved        = 0x00000020,
		LeftMouseDragged  = 0x00000040,
		RightMouseDragged = 0x00000080,
		KeyDown           = 0x00000400,
		KeyUp             = 0x00000800,
		FlagsChanged      = 0x00001000,
		ScrollWheel       = 0x00400000,
		TabletPointer     = 0x00800000,
		TabletProximity   = 0x01000000,
		OtherMouseDown    = 0x02000000,
		OtherMouseUp      = 0x04000000,
		OtherMouseDragged = 0x08000000,
	}

	// CGEventTypes.h:typedef uint64_t CGEventFlags;
	[Flags]
	public enum CGEventFlags : ulong {
		NonCoalesced = 0x00000100,
		AlphaShift   = 0x00010000,
		Shift        = 0x00020000,
		Control      = 0x00040000,
		Alternate    = 0x00080000,
		Command      = 0x00100000,
		NumericPad   = 0x00200000,
		Help         = 0x00400000,
		SecondaryFn  = 0x00800000,
	}

	// CGEventTypes.h:typedef uint32_t CGEventField;
	internal enum CGEventField : int {
		MouseEventNumber = 0,
		MouseEventClickState = 1,
		MouseEventPressure = 2,
		MouseEventButtonNumber = 3,
		MouseEventDeltaX = 4,
		MouseEventDeltaY = 5,
		MouseEventInstantMouser = 6,
		MouseEventSubtype = 7,
		KeyboardEventAutorepeat = 8,
		KeyboardEventKeycode = 9,
		KeyboardEventKeyboardType = 10,
		ScrollWheelEventDeltaAxis1 = 11,
		ScrollWheelEventDeltaAxis2 = 12,
		ScrollWheelEventDeltaAxis3 = 13,
		ScrollWheelEventFixedPtDeltaAxis1 = 93,
		ScrollWheelEventFixedPtDeltaAxis2 = 94,
		ScrollWheelEventFixedPtDeltaAxis3 = 95,
		ScrollWheelEventPointDeltaAxis1 = 96,
		ScrollWheelEventPointDeltaAxis2 = 97,
		ScrollWheelEventPointDeltaAxis3 = 98,
		ScrollWheelEventInstantMouser = 14,
		TabletEventPointX = 15,
		TabletEventPointY = 16,
		TabletEventPointZ = 17,
		TabletEventPointButtons = 18,
		TabletEventPointPressure = 19,
		TabletEventTiltX = 20,
		TabletEventTiltY = 21,
		TabletEventRotation = 22,
		TabletEventTangentialPressure = 23,
		TabletEventDeviceID = 24,
		TabletEventVendor1 = 25,
		TabletEventVendor2 = 26,
		TabletEventVendor3 = 27,
		TabletProximityEventVendorID = 28,
		TabletProximityEventTabletID = 29,
		TabletProximityEventPointerID = 30,
		TabletProximityEventDeviceID = 31,
		TabletProximityEventSystemTabletID = 32,
		TabletProximityEventVendorPointerType = 33,
		TabletProximityEventVendorPointerSerialNumber = 34,
		TabletProximityEventVendorUniqueID = 35,
		TabletProximityEventCapabilityMask = 36,
		TabletProximityEventPointerType = 37,
		TabletProximityEventEnterProximity = 38,
		EventTargetProcessSerialNumber = 39,
		EventTargetUnixProcessID = 40,
		EventSourceUnixProcessID = 41,
		EventSourceUserData = 42,
		EventSourceUserID = 43,
		EventSourceGroupID = 44,
		EventSourceStateID = 45,
		ScrollWheelEventIsContinuous = 88
	}

	// CGEventTypes.h:typedef uint32_t CGEventType;
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
	}

	// CGEventTypes.h:typedef uint32_t CGEventMouseSubtype;
	public enum CGEventMouseSubtype : uint {
		Default, TabletPoint, TabletProximity
	}

	// CGEventTypes.h:typedef uint32_t CGEventSourceStateID;
	public enum CGEventSourceStateID : int {
		Private = -1, CombinedSession = 0, HidSystem = 1
	}

	// CGRemoteOperation.h:typedef uint32_t CGEventFilterMask;
	[Flags]
	public enum CGEventFilterMask : uint {
		PermitLocalMouseEvents = 1,
		PermitLocalKeyboardEvents = 2,
		PermitSystemDefinedEvents = 4
	}

	// CGRemoteOperation.h:typedef uint32_t CGEventSuppressionState;
	public enum CGEventSuppressionState : int {
		SuppressionInterval,
		RemoteMouseDrag,
	}
	
}

#endif // MONOMAC
