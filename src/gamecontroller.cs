//
// gamecontroller.cs: binding for iOS (7+) GameController framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//   TJ Lambert (antlambe@microsoft.com)
//   Whitney Schmidt (whschm@microsoft.com)
//
// Copyright 2013, 2015 Xamarin Inc.
// Copyright 2019, 2020 Microsoft Corporation

using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;
#if !NET
using OpenTK;
#endif
#if MONOMAC
using AppKit;
using UIViewController = AppKit.NSViewController;
using CHHapticEngine = Foundation.NSObject;
using BezierPath = AppKit.NSBezierPath;
#else
using CoreHaptics;
using UIKit;
using BezierPath = UIKit.UIBezierPath;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace GameController {

	[MacCatalyst (13, 1)]
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

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("sfSymbolsName", ArgumentSemantic.Strong)]
		string SfSymbolsName { get; set; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("localizedName", ArgumentSemantic.Strong)]
		string LocalizedName { get; set; }

		[TV (14, 2), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("unmappedSfSymbolsName", ArgumentSemantic.Strong)]
		string UnmappedSfSymbolsName { get; set; }

		[TV (14, 2), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("unmappedLocalizedName", ArgumentSemantic.Strong)]
		string UnmappedLocalizedName { get; set; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("aliases")]
		NSSet<NSString> Aliases { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("boundToSystemGesture")]
		bool IsBoundToSystemGesture { [Bind ("isBoundToSystemGesture")] get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("preferredSystemGestureState", ArgumentSemantic.Assign)]
		GCSystemGestureState PreferredSystemGestureState { get; set; }
	}

	delegate void GCControllerAxisValueChangedHandler (GCControllerAxisInput axis, float /* float, not CGFloat */ value);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCControllerAxisInput {

		[NullAllowed]
		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCControllerAxisValueChangedHandler ValueChangedHandler { get; set; }

		[Export ("value")]
		float Value {  /* float, not CGFloat */
			get;
			[iOS (13, 0)]
			[TV (13, 0)]
			[MacCatalyst (13, 1)]
			set;
		}
	}

	delegate void GCControllerButtonValueChanged (GCControllerButtonInput button, float /* float, not CGFloat */ buttonValue, bool pressed);
	delegate void GCControllerButtonTouchedChanged (GCControllerButtonInput button, float value, bool pressed, bool touched);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCControllerButtonInput {

#if !NET
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
			[iOS (13, 0)]
			[TV (13, 0)]
			[MacCatalyst (13, 1)]
			set;
		}

		[Export ("pressed")]
		bool IsPressed { [Bind ("isPressed")] get; }

#if !NET
		[Obsolete ("Use the 'PressedChangedHandler' property.")]
		[Wrap ("PressedChangedHandler = handler;", IsVirtual = true)]
		void SetPressedChangedHandler (GCControllerButtonValueChanged handler);
