#if !__MACCATALYST__
using System;
using System.Diagnostics;

using Foundation;
using CoreGraphics;
using ObjCRuntime;

#nullable enable

namespace AppKit {

	[DebuggerTypeProxy (typeof (NSEvent.NSEventDebuggerProxy))]
	public partial class NSEvent {

		class NSEventDebuggerProxy {
			NSEvent target;

			public NSEventDebuggerProxy (NSEvent target)
			{
				this.target = target;
			}

			#region Generic Event Information

			// FIXME: broken
			internal NSGraphicsContext Context {
				get {
					return target.Context;
				}
			}

			public CGPoint LocationInWindow {
				get {
					return target.LocationInWindow;
				}
			}

			public NSEventModifierMask ModifierFlags {
				get {
					return target.ModifierFlags;
				}
			}

			public double Timestamp {
				get {
					return target.Timestamp;
				}
			}

			public NSEventType Type {
				get {
					return target.Type;
				}
			}

			public NSWindow Window {
				get {
					return target.Window;
				}
			}

			public nint WindowNumber {
				get {
					return target.WindowNumber;
				}
			}

			public IntPtr CGEvent {
				get {
					return target.CGEvent;
				}
			}

			#endregion

			#region Key Event Information

			bool IsKeyEvent ()
			{
				switch (target.Type) {
				case NSEventType.KeyDown:
				case NSEventType.KeyUp:
					return true;
				default:
					return false;
				}
			}

			void CheckKeyEvent ()
			{
				if (IsKeyEvent ())
					return;
				throw new InvalidOperationException ("Not a key event.");
			}

			public string Characters {
				get {
					CheckKeyEvent ();
					return target.Characters;
				}
			}

			public string CharactersIgnoringModifiers {
				get {
					CheckKeyEvent ();
					return target.CharactersIgnoringModifiers;
				}
			}

			public bool IsARepeat {
				get {
					CheckKeyEvent ();
					return target.IsARepeat;
				}
			}

			public ushort KeyCode {
				get {
					CheckKeyEvent ();
					return target.KeyCode;
				}
			}

			#endregion

			#region Tablet Pointing Information

			const int TabletPointEventSubtype = 1;
			const int TabletProximityEventSubtype = 2;

			bool IsMouseEvent ()
			{
				switch (target.Type) {
				case NSEventType.LeftMouseDown:
				case NSEventType.LeftMouseUp:
				case NSEventType.RightMouseDown:
				case NSEventType.RightMouseUp:
				case NSEventType.MouseMoved:
				case NSEventType.LeftMouseDragged:
				case NSEventType.RightMouseDragged:
				case NSEventType.MouseEntered:
				case NSEventType.MouseExited:
				case NSEventType.OtherMouseDown:
				case NSEventType.OtherMouseUp:
				case NSEventType.OtherMouseDragged:
					return true;
				default:
					return false;
				}
			}

			bool IsTabletPointingEvent ()
			{
				if (IsMouseEvent ())
					return target.Subtype == TabletPointEventSubtype;
				return target.Type == NSEventType.TabletPoint;
			}

			void CheckTabletPointingEvent ()
			{
				if (IsTabletPointingEvent ())
					return;
				throw new InvalidOperationException ("Not a tablet pointing event.");
			}

			public nint AbsoluteX {
				get {
					CheckTabletPointingEvent ();
					return target.AbsoluteX;
				}
			}

			public nint AbsoluteY {
				get {
					CheckTabletPointingEvent ();
					return target.AbsoluteY;
				}
			}

			public nint AbsoluteZ {
				get {
					CheckTabletPointingEvent ();
					return target.AbsoluteZ;
				}
			}

			public nuint ButtonMask {
				get {
					CheckTabletPointingEvent ();
					return target.ButtonMask;
				}
			}

			public float Rotation {
				get {
					CheckTabletPointingEvent ();
					return target.Rotation;
				}
			}

			public float TangentialPressure {
				get {
					CheckTabletPointingEvent ();
					return target.TangentialPressure;
				}
			}

			public CGPoint Tilt {
				get {
					CheckTabletPointingEvent ();
					return target.Tilt;
				}
			}

			public NSObject VendorDefined {
				get {
					CheckTabletPointingEvent ();
					return target.VendorDefined;
				}
			}

			#endregion

			#region Mouse Event Information

			void CheckMouseEvent ()
			{
				if (IsMouseEvent ())
					return;
				throw new InvalidOperationException ("Not a mouse event.");
			}

			public nint ButtonNumber {
				get {
					CheckMouseEvent ();
					return target.ButtonNumber;
				}
			}

			public nint ClickCount {
				get {
					CheckMouseEvent ();
					return target.ClickCount;
				}
			}

			public float Pressure {
				get {
					CheckMouseEvent ();
					return target.Pressure;
				}
			}

			#endregion

			#region Mouse Tracking Event Information

			bool IsMouseTrackingEvent ()
			{
				// FIXME
				return false;
			}

			void CheckMouseTrackingEvent ()
			{
				if (IsMouseTrackingEvent ())
					return;
				throw new InvalidOperationException ("Not a mouse tracking event.");
			}

			internal nint EventNumber {
				get {
					CheckMouseTrackingEvent ();
					return target.EventNumber;
				}
			}

			internal nint TrackingNumber {
				get {
					CheckMouseTrackingEvent ();
					return target.TrackingNumber;
				}
			}

			internal NSTrackingArea TrackingArea {
				get {
					CheckMouseTrackingEvent ();
					return target.TrackingArea;
				}
			}

