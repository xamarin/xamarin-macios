//
// Test the generated API for common protocol support
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013, 2015 Xamarin Inc.
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;

using Foundation;
using ObjCRuntime;

namespace Introspection {

	public abstract class ApiProtocolTest : ApiBaseTest {

		static IntPtr conform_to = Selector.GetHandle ("conformsToProtocol:");

		public ApiProtocolTest ()
		{
			ContinueOnFailure = true;
		}

		static bool ConformTo (IntPtr klass, IntPtr protocol)
		{
			return bool_objc_msgSend_IntPtr (klass, conform_to, protocol);
		}

		protected virtual bool Skip (Type type)
		{
			switch (type.Namespace) {
			// Xcode 15:
			case "MetalFX":
			case "Cinematic":
				// only present on device :/
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
			}

			switch (type.Name) {
			// *** NSForwarding: warning: object 0x5cbd078 of class 'JSExport' does not implement methodSignatureForSelector: -- trouble ahead
			// *** NSForwarding: warning: object 0x5cbd078 of class 'JSExport' does not implement doesNotRecognizeSelector: -- abort
			case "JSExport":
				return true;
#if !NET
			case "MTLCounter":
			case "MTLCounterSampleBuffer":
			case "MTLCounterSet":
				return true; // Incorrectly bound, will be fixed for .NET.
#endif
			case "MPSImageLaplacianPyramid":
			case "MPSImageLaplacianPyramidSubtract":
			case "MPSImageLaplacianPyramidAdd":
			case "MPSCnnYoloLossNode":
				// The presence of these seem to depend on hardware capabilities, and not only OS version.
				// Unfortunately I couldn't find any documentation related to determining exactly which
				// hardware capability these need (or how to detect them), so just ignore them.
				return true;
			case "CLKComplicationWidgetMigrator":
				// Not present in the simulator, is a migration class
				return true;
			// was removed by apple and is a compat class.
			case "HMMatterRequestHandler":
				return true;
			case "CIFilterGenerator":
				// only present on device :/
				return TestRuntime.IsSimulatorOrDesktop;
			default:
				return SkipDueToAttribute (type);
			}
		}

		IntPtr GetClass (Type type)
		{
			return Class.GetHandle (type);
		}

