//
// Mac specific selector validators
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Reflection;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif

using NUnit.Framework;
using Xamarin.Tests;

namespace Introspection {
	
	[TestFixture]
	public class MacApiSelectorTest : CoreSelectorTest {
		
		static MacApiSelectorTest ()
		{
			Runtime.RegisterAssembly (typeof (NSObject).Assembly);
		}

		public MacApiSelectorTest ()
		{
			//LogProgress = true;    Don't uncomment this anymore, export API_TEST_LOG_PROGRESS=1
			ContinueOnFailure = true;
		}

		protected override bool Skip (Type type)
		{
			switch (type.FullName) {
			case "MonoMac.AppKit.NSWindowTabGroup":
			case "AppKit.NSWindowTabGroup":
				return true; /// 32930276
			case "MonoMac.CIFilter.CIMaskedVariableBlur": // Appears to be missing from 10.11, not documented
			case "CIFilter.CIMaskedVariableBlur":
				if (Mac.CheckSystemVersion (10, 11))
					return true;
				break;
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
				// These classes are available in 32-bit Yosemite, or 32+64bit Mavericks.
				if (IntPtr.Size == 4 && !Mac.CheckSystemVersion (10, 10))
					return true;
				break;
			case "MonoMac.AppKit.NSSharingService":
			case "AppKit.NSSharingService":
			case "MonoMac.AppKit.NSSharingServicePicker":
			case "AppKit.NSSharingServicePicker":
			case "MonoMac.Foundation.NSByteCountFormatter":
			case "Foundation.NSByteCountFormatter":
			case "MonoMac.Foundation.NSUserNotification":
			case "Foundation.NSUserNotification":
			case "MonoMac.Foundation.NSUserNotificationCenter":
			case "Foundation.NSUserNotificationCenter":
			case "MonoMac.AVFoundation.AVPlayerItemOutput":
			case "AVFoundation.AVPlayerItemOutput":
			case "MonoMac.AVFoundation.AVPlayerItemVideoOutput":
			case "AVFoundation.AVPlayerItemVideoOutput":
			case "MonoMac.Foundation.NSUuid":
			case "Foundation.NSUuid":
				if (!Mac.CheckSystemVersion (10, 8))
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
			// not installed by default
			case "MonoMac.Growl":
			case "Growl":
				return true;
			}

			return base.Skip (type);
		}

