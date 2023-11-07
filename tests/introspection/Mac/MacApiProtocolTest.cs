//
// Test the generated API for common protocol support
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

using System;

using Foundation;
using AppKit;
using CoreImage;

using NUnit.Framework;
using Xamarin.Tests;

namespace Introspection {

	[TestFixture]
	public class MacApiProtocolTest : ApiProtocolTest {

		protected override bool Skip (Type type)
		{
#if !NET
			switch (type.Namespace) {
			case "Chip":
				// The Chip framework is not stable, it's been added and removed and added and removed a few times already, so just skip verifying the entire framework.
				// This is legacy Xamarin only, because we removed the framework for .NET.
				return true;
			}
#endif

			switch (type.Name) {
#if !NET
			case "NSDraggingInfo":
				return true; // Incorrectly bound (BaseType on protocol), will be fixed for .NET.
#endif
			// special cases wrt sandboxing
			case "NSRemoteOpenPanel":
			case "NSRemoteSavePanel":
				return true;
			case "AVCaptureSynchronizedDataCollection":
			case "AVCaptureSynchronizedData":
			case "CXProvider":
				return TestRuntime.IsVM; // skip only on vms
#if !NET // NSMenuView does not exist in .NET
			case "NSMenuView": // not longer supported
				return true;
#endif // !NET
			case "ASAuthorizationProviderExtensionRegistrationHandler":
				return true;
			default:
				return base.Skip (type);
			}
		}