		protected virtual bool Skip (Type type, string protocolName)
		{
			// The following protocols are skipped in classic since they were added in 
			// later versions
			if (IntPtr.Size == 4) {
				switch (protocolName) {
				case "UIUserActivityRestoring":
					return true;
				}
			}

			switch (protocolName) {
			case "NSCopying":
				switch (type.Name) {
				// undocumented conformance (up to 7.0) and conformity varies between iOS versions
				case "CAEmitterCell":
				case "GKAchievement":
				case "GKScore":
				case "MPMediaItem":
				// new in iOS8 and 10.0
				case "NSExtensionContext":
				case "NSLayoutAnchor`1":
				case "NSLayoutDimension":
				case "NSLayoutXAxisAnchor":
				case "NSLayoutYAxisAnchor":
				// iOS 10 beta 3
				case "GKCloudPlayer":
				// iOS 10 : test throw because of generic usage
				case "NSMeasurement`1":
				// Xcode 9 - Conformance not in headers
				case "MLDictionaryConstraint":
				case "MLImageConstraint":
				case "MLMultiArrayConstraint":
				case "VSSubscription":
					return true; // skip
								 // xcode 10
				case "VSAccountMetadata":
				case "VSAccountMetadataRequest":
				case "VSAccountProviderResponse":
				case "PHEditingExtensionContext":
				case "HKCumulativeQuantitySeriesSample":
				// Xcode 11 - Conformance not in headers
				case "UISceneActivationConditions":
				case "UISceneSession":
					return true;
				// Xcode 11
				case "NSFileProviderSearchQuery":
					return true;
				// Xcode 12
				case "ACAccountType":
				case "ASAccountAuthenticationModificationExtensionContext":
				case "ASCredentialProviderExtensionContext":
				case "AVAssetDownloadUrlSession":
				case "NSFileProviderDomain": // Conformance not in headers
				case "NSUrlSession":
				case "SNClassification":
				case "SNClassificationResult":
					return true;
				// PassKit now available on macOS 11+
				case "PKPaymentMethod":
				case "PKPaymentMerchantSession":
				case "PKPaymentSummaryItem":
				case "PKShareablePassMetadata":
				case "PKShippingMethod":
				case "PKSuicaPassProperties": // Conformance not in headers
				case "PKTransitPassProperties": // Conformance not in headers
												// Xcode 12.2
				case "VSAccountApplicationProvider": // Conformance not in headers
													 // Xcode 12.5
				case "HMCharacteristicMetadata":
				case "HMAccessoryCategory":
					return true;
				// Xcode 13
				case "HKVerifiableClinicalRecord":
				case "PKDeferredPaymentSummaryItem":
				case "PKRecurringPaymentSummaryItem":
				case "PKStoredValuePassProperties":
				case "SNTimeDurationConstraint": // Conformance not in headers
												 // Xcode 13.3
				case "HMAccessorySetupPayload": // Conformance not in headers
					return true;
				// Xcode 14
				case "CLKComplicationIntentWidgetMigrationConfiguration":
				case "CLKComplicationStaticWidgetMigrationConfiguration":
				case "CLKComplicationWidgetMigrationConfiguration":
				case "HKElectrocardiogramVoltageMeasurement":
				case "AVPlayerInterstitialEvent":
				case "HKWorkoutActivity":
				case "HKContactsPrescription":
				case "HKGlassesPrescription":
				case "HKVisionPrescription":
				case "PKAutomaticReloadPaymentRequest":
				case "PKAutomaticReloadPaymentSummaryItem":
				case "PKPaymentOrderDetails":
				case "PKPaymentTokenContext":
				case "PKRecurringPaymentRequest":
				case "PKShareablePassMetadataPreview":
				// Xcode 14.3, Conformance not in headers
				case "PKDeferredPaymentRequest":
					return true;
				// Xcode 15, Conformance not in headers
				case "GKBasePlayer":
				case "GKLocalPlayer":
				case "GKPlayer":
				case "PKDisbursementRequest":
				case "PKContact":
					return true;
				}
				break;
			case "NSMutableCopying":
				switch (type.Name) {
				// iOS 10 : test throw because of generic usage
				case "NSMeasurement`1":
					return true; // skip
								 // Xcode 10
				case "UNNotificationCategory":
				case "UNNotificationSound":
				// Xcode 11 - Conformance not in headers
				case "UISceneSession":
					return true;
				// xocde 12 beta 1
				case "NSUrlSessionConfiguration":
					return true;
				}
				break;
			case "NSCoding":
				switch (type.Name) {
				// only documented to support NSCopying - not NSCoding (fails on iOS 7.1 but not 8.0)
				case "NSUrlSessionTask":
				case "NSUrlSessionDataTask":
				case "NSUrlSessionUploadTask":
				case "NSUrlSessionDownloadTask":
				// 
				case "NSUrlSessionConfiguration":
				case "NSMergeConflict":
				// new in iOS8 and 10.0
				case "NSExtensionContext":
				case "NSItemProvider":
				// iOS9 / 10.11
				case "CNSaveRequest":
				case "NSLayoutAnchor`1":
				case "NSLayoutDimension":
				case "NSLayoutXAxisAnchor":
				case "NSLayoutYAxisAnchor":
				case "GKCloudPlayer":
				case "GKGameSession":
				// iOS 10 : test throw because of generic usage
				case "NSMeasurement`1":
				// iOS 11 / tvOS 11
				case "VSSubscription":
				// iOS 11.3 / macOS 10.13.4
				case "NSEntityMapping":
				case "NSMappingModel":
				case "NSPropertyMapping":
					return true;
				// Xcode 10
				case "NSManagedObjectID":
				case "VSAccountMetadata":
				case "VSAccountMetadataRequest":
				case "VSAccountProviderResponse":
				case "PHEditingExtensionContext":
					return true;
				// Xcode 11 (on device only)
				case "ICHandle":
				case "ICNotification":
				case "ICNotificationManagerConfiguration":
				case "MPSNNNeuronDescriptor":
				// Xcode 11
				case "NSFileProviderItemVersion":
				case "NSFileProviderRequest":
				case "NSFileProviderSearchQuery":
					return true;
				// Xcode 11.4, not documented
				case "NSHttpCookie":
					return true;
				// Xcode 12 beta 1
				case "ASAccountAuthenticationModificationExtensionContext":
				case "ASCredentialProviderExtensionContext":
				case "GCController":
				case "GCExtendedGamepad":
				case "GCMicroGamepad":
				case "GCMotion":
				case "INFile":
				case "SNClassification":
				case "SNClassificationResult":
					return true;
				// PassKit now available on macOS 11+
				case "PKPayment":
				case "PKPaymentSummaryItem":
				case "PKShippingMethod":
				case "PKPaymentRequest":
				case "PKPaymentToken":
				case "PKLabeledValue":
				case "PKPaymentAuthorizationResult":
				case "PKPaymentRequestShippingMethodUpdate":
				case "PKPaymentRequestUpdate":
				case "PKPaymentRequestPaymentMethodUpdate":
				case "PKPaymentRequestShippingContactUpdate":
				case "PKSuicaPassProperties": // Conformance not in headers
				case "PKTransitPassProperties": // Conformance not in headers
				case "PKDisbursementRequest":
				case "PKDisbursementVoucher":
				case "PKAddCarKeyPassConfiguration":
				case "PKAddSecureElementPassConfiguration":
				case "PKAddShareablePassConfiguration":
				case "PKBarcodeEventConfigurationRequest":
				case "PKBarcodeEventMetadataRequest":
				case "PKBarcodeEventMetadataResponse":
				case "PKBarcodeEventSignatureRequest":
				case "PKBarcodeEventSignatureResponse":
				case "PKIssuerProvisioningExtensionPaymentPassEntry":
				case "PKIssuerProvisioningExtensionStatus":
				case "PKPaymentMerchantSession":
				case "PKPaymentRequestMerchantSessionUpdate":
				case "PKShareablePassMetadata":
				// Xcode 12.2
				case "VSAccountApplicationProvider": // Conformance not in headers
					return true;
				// Xcode 12.3
				case "GCDirectionalGamepad":
				case "GCExtendedGamepadSnapshot":
				case "GCGamepadSnapshot":
				case "GCMicroGamepadSnapshot":
				case "GCGamepad":
					return true;
				// Xcode 12.5
				case "GCDualSenseGamepad":
				// Xcode 13
				case "AVAssetDownloadConfiguration":
				case "AVAssetDownloadContentConfiguration":
				case "AVAssetVariant":
				case "AVAssetVariantQualifier":
				case "PKDeferredPaymentSummaryItem":
				case "PKPaymentRequestCouponCodeUpdate":
				case "PKRecurringPaymentSummaryItem":
				case "PKStoredValuePassBalance":
				case "PKStoredValuePassProperties":
				case "QLPreviewReply": // conformance not in headers
				case "QLPreviewReplyAttachment": // conformance not in headers
				case "SNTimeDurationConstraint": // Conformance not in headers
												 // Xcode 13.3
				case "SRWristDetection": // Conformance not in headers
				case "HMAccessorySetupPayload": // Conformance not in headers
				case "HMAccessorySetupRequest": // Conformance not in headers
				case "HMAccessorySetupResult": // Conformance not in headers
					return true;
				// Xcode 14 beta 2
				case "CLKComplicationIntentWidgetMigrationConfiguration":
				case "CLKComplicationStaticWidgetMigrationConfiguration":
				case "CLKComplicationWidgetMigrationConfiguration":
				case "PHPickerConfiguration":
				case "PHAssetChangeRequest":
				case "PHAssetCreationRequest":
				case "NSUserActivity":
				case "UIDictationPhrase":
				case "HKWorkoutActivity":
				case "PKAutomaticReloadPaymentRequest":
				case "PKAutomaticReloadPaymentSummaryItem":
				case "PKPaymentOrderDetails":
				case "PKPaymentTokenContext":
				case "PKRecurringPaymentRequest":
				case "PKShareablePassMetadataPreview":
				// Xcode 14.3, Conformance not in headers
				case "PKDeferredPaymentRequest":
					return true;
				// Xcode 15, Conformance not in headers
				case "MKGeodesicPolyline":
				case "MKPolyline":
				case "MKCircle":
				case "MKCircleRenderer":
				case "MKGradientPolylineRenderer":
				case "MKMultiPolygon":
				case "MKMultiPolygonRenderer":
				case "MKMultiPolyline":
				case "MKMultiPolylineRenderer":
				case "MKPolygonRenderer":
				case "MKPolylineRenderer":
				case "AVAudioPcmBuffer":
					return true;
				}
				break;
			case "NSSecureCoding":
				switch (type.Name) {
				case "NSMergeConflict": // undocumented
										// only documented to support NSCopying (and OSX side only does that)
				case "NSUrlSessionTask":
				case "NSUrlSessionDataTask":
				case "NSUrlSessionUploadTask":
				case "NSUrlSessionDownloadTask":
				case "NSUrlSessionConfiguration":
				// new in iOS8 and 10.0
				case "NSExtensionContext":
				case "NSItemProvider":
				case "NSParagraphStyle": //17770106
				case "NSMutableParagraphStyle": //17770106
					return true; // skip
								 // iOS9 / 10.11
				case "CNSaveRequest":
				case "NSPersonNameComponentsFormatter":
				case "GKCloudPlayer":
				case "GKGameSession":
				// iOS 10 : test throw because of generic usage
				case "NSMeasurement`1":
					return true; // skip
								 // xcode 9
				case "NSConstraintConflict": // Conformance not in headers
				case "VSSubscription":
				// iOS 11.3 / macOS 10.13.4
				case "NSEntityMapping":
				case "NSMappingModel":
				case "NSPropertyMapping":
					return true;
				case "MPSImageAllocator": // Header shows NSSecureCoding, but intro gives: MPSImageAllocator conforms to NSSecureCoding but SupportsSecureCoding returned false
					return true;
				// Xcode 10
				case "ARDirectionalLightEstimate":
				case "ARFrame":
				case "ARLightEstimate":
				case "NSManagedObjectID":
				// beta 2
				case "NSTextAttachment":
				case "VSAccountMetadata":
				case "VSAccountMetadataRequest":
				case "VSAccountProviderResponse":
				case "PHEditingExtensionContext":
					return true;
				// Xcode 11 (on device only)
				case "ICHandle":
				case "ICNotification":
				case "ICNotificationManagerConfiguration":
				case "MPSNNNeuronDescriptor":
				case "MPSNDArrayAllocator": // Conformance in header, but fails at runtime.
				case "MPSNNLossCallback": // Conformance in header, but fails at runtime.
					return true;
				// Xcode 11
				case "NSFileProviderItemVersion":
				case "NSFileProviderRequest":
				case "NSFileProviderSearchQuery":
				case "INUserContext": // Header shows NSSecureCoding but intro on both device and simulator says nope FB7604793
					return true;
				// Xcode 11.4, not documented
				case "NSHttpCookie":
					return true;
				// Xcode 12
				case "ASAccountAuthenticationModificationExtensionContext":
				case "ASCredentialProviderExtensionContext":
				case "GCController":
				case "GCExtendedGamepad":
				case "GCMicroGamepad":
				case "GCMotion":
				case "SNClassification":
				case "SNClassificationResult":
				case "CPMessageListItem": // Conformance not in headers
					return true;
				// PassKit now available on macOS 11+
				case "PKPayment":
				case "PKPaymentSummaryItem":
				case "PKShippingMethod":
				case "PKPaymentRequest":
				case "PKPaymentToken":
				case "PKLabeledValue":
				case "PKPaymentAuthorizationResult":
				case "PKPaymentRequestShippingMethodUpdate":
				case "PKPaymentRequestUpdate":
				case "PKPaymentRequestPaymentMethodUpdate":
				case "PKPaymentRequestShippingContactUpdate":
				case "PKSuicaPassProperties": // Conformance not in headers
				case "PKTransitPassProperties": // Conformance not in headers
				case "PKDisbursementRequest":
				case "PKDisbursementVoucher":
				case "PKAddCarKeyPassConfiguration":
				case "PKAddSecureElementPassConfiguration":
				case "PKAddShareablePassConfiguration":
				case "PKBarcodeEventConfigurationRequest":
				case "PKBarcodeEventMetadataRequest":
				case "PKBarcodeEventMetadataResponse":
				case "PKBarcodeEventSignatureRequest":
				case "PKBarcodeEventSignatureResponse":
				case "PKIssuerProvisioningExtensionPaymentPassEntry":
				case "PKIssuerProvisioningExtensionStatus":
				case "PKPaymentMerchantSession":
				case "PKPaymentRequestMerchantSessionUpdate":
				case "PKShareablePassMetadata":
				// Xcode 12.2
				case "VSAccountApplicationProvider": // Conformance not in headers
													 // Xcode 12.3
				case "ARAppClipCodeAnchor": // Conformance comes from the base type, ARAppClipCodeAnchor conforms to NSSecureCoding but SupportsSecureCoding returned false.
				case "GCDirectionalGamepad":
				case "GCExtendedGamepadSnapshot":
				case "GCGamepadSnapshot":
				case "GCMicroGamepadSnapshot":
				case "GCGamepad":
					return true;
				// Xcode 12.5
				case "GCDualSenseGamepad":
				// xcode 13
				case "AVAssetDownloadConfiguration":
				case "AVAssetDownloadContentConfiguration":
				case "AVAssetVariant":
				case "AVAssetVariantQualifier":
				case "PKDeferredPaymentSummaryItem":
				case "PKPaymentRequestCouponCodeUpdate":
				case "PKRecurringPaymentSummaryItem":
				case "PKStoredValuePassBalance":
				case "PKStoredValuePassProperties":
				case "QLPreviewReply": // conformance not in headers
				case "QLPreviewReplyAttachment": // conformance not in headers
				case "SNTimeDurationConstraint": // Conformance not in headers
				case "NSInflectionRule":
				// Xcode 13.3
				case "SRWristDetection": // Conformance not in headers
				case "HMAccessorySetupPayload": // Conformance not in headers
				case "HMAccessorySetupRequest": // Conformance not in headers
				case "HMAccessorySetupResult": // Conformance not in headers
					return true;
				// Xcode 14
				case "CLKComplicationIntentWidgetMigrationConfiguration":
				case "CLKComplicationStaticWidgetMigrationConfiguration":
				case "CLKComplicationWidgetMigrationConfiguration":
				case "PHPickerConfiguration":
				case "PHAssetChangeRequest":
				case "PHAssetCreationRequest":
				case "NSUserActivity":
				case "UIDictationPhrase":
				case "HKWorkoutActivity":
				case "PKAutomaticReloadPaymentRequest":
				case "PKAutomaticReloadPaymentSummaryItem":
				case "PKPaymentOrderDetails":
				case "PKPaymentTokenContext":
				case "PKRecurringPaymentRequest":
				case "PKShareablePassMetadataPreview":
				// Xcode 14.3, Conformance not in headers
				case "PKDeferredPaymentRequest":
					return true;
				// Xcode 15, Conformance not in headers
				case "MKGeodesicPolyline":
				case "MKPolyline":
				case "MKCircle":
				case "MKCircleRenderer":
				case "MKGradientPolylineRenderer":
				case "MKMultiPolygon":
				case "MKMultiPolygonRenderer":
				case "MKMultiPolyline":
				case "MKMultiPolylineRenderer":
				case "MKPolygonRenderer":
				case "MKPolylineRenderer":
				case "AVAudioPcmBuffer":
					return true;
				}
				break;
			// conformance added in Xcode 8 (iOS 10 / macOS 10.12)
			case "MDLNamed":
				switch (type.Name) {
				case "MTKMeshBuffer":
				case "MDLVoxelArray": // base class changed to MDLObject (was NSObject before)
					return true;
				}
				break;
			case "CALayerDelegate":
				switch (type.Name) {
				case "MTKView":
					return true;
				}
				break;
			case "NSProgressReporting":
				switch (type.Name) {
				case "NSOperationQueue":
					if (!TestRuntime.CheckXcodeVersion (11, 0))
						return true;
					break;
				default:
					if (!TestRuntime.CheckXcodeVersion (9, 0))
						return true;
					break;
				}
				break;
			case "GKSceneRootNodeType":
				// it's an empty protocol, defined by a category and does not reply as expected
				switch (type.Name) {
				// GameplayKit.framework/Headers/SceneKit+Additions.h
				case "SCNScene":
				// GameplayKit.framework/Headers/SpriteKit+Additions.h
				case "SKScene":
					return true;
				}
				break;
			case "UIUserActivityRestoring":
				if (!TestRuntime.CheckXcodeVersion (10, 0))
					return true;

				switch (type.Name) {
				// UIKit.framework/Headers/UIDocument.h
				case "UIDocument":
				// inherits it from UIDocument
				case "UIManagedDocument":
					return true;
				}
				break;
			case "SCNTechniqueSupport":
				if (!TestRuntime.CheckXcodeVersion (11, 0))
					return false;
				switch (type.Name) {
				case "SCNLight":
					return true;
				}
				break;
			case "VNRequestRevisionProviding":
				switch (type.Name) {
				case "VNFaceLandmarks":
				case "VNFaceLandmarks2D":
				case "VNFaceLandmarkRegion":
				case "VNFaceLandmarkRegion2D":
					if (!TestRuntime.CheckXcodeVersion (11, 0))
						return true;
					break;
				case "VNRecognizedText":
					// Conformance added in Xcode 13
					if (!TestRuntime.CheckXcodeVersion (13, 0))
						return true;
					break;
				}
				break;
			case "NSUserActivityRestoring":
				return true;
#if __MACCATALYST__
			case "UIScrollViewDelegate":
				// The headers say PKCanvasViewDelegate implements UIScrollViewDelegate
				if (type.Name == "PKCanvasViewDelegate")
					return true;
				break;
#endif
			case "NSExtensionRequestHandling":
				if (type.Name == "HMChipServiceRequestHandler") // Apple removed this class
					return true;
				break;
			case "QLPreviewItem":
				if (type.Name == "NSUrl")
					return true;
				break;
			}
			return false;
		}

