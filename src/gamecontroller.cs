//
// gamecontroller.cs: binding for iOS (7+) GameController framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013, 2015 Xamarin Inc.

using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;
using OpenTK;
#if MONOMAC
using AppKit;
using UIViewController = AppKit.NSViewController;
#else
using UIKit;
#endif

namespace GameController {

	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // The GCControllerElement class is never instantiated directly.
	partial interface GCControllerElement {

		// NOTE: ArgumentSemantic.Weak if ARC, ArgumentSemantic.Assign otherwise;
		// currently MonoTouch is not ARC, neither is Xammac, so go with assign.
		[Export ("collection", ArgumentSemantic.Assign)]
		GCControllerElement Collection { get; }

		[Export ("analog")]
		bool IsAnalog { [Bind ("isAnalog")] get; }
	}

	delegate void GCControllerAxisValueChangedHandler (GCControllerAxisInput axis, float /* float, not CGFloat */ value);

	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCControllerAxisInput {

		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCControllerAxisValueChangedHandler ValueChangedHandler { get; set; }

		[Export ("value")]
		float Value { get; } /* float, not CGFloat */
	}

	delegate void GCControllerButtonValueChanged (GCControllerButtonInput button, float /* float, not CGFloat */ buttonValue, bool pressed);

	[iOS (7,0), Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCControllerButtonInput {

#if !XAMCORE_4_0
		[Obsolete ("Use the 'ValueChangedHandler' property.")]
		[Wrap ("ValueChangedHandler = handler;", IsVirtual = true)]
		void SetValueChangedHandler (GCControllerButtonValueChanged handler);
#endif

		[NullAllowed]
		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCControllerButtonValueChanged ValueChangedHandler { get; set; }

		[Export ("value")]
		float Value { get; } /* float, not CGFloat */

		[Export ("pressed")]
		bool IsPressed { [Bind ("isPressed")] get; }

#if !XAMCORE_4_0
		[iOS (8,0), Mac (10,10)]
		[Obsolete ("Use the 'PressedChangedHandler' property.")]
		[Wrap ("PressedChangedHandler = handler;", IsVirtual = true)]
		void SetPressedChangedHandler (GCControllerButtonValueChanged handler);
#endif

		[iOS (8,0), Mac (10,10)]
		[NullAllowed]
		[Export ("pressedChangedHandler", ArgumentSemantic.Copy)]
		GCControllerButtonValueChanged PressedChangedHandler { get; set; }
	}

	delegate void GCControllerDirectionPadValueChangedHandler (GCControllerDirectionPad dpad, float /* float, not CGFloat */ xValue, float /* float, not CGFloat */ yValue);

	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCControllerDirectionPad {

		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCControllerDirectionPadValueChangedHandler ValueChangedHandler { get; set; }

		[Export ("xAxis")]
		GCControllerAxisInput XAxis { get; }

		[Export ("yAxis")]
		GCControllerAxisInput YAxis { get; }

		[Export ("up")]
		GCControllerButtonInput Up { get; }

		[Export ("down")]
		GCControllerButtonInput Down { get; }

		[Export ("left")]
		GCControllerButtonInput Left { get; }

		[Export ("right")]
		GCControllerButtonInput Right { get; }
	}

	delegate void GCGamepadValueChangedHandler (GCGamepad gamepad, GCControllerElement element);

	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCGamepad {

		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCGamepadValueChangedHandler ValueChangedHandler { get; set; }

		[Export ("saveSnapshot")]
		GCGamepadSnapshot SaveSnapshot { get; }

		[Export ("dpad")]
		GCControllerDirectionPad DPad { get; }

		[Export ("buttonA")]
		GCControllerButtonInput ButtonA { get; }

		[Export ("buttonB")]
		GCControllerButtonInput ButtonB { get; }

		[Export ("buttonX")]
		GCControllerButtonInput ButtonX { get; }

		[Export ("buttonY")]
		GCControllerButtonInput ButtonY { get; }

		[Export ("leftShoulder")]
		GCControllerButtonInput LeftShoulder { get; }

		[Export ("rightShoulder")]
		GCControllerButtonInput RightShoulder { get; }
	}

	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCGamepad))]
	[DisableDefaultCtor]
	partial interface GCGamepadSnapshot {

		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		IntPtr Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		IntPtr Constructor (GCController controller, NSData data);
	}

	delegate void GCExtendedGamepadValueChangedHandler (GCExtendedGamepad gamepad, GCControllerElement element);

	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCExtendedGamepad {

		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCExtendedGamepadValueChangedHandler ValueChangedHandler { get; set; }

		[Export ("saveSnapshot")]
		GCExtendedGamepadSnapshot SaveSnapshot ();

		[Export ("dpad")]
		GCControllerDirectionPad DPad { get; }

		[Export ("buttonA")]
		GCControllerButtonInput ButtonA { get; }

		[Export ("buttonB")]
		GCControllerButtonInput ButtonB { get; }

		[Export ("buttonX")]
		GCControllerButtonInput ButtonX { get; }

		[Export ("buttonY")]
		GCControllerButtonInput ButtonY { get; }

		[Export ("leftThumbstick")]
		GCControllerDirectionPad LeftThumbstick { get; }

		[Export ("rightThumbstick")]
		GCControllerDirectionPad RightThumbstick { get; }

		[Export ("leftShoulder")]
		GCControllerButtonInput LeftShoulder { get; }

		[Export ("rightShoulder")]
		GCControllerButtonInput RightShoulder { get; }

		[Export ("leftTrigger")]
		GCControllerButtonInput LeftTrigger { get; }

		[Export ("rightTrigger")]
		GCControllerButtonInput RightTrigger { get; }
	}

	[iOS (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCExtendedGamepad))]
	[DisableDefaultCtor]
	partial interface GCExtendedGamepadSnapshot {

		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		IntPtr Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		IntPtr Constructor (GCController controller, NSData data);
	}