		protected override bool Skip (Type type, string selectorName)
		{
			if (SkipAccessibilitySelectors (type, selectorName))
				return true;
			
			switch (selectorName) {
			case "encodeWithCoder:":
				switch (type.Name) {
				case "CNContactFetchRequest":
				case "NWEndpoint":
				case "GKEntity":
				case "GKPolygonObstacle":
				case "GKComponent":
				case "GKGraph":
				case "GKGraphNode":
				case "GKAgent2D":
				case "GKAgent":
					if (!Mac.CheckSystemVersion (10, 12)) // NSCoding was added in 10.12
						return true;
					break;
				}
				break;
			case "accessibilityNotifiesWhenDestroyed":
				// The header declares this on an NSObject category but 
				// it doesn't even respondsToSelector on NSView/NSCell...
				return true;
#if !XAMCORE_4_0
			case "xamarinselector:removed:":
				return true;
#endif
#if !XAMCORE_3_0
			case "initWithPasteboardPropertyList:ofType:":
				// This is a broken binding, but it's an abstract protocol
				// method, so there's no way to remove it without breaking
				// compat.
				return true;
#endif
			case "waitUntilExit":
				// category, NSTask won't respond -> @interface NSTask (NSTaskConveniences)
				if (type.Name == "NSTask")
					return true;
				break;
			case "readInBackgroundAndNotifyForModes:":
			case "readInBackgroundAndNotify":
			case "readToEndOfFileInBackgroundAndNotifyForModes:":
			case "readToEndOfFileInBackgroundAndNotify":
			case "acceptConnectionInBackgroundAndNotifyForModes:":
			case "acceptConnectionInBackgroundAndNotify":
			case "waitForDataInBackgroundAndNotifyForModes:":
			case "waitForDataInBackgroundAndNotify":
				// category, NSFileHandle won't respond -> @interface NSFileHandle (NSFileHandleAsynchronousAccess)
				if (type.Name == "NSFileHandle")
					return true;
				break;
			// initWithPlayerIDs: works (valid handle) and is documented - but does not respond when queried
			case "initWithPlayerIDs:":
				if (type.Name == "GKLeaderboard")
					return true;
				break;
			// some types had NSCopying added after they were first introduced
			case "copyWithZone:":
				switch (type.Name) {
				case "MDLTransform":
				case "NWEndpoint":
				case "GKBehavior":
				case "GKGraph":
					if (!Mac.CheckSystemVersion (10, 12)) // NSCopying was added in 10.12
						return true;
					break;
				case "CBPeripheral":
					if (!Mac.CheckSystemVersion (10, 9))
						return true;
					break;
				}
				break;
			case "readingOptionsForType:pasteboard:":
			case "writingOptionsForType:pasteboard:":
				return true; // Optional selectors on NSPasteboardReading/NSPasteboardWriting
			// Xcode 9.4 removed both selectors from MPSCnnBinaryKernel, reported radar https://trello.com/c/7EAM0qk1
			// but apple says this was intentional.
			case "kernelHeight":
			case "kernelWidth":
				switch (type.Name) {
				case "MPSCnnBinaryKernel":
					return true;
				}
				break;
			}

			switch (type.Namespace) {
			// Notifications seems to follow a pattern were selectors are routed to an internal concrete type
			case "MonoMac.Foundation":
			case "Foundation":
				switch (type.Name) {
				// looks like it's routed to (private) NSConcreteUserNotificationCenter
				case "NSUserNotificationCenter":
					switch (selectorName) {
					case "delegate":
					case "setDelegate:":
					case "scheduledNotifications":
					case "setScheduledNotifications:":
					case "deliveredNotifications":
						return true;
					}
					break;
				// looks like it's routed to (private) NSConcreteUserNotification
				case "NSUserNotification":
					switch (selectorName) {
					case "title":
					case "setTitle:":
					case "subtitle":
					case "setSubtitle:":
					case "informativeText":
					case "setInformativeText:":
					case "actionButtonTitle":
					case "setActionButtonTitle:":
					case "userInfo":
					case "setUserInfo:":
					case "deliveryDate":
					case "setDeliveryDate:":
					case "deliveryTimeZone":
					case "setDeliveryTimeZone:":
					case "deliveryRepeatInterval":
					case "setDeliveryRepeatInterval:":
					case "actualDeliveryDate":
					case "isPresented":
					case "isRemote":
					case "soundName":
					case "setSoundName:":
					case "hasActionButton":
					case "setHasActionButton:":
					case "activationType":
					case "otherButtonTitle":
					case "setOtherButtonTitle:":
					case "additionalActions":
					case "setAdditionalActions:":
					case "additionalActivationAction":
					case "contentImage":
					case "hasReplyButton":
					case "setHasReplyButton:":
					case "identifier":
					case "setIdentifier:":
					case "response":
					case "responsePlaceholder":
					case "setResponsePlaceholder:":
						return true;
					}
					break;
				// looks like it's routed to (private) NSConcreteUserNotificationAction
				case "NSUserNotificationAction":
					switch (selectorName) {
					case "identifier":
					case "title":
						return true;
					}
					break;
				case "NSFileHandle": //Fails on Lion
				case "NSUrlAuthenticationChallenge":
				case "NSUrlCredential":
				case "NSUrlProtectionSpace":
				case "NSAppleEventDescriptor":
					if (selectorName == "encodeWithCoder:" && !Mac.CheckSystemVersion (10, 8))
						return true;
					break;
				case "NSValue":
					switch (selectorName) {
					case "SCNMatrix4Value":
					case "SCNVector3Value":
					case "SCNVector4Value":
					case "valueWithSCNMatrix4:":
						if (IntPtr.Size != 8)
							return true;
						break;
					}
					break;
				case "NSUrlSession":
					switch (selectorName) {
					case "delegateQueue":
					case "sessionDescription":
					case "setSessionDescription:":
					case "delegate":
						if (Mac.CheckSystemVersion (10, 11))
							return true;
						break;
					}
					break;
				case "NSUrlSessionStreamTask":
					switch (selectorName) {
					case "captureStreams":
					case "closeRead":
					case "closeWrite":
					case "readDataOfMinLength:maxLength:timeout:completionHandler:":
					case "startSecureConnection":
					case "stopSecureConnection":
					case "writeData:timeout:completionHandler:":
						if (Mac.CheckSystemVersion (10, 11))
							return true;
						break;
					}
					break;
				case "NSUrlSessionTask":
					switch (selectorName) {
					case "countOfBytesExpectedToReceive":
					case "countOfBytesExpectedToSend":
					case "countOfBytesReceived":
					case "countOfBytesSent":
					case "currentRequest":
					case "error":
					case "originalRequest":
					case "response":
					case "state":
					case "taskDescription":
					case "setTaskDescription:":
					case "taskIdentifier":
						if (Mac.CheckSystemVersion (10, 11))
							return true;
						break;
					case "earliestBeginDate":
					case "setEarliestBeginDate:":
					case "countOfBytesClientExpectsToSend":
					case "setCountOfBytesClientExpectsToSend:":
					case "countOfBytesClientExpectsToReceive":
					case "setCountOfBytesClientExpectsToReceive:":
						if (Mac.CheckSystemVersion (10, 13))
							return true;
						break;
					}
					break;
				case "NSUrlSessionConfiguration":
					if (Mac.IsAtLeast (10, 11))
						return true;
					break;
				}
				break;
			case "MonoMac.AppKit":
			case "AppKit":
				switch (type.Name) {
				case "NSMenu":
				case "NSMenuItem":
					switch (selectorName) {
					case "accessibilityAddChildElement:":
					case "accessibilityElementWithRole:frame:label:parent:":
						// Defined in header NSAccessibilityElement.h for NSAccessibilityElement which they implement, in theory
						return true;
					}
					break;

#if !XAMCORE_3_0		// These should be not be marked [Abstract] but can't fix w/o breaking change...
				case "NSScrollView":
				case "NSTextView":
					switch (selectorName) {
					case "contentViewAtIndex:effectiveCharacterRange:":
					case "didReplaceCharacters":
					case "drawCharactersInRange:forContentView:":
					case "rectsForCharacterRange:":
					case "replaceCharactersInRange:withString:":
					case "scrollRangeToVisible:":
					case "shouldReplaceCharactersInRanges:withStrings:":
					case "stringAtIndex:effectiveRange:endsWithSearchBoundary:":
					case "stringLength":
					case "allowsMultipleSelection":
					case "isEditable":
					case "firstSelectedRange":
					case "isSelectable":
					case "selectedRanges":
					case "setSelectedRanges:":
					case "string":
					case "visibleCharacterRanges":
						return true;
					}
					break;
#endif
				case "NSMenuDelegate":
					switch (selectorName) {
#if !XAMCORE_3_0
					case "menu:willHighlightItem:":
						return true; // bound
#endif
					}
					break;
				case "NSResponder":
					switch (selectorName) {
					case "smartMagnifyWithEvent:":
					case "quickLookWithEvent:":
						if (!Mac.CheckSystemVersion (10, 8))
							return true;
						break;
					case "newWindowForTab:": // Cocoa checks to see if implemented in responder chain - " A plus button on tabbed windows will only be shown if this method exists in the responder chain."
						return true;
					}
					break;
				case "NSViewController":
					switch (selectorName) {
					case "identifier": // 
					case "setIdentifier:":
						// In Yosemite (but not before) NSViewController implements the NSUserInterfaceItemIdentification
						// protocol (which brings a r/w Identifier property). We don't have any way of expressing that
						// a type started implementing a protocol in a particular version, so just ignore these selectors. 
						if (!Mac.CheckSystemVersion (10, 10))
							return true;
						break;
					}
					break;
				}
				break;
			// GameKit seems to follow a pattern were selectors are routed to an internal concrete type
			case "MonoMac.GameKit":
			case "GameKit":
				switch (type.Name) {
				case "GKTurnBasedExchange":
					switch (selectorName) {
					case "completionDate":
					case "data":
					case "exchangeID":
					case "sendDate":
					case "timeoutDate":
						return true;
					}
					break;
				case "GKTurnBasedExchangeReply":
					switch (selectorName) {
					case "data":
					case "replyDate":
						return true;
					}
					break;
				// looks like it's routed to (private) GKDialogController_Concrete
				case "GKDialogController":
					switch (selectorName) {
					case "parentWindow":
					case "setParentWindow:":
						return true;
					}
					break;
				// looks like it's routed to (private) GKVoiceChat_Concrete
				case "GKVoiceChat":
					switch (selectorName) {
					case "start":
					case "stop":
					case "setMute:forPlayer:":
					case "name":
					case "isActive":
					case "setActive:":
					case "volume":
					case "setVolume:":
					case "playerStateUpdateHandler":
					case "setPlayerStateUpdateHandler:":
					case "playerIDs":
						return true;
					}
					break;
				// looks like it's routed to (private) GKLeaderboard_Concrete
				case "GKLeaderboard":
					switch (selectorName) {
					case "loadScoresWithCompletionHandler:":
					case "timeScope":
					case "setTimeScope:":
					case "playerScope":
					case "setPlayerScope:":
					case "maxRange":
					case "category":
					case "setCategory:":
					case "title":
					case "range":
					case "setRange:":
					case "scores":
					case "localPlayerScore":
					case "groupIdentifier":
					case "setGroupIdentifier:":
						return true;
					}
					break;
				// looks like it's routed to (private) GKLeaderboard_Concrete
				case "GKLocalPlayer":
					switch (selectorName) {
					case "authenticateWithCompletionHandler:":
					case "loadDefaultLeaderboardCategoryIDWithCompletionHandler:":
					case "setDefaultLeaderboardCategoryID:completionHandler:":
					case "authenticateHandler":
					case "setAuthenticateHandler:":
					case "loadFriendsWithCompletionHandler:":
					case "isAuthenticated":
						return true;
					}
					break;
				// looks like it's routed to (private) GKMatch_Concrete
				case "GKMatch":
					switch (selectorName) {
					case "sendData:toPlayers:withDataMode:error:":
					case "sendDataToAllPlayers:withDataMode:error:":
					case "disconnect":
					case "voiceChatWithName:":
					case "playerIDs":
					case "delegate":
					case "setDelegate:":
					case "expectedPlayerCount":
						return true;
					}
					break;
				// looks like it's routed to (private) GKMatchmater_Concrete
				case "GKMatchmaker":
					switch (selectorName) {
					case "inviteHandler":
					case "setInviteHandler:":
					case "findMatchForRequest:withCompletionHandler:":
					case "findPlayersForHostedMatchRequest:withCompletionHandler:":
					case "addPlayersToMatch:matchRequest:completionHandler:":
					case "cancel":
					case "queryPlayerGroupActivity:withCompletionHandler:":
					case "queryActivityWithCompletionHandler:":
						return true;
					}
					break;
				}
				break;
			// Gone in Mavericks
			case "MonoMac.StoreKit":
			case "StoreKit":
				switch (type.Name) {
				case "SKPayment":
				case "SKMutablePayment":
					switch (selectorName) {
					case "paymentWithProductIdentifier:":
						if (Mac.CheckSystemVersion (10, 9))
							return true;
						break;
					}
					break;
				}
				break;
			case "MonoMac.PdfKit": // Bug 20232
			case "PdfKit":
				switch (type.Name) {
				case "PdfBorder": // Fails on Lion
				case "PdfAnnotation":
					if (selectorName == "encodeWithCoder:" && !Mac.CheckSystemVersion (10, 8))
						return true;
					break;
				case "PdfView":
					switch (selectorName) {
#if !XAMCORE_3_0					
					case "menu:willHighlightItem:":
						return true;
#endif
					}
					break;
				}
				break;
			case "MonoMac.SceneKit":
			case "SceneKit":
				switch (type.Name) {
				case "SCNGeometryElement":
					// Ignore on 10.8 where SCNGeometryPrimitiveType is int (32), but
					// on 10.9+ is NSInteger/nint (32/64). SceneKit is next to useless
					// on 10.8 anyway. -abock
					if (selectorName == "primitiveType" && Mac.CheckSystemVersion (10, 8))
						return true;
					// fall through
					goto case "SCNCamera";
				case "SCNCamera":
				case "SCNGeometry":
				case "SCNGeometrySource":
				case "SCNLight":
				case "SCNMaterial":
				case "SCNMaterialProperty":
				case "SCNNode":
				case "SCNProgram":
				case "SCNScene":
				case "SCNMorpher":
				case "SCNSkinner":
				case "SCNConstraint":
				case "SCNLevelOfDetail":
					// The NSSecureCoding protocol was added to these types in Yosemite,
					// and we can't (yet?) describe "type added protocol P in version X.Y"
					// with our AvailabilityAttribute, so do this check manually.
					if (selectorName == "encodeWithCoder:" && !Mac.CheckSystemVersion (10, 10))
						return true;
					break;
				}

				switch (type.Name) {
				case "SCNGeometry":
					// SCNGeometry added the SCNShadable protocol in 10.9, which brings in the 'program' selector.
					switch (selectorName) {
					case "program":
					case "setProgram:":
						if (!Mac.CheckSystemVersion (10, 9))
							return true;
						break;
					}
					break;
				}
				break;
			case "MonoMac.CoreBluetooth":
				switch (type.Name) {
				case "CBCentral":
				case "CBPeripheral":
					if (selectorName == "UUID" && Mac.CheckSystemVersion (10, 13)) // UUID removed from headers in 10.13
						return true;
					break;
				}
				break;
			}
			return base.Skip (type, selectorName);
		}

