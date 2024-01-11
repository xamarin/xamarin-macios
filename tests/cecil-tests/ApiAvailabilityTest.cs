using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Mono.Cecil;

using Xamarin.Utils;
using Xamarin.Tests;

#nullable enable

namespace Cecil.Tests {
	[TestFixture]
	public class ApiAvailabilityTest {

		public record ObsoletedFailure : IComparable {

			public string Key { get; }
			public ICustomAttributeProvider Api { get; }
			public OSPlatformAttributes [] Obsoleted { get; }
			public OSPlatformAttributes [] Supported { get; }

			public ObsoletedFailure (string key, ICustomAttributeProvider api, OSPlatformAttributes [] obsoleted, OSPlatformAttributes [] supported)
			{
				Key = key;
				Api = api;
				Obsoleted = obsoleted;
				Supported = supported;
			}

			public override string ToString ()
				=> $"{Key}: {Api} is obsoleted on {string.Join (", ", Obsoleted.Select (v => v.Platform))} but not on {string.Join (", ", Supported.Select (v => v.Platform))}";

			public int CompareTo (object? obj)
			{
				if (obj is not ObsoletedFailure other)
					return -1;
				return Key.CompareTo (other.Key);
			}

		}

		// This test will flag any API that's only obsoleted on some platforms.
		[Test]
		public void FindMissingObsoleteAttributes ()
		{
			Configuration.IgnoreIfAnyIgnoredPlatforms ();

			var harvestedInfo = Helper.MappedNetApi;

			var failures = new Dictionary<string, ObsoletedFailure> ();
			var mismatchedObsoleteMessages = new List<string> ();
			foreach (var kvp in harvestedInfo) {
				var attributes = kvp.Value.Select (v => v.Api.GetAvailabilityAttributes (v.Platform) ?? new OSPlatformAttributes (v.Api, v.Platform) ?? new OSPlatformAttributes (v.Api, v.Platform)).ToArray ();
				var obsoleted = attributes.Where (v => v?.Obsoleted is not null).ToArray ();

				// No obsoleted, nothing to check
				if (obsoleted.Length == 0)
					continue;

				// All obsoleted, nothing to check
				if (obsoleted.Length == attributes.Length)
					continue;

				// If the non-obsoleted APIs are all unsupported, then there's nothing to do.
				var notObsoletedNorUnsupported = attributes.Where (v => v?.Obsoleted is null && v?.Unsupported is null).ToArray ();
				if (!notObsoletedNorUnsupported.Any ())
					continue;

				var failure = new ObsoletedFailure (kvp.Key, kvp.Value.First ().Api, obsoleted, notObsoletedNorUnsupported);
				failures [failure.Key] = failure;

				var obsoleteMessages = obsoleted.Select (v => v.Obsoleted?.Message).Distinct ().ToArray ();
				if (obsoleteMessages.Length > 1) {
					var obsoleteFailure = new StringBuilder ();
					obsoleteFailure.AppendLine ($"{failure.Key}: Found different {obsoleteMessages.Length} obsolete messages:");
					foreach (var msg in obsoleteMessages)
						obsoleteFailure.AppendLine ($"    {(msg is null ? "null" : (msg.Length == 0 ? "<empty string>" : "\"" + msg + "\""))}");
					mismatchedObsoleteMessages.Add (obsoleteFailure.ToString ());
					Console.WriteLine (obsoleteFailure);
				}
			}