		protected override bool Skip (Type type, string protocolName)
		{
			switch (protocolName) {
			case "NSSecureCoding":
				switch (type.Name) {
				case "CNContactFormatter": // iOS declares but macOS header does not. Failure was missed :(
				case "NSCachedImageRep": // Not declared in header file
				case "NSCIImageRep":
				case "NSCustomImageRep":
				case "NSEPSImageRep":
				case "NSBitmapImageRep":
				case "NSImageRep":
				case "NSPdfImageRep":
				case "AVAssetTrackSegment": // Not declared in header file
				case "AVComposition": // Not declared in header file
				case "AVMutableComposition": // Not declared in header file
				case "AVCompositionTrackSegment": // Not declared in header file
				case "MKMapSnapshotOptions": // Not declared in header file
				case "NSTextTab": // Not declared in header file
				case "NSTextList": // Not declared in header file
				case "SFSafariPage": // Not declared in header file
				case "SFSafariPageProperties": // Not declared in header file
				case "SFSafariTab": // Not declared in header file
				case "SFSafariToolbarItem": // Not declared in header file
				case "SFSafariWindow": // Not declared in header file
				case "SFContentBlockerState": // Not declared in header file
				case "NEFlowMetaData": // Not declared in header file
				case "MKMapItem": // Not declared in header file
				case "CAConstraintLayoutManager": // Not declared in header file
				case "NSQueryGenerationToken": // Declared in header file but SupportsSecureCoding returns false - radar 32856944
				case "NSPersistentHistoryToken": // Conformance not in headers
				case "NSPropertyDescription": // NSPropertyDescription and children does not declare in header file
				case "NSAttributeDescription":
				case "NSExpressionDescription":
				case "NSFetchedPropertyDescription":
				case "NSRelationshipDescription":
				case "NSEntityDescription": // Not declared in header file
				case "NSFetchIndexDescription": // Not declared in header file
				case "NSFetchIndexElementDescription": // Not declared in header file
				case "NSFetchRequest": // Not declared in header file
				case "NSManagedObjectModel": // Not declared in header file
				case "NSUserInterfaceCompressionOptions": // Not declared in header file
														  // Xcode 10 (running on macOS 10.14)
				case "NSTextAlternatives":
				case "QTDataReference": // no header files anymore for deprecated QuickTime
				case "NSTextBlock":
				case "NSTextTable":
				case "NSTextTableBlock":
				// Xcode 11 (running on macOS 10.15)
				case "LAContext": // Conformance not in headers
				case "ICHandle": // Conformance not in headers
				case "ICNotification": // Conformance not in headers
				case "ICNotificationManagerConfiguration": // Conformance not in headers
				case "MKPolygon": // Conformance not in headers
				case "MLPredictionOptions": // Conformance not in headers
				case "NSUrlSessionTaskMetrics": // Conformance not in headers
				case "NSUrlSessionTaskTransactionMetrics": // Conformance not in headers
				case "NSFileProviderDomain": // Conformance not in headers
				case "FPUIActionExtensionContext": // Conformance not in headers
					return true;
				// macOS 10.15.2
				case "NSPrintInfo": // Conformance not in headers
				case "NSPrinter": // Conformance not in headers
					return true;
				// Xcode 12.5
				case "CXCall": // Conformance not in headers
				case "CXCallUpdate": // Conformance not in headers
				case "CXProviderConfiguration": // Conformance not in headers
					return true;
				// xcode 13 / macOS 12
				case "OSLogEntry":
				case "OSLogEntryActivity":
				case "OSLogEntryBoundary":
				case "OSLogEntryLog":
				case "OSLogEntrySignpost":
				case "OSLogMessageComponent":
				case "NSImageSymbolConfiguration":
				case "NSMergePolicy":
				case "MEComposeSession":
				case "MEComposeContext":
				// xcode 14
				case "SNClassifySoundRequest":
				case "PKInk":
				case "VSUserAccount":
				case "SCContentFilter":
				case "SCDisplay":
				case "SCRunningApplication":
				case "SCWindow":
				case "HKAudiogramSensitivityPoint":
				// xcode 15
				case "NSCompositeAttributeDescription":
				case "CKSyncEnginePendingDatabaseChange":
				case "CKSyncEnginePendingRecordZoneChange":
				case "CKSyncEnginePendingZoneDelete":
				case "CKSyncEnginePendingZoneSave":
				case "CKSyncEngineState":
				case "NSCursor":
					return true;
				default:
					// CIFilter started implementing NSSecureCoding in 10.11
					if (!Mac.CheckSystemVersion (10, 11) && (type == typeof (CIFilter) || type.IsSubclassOf (typeof (CIFilter))))
						return true;
					break;
				}
				break;
			case "NSCopying":
				switch (type.Name) {
				case "WKPreferences": // Not declared in header file
				case "DomNodeFilter": // Not declared in header file
				case "MKDirectionsRequest": // Not declared in header file
				case "EKObject": // Not declared in header file
				case "EKCalendarItem": // Not declared in header file
				case "EKSource": // Not declared in header file
				case "EKCalendar": // Not declared in header file
				case "EKEvent": // Not declared in header file
				case "EKReminder": // Not declared in header file
				case "ACAccount": // Not declared in header file
				case "NEFlowMetaData": // Not declared in header file
				case "ACAccountCredential": // Not declared in header file
											// Xcode 11 (running on macOS 10.15)
				case "NSCollectionViewUpdateItem": // Not declared in header file
				case "MLPredictionOptions": // Not declared in header file
				case "FPUIActionExtensionContext": // Conformance not in headers
												   // Xcode 12.5
				case "CXCall": // Conformance not in headers
					return true;
				// xcode 13 / macOS 12
				case "PHCloudIdentifier":
				case "NSMergePolicy":
				case "NSEntityMapping":
				case "NSMappingModel":
				case "NSPropertyMapping":
				case "HMAccessoryOwnershipToken":
				case "MEComposeSession":
				// xcode 14
				case "SNClassifySoundRequest":
				case "SCContentFilter":
				case "SCDisplay":
				case "SCRunningApplication":
				case "SCStreamConfiguration":
				case "SCWindow":
				case "HKAudiogramSample":
				case "HKCategorySample":
				case "HKCdaDocumentSample":
				case "HKCorrelation":
				case "HKCumulativeQuantitySample":
				case "HKDiscreteQuantitySample":
				case "HKDocumentSample":
				case "HKElectrocardiogram":
				case "HKObject":
				case "HKQuantitySample":
				case "HKSample":
				case "HKWorkout":
				// xcode 15
				case "MEMessage":
				case "CKSyncEnginePendingDatabaseChange":
				case "CKSyncEnginePendingRecordZoneChange":
				case "CKSyncEnginePendingZoneDelete":
				case "CKSyncEnginePendingZoneSave":
					return true;
				}
				break;
			case "NSMutableCopying":
				switch (type.Name) {
				case "EKObject": // Not declared in header file
				case "EKCalendarItem": // Not declared in header file
				case "EKSource": // Not declared in header file
				case "EKStructuredLocation": // Not declared in header file
				case "EKAlarm": // Not declared in header file
				case "EKCalendar": // Not declared in header file
				case "EKEvent": // Not declared in header file
				case "EKParticipant": // Not declared in header file
				case "EKRecurrenceRule": // Not declared in header file
				case "EKReminder": // Not declared in header file
				case "INPerson": // Not declared in header file
					return true;
				// xcode 13 / macOS 12
				case "NSMergePolicy":
				case "UNNotificationSettings":
					return true;
				}
				break;
			case "NSCoding":
				switch (type.Name) {
				case "EKObject": // Not declared in header file
				case "EKCalendarItem": // Not declared in header file
				case "EKSource": // Not declared in header file
				case "EKStructuredLocation": // Not declared in header file
				case "EKAlarm": // Not declared in header file
				case "EKCalendar": // Not declared in header file
				case "EKEvent": // Not declared in header file
				case "EKParticipant": // Not declared in header file
				case "EKRecurrenceRule": // Not declared in header file
				case "EKReminder": // Not declared in header file
				case "AVAssetTrackSegment": // Not declared in header file
				case "AVComposition": // Not declared in header file
				case "AVMutableComposition": // Not declared in header file
				case "AVCompositionTrackSegment": // Not declared in header file
				case "MKMapSnapshotOptions": // Not declared in header file
				case "SFContentBlockerState": // Not declared in header file
				case "SFSafariPage": // Not declared in header file
				case "SFSafariPageProperties": // Not declared in header file
				case "SFSafariTab": // Not declared in header file
				case "SFSafariToolbarItem": // Not declared in header file
				case "SFSafariWindow": // Not declared in header file
				case "NEFlowMetaData": // Not declared in header file
				case "MKMapItem": // Not declared in header file
				case "NSConstraintConflict": // Not declared in header file
				case "NSQueryGenerationToken": // Declared in header file but SupportsSecureCoding returns false - radar 32856944
				case "NSPersistentHistoryToken": // Conformance not in headers
												 // Xcode 10 (running on macOS 10.14)
				case "NSTextAlternatives":
				// Xcode 11 (running on macOS 10.15)
				case "ICHandle": // Conformance not in headers
				case "ICNotification": // Conformance not in headers
				case "ICNotificationManagerConfiguration": // Conformance not in headers
				case "LAContext": // Conformance not in headers
				case "MKPolygon": // Conformance not in headers
				case "MLPredictionOptions": // Conformance not in headers
				case "NSUrlSessionTaskMetrics": // Conformance not in headers
				case "NSUrlSessionTaskTransactionMetrics": // Conformance not in headers
				case "NSFileProviderDomain": // Conformance not in headers
				case "FPUIActionExtensionContext": // Conformance not in headers
												   // Xcode 12.5
				case "CXCall": // Conformance not in headers
				case "CXCallUpdate": // Conformance not in headers
				case "CXProviderConfiguration": // Conformance not in headers
					return true;
				// xcode 13 / macOS 12
				case "OSLogEntry":
				case "OSLogEntryActivity":
				case "OSLogEntryBoundary":
				case "OSLogEntryLog":
				case "OSLogEntrySignpost":
				case "OSLogMessageComponent":
				case "NSImageSymbolConfiguration":
				case "NSMergePolicy":
				case "MEComposeContext":
				// xcode 14 
				case "SNClassifySoundRequest":
				case "PKInk":
				case "VSUserAccount":
				case "SCContentFilter":
				case "SCDisplay":
				case "SCRunningApplication":
				case "SCWindow":
				case "HKAudiogramSensitivityPoint":
				// xcode 15
				case "CKSyncEnginePendingDatabaseChange":
				case "CKSyncEnginePendingRecordZoneChange":
				case "CKSyncEnginePendingZoneDelete":
				case "CKSyncEnginePendingZoneSave":
				case "CKSyncEngineState":
					return true;
				}
				break;
			case "NSWindowRestoration":
				switch (type.Name) {
				case "NSDocumentController":
					// There's a category that implements the NSWindowRestoration protocol for NSDocumentController,
					// but that apparently doesn't make conformsToProtocol: return true.
					// '@interface NSDocumentController (NSWindowRestoration) <NSWindowRestoration>'
					return true;
				}
				break;
			case "NSUserInterfaceItemIdentification":
				// NSViewController started implementing NSUserInterfaceItemIdentification in 10.10
				if (!Mac.CheckSystemVersion (10, 10) && (type == typeof (NSViewController) || type.IsSubclassOf (typeof (NSViewController))))
					return true;

				switch (type.Name) {
				case "NSMenuItem":
					if (!Mac.CheckSystemVersion (10, 12))
						return true;
					break;
				}
				break;
			case "NSMenuDelegate":
				switch (type.Name) {
				case "PdfView":
					if (!Mac.CheckSystemVersion (10, 10))
						return true;
					break;
				}
				break;
			case "NSTextFinderClient": // Not listed in header, nor conformsToProtocol, but works in sample. But it just repondsToSelectors
				if (type.Name == "NSTextView")
					return true;
				break;
#if !NET
			case "NSDraggingInfo":
				return true; // We have to keep the type to maintain backwards compatibility.
#endif
			case "NSAccessibility":
			case "NSAccessibilityElement":
				switch (type.Name) {
				case "NSMenu":
				case "NSMenuItem":
					if (!Mac.CheckSystemVersion (10, 12))
						return true;
					break;
				}
				break;
			case "NSAnimationDelegate":
			case "NSAnimatablePropertyContainer":
				switch (type.Name) {
				case "NSTitlebarAccessoryViewController":
					return true;
				}
				break;
			case "NSCandidateListTouchBarItemDelegate":
			case "NSTouchBarDelegate":
				switch (type.Name) {
				case "NSTextView":
					return true;
				}
				break;
			case "NSExtensionRequestHandling":
				return IntPtr.Size == 4;
			case "NSAppearanceCustomization":
				switch (type.Name) {
				case "NSPopover":
					if (!Mac.CheckSystemVersion (10, 13) || IntPtr.Size == 4) // Was added in 10.13
						return true;
					break;
				case "NSApplication":
					if (!Mac.CheckSystemVersion (10, 14)) // Was added in 10.14
						return true;
					break;
				case "NSMenu":
					if (!Mac.CheckSystemVersion (11, 0))
						return true;
					break;
				}
				break;
			case "NSUserInterfaceValidations":
				switch (type.Name) {
				case "NSSplitViewController":
					if (!Mac.CheckSystemVersion (10, 13)) // Was added in 10.13
						return true;
					break;
				}
				break;
			case "NSItemProviderWriting":
				switch (type.Name) {
				case "NSMutableString":
				case "NSString":
				case "NSUrl":
					if (IntPtr.Size == 4) // Only on 64-bit version of these types
						return true;
					break;
				}
				break;
			case "CAAction":
				switch (type.Name) {
				case "NSNull":
					if (!Mac.CheckSystemVersion (10, 11)) // NSNull started implementing the CAAction protocol in 10.11
						return true;
					break;
				}
				break;
			case "NSTextContent":
				switch (type.Name) {
				case "NSTextField":
				case "NSTextView":
				case "NSTokenField":
				case "NSComboBox":
				case "NSSearchField":
				case "NSSecureTextField":
					if (!Mac.CheckSystemVersion (11, 0))
						return true;
					break;

				}
				break;
			}

			switch (type.Name) {
#if !XAMCORE_3_0
			case "NSRemoteSavePanel":
			case "NSRemoteOpenPanel":
				return true; // These two classes don't show up in any documentation.
#endif
			}

			switch (type.Namespace) {
			case "MonoMac.SceneKit":
			case "SceneKit":
				// skip on 32 bits but continue otherwise
				if (IntPtr.Size == 4)
					return true;
				break;
			}

			return base.Skip (type, protocolName);
		}

		[Test]
		public override void SecureCoding ()
		{
			if (!Mac.CheckSystemVersion (10, 8))
				return;

			base.SecureCoding ();
		}

		[Test]
		public override void SupportsSecureCoding ()
		{
			if (!Mac.CheckSystemVersion (10, 8))
				return;

			base.SupportsSecureCoding ();
		}
	}
}
