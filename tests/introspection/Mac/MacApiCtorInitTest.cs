//
// Mac-specific `init*` selectors validations
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013,2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Reflection;

#if XAMCORE_2_0
using Foundation;
#else
using MonoMac.Foundation;
#endif

using NUnit.Framework;
using Xamarin.Tests;

namespace Introspection {

	[TestFixture]
	public class MacApiCtorInitTest : ApiCtorInitTest {

		public MacApiCtorInitTest ()
		{
			//LogProgress = true;
			ContinueOnFailure = true;
		}

		protected override bool Skip (Attribute attr)
		{
			return base.Skip (attr);
		}

		protected override bool Skip (Type type)
		{
			switch (type.FullName) {
#if !XAMCORE_4_0
			case "AppKit.NSDraggingInfo":
			case "MonoMac.AppKit.NSDraggingInfo": // binding mistakes.
				return true;
#endif
			// Random failures on build machine
			case "QuickLookUI.QLPreviewPanel":
			case "MonoMac.QuickLookUI.QLPreviewPanel":
				return true;
			// These should be DisableDefaultCtor but can't due to backward compat
			case "MonoMac.AppKit.NSCollectionViewTransitionLayout":
			case "AppKit.NSCollectionViewTransitionLayout":
			case "Foundation.NSUnitDispersion": // -init should never be called on NSUnit!
			case "Foundation.NSUnitVolume": // -init should never be called on NSUnit!
			case "Foundation.NSUnitDuration": // -init should never be called on NSUnit!
			case "Foundation.NSUnitElectricCharge": // -init should never be called on NSUnit!
			case "Foundation.NSUnitElectricCurrent": // -init should never be called on NSUnit!
			case "Foundation.NSUnitElectricPotentialDifference": // -init should never be called on NSUnit!
			case "Foundation.NSUnitElectricResistance": // -init should never be called on NSUnit!
			case "Foundation.NSUnit": // -init should never be called on NSUnit!
			case "Foundation.NSUnitEnergy": // -init should never be called on NSUnit!
			case "Foundation.NSUnitAcceleration": // -init should never be called on NSUnit!
			case "Foundation.NSUnitFrequency": // -init should never be called on NSUnit!
			case "Foundation.NSUnitAngle": // -init should never be called on NSUnit!
			case "Foundation.NSUnitFuelEfficiency": // -init should never be called on NSUnit!
			case "Foundation.NSUnitArea": // -init should never be called on NSUnit!
			case "Foundation.NSUnitIlluminance": // -init should never be called on NSUnit!
			case "Foundation.NSUnitConcentrationMass": // -init should never be called on NSUnit!
			case "Foundation.NSUnitLength": // -init should never be called on NSUnit!
			case "Foundation.NSUnitMass": // -init should never be called on NSUnit!
			case "Foundation.NSUnitPower": // -init should never be called on NSUnit!
			case "Foundation.NSUnitPressure": // -init should never be called on NSUnit!
			case "Foundation.NSUnitSpeed": // -init should never be called on NSUnit!
			case "MonoMac.EventKit.EKParticipant":
			case "EventKit.EKParticipant":
			case "XamCore.CoreImage.CISampler":
			case "CoreImage.CISampler":
				return true;
			// OSX 10.8+
			case "MonoMac.AppKit.NSSharingService":
			case "AppKit.NSSharingService":
			case "MonoMac.AppKit.NSSharingServicePicker":
			case "AppKit.NSSharingServicePicker":
			case "MonoMac.Foundation.NSUserNotification":
			case "Foundation.NSUserNotification":
			case "MonoMac.Foundation.NSUserNotificationCenter":
			case "Foundation.NSUserNotificationCenter":
			case "MonoMac.AVFoundation.AVPlayerItemVideoOutput":
			case "AVFoundation.AVPlayerItemVideoOutput":
			case "MonoMac.Foundation.NSUuid":
			case "Foundation.NSUuid":
				if (!Mac.CheckSystemVersion (10, 8))
					return true;
				break;
			// Native exception coming from [NSWindow init] which calls
			// [NSWindow initWithContentRect:styleMask:backing:defer]
			case "MonoMac.AppKit.NSWindow":
			case "AppKit.NSWindow":
				return true;
#if !XAMCORE_2_0
			case "MonoMac.AppKit.NSToolbar":   // mono[10518:626783] *** -[__NSDictionaryM removeObjectForKey:]: key cannot be nil
			case "MonoMac.SceneKit.SCNRenderer": // -[SCNRenderer init]: unrecognized selector sent to instance 0x7c6446c0
			case "MonoMac.SceneKit.SCNLookAtConstraint":
				return true;
#endif
			case "MonoMac.Foundation.NSUrlSession":
			case "Foundation.NSUrlSession":
			case "MonoMac.Foundation.NSUrlSessionTask":
			case "Foundation.NSUrlSessionTask":
			case "MonoMac.Foundation.NSUrlSessionDataTask":
			case "Foundation.NSUrlSessionDataTask":
			case "MonoMac.Foundation.NSUrlSessionUploadTask":
			case "Foundation.NSUrlSessionUploadTask":
			case "MonoMac.Foundation.NSUrlSessionDownloadTask":
			case "Foundation.NSUrlSessionDownloadTask":
			case "MonoMac.Foundation.NSUrlSessionConfiguration":
			case "Foundation.NSUrlSessionConfiguration":
				// These types were introduced as 64-bit only in Mavericks, and 32+64bits in Yosemite. We can't
				// express that with our AvailabilityAttribute, we set it as available (for all architectures, since
				// we can't distinguish them) starting with Mavericks.
				if (Mac.Is32BitMavericks)
					return true;
				break;

			case "GLKit.GLKSkyboxEffect":
				// Crashes inside libGL.dylib, most likely because something hasn't been initialized yet, because
				// I can reproduce in an Xcode project if I put [[GLKSkyboxEffect alloc] init]; in main, but it doesn't
				// crash in applicationDidFinishLaunching.
				//
				//  frame #0: 0x00007fff8d570db1 libGL.dylib`glGetError + 13
				//  frame #1: 0x0000000100025542 GLKit`-[GLKEffect initWithPropertyArray:] + 1142
				//  frame #2: 0x0000000100022c2d GLKit`-[GLKSkyboxEffect init] + 493
				//
				if (IntPtr.Size == 8)
					return true;
				break;

#if !XAMCORE_3_0
			case "SpriteKit.SKView":
				// Causes a crash later. Filed as radar://18440271.
				// Apple said they won't fix this ('init' isn't a designated initializer)
				if (IntPtr.Size == 8)
					return true;
				break;
#endif

			case "MonoMac.AppKit.NSSpeechRecognizer":
			case "AppKit.NSSpeechRecognizer":
				// Makes OSX put up "a download is required for speech recognition" dialog.
				return true;

			case "MonoMac.Foundation.NSUserActivity":
			case "Foundation.NSUserActivity":
				// Crashes by default:
				// Terminating app due to uncaught exception 'NSInvalidArgumentException', reason: 'Caller did not provide an activityType, and this process does not have a NSUserActivityTypes in its Info.plist.
				// but since it looks like the constructor is usable with the proper Info.plist, we can't remove it.
				return true;
			case "MonoMac.AppKit.NSTextTableBlock":
			case "AppKit.NSTextTableBlock":
			case "MonoMac.AppKit.NSMutableFontCollection":
			case "AppKit.NSMutableFontCollection":
				return true; // Crashes in 10.12
			case "CoreBluetooth.CBCentralManager":
			case "MonoMac.CoreBluetooth.CBCentralManager":
				if (IntPtr.Size == 4 && Mac.CheckSystemVersion (10, 13)) // 32-bit removed unannounced in 10.13
					return true;
				break;
			}

			switch (type.Namespace) {
			// OSX 10.8+
			case "MonoMac.Accounts":
			case "Accounts":
			case "MonoMac.GameKit":
			case "GameKit":
			case "MonoMac.Social":
			case "Social":
			case "MonoMac.StoreKit":
			case "StoreKit":
				if (!Mac.CheckSystemVersion (10, 8))
					return true;
				break;
			case "SceneKit":
			case "MonoMac.SceneKit":
				if (!Mac.CheckSystemVersion (10, 8) || IntPtr.Size != 8)
					return true;
				break;
			case "MediaPlayer":
			case "MonoMac.MediaPlayer":
				if (!Mac.CheckSystemVersion (10, 12) || IntPtr.Size != 8)
					return true;
				break;
			}

			return base.Skip (type);
		}