		public bool SkipAccessibilitySelectors (Type type, string selectorName)
		{
			switch (selectorName) {
			case "accessibilityAddChildElement:":
			case "accessibilityPerformCancel":
			case "accessibilityPerformConfirm":
			case "accessibilityPerformDecrement":
			case "accessibilityPerformDelete":
			case "accessibilityPerformIncrement":
			case "accessibilityPerformPick":
			case "accessibilityPerformPress":
			case "accessibilityPerformRaise":
			case "accessibilityPerformShowAlternateUI":
			case "accessibilityPerformShowDefaultUI":
			case "accessibilityPerformShowMenu":
			case "accessibilityAttributedStringForRange:":
			case "accessibilityCellForColumn:row:":
			case "accessibilityFrameForRange:":
			case "accessibilityLayoutPointForScreenPoint:":
			case "accessibilityLayoutSizeForScreenSize:":
			case "accessibilityLineForIndex:":
			case "accessibilityRangeForPosition:":
			case "accessibilityRangeForIndex:":
			case "accessibilityRangeForLine:":
			case "accessibilityRTFForRange:":
			case "accessibilityScreenPointForLayoutPoint:":
			case "accessibilityScreenSizeForLayoutSize:":
			case "accessibilityStringForRange:":
			case "accessibilityStyleRangeForIndex:":
			case "isAccessibilitySelectorAllowed:":
			case "accessibilityActivationPoint":
			case "setAccessibilityActivationPoint:":
			case "accessibilityAllowedValues":
			case "setAccessibilityAllowedValues:":
			case "isAccessibilityAlternateUIVisible":
			case "setAccessibilityAlternateUIVisible:":
			case "accessibilityApplicationFocusedUIElement":
			case "setAccessibilityApplicationFocusedUIElement:":
			case "accessibilityCancelButton":
			case "setAccessibilityCancelButton:":
			case "accessibilityChildren":
			case "setAccessibilityChildren:":
			case "accessibilityClearButton":
			case "setAccessibilityClearButton:":
			case "accessibilityCloseButton":
			case "setAccessibilityCloseButton:":
			case "accessibilityColumnCount":
			case "setAccessibilityColumnCount:":
			case "accessibilityColumnHeaderUIElements":
			case "setAccessibilityColumnHeaderUIElements:":
			case "accessibilityColumnIndexRange":
			case "setAccessibilityColumnIndexRange:":
			case "accessibilityColumns":
			case "setAccessibilityColumns:":
			case "accessibilityColumnTitles":
			case "setAccessibilityColumnTitles:":
			case "accessibilityContents":
			case "setAccessibilityContents:":
			case "accessibilityCriticalValue":
			case "setAccessibilityCriticalValue:":
			case "accessibilityDecrementButton":
			case "setAccessibilityDecrementButton:":
			case "accessibilityDefaultButton":
			case "setAccessibilityDefaultButton:":
			case "isAccessibilityDisclosed":
			case "setAccessibilityDisclosed:":
			case "accessibilityDisclosedByRow":
			case "setAccessibilityDisclosedByRow:":
			case "accessibilityDisclosedRows":
			case "setAccessibilityDisclosedRows:":
			case "accessibilityDisclosureLevel":
			case "setAccessibilityDisclosureLevel:":
			case "accessibilityDocument":
			case "setAccessibilityDocument:":
			case "isAccessibilityEdited":
			case "setAccessibilityEdited:":
			case "isAccessibilityElement":
			case "setAccessibilityElement:":
			case "isAccessibilityEnabled":
			case "setAccessibilityEnabled:":
			case "isAccessibilityExpanded":
			case "setAccessibilityExpanded:":
			case "accessibilityExtrasMenuBar":
			case "setAccessibilityExtrasMenuBar:":
			case "accessibilityFilename":
			case "setAccessibilityFilename:":
			case "isAccessibilityFocused":
			case "setAccessibilityFocused:":
			case "accessibilityFocusedWindow":
			case "setAccessibilityFocusedWindow:":
			case "accessibilityFrame":
			case "setAccessibilityFrame:":
			case "accessibilityFrameInParentSpace":
			case "setAccessibilityFrameInParentSpace:":
			case "isAccessibilityFrontmost":
			case "setAccessibilityFrontmost:":
			case "accessibilityFullScreenButton":
			case "setAccessibilityFullScreenButton:":
			case "accessibilityGrowArea":
			case "setAccessibilityGrowArea:":
			case "accessibilityHandles":
			case "setAccessibilityHandles:":
			case "accessibilityHeader":
			case "setAccessibilityHeader:":
			case "accessibilityHelp":
			case "setAccessibilityHelp:":
			case "isAccessibilityHidden":
			case "setAccessibilityHidden:":
			case "accessibilityHorizontalScrollBar":
			case "setAccessibilityHorizontalScrollBar:":
			case "accessibilityHorizontalUnitDescription":
			case "setAccessibilityHorizontalUnitDescription:":
			case "accessibilityHorizontalUnits":
			case "setAccessibilityHorizontalUnits:":
			case "accessibilityIdentifier":
			case "setAccessibilityIdentifier:":
			case "accessibilityIncrementButton":
			case "setAccessibilityIncrementButton:":
			case "accessibilityIndex":
			case "setAccessibilityIndex:":
			case "accessibilityInsertionPointLineNumber":
			case "setAccessibilityInsertionPointLineNumber:":
			case "accessibilityLabel":
			case "setAccessibilityLabel:":
			case "accessibilityLabelUIElements":
			case "setAccessibilityLabelUIElements:":
			case "accessibilityLabelValue":
			case "setAccessibilityLabelValue:":
			case "accessibilityLinkedUIElements":
			case "setAccessibilityLinkedUIElements:":
			case "isAccessibilityMain":
			case "setAccessibilityMain:":
			case "accessibilityMainWindow":
			case "setAccessibilityMainWindow:":
			case "accessibilityMarkerGroupUIElement":
			case "setAccessibilityMarkerGroupUIElement:":
			case "accessibilityMarkerTypeDescription":
			case "setAccessibilityMarkerTypeDescription:":
			case "accessibilityMarkerUIElements":
			case "setAccessibilityMarkerUIElements:":
			case "accessibilityMarkerValues":
			case "setAccessibilityMarkerValues:":
			case "accessibilityMaxValue":
			case "setAccessibilityMaxValue:":
			case "accessibilityMenuBar":
			case "setAccessibilityMenuBar:":
			case "accessibilityMinimizeButton":
			case "setAccessibilityMinimizeButton:":
			case "isAccessibilityMinimized":
			case "setAccessibilityMinimized:":
			case "accessibilityMinValue":
			case "setAccessibilityMinValue:":
			case "isAccessibilityModal":
			case "setAccessibilityModal:":
			case "accessibilityNextContents":
			case "setAccessibilityNextContents:":
			case "accessibilityNumberOfCharacters":
			case "setAccessibilityNumberOfCharacters:":
			case "isAccessibilityOrderedByRow":
			case "setAccessibilityOrderedByRow:":
			case "accessibilityOrientation":
			case "setAccessibilityOrientation:":
			case "accessibilityOverflowButton":
			case "setAccessibilityOverflowButton:":
			case "accessibilityParent":
			case "setAccessibilityParent:":
			case "accessibilityPlaceholderValue":
			case "setAccessibilityPlaceholderValue:":
			case "accessibilityPreviousContents":
			case "setAccessibilityPreviousContents:":
			case "isAccessibilityProtectedContent":
			case "setAccessibilityProtectedContent:":
			case "accessibilityProxy":
			case "setAccessibilityProxy:":
			case "accessibilityRole":
			case "setAccessibilityRole:":
			case "accessibilityRoleDescription":
			case "setAccessibilityRoleDescription:":
			case "accessibilityRowCount":
			case "setAccessibilityRowCount:":
			case "accessibilityRowHeaderUIElements":
			case "setAccessibilityRowHeaderUIElements:":
			case "accessibilityRowIndexRange":
			case "setAccessibilityRowIndexRange:":
			case "accessibilityRows":
			case "setAccessibilityRows:":
			case "accessibilityRulerMarkerType":
			case "setAccessibilityRulerMarkerType:":
			case "accessibilitySearchButton":
			case "setAccessibilitySearchButton:":
			case "accessibilitySearchMenu":
			case "setAccessibilitySearchMenu:":
			case "isAccessibilitySelected":
			case "setAccessibilitySelected:":
			case "accessibilitySelectedCells":
			case "setAccessibilitySelectedCells:":
			case "accessibilitySelectedChildren":
			case "setAccessibilitySelectedChildren:":
			case "accessibilitySelectedColumns":
			case "setAccessibilitySelectedColumns:":
			case "accessibilitySelectedRows":
			case "setAccessibilitySelectedRows:":
			case "accessibilitySelectedText":
			case "setAccessibilitySelectedText:":
			case "accessibilitySelectedTextRange":
			case "setAccessibilitySelectedTextRange:":
			case "accessibilitySelectedTextRanges":
			case "setAccessibilitySelectedTextRanges:":
			case "accessibilityServesAsTitleForUIElements":
			case "setAccessibilityServesAsTitleForUIElements:":
			case "accessibilitySharedCharacterRange":
			case "setAccessibilitySharedCharacterRange:":
			case "accessibilitySharedFocusElements":
			case "setAccessibilitySharedFocusElements:":
			case "accessibilitySharedTextUIElements":
			case "setAccessibilitySharedTextUIElements:":
			case "accessibilityShownMenu":
			case "setAccessibilityShownMenu:":
			case "accessibilitySortDirection":
			case "setAccessibilitySortDirection:":
			case "accessibilitySplitters":
			case "setAccessibilitySplitters:":
			case "accessibilitySubrole":
			case "setAccessibilitySubrole:":
			case "accessibilityTabs":
			case "setAccessibilityTabs:":
			case "accessibilityTitle":
			case "setAccessibilityTitle:":
			case "accessibilityTitleUIElement":
			case "setAccessibilityTitleUIElement:":
			case "accessibilityToolbarButton":
			case "setAccessibilityToolbarButton:":
			case "accessibilityTopLevelUIElement":
			case "setAccessibilityTopLevelUIElement:":
			case "accessibilityUnitDescription":
			case "setAccessibilityUnitDescription:":
			case "accessibilityUnits":
			case "setAccessibilityUnits:":
			case "accessibilityURL":
			case "setAccessibilityURL:":
			case "accessibilityValue":
			case "setAccessibilityValue:":
			case "accessibilityValueDescription":
			case "setAccessibilityValueDescription:":
			case "accessibilityVerticalScrollBar":
			case "setAccessibilityVerticalScrollBar:":
			case "accessibilityVerticalUnitDescription":
			case "setAccessibilityVerticalUnitDescription:":
			case "accessibilityVerticalUnits":
			case "setAccessibilityVerticalUnits:":
			case "accessibilityVisibleCells":
			case "setAccessibilityVisibleCells:":
			case "accessibilityVisibleCharacterRange":
			case "setAccessibilityVisibleCharacterRange:":
			case "accessibilityVisibleChildren":
			case "setAccessibilityVisibleChildren:":
			case "accessibilityVisibleColumns":
			case "setAccessibilityVisibleColumns:":
			case "accessibilityVisibleRows":
			case "setAccessibilityVisibleRows:":
			case "accessibilityWarningValue":
			case "setAccessibilityWarningValue:":
			case "accessibilityWindow":
			case "setAccessibilityWindow:":
			case "accessibilityWindows":
			case "setAccessibilityWindows:":
			case "accessibilityZoomButton":
			case "setAccessibilityZoomButton:":
			case "accessibilityElementWithRole:frame:label:parent:":
				switch (type.Name) {
				case "NSMenu":
				case "NSMenuItem":
					if (!Mac.CheckSystemVersion (10, 12))
						return true;
					break;
				}

				break;
			}

			return false;
		}

