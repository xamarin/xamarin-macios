//
// gamecontroller.cs: binding for iOS (7+) GameController framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013, 2015 Xamarin Inc.

using System;

using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using OpenTK;
#if !MONOMAC
using XamCore.UIKit;
#endif

namespace XamCore.GameController {

	[Since (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // The GCControllerElement class is never instantiated directly.
	public partial interface GCControllerElement {

		// NOTE: ArgumentSemantic.Weak if ARC, ArgumentSemantic.Assign otherwise;
		// currently MonoTouch is not ARC, neither is Xammac, so go with assign.
		[Export ("collection", ArgumentSemantic.Assign)]
		GCControllerElement Collection { get; }

		[Export ("analog")]
		bool IsAnalog { [Bind ("isAnalog")] get; }
	}

	public delegate void GCControllerAxisValueChangedHandler (GCControllerAxisInput axis, float /* float, not CGFloat */ value);

	[Since (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	public partial interface GCControllerAxisInput {

		[Export ("valueChangedHandler", ArgumentSemantic.Copy)]
		GCControllerAxisValueChangedHandler ValueChangedHandler { get; set; }

		[Export ("value")]
		float Value { get; } /* float, not CGFloat */
	}

	public delegate void GCControllerButtonValueChanged (GCControllerButtonInput button, float /* float, not CGFloat */ buttonValue, bool pressed);

	[Since (7,0), Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	public partial interface GCControllerButtonInput {

		[Export ("setValueChangedHandler:", ArgumentSemantic.Copy)]
		void SetValueChangedHandler (GCControllerButtonValueChanged handler);

		[Export ("value")]
		float Value { get; } /* float, not CGFloat */

		[Export ("pressed")]
		bool IsPressed { [Bind ("isPressed")] get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("setPressedChangedHandler:", ArgumentSemantic.Copy)]
		void SetPressedChangedHandler (GCControllerButtonValueChanged handler);
	}

	public delegate void GCControllerDirectionPadValueChangedHandler (GCControllerDirectionPad dpad, float /* float, not CGFloat */ xValue, float /* float, not CGFloat */ yValue);

	[Since (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCControllerElement))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	public partial interface GCControllerDirectionPad {

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

	public delegate void GCGamepadValueChangedHandler (GCGamepad gamepad, GCControllerElement element);

	[Since (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	public partial interface GCGamepad {

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

	[Since (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCGamepad))]
	[DisableDefaultCtor]
	public partial interface GCGamepadSnapshot {

		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		IntPtr Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		IntPtr Constructor (GCController controller, NSData data);
	}

	public delegate void GCExtendedGamepadValueChangedHandler (GCExtendedGamepad gamepad, GCControllerElement element);

	[Since (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // return nil handle -> only exposed as getter
	public partial interface GCExtendedGamepad {

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

	[Since (7,0)]
	[Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (GCExtendedGamepad))]
	[DisableDefaultCtor]
	public partial interface GCExtendedGamepadSnapshot {

		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		IntPtr Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		IntPtr Constructor (GCController controller, NSData data);
	}

#if !XAMCORE_2_0
	public delegate void GCControllerPausedHandler (GCController controller);
#endif

	[Since (7,0), Mac (10,9, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public partial interface GCController {

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

		[NoMac][NoiOS]
		[NullAllowed, Export ("microGamepad", ArgumentSemantic.Retain)]
		GCMicroGamepad MicroGamepad { get; }

		[Static, Export ("controllers")]
		GCController [] Controllers { get; }

		[Static, Export ("startWirelessControllerDiscoveryWithCompletionHandler:")]
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
	public partial interface GCMotion {

		[Export ("controller", ArgumentSemantic.Assign)]
		GCController Controller { get; }

		[Export ("setValueChangedHandler:", ArgumentSemantic.Copy)]
		void SetValueChangedHandler (Action<GCMotion> handler);

		[Export ("gravity", ArgumentSemantic.Assign)]
		Vector3d Gravity { get; }

		[Export ("userAcceleration", ArgumentSemantic.Assign)]
		Vector3d UserAcceleration { get; }

		[NoTV] // Xcode 7.2
		[Export ("attitude", ArgumentSemantic.Assign)]
		Quaterniond Attitude { get; }

		[NoTV] // Xcode 7.2
		[Export ("rotationRate", ArgumentSemantic.Assign)]
		Vector3d RotationRate { get; }
	}

	[NoiOS][NoMac]
	[TV (9,0)]
	public delegate void GCMicroGamepadValueChangedHandler (GCMicroGamepad gamepad, GCControllerElement element);

	[NoiOS][NoMac]
	[TV (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface GCMicroGamepad {
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

	[NoiOS][NoMac]
	[TV (9,0)]
	[BaseType (typeof (GCMicroGamepad))]
	public interface GCMicroGamepadSnapshot {
		[Export ("snapshotData", ArgumentSemantic.Copy)]
		NSData SnapshotData { get; set; }

		[Export ("initWithSnapshotData:")]
		IntPtr Constructor (NSData data);

		[Export ("initWithController:snapshotData:")]
		IntPtr Constructor (GCController controller, NSData data);
	}

#if TVOS
	[NoiOS][NoMac]
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
#endif
}