		protected override bool Match (ConstructorInfo ctor, Type type)
		{
			switch (type.FullName) {
			case "ScriptingBridge.SBElementArray":
				// Adding a [DesignatedInitializer] on NSMutableArray triggers:
				// 	[FAIL] ScriptingBridge.SBElementArray should re-expose NSMutableArray::.ctor()
				// but the default constructor is disable for a good reason, i.e. assert at runtime
				//	-[SBElementArray init]: should never be used.
				if (ctor.ToString () == "Void .ctor()")
					return true;
				break;
			}
			return base.Match (ctor, type);
		}

		protected override void CheckNSObjectProtocol (NSObject obj)
		{
			switch (obj.GetType ().Name) {
			case "NSString":
				// according to bots `isKindOf (null)` returns true before Yosemite
				break;
			case "SBObject":
				// *** NSForwarding: warning: object 0x77a49a0 of class '__NSMessageBuilder' does not implement doesNotRecognizeSelector: -- abort
				break;
			default:
				base.CheckNSObjectProtocol (obj);
				break;
			}
		}

		protected override void CheckToString (NSObject obj)
		{
			switch (obj.GetType ().FullName) {
			// Crashes on 10.13
			case "MonoMac.AppKit.NSStoryboard":
			case "AppKit.NSStoryboard":
			case "MonoMac.AVFoundation.AVCaptureInputPort": // https://bugzilla.xamarin.com/show_bug.cgi?id=57668
			case "AVFoundation.AVCaptureInputPort": // https://bugzilla.xamarin.com/show_bug.cgi?id=57668
			// Crashes on 10.12
			case "Contacts.CNContainer":
			// native crash calling MonoMac.Foundation.NSObject.get_Description ()
			case "WebKit.WKNavigationAction":
			case "WebKit.WKFrameInfo": //  EXC_BAD_ACCESS (code=1, address=0x0)
			case "MonoMac.Foundation.NSUrlConnection":
			case "Foundation.NSUrlConnection":
			case "MonoMac.AppKit.NSLayoutConstraint": // Unable to create description in descriptionForLayoutAttribute_layoutItem_coefficient. Something is nil
			case "AppKit.NSLayoutConstraint":
			case "MonoMac.AVFoundation.AVPlayerItemTrack":
			case "AVFoundation.AVPlayerItemTrack":
			// 10.8
			case "MonoMac.AVFoundation.AVComposition":
			case "AVFoundation.AVComposition":
			case "MonoMac.GameKit.GKPlayer": // Crashing on 10.8.3 from the Apple beta channel for abock (on 2013-01-30)
			case "GameKit.GKPlayer":
			case "MonoMac.AVFoundation.AVAssetResourceLoadingRequest": // Crashing on 10.9.1 for abock (2014-01-13)
			case "AVFoundation.AVAssetResourceLoadingRequest":
			case "MonoMac.AVFoundation.AVAssetResourceLoadingDataRequest": // Crashes on 10.9.3 for chamons (constructor found in AVCompat)
			case "AVFoundation.AVAssetResourceLoadingDataRequest":
			case "MonoMac.AVFoundation.AVCaptureDeviceInputSource": // Crashes on 10.9.5
			case "AVFoundation.AVCaptureDeviceInputSource":
				break;
			default:
				base.CheckToString (obj);
				break;
			}
		}

