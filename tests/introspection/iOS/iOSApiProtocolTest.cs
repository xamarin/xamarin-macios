//
// Test the generated API for common protocol support
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc.
//

using System;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using UIKit;
#if !__TVOS__
using WatchConnectivity;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MonoTouch.WatchConnectivity;
#endif
using NUnit.Framework;

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class iOSApiProtocolTest : ApiProtocolTest {

		public iOSApiProtocolTest ()
		{
			ContinueOnFailure = true;
			// LogProgress = true;
		}

		protected override bool Skip (Type type)
		{
			switch (type.Namespace) {
			case "MetalKit":
			case "MonoTouch.MetalKit":
			case "MetalPerformanceShaders":
			case "MonoTouch.MetalPerformanceShaders":
				if (Runtime.Arch == Arch.SIMULATOR)
					return true;
				break;
			}

			switch (type.Name) {
			// Apple does not ship a PushKit for every arch on some devices :(
			case "PKPushCredentials":
			case "PKPushPayload":
			case "PKPushRegistry":
				if (Runtime.Arch != Arch.DEVICE)
					return true;

				// Requires iOS 8.2 or later in 32-bit mode
				if (!CheckiOSSystemVersion (8, 2) && IntPtr.Size == 4)
					return true;

				break;
			}

			return base.Skip (type);
		}

		protected override bool Skip (Type type, string protocolName)
		{
			// some code cannot be run on the simulator (e.g. missing frameworks)
			switch (type.Namespace) {
			case "MonoTouch.Metal":
			case "Metal":
			case "MonoTouch.CoreAudioKit":
			case "CoreAudioKit":
				// they works with iOS9 beta 4 (but won't work on older simulators)
				if ((Runtime.Arch == Arch.SIMULATOR) && !CheckiOSOrTVOSSystemVersion (9,0))
					return true;
				break;

#if !__TVOS__
			case "WatchConnectivity":
			case "MonoTouch.WatchConnectivity":
				if (!WCSession.IsSupported)
					return true;
				break;
#endif // !__TVOS__
			}

			switch (type.Name) {
			case "CAMetalLayer":
				// that one still does not work with iOS9 beta 4
				if (Runtime.Arch == Arch.SIMULATOR)
					return true;
				break;
#if !XAMCORE_3_0
				// mistake (base type) fixed by a breaking change
			case "MFMailComposeViewControllerDelegate":
				if (protocolName == "UINavigationControllerDelegate")
					return true;
				break;
#endif
				// special case: the Delegate property is id<A,B> so we made A subclass B in managed
				// but this test see the conformance is not correct
			case "UIImagePickerControllerDelegate":
			case "UIVideoEditorControllerDelegate":
				if (protocolName == "UINavigationControllerDelegate")
					return true;
				break;
			}

			switch (protocolName) {
			case "NSCoding":
				switch (type.Name) {
				case "GKPlayer":
				case "GKLocalPlayer":
					// NSSecureCoding is still undocumented, for iOS, and neither is NSCoding for OSX
					// and it did not respond before 6.0 (when NSSecureCoding was introduced)
					return !CheckiOSOrTVOSSystemVersion (6,0);
				case "UITableViewDataSource":
					// this is a *protocol( and we do not want to force people to conform to (an
					// undocumented "requirement") NSCoding - as ObjC do not have to do this
					return true;
				// part of HomeKit are *privately* conforming to NSCoding
				case "HMCharacteristic":
				case "HMCharacteristicMetadata":
				case "HMHome":
				case "HMService":
				case "HMAccessory":
				case "HMActionSet":
				case "HMCharacteristicWriteAction":
				case "HMRoom":
				case "HMServiceGroup":
				case "HMTimerTrigger":
				case "HMTrigger":
				case "HMUser":
				case "HMZone":
				case "HMAccessoryCategory":
				case "HMCharacteristicEvent":
				case "HMEvent":
				case "HMEventTrigger":
				case "HMLocationEvent":
				// new PassKit for payment also *privately* conforms to NSCoding
				case "PKPayment":
				case "PKPaymentSummaryItem":
				case "PKShippingMethod":
				case "PKPaymentRequest":
				case "PKPaymentToken":
				// iOS9
				case "UIFont":
				case "AVAssetTrackSegment":
				case "AVComposition":
				case "AVMutableComposition":
				case "AVCompositionTrackSegment":
				case "MKMapSnapshotOptions":
				case "WCSessionFile":
				case "WCSessionFileTransfer":
				// iOS10
				case "CXCall":
				case "CXCallDirectoryExtensionContext":
				case "CXCallUpdate":
				case "CXProviderConfiguration":
				case "MSMessageTemplateLayout":
				case "MSSession":
				case "SFContentBlockerState":
				case "SFSafariViewControllerConfiguration":
				case "VSAccountMetadata":
				case "VSAccountMetadataRequest":
					return true;
				}
				break;
			case "NSSecureCoding":
				switch (type.Name) {
				// part of HomeKit are *privately* conforming to NSSecureCoding
				case "HMCharacteristic":
				case "HMCharacteristicMetadata":
				case "HMHome":
				case "HMService":
				case "HMAccessory":
				case "HMActionSet":
				case "HMCharacteristicWriteAction":
				case "HMRoom":
				case "HMServiceGroup":
				case "HMTimerTrigger":
				case "HMTrigger":
				case "HMUser":
				case "HMZone":
				case "HMAccessoryCategory":
				case "HMCharacteristicEvent":
				case "HMEvent":
				case "HMEventTrigger":
				case "HMLocationEvent":
					return true;
				// new PassKit for payment also *privately* conforms to NSCoding
				case "PKPayment":
				case "PKPaymentSummaryItem":
				case "PKShippingMethod":
				case "PKPaymentRequest":
				case "PKPaymentToken":
				// iOS9
				case "UIFont":
				case "AVAssetTrackSegment":
				case "AVComposition":
				case "AVMutableComposition":
				case "AVCompositionTrackSegment":
				case "MKMapSnapshotOptions":
				case "NSTextTab":
				case "WCSessionFile":
				case "WCSessionFileTransfer":
				// iOS10
				case "CXCall":
				case "CXCallDirectoryExtensionContext":
				case "CXCallUpdate":
				case "CXProviderConfiguration":
				case "MSMessageTemplateLayout":
				case "MSSession":
				case "SFContentBlockerState":
				case "SFSafariViewControllerConfiguration":
				case "VSAccountMetadata":
				case "VSAccountMetadataRequest":
					return true;
				}
				break;
			case "NSCopying":
				switch (type.Name) {
				// undocumented conformance (up to 7.0) and conformity varies between iOS versions
				case "MKDirectionsRequest":
				case "MPMediaItem":
				case "MPMediaPlaylist":
				case "MPMediaItemCollection":
				case "MPMediaEntity":
					return true; // skip
					// new PassKit for payment also *privately* conforms to NSCoding
				case "PKPaymentSummaryItem":
				case "PKShippingMethod":
					return true; // skip
				// iOS9
				case "ACAccount":
				case "HKCategorySample":
				case "HKCorrelation":
				case "HKObject":
				case "HKQuantitySample":
				case "HKSample":
				case "HKWorkout":
				case "PKPaymentMethod":
				// iOS 10
				case "CXCallDirectoryExtensionContext":
				case "SFSafariViewControllerConfiguration":
				case "VSAccountMetadata":
				case "VSAccountMetadataRequest":
					return true;
				}
				break;
			case "UIAccessibilityIdentification":
				// UIView satisfy the contract - but return false for conformance (and so does all it's subclasses)
				return true;
			case "UIAppearance":
				// we added UIAppearance to some types that do not conform to it
				// note: removing them cause the *Appearance types to be removed too
				switch (type.Name) {
				case "ABPeoplePickerNavigationController":
				case "EKEventEditViewController":
				case "GKAchievementViewController":
				case "GKFriendRequestComposeViewController":
				case "GKLeaderboardViewController":
				case "GKTurnBasedMatchmakerViewController":
				case "MFMailComposeViewController":
				case "MFMessageComposeViewController":
					return true;
				}
				break;
			case "UITextInputTraits":
				// UISearchBar conformance fails before 7.1 - reference bug #33333
				if ((type.Name == "UISearchBar") && !CheckiOSOrTVOSSystemVersion (7,1))
					return true;
				break;
#if !XAMCORE_3_0
			case "UINavigationControllerDelegate":
				switch (type.Name) {
				case "ABPeoplePickerNavigationControllerDelegate": // 37180
					return true;
				}
				break;
#endif
			case "GKSavedGameListener":
				switch (type.Name) {
				case "GKLocalPlayerListener": // 37180
					return !CheckiOSOrTVOSSystemVersion (8, 0);
				}
				break;
			}
			return base.Skip (type, protocolName);
		}

		[Test]
		public override void SecureCoding ()
		{
			if (!CheckiOSOrTVOSSystemVersion (6,0))
				Assert.Inconclusive ("Requires iOS 6+");

			base.SecureCoding ();
		}

		[Test]
		public override void SupportsSecureCoding ()
		{
			if (!CheckiOSOrTVOSSystemVersion (6,0))
				Assert.Inconclusive ("Requires iOS 6+");

			base.SupportsSecureCoding ();
		}
	}
}
