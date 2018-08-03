//
// Test the generated API for common protocol support
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

using System;

#if XAMCORE_2_0
using Foundation;
using AppKit;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.CoreImage;
#endif

using NUnit.Framework;
using Xamarin.Tests;

namespace Introspection {

	[TestFixture]
	public class MonoMacFixtures : ApiProtocolTest {

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
					return true;
				default:
					// CIFilter started implementing NSSecureCoding in 10.11
					if (!Mac.CheckSystemVersion (10, 11) && (type == typeof(CIFilter) || type.IsSubclassOf (typeof(CIFilter))))
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
				if (!Mac.CheckSystemVersion (10, 10) && (type == typeof(NSViewController) || type.IsSubclassOf (typeof (NSViewController))))
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
#if !XAMCORE_4_0
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
			if (!Mac.CheckSystemVersion (10,8))
				return;

			base.SupportsSecureCoding ();
		}
	}
}