		static List<NSObject> do_not_dispose = new List<NSObject> ();

		protected override void Dispose (NSObject obj, Type type)
		{
			switch (type.FullName) {
			// FIXME: those crash the application when Dispose is called
			case "MonoMac.AVFoundation.AVAudioRecorder":
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
				do_not_dispose.Add (obj);
				break;
			default:
				base.Dispose (obj, type);
				break;
			}
		}

		protected override bool CheckResponse (bool value, Type actualType, MethodBase method, ref string name)
		{
			var declaredType = method.DeclaringType;

			switch (name) {
			// NSDraggingDestination protocol
			case "concludeDragOperation:":		// e.g. NSTokenField
			case "draggingEnded:":			// e.g. NSMatrix
			case "draggingExited:":			// e.g. NSTokenField
			case "draggingUpdated:":		// e.g. NSTokenField
			case "performDragOperation:":		// e.g. NSTokenField
			case "prepareForDragOperation:":	// e.g. NSTokenField
			case "wantsPeriodicDraggingUpdates":	// e.g. NSBrowser - optional, [DefaultValue(true)] if it does not exists
				return true;
			// NSDraggingSource
			case "draggedImage:beganAt:":				// e.g. NSTextView
			case "draggedImage:endedAt:deposited:":			// e.g. NSTableView
			case "draggedImage:movedTo:":				// e.g. NSCollectionView
			case "ignoreModifierKeysWhileDragging":			// e.g. NSTextView
			case "namesOfPromisedFilesDroppedAtDestination:":	// e.g. NSTextView
				return true;
			// NSAnimatablePropertyContainer
			case "animationForKey:":		// e.g. NSViewAnimation
			case "animator":
			case "animations":
			case "setAnimations:":
				return true;
			// NSTypeSetter - Layout Phase Interface (needs to be overridden, not really part of the provided types)
			case "willSetLineFragmentRect:forGlyphRange:usedRect:baselineOffset:":
			case "shouldBreakLineByWordBeforeCharacterAtIndex:":
			case "shouldBreakLineByHyphenatingBeforeCharacterAtIndex:":
			case "hyphenationFactorForGlyphAtIndex:":
			case "hyphenCharacterForGlyphAtIndex:":
			case "boundingBoxForControlGlyphAtIndex:forTextContainer:proposedLineFragment:glyphPosition:characterIndex:":
				return true;
			// in NSView documentation but defined in NSClipView - no answers to call 
			case "scrollClipView:toPoint:":
				return true;
			// IKFilterBrowserPanel ??? not clear why
			case "finish:":
				return true;
			// IKFilterUIView
			case "viewForUIConfiguration:excludedKeys:":	// [Target] on CIFilter
				return true;
			// NSDictionaryEnumerator
			case "fileModificationDate":
			case "fileType":
			case "filePosixPermissions":
			case "fileOwnerAccountName":
			case "fileGroupOwnerAccountName":
			case "fileSystemNumber":
			case "fileSystemFileNumber":
			case "fileExtensionHidden":
			case "fileHFSCreatorCode":
			case "fileHFSTypeCode":
			case "fileIsImmutable":
			case "fileIsAppendOnly":
			case "fileCreationDate":
			case "fileOwnerAccountID":
			case "fileGroupOwnerAccountID":
				return true;				// all [Target] on NSDictionary
			// SBObject
			case "get":
				return true;

			// NSCoder - documented as available in 10.8
			case "allowedClasses":
			case "requiresSecureCoding":
				return true;

			// NSFileManager
			case "ubiquityIdentityToken":		// documented in 10.8
				return true;
			// NS[Mutable]UrlRequest
			case "allowsCellularAccess":		// documented in 10.8
			case "setAllowsCellularAccess:":	// documented in 10.8
				return true;
			// NSString
			case "capitalizedStringWithLocale:":	// documented in 10.8
			case "lowercaseStringWithLocale:":	// documented in 10.8
			case "uppercaseStringWithLocale:":	// documented in 10.8
				return true;
			// AVVideoComposition
			case "isValidForAsset:timeRange:validationDelegate:":	// documented in 10.8
				return true;
			// AVPlayer
			case "setRate:time:atHostTime:":                // 10.8+
			case "prerollAtRate:completionHandler:":        // 10.8+
			case "cancelPendingPrerolls":                   // 10.8+
			case "masterClock":                             // 10.8+
			case "setMasterClock:":				// 10.8+
			// NSDateComponents
			case "isLeapMonth":				// 10.8+
			case "setLeapMonth:":				// 10.8+
			// NSFileCoordinator
			case "itemAtURL:willMoveToURL:":		// 10.8+
				return true; // documented (but does not respond)
			
			// MonoMac.CoreImage.CIDetector (10.8+)
			case "featuresInImage:options:":
			// MonoMac.CoreImage.CIFaceFeature (10.8+)
			case "hasTrackingID":
			case "trackingID":
			case "hasTrackingFrameCount":
			case "trackingFrameCount":
			// MonoMac.CoreImage.CIImage : only in 10.8+
			case "properties": // only documented in header files
			case "autoAdjustmentFilters":
			case "autoAdjustmentFiltersWithOptions:":
			// MonoMac.AVFoundation.AVAssetExportSession (10.8+)
			case "asset":
			// MonoMac.AVFoundation.AVAssetReaderOutput
			case "alwaysCopiesSampleData":
			case "setAlwaysCopiesSampleData:":
			// MonoMac.AVFoundation.AVAssetTrack
			case "isPlayable":
			// MonoMac.AVFoundation.AVAudioPlayer (10.8+)
			case "enableRate":
			case "setEnableRate:":
			case "rate":
			case "setRate:":
			// MonoMac.AVFoundation.AVMutableCompositionTrack
			case "insertTimeRanges:ofTracks:atTime:error:":
			// MonoMac.AVFoundation.AVPlayerItem
			case "addOutput:":
			case "removeOutput:":
			case "canPlayFastReverse":
			case "canPlayFastForward":
			case "canPlaySlowForward":
			case "canPlayReverse":
			case "canPlaySlowReverse":
			case "canStepForward":
			case "canStepBackward":
			case "outputs":
			case "timebase":
			// MonoMac.CoreAnimation.CAAnimation : only on OSX, added for SceneKit
			case "usesSceneTimeBase":
			case "setUsesSceneTimeBase:":
				if (!Mac.CheckSystemVersion (10, 8))
					return true;
				break;
			case "initWithString:":
				if (declaredType.Name == "NSTextStorage")
					return true;
				break;
			}

			switch (declaredType.Name) {
			case "NSUrlSession":
			case "Foundation.NSUrlSession":
				switch (name) {
				case "delegateQueue":
				case "sessionDescription":
				case "setSessionDescription:":
				case "delegate":
					if (!Mac.CheckSystemVersion (10, 11))
						return true;
					break;
				}
				break;
			case "NSUrlSessionTask":
			case "Foundation.NSUrlSessionTask":
				switch (name) {
				case "countOfBytesExpectedToReceive":
				case "countOfBytesExpectedToSend":
				case "countOfBytesReceived":
				case "countOfBytesSent":
				case "currentRequest":
				case "error":
				case "originalRequest":
				case "response":
				case "state":
				case "taskDescription":
				case "setTaskDescription:":
				case "taskIdentifier":
				case "priority":
				case "setPriority:":
					if (!Mac.CheckSystemVersion (10, 11))
						return true;
					break;
				}
				break;
			case "Foundation.NSUrlSessionStreamTask":
			case "NSUrlSessionStreamTask":
				switch (name) {
					case "captureStreams":
					case "closeRead":
					case "closeWrite":
					case "readDataOfMinLength:maxLength:timeout:completionHandler:":
					case "startSecureConnection":
					case "stopSecureConnection":
					case "writeData:timeout:completionHandler:":
						if (!Mac.CheckSystemVersion (10, 11))
							return true;
						break;
				}
				break;
			}

//			Console.WriteLine ("{0} {1}", declaredType, name);
			return base.CheckResponse (value, actualType, method, ref name);
		}

