//
// gamecontroller.cs: binding for iOS (7+) GameController framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//   TJ Lambert (t-anlamb@microsoft.com)
//
// Copyright 2013, 2015 Xamarin Inc.
// Copyright 2019 Microsoft Corporation

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
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // The GCControllerElement class is never instantiated directly.
	partial interface GCControllerElement {

		// NOTE: ArgumentSemantic.Weak if ARC, ArgumentSemantic.Assign otherwise;
		// currently MonoTouch is not ARC, neither is Xammac, so go with assign.
		[NullAllowed]
		[Export ("collection", ArgumentSemantic.Assign)]
		GCControllerElement Collection { get; }

		[Export ("analog")]
		bool IsAnalog { [Bind ("isAnalog")] get; }
	}

	delegate void GCControllerAxisValueChangedHandler (GCControllerAxisInput axis, float /* float, not CGFloat */ value);

	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCControllerAxisInput {

		[NullAllowed]
		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCControllerAxisValueChangedHandler ValueChangedHandler { get; set; }

		[Export ("value")]
		float Value {  /* float, not CGFloat */
			get;
			[Mac (10,15)][iOS (13,0)][TV (13,0)]
			set;
		}
	}

	delegate void GCControllerButtonValueChanged (GCControllerButtonInput button, float /* float, not CGFloat */ buttonValue, bool pressed);

	[iOS (7,0), Mac (10,9)]
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
		float Value {  /* float, not CGFloat */
			get;
			[Mac (10,15)][iOS (13,0)][TV (13,0)]
			set;
		}

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
	[Mac (10,9)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCControllerDirectionPad {

		[NullAllowed]
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

		[Mac (10,15), iOS (13,0)]
		[TV (13,0)]
		[Export ("setValueForXAxis:yAxis:")]
		void SetValue (float xAxis, float yAxis);
	}

	delegate void GCGamepadValueChangedHandler (GCGamepad gamepad, GCControllerElement element);

	[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'GCExtendedGamepad' instead.")]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'GCExtendedGamepad' instead.")]
	[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'GCExtendedGamepad' instead.")]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCGamepad {

		[NullAllowed]
		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

		[NullAllowed]
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

	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCExtendedGamepad' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCExtendedGamepad' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCExtendedGamepad' instead.")]
	[iOS (7,0)]
	[Mac (10,9)]
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
	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCExtendedGamepad {

		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

		[NullAllowed]
		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCExtendedGamepadValueChangedHandler ValueChangedHandler { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.Capture()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
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

		[TV (12, 1), Mac (10, 14, 1), iOS (12, 1)]
		[NullAllowed, Export ("leftThumbstickButton")]
		GCControllerButtonInput LeftThumbstickButton { get; }

		[TV (12, 1), Mac (10, 14, 1), iOS (12, 1)]
		[NullAllowed, Export ("rightThumbstickButton")]
		GCControllerButtonInput RightThumbstickButton { get; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Export ("buttonMenu")]
		GCControllerButtonInput ButtonMenu { get; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[NullAllowed, Export ("buttonOptions")]
		GCControllerButtonInput ButtonOptions { get; }

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("setStateFromExtendedGamepad:")]
		void SetState (GCExtendedGamepad extendedGamepad);
	}

	[iOS (7,0)]
	[Mac (10,9)]
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[BaseType (typeof (GCExtendedGamepad))]
	[DisableDefaultCtor]
	partial interface GCExtendedGamepadSnapshot {

		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		IntPtr Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		IntPtr Constructor (GCController controller, NSData data);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[TV (12, 2), Mac (10, 14, 4), iOS (12, 2)]
		[Field ("GCCurrentExtendedGamepadSnapshotDataVersion")]
		GCExtendedGamepadSnapshotDataVersion DataVersion { get; }
	}

	[iOS (7,0), Mac (10,9)]
	[BaseType (typeof (NSObject))]
	partial interface GCController {

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the Menu button found on the controller's profile, if it exists.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the Menu button found on the controller's profile, if it exists.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the Menu button found on the controller's profile, if it exists.")]

		[NullAllowed]
		[Export ("controllerPausedHandler", ArgumentSemantic.Copy)]
		Action<GCController> ControllerPausedHandler { get; set; }

		[NullAllowed]
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

		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[NullAllowed]
		[Export ("gamepad", ArgumentSemantic.Retain)]
		GCGamepad Gamepad { get; }

		[NullAllowed]
		[Export ("extendedGamepad", ArgumentSemantic.Retain)]
		GCExtendedGamepad ExtendedGamepad { get; }

		[Mac (10,12)]
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
		[NullAllowed]
		[Export ("motion", ArgumentSemantic.Retain)]
		GCMotion Motion { get; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("handlerQueue", ArgumentSemantic.Retain)]
		DispatchQueue HandlerQueue { get; set; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Export ("productCategory")]
		string ProductCategory { get; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Export ("snapshot")]
		bool Snapshot { [Bind ("isSnapshot")] get; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Export ("capture")]
		GCController Capture ();

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Static]
		[Export ("controllerWithMicroGamepad")]
		GCController GetMicroGamepadController ();

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Static]
		[Export ("controllerWithExtendedGamepad")]
		GCController GetExtendedGamepadController ();
	}

	[iOS (8,0), Mac (10,10)]
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

		[TV (11,0), iOS (11,0), Mac (10,13)]
		[Export ("rotationRate", ArgumentSemantic.Assign)]
		Vector3d RotationRate { get; }

		[TV (11,0)]
		[iOS (11,0)]
		[Mac (10,13)]
		[Export ("hasAttitudeAndRotationRate")]
		bool HasAttitudeAndRotationRate { get; }

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("setGravity:")]
		void SetGravity (GCAcceleration gravity);

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("setUserAcceleration:")]
		void SetUserAcceleration (GCAcceleration userAcceleration);

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("setAttitude:")]
		void SetAttitude (GCQuaternion attitude);

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("setRotationRate:")]
		void SetRotationRate (GCRotationRate rotationRate);

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("setStateFromMotion:")]
		void SetState (GCMotion motion);
	}

	[Mac (10,11)]
	[iOS (10,0)]
	[TV (9,0)]
	delegate void GCMicroGamepadValueChangedHandler (GCMicroGamepad gamepad, GCControllerElement element);

	[Mac (10,11)]
	[iOS (10,0)]
	[TV (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCMicroGamepad {
		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

		[NullAllowed, Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCMicroGamepadValueChangedHandler ValueChangedHandler { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.Capture()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
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

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Export ("buttonMenu")]
		GCControllerButtonInput ButtonMenu { get; }

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("setStateFromMicroGamepad:")]
		void SetState (GCMicroGamepad microGamepad);
	}

	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.Capture()' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
	[Mac (10,12)]
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

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCControler.GetMicroGamepadController()' instead.")]
		[TV (12, 2), Mac (10, 14, 4), iOS (12, 2)]
		[Field ("GCCurrentMicroGamepadSnapshotDataVersion")]
		GCMicroGamepadSnapshotDataVersion DataVersion { get; }
	}

	[Mac (10,12)]
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