			internal IntPtr UserData {
				get {
					CheckMouseTrackingEvent ();
					return target.UserData;
				}
			}

			#endregion

			#region Custom Event Information

			bool IsCustomEvent ()
			{
				switch (target.Type) {
				case NSEventType.AppKitDefined:
				case NSEventType.SystemDefined:
				case NSEventType.ApplicationDefined:
				case NSEventType.Periodic:
					return true;
				default:
					return false;
				}
			}

			void CheckCustomEvent ()
			{
				if (IsCustomEvent ())
					return;
				throw new InvalidOperationException ("Not a custom event.");
			}

			public short Subtype {
				get {
					CheckCustomEvent ();
					return target.Subtype;
				}
			}

			public nint Data1 {
				get {
					CheckCustomEvent ();
					return target.Data1;
				}
			}

			public nint Data2 {
				get {
					CheckCustomEvent ();
					return target.Data1;
				}
			}

			#endregion

			#region Scroll Wheel Event Information

			bool IsScrollWheelEvent ()
			{
				switch (target.Type) {
				case NSEventType.ScrollWheel:
					return true;
				default:
					return false;
				}
			}

			void CheckScrollWheelEvent ()
			{
				if (IsScrollWheelEvent ())
					return;
				throw new InvalidOperationException ("Not a scroll wheel event.");
			}

			public float DeltaX {
				get {
					CheckScrollWheelEvent ();
					return target.Data1;
				}
			}

			public float DeltaY {
				get {
					CheckScrollWheelEvent ();
					return target.Data1;
				}
			}

			public float DeltaZ {
				get {
					CheckScrollWheelEvent ();
					return target.Data1;
				}
			}

			#endregion

			#region Tablet Proximity Information

			bool IsTabletProximityEvent ()
			{
				if (IsMouseEvent ())
					return target.Subtype == TabletProximityEventSubtype;
				return target.Type == NSEventType.TabletProximity;
			}

			void CheckTabletProximityEvent ()
			{
				if (IsTabletProximityEvent ())
					return;
				throw new InvalidOperationException ("Not a tablet proximity event.");
			}

			public nuint CapabilityMask {
				get {
					CheckTabletProximityEvent ();
					return target.CapabilityMask;
				}
			}

			public nuint DeviceID {
				get {
					CheckTabletProximityEvent ();
					return target.DeviceID;
				}
			}

			public bool IsEnteringProximity {
				get {
					CheckTabletProximityEvent ();
					return target.IsEnteringProximity;
				}
			}

			public nuint PointingDeviceSerialNumber {
				get {
					CheckTabletProximityEvent ();
					return target.PointingDeviceSerialNumber;
				}
			}

			public nuint PointingDeviceID {
				get {
					CheckTabletProximityEvent ();
					return target.PointingDeviceID ();
				}
			}

			public NSPointingDeviceType PointingDeviceType {
				get {
					CheckTabletProximityEvent ();
					return target.PointingDeviceType;
				}
			}

			public nuint SystemTabletID {
				get {
					CheckTabletProximityEvent ();
					return target.SystemTabletID;
				}
			}

			public nuint TabletID {
				get {
					CheckTabletProximityEvent ();
					return target.TabletID;
				}
			}

			public long UniqueID {
				get {
					CheckTabletProximityEvent ();
					return target.UniqueID;
				}
			}

			public nuint VendorID {
				get {
					CheckTabletProximityEvent ();
					return target.VendorID;
				}
			}

			public nuint VendorPointingDeviceType {
				get {
					CheckTabletProximityEvent ();
					return target.VendorPointingDeviceType;
				}
			}

			#endregion

			#region Touch and Gesture Events

			bool IsTouchOrGestureEvent ()
			{
				// FIXME
				return false;
			}

			void CheckTouchOrGestureEvent ()
			{
				if (IsTouchOrGestureEvent ())
					return;
				throw new InvalidOperationException ("Not a touch or gesture event.");
			}

			internal nfloat Magnification {
				get {
					CheckTouchOrGestureEvent ();
					return target.Magnification;
				}
			}

			#endregion

			#region Scroll Wheel and Flick Events

			bool IsScrollWheelOrFlickEvent ()
			{
				if (IsScrollWheelEvent ())
					return true;
				return false;
			}

			void CheckScrollWheelOrFlickEvent ()
			{
				if (IsScrollWheelOrFlickEvent ())
					return;
				throw new InvalidOperationException ("Not a scroll wheel or flick event.");
			}

			public bool HasPreciseScrollingDeltas {
				get {
					CheckScrollWheelOrFlickEvent ();
					return target.HasPreciseScrollingDeltas;
				}
			}

			public nfloat ScrollingDeltaX {
				get {
					CheckScrollWheelOrFlickEvent ();
					return target.ScrollingDeltaX;
				}
			}

			public nfloat ScrollingDeltaY {
				get {
					CheckScrollWheelOrFlickEvent ();
					return target.ScrollingDeltaY;
				}
			}

			public NSEventPhase MomentumPhase {
				get {
					CheckScrollWheelOrFlickEvent ();
					return target.MomentumPhase;
				}
			}

			public NSEventPhase Phase {
				get {
					CheckScrollWheelOrFlickEvent ();
					return target.Phase;
				}
			}

			public bool IsDirectionInvertedFromDevice {
				get {
					CheckScrollWheelOrFlickEvent ();
					return target.IsDirectionInvertedFromDevice;
				}
			}
			#endregion
		}

	}
}
#endif // !__MACCATALYST__