			Helper.AssertFailures (failures,
				knownFailuresInMissingObsoleteAttributes,
				nameof (knownFailuresInMissingObsoleteAttributes),
				"Missing obsolete attributes",
				(failure) => {
					return $"{failure.Key}: {failure.Api.RenderLocation ()}\n" +
						$"    Obsoleted in: {string.Join (", ", failure.Obsoleted.Select (v => v!.Obsoleted!.PlatformName))}\n" +
						$"    Not obsoleted in: {string.Join (", ", failure.Supported.Select (v => v?.Supported?.PlatformName ?? v?.Platform.ToString ()))}\n";
				});
		}

		static HashSet<string> knownFailuresInMissingObsoleteAttributes = new HashSet<string> {
			"AudioToolbox.AudioSessionActiveFlags",
			"AVFoundation.AVAssetDownloadUrlSession.GetAssetDownloadTask(AVFoundation.AVUrlAsset, Foundation.NSUrl, Foundation.NSDictionary)",
			"AVFoundation.AVCaptureDevice.IsFlashModeSupported(AVFoundation.AVCaptureFlashMode)",
			"AVFoundation.AVCaptureFlashMode AVFoundation.AVCaptureDevice::FlashMode()",
			"CFNetwork.CFHTTPStream",
			"CoreBluetooth.CBCentralManagerState",
			"CoreBluetooth.CBPeripheralManagerState",
			"CoreData.NSPersistentStoreCoordinator.get_DidImportUbiquitousContentChangesNotification()",
			"CoreData.NSPersistentStoreCoordinator.get_PersistentStoreUbiquitousContentNameKey()",
			"CoreData.NSPersistentStoreCoordinator.get_PersistentStoreUbiquitousContentUrlKey()",
			"CoreFoundation.DispatchQueue CoreFoundation.DispatchQueue::CurrentQueue()",
			"CoreGraphics.CGColorSpace.CreateIccProfile(Foundation.NSData)",
			"CoreGraphics.CGColorSpace.GetIccProfile()",
			"CoreGraphics.CGContext.SelectFont(System.String, System.Runtime.InteropServices.NFloat, CoreGraphics.CGTextEncoding)",
			"CoreGraphics.CGContext.ShowGlyphs(System.UInt16[])",
			"CoreGraphics.CGContext.ShowGlyphs(System.UInt16[], System.Int32)",
			"CoreGraphics.CGContext.ShowGlyphsAtPoint(System.Runtime.InteropServices.NFloat, System.Runtime.InteropServices.NFloat, System.UInt16[])",
			"CoreGraphics.CGContext.ShowGlyphsAtPoint(System.Runtime.InteropServices.NFloat, System.Runtime.InteropServices.NFloat, System.UInt16[], System.Int32)",
			"CoreGraphics.CGContext.ShowGlyphsWithAdvances(System.UInt16[], CoreGraphics.CGSize[], System.Int32)",
			"CoreGraphics.CGContext.ShowText(System.Byte[])",
			"CoreGraphics.CGContext.ShowText(System.Byte[], System.Int32)",
			"CoreGraphics.CGContext.ShowText(System.String)",
			"CoreGraphics.CGContext.ShowText(System.String, System.Int32)",
			"CoreGraphics.CGContext.ShowTextAtPoint(System.Runtime.InteropServices.NFloat, System.Runtime.InteropServices.NFloat, System.String)",
			"CoreGraphics.CGContext.ShowTextAtPoint(System.Runtime.InteropServices.NFloat, System.Runtime.InteropServices.NFloat, System.String, System.Int32)",
			"CoreGraphics.CGImage PassKit.PKShareablePassMetadata::PassThumbnailImage()",
			"CoreLocation.CLAuthorizationStatus CoreLocation.CLAuthorizationStatus::Authorized",
			"CoreLocation.CLAuthorizationStatus CoreLocation.CLAuthorizationStatus::AuthorizedWhenInUse",
			"CoreLocation.CLLocationManagerDelegate.UpdatedLocation(CoreLocation.CLLocationManager, CoreLocation.CLLocation, CoreLocation.CLLocation)",
			"CoreLocation.CLLocationManagerDelegate_Extensions.UpdatedLocation(CoreLocation.ICLLocationManagerDelegate, CoreLocation.CLLocationManager, CoreLocation.CLLocation, CoreLocation.CLLocation)",
			"CoreMedia.CMTime AVFoundation.AVCaptureConnection::VideoMaxFrameDuration()",
			"CoreMedia.CMTime AVFoundation.AVCaptureConnection::VideoMinFrameDuration()",
			"CoreMedia.CMTime AVFoundation.AVCaptureVideoDataOutput::MinFrameDuration()",
			"CoreMidi.MidiClient.CreateVirtualDestination(System.String, out CoreMidi.MidiError&)",
			"CoreMidi.MidiClient.CreateVirtualSource(System.String, out CoreMidi.MidiError&)",
			"CoreMidi.MidiDevice.Add(System.String, System.Boolean, System.UIntPtr, System.UIntPtr, CoreMidi.MidiEntity)",
			"CoreMidi.MidiEndpoint.Received(CoreMidi.MidiPacket[])",
			"CoreMidi.MidiPort.Send(CoreMidi.MidiEndpoint, CoreMidi.MidiPacket[])",
			"CoreText.CTFontFeatureLetterCase",
			"CoreText.CTFontManager.RegisterFontsForUrl(Foundation.NSUrl[], CoreText.CTFontManagerScope)",
			"CoreText.CTFontManager.UnregisterFontsForUrl(Foundation.NSUrl[], CoreText.CTFontManagerScope)",
			"CoreText.CTFontManagerAutoActivation CoreText.CTFontManagerAutoActivation::PromptUser",
			"CoreText.CTTypesetterOptionKey.get_DisableBidiProcessing()",
			"CoreText.FontFeatureGroup CoreText.FontFeatureGroup::LetterCase",
			"Foundation.NSData HealthKit.HKVerifiableClinicalRecord::JwsRepresentation()",
			"Foundation.NSDate HealthKit.HKWorkoutEvent::Date()",
			"Foundation.NSString CoreData.NSPersistentStoreCoordinator::DidImportUbiquitousContentChangesNotification()",
			"Foundation.NSString CoreData.NSPersistentStoreCoordinator::PersistentStoreUbiquitousContentNameKey()",
			"Foundation.NSString CoreData.NSPersistentStoreCoordinator::PersistentStoreUbiquitousContentUrlKey()",
			"Foundation.NSString CoreText.CTTypesetterOptionKey::DisableBidiProcessing()",
			"Foundation.NSString Foundation.NSUrl::UbiquitousItemIsDownloadingKey()",
			"Foundation.NSTask.LaunchFromPath(System.String, System.String[])",
			"Foundation.NSUrl.get_UbiquitousItemIsDownloadingKey()",
			"Foundation.NSUrlSessionConfiguration.BackgroundSessionConfiguration(System.String)",
			"Foundation.NSUserDefaults..ctor(System.String)",
			"GameController.GCGamepadSnapShotDataV100",
			"GameController.GCMicroGamepadSnapshot.TryGetSnapshotData(Foundation.NSData, out GameController.GCMicroGamepadSnapshotData&)",
			"GameController.GCMicroGamepadSnapshot.TryGetSnapshotData(Foundation.NSData, out GameController.GCMicroGamepadSnapShotDataV100&)",
			"GameController.GCMicroGamepadSnapshotData",
			"GameController.GCMicroGamepadSnapshotData.ToNSData()",
			"GameController.GCMicroGamepadSnapShotDataV100",
			"HealthKit.HKAnchoredObjectQuery..ctor(HealthKit.HKSampleType, Foundation.NSPredicate, System.UIntPtr, System.UIntPtr, HealthKit.HKAnchoredObjectResultHandler)",
			"HealthKit.HKCategoryValueOvulationTestResult HealthKit.HKCategoryValueOvulationTestResult::Positive",
			"HealthKit.HKHealthStore.GetDateOfBirth(out Foundation.NSError&)",
			"HealthKit.HKHealthStore.SplitTotalEnergy(HealthKit.HKQuantity, Foundation.NSDate, Foundation.NSDate, System.Action`3<HealthKit.HKQuantity,HealthKit.HKQuantity,Foundation.NSError>)",
			"HealthKit.HKQuantity HealthKit.HKActivitySummary::AppleExerciseTimeGoal()",
			"HealthKit.HKQuantity HealthKit.HKActivitySummary::AppleStandHoursGoal()",
			"HealthKit.HKQuantityAggregationStyle HealthKit.HKQuantityAggregationStyle::Discrete",
			"HealthKit.HKQuantitySeriesSampleQuery..ctor(HealthKit.HKQuantitySample, HealthKit.HKQuantitySeriesSampleQueryQuantityDelegate)",
			"HealthKit.HKSampleType HealthKit.HKQuery::SampleType()",
			"HealthKit.HKSource HealthKit.HKObject::Source()",
			"HealthKit.HKUnit HealthKit.HKUnit::Calorie()",
			"HealthKit.HKWorkoutActivityType HealthKit.HKWorkoutActivityType::DanceInspiredTraining",
			"HealthKit.HKWorkoutActivityType HealthKit.HKWorkoutActivityType::MixedMetabolicCardioTraining",
			"HealthKit.HKWorkoutEvent.Create(HealthKit.HKWorkoutEventType, Foundation.NSDate)",
			"HealthKit.HKWorkoutEvent.Create(HealthKit.HKWorkoutEventType, Foundation.NSDate, Foundation.NSDictionary)",
			"HealthKit.HKWorkoutEvent.Create(HealthKit.HKWorkoutEventType, Foundation.NSDate, HealthKit.HKMetadata)",
			"HomeKit.HMEventTrigger.CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent(HomeKit.HMSignificantEvent, Foundation.NSDateComponents)",
			"HomeKit.HMEventTrigger.CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent(HomeKit.HMSignificantEvent, Foundation.NSDateComponents)",
			"Intents.INCallRecord..ctor(System.String, Foundation.NSDate, Intents.INPerson, Intents.INCallRecordType, Intents.INCallCapability, System.Nullable`1<System.Double>, System.Nullable`1<System.Boolean>, System.Nullable`1<System.Int32>)",
			"Intents.INCallRecordType Intents.INStartCallIntent::RecordTypeForRedialing()",
			"Intents.INSetClimateSettingsInCarIntent..ctor(System.Nullable`1<System.Boolean>, System.Nullable`1<System.Boolean>, System.Nullable`1<System.Boolean>, System.Nullable`1<System.Boolean>, Intents.INCarAirCirculationMode, Foundation.NSNumber, Foundation.NSNumber, Intents.INRelativeSetting, Foundation.NSMeasurement`1<Foundation.NSUnitTemperature>, Intents.INRelativeSetting, Intents.INCarSeat)",
			"Intents.INSetDefrosterSettingsInCarIntent..ctor(System.Nullable`1<System.Boolean>, Intents.INCarDefroster)",
			"Intents.INSetProfileInCarIntent..ctor(Foundation.NSNumber, System.String, Foundation.NSNumber)",
			"Intents.INSetProfileInCarIntent..ctor(Foundation.NSNumber, System.String, System.Nullable`1<System.Boolean>)",
			"Intents.INSetSeatSettingsInCarIntent..ctor(System.Nullable`1<System.Boolean>, System.Nullable`1<System.Boolean>, System.Nullable`1<System.Boolean>, Intents.INCarSeat, Foundation.NSNumber, Intents.INRelativeSetting)",
			"Intents.INStartCallIntent..ctor(Intents.INCallAudioRoute, Intents.INCallDestinationType, Intents.INPerson[], Intents.INCallRecordType, Intents.INCallCapability)",
			"MapKit.MKOverlayView",
			"MediaPlayer.MPVolumeSettings.AlertHide()",
			"MediaPlayer.MPVolumeSettings.AlertIsVisible()",
			"MediaPlayer.MPVolumeSettings.AlertShow()",
			"MetalPerformanceShaders.MPSCnnConvolutionDescriptor.GetConvolutionDescriptor(System.UIntPtr, System.UIntPtr, System.UIntPtr, System.UIntPtr, MetalPerformanceShaders.MPSCnnNeuron)",
			"MetalPerformanceShaders.MPSCnnFullyConnected..ctor(Metal.IMTLDevice, MetalPerformanceShaders.MPSCnnConvolutionDescriptor, System.Single[], System.Single[], MetalPerformanceShaders.MPSCnnConvolutionFlags)",
			"MetalPerformanceShaders.MPSCnnNeuron MetalPerformanceShaders.MPSCnnConvolution::Neuron()",
			"MetalPerformanceShaders.MPSCnnNeuron MetalPerformanceShaders.MPSCnnConvolutionDescriptor::Neuron()",
			"MetalPerformanceShaders.MPSCnnNeuronPReLU..ctor(Metal.IMTLDevice, System.Single[])",
			"MetalPerformanceShaders.MPSMatrixDescriptor.Create(System.UIntPtr, System.UIntPtr, System.UIntPtr, MetalPerformanceShaders.MPSDataType)",
			"MetalPerformanceShaders.MPSMatrixDescriptor.GetRowBytesFromColumns(System.UIntPtr, MetalPerformanceShaders.MPSDataType)",
			"MobileCoreServices.UTType.CopyAllTags(System.String, System.String)",
			"MobileCoreServices.UTType.Equals(Foundation.NSString, Foundation.NSString)",
			"MobileCoreServices.UTType.IsDeclared(System.String)",
			"MobileCoreServices.UTType.IsDynamic(System.String)",
			"PassKit.PKShareablePassMetadata..ctor(System.String, System.String, CoreGraphics.CGImage, System.String, System.String, System.String, System.String, System.String, System.Boolean)",
			"PassKit.PKShareablePassMetadata..ctor(System.String, System.String, System.String, CoreGraphics.CGImage, System.String, System.String)",
			"Security.Authorization.ExecuteWithPrivileges(System.String, Security.AuthorizationFlags, System.String[])",
			"Security.SecAccessible Security.SecAccessible::Always",
			"Security.SecAccessible Security.SecAccessible::AlwaysThisDeviceOnly",
			"Security.SecCertificate.GetSerialNumber()",
			"Security.SecKey.Decrypt(Security.SecPadding, System.IntPtr, System.IntPtr, System.IntPtr, System.IntPtr&)",
			"Security.SecKey.Encrypt(Security.SecPadding, System.IntPtr, System.IntPtr, System.IntPtr, System.IntPtr&)",
			"Security.SecKey.RawSign(Security.SecPadding, System.IntPtr, System.Int32, out System.Byte[]&)",
			"Security.SecKey.RawVerify(Security.SecPadding, System.IntPtr, System.Int32, System.IntPtr, System.Int32)",
			"Security.SecProtocolOptions.AddTlsCipherSuiteGroup(Security.SslCipherSuiteGroup)",
			"Security.SecSharedCredential.RequestSharedWebCredential(System.String, System.String, System.Action`2<Security.SecSharedCredentialInfo[],Foundation.NSError>)",
			"Security.SecTrust.Evaluate()",
			"Security.SecTrust.Evaluate(CoreFoundation.DispatchQueue, Security.SecTrustCallback)",
			"Security.SecTrust.GetPublicKey()",
			"Security.SslContext.GetAlpnProtocols()",
			"Security.SslContext.GetAlpnProtocols(out System.Int32&)",
			"Security.SslContext.GetRequestedPeerName()",
			"Security.SslContext.ReHandshake()",
			"Security.SslContext.SetAlpnProtocols(System.String[])",
			"Security.SslContext.SetEncryptionCertificate(Security.SecIdentity, System.Collections.Generic.IEnumerable`1<Security.SecCertificate>)",
			"Security.SslContext.SetError(Security.SecStatusCode)",
			"Security.SslContext.SetOcspResponse(Foundation.NSData)",
			"Security.SslContext.SetSessionConfig(Foundation.NSString)",
			"Security.SslContext.SetSessionConfig(Security.SslSessionConfig)",
			"Security.SslContext.SetSessionTickets(System.Boolean)",
			"Security.SslProtocol Security.SecProtocolMetadata::NegotiatedProtocolVersion()",
			"Speech.SFVoiceAnalytics Speech.SFTranscriptionSegment::VoiceAnalytics()",
			"StoreKit.SKCloudServiceController.RequestPersonalizationToken(System.String, System.Action`2<Foundation.NSString,Foundation.NSError>)",
			"StoreKit.SKCloudServiceController.RequestPersonalizationTokenAsync(System.String)",
			"StoreKit.SKMutablePayment.PaymentWithProduct(System.String)",
			"StoreKit.SKStoreReviewController.RequestReview()",
			"System.Boolean AVFoundation.AVCaptureConnection::SupportsVideoMaxFrameDuration()",
			"System.Boolean AVFoundation.AVCaptureConnection::SupportsVideoMinFrameDuration()",
			"System.Boolean AVFoundation.AVCapturePhotoSettings::AutoDualCameraFusionEnabled()",
			"System.Boolean AVFoundation.AVCapturePhotoSettings::DualCameraDualPhotoDeliveryEnabled()",
			"System.Boolean AVFoundation.AVCaptureResolvedPhotoSettings::DualCameraFusionEnabled()",
			"System.Boolean CoreGraphics.CGColorSpace::IsHdr()",
			"System.Boolean CoreText.CTTypesetterOptions::DisableBidiProcessing()",
			"System.Boolean NetworkExtension.NEFilterProviderConfiguration::FilterBrowsers()",
			"System.Boolean Security.SecRecord::UseNoAuthenticationUI()",
			"System.Double Speech.SFTranscription::AveragePauseDuration()",
			"System.Double Speech.SFTranscription::SpeakingRate()",
			"System.String PassKit.PKAddShareablePassConfiguration::ProvisioningPolicyIdentifier()",
			"System.String PassKit.PKShareablePassMetadata::LocalizedDescription()",
			"System.String PassKit.PKShareablePassMetadata::OwnerDisplayName()",
			"System.String PassKit.PKShareablePassMetadata::TemplateIdentifier()",
			"System.String Speech.SFSpeechRecognitionRequest::InteractionIdentifier()",
			"System.String StoreKit.SKProduct::ContentVersion()",
			"System.String UserNotifications.UNMutableNotificationContent::SummaryArgument()",
			"System.String UserNotifications.UNNotificationContent::SummaryArgument()",
			"System.UIntPtr UserNotifications.UNMutableNotificationContent::SummaryArgumentCount()",
			"System.UIntPtr UserNotifications.UNNotificationContent::SummaryArgumentCount()",
			"SystemConfiguration.CaptiveNetwork.MarkPortalOffline(System.String)",
			"SystemConfiguration.CaptiveNetwork.MarkPortalOnline(System.String)",
			"SystemConfiguration.CaptiveNetwork.SetSupportedSSIDs(System.String[])",
			"SystemConfiguration.CaptiveNetwork.TryGetSupportedInterfaces(out System.String[]&)",
			"UIKit.UIGestureRecognizer UIKit.UIScrollView::DirectionalPressGestureRecognizer()",
			"UIKit.UIGraphicsRendererFormat UIKit.UIGraphicsRendererFormat::DefaultFormat()",
			"UIKit.UIStringDrawing.DrawString(System.String, CoreGraphics.CGPoint, System.Runtime.InteropServices.NFloat, UIKit.UIFont, System.Runtime.InteropServices.NFloat, System.Runtime.InteropServices.NFloat&, UIKit.UILineBreakMode, UIKit.UIBaselineAdjustment)",
			"UIKit.UIStringDrawing.DrawString(System.String, CoreGraphics.CGPoint, System.Runtime.InteropServices.NFloat, UIKit.UIFont, System.Runtime.InteropServices.NFloat, UIKit.UILineBreakMode, UIKit.UIBaselineAdjustment)",
			"UIKit.UIStringDrawing.DrawString(System.String, CoreGraphics.CGPoint, System.Runtime.InteropServices.NFloat, UIKit.UIFont, UIKit.UILineBreakMode)",
			"UIKit.UIStringDrawing.DrawString(System.String, CoreGraphics.CGPoint, UIKit.UIFont)",
			"UIKit.UIStringDrawing.DrawString(System.String, CoreGraphics.CGRect, UIKit.UIFont)",
			"UIKit.UIStringDrawing.DrawString(System.String, CoreGraphics.CGRect, UIKit.UIFont, UIKit.UILineBreakMode)",
			"UIKit.UIStringDrawing.DrawString(System.String, CoreGraphics.CGRect, UIKit.UIFont, UIKit.UILineBreakMode, UIKit.UITextAlignment)",
			"UIKit.UIStringDrawing.StringSize(System.String, UIKit.UIFont)",
			"UIKit.UIStringDrawing.StringSize(System.String, UIKit.UIFont, CoreGraphics.CGSize)",
			"UIKit.UIStringDrawing.StringSize(System.String, UIKit.UIFont, CoreGraphics.CGSize, UIKit.UILineBreakMode)",
			"UIKit.UIStringDrawing.StringSize(System.String, UIKit.UIFont, System.Runtime.InteropServices.NFloat, System.Runtime.InteropServices.NFloat&, System.Runtime.InteropServices.NFloat, UIKit.UILineBreakMode)",
			"UIKit.UIStringDrawing.StringSize(System.String, UIKit.UIFont, System.Runtime.InteropServices.NFloat, UIKit.UILineBreakMode)",
		};

		HashSet<string> knownConsistencyIssues = new HashSet<string> { };

		// This test verifies that the SupportedOSPlatform and UnavailableOSPlatform/ObsoletedOSplatform attributes are consistent.
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblyDefinitions))]
		public void AttributeConsistency (AssemblyInfo info)
		{
			var assembly = info.Assembly;
			var platform = info.Platform;
			var failures = new HashSet<string> ();

			foreach (var api in assembly.EnumerateAttributeProviders ()) {
				var availability = api.GetAvailabilityAttributes (platform);

				var apiSupportedVersion = availability?.Supported?.Version;
				var apiObsoletedVersion = availability?.Obsoleted?.Version;
				var apiUnsupportedVersion = availability?.Unsupported?.Version;
				var apiSupportedAttribute = availability?.Supported?.Attribute;
				var apiObsoletedAttribute = availability?.Obsoleted?.Attribute;
				var apiUnsupportedAttribute = availability?.Unsupported?.Attribute;
				var supportedPlatformName = availability?.Supported?.PlatformName;
				var obsoletedPlatformName = availability?.Obsoleted?.PlatformName;
				var unsupportedPlatformName = availability?.Unsupported?.PlatformName;

				// Verify that any SupportedOSPlatform attributes don't specify a version that is
				// either earlier than our minimum deployment target, or later than the current OS version.
				if (apiSupportedVersion is not null && !(api is AssemblyDefinition)) {
					var minimum = Xamarin.SdkVersions.GetMinVersion (platform);
					var maximum = Xamarin.SdkVersions.GetVersion (platform);

					if (apiSupportedVersion <= minimum)
						failures.Add ($"[FAIL] {apiSupportedVersion} <= {minimum} (Min) on '{api.AsFullName ()}'.");
					if (apiSupportedVersion > maximum)
						failures.Add ($"[FAIL] {apiSupportedVersion} > {maximum} (Max) on '{api.AsFullName ()}'.");
				}

				// APIs shouldn't become unsupported in the same version they become supported.
				//     [SupportedOSPlatform ("ios12.0")]
				//     [UnsupportedOSPlatform ("ios12.0")]
				// Exceptions:
				// * Apple introduced numerous already deprecated frameworks in Mac Catalyst 13.* and Mac Catalyst 14.0, so it's correct to declare those
				//   both supported and unsupported in the same version - so we skip any APIs introduced in Mac Catalyst 14.0 or earlier.
				if (apiSupportedVersion is not null && apiUnsupportedVersion is not null && supportedPlatformName == unsupportedPlatformName) {
					var macCatalystEarlyBirds = platform == ApplePlatform.MacCatalyst && apiUnsupportedVersion == new Version (14, 0);
					if (!macCatalystEarlyBirds)
						failures.Add ($"[FAIL] {api.AsFullName ()} is marked both supported and unsupported in the same version ({supportedPlatformName})");
				}

				// APIs shouldn't become obsolete in the same version they become supported, although Apple does that somewhat frequently, so we have an escape hatch here.
				//     [SupportedOSPlatform("ios12.0")]
				//     [ObsoletedOSPlatform("ios12.0")]
				if (apiSupportedVersion is not null && apiObsoletedVersion is not null && supportedPlatformName == obsoletedPlatformName && !SkipSupportedAndObsoleteAtTheSameTime (api, platform, apiObsoletedVersion))
					failures.Add ($"[FAIL] {api.AsFullName ()} is marked both supported and obsoleted in the same version ({supportedPlatformName})");

				// If there's an ObsoleteOSPlatform, there must also be a SupportedOSPlatform.
				if (apiSupportedAttribute is null && apiObsoletedAttribute is not null)
					failures.Add ($"[FAIL] {api.AsFullName ()} is obsoleted (in {obsoletedPlatformName}), but does not have a SupportedOSPlatform attribute.");

				// If there's an UnsupportedOSPlatform with version, there must also be a SupportedOSPlatform.
				if (apiSupportedAttribute is null && apiUnsupportedVersion is not null)
					failures.Add ($"[FAIL] {api.AsFullName ()} is unsupported (in {apiUnsupportedVersion}), but does not have a SupportedOSPlatform attribute.");

				// APIs are first obsoleted, then unsupported.
				// Invalid (unsupported before obsoleted)
				//     [ObsoletedOSPlatform ("ios12.0")]
				//     [UnsupportedOSPlatform ("ios11.0")]
				// or (unsupported at the same time as obsoleted)
				//     [ObsoletedOSPlatform ("ios12.0")]
				//     [UnsupportedOSPlatform ("ios12.0")]
				if (apiUnsupportedVersion is not null && apiObsoletedVersion is not null && apiUnsupportedVersion <= apiObsoletedVersion)
					failures.Add ($"[FAIL] {api.AsFullName ()} can only be marked unsupported (in {unsupportedPlatformName}) after it's obsoleted (in {obsoletedPlatformName})");

				// If an API is just unavailable, it shouldn't be here in the first place.
				//     [UnsupportedOSPlatform ("ios")]
				// Exceptions:
				// * If the API is obsolete, or has EditorBrowsable (Never), then we skip this check (it's likely a mistake of some sort).
				// * We expose enum values that aren't supported on a given platform for error enums.
				if (apiUnsupportedAttribute is not null && apiUnsupportedVersion is null && !(api.IsObsolete () || api.HasEditorBrowseableNeverAttribute ())) {
					if (!IsEnumField (api))
						failures.Add ($"[FAIL] {api.AsFullName ()} is marked unsupported: \"{unsupportedPlatformName}\" {api.RenderLocation ()}");
				}

				// If an API has any availabily attributes, it must have at least a SupportedOSPlatform attribute (no API can be only obsoleted or unsupported, it must also have been supported at some point).
				// Exceptions:
				// * If the API is obsolete, or has EditorBrowsable (Never), then we skip this check (it's likely a mistake of some sort).
				// * We expose enum values that aren't supported on a given platform for error enums.
				if (apiSupportedAttribute is null && (apiObsoletedAttribute is not null || apiUnsupportedAttribute is not null) && !(api.IsObsolete () || api.HasEditorBrowseableNeverAttribute ())) {
					if (!IsEnumField (api))
						failures.Add ($"[FAIL] {api.AsFullName ()} does not have a SupportedOSPlatform attribute for {platform}, but it's still: {string.Join (", ", new [] { apiObsoletedAttribute, apiUnsupportedAttribute }.Where (v => v is not null).Select (v => v!.AsOSPlatformAttributeString ()))}");
				}

				// The subsequent tests are limited to members of the current API, so just continue looping if we're not a type.
				if (!(api is TypeDefinition type))
					continue;

				// Verify that no the members in a type don't contradict attributes on the type.
				foreach (var member in type.EnumerateAttributeProviders ()) {
					var memberAvailability = member.GetAvailabilityAttributes (platform);
					if (memberAvailability is null)
						continue;

					var memberSupportedAttribute = memberAvailability.Supported?.Attribute;
					var memberUnsupportedAttribute = memberAvailability.Unsupported?.Attribute;
					var memberSupportedVersion = memberAvailability.Supported?.Version;

					// Check that the member must be marked unsupported if the type is
					if (apiUnsupportedAttribute is not null && memberSupportedAttribute is not null && memberUnsupportedAttribute is null)
						failures.Add ($"{member.AsFullName ()} is marked available in {memberSupportedVersion} with '{memberSupportedAttribute.AsOSPlatformAttributeString ()}', but the declaring type {type.FullName} is marked unavailable in {apiUnsupportedVersion} with '{apiUnsupportedAttribute.AsOSPlatformAttributeString ()}'");

					// Check that the member isn't supported before the type.
					if (apiSupportedVersion is not null && memberSupportedVersion is not null && memberSupportedVersion < apiSupportedVersion)
						failures.Add ($"{member.AsFullName ()} is marked available with '{memberSupportedVersion}', but the declaring type {type.FullName} is only available in '{apiSupportedVersion}'");
				}
			}

			Helper.AssertFailures (failures, knownConsistencyIssues, nameof(AttributeConsistency), "API with inconsistent availability attributes");
		}

		static bool IsEnumField (ICustomAttributeProvider api)
		{
			if (!(api is FieldDefinition fd))
				return false;

			return fd.DeclaringType.BaseType.Is ("System", "Enum");
		}

		bool SkipSupportedAndObsoleteAtTheSameTime (ICustomAttributeProvider api, ApplePlatform platform, Version version)
		{
			var fullname = api.AsFullName ();

			switch (fullname) {
			case "SceneKit.SCNAnimationPlayer.SetSpeed(System.Runtime.InteropServices.NFloat, Foundation.NSString)":
				// SetSpeed is in the SCNAnimatable protocol, which was added in iOS 8.0.
				// The SetSpeed method was added in iOS 10.0, and deprecated in iOS 11.
				// The SCNAnimatable protocol is implemented by the SCNAnimationPlayer class, which was added in iOS 11.
				// Thus it's expected that the method was introduced and deprecated in the same OS version.
				return true;
			}

			switch (platform) {
			case ApplePlatform.iOS:
				switch (fullname) {
				case "CarPlay.CPApplicationDelegate.DidDiscardSceneSessions(UIKit.UIApplication, Foundation.NSSet`1<UIKit.UISceneSession>)":
				case "CarPlay.CPApplicationDelegate.GetConfiguration(UIKit.UIApplication, UIKit.UISceneSession, UIKit.UISceneConnectionOptions)":
				case "Intents.INNoteContentTypeResolutionResult.GetConfirmationRequired(Foundation.NSObject, System.IntPtr)":
				case "Intents.INNoteContentTypeResolutionResult.GetUnsupported(System.IntPtr)":
				case "PdfKit.PdfAnnotation..ctor(CoreGraphics.CGRect)":
				case "SceneKit.SCNAnimationPlayer.GetAnimation(Foundation.NSString)":
				case "SceneKit.SCNAnimationPlayer.IsAnimationPaused(Foundation.NSString)":
				case "SceneKit.SCNAnimationPlayer.PauseAnimation(Foundation.NSString)":
				case "SceneKit.SCNAnimationPlayer.RemoveAnimation(Foundation.NSString, System.Runtime.InteropServices.NFloat)":
				case "SceneKit.SCNAnimationPlayer.ResumeAnimation(Foundation.NSString)":
					return true;
				}
				break;
			case ApplePlatform.TVOS:
				switch (fullname) {
				case "SceneKit.SCNAnimationPlayer.GetAnimation(Foundation.NSString)":
				case "SceneKit.SCNAnimationPlayer.IsAnimationPaused(Foundation.NSString)":
				case "SceneKit.SCNAnimationPlayer.PauseAnimation(Foundation.NSString)":
				case "SceneKit.SCNAnimationPlayer.RemoveAnimation(Foundation.NSString, System.Runtime.InteropServices.NFloat)":
				case "SceneKit.SCNAnimationPlayer.ResumeAnimation(Foundation.NSString)":
					return true;
				}
				break;
			case ApplePlatform.MacOSX:
				switch (fullname) {
				case "HealthKit.HKQuantity HealthKit.HKWorkout::TotalDistance()":
				case "HealthKit.HKQuantity HealthKit.HKWorkout::TotalEnergyBurned()":
				case "HealthKit.HKQuantity HealthKit.HKWorkout::TotalFlightsClimbed()":
				case "HealthKit.HKQuantity HealthKit.HKWorkout::TotalSwimmingStrokeCount()":
				case "HealthKit.HKWorkout.get_TotalDistance()":
				case "HealthKit.HKWorkout.get_TotalEnergyBurned()":
				case "HealthKit.HKWorkout.get_TotalFlightsClimbed()":
				case "HealthKit.HKWorkout.get_TotalSwimmingStrokeCount()":
				case "HealthKit.HKCumulativeQuantitySeriesSample":
				case "HealthKit.HKCumulativeQuantitySeriesSample.get_Sum()":
				case "HealthKit.HKQuantity HealthKit.HKCumulativeQuantitySeriesSample::Sum()":
				case "Intents.INCallRecord.get_Caller()":
				case "Intents.INPerson Intents.INCallRecord::Caller()":
				case "Intents.INSendMessageIntent..ctor(Intents.INPerson[], System.String, Intents.INSpeakableString, System.String, System.String, Intents.INPerson)":
				case "Intents.INSendMessageIntent..ctor(Intents.INPerson[], System.String, System.String, System.String, Intents.INPerson)":
				case "Intents.INSendMessageIntent.get_GroupName()":
				case "Intents.INSendMessageIntentHandling_Extensions.ResolveGroupName(Intents.IINSendMessageIntentHandling, Intents.INSendMessageIntent, System.Action`1<Intents.INStringResolutionResult>)":
				case "Security.SecSharedCredential.RequestSharedWebCredential(System.String, System.String, System.Action`2<Security.SecSharedCredentialInfo[],Foundation.NSError>)":
				case "System.String Intents.INSendMessageIntent::GroupName()":
					return true;
				}
				break;
			case ApplePlatform.MacCatalyst:
				// Apple added a lot of new frameworks to Mac Catalyst 13.1 and 14.0 that already had obsoleted
				// API from other platforms, so just ignore anything that was obsoleted in Mac Catalyst 14.0 or earlier.
				if (version <= new Version (14, 0))
					return true;
				break;
			}

			// Common for all platforms.
			switch (fullname) {
			case "GameKit.GKScore.ReportLeaderboardScores(GameKit.GKLeaderboardScore[], GameKit.GKChallenge[], System.Action`1<Foundation.NSError>)":
			case "GameKit.GKScore.ReportLeaderboardScoresAsync(GameKit.GKLeaderboardScore[], GameKit.GKChallenge[])":
				return true;
			}

			// Print out the line to potentially add here.
			Console.WriteLine ($"case \"{fullname}\":");

			return false;
		}
	}
}