		void CheckProtocol (string protocolName, Action<Type, IntPtr, bool> action)
		{
			IntPtr protocol = Runtime.GetProtocol (protocolName);
			Assert.AreNotEqual (protocol, IntPtr.Zero, protocolName);

			int n = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (!NSObjectType.IsAssignableFrom (t))
					continue;

				if (Skip (t) || Skip (t, protocolName))
					continue;

				if (LogProgress)
					Console.WriteLine ("{0}. {1} conforms to {2}", ++n, t.FullName, protocolName);

				IntPtr klass = GetClass (t);
				action (t, klass, ConformTo (klass, protocol));
			}
		}

		[Test]
		public void Coding ()
		{
			Errors = 0;
			var list = new List<string> ();
			CheckProtocol ("NSCoding", delegate (Type type, IntPtr klass, bool result)
			{
				// `type` conforms to (native) NSCoding so...
				if (result) {
					// the type should implements INSCoding
					if (!typeof (INSCoding).IsAssignableFrom (type)) {
						list.Add (type.Name);
						ReportError ("{0} conforms to NSCoding but does not implement INSCoding", type.Name);
					}
					// FIXME: and implement the .ctor(NSCoder)
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSCoding but does not implement INSCoding: {1}", Errors, String.Join ('\n', list));
		}

		// [Test] -> iOS 6.0+ and Mountain Lion (10.8) +
		public virtual void SecureCoding ()
		{
			Errors = 0;
			var list = new List<string> ();
			CheckProtocol ("NSSecureCoding", delegate (Type type, IntPtr klass, bool result)
			{
				if (result) {
					// the type should implements INSSecureCoding
					if (!typeof (INSSecureCoding).IsAssignableFrom (type)) {
						ReportError ("{0} conforms to NSSecureCoding but does not implement INSSecureCoding", type.Name);
					}
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSSecureCoding but does not implement INSSecureCoding: {1}", Errors, String.Join ('\n', list));
		}

		bool SupportsSecureCoding (Type type)
		{
			Class cls = new Class (type);
			if (!bool_objc_msgSend_IntPtr (cls.Handle, Selector.GetHandle ("respondsToSelector:"), Selector.GetHandle ("supportsSecureCoding")))
				return false;

			return NSSecureCoding.SupportsSecureCoding (type);
		}


		// [Test] -> iOS 6.0+ and Mountain Lion (10.8) +
		public virtual void SupportsSecureCoding ()
		{
			Errors = 0;
			CheckProtocol ("NSSecureCoding", delegate (Type type, IntPtr klass, bool result)
			{
				bool supports = SupportsSecureCoding (type);
				if (result) {
					// check that +supportsSecureCoding returns YES
					if (!supports) {
#if __IOS__
						// broken in xcode 12 beta 1 simulator (only)
						if (TestRuntime.IsSimulatorOrDesktop && TestRuntime.CheckXcodeVersion (12, 0)) {
							switch (type.Name) {
							case "ARFaceGeometry":
							case "ARPlaneGeometry":
							case "ARPointCloud":
							case "ARAnchor":
							case "ARBodyAnchor":
							case "AREnvironmentProbeAnchor":
							case "ARFaceAnchor":
							case "ARGeoAnchor":
							case "ARGeometryElement":
							case "ARGeometrySource":
							case "ARImageAnchor":
							case "ARMeshAnchor":
							case "ARMeshGeometry":
							case "ARObjectAnchor":
							case "ARParticipantAnchor":
							case "ARPlaneAnchor":
							case "ARReferenceObject":
							case "ARSkeletonDefinition": // iOS15 / device only
							case "ARWorldMap":
								return;
							}
						}
#endif
						ReportError ("{0} conforms to NSSecureCoding but SupportsSecureCoding returned false", type.Name);
					}
				} else if (type.IsPublic && supports) {
					// there are internal types, e.g. DataWrapper : NSData, that subclass NSSecureCoding-types without
					// [re-]declaring their allegiance - but we can live with those small betrayals
					Assert.IsFalse (NSSecureCoding.SupportsSecureCoding (type), "{0} !SupportsSecureCoding", type.Name);
					ReportError ("SupportsSecureCoding returns true but {0} does not conforms to NSSecureCoding", type.Name);
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSCoding but does not implement INSSecureCoding", Errors);
		}

		[Test]
		public void Copying ()
		{
			Errors = 0;
			var list = new List<string> ();
			CheckProtocol ("NSCopying", delegate (Type type, IntPtr klass, bool result)
			{
				// `type` conforms to (native) NSCopying so...
				if (result) {
					// the type should implements INSCopying
					if (!typeof (INSCopying).IsAssignableFrom (type)) {
						list.Add (type.Name);
						ReportError ("{0} conforms to NSCopying but does not implement INSCopying", type.Name);
					}
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSCopying but does not implement INSCopying: {1}", Errors, String.Join ('\n', list));
		}

		[Test]
		public void MutableCopying ()
		{
			Errors = 0;
			var list = new List<string> ();
			CheckProtocol ("NSMutableCopying", delegate (Type type, IntPtr klass, bool result)
			{
				// `type` conforms to (native) NSMutableCopying so...
				if (result) {
					// the type should implements INSMutableCopying
					if (!typeof (INSMutableCopying).IsAssignableFrom (type)) {
						list.Add (type.Name);
						ReportError ("{0} conforms to NSMutableCopying but does not implement INSMutableCopying", type.Name);
					}
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSMutableCopying but does not implement INSMutableCopying: {1}", Errors, String.Join ('\n', list));
		}

		[Test]
		public void GeneralCase ()
		{
			Errors = 0;
			var list = new List<string> ();
			foreach (Type t in Assembly.GetTypes ()) {
				if (!NSObjectType.IsAssignableFrom (t))
					continue;

				if (Skip (t))
					continue;

				var klass = new Class (t);
				if (klass.Handle == IntPtr.Zero) {
					// This can often by caused by [Protocol] classes with no [Model] but having a [BaseType].
					// Either have both a Model and BaseType or neither
					switch (t.Name) {
					case "AVPlayerInterstitialEventMonitor": // deprecated
						continue;
#if !MONOMAC
					case "MTLCaptureManager":
					case "NEHotspotConfiguration":
					case "NEHotspotConfigurationManager":
					case "NEHotspotEapSettings":
					case "NEHotspotHS20Settings":
					case "SCNGeometryTessellator":
					case "SKRenderer":
						// was not possible in iOS 11.4 (current minimum) simulator
						if (!TestRuntime.CheckXcodeVersion (12, 0)) {
							if (TestRuntime.IsSimulatorOrDesktop)
								continue;
						}
						break;
#endif
					default:
						var e = $"[FAIL] Could not load {t.FullName}";
						list.Add (e);
						AddErrorLine (e);
						continue;
					}
				}

				foreach (var intf in t.GetInterfaces ()) {
					if (SkipDueToAttribute (intf))
						continue;

					string protocolName = intf.Name.Substring (1);
					switch (protocolName) {
					case "NSCoding":
					case "NSSecureCoding":
					case "NSCopying":
					case "NSMutableCopying":
						// we have special test cases for them
						continue;
					default:
						if (Skip (t, protocolName))
							continue;
						break;
					}

					var a = intf.GetCustomAttribute<ProtocolAttribute> (true);
					if (a is null || a.IsInformal)
						continue;

					IntPtr protocol = Runtime.GetProtocol (protocolName);
					if (protocol == IntPtr.Zero)
						continue; // not a protocol

					if (LogProgress)
						Console.WriteLine ("{0} conforms to {1}", t.FullName, protocolName);

					if (t.IsPublic && !ConformTo (klass.Handle, protocol)) {
						// note: some internal types, e.g. like UIAppearance subclasses, return false (and there's not much value in changing this)
						var msg = $"Type {t.FullName} (native: {klass.Name}) does not conform {protocolName}";
						list.Add (msg);
						ReportError (msg);
					}
				}
			}
			AssertIfErrors ("{0} types do not really conform to the protocol interfaces: {1}", Errors, String.Join ('\n', list));
		}
	}
}