#endif

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("pressedChangedHandler", ArgumentSemantic.Copy)]
		GCControllerButtonValueChanged PressedChangedHandler { get; set; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("touchedChangedHandler", ArgumentSemantic.Copy)]
		GCControllerButtonTouchedChanged TouchedChangedHandler { get; set; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("touched")]
		bool Touched { [Bind ("isTouched")] get; }
	}

	delegate void GCControllerDirectionPadValueChangedHandler (GCControllerDirectionPad dpad, float /* float, not CGFloat */ xValue, float /* float, not CGFloat */ yValue);

	[MacCatalyst (13, 1)]
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

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setValueForXAxis:yAxis:")]
		void SetValue (float xAxis, float yAxis);
	}

	delegate void GCGamepadValueChangedHandler (GCGamepad gamepad, GCControllerElement element);

	[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'GCExtendedGamepad' instead.")]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'GCExtendedGamepad' instead.")]
	[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'GCExtendedGamepad' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GCExtendedGamepad' instead.")]
	[BaseType (typeof (GCPhysicalInputProfile))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	partial interface GCGamepad {

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
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GCExtendedGamepad' instead.")]
	[BaseType (typeof (GCGamepad))]
	[DisableDefaultCtor]
	partial interface GCGamepadSnapshot {

		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		NativeHandle Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		NativeHandle Constructor (GCController controller, NSData data);
	}

	delegate void GCExtendedGamepadValueChangedHandler (GCExtendedGamepad gamepad, GCControllerElement element);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (GCPhysicalInputProfile))]
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
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GCController.Capture()' instead.")]
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

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("leftThumbstickButton")]
		GCControllerButtonInput LeftThumbstickButton { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("rightThumbstickButton")]
		GCControllerButtonInput RightThumbstickButton { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("buttonMenu")]
		GCControllerButtonInput ButtonMenu { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("buttonOptions")]
		GCControllerButtonInput ButtonOptions { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setStateFromExtendedGamepad:")]
		void SetState (GCExtendedGamepad extendedGamepad);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("buttonHome")]
		GCControllerButtonInput ButtonHome { get; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
	[BaseType (typeof (GCExtendedGamepad))]
	[DisableDefaultCtor]
	partial interface GCExtendedGamepadSnapshot {

		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		NativeHandle Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		NativeHandle Constructor (GCController controller, NSData data);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GCController.GetExtendedGamepadController()' instead.")]
		[Field ("GCCurrentExtendedGamepadSnapshotDataVersion")]
		GCExtendedGamepadSnapshotDataVersion DataVersion { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface GCController : GCDevice {

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the Menu button found on the controller's profile, if it exists.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the Menu button found on the controller's profile, if it exists.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the Menu button found on the controller's profile, if it exists.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the Menu button found on the controller's profile, if it exists.")]
		[NullAllowed]
		[Export ("controllerPausedHandler", ArgumentSemantic.Copy)]
		Action<GCController> ControllerPausedHandler { get; set; }

		[NullAllowed]
		[Export ("vendorName", ArgumentSemantic.Copy)]
		new string VendorName { get; }

		[Export ("attachedToDevice")]
		bool AttachedToDevice { [Bind ("isAttachedToDevice")] get; }

		[Export ("playerIndex")]
#if NET
		// enum only added in iOS9 / OSX 10.11 - but with compatible values
		GCControllerPlayerIndex PlayerIndex { get; set; }
#else
		nint PlayerIndex { get; set; }
#endif

		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[NullAllowed]
		[Export ("gamepad", ArgumentSemantic.Retain)]
		GCGamepad Gamepad { get; }

		[NullAllowed]
		[Export ("extendedGamepad", ArgumentSemantic.Retain)]
		GCExtendedGamepad ExtendedGamepad { get; }

		[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("motion", ArgumentSemantic.Retain)]
		GCMotion Motion { get; }

		[MacCatalyst (13, 1)]
		[Export ("handlerQueue", ArgumentSemantic.Retain)]
		new DispatchQueue HandlerQueue { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("productCategory")]
		new string ProductCategory { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("snapshot")]
		bool Snapshot { [Bind ("isSnapshot")] get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("capture")]
		GCController Capture ();

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("controllerWithMicroGamepad")]
		GCController GetMicroGamepadController ();

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("controllerWithExtendedGamepad")]
		GCController GetExtendedGamepadController ();

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[NullAllowed, Export ("current", ArgumentSemantic.Strong)]
		GCController Current { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("light", ArgumentSemantic.Retain)]
		GCDeviceLight Light { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("haptics", ArgumentSemantic.Retain)]
		GCDeviceHaptics Haptics { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("battery", ArgumentSemantic.Copy)]
		GCDeviceBattery Battery { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Notification, Field ("GCControllerDidBecomeCurrentNotification")]
		NSString DidBecomeCurrentNotification { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Notification, Field ("GCControllerDidStopBeingCurrentNotification")]
		NSString DidStopBeingCurrentNotification { get; }

		[TV (14, 5)]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Export ("shouldMonitorBackgroundEvents")]
		bool ShouldMonitorBackgroundEvents { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // access thru GCController.Motion - returns a nil Handle
	partial interface GCMotion {

		[NullAllowed]
		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

#if !NET
		[Obsolete ("Use the 'ValueChangedHandler' property.")]
		[Wrap ("ValueChangedHandler = handler;", IsVirtual = true)]
		void SetValueChangedHandler (Action<GCMotion> handler);
#endif

		[NullAllowed]
		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		Action<GCMotion> ValueChangedHandler { get; set; }

		[Export ("gravity", ArgumentSemantic.Assign)]
#if NET
		GCAcceleration Gravity { get; }
#else
		Vector3d Gravity { get; }
#endif

		[Export ("userAcceleration", ArgumentSemantic.Assign)]
#if NET
		GCAcceleration UserAcceleration { get; }
#else
		Vector3d UserAcceleration { get; }
#endif

		[MacCatalyst (13, 1)]
		[Export ("attitude", ArgumentSemantic.Assign)]
#if NET
		GCQuaternion Attitude { get; }
#else
		Quaterniond Attitude { get; }
#endif

		[MacCatalyst (13, 1)]
		[Export ("rotationRate", ArgumentSemantic.Assign)]
#if NET
		GCRotationRate RotationRate { get; }
#else
		Vector3d RotationRate { get; }
#endif

		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'HasAttitude' and 'HasRotationRate' instead.")]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'HasAttitude' and 'HasRotationRate' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'HasAttitude' and 'HasRotationRate' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'HasAttitude' and 'HasRotationRate' instead.")]
		[Export ("hasAttitudeAndRotationRate")]
		bool HasAttitudeAndRotationRate { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setGravity:")]
		void SetGravity (GCAcceleration gravity);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setUserAcceleration:")]
		void SetUserAcceleration (GCAcceleration userAcceleration);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setAttitude:")]
		void SetAttitude (GCQuaternion attitude);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setRotationRate:")]
		void SetRotationRate (GCRotationRate rotationRate);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setStateFromMotion:")]
		void SetState (GCMotion motion);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("hasAttitude")]
		bool HasAttitude { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("hasRotationRate")]
		bool HasRotationRate { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sensorsRequireManualActivation")]
		bool SensorsRequireManualActivation { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sensorsActive")]
		bool SensorsActive { get; set; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("hasGravityAndUserAcceleration")]
		bool HasGravityAndUserAcceleration { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("acceleration")]
		GCAcceleration Acceleration { get; set; }
	}

	[MacCatalyst (13, 1)]
	delegate void GCMicroGamepadValueChangedHandler (GCMicroGamepad gamepad, GCControllerElement element);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (GCPhysicalInputProfile))]
	[DisableDefaultCtor]
	interface GCMicroGamepad {
		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

		[NullAllowed, Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCMicroGamepadValueChangedHandler ValueChangedHandler { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.Capture()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GCController.Capture()' instead.")]
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

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("buttonMenu")]
		GCControllerButtonInput ButtonMenu { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setStateFromMicroGamepad:")]
		void SetState (GCMicroGamepad microGamepad);
	}

	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.Capture()' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.Capture()' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GCController.Capture()' instead.")]
	[BaseType (typeof (GCMicroGamepad))]
	interface GCMicroGamepadSnapshot {
		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		NativeHandle Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		NativeHandle Constructor (GCController controller, NSData data);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCControler.GetMicroGamepadController()' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
		[Field ("GCCurrentMicroGamepadSnapshotDataVersion")]
		GCMicroGamepadSnapshotDataVersion DataVersion { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface GCEventViewController {

		// inlined ctor
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("controllerUserInteractionEnabled")]
		bool ControllerUserInteractionEnabled { get; set; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCColor : NSCopying, NSSecureCoding {
		[Export ("initWithRed:green:blue:")]
		NativeHandle Constructor (float red, float green, float blue);

		[Export ("red")]
		float Red { get; }

		[Export ("green")]
		float Green { get; }

		[Export ("blue")]
		float Blue { get; }
	}

	delegate void GCControllerTouchpadHandler (GCControllerTouchpad touchpad, float xValue, float yValue, float buttonValue, bool buttonPressed);

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (GCControllerElement))]
	interface GCControllerTouchpad {
		[Export ("button")]
		GCControllerButtonInput Button { get; }

		[NullAllowed, Export ("touchDown", ArgumentSemantic.Copy)]
		GCControllerTouchpadHandler TouchDown { get; set; }

		[NullAllowed, Export ("touchMoved", ArgumentSemantic.Copy)]
		GCControllerTouchpadHandler TouchMoved { get; set; }

		[NullAllowed, Export ("touchUp", ArgumentSemantic.Copy)]
		GCControllerTouchpadHandler TouchUp { get; set; }

		[Export ("touchSurface")]
		GCControllerDirectionPad TouchSurface { get; }

		[Export ("touchState")]
		GCTouchState TouchState { get; }

		[Export ("reportsAbsoluteTouchSurfaceValues")]
		bool ReportsAbsoluteTouchSurfaceValues { get; set; }

		[Export ("setValueForXAxis:yAxis:touchDown:buttonValue:")]
		void SetValue (float xAxis, float yAxis, bool touchDown, float buttonValue);
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCDeviceBattery : NSSecureCoding, NSCoding {
		[Export ("batteryLevel")]
		float BatteryLevel { get; }

		[Export ("batteryState")]
		GCDeviceBatteryState BatteryState { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCDeviceHaptics {
		[Export ("supportedLocalities", ArgumentSemantic.Strong)]
		NSSet<NSString> SupportedLocalities { get; }

		[NoMac] // TODO: Remove [NoMac] when CoreHaptics can compile on Mac OSX: https://github.com/xamarin/maccore/issues/2261
		[MacCatalyst (13, 1)]
		[Export ("createEngineWithLocality:")]
		[return: NullAllowed]
		CHHapticEngine CreateEngine (string locality);

		[Field ("GCHapticDurationInfinite")]
		float HapticDurationInfinite { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Static]
	interface GCHapticsLocality {

		[Field ("GCHapticsLocalityDefault")]
		NSString Default { get; }

		[Field ("GCHapticsLocalityAll")]
		NSString All { get; }

		[Field ("GCHapticsLocalityHandles")]
		NSString Handles { get; }

		[Field ("GCHapticsLocalityLeftHandle")]
		NSString LeftHandle { get; }

		[Field ("GCHapticsLocalityRightHandle")]
		NSString RightHandle { get; }

		[Field ("GCHapticsLocalityTriggers")]
		NSString Triggers { get; }

		[Field ("GCHapticsLocalityLeftTrigger")]
		NSString LeftTrigger { get; }

		[Field ("GCHapticsLocalityRightTrigger")]
		NSString RightTrigger { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCDeviceLight : NSSecureCoding, NSCoding {
		[Export ("color", ArgumentSemantic.Copy)]
		GCColor Color { get; set; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (GCExtendedGamepad))]
	interface GCDualShockGamepad : NSSecureCoding, NSCoding {
		[Export ("touchpadButton")]
		GCControllerButtonInput TouchpadButton { get; }

		[Export ("touchpadPrimary")]
		GCControllerDirectionPad TouchpadPrimary { get; }

		[Export ("touchpadSecondary")]
		GCControllerDirectionPad TouchpadSecondary { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
#if !XAMCORE_5_0
	interface GCKeyboard : GCDevice, NSSecureCoding, NSCoding {
#else
	interface GCKeyboard : GCDevice {
#endif

		[NullAllowed, Export ("keyboardInput", ArgumentSemantic.Strong)]
		GCKeyboardInput KeyboardInput { get; }

		[Static]
		[NullAllowed, Export ("coalescedKeyboard", ArgumentSemantic.Strong)]
		GCKeyboard CoalescedKeyboard { get; }

		[Notification, Field ("GCKeyboardDidConnectNotification")]
		NSString DidConnectNotification { get; }

		[Notification, Field ("GCKeyboardDidDisconnectNotification")]
		NSString DidDisconnectNotification { get; }
	}

	delegate void GCKeyboardValueChangedHandler (GCKeyboardInput keyboard, GCControllerButtonInput key, nint keyCode, bool pressed);

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (GCPhysicalInputProfile))]
	interface GCKeyboardInput {
		[NullAllowed, Export ("keyChangedHandler", ArgumentSemantic.Copy)]
		GCKeyboardValueChangedHandler KeyChangedHandler { get; set; }

		[Export ("anyKeyPressed")]
		bool IsAnyKeyPressed { [Bind ("isAnyKeyPressed")] get; }

		[Export ("buttonForKeyCode:")]
		[return: NullAllowed]
		GCControllerButtonInput GetButton (nint code);
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface GCMouse : GCDevice, NSSecureCoding, NSCoding {
		[NullAllowed, Export ("mouseInput", ArgumentSemantic.Strong)]
		GCMouseInput MouseInput { get; }

		[Static]
		[NullAllowed, Export ("current", ArgumentSemantic.Strong)]
		GCMouse Current { get; }

		[Static]
		[Export ("mice")]
		GCMouse [] Mice { get; }

		[Notification, Field ("GCMouseDidConnectNotification")]
		NSString DidConnectNotification { get; }

		[Notification, Field ("GCMouseDidDisconnectNotification")]
		NSString DidDisconnectNotification { get; }

		[Notification, Field ("GCMouseDidBecomeCurrentNotification")]
		NSString DidBecomeCurrentNotification { get; }

		[Notification, Field ("GCMouseDidStopBeingCurrentNotification")]
		NSString DidStopBeingCurrentNotification { get; }
	}

	delegate void GCMouseMoved (GCMouseInput mouse, float deltaX, float deltaY);

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (GCControllerDirectionPad))]
	interface GCDeviceCursor { }

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (GCPhysicalInputProfile))]
	interface GCMouseInput {
		[NullAllowed, Export ("mouseMovedHandler", ArgumentSemantic.Copy)]
		GCMouseMoved MouseMovedHandler { get; set; }

		[Export ("scroll")]
		GCDeviceCursor Scroll { get; }

		[Export ("leftButton")]
		GCControllerButtonInput LeftButton { get; }

		[NullAllowed, Export ("rightButton")]
		GCControllerButtonInput RightButton { get; }

		[NullAllowed, Export ("middleButton")]
		GCControllerButtonInput MiddleButton { get; }

		[NullAllowed, Export ("auxiliaryButtons")]
		GCControllerButtonInput [] AuxiliaryButtons { get; }
	}

	interface IGCDevice { }

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface GCDevice {
		[Abstract]
		[Export ("handlerQueue", ArgumentSemantic.Strong)]
		DispatchQueue HandlerQueue { get; set; }

		[Abstract]
		[NullAllowed, Export ("vendorName")]
		string VendorName { get; }

		[Abstract]
		[Export ("productCategory")]
		string ProductCategory { get; }

		[Abstract]
		[Export ("physicalInputProfile", ArgumentSemantic.Strong)]
		GCPhysicalInputProfile PhysicalInputProfile { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCPhysicalInputProfile {
		[NullAllowed, Export ("device", ArgumentSemantic.Weak)]
		IGCDevice Device { get; }

		[Export ("lastEventTimestamp")]
		double LastEventTimestamp { get; }

		[Export ("elements", ArgumentSemantic.Strong)]
		NSDictionary<NSString, GCControllerElement> Elements { get; }

		[Export ("buttons", ArgumentSemantic.Strong)]
		NSDictionary<NSString, GCControllerButtonInput> Buttons { get; }

		[Export ("axes", ArgumentSemantic.Strong)]
		NSDictionary<NSString, GCControllerAxisInput> Axes { get; }

		[Export ("dpads", ArgumentSemantic.Strong)]
		NSDictionary<NSString, GCControllerDirectionPad> Dpads { get; }

		[Export ("allElements", ArgumentSemantic.Strong)]
		NSSet<GCControllerElement> AllElements { get; }

		[Export ("allButtons", ArgumentSemantic.Strong)]
		NSSet<GCControllerButtonInput> AllButtons { get; }

		[Export ("allAxes", ArgumentSemantic.Strong)]
		NSSet<GCControllerAxisInput> AllAxes { get; }

		[Export ("allDpads", ArgumentSemantic.Strong)]
		NSSet<GCControllerDirectionPad> AllDpads { get; }

		[Export ("objectForKeyedSubscript:")]
		[return: NullAllowed]
		GCControllerElement GetObjectForKeyedSubscript (string key);

		[Export ("capture")]
		GCPhysicalInputProfile Capture ();

		[Export ("setStateFromPhysicalInput:")]
		void SetState (GCPhysicalInputProfile physicalInput);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("allTouchpads", ArgumentSemantic.Strong)]
		NSSet<GCControllerTouchpad> AllTouchpads { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("touchpads", ArgumentSemantic.Strong)]
		NSDictionary<NSString, GCControllerTouchpad> Touchpads { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("hasRemappedElements")]
		bool HasRemappedElements { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("mappedElementAliasForPhysicalInputName:")]
		string GetMappedElementAlias (string physicalInputName);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("mappedPhysicalInputNamesForElementAlias:")]
		NSSet<NSString> GetMappedPhysicalInputNames (string elementAlias);

		[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch]
		[NullAllowed, Export ("valueDidChangeHandler", ArgumentSemantic.Copy)]
		Action<GCPhysicalInputProfile, GCControllerElement> ValueDidChangeHandler { get; set; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Static]
	interface GCInputXbox {

		[Field ("GCInputXboxPaddleOne")]
		NSString PaddleOne { get; }

		[Field ("GCInputXboxPaddleTwo")]
		NSString PaddleTwo { get; }

		[Field ("GCInputXboxPaddleThree")]
		NSString PaddleThree { get; }

		[Field ("GCInputXboxPaddleFour")]
		NSString PaddleFour { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Static]
	interface GCInput {

		[Field ("GCInputButtonA")]
		NSString ButtonA { get; }

		[Field ("GCInputButtonB")]
		NSString ButtonB { get; }

		[Field ("GCInputButtonX")]
		NSString ButtonX { get; }

		[Field ("GCInputButtonY")]
		NSString ButtonY { get; }

		[Field ("GCInputDirectionPad")]
		NSString DirectionPad { get; }

		[Field ("GCInputLeftThumbstick")]
		NSString LeftThumbstick { get; }

		[Field ("GCInputRightThumbstick")]
		NSString RightThumbstick { get; }

		[Field ("GCInputLeftShoulder")]
		NSString LeftShoulder { get; }

		[Field ("GCInputRightShoulder")]
		NSString RightShoulder { get; }

		[Field ("GCInputLeftTrigger")]
		NSString LeftTrigger { get; }

		[Field ("GCInputRightTrigger")]
		NSString RightTrigger { get; }

		[Field ("GCInputLeftThumbstickButton")]
		NSString LeftThumbstickButton { get; }

		[Field ("GCInputRightThumbstickButton")]
		NSString RightThumbstickButton { get; }

		[Field ("GCInputButtonHome")]
		NSString ButtonHome { get; }

		[Field ("GCInputButtonMenu")]
		NSString ButtonMenu { get; }

		[Field ("GCInputButtonOptions")]
		NSString ButtonOptions { get; }

		[Field ("GCInputDualShockTouchpadOne")]
		NSString DualShockTouchpadOne { get; }

		[Field ("GCInputDualShockTouchpadTwo")]
		NSString DualShockTouchpadTwo { get; }

		[Field ("GCInputDualShockTouchpadButton")]
		NSString DualShockTouchpadButton { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCInputButtonShare")]
		NSString ButtonShare { get; }

		[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
		[Field ("GCInputLeftPaddle")]
		NSString /* IGCButtonElementName */ LeftPaddle { get; }

		[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
		[Field ("GCInputPedalAccelerator")]
		NSString /* IGCButtonElementName */ PedalAccelerator { get; }

		[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
		[Field ("GCInputPedalBrake")]
		NSString /* IGCButtonElementName */ PedalBrake { get; }

		[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
		[Field ("GCInputPedalClutch")]
		NSString /* IGCButtonElementName */ PedalClutch { get; }

		[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
		[Field ("GCInputRightPaddle")]
		NSString /* IGCButtonElementName */ RightPaddle { get; }

		[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
		[Field ("GCInputShifter")]
		NSString /* IGCPhysicalInputElementName */ Shifter { get; }

		[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
		[Field ("GCInputSteeringWheel")]
		NSString /* IGCAxisElementName */ SteeringWheel { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (GCExtendedGamepad))]
	interface GCXboxGamepad : NSSecureCoding, NSCoding {
		[NullAllowed, Export ("paddleButton1")]
		GCControllerButtonInput PaddleButton1 { get; }

		[NullAllowed, Export ("paddleButton2")]
		GCControllerButtonInput PaddleButton2 { get; }

		[NullAllowed, Export ("paddleButton3")]
		GCControllerButtonInput PaddleButton3 { get; }

		[NullAllowed, Export ("paddleButton4")]
		GCControllerButtonInput PaddleButton4 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("buttonShare")]
		GCControllerButtonInput ButtonShare { get; }
	}

	[Static]
	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	partial interface GCKey {
		[Field ("GCKeyA")]
		NSString A { get; }

		[Field ("GCKeyB")]
		NSString B { get; }

		[Field ("GCKeyC")]
		NSString C { get; }

		[Field ("GCKeyD")]
		NSString D { get; }

		[Field ("GCKeyE")]
		NSString E { get; }

		[Field ("GCKeyF")]
		NSString F { get; }

		[Field ("GCKeyG")]
		NSString G { get; }

		[Field ("GCKeyH")]
		NSString H { get; }

		[Field ("GCKeyI")]
		NSString I { get; }

		[Field ("GCKeyJ")]
		NSString J { get; }

		[Field ("GCKeyK")]
		NSString K { get; }

		[Field ("GCKeyL")]
		NSString L { get; }

		[Field ("GCKeyM")]
		NSString M { get; }

		[Field ("GCKeyN")]
		NSString N { get; }

		[Field ("GCKeyO")]
		NSString O { get; }

		[Field ("GCKeyP")]
		NSString P { get; }

		[Field ("GCKeyQ")]
		NSString Q { get; }

		[Field ("GCKeyR")]
		NSString R { get; }

		[Field ("GCKeyS")]
		NSString S { get; }

		[Field ("GCKeyT")]
		NSString T { get; }

		[Field ("GCKeyU")]
		NSString U { get; }

		[Field ("GCKeyV")]
		NSString V { get; }

		[Field ("GCKeyW")]
		NSString W { get; }

		[Field ("GCKeyX")]
		NSString X { get; }

		[Field ("GCKeyY")]
		NSString Y { get; }

		[Field ("GCKeyZ")]
		NSString Z { get; }

		[Field ("GCKeyOne")]
		NSString One { get; }

		[Field ("GCKeyTwo")]
		NSString Two { get; }

		[Field ("GCKeyThree")]
		NSString Three { get; }

		[Field ("GCKeyFour")]
		NSString Four { get; }

		[Field ("GCKeyFive")]
		NSString Five { get; }

		[Field ("GCKeySix")]
		NSString Six { get; }

		[Field ("GCKeySeven")]
		NSString Seven { get; }

		[Field ("GCKeyEight")]
		NSString Eight { get; }

		[Field ("GCKeyNine")]
		NSString Nine { get; }

		[Field ("GCKeyZero")]
		NSString Zero { get; }

		[Field ("GCKeyReturnOrEnter")]
		NSString ReturnOrEnter { get; }

		[Field ("GCKeyEscape")]
		NSString Escape { get; }

		[Field ("GCKeyDeleteOrBackspace")]
		NSString DeleteOrBackspace { get; }

		[Field ("GCKeyTab")]
		NSString Tab { get; }

		[Field ("GCKeySpacebar")]
		NSString Spacebar { get; }

		[Field ("GCKeyHyphen")]
		NSString Hyphen { get; }

		[Field ("GCKeyEqualSign")]
		NSString EqualSign { get; }

		[Field ("GCKeyOpenBracket")]
		NSString OpenBracket { get; }

		[Field ("GCKeyCloseBracket")]
		NSString CloseBracket { get; }

		[Field ("GCKeyBackslash")]
		NSString Backslash { get; }

		[Field ("GCKeyNonUSPound")]
		NSString NonUSPound { get; }

		[Field ("GCKeySemicolon")]
		NSString Semicolon { get; }

		[Field ("GCKeyQuote")]
		NSString Quote { get; }

		[Field ("GCKeyGraveAccentAndTilde")]
		NSString GraveAccentAndTilde { get; }

		[Field ("GCKeyComma")]
		NSString Comma { get; }

		[Field ("GCKeyPeriod")]
		NSString Period { get; }

		[Field ("GCKeySlash")]
		NSString Slash { get; }

		[Field ("GCKeyCapsLock")]
		NSString CapsLock { get; }

		[Field ("GCKeyF1")]
		NSString F1 { get; }

		[Field ("GCKeyF2")]
		NSString F2 { get; }

		[Field ("GCKeyF3")]
		NSString F3 { get; }

		[Field ("GCKeyF4")]
		NSString F4 { get; }

		[Field ("GCKeyF5")]
		NSString F5 { get; }

		[Field ("GCKeyF6")]
		NSString F6 { get; }

		[Field ("GCKeyF7")]
		NSString F7 { get; }

		[Field ("GCKeyF8")]
		NSString F8 { get; }

		[Field ("GCKeyF9")]
		NSString F9 { get; }

		[Field ("GCKeyF10")]
		NSString F10 { get; }

		[Field ("GCKeyF11")]
		NSString F11 { get; }

		[Field ("GCKeyF12")]
		NSString F12 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyF13")]
		NSString F13 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyF14")]
		NSString F14 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyF15")]
		NSString F15 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyF16")]
		NSString F16 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyF17")]
		NSString F17 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyF18")]
		NSString F18 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyF19")]
		NSString F19 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyF20")]
		NSString F20 { get; }

		[Field ("GCKeyPrintScreen")]
		NSString PrintScreen { get; }

		[Field ("GCKeyScrollLock")]
		NSString ScrollLock { get; }

		[Field ("GCKeyPause")]
		NSString Pause { get; }

		[Field ("GCKeyInsert")]
		NSString Insert { get; }

		[Field ("GCKeyHome")]
		NSString Home { get; }

		[Field ("GCKeyPageUp")]
		NSString PageUp { get; }

		[Field ("GCKeyDeleteForward")]
		NSString DeleteForward { get; }

		[Field ("GCKeyEnd")]
		NSString End { get; }

		[Field ("GCKeyPageDown")]
		NSString PageDown { get; }

		[Field ("GCKeyRightArrow")]
		NSString RightArrow { get; }

		[Field ("GCKeyLeftArrow")]
		NSString LeftArrow { get; }

		[Field ("GCKeyDownArrow")]
		NSString DownArrow { get; }

		[Field ("GCKeyUpArrow")]
		NSString UpArrow { get; }

		[Field ("GCKeyKeypadNumLock")]
		NSString KeypadNumLock { get; }

		[Field ("GCKeyKeypadSlash")]
		NSString KeypadSlash { get; }

		[Field ("GCKeyKeypadAsterisk")]
		NSString KeypadAsterisk { get; }

		[Field ("GCKeyKeypadHyphen")]
		NSString KeypadHyphen { get; }

		[Field ("GCKeyKeypadPlus")]
		NSString KeypadPlus { get; }

		[Field ("GCKeyKeypadEnter")]
		NSString KeypadEnter { get; }

		[Field ("GCKeyKeypad1")]
		NSString Keypad1 { get; }

		[Field ("GCKeyKeypad2")]
		NSString Keypad2 { get; }

		[Field ("GCKeyKeypad3")]
		NSString Keypad3 { get; }

		[Field ("GCKeyKeypad4")]
		NSString Keypad4 { get; }

		[Field ("GCKeyKeypad5")]
		NSString Keypad5 { get; }

		[Field ("GCKeyKeypad6")]
		NSString Keypad6 { get; }

		[Field ("GCKeyKeypad7")]
		NSString Keypad7 { get; }

		[Field ("GCKeyKeypad8")]
		NSString Keypad8 { get; }

		[Field ("GCKeyKeypad9")]
		NSString Keypad9 { get; }

		[Field ("GCKeyKeypad0")]
		NSString Keypad0 { get; }

		[Field ("GCKeyKeypadPeriod")]
		NSString KeypadPeriod { get; }

		[Field ("GCKeyKeypadEqualSign")]
		NSString KeypadEqualSign { get; }

		[Field ("GCKeyNonUSBackslash")]
		NSString NonUSBackslash { get; }

		[Field ("GCKeyApplication")]
		NSString Application { get; }

		[Field ("GCKeyPower")]
		NSString Power { get; }

		[Field ("GCKeyInternational1")]
		NSString International1 { get; }

		[Field ("GCKeyInternational2")]
		NSString International2 { get; }

		[Field ("GCKeyInternational3")]
		NSString International3 { get; }

		[Field ("GCKeyInternational4")]
		NSString International4 { get; }

		[Field ("GCKeyInternational5")]
		NSString International5 { get; }

		[Field ("GCKeyInternational6")]
		NSString International6 { get; }

		[Field ("GCKeyInternational7")]
		NSString International7 { get; }

		[Field ("GCKeyInternational8")]
		NSString International8 { get; }

		[Field ("GCKeyInternational9")]
		NSString International9 { get; }

		[Field ("GCKeyLANG1")]
		NSString Lang1 { get; }

		[Field ("GCKeyLANG2")]
		NSString Lang2 { get; }

		[Field ("GCKeyLANG3")]
		NSString Lang3 { get; }

		[Field ("GCKeyLANG4")]
		NSString Lang4 { get; }

		[Field ("GCKeyLANG5")]
		NSString Lang5 { get; }

		[Field ("GCKeyLANG6")]
		NSString Lang6 { get; }

		[Field ("GCKeyLANG7")]
		NSString Lang7 { get; }

		[Field ("GCKeyLANG8")]
		NSString Lang8 { get; }

		[Field ("GCKeyLANG9")]
		NSString Lang9 { get; }

		[Field ("GCKeyLeftControl")]
		NSString LeftControl { get; }

		[Field ("GCKeyLeftShift")]
		NSString LeftShift { get; }

		[Field ("GCKeyLeftAlt")]
		NSString LeftAlt { get; }

		[Field ("GCKeyLeftGUI")]
		NSString LeftGui { get; }

		[Field ("GCKeyRightControl")]
		NSString RightControl { get; }

		[Field ("GCKeyRightShift")]
		NSString RightShift { get; }

		[Field ("GCKeyRightAlt")]
		NSString RightAlt { get; }

		[Field ("GCKeyRightGUI")]
		NSString RightGui { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Static]
	interface GCKeyCode {
		[Field ("GCKeyCodeKeyA")]
		nint KeyA { get; }

		[Field ("GCKeyCodeKeyB")]
		nint KeyB { get; }

		[Field ("GCKeyCodeKeyC")]
		nint KeyC { get; }

		[Field ("GCKeyCodeKeyD")]
		nint KeyD { get; }

		[Field ("GCKeyCodeKeyE")]
		nint KeyE { get; }

		[Field ("GCKeyCodeKeyF")]
		nint KeyF { get; }

		[Field ("GCKeyCodeKeyG")]
		nint KeyG { get; }

		[Field ("GCKeyCodeKeyH")]
		nint KeyH { get; }

		[Field ("GCKeyCodeKeyI")]
		nint KeyI { get; }

		[Field ("GCKeyCodeKeyJ")]
		nint KeyJ { get; }

		[Field ("GCKeyCodeKeyK")]
		nint KeyK { get; }

		[Field ("GCKeyCodeKeyL")]
		nint KeyL { get; }

		[Field ("GCKeyCodeKeyM")]
		nint KeyM { get; }

		[Field ("GCKeyCodeKeyN")]
		nint KeyN { get; }

		[Field ("GCKeyCodeKeyO")]
		nint KeyO { get; }

		[Field ("GCKeyCodeKeyP")]
		nint KeyP { get; }

		[Field ("GCKeyCodeKeyQ")]
		nint KeyQ { get; }

		[Field ("GCKeyCodeKeyR")]
		nint KeyR { get; }

		[Field ("GCKeyCodeKeyS")]
		nint KeyS { get; }

		[Field ("GCKeyCodeKeyT")]
		nint KeyT { get; }

		[Field ("GCKeyCodeKeyU")]
		nint KeyU { get; }

		[Field ("GCKeyCodeKeyV")]
		nint KeyV { get; }

		[Field ("GCKeyCodeKeyW")]
		nint KeyW { get; }

		[Field ("GCKeyCodeKeyX")]
		nint KeyX { get; }

		[Field ("GCKeyCodeKeyY")]
		nint KeyY { get; }

		[Field ("GCKeyCodeKeyZ")]
		nint KeyZ { get; }

		[Field ("GCKeyCodeOne")]
		nint One { get; }

		[Field ("GCKeyCodeTwo")]
		nint Two { get; }

		[Field ("GCKeyCodeThree")]
		nint Three { get; }

		[Field ("GCKeyCodeFour")]
		nint Four { get; }

		[Field ("GCKeyCodeFive")]
		nint Five { get; }

		[Field ("GCKeyCodeSix")]
		nint Six { get; }

		[Field ("GCKeyCodeSeven")]
		nint Seven { get; }

		[Field ("GCKeyCodeEight")]
		nint Eight { get; }

		[Field ("GCKeyCodeNine")]
		nint Nine { get; }

		[Field ("GCKeyCodeZero")]
		nint Zero { get; }

		[Field ("GCKeyCodeReturnOrEnter")]
		nint ReturnOrEnter { get; }

		[Field ("GCKeyCodeEscape")]
		nint Escape { get; }

		[Field ("GCKeyCodeDeleteOrBackspace")]
		nint DeleteOrBackspace { get; }

		[Field ("GCKeyCodeTab")]
		nint Tab { get; }

		[Field ("GCKeyCodeSpacebar")]
		nint Spacebar { get; }

		[Field ("GCKeyCodeHyphen")]
		nint Hyphen { get; }

		[Field ("GCKeyCodeEqualSign")]
		nint EqualSign { get; }

		[Field ("GCKeyCodeOpenBracket")]
		nint OpenBracket { get; }

		[Field ("GCKeyCodeCloseBracket")]
		nint CloseBracket { get; }

		[Field ("GCKeyCodeBackslash")]
		nint Backslash { get; }

		[Field ("GCKeyCodeNonUSPound")]
		nint NonUSPound { get; }

		[Field ("GCKeyCodeSemicolon")]
		nint Semicolon { get; }

		[Field ("GCKeyCodeQuote")]
		nint Quote { get; }

		[Field ("GCKeyCodeGraveAccentAndTilde")]
		nint GraveAccentAndTilde { get; }

		[Field ("GCKeyCodeComma")]
		nint Comma { get; }

		[Field ("GCKeyCodePeriod")]
		nint Period { get; }

		[Field ("GCKeyCodeSlash")]
		nint Slash { get; }

		[Field ("GCKeyCodeCapsLock")]
		nint CapsLock { get; }

		[Field ("GCKeyCodeF1")]
		nint F1 { get; }

		[Field ("GCKeyCodeF2")]
		nint F2 { get; }

		[Field ("GCKeyCodeF3")]
		nint F3 { get; }

		[Field ("GCKeyCodeF4")]
		nint F4 { get; }

		[Field ("GCKeyCodeF5")]
		nint F5 { get; }

		[Field ("GCKeyCodeF6")]
		nint F6 { get; }

		[Field ("GCKeyCodeF7")]
		nint F7 { get; }

		[Field ("GCKeyCodeF8")]
		nint F8 { get; }

		[Field ("GCKeyCodeF9")]
		nint F9 { get; }

		[Field ("GCKeyCodeF10")]
		nint F10 { get; }

		[Field ("GCKeyCodeF11")]
		nint F11 { get; }

		[Field ("GCKeyCodeF12")]
		nint F12 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyCodeF13")]
		nint F13 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyCodeF14")]
		nint F14 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyCodeF15")]
		nint F15 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyCodeF16")]
		nint F16 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyCodeF17")]
		nint F17 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyCodeF18")]
		nint F18 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyCodeF19")]
		nint F19 { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCKeyCodeF20")]
		nint F20 { get; }

		[Field ("GCKeyCodePrintScreen")]
		nint PrintScreen { get; }

		[Field ("GCKeyCodeScrollLock")]
		nint ScrollLock { get; }

		[Field ("GCKeyCodePause")]
		nint Pause { get; }

		[Field ("GCKeyCodeInsert")]
		nint Insert { get; }

		[Field ("GCKeyCodeHome")]
		nint Home { get; }

		[Field ("GCKeyCodePageUp")]
		nint PageUp { get; }

		[Field ("GCKeyCodeDeleteForward")]
		nint DeleteForward { get; }

		[Field ("GCKeyCodeEnd")]
		nint End { get; }

		[Field ("GCKeyCodePageDown")]
		nint PageDown { get; }

		[Field ("GCKeyCodeRightArrow")]
		nint RightArrow { get; }

		[Field ("GCKeyCodeLeftArrow")]
		nint LeftArrow { get; }

		[Field ("GCKeyCodeDownArrow")]
		nint DownArrow { get; }

		[Field ("GCKeyCodeUpArrow")]
		nint UpArrow { get; }

		[Field ("GCKeyCodeKeypadNumLock")]
		nint KeypadNumLock { get; }

		[Field ("GCKeyCodeKeypadSlash")]
		nint KeypadSlash { get; }

		[Field ("GCKeyCodeKeypadAsterisk")]
		nint KeypadAsterisk { get; }

		[Field ("GCKeyCodeKeypadHyphen")]
		nint KeypadHyphen { get; }

		[Field ("GCKeyCodeKeypadPlus")]
		nint KeypadPlus { get; }

		[Field ("GCKeyCodeKeypadEnter")]
		nint KeypadEnter { get; }

		[Field ("GCKeyCodeKeypad1")]
		nint Keypad1 { get; }

		[Field ("GCKeyCodeKeypad2")]
		nint Keypad2 { get; }

		[Field ("GCKeyCodeKeypad3")]
		nint Keypad3 { get; }

		[Field ("GCKeyCodeKeypad4")]
		nint Keypad4 { get; }

		[Field ("GCKeyCodeKeypad5")]
		nint Keypad5 { get; }

		[Field ("GCKeyCodeKeypad6")]
		nint Keypad6 { get; }

		[Field ("GCKeyCodeKeypad7")]
		nint Keypad7 { get; }

		[Field ("GCKeyCodeKeypad8")]
		nint Keypad8 { get; }

		[Field ("GCKeyCodeKeypad9")]
		nint Keypad9 { get; }

		[Field ("GCKeyCodeKeypad0")]
		nint Keypad0 { get; }

		[Field ("GCKeyCodeKeypadPeriod")]
		nint KeypadPeriod { get; }

		[Field ("GCKeyCodeKeypadEqualSign")]
		nint KeypadEqualSign { get; }

		[Field ("GCKeyCodeNonUSBackslash")]
		nint NonUSBackslash { get; }

		[Field ("GCKeyCodeApplication")]
		nint Application { get; }

		[Field ("GCKeyCodePower")]
		nint Power { get; }

		[Field ("GCKeyCodeInternational1")]
		nint International1 { get; }

		[Field ("GCKeyCodeInternational2")]
		nint International2 { get; }

		[Field ("GCKeyCodeInternational3")]
		nint International3 { get; }

		[Field ("GCKeyCodeInternational4")]
		nint International4 { get; }

		[Field ("GCKeyCodeInternational5")]
		nint International5 { get; }

		[Field ("GCKeyCodeInternational6")]
		nint International6 { get; }

		[Field ("GCKeyCodeInternational7")]
		nint International7 { get; }

		[Field ("GCKeyCodeInternational8")]
		nint International8 { get; }

		[Field ("GCKeyCodeInternational9")]
		nint International9 { get; }

		[Field ("GCKeyCodeLANG1")]
		nint Lang1 { get; }

		[Field ("GCKeyCodeLANG2")]
		nint Lang2 { get; }

		[Field ("GCKeyCodeLANG3")]
		nint Lang3 { get; }

		[Field ("GCKeyCodeLANG4")]
		nint Lang4 { get; }

		[Field ("GCKeyCodeLANG5")]
		nint Lang5 { get; }

		[Field ("GCKeyCodeLANG6")]
		nint Lang6 { get; }

		[Field ("GCKeyCodeLANG7")]
		nint Lang7 { get; }

		[Field ("GCKeyCodeLANG8")]
		nint Lang8 { get; }

		[Field ("GCKeyCodeLANG9")]
		nint Lang9 { get; }

		[Field ("GCKeyCodeLeftControl")]
		nint LeftControl { get; }

		[Field ("GCKeyCodeLeftShift")]
		nint LeftShift { get; }

		[Field ("GCKeyCodeLeftAlt")]
		nint LeftAlt { get; }

		[Field ("GCKeyCodeLeftGUI")]
		nint LeftGui { get; }

		[Field ("GCKeyCodeRightControl")]
		nint RightControl { get; }

		[Field ("GCKeyCodeRightShift")]
		nint RightShift { get; }

		[Field ("GCKeyCodeRightAlt")]
		nint RightAlt { get; }

		[Field ("GCKeyCodeRightGUI")]
		nint RightGui { get; }
	}

	[iOS (14, 3)]
	[TV (14, 3)]
	[Mac (11, 1)]
	[MacCatalyst (14, 3)]
	[BaseType (typeof (GCMicroGamepad))]
	[DisableDefaultCtor]
	interface GCDirectionalGamepad {
	}

	[TV (14, 5)]
	[Mac (11, 3)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Native]
	enum GCDualSenseAdaptiveTriggerMode : long {
		Off = 0,
		Feedback = 1,
		Weapon = 2,
		Vibration = 3,
		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		SlopeFeedback = 4,
	}

	[TV (14, 5)]
	[Mac (11, 3)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Native]
	enum GCDualSenseAdaptiveTriggerStatus : long {
		Unknown = -1,
		FeedbackNoLoad,
		FeedbackLoadApplied,
		WeaponReady,
		WeaponFiring,
		WeaponFired,
		VibrationNotVibrating,
		VibrationIsVibrating,
		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		SlopeFeedbackReady,
		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		SlopeFeedbackApplyingLoad,
		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		SlopeFeedbackFinished,
	}

	[TV (14, 5)]
	[Mac (11, 3)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (GCControllerButtonInput))]
	[DisableDefaultCtor]
	interface GCDualSenseAdaptiveTrigger {

		[Export ("mode")]
		GCDualSenseAdaptiveTriggerMode Mode { get; }

		[Export ("status")]
		GCDualSenseAdaptiveTriggerStatus Status { get; }

		[Export ("armPosition")]
		float ArmPosition { get; }

		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("setModeSlopeFeedbackWithStartPosition:endPosition:startStrength:endStrength:")]
		void SetModeSlopeFeedback (float startPosition, float endPosition, float startStrength, float endStrength);

		[Export ("setModeFeedbackWithStartPosition:resistiveStrength:")]
		void SetModeFeedback (float startPosition, float resistiveStrength);

		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("setModeFeedbackWithResistiveStrengths:")]
		void SetModeFeedback (GCDualSenseAdaptiveTriggerPositionalResistiveStrengths positionalResistiveStrengths);

		[Export ("setModeWeaponWithStartPosition:endPosition:resistiveStrength:")]
		void SetModeWeapon (float startPosition, float endPosition, float resistiveStrength);

		[Export ("setModeVibrationWithStartPosition:amplitude:frequency:")]
		void SetModeVibration (float startPosition, float amplitude, float frequency);

		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("setModeVibrationWithAmplitudes:frequency:")]
		void SetModeVibration (GCDualSenseAdaptiveTriggerPositionalAmplitudes positionalAmplitudes, float frequency);

		[Export ("setModeOff")]
		void SetModeOff ();
	}

	[TV (14, 5)]
	[Mac (11, 3)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (GCExtendedGamepad))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[GCControllerButtonInput setIndex:]: unrecognized selector sent to instance 0x60000147eac0
	interface GCDualSenseGamepad {

		[Export ("touchpadButton")]
		GCControllerButtonInput TouchpadButton { get; }

		[Export ("touchpadPrimary")]
		GCControllerDirectionPad TouchpadPrimary { get; }

		[Export ("touchpadSecondary")]
		GCControllerDirectionPad TouchpadSecondary { get; }

		[Export ("leftTrigger")]
		GCDualSenseAdaptiveTrigger LeftTrigger { get; }

		[Export ("rightTrigger")]
		GCDualSenseAdaptiveTrigger RightTrigger { get; }
	}

	[TV (14, 5)]
	[Mac (11, 3)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	enum GCInputDirectional {
		[Field ("GCInputDirectionalDpad")]
		Dpad,

		[Field ("GCInputDirectionalCardinalDpad")]
		CardinalDpad,

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCInputDirectionalCenterButton")]
		CenterButton,

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("GCInputDirectionalTouchSurfaceButton")]
		TouchSurfaceButton,
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	enum GCInputMicroGamepad {
		[Field ("GCInputMicroGamepadDpad")]
		Dpad,

		[Field ("GCInputMicroGamepadButtonA")]
		ButtonA,

		[Field ("GCInputMicroGamepadButtonX")]
		ButtonX,

		[Field ("GCInputMicroGamepadButtonMenu")]
		ButtonMenu,
	}

	delegate GCVirtualControllerElementConfiguration GCVirtualControllerElementUpdateBlock (GCVirtualControllerElementConfiguration configuration);

	[NoTV, NoMac, NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCVirtualController {
		[Static]
		[Export ("virtualControllerWithConfiguration:")]
		GCVirtualController Create (GCVirtualControllerConfiguration configuration);

		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (GCVirtualControllerConfiguration configuration);

		[Async]
		[Export ("connectWithReplyHandler:")]
		void Connect ([NullAllowed] Action<NSError> reply);

		[Export ("disconnect")]
		void Disconnect ();

		[NullAllowed, Export ("controller", ArgumentSemantic.Weak)]
		GCController Controller { get; }

		[Export ("updateConfigurationForElement:configuration:")]
		void UpdateConfiguration (string element, GCVirtualControllerElementUpdateBlock configuration);
	}

	[NoTV, NoMac, NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface GCVirtualControllerConfiguration {
		[Export ("elements", ArgumentSemantic.Strong)]
		NSSet<NSString> Elements { get; set; }
	}

	[NoTV, NoMac, NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface GCVirtualControllerElementConfiguration {
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[NullAllowed, Export ("path", ArgumentSemantic.Strong)]
		BezierPath Path { get; set; }

		[Export ("actsAsTouchpad")]
		bool ActsAsTouchpad { get; set; }
	}

	// Deliberate decision on not enum'ifying these
	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Static]
	interface GCProductCategory {
		[Field ("GCProductCategoryDualSense")]
		NSString DualSense { get; }

		[Field ("GCProductCategoryDualShock4")]
		NSString DualShock4 { get; }

		[Field ("GCProductCategoryMFi")]
		NSString MFi { get; }

		[Field ("GCProductCategoryXboxOne")]
		NSString XboxOne { get; }

		[Field ("GCProductCategorySiriRemote1stGen")]
		NSString SiriRemote1stGen { get; }

		[Field ("GCProductCategorySiriRemote2ndGen")]
		NSString SiriRemote2ndGen { get; }

		[Field ("GCProductCategoryControlCenterRemote")]
		NSString ControlCenterRemote { get; }

		[Field ("GCProductCategoryUniversalElectronicsRemote")]
		NSString UniversalElectronicsRemote { get; }

		[Field ("GCProductCategoryCoalescedRemote")]
		NSString CoalescedRemote { get; }

		[Field ("GCProductCategoryMouse")]
		NSString Mouse { get; }

		[Field ("GCProductCategoryKeyboard")]
		NSString Keyboard { get; }

		[iOS (16, 0), Mac (13, 0), NoWatch, TV (16, 0), MacCatalyst (16, 0)]
		[Field ("GCProductCategoryHID")]
		NSString GCProductCategoryHid { get; }
	}

	[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCRacingWheel : GCDevice {
		[Static]
		[Export ("connectedRacingWheels")]
		NSSet<GCRacingWheel> ConnectedRacingWheels { get; }

		[Export ("acquireDeviceWithError:")]
		bool AcquireDevice ([NullAllowed] out NSError error);

		[Export ("relinquishDevice")]
		void RelinquishDevice ();

		[Export ("acquired")]
		bool Acquired { [Bind ("isAcquired")] get; }

		[Export ("wheelInput", ArgumentSemantic.Strong)]
		GCRacingWheelInput WheelInput { get; }

		[Export ("snapshot")]
		bool Snapshot { [Bind ("isSnapshot")] get; }

		[Export ("capture")]
		GCRacingWheel Capture { get; }

		[Notification, Field ("GCRacingWheelDidConnectNotification")]
		NSString DidConnectNotification { get; }

		[Notification, Field ("GCRacingWheelDidDisconnectNotification")]
		NSString DidDisconnectNotification { get; }
	}

	[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
	[BaseType (typeof (GCRacingWheelInputState))]
	interface GCRacingWheelInput : GCDevicePhysicalInput {
		// Sealed since GCDevicePhysicalInput.Capture returns IGCDevicePhysicalInputState
		[Sealed, Export ("capture")]
		GCRacingWheelInputState WheelInputCapture { get; }

		// Sealed since GCDevicePhysicalInput.NextInputState returns NSObject
		[Sealed, NullAllowed, Export ("nextInputState")]
		IGCDevicePhysicalInputStateDiff WheelInputNextInputState { get; }
	}

	[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface GCRacingWheelInputState : GCDevicePhysicalInputState {
		[Export ("wheel")]
		GCSteeringWheelElement Wheel { get; }

		[NullAllowed, Export ("acceleratorPedal")]
		IGCButtonElement AcceleratorPedal { get; }

		[NullAllowed, Export ("brakePedal")]
		IGCButtonElement BrakePedal { get; }

		[NullAllowed, Export ("clutchPedal")]
		IGCButtonElement ClutchPedal { get; }

		[NullAllowed, Export ("shifter")]
		GCGearShifterElement Shifter { get; }
	}

	[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCSteeringWheelElement : GCAxisElement {
		[Export ("maximumDegreesOfRotation")]
		float MaximumDegreesOfRotation { get; }
	}

	// There are issues with the Generic Types listed here: https://github.com/xamarin/xamarin-macios/issues/15725
	// [iOS (16,0), Mac (13,0), NoWatch, TV (16,0), MacCatalyst (16,0)]
	// [BaseType (typeof (NSObject))]
	// [DisableDefaultCtor]
	// interface GCPhysicalInputElementCollection<KeyIdentifierType, ElementIdentifierType> // : INSFastEnumeration // # no generator support for FastEnumeration - https://bugzilla.xamarin.com/show_bug.cgi?id=4391
	// 	where KeyIdentifierType : IGCPhysicalInputElementName /* NSString */ // there's currently not an conversion from GCPhysicalInputElementName, GCButtonElementName, and GCDirectionPadElementName to NSString
	// 	where ElementIdentifierType : IGCPhysicalInputElement /* id<GCPhysicalInputElement>> */
	// {
	// 	[Export ("count")]
	// 	nuint Count { get; }

	// 	[Export ("elementForAlias:")]
	// 	[return: NullAllowed]
	// 	IGCPhysicalInputElement GetElement (string alias);

	// 	[Export ("objectForKeyedSubscript:")]
	// 	[return: NullAllowed]
	// 	IGCPhysicalInputElement GetObject (string key);

	// 	[Export ("elementEnumerator")]
	// 	NSEnumerator<IGCPhysicalInputElement> ElementEnumerator { get; }
	// }

	interface IGCDevicePhysicalInputState { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCDevicePhysicalInputState {
		[Abstract]
		[NullAllowed, Export ("device", ArgumentSemantic.Weak)]
		IGCDevice Device { get; }

		[Abstract]
		[Export ("lastEventTimestamp")]
		double LastEventTimestamp { get; }

		[Abstract]
		[Export ("lastEventLatency")]
		double LastEventLatency { get; }

		// Issue with GCPhysicalInputElementCollection found here: https://github.com/xamarin/xamarin-macios/issues/15725
		// [Abstract]
		// [Export ("elements")]
		// GCPhysicalInputElementCollection<IGCPhysicalInputElementName, IGCPhysicalInputElement> Elements { get; }

		// Issue with GCPhysicalInputElementCollection found here: https://github.com/xamarin/xamarin-macios/issues/15725
		// [Abstract]
		// [Export ("buttons")]
		// GCPhysicalInputElementCollection<IGCButtonElementName, IGCButtonElement> Buttons { get; }

		// Issue with GCPhysicalInputElementCollection found here: https://github.com/xamarin/xamarin-macios/issues/15725
		// [Abstract]
		// [Export ("axes")]
		// GCPhysicalInputElementCollection<IGCAxisElementName, IGCAxisElement> Axes { get; }

		// Issue with GCPhysicalInputElementCollection found here: https://github.com/xamarin/xamarin-macios/issues/15725
		// [Abstract]
		// [Export ("switches")]
		// GCPhysicalInputElementCollection<IGCSwitchElementName, IGCSwitchElement> Switches { get; }

		// Issue with GCPhysicalInputElementCollection found here: https://github.com/xamarin/xamarin-macios/issues/15725
		// [Abstract]
		// [Export ("dpads")]
		// GCPhysicalInputElementCollection<IGCDirectionPadElementName, IGCDirectionPadElement> Dpads { get; }

		[Abstract]
		[Export ("objectForKeyedSubscript:")]
		[return: NullAllowed]
		IGCPhysicalInputElement GetObject (string key);
	}

	interface IGCAxisInput { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCAxisInput {
		[Abstract]
		[NullAllowed, Export ("valueDidChangeHandler", ArgumentSemantic.Copy)]
		Action<IGCPhysicalInputElement, IGCAxisInput, float> ValueDidChangeHandler { get; set; }

		[Abstract]
		[Export ("value")]
		float Value { get; }

		[Abstract]
		[Export ("analog")]
		bool Analog { [Bind ("isAnalog")] get; }

		[Abstract]
		[Export ("canWrap")]
		bool CanWrap { get; }

		[Abstract]
		[Export ("lastValueTimestamp")]
		double LastValueTimestamp { get; }

		[Abstract]
		[Export ("lastValueLatency")]
		double LastValueLatency { get; }
	}

	interface IGCAxisElement { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCAxisElement : GCPhysicalInputElement {
		[Abstract]
		[NullAllowed, Export ("absoluteInput")]
		IGCAxisInput AbsoluteInput { get; }

		[Abstract]
		[Export ("relativeInput")]
		IGCRelativeInput RelativeInput { get; }
	}

	interface IGCButtonElement { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCButtonElement : GCPhysicalInputElement {
		[Abstract]
		[Export ("pressedInput")]
		NSObject PressedInput { get; }

		[Abstract]
		[NullAllowed, Export ("touchedInput")]
		IGCTouchedStateInput TouchedInput { get; }
	}

	delegate void ElementValueDidChangeHandler (IGCDevicePhysicalInput physicalInput, IGCPhysicalInputElement element);
	delegate void InputStateAvailableHandler (IGCDevicePhysicalInput physicalInput);

	interface IGCDevicePhysicalInput { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCDevicePhysicalInput : GCDevicePhysicalInputState {
#if !XAMCORE_5_0
#pragma warning disable 0108 // warning CS0108: 'GCDevicePhysicalInput.Device' hides inherited member 'GCDevicePhysicalInputState.Device'. Use the new keyword if hiding was intended.
		[Abstract]
		[NullAllowed, Export ("device", ArgumentSemantic.Weak)]
		IGCDevice Device { get; }
#pragma warning restore
#endif

		[Abstract]
		[NullAllowed, Export ("elementValueDidChangeHandler", ArgumentSemantic.Copy)]
		ElementValueDidChangeHandler ElementValueDidChangeHandler { get; set; }

		[Abstract]
		[Export ("capture")]
		IGCDevicePhysicalInputState Capture { get; }

		[Abstract]
		[NullAllowed, Export ("inputStateAvailableHandler", ArgumentSemantic.Copy)]
		InputStateAvailableHandler InputStateAvailableHandler { get; set; }

		[Abstract]
		[Export ("inputStateQueueDepth")]
		nint InputStateQueueDepth { get; set; }

		[Abstract]
		[NullAllowed, Export ("nextInputState")]
		NSObject NextInputState { get; }
	}

	interface IGCDevicePhysicalInputStateDiff { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCDevicePhysicalInputStateDiff {
		[Abstract]
		[Export ("changeForElement:")]
		GCDevicePhysicalInputElementChange GetChange (IGCPhysicalInputElement element);

		[Abstract]
		[NullAllowed, Export ("changedElements")]
		NSEnumerator<IGCPhysicalInputElement> ChangedElements { get; }
	}

	interface IGCDirectionPadElement { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCDirectionPadElement : GCPhysicalInputElement {
		[Abstract]
		[Export ("xAxis")]
		IGCAxisInput XAxis { get; }

		[Abstract]
		[Export ("yAxis")]
		IGCAxisInput YAxis { get; }

		[Abstract]
		[Export ("up")]
		NSObject Up { get; }

		[Abstract]
		[Export ("down")]
		NSObject Down { get; }

		[Abstract]
		[Export ("left")]
		NSObject Left { get; }

		[Abstract]
		[Export ("right")]
		NSObject Right { get; }
	}

	interface IGCLinearInput { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCLinearInput {
		[Abstract]
		[NullAllowed, Export ("valueDidChangeHandler", ArgumentSemantic.Copy)]
		Action<IGCPhysicalInputElement, IGCLinearInput, float> ValueDidChangeHandler { get; set; }

		[Abstract]
		[Export ("value")]
		float Value { get; }

		[Abstract]
		[Export ("analog")]
		bool Analog { [Bind ("isAnalog")] get; }

		[Abstract]
		[Export ("canWrap")]
		bool CanWrap { get; }

		[Abstract]
		[Export ("lastValueTimestamp")]
		double LastValueTimestamp { get; }

		[Abstract]
		[Export ("lastValueLatency")]
		double LastValueLatency { get; }
	}

	interface IGCPhysicalInputElement { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCPhysicalInputElement {
		[Abstract]
		[NullAllowed, Export ("sfSymbolsName")]
		string SfSymbolsName { get; }

		[Abstract]
		[NullAllowed, Export ("localizedName")]
		string LocalizedName { get; }

		[Abstract]
		[Export ("aliases")]
		NSSet<NSString> Aliases { get; }
	}

	interface IGCPressedStateInput { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCPressedStateInput {
		[Abstract]
		[NullAllowed, Export ("pressedDidChangeHandler", ArgumentSemantic.Copy)]
		Action<IGCPhysicalInputElement, IGCPressedStateInput, bool> PressedDidChangeHandler { get; set; }

		[Abstract]
		[Export ("pressed")]
		bool Pressed { [Bind ("isPressed")] get; }

		[Abstract]
		[Export ("lastPressedStateTimestamp")]
		double LastPressedStateTimestamp { get; }

		[Abstract]
		[Export ("lastPressedStateLatency")]
		double LastPressedStateLatency { get; }
	}

	interface IGCRelativeInput { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCRelativeInput {
		[Abstract]
		[NullAllowed, Export ("deltaDidChangeHandler", ArgumentSemantic.Copy)]
		Action<IGCPhysicalInputElement, IGCRelativeInput, float> DeltaDidChangeHandler { get; set; }

		[Abstract]
		[Export ("delta")]
		float Delta { get; }

		[Abstract]
		[Export ("analog")]
		bool Analog { [Bind ("isAnalog")] get; }

		[Abstract]
		[Export ("lastDeltaTimestamp")]
		double LastDeltaTimestamp { get; }

		[Abstract]
		[Export ("lastDeltaLatency")]
		double LastDeltaLatency { get; }
	}

	interface IGCSwitchElement { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCSwitchElement : GCPhysicalInputElement {
		[Abstract]
		[Export ("positionInput")]
		IGCSwitchPositionInput PositionInput { get; }
	}

	interface IGCSwitchPositionInput { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCSwitchPositionInput {
		[Abstract]
		[NullAllowed, Export ("positionDidChangeHandler", ArgumentSemantic.Copy)]
		Action<IGCPhysicalInputElement, IGCSwitchPositionInput, nint> PositionDidChangeHandler { get; set; }

		[Abstract]
		[Export ("position")]
		nint Position { get; }

		[Abstract]
		[Export ("positionRange")]
		NSRange PositionRange { get; }

		[Abstract]
		[Export ("sequential")]
		bool Sequential { [Bind ("isSequential")] get; }

		[Abstract]
		[Export ("canWrap")]
		bool CanWrap { get; }

		[Abstract]
		[Export ("lastPositionTimestamp")]
		double LastPositionTimestamp { get; }

		[Abstract]
		[Export ("lastPositionLatency")]
		double LastPositionLatency { get; }
	}

	interface IGCTouchedStateInput { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
	[Protocol]
	interface GCTouchedStateInput {
		[Abstract]
		[NullAllowed, Export ("touchedDidChangeHandler", ArgumentSemantic.Copy)]
		Action<IGCPhysicalInputElement, IGCTouchedStateInput, bool> TouchedDidChangeHandler { get; set; }

		[Abstract]
		[Export ("touched")]
		bool Touched { [Bind ("isTouched")] get; }

		[Abstract]
		[Export ("lastTouchedStateTimestamp")]
		double LastTouchedStateTimestamp { get; }

		[Abstract]
		[Export ("lastTouchedStateLatency")]
		double LastTouchedStateLatency { get; }
	}

	[NoiOS, Mac (13, 0), NoWatch, NoTV, MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GCGearShifterElement : GCPhysicalInputElement {
		[NullAllowed, Export ("patternInput")]
		IGCSwitchPositionInput PatternInput { get; }

		[NullAllowed, Export ("sequentialInput")]
		IGCRelativeInput SequentialInput { get; }
	}

	[Static]
	[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch]
	interface GCControllerUserCustomizations {
		[Notification, Field ("GCControllerUserCustomizationsDidChangeNotification")]
		NSString DidChangeNotification { get; }
	}
}