		static List<NSObject> do_not_dispose = new List<NSObject> ();

		protected override void Dispose (NSObject obj, Type type)
		{
			switch (type.FullName) {
			// FIXME: those crash the application when Dispose is called
			case "MonoMac.AppKit.NSTextInputContext":
			case "AppKit.NSTextInputContext":
				if (Mac.CheckSystemVersion (10, 13))
					goto case "MonoMac.ImageKit.IKScannerDeviceView"; // fallthrough
				goto default;
			case "MonoMac.JavaScriptCore.JSManagedValue":
			case "JavaScriptCore.JSManagedValue":
				// JSManagedValue crashes in Yosemite (b7), but not Mavericks.
				if (!Mac.CheckSystemVersion (10, 10))
					goto default;
				goto case "MonoMac.ImageKit.IKScannerDeviceView"; // fallthrough
			case "MonoMac.ImageKit.IKScannerDeviceView": // 19835
			case "ImageKit.IKScannerDeviceView":
			case "MonoMac.AppKit.NSFontPanel": // *** Terminating app due to uncaught exception 'NSInternalInconsistencyException', reason: 'An instance 0x11491cc00 of class NSButton was deallocated while key value observers were still registered with it.
			case "AppKit.NSFontPanel":
			case "MonoMac.AVFoundation.AVAudioRecorder":						// same on iOS
			case "AVFoundation.AVAudioRecorder":
			case "MonoMac.Foundation.NSUrlConnection":
			case "Foundation.NSUrlConnection":
			// 10.8:
			case "MonoMac.Accounts.ACAccount":									// maybe the default .ctor is not allowed ?
			case "Accounts.ACAccount":
			case "MonoMac.Accounts.ACAccountCredential":
			case "Accounts.ACAccountCredential":
			case "MonoMac.Accounts.ACAccountStore":
			case "Accounts.ACAccountStore":
			case "MonoMac.Accounts.ACAccountType":
			case "Accounts.ACAccountType":
			case "MonoMac.CoreData.NSPersistentStoreCoordinator":
			case "CoreData.NSPersistentStoreCoordinator":
			case "AppKit.NSColorPanel":
			case "MonoMac.AppKit.NSColorPanel":
			case "Foundation.NSFileProviderService":
			case "MonoMac.Foundation.NSFileProviderService":
				do_not_dispose.Add (obj);
				break;
			// 10.11
			case "MonoMac.CoreImage.CIImageAccumulator":
			case "CoreImage.CIImageAccumulator":
			case "WebKit.WKNavigation":
				// crashes on El Capitan (b2) but not before
				if (!Mac.CheckSystemVersion (10, 11))
					goto default;
				do_not_dispose.Add (obj);
				break;
			default:
				base.Dispose (obj, type);
				break;
			}
		}
	}
}