		protected override bool CheckStaticResponse (bool value, Type actualType, Type declaredType, ref string name)
		{
			switch (name) {
			// 10.7 exceptions

			// NSAnimatablePropertyContainer protocol (10.7) -> NSViewAnimation
			case "defaultAnimationForKey:":
				return true;

			// 10.8 exceptions

			// GKPlayer - documented as available in 10.8
			case "loadPlayersForIdentifiers:withCompletionHandler:":
			// SLRequest - documented as available in 10.8
			case "requestForServiceType:requestMethod:URL:parameters:":
			// NSDictionary - documented as available in 10.8
			case "dictionaryWithSharedKeySet:":
			case "sharedKeySetForKeys:":
			// AVMetadataItem - documented as available in 10.8
			case "metadataItemsFromArray:filteredAndSortedAccordingToPreferredLanguages:":
				return true;
			}
			return base.CheckStaticResponse (value, actualType, declaredType, ref name);
		}

		protected override bool SkipInit (string selector, MethodBase m)
		{
			switch (selector) {
#if !XAMCORE_3_0
			// DomEvent
			case "initEvent:canBubbleArg:cancelableArg:":
			// DomOverflowEvent
			case "initOverflowEvent:horizontalOverflow:verticalOverflow:":
			// DomUIEvent
			case "initUIEvent:canBubble:cancelable:view:detail:":
			// DomKeyboardEvent
			case "initKeyboardEvent:canBubble:cancelable:view:keyIdentifier:keyLocation:ctrlKey:altKey:shiftKey:metaKey:altGraphKey:":
			case "initKeyboardEvent:canBubble:cancelable:view:keyIdentifier:keyLocation:ctrlKey:altKey:shiftKey:metaKey:":
			// DomMouseEvent
			case "initMouseEvent:canBubble:cancelable:view:detail:screenX:screenY:clientX:clientY:ctrlKey:altKey:shiftKey:metaKey:button:relatedTarget:":
			// DomWheelEvent
			case "initWheelEvent:wheelDeltaY:view:screenX:screenY:clientX:clientY:ctrlKey:altKey:shiftKey:metaKey:":
			// QTMovie
			case "movieWithTimeRange:error:":
			case "initWithQuickTimeMedia:error:":
			// NSAppleEventDescriptor
			case "initListDescriptor":
			case "initRecordDescriptor":
			// NSAnimation
			case "initWithDuration:animationCurve:":
				return true;
#endif
			// NSImage
			case "initWithDataIgnoringOrientation:":
				var mi = m as MethodInfo;
				return mi != null && !mi.IsPublic && mi.ReturnType.Name == "IntPtr";
			default:
				return base.SkipInit (selector, m);
			}
		}
	}
}
