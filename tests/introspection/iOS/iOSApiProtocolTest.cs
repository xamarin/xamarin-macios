//
// Test the generated API for common protocol support
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;
using UIKit;
#if HAS_WATCHCONNECTIVITY
using WatchConnectivity;
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
			case "Chip":
			case "MetalKit":
			case "MonoTouch.MetalKit":
			case "MetalPerformanceShaders":
			case "MonoTouch.MetalPerformanceShaders":
			case "MLCompute":
			case "MediaSetup":
			case "Phase":
			case "ThreadNetwork":
			case "PushToTalk":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
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
			}

			switch (type.Name) {
			// Apple does not ship a PushKit for every arch on some devices :(
			case "PKPushCredentials":
			case "PKPushPayload":
			case "PKPushRegistry":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;

				// Requires iOS 8.2 or later in 32-bit mode
				if (!TestRuntime.CheckXcodeVersion (6, 2) && IntPtr.Size == 4)
					return true;

				break;
			case "MTLCounter":
			case "MTLCounterSampleBuffer":
			case "MTLCounterSet":
			case "MTLFence":
			case "MTLHeap":
			case "MTLSharedTextureHandle":
			case "RPSystemBroadcastPickerView": // Symbol not available in simulator
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;

				// Requires iOS 10
				if (!TestRuntime.CheckXcodeVersion (8, 0))
					return true;
				break;
			case "CAMetalLayer":
			case "MTLFunctionConstantValues":
			case "MTLHeapDescriptor":
			case "SWCollaborationActionHandler":
				// Symbol not available in simulator - but works on BigSur (others might too)
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
			case "CMMovementDisorderManager":
				// From Xcode 10 beta 2:
				// This requires a special entitlement:
				//     Usage of CMMovementDisorderManager requires a special entitlement.  Please see for more information https://developer.apple.com/documentation/coremotion/cmmovementdisordermanager
				// but that web page doesn't explain anything (it's mostly empty, so this is probably just lagging documentation)
				// I also tried enabling every entitlement in Xcode, but it still didn't work.
				return true;
#if __WATCHOS__ && !NET
			case "INCarAirCirculationModeResolutionResult":
			case "INCarAudioSourceResolutionResult":
			case "INCarDefrosterResolutionResult":
			case "INCarSeatResolutionResult":
			case "INRadioTypeResolutionResult":
			case "INRelativeSettingResolutionResult":
			case "INRelativeReferenceResolutionResult":
				// These were bound by mistake, and they're gone in NET.
				return true;
#endif
			case "SNClassificationResult": // Class is not being created properly
				return true;
			case "SNClassifySoundRequest": // Class is not being created properly
				return true;
#if __IOS__
			// new types - but marked as iOS 8/9 since they are new base types and
			// we need 32bits bindings to work (generator would not produce them for iOS 13)
			case "CNFetchRequest":
			case "PHChangeRequest":
			case "PHAssetChangeRequest": // subclass of PHChangeRequest
			case "PHAssetCollectionChangeRequest": // subclass of PHChangeRequest
			case "PHAssetCreationRequest": // subclass of PHAssetChangeRequest
			case "PHCollectionListChangeRequest": // subclass of PHChangeRequest
				if (!TestRuntime.CheckXcodeVersion (11, 0))
					return true;
				break;
			case "SWHighlightEvent":
				return TestRuntime.IsSimulatorOrDesktop;

#endif
			case "UILayoutGuideAspectFitting":
			case "UISceneWindowingBehaviors":
				// Symbol not available in simulator - but works on BigSur (others might too)
				if (TestRuntime.IsSimulatorOrDesktop)
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
				if (TestRuntime.IsSimulatorOrDesktop && !TestRuntime.CheckXcodeVersion (7, 0))
					return true;
				break;

#if HAS_WATCHCONNECTIVITY
			case "WatchConnectivity":
			case "MonoTouch.WatchConnectivity":
				if (!WCSession.IsSupported)
					return true;
				break;
#endif // HAS_WATCHCONNECTIVITY
			}

			switch (type.Name) {
			case "CAMetalLayer":
				return TestRuntime.IsSimulatorOrDesktop && !TestRuntime.CheckXcodeVersion (11, 0);
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
#if __IOS__
			case "ARSCNView":
			case "ARSKView":
				if (protocolName == "ARSessionProviding")
					return true;
				break;
#endif
			case "UIAccessibilityElement":
				if (protocolName == "UIResponderStandardEditActions" && !TestRuntime.CheckXcodeVersion (11, 0))
					return true;
				if (protocolName == "UIUserActivityRestoring")
					return true;
				break;
			case "ARImageAnchor":
				// both type and protocol were added in iOS 11.3 but the conformance, for that type, started with iOS 12.0
				if (protocolName == "ARTrackable")
					return !TestRuntime.CheckXcodeVersion (10, 0);
				break;
			case "PHLivePhoto":
				if (protocolName == "NSItemProviderReading")
					return !TestRuntime.CheckXcodeVersion (12, 0);
				break;
#if __MACCATALYST__
			case "BCChatButton":
			case "PKAddPassButton":
			case "UIButton":
			case "UIControl":
			case "UISegmentedControl":
			case "UITextField":
			case "UIDatePicker":
			case "UIPageControl":
			case "UIRefreshControl":
			case "UISearchTextField":
			case "UISlider":
			case "UIStepper":
			case "UISwitch":
			case "ASAuthorizationAppleIdButton":
			case "INUIAddVoiceShortcutButton":
				if (protocolName == "UIContextMenuInteractionDelegate")
					return !TestRuntime.CheckXcodeVersion (12, 0);
				break;

			// We have to special case the following PKPayment* in MacCatalyst
			// since it gets all of these via inheritance from UIView
			case "PKPaymentButton":
			case "PKPaymentAuthorizationViewController":
				switch (protocolName) {
				case "UIUserActivityRestoring":
				case "UIAppearanceContainer":
				case "UIFocusItem":
				case "UICoordinateSpace":
				case "UIPopoverPresentationControllerSourceItem":
				case "UIContextMenuInteractionDelegate":
				case "UIFocusItemContainer":
				case "UITraitEnvironment":
				case "UIActivityItemsConfigurationProviding":
				case "UIResponderStandardEditActions":
				case "UILargeContentViewerItem":
				case "UIDynamicItem":
				case "UIAppearance":
				case "UIAccessibilityContentSizeCategoryImageAdjusting":
				case "UIContentContainer":
					if (TestRuntime.CheckXcodeVersion (14, 0))
						return true;
					break;
				}
				break;
#endif
			}

			switch (protocolName) {
			case "NSCoding":
				switch (type.Name) {
				case "GKPlayer":
				case "GKLocalPlayer":
					// NSSecureCoding is still undocumented, for iOS, and neither is NSCoding for OSX
					// and it did not respond before 6.0 (when NSSecureCoding was introduced)
					return !TestRuntime.CheckXcodeVersion (4, 5);
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
				// iOS 10.3
				case "MPMusicPlayerControllerMutableQueue":
				case "MPMusicPlayerControllerQueue":
					return true;
				// iOS 11.0
				case "UICollectionViewUpdateItem": // Conformance not in headers
				case "MKMapItem": // Conformance not in headers
				case "NSConstraintConflict": // Conformance not in headers
				case "NSQueryGenerationToken": // Conformance not in headers
				case "NSPersistentHistoryToken": // Conformance not in headers
				case "ARCamera":
				case "HMPresenceEvent":
				case "HMMutablePresenceEvent":
				case "HMSignificantTimeEvent":
				case "HMMutableSignificantTimeEvent":
				case "HMCalendarEvent":
				case "HMMutableCalendarEvent":
				case "HMCharacteristicThresholdRangeEvent":
				case "HMMutableCharacteristicThresholdRangeEvent":
				case "HMDurationEvent":
				case "HMMutableDurationEvent":
				case "HMMutableCharacteristicEvent":
				case "HMMutableLocationEvent":
				case "HMTimeEvent":
				case "ILMessageFilterExtensionContext": // Conformance not in headers
				case "MSMessageLiveLayout":
				case "NSFileProviderDomain": // Conformance not in headers
				case "FPUIActionExtensionContext": // Conformance not in headers
				case "UIDocumentBrowserAction": // Conformance not in headers
					return true;
				// iOS 11.3
				case "ARReferenceImage": // Conformance removed from headers in Xcode 9.3 Beta 4
				case "NKAssetDownload":
				case "NKIssue":
					return true;
				// Header shows implementing NSSecureCoding, but supportsSecureCoding returns false.  Radar #34800025
				case "HKSeriesBuilder":
				case "HKWorkoutRouteBuilder":
					return true;
				// Xcode 9.2 undocumented conformance (like due to new base type)
				case "HMHomeAccessControl":
				case "HMAccessControl":
					return true;
				// iOS 12
				case "ARDirectionalLightEstimate":
				case "ARFrame":
				case "ARLightEstimate":
				case "ILClassificationUIExtensionContext": // Conformance not in headers
					return true;
				// iOS 13 beta 1 (to be reviewed)
				case "LAContext":
				case "VNFaceLandmarkRegion2D":
				case "VNFaceLandmarkRegion":
				case "VNFaceLandmarks":
				case "VNFaceLandmarks2D":
				case "MKPolygon":
				case "HMAction":
				case "NSUrlSessionTaskMetrics":
				case "NSUrlSessionTaskTransactionMetrics":
				case "MLMultiArrayConstraint":
				case "MLDictionaryConstraint":
				case "MLFeatureDescription":
				case "MLImageConstraint":
				case "MLImageSize":
				case "MLImageSizeConstraint":
				case "MLModelConfiguration":
				case "MLModelDescription":
				case "MLMultiArrayShapeConstraint":
				case "MLPredictionOptions":
				case "MLSequenceConstraint":
				case "ARSkeleton3D": // Conformance not in headers
				case "ARSkeleton2D": // Conformance not in headers
				case "ARCollaborationData":  // Conformance not in headers
				case "UIFontPickerViewControllerConfiguration": // Conformance not in headers
				case "HKAudiogramSample": // Conformance not in headers
				case "HKAudiogramSensitivityPoint": // Conformance not in headers
				case "HMAccessoryOwnershipToken": // Conformance not in headers
				case "HMAddAccessoryRequest": // Conformance not in headers
				case "OSLogEntry":
				case "OSLogEntryActivity":
				case "OSLogEntryBoundary":
				case "OSLogEntryLog":
				case "OSLogEntrySignpost":
					return true;
#if __WATCHOS__
				case "CLKComplicationTemplate":
				case "CLKComplicationTemplateCircularSmallRingImage":
				case "CLKComplicationTemplateCircularSmallRingText":
				case "CLKComplicationTemplateCircularSmallSimpleImage":
				case "CLKComplicationTemplateCircularSmallSimpleText":
				case "CLKComplicationTemplateCircularSmallStackImage":
				case "CLKComplicationTemplateCircularSmallStackText":
				case "CLKComplicationTemplateModularLargeColumns":
				case "CLKComplicationTemplateModularLargeStandardBody":
				case "CLKComplicationTemplateModularLargeTable":
				case "CLKComplicationTemplateModularLargeTallBody":
				case "CLKComplicationTemplateModularSmallColumnsText":
				case "CLKComplicationTemplateModularSmallRingImage":
				case "CLKComplicationTemplateModularSmallRingText":
				case "CLKComplication":
				case "CLKComplicationTemplateModularSmallSimpleImage":
				case "CLKTextProvider":
				case "CLKComplicationTemplateModularSmallSimpleText":
				case "CLKTimeIntervalTextProvider":
				case "CLKComplicationTemplateModularSmallStackImage":
				case "CLKTimeTextProvider":
				case "CLKComplicationTemplateModularSmallStackText":
				case "CLKComplicationTemplateUtilitarianLargeFlat":
				case "CLKComplicationTemplateUtilitarianSmallFlat":
				case "CLKComplicationTemplateUtilitarianSmallRingImage":
				case "CLKComplicationTemplateUtilitarianSmallRingText":
				case "CLKComplicationTemplateUtilitarianSmallSquare":
				case "CLKComplicationTimelineEntry":
				case "CLKDateTextProvider":
				case "CLKImageProvider":
				case "CLKRelativeDateTextProvider":
				case "CLKSimpleTextProvider":
				case "WKAlertAction":
				// watchOS 3
				case "CLKComplicationTemplateExtraLargeSimpleImage":
				case "CLKComplicationTemplateExtraLargeSimpleText":
				case "CLKComplicationTemplateExtraLargeStackImage":
				case "CLKComplicationTemplateExtraLargeStackText":
				case "CLKComplicationTemplateExtraLargeColumnsText":
				case "CLKComplicationTemplateExtraLargeRingImage":
				case "CLKComplicationTemplateExtraLargeRingText":
				// watchOS 5 / Xcode 10 GM
				case "CLKComplicationTemplateGraphicBezelCircularText":
				case "CLKComplicationTemplateGraphicCircular":
				case "CLKComplicationTemplateGraphicCircularClosedGaugeImage":
				case "CLKComplicationTemplateGraphicCircularClosedGaugeText":
				case "CLKComplicationTemplateGraphicCircularImage":
				case "CLKComplicationTemplateGraphicCircularOpenGaugeImage":
				case "CLKComplicationTemplateGraphicCircularOpenGaugeRangeText":
				case "CLKComplicationTemplateGraphicCircularOpenGaugeSimpleText":
				case "CLKComplicationTemplateGraphicCornerCircularImage":
				case "CLKComplicationTemplateGraphicCornerGaugeImage":
				case "CLKComplicationTemplateGraphicCornerGaugeText":
				case "CLKComplicationTemplateGraphicCornerStackText":
				case "CLKComplicationTemplateGraphicCornerTextImage":
				case "CLKComplicationTemplateGraphicRectangularLargeImage":
				case "CLKComplicationTemplateGraphicRectangularStandardBody":
				case "CLKComplicationTemplateGraphicRectangularTextGauge":
				case "CLKFullColorImageProvider":
				case "CLKGaugeProvider":
				case "CLKSimpleGaugeProvider":
				case "CLKTimeIntervalGaugeProvider":
				// watchOS 6 / Xcode 11
				case "CLKComplicationTemplateGraphicCircularStackImage":
				case "CLKComplicationTemplateGraphicCircularStackText":
					return true;
#elif __TVOS__
				case "TVTopShelfAction":
				case "TVTopShelfCarouselContent":
				case "TVTopShelfCarouselItem":
				case "TVTopShelfInsetContent":
				case "TVTopShelfItem":
				case "TVTopShelfItemCollection":
				case "TVTopShelfNamedAttribute":
				case "TVTopShelfObject":
				case "TVTopShelfSectionedContent":
				case "TVTopShelfSectionedItem":
					return true;
#endif
				// Xcode 12 beta 2
				case "MSServiceAccount":
				case "SRAmbientLightSample":
				case "SRApplicationUsage":
				case "SRDeviceUsageReport":
				case "SRKeyboardMetrics":
				case "SRKeyboardProbabilityMetric`1":
				case "SRMessagesUsageReport":
				case "SRNotificationUsage":
				case "SRPhoneUsageReport":
				case "SRVisit":
				case "SRWebUsage":
					return true;
				// Xcode 12 beta 6
				case "CPButton": // conformance not in headers
				case "CPContactCallButton": // conformance not in headers
				case "CPContactDirectionsButton": // conformance not in headers
				case "CPContactMessageButton": // conformance not in headers
				case "CPMessageListItem": // Conformance not in headers
				case "CPMessageListItemLeadingConfiguration": // conformance not in headers
				case "CPMessageListItemTrailingConfiguration": // conformance not in headers
				case "CPTextButton": // conformance not in headers
				case "CPListImageRowItem": // conformance not in headers
				case "CPListItem": // conformance not in headers
					return true;
				// Xcode 13
				case "ARDepthData":
				case "ARSkeletonDefinition": // device only
				case "ARVideoFormat": // device only
				case "NSMergePolicy":
				case "SFSafariViewControllerPrewarmingToken": // conformance not in headers
				case "SRTextInputSession": // conformance not in headers
				case "UISheetPresentationControllerDetent": // Conformance not in headers
				case "UIWindowSceneActivationConfiguration": // Conformance not in headers
					return true;
				// Xcode 14
				case "VSUserAccount": // Conformance not in headers
				case "PKInk":
					return true;
				// XCode1 5
				case "CKSyncEnginePendingRecordZoneChange":
				case "CKSyncEnginePendingZoneDelete":
				case "CKSyncEnginePendingZoneSave":
				case "CKSyncEngineState":
				case "CKSyncEnginePendingDatabaseChange":
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
				// iOS 10.3
				case "MPMusicPlayerControllerMutableQueue":
				case "MPMusicPlayerControllerQueue":
					return true;
				// iOS 11.0
				case "MKMapItem": // Conformance not in headers
				case "NSQueryGenerationToken": // Conformance not in headers
				case "NSPersistentHistoryToken": // Conformance not in headers
				case "ARCamera":
				case "HMPresenceEvent":
				case "HMMutablePresenceEvent":
				case "HMSignificantTimeEvent":
				case "HMMutableSignificantTimeEvent":
				case "HMCalendarEvent":
				case "HMMutableCalendarEvent":
				case "HMCharacteristicThresholdRangeEvent":
				case "HMMutableCharacteristicThresholdRangeEvent":
				case "HMDurationEvent":
				case "HMMutableDurationEvent":
				case "HMMutableCharacteristicEvent":
				case "HMMutableLocationEvent":
				case "HMTimeEvent":
				case "ILMessageFilterExtensionContext": // Conformance not in headers
				case "NSAttributeDescription":
				case "NSEntityDescription":
				case "NSExpressionDescription":
				case "NSFetchedPropertyDescription":
				case "NSFetchIndexDescription":
				case "NSFetchIndexElementDescription":
				case "NSFetchRequest":
				case "NSManagedObjectModel":
				case "NSPropertyDescription":
				case "NSRelationshipDescription":
				case "MSMessageLiveLayout":
				case "NSFileProviderDomain": // Conformance not in headers
				case "FPUIActionExtensionContext": // Conformance not in headers
				case "UIDocumentBrowserAction": // Conformance not in headers
					return true;
				// iOS 11.3
				case "ARReferenceImage": // Conformance removed from headers in Xcode 9.3 Beta 4
				case "NKAssetDownload":
				case "NKIssue":
					return true;
				// Header shows implementing NSSecureCoding, but supportsSecureCoding returns false.  Radar #34800025
				case "HKSeriesBuilder":
				case "HKWorkoutRouteBuilder":
					return true;
				// Xcode 9.2 undocumented conformance (like due to new base type)
				case "HMHomeAccessControl":
				case "HMAccessControl":
					return true;
				// Xcode 10
				case "ILClassificationUIExtensionContext": // Conformance not in headers
					return true;
				// iOS 13 beta 1 (to be reviewed)
				case "LAContext":
				case "VNFaceLandmarkRegion2D":
				case "VNFaceLandmarkRegion":
				case "VNFaceLandmarks":
				case "VNFaceLandmarks2D":
				case "MKPolygon":
				case "HMAction":
				case "NSLayoutManager":
				case "NSTextContainer":
				case "UISpringTimingParameters":
				case "NSException":
				case "NSUrlSessionTaskMetrics":
				case "NSUrlSessionTaskTransactionMetrics":
				case "MLMultiArrayConstraint":
				case "MLDictionaryConstraint":
				case "MLFeatureDescription":
				case "MLImageConstraint":
				case "MLImageSize":
				case "MLImageSizeConstraint":
				case "MLModelConfiguration":
				case "MLModelDescription":
				case "MLMultiArrayShapeConstraint":
				case "MLPredictionOptions":
				case "MLSequenceConstraint":
				case "ARSkeleton3D": // Conformance not in headers
				case "ARSkeleton2D": // Conformance not in headers
				case "ARCollaborationData":  // Conformance not in headers
				case "UIFontPickerViewControllerConfiguration":  // Conformance not in headers
				case "HKAudiogramSensitivityPoint":  // Conformance not in headers
				case "HMAccessoryOwnershipToken":  // Conformance not in headers
				case "HMAddAccessoryRequest":  // Conformance not in headers
				case "OSLogEntry":
				case "OSLogEntryActivity":
				case "OSLogEntryBoundary":
				case "OSLogEntryLog":
				case "OSLogEntrySignpost":
					return true;
#if __WATCHOS__
				case "CLKComplicationTemplate":
				case "CLKComplicationTemplateCircularSmallRingImage":
				case "CLKComplicationTemplateCircularSmallRingText":
				case "CLKComplicationTemplateCircularSmallSimpleImage":
				case "CLKComplicationTemplateCircularSmallSimpleText":
				case "CLKComplicationTemplateCircularSmallStackImage":
				case "CLKComplicationTemplateCircularSmallStackText":
				case "CLKComplicationTemplateModularLargeColumns":
				case "CLKComplicationTemplateModularLargeStandardBody":
				case "CLKComplicationTemplateModularLargeTable":
				case "CLKComplicationTemplateModularLargeTallBody":
				case "CLKComplicationTemplateModularSmallColumnsText":
				case "CLKComplicationTemplateModularSmallRingImage":
				case "CLKComplicationTemplateModularSmallRingText":
				case "CLKComplicationTemplateModularSmallSimpleImage":
				case "CLKComplicationTemplateModularSmallSimpleText":
				case "CLKComplicationTemplateModularSmallStackImage":
				case "CLKComplicationTemplateModularSmallStackText":
				case "CLKComplicationTemplateUtilitarianLargeFlat":
				case "CLKComplicationTemplateUtilitarianSmallFlat":
				case "CLKComplicationTemplateUtilitarianSmallRingImage":
				case "CLKComplicationTemplateUtilitarianSmallRingText":
				case "CLKComplicationTemplateUtilitarianSmallSquare":
				case "CLKComplicationTimelineEntry":
				case "CLKDateTextProvider":
				case "CLKImageProvider":
				case "CLKRelativeDateTextProvider":
				case "CLKSimpleTextProvider":
				case "CLKTextProvider":
				case "CLKTimeIntervalTextProvider":
				case "CLKTimeTextProvider":
				case "CLKComplication":
				case "WKAlertAction":
				// watchOS 3
				case "CLKComplicationTemplateExtraLargeSimpleImage":
				case "CLKComplicationTemplateExtraLargeSimpleText":
				case "CLKComplicationTemplateExtraLargeStackImage":
				case "CLKComplicationTemplateExtraLargeStackText":
				case "CLKComplicationTemplateExtraLargeColumnsText":
				case "CLKComplicationTemplateExtraLargeRingImage":
				case "CLKComplicationTemplateExtraLargeRingText":
				// watchOS 5 / Xcode 10 GM
				case "CLKComplicationTemplateGraphicBezelCircularText":
				case "CLKComplicationTemplateGraphicCircular":
				case "CLKComplicationTemplateGraphicCircularClosedGaugeImage":
				case "CLKComplicationTemplateGraphicCircularClosedGaugeText":
				case "CLKComplicationTemplateGraphicCircularImage":
				case "CLKComplicationTemplateGraphicCircularOpenGaugeImage":
				case "CLKComplicationTemplateGraphicCircularOpenGaugeRangeText":
				case "CLKComplicationTemplateGraphicCircularOpenGaugeSimpleText":
				case "CLKComplicationTemplateGraphicCornerCircularImage":
				case "CLKComplicationTemplateGraphicCornerGaugeImage":
				case "CLKComplicationTemplateGraphicCornerGaugeText":
				case "CLKComplicationTemplateGraphicCornerStackText":
				case "CLKComplicationTemplateGraphicCornerTextImage":
				case "CLKComplicationTemplateGraphicRectangularLargeImage":
				case "CLKComplicationTemplateGraphicRectangularStandardBody":
				case "CLKComplicationTemplateGraphicRectangularTextGauge":
				case "CLKFullColorImageProvider":
				case "CLKGaugeProvider":
				case "CLKSimpleGaugeProvider":
				case "CLKTimeIntervalGaugeProvider":
				// watchOS 6 / Xcode 11
				case "CLKComplicationTemplateGraphicCircularStackImage":
				case "CLKComplicationTemplateGraphicCircularStackText":
					return true;
#elif __TVOS__
				case "TVTopShelfAction":
				case "TVTopShelfCarouselContent":
				case "TVTopShelfCarouselItem":
				case "TVTopShelfInsetContent":
				case "TVTopShelfItem":
				case "TVTopShelfItemCollection":
				case "TVTopShelfNamedAttribute":
				case "TVTopShelfObject":
				case "TVTopShelfSectionedContent":
				case "TVTopShelfSectionedItem":
					return true;
#endif
				// Xcode 12 beta 2
				case "MSServiceAccount":
				case "SRAmbientLightSample":
				case "SRApplicationUsage":
				case "SRDeviceUsageReport":
				case "SRKeyboardMetrics":
				case "SRKeyboardProbabilityMetric`1":
				case "SRMessagesUsageReport":
				case "SRNotificationUsage":
				case "SRPhoneUsageReport":
				case "SRVisit":
				case "SRWebUsage":
					return true;
				// Xcode 12 beta 6
				case "CPButton": // conformance not in headers
				case "CPContactCallButton": // conformance not in headers
				case "CPContactDirectionsButton": // conformance not in headers
				case "CPContactMessageButton": // conformance not in headers
				case "CPMessageListItemLeadingConfiguration": // conformance not in headers
				case "CPMessageListItemTrailingConfiguration": // conformance not in headers
				case "CPTextButton": // conformance not in headers
				case "CPListImageRowItem": // conformance not in headers
					return true;
				// Xcode 13
				case "ARDepthData":
				case "ARSkeletonDefinition": // device only
				case "ARVideoFormat": // device only
				case "NSMergePolicy":
				case "SFSafariViewControllerPrewarmingToken": // conformance not in headers
				case "SRTextInputSession": // conformance not in headers
				case "UISheetPresentationControllerDetent": // conformance not in headers
				case "CAConstraintLayoutManager": // Not declared in header file
					return true;
				// Xcode 14
				case "VSUserAccount": // Conformance not in headers
				case "PKInk":
					return true;
				// Xcode1 5
				case "NSCompositeAttributeDescription":
				case "CKSyncEnginePendingDatabaseChange":
				case "CKSyncEnginePendingRecordZoneChange":
				case "CKSyncEnginePendingZoneDelete":
				case "CKSyncEnginePendingZoneSave":
				case "CKSyncEngineState":
					return true;
				}
				break;
			case "NSCopying":
				switch (type.Name) {
				// undocumented conformance (up to 7.0) and conformity varies between iOS versions
				case "MKDirectionsRequest":
				case "MPMediaPlaylist":
				case "MPMediaItemCollection":
				case "MPMediaEntity":
					return true; // skip
								 // iOS9
				case "ACAccount":
				case "HKCategorySample":
				case "HKCorrelation":
				case "HKObject":
				case "HKQuantitySample":
				case "HKSample":
				case "HKWorkout":
				// iOS 10
				case "CXCallDirectoryExtensionContext":
				case "HKDocumentSample":
				case "HKCdaDocumentSample":
				case "SFSafariViewControllerConfiguration":
					return true;
				// iOS 11.0
				case "UICollectionViewUpdateItem": // Conformance not in headers
				case "ACAccountCredential": // b2: Conformance not in headers
				case "ILMessageFilterExtensionContext": // b2: Conformance not in headers
				case "HMCharacteristicEvent": // Selectors not available on 32 bit
				case "FPUIActionExtensionContext": // Conformance not in headers
				case "CXCall": // Conformance not in headers
				case "UIDocumentBrowserAction": // Conformance not in headers
					return true;
				// iOS 11.1
				case "ARDirectionalLightEstimate":
					return true;
				// iOS 11.3
				case "WKPreferences": // Conformance not in headers
				case "QLPreviewSceneActivationConfiguration":
					return true;
#if __WATCHOS__
				case "CLKComplicationTimelineEntry":
					return true;
#endif
				// Xcode 10
				case "ILClassificationUIExtensionContext": // Conformance not in headers
					return true;
				// iOS 12.2
				case "PKDisbursementVoucher":
					return true;
				// iOS 13 beta 1 (to be reviewed)
				case "VNFaceLandmarkRegion2D":
				case "VNFaceLandmarkRegion":
				case "VNFaceLandmarks":
				case "VNFaceLandmarks2D":
				case "HMAction":
				case "HMCharacteristicWriteAction":
				case "MLPredictionOptions":
				case "HKCumulativeQuantitySample": // Conformance not in headers
				case "HKDiscreteQuantitySample": // Conformance not in headers
				case "HKAudiogramSample": // Conformance not in headers
				case "UIImage": // only complains on tvOS beta 6
				case "HMAccessoryOwnershipToken":
					return true;
				// Xcode 12 beta 2
				case "HKElectrocardiogram": // Conformance not in headers
					return true;
#if __MACCATALYST__
				// Conformance not in headers
				case "EKCalendar":
				case "EKCalendarItem":
				case "EKEvent":
				case "EKObject":
				case "EKReminder":
				case "EKSource":
					return true;
#endif
				// Xcode 13
				case "PHCloudIdentifier":
				case "PHCloudIdentifierMapping":
				case "NSEntityMapping":
				case "NSMappingModel":
				case "NSMergePolicy":
				case "NSPropertyMapping":
				case "UIWindowSceneActivationConfiguration":
					return true;
				// Xcode 15
				case "CKSyncEnginePendingDatabaseChange":
				case "CKSyncEnginePendingRecordZoneChange":
				case "CKSyncEnginePendingZoneDelete":
				case "CKSyncEnginePendingZoneSave":
					return true;
				}
				break;
			case "NSMutableCopying":
				switch (type.Name) {
				// iOS 10.3
				case "MPMusicPlayerControllerMutableQueue":
				case "MPMusicPlayerControllerQueue":
				// iOS 11
				case "INRideDriver":
				case "INRestaurantGuest":
				case "INPerson":
				case "HMCharacteristicEvent": // Selectors not available on 32 bit
											  // iOS 13 beta 1 (to be reviewed)
				case "UIKeyCommand":
					return true;
#if __MACCATALYST__
				// Conformance not in headers
				case "EKAlarm":
				case "EKCalendar":
				case "EKCalendarItem":
				case "EKEvent":
				case "EKObject":
				case "EKParticipant":
				case "EKRecurrenceRule":
				case "EKReminder":
				case "EKSource":
				case "EKStructuredLocation":
					return true;
#endif
				// Xcode 13
				case "NSMergePolicy":
				case "UNNotificationSettings":
				case "UIWindowSceneActivationConfiguration":
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
				if ((type.Name == "UISearchBar") && !TestRuntime.CheckXcodeVersion (5, 1))
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
					return !TestRuntime.CheckXcodeVersion (6, 0);
				}
				break;

			case "UIFocusEnvironment":
				switch (type.Name) {
				case "SK3DNode":
				case "SKAudioNode":
				case "SKCameraNode":
				case "SKCropNode":
				case "SKEffectNode":
				case "SKEmitterNode":
				case "SKFieldNode":
				case "SKLabelNode":
				case "SKLightNode":
				case "SKNode":
				case "SKReferenceNode":
				case "SKScene":
				case "SKShapeNode":
				case "SKVideoNode":
				case "SKSpriteNode":
					return !TestRuntime.CheckXcodeVersion (8, 0);
				case "SCNNode":
				case "SCNReferenceNode":
					return !TestRuntime.CheckXcodeVersion (9, 0);
				}
				break;

			case "CALayerDelegate": // UIView now conforms to CALayerDelegate in iOS 10
				switch (type.Name) {
				case "UISearchBar":
				case "UISegmentedControl":
				case "UITableView":
				case "UITableViewCell":
				case "UITextField":
				case "UITextView":
				case "UIToolbar":
				case "UIView":
				case "MKPinAnnotationView":
				case "UIImageView":
				case "PHLivePhotoView":
				case "UIInputView":
				case "UILabel":
				case "UIActionSheet":
				case "UIButton":
				case "UICollectionView":
				case "UINavigationBar":
				case "UIControl":
				case "UIPickerView":
				case "UIPageControl":
				case "MPVolumeView":
				case "UIPopoverBackgroundView":
				case "UIProgressView":
				case "UIRefreshControl":
				case "HKActivityRingView":
				case "UIScrollView":
				case "CAInterAppAudioSwitcherView":
				case "CAInterAppAudioTransportView":
				case "UISlider":
				case "UIStackView":
				case "SCNView":
				case "UIStepper":
				case "UISwitch":
				case "UITabBar":
				case "UITableViewHeaderFooterView":
				case "GLKView":
				case "SKView":
				case "MKMapView":
				case "MKAnnotationView":
				case "PKAddPassButton":
				case "PKPaymentButton":
				case "UIActivityIndicatorView":
				case "UICollectionReusableView":
				case "UIWebView":
				case "UICollectionViewCell":
				case "UIWindow":
				case "UIDatePicker":
				case "UIVisualEffectView":
				case "WKWebView":
				case "ADBannerView":
				case "UIAlertView":
					return !TestRuntime.CheckXcodeVersion (8, 0);
				case "MKOverlayView":
				case "MKCircleView":
				case "MKOverlayPathView":
				case "MKPolygonView":
				case "MKPolylineView":
					return !TestRuntime.CheckXcodeVersion (7, 0);
				}
				break;

			case "UIFocusItem": // UIView now conforms to UIFocusItem in iOS 10
				switch (type.Name) {
				case "UIButton":
				case "UICollectionReusableView":
				case "UICollectionView":
				case "UICollectionViewCell":
				case "MKAnnotationView":
				case "UIControl":
				case "MKMapView":
				case "UISearchBar":
				case "UISegmentedControl":
				case "UITableView":
				case "UITableViewCell":
				case "UITextField":
				case "UITextView":
				case "MKPinAnnotationView":
				case "UIView":
				case "SKNode":
				case "SKShapeNode":
				case "SKVideoNode":
				case "UIImageView":
				case "UIInputView":
				case "UILabel":
				case "UINavigationBar":
				case "UIPageControl":
				case "UIPopoverBackgroundView":
				case "UIProgressView":
				case "SCNView":
				case "UIScrollView":
				case "SK3DNode":
				case "MTKView":
				case "SKAudioNode":
				case "SKCameraNode":
				case "SKCropNode":
				case "SKEffectNode":
				case "SKEmitterNode":
				case "SKFieldNode":
				case "SKLabelNode":
				case "SKLightNode":
				case "UIStackView":
				case "UITabBar":
				case "SKReferenceNode":
				case "GLKView":
				case "SKScene":
				case "SKSpriteNode":
				case "SKView":
				case "UITableViewHeaderFooterView":
				case "UIActivityIndicatorView":
				case "UIVisualEffectView":
				case "UIWindow":
					return !TestRuntime.CheckXcodeVersion (8, 0);
				case "SCNNode":
				case "SCNReferenceNode":
					return !TestRuntime.CheckXcodeVersion (9, 0);
				}
				break;

			case "UIContentSizeCategoryAdjusting": // new conformations of UIContentSizeCategoryAdjusting in iOS 10
				switch (type.Name) {
				case "UITextField":
				case "UITextView":
				case "UILabel":
					return !TestRuntime.CheckXcodeVersion (8, 0);
				}
				break;

			case "UISpringLoadedInteractionSupporting": // types do not conform to protocol but protocol methods work on those types (see monotouch-test)
				switch (type.Name) {
				case "UIButton":
				case "UICollectionView":
				case "UISegmentedControl":
				case "UITableView":
				case "UITabBar":
				case "UITabBarItem": // As https://github.com/xamarin/xamarin-macios/commit/e1873169f2d3b2b034d36f844909b6eb4e8abcca points out works as expected on both sim and device
				case "UIAlertController":
				case "PKPaymentButton":
				case "PKAddPassButton":
				case "INUIAddVoiceShortcutButton":
					return true;
				// xcode 12 beta 1
				case "UIBarButtonItem":
				case "MKUserTrackingBarButtonItem":
					return true;
				}
				break;

			case "UIPasteConfigurationSupporting": // types do not conform to protocol but protocol methods work on those types (base type tests in monotouch-test)
				return true; // Skip everything because 'UIResponder' implements 'UIPasteConfigurationSupporting' and that's 130+ types
			case "CAAction":
				switch (type.Name) {
				case "NSNull":
					return !TestRuntime.CheckXcodeVersion (8, 0);
				}
				break;
#if !__WATCHOS__
			// Undocumented conformance (members were inlinded in 'UIViewController' before so all subtypes should conform)
			case "UIStateRestoring":
				return type.Name == "UIViewController" || type.IsSubclassOf (typeof (UIViewController));
#endif
			}
			return base.Skip (type, protocolName);
		}

		[Test]
		public override void SecureCoding ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			base.SecureCoding ();
		}

		[Test]
		public override void SupportsSecureCoding ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			base.SupportsSecureCoding ();
		}
	}
}
