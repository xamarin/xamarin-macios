//
// Test the generated API `init` selectors are usable by developers
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Reflection;
#if !__TVOS__
using PassKit;
#endif
using Foundation;
#if !__WATCHOS__
using Metal;
#endif
using ObjCRuntime;
using UIKit;

using NUnit.Framework;

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class iOSApiCtorInitTest : ApiCtorInitTest {

		public iOSApiCtorInitTest ()
		{
			Class.ThrowOnInitFailure = false;
			ContinueOnFailure = true;
			//LogProgress = true;
		}

		protected override bool Skip (Type type)
		{
			switch (type.Namespace) {
			// all default ctor did not work and were replaced with [Obsolete("",true)] placeholders
			// reflecting on those would create invalid instances (no handle) that crash the app
			case "CoreBluetooth":
			case "MonoTouch.CoreBluetooth":
				return true;

			case "CoreAudioKit":
			case "MonoTouch.CoreAudioKit":
			case "Metal":
			case "MonoTouch.Metal":
				// they works with iOS9 beta 4 (but won't work on older simulators)
				if (TestRuntime.IsSimulatorOrDesktop && !TestRuntime.CheckXcodeVersion (7, 0))
					return true;
				break;
#if !__WATCHOS__
			case "MetalKit":
			case "MonoTouch.MetalKit":
			case "MetalPerformanceShaders":
			case "MonoTouch.MetalPerformanceShaders":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				// some devices don't support metal and that crash some API that does not check that, e.g. #33153
				if (!TestRuntime.CheckXcodeVersion (7, 0) || (MTLDevice.SystemDefault is null))
					return true;
				break;
#endif // !__WATCHOS__
#if __TVOS__
			case "MetalPerformanceShadersGraph":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
#endif // __TVOS__
			case "CoreNFC": // Only available on devices that support NFC, so check if NFCNDEFReaderSession is present.
				if (Class.GetHandle ("NFCNDEFReaderSession") == IntPtr.Zero)
					return true;
				break;
			case "Cinematic":
			case "DeviceCheck": // Only available on device
			case "MLCompute": // Only available on device
			case "PushToTalk":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
			}

			switch (type.Name) {
			// under iOS7 creating this type will crash later (after test execution) with a stack similar to:
			// https://gist.github.com/rolfbjarne/457f78e20c8c31edef5c
			case "EKCalendarChooserDelegate":
			case "EKEventEditViewController":
				return true;

			// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: There can only be one UIApplication instance.
			case "UIApplication":
				return true;
			// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: UISplitViewController is only supported when running under UIUserInterfaceIdiomPad
			case "UISplitViewController":
#if !__WATCHOS__
			// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: ADInterstitialAd is available on iPad only.
			case "ADInterstitialAd":
				return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone;
#endif

			case "UIVideoEditorController":
				return true;
			// shows an alert on the simulator
			case "MFMessageComposeViewController":
				return true;
			// shows an alert on the device (if no email address is configured)
			case "MFMailComposeViewController":
				return true;

#if !__TVOS__
			// PassKit is not available on iPads
			case "PKPassLibrary":
				return !PKPassLibrary.IsAvailable;
#endif // !__TVOS__


			// default ctor started to throw on iOS7 - we should never have exposed it but, for API compatibility,
			// we now have an "empty" obsolete ctor
			case "UIFont":
				return true;

			case "NSUrlSessionConfiguration":
			case "NSUrlSession":
				// This crashes when arc frees this object at the end of the scope:
				// { NSURLSession *var = [[NSURLSession alloc] init]; }
				return true;

			case "GKAchievementViewController":
			case "GKLeaderboardViewController":
				// technically available since 4.1 - however it got a new base class in 6.0
				// and that new base class GKGameCenterViewController did not exists before 6.0
				// which makes the type unusable in 5.x, ref: https://gist.github.com/spouliot/271b6230a3aa2b58bc6e
				return !TestRuntime.CheckXcodeVersion (4, 5);
#if __MACCATALYST__
			// just like macOS the native 'init' method returned nil.
			// Web docs mentions uthenticaing a local player is required and sample shows a null check after creating the vc
			case "GKGameCenterViewController":
				return true;
#endif
			// mistake - we should not have exposed those default ctor and now we must live with them
			case "GCControllerElement":
			case "GCControllerAxisInput":
			case "GCControllerButtonInput":
			case "GCControllerDirectionPad":
			case "GCGamepad":
			case "GCExtendedGamepad":
			case "GCController":
				return true;

			// default constructor are not working on iOS8 so we removed them
			// and can't test them even in earlier iOS versions
			case "JSManagedValue":
			case "MKLocalSearch":
			case "MKTileOverlayRenderer":
			case "AVAssetResourceLoadingDataRequest":
			case "CLBeaconRegion":
			case "NSPersistentStoreCoordinator":
				return true;

			// Metal is not available on the (iOS8) simulator
			case "CAMetalLayer":
				return TestRuntime.IsSimulatorOrDesktop && !TestRuntime.CheckXcodeVersion (11, 0);

			// in 8.2 beta 1 this crash the app (simulator) without giving any details in the logs
			case "WKUserNotificationInterfaceController":
				return true;

			// Both reported in radar #21548819
			// NSUnknownKeyException [<CIDepthOfField 0x158586970> valueForUndefinedKey:]: this class is not key value coding-compliant for the key inputPoint2.
			case "CIDepthOfField":
			// NSUnknownKeyException [<CISunbeamsGenerator 0x1586d0810> valueForUndefinedKey:]: this class is not key value coding-compliant for the key inputCropAmount.
			case "CISunbeamsGenerator":
				return true;

			case "MPMediaItemArtwork":
				// NSInvalidArgumentException Reason: image must be non-nil
				return true;

			// these work only on devices, so we skip the simulator
			case "MTLHeapDescriptor":
			case "MTLSharedEventListener":
				return TestRuntime.IsSimulatorOrDesktop;
#if __WATCHOS__
			// The following watchOS 3.2 Beta 2 types Fails, but they can be created we verified using an ObjC app, we will revisit before stable
			case "INRequestPaymentIntent":
			case "INRequestRideIntent":
			case "INResumeWorkoutIntent":
			case "INRideVehicle":
			case "INSearchCallHistoryIntent":
			case "INSearchForMessagesIntent":
			case "INSearchForPhotosIntent":
			case "INSendMessageIntent":
			case "INSendPaymentIntent":
			case "INStartAudioCallIntent":
			case "INStartPhotoPlaybackIntent":
			case "INStartWorkoutIntent":
			case "CLKComplicationWidgetMigrator": // Only available on device
				return true;
#endif
			// iOS 11 Beta 1
			case "UICollectionViewFocusUpdateContext": // [Assert] -init is not a useful initializer for this class. Use one of the designated initializers instead
			case "UIFocusUpdateContext": // [Assert] -init is not a useful initializer for this class. Use one of the designated initializers instead
			case "EKCalendarItem": // Fails with NSInvalidArgumentException +[EKCalendarItem frozenClass]: unrecognized selector sent to class, will fill a radar
			case "EKParticipant": // ctor disabled in XAMCORE_3_0
			case "UITableViewFocusUpdateContext": // Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: Invalid parameter not satisfying: focusSystem, will fill a radar
				return true;
			case "INBookRestaurantReservationIntentResponse": // iOS 11 beta 2: stack overflow in description. radar:32945914
				return true;
			case "IOSurface": // Only works on device before Xcode 11
				return !TestRuntime.CheckXcodeVersion (11, 0);
			case "NEHotspotEapSettings": // Wireless Accessory Configuration is not supported in the simulator.
			case "NEHotspotConfigurationManager":
			case "NEHotspotHS20Settings":
				return TestRuntime.IsSimulatorOrDesktop;
			// iOS 12
			case "INGetAvailableRestaurantReservationBookingDefaultsIntentResponse": // Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: Unable to initialize 'INGetAvailableRestaurantReservationBookingDefaultsIntentResponse'. Please make sure that your intent definition file is valid.
			case "INGetAvailableRestaurantReservationBookingsIntentResponse": // Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: Unable to initialize 'INGetAvailableRestaurantReservationBookingsIntentResponse'. Please make sure that your intent definition file is valid.
			case "INGetRestaurantGuestIntentResponse": // Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: Unable to initialize 'INGetRestaurantGuestIntentResponse'. Please make sure that your intent definition file is valid.
				return TestRuntime.CheckXcodeVersion (10, 0);
			case "CMMovementDisorderManager": // Not available in simulator, added info to radar://41110708 
#if __WATCHOS__
				// Doesn't exist in the simulator; aborts on device if the required entitlement isn't available.
				return true;
#endif
				return TestRuntime.IsSimulatorOrDesktop;
			case "RPSystemBroadcastPickerView": // Symbol not available in simulator
				return TestRuntime.IsSimulatorOrDesktop;
			case "ICNotificationManagerConfiguration": // This works on device but not on simulator, and docs explicitly says it is user creatable
				return TestRuntime.IsSimulatorOrDesktop;
			case "VNDocumentCameraViewController": // Name: NSGenericException Reason: Document camera is not available on simulator
				return TestRuntime.IsSimulatorOrDesktop;
			case "AVAudioRecorder": // Stopped working with Xcode 11.2 beta 2
				return TestRuntime.CheckXcodeVersion (11, 2);
			case "UIMenuController": // Stopped working with Xcode 11.3 beta 1
				return TestRuntime.CheckXcodeVersion (11, 3);
			case "THClient":
				return TestRuntime.IsSimulatorOrDesktop;
#if __TVOS__
			case "MPSPredicate":
				// the device .ctor ends up calling `initWithBuffer:offset:` and crash on older (non 4k AppleTV devices)
				// MPSPredicate.mm:102: failed assertion `[MPSPredicate initWithBuffer:offset:] device: Apple A8 GPU does not support predication.'
				return (TestRuntime.IsDevice && (UIScreen.MainScreen.NativeBounds.Width <= 1920));
#endif
#if __TVOS__ || __WATCHOS__
			case "NSMetadataQuery":
				// hangs on xcode 13 beta 1 on simulator
				if (TestRuntime.CheckXcodeVersion (13, 0))
					return true;
				break;
#endif
			}
			return base.Skip (type);
		}

		protected override bool SkipCheckShouldReExposeBaseCtor (Type type)
		{
			switch (type.Name) {
			case "SWRemoveParticipantAlertController":
				return true;
			default:
				return false;
			}

			return base.SkipCheckShouldReExposeBaseCtor (type);
		}

		static List<NSObject> do_not_dispose = new List<NSObject> ();

		protected override void Dispose (NSObject obj, Type type)
		{
			switch (type.Name) {
			// this crash the application after test completed their execution so we keep it alive
			case "AVAudioRecorder":
			case "AVCaptureConnection":
			case "GKFriendRequestComposeViewController":
			case "SKView":
			// NSInvalidArgumentException *** -[__NSDictionaryM removeObjectForKey:]: key cannot be nil
			case "SKTextureAtlas":
			// fails under iOS5 with NSInvalidArgumentException Reason: -[__NSCFDictionary removeObjectForKey:]: attempt to remove nil key
			case "NSBundle":
			case "NSUrlConnection": // crash too (only on iOS5)
									// iOS8 beta 5 -> SIGABRT (only on devices)
			case "CABTMidiCentralViewController":
			case "CABTMidiLocalPeripheralViewController":
				do_not_dispose.Add (obj);
				break;
			// iOS 9 beta 1 - crash when disposed
			case "MidiNetworkConnection":
			case "WKNavigation":
			case "CIImageAccumulator":
			case "NEAppProxyTcpFlow":
			case "NEAppProxyUdpFlow":
				do_not_dispose.Add (obj);
				break;
			// iOS 10 beta 1 - crash when disposed
			case "CLBeacon":
				do_not_dispose.Add (obj);
				break;
			default:
				base.Dispose (obj, type);
				break;
			}
		}

		protected override void CheckHandle (NSObject obj)
		{
			bool result = obj.Handle != IntPtr.Zero;
			if (!result) {
				string name = obj.GetType ().Name;
				switch (name) {
				// FIXME: it's not clear what's the alternative to 'init' and it could be because I have no phone device
				case "CTCallCenter":
				case "CTTelephonyNetworkInfo":
					return;
				// to avoid crashes we do not really create (natively) default instances (iOS gives them to us) 
				// for compatibility purpose - we should never had included the default .ctor in monotouch.dll
				case "CAMediaTimingFunction":
				case "CLHeading":
				case "CLRegion":
				case "CLPlacemark":
				case "CMAccelerometerData":
				case "CMLogItem":
				case "CMAttitude":
				case "CMDeviceMotion":
				case "CMGyroData":
				case "CMMagnetometerData":
					return;
				// under iOS5 only - MPMediaPickerController: Unable to access iPod library.
				case "MPMediaPickerController":
					return;
				// re-enabled as an [Obsolete ("", true)] but it will crash if we create it (which we can since we use reflection)
				case "NSTimer":
				case "NSCompoundPredicate":
					return;
				// iOS9 - the instance was "kind of valid" before
				case "PKPaymentAuthorizationViewController":
					if (TestRuntime.CheckXcodeVersion (7, 0))
						return;
					break;
				// iOS 12
				case "CIAztecCodeGenerator":
				case "CIBarcodeGenerator":
				case "CICode128BarcodeGenerator":
				case "CIPdf417BarcodeGenerator":
				case "CIQRCodeGenerator":
					if (TestRuntime.CheckXcodeVersion (10, 0))
						return;
					break;
				}
				base.CheckHandle (obj);
			}
		}

		protected override void CheckToString (NSObject obj)
		{
			string name = obj.GetType ().Name;
			switch (name) {
			// crash at at MonoTouch.Foundation.NSObject.get_Description () [0x0000b] in /mono/ios/monotouch-ios7/monotouch/src/Foundation/NSObject.g.cs:500
			case "SKTexture":
			case "MCSession":
			// crash at at MonoTouch.Foundation.NSObject.get_Description () [0x0000b] in /Developer/MonoTouch/Source/monotouch/src/Foundation/NSObject.g.cs:554
			case "AVPlayerItemTrack":
			case "AVCaptureConnection":
				return;
			// worked before ios6.0 beta 1
			case "AVComposition":
			// new API in iOS7 
			case "AVAssetResourceLoadingDataRequest":
			// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: Unable to create description in descriptionForLayoutAttribute_layoutItem_coefficient. Something is nil
			case "NSLayoutConstraint":
			// new in 6.0
			case "AVAssetResourceLoadingRequest":
			case "GKScoreChallenge": // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[GKScoreChallenge challengeID]: unrecognized selector sent to instance 0x18acc340
			case "GKAchievementChallenge": // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[GKAchievementChallenge challengeID]: unrecognized selector sent to instance 0x160f4840
				if (TestRuntime.CheckXcodeVersion (4, 5))
					return;
				break;
			// iOS 9 beta 1 - crash when called
			case "WKFrameInfo":
			case "WKNavigation":
			case "WKNavigationAction":
				if (TestRuntime.CheckXcodeVersion (7, 0))
					return;
				break;
			// new iOS 10 beta 1 - crash when calling `description` selector
			case "AVAudioSessionDataSourceDescription":
			case "AVAudioSessionPortDescription":
			case "CLBeacon":
			case "CLCircularRegion":
			// beta 2
			case "CTCallCenter":
			// beta 3
			case "CNContainer":
				if (TestRuntime.CheckXcodeVersion (8, 0))
					return;
				break;
			// Xcode 9 Beta 1 to avoid crashes
			case "CIImageAccumulator":
				if (TestRuntime.CheckXcodeVersion (9, 0))
					return;
				break;
			// crash with xcode 12 beta 2
			case "AVMediaSelection":
			case "AVMutableMediaSelection":
			// crash with xcode 12 beta 3
			case "GKTurnBasedMatch":
			// crash with xcode 12 GM
			case "CSLocalizedString":
				if (TestRuntime.CheckXcodeVersion (12, 0))
					return;
				break;
			case "IOSurface": // crash with Xcode 14 beta 1
				if (TestRuntime.CheckXcodeVersion (14, 0))
					return;
				break;
			default:
				base.CheckToString (obj);
				break;
			}
		}


		protected override void CheckNSObjectProtocol (NSObject obj)
		{
			switch (obj.GetType ().Name) {
			case "NSString":
				// according to bots `isKindOf (null)` returns true before iOS 8, ref: #36726
				if (!TestRuntime.CheckXcodeVersion (6, 0))
					return;
				break;
			}
			base.CheckNSObjectProtocol (obj);
		}

		// notes:
		// * Splitview controller <UISplitViewController: 0xda106e0> is expected to have a view controller at index 0 before it's used!
		//	this happens when we dispose an empty UISplitViewController, harmless
	}
}