#if !XAMCORE_2_0
	delegate void GCControllerPausedHandler (GCController controller);
#endif

	[iOS (7,0), Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	partial interface GCController {

		[Export ("controllerPausedHandler", ArgumentSemantic.Copy)]
#if XAMCORE_2_0
		Action<GCController> ControllerPausedHandler { get; set; }
#else
		GCControllerPausedHandler ControllerPausedHandler { get; set; }
#endif

		[Export ("vendorName", ArgumentSemantic.Copy)]
		string VendorName { get; }

		[Export ("attachedToDevice")]
		bool AttachedToDevice { [Bind ("isAttachedToDevice")] get; }

		[Export ("playerIndex")]
#if XAMCORE_4_0
		// enum only added in iOS9 / OSX 10.11 - but with compatible values
		GCControllerPlayerIndex PlayerIndex { get; set; }
#else
		nint PlayerIndex { get; set; }
#endif

		[Export ("gamepad", ArgumentSemantic.Retain)]
		GCGamepad Gamepad { get; }

		[Export ("extendedGamepad", ArgumentSemantic.Retain)]
		GCExtendedGamepad ExtendedGamepad { get; }

		[Mac (10,12, onlyOn64: true)]
		[iOS (10,0)]
		[NullAllowed, Export ("microGamepad", ArgumentSemantic.Retain)]
		GCMicroGamepad MicroGamepad { get; }

		[Static, Export ("controllers")]
		GCController [] Controllers { get; }

		[Static, Export ("startWirelessControllerDiscoveryWithCompletionHandler:")]
		[Async]
		void StartWirelessControllerDiscovery ([NullAllowed] Action completionHandler);

		[Static, Export ("stopWirelessControllerDiscovery")]
		void StopWirelessControllerDiscovery ();

		[Notification, Field ("GCControllerDidConnectNotification")]
		NSString DidConnectNotification { get; }

		[Notification, Field ("GCControllerDidDisconnectNotification")]
		NSString DidDisconnectNotification { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("motion", ArgumentSemantic.Retain)]
		GCMotion Motion { get; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("handlerQueue", ArgumentSemantic.Retain)]
		DispatchQueue HandlerQueue { get; set; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // access thru GCController.Motion - returns a nil Handle
	partial interface GCMotion {

		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

#if !XAMCORE_4_0
		[Obsolete ("Use the 'ValueChangedHandler' property.")]
		[Wrap ("ValueChangedHandler = handler;", IsVirtual = true)]
		void SetValueChangedHandler (Action<GCMotion> handler);
#endif

		[NullAllowed]
		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		Action<GCMotion> ValueChangedHandler { get; set; }

		[Export ("gravity", ArgumentSemantic.Assign)]
		Vector3d Gravity { get; }

		[Export ("userAcceleration", ArgumentSemantic.Assign)]
		Vector3d UserAcceleration { get; }

		[TV (11,0)]
		[Export ("attitude", ArgumentSemantic.Assign)]
		Quaterniond Attitude { get; }

		[TV (11,0)]
		[Export ("rotationRate", ArgumentSemantic.Assign)]
		Vector3d RotationRate { get; }

		[TV (11,0)]
		[iOS (11,0)]
		[Mac (10,13, onlyOn64: true)]
		[Export ("hasAttitudeAndRotationRate")]
		bool HasAttitudeAndRotationRate { get; }
	}

	[Mac (10,11, onlyOn64: true)]
	[iOS (10,0)]
	[TV (9,0)]
	delegate void GCMicroGamepadValueChangedHandler (GCMicroGamepad gamepad, GCControllerElement element);

	[Mac (10,11, onlyOn64: true)]
	[iOS (10,0)]
	[TV (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCMicroGamepad {
		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

		[NullAllowed, Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCMicroGamepadValueChangedHandler ValueChangedHandler { get; set; }

		[Export ("saveSnapshot")]
		GCMicroGamepadSnapshot SaveSnapshot { get; }

		[Export ("dpad", ArgumentSemantic.Retain)]
		GCControllerDirectionPad Dpad { get; }

		[Export ("buttonA", ArgumentSemantic.Retain)]
		GCControllerButtonInput ButtonA { get; }

		[Export ("buttonX", ArgumentSemantic.Retain)]
		GCControllerButtonInput ButtonX { get; }

		[Export ("reportsAbsoluteDpadValues")]
		bool ReportsAbsoluteDpadValues { get; set; }

		[Export ("allowsRotation")]
		bool AllowsRotation { get; set; }
	}

	[Mac (10,12, onlyOn64: true)]
	[iOS (10,0)]
	[TV (9,0)]
	[BaseType (typeof (GCMicroGamepad))]
	interface GCMicroGamepadSnapshot {
		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		IntPtr Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		IntPtr Constructor (GCController controller, NSData data);
	}

	[Mac (10,12, onlyOn64: true)]
	[iOS (10,0)]
	[TV (9,0)]
	[BaseType (typeof (UIViewController))]
	interface GCEventViewController {

		// inlined ctor
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("controllerUserInteractionEnabled")]
		bool ControllerUserInteractionEnabled { get; set; }
	}
}
